
using System.Collections.Generic;
using System.Text.Json.Serialization.Tests;
using Xunit;

namespace System.Text.Json.Tests.RealWorld
{
    public class RealWorldTest : ITestClass
    {
        public virtual void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Verify()
        {
            VerifySerializer();
            VerifyDocument();
            VerifyReader();
            VerifyWriter();
        }

        public string PayloadAsString = null;

        public byte[] PayloadAsUtf8Bytes => Encoding.UTF8.GetBytes(PayloadAsString);

        public virtual void VerifySerializer()
        {
            throw new NotImplementedException();
        }

        public virtual void VerifyDocument()
        {
            throw new NotImplementedException();
        }

        public virtual void VerifyReader()
        {
            throw new NotImplementedException();
        }

        public virtual void VerifyWriter()
        {
            throw new NotImplementedException();
        }
    }

    public static class RealWorldTests
    {
        public static IEnumerable<object[]> TestData
        {
            get
            {
                yield return new object[] { new BlogPostTest() };
            }
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public static void TestRealWorldUseCases(RealWorldTest test)
        {
            test.Initialize();
            test.Verify();
        }
    }
}
