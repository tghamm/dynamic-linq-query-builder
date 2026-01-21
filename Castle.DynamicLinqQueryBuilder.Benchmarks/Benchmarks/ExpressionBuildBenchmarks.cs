using BenchmarkDotNet.Attributes;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Data;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Filters;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Models;

namespace Castle.DynamicLinqQueryBuilder.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks for core expression building operations.
/// Measures the cost of BuildExpressionLambda in isolation.
/// </summary>
[MemoryDiagnoser]
[ShortRunJob]
public class ExpressionBuildBenchmarks
{
    private List<BenchmarkEntity> _data = null!;
    private IQueryable<BenchmarkEntity> _queryable = null!;
    
    private QueryBuilderFilterRule _simpleFilter = null!;
    private QueryBuilderFilterRule _nestedPropertyFilter = null!;
    private QueryBuilderFilterRule _collectionPropertyFilter = null!;
    private QueryBuilderFilterRule _dictionaryFilter = null!;
    private BuildExpressionOptions _defaultOptions = null!;

    [GlobalSetup]
    public void Setup()
    {
        _data = DataGenerator.GenerateBenchmarkEntities(1000, seed: 42);
        _queryable = _data.AsQueryable();
        _defaultOptions = new BuildExpressionOptions();

        // Simple single rule - direct property access
        _simpleFilter = FilterFactory.CreateSingleRule("equal", "integer", "ContentTypeId", "1");

        // Nested property path (ChildClasses.ClassName)
        _nestedPropertyFilter = FilterFactory.CreateNestedPropertyFilter(
            "ChildClasses.ClassName", 
            "contains", 
            "string", 
            "Child");

        // Collection traversal filter
        _collectionPropertyFilter = FilterFactory.CreateSingleRule(
            "in", 
            "integer", 
            "IntList", 
            "1", "2", "3");

        // Dictionary access filter
        _dictionaryFilter = FilterFactory.CreateNestedPropertyFilter(
            "Dictionary.first_name",
            "equal",
            "string",
            "Emma");
    }

    [Benchmark(Baseline = true)]
    public void BuildExpression_SimpleProperty()
    {
        _simpleFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
    }

    [Benchmark]
    public void BuildExpression_NestedProperty()
    {
        _nestedPropertyFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
    }

    [Benchmark]
    public void BuildExpression_CollectionProperty()
    {
        _collectionPropertyFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
    }

    [Benchmark]
    public void BuildExpression_DictionaryAccess()
    {
        _dictionaryFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
    }

    [Benchmark]
    public void BuildQuery_SimpleProperty()
    {
        _queryable.BuildQuery(_simpleFilter, _defaultOptions);
    }

    [Benchmark]
    public void BuildQuery_NestedProperty()
    {
        _queryable.BuildQuery(_nestedPropertyFilter, _defaultOptions);
    }

    [Benchmark]
    public void BuildAndExecute_SimpleProperty()
    {
        _queryable.BuildQuery(_simpleFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void BuildAndExecute_NestedProperty()
    {
        _queryable.BuildQuery(_nestedPropertyFilter, _defaultOptions).ToList();
    }
}
