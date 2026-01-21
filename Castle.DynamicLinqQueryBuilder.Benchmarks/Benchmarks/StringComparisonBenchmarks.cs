using BenchmarkDotNet.Attributes;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Data;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Filters;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Models;

namespace Castle.DynamicLinqQueryBuilder.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks comparing ToLower() (default/ORM-compatible) vs OrdinalIgnoreCase (optimized) 
/// string comparison modes. Tests the effect of UseOrdinalStringComparison option.
/// </summary>
[MemoryDiagnoser]
[ShortRunJob]
public class StringComparisonBenchmarks
{
    private List<BenchmarkEntity> _data = null!;
    private IQueryable<BenchmarkEntity> _queryable = null!;
    
    // Options for comparison
    private BuildExpressionOptions _defaultOptions = null!;           // ToLower() approach - ORM compatible
    private BuildExpressionOptions _ordinalOptions = null!;           // OrdinalIgnoreCase - optimized
    private BuildExpressionOptions _caseSensitiveOptions = null!;     // Case sensitive (no transformation)

    // Filters
    private QueryBuilderFilterRule _equalStringFilter = null!;
    private QueryBuilderFilterRule _containsFilter = null!;
    private QueryBuilderFilterRule _beginsWithFilter = null!;
    private QueryBuilderFilterRule _endsWithFilter = null!;
    private QueryBuilderFilterRule _inStringFilter5 = null!;
    private QueryBuilderFilterRule _inStringFilter20 = null!;
    private QueryBuilderFilterRule _complexStringFilter = null!;

    [GlobalSetup]
    public void Setup()
    {
        _data = DataGenerator.GenerateBenchmarkEntities(1000, seed: 42);
        _queryable = _data.AsQueryable();
        
        // Default: case-insensitive using ToLower() - ORM compatible
        _defaultOptions = new BuildExpressionOptions 
        { 
            StringCaseSensitiveComparison = false,
            UseOrdinalStringComparison = false  // Use ToLower()
        };
        
        // Optimized: case-insensitive using StringComparison.OrdinalIgnoreCase
        _ordinalOptions = new BuildExpressionOptions 
        { 
            StringCaseSensitiveComparison = false,
            UseOrdinalStringComparison = true   // Use OrdinalIgnoreCase
        };
        
        // Case sensitive: no transformation needed
        _caseSensitiveOptions = new BuildExpressionOptions 
        { 
            StringCaseSensitiveComparison = true 
        };

        // Build filters
        _equalStringFilter = FilterFactory.CreateSingleRule("equal", "string", "ContentTypeName", "Multiple-Choice");
        _containsFilter = FilterFactory.CreateSingleRule("contains", "string", "ContentTypeName", "Choice");
        _beginsWithFilter = FilterFactory.CreateSingleRule("begins_with", "string", "ContentTypeName", "Multi");
        _endsWithFilter = FilterFactory.CreateSingleRule("ends_with", "string", "ContentTypeName", "Choice");
        
        // In filter with string values
        _inStringFilter5 = CreateInStringFilter(5);
        _inStringFilter20 = CreateInStringFilter(20);
        
        // Complex filter combining multiple string operations
        _complexStringFilter = CreateComplexStringFilter();
    }

    private static QueryBuilderFilterRule CreateInStringFilter(int count)
    {
        var values = new List<string>();
        for (int i = 0; i < count; i++)
        {
            values.Add($"Value{i}");
        }
        // Add one that actually matches
        values[0] = "Multiple-Choice";
        
        return new QueryBuilderFilterRule
        {
            Condition = "and",
            Rules = new List<QueryBuilderFilterRule>
            {
                new QueryBuilderFilterRule
                {
                    Field = "ContentTypeName",
                    Operator = "in",
                    Type = "string",
                    Value = values.ToArray()
                }
            }
        };
    }

    private static QueryBuilderFilterRule CreateComplexStringFilter()
    {
        return new QueryBuilderFilterRule
        {
            Condition = "or",
            Rules = new List<QueryBuilderFilterRule>
            {
                new QueryBuilderFilterRule
                {
                    Condition = "and",
                    Rules = new List<QueryBuilderFilterRule>
                    {
                        new QueryBuilderFilterRule
                        {
                            Field = "ContentTypeName",
                            Operator = "begins_with",
                            Type = "string",
                            Value = new[] { "Multi" }
                        },
                        new QueryBuilderFilterRule
                        {
                            Field = "ContentTypeName",
                            Operator = "ends_with",
                            Type = "string",
                            Value = new[] { "Choice" }
                        }
                    }
                },
                new QueryBuilderFilterRule
                {
                    Condition = "and",
                    Rules = new List<QueryBuilderFilterRule>
                    {
                        new QueryBuilderFilterRule
                        {
                            Field = "LongerTextToFilter",
                            Operator = "contains",
                            Type = "string",
                            Value = new[] { "test" }
                        },
                        new QueryBuilderFilterRule
                        {
                            Field = "LongerTextToFilter",
                            Operator = "is_not_null",
                            Type = "string"
                        }
                    }
                }
            }
        };
    }

