using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;

namespace Castle.DynamicLinqQueryBuilder.Tests.Rules
{
    /// <summary>
    /// Tests for type conversion scenarios, including the HashSet optimization bug fix
    /// and new numeric type support.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class TypeConversionTests
    {
        #region Test Entity Classes

        public class NumericTypesTestClass
        {
            public ushort UShortValue { get; set; }
            public ushort? NullableUShortValue { get; set; }
            public short ShortValue { get; set; }
            public short? NullableShortValue { get; set; }
            public uint UIntValue { get; set; }
            public uint? NullableUIntValue { get; set; }
            public ulong ULongValue { get; set; }
            public ulong? NullableULongValue { get; set; }
            public byte ByteValue { get; set; }
            public byte? NullableByteValue { get; set; }
            public sbyte SByteValue { get; set; }
            public sbyte? NullableSByteValue { get; set; }
            public float FloatValue { get; set; }
            public float? NullableFloatValue { get; set; }
            public decimal DecimalValue { get; set; }
            public decimal? NullableDecimalValue { get; set; }
            public int IntValue { get; set; }
            public string Name { get; set; }
        }

        private static List<NumericTypesTestClass> GetTestData()
        {
            return new List<NumericTypesTestClass>
            {
                new NumericTypesTestClass
                {
                    UShortValue = 100,
                    NullableUShortValue = 100,
                    ShortValue = -50,
                    NullableShortValue = -50,
                    UIntValue = 1000,
                    NullableUIntValue = 1000,
                    ULongValue = 10000,
                    NullableULongValue = 10000,
                    ByteValue = 10,
                    NullableByteValue = 10,
                    SByteValue = -10,
                    NullableSByteValue = -10,
                    FloatValue = 1.5f,
                    NullableFloatValue = 1.5f,
                    DecimalValue = 100.50m,
                    NullableDecimalValue = 100.50m,
                    IntValue = 1,
                    Name = "Item1"
                },
                new NumericTypesTestClass
                {
                    UShortValue = 200,
                    NullableUShortValue = 200,
                    ShortValue = 50,
                    NullableShortValue = 50,
                    UIntValue = 2000,
                    NullableUIntValue = 2000,
                    ULongValue = 20000,
                    NullableULongValue = 20000,
                    ByteValue = 20,
                    NullableByteValue = 20,
                    SByteValue = 10,
                    NullableSByteValue = 10,
                    FloatValue = 2.5f,
                    NullableFloatValue = 2.5f,
                    DecimalValue = 200.50m,
                    NullableDecimalValue = 200.50m,
                    IntValue = 2,
                    Name = "Item2"
                },
                new NumericTypesTestClass
                {
                    UShortValue = 300,
                    NullableUShortValue = null,
                    ShortValue = 100,
                    NullableShortValue = null,
                    UIntValue = 3000,
                    NullableUIntValue = null,
                    ULongValue = 30000,
                    NullableULongValue = null,
                    ByteValue = 30,
                    NullableByteValue = null,
                    SByteValue = 30,
                    NullableSByteValue = null,
                    FloatValue = 3.5f,
                    NullableFloatValue = null,
                    DecimalValue = 300.50m,
                    NullableDecimalValue = null,
                    IntValue = 3,
                    Name = "Item3"
                }
            };
        }

        #endregion

        #region HashSet Optimization Bug Fix Tests (Issue: Int32 to UInt16 conversion)

        /// <summary>
        /// Regression test: This verifies the fix for the bug where using the "in" operator
        /// with 10+ values on a UInt16 property with Type="integer" would throw:
        /// "Object of type 'System.Int32' cannot be converted to type 'System.UInt16'"
        /// 
        /// The bug was in CreateHashSetContainsExpression which created a HashSet&lt;UInt16&gt;
        /// but tried to add Int32 values to it without conversion.
        /// </summary>
        [Test]
        public void InOperator_WithMoreThan10Values_OnUInt16Property_WithIntegerType_ShouldWork()
        {
            var testData = GetTestData();
            var query = testData.AsQueryable();

            // Create 12 values to exceed the HashSet threshold of 10
            var values = Enumerable.Range(95, 12).Select(i => i.ToString()).ToArray();

            var filter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "UShortValue",
                        Id = "UShortValue",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer", // Using integer type for a ushort property
                        Value = values
                    }
                }
            };

            // This should not throw an exception after the fix
            var result = query.BuildQuery(filter).ToList();
            
            // 100 is in the range 95-106, so Item1 should match
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Item1"));
        }

        /// <summary>
        /// Verify that using the correct ushort type also works with 10+ values.
        /// </summary>
        [Test]
        public void InOperator_WithMoreThan10Values_OnUInt16Property_WithUShortType_ShouldWork()
        {
            var testData = GetTestData();
            var query = testData.AsQueryable();

            // Create 12 values to exceed the HashSet threshold of 10
            var values = Enumerable.Range(95, 12).Select(i => i.ToString()).ToArray();

            var filter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "UShortValue",
                        Id = "UShortValue",
                        Input = "NA",
                        Operator = "in",
                        Type = "ushort", // Using the correct type
                        Value = values
                    }
                }
            };

            var result = query.BuildQuery(filter).ToList();
            
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Item1"));
        }

        /// <summary>
        /// Test that nullable UInt16 also works with type conversion.
        /// </summary>
        [Test]
        public void InOperator_WithMoreThan10Values_OnNullableUInt16Property_ShouldWork()
        {
            var testData = GetTestData();
            var query = testData.AsQueryable();

            // Create 12 values to exceed the HashSet threshold of 10
            var values = Enumerable.Range(95, 12).Select(i => i.ToString()).ToArray();

            var filter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "NullableUShortValue",
                        Id = "NullableUShortValue",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = values
                    }
                }
            };

            var result = query.BuildQuery(filter).ToList();
            
            // 100 is in range, but Item3 has null so should not match
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Item1"));
        }

        #endregion

        #region New Numeric Type Support Tests

        [Test]
        public void GetCSharpType_UShort_ReturnsCorrectType()
        {
            Assert.That(QueryBuilder.GetCSharpType("ushort"), Is.EqualTo(typeof(ushort)));
            Assert.That(QueryBuilder.GetCSharpType("uint16"), Is.EqualTo(typeof(ushort)));
        }

        [Test]
        public void GetCSharpType_Short_ReturnsCorrectType()
        {
            Assert.That(QueryBuilder.GetCSharpType("short"), Is.EqualTo(typeof(short)));
            Assert.That(QueryBuilder.GetCSharpType("int16"), Is.EqualTo(typeof(short)));
        }

        [Test]
        public void GetCSharpType_UInt_ReturnsCorrectType()
        {
            Assert.That(QueryBuilder.GetCSharpType("uint"), Is.EqualTo(typeof(uint)));
            Assert.That(QueryBuilder.GetCSharpType("uint32"), Is.EqualTo(typeof(uint)));
        }

        [Test]
        public void GetCSharpType_ULong_ReturnsCorrectType()
        {
            Assert.That(QueryBuilder.GetCSharpType("ulong"), Is.EqualTo(typeof(ulong)));
            Assert.That(QueryBuilder.GetCSharpType("uint64"), Is.EqualTo(typeof(ulong)));
        }

        [Test]
        public void GetCSharpType_Byte_ReturnsCorrectType()
        {
            Assert.That(QueryBuilder.GetCSharpType("byte"), Is.EqualTo(typeof(byte)));
        }

        [Test]
        public void GetCSharpType_SByte_ReturnsCorrectType()
        {
            Assert.That(QueryBuilder.GetCSharpType("sbyte"), Is.EqualTo(typeof(sbyte)));
        }

        [Test]
        public void GetCSharpType_Float_ReturnsCorrectType()
        {
            Assert.That(QueryBuilder.GetCSharpType("float"), Is.EqualTo(typeof(float)));
            Assert.That(QueryBuilder.GetCSharpType("single"), Is.EqualTo(typeof(float)));
        }

        [Test]
        public void GetCSharpType_Decimal_ReturnsCorrectType()
        {
            Assert.That(QueryBuilder.GetCSharpType("decimal"), Is.EqualTo(typeof(decimal)));
        }

        #endregion

        #region Filter Operations with New Types

        [Test]
        public void EqualOperator_WithUShortType_ShouldWork()
        {
            var testData = GetTestData();
            var query = testData.AsQueryable();

            var filter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "UShortValue",
                        Id = "UShortValue",
                        Input = "NA",
                        Operator = "equal",
                        Type = "ushort",
                        Value = new[] { "100" }
                    }
                }
            };

            var result = query.BuildQuery(filter).ToList();
            
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].UShortValue, Is.EqualTo(100));
        }

        [Test]
        public void EqualOperator_WithShortType_ShouldWork()
        {
            var testData = GetTestData();
            var query = testData.AsQueryable();

            var filter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "ShortValue",
                        Id = "ShortValue",
                        Input = "NA",
                        Operator = "equal",
                        Type = "short",
                        Value = new[] { "-50" }
                    }
                }
            };

            var result = query.BuildQuery(filter).ToList();
            
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].ShortValue, Is.EqualTo(-50));
        }

        [Test]
        public void InOperator_WithDecimalType_ShouldWork()
        {
            var testData = GetTestData();
            var query = testData.AsQueryable();

            var filter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "DecimalValue",
                        Id = "DecimalValue",
                        Input = "NA",
                        Operator = "in",
                        Type = "decimal",
                        Value = new[] { "100.50", "200.50" }
                    }
                }
            };

            var result = query.BuildQuery(filter).ToList();
            
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void InOperator_WithFloatType_ShouldWork()
        {
            var testData = GetTestData();
            var query = testData.AsQueryable();

            var filter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "FloatValue",
                        Id = "FloatValue",
                        Input = "NA",
                        Operator = "in",
                        Type = "float",
                        Value = new[] { "1.5", "2.5" }
                    }
                }
            };

            var result = query.BuildQuery(filter).ToList();
            
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void GreaterThanOperator_WithByteType_ShouldWork()
        {
            var testData = GetTestData();
            var query = testData.AsQueryable();

            var filter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "ByteValue",
                        Id = "ByteValue",
                        Input = "NA",
                        Operator = "greater",
                        Type = "byte",
                        Value = new[] { "15" }
                    }
                }
            };

            var result = query.BuildQuery(filter).ToList();
            
            Assert.That(result.Count, Is.EqualTo(2)); // Items with ByteValue 20 and 30
        }

        [Test]
        public void BetweenOperator_WithUIntType_ShouldWork()
        {
            var testData = GetTestData();
            var query = testData.AsQueryable();

            var filter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "UIntValue",
                        Id = "UIntValue",
                        Input = "NA",
                        Operator = "between",
                        Type = "uint",
                        Value = new[] { "1500", "2500" }
                    }
                }
            };

            var result = query.BuildQuery(filter).ToList();
            
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].UIntValue, Is.EqualTo(2000));
        }

        [Test]
        public void LessThanOperator_WithULongType_ShouldWork()
        {
            var testData = GetTestData();
            var query = testData.AsQueryable();

            var filter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "ULongValue",
                        Id = "ULongValue",
                        Input = "NA",
                        Operator = "less",
                        Type = "ulong",
                        Value = new[] { "15000" }
                    }
                }
            };

            var result = query.BuildQuery(filter).ToList();
            
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].ULongValue, Is.EqualTo(10000UL));
        }

        #endregion

        #region Mixed Type Conversion Tests

        /// <summary>
        /// Test that filtering with "integer" type works on properties of various smaller integer types
        /// due to the Convert.ChangeType fix in CreateHashSetContainsExpression.
        /// </summary>
        [Test]
        public void InOperator_WithIntegerType_OnByteProperty_WithManyValues_ShouldWork()
        {
            var testData = GetTestData();
            var query = testData.AsQueryable();

            // Create 12 values to trigger HashSet optimization
            var values = Enumerable.Range(5, 12).Select(i => i.ToString()).ToArray();

            var filter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "ByteValue",
                        Id = "ByteValue",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer", // Using integer type for byte property
                        Value = values
                    }
                }
            };

            var result = query.BuildQuery(filter).ToList();
            
            // 10 is in range 5-16, so Item1 should match
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].ByteValue, Is.EqualTo(10));
        }

        /// <summary>
        /// Test that filtering with "long" type works on UInt32 properties.
        /// </summary>
        [Test]
        public void InOperator_WithLongType_OnUIntProperty_WithManyValues_ShouldWork()
        {
            var testData = GetTestData();
            var query = testData.AsQueryable();

            // Create 12 values to trigger HashSet optimization
            var values = Enumerable.Range(995, 12).Select(i => i.ToString()).ToArray();

            var filter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "UIntValue",
                        Id = "UIntValue",
                        Input = "NA",
                        Operator = "in",
                        Type = "long", // Using long type for uint property
                        Value = values
                    }
                }
            };

            var result = query.BuildQuery(filter).ToList();
            
            // 1000 is in range 995-1006, so Item1 should match
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].UIntValue, Is.EqualTo(1000u));
        }

        #endregion
    }
}
