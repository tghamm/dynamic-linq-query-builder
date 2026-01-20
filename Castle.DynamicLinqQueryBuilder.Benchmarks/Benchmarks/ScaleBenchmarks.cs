using BenchmarkDotNet.Attributes;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Data;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Filters;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Models;

namespace Castle.DynamicLinqQueryBuilder.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks for dataset size impact on query performance.
/// Measures both BuildQuery().ToList() and pre-compiled BuildPredicate().
/// </summary>
[MemoryDiagnoser]
[ShortRunJob]
public class ScaleBenchmarks
{
    private BuildExpressionOptions _defaultOptions = null!;
    private QueryBuilderFilterRule _simpleFilter = null!;
    private QueryBuilderFilterRule _complexFilter = null!;
    
    // Different sized datasets
    private List<BenchmarkEntity> _data100 = null!;
    private List<BenchmarkEntity> _data1000 = null!;
    private List<BenchmarkEntity> _data10000 = null!;
    private List<BenchmarkEntity> _data100000 = null!;

    // Pre-compiled predicates
    private Func<BenchmarkEntity, bool> _simplePredicateCompiled = null!;
    private Func<BenchmarkEntity, bool> _complexPredicateCompiled = null!;

    [GlobalSetup]
    public void Setup()
    {
        _defaultOptions = new BuildExpressionOptions();

        // Generate datasets of varying sizes
        _data100 = DataGenerator.GenerateBenchmarkEntities(100, seed: 42);
        _data1000 = DataGenerator.GenerateBenchmarkEntities(1000, seed: 42);
        _data10000 = DataGenerator.GenerateBenchmarkEntities(10000, seed: 42);
        _data100000 = DataGenerator.GenerateBenchmarkEntities(100000, seed: 42);

        // Simple filter - single equality check
        _simpleFilter = FilterFactory.CreateSingleRule("equal", "integer", "ContentTypeId", "1");

        // Complex filter - multiple conditions with different operators
        _complexFilter = new QueryBuilderFilterRule
        {
            Condition = "and",
            Rules = new List<QueryBuilderFilterRule>
            {
                new QueryBuilderFilterRule
                {
                    Condition = "or",
                    Rules = new List<QueryBuilderFilterRule>
                    {
                        FilterFactory.CreateSingleRule("in", "integer", "ContentTypeId", "1", "2", "3").Rules![0],
                        FilterFactory.CreateSingleRule("contains", "string", "ContentTypeName", "Choice").Rules![0]
                    }
                },
                FilterFactory.CreateSingleRule("greater", "double", "StatValue", "10").Rules![0],
                FilterFactory.CreateSingleRule("is_not_null", "string", "ContentTypeName").Rules![0]
            }
        };

        // Pre-compile predicates for comparison benchmarks
        _simplePredicateCompiled = _simpleFilter.BuildPredicate<BenchmarkEntity>(_defaultOptions);
        _complexPredicateCompiled = _complexFilter.BuildPredicate<BenchmarkEntity>(_defaultOptions);
    }

    #region BuildQuery - Simple Filter by Dataset Size

    [Benchmark(Baseline = true)]
    public void BuildQuery_Simple_100()
    {
        _data100.AsQueryable().BuildQuery(_simpleFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void BuildQuery_Simple_1000()
    {
        _data1000.AsQueryable().BuildQuery(_simpleFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void BuildQuery_Simple_10000()
    {
        _data10000.AsQueryable().BuildQuery(_simpleFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void BuildQuery_Simple_100000()
    {
        _data100000.AsQueryable().BuildQuery(_simpleFilter, _defaultOptions).ToList();
    }

    #endregion

    #region BuildQuery - Complex Filter by Dataset Size

    [Benchmark]
    public void BuildQuery_Complex_100()
    {
        _data100.AsQueryable().BuildQuery(_complexFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void BuildQuery_Complex_1000()
    {
        _data1000.AsQueryable().BuildQuery(_complexFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void BuildQuery_Complex_10000()
    {
        _data10000.AsQueryable().BuildQuery(_complexFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void BuildQuery_Complex_100000()
    {
        _data100000.AsQueryable().BuildQuery(_complexFilter, _defaultOptions).ToList();
    }

    #endregion

    #region Pre-compiled Predicate - Simple Filter by Dataset Size

    [Benchmark]
    public void PreCompiled_Simple_100()
    {
        _data100.Where(_simplePredicateCompiled).ToList();
    }

    [Benchmark]
    public void PreCompiled_Simple_1000()
    {
        _data1000.Where(_simplePredicateCompiled).ToList();
    }

    [Benchmark]
    public void PreCompiled_Simple_10000()
    {
        _data10000.Where(_simplePredicateCompiled).ToList();
    }

    [Benchmark]
    public void PreCompiled_Simple_100000()
    {
        _data100000.Where(_simplePredicateCompiled).ToList();
    }

    #endregion

    #region Pre-compiled Predicate - Complex Filter by Dataset Size

    [Benchmark]
    public void PreCompiled_Complex_100()
    {
        _data100.Where(_complexPredicateCompiled).ToList();
    }

    [Benchmark]
    public void PreCompiled_Complex_1000()
    {
        _data1000.Where(_complexPredicateCompiled).ToList();
    }

    [Benchmark]
    public void PreCompiled_Complex_10000()
    {
        _data10000.Where(_complexPredicateCompiled).ToList();
    }

    [Benchmark]
    public void PreCompiled_Complex_100000()
    {
        _data100000.Where(_complexPredicateCompiled).ToList();
    }

    #endregion

    #region Build-Only Benchmarks (to isolate expression building cost)

    [Benchmark]
    public void BuildOnly_Simple_NoExecution()
    {
        _simpleFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
    }

    [Benchmark]
    public void BuildOnly_Complex_NoExecution()
    {
        _complexFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
    }

    [Benchmark]
    public void BuildAndCompile_Simple()
    {
        _simpleFilter.BuildPredicate<BenchmarkEntity>(_defaultOptions);
    }

    [Benchmark]
    public void BuildAndCompile_Complex()
    {
        _complexFilter.BuildPredicate<BenchmarkEntity>(_defaultOptions);
    }

    #endregion
}
