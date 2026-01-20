using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;

namespace Castle.DynamicLinqQueryBuilder
{
    /// <summary>
    /// Options to use when building expressions
    /// </summary>
    public class BuildExpressionOptions
    {
        /// <summary>
        /// Default maximum cache size for expression caching.
        /// </summary>
        public const int DefaultCacheMaxSize = 1000;

        /// <summary>
        /// The <see cref="CultureInfo"/> to use when converting string representations (default InvariantCulture).
        /// </summary>
        public CultureInfo CultureInfo { get; set; } = CultureInfo.InvariantCulture;

        /// <summary>
        /// Whether <see cref="DateTime"/> types should be parsed as UTC.
        /// </summary>
        public bool ParseDatesAsUtc { get; set; }

        /// <summary>
        /// Whether or not to use indexed property
        /// </summary>
        public bool UseIndexedProperty { get; set; }

        /// <summary>
        /// The name of indexable property to use.
        /// </summary>
        public string? IndexedPropertyName { get; set; }

        /// <summary>
        /// Custom operators
        /// </summary>
        public IEnumerable<IFilterOperator>? Operators { get; set; }

        /// <summary>
        /// Flag to null check CLR objects in nested queries. May Cause ORM queries to fail.
        /// </summary>
        public bool NullCheckNestedCLRObjects { get; set; } = false;

        /// <summary>
        /// Indicates whether string comparisons are case sensitive or not.
        /// When false (default), case-insensitive comparison is used.
        /// </summary>
        public bool StringCaseSensitiveComparison { get; set; } = false;

        /// <summary>
        /// When true and StringCaseSensitiveComparison is false, uses StringComparison.OrdinalIgnoreCase
        /// instead of ToLower() for case-insensitive comparisons. This is more efficient but incompatible
        /// with ORM query translation (e.g., EF Core). Default is false for ORM compatibility.
        /// </summary>
        public bool UseOrdinalStringComparison { get; set; } = false;
        
        /// <summary>
        /// Indicates whether to require explicit ToString() conversion for non-string types
        /// </summary>
        public bool RequireExplicitToStringConversion { get; set; } = true;

        /// <summary>
        /// Whether to enable caching of built expression trees. Default is false.
        /// When enabled, identical filter rules will reuse previously built expressions.
        /// Note: Caching uses a global static cache shared across all instances.
        /// </summary>
        public bool EnableExpressionCaching { get; set; } = false;

        /// <summary>
        /// Whether to enable caching of compiled predicates. Default is false.
        /// When enabled, BuildPredicate will cache and reuse compiled delegates.
        /// Note: Caching uses a global static cache shared across all instances.
        /// </summary>
        public bool EnablePredicateCaching { get; set; } = false;

        /// <summary>
        /// Maximum number of entries to keep in the expression/predicate caches.
        /// When exceeded, least recently used entries are evicted. Default is 1000.
        /// </summary>
        public int CacheMaxSize { get; set; } = DefaultCacheMaxSize;
    }

    /// <summary>
    /// Global cache manager for expression and predicate caching.
    /// </summary>
    public static class QueryBuilderCache
    {
        private static ExpressionCache<object>? _expressionCache;
        private static ExpressionCache<object>? _predicateCache;
        private static int _maxSize = BuildExpressionOptions.DefaultCacheMaxSize;
        private static readonly object _initLock = new();

        /// <summary>
        /// Gets or sets the maximum cache size. Changes take effect on next cache access after Clear().
        /// </summary>
        public static int MaxSize
        {
            get => _maxSize;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Max size must be greater than 0.");
                _maxSize = value;
            }
        }

        internal static ExpressionCache<object> ExpressionCache
        {
            get
            {
                if (_expressionCache == null)
                {
                    lock (_initLock)
                    {
                        _expressionCache ??= new ExpressionCache<object>(_maxSize);
                    }
                }
                return _expressionCache;
            }
        }

        internal static ExpressionCache<object> PredicateCache
        {
            get
            {
                if (_predicateCache == null)
                {
                    lock (_initLock)
                    {
                        _predicateCache ??= new ExpressionCache<object>(_maxSize);
                    }
                }
                return _predicateCache;
            }
        }

        /// <summary>
        /// Clears all cached expressions and predicates.
        /// </summary>
        public static void Clear()
        {
            lock (_initLock)
            {
                _expressionCache?.Clear();
                _predicateCache?.Clear();
            }
        }

        /// <summary>
        /// Reinitializes the caches with a new maximum size.
        /// This clears all existing cached entries.
        /// </summary>
        /// <param name="maxSize">The new maximum cache size.</param>
        public static void Reinitialize(int maxSize)
        {
            if (maxSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxSize), "Max size must be greater than 0.");

            lock (_initLock)
            {
                _maxSize = maxSize;
                _expressionCache = new ExpressionCache<object>(maxSize);
                _predicateCache = new ExpressionCache<object>(maxSize);
            }
        }

        /// <summary>
        /// Gets the current number of cached expressions.
        /// </summary>
        public static int ExpressionCacheCount => _expressionCache?.Count ?? 0;

        /// <summary>
        /// Gets the current number of cached predicates.
        /// </summary>
        public static int PredicateCacheCount => _predicateCache?.Count ?? 0;
    }
}
