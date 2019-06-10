﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Text.Json.Serialization.Policies;

namespace System.Text.Json.Serialization.Converters
{
    internal sealed class DefaultICollectionConverter : JsonEnumerableConverter
    {
        public override IEnumerable CreateFromList(ref ReadStack state, IList sourceList, JsonSerializerOptions options)
        {
            Type enumerableType = state.Current.JsonPropertyInfo.RuntimePropertyType;
            return (IEnumerable)Activator.CreateInstance(enumerableType, sourceList);
        }

        internal IDictionary CreateFromDictionary(ref ReadStack state, IDictionary sourceDictionary, JsonSerializerOptions options)
        {
            Type enumerableType = state.Current.JsonPropertyInfo.RuntimePropertyType;
            return (IDictionary)Activator.CreateInstance(enumerableType, sourceDictionary);
        }
    }
}
