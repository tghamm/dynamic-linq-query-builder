using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Castle.DynamicLinqQueryBuilder
{
    /// <summary>
    /// Provides cached reflection helpers for improved performance.
    /// </summary>
    public static class ReflectionHelpers
    {
        #region Existing Static Methods

        /// <summary>Cached Select extension method.</summary>
        public static readonly MethodInfo SelectMethod;
        /// <summary>Cached Where extension method.</summary>
        public static readonly MethodInfo WhereMethod;
        /// <summary>Cached Contains extension method.</summary>
        public static readonly MethodInfo ContainsMethod;
        /// <summary>Cached ToList extension method.</summary>
        public static readonly MethodInfo ToListMethod;

        #endregion

        #region Cached String Methods (for expression building)

        /// <summary>String.ToLower() method info.</summary>
        public static readonly MethodInfo StringToLowerMethod;
        /// <summary>String.ToString() method info (parameterless).</summary>
        public static readonly MethodInfo StringToStringMethod;
        /// <summary>String.Contains(string) method info.</summary>
        public static readonly MethodInfo StringContainsMethod;
        /// <summary>String.StartsWith(string) method info.</summary>
        public static readonly MethodInfo StringStartsWithMethod;
        /// <summary>String.EndsWith(string) method info.</summary>
        public static readonly MethodInfo StringEndsWithMethod;
        /// <summary>String.Contains(string, StringComparison) method info.</summary>
        public static readonly MethodInfo StringContainsWithComparisonMethod;
        /// <summary>String.StartsWith(string, StringComparison) method info.</summary>
        public static readonly MethodInfo StringStartsWithComparisonMethod;
        /// <summary>String.EndsWith(string, StringComparison) method info.</summary>
        public static readonly MethodInfo StringEndsWithComparisonMethod;
        /// <summary>String.Equals(string, string, StringComparison) static method info.</summary>
        public static readonly MethodInfo StringEqualsStaticMethod;

        #endregion

        #region Caches

        private static readonly ConcurrentDictionary<(Type, string), PropertyInfo?> PropertyCache = new();
        private static readonly ConcurrentDictionary<Type, TypeConverter> TypeConverterCache = new();
        private static readonly ConcurrentDictionary<(Type, string, int), MethodInfo?> MethodCache = new();
        private static readonly ConcurrentDictionary<Type, Type?> InterfaceCache = new();

        #endregion

        static ReflectionHelpers()
        {
            // Initialize existing method lookups
            SelectMethod = GetExtensionMethod(typeof(Enumerable).Assembly, "Select")!;
            WhereMethod = GetExtensionMethod(typeof(Enumerable).Assembly, "Where")!;
            ContainsMethod = GetExtensionMethod(typeof(Enumerable).Assembly, "Contains")!;
            ToListMethod = GetExtensionMethod(typeof(Enumerable).Assembly, "ToList")!;

            // Cache commonly used string methods
            StringToLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
            StringToStringMethod = typeof(object).GetMethod("ToString", Type.EmptyTypes)!;
            StringContainsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
            StringStartsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!;
            StringEndsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!;
            
            // String methods with StringComparison parameter
            StringContainsWithComparisonMethod = typeof(string).GetMethod("Contains", new[] { typeof(string), typeof(StringComparison) })!;
            StringStartsWithComparisonMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string), typeof(StringComparison) })!;
            StringEndsWithComparisonMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string), typeof(StringComparison) })!;
            StringEqualsStaticMethod = typeof(string).GetMethod("Equals", new[] { typeof(string), typeof(string), typeof(StringComparison) })!;
        }

        #region Public Cache Accessors

        /// <summary>
        /// Gets a cached PropertyInfo for a type and property name.
        /// </summary>
        /// <param name="type">The type to get the property from.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The PropertyInfo, or null if not found.</returns>
        public static PropertyInfo? GetCachedProperty(Type type, string propertyName)
        {
            return PropertyCache.GetOrAdd((type, propertyName), key => key.Item1.GetProperty(key.Item2));
        }

        /// <summary>
        /// Gets a cached TypeConverter for a type.
        /// </summary>
        /// <param name="type">The type to get the converter for.</param>
        /// <returns>The TypeConverter for the type.</returns>
        public static TypeConverter GetCachedConverter(Type type)
        {
            return TypeConverterCache.GetOrAdd(type, TypeDescriptor.GetConverter);
        }

        /// <summary>
        /// Gets a cached MethodInfo for a type, method name, and parameter types.
        /// </summary>
        /// <param name="type">The type to get the method from.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        /// <returns>The MethodInfo, or null if not found.</returns>
        public static MethodInfo? GetCachedMethod(Type type, string methodName, Type[] parameterTypes)
        {
            // Use hash code of parameter types array for cache key
            var paramHash = GetParameterTypesHash(parameterTypes);
            return MethodCache.GetOrAdd((type, methodName, paramHash), key => key.Item1.GetMethod(key.Item2, parameterTypes));
        }

        /// <summary>
        /// Gets a cached interface type for a type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="interfaceName">The name of the interface (e.g., "IEnumerable`1").</param>
        /// <returns>The interface type, or null if not implemented.</returns>
        public static Type? GetCachedInterface(Type type, string interfaceName)
        {
            // For simplicity, just use GetInterface - it's already quite fast
            return type.GetInterface(interfaceName);
        }

        /// <summary>
        /// Clears all reflection caches.
        /// </summary>
        public static void ClearCaches()
        {
            PropertyCache.Clear();
            TypeConverterCache.Clear();
            MethodCache.Clear();
            InterfaceCache.Clear();
        }

        #endregion

        #region Extension Method Helpers

        /// <summary>
        /// Gets all extension methods from an assembly.
        /// </summary>
        public static IEnumerable<MethodInfo> GetExtensionMethods(Assembly extensionsAssembly)
            => from t in extensionsAssembly.GetTypes()
               where !t.IsGenericType && !t.IsNested
               from m in t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
               where m.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false)
               select m;

        /// <summary>
        /// Gets an extension method by name from an assembly.
        /// </summary>
        public static MethodInfo? GetExtensionMethod(Assembly extensionsAssembly, string name)
            => GetExtensionMethods(extensionsAssembly).FirstOrDefault(m => m.Name == name);

        #endregion

        #region Private Helpers

        private static int GetParameterTypesHash(Type[] parameterTypes)
        {
            if (parameterTypes == null || parameterTypes.Length == 0)
                return 0;

            unchecked
            {
                int hash = 17;
                foreach (var type in parameterTypes)
                {
                    hash = hash * 31 + (type?.GetHashCode() ?? 0);
                }
                return hash;
            }
        }

        #endregion
    }
}
