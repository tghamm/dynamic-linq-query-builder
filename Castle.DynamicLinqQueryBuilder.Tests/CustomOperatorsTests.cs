using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Security;
using NUnit.Framework;

namespace Castle.DynamicLinqQueryBuilder.Tests
{
      

    public class MyRecord { 
        public string UserHostAddress { get; set; }
    }

    public class InIpRangeOperator : IFilterOperator
    {
        public string Operator =>  "in_ip_range";

        public Expression GetExpression(Type type, IFilterRule rule, Expression propertyExp, BuildExpressionOptions options)
        {
            
            return Expression.Call(this.GetType().GetMethod("ContainsIP"), new[] { propertyExp, Expression.Constant(rule.Value) });
        
        }
        public static bool ContainsIP(string ip, string[] ranges) {
            return true; //TODO: implement custom ip range validation
        }
    }


    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class CustomOperatorsTests
    {

        [Test]
        public void CustomInOperator_Test() {
            var rec = new MyRecord();

            rec.UserHostAddress = "8.10.8.13";

            var records = new List<MyRecord>() { rec };

            var myFilter = new QueryBuilderFilterRule()
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>()
                {
                    new QueryBuilderFilterRule()
                    {
                        Condition = "and",
                        Field = "UserHostAddress",
                        Operator = "in_ip_range",
                        Type = "string",
                        Value = new [] { "10.10.10.*" }
                    }
                }
            };
            var options = new  BuildExpressionOptions();
            options.Operators = new List<IFilterOperator>() { new InIpRangeOperator()};
            var result = records.AsQueryable().BuildQuery<MyRecord>(myFilter, options).ToList();
            var len = result.Count;
            Assert.AreEqual(1, len);
        }

        public class GuidContainsOperator: IFilterOperator
        {
            public string Operator => "contains_guid";
            public Expression GetExpression(Type type, IFilterRule rule, Expression propertyExp, BuildExpressionOptions options)
            {
                var value = rule.Value;
                if (value is Array items) value = items.GetValue(0);

                var someValue = Expression.Constant(value.ToString().ToLower(), typeof(string));

                var nullCheck = QueryBuilder.GetNullCheckExpression(propertyExp);

                var propertyExpString = Expression.Call(propertyExp, propertyExp.Type.GetMethod("ToString", Type.EmptyTypes));
                var method = propertyExpString.Type.GetMethod("Contains", new[] { type });
                Expression exOut = Expression.Call(propertyExpString, typeof(string).GetMethod("ToLower", Type.EmptyTypes));

                exOut = Expression.AndAlso(nullCheck, Expression.Call(exOut, method, someValue));

                return exOut;
            }
        }

        public class GuidEndsWithOperator : IFilterOperator
        {
            public string Operator => "ends_with_guid";
            public Expression GetExpression(Type type, IFilterRule rule, Expression propertyExp, BuildExpressionOptions options)
            {
                var value = rule.Value;
                if (value is Array items) value = items.GetValue(0);

                var someValue = Expression.Constant(value.ToString().ToLower(), typeof(string));

                var nullCheck = QueryBuilder.GetNullCheckExpression(propertyExp);

                var propertyExpString = Expression.Call(propertyExp, propertyExp.Type.GetMethod("ToString", Type.EmptyTypes));
                var method = propertyExpString.Type.GetMethod("EndsWith", new[] { type });
                Expression exOut = Expression.Call(propertyExpString, typeof(string).GetMethod("ToLower", Type.EmptyTypes));

                exOut = Expression.AndAlso(nullCheck, Expression.Call(exOut, method, someValue));

                return exOut;
            }
        }

