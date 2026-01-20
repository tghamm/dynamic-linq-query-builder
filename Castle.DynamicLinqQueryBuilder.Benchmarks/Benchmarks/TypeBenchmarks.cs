using BenchmarkDotNet.Attributes;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Data;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Filters;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Models;

namespace Castle.DynamicLinqQueryBuilder.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks for type conversion overhead.
/// Measures GetConstants() and TypeDescriptor.GetConverter() performance per type.
/// </summary>
[MemoryDiagnoser]
[ShortRunJob]
public class TypeBenchmarks
{
    private List<BenchmarkEntity> _data = null!;
    private IQueryable<BenchmarkEntity> _queryable = null!;
    private BuildExpressionOptions _defaultOptions = null!;
    private BuildExpressionOptions _utcOptions = null!;

    // Pre-built filters for each type
    private QueryBuilderFilterRule _integerFilter = null!;
    private QueryBuilderFilterRule _longFilter = null!;
    private QueryBuilderFilterRule _doubleFilter = null!;
    private QueryBuilderFilterRule _stringFilter = null!;
    private QueryBuilderFilterRule _dateFilter = null!;
    private QueryBuilderFilterRule _datetimeFilter = null!;
    private QueryBuilderFilterRule _booleanFilter = null!;
    private QueryBuilderFilterRule _guidFilter = null!;

    // Nullable type filters
    private QueryBuilderFilterRule _nullableIntegerFilter = null!;
    private QueryBuilderFilterRule _nullableLongFilter = null!;
    private QueryBuilderFilterRule _nullableDoubleFilter = null!;
    private QueryBuilderFilterRule _nullableDatetimeFilter = null!;
    private QueryBuilderFilterRule _nullableBooleanFilter = null!;
    private QueryBuilderFilterRule _nullableGuidFilter = null!;

    // In filters with multiple values (tests collection conversion)
    private QueryBuilderFilterRule _integerInFilter = null!;
    private QueryBuilderFilterRule _stringInFilter = null!;
    private QueryBuilderFilterRule _datetimeInFilter = null!;
    private QueryBuilderFilterRule _guidInFilter = null!;

    [GlobalSetup]
    public void Setup()
    {
        _data = DataGenerator.GenerateBenchmarkEntities(1000, seed: 42);
        _queryable = _data.AsQueryable();
        _defaultOptions = new BuildExpressionOptions();
        _utcOptions = new BuildExpressionOptions { ParseDatesAsUtc = true };

        // Standard type filters
        _integerFilter = FilterFactory.CreateSingleRule("equal", "integer", "ContentTypeId", "42");
        _longFilter = FilterFactory.CreateSingleRule("equal", "long", "ContentTypeLong", "100000");
        _doubleFilter = FilterFactory.CreateSingleRule("equal", "double", "StatValue", "50.5");
        _stringFilter = FilterFactory.CreateSingleRule("equal", "string", "ContentTypeName", "Multiple-Choice");
        _dateFilter = FilterFactory.CreateSingleRule("equal", "date", "LastModified", DateTime.UtcNow.Date.ToString("yyyy-MM-dd"));
        _datetimeFilter = FilterFactory.CreateSingleRule("equal", "datetime", "LastModified", DateTime.UtcNow.ToString("o"));
        _booleanFilter = FilterFactory.CreateSingleRule("equal", "boolean", "IsSelected", "true");
        _guidFilter = FilterFactory.CreateSingleRule("equal", "guid", "ContentTypeGuid", Guid.Empty.ToString());

        // Nullable type filters
        _nullableIntegerFilter = FilterFactory.CreateSingleRule("equal", "integer", "NullableContentTypeId", "42");
        _nullableLongFilter = FilterFactory.CreateSingleRule("equal", "long", "NullableContentTypeLong", "100000");
        _nullableDoubleFilter = FilterFactory.CreateSingleRule("equal", "double", "PossiblyEmptyStatValue", "50.5");
        _nullableDatetimeFilter = FilterFactory.CreateSingleRule("equal", "datetime", "NullableDateTime", DateTime.UtcNow.ToString("o"));
        _nullableBooleanFilter = FilterFactory.CreateSingleRule("equal", "boolean", "IsPossiblyNotSetBool", "true");
        _nullableGuidFilter = FilterFactory.CreateSingleRule("equal", "guid", "NullableContentTypeGuid", Guid.NewGuid().ToString());

        // In filters for collection conversion benchmarks
        _integerInFilter = FilterFactory.CreateInFilter("ContentTypeId", "integer", 10);
        _stringInFilter = FilterFactory.CreateInFilter("ContentTypeName", "string", 10);
        _datetimeInFilter = CreateDatetimeInFilter(10);
        _guidInFilter = FilterFactory.CreateInFilter("ContentTypeGuid", "guid", 10);
    }

    private QueryBuilderFilterRule CreateDatetimeInFilter(int count)
    {
        var values = new string[count];
        var baseDate = DateTime.UtcNow;
        for (int i = 0; i < count; i++)
        {
            values[i] = baseDate.AddDays(-i).ToString("o");
        }
        return FilterFactory.CreateSingleRule("in", "datetime", "LastModified", values);
    }

    #region Standard Type Benchmarks

    [Benchmark(Baseline = true)]
    public void Type_Integer()
    {
        _queryable.BuildQuery(_integerFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Type_Long()
    {
        _queryable.BuildQuery(_longFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Type_Double()
    {
        _queryable.BuildQuery(_doubleFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Type_String()
    {
        _queryable.BuildQuery(_stringFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Type_Date()
    {
        _queryable.BuildQuery(_dateFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Type_DateTime_Local()
    {
        _queryable.BuildQuery(_datetimeFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Type_DateTime_UTC()
    {
        _queryable.BuildQuery(_datetimeFilter, _utcOptions).ToList();
    }

    [Benchmark]
    public void Type_Boolean()
    {
        _queryable.BuildQuery(_booleanFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Type_Guid()
    {
        _queryable.BuildQuery(_guidFilter, _defaultOptions).ToList();
    }

    #endregion

    #region Nullable Type Benchmarks

    [Benchmark]
    public void Type_NullableInteger()
    {
        _queryable.BuildQuery(_nullableIntegerFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Type_NullableLong()
    {
        _queryable.BuildQuery(_nullableLongFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Type_NullableDouble()
    {
        _queryable.BuildQuery(_nullableDoubleFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Type_NullableDateTime()
    {
        _queryable.BuildQuery(_nullableDatetimeFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Type_NullableBoolean()
    {
        _queryable.BuildQuery(_nullableBooleanFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Type_NullableGuid()
    {
        _queryable.BuildQuery(_nullableGuidFilter, _defaultOptions).ToList();
    }

    #endregion

    #region Collection Conversion Benchmarks (In operator with multiple values)

    [Benchmark]
    public void TypeCollection_Integer_10Values()
    {
        _queryable.BuildQuery(_integerInFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void TypeCollection_String_10Values()
    {
        _queryable.BuildQuery(_stringInFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void TypeCollection_DateTime_10Values()
    {
        _queryable.BuildQuery(_datetimeInFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void TypeCollection_DateTime_10Values_UTC()
    {
        _queryable.BuildQuery(_datetimeInFilter, _utcOptions).ToList();
    }

    [Benchmark]
    public void TypeCollection_Guid_10Values()
    {
        _queryable.BuildQuery(_guidInFilter, _defaultOptions).ToList();
    }

    #endregion
}
