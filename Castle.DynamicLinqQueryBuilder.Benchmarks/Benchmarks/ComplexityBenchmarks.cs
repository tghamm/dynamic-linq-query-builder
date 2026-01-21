using BenchmarkDotNet.Attributes;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Data;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Filters;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Models;

namespace Castle.DynamicLinqQueryBuilder.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks for filter complexity impact on build time.
/// Tests flat rules, nested rules, and mixed AND/OR conditions.
/// </summary>
[MemoryDiagnoser]
[ShortRunJob]
public class ComplexityBenchmarks
{
    private List<BenchmarkEntity> _data = null!;
    private IQueryable<BenchmarkEntity> _queryable = null!;
    private BuildExpressionOptions _defaultOptions = null!;

    // Flat AND filters
    private QueryBuilderFilterRule _flat1Rule = null!;
    private QueryBuilderFilterRule _flat5Rules = null!;
    private QueryBuilderFilterRule _flat10Rules = null!;
    private QueryBuilderFilterRule _flat20Rules = null!;

    // Flat OR filters
    private QueryBuilderFilterRule _flatOr5Rules = null!;
    private QueryBuilderFilterRule _flatOr10Rules = null!;
    private QueryBuilderFilterRule _flatOr20Rules = null!;

    // Nested depth filters
    private QueryBuilderFilterRule _nested2Levels = null!;
    private QueryBuilderFilterRule _nested3Levels = null!;
    private QueryBuilderFilterRule _nested5Levels = null!;
    private QueryBuilderFilterRule _nested10Levels = null!;

    // Mixed AND/OR filters
    private QueryBuilderFilterRule _mixed2x2 = null!;
    private QueryBuilderFilterRule _mixed3x3 = null!;
    private QueryBuilderFilterRule _mixed5x5 = null!;

    // Complex real-world-like filter
    private QueryBuilderFilterRule _complexRealWorld = null!;

    [GlobalSetup]
    public void Setup()
    {
        _data = DataGenerator.GenerateBenchmarkEntities(1000, seed: 42);
        _queryable = _data.AsQueryable();
        _defaultOptions = new BuildExpressionOptions();

        // Flat AND filters (all conditions must match)
        _flat1Rule = FilterFactory.CreateFlatFilter(1);
        _flat5Rules = FilterFactory.CreateFlatFilter(5);
        _flat10Rules = FilterFactory.CreateFlatFilter(10);
        _flat20Rules = FilterFactory.CreateFlatFilter(20);

        // Flat OR filters (any condition can match)
        _flatOr5Rules = FilterFactory.CreateFlatOrFilter(5);
        _flatOr10Rules = FilterFactory.CreateFlatOrFilter(10);
        _flatOr20Rules = FilterFactory.CreateFlatOrFilter(20);

        // Nested depth filters
        _nested2Levels = FilterFactory.CreateNestedFilter(2);
        _nested3Levels = FilterFactory.CreateNestedFilter(3);
        _nested5Levels = FilterFactory.CreateNestedFilter(5);
        _nested10Levels = FilterFactory.CreateNestedFilter(10);

        // Mixed AND/OR filters (depth x breadth)
        _mixed2x2 = FilterFactory.CreateMixedFilter(2, 2);
        _mixed3x3 = FilterFactory.CreateMixedFilter(3, 3);
        _mixed5x5 = FilterFactory.CreateMixedFilter(5, 5);

        // Complex real-world-like filter simulating a typical search scenario
        _complexRealWorld = CreateComplexRealWorldFilter();
    }