    #region Equal String Comparisons

    [Benchmark(Baseline = true)]
    public List<BenchmarkEntity> Equal_ToLower()
    {
        return _queryable.BuildQuery(_equalStringFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public List<BenchmarkEntity> Equal_OrdinalIgnoreCase()
    {
        return _queryable.BuildQuery(_equalStringFilter, _ordinalOptions).ToList();
    }

    [Benchmark]
    public List<BenchmarkEntity> Equal_CaseSensitive()
    {
        return _queryable.BuildQuery(_equalStringFilter, _caseSensitiveOptions).ToList();
    }

    #endregion

    #region Contains Comparisons

    [Benchmark]
    public List<BenchmarkEntity> Contains_ToLower()
    {
        return _queryable.BuildQuery(_containsFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public List<BenchmarkEntity> Contains_OrdinalIgnoreCase()
    {
        return _queryable.BuildQuery(_containsFilter, _ordinalOptions).ToList();
    }

    [Benchmark]
    public List<BenchmarkEntity> Contains_CaseSensitive()
    {
        return _queryable.BuildQuery(_containsFilter, _caseSensitiveOptions).ToList();
    }

    #endregion

    #region BeginsWith Comparisons

    [Benchmark]
    public List<BenchmarkEntity> BeginsWith_ToLower()
    {
        return _queryable.BuildQuery(_beginsWithFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public List<BenchmarkEntity> BeginsWith_OrdinalIgnoreCase()
    {
        return _queryable.BuildQuery(_beginsWithFilter, _ordinalOptions).ToList();
    }

    [Benchmark]
    public List<BenchmarkEntity> BeginsWith_CaseSensitive()
    {
        return _queryable.BuildQuery(_beginsWithFilter, _caseSensitiveOptions).ToList();
    }

    #endregion

    #region EndsWith Comparisons

    [Benchmark]
    public List<BenchmarkEntity> EndsWith_ToLower()
    {
        return _queryable.BuildQuery(_endsWithFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public List<BenchmarkEntity> EndsWith_OrdinalIgnoreCase()
    {
        return _queryable.BuildQuery(_endsWithFilter, _ordinalOptions).ToList();
    }

    [Benchmark]
    public List<BenchmarkEntity> EndsWith_CaseSensitive()
    {
        return _queryable.BuildQuery(_endsWithFilter, _caseSensitiveOptions).ToList();
    }

    #endregion

    #region In Operator String Comparisons

    [Benchmark]
    public List<BenchmarkEntity> In5Strings_ToLower()
    {
        return _queryable.BuildQuery(_inStringFilter5, _defaultOptions).ToList();
    }

    [Benchmark]
    public List<BenchmarkEntity> In5Strings_OrdinalIgnoreCase()
    {
        return _queryable.BuildQuery(_inStringFilter5, _ordinalOptions).ToList();
    }

    [Benchmark]
    public List<BenchmarkEntity> In20Strings_ToLower()
    {
        return _queryable.BuildQuery(_inStringFilter20, _defaultOptions).ToList();
    }

    [Benchmark]
    public List<BenchmarkEntity> In20Strings_OrdinalIgnoreCase()
    {
        return _queryable.BuildQuery(_inStringFilter20, _ordinalOptions).ToList();
    }

    #endregion

    #region Complex String Filter Comparisons

    [Benchmark]
    public List<BenchmarkEntity> Complex_ToLower()
    {
        return _queryable.BuildQuery(_complexStringFilter, _defaultOptions).ToList();
    }

    [Benchmark]
    public List<BenchmarkEntity> Complex_OrdinalIgnoreCase()
    {
        return _queryable.BuildQuery(_complexStringFilter, _ordinalOptions).ToList();
    }

    [Benchmark]
    public List<BenchmarkEntity> Complex_CaseSensitive()
    {
        return _queryable.BuildQuery(_complexStringFilter, _caseSensitiveOptions).ToList();
    }

    #endregion
}
