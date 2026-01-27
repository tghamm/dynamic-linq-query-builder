using System;
using System.Collections;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Castle.DynamicLinqQueryBuilder
{
    /// <summary>
    /// Generic IQueryable filter implementation.  Based upon configuration of FilterRules 
    /// mapping to the data source.  When applied, acts as an advanced filter mechanism.
    /// </summary>
    public static class QueryBuilder
    {
        /// <summary>
        /// Gets or sets a value indicating whether incoming dates in the filter should be parsed as UTC.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [parse dates as UTC]; otherwise, <c>false</c>.
        /// </value>
        public static bool ParseDatesAsUtc { get; set; }

        /// <summary>
        /// Gets the filtered collection after applying the provided filter rules.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="queryable">The queryable.</param>
        /// <param name="filterRule">The filter rule.</param>
        /// <param name="useIndexedProperty">Whether or not to use indexed property</param>
        /// <param name="indexedPropertyName">The indexable property to use</param>
        /// <returns>Filtered IQueryable</returns>
        public static IQueryable<T> BuildQuery<T>(this IEnumerable<T> queryable, IFilterRule filterRule, bool useIndexedProperty = false, string indexedPropertyName = null)
        {
            string parsedQuery;
            return BuildQuery(queryable.AsQueryable(), filterRule, out parsedQuery, useIndexedProperty, indexedPropertyName);
        }

        /// <summary>
        /// Gets the filtered collection after applying the provided filter rules.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="queryable">The queryable.</param>
        /// <param name="filterRule">The filter rule.</param>
        /// <param name="useIndexedProperty">Whether or not to use indexed property</param>
        /// <param name="indexedPropertyName">The indexable property to use</param>
        /// <returns>Filtered IQueryable</returns>
        public static IQueryable<T> BuildQuery<T>(this IList<T> queryable, IFilterRule filterRule, bool useIndexedProperty = false, string indexedPropertyName = null)
        {
            string parsedQuery;
            return BuildQuery(queryable.AsQueryable(), filterRule, out parsedQuery, useIndexedProperty, indexedPropertyName);
        }

        /// <summary>
        /// Gets the filtered collection after applying the provided filter rules.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="queryable">The queryable.</param>
        /// <param name="filterRule">The filter rule.</param>
        /// <param name="useIndexedProperty">Whether or not to use indexed property</param>
        /// <param name="indexedPropertyName">The indexable property to use</param>
        /// <returns>Filtered IQueryable</returns>
        public static IQueryable<T> BuildQuery<T>(this IQueryable<T> queryable, IFilterRule filterRule, bool useIndexedProperty = false, string indexedPropertyName = null)
        {
            string parsedQuery;
            return BuildQuery(queryable, filterRule, out parsedQuery, useIndexedProperty, indexedPropertyName);
        }

        /// <summary>
        /// Gets the filtered collection after applying the provided filter rules. 
        /// Returns the string representation for diagnostic purposes.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="queryable">The queryable.</param>
        /// <param name="filterRule">The filter rule.</param>
        /// <param name="parsedQuery">The parsed query.</param>
        /// <param name="useIndexedProperty">Whether or not to use indexed property</param>
        /// <param name="indexedPropertyName">The indexable property to use</param>
        /// <returns>Filtered IQueryable.</returns>
        public static IQueryable<T> BuildQuery<T>(this IQueryable<T> queryable, IFilterRule filterRule, out string parsedQuery, bool useIndexedProperty = false, string indexedPropertyName = null)
        {
            return BuildQuery(queryable, filterRule, new BuildExpressionOptions { UseIndexedProperty = useIndexedProperty, IndexedPropertyName = indexedPropertyName }, out parsedQuery);
        }

        /// <summary>
        /// Gets the filtered collection after applying the provided filter rules. 
        /// Returns the string representation for diagnostic purposes.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="queryable">The queryable.</param>
        /// <param name="filterRule">The filter rule.</param>
        /// <param name="options">The options to use when building the expression</param>
        /// <returns>Filtered IQueryable.</returns>
        public static IQueryable<T> BuildQuery<T>(this IQueryable<T> queryable, IFilterRule filterRule, BuildExpressionOptions options)
        {
            string parsedQuery;
            return BuildQuery(queryable, filterRule, options, out parsedQuery);
        }

        /// <summary>
        /// Gets the filtered collection after applying the provided filter rules. 
        /// Returns the string representation for diagnostic purposes.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="queryable">The queryable.</param>
        /// <param name="filterRule">The filter rule.</param>
        /// <param name="options">The options to use when building the expression</param>
        /// <param name="parsedQuery">The parsed query.</param>
        /// <returns>Filtered IQueryable.</returns>
        public static IQueryable<T> BuildQuery<T>(this IQueryable<T> queryable, IFilterRule filterRule, BuildExpressionOptions options, out string parsedQuery)
        {
            var expression = BuildExpressionLambda<T>(filterRule, options, out parsedQuery);

            if (expression == null)
            {
                return queryable;
            }

            var whereCallExpression = Expression.Call(
                typeof(Queryable),
                "Where",
                new[] { queryable.ElementType },
                queryable.Expression,
                expression);

            var filteredResults = queryable.Provider.CreateQuery<T>(whereCallExpression);

            return filteredResults;
        }

        // Cached static predicate for null/empty rules
        private static readonly Func<object, bool> TruePredicateObject = _ => true;
        
        // Cached common expressions to avoid repeated allocations
        private static readonly ConstantExpression TrueExpression = Expression.Constant(true, typeof(bool));
        private static readonly ConstantExpression FalseExpression = Expression.Constant(false, typeof(bool));
        private static readonly ConstantExpression NullObjectExpression = Expression.Constant(null, typeof(object));
        private static readonly ConstantExpression ZeroIntExpression = Expression.Constant(0, typeof(int));
        private static readonly ConstantExpression GuidEmptyExpression = Expression.Constant(Guid.Empty, typeof(Guid));

        /// <summary>
        /// Builds a predicate that returns whether an input test object passes the filter rule.
        /// </summary>
        /// <typeparam name="T">The generic type of the input object to test.</typeparam>
        /// <param name="filterRule">The filter rule.</param>
        /// <param name="options">The options to use when building the expression</param>
        /// <returns>A predicate function implementing the filter rule</returns>
        public static Func<T, bool> BuildPredicate<T>(this IFilterRule filterRule, BuildExpressionOptions options)
        {
            string parsedQuery;
            return BuildPredicate<T>(filterRule, options, out parsedQuery);
        }

        /// <summary>
        /// Builds a predicate that returns whether an input test object passes the filter rule.
        /// </summary>
        /// <typeparam name="T">The generic type of the input object to test.</typeparam>
        /// <param name="filterRule">The filter rule.</param>
        /// <param name="parsedQuery">The parsed query.</param>
        /// <param name="options">The options to use when building the expression</param>
        /// <returns>A predicate function implementing the filter rule</returns>
        public static Func<T, bool> BuildPredicate<T>(this IFilterRule filterRule, BuildExpressionOptions options, out string parsedQuery)
        {
            if (filterRule == null)
            {
                parsedQuery = "";
                return (Func<T, bool>)(object)TruePredicateObject;
            }

            // Try to get from predicate cache
            if (options.EnablePredicateCaching)
            {
                var cacheKey = GenerateCacheKey<T>(filterRule, options, "pred");
                if (QueryBuilderCache.PredicateCache.TryGet(cacheKey, out var cached))
                {
                    var cachedEntry = (CachedPredicate<T>)cached!;
                    parsedQuery = cachedEntry.ParsedQuery;
                    return cachedEntry.Predicate;
                }

                var expression = BuildExpressionLambdaInternal<T>(filterRule, options, out parsedQuery);
                if (expression == null)
                {
                    return (Func<T, bool>)(object)TruePredicateObject;
                }

                var predicate = expression.Compile();
                QueryBuilderCache.PredicateCache.Set(cacheKey, new CachedPredicate<T>(predicate, parsedQuery));
                return predicate;
            }

            var expr = BuildExpressionLambdaInternal<T>(filterRule, options, out parsedQuery);
            if (expr == null)
            {
                return (Func<T, bool>)(object)TruePredicateObject;
            }
            return expr.Compile();
        }

        /// <summary>
        /// Builds an expression lambda for the filter rule.
        /// </summary>
        /// <typeparam name="T">The generic type of the input object to test.</typeparam>
        /// <param name="filterRule">The filter rule.</param>
        /// <param name="parsedQuery">The parsed query.</param>
        /// <param name="options">The options to use when building the expression</param>
        /// <returns>An expression lambda that implements the filter rule</returns>
        public static Expression<Func<T, bool>>? BuildExpressionLambda<T>(this IFilterRule filterRule, BuildExpressionOptions options, out string parsedQuery)
        {
            if (filterRule == null)
            {
                parsedQuery = "";
                return null;
            }

            // Try to get from expression cache
            if (options.EnableExpressionCaching)
            {
                var cacheKey = GenerateCacheKey<T>(filterRule, options, "expr");
                if (QueryBuilderCache.ExpressionCache.TryGet(cacheKey, out var cached))
                {
                    var cachedEntry = (CachedExpression<T>)cached!;
                    parsedQuery = cachedEntry.ParsedQuery;
                    return cachedEntry.Expression;
                }

                var expression = BuildExpressionLambdaInternal<T>(filterRule, options, out parsedQuery);
                if (expression != null)
                {
                    QueryBuilderCache.ExpressionCache.Set(cacheKey, new CachedExpression<T>(expression, parsedQuery));
                }
                return expression;
            }

            return BuildExpressionLambdaInternal<T>(filterRule, options, out parsedQuery);
        }

        private static Expression<Func<T, bool>>? BuildExpressionLambdaInternal<T>(IFilterRule filterRule, BuildExpressionOptions options, out string parsedQuery)
        {
            var pe = Expression.Parameter(typeof(T), "item");

            var expressionTree = BuildExpressionTree(pe, filterRule, options);
            if (expressionTree == null)
            {
                parsedQuery = "";
                return null;
            }

            parsedQuery = expressionTree.ToString();

            return Expression.Lambda<Func<T, bool>>(expressionTree, pe);
        }

        #region Cache Key Generation

        private sealed class CachedExpression<T>
        {
            public Expression<Func<T, bool>> Expression { get; }
            public string ParsedQuery { get; }

            public CachedExpression(Expression<Func<T, bool>> expression, string parsedQuery)
            {
                Expression = expression;
                ParsedQuery = parsedQuery;
            }
        }

        private sealed class CachedPredicate<T>
        {
            public Func<T, bool> Predicate { get; }
            public string ParsedQuery { get; }

            public CachedPredicate(Func<T, bool> predicate, string parsedQuery)
            {
                Predicate = predicate;
                ParsedQuery = parsedQuery;
            }
        }

        private static string GenerateCacheKey<T>(IFilterRule rule, BuildExpressionOptions options, string prefix)
        {
            var sb = new StringBuilder(256);
            sb.Append(prefix);
            sb.Append('|');
            sb.Append(typeof(T).FullName);
            sb.Append('|');
            
            // Options that affect expression generation
            sb.Append(options.StringCaseSensitiveComparison ? '1' : '0');
            sb.Append(options.UseOrdinalStringComparison ? '1' : '0');
            sb.Append(options.ParseDatesAsUtc || ParseDatesAsUtc ? '1' : '0');
            sb.Append(options.NullCheckNestedCLRObjects ? '1' : '0');
            sb.Append(options.UseIndexedProperty ? '1' : '0');
            sb.Append(options.RequireExplicitToStringConversion ? '1' : '0');
            if (options.UseIndexedProperty && options.IndexedPropertyName != null)
            {
                sb.Append(options.IndexedPropertyName);
            }
            sb.Append('|');

            // Include custom operators in cache key (their presence affects expression generation)
            if (options.Operators != null)
            {
                foreach (var op in options.Operators)
                {
                    sb.Append(op.Operator);
                    sb.Append(';');
                }
            }
            sb.Append('|');

            // Rule structure
            AppendRuleToKey(sb, rule);

            return sb.ToString();
        }

        private static void AppendRuleToKey(StringBuilder sb, IFilterRule rule)
        {
            if (rule.Rules != null && rule.Rules.Any())
            {
                sb.Append('(');
                sb.Append(rule.Condition ?? "and");
                foreach (var childRule in rule.Rules)
                {
                    sb.Append(',');
                    AppendRuleToKey(sb, childRule);
                }
                sb.Append(')');
            }
            else
            {
                sb.Append('[');
                sb.Append(rule.Field ?? "");
                sb.Append(':');
                sb.Append(rule.Operator ?? "");
                sb.Append(':');
                sb.Append(rule.Type ?? "");
                sb.Append(':');
                AppendValue(sb, rule.Value);
                sb.Append(']');
            }
        }

        private static void AppendValue(StringBuilder sb, object? value)
        {
            if (value == null)
            {
                sb.Append("null");
                return;
            }

            if (value is string s)
            {
                sb.Append(s);
                return;
            }

            if (value is IEnumerable enumerable and not string)
            {
                sb.Append('{');
                var first = true;
                foreach (var item in enumerable)
                {
                    if (!first) sb.Append(',');
                    first = false;
                    sb.Append(item?.ToString() ?? "null");
                }
                sb.Append('}');
                return;
            }

            sb.Append(value.ToString());
        }

        #endregion

        private static Expression BuildExpressionTree(ParameterExpression pe, IFilterRule rule, BuildExpressionOptions options)
        {

            if (rule.Rules != null && rule.Rules.Any())
            {
                var expressions =
                    rule.Rules.Select(childRule => BuildExpressionTree(pe, childRule, options))
                        .Where(expression => expression != null)
                        .ToList();

                var expressionTree = expressions.First();

                var counter = 1;
                while (counter < expressions.Count)
                {
                    expressionTree = string.Equals(rule.Condition, "or", StringComparison.OrdinalIgnoreCase)
                        ? Expression.OrElse(expressionTree, expressions[counter])
                        : Expression.AndAlso(expressionTree, expressions[counter]);
                    counter++;
                }

                return expressionTree;
            }
            if (rule.Field != null)
            {
                Type type = GetCSharpType(rule.Type);

                if (options.UseIndexedProperty)
                {
                    var propertyExp = Expression.Property(pe, options.IndexedPropertyName, Expression.Constant(rule.Field));
                    return BuildOperatorExpression(propertyExp, rule, options, type);
                }
                else
                {
                    var propertyList = rule.Field.Split('.');
                    if (propertyList.Length > 1)
                    {
                        var propertyCollectionEnumerator = propertyList.AsEnumerable().GetEnumerator();
                        return BuildNestedExpression(pe, propertyCollectionEnumerator, rule, options, type);
                    }
                    else
                    {
                        var propertyExp = Expression.Property(pe, rule.Field);
                        return BuildOperatorExpression(propertyExp, rule, options, type);
                    }
                }
            }
            return null;
        }

        public static System.Type GetCSharpType(string typeName)
        {
            Type type;

            switch (typeName)
            {
                case "integer":
                    type = typeof(int);
                    break;
                case "double":
                    type = typeof(double);
                    break;
                case "long":
                    type = typeof(long);
                    break;
                case "string":
                    type = typeof(string);
                    break;
                case "date":
                    type = typeof(System.DateOnly);
                    break;
                case "datetime":
                    type = typeof(DateTime);
                    break;
                case "boolean":
                    type = typeof(bool);
                    break;
                case "guid":
                    type = typeof(Guid);
                    break;
                // Additional numeric types
                case "short":
                case "int16":
                    type = typeof(short);
                    break;
                case "ushort":
                case "uint16":
                    type = typeof(ushort);
                    break;
                case "uint":
                case "uint32":
                    type = typeof(uint);
                    break;
                case "ulong":
                case "uint64":
                    type = typeof(ulong);
                    break;
                case "byte":
                    type = typeof(byte);
                    break;
                case "sbyte":
                    type = typeof(sbyte);
                    break;
                case "float":
                case "single":
                    type = typeof(float);
                    break;
                case "decimal":
                    type = typeof(decimal);
                    break;
                default:
                    throw new Exception($"Unexpected data type {typeName}");
            }

            return type;
        }

        private static Expression BuildNestedExpression(Expression expression, IEnumerator<string> propertyCollectionEnumerator, IFilterRule rule, BuildExpressionOptions options, Type type)
        {
            while (propertyCollectionEnumerator.MoveNext())
            {
                var propertyName = propertyCollectionEnumerator.Current;
                var property = ReflectionHelpers.GetCachedProperty(expression.Type, propertyName);
                expression = Expression.Property(expression, property!);

                var propertyType = property!.PropertyType;
                var enumerable = propertyType.GetInterface("IEnumerable`1");
                // If the filter tries to access the Dictionary content
                if (IsDictionary(propertyType) && propertyCollectionEnumerator.MoveNext())
                {
                    var key = propertyCollectionEnumerator.Current;
                    var indexExpr = Expression.Constant(key);

                    var getItemMethod = ReflectionHelpers.GetCachedMethod(propertyType, "get_Item", new[] { typeof(string) });
                    expression = Expression.Call(expression, getItemMethod!, indexExpr);
                    // recursively build the body of the lambda expression for the nested properties
                    var body = BuildNestedExpression(expression, propertyCollectionEnumerator, rule, options, type); 
                    return body;
                }
                if (propertyType != typeof(string) && enumerable != null)
                {
                    var elementType = enumerable.GetGenericArguments()[0];
                    var predicateFnType = typeof(Func<,>).MakeGenericType(elementType, typeof(bool));
                    var parameterExpression = Expression.Parameter(elementType);

                    Expression body = BuildNestedExpression(parameterExpression, propertyCollectionEnumerator, rule, options, type);
                    var predicate = Expression.Lambda(predicateFnType, body, parameterExpression);
                    var queryable = Expression.Call(typeof(Queryable), "AsQueryable", new[] { elementType }, expression);

                    if (options.NullCheckNestedCLRObjects)
                    {
                        var notnull = Expression.NotEqual(expression, NullObjectExpression);
                        return Expression.AndAlso(notnull, Expression.Call(
                            typeof(Queryable),
                            "Any",
                            new[] { elementType },
                            queryable,
                            predicate
                        ));
                    }
                    else
                    {
                        return Expression.Call(
                            typeof(Queryable),
                            "Any",
                            new[] { elementType },
                            queryable,
                            predicate
                        );
                    }
                    
                }
                if (options.NullCheckNestedCLRObjects && !expression.Type.IsValueType && propertyType != typeof(string))
                {
                    var notnull = IsNotNull(expression);
                    Expression body = BuildNestedExpression(expression, propertyCollectionEnumerator, rule, options, type);
                    return Expression.AndAlso(notnull, body);
                }
            }

            return BuildOperatorExpression(expression, rule, options, type);
        }

        private static bool IsDictionary(Type type)
        {
            if (type.GetInterface("IDictionary`2") is not null)
            {
                var genericTypes = type.GetGenericArguments();
                var keyType = genericTypes[0];
                if (keyType != typeof(string))
                {
                    throw new NotSupportedException("Non string key types are not supported");
                }

                return true;
            }

            return false;
        }
        
        // Operator handler delegate type - unified signature for all operators
        private delegate Expression OperatorHandler(Type type, object value, Expression propertyExp, BuildExpressionOptions options);

        // FrozenDictionary for O(1) case-insensitive operator dispatch (no allocations)
        private static readonly FrozenDictionary<string, OperatorHandler> OperatorHandlers = 
            new Dictionary<string, OperatorHandler>(StringComparer.OrdinalIgnoreCase)
            {
                ["in"] = In,
                ["not_in"] = NotIn,
                ["equal"] = Equals,
                ["not_equal"] = NotEquals,
                ["between"] = Between,
                ["not_between"] = NotBetween,
                ["less"] = LessThan,
                ["less_or_equal"] = LessThanOrEqual,
                ["greater"] = GreaterThan,
                ["greater_or_equal"] = GreaterThanOrEqual,
                ["begins_with"] = BeginsWith,
                ["not_begins_with"] = NotBeginsWith,
                ["contains"] = Contains,
                ["not_contains"] = NotContains,
                ["ends_with"] = EndsWith,
                ["not_ends_with"] = NotEndsWith,
                // Wrap property-only operators to match the unified signature
                ["is_empty"] = (_, _, prop, _) => IsEmpty(prop),
                ["is_not_empty"] = (_, _, prop, _) => IsNotEmpty(prop),
                ["is_null"] = (_, _, prop, _) => IsNull(prop),
                ["is_not_null"] = (_, _, prop, _) => IsNotNull(prop),
            }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

        private static Expression BuildOperatorExpression(Expression propertyExp, IFilterRule rule, BuildExpressionOptions options, Type type)
        {
            // Try built-in operators first (O(1) lookup, no allocation)
            if (OperatorHandlers.TryGetValue(rule.Operator, out var handler))
            {
                return handler(type, rule.Value, propertyExp, options);
            }

            // Custom operators support
            var operators = options.Operators;
            if (operators == null || !operators.Any())
            {
                throw new Exception($"Unknown expression operator: {rule.Operator}");
            }

            var customOperator = operators.FirstOrDefault(p => 
                string.Equals(p.Operator, rule.Operator, StringComparison.OrdinalIgnoreCase));
            
            if (customOperator != null)
            {
                return customOperator.GetExpression(type, rule, propertyExp, options);
            }

            throw new Exception($"Unknown expression operator: {rule.Operator}");
        }

     public static List<ConstantExpression> GetConstants(Type type, object value, bool isCollection, BuildExpressionOptions options)
        {
            if (type == typeof(System.DateOnly))
                type = typeof(DateTime);
            if (type == typeof(DateTime) && (options.ParseDatesAsUtc || ParseDatesAsUtc))
            {
                DateTime tDate;
                if (isCollection)
                {
                    if (!(value is string) && value is IEnumerable list)
                    {
                        var constants = new List<ConstantExpression>();

                        foreach (object item in list)
                        {
                            var date = DateTime.TryParse(item.ToString().Trim(), options.CultureInfo,
                                            DateTimeStyles.AdjustToUniversal, out tDate)
                                            ? (DateTime?)
                                                tDate
                                            : null;
                            constants.Add(Expression.Constant(date, type));
                        }

                        return constants;
                    }
                    else
                    {
                        var vals =
                            value.ToString().Split(new[] { ",", "[", "]", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                                .Where(p => !string.IsNullOrWhiteSpace(p))
                                .Select(
                                    p =>
                                        DateTime.TryParse(p.Trim(), options.CultureInfo,
                                            DateTimeStyles.AdjustToUniversal, out tDate)
                                            ? (DateTime?)
                                                tDate
                                            : null).Select(p =>
                                                Expression.Constant(p, type));
                        return vals.ToList();
                    }
                }
                else
                {
                    if (value is Array items) value = items.GetValue(0);
                    return new List<ConstantExpression>()
                    {
                        Expression.Constant(DateTime.TryParse(value.ToString().Trim(), options.CultureInfo,
                            DateTimeStyles.AdjustToUniversal, out tDate)
                            ? (DateTime?)
                                tDate
                            : null)
                    };
                }
            }
            else
            {
                if (isCollection)
                {
                    var tc = ReflectionHelpers.GetCachedConverter(type);
                    if (type == typeof(string))
                    {
                        if (!(value is string) && value is IEnumerable list)
                        {
                            var expressions = new List<ConstantExpression>();

                            foreach (object item in list)
                            {
                                expressions.Add(Expression.Constant(tc.ConvertFromString(null, options.CultureInfo, item.ToString()), type));
                            }

                            return expressions;
                        }
                        else
                        {
                            var bracketSplit = value.ToString().Split(new[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
                            var vals =
                                    bracketSplit.SelectMany(v => v.Split(new[] { ",", "\r\n" }, StringSplitOptions.None))
                                    .Select(p => tc.ConvertFromString(null, options.CultureInfo, p.Trim())).Select(p =>
                                        Expression.Constant(p, type));
                            return vals.Distinct().ToList();
                        }
                    }
                    else
                    {
                        if (!(value is string) && value is IEnumerable list)
                        {
                            var expressions = new List<ConstantExpression>();

                            foreach (object item in list)
                            {
                                expressions.Add(Expression.Constant(tc.ConvertFromString(null, options.CultureInfo, item.ToString()), type));
                            }

                            return expressions;
                        }
                        else
                        {
                            var vals =
                            value.ToString().Split(new[] { ",", "[", "]", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                                .Where(p => !string.IsNullOrWhiteSpace(p))
                                .Select(p => tc.ConvertFromString(null, options.CultureInfo, p.Trim())).Select(p =>
                                    Expression.Constant(p, type));
                            return vals.ToList();
                        }
                    }
                }
                else
                {
                    var tc = ReflectionHelpers.GetCachedConverter(type);
                    if (value is Array items) value = items.GetValue(0);

                    return new List<ConstantExpression>
                    {
                        Expression.Constant(tc.ConvertFromString(null, options.CultureInfo, value.ToString().Trim()))
                    };
                }
            }



        }

        #region Expression Types

        /// <summary>
        /// Checks expression for null as a short circuit
        /// </summary>
        /// <param name="propertyExp">Expression to be checked.</param>
        /// <returns>Expression determining null state</returns>
        public static Expression GetNullCheckExpression(Expression propertyExp)
        {
            var isNullable = !propertyExp.Type.IsValueType || Nullable.GetUnderlyingType(propertyExp.Type) != null;

            if (isNullable)
            {
                return Expression.NotEqual(propertyExp,
                    Expression.Constant(propertyExp.Type.GetDefaultValue(), propertyExp.Type));

            }
            return TrueExpression;
        }



        private static Expression IsNull(Expression propertyExp)
        {
            var isNullable = !propertyExp.Type.IsValueType || Nullable.GetUnderlyingType(propertyExp.Type) != null;

            if (isNullable)
            {
                var someValue = Expression.Constant(null, propertyExp.Type);

                Expression exOut = Expression.Equal(propertyExp, someValue);

                return exOut;
            }
            
            return FalseExpression;
        }

        private static Expression IsNotNull(Expression propertyExp)
        {
            return Expression.Not(IsNull(propertyExp));
        }

        private static Expression IsEmpty(Expression propertyExp)
        {
            var nullCheck = GetNullCheckExpression(propertyExp);

            Expression exOut;

            if (IsGenericList(propertyExp.Type))
            {
                exOut = Expression.Property(propertyExp, ReflectionHelpers.GetCachedProperty(propertyExp.Type, "Count")!);

                exOut = Expression.AndAlso(nullCheck, Expression.Equal(exOut, ZeroIntExpression));
            }
            else if (IsGuid(propertyExp.Type))
            {
                // For Guid, compare to Guid.Empty (need to convert cached expression for nullable types)
                var emptyValue = Nullable.GetUnderlyingType(propertyExp.Type) != null 
                    ? Expression.Constant(Guid.Empty, propertyExp.Type)
                    : GuidEmptyExpression;

                exOut = Expression.AndAlso(nullCheck, Expression.Equal(propertyExp, emptyValue));
            }
            else
            {
                exOut = Expression.Property(propertyExp, ReflectionHelpers.GetCachedProperty(typeof(string), "Length")!);

                exOut = Expression.AndAlso(nullCheck, Expression.Equal(exOut, ZeroIntExpression));
            }

            return exOut;
        }

        private static Expression IsNotEmpty(Expression propertyExp)
        {
            if (IsGuid(propertyExp.Type))
            {
                var emptyValue = Nullable.GetUnderlyingType(propertyExp.Type) != null 
                    ? Expression.Constant(Guid.Empty, propertyExp.Type)
                    : GuidEmptyExpression;
                return Expression.AndAlso(GetNullCheckExpression(propertyExp), Expression.NotEqual(propertyExp, emptyValue));
            }
            return Expression.Not(IsEmpty(propertyExp));
        }

        private static Expression Contains(Type type, object value, Expression propertyExp,
            BuildExpressionOptions options)
        {
            if (value is Array items) value = items.GetValue(0);
            var nullCheck = GetNullCheckExpression(propertyExp);
            MethodCallExpression? propertyExpString = null;

            if (ShouldConvertToString(propertyExp.Type, options))
            {
                propertyExpString = Expression.Call(propertyExp, ReflectionHelpers.GetCachedMethod(propertyExp.Type, "ToString", Type.EmptyTypes)!);
                type = typeof(string);
            }

            var targetExpr = (Expression?)propertyExpString ?? propertyExp;
            Expression exOut;

            if (IsDictionary(propertyExp.Type))
            {
                var method = ReflectionHelpers.GetCachedMethod(targetExpr.Type, "ContainsKey", new[] { type });
                var argument = Expression.Constant(value.ToString(), typeof(string));
                exOut = Expression.AndAlso(nullCheck, Expression.Call(targetExpr, method!, argument));
            }
            else if (ShouldUseOrdinalComparison(options) && type == typeof(string))
            {
                // Use Contains(value, StringComparison.OrdinalIgnoreCase)
                var strValue = Expression.Constant(value.ToString(), typeof(string));
                exOut = Expression.Call(
                    targetExpr,
                    ReflectionHelpers.StringContainsWithComparisonMethod,
                    strValue,
                    OrdinalIgnoreCaseConstant);
                exOut = Expression.AndAlso(nullCheck, exOut);
            }
            else
            {
                var method = ReflectionHelpers.GetCachedMethod(targetExpr.Type, "Contains", new[] { type });
                GetExpressionsOperands(options, targetExpr, value, out exOut, out var argument);
                exOut = Expression.AndAlso(nullCheck, Expression.Call(exOut, method!, argument));
            }

            return exOut;
        }

        // Helper methods for string operations with case sensitivity support
        // When UseOrdinalStringComparison is true, operands are returned unchanged
        // and callers use StringComparison overloads. Otherwise uses ToLower() for ORM compatibility.
        
        private static void GetExpressionsOperands(BuildExpressionOptions options, Expression property,
            object value, out Expression operand, out Expression argument)
        {
            var strValue = value.ToString()!;
            operand = property;
            
            // If case-insensitive and NOT using ordinal comparison, apply ToLower()
            if (!options.StringCaseSensitiveComparison && !options.UseOrdinalStringComparison)
            {
                strValue = strValue.ToLower();
                operand = Expression.Call(property, ReflectionHelpers.StringToLowerMethod);
            }
                
            argument = Expression.Constant(strValue, typeof(string));
        }
        
        private static void GetExpressionsOperands(BuildExpressionOptions options, Expression property,
            Expression value, out Expression operand, out Expression argument)
        {
            operand = property;
            
            // If case-insensitive and NOT using ordinal comparison, apply ToLower()
            if (!options.StringCaseSensitiveComparison && !options.UseOrdinalStringComparison)
            {
                value = Expression.Call(value, ReflectionHelpers.StringToLowerMethod);
                operand = Expression.Call(property, ReflectionHelpers.StringToLowerMethod);
            }

            argument = value;
        }
        
        /// <summary>
        /// Determines if we should use StringComparison.OrdinalIgnoreCase for string operations.
        /// </summary>
        private static bool ShouldUseOrdinalComparison(BuildExpressionOptions options)
        {
            return !options.StringCaseSensitiveComparison && options.UseOrdinalStringComparison;
        }

        private static Expression NotContains(Type type, object value, Expression propertyExp,
            BuildExpressionOptions options)
        {
            return Expression.Not(Contains(type, value, propertyExp, options));
        }

        private static Expression EndsWith(Type type, object value, Expression propertyExp,
            BuildExpressionOptions options)
        {
            if (value is Array items) value = items.GetValue(0);

            var nullCheck = GetNullCheckExpression(propertyExp);
            MethodCallExpression? propertyExpString = null;

            if (ShouldConvertToString(propertyExp.Type, options))
            {
                propertyExpString = Expression.Call(propertyExp, ReflectionHelpers.GetCachedMethod(propertyExp.Type, "ToString", Type.EmptyTypes)!);
                type = typeof(string);
            }
            var targetExpr = (Expression?)propertyExpString ?? propertyExp;

            Expression exOut;
            if (ShouldUseOrdinalComparison(options))
            {
                // Use EndsWith(value, StringComparison.OrdinalIgnoreCase)
                var strValue = Expression.Constant(value.ToString(), typeof(string));
                exOut = Expression.Call(
                    targetExpr,
                    ReflectionHelpers.StringEndsWithComparisonMethod,
                    strValue,
                    OrdinalIgnoreCaseConstant);
                exOut = Expression.AndAlso(nullCheck, exOut);
            }
            else
            {
                var method = ReflectionHelpers.GetCachedMethod(targetExpr.Type, "EndsWith", new[] { type });
                GetExpressionsOperands(options, targetExpr, value, out exOut, out var argument);
                exOut = Expression.AndAlso(nullCheck, Expression.Call(exOut, method!, argument));
            }
            return exOut;
        }

        private static Expression NotEndsWith(Type type, object value, Expression propertyExp,
            BuildExpressionOptions buildExpressionOptions)
        {
            return Expression.Not(EndsWith(type, value, propertyExp, buildExpressionOptions));
        }

        private static Expression BeginsWith(Type type, object value, Expression propertyExp,
            BuildExpressionOptions options)
        {
            if (value is Array items) value = items.GetValue(0);

            var nullCheck = GetNullCheckExpression(propertyExp);

            MethodCallExpression? propertyExpString = null;

            if (ShouldConvertToString(propertyExp.Type, options))
            {
                propertyExpString = Expression.Call(propertyExp, ReflectionHelpers.GetCachedMethod(propertyExp.Type, "ToString", Type.EmptyTypes)!);
                type = typeof(string);
            }
            var targetExpr = (Expression?)propertyExpString ?? propertyExp;

            Expression exOut;
            if (ShouldUseOrdinalComparison(options))
            {
                // Use StartsWith(value, StringComparison.OrdinalIgnoreCase)
                var strValue = Expression.Constant(value.ToString(), typeof(string));
                exOut = Expression.Call(
                    targetExpr,
                    ReflectionHelpers.StringStartsWithComparisonMethod,
                    strValue,
                    OrdinalIgnoreCaseConstant);
                exOut = Expression.AndAlso(nullCheck, exOut);
            }
            else
            {
                var method = ReflectionHelpers.GetCachedMethod(targetExpr.Type, "StartsWith", new[] { type });
                GetExpressionsOperands(options, targetExpr, value, out exOut, out var argument);
                exOut = Expression.AndAlso(nullCheck, Expression.Call(exOut, method!, argument));
            }
            return exOut;
        }

        private static Expression NotBeginsWith(Type type, object value, Expression propertyExp,
            BuildExpressionOptions options)
        {
            return Expression.Not(BeginsWith(type, value, propertyExp, options));
        }



        private static Expression NotEquals(Type type, object value, Expression propertyExp, BuildExpressionOptions options)
        {
            return Expression.Not(Equals(type, value, propertyExp, options));
        }

        // Using System.DateOnly available in .NET 6+
        // Cached constant for StringComparison.OrdinalIgnoreCase
        private static readonly ConstantExpression OrdinalIgnoreCaseConstant = 
            Expression.Constant(StringComparison.OrdinalIgnoreCase, typeof(StringComparison));

        private static Expression Equals(Type type, object value, Expression propertyExp, BuildExpressionOptions options)
        {
            Expression someValue = GetConstants(type, value, false, options).First();

            Expression exOut;    
            if (type == typeof(string))
            {
                var nullCheck = GetNullCheckExpression(propertyExp);

                MethodCallExpression? propertyExpString = null;

                if (ShouldConvertToString(propertyExp.Type, options))
                {
                    propertyExpString = Expression.Call(propertyExp, ReflectionHelpers.GetCachedMethod(propertyExp.Type, "ToString", Type.EmptyTypes)!);
                }

                var targetProperty = (Expression?)propertyExpString ?? propertyExp;
                
                if (ShouldUseOrdinalComparison(options))
                {
                    // Use String.Equals(property, value, StringComparison.OrdinalIgnoreCase)
                    exOut = Expression.Call(
                        ReflectionHelpers.StringEqualsStaticMethod,
                        targetProperty,
                        someValue,
                        OrdinalIgnoreCaseConstant);
                    exOut = Expression.AndAlso(nullCheck, exOut);
                }
                else
                {
                    GetExpressionsOperands(options, targetProperty, someValue, out exOut, out var argument);
                    exOut = Expression.AndAlso(nullCheck, Expression.Equal(exOut, argument));
                }
            }
            else if (type == typeof(System.DateOnly))
            {
                if (Nullable.GetUnderlyingType(propertyExp.Type) != null)
                {
                    exOut = Expression.Property(propertyExp, ReflectionHelpers.GetCachedProperty(typeof(DateTime?), "Value")!);
                    exOut = Expression.Equal(
                        Expression.Property(exOut, ReflectionHelpers.GetCachedProperty(typeof(DateTime), "Date")!),
                        Expression.Convert(someValue, typeof(DateTime)));

                    exOut = Expression.AndAlso(GetNullCheckExpression(propertyExp), exOut);
                }
                else
                {
                    exOut = Expression.Equal(
                        Expression.Property(propertyExp, ReflectionHelpers.GetCachedProperty(typeof(DateTime), "Date")!),
                        Expression.Convert(someValue, propertyExp.Type));
                }
            }
            else
            {
                PerformCasting(propertyExp, someValue, type, out propertyExp, out someValue);
                exOut = Expression.Equal(propertyExp, someValue);
            }

            return exOut;
        }

        private static Expression LessThan(Type type, object value, Expression propertyExp, BuildExpressionOptions options)
        {
            Expression someValue = GetConstants(type, value, false, options).First();
            PerformCasting(propertyExp, someValue, type, out propertyExp, out someValue);
            Expression exOut = Expression.LessThan(propertyExp, someValue);
            
            return exOut;
        }

        private static Expression LessThanOrEqual(Type type, object value, Expression propertyExp, BuildExpressionOptions options)
        {
            var dtOnly = IsDateOnly(type);
            if (dtOnly)
                value = ShiftOneDayForDate(value, 0, options.CultureInfo).First();

            Expression someValue = GetConstants(type, value, false, options).First();
            PerformCasting(propertyExp, someValue, type, out propertyExp, out someValue);

            return dtOnly 
                ? Expression.LessThan(propertyExp, someValue)
                : Expression.LessThanOrEqual(propertyExp, someValue);
        }

        private static Expression GreaterThan(Type type, object value, Expression propertyExp, BuildExpressionOptions options)
        {
            var dtOnly = IsDateOnly(type);
            if (dtOnly)
                value = ShiftOneDayForDate(value, 0, options.CultureInfo).First();

            Expression someValue = GetConstants(type, value, false, options).First();
            PerformCasting(propertyExp, someValue, type, out propertyExp, out someValue);

            return dtOnly
                ? Expression.GreaterThanOrEqual(propertyExp, someValue)
                : Expression.GreaterThan(propertyExp, someValue);
        }   

        private static Expression GreaterThanOrEqual(Type type, object value, Expression propertyExp, BuildExpressionOptions options)
        {
            Expression someValue = GetConstants(type, value, false, options).First();
            PerformCasting(propertyExp, someValue, type, out propertyExp, out someValue);
            Expression exOut = Expression.GreaterThanOrEqual(propertyExp, someValue);
            return exOut;
        }

        private static Expression Between(Type type, object value, Expression propertyExp, BuildExpressionOptions options)
        {
            if (IsDateOnly(type)) // value 2 must be increased by 1 day to be inclusive regarding the time portion in the property.
            {
                value = ShiftOneDayForDate(value, 1, options.CultureInfo);
            }
            var someValue = GetConstants(type, value, true, options);            
            
            PerformCasting(propertyExp, someValue[0], type, out var castedProperty, out var greaterThanValue);
            Expression exBelow = Expression.GreaterThanOrEqual(castedProperty, greaterThanValue);
            PerformCasting(propertyExp, someValue[1], type, out castedProperty, out var lessThanValue);
            Expression exAbove = Expression.LessThanOrEqual(castedProperty, lessThanValue);

            return Expression.And(exBelow, exAbove);


        }

        private static Expression NotBetween(Type type, object value, Expression propertyExp, BuildExpressionOptions options)
        {
            return Expression.Not(Between(type, value, propertyExp, options));
        }

        private static Expression In(Type type, object value, Expression propertyExp, BuildExpressionOptions options)
        {
            var someValues = GetConstants(type, value, true, options);

            var nullCheck = GetNullCheckExpression(propertyExp);
            
            if (IsGenericList(propertyExp.Type))
            {
                var genericType = propertyExp.Type.GetGenericArguments().First();
                var method = ReflectionHelpers.GetCachedMethod(propertyExp.Type, "Contains", new[] { genericType });
                Expression exOut = default!;
                
                bool isDtOnly = type == typeof(System.DateOnly);
                MethodCallExpression? listCallExp = null;

                if (isDtOnly)
                {
                    MethodCallExpression? whereCallExp = null;
                    var isNullable = Nullable.GetUnderlyingType(genericType) != null;
                    
                    if (isNullable)
                    {
                        var paramExp1 = Expression.Parameter(typeof(DateTime?), "d");
                        var memberExpression1 = GetNullCheckExpression(paramExp1);
                        var lambdaExp1 = Expression.Lambda(memberExpression1, paramExp1);
                        whereCallExp = Expression.Call(ReflectionHelpers.WhereMethod.MakeGenericMethod(typeof(DateTime?)), propertyExp, lambdaExp1);
                    }

                    var paramExp = Expression.Parameter(genericType, "d");
                    var memberExpression = isNullable
                        ? Expression.Property(Expression.Property(paramExp, ReflectionHelpers.GetCachedProperty(genericType, "Value")!), ReflectionHelpers.GetCachedProperty(typeof(DateTime), "Date")!)
                        : Expression.Property(paramExp, ReflectionHelpers.GetCachedProperty(genericType, "Date")!);
                    var lambdaExp = Expression.Lambda(memberExpression, paramExp);

                    var selectCallExp = Expression.Call(ReflectionHelpers.SelectMethod.MakeGenericMethod(genericType, typeof(DateTime)), whereCallExp ?? propertyExp, lambdaExp);
                    listCallExp = Expression.Call(ReflectionHelpers.ToListMethod.MakeGenericMethod(typeof(DateTime)), selectCallExp);
                    exOut = Expression.Call(ReflectionHelpers.ContainsMethod.MakeGenericMethod(typeof(DateTime)), listCallExp, Expression.Convert(someValues[0], typeof(DateTime)));
                }
                else
                    exOut = Expression.Call(propertyExp, method, Expression.Convert(someValues[0], genericType));

           
                var counter = 1;

                while (counter < someValues.Count)
                {
                    MethodCallExpression methodCall = null;
                    if (isDtOnly)
                    {
                        methodCall = Expression.Call(
                            ReflectionHelpers.ContainsMethod.MakeGenericMethod(typeof(DateTime)),
                            listCallExp,
                            Expression.Convert(someValues[counter], typeof(DateTime)));
                    }
                    else
                    {
                        methodCall = Expression.Call(propertyExp, method, Expression.Convert(someValues[counter], genericType));
                    }

                    exOut = Expression.Or(exOut, methodCall);
                    counter++;
                }
                

                return Expression.AndAlso(nullCheck, exOut);
            }
            else
            {
                Expression exOut;

                // Threshold for using HashSet-based Contains instead of chained Or expressions
                // HashSet.Contains is O(1) vs O(n) for Or chains
                const int HashSetThreshold = 10;
                
                if (someValues.Count > 1)
                {
                    if (type == typeof(string))
                    {
                        Expression? propertyExpString = null;
                        if (ShouldConvertToString(propertyExp.Type, options))
                        {
                            propertyExpString = Expression.Call(propertyExp, ReflectionHelpers.GetCachedMethod(propertyExp.Type, "ToString", Type.EmptyTypes)!);
                        }

                        var property = propertyExpString ?? propertyExp;
                        
                        if (ShouldUseOrdinalComparison(options))
                        {
                            // Use String.Equals with StringComparison.OrdinalIgnoreCase
                            exOut = Expression.Call(
                                ReflectionHelpers.StringEqualsStaticMethod,
                                property,
                                someValues[0],
                                OrdinalIgnoreCaseConstant);
                            var counter = 1;
                            while (counter < someValues.Count)
                            {
                                var equalsExpr = Expression.Call(
                                    ReflectionHelpers.StringEqualsStaticMethod,
                                    property,
                                    someValues[counter],
                                    OrdinalIgnoreCaseConstant);
                                exOut = Expression.Or(exOut, equalsExpr);
                                counter++;
                            }
                        }
                        else
                        {
                            // Use ToLower() approach for ORM compatibility
                            GetExpressionsOperands(options, property, someValues[0], out var leftOperand, out var argument);
                            exOut = Expression.Equal(leftOperand, argument);
                            var counter = 1;
                            while (counter < someValues.Count)
                            {
                                GetExpressionsOperands(options, property, someValues[counter], out leftOperand, out argument);
                                exOut = Expression.Or(exOut, Expression.Equal(leftOperand, argument));
                                counter++;
                            }
                        }
                    }
                    else if (someValues.Count >= HashSetThreshold)
                    {
                        // Use HashSet.Contains for O(1) lookup with large value sets
                        exOut = CreateHashSetContainsExpression(propertyExp, someValues, type);
                    }
                    else
                    {
                        // Use chained Or for small value sets (lower overhead)
                        PerformCasting(propertyExp, someValues[0], type, out var castedProperty, out var someValue);
                        exOut = Expression.Equal(castedProperty, someValue);
                        var counter = 1;
                        while (counter < someValues.Count)
                        {
                            PerformCasting(propertyExp, someValues[counter], type, out castedProperty, out someValue);
                            exOut = Expression.Or(exOut,
                                Expression.Equal(castedProperty, someValue));
                            counter++;
                        }
                    }
                }
                else
                {
                    if (type == typeof(string))
                    {
                        Expression? propertyExpString = null;
                        if (ShouldConvertToString(propertyExp.Type, options))
                        {
                            propertyExpString = Expression.Call(propertyExp, ReflectionHelpers.GetCachedMethod(propertyExp.Type, "ToString", Type.EmptyTypes)!);
                        }
                        var property = propertyExpString ?? propertyExp;
                        
                        if (ShouldUseOrdinalComparison(options))
                        {
                            // Use String.Equals with StringComparison.OrdinalIgnoreCase
                            exOut = Expression.Call(
                                ReflectionHelpers.StringEqualsStaticMethod,
                                property,
                                someValues.First(),
                                OrdinalIgnoreCaseConstant);
                        }
                        else
                        {
                            GetExpressionsOperands(options, property, someValues.First(), out exOut, out var argument);
                            exOut = Expression.Equal(exOut, argument);
                        }
                    }
                    else
                    {
                        exOut = Equals(type, someValues.First(), propertyExp, options);
                    }
                }


                return Expression.AndAlso(nullCheck, exOut);
            }
        }

        private static Expression NotIn(Type type, object value, Expression propertyExp, BuildExpressionOptions options)
        {
            return Expression.Not(In(type, value, propertyExp, options));
        }

        /// <summary>
        /// Creates a HashSet.Contains expression for O(1) lookup with large value sets.
        /// Generates: new HashSet&lt;T&gt;(values).Contains(property)
        /// </summary>
        private static Expression CreateHashSetContainsExpression(
            Expression propertyExp, 
            List<ConstantExpression> someValues, 
            Type type)
        {
            // Get the actual property type (handle nullable)
            var propertyType = propertyExp.Type;
            var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
            
            // Create the HashSet<T> type
            var hashSetType = typeof(HashSet<>).MakeGenericType(underlyingType);
            
            // Extract values from constant expressions and create the HashSet
            var values = someValues.Select(c => c.Value).ToArray();
            var hashSet = Activator.CreateInstance(hashSetType);
            var addMethod = hashSetType.GetMethod("Add")!;
            foreach (var v in values)
            {
                if (v != null)
                {
                    // Convert value to target type to handle type mismatches (e.g., Int32 filter value -> UInt16 property)
                    var convertedValue = Convert.ChangeType(v, underlyingType);
                    addMethod.Invoke(hashSet, new[] { convertedValue });
                }
            }
            
            // Create a constant expression for the HashSet
            var hashSetConstant = Expression.Constant(hashSet, hashSetType);
            
            // Get the Contains method
            var containsMethod = hashSetType.GetMethod("Contains", new[] { underlyingType })!;
            
            // If property is nullable, need to access .Value for the Contains call
            Expression propertyToCheck = propertyExp;
            if (Nullable.GetUnderlyingType(propertyType) != null)
            {
                propertyToCheck = Expression.Property(propertyExp, "Value");
            }
            
            // Generate: hashSet.Contains(property)
            return Expression.Call(hashSetConstant, containsMethod, propertyToCheck);
        }

        #endregion

        public static bool IsGenericList(this Type o)
        {
            var isGenericList = false;

            var oType = o;

            if (oType.IsGenericType && ((oType.GetGenericTypeDefinition() == typeof(IEnumerable<>)) ||
                                        (oType.GetGenericTypeDefinition() == typeof(ICollection<>)) ||
                                        (oType.GetGenericTypeDefinition() == typeof(List<>)) || 
                                        (oType.GetGenericTypeDefinition() == typeof(HashSet<>))))
                isGenericList = true;

            return isGenericList;
        }

        private static bool IsGuid(this Type o) => o.UnderlyingSystemType.Name == "Guid" || Nullable.GetUnderlyingType(o)?.Name == "Guid";

        private static bool IsTypeFitForToStringConversion(this Type o) => IsGuid(o) || o == typeof(object) || o.IsEnum;

        private static bool ShouldConvertToString(this Type o, BuildExpressionOptions options) =>
            IsTypeFitForToStringConversion(o) && options.RequireExplicitToStringConversion;

        private static object GetDefaultValue(this Type type)
        {
            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
        }
        
        private static void PerformCasting(Expression propertyExp, Expression constant, Type type, out Expression castedProperty, out Expression castedConstant)
        {
            castedProperty = propertyExp;
            castedConstant = constant;
            // if our type is a super class of the compared type, downcast our type
            // for example compare object to int
            if (type.IsSubclassOf(propertyExp.Type))
            {
                castedProperty = Expression.Convert(propertyExp, type);
            }
            // support nullables
            else // int is not a subclass of nullable int
            {
                castedConstant = Expression.Convert(constant, propertyExp.Type);
            }
        }

        private static bool IsDateOnly(Type type)
            => type.Name == "DateOnly";

        private static List<string> ShiftOneDayForDate(object value, byte index, CultureInfo cultureInfo)
        {
            List<string> newValues = new List<string>();
            byte i = 0;
            foreach (var item in value as IEnumerable<string>)
            {
                if (i == index)
                {
                    newValues.Add(DateTime.Parse(item).AddDays(1).Date.ToString(cultureInfo));
                    break;
                }
                else
                    newValues.Add(item);
                i++;
            }

            return newValues;
        }
    }
}
