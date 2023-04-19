﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Castle.DynamicLinqQueryBuilder
{
    public static class ReflectionHelpers
    {
        public static MethodInfo SelectMethod;
        public static MethodInfo ToListMethod;

        static ReflectionHelpers()
        {
            SelectMethod = GetExtensionMethod(typeof(Enumerable).Assembly, "Select");
            ToListMethod = GetExtensionMethod(typeof(Enumerable).Assembly, "ToList");
        }

        public static IEnumerable<MethodInfo> GetExtensionMethods(Assembly extensionsAssembly)
            => from t in extensionsAssembly.GetTypes()
               where !t.IsGenericType && !t.IsNested
               from m in t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
               where m.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false)
               //where m.GetParameters()[0].ParameterType == type
               select m;

        public static MethodInfo GetExtensionMethod(Assembly extensionsAssembly, string name)
            => GetExtensionMethods(extensionsAssembly).FirstOrDefault(m => m.Name == name);

        public static MethodInfo GetExtensionMethod(Assembly extensionsAssembly, string name, Type[] types)
        {
            var methods = (from m in GetExtensionMethods(extensionsAssembly)
                           where m.Name == name && m.GetParameters().Count() == types.Length + 1 // + 1 because extension method parameter (this)
                           select m).ToList();

            if (!methods.Any())
            {
                return default;
            }

            return methods.First(); // Not the best option.
        }
    }
}