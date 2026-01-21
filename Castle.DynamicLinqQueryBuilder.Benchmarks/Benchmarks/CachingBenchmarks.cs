using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Data;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Filters;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Models;

namespace Castle.DynamicLinqQueryBuilder.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks for analyzing caching ROI.
/// Measures repeated identical filter builds to establish baseline for caching.
/// </summary>
[MemoryDiagnoser]
[ShortRunJob]
public class CachingBenchmarks
{
    private List<BenchmarkEntity> _data = null!;
    private IQueryable<BenchmarkEntity> _queryable = null!;
    private BuildExpressionOptions _defaultOptions = null!;

    // Filters for repeated builds
    private QueryBuilderFilterRule _simpleFilter = null!;
    private QueryBuilderFilterRule _mediumFilter = null!;
    private QueryBuilderFilterRule _complexFilter = null!;

    // Cached expressions (simulating what caching would provide)
    private Expression<Func<BenchmarkEntity, bool>> _cachedSimpleExpression = null!;
    private Expression<Func<BenchmarkEntity, bool>> _cachedMediumExpression = null!;
    private Expression<Func<BenchmarkEntity, bool>> _cachedComplexExpression = null!;

    // Cached compiled predicates
    private Func<BenchmarkEntity, bool> _cachedSimplePredicate = null!;
    private Func<BenchmarkEntity, bool> _cachedMediumPredicate = null!;
    private Func<BenchmarkEntity, bool> _cachedComplexPredicate = null!;