    private QueryBuilderFilterRule CreateComplexRealWorldFilter()
    {
        // Simulates: Find records where
        // - ContentTypeId in [1,2,3] OR ContentTypeName contains "Choice"
        // - AND StatValue > 10
        // - AND (IsSelected = true OR LastModified > some date)
        // - AND ContentTypeName is not null

        return new QueryBuilderFilterRule
        {
            Condition = "and",
            Rules = new List<QueryBuilderFilterRule>
            {
                // First OR group
                new QueryBuilderFilterRule
                {
                    Condition = "or",
                    Rules = new List<QueryBuilderFilterRule>
                    {
                        new QueryBuilderFilterRule
                        {
                            Field = "ContentTypeId",
                            Operator = "in",
                            Type = "integer",
                            Value = new[] { "1", "2", "3" }
                        },
                        new QueryBuilderFilterRule
                        {
                            Field = "ContentTypeName",
                            Operator = "contains",
                            Type = "string",
                            Value = new[] { "Choice" }
                        }
                    }
                },
                // Simple condition
                new QueryBuilderFilterRule
                {
                    Field = "StatValue",
                    Operator = "greater",
                    Type = "double",
                    Value = new[] { "10" }
                },
                // Second OR group
                new QueryBuilderFilterRule
                {
                    Condition = "or",
                    Rules = new List<QueryBuilderFilterRule>
                    {
                        new QueryBuilderFilterRule
                        {
                            Field = "IsSelected",
                            Operator = "equal",
                            Type = "boolean",
                            Value = new[] { "true" }
                        },
                        new QueryBuilderFilterRule
                        {
                            Field = "LastModified",
                            Operator = "greater",
                            Type = "datetime",
                            Value = new[] { DateTime.UtcNow.AddDays(-30).ToString("o") }
                        }
                    }
                },
                // Null check
                new QueryBuilderFilterRule
                {
                    Field = "ContentTypeName",
                    Operator = "is_not_null",
                    Type = "string",
                    Value = Array.Empty<string>()
                }
            }
        };
    }

    #region Flat AND Filter Benchmarks

    [Benchmark(Baseline = true)]
    public void Flat_1Rule()
    {
        _queryable.BuildQuery(_flat1Rule, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Flat_5Rules()
    {
        _queryable.BuildQuery(_flat5Rules, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Flat_10Rules()
    {
        _queryable.BuildQuery(_flat10Rules, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Flat_20Rules()
    {
        _queryable.BuildQuery(_flat20Rules, _defaultOptions).ToList();
    }

    #endregion

    #region Flat OR Filter Benchmarks

    [Benchmark]
    public void FlatOr_5Rules()
    {
        _queryable.BuildQuery(_flatOr5Rules, _defaultOptions).ToList();
    }

    [Benchmark]
    public void FlatOr_10Rules()
    {
        _queryable.BuildQuery(_flatOr10Rules, _defaultOptions).ToList();
    }

    [Benchmark]
    public void FlatOr_20Rules()
    {
        _queryable.BuildQuery(_flatOr20Rules, _defaultOptions).ToList();
    }

    #endregion

    #region Nested Depth Benchmarks

    [Benchmark]
    public void Nested_2Levels()
    {
        _queryable.BuildQuery(_nested2Levels, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Nested_3Levels()
    {
        _queryable.BuildQuery(_nested3Levels, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Nested_5Levels()
    {
        _queryable.BuildQuery(_nested5Levels, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Nested_10Levels()
    {
        _queryable.BuildQuery(_nested10Levels, _defaultOptions).ToList();
    }

    #endregion

    #region Mixed AND/OR Benchmarks

    [Benchmark]
    public void Mixed_2x2()
    {
        _queryable.BuildQuery(_mixed2x2, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Mixed_3x3()
    {
        _queryable.BuildQuery(_mixed3x3, _defaultOptions).ToList();
    }

    [Benchmark]
    public void Mixed_5x5()
    {
        _queryable.BuildQuery(_mixed5x5, _defaultOptions).ToList();
    }

    #endregion

    #region Real World Complexity

    [Benchmark]
    public void ComplexRealWorld()
    {
        _queryable.BuildQuery(_complexRealWorld, _defaultOptions).ToList();
    }

    #endregion

    #region Build-Only Benchmarks (no execution)

    [Benchmark]
    public void BuildOnly_Flat_1Rule()
    {
        _flat1Rule.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
    }

    [Benchmark]
    public void BuildOnly_Flat_20Rules()
    {
        _flat20Rules.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
    }

    [Benchmark]
    public void BuildOnly_Nested_10Levels()
    {
        _nested10Levels.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
    }

    [Benchmark]
    public void BuildOnly_ComplexRealWorld()
    {
        _complexRealWorld.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
    }

    #endregion
}
