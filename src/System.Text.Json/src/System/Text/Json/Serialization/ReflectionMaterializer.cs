﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization.Converters;

namespace System.Text.Json.Serialization
{
    internal sealed class ReflectionMaterializer : ClassMaterializer
    {
        public override JsonClassInfo.ConstructorDelegate CreateConstructor(Type type)
        {
            return () => Activator.CreateInstance(type);
        }

        public override object ImmutableCreateRange(Type constructingType, Type elementType, bool constructingTypeIsDict)
        {
            MethodInfo createRange = ImmutableCreateRangeMethod(constructingType, elementType, constructingTypeIsDict);

            Debug.Assert(createRange != null);

            object createRangeDelegate;
            if (constructingTypeIsDict)
            {
                createRangeDelegate = createRange.CreateDelegate(
                    typeof(DefaultImmutableConverter.ImmutableDictCreateRangeDelegate<,>).MakeGenericType(typeof(string), elementType), null);
            }
            else
            {
                createRangeDelegate = createRange.CreateDelegate(
                typeof(DefaultImmutableConverter.ImmutableCreateRangeDelegate<>).MakeGenericType(elementType), null);
            }

            Debug.Assert(createRangeDelegate != null);
            return createRangeDelegate;
        }
    }
}
