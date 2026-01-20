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
| BuildQuery_Simple_100         |   180,691.2 ns |     8,473.82 ns |     464.48 ns |  1.000 |    0.00 |   0.4883 |       - |   14975 B |       1.000 |
| BuildQuery_Simple_1000        |   190,008.0 ns |   115,254.26 ns |   6,317.48 ns |  1.052 |    0.03 |   0.4883 |       - |   15118 B |       1.010 |
| BuildQuery_Simple_10000       |   250,143.6 ns |   170,898.74 ns |   9,367.54 ns |  1.384 |    0.05 |   0.4883 |       - |   17131 B |       1.144 |
| BuildQuery_Simple_100000      | 1,313,853.8 ns | 3,174,128.27 ns | 173,984.68 ns |  7.271 |    0.83 |        - |       - |   31507 B |       2.104 |
| BuildQuery_Complex_100        |   446,108.3 ns |   150,753.96 ns |   8,263.33 ns |  2.469 |    0.04 |   0.9766 |       - |   27069 B |       1.808 |
| BuildQuery_Complex_1000       |   486,241.5 ns |   296,948.35 ns |  16,276.74 ns |  2.691 |    0.08 |   2.9297 |  0.9766 |   72340 B |       4.831 |
| BuildQuery_Complex_10000      |   808,704.7 ns | 1,279,408.86 ns |  70,128.72 ns |  4.476 |    0.34 |  27.3438 |  7.8125 |  534780 B |      35.712 |
| BuildQuery_Complex_100000     | 7,808,813.3 ns | 3,939,433.08 ns | 215,933.62 ns | 43.217 |    1.04 | 250.0000 | 31.2500 | 5093583 B |     340.139 |
| PreCompiled_Simple_100        |       121.7 ns |        31.41 ns |       1.72 ns |  0.001 |    0.00 |   0.0055 |       - |     104 B |       0.007 |
| PreCompiled_Simple_1000       |     1,334.3 ns |       123.06 ns |       6.75 ns |  0.007 |    0.00 |   0.0114 |       - |     248 B |       0.017 |
| PreCompiled_Simple_10000      |    16,590.9 ns |    19,827.66 ns |   1,086.82 ns |  0.092 |    0.01 |   0.0916 |       - |    2264 B |       0.151 |
| PreCompiled_Simple_100000     |   376,437.2 ns |   206,246.63 ns |  11,305.07 ns |  2.083 |    0.05 |   0.4883 |       - |   16672 B |       1.113 |
| PreCompiled_Complex_100       |     1,896.1 ns |        99.26 ns |       5.44 ns |  0.010 |    0.00 |   0.2785 |       - |    5296 B |       0.354 |
| PreCompiled_Complex_1000      |    20,982.6 ns |     9,847.20 ns |     539.76 ns |  0.116 |    0.00 |   2.6855 |       - |   50560 B |       3.376 |
| PreCompiled_Complex_10000     |   278,781.8 ns |    29,836.11 ns |   1,635.42 ns |  1.543 |    0.01 |  26.8555 |  1.4648 |  512960 B |      34.254 |
| PreCompiled_Complex_100000    | 7,321,482.3 ns | 1,274,547.29 ns |  69,862.24 ns | 40.519 |    0.35 | 257.8125 | 23.4375 | 5071331 B |     338.653 |
| BuildOnly_Simple_NoExecution  |       683.5 ns |        48.29 ns |       2.65 ns |  0.004 |    0.00 |   0.0887 |       - |    1680 B |       0.112 |
| BuildOnly_Complex_NoExecution |     3,426.7 ns |     1,616.23 ns |      88.59 ns |  0.019 |    0.00 |   0.3242 |       - |    6128 B |       0.409 |
| BuildAndCompile_Simple        |    32,324.3 ns |    32,471.80 ns |   1,779.89 ns |  0.179 |    0.01 |   0.3052 |  0.1831 |    5885 B |       0.393 |
| BuildAndCompile_Complex       |   244,646.2 ns |   145,402.15 ns |   7,969.98 ns |  1.354 |    0.04 |   0.4883 |       - |   13314 B |       0.889 |
