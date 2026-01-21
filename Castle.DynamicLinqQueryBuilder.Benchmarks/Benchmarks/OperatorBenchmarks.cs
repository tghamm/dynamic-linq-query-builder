using BenchmarkDotNet.Attributes;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Data;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Filters;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Models;

namespace Castle.DynamicLinqQueryBuilder.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks for individual operators to identify expensive operations.
/// Tests each operator with appropriate types.
/// </summary>
[MemoryDiagnoser]
[ShortRunJob]
public class OperatorBenchmarks
{
    private List<BenchmarkEntity> _data = null!;
    private IQueryable<BenchmarkEntity> _queryable = null!;
    private BuildExpressionOptions _defaultOptions = null!;
    private BuildExpressionOptions _caseSensitiveOptions = null!;

    // Pre-built filters for each operator
    private QueryBuilderFilterRule _equalIntFilter = null!;
    private QueryBuilderFilterRule _notEqualIntFilter = null!;
    private QueryBuilderFilterRule _equalStringFilter = null!;
    private QueryBuilderFilterRule _notEqualStringFilter = null!;
    private QueryBuilderFilterRule _inFilter5 = null!;
    private QueryBuilderFilterRule _inFilter50 = null!;
    private QueryBuilderFilterRule _inFilter500 = null!;
    private QueryBuilderFilterRule _notInFilter = null!;
    private QueryBuilderFilterRule _lessFilter = null!;
    private QueryBuilderFilterRule _lessOrEqualFilter = null!;
    private QueryBuilderFilterRule _greaterFilter = null!;
    private QueryBuilderFilterRule _greaterOrEqualFilter = null!;
    private QueryBuilderFilterRule _betweenFilter = null!;
    private QueryBuilderFilterRule _notBetweenFilter = null!;
    private QueryBuilderFilterRule _beginsWithFilter = null!;
    private QueryBuilderFilterRule _notBeginsWithFilter = null!;
    private QueryBuilderFilterRule _containsFilter = null!;
    private QueryBuilderFilterRule _notContainsFilter = null!;
    private QueryBuilderFilterRule _endsWithFilter = null!;
    private QueryBuilderFilterRule _notEndsWithFilter = null!;
    private QueryBuilderFilterRule _isNullFilter = null!;
    private QueryBuilderFilterRule _isNotNullFilter = null!;
    private QueryBuilderFilterRule _isEmptyFilter = null!;
    private QueryBuilderFilterRule _isNotEmptyFilter = null!;

    [GlobalSetup]
    public void Setup()
    {
        _data = DataGenerator.GenerateBenchmarkEntities(1000, seed: 42);
        _queryable = _data.AsQueryable();
        _defaultOptions = new BuildExpressionOptions();
        _caseSensitiveOptions = new BuildExpressionOptions { StringCaseSensitiveComparison = true };

        // Equality operators
        _equalIntFilter = FilterFactory.CreateSingleRule("equal", "integer", "ContentTypeId", "1");
        _notEqualIntFilter = FilterFactory.CreateSingleRule("not_equal", "integer", "ContentTypeId", "1");
        _equalStringFilter = FilterFactory.CreateSingleRule("equal", "string", "ContentTypeName", "Multiple-Choice");
        _notEqualStringFilter = FilterFactory.CreateSingleRule("not_equal", "string", "ContentTypeName", "Multiple-Choice");

        // In/Not In operators with varying sizes
        _inFilter5 = FilterFactory.CreateInFilter("ContentTypeId", "integer", 5);
        _inFilter50 = FilterFactory.CreateInFilter("ContentTypeId", "integer", 50);
        _inFilter500 = FilterFactory.CreateInFilter("ContentTypeId", "integer", 500);
        _notInFilter = FilterFactory.CreateSingleRule("not_in", "integer", "ContentTypeId", "1", "2", "3", "4", "5");

        // Comparison operators
        _lessFilter = FilterFactory.CreateSingleRule("less", "integer", "ContentTypeId", "50");
        _lessOrEqualFilter = FilterFactory.CreateSingleRule("less_or_equal", "integer", "ContentTypeId", "50");
        _greaterFilter = FilterFactory.CreateSingleRule("greater", "integer", "ContentTypeId", "50");
        _greaterOrEqualFilter = FilterFactory.CreateSingleRule("greater_or_equal", "integer", "ContentTypeId", "50");
        _betweenFilter = FilterFactory.CreateBetweenFilter("ContentTypeId", "integer", "10", "90");
        _notBetweenFilter = FilterFactory.CreateSingleRule("not_between", "integer", "ContentTypeId", "10", "90");

        // String operators
        _beginsWithFilter = FilterFactory.CreateSingleRule("begins_with", "string", "ContentTypeName", "Multi");
        _notBeginsWithFilter = FilterFactory.CreateSingleRule("not_begins_with", "string", "ContentTypeName", "Multi");
        _containsFilter = FilterFactory.CreateSingleRule("contains", "string", "ContentTypeName", "Choice");
        _notContainsFilter = FilterFactory.CreateSingleRule("not_contains", "string", "ContentTypeName", "Choice");
        _endsWithFilter = FilterFactory.CreateSingleRule("ends_with", "string", "ContentTypeName", "Choice");
        _notEndsWithFilter = FilterFactory.CreateSingleRule("not_ends_with", "string", "ContentTypeName", "Choice");

        // Null/Empty operators
        _isNullFilter = FilterFactory.CreateSingleRule("is_null", "string", "LongerTextToFilter");
        _isNotNullFilter = FilterFactory.CreateSingleRule("is_not_null", "string", "LongerTextToFilter");
        _isEmptyFilter = FilterFactory.CreateSingleRule("is_empty", "string", "ContentTypeName");
        _isNotEmptyFilter = FilterFactory.CreateSingleRule("is_not_empty", "string", "ContentTypeName");
    }

