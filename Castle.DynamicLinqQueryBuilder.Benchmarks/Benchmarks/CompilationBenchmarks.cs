using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Data;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Filters;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Models;

namespace Castle.DynamicLinqQueryBuilder.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks for Expression.Compile() cost analysis.
/// Isolates compilation overhead from expression building.
/// </summary>
[MemoryDiagnoser]
[ShortRunJob]
public class CompilationBenchmarks
{
    private List<BenchmarkEntity> _data = null!;
    private BuildExpressionOptions _defaultOptions = null!;

    // Pre-built expressions for compilation benchmarks
    private Expression<Func<BenchmarkEntity, bool>> _simpleExpression = null!;
    private Expression<Func<BenchmarkEntity, bool>> _mediumExpression = null!;
    private Expression<Func<BenchmarkEntity, bool>> _complexExpression = null!;
    private Expression<Func<BenchmarkEntity, bool>> _veryComplexExpression = null!;

    // Filters for building
    private QueryBuilderFilterRule _simpleFilter = null!;
    private QueryBuilderFilterRule _mediumFilter = null!;
    private QueryBuilderFilterRule _complexFilter = null!;
    private QueryBuilderFilterRule _veryComplexFilter = null!;

    [GlobalSetup]
    public void Setup()
    {
        _data = DataGenerator.GenerateBenchmarkEntities(1000, seed: 42);
        _defaultOptions = new BuildExpressionOptions();

        // Simple filter - single rule
        _simpleFilter = FilterFactory.CreateSingleRule("equal", "integer", "ContentTypeId", "1");

        // Medium filter - 3 rules
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

        // Complex filter - nested with various operators
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
                FilterFactory.CreateSingleRule("is_not_null", "datetime", "LastModifiedIfPresent").Rules![0]
            }
        };

        // Very complex filter - deeply nested
        _veryComplexFilter = CreateVeryComplexFilter();

        // Build expressions upfront for compilation benchmarks
        _simpleExpression = _simpleFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _)!;
        _mediumExpression = _mediumFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _)!;
        _complexExpression = _complexFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _)!;
        _veryComplexExpression = _veryComplexFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _)!;
    }

    private QueryBuilderFilterRule CreateVeryComplexFilter()
    {
        return new QueryBuilderFilterRule
        {
            Condition = "and",
            Rules = new List<QueryBuilderFilterRule>
            {
                new QueryBuilderFilterRule
                {
                    Condition = "or",
                    Rules = new List<QueryBuilderFilterRule>
                    {
                        new QueryBuilderFilterRule
                        {
                            Condition = "and",
                            Rules = new List<QueryBuilderFilterRule>
                            {
                                FilterFactory.CreateSingleRule("in", "integer", "ContentTypeId", "1", "2", "3").Rules![0],
                                FilterFactory.CreateSingleRule("begins_with", "string", "ContentTypeName", "Multi").Rules![0]
                            }
                        },
                        new QueryBuilderFilterRule
                        {
                            Condition = "and",
                            Rules = new List<QueryBuilderFilterRule>
                            {
                                FilterFactory.CreateSingleRule("greater", "double", "StatValue", "50").Rules![0],
                                FilterFactory.CreateSingleRule("equal", "boolean", "IsSelected", "true").Rules![0]
                            }
                        }
                    }
                },
                new QueryBuilderFilterRule
                {
                    Condition = "or",
                    Rules = new List<QueryBuilderFilterRule>
                    {
                        FilterFactory.CreateSingleRule("is_not_null", "datetime", "LastModifiedIfPresent").Rules![0],
                        FilterFactory.CreateSingleRule("less", "datetime", "LastModified", 
                            DateTime.UtcNow.AddDays(-30).ToString("o")).Rules![0]
                    }
                },
                FilterFactory.CreateSingleRule("not_in", "integer", "ContentTypeId", "99", "98", "97").Rules![0],
                FilterFactory.CreateSingleRule("contains", "string", "LongerTextToFilter", "interesting").Rules![0]
            }
        };
    }

    #region Build-Only Benchmarks (No Compilation)

    [Benchmark(Baseline = true)]
    public void BuildOnly_Simple()
    {
        _simpleFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
    }

    [Benchmark]
    public void BuildOnly_Medium()
    {
        _mediumFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
    }

    [Benchmark]
    public void BuildOnly_Complex()
    {
        _complexFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
    }

    [Benchmark]
    public void BuildOnly_VeryComplex()
    {
        _veryComplexFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
    }

    #endregion

    #region Compile-Only Benchmarks (Pre-built Expressions)

    [Benchmark]
    public void CompileOnly_Simple()
    {
        _simpleExpression.Compile();
    }

    [Benchmark]
    public void CompileOnly_Medium()
    {
        _mediumExpression.Compile();
    }

    [Benchmark]
    public void CompileOnly_Complex()
    {
        _complexExpression.Compile();
    }

    [Benchmark]
    public void CompileOnly_VeryComplex()
    {
        _veryComplexExpression.Compile();
    }

    #endregion

    #region Build + Compile Benchmarks

    [Benchmark]
    public void BuildAndCompile_Simple()
    {
        var expr = _simpleFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
        expr!.Compile();
    }

    [Benchmark]
    public void BuildAndCompile_Medium()
    {
        var expr = _mediumFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
        expr!.Compile();
    }

    [Benchmark]
    public void BuildAndCompile_Complex()
    {
        var expr = _complexFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
        expr!.Compile();
    }

    [Benchmark]
    public void BuildAndCompile_VeryComplex()
    {
        var expr = _veryComplexFilter.BuildExpressionLambda<BenchmarkEntity>(_defaultOptions, out _);
        expr!.Compile();
    }

    #endregion

    #region Compile with preferInterpretation Flag

    [Benchmark]
    public void CompileInterpreted_Simple()
    {
        _simpleExpression.Compile(preferInterpretation: true);
    }

    [Benchmark]
    public void CompileInterpreted_Complex()
    {
        _complexExpression.Compile(preferInterpretation: true);
    }

    [Benchmark]
    public void CompileJIT_Simple()
    {
        _simpleExpression.Compile(preferInterpretation: false);
    }

    [Benchmark]
    public void CompileJIT_Complex()
    {
        _complexExpression.Compile(preferInterpretation: false);
    }

    #endregion

    #region Execution Comparison: Interpreted vs JIT

    [Benchmark]
    public void Execute_Interpreted_Simple()
    {
        var compiled = _simpleExpression.Compile(preferInterpretation: true);
        _data.Where(compiled).ToList();
    }

    [Benchmark]
    public void Execute_JIT_Simple()
    {
        var compiled = _simpleExpression.Compile(preferInterpretation: false);
        _data.Where(compiled).ToList();
    }

    [Benchmark]
    public void Execute_Interpreted_Complex()
    {
        var compiled = _complexExpression.Compile(preferInterpretation: true);
        _data.Where(compiled).ToList();
    }

    [Benchmark]
    public void Execute_JIT_Complex()
    {
        var compiled = _complexExpression.Compile(preferInterpretation: false);
        _data.Where(compiled).ToList();
    }

    #endregion

    #region Build Predicate API (Uses Compile internally)

    [Benchmark]
    public void BuildPredicate_Simple()
    {
        _simpleFilter.BuildPredicate<BenchmarkEntity>(_defaultOptions);
    }

    [Benchmark]
    public void BuildPredicate_Medium()
    {
        _mediumFilter.BuildPredicate<BenchmarkEntity>(_defaultOptions);
    }

    [Benchmark]
    public void BuildPredicate_Complex()
    {
        _complexFilter.BuildPredicate<BenchmarkEntity>(_defaultOptions);
    }

    [Benchmark]
    public void BuildPredicate_VeryComplex()
    {
        _veryComplexFilter.BuildPredicate<BenchmarkEntity>(_defaultOptions);
    }

    #endregion
}
