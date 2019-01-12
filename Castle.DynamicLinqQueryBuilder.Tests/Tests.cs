using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

using NUnit.Framework;

namespace Castle.DynamicLinqQueryBuilder.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class Tests
    {
        #region Expression Tree Builder

        public class ExpressionTreeBuilderTestClass
        {
            public int ContentTypeId { get; set; }
            public int? NullableContentTypeId { get; set; }
            public List<int> Enemies { get; set; }
            public List<string> Flags { get; set; }
            public string ContentTypeName { get; set; }
            public string LongerTextToFilter { get; set; }
            public bool IsSelected { get; set; }
            public bool? IsPossiblyNotSetBool { get; set; }
            public DateTime LastModified { get; set; }
            public DateTime? LastModifiedIfPresent { get; set; }
            public double StatValue { get; set; }
            public double? PossiblyEmptyStatValue { get; set; }
            public List<int> IntList { get; set; }
            public List<int?> IntNullList { get; set; }
            public List<DateTime> DateList { get; set; }
            public List<double> DoubleList { get; set; }
            public List<string> StrList { get; set; }

        }

        private List<ExpressionTreeBuilderTestClass> GetDateExpressionTreeData()
        {
            var tData = new List<ExpressionTreeBuilderTestClass>();

            var entry1 = new ExpressionTreeBuilderTestClass()
            {
                ContentTypeId = 1,
                ContentTypeName = "Multiple-Choice",
                Enemies = new List<int>(),
                Flags = new List<string>(),
                IsPossiblyNotSetBool = true,
                IsSelected = true,
                LastModified = DateTime.Parse("2/23/2016", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                LastModifiedIfPresent = DateTime.UtcNow.Date,
                LongerTextToFilter = "There is something interesting about this text",
                NullableContentTypeId = 1,
                PossiblyEmptyStatValue = null,
                StatValue = 1.11,
                IntList = new List<int>() { 1, 3, 5, 7 },
                StrList = new List<string>() { "Str1", "Str2" },
                DateList = new List<DateTime>() { DateTime.Parse("2/23/2016", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal), DateTime.UtcNow.Date.AddDays(-2) },
                DoubleList = new List<double>() { 1.48, 1.84, 1.33 },
                IntNullList = new List<int?>() { 3, 4, 5, null }
            };
            tData.Add(entry1);

            return tData;
        }

        private List<ExpressionTreeBuilderTestClass> GetExpressionTreeData()
        {
            var tData = new List<ExpressionTreeBuilderTestClass>();

            var entry1 = new ExpressionTreeBuilderTestClass()
            {
                ContentTypeId = 1,
                ContentTypeName = "Multiple-Choice",
                Enemies = new List<int>(),
                Flags = new List<string>(),
                IsPossiblyNotSetBool = true,
                IsSelected = true,
                LastModified = DateTime.UtcNow.Date,
                LastModifiedIfPresent = DateTime.UtcNow.Date,
                LongerTextToFilter = "There is something interesting about this text",
                NullableContentTypeId = 1,
                PossiblyEmptyStatValue = null,
                StatValue = 1.11,
                IntList = new List<int>() { 1, 3, 5, 7 },
                StrList = new List<string>() { "Str1", "Str2" },
                DateList = new List<DateTime>() { DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(-2) },
                DoubleList = new List<double>() { 1.48, 1.84, 1.33 },
                IntNullList = new List<int?>() { 3, 4, 5, null }
            };
            tData.Add(entry1);

            var entry2 = new ExpressionTreeBuilderTestClass()
            {
                ContentTypeId = 2,
                ContentTypeName = "Multiple-Select",
                Enemies = null,
                Flags = null,
                IsPossiblyNotSetBool = false,
                IsSelected = false,
                LastModified = DateTime.UtcNow.Date,
                LastModifiedIfPresent = DateTime.UtcNow.Date,
                LongerTextToFilter = null,
                NullableContentTypeId = 2,
                PossiblyEmptyStatValue = 1.112,
                StatValue = 1.12,
                IntList = new List<int>() { 5, 7 },
                StrList = new List<string>() { "Str1", "Str2" },
                DateList = new List<DateTime>() { DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(-2) },
                DoubleList = new List<double>() { 1.48, 1.84, 1.33 },
                IntNullList = new List<int?>() { 3, 4, 5, null }
            };
            tData.Add(entry2);

            var entry3 = new ExpressionTreeBuilderTestClass()
            {
                ContentTypeId = 3,
                ContentTypeName = "Drag-and-Drop Item",
                Enemies = new List<int>() { 3391, 3985 },
                Flags = new List<string>() { "this is a flag" },
                IsPossiblyNotSetBool = null,
                IsSelected = true,
                LastModified = DateTime.UtcNow.Date,
                LastModifiedIfPresent = null,
                LongerTextToFilter = "There is something interesting about this text",
                NullableContentTypeId = 3,
                PossiblyEmptyStatValue = null,
                StatValue = 1.13,
                IntList = new List<int>() { 1, 3, 5, 7 },
                StrList = new List<string>() { "Str1", "" },
                DateList = new List<DateTime>() { DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(-2) },
                DoubleList = new List<double>() { 1.48, 1.84, 1.33 },
                IntNullList = new List<int?>() { 3, 4, 5, null }
            };
            tData.Add(entry3);

            var entry4 = new ExpressionTreeBuilderTestClass()
            {
                ContentTypeId = 1,
                ContentTypeName = "Multiple-Choice",
                Enemies = new List<int>(),
                Flags = new List<string>() { "THIS IS A FLAG" },
                IsPossiblyNotSetBool = true,
                IsSelected = true,
                LastModified = DateTime.UtcNow.Date,
                LastModifiedIfPresent = DateTime.UtcNow.Date,
                LongerTextToFilter = "THERE IS SOMETHING INTERESTING ABOUT THIS TEXT",
                NullableContentTypeId = null,
                PossiblyEmptyStatValue = 1.112,
                StatValue = 1.11,
                IntList = new List<int>() { 1, 3, 5, 7 },
                StrList = new List<string>() { "Str1", "Str2" },
                DateList = new List<DateTime>() { DateTime.UtcNow.Date },
                DoubleList = new List<double>() { 1.48, },
                IntNullList = new List<int?>() { 3, 4, null, null }
            };
            tData.Add(entry4);


            return tData;
        }

        [Test]
        public void DateHandling()
        {
            QueryBuilder.ParseDatesAsUtc = true;
            var startingQuery = GetDateExpressionTreeData().AsQueryable();
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var queryable = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter);
            var contentIdFilteredList = queryable.ToList();
            Assert.IsTrue(contentIdFilteredList != null);
            Assert.IsTrue(contentIdFilteredList.Count == 1);

            contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
                var contentIdFilteredListNull1 = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            });
            
            
            contentIdFilter = new FilterRule()
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
                        Type = "date",
                        Value = "2/23/2016"
                    }
                }
            };
            queryable = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter);
            var contentIdFilteredList2 = queryable.ToList();
            Assert.IsTrue(contentIdFilteredList2 != null);
            Assert.IsTrue(contentIdFilteredList2.Count == 1);

            contentIdFilter = new FilterRule()
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
                        Operator = "between",
                        Type = "date",
                        Value = ""
                    }
                }
            };
            ExceptionAssert.Throws<Exception>(() =>
            {
                var contentIdFilteredListNull2 = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });
            
            QueryBuilder.ParseDatesAsUtc = false;
        }

        [Test]
        public void InClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect two entries to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = "[1,2]"
                    }
                }
            };
            var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            Assert.IsTrue(contentIdFilteredList != null);
            Assert.IsTrue(contentIdFilteredList.Count == 3);
            Assert.IsTrue(contentIdFilteredList.All(p => (new List<int>() { 1, 2 }).Contains(p.ContentTypeId)));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            //single value test
            contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = "[1]"
                    }
                }
            };
            contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            Assert.IsTrue(contentIdFilteredList != null);
            Assert.IsTrue(contentIdFilteredList.Count == 2);
            Assert.IsTrue(contentIdFilteredList.All(p => (new List<int>() { 1 }).Contains(p.ContentTypeId)));

            //expect two entries to match for an integer comparison
            var nullableContentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = "[1,2]"
                    }
                }
            };
            var nullableContentIdFilteredList =
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
            Assert.IsTrue(nullableContentIdFilteredList != null);
            Assert.IsTrue(nullableContentIdFilteredList.Count == 2);
            Assert.IsTrue(
                nullableContentIdFilteredList.All(p => (new List<int>() { 1, 2 }).Contains(p.NullableContentTypeId.Value)));




            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "in",
                        Type = "string",
                        Value = "there is something interesting about this text,there is something interesting about this text2"
                    }
                }
            };
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            Assert.IsTrue(longerTextToFilterList != null);
            Assert.IsTrue(longerTextToFilterList.Count == 3);
            Assert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter.ToLower())
                    .All(p => p == "there is something interesting about this text"));

            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilterCaps = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "in",
                        Type = "string",
                        Value = "THERE is something interesting about this text,there is something interesting about this text2"
                    }
                }
            };
            var longerTextToFilterListCaps = startingQuery.BuildQuery(longerTextToFilterFilterCaps).ToList();
            Assert.IsTrue(longerTextToFilterListCaps != null);
            Assert.IsTrue(longerTextToFilterListCaps.Count == 3);
            Assert.IsTrue(
                longerTextToFilterListCaps.Select(p => p.LongerTextToFilter.ToLower())
                    .All(p => p == "there is something interesting about this text"));


            //expect 4 entries to match for a Date comparison
            var lastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "in",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
            Assert.IsTrue(lastModifiedFilterList != null);
            Assert.IsTrue(lastModifiedFilterList.Count == 4);
            Assert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => p == DateTime.UtcNow.Date));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

            });

            //expect 3 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "in",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.Date.AddDays(1).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            Assert.IsTrue(nullableLastModifiedFilterList != null);
            Assert.IsTrue(nullableLastModifiedFilterList.Count == 3);
            Assert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => p == DateTime.UtcNow.Date));


            //expect 2 entries to match for a double field
            var statValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "in",
                        Type = "double",
                        Value = "1.11,1.12"
                    }
                }
            };
            var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
            Assert.IsTrue(statValueFilterList != null);
            Assert.IsTrue(statValueFilterList.Count == 3);
            Assert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => (new List<double>() { 1.11, 1.12 }).Contains(p)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable double field
            var nullableStatValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "in",
                        Type = "double",
                        Value = "1.112, 1.113"
                    }
                }
            };
            var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
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
            var dateListFilterList = startingQuery.ToList().BuildQuery(dateListFilter).ToList();
            Assert.IsTrue(dateListFilterList != null);
            Assert.IsTrue(dateListFilterList.Count == 3);
            Assert.IsTrue(dateListFilterList.All(p => p.DateList.Contains(DateTime.UtcNow.Date.AddDays(-2))));

            //expect failure when an invalid date is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                dateListFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(dateListFilter).ToList();

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
            var strListFilterList = startingQuery.AsEnumerable().BuildQuery(strListFilter).ToList();
            Assert.IsTrue(strListFilterList != null);
            Assert.IsTrue(strListFilterList.Count == 3);
            Assert.IsTrue(strListFilterList.All(p => p.StrList.Contains("Str2")));









            //expect 2 entries to match for a List<int> field
            var intListFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "IntList",
                        Id = "IntList",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = "1,3"
                    }
                }
            };
            var intListFilterList = startingQuery.BuildQuery(intListFilter).ToList();
            Assert.IsTrue(intListFilterList != null);
            Assert.IsTrue(intListFilterList.Count == 3);
            Assert.IsTrue(intListFilterList.All(p => p.IntList.Contains(1) || p.IntList.Contains(3)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                intListFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(intListFilter).ToList();

            });

            //expect 2 entries to match for a nullable nullable int field
            var nullableIntListFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "IntNullList",
                        Id = "IntNullList",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = "5"
                    }
                }
            };
            var nullableIntListList = startingQuery.BuildQuery(nullableIntListFilter).ToList();
            Assert.IsTrue(nullableIntListList != null);
            Assert.IsTrue(nullableIntListList.Count == 3);
            Assert.IsTrue(
                nullableIntListList.All(p => p.IntNullList.Contains(5)));


            startingQuery = GetExpressionTreeData().AsQueryable();
            var multipleWithBlankRule = new FilterRule()
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
                        Value = "[,Str2]"
                    }
                }
            };
            var multipleWithBlankList = startingQuery.BuildQuery(multipleWithBlankRule).ToList();
            Assert.IsTrue(multipleWithBlankList != null);
            Assert.IsTrue(multipleWithBlankList.Count == 4);
            Assert.IsTrue(
                multipleWithBlankList.All(p => p.StrList.Contains("") || p.StrList.Contains("Str2")));
        }

        [Test]
        public void NotInClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect two entries to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "integer",
                        Value = "[1,2]"
                    }
                }
            };
            var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            Assert.IsTrue(contentIdFilteredList != null);
            Assert.IsTrue(contentIdFilteredList.Count == 1);
            Assert.IsTrue(contentIdFilteredList.All(p => (new List<int>() { 3 }).Contains(p.ContentTypeId)));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            //expect two entries to match for an integer comparison
            var nullableContentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "integer",
                        Value = "[1,2]"
                    }
                }
            };
            var nullableContentIdFilteredList =
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
            Assert.IsTrue(nullableContentIdFilteredList != null);
            Assert.IsTrue(nullableContentIdFilteredList.Count == 2);
            Assert.IsTrue(
                nullableContentIdFilteredList.All(p => !(new List<int>() { 1, 2 }).Contains(p.NullableContentTypeId.GetValueOrDefault())));




            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "string",
                        Value = "there is something interesting about this text"
                    }
                }
            };
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            Assert.IsTrue(longerTextToFilterList != null);
            Assert.IsTrue(longerTextToFilterList.Count == 1);
            Assert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => p == null || p != "there is something interesting about this text"));


            //expect 4 entries to match for a Date comparison
            var lastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
            Assert.IsTrue(lastModifiedFilterList != null);
            Assert.IsTrue(lastModifiedFilterList.Count == 0);
            Assert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => p == DateTime.UtcNow.Date));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

            });

            //expect 3 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.Date.AddDays(1).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            Assert.IsTrue(nullableLastModifiedFilterList != null);
            Assert.IsTrue(nullableLastModifiedFilterList.Count == 1);
            Assert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => p != DateTime.UtcNow.Date));


            //expect 2 entries to match for a double field
            var statValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "double",
                        Value = "1.11,1.12"
                    }
                }
            };
            var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
            Assert.IsTrue(statValueFilterList != null);
            Assert.IsTrue(statValueFilterList.Count == 1);
            Assert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => !(new List<double>() { 1.11, 1.12 }).Contains(p)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable double field
            var nullableStatValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "double",
                        Value = "1.112, 1.113"
                    }
                }
            };
            var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
            Assert.IsTrue(nullableStatFilterList != null);
            Assert.IsTrue(nullableStatFilterList.Count == 2);
            Assert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p != 1.112));







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
                        Operator = "not_in",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var dateListFilterList = startingQuery.BuildQuery(dateListFilter).ToList();
            Assert.IsTrue(dateListFilterList != null);
            Assert.IsTrue(dateListFilterList.Count == 1);
            Assert.IsTrue(dateListFilterList.All(p => !p.DateList.Contains(DateTime.UtcNow.Date.AddDays(-2))));

            //expect failure when an invalid date is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                dateListFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(dateListFilter).ToList();

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
                        Operator = "not_in",
                        Type = "string",
                        Value = "Str2"
                    }
                }
            };
            var strListFilterList = startingQuery.BuildQuery(strListFilter).ToList();
            Assert.IsTrue(strListFilterList != null);
            Assert.IsTrue(strListFilterList.Count == 1);
            Assert.IsTrue(strListFilterList.All(p => !p.StrList.Contains("Str2")));









            //expect 2 entries to match for a List<int> field
            var intListFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "IntList",
                        Id = "IntList",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "integer",
                        Value = "1,3"
                    }
                }
            };
            var intListFilterList = startingQuery.BuildQuery(intListFilter).ToList();
            Assert.IsTrue(intListFilterList != null);
            Assert.IsTrue(intListFilterList.Count == 1);
            Assert.IsTrue(intListFilterList.All(p => !p.IntList.Contains(1) && !p.IntList.Contains(3)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                intListFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(intListFilter).ToList();

            });

            //expect 2 entries to match for a nullable nullable int field
            var nullableIntListFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "IntNullList",
                        Id = "IntNullList",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "integer",
                        Value = "5"
                    }
                }
            };
            var nullableIntListList = startingQuery.BuildQuery(nullableIntListFilter).ToList();
            Assert.IsTrue(nullableIntListList != null);
            Assert.IsTrue(nullableIntListList.Count == 1);
            Assert.IsTrue(
                nullableIntListList.All(p => !p.IntNullList.Contains(5)));



        }

        [Test]
        public void IsNullClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();

            //expect 1 entries to match for a case-insensitive string comparison (nullable type)
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "is_null",
                        Type = "string",
                        Value = ""
                    }
                }
            };
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            Assert.IsTrue(longerTextToFilterList != null);
            Assert.IsTrue(longerTextToFilterList.Count == 1);
            Assert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => p == null));


            //expect 0 entries to match for a non-nullable type
            var contentTypeIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "is_null",
                        Type = "integer",
                        Value = ""
                    }
                }
            };
            var contentTypeIdFilterList = startingQuery.BuildQuery(contentTypeIdFilter).ToList();
            Assert.IsTrue(contentTypeIdFilterList != null);
            Assert.IsTrue(contentTypeIdFilterList.Count == 0);
            Assert.IsTrue(
                contentTypeIdFilterList.Select(p => p.ContentTypeId)
                    .All(p => p == 0));

        }

        [Test]
        public void IsNotNullClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();

            //expect 3 entries to match for a case-insensitive string comparison (nullable type)
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "is_not_null",
                        Type = "string",
                        Value = ""
                    }
                }
            };
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            Assert.IsTrue(longerTextToFilterList != null);
            Assert.IsTrue(longerTextToFilterList.Count == 3);
            Assert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => p != null));


            //expect 0 entries to match for a non-nullable type
            var contentTypeIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "is_not_null",
                        Type = "integer",
                        Value = ""
                    }
                }
            };
            var contentTypeIdFilterList = startingQuery.BuildQuery(contentTypeIdFilter).ToList();
            Assert.IsTrue(contentTypeIdFilterList != null);
            Assert.IsTrue(contentTypeIdFilterList.Count == 4);
            Assert.IsTrue(
                contentTypeIdFilterList.Select(p => p.ContentTypeId)
                    .All(p => p != 0));

        }

        [Test]
        public void IsEmptyClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();

            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "is_empty",
                        Type = "string",
                        Value = ""
                    }
                }
            };
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            Assert.IsTrue(longerTextToFilterList != null);
            Assert.IsTrue(longerTextToFilterList.Count == 0);


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
                        Operator = "is_empty",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var dateListFilterList = startingQuery.BuildQuery(dateListFilter).ToList();
            Assert.IsTrue(dateListFilterList != null);
            Assert.IsTrue(dateListFilterList.Count == 0);
            //Assert.IsTrue(dateListFilterList.All(p => !p.DateList.Contains(DateTime.UtcNow.Date.AddDays(-2))));



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
                        Operator = "is_empty",
                        Type = "string",
                        Value = "Str2"
                    }
                }
            };
            var strListFilterList = startingQuery.BuildQuery(strListFilter).ToList();
            Assert.IsTrue(strListFilterList != null);
            Assert.IsTrue(strListFilterList.Count == 0);
            //Assert.IsTrue(strListFilterList.All(p => !p.StrList.Contains("Str2")));


            //expect 2 entries to match for a List<int> field
            var intListFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "IntList",
                        Id = "IntList",
                        Input = "NA",
                        Operator = "is_empty",
                        Type = "integer",
                        Value = "1,3"
                    }
                }
            };
            var intListFilterList = startingQuery.BuildQuery(intListFilter).ToList();
            Assert.IsTrue(intListFilterList != null);
            Assert.IsTrue(intListFilterList.Count == 0);



        }

        [Test]
        public void IsNotEmptyClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();

            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "is_not_empty",
                        Type = "string",
                        Value = ""
                    }
                }
            };
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            Assert.IsTrue(longerTextToFilterList != null);
            Assert.IsTrue(longerTextToFilterList.Count == 4);
            Assert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => p == null || p.Length > 0));


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
                        Operator = "is_not_empty",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var dateListFilterList = startingQuery.BuildQuery(dateListFilter).ToList();
            Assert.IsTrue(dateListFilterList != null);
            Assert.IsTrue(dateListFilterList.Count == 4);
            //Assert.IsTrue(dateListFilterList.All(p => !p.DateList.Contains(DateTime.UtcNow.Date.AddDays(-2))));



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
                        Operator = "is_not_empty",
                        Type = "string",
                        Value = "Str2"
                    }
                }
            };
            var strListFilterList = startingQuery.BuildQuery(strListFilter).ToList();
            Assert.IsTrue(strListFilterList != null);
            Assert.IsTrue(strListFilterList.Count == 4);
            //Assert.IsTrue(strListFilterList.All(p => !p.StrList.Contains("Str2")));


            //expect 2 entries to match for a List<int> field
            var intListFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "IntList",
                        Id = "IntList",
                        Input = "NA",
                        Operator = "is_not_empty",
                        Type = "integer",
                        Value = "1,3"
                    }
                }
            };
            var intListFilterList = startingQuery.BuildQuery(intListFilter).ToList();
            Assert.IsTrue(intListFilterList != null);
            Assert.IsTrue(intListFilterList.Count == 4);

        }

        [Test]
        public void ContainsClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();

            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "contains",
                        Type = "string",
                        Value = "something interesting"
                    }
                }
            };
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            Assert.IsTrue(longerTextToFilterList != null);
            Assert.IsTrue(longerTextToFilterList.Count == 3);
            Assert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter.ToLower())
                    .All(p => p.Contains("something interesting")));

        }

        [Test]
        public void NotContainsClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();

            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "not_contains",
                        Type = "string",
                        Value = "something interesting"
                    }
                }
            };
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            Assert.IsTrue(longerTextToFilterList != null);
            Assert.IsTrue(longerTextToFilterList.Count == 1);
            Assert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => p == null));

        }

        [Test]
        public void NotEndsWithClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();

            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "not_ends_with",
                        Type = "string",
                        Value = "about this text"
                    }
                }
            };
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            Assert.IsTrue(longerTextToFilterList != null);
            Assert.IsTrue(longerTextToFilterList.Count == 1);
            Assert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => p == null));

        }

        [Test]
        public void EndsWithClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();

            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "ends_with",
                        Type = "string",
                        Value = "about this text"
                    }
                }
            };
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            Assert.IsTrue(longerTextToFilterList != null);
            Assert.IsTrue(longerTextToFilterList.Count == 3);
            Assert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter.ToLower())
                    .All(p => p.EndsWith("about this text")));

        }

        [Test]
        public void NotBeginsWithClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();

            //expect 1 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "not_begins_with",
                        Type = "string",
                        Value = "there is something"
                    }
                }
            };
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            Assert.IsTrue(longerTextToFilterList != null);
            Assert.IsTrue(longerTextToFilterList.Count == 1);
            Assert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => p == null));

        }

        [Test]
        public void BeginsWithClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();

            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "begins_with",
                        Type = "string",
                        Value = "there is something"
                    }
                }
            };
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            Assert.IsTrue(longerTextToFilterList != null);
            Assert.IsTrue(longerTextToFilterList.Count == 3);
            Assert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter.ToLower())
                    .All(p => p.StartsWith("there is something")));

        }

        [Test]
        public void EqualsClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect two entries to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "equal",
                        Type = "integer",
                        Value = "1"
                    }
                }
            };
            var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            Assert.IsTrue(contentIdFilteredList != null);
            Assert.IsTrue(contentIdFilteredList.Count == 2);
            Assert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId == 1));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            //expect two entries to match for an integer comparison
            var nullableContentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "equal",
                        Type = "integer",
                        Value = "1"
                    }
                }
            };
            var nullableContentIdFilteredList =
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
            Assert.IsTrue(nullableContentIdFilteredList != null);
            Assert.IsTrue(nullableContentIdFilteredList.Count == 1);
            Assert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId == 1));




            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "equal",
                        Type = "string",
                        Value = "there is something interesting about this text"
                    }
                }
            };
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            Assert.IsTrue(longerTextToFilterList != null);
            Assert.IsTrue(longerTextToFilterList.Count == 3);
            Assert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter.ToLower())
                    .All(p => p == "there is something interesting about this text"));


            //expect 4 entries to match for a Date comparison
            var lastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "equal",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
            Assert.IsTrue(lastModifiedFilterList != null);
            Assert.IsTrue(lastModifiedFilterList.Count == 4);
            Assert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => p == DateTime.UtcNow.Date));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

            });

            //expect 3 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "equal",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            Assert.IsTrue(nullableLastModifiedFilterList != null);
            Assert.IsTrue(nullableLastModifiedFilterList.Count == 3);
            Assert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => p == DateTime.UtcNow.Date));







            //expect 3 entries to match for a boolean field
            var isSelectedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "IsSelected",
                        Id = "IsSelected",
                        Input = "NA",
                        Operator = "equal",
                        Type = "boolean",
                        Value = "true"
                    }
                }
            };
            var isSelectedFilterList = startingQuery.BuildQuery(isSelectedFilter).ToList();
            Assert.IsTrue(isSelectedFilterList != null);
            Assert.IsTrue(isSelectedFilterList.Count == 3);
            Assert.IsTrue(
                isSelectedFilterList.Select(p => p.IsSelected)
                    .All(p => p == true));

            //expect failure when an invalid bool is encountered in bool comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                isSelectedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(isSelectedFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableIsSelectedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "IsPossiblyNotSetBool",
                        Id = "IsPossiblyNotSetBool",
                        Input = "NA",
                        Operator = "equal",
                        Type = "boolean",
                        Value = "true"
                    }
                }
            };
            var nullableIsSelectedFilterList = startingQuery.BuildQuery(nullableIsSelectedFilter).ToList();
            Assert.IsTrue(nullableIsSelectedFilterList != null);
            Assert.IsTrue(nullableIsSelectedFilterList.Count == 2);
            Assert.IsTrue(
                nullableIsSelectedFilterList.Select(p => p.IsPossiblyNotSetBool)
                    .All(p => p == true));


            //expect 2 entries to match for a double field
            var statValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "equal",
                        Type = "double",
                        Value = "1.11"
                    }
                }
            };
            var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
            Assert.IsTrue(statValueFilterList != null);
            Assert.IsTrue(statValueFilterList.Count == 2);
            Assert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => p == 1.11));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableStatValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "equal",
                        Type = "double",
                        Value = "1.112"
                    }
                }
            };
            var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
            Assert.IsTrue(nullableStatFilterList != null);
            Assert.IsTrue(nullableStatFilterList.Count == 2);
            Assert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p == 1.112));



        }

        [Test]
        public void NotEqualsClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect two entries to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "integer",
                        Value = "1"
                    }
                }
            };
            var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            Assert.IsTrue(contentIdFilteredList != null);
            Assert.IsTrue(contentIdFilteredList.Count == 2);
            Assert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId != 1));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            //expect 3 entries to match for an integer comparison
            var nullableContentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "integer",
                        Value = "1"
                    }
                }
            };
            var nullableContentIdFilteredList =
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
            Assert.IsTrue(nullableContentIdFilteredList != null);
            Assert.IsTrue(nullableContentIdFilteredList.Count == 3);
            Assert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId != 1));




            //expect 1 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "string",
                        Value = "there is something interesting about this text"
                    }
                }
            };
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            Assert.IsTrue(longerTextToFilterList != null);
            Assert.IsTrue(longerTextToFilterList.Count == 1);
            Assert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => (p == null) || (p.ToLower() != "there is something interesting about this text")));


            //expect 0 entries to match for a Date comparison
            var lastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
            Assert.IsTrue(lastModifiedFilterList != null);
            Assert.IsTrue(lastModifiedFilterList.Count == 0);
            Assert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => p == DateTime.UtcNow.Date));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

            });

            //expect 1 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            Assert.IsTrue(nullableLastModifiedFilterList != null);
            Assert.IsTrue(nullableLastModifiedFilterList.Count == 1);
            Assert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => p != DateTime.UtcNow.Date));


            //expect 1 entries to match for a boolean field
            var isSelectedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "IsSelected",
                        Id = "IsSelected",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "boolean",
                        Value = "true"
                    }
                }
            };
            var isSelectedFilterList = startingQuery.BuildQuery(isSelectedFilter).ToList();
            Assert.IsTrue(isSelectedFilterList != null);
            Assert.IsTrue(isSelectedFilterList.Count == 1);
            Assert.IsTrue(
                isSelectedFilterList.Select(p => p.IsSelected)
                    .All(p => p != true));

            //expect failure when an invalid bool is encountered in bool comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                isSelectedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(isSelectedFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableIsSelectedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "IsPossiblyNotSetBool",
                        Id = "IsPossiblyNotSetBool",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "boolean",
                        Value = "true"
                    }
                }
            };
            var nullableIsSelectedFilterList = startingQuery.BuildQuery(nullableIsSelectedFilter).ToList();
            Assert.IsTrue(nullableIsSelectedFilterList != null);
            Assert.IsTrue(nullableIsSelectedFilterList.Count == 2);
            Assert.IsTrue(
                nullableIsSelectedFilterList.Select(p => p.IsPossiblyNotSetBool)
                    .All(p => p != true));


            //expect 2 entries to match for a double field
            var statValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "double",
                        Value = "1.11"
                    }
                }
            };
            var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
            Assert.IsTrue(statValueFilterList != null);
            Assert.IsTrue(statValueFilterList.Count == 2);
            Assert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => p != 1.11));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableStatValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "double",
                        Value = "1.112"
                    }
                }
            };
            var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
            Assert.IsTrue(nullableStatFilterList != null);
            Assert.IsTrue(nullableStatFilterList.Count == 2);
            Assert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p != 1.112));

        }

        [Test]
        public void BetweenClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect 3 entries to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "between",
                        Type = "integer",
                        Value = "1,2"
                    }
                }
            };
            var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            Assert.IsTrue(contentIdFilteredList != null);
            Assert.IsTrue(contentIdFilteredList.Count == 3);
            Assert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId < 3));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            //expect 2 entries to match for an integer comparison
            var nullableContentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "between",
                        Type = "integer",
                        Value = "1,2"
                    }
                }
            };
            var nullableContentIdFilteredList =
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
            Assert.IsTrue(nullableContentIdFilteredList != null);
            Assert.IsTrue(nullableContentIdFilteredList.Count == 2);
            Assert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId < 3));





            //expect 4 entries to match for a Date comparison
            var lastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "between",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
            Assert.IsTrue(lastModifiedFilterList != null);
            Assert.IsTrue(lastModifiedFilterList.Count == 4);
            Assert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => (p >= DateTime.UtcNow.Date.AddDays(-2)) && (p <= DateTime.UtcNow.Date)));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

            });

            //expect 3 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "between",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            Assert.IsTrue(nullableLastModifiedFilterList != null);
            Assert.IsTrue(nullableLastModifiedFilterList.Count == 3);
            Assert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => (p >= DateTime.UtcNow.Date.AddDays(-2)) && (p <= DateTime.UtcNow.Date)));


            //expect 3 entries to match for a double field
            var statValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "between",
                        Type = "double",
                        Value = "1.0,1.12"
                    }
                }
            };
            var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
            Assert.IsTrue(statValueFilterList != null);
            Assert.IsTrue(statValueFilterList.Count == 3);
            Assert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => (p >= 1.0) && (p <= 1.12)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableStatValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "between",
                        Type = "double",
                        Value = "1.112,1.112"
                    }
                }
            };
            var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
            Assert.IsTrue(nullableStatFilterList != null);
            Assert.IsTrue(nullableStatFilterList.Count == 2);
            Assert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p == 1.112));

        }

        [Test]
        public void NotBetweenClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect 1 entry to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "not_between",
                        Type = "integer",
                        Value = "1,2"
                    }
                }
            };
            var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            Assert.IsTrue(contentIdFilteredList != null);
            Assert.IsTrue(contentIdFilteredList.Count == 1);
            Assert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId < 1 || p.ContentTypeId > 2));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            //expect 2 entries to match for an integer comparison
            var nullableContentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "not_between",
                        Type = "integer",
                        Value = "1,2"
                    }
                }
            };
            var nullableContentIdFilteredList =
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
            Assert.IsTrue(nullableContentIdFilteredList != null);
            Assert.IsTrue(nullableContentIdFilteredList.Count == 2);
            Assert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId < 1 || p.NullableContentTypeId > 2 || p.NullableContentTypeId == null));





            //expect 0 entries to match for a Date comparison
            var lastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "not_between",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
            Assert.IsTrue(lastModifiedFilterList != null);
            Assert.IsTrue(lastModifiedFilterList.Count == 0);
            Assert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => (p <= DateTime.UtcNow.Date.AddDays(-2)) && (p >= DateTime.UtcNow.Date)));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

            });

            //expect 1 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "not_between",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            Assert.IsTrue(nullableLastModifiedFilterList != null);
            Assert.IsTrue(nullableLastModifiedFilterList.Count == 1);
            Assert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => (p <= DateTime.UtcNow.Date.AddDays(-2) && p >= DateTime.UtcNow.Date) || p == null ));


            //expect 3 entries to match for a double field
            var statValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "not_between",
                        Type = "double",
                        Value = "1.0,1.12"
                    }
                }
            };
            var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
            Assert.IsTrue(statValueFilterList != null);
            Assert.IsTrue(statValueFilterList.Count == 1);
            Assert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => (p <= 1.0) || (p >= 1.12)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableStatValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "not_between",
                        Type = "double",
                        Value = "1.112,1.112"
                    }
                }
            };
            var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
            Assert.IsTrue(nullableStatFilterList != null);
            Assert.IsTrue(nullableStatFilterList.Count == 2);
            Assert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p != 1.112));

        }

        [Test]
        public void GreaterOrEqualClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect 1 entries to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "greater_or_equal",
                        Type = "integer",
                        Value = "2"
                    }
                }
            };
            var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            Assert.IsTrue(contentIdFilteredList != null);
            Assert.IsTrue(contentIdFilteredList.Count == 2);
            Assert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId >= 2));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            //expect 1 entries to match for an integer comparison
            var nullableContentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "greater_or_equal",
                        Type = "integer",
                        Value = "2"
                    }
                }
            };
            var nullableContentIdFilteredList =
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
            Assert.IsTrue(nullableContentIdFilteredList != null);
            Assert.IsTrue(nullableContentIdFilteredList.Count == 2);
            Assert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId >= 2));


            //expect 4 entries to match for a Date comparison
            var lastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "greater_or_equal",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
            Assert.IsTrue(lastModifiedFilterList != null);
            Assert.IsTrue(lastModifiedFilterList.Count == 4);
            Assert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => (p >= DateTime.UtcNow.Date.AddDays(-2))));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

            });

            //expect 0 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "greater_or_equal",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(1).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            Assert.IsTrue(nullableLastModifiedFilterList != null);
            Assert.IsTrue(nullableLastModifiedFilterList.Count == 0);
            Assert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => (p >= DateTime.UtcNow.Date.AddDays(1))));


            //expect 4 entries to match for a double field
            var statValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "greater_or_equal",
                        Type = "double",
                        Value = "1"
                    }
                }
            };
            var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
            Assert.IsTrue(statValueFilterList != null);
            Assert.IsTrue(statValueFilterList.Count == 4);
            Assert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => (p >= 1)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableStatValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "greater_or_equal",
                        Type = "double",
                        Value = "1.112"
                    }
                }
            };
            var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
            Assert.IsTrue(nullableStatFilterList != null);
            Assert.IsTrue(nullableStatFilterList.Count == 2);
            Assert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p >= 1.112));

        }

        [Test]
        public void GreaterClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect 1 entries to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "greater",
                        Type = "integer",
                        Value = "2"
                    }
                }
            };
            var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            Assert.IsTrue(contentIdFilteredList != null);
            Assert.IsTrue(contentIdFilteredList.Count == 1);
            Assert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId > 2));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            //expect 1 entries to match for an integer comparison
            var nullableContentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "greater",
                        Type = "integer",
                        Value = "2"
                    }
                }
            };
            var nullableContentIdFilteredList =
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
            Assert.IsTrue(nullableContentIdFilteredList != null);
            Assert.IsTrue(nullableContentIdFilteredList.Count == 1);
            Assert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId > 2));


            //expect 4 entries to match for a Date comparison
            var lastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "greater",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
            Assert.IsTrue(lastModifiedFilterList != null);
            Assert.IsTrue(lastModifiedFilterList.Count == 4);
            Assert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => (p > DateTime.UtcNow.Date.AddDays(-2))));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

            });

            //expect 0 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "greater",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(1).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            Assert.IsTrue(nullableLastModifiedFilterList != null);
            Assert.IsTrue(nullableLastModifiedFilterList.Count == 0);
            Assert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => (p > DateTime.UtcNow.Date.AddDays(1))));


            //expect 4 entries to match for a double field
            var statValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "greater",
                        Type = "double",
                        Value = "1"
                    }
                }
            };
            var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
            Assert.IsTrue(statValueFilterList != null);
            Assert.IsTrue(statValueFilterList.Count == 4);
            Assert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => (p > 1)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableStatValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "greater",
                        Type = "double",
                        Value = "1.112"
                    }
                }
            };
            var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
            Assert.IsTrue(nullableStatFilterList != null);
            Assert.IsTrue(nullableStatFilterList.Count == 0);
            Assert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p > 1.112));

        }

        [Test]
        public void LessClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect 2 entries to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "less",
                        Type = "integer",
                        Value = "2"
                    }
                }
            };
            var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            Assert.IsTrue(contentIdFilteredList != null);
            Assert.IsTrue(contentIdFilteredList.Count == 2);
            Assert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId < 2));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            //expect 1 entries to match for an integer comparison
            var nullableContentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "less",
                        Type = "integer",
                        Value = "2"
                    }
                }
            };
            var nullableContentIdFilteredList =
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
            Assert.IsTrue(nullableContentIdFilteredList != null);
            Assert.IsTrue(nullableContentIdFilteredList.Count == 1);
            Assert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId < 2));


            //expect 0 entries to match for a Date comparison
            var lastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "less",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
            Assert.IsTrue(lastModifiedFilterList != null);
            Assert.IsTrue(lastModifiedFilterList.Count == 0);
            Assert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => (p <= DateTime.UtcNow.Date.AddDays(-2))));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

            });

            //expect 3 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "less",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(1).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            Assert.IsTrue(nullableLastModifiedFilterList != null);
            Assert.IsTrue(nullableLastModifiedFilterList.Count == 3);
            Assert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => (p <= DateTime.UtcNow.Date.AddDays(1))));


            //expect 3 entries to match for a double field
            var statValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "less",
                        Type = "double",
                        Value = "1.13"
                    }
                }
            };
            var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
            Assert.IsTrue(statValueFilterList != null);
            Assert.IsTrue(statValueFilterList.Count == 3);
            Assert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => (p <= 1.12)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableStatValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "less",
                        Type = "double",
                        Value = "1.113"
                    }
                }
            };
            var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
            Assert.IsTrue(nullableStatFilterList != null);
            Assert.IsTrue(nullableStatFilterList.Count == 2);
            Assert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p < 1.113));

        }

        [Test]
        public void LessOrEqualClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect 3 entries to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "less_or_equal",
                        Type = "integer",
                        Value = "2"
                    }
                }
            };
            var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            Assert.IsTrue(contentIdFilteredList != null);
            Assert.IsTrue(contentIdFilteredList.Count == 3);
            Assert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId <= 2));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            //expect 2 entries to match for an integer comparison
            var nullableContentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "less_or_equal",
                        Type = "integer",
                        Value = "2"
                    }
                }
            };
            var nullableContentIdFilteredList =
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
            Assert.IsTrue(nullableContentIdFilteredList != null);
            Assert.IsTrue(nullableContentIdFilteredList.Count == 2);
            Assert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId <= 2));


            //expect 0 entries to match for a Date comparison
            var lastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "less_or_equal",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
            Assert.IsTrue(lastModifiedFilterList != null);
            Assert.IsTrue(lastModifiedFilterList.Count == 0);
            Assert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => (p <= DateTime.UtcNow.Date.AddDays(-2))));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

            });

            //expect 3 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "less_or_equal",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(1).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            Assert.IsTrue(nullableLastModifiedFilterList != null);
            Assert.IsTrue(nullableLastModifiedFilterList.Count == 3);
            Assert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => (p <= DateTime.UtcNow.Date.AddDays(1))));


            //expect 3 entries to match for a double field
            var statValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "less_or_equal",
                        Type = "double",
                        Value = "1.13"
                    }
                }
            };
            var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
            Assert.IsTrue(statValueFilterList != null);
            Assert.IsTrue(statValueFilterList.Count == 4);
            Assert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => (p <= 1.13)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable double field
            var nullableStatValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "less_or_equal",
                        Type = "double",
                        Value = "1.113"
                    }
                }
            };
            var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
            Assert.IsTrue(nullableStatFilterList != null);
            Assert.IsTrue(nullableStatFilterList.Count == 2);
            Assert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p <= 1.113));

        }

        [Test]
        public void FilterWithInvalidParameters()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect 3 entries to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "less_or_equal",
                        Type = "integer",
                        Value = "2"
                    },
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "less_or_equal",
                        Type = "integer",
                        Value = "2"
                    }
                }
            };

            startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            contentIdFilter.Condition = "or";

            startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(null).ToList();

            startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(new FilterRule()).ToList();

            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Type = "NOT_A_TYPE";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Type = "integer";
                contentIdFilter.Rules.First().Operator = "NOT_AN_OPERATOR";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            });
        }

        private class IndexedClass
        {
            int this[string someIndex]
            {
                get
                {
                    return 2;
                }
            }
        }

        [Test]
        public void IndexedExpression_Test()
        {
            var rule = new FilterRule
            {
                Condition = "and",
                Field = "ContentTypeId",
                Id = "ContentTypeId",
                Input = "NA",
                Operator = "equal",
                Type = "integer",
                Value = "2"
            };

            var result = new List<IndexedClass> {new IndexedClass()}.AsQueryable().BuildQuery(rule,
                new BuildExpressionOptions() {UseIndexedProperty = true, IndexedPropertyName = "Item"});
            Assert.IsTrue(result.Any());

            rule.Value = "3";
            result = new[] { new IndexedClass() }.BuildQuery(rule, true, "Item");
            Assert.IsFalse(result.Any());
        }
        #endregion

        #region Predicate
        [Test]
        public void Predicate_Test()
        {
            var rule = new FilterRule
            {
                Condition = "and",
                Field = "ContentTypeId",
                Id = "ContentTypeId",
                Input = "NA",
                Operator = "equal",
                Type = "integer",
                Value = "2",
            };


            var predicate = rule.BuildPredicate<IndexedClass>(new BuildExpressionOptions { IndexedPropertyName = "Item", UseIndexedProperty = true});

            var result = new[] {new IndexedClass()}.Where(predicate);
            Assert.IsTrue(result.Any());

            rule.Value = "3";
            result = new[] { new IndexedClass() }.BuildQuery(rule, true, "Item");
            Assert.IsFalse(result.Any());
        }
        [Test]
        public void Build_Predicate_Null_Test()
        {
            FilterRule rule = null;
            var predicate = rule.BuildPredicate<ExpressionTreeBuilderTestClass>(new BuildExpressionOptions() {ParseDatesAsUtc = true},
                out _);

            var resData = GetExpressionTreeData();

            var res = resData.Where(predicate).ToList();

            Assert.IsTrue(res.Count == 4);

        }

        #endregion

        #region Misc
        [Test]
        public void Build_Query_Null_Test()
        {
            FilterRule rule = null;


            var data = GetExpressionTreeData();

            var res = data.AsQueryable().BuildQuery(rule, new BuildExpressionOptions() { ParseDatesAsUtc = true })
                .ToList();

            Assert.IsTrue(res.Count == 4);


        }
        [Test]
        public void ComparePerformanceOfMethods_Test()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect two entries to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = "[1,2]"
                    }
                }
            };
            var sw1 = new Stopwatch();
            sw1.Start();
            for (var x = 0; x < 1000; x++)
            {
                var contentIdFilteredList =
                    startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            }
            sw1.Stop();

            var sw2 = new Stopwatch();
            sw2.Start();
            var predicate =
                contentIdFilter.BuildPredicate<ExpressionTreeBuilderTestClass>(new BuildExpressionOptions()
                    {ParseDatesAsUtc = true});
            for (var x = 0; x < 1000; x++)
            {
                var contentIdFilteredList =
                    startingQuery.Where(predicate).ToList();
            }
            sw2.Stop();
        }
        #endregion


        #region Column Definition Builder

        public class ColumnBuilderTestClass
        {
            public int Age { get; set; }
            public int? FavoriteNumber { get; set; }
            public string Name { get; set; }
            public DateTime Birthday { get; set; }
            public DateTime? FavoriteBirthday { get; set; }
            public double DollarsInWallet { get; set; }
            public double? DesiredDollarsInWallet { get; set; }
#pragma warning disable IDE1006 // Naming Styles
            public string camelCaseField { get; set; }
#pragma warning restore IDE1006 // Naming Styles
            [IgnoreDataMember]
            public int IgnoreField { get; set; }
        }

        [Test]
        public void ColumnBuilderTest()
        {
            var result = typeof (ColumnBuilderTestClass).GetDefaultColumnDefinitionsForType();

            Assert.IsTrue(result.Count == 8);

            result = typeof(ColumnBuilderTestClass).GetDefaultColumnDefinitionsForType(true);

            Assert.IsTrue(result.Count == 8);

        }

        #endregion
    }
}