        public class GuidBeginsWithOperator : IFilterOperator
        {
            public string Operator => "begins_with_guid";
            public Expression GetExpression(Type type, IFilterRule rule, Expression propertyExp, BuildExpressionOptions options)
            {
                var value = rule.Value;
                if (value is Array items) value = items.GetValue(0);

                var someValue = Expression.Constant(value.ToString().ToLower(), typeof(string));

                var nullCheck = QueryBuilder.GetNullCheckExpression(propertyExp);

                var propertyExpString = Expression.Call(propertyExp, propertyExp.Type.GetMethod("ToString", Type.EmptyTypes));
                var method = propertyExpString.Type.GetMethod("StartsWith", new[] { type });
                Expression exOut = Expression.Call(propertyExpString, typeof(string).GetMethod("ToLower", Type.EmptyTypes));

                exOut = Expression.AndAlso(nullCheck, Expression.Call(exOut, method, someValue));

                return exOut;
            }
        }

        public class GuidEqualsOperator : IFilterOperator
        {
            public string Operator => "equal_guid";
            public Expression GetExpression(Type type, IFilterRule rule, Expression propertyExp, BuildExpressionOptions options)
            {
                var value = rule.Value;
                Expression someValue = QueryBuilder.GetConstants(type, value, false, options).First();

                Expression exOut;
                if (type == typeof(string))
                {
                    var nullCheck = QueryBuilder.GetNullCheckExpression(propertyExp);

                    var propertyExpString = Expression.Call(propertyExp, propertyExp.Type.GetMethod("ToString", Type.EmptyTypes));
                    exOut = Expression.Call(propertyExpString, typeof(string).GetMethod("ToLower", Type.EmptyTypes));

                    var someValueString = Expression.Call(someValue, someValue.Type.GetMethod("ToString", Type.EmptyTypes));
                    someValue = Expression.Call(someValueString, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                    exOut = Expression.AndAlso(nullCheck, Expression.Equal(exOut, someValue));
                }
                else
                {
                    exOut = Expression.Equal(propertyExp, Expression.Convert(someValue, propertyExp.Type));
                }

                return exOut;
            }
        }

        public class GuidInOperator : IFilterOperator
        {
            public string Operator => "in_guid";
            public Expression GetExpression(Type type, IFilterRule rule, Expression propertyExp, BuildExpressionOptions options)
            {
                var value = rule.Value;
                var someValues = QueryBuilder.GetConstants(type, value, true, options);

                var nullCheck = QueryBuilder.GetNullCheckExpression(propertyExp);

                if (QueryBuilder.IsGenericList(propertyExp.Type))
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

                            var propertyExpString = Expression.Call(propertyExp, propertyExp.Type.GetMethod("ToString", Type.EmptyTypes));
                            exOut = Expression.Call(propertyExpString, typeof(string).GetMethod("ToLower", Type.EmptyTypes));

                            var someValueString = Expression.Call(someValues[0], someValues[0].Type.GetMethod("ToString", Type.EmptyTypes));
                            var somevalue = Expression.Call(someValueString, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                            exOut = Expression.Equal(exOut, somevalue);
                            var counter = 1;
                            while (counter < someValues.Count)
                            {

                                var nextvalueString = Expression.Call(someValues[counter], someValues[counter].Type.GetMethod("ToString", Type.EmptyTypes));
                                var nextvalue = Expression.Call(nextvalueString, typeof(string).GetMethod("ToLower", Type.EmptyTypes));

                                exOut = Expression.Or(exOut,
                                    Expression.Equal(
                                        Expression.Call(propertyExpString, typeof(string).GetMethod("ToLower", Type.EmptyTypes)),
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
                            var propertyExpString = Expression.Call(propertyExp, propertyExp.Type.GetMethod("ToString", Type.EmptyTypes));
                            exOut = Expression.Call(propertyExpString, typeof(string).GetMethod("ToLower", Type.EmptyTypes));

                            var someValueString = Expression.Call(someValues.First(), someValues.First().Type.GetMethod("ToString", Type.EmptyTypes));
                            var somevalue = Expression.Call(someValueString, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
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
        }

    }
}
