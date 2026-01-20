```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.7623)
Intel Core i9-14900KF, 1 CPU, 32 logical and 24 physical cores
.NET SDK 9.0.306
  [Host]   : .NET 8.0.21 (8.0.2125.47513), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.21 (8.0.2125.47513), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                        | Mean           | Error           | StdDev        | Ratio  | RatioSD | Gen0     | Gen1    | Allocated | Alloc Ratio |
|------------------------------ |---------------:|----------------:|--------------:|-------:|--------:|---------:|--------:|----------:|------------:|
| BuildQuery_Simple_100         |   185,765.3 ns |    34,341.90 ns |   1,882.40 ns |  1.000 |    0.01 |   0.7324 |  0.4883 |   15109 B |       1.000 |
| BuildQuery_Simple_1000        |   190,450.3 ns |    36,968.67 ns |   2,026.38 ns |  1.025 |    0.01 |   0.4883 |       - |   15119 B |       1.001 |
| BuildQuery_Simple_10000       |   241,827.1 ns |    87,040.56 ns |   4,770.99 ns |  1.302 |    0.03 |   0.4883 |       - |   17131 B |       1.134 |
| BuildQuery_Simple_100000      |   998,186.3 ns | 2,067,879.53 ns | 113,347.46 ns |  5.374 |    0.53 |        - |       - |   31510 B |       2.086 |
| BuildQuery_Complex_100        |   420,130.2 ns |    21,200.58 ns |   1,162.08 ns |  2.262 |    0.02 |   0.9766 |       - |   26941 B |       1.783 |
| BuildQuery_Complex_1000       |   459,124.1 ns |    42,813.95 ns |   2,346.78 ns |  2.472 |    0.02 |   2.9297 |  0.9766 |   72364 B |       4.789 |
| BuildQuery_Complex_10000      |   733,901.8 ns |    78,920.00 ns |   4,325.87 ns |  3.951 |    0.04 |  27.3438 |  7.8125 |  534648 B |      35.386 |
| BuildQuery_Complex_100000     | 7,724,930.7 ns | 1,486,339.10 ns |  81,471.26 ns | 41.587 |    0.53 | 250.0000 | 31.2500 | 5093417 B |     337.111 |
| PreCompiled_Simple_100        |       121.3 ns |        20.50 ns |       1.12 ns |  0.001 |    0.00 |   0.0055 |       - |     104 B |       0.007 |
| PreCompiled_Simple_1000       |     1,283.1 ns |       106.02 ns |       5.81 ns |  0.007 |    0.00 |   0.0114 |       - |     248 B |       0.016 |
| PreCompiled_Simple_10000      |    14,905.0 ns |     2,168.24 ns |     118.85 ns |  0.080 |    0.00 |   0.0916 |       - |    2264 B |       0.150 |
| PreCompiled_Simple_100000     |   436,970.7 ns |   642,447.67 ns |  35,214.72 ns |  2.352 |    0.17 |   0.4883 |       - |   16672 B |       1.103 |
| PreCompiled_Complex_100       |     1,912.0 ns |     1,639.47 ns |      89.87 ns |  0.010 |    0.00 |   0.2785 |       - |    5296 B |       0.351 |
| PreCompiled_Complex_1000      |    20,567.6 ns |     3,460.40 ns |     189.68 ns |  0.111 |    0.00 |   2.6855 |       - |   50560 B |       3.346 |
| PreCompiled_Complex_10000     |   290,230.8 ns |   433,041.22 ns |  23,736.45 ns |  1.562 |    0.11 |  26.8555 |  1.4648 |  512960 B |      33.951 |
| PreCompiled_Complex_100000    | 7,429,916.9 ns | 7,521,375.09 ns | 412,271.95 ns | 39.999 |    1.95 | 257.8125 | 23.4375 | 5071334 B |     335.650 |
| BuildOnly_Simple_NoExecution  |       541.5 ns |       397.86 ns |      21.81 ns |  0.003 |    0.00 |   0.0830 |       - |    1568 B |       0.104 |
| BuildOnly_Complex_NoExecution |     3,231.2 ns |     4,056.13 ns |     222.33 ns |  0.017 |    0.00 |   0.3052 |       - |    6000 B |       0.397 |
| BuildAndCompile_Simple        |    36,385.9 ns |    28,157.43 ns |   1,543.40 ns |  0.196 |    0.01 |   0.3052 |  0.2441 |    5886 B |       0.390 |
| BuildAndCompile_Complex       |   250,327.5 ns |   124,534.62 ns |   6,826.16 ns |  1.348 |    0.03 |   0.4883 |       - |   12643 B |       0.837 |