    [GlobalSetup]
    public void Setup()
    {
        _data = DataGenerator.GenerateBenchmarkEntities(1000, seed: 42);
        _queryable = _data.AsQueryable();
        _defaultOptions = new BuildExpressionOptions();

        // Simple filter
        _simpleFilter = FilterFactory.CreateSingleRule("equal", "integer", "ContentTypeId", "1");

        // Medium complexity filter
        _mediumFilter = new QueryBuilderFilterRule
        {
            Condition = "and",
            Rules = new List<QueryBuilderFilterRule>
            {
                FilterFactory.CreateSingleRule("greater", "integer", "ContentTypeId", "10").Rules![0],
                FilterFactory.CreateSingleRule("contains", "string", "ContentTypeName", "Choice").Rules![0],
                FilterFactory.CreateSingleRule("equal", "boolean", "IsSelected", "true").Rules![0]
            }
        };

        // Complex filter
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
                        FilterFactory.CreateSingleRule("in", "integer", "ContentTypeId", "1", "2", "3", "4", "5").Rules![0],
                        FilterFactory.CreateSingleRule("contains", "string", "ContentTypeName", "Select").Rules![0]
                    }
                },
                FilterFactory.CreateSingleRule("between", "double", "StatValue", "10", "90").Rules![0],
                FilterFactory.CreateSingleRule("is_not_null", "datetime", "LastModifiedIfPresent").Rules![0],
                new QueryBuilderFilterRule
                {
                    Condition = "or",
                    Rules = new List<QueryBuilderFilterRule>
                    {
                        FilterFactory.CreateSingleRule("equal", "boolean", "IsSelected", "true").Rules![0],
                        FilterFactory.CreateSingleRule("greater", "datetime", "LastModified", 
                            DateTime.UtcNow.AddDays(-7).ToString("o")).Rules![0]
                    }
                }
            }
        };

        // Pre-cache expressions
        _cachedSimpleExpression = _simpleFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _)!;
        _cachedMediumExpression = _mediumFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _)!;
        _cachedComplexExpression = _complexFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _)!;

        // Pre-cache compiled predicates
        _cachedSimplePredicate = _cachedSimpleExpression.Compile();
        _cachedMediumPredicate = _cachedMediumExpression.Compile();
        _cachedComplexPredicate = _cachedComplexExpression.Compile();
    }

    #region Single Build vs Cached (Simple Filter)

    [Benchmark(Baseline = true)]
    public void Build_Simple_Once()
    {
        _simpleFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
    }

    [Benchmark]
    public void UseCached_Simple_Once()
    {
        // Simulates cache hit - just return cached expression (baseline for cache benefit)
        _ = _cachedSimpleExpression;
    }

    #endregion

    #region Repeated Builds - Simple Filter

    [Benchmark]
    public void Build_Simple_10x()
    {
        for (int i = 0; i < 10; i++)
        {
            _simpleFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
        }
    }

    [Benchmark]
    public void Build_Simple_100x()
    {
        for (int i = 0; i < 100; i++)
        {
            _simpleFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
        }
    }

    [Benchmark]
    public void Build_Simple_1000x()
    {
        for (int i = 0; i < 1000; i++)
        {
            _simpleFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
        }
    }

    #endregion

    #region Repeated Builds - Medium Filter

    [Benchmark]
    public void Build_Medium_Once()
    {
        _mediumFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
    }

    [Benchmark]
    public void Build_Medium_10x()
    {
        for (int i = 0; i < 10; i++)
        {
            _mediumFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
        }
    }

    [Benchmark]
    public void Build_Medium_100x()
    {
        for (int i = 0; i < 100; i++)
        {
            _mediumFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
        }
    }

    #endregion

    #region Repeated Builds - Complex Filter

    [Benchmark]
    public void Build_Complex_Once()
    {
        _complexFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
    }

    [Benchmark]
    public void Build_Complex_10x()
    {
        for (int i = 0; i < 10; i++)
        {
            _complexFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
        }
    }

    [Benchmark]
    public void Build_Complex_100x()
    {
        for (int i = 0; i < 100; i++)
        {
            _complexFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
        }
    }

    #endregion

    #region Build + Execute vs Cached Execute

    [Benchmark]
    public void BuildAndExecute_Simple()
    {
        _queryable.BuildQuery(_simpleFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void CachedExecute_Simple()
    {
        _data.Where(_cachedSimplePredicate).ToList();
    }

    [Benchmark]
    public void BuildAndExecute_Complex()
    {
        _queryable.BuildQuery(_complexFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public void CachedExecute_Complex()
    {
        _data.Where(_cachedComplexPredicate).ToList();
    }

    #endregion

    #region Repeated Build + Execute vs Cached Execute

    [Benchmark]
    public void BuildAndExecute_Simple_10x()
    {
        for (int i = 0; i < 10; i++)
        {
            _queryable.BuildQuery(_simpleFilter, _defaultOptions).ToList();
        }
    }

    [Benchmark]
    public void CachedExecute_Simple_10x()
    {
        for (int i = 0; i < 10; i++)
        {
            _data.Where(_cachedSimplePredicate).ToList();
        }
    }

    [Benchmark]
    public void BuildAndExecute_Complex_10x()
    {
        for (int i = 0; i < 10; i++)
        {
            _queryable.BuildQuery(_complexFilter, _defaultOptions).ToList();
        }
    }

    [Benchmark]
    public void CachedExecute_Complex_10x()
    {
        for (int i = 0; i < 10; i++)
        {
            _data.Where(_cachedComplexPredicate).ToList();
        }
    }

    #endregion

    #region Build + Compile vs Cached Compile

    [Benchmark]
    public void BuildAndCompile_Simple()
    {
        var expr = _simpleFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
        expr!.Compile();
    }

    [Benchmark]
    public void BuildAndCompile_Complex()
    {
        var expr = _complexFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
        expr!.Compile();
    }

    [Benchmark]
    public void BuildAndCompile_Simple_10x()
    {
        for (int i = 0; i < 10; i++)
        {
            var expr = _simpleFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
            expr!.Compile();
        }
    }

    [Benchmark]
    public void BuildAndCompile_Complex_10x()
    {
        for (int i = 0; i < 10; i++)
        {
            var expr = _complexFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
            expr!.Compile();
        }
    }

    #endregion
}
