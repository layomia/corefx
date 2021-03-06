// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.DotNet.RemoteExecutor;
using Xunit;

namespace System.Threading.Tests
{
    public class MutexTests : FileCleanupTestBase
    {
        [Fact]
        public void Ctor_ConstructWaitRelease()
        {
            using (Mutex m = new Mutex())
            {
                m.CheckedWait();
                m.ReleaseMutex();
            }

            using (Mutex m = new Mutex(false))
            {
                m.CheckedWait();
                m.ReleaseMutex();
            }

            using (Mutex m = new Mutex(true))
            {
                m.CheckedWait();
                m.ReleaseMutex();
                m.ReleaseMutex();
            }
        }

        [Fact]
        [PlatformSpecific(TestPlatforms.AnyUnix)]
        public void Ctor_InvalidNames_Unix()
        {
            AssertExtensions.Throws<ArgumentException>("name", null, () => new Mutex(false, new string('a', 1000), out bool createdNew));
        }

        [Theory]
        [MemberData(nameof(GetValidNames))]
        public void Ctor_ValidName(string name)
        {
            bool createdNew;
            using (Mutex m1 = new Mutex(false, name, out createdNew))
            {
                Assert.True(createdNew);
                using (Mutex m2 = new Mutex(false, name, out createdNew))
                {
                    Assert.False(createdNew);
                }
            }
        }

        [PlatformSpecific(TestPlatforms.Windows)]  // named semaphores aren't supported on Unix
        [Fact]
        public void Ctor_NameUsedByOtherSynchronizationPrimitive_Windows()
        {
            string name = Guid.NewGuid().ToString("N");
            using (Semaphore s = new Semaphore(1, 1, name))
            {
                Assert.Throws<WaitHandleCannotBeOpenedException>(() => new Mutex(false, name));
            }
        }

