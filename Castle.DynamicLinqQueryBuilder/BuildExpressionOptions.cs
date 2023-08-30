using System;
using System.Collections.Generic;
using System.Globalization;

namespace Castle.DynamicLinqQueryBuilder
{
    /// <summary>
    /// Options to use when building expressions
    /// </summary>
    public class BuildExpressionOptions
    {
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
        public string IndexedPropertyName { get; set; }

        /// <summary>
        /// Custom operators
        /// </summary>
        public IEnumerable<IFilterOperator> Operators { get; set; }

        /// <summary>
        /// Flag to null check CLR objects in nested queries. May Cause ORM queries to fail.
        /// </summary>
        public bool NullCheckNestedCLRObjects { get; set; } = false;

        /// <summary>
        /// Indicates whether string comparisons are case sensitive or not, utilizing .ToLower() method
        /// </summary>
        public bool StringCaseSensitiveComparison { get; set; } = false;
        
        /// <summary>
        /// Indicates whether to require explicit ToString() conversion for non-string types
        /// </summary>
        public bool RequireExplicitToStringConversion { get; set; } = true;
    }
}
