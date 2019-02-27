using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castle.DynamicLinqQueryBuilder.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class QueryBuilderFilterRuleTests
    {
        IQueryable<Tests.ExpressionTreeBuilderTestClass> StartingQuery;
        IQueryable<Tests.ExpressionTreeBuilderTestClass> StartingDateQuery;

        [SetUp]
        public void Setup()
        {
            StartingQuery = Tests.GetExpressionTreeData().AsQueryable();
            StartingDateQuery = Tests.GetDateExpressionTreeData().AsQueryable();
        }

        #region Wrapper
        /// <summary>
        /// Some libraries, such as Newtonsoft.Json, will deserialize the elements of an array (that should be placed in the <see cref="IFilterRule.Value"/>) into a wrapper object
        /// </summary>
        private class Wrapper
        {
            public object Value { get; }

            public Wrapper(object value)
            {
                Value = value;
            }

            public override string ToString() => Value?.ToString();
        }
        #endregion

        #region Expression Tree Builder        

        [Test]
        public void DateHandling()
        {
            QueryBuilder.ParseDatesAsUtc = true;
            var contentIdFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "equal",
                        Type = "date",
                        Value = "2/23/2016"
                    }
                }
            };
            var queryable = StartingDateQuery.BuildQuery<Tests.ExpressionTreeBuilderTestClass>(contentIdFilter);
            var contentIdFilteredList = queryable.ToList();
            Assert.IsTrue(contentIdFilteredList != null);
            Assert.IsTrue(contentIdFilteredList.Count == 1);

            contentIdFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "equal",
                        Type = "date",
                        Value = ""
                    }
                }
            };
            ExceptionAssert.Throws<Exception>(() =>
            {
                var contentIdFilteredListNull1 = StartingQuery.BuildQuery(contentIdFilter).ToList();
            });


            contentIdFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "DateList",
                        Id = "DateList",
                        Input = "NA",
                        Operator = "in",
                        Type = "date",
                        Value = "2/23/2016"
                    }
                }
            };
            queryable = StartingDateQuery.BuildQuery(contentIdFilter);
            var contentIdFilteredList2 = queryable.ToList();
            Assert.IsTrue(contentIdFilteredList2 != null);
            Assert.IsTrue(contentIdFilteredList2.Count == 1);

            contentIdFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "DateList",
                        Id = "DateList",
                        Input = "NA",
                        Operator = "between",
                        Type = "date",
                        Value = ""
                    }
                }
            };
            ExceptionAssert.Throws<Exception>(() =>
            {
                var contentIdFilteredListNull2 = StartingDateQuery.BuildQuery(contentIdFilter).ToList();

            });

            QueryBuilder.ParseDatesAsUtc = false;
        }

        [Test]
        public void InClause()
        {
            //expect two entries to match for an integer comparison
            var contentIdFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = new[] { 1, 2 }
                    }
                }
            };
            var contentIdFilteredList = StartingQuery.BuildQuery(contentIdFilter).ToList();
            Assert.IsTrue(contentIdFilteredList != null);
            Assert.IsTrue(contentIdFilteredList.Count == 3);
            Assert.IsTrue(contentIdFilteredList.All(p => (new List<int>() { 1, 2 }).Contains(p.ContentTypeId)));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(contentIdFilter).ToList();

            });

            //single value test
            contentIdFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = new[] { 1 }
                    }
                }
            };
            contentIdFilteredList = StartingQuery.BuildQuery(contentIdFilter).ToList();
            Assert.IsTrue(contentIdFilteredList != null);
            Assert.IsTrue(contentIdFilteredList.Count == 2);
            Assert.IsTrue(contentIdFilteredList.All(p => (new List<int>() { 1 }).Contains(p.ContentTypeId)));

            //expect two entries to match for an integer comparison
            var nullableContentIdFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = new[] { 1, 2 }
                    }
                }
            };
            var nullableContentIdFilteredList =
                StartingQuery.BuildQuery(nullableContentIdFilter).ToList();
            Assert.IsTrue(nullableContentIdFilteredList != null);
            Assert.IsTrue(nullableContentIdFilteredList.Count == 2);
            Assert.IsTrue(
                nullableContentIdFilteredList.All(p => (new List<int>() { 1, 2 }).Contains(p.NullableContentTypeId.Value)));




            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "in",
                        Type = "string",
                        Value = new[] { "there is something interesting about this text", "there is something interesting about this text2" }
                    }
                }
            };
            var longerTextToFilterList = StartingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            Assert.IsTrue(longerTextToFilterList != null);
            Assert.IsTrue(longerTextToFilterList.Count == 3);
            Assert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter.ToLower())
                    .All(p => p == "there is something interesting about this text"));

            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilterCaps = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "in",
                        Type = "string",
                        Value = new[] { "THERE is something interesting about this text", "there is something interesting about this text2" }
                    }
                }
            };
            var longerTextToFilterListCaps = StartingQuery.BuildQuery(longerTextToFilterFilterCaps).ToList();
            Assert.IsTrue(longerTextToFilterListCaps != null);
            Assert.IsTrue(longerTextToFilterListCaps.Count == 3);
            Assert.IsTrue(
                longerTextToFilterListCaps.Select(p => p.LongerTextToFilter.ToLower())
                    .All(p => p == "there is something interesting about this text"));


            //expect 4 entries to match for a Date comparison
            var lastModifiedFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "in",
                        Type = "datetime",
                        Value = new[] { DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture), DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture) }
                    }
                }
            };
            var lastModifiedFilterList = StartingQuery.BuildQuery(lastModifiedFilter).ToList();
            Assert.IsTrue(lastModifiedFilterList != null);
            Assert.IsTrue(lastModifiedFilterList.Count == 4);
            Assert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => p == DateTime.UtcNow.Date));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(lastModifiedFilter).ToList();

            });

            //expect 3 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "in",
                        Type = "datetime",
                        Value = new[] { DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture), DateTime.UtcNow.Date.AddDays(1).ToString("d", CultureInfo.InvariantCulture) }
                    }
                }
            };
            var nullableLastModifiedFilterList = StartingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            Assert.IsTrue(nullableLastModifiedFilterList != null);
            Assert.IsTrue(nullableLastModifiedFilterList.Count == 3);
            Assert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => p == DateTime.UtcNow.Date));


            //expect 2 entries to match for a double field
            var statValueFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "in",
                        Type = "double",
                        Value = new[] { 1.11d, 1.12d }
                    }
                }
            };
            var statValueFilterList = StartingQuery.BuildQuery(statValueFilter).ToList();
            Assert.IsTrue(statValueFilterList != null);
            Assert.IsTrue(statValueFilterList.Count == 3);
            Assert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => (new List<double>() { 1.11, 1.12 }).Contains(p)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable double field
            var nullableStatValueFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "in",
                        Type = "double",
                        Value = new[] {1.112D, 1.113D }
                    }
                }
            };
            var nullableStatFilterList = StartingQuery.BuildQuery(nullableStatValueFilter).ToList();
            Assert.IsTrue(nullableStatFilterList != null);
            Assert.IsTrue(nullableStatFilterList.Count == 2);
            Assert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p == 1.112));

            //expect 2 entries to match for a List<DateTime> field
            var dateListFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "DateList",
                        Id = "DateList",
                        Input = "NA",
                        Operator = "in",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var dateListFilterList = StartingQuery.ToList().BuildQuery(dateListFilter).ToList();
            Assert.IsTrue(dateListFilterList != null);
            Assert.IsTrue(dateListFilterList.Count == 3);
            Assert.IsTrue(dateListFilterList.All(p => p.DateList.Contains(DateTime.UtcNow.Date.AddDays(-2))));

            //expect failure when an invalid date is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                dateListFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(dateListFilter).ToList();

            });

            //expect 2 entries to match for a List<string> field
            var strListFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "StrList",
                        Id = "StrList",
                        Input = "NA",
                        Operator = "in",
                        Type = "string",
                        Value = "Str2"
                    }
                }
            };
            var strListFilterList = StartingQuery.AsEnumerable().BuildQuery(strListFilter).ToList();
            Assert.IsTrue(strListFilterList != null);
            Assert.IsTrue(strListFilterList.Count == 3);
            Assert.IsTrue(strListFilterList.All(p => p.StrList.Contains("Str2")));

            //expect 2 entries to match for a List<int> field
            var intListFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "IntList",
                        Id = "IntList",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = new[] {"1", "3"}
                    }
                }
            };
            var intListFilterList = StartingQuery.BuildQuery(intListFilter).ToList();
            Assert.IsTrue(intListFilterList != null);
            Assert.IsTrue(intListFilterList.Count == 3);
            Assert.IsTrue(intListFilterList.All(p => p.IntList.Contains(1) || p.IntList.Contains(3)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                intListFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(intListFilter).ToList();

            });

            //expect 2 entries to match for a nullable nullable int field
            var nullableIntListFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "IntNullList",
                        Id = "IntNullList",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = 5
                    }
                }
            };
            var nullableIntListList = StartingQuery.BuildQuery(nullableIntListFilter).ToList();
            Assert.IsTrue(nullableIntListList != null);
            Assert.IsTrue(nullableIntListList.Count == 3);
            Assert.IsTrue(nullableIntListList.All(p => p.IntNullList.Contains(5)));

            var multipleWithBlankRule = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "StrList",
                        Id = "StrList",
                        Input = "NA",
                        Operator = "in",
                        Type = "string",
                        Value = new[] {"", "Str2" }
                    }
                }
            };
            var multipleWithBlankList = StartingQuery.BuildQuery(multipleWithBlankRule).ToList();
            Assert.IsTrue(multipleWithBlankList != null);
            Assert.IsTrue(multipleWithBlankList.Count == 4);
            Assert.IsTrue(multipleWithBlankList.All(p => p.StrList.Contains("") || p.StrList.Contains("Str2")));

            //expect 2 entries to match for a nullable double field
            var nullableWrappedStatValueFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "in",
                        Type = "double",
                        Value = new[] {new Wrapper(1.112D), new Wrapper(1.113D) }
                    }
                }
            };
            var nullableWrappedStatFilterList = StartingQuery.BuildQuery(nullableWrappedStatValueFilter).ToList();
            Assert.IsTrue(nullableWrappedStatFilterList != null);
            Assert.IsTrue(nullableWrappedStatFilterList.Count == 2);
            Assert.IsTrue(
                nullableWrappedStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p == 1.112));
        }

        //[Test]
        //public void NotInClause()
        //{
        //    var startingQuery = GetExpressionTreeData().AsQueryable();


        //    //expect two entries to match for an integer comparison
        //    var contentIdFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "ContentTypeId",
        //                Id = "ContentTypeId",
        //                Input = "NA",
        //                Operator = "not_in",
        //                Type = "integer",
        //                Value = "[1,2]"
        //            }
        //        }
        //    };
        //    var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
        //    Assert.IsTrue(contentIdFilteredList != null);
        //    Assert.IsTrue(contentIdFilteredList.Count == 1);
        //    Assert.IsTrue(contentIdFilteredList.All(p => (new List<int>() { 3 }).Contains(p.ContentTypeId)));

        //    //expect failure when non-numeric value is encountered in integer comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        contentIdFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

        //    });

        //    //expect two entries to match for an integer comparison
        //    var nullableContentIdFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "NullableContentTypeId",
        //                Id = "NullableContentTypeId",
        //                Input = "NA",
        //                Operator = "not_in",
        //                Type = "integer",
        //                Value = "[1,2]"
        //            }
        //        }
        //    };
        //    var nullableContentIdFilteredList =
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
        //    Assert.IsTrue(nullableContentIdFilteredList != null);
        //    Assert.IsTrue(nullableContentIdFilteredList.Count == 2);
        //    Assert.IsTrue(
        //        nullableContentIdFilteredList.All(p => !(new List<int>() { 1, 2 }).Contains(p.NullableContentTypeId.GetValueOrDefault())));




        //    //expect 3 entries to match for a case-insensitive string comparison
        //    var longerTextToFilterFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LongerTextToFilter",
        //                Id = "LongerTextToFilter",
        //                Input = "NA",
        //                Operator = "not_in",
        //                Type = "string",
        //                Value = "there is something interesting about this text"
        //            }
        //        }
        //    };
        //    var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
        //    Assert.IsTrue(longerTextToFilterList != null);
        //    Assert.IsTrue(longerTextToFilterList.Count == 1);
        //    Assert.IsTrue(
        //        longerTextToFilterList.Select(p => p.LongerTextToFilter)
        //            .All(p => p == null || p != "there is something interesting about this text"));


        //    //expect 4 entries to match for a Date comparison
        //    var lastModifiedFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LastModified",
        //                Id = "LastModified",
        //                Input = "NA",
        //                Operator = "not_in",
        //                Type = "datetime",
        //                Value = DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
        //            }
        //        }
        //    };
        //    var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
        //    Assert.IsTrue(lastModifiedFilterList != null);
        //    Assert.IsTrue(lastModifiedFilterList.Count == 0);
        //    Assert.IsTrue(
        //        lastModifiedFilterList.Select(p => p.LastModified)
        //            .All(p => p == DateTime.UtcNow.Date));

        //    //expect failure when an invalid date is encountered in date comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        lastModifiedFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

        //    });

        //    //expect 3 entries to match for a possibly empty Date comparison
        //    var nullableLastModifiedFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LastModifiedIfPresent",
        //                Id = "LastModifiedIfPresent",
        //                Input = "NA",
        //                Operator = "not_in",
        //                Type = "datetime",
        //                Value = DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.Date.AddDays(1).ToString("d", CultureInfo.InvariantCulture)
        //            }
        //        }
        //    };
        //    var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
        //    Assert.IsTrue(nullableLastModifiedFilterList != null);
        //    Assert.IsTrue(nullableLastModifiedFilterList.Count == 1);
        //    Assert.IsTrue(
        //        nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
        //            .All(p => p != DateTime.UtcNow.Date));


        //    //expect 2 entries to match for a double field
        //    var statValueFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "StatValue",
        //                Id = "StatValue",
        //                Input = "NA",
        //                Operator = "not_in",
        //                Type = "double",
        //                Value = "1.11,1.12"
        //            }
        //        }
        //    };
        //    var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
        //    Assert.IsTrue(statValueFilterList != null);
        //    Assert.IsTrue(statValueFilterList.Count == 1);
        //    Assert.IsTrue(
        //        statValueFilterList.Select(p => p.StatValue)
        //            .All(p => !(new List<double>() { 1.11, 1.12 }).Contains(p)));

        //    //expect failure when an invalid double is encountered in double comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        statValueFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

        //    });

        //    //expect 2 entries to match for a nullable double field
        //    var nullableStatValueFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "PossiblyEmptyStatValue",
        //                Id = "PossiblyEmptyStatValue",
        //                Input = "NA",
        //                Operator = "not_in",
        //                Type = "double",
        //                Value = "1.112, 1.113"
        //            }
        //        }
        //    };
        //    var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
        //    Assert.IsTrue(nullableStatFilterList != null);
        //    Assert.IsTrue(nullableStatFilterList.Count == 2);
        //    Assert.IsTrue(
        //        nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
        //            .All(p => p != 1.112));







        //    //expect 2 entries to match for a List<DateTime> field
        //    var dateListFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "DateList",
        //                Id = "DateList",
        //                Input = "NA",
        //                Operator = "not_in",
        //                Type = "datetime",
        //                Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
        //            }
        //        }
        //    };
        //    var dateListFilterList = startingQuery.BuildQuery(dateListFilter).ToList();
        //    Assert.IsTrue(dateListFilterList != null);
        //    Assert.IsTrue(dateListFilterList.Count == 1);
        //    Assert.IsTrue(dateListFilterList.All(p => !p.DateList.Contains(DateTime.UtcNow.Date.AddDays(-2))));

        //    //expect failure when an invalid date is encountered in double comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        dateListFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(dateListFilter).ToList();

        //    });

        //    //expect 2 entries to match for a List<string> field
        //    var strListFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "StrList",
        //                Id = "StrList",
        //                Input = "NA",
        //                Operator = "not_in",
        //                Type = "string",
        //                Value = "Str2"
        //            }
        //        }
        //    };
        //    var strListFilterList = startingQuery.BuildQuery(strListFilter).ToList();
        //    Assert.IsTrue(strListFilterList != null);
        //    Assert.IsTrue(strListFilterList.Count == 1);
        //    Assert.IsTrue(strListFilterList.All(p => !p.StrList.Contains("Str2")));









        //    //expect 2 entries to match for a List<int> field
        //    var intListFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "IntList",
        //                Id = "IntList",
        //                Input = "NA",
        //                Operator = "not_in",
        //                Type = "integer",
        //                Value = "1,3"
        //            }
        //        }
        //    };
        //    var intListFilterList = startingQuery.BuildQuery(intListFilter).ToList();
        //    Assert.IsTrue(intListFilterList != null);
        //    Assert.IsTrue(intListFilterList.Count == 1);
        //    Assert.IsTrue(intListFilterList.All(p => !p.IntList.Contains(1) && !p.IntList.Contains(3)));

        //    //expect failure when an invalid double is encountered in double comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        intListFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(intListFilter).ToList();

        //    });

        //    //expect 2 entries to match for a nullable nullable int field
        //    var nullableIntListFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "IntNullList",
        //                Id = "IntNullList",
        //                Input = "NA",
        //                Operator = "not_in",
        //                Type = "integer",
        //                Value = "5"
        //            }
        //        }
        //    };
        //    var nullableIntListList = startingQuery.BuildQuery(nullableIntListFilter).ToList();
        //    Assert.IsTrue(nullableIntListList != null);
        //    Assert.IsTrue(nullableIntListList.Count == 1);
        //    Assert.IsTrue(
        //        nullableIntListList.All(p => !p.IntNullList.Contains(5)));



        //}

        //[Test]
        //public void IsNullClause()
        //{
        //    var startingQuery = GetExpressionTreeData().AsQueryable();

        //    //expect 1 entries to match for a case-insensitive string comparison (nullable type)
        //    var longerTextToFilterFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LongerTextToFilter",
        //                Id = "LongerTextToFilter",
        //                Input = "NA",
        //                Operator = "is_null",
        //                Type = "string",
        //                Value = ""
        //            }
        //        }
        //    };
        //    var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
        //    Assert.IsTrue(longerTextToFilterList != null);
        //    Assert.IsTrue(longerTextToFilterList.Count == 1);
        //    Assert.IsTrue(
        //        longerTextToFilterList.Select(p => p.LongerTextToFilter)
        //            .All(p => p == null));


        //    //expect 0 entries to match for a non-nullable type
        //    var contentTypeIdFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "ContentTypeId",
        //                Id = "ContentTypeId",
        //                Input = "NA",
        //                Operator = "is_null",
        //                Type = "integer",
        //                Value = ""
        //            }
        //        }
        //    };
        //    var contentTypeIdFilterList = startingQuery.BuildQuery(contentTypeIdFilter).ToList();
        //    Assert.IsTrue(contentTypeIdFilterList != null);
        //    Assert.IsTrue(contentTypeIdFilterList.Count == 0);
        //    Assert.IsTrue(
        //        contentTypeIdFilterList.Select(p => p.ContentTypeId)
        //            .All(p => p == 0));

        //}

        //[Test]
        //public void IsNotNullClause()
        //{
        //    var startingQuery = GetExpressionTreeData().AsQueryable();

        //    //expect 3 entries to match for a case-insensitive string comparison (nullable type)
        //    var longerTextToFilterFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LongerTextToFilter",
        //                Id = "LongerTextToFilter",
        //                Input = "NA",
        //                Operator = "is_not_null",
        //                Type = "string",
        //                Value = ""
        //            }
        //        }
        //    };
        //    var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
        //    Assert.IsTrue(longerTextToFilterList != null);
        //    Assert.IsTrue(longerTextToFilterList.Count == 3);
        //    Assert.IsTrue(
        //        longerTextToFilterList.Select(p => p.LongerTextToFilter)
        //            .All(p => p != null));


        //    //expect 0 entries to match for a non-nullable type
        //    var contentTypeIdFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "ContentTypeId",
        //                Id = "ContentTypeId",
        //                Input = "NA",
        //                Operator = "is_not_null",
        //                Type = "integer",
        //                Value = ""
        //            }
        //        }
        //    };
        //    var contentTypeIdFilterList = startingQuery.BuildQuery(contentTypeIdFilter).ToList();
        //    Assert.IsTrue(contentTypeIdFilterList != null);
        //    Assert.IsTrue(contentTypeIdFilterList.Count == 4);
        //    Assert.IsTrue(
        //        contentTypeIdFilterList.Select(p => p.ContentTypeId)
        //            .All(p => p != 0));

        //}

        //[Test]
        //public void IsEmptyClause()
        //{
        //    var startingQuery = GetExpressionTreeData().AsQueryable();

        //    //expect 3 entries to match for a case-insensitive string comparison
        //    var longerTextToFilterFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LongerTextToFilter",
        //                Id = "LongerTextToFilter",
        //                Input = "NA",
        //                Operator = "is_empty",
        //                Type = "string",
        //                Value = ""
        //            }
        //        }
        //    };
        //    var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
        //    Assert.IsTrue(longerTextToFilterList != null);
        //    Assert.IsTrue(longerTextToFilterList.Count == 0);


        //    //expect 2 entries to match for a List<DateTime> field
        //    var dateListFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "DateList",
        //                Id = "DateList",
        //                Input = "NA",
        //                Operator = "is_empty",
        //                Type = "datetime",
        //                Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
        //            }
        //        }
        //    };
        //    var dateListFilterList = startingQuery.BuildQuery(dateListFilter).ToList();
        //    Assert.IsTrue(dateListFilterList != null);
        //    Assert.IsTrue(dateListFilterList.Count == 0);
        //    //Assert.IsTrue(dateListFilterList.All(p => !p.DateList.Contains(DateTime.UtcNow.Date.AddDays(-2))));



        //    //expect 2 entries to match for a List<string> field
        //    var strListFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "StrList",
        //                Id = "StrList",
        //                Input = "NA",
        //                Operator = "is_empty",
        //                Type = "string",
        //                Value = "Str2"
        //            }
        //        }
        //    };
        //    var strListFilterList = startingQuery.BuildQuery(strListFilter).ToList();
        //    Assert.IsTrue(strListFilterList != null);
        //    Assert.IsTrue(strListFilterList.Count == 0);
        //    //Assert.IsTrue(strListFilterList.All(p => !p.StrList.Contains("Str2")));


        //    //expect 2 entries to match for a List<int> field
        //    var intListFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "IntList",
        //                Id = "IntList",
        //                Input = "NA",
        //                Operator = "is_empty",
        //                Type = "integer",
        //                Value = "1,3"
        //            }
        //        }
        //    };
        //    var intListFilterList = startingQuery.BuildQuery(intListFilter).ToList();
        //    Assert.IsTrue(intListFilterList != null);
        //    Assert.IsTrue(intListFilterList.Count == 0);



        //}

        //[Test]
        //public void IsNotEmptyClause()
        //{
        //    var startingQuery = GetExpressionTreeData().AsQueryable();

        //    //expect 3 entries to match for a case-insensitive string comparison
        //    var longerTextToFilterFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LongerTextToFilter",
        //                Id = "LongerTextToFilter",
        //                Input = "NA",
        //                Operator = "is_not_empty",
        //                Type = "string",
        //                Value = ""
        //            }
        //        }
        //    };
        //    var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
        //    Assert.IsTrue(longerTextToFilterList != null);
        //    Assert.IsTrue(longerTextToFilterList.Count == 4);
        //    Assert.IsTrue(
        //        longerTextToFilterList.Select(p => p.LongerTextToFilter)
        //            .All(p => p == null || p.Length > 0));


        //    //expect 2 entries to match for a List<DateTime> field
        //    var dateListFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "DateList",
        //                Id = "DateList",
        //                Input = "NA",
        //                Operator = "is_not_empty",
        //                Type = "datetime",
        //                Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
        //            }
        //        }
        //    };
        //    var dateListFilterList = startingQuery.BuildQuery(dateListFilter).ToList();
        //    Assert.IsTrue(dateListFilterList != null);
        //    Assert.IsTrue(dateListFilterList.Count == 4);
        //    //Assert.IsTrue(dateListFilterList.All(p => !p.DateList.Contains(DateTime.UtcNow.Date.AddDays(-2))));



        //    //expect 2 entries to match for a List<string> field
        //    var strListFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "StrList",
        //                Id = "StrList",
        //                Input = "NA",
        //                Operator = "is_not_empty",
        //                Type = "string",
        //                Value = "Str2"
        //            }
        //        }
        //    };
        //    var strListFilterList = startingQuery.BuildQuery(strListFilter).ToList();
        //    Assert.IsTrue(strListFilterList != null);
        //    Assert.IsTrue(strListFilterList.Count == 4);
        //    //Assert.IsTrue(strListFilterList.All(p => !p.StrList.Contains("Str2")));


        //    //expect 2 entries to match for a List<int> field
        //    var intListFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "IntList",
        //                Id = "IntList",
        //                Input = "NA",
        //                Operator = "is_not_empty",
        //                Type = "integer",
        //                Value = "1,3"
        //            }
        //        }
        //    };
        //    var intListFilterList = startingQuery.BuildQuery(intListFilter).ToList();
        //    Assert.IsTrue(intListFilterList != null);
        //    Assert.IsTrue(intListFilterList.Count == 4);

        //}

        //[Test]
        //public void ContainsClause()
        //{
        //    var startingQuery = GetExpressionTreeData().AsQueryable();

        //    //expect 3 entries to match for a case-insensitive string comparison
        //    var longerTextToFilterFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LongerTextToFilter",
        //                Id = "LongerTextToFilter",
        //                Input = "NA",
        //                Operator = "contains",
        //                Type = "string",
        //                Value = "something interesting"
        //            }
        //        }
        //    };
        //    var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
        //    Assert.IsTrue(longerTextToFilterList != null);
        //    Assert.IsTrue(longerTextToFilterList.Count == 3);
        //    Assert.IsTrue(
        //        longerTextToFilterList.Select(p => p.LongerTextToFilter.ToLower())
        //            .All(p => p.Contains("something interesting")));

        //}

        //[Test]
        //public void NotContainsClause()
        //{
        //    var startingQuery = GetExpressionTreeData().AsQueryable();

        //    //expect 3 entries to match for a case-insensitive string comparison
        //    var longerTextToFilterFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LongerTextToFilter",
        //                Id = "LongerTextToFilter",
        //                Input = "NA",
        //                Operator = "not_contains",
        //                Type = "string",
        //                Value = "something interesting"
        //            }
        //        }
        //    };
        //    var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
        //    Assert.IsTrue(longerTextToFilterList != null);
        //    Assert.IsTrue(longerTextToFilterList.Count == 1);
        //    Assert.IsTrue(
        //        longerTextToFilterList.Select(p => p.LongerTextToFilter)
        //            .All(p => p == null));

        //}

        //[Test]
        //public void NotEndsWithClause()
        //{
        //    var startingQuery = GetExpressionTreeData().AsQueryable();

        //    //expect 3 entries to match for a case-insensitive string comparison
        //    var longerTextToFilterFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LongerTextToFilter",
        //                Id = "LongerTextToFilter",
        //                Input = "NA",
        //                Operator = "not_ends_with",
        //                Type = "string",
        //                Value = "about this text"
        //            }
        //        }
        //    };
        //    var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
        //    Assert.IsTrue(longerTextToFilterList != null);
        //    Assert.IsTrue(longerTextToFilterList.Count == 1);
        //    Assert.IsTrue(
        //        longerTextToFilterList.Select(p => p.LongerTextToFilter)
        //            .All(p => p == null));

        //}

        //[Test]
        //public void EndsWithClause()
        //{
        //    var startingQuery = GetExpressionTreeData().AsQueryable();

        //    //expect 3 entries to match for a case-insensitive string comparison
        //    var longerTextToFilterFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LongerTextToFilter",
        //                Id = "LongerTextToFilter",
        //                Input = "NA",
        //                Operator = "ends_with",
        //                Type = "string",
        //                Value = "about this text"
        //            }
        //        }
        //    };
        //    var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
        //    Assert.IsTrue(longerTextToFilterList != null);
        //    Assert.IsTrue(longerTextToFilterList.Count == 3);
        //    Assert.IsTrue(
        //        longerTextToFilterList.Select(p => p.LongerTextToFilter.ToLower())
        //            .All(p => p.EndsWith("about this text")));

        //}

        //[Test]
        //public void NotBeginsWithClause()
        //{
        //    var startingQuery = GetExpressionTreeData().AsQueryable();

        //    //expect 1 entries to match for a case-insensitive string comparison
        //    var longerTextToFilterFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LongerTextToFilter",
        //                Id = "LongerTextToFilter",
        //                Input = "NA",
        //                Operator = "not_begins_with",
        //                Type = "string",
        //                Value = "there is something"
        //            }
        //        }
        //    };
        //    var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
        //    Assert.IsTrue(longerTextToFilterList != null);
        //    Assert.IsTrue(longerTextToFilterList.Count == 1);
        //    Assert.IsTrue(
        //        longerTextToFilterList.Select(p => p.LongerTextToFilter)
        //            .All(p => p == null));

        //}

        //[Test]
        //public void BeginsWithClause()
        //{
        //    var startingQuery = GetExpressionTreeData().AsQueryable();

        //    //expect 3 entries to match for a case-insensitive string comparison
        //    var longerTextToFilterFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LongerTextToFilter",
        //                Id = "LongerTextToFilter",
        //                Input = "NA",
        //                Operator = "begins_with",
        //                Type = "string",
        //                Value = "there is something"
        //            }
        //        }
        //    };
        //    var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
        //    Assert.IsTrue(longerTextToFilterList != null);
        //    Assert.IsTrue(longerTextToFilterList.Count == 3);
        //    Assert.IsTrue(
        //        longerTextToFilterList.Select(p => p.LongerTextToFilter.ToLower())
        //            .All(p => p.StartsWith("there is something")));

        //}

        //[Test]
        //public void EqualsClause()
        //{
        //    var startingQuery = GetExpressionTreeData().AsQueryable();


        //    //expect two entries to match for an integer comparison
        //    var contentIdFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "ContentTypeId",
        //                Id = "ContentTypeId",
        //                Input = "NA",
        //                Operator = "equal",
        //                Type = "integer",
        //                Value = "1"
        //            }
        //        }
        //    };
        //    var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
        //    Assert.IsTrue(contentIdFilteredList != null);
        //    Assert.IsTrue(contentIdFilteredList.Count == 2);
        //    Assert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId == 1));

        //    //expect failure when non-numeric value is encountered in integer comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        contentIdFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

        //    });

        //    //expect two entries to match for an integer comparison
        //    var nullableContentIdFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "NullableContentTypeId",
        //                Id = "NullableContentTypeId",
        //                Input = "NA",
        //                Operator = "equal",
        //                Type = "integer",
        //                Value = "1"
        //            }
        //        }
        //    };
        //    var nullableContentIdFilteredList =
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
        //    Assert.IsTrue(nullableContentIdFilteredList != null);
        //    Assert.IsTrue(nullableContentIdFilteredList.Count == 1);
        //    Assert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId == 1));




        //    //expect 3 entries to match for a case-insensitive string comparison
        //    var longerTextToFilterFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LongerTextToFilter",
        //                Id = "LongerTextToFilter",
        //                Input = "NA",
        //                Operator = "equal",
        //                Type = "string",
        //                Value = "there is something interesting about this text"
        //            }
        //        }
        //    };
        //    var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
        //    Assert.IsTrue(longerTextToFilterList != null);
        //    Assert.IsTrue(longerTextToFilterList.Count == 3);
        //    Assert.IsTrue(
        //        longerTextToFilterList.Select(p => p.LongerTextToFilter.ToLower())
        //            .All(p => p == "there is something interesting about this text"));


        //    //expect 4 entries to match for a Date comparison
        //    var lastModifiedFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LastModified",
        //                Id = "LastModified",
        //                Input = "NA",
        //                Operator = "equal",
        //                Type = "datetime",
        //                Value = DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
        //            }
        //        }
        //    };
        //    var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
        //    Assert.IsTrue(lastModifiedFilterList != null);
        //    Assert.IsTrue(lastModifiedFilterList.Count == 4);
        //    Assert.IsTrue(
        //        lastModifiedFilterList.Select(p => p.LastModified)
        //            .All(p => p == DateTime.UtcNow.Date));

        //    //expect failure when an invalid date is encountered in date comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        lastModifiedFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

        //    });

        //    //expect 3 entries to match for a possibly empty Date comparison
        //    var nullableLastModifiedFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LastModifiedIfPresent",
        //                Id = "LastModifiedIfPresent",
        //                Input = "NA",
        //                Operator = "equal",
        //                Type = "datetime",
        //                Value = DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
        //            }
        //        }
        //    };
        //    var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
        //    Assert.IsTrue(nullableLastModifiedFilterList != null);
        //    Assert.IsTrue(nullableLastModifiedFilterList.Count == 3);
        //    Assert.IsTrue(
        //        nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
        //            .All(p => p == DateTime.UtcNow.Date));







        //    //expect 3 entries to match for a boolean field
        //    var isSelectedFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "IsSelected",
        //                Id = "IsSelected",
        //                Input = "NA",
        //                Operator = "equal",
        //                Type = "boolean",
        //                Value = "true"
        //            }
        //        }
        //    };
        //    var isSelectedFilterList = startingQuery.BuildQuery(isSelectedFilter).ToList();
        //    Assert.IsTrue(isSelectedFilterList != null);
        //    Assert.IsTrue(isSelectedFilterList.Count == 3);
        //    Assert.IsTrue(
        //        isSelectedFilterList.Select(p => p.IsSelected)
        //            .All(p => p == true));

        //    //expect failure when an invalid bool is encountered in bool comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        isSelectedFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(isSelectedFilter).ToList();

        //    });

        //    //expect 2 entries to match for a nullable boolean field
        //    var nullableIsSelectedFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "IsPossiblyNotSetBool",
        //                Id = "IsPossiblyNotSetBool",
        //                Input = "NA",
        //                Operator = "equal",
        //                Type = "boolean",
        //                Value = "true"
        //            }
        //        }
        //    };
        //    var nullableIsSelectedFilterList = startingQuery.BuildQuery(nullableIsSelectedFilter).ToList();
        //    Assert.IsTrue(nullableIsSelectedFilterList != null);
        //    Assert.IsTrue(nullableIsSelectedFilterList.Count == 2);
        //    Assert.IsTrue(
        //        nullableIsSelectedFilterList.Select(p => p.IsPossiblyNotSetBool)
        //            .All(p => p == true));


        //    //expect 2 entries to match for a double field
        //    var statValueFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "StatValue",
        //                Id = "StatValue",
        //                Input = "NA",
        //                Operator = "equal",
        //                Type = "double",
        //                Value = "1.11"
        //            }
        //        }
        //    };
        //    var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
        //    Assert.IsTrue(statValueFilterList != null);
        //    Assert.IsTrue(statValueFilterList.Count == 2);
        //    Assert.IsTrue(
        //        statValueFilterList.Select(p => p.StatValue)
        //            .All(p => p == 1.11));

        //    //expect failure when an invalid double is encountered in double comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        statValueFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

        //    });

        //    //expect 2 entries to match for a nullable boolean field
        //    var nullableStatValueFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "PossiblyEmptyStatValue",
        //                Id = "PossiblyEmptyStatValue",
        //                Input = "NA",
        //                Operator = "equal",
        //                Type = "double",
        //                Value = "1.112"
        //            }
        //        }
        //    };
        //    var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
        //    Assert.IsTrue(nullableStatFilterList != null);
        //    Assert.IsTrue(nullableStatFilterList.Count == 2);
        //    Assert.IsTrue(
        //        nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
        //            .All(p => p == 1.112));



        //}

        //[Test]
        //public void NotEqualsClause()
        //{
        //    var startingQuery = GetExpressionTreeData().AsQueryable();


        //    //expect two entries to match for an integer comparison
        //    var contentIdFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "ContentTypeId",
        //                Id = "ContentTypeId",
        //                Input = "NA",
        //                Operator = "not_equal",
        //                Type = "integer",
        //                Value = "1"
        //            }
        //        }
        //    };
        //    var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
        //    Assert.IsTrue(contentIdFilteredList != null);
        //    Assert.IsTrue(contentIdFilteredList.Count == 2);
        //    Assert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId != 1));

        //    //expect failure when non-numeric value is encountered in integer comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        contentIdFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

        //    });

        //    //expect 3 entries to match for an integer comparison
        //    var nullableContentIdFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "NullableContentTypeId",
        //                Id = "NullableContentTypeId",
        //                Input = "NA",
        //                Operator = "not_equal",
        //                Type = "integer",
        //                Value = "1"
        //            }
        //        }
        //    };
        //    var nullableContentIdFilteredList =
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
        //    Assert.IsTrue(nullableContentIdFilteredList != null);
        //    Assert.IsTrue(nullableContentIdFilteredList.Count == 3);
        //    Assert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId != 1));




        //    //expect 1 entries to match for a case-insensitive string comparison
        //    var longerTextToFilterFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LongerTextToFilter",
        //                Id = "LongerTextToFilter",
        //                Input = "NA",
        //                Operator = "not_equal",
        //                Type = "string",
        //                Value = "there is something interesting about this text"
        //            }
        //        }
        //    };
        //    var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
        //    Assert.IsTrue(longerTextToFilterList != null);
        //    Assert.IsTrue(longerTextToFilterList.Count == 1);
        //    Assert.IsTrue(
        //        longerTextToFilterList.Select(p => p.LongerTextToFilter)
        //            .All(p => (p == null) || (p.ToLower() != "there is something interesting about this text")));


        //    //expect 0 entries to match for a Date comparison
        //    var lastModifiedFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LastModified",
        //                Id = "LastModified",
        //                Input = "NA",
        //                Operator = "not_equal",
        //                Type = "datetime",
        //                Value = DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
        //            }
        //        }
        //    };
        //    var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
        //    Assert.IsTrue(lastModifiedFilterList != null);
        //    Assert.IsTrue(lastModifiedFilterList.Count == 0);
        //    Assert.IsTrue(
        //        lastModifiedFilterList.Select(p => p.LastModified)
        //            .All(p => p == DateTime.UtcNow.Date));

        //    //expect failure when an invalid date is encountered in date comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        lastModifiedFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

        //    });

        //    //expect 1 entries to match for a possibly empty Date comparison
        //    var nullableLastModifiedFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LastModifiedIfPresent",
        //                Id = "LastModifiedIfPresent",
        //                Input = "NA",
        //                Operator = "not_equal",
        //                Type = "datetime",
        //                Value = DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
        //            }
        //        }
        //    };
        //    var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
        //    Assert.IsTrue(nullableLastModifiedFilterList != null);
        //    Assert.IsTrue(nullableLastModifiedFilterList.Count == 1);
        //    Assert.IsTrue(
        //        nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
        //            .All(p => p != DateTime.UtcNow.Date));


        //    //expect 1 entries to match for a boolean field
        //    var isSelectedFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "IsSelected",
        //                Id = "IsSelected",
        //                Input = "NA",
        //                Operator = "not_equal",
        //                Type = "boolean",
        //                Value = "true"
        //            }
        //        }
        //    };
        //    var isSelectedFilterList = startingQuery.BuildQuery(isSelectedFilter).ToList();
        //    Assert.IsTrue(isSelectedFilterList != null);
        //    Assert.IsTrue(isSelectedFilterList.Count == 1);
        //    Assert.IsTrue(
        //        isSelectedFilterList.Select(p => p.IsSelected)
        //            .All(p => p != true));

        //    //expect failure when an invalid bool is encountered in bool comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        isSelectedFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(isSelectedFilter).ToList();

        //    });

        //    //expect 2 entries to match for a nullable boolean field
        //    var nullableIsSelectedFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "IsPossiblyNotSetBool",
        //                Id = "IsPossiblyNotSetBool",
        //                Input = "NA",
        //                Operator = "not_equal",
        //                Type = "boolean",
        //                Value = "true"
        //            }
        //        }
        //    };
        //    var nullableIsSelectedFilterList = startingQuery.BuildQuery(nullableIsSelectedFilter).ToList();
        //    Assert.IsTrue(nullableIsSelectedFilterList != null);
        //    Assert.IsTrue(nullableIsSelectedFilterList.Count == 2);
        //    Assert.IsTrue(
        //        nullableIsSelectedFilterList.Select(p => p.IsPossiblyNotSetBool)
        //            .All(p => p != true));


        //    //expect 2 entries to match for a double field
        //    var statValueFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "StatValue",
        //                Id = "StatValue",
        //                Input = "NA",
        //                Operator = "not_equal",
        //                Type = "double",
        //                Value = "1.11"
        //            }
        //        }
        //    };
        //    var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
        //    Assert.IsTrue(statValueFilterList != null);
        //    Assert.IsTrue(statValueFilterList.Count == 2);
        //    Assert.IsTrue(
        //        statValueFilterList.Select(p => p.StatValue)
        //            .All(p => p != 1.11));

        //    //expect failure when an invalid double is encountered in double comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        statValueFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

        //    });

        //    //expect 2 entries to match for a nullable boolean field
        //    var nullableStatValueFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "PossiblyEmptyStatValue",
        //                Id = "PossiblyEmptyStatValue",
        //                Input = "NA",
        //                Operator = "not_equal",
        //                Type = "double",
        //                Value = "1.112"
        //            }
        //        }
        //    };
        //    var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
        //    Assert.IsTrue(nullableStatFilterList != null);
        //    Assert.IsTrue(nullableStatFilterList.Count == 2);
        //    Assert.IsTrue(
        //        nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
        //            .All(p => p != 1.112));

        //}

        //[Test]
        //public void BetweenClause()
        //{
        //    var startingQuery = GetExpressionTreeData().AsQueryable();


        //    //expect 3 entries to match for an integer comparison
        //    var contentIdFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "ContentTypeId",
        //                Id = "ContentTypeId",
        //                Input = "NA",
        //                Operator = "between",
        //                Type = "integer",
        //                Value = "1,2"
        //            }
        //        }
        //    };
        //    var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
        //    Assert.IsTrue(contentIdFilteredList != null);
        //    Assert.IsTrue(contentIdFilteredList.Count == 3);
        //    Assert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId < 3));

        //    //expect failure when non-numeric value is encountered in integer comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        contentIdFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

        //    });

        //    //expect 2 entries to match for an integer comparison
        //    var nullableContentIdFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "NullableContentTypeId",
        //                Id = "NullableContentTypeId",
        //                Input = "NA",
        //                Operator = "between",
        //                Type = "integer",
        //                Value = "1,2"
        //            }
        //        }
        //    };
        //    var nullableContentIdFilteredList =
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
        //    Assert.IsTrue(nullableContentIdFilteredList != null);
        //    Assert.IsTrue(nullableContentIdFilteredList.Count == 2);
        //    Assert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId < 3));





        //    //expect 4 entries to match for a Date comparison
        //    var lastModifiedFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LastModified",
        //                Id = "LastModified",
        //                Input = "NA",
        //                Operator = "between",
        //                Type = "datetime",
        //                Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
        //            }
        //        }
        //    };
        //    var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
        //    Assert.IsTrue(lastModifiedFilterList != null);
        //    Assert.IsTrue(lastModifiedFilterList.Count == 4);
        //    Assert.IsTrue(
        //        lastModifiedFilterList.Select(p => p.LastModified)
        //            .All(p => (p >= DateTime.UtcNow.Date.AddDays(-2)) && (p <= DateTime.UtcNow.Date)));

        //    //expect failure when an invalid date is encountered in date comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        lastModifiedFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

        //    });

        //    //expect 3 entries to match for a possibly empty Date comparison
        //    var nullableLastModifiedFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LastModifiedIfPresent",
        //                Id = "LastModifiedIfPresent",
        //                Input = "NA",
        //                Operator = "between",
        //                Type = "datetime",
        //                Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
        //            }
        //        }
        //    };
        //    var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
        //    Assert.IsTrue(nullableLastModifiedFilterList != null);
        //    Assert.IsTrue(nullableLastModifiedFilterList.Count == 3);
        //    Assert.IsTrue(
        //        nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
        //            .All(p => (p >= DateTime.UtcNow.Date.AddDays(-2)) && (p <= DateTime.UtcNow.Date)));


        //    //expect 3 entries to match for a double field
        //    var statValueFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "StatValue",
        //                Id = "StatValue",
        //                Input = "NA",
        //                Operator = "between",
        //                Type = "double",
        //                Value = "1.0,1.12"
        //            }
        //        }
        //    };
        //    var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
        //    Assert.IsTrue(statValueFilterList != null);
        //    Assert.IsTrue(statValueFilterList.Count == 3);
        //    Assert.IsTrue(
        //        statValueFilterList.Select(p => p.StatValue)
        //            .All(p => (p >= 1.0) && (p <= 1.12)));

        //    //expect failure when an invalid double is encountered in double comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        statValueFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

        //    });

        //    //expect 2 entries to match for a nullable boolean field
        //    var nullableStatValueFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "PossiblyEmptyStatValue",
        //                Id = "PossiblyEmptyStatValue",
        //                Input = "NA",
        //                Operator = "between",
        //                Type = "double",
        //                Value = "1.112,1.112"
        //            }
        //        }
        //    };
        //    var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
        //    Assert.IsTrue(nullableStatFilterList != null);
        //    Assert.IsTrue(nullableStatFilterList.Count == 2);
        //    Assert.IsTrue(
        //        nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
        //            .All(p => p == 1.112));

        //}

        //[Test]
        //public void NotBetweenClause()
        //{
        //    var startingQuery = GetExpressionTreeData().AsQueryable();


        //    //expect 1 entry to match for an integer comparison
        //    var contentIdFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "ContentTypeId",
        //                Id = "ContentTypeId",
        //                Input = "NA",
        //                Operator = "not_between",
        //                Type = "integer",
        //                Value = "1,2"
        //            }
        //        }
        //    };
        //    var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
        //    Assert.IsTrue(contentIdFilteredList != null);
        //    Assert.IsTrue(contentIdFilteredList.Count == 1);
        //    Assert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId < 1 || p.ContentTypeId > 2));

        //    //expect failure when non-numeric value is encountered in integer comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        contentIdFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

        //    });

        //    //expect 2 entries to match for an integer comparison
        //    var nullableContentIdFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "NullableContentTypeId",
        //                Id = "NullableContentTypeId",
        //                Input = "NA",
        //                Operator = "not_between",
        //                Type = "integer",
        //                Value = "1,2"
        //            }
        //        }
        //    };
        //    var nullableContentIdFilteredList =
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
        //    Assert.IsTrue(nullableContentIdFilteredList != null);
        //    Assert.IsTrue(nullableContentIdFilteredList.Count == 2);
        //    Assert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId < 1 || p.NullableContentTypeId > 2 || p.NullableContentTypeId == null));





        //    //expect 0 entries to match for a Date comparison
        //    var lastModifiedFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LastModified",
        //                Id = "LastModified",
        //                Input = "NA",
        //                Operator = "not_between",
        //                Type = "datetime",
        //                Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
        //            }
        //        }
        //    };
        //    var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
        //    Assert.IsTrue(lastModifiedFilterList != null);
        //    Assert.IsTrue(lastModifiedFilterList.Count == 0);
        //    Assert.IsTrue(
        //        lastModifiedFilterList.Select(p => p.LastModified)
        //            .All(p => (p <= DateTime.UtcNow.Date.AddDays(-2)) && (p >= DateTime.UtcNow.Date)));

        //    //expect failure when an invalid date is encountered in date comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        lastModifiedFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

        //    });

        //    //expect 1 entries to match for a possibly empty Date comparison
        //    var nullableLastModifiedFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LastModifiedIfPresent",
        //                Id = "LastModifiedIfPresent",
        //                Input = "NA",
        //                Operator = "not_between",
        //                Type = "datetime",
        //                Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
        //            }
        //        }
        //    };
        //    var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
        //    Assert.IsTrue(nullableLastModifiedFilterList != null);
        //    Assert.IsTrue(nullableLastModifiedFilterList.Count == 1);
        //    Assert.IsTrue(
        //        nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
        //            .All(p => (p <= DateTime.UtcNow.Date.AddDays(-2) && p >= DateTime.UtcNow.Date) || p == null));


        //    //expect 3 entries to match for a double field
        //    var statValueFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "StatValue",
        //                Id = "StatValue",
        //                Input = "NA",
        //                Operator = "not_between",
        //                Type = "double",
        //                Value = "1.0,1.12"
        //            }
        //        }
        //    };
        //    var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
        //    Assert.IsTrue(statValueFilterList != null);
        //    Assert.IsTrue(statValueFilterList.Count == 1);
        //    Assert.IsTrue(
        //        statValueFilterList.Select(p => p.StatValue)
        //            .All(p => (p <= 1.0) || (p >= 1.12)));

        //    //expect failure when an invalid double is encountered in double comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        statValueFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

        //    });

        //    //expect 2 entries to match for a nullable boolean field
        //    var nullableStatValueFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "PossiblyEmptyStatValue",
        //                Id = "PossiblyEmptyStatValue",
        //                Input = "NA",
        //                Operator = "not_between",
        //                Type = "double",
        //                Value = "1.112,1.112"
        //            }
        //        }
        //    };
        //    var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
        //    Assert.IsTrue(nullableStatFilterList != null);
        //    Assert.IsTrue(nullableStatFilterList.Count == 2);
        //    Assert.IsTrue(
        //        nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
        //            .All(p => p != 1.112));

        //}

        //[Test]
        //public void GreaterOrEqualClause()
        //{
        //    var startingQuery = GetExpressionTreeData().AsQueryable();


        //    //expect 1 entries to match for an integer comparison
        //    var contentIdFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "ContentTypeId",
        //                Id = "ContentTypeId",
        //                Input = "NA",
        //                Operator = "greater_or_equal",
        //                Type = "integer",
        //                Value = "2"
        //            }
        //        }
        //    };
        //    var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
        //    Assert.IsTrue(contentIdFilteredList != null);
        //    Assert.IsTrue(contentIdFilteredList.Count == 2);
        //    Assert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId >= 2));

        //    //expect failure when non-numeric value is encountered in integer comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        contentIdFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

        //    });

        //    //expect 1 entries to match for an integer comparison
        //    var nullableContentIdFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "NullableContentTypeId",
        //                Id = "NullableContentTypeId",
        //                Input = "NA",
        //                Operator = "greater_or_equal",
        //                Type = "integer",
        //                Value = "2"
        //            }
        //        }
        //    };
        //    var nullableContentIdFilteredList =
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
        //    Assert.IsTrue(nullableContentIdFilteredList != null);
        //    Assert.IsTrue(nullableContentIdFilteredList.Count == 2);
        //    Assert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId >= 2));


        //    //expect 4 entries to match for a Date comparison
        //    var lastModifiedFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LastModified",
        //                Id = "LastModified",
        //                Input = "NA",
        //                Operator = "greater_or_equal",
        //                Type = "datetime",
        //                Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
        //            }
        //        }
        //    };
        //    var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
        //    Assert.IsTrue(lastModifiedFilterList != null);
        //    Assert.IsTrue(lastModifiedFilterList.Count == 4);
        //    Assert.IsTrue(
        //        lastModifiedFilterList.Select(p => p.LastModified)
        //            .All(p => (p >= DateTime.UtcNow.Date.AddDays(-2))));

        //    //expect failure when an invalid date is encountered in date comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        lastModifiedFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

        //    });

        //    //expect 0 entries to match for a possibly empty Date comparison
        //    var nullableLastModifiedFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LastModifiedIfPresent",
        //                Id = "LastModifiedIfPresent",
        //                Input = "NA",
        //                Operator = "greater_or_equal",
        //                Type = "datetime",
        //                Value = DateTime.UtcNow.Date.AddDays(1).ToString("d", CultureInfo.InvariantCulture)
        //            }
        //        }
        //    };
        //    var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
        //    Assert.IsTrue(nullableLastModifiedFilterList != null);
        //    Assert.IsTrue(nullableLastModifiedFilterList.Count == 0);
        //    Assert.IsTrue(
        //        nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
        //            .All(p => (p >= DateTime.UtcNow.Date.AddDays(1))));


        //    //expect 4 entries to match for a double field
        //    var statValueFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "StatValue",
        //                Id = "StatValue",
        //                Input = "NA",
        //                Operator = "greater_or_equal",
        //                Type = "double",
        //                Value = "1"
        //            }
        //        }
        //    };
        //    var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
        //    Assert.IsTrue(statValueFilterList != null);
        //    Assert.IsTrue(statValueFilterList.Count == 4);
        //    Assert.IsTrue(
        //        statValueFilterList.Select(p => p.StatValue)
        //            .All(p => (p >= 1)));

        //    //expect failure when an invalid double is encountered in double comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        statValueFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

        //    });

        //    //expect 2 entries to match for a nullable boolean field
        //    var nullableStatValueFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "PossiblyEmptyStatValue",
        //                Id = "PossiblyEmptyStatValue",
        //                Input = "NA",
        //                Operator = "greater_or_equal",
        //                Type = "double",
        //                Value = "1.112"
        //            }
        //        }
        //    };
        //    var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
        //    Assert.IsTrue(nullableStatFilterList != null);
        //    Assert.IsTrue(nullableStatFilterList.Count == 2);
        //    Assert.IsTrue(
        //        nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
        //            .All(p => p >= 1.112));

        //}

        //[Test]
        //public void GreaterClause()
        //{
        //    var startingQuery = GetExpressionTreeData().AsQueryable();


        //    //expect 1 entries to match for an integer comparison
        //    var contentIdFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "ContentTypeId",
        //                Id = "ContentTypeId",
        //                Input = "NA",
        //                Operator = "greater",
        //                Type = "integer",
        //                Value = "2"
        //            }
        //        }
        //    };
        //    var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
        //    Assert.IsTrue(contentIdFilteredList != null);
        //    Assert.IsTrue(contentIdFilteredList.Count == 1);
        //    Assert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId > 2));

        //    //expect failure when non-numeric value is encountered in integer comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        contentIdFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

        //    });

        //    //expect 1 entries to match for an integer comparison
        //    var nullableContentIdFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "NullableContentTypeId",
        //                Id = "NullableContentTypeId",
        //                Input = "NA",
        //                Operator = "greater",
        //                Type = "integer",
        //                Value = "2"
        //            }
        //        }
        //    };
        //    var nullableContentIdFilteredList =
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
        //    Assert.IsTrue(nullableContentIdFilteredList != null);
        //    Assert.IsTrue(nullableContentIdFilteredList.Count == 1);
        //    Assert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId > 2));


        //    //expect 4 entries to match for a Date comparison
        //    var lastModifiedFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LastModified",
        //                Id = "LastModified",
        //                Input = "NA",
        //                Operator = "greater",
        //                Type = "datetime",
        //                Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
        //            }
        //        }
        //    };
        //    var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
        //    Assert.IsTrue(lastModifiedFilterList != null);
        //    Assert.IsTrue(lastModifiedFilterList.Count == 4);
        //    Assert.IsTrue(
        //        lastModifiedFilterList.Select(p => p.LastModified)
        //            .All(p => (p > DateTime.UtcNow.Date.AddDays(-2))));

        //    //expect failure when an invalid date is encountered in date comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        lastModifiedFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

        //    });

        //    //expect 0 entries to match for a possibly empty Date comparison
        //    var nullableLastModifiedFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LastModifiedIfPresent",
        //                Id = "LastModifiedIfPresent",
        //                Input = "NA",
        //                Operator = "greater",
        //                Type = "datetime",
        //                Value = DateTime.UtcNow.Date.AddDays(1).ToString("d", CultureInfo.InvariantCulture)
        //            }
        //        }
        //    };
        //    var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
        //    Assert.IsTrue(nullableLastModifiedFilterList != null);
        //    Assert.IsTrue(nullableLastModifiedFilterList.Count == 0);
        //    Assert.IsTrue(
        //        nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
        //            .All(p => (p > DateTime.UtcNow.Date.AddDays(1))));


        //    //expect 4 entries to match for a double field
        //    var statValueFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "StatValue",
        //                Id = "StatValue",
        //                Input = "NA",
        //                Operator = "greater",
        //                Type = "double",
        //                Value = "1"
        //            }
        //        }
        //    };
        //    var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
        //    Assert.IsTrue(statValueFilterList != null);
        //    Assert.IsTrue(statValueFilterList.Count == 4);
        //    Assert.IsTrue(
        //        statValueFilterList.Select(p => p.StatValue)
        //            .All(p => (p > 1)));

        //    //expect failure when an invalid double is encountered in double comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        statValueFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

        //    });

        //    //expect 2 entries to match for a nullable boolean field
        //    var nullableStatValueFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "PossiblyEmptyStatValue",
        //                Id = "PossiblyEmptyStatValue",
        //                Input = "NA",
        //                Operator = "greater",
        //                Type = "double",
        //                Value = "1.112"
        //            }
        //        }
        //    };
        //    var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
        //    Assert.IsTrue(nullableStatFilterList != null);
        //    Assert.IsTrue(nullableStatFilterList.Count == 0);
        //    Assert.IsTrue(
        //        nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
        //            .All(p => p > 1.112));

        //}

        //[Test]
        //public void LessClause()
        //{
        //    var startingQuery = GetExpressionTreeData().AsQueryable();


        //    //expect 2 entries to match for an integer comparison
        //    var contentIdFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "ContentTypeId",
        //                Id = "ContentTypeId",
        //                Input = "NA",
        //                Operator = "less",
        //                Type = "integer",
        //                Value = "2"
        //            }
        //        }
        //    };
        //    var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
        //    Assert.IsTrue(contentIdFilteredList != null);
        //    Assert.IsTrue(contentIdFilteredList.Count == 2);
        //    Assert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId < 2));

        //    //expect failure when non-numeric value is encountered in integer comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        contentIdFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

        //    });

        //    //expect 1 entries to match for an integer comparison
        //    var nullableContentIdFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "NullableContentTypeId",
        //                Id = "NullableContentTypeId",
        //                Input = "NA",
        //                Operator = "less",
        //                Type = "integer",
        //                Value = "2"
        //            }
        //        }
        //    };
        //    var nullableContentIdFilteredList =
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
        //    Assert.IsTrue(nullableContentIdFilteredList != null);
        //    Assert.IsTrue(nullableContentIdFilteredList.Count == 1);
        //    Assert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId < 2));


        //    //expect 0 entries to match for a Date comparison
        //    var lastModifiedFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LastModified",
        //                Id = "LastModified",
        //                Input = "NA",
        //                Operator = "less",
        //                Type = "datetime",
        //                Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
        //            }
        //        }
        //    };
        //    var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
        //    Assert.IsTrue(lastModifiedFilterList != null);
        //    Assert.IsTrue(lastModifiedFilterList.Count == 0);
        //    Assert.IsTrue(
        //        lastModifiedFilterList.Select(p => p.LastModified)
        //            .All(p => (p <= DateTime.UtcNow.Date.AddDays(-2))));

        //    //expect failure when an invalid date is encountered in date comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        lastModifiedFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

        //    });

        //    //expect 3 entries to match for a possibly empty Date comparison
        //    var nullableLastModifiedFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LastModifiedIfPresent",
        //                Id = "LastModifiedIfPresent",
        //                Input = "NA",
        //                Operator = "less",
        //                Type = "datetime",
        //                Value = DateTime.UtcNow.Date.AddDays(1).ToString("d", CultureInfo.InvariantCulture)
        //            }
        //        }
        //    };
        //    var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
        //    Assert.IsTrue(nullableLastModifiedFilterList != null);
        //    Assert.IsTrue(nullableLastModifiedFilterList.Count == 3);
        //    Assert.IsTrue(
        //        nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
        //            .All(p => (p <= DateTime.UtcNow.Date.AddDays(1))));


        //    //expect 3 entries to match for a double field
        //    var statValueFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "StatValue",
        //                Id = "StatValue",
        //                Input = "NA",
        //                Operator = "less",
        //                Type = "double",
        //                Value = "1.13"
        //            }
        //        }
        //    };
        //    var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
        //    Assert.IsTrue(statValueFilterList != null);
        //    Assert.IsTrue(statValueFilterList.Count == 3);
        //    Assert.IsTrue(
        //        statValueFilterList.Select(p => p.StatValue)
        //            .All(p => (p <= 1.12)));

        //    //expect failure when an invalid double is encountered in double comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        statValueFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

        //    });

        //    //expect 2 entries to match for a nullable boolean field
        //    var nullableStatValueFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "PossiblyEmptyStatValue",
        //                Id = "PossiblyEmptyStatValue",
        //                Input = "NA",
        //                Operator = "less",
        //                Type = "double",
        //                Value = "1.113"
        //            }
        //        }
        //    };
        //    var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
        //    Assert.IsTrue(nullableStatFilterList != null);
        //    Assert.IsTrue(nullableStatFilterList.Count == 2);
        //    Assert.IsTrue(
        //        nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
        //            .All(p => p < 1.113));

        //}

        //[Test]
        //public void LessOrEqualClause()
        //{
        //    var startingQuery = GetExpressionTreeData().AsQueryable();


        //    //expect 3 entries to match for an integer comparison
        //    var contentIdFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "ContentTypeId",
        //                Id = "ContentTypeId",
        //                Input = "NA",
        //                Operator = "less_or_equal",
        //                Type = "integer",
        //                Value = "2"
        //            }
        //        }
        //    };
        //    var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
        //    Assert.IsTrue(contentIdFilteredList != null);
        //    Assert.IsTrue(contentIdFilteredList.Count == 3);
        //    Assert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId <= 2));

        //    //expect failure when non-numeric value is encountered in integer comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        contentIdFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

        //    });

        //    //expect 2 entries to match for an integer comparison
        //    var nullableContentIdFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "NullableContentTypeId",
        //                Id = "NullableContentTypeId",
        //                Input = "NA",
        //                Operator = "less_or_equal",
        //                Type = "integer",
        //                Value = "2"
        //            }
        //        }
        //    };
        //    var nullableContentIdFilteredList =
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
        //    Assert.IsTrue(nullableContentIdFilteredList != null);
        //    Assert.IsTrue(nullableContentIdFilteredList.Count == 2);
        //    Assert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId <= 2));


        //    //expect 0 entries to match for a Date comparison
        //    var lastModifiedFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LastModified",
        //                Id = "LastModified",
        //                Input = "NA",
        //                Operator = "less_or_equal",
        //                Type = "datetime",
        //                Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
        //            }
        //        }
        //    };
        //    var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
        //    Assert.IsTrue(lastModifiedFilterList != null);
        //    Assert.IsTrue(lastModifiedFilterList.Count == 0);
        //    Assert.IsTrue(
        //        lastModifiedFilterList.Select(p => p.LastModified)
        //            .All(p => (p <= DateTime.UtcNow.Date.AddDays(-2))));

        //    //expect failure when an invalid date is encountered in date comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        lastModifiedFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

        //    });

        //    //expect 3 entries to match for a possibly empty Date comparison
        //    var nullableLastModifiedFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "LastModifiedIfPresent",
        //                Id = "LastModifiedIfPresent",
        //                Input = "NA",
        //                Operator = "less_or_equal",
        //                Type = "datetime",
        //                Value = DateTime.UtcNow.Date.AddDays(1).ToString("d", CultureInfo.InvariantCulture)
        //            }
        //        }
        //    };
        //    var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
        //    Assert.IsTrue(nullableLastModifiedFilterList != null);
        //    Assert.IsTrue(nullableLastModifiedFilterList.Count == 3);
        //    Assert.IsTrue(
        //        nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
        //            .All(p => (p <= DateTime.UtcNow.Date.AddDays(1))));


        //    //expect 3 entries to match for a double field
        //    var statValueFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "StatValue",
        //                Id = "StatValue",
        //                Input = "NA",
        //                Operator = "less_or_equal",
        //                Type = "double",
        //                Value = "1.13"
        //            }
        //        }
        //    };
        //    var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
        //    Assert.IsTrue(statValueFilterList != null);
        //    Assert.IsTrue(statValueFilterList.Count == 4);
        //    Assert.IsTrue(
        //        statValueFilterList.Select(p => p.StatValue)
        //            .All(p => (p <= 1.13)));

        //    //expect failure when an invalid double is encountered in double comparison
        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        statValueFilter.Rules.First().Value = "hello";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

        //    });

        //    //expect 2 entries to match for a nullable double field
        //    var nullableStatValueFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "PossiblyEmptyStatValue",
        //                Id = "PossiblyEmptyStatValue",
        //                Input = "NA",
        //                Operator = "less_or_equal",
        //                Type = "double",
        //                Value = "1.113"
        //            }
        //        }
        //    };
        //    var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
        //    Assert.IsTrue(nullableStatFilterList != null);
        //    Assert.IsTrue(nullableStatFilterList.Count == 2);
        //    Assert.IsTrue(
        //        nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
        //            .All(p => p <= 1.113));

        //}

        //[Test]
        //public void FilterWithInvalidParameters()
        //{
        //    var startingQuery = GetExpressionTreeData().AsQueryable();


        //    //expect 3 entries to match for an integer comparison
        //    var contentIdFilter = new FilterRule()
        //    {
        //        Condition = "and",
        //        Rules = new List<FilterRule>()
        //        {
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "ContentTypeId",
        //                Id = "ContentTypeId",
        //                Input = "NA",
        //                Operator = "less_or_equal",
        //                Type = "integer",
        //                Value = "2"
        //            },
        //            new FilterRule()
        //            {
        //                Condition = "and",
        //                Field = "ContentTypeId",
        //                Id = "ContentTypeId",
        //                Input = "NA",
        //                Operator = "less_or_equal",
        //                Type = "integer",
        //                Value = "2"
        //            }
        //        }
        //    };

        //    startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

        //    contentIdFilter.Condition = "or";

        //    startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

        //    startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(null).ToList();

        //    startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(new FilterRule()).ToList();

        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        contentIdFilter.Rules.First().Type = "NOT_A_TYPE";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

        //    });

        //    ExceptionAssert.Throws<Exception>(() =>
        //    {
        //        contentIdFilter.Rules.First().Type = "integer";
        //        contentIdFilter.Rules.First().Operator = "NOT_AN_OPERATOR";
        //        startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
        //    });
        //}

        //private class IndexedClass
        //{
        //    int this[string someIndex]
        //    {
        //        get
        //        {
        //            return 2;
        //        }
        //    }
        //}

        //[Test]
        //public void IndexedExpression_Test()
        //{
        //    var rule = new FilterRule
        //    {
        //        Condition = "and",
        //        Field = "ContentTypeId",
        //        Id = "ContentTypeId",
        //        Input = "NA",
        //        Operator = "equal",
        //        Type = "integer",
        //        Value = "2"
        //    };

        //    var result = new List<IndexedClass> { new IndexedClass() }.AsQueryable().BuildQuery(rule,
        //        new BuildExpressionOptions() { UseIndexedProperty = true, IndexedPropertyName = "Item" });
        //    Assert.IsTrue(result.Any());

        //    rule.Value = "3";
        //    result = new[] { new IndexedClass() }.BuildQuery(rule, true, "Item");
        //    Assert.IsFalse(result.Any());
        //}
        #endregion
    }
}
