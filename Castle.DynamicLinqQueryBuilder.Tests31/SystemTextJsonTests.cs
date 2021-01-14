using Castle.DynamicLinqQueryBuilder.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Castle.DynamicLinqQueryBuilder.Tests31
{
    [ExcludeFromCodeCoverage]
    [TestFixture]

    class SystemTextJsonTests
    {
        IQueryable<Tests.Tests.ExpressionTreeBuilderTestClass> StartingQuery;
        IQueryable<Tests.Tests.ExpressionTreeBuilderTestClass> StartingDateQuery;

        [SetUp]
        public void Setup()
        {
            StartingQuery = Tests.Tests.GetExpressionTreeData().AsQueryable();
            StartingDateQuery = Tests.Tests.GetDateExpressionTreeData().AsQueryable();
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
        public void InClause()
        {
            //expect two entries to match for an integer comparison
            var contentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

            contentIdFilter = JsonSerializer.Deserialize<JsonNetFilterRule>(JsonSerializer.Serialize(contentIdFilter));

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
            contentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

            contentIdFilter = JsonSerializer.Deserialize<JsonNetFilterRule>(JsonSerializer.Serialize(contentIdFilter));

            contentIdFilteredList = StartingQuery.BuildQuery(contentIdFilter).ToList();
            Assert.IsTrue(contentIdFilteredList != null);
            Assert.IsTrue(contentIdFilteredList.Count == 2);
            Assert.IsTrue(contentIdFilteredList.All(p => (new List<int>() { 1 }).Contains(p.ContentTypeId)));

            //expect two entries to match for an integer comparison
            var nullableContentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

            nullableContentIdFilter = JsonSerializer.Deserialize<JsonNetFilterRule>(JsonSerializer.Serialize(nullableContentIdFilter));
            var nullableContentIdFilteredList =
                StartingQuery.BuildQuery(nullableContentIdFilter).ToList();
            Assert.IsTrue(nullableContentIdFilteredList != null);
            Assert.IsTrue(nullableContentIdFilteredList.Count == 2);
            Assert.IsTrue(
                nullableContentIdFilteredList.All(p => (new List<int>() { 1, 2 }).Contains(p.NullableContentTypeId.Value)));




            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

            longerTextToFilterFilter = JsonSerializer.Deserialize<JsonNetFilterRule>(JsonSerializer.Serialize(longerTextToFilterFilter));
            var longerTextToFilterList = StartingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            Assert.IsTrue(longerTextToFilterList != null);
            Assert.IsTrue(longerTextToFilterList.Count == 3);
            Assert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter.ToLower())
                    .All(p => p == "there is something interesting about this text"));

            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilterCaps = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

            longerTextToFilterFilterCaps = JsonSerializer.Deserialize<JsonNetFilterRule>(JsonSerializer.Serialize(longerTextToFilterFilterCaps));
            var longerTextToFilterListCaps = StartingQuery.BuildQuery(longerTextToFilterFilterCaps).ToList();
            Assert.IsTrue(longerTextToFilterListCaps != null);
            Assert.IsTrue(longerTextToFilterListCaps.Count == 3);
            Assert.IsTrue(
                longerTextToFilterListCaps.Select(p => p.LongerTextToFilter.ToLower())
                    .All(p => p == "there is something interesting about this text"));


            //expect 4 entries to match for a Date comparison
            var lastModifiedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

            lastModifiedFilter = JsonSerializer.Deserialize<JsonNetFilterRule>(JsonSerializer.Serialize(lastModifiedFilter));
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
            var nullableLastModifiedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

            nullableLastModifiedFilter = JsonSerializer.Deserialize<JsonNetFilterRule>(JsonSerializer.Serialize(nullableLastModifiedFilter));
            var nullableLastModifiedFilterList = StartingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            Assert.IsTrue(nullableLastModifiedFilterList != null);
            Assert.IsTrue(nullableLastModifiedFilterList.Count == 3);
            Assert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => p == DateTime.UtcNow.Date));


            //expect 2 entries to match for a double field
            var statValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

            statValueFilter = JsonSerializer.Deserialize<JsonNetFilterRule>(JsonSerializer.Serialize(statValueFilter));
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
            var nullableStatValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

            nullableStatValueFilter = JsonSerializer.Deserialize<JsonNetFilterRule>(JsonSerializer.Serialize(nullableStatValueFilter));
            var nullableStatFilterList = StartingQuery.BuildQuery(nullableStatValueFilter).ToList();
            Assert.IsTrue(nullableStatFilterList != null);
            Assert.IsTrue(nullableStatFilterList.Count == 2);
            Assert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p == 1.112));

            //expect 2 entries to match for a List<DateTime> field
            var dateListFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

            dateListFilter = JsonSerializer.Deserialize<JsonNetFilterRule>(JsonSerializer.Serialize(dateListFilter));
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
            var strListFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

            strListFilter = JsonSerializer.Deserialize<JsonNetFilterRule>(JsonSerializer.Serialize(strListFilter));
            var strListFilterList = StartingQuery.AsEnumerable().BuildQuery(strListFilter).ToList();
            Assert.IsTrue(strListFilterList != null);
            Assert.IsTrue(strListFilterList.Count == 3);
            Assert.IsTrue(strListFilterList.All(p => p.StrList.Contains("Str2")));

            //expect 2 entries to match for a List<int> field
            var intListFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

            intListFilter = JsonSerializer.Deserialize<JsonNetFilterRule>(JsonSerializer.Serialize(intListFilter));
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
            var nullableIntListFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

            nullableIntListFilter = JsonSerializer.Deserialize<JsonNetFilterRule>(JsonSerializer.Serialize(nullableIntListFilter));
            var nullableIntListList = StartingQuery.BuildQuery(nullableIntListFilter).ToList();
            Assert.IsTrue(nullableIntListList != null);
            Assert.IsTrue(nullableIntListList.Count == 3);
            Assert.IsTrue(nullableIntListList.All(p => p.IntNullList.Contains(5)));

            var multipleWithBlankRule = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

            multipleWithBlankRule = JsonSerializer.Deserialize<JsonNetFilterRule>(JsonSerializer.Serialize(multipleWithBlankRule));
            var multipleWithBlankList = StartingQuery.BuildQuery(multipleWithBlankRule).ToList();
            Assert.IsTrue(multipleWithBlankList != null);
            Assert.IsTrue(multipleWithBlankList.Count == 4);
            Assert.IsTrue(multipleWithBlankList.All(p => p.StrList.Contains("") || p.StrList.Contains("Str2")));
        }
    }
    
    #endregion
}
