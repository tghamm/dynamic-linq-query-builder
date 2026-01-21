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
| BuildExpression_SimpleProperty     |         558.3 ns |         454.9 ns |         24.94 ns |       1.00 |      0.05 | 0.0801 |     1.53 KB |        1.00 |
| BuildExpression_NestedProperty     |       7,740.7 ns |       6,163.8 ns |        337.86 ns |      13.88 |      0.75 | 0.5493 |    10.29 KB |        6.72 |
| BuildExpression_CollectionProperty |       1,997.6 ns |       2,739.8 ns |        150.18 ns |       3.58 |      0.27 | 0.1755 |      3.3 KB |        2.16 |
| BuildExpression_DictionaryAccess   |       2,332.7 ns |       4,012.7 ns |        219.95 ns |       4.18 |      0.38 | 0.1755 |      3.3 KB |        2.15 |
| BuildQuery_SimpleProperty          |       2,610.0 ns |       1,592.0 ns |         87.26 ns |       4.68 |      0.23 | 0.2441 |     4.73 KB |        3.09 |
| BuildQuery_NestedProperty          |       9,748.5 ns |       5,340.2 ns |        292.72 ns |      17.48 |      0.81 | 0.7324 |    13.64 KB |        8.91 |
| BuildAndExecute_SimpleProperty     |     191,904.6 ns |      65,142.5 ns |      3,570.68 ns |     344.17 |     14.41 | 0.4883 |    14.71 KB |        9.61 |
| BuildAndExecute_NestedProperty     | 314,293,633.3 ns | 354,777,146.5 ns | 19,446,532.62 ns | 563,659.76 | 37,248.57 |      - | 10609.05 KB |    6,928.36 |
