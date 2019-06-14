// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if BUILDING_INBOX_LIBRARY
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Text.Json
{
    internal sealed class ReflectionEmitMaterializer : ClassMaterializer
    {
        public override JsonClassInfo.ConstructorDelegate CreateConstructor(Type type)
        {
            Debug.Assert(type != null);
            ConstructorInfo realMethod = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, binder: null, Type.EmptyTypes, modifiers: null);

            if (realMethod == null && !type.IsValueType)
            {
                return null;
            }

            var dynamicMethod = new DynamicMethod(
                ConstructorInfo.ConstructorName,
                typeof(object),
                Type.EmptyTypes,
                typeof(ReflectionEmitMaterializer).Module,
                skipVisibility: true);

            ILGenerator generator = dynamicMethod.GetILGenerator();

            if (realMethod == null)
            {
                LocalBuilder local = generator.DeclareLocal(type);

                generator.Emit(OpCodes.Ldloca_S, local);
                generator.Emit(OpCodes.Initobj, type);
                generator.Emit(OpCodes.Ldloc, local);
                generator.Emit(OpCodes.Box, type);
            }
            else
            {
                generator.Emit(OpCodes.Newobj, realMethod);
            }

            generator.Emit(OpCodes.Ret);

            return (JsonClassInfo.ConstructorDelegate)dynamicMethod.CreateDelegate(typeof(JsonClassInfo.ConstructorDelegate));
        }

        //public abstract MethodWithGenericIEnumerableParameterDelegate<T> CreateDelegateForMethodWithGenericIEnumerableParameter<T>(IEnumerable<T> items);

        //public abstract MethodWithGenericIEnumerableOfKeyValuePairParameterDelegate<TKey, TValue> CreateDelegateFor<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> items);

        //public abstract MethodWithICollectionParameterDelegate CreateDelegateForMethodWithICollectionParameter(ICollection items);

        //public abstract MethodWithIDictionaryParameterDelegate CreateDelegateForMethodWithIDictionaryParameter(IDictionary items);

        public override MethodWithGenericIEnumerableParameterDelegate<T> CreateDelegateForMethodWithGenericIEnumerableParameter<T>(Type type)
        {
            Debug.Assert(type != null);
            ConstructorInfo realMethod = type.GetConstructor(new Type[] { typeof(IEnumerable<T>) });

            if (realMethod == null && !type.IsValueType)
            {
                return null;
            }

            var dynamicMethod = new DynamicMethod(
                ConstructorInfo.ConstructorName,
                typeof(object),
                Type.EmptyTypes,
                typeof(ReflectionEmitMaterializer).Module,
                skipVisibility: true);

            ILGenerator generator = dynamicMethod.GetILGenerator();

            if (realMethod == null)
            {
                LocalBuilder local = generator.DeclareLocal(type);

                generator.Emit(OpCodes.Ldloca_S, local);
                generator.Emit(OpCodes.Initobj, type);
                generator.Emit(OpCodes.Ldloc, local);
                generator.Emit(OpCodes.Box, type);
            }
            else
            {
                generator.Emit(OpCodes.Newobj, realMethod);
            }

            return (MethodWithGenericIEnumerableParameterDelegate<T>)dynamicMethod.CreateDelegate(typeof(MethodWithGenericIEnumerableParameterDelegate<T>));
        }

        public override MethodWithGenericIEnumerableOfKeyValuePairParameterDelegate<TKey, TValue> CreateDelegateForMethodWithGenericIEnumerableOfKeyValuePairParameter<TKey, TValue>(Type type)
        {
            Debug.Assert(type != null);
            ConstructorInfo realMethod = type.GetConstructor(new Type[] { typeof(IEnumerable<KeyValuePair<TKey, TValue>>) });

            if (realMethod == null && !type.IsValueType)
            {
                return null;
            }

            var dynamicMethod = new DynamicMethod(
                ConstructorInfo.ConstructorName,
                typeof(object),
                Type.EmptyTypes,
                typeof(ReflectionEmitMaterializer).Module,
                skipVisibility: true);

            ILGenerator generator = dynamicMethod.GetILGenerator();

            if (realMethod == null)
            {
                LocalBuilder local = generator.DeclareLocal(type);

                generator.Emit(OpCodes.Ldloca_S, local);
                generator.Emit(OpCodes.Initobj, type);
                generator.Emit(OpCodes.Ldloc, local);
                generator.Emit(OpCodes.Box, type);
            }
            else
            {
                generator.Emit(OpCodes.Newobj, realMethod);
            }

            return (MethodWithGenericIEnumerableOfKeyValuePairParameterDelegate<TKey, TValue>)dynamicMethod.CreateDelegate(
                typeof(MethodWithGenericIEnumerableOfKeyValuePairParameterDelegate<TKey, TValue>));
        }

        public override MethodWithGenericIEnumerableParameterDelegate<T> CreateDelegateForMethodWithGenericIEnumerableParameter<T>(Type type)
        {
            Debug.Assert(type != null);
            ConstructorInfo realMethod = type.GetConstructor(new Type[] { typeof(IEnumerable<T>) });

            if (realMethod == null && !type.IsValueType)
            {
                return null;
            }

            var dynamicMethod = new DynamicMethod(
                ConstructorInfo.ConstructorName,
                typeof(object),
                Type.EmptyTypes,
                typeof(ReflectionEmitMaterializer).Module,
                skipVisibility: true);

            ILGenerator generator = dynamicMethod.GetILGenerator();

            if (realMethod == null)
            {
                LocalBuilder local = generator.DeclareLocal(type);

                generator.Emit(OpCodes.Ldloca_S, local);
                generator.Emit(OpCodes.Initobj, type);
                generator.Emit(OpCodes.Ldloc, local);
                generator.Emit(OpCodes.Box, type);
            }
            else
            {
                generator.Emit(OpCodes.Newobj, realMethod);
            }

            return (MethodWithGenericIEnumerableParameterDelegate<T>)dynamicMethod.CreateDelegate(typeof(MethodWithGenericIEnumerableParameterDelegate<T>));
        }

        public override MethodWithGenericIEnumerableParameterDelegate<T> CreateDelegateForMethodWithGenericIEnumerableParameter<T>(Type type)
        {
            Debug.Assert(type != null);
            ConstructorInfo realMethod = type.GetConstructor(new Type[] { typeof(IEnumerable<T>) });

            if (realMethod == null && !type.IsValueType)
            {
                return null;
            }

            var dynamicMethod = new DynamicMethod(
                ConstructorInfo.ConstructorName,
                typeof(object),
                Type.EmptyTypes,
                typeof(ReflectionEmitMaterializer).Module,
                skipVisibility: true);

            ILGenerator generator = dynamicMethod.GetILGenerator();

            if (realMethod == null)
            {
                LocalBuilder local = generator.DeclareLocal(type);

                generator.Emit(OpCodes.Ldloca_S, local);
                generator.Emit(OpCodes.Initobj, type);
                generator.Emit(OpCodes.Ldloc, local);
                generator.Emit(OpCodes.Box, type);
            }
            else
            {
                generator.Emit(OpCodes.Newobj, realMethod);
            }

            return (MethodWithGenericIEnumerableParameterDelegate<T>)dynamicMethod.CreateDelegate(typeof(MethodWithGenericIEnumerableParameterDelegate<T>));
        }

        public override object ImmutableCollectionCreateRange(Type constructingType, Type elementType)
        {
            MethodInfo createRange = ImmutableCollectionCreateRangeMethod(constructingType, elementType);

            if (createRange == null)
            {
                return null;
            }

            return createRange.CreateDelegate(
                typeof(MethodWithGenericIEnumerableParameterDelegate<>).MakeGenericType(elementType), null);
        }

        public override object ImmutableDictionaryCreateRange(Type constructingType, Type elementType)
        {
            MethodInfo createRange = ImmutableDictionaryCreateRangeMethod(constructingType, elementType);

            if (createRange == null)
            {
                return null;
            }

            return createRange.CreateDelegate(
                typeof(MethodWithGenericIEnumerableOfKeyValuePairParameterDelegate<,>).MakeGenericType(typeof(string), elementType), null);
        }
    }
}
#endif