        [PlatformSpecific(TestPlatforms.Windows)]
        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotInAppContainer))] // Can't create global objects in appcontainer
        public void Ctor_ImpersonateAnonymousAndTryCreateGlobalMutexTest()
        {
            ThreadTestHelpers.RunTestInBackgroundThread(() =>
            {
                if (!ImpersonateAnonymousToken(GetCurrentThread()))
                {
                    // Impersonation is not allowed in the current context, this test is inappropriate in such a case
                    return;
                }

                Assert.Throws<UnauthorizedAccessException>(() => new Mutex(false, "Global\\" + Guid.NewGuid().ToString("N")));
                Assert.True(RevertToSelf());
            });
        }

        [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsInAppContainer))] // Can't create global objects in appcontainer
        [PlatformSpecific(TestPlatforms.Windows)]
        public void Ctor_TryCreateGlobalMutexTest_Uwp()
        {
            ThreadTestHelpers.RunTestInBackgroundThread(() =>
                Assert.Throws<UnauthorizedAccessException>(() => new Mutex(false, "Global\\" + Guid.NewGuid().ToString("N"))));
        }

        [Theory]
        [MemberData(nameof(GetValidNames))]
        public void OpenExisting(string name)
        {
            Mutex resultHandle;
            Assert.False(Mutex.TryOpenExisting(name, out resultHandle));

            using (Mutex m1 = new Mutex(false, name))
            {
                using (Mutex m2 = Mutex.OpenExisting(name))
                {
                    m1.CheckedWait();
                    Assert.False(Task.Factory.StartNew(() => m2.WaitOne(0), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).Result);
                    m1.ReleaseMutex();

                    m2.CheckedWait();
                    Assert.False(Task.Factory.StartNew(() => m1.WaitOne(0), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).Result);
                    m2.ReleaseMutex();
                }

                Assert.True(Mutex.TryOpenExisting(name, out resultHandle));
                Assert.NotNull(resultHandle);
                resultHandle.Dispose();
            }
        }

        [Fact]
        public void OpenExisting_InvalidNames()
        {
            AssertExtensions.Throws<ArgumentNullException>("name", () => Mutex.OpenExisting(null));
            AssertExtensions.Throws<ArgumentException>("name", null, () => Mutex.OpenExisting(string.Empty));
        }

        [Fact]
        public void OpenExisting_UnavailableName()
        {
            string name = Guid.NewGuid().ToString("N");
            Assert.Throws<WaitHandleCannotBeOpenedException>(() => Mutex.OpenExisting(name));
            Mutex ignored;
            Assert.False(Mutex.TryOpenExisting(name, out ignored));
        }

        [PlatformSpecific(TestPlatforms.Windows)]  // named semaphores aren't supported on Unix
        [Fact]
        public void OpenExisting_NameUsedByOtherSynchronizationPrimitive_Windows()
        {
            string name = Guid.NewGuid().ToString("N");
            using (Semaphore sema = new Semaphore(1, 1, name))
            {
                Assert.Throws<WaitHandleCannotBeOpenedException>(() => Mutex.OpenExisting(name));
                Mutex ignored;
                Assert.False(Mutex.TryOpenExisting(name, out ignored));
            }
        }

        private static IEnumerable<string> GetNamePrefixes()
        {
            yield return string.Empty;
            yield return "Local\\";

            // Creating global sync objects is not allowed in UWP apps
            if (!PlatformDetection.IsUap)
            {
                yield return "Global\\";
            }
        }

        public static IEnumerable<object[]> AbandonExisting_MemberData()
        {
            var nameGuidStr = Guid.NewGuid().ToString("N");
            for (int waitType = 0; waitType < 2; ++waitType) // 0 == WaitOne, 1 == WaitAny
            {
                yield return new object[] { null, waitType };
                foreach (var namePrefix in GetNamePrefixes())
                {
                    yield return new object[] { namePrefix + nameGuidStr, waitType };
                }
            }
        }

        [Theory]
        [MemberData(nameof(AbandonExisting_MemberData))]
        public void AbandonExisting(string name, int waitType)
        {
            ThreadTestHelpers.RunTestInBackgroundThread(() =>
            {
                using (var m = new Mutex(false, name))
                {
                    Task t = Task.Factory.StartNew(() =>
                    {
                        m.CheckedWait();
                        // don't release the mutex; abandon it on this thread
                    }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                    t.CheckedWait();

                    switch (waitType)
                    {
                        case 0: // WaitOne
                            Assert.Throws<AbandonedMutexException>(() => m.CheckedWait());
                            break;

                        case 1: // WaitAny
                            AbandonedMutexException ame =
                                Assert.Throws<AbandonedMutexException>(
                                    () => WaitHandle.WaitAny(new[] { m }, ThreadTestHelpers.UnexpectedTimeoutMilliseconds));
                            Assert.Equal(0, ame.MutexIndex);
                            Assert.Equal(m, ame.Mutex);
                            break;
                    }
                }
            });
        }

        public static IEnumerable<object[]> CrossProcess_NamedMutex_ProtectedFileAccessAtomic_MemberData()
        {
            var nameGuidStr = Guid.NewGuid().ToString("N");
            foreach (var namePrefix in GetNamePrefixes())
            {
                yield return new object[] { namePrefix + nameGuidStr };
            }
        }

        [Theory]
        [MemberData(nameof(CrossProcess_NamedMutex_ProtectedFileAccessAtomic_MemberData))]
        public void CrossProcess_NamedMutex_ProtectedFileAccessAtomic(string prefix)
        {
            ThreadTestHelpers.RunTestInBackgroundThread(() =>
            {
                string mutexName = prefix + Guid.NewGuid().ToString("N");
                string fileName = GetTestFilePath();

                Func<string, string, int> otherProcess = (m, f) =>
                {
                    using (var mutex = Mutex.OpenExisting(m))
                    {
                        mutex.CheckedWait();
                        try
                        { File.WriteAllText(f, "0"); }
                        finally { mutex.ReleaseMutex(); }

                        IncrementValueInFileNTimes(mutex, f, 10);
                    }
                    return RemoteExecutor.SuccessExitCode;
                };

                using (var mutex = new Mutex(false, mutexName))
                using (var remote = RemoteExecutor.Invoke(otherProcess, mutexName, fileName))
                {
                    SpinWait.SpinUntil(() => File.Exists(fileName), ThreadTestHelpers.UnexpectedTimeoutMilliseconds);

                    IncrementValueInFileNTimes(mutex, fileName, 10);
                }

                Assert.Equal(20, int.Parse(File.ReadAllText(fileName)));
            });
        }

        private static void IncrementValueInFileNTimes(Mutex mutex, string fileName, int n)
        {
            for (int i = 0; i < n; i++)
            {
                mutex.CheckedWait();
                try
                {
                    int current = int.Parse(File.ReadAllText(fileName));
                    Thread.Sleep(10);
                    File.WriteAllText(fileName, (current + 1).ToString());
                }
                finally { mutex.ReleaseMutex(); }
            }
        }

        public static TheoryData<string> GetValidNames()
        {
            var names  =  new TheoryData<string>() { Guid.NewGuid().ToString("N") };

            if (PlatformDetection.IsWindows)
                names.Add(Guid.NewGuid().ToString("N") + new string('a', 1000));

            return names;
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentThread();

        [DllImport("advapi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ImpersonateAnonymousToken(IntPtr threadHandle);

        [DllImport("advapi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RevertToSelf();
    }
}
