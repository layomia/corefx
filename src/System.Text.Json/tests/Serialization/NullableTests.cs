﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.Immutable;
using System.Linq;
using System;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class NullableTests
    {
        [Fact]
        public static void DictionaryWithNullableValue()
        {
            Dictionary<string, float?> dictWithFloatValue = new Dictionary<string, float?> { { "key", 42.0f } };
            Dictionary<string, float?> dictWithFloatNull = new Dictionary<string, float?> { { "key", null } };
            TestDictionaryWithNullableValue<Dictionary<string, float?>, Dictionary<string, Dictionary<string, float?>>, float?>(
                dictWithFloatValue,
                dictWithFloatNull,
                dictOfDictWithValue: new Dictionary<string, Dictionary<string, float?>> { { "key", dictWithFloatValue } },
                dictOfDictWithNull: new Dictionary<string, Dictionary<string, float?>> { { "key", dictWithFloatNull } },
                42.0f);

            DateTime now = DateTime.Now;
            Dictionary<string, DateTime?> dictWithDateTimeValue = new Dictionary<string, DateTime?> { { "key", now } };
            Dictionary<string, DateTime?> dictWithDateTimeNull = new Dictionary<string, DateTime?> { { "key", null } };
            TestDictionaryWithNullableValue<Dictionary<string, DateTime?>, Dictionary<string, Dictionary<string, DateTime?>>, DateTime?>(
                dictWithDateTimeValue,
                dictWithDateTimeNull,
                dictOfDictWithValue: new Dictionary<string, Dictionary<string, DateTime?>> { { "key", dictWithDateTimeValue } },
                dictOfDictWithNull: new Dictionary<string, Dictionary<string, DateTime?>> { { "key", dictWithDateTimeNull } },
                now);

            IDictionary<string, DateTime?> idictWithDateTimeValue = new Dictionary<string, DateTime?> { { "key", now } };
            IDictionary<string, DateTime?> idictWithDateTimeNull = new Dictionary<string, DateTime?> { { "key", null } };
            TestDictionaryWithNullableValue<IDictionary<string, DateTime?>, IDictionary<string, IDictionary<string, DateTime?>>, DateTime?>(
                idictWithDateTimeValue,
                idictWithDateTimeNull,
                dictOfDictWithValue: new Dictionary<string, IDictionary<string, DateTime?>> { { "key", idictWithDateTimeValue } },
                dictOfDictWithNull: new Dictionary<string, IDictionary<string, DateTime?>> { { "key", idictWithDateTimeNull } },
                now);

            ImmutableDictionary<string, DateTime?> immutableDictWithDateTimeValue = ImmutableDictionary.CreateRange(new Dictionary<string, DateTime?> { { "key", now } });
            ImmutableDictionary<string, DateTime?> immutableDictWithDateTimeNull = ImmutableDictionary.CreateRange(new Dictionary<string, DateTime?> { { "key", null } });
            TestDictionaryWithNullableValue<ImmutableDictionary<string, DateTime?>, ImmutableDictionary<string, ImmutableDictionary<string, DateTime?>>, DateTime?>(
                immutableDictWithDateTimeValue,
                immutableDictWithDateTimeNull,
                dictOfDictWithValue: ImmutableDictionary.CreateRange(new Dictionary<string, ImmutableDictionary<string, DateTime?>> { { "key", immutableDictWithDateTimeValue } }),
                dictOfDictWithNull: ImmutableDictionary.CreateRange(new Dictionary<string, ImmutableDictionary<string, DateTime?>> { { "key", immutableDictWithDateTimeNull } }),
                now);
        }

        public class MyOverflowWrapper
        {
            [JsonExtensionData]
            public Dictionary<string, JsonElement?> MyOverflow { get; set; }
        }

        public class AnotherOverflowWrapper
        {
            public MyOverflowWrapper Wrapper { get; set; }
        }

        [Fact]
        public static void ExtensionDataWithNullableJsonElement_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => JsonSerializer.Deserialize<MyOverflowWrapper>(@"{""key"":""value""}"));
            Assert.Throws<InvalidOperationException>(() => JsonSerializer.Deserialize<AnotherOverflowWrapper>(@"{""Wrapper"": {""key"":""value""}}"));
        }

        private static void TestDictionaryWithNullableValue<TDict, TDictOfDict, TValue>(
            TDict dictWithValue,
            TDict dictWithNull,
            TDictOfDict dictOfDictWithValue,
            TDictOfDict dictOfDictWithNull,
            TValue value)
        {
            string valueSerialized = JsonSerializer.Serialize(value);

            static void ValidateDict(TDict dict, TValue expectedValue)
            {
                IDictionary<string, TValue> genericIDict = (IDictionary<string, TValue>)dict;
                Assert.Equal(1, genericIDict.Count);
                Assert.Equal(expectedValue, genericIDict["key"]);
            }

            static void ValidateDictOfDict(TDictOfDict dictOfDict, TValue expectedValue)
            {
                IDictionary<string, TDict> genericIDict = (IDictionary<string, TDict>)dictOfDict;
                Assert.Equal(1, genericIDict.Count);

                IDictionary<string, TValue> nestedDict = (IDictionary<string, TValue>)genericIDict["key"];
                Assert.Equal(1, nestedDict.Count);
                Assert.Equal(expectedValue, nestedDict["key"]);
            }

            string json = JsonSerializer.Serialize(dictWithValue);
            Assert.Equal(@"{""key"":" + valueSerialized + "}", json);

            TDict parsedDictWithValue = JsonSerializer.Deserialize<TDict>(json);
            ValidateDict(parsedDictWithValue, value);

            json = JsonSerializer.Serialize(dictWithNull);
            Assert.Equal(@"{""key"":null}", json);

            TDict parsedDictWithNull = JsonSerializer.Deserialize<TDict>(json);
            ValidateDict(parsedDictWithNull, default);

            // Test nested dicts with nullable values.
            json = JsonSerializer.Serialize(dictOfDictWithValue);
            Assert.Equal(@"{""key"":{""key"":" + valueSerialized + "}}", json);

            TDictOfDict parsedDictOfDictWithValue = JsonSerializer.Deserialize<TDictOfDict>(json);
            ValidateDictOfDict(parsedDictOfDictWithValue, value);

            json = JsonSerializer.Serialize(dictOfDictWithNull);
            Assert.Equal(@"{""key"":{""key"":null}}", json);

            TDictOfDict parsedDictOfDictWithNull = JsonSerializer.Deserialize<TDictOfDict>(json);
            ValidateDictOfDict(parsedDictOfDictWithNull, default);
        }

        [Fact]
        public static void EnumerableWithNullableValue()
        {
            IEnumerable<float?> ieWithFloatValue = new List<float?> { 42.0f };
            IEnumerable<float?> ieWithFloatNull = new List<float?> { null };
            TestEnumerableWithNullableValue<IEnumerable<float?>, IEnumerable<IEnumerable<float?>>, float?>(
                ieWithFloatValue,
                ieWithFloatNull,
                enumerableOfEnumerableWithValue: new List<IEnumerable<float?>> { ieWithFloatValue },
                enumerableOfEnumerableWithNull: new List<IEnumerable<float?>> { ieWithFloatNull },
                42.0f);

            DateTime now = DateTime.Now;
            IEnumerable<DateTime?> ieWithDateTimeValue = new List<DateTime?> { now };
            IEnumerable<DateTime?> ieWithDateTimeNull = new List<DateTime?> { null };
            TestEnumerableWithNullableValue<IEnumerable<DateTime?>, IEnumerable<IEnumerable<DateTime?>>, DateTime?>(
                ieWithDateTimeValue,
                ieWithDateTimeNull,
                enumerableOfEnumerableWithValue: new List<IEnumerable<DateTime?>> { ieWithDateTimeValue },
                enumerableOfEnumerableWithNull: new List<IEnumerable<DateTime?>> { ieWithDateTimeNull },
                now);

            IReadOnlyList<DateTime?> irlWithDateTimeValue = new List<DateTime?> { now };
            IReadOnlyList<DateTime?> irlWithDateTimeNull = new List<DateTime?> { null };
            TestEnumerableWithNullableValue<IReadOnlyList<DateTime?>, IReadOnlyList<IReadOnlyList<DateTime?>>, DateTime?>(
                irlWithDateTimeValue,
                irlWithDateTimeNull,
                enumerableOfEnumerableWithValue: new List<IReadOnlyList<DateTime?>> { irlWithDateTimeValue },
                enumerableOfEnumerableWithNull: new List<IReadOnlyList<DateTime?>> { irlWithDateTimeNull },
                now);

            Stack<DateTime?> stWithDateTimeValue = new Stack<DateTime?>();
            stWithDateTimeValue.Push(now);

            Stack<DateTime?> stWithDateTimeNull = new Stack<DateTime?>();
            stWithDateTimeNull.Push(null);

            Stack<Stack<DateTime?>> enumerableOfEnumerableWithValue = new Stack<Stack<DateTime?>>();
            enumerableOfEnumerableWithValue.Push(stWithDateTimeValue);

            Stack<Stack<DateTime?>> enumerableOfEnumerableWithNull = new Stack<Stack<DateTime?>>();
            enumerableOfEnumerableWithNull.Push(stWithDateTimeNull);

            TestEnumerableWithNullableValue<Stack<DateTime?>, Stack<Stack<DateTime?>>, DateTime?>(
                stWithDateTimeValue,
                stWithDateTimeNull,
                enumerableOfEnumerableWithValue,
                enumerableOfEnumerableWithNull,
                now);

            IImmutableList<DateTime?> imlWithDateTimeValue = ImmutableList.CreateRange(new List<DateTime?> { now });
            IImmutableList<DateTime?> imlWithDateTimeNull = ImmutableList.CreateRange(new List<DateTime?> { null });
            TestEnumerableWithNullableValue<IImmutableList<DateTime?>, IImmutableList<IImmutableList<DateTime?>>, DateTime?>(
                imlWithDateTimeValue,
                imlWithDateTimeNull,
                enumerableOfEnumerableWithValue: ImmutableList.CreateRange(new List<IImmutableList<DateTime?>> { imlWithDateTimeValue }),
                enumerableOfEnumerableWithNull: ImmutableList.CreateRange(new List<IImmutableList<DateTime?>> { imlWithDateTimeNull }),
                now);
        }

        private static void TestEnumerableWithNullableValue<TEnumerable, TEnumerableOfEnumerable, TValue>(
            TEnumerable enumerableWithValue,
            TEnumerable enumerableWithNull,
            TEnumerableOfEnumerable enumerableOfEnumerableWithValue,
            TEnumerableOfEnumerable enumerableOfEnumerableWithNull,
            TValue value)
        {
            string valueSerialized = JsonSerializer.Serialize(value);

            static void ValidateEnumerable(TEnumerable enumerable, TValue expectedValue)
            {
                IEnumerable<TValue> ienumerable = (IEnumerable<TValue>)enumerable;
                int count = 0;
                foreach (TValue val in ienumerable)
                {
                    Assert.Equal(expectedValue, val);
                    count += 1;
                }
                Assert.Equal(1, count);
            }

            static void ValidateEnumerableOfEnumerable(TEnumerableOfEnumerable dictOfDict, TValue expectedValue)
            {
                IEnumerable<TEnumerable> ienumerable = (IEnumerable<TEnumerable>)dictOfDict;
                int ienumerableCount = 0;
                int nestedIEnumerableCount = 0;

                foreach (IEnumerable<TValue> nestedIEnumerable in ienumerable)
                {
                    foreach (TValue val in nestedIEnumerable)
                    {
                        Assert.Equal(expectedValue, val);
                        nestedIEnumerableCount += 1;
                    }
                    ienumerableCount += 1;
                }
                Assert.Equal(1, ienumerableCount);
                Assert.Equal(1, nestedIEnumerableCount);
            }

            string json = JsonSerializer.Serialize(enumerableWithValue);
            Assert.Equal($"[{valueSerialized}]", json);

            TEnumerable parsedEnumerableWithValue = JsonSerializer.Deserialize<TEnumerable>(json);
            ValidateEnumerable(parsedEnumerableWithValue, value);

            json = JsonSerializer.Serialize(enumerableWithNull);
            Assert.Equal("[null]", json);

            TEnumerable parsedEnumerableWithNull = JsonSerializer.Deserialize<TEnumerable>(json);
            ValidateEnumerable(parsedEnumerableWithNull, default);

            // Test nested dicts with nullable values.
            json = JsonSerializer.Serialize(enumerableOfEnumerableWithValue);
            Assert.Equal($"[[{valueSerialized}]]", json);

            TEnumerableOfEnumerable parsedEnumerableOfEnumerableWithValue = JsonSerializer.Deserialize<TEnumerableOfEnumerable>(json);
            ValidateEnumerableOfEnumerable(parsedEnumerableOfEnumerableWithValue, value);

            json = JsonSerializer.Serialize(enumerableOfEnumerableWithNull);
            Assert.Equal("[[null]]", json);

            TEnumerableOfEnumerable parsedEnumerableOfEnumerableWithNull = JsonSerializer.Deserialize<TEnumerableOfEnumerable>(json);
            ValidateEnumerableOfEnumerable(parsedEnumerableOfEnumerableWithNull, default);
        }
    }
}