    #region Equality Operators

    [Benchmark(Baseline = true)]
    public void Equal_Integer()
    {
        _queryable.BuildQuery(_equalIntFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void NotEqual_Integer()
    {
        _queryable.BuildQuery(_notEqualIntFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Equal_String_CaseInsensitive()
    {
        _queryable.BuildQuery(_equalStringFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Equal_String_CaseSensitive()
    {
        _queryable.BuildQuery(_equalStringFilter, _caseSensitiveOptions).ToList();
    }

    [Benchmark]
    public void NotEqual_String()
    {
        _queryable.BuildQuery(_notEqualStringFilter, _defaultOptions).ToList();
    }

    #endregion

    #region In/Not In Operators

    [Benchmark]
    public void In_5Values()
    {
        _queryable.BuildQuery(_inFilter5, _defaultOptions).ToList();
    }

    [Benchmark]
    public void In_50Values()
    {
        _queryable.BuildQuery(_inFilter50, _defaultOptions).ToList();
    }

    [Benchmark]
    public void In_500Values()
    {
        _queryable.BuildQuery(_inFilter500, _defaultOptions).ToList();
    }

    [Benchmark]
    public void NotIn_5Values()
    {
        _queryable.BuildQuery(_notInFilter, _defaultOptions).ToList();
    }

    #endregion

    #region Comparison Operators

    [Benchmark]
    public void Less()
    {
        _queryable.BuildQuery(_lessFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void LessOrEqual()
    {
        _queryable.BuildQuery(_lessOrEqualFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Greater()
    {
        _queryable.BuildQuery(_greaterFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void GreaterOrEqual()
    {
        _queryable.BuildQuery(_greaterOrEqualFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Between()
    {
        _queryable.BuildQuery(_betweenFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void NotBetween()
    {
        _queryable.BuildQuery(_notBetweenFilter, _defaultOptions).ToList();
    }

    #endregion

    #region String Operators

    [Benchmark]
    public void BeginsWith_CaseInsensitive()
    {
        _queryable.BuildQuery(_beginsWithFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void BeginsWith_CaseSensitive()
    {
        _queryable.BuildQuery(_beginsWithFilter, _caseSensitiveOptions).ToList();
    }

    [Benchmark]
    public void NotBeginsWith()
    {
        _queryable.BuildQuery(_notBeginsWithFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Contains_CaseInsensitive()
    {
        _queryable.BuildQuery(_containsFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Contains_CaseSensitive()
    {
        _queryable.BuildQuery(_containsFilter, _caseSensitiveOptions).ToList();
    }

    [Benchmark]
    public void NotContains()
    {
        _queryable.BuildQuery(_notContainsFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void EndsWith_CaseInsensitive()
    {
        _queryable.BuildQuery(_endsWithFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void EndsWith_CaseSensitive()
    {
        _queryable.BuildQuery(_endsWithFilter, _caseSensitiveOptions).ToList();
    }

    [Benchmark]
    public void NotEndsWith()
    {
        _queryable.BuildQuery(_notEndsWithFilter, _defaultOptions).ToList();
    }

    #endregion

    #region Null/Empty Operators

    [Benchmark]
    public void IsNull()
    {
        _queryable.BuildQuery(_isNullFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void IsNotNull()
    {
        _queryable.BuildQuery(_isNotNullFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void IsEmpty()
    {
        _queryable.BuildQuery(_isEmptyFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void IsNotEmpty()
    {
        _queryable.BuildQuery(_isNotEmptyFilter, _defaultOptions).ToList();
    }

    #endregion
}
