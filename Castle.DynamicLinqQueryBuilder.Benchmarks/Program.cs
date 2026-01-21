using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Running;
using Castle.DynamicLinqQueryBuilder.Benchmarks.Benchmarks;

namespace Castle.DynamicLinqQueryBuilder.Benchmarks;

/// <summary>
/// BenchmarkDotNet runner for Castle.DynamicLinqQueryBuilder performance analysis.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        // Run all benchmarks or filter by command line args
        if (args.Length == 0)
        {
            ShowMenu();
            return;
        }

        var config = ManualConfig.Create(DefaultConfig.Instance)
            .AddExporter(JsonExporter.Full)
            .AddExporter(MarkdownExporter.GitHub)
            .AddExporter(HtmlExporter.Default);

        // Allow running specific benchmark class or all
        var arg = args[0].ToLowerInvariant();
        
        switch (arg)
        {
            case "all":
                RunAllBenchmarks(config);
                break;
            case "expression":
                BenchmarkRunner.Run<ExpressionBuildBenchmarks>(config);
                break;
            case "operator":
                BenchmarkRunner.Run<OperatorBenchmarks>(config);
                break;
            case "type":
                BenchmarkRunner.Run<TypeBenchmarks>(config);
                break;
            case "complexity":
                BenchmarkRunner.Run<ComplexityBenchmarks>(config);
                break;
            case "scale":
                BenchmarkRunner.Run<ScaleBenchmarks>(config);
                break;
            case "caching":
                BenchmarkRunner.Run<CachingBenchmarks>(config);
                break;
            case "compilation":
                BenchmarkRunner.Run<CompilationBenchmarks>(config);
                break;
            case "quick":
                // Quick test - just run expression build benchmarks
                BenchmarkRunner.Run<ExpressionBuildBenchmarks>(config);
                break;
            default:
                // Use BenchmarkSwitcher for more flexibility
                BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
                break;
        }
    }

    private static void ShowMenu()
    {
        Console.WriteLine("Castle.DynamicLinqQueryBuilder Benchmark Suite");
        Console.WriteLine("=".PadRight(50, '='));
        Console.WriteLine();
        Console.WriteLine("Usage: dotnet run -c Release -- <benchmark>");
        Console.WriteLine();
        Console.WriteLine("Available benchmarks:");
        Console.WriteLine("  all         - Run all benchmarks (takes a long time)");
        Console.WriteLine("  expression  - ExpressionBuildBenchmarks (core build path)");
        Console.WriteLine("  operator    - OperatorBenchmarks (all operators)");
        Console.WriteLine("  type        - TypeBenchmarks (type conversion)");
        Console.WriteLine("  complexity  - ComplexityBenchmarks (filter nesting)");
        Console.WriteLine("  scale       - ScaleBenchmarks (dataset sizes)");
        Console.WriteLine("  caching     - CachingBenchmarks (repeated builds)");
        Console.WriteLine("  compilation - CompilationBenchmarks (Expression.Compile)");
        Console.WriteLine("  quick       - Quick test run (expression benchmarks only)");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run -c Release -- all");
        Console.WriteLine("  dotnet run -c Release -- expression");
        Console.WriteLine("  dotnet run -c Release -- operator");
        Console.WriteLine();
        Console.WriteLine("For BenchmarkDotNet's filter syntax:");
        Console.WriteLine("  dotnet run -c Release -- --filter *Contains*");
        Console.WriteLine("  dotnet run -c Release -- --filter OperatorBenchmarks.*");
        Console.WriteLine();
        Console.WriteLine("Results will be saved to BenchmarkDotNet.Artifacts/");
    }

    private static void RunAllBenchmarks(IConfig config)
    {
        Console.WriteLine("Running all benchmarks. This may take a while...");
        Console.WriteLine();

        BenchmarkRunner.Run<ExpressionBuildBenchmarks>(config);
        BenchmarkRunner.Run<OperatorBenchmarks>(config);
        BenchmarkRunner.Run<TypeBenchmarks>(config);
        BenchmarkRunner.Run<ComplexityBenchmarks>(config);
        BenchmarkRunner.Run<ScaleBenchmarks>(config);
        BenchmarkRunner.Run<CachingBenchmarks>(config);
        BenchmarkRunner.Run<CompilationBenchmarks>(config);

        Console.WriteLine();
        Console.WriteLine("All benchmarks complete. Results in BenchmarkDotNet.Artifacts/");
    }
}
