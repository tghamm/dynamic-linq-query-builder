using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
            var expression = BuildExpressionLambda<T>(filterRule, options, out parsedQuery);

            if (expression == null)
            {
                return _ => true;
            }

            return expression.Compile();
        }

        /// <summary>
        /// Builds an expression lambda for the filter rule.
        /// </summary>
        /// <typeparam name="T">The generic type of the input object to test.</typeparam>
        /// <param name="filterRule">The filter rule.</param>
        /// <param name="parsedQuery">The parsed query.</param>
        /// <param name="options">The options to use when building the expression</param>
        /// <returns>An expression lambda that implements the filter rule</returns>
        public static Expression<Func<T, bool>> BuildExpressionLambda<T>(this IFilterRule filterRule, BuildExpressionOptions options, out string parsedQuery)
        {
            if (filterRule == null)
            {
                parsedQuery = "";
                return null;
            }

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
                    expressionTree = rule.Condition.ToLower() == "or"
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
                case "string":
                    type = typeof(string);
                    break;
                case "date":
                case "datetime":
                    type = typeof(DateTime);
                    break;
                case "boolean":
                    type = typeof(bool);
                    break;
                case "guid":
                    type = typeof(Guid);
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
                var property = expression.Type.GetProperty(propertyName);
                expression = Expression.Property(expression, property);

                var propertyType = property.PropertyType;
                var enumerable = propertyType.GetInterface("IEnumerable`1");
                if (propertyType != typeof(string) && enumerable != null)
                {
                    var elementType = enumerable.GetGenericArguments()[0];
                    var predicateFnType = typeof(Func<,>).MakeGenericType(elementType, typeof(bool));
                    var parameterExpression = Expression.Parameter(elementType);

                    Expression body = BuildNestedExpression(parameterExpression, propertyCollectionEnumerator, rule, options, type);
                    var predicate = Expression.Lambda(predicateFnType, body, parameterExpression);

                    var queryable = Expression.Call(typeof(Queryable), "AsQueryable", new[] { elementType }, expression);


                    return Expression.Call(
                        typeof(Queryable),
                        "Any",
                        new[] { elementType },
                        queryable,
                        predicate
                    );
                }
            }

            return BuildOperatorExpression(expression, rule, options, type);
        }

        private static Expression BuildOperatorExpression(Expression propertyExp, IFilterRule rule, BuildExpressionOptions options, Type type)
        {
            Expression expression;
            string oper = rule.Operator.ToLower();

            switch (oper)
            {
                case "in":
                    expression = In(type, rule.Value, propertyExp, options);
                    break;
                case "not_in":
                    expression = NotIn(type, rule.Value, propertyExp, options);
                    break;
                case "equal":
                    expression = Equals(type, rule.Value, propertyExp, options);
                    break;
                case "not_equal":
                    expression = NotEquals(type, rule.Value, propertyExp, options);
                    break;
                case "between":
                    expression = Between(type, rule.Value, propertyExp, options);
                    break;
                case "not_between":
                    expression = NotBetween(type, rule.Value, propertyExp, options);
                    break;
                case "less":
                    expression = LessThan(type, rule.Value, propertyExp, options);
                    break;
                case "less_or_equal":
                    expression = LessThanOrEqual(type, rule.Value, propertyExp, options);
                    break;
                case "greater":
                    expression = GreaterThan(type, rule.Value, propertyExp, options);
                    break;
                case "greater_or_equal":
                    expression = GreaterThanOrEqual(type, rule.Value, propertyExp, options);
                    break;
                case "begins_with":
                    expression = BeginsWith(type, rule.Value, propertyExp);
                    break;
                case "not_begins_with":
                    expression = NotBeginsWith(type, rule.Value, propertyExp);
                    break;
                case "contains":
                    expression = Contains(type, rule.Value, propertyExp);
                    break;
                case "not_contains":
                    expression = NotContains(type, rule.Value, propertyExp);
                    break;
                case "ends_with":
                    expression = EndsWith(type, rule.Value, propertyExp);
                    break;
                case "not_ends_with":
                    expression = NotEndsWith(type, rule.Value, propertyExp);
                    break;
                case "is_empty":
                    expression = IsEmpty(propertyExp);
                    break;
                case "is_not_empty":
                    expression = IsNotEmpty(propertyExp);
                    break;
                case "is_null":
                    expression = IsNull(propertyExp);
                    break;
                case "is_not_null":
                    expression = IsNotNull(propertyExp);
                    break;
                default:
                    //custom operators support
                    var operators = options.Operators;
                    if (operators == null || operators.Count() <= 0)
                        throw new Exception($"Unknown expression operator: {rule.Operator}");

                    var customOperator = (from p in operators where p.Operator.ToLower() == oper select p).FirstOrDefault();
                    if (customOperator != null)
                    {
                        expression = customOperator.GetExpression(type, rule, propertyExp, options);
                    }
                    else
                    {
                        throw new Exception($"Unknown expression operator: {rule.Operator}");
                    }
                    break;
            }

            return expression;
        }

        public static List<ConstantExpression> GetConstants(Type type, object value, bool isCollection, BuildExpressionOptions options)
        {
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
                    var tc = TypeDescriptor.GetConverter(type);
                    if (type == typeof(string))
                    {
                        if (!(value is string) && value is IEnumerable list)
                        {
                            var expressions = new List<ConstantExpression>();

                            foreach (object item in list)
                            {
                                expressions.Add(Expression.Constant(tc.ConvertFromString(item.ToString()), type));
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
                                expressions.Add(Expression.Constant(tc.ConvertFromString(item.ToString()), type));
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
                    var tc = TypeDescriptor.GetConverter(type);
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
            return Expression.Equal(Expression.Constant(true, typeof(bool)),
                Expression.Constant(true, typeof(bool)));
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
            return Expression.Equal(Expression.Constant(true, typeof(bool)),
                Expression.Constant(false, typeof(bool)));
        }

        private static Expression IsNotNull(Expression propertyExp)
        {
            return Expression.Not(IsNull(propertyExp));
        }

        private static Expression IsEmpty(Expression propertyExp)
        {
            var someValue = Expression.Constant(0, typeof(int));

            var nullCheck = GetNullCheckExpression(propertyExp);

            Expression exOut;

            if (IsGenericList(propertyExp.Type))
            {
                exOut = Expression.Property(propertyExp, propertyExp.Type.GetProperty("Count"));

                exOut = Expression.AndAlso(nullCheck, Expression.Equal(exOut, someValue));
            }
            else if (IsGuid(propertyExp.Type))
            {
                someValue = Expression.Constant(Guid.Empty, propertyExp.Type);

                exOut = Expression.AndAlso(nullCheck, Expression.Equal(propertyExp, someValue));
            }
            else
            {
                exOut = Expression.Property(propertyExp, typeof(string).GetProperty("Length"));

                exOut = Expression.AndAlso(nullCheck, Expression.Equal(exOut, someValue));
            }

            return exOut;
        }

        private static Expression IsNotEmpty(Expression propertyExp)
            => IsGuid(propertyExp.Type)
                ? Expression.AndAlso(GetNullCheckExpression(propertyExp), Expression.NotEqual(propertyExp, Expression.Constant(Guid.Empty, propertyExp.Type)))
                : Expression.Not(IsEmpty(propertyExp));

        private static Expression Contains(Type type, object value, Expression propertyExp)
        {
            if (value is Array items) value = items.GetValue(0);

            var someValue = Expression.Constant(value.ToString().ToLower(), typeof(string));

            var nullCheck = GetNullCheckExpression(propertyExp);

            MethodCallExpression propertyExpString = null;

            if (IsGuid(propertyExp.Type))
            {
                propertyExpString = Expression.Call(propertyExp, propertyExp.Type.GetMethod("ToString", Type.EmptyTypes));
                type = typeof(string);
            }
            var method = (propertyExpString ?? propertyExp).Type.GetMethod("Contains", new[] { type });

            Expression exOut = Expression.Call((propertyExpString ?? propertyExp), typeof(string).GetMethod("ToLower", Type.EmptyTypes));

            exOut = Expression.AndAlso(nullCheck, Expression.Call(exOut, method, someValue));

            return exOut;
        }

        private static Expression NotContains(Type type, object value, Expression propertyExp)
        {
            return Expression.Not(Contains(type, value, propertyExp));
        }

        private static Expression EndsWith(Type type, object value, Expression propertyExp)
        {
            if (value is Array items) value = items.GetValue(0);

            var someValue = Expression.Constant(value.ToString().ToLower(), typeof(string));

            var nullCheck = GetNullCheckExpression(propertyExp);

            MethodCallExpression propertyExpString = null;

            if (IsGuid(propertyExp.Type))
            {
                propertyExpString = Expression.Call(propertyExp, propertyExp.Type.GetMethod("ToString", Type.EmptyTypes));
                type = typeof(string);
            }
            var method = (propertyExpString ?? propertyExp).Type.GetMethod("EndsWith", new[] { type });

            Expression exOut = Expression.Call((propertyExpString ?? propertyExp), typeof(string).GetMethod("ToLower", Type.EmptyTypes));

            exOut = Expression.AndAlso(nullCheck, Expression.Call(exOut, method, someValue));

            return exOut;
        }

        private static Expression NotEndsWith(Type type, object value, Expression propertyExp)
        {
            return Expression.Not(EndsWith(type, value, propertyExp));
        }

        private static Expression BeginsWith(Type type, object value, Expression propertyExp)
        {
            if (value is Array items) value = items.GetValue(0);

            var someValue = Expression.Constant(value.ToString().ToLower(), typeof(string));

            var nullCheck = GetNullCheckExpression(propertyExp);

            MethodCallExpression propertyExpString = null;

            if (IsGuid(propertyExp.Type))
            {
                propertyExpString = Expression.Call(propertyExp, propertyExp.Type.GetMethod("ToString", Type.EmptyTypes));
                type = typeof(string);
            }
            var method = (propertyExpString ?? propertyExp).Type.GetMethod("StartsWith", new[] { type });

            Expression exOut = Expression.Call(propertyExpString ?? propertyExp, typeof(string).GetMethod("ToLower", Type.EmptyTypes));

            exOut = Expression.AndAlso(nullCheck, Expression.Call(exOut, method, someValue));

            return exOut;
        }

        private static Expression NotBeginsWith(Type type, object value, Expression propertyExp)
        {
            return Expression.Not(BeginsWith(type, value, propertyExp));
        }



        private static Expression NotEquals(Type type, object value, Expression propertyExp, BuildExpressionOptions options)
        {
            return Expression.Not(Equals(type, value, propertyExp, options));
        }



        private static Expression Equals(Type type, object value, Expression propertyExp, BuildExpressionOptions options)
        {
            Expression someValue = GetConstants(type, value, false, options).First();

            Expression exOut;
            if (type == typeof(string))
            {
                var nullCheck = GetNullCheckExpression(propertyExp);

                MethodCallExpression propertyExpString = null;

                if (IsGuid(propertyExp.Type))
                {
                    propertyExpString = Expression.Call(propertyExp, propertyExp.Type.GetMethod("ToString", Type.EmptyTypes));
                    type = typeof(string);
                }

                exOut = Expression.Call((propertyExpString ?? propertyExp), typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                someValue = Expression.Call(someValue, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                exOut = Expression.AndAlso(nullCheck, Expression.Equal(exOut, someValue));
            }
            else
            {
                exOut = Expression.Equal(propertyExp, Expression.Convert(someValue, propertyExp.Type));
            }

            return exOut;


        }

        private static Expression LessThan(Type type, object value, Expression propertyExp, BuildExpressionOptions options)
        {
            var someValue = GetConstants(type, value, false, options).First();

            Expression exOut = Expression.LessThan(propertyExp, Expression.Convert(someValue, propertyExp.Type));


            return exOut;


        }

        private static Expression LessThanOrEqual(Type type, object value, Expression propertyExp, BuildExpressionOptions options)
        {
            var someValue = GetConstants(type, value, false, options).First();

            Expression exOut = Expression.LessThanOrEqual(propertyExp, Expression.Convert(someValue, propertyExp.Type));


            return exOut;


        }

        private static Expression GreaterThan(Type type, object value, Expression propertyExp, BuildExpressionOptions options)
        {

            var someValue = GetConstants(type, value, false, options).First();



            Expression exOut = Expression.GreaterThan(propertyExp, Expression.Convert(someValue, propertyExp.Type));


            return exOut;


        }

        private static Expression GreaterThanOrEqual(Type type, object value, Expression propertyExp, BuildExpressionOptions options)
        {
            var someValue = GetConstants(type, value, false, options).First();

            Expression exOut = Expression.GreaterThanOrEqual(propertyExp, Expression.Convert(someValue, propertyExp.Type));


            return exOut;


        }

        private static Expression Between(Type type, object value, Expression propertyExp, BuildExpressionOptions options)
        {
            var someValue = GetConstants(type, value, true, options);


            Expression exBelow = Expression.GreaterThanOrEqual(propertyExp, Expression.Convert(someValue[0], propertyExp.Type));
            Expression exAbove = Expression.LessThanOrEqual(propertyExp, Expression.Convert(someValue[1], propertyExp.Type));

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
                var method = propertyExp.Type.GetMethod("Contains", new[] { genericType });
                Expression exOut;

                if (someValues.Count > 1)
                {
                    exOut = Expression.Call(propertyExp, method, Expression.Convert(someValues[0], genericType));
                    var counter = 1;
                    while (counter < someValues.Count)
                    {
                        exOut = Expression.Or(exOut,
                            Expression.Call(propertyExp, method, Expression.Convert(someValues[counter], genericType)));
                        counter++;
                    }
                }
                else
                {
                    exOut = Expression.Call(propertyExp, method, Expression.Convert(someValues.First(), genericType));
                }


                return Expression.AndAlso(nullCheck, exOut);
            }
            else
            {
                Expression exOut;

                if (someValues.Count > 1)
                {
                    if (type == typeof(string))
                    {
                        Expression propertyExpString = null;
                        if (IsGuid(propertyExp.Type))
                        {
                            propertyExpString = Expression.Call(propertyExp, propertyExp.Type.GetMethod("ToString", Type.EmptyTypes));
                        }
                        exOut = Expression.Call(propertyExpString ?? propertyExp, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                        var somevalue = Expression.Call(someValues[0], typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                        exOut = Expression.Equal(exOut, somevalue);
                        var counter = 1;
                        while (counter < someValues.Count)
                        {

                            var nextvalue = Expression.Call(someValues[counter], typeof(string).GetMethod("ToLower", Type.EmptyTypes));

                            exOut = Expression.Or(exOut,
                                Expression.Equal(
                                    Expression.Call(propertyExpString ?? propertyExp, typeof(string).GetMethod("ToLower", Type.EmptyTypes)),
                                    nextvalue));
                            counter++;
                        }
                    }
                    else
                    {
                        exOut = Expression.Equal(propertyExp, Expression.Convert(someValues[0], propertyExp.Type));
                        var counter = 1;
                        while (counter < someValues.Count)
                        {
                            exOut = Expression.Or(exOut,
                                Expression.Equal(propertyExp, Expression.Convert(someValues[counter], propertyExp.Type)));
                            counter++;
                        }
                    }
                }
                else
                {
                    if (type == typeof(string))
                    {
                        Expression propertyExpString = null;
                        if (IsGuid(propertyExp.Type))
                        {
                            propertyExpString = Expression.Call(propertyExp, propertyExp.Type.GetMethod("ToString", Type.EmptyTypes));
                        }

                        exOut = Expression.Call(propertyExpString ?? propertyExp, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                        var somevalue = Expression.Call(someValues.First(), typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                        exOut = Expression.Equal(exOut, somevalue);
                    }
                    else
                    {
                        exOut = Expression.Equal(propertyExp, Expression.Convert(someValues.First(), propertyExp.Type));
                    }
                }


                return Expression.AndAlso(nullCheck, exOut);
            }
        }

        private static Expression NotIn(Type type, object value, Expression propertyExp, BuildExpressionOptions options)
        {
            return Expression.Not(In(type, value, propertyExp, options));
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

        public static bool IsGuid(this Type o) => o.UnderlyingSystemType.Name == "Guid" || Nullable.GetUnderlyingType(o)?.Name == "Guid";

        private static object GetDefaultValue(this Type type)
        {
            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
        }

    }
}
