// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace System.Text.Json
{
    internal abstract class ClassMaterializer
    {
        // Delegate for methods with a single IEnumerable<T> parameter that returns some Foo<T>, e.g.
        // Queue<T>(IEnumerable<T>) returns an instance of Queue<T>,
        // ImmutableList.CreateRange(IEnumerable<T>) returns an instance of ImmutableList<T>.
        public delegate object MethodWithGenericIEnumerableParameterDelegate<T>(IEnumerable<T> items);

        // Delegate for methods with a single IEnumerable<KeyValuePair<TKey, TValue>> parameter that returns some Foo<TKey, TValue>, e.g.
        // ImmutableDictionary.CreateRange(IEnumerable<KeyValuePair<string, TValue>>) returns an instance of ImmutableDictionary<string, TValue>
        public delegate object MethodWithGenericIEnumerableOfKeyValuePairParameterDelegate<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> items);

        // Delegate for methods with single ICollection parameter that returns some Foo, e.g.
        // that returns an IEnumerable or IDictionary.
        // e.g. Stack(ICollection) returns an instance of Stack        
        public delegate object MethodWithICollectionParameterDelegate(ICollection items);

        // Delegate for methods with single IDictionary parameter that returns some Foo, e.g.
        // Hashtable(IDictionary) returns an instance of Hashtable,
        public delegate object MethodWithIDictionaryParameterDelegate(IDictionary items);

        public abstract JsonClassInfo.ConstructorDelegate CreateConstructor(Type classType);

        public abstract MethodWithGenericIEnumerableParameterDelegate<T> CreateDelegateForMethodWithGenericIEnumerableParameter<T>(Type type);

        public abstract MethodWithGenericIEnumerableOfKeyValuePairParameterDelegate<TKey, TValue> CreateDelegateForMethodWithGenericIEnumerableOfKeyValuePairParameter<TKey, TValue>(Type type);

        public abstract MethodWithICollectionParameterDelegate CreateDelegateForMethodWithICollectionParameter(Type type);

        public abstract MethodWithIDictionaryParameterDelegate CreateDelegateForMethodWithIDictionaryParameter(Type type);

        public abstract object ImmutableCollectionCreateRange(Type constructingType, Type elementType);
        public abstract object ImmutableDictionaryCreateRange(Type constructingType, Type elementType);

        protected MethodInfo ImmutableCollectionCreateRangeMethod(Type constructingType, Type elementType)
        {
            MethodInfo createRangeMethod = FindImmutableCreateRangeMethod(constructingType);

            if (createRangeMethod == null)
            {
                return null;
            }

            return createRangeMethod.MakeGenericMethod(elementType);
        }

        protected MethodInfo ImmutableDictionaryCreateRangeMethod(Type constructingType, Type elementType)
        {
            MethodInfo createRangeMethod = FindImmutableCreateRangeMethod(constructingType);

            if (createRangeMethod == null)
            {
                return null;
            }

            return createRangeMethod.MakeGenericMethod(typeof(string), elementType);
        }

        private MethodInfo FindImmutableCreateRangeMethod(Type constructingType)
        {
            MethodInfo[] constructingTypeMethods = constructingType.GetMethods();

            foreach (MethodInfo method in constructingTypeMethods)
            {
                if (method.Name == "CreateRange" && method.GetParameters().Length == 1)
                {
                    return method;
                }
            }

            // This shouldn't happen because constructingType should be an immutable type with
            // a CreateRange method. `null` being returned here will cause a JsonException to be
            // thrown when the desired CreateRange delegate is about to be invoked.
            Debug.Fail("Could not create the appropriate CreateRange method.");
            return null;
        }
    }
}
