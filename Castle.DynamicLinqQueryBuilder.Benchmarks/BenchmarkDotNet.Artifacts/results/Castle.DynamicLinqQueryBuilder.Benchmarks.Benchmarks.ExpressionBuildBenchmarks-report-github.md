```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.7623)
Intel Core i9-14900KF, 1 CPU, 32 logical and 24 physical cores
.NET SDK 9.0.306
  [Host]   : .NET 8.0.21 (8.0.2125.47513), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.21 (8.0.2125.47513), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                             | Mean             | Error            | StdDev           | Ratio      | RatioSD   | Gen0   | Allocated   | Alloc Ratio |
|----------------------------------- |-----------------:|-----------------:|-----------------:|-----------:|----------:|-------:|------------:|------------:|
| BuildExpression_SimpleProperty     |         596.3 ns |         197.8 ns |         10.84 ns |       1.00 |      0.02 | 0.0801 |     1.53 KB |        1.00 |
| BuildExpression_NestedProperty     |       7,119.8 ns |       1,771.3 ns |         97.09 ns |      11.94 |      0.24 | 0.5493 |    10.37 KB |        6.77 |
| BuildExpression_CollectionProperty |       1,981.1 ns |         319.4 ns |         17.50 ns |       3.32 |      0.06 | 0.1831 |     3.42 KB |        2.23 |
| BuildExpression_DictionaryAccess   |       2,136.8 ns |         206.0 ns |         11.29 ns |       3.58 |      0.06 | 0.1678 |     3.09 KB |        2.02 |
| BuildQuery_SimpleProperty          |       2,702.1 ns |         401.2 ns |         21.99 ns |       4.53 |      0.08 | 0.2518 |     4.73 KB |        3.09 |
| BuildQuery_NestedProperty          |       9,473.8 ns |       1,064.6 ns |         58.36 ns |      15.89 |      0.26 | 0.7324 |    13.68 KB |        8.93 |
| BuildAndExecute_SimpleProperty     |     190,441.2 ns |      14,347.9 ns |        786.46 ns |     319.46 |      5.16 | 0.4883 |    14.71 KB |        9.61 |
| BuildAndExecute_NestedProperty     | 313,351,700.0 ns | 226,912,640.4 ns | 12,437,847.55 ns | 525,646.82 | 19,878.40 |      - | 10610.36 KB |    6,929.21 |
