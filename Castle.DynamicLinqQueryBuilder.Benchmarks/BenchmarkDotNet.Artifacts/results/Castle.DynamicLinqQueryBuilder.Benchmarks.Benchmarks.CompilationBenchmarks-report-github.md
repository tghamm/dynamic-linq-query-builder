```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.7623)
Intel Core i9-14900KF, 1 CPU, 32 logical and 24 physical cores
.NET SDK 9.0.306
  [Host]   : .NET 8.0.21 (8.0.2125.47513), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.21 (8.0.2125.47513), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                      | Mean         | Error        | StdDev       | Ratio  | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
|---------------------------- |-------------:|-------------:|-------------:|-------:|--------:|--------:|-------:|----------:|------------:|
| BuildOnly_Simple            |     585.3 ns |     327.7 ns |     17.96 ns |   1.00 |    0.04 |  0.0830 |      - |   1.53 KB |        1.00 |
| BuildOnly_Medium            |   2,272.3 ns |   4,207.5 ns |    230.63 ns |   3.88 |    0.36 |  0.1984 |      - |   3.78 KB |        2.47 |
| BuildOnly_Complex           |   5,202.6 ns |   2,664.7 ns |    146.06 ns |   8.89 |    0.32 |  0.3967 |      - |   7.39 KB |        4.83 |
| BuildOnly_VeryComplex       |   8,074.2 ns |  13,464.1 ns |    738.01 ns |  13.80 |    1.15 |  0.6409 |      - |  12.33 KB |        8.05 |
| CompileOnly_Simple          |  28,699.8 ns |  27,216.9 ns |  1,491.85 ns |  49.07 |    2.56 |  0.1831 | 0.1221 |   4.21 KB |        2.75 |
| CompileOnly_Medium          | 170,768.6 ns |  36,833.9 ns |  2,018.99 ns | 291.96 |    8.21 |  0.2441 |      - |   5.45 KB |        3.56 |
| CompileOnly_Complex         | 317,065.4 ns |  74,060.3 ns |  4,059.49 ns | 542.08 |   15.41 |  0.4883 |      - |   9.71 KB |        6.34 |
| CompileOnly_VeryComplex     | 485,738.9 ns | 179,277.0 ns |  9,826.78 ns | 830.45 |   26.16 |       - |      - |  11.97 KB |        7.82 |
| BuildAndCompile_Simple      |  32,505.9 ns |  22,484.4 ns |  1,232.45 ns |  55.57 |    2.33 |  0.3052 | 0.1831 |   5.75 KB |        3.75 |
| BuildAndCompile_Medium      | 189,359.3 ns | 176,559.7 ns |  9,677.83 ns | 323.74 |   16.65 |  0.4883 |      - |   9.23 KB |        6.03 |
| BuildAndCompile_Complex     | 360,451.1 ns | 102,346.8 ns |  5,609.97 ns | 616.25 |   18.15 |  0.4883 |      - |  17.09 KB |       11.16 |
| BuildAndCompile_VeryComplex | 571,343.8 ns | 566,156.8 ns | 31,032.97 ns | 976.81 |   52.60 |  0.9766 |      - |  24.32 KB |       15.88 |
| CompileInterpreted_Simple   |     744.0 ns |     533.7 ns |     29.25 ns |   1.27 |    0.05 |  0.0677 |      - |   1.26 KB |        0.82 |
| CompileInterpreted_Complex  |   4,924.6 ns |   3,026.1 ns |    165.87 ns |   8.42 |    0.33 |  0.3815 |      - |   7.11 KB |        4.64 |
| CompileJIT_Simple           |  30,384.2 ns |  40,612.1 ns |  2,226.09 ns |  51.95 |    3.57 |  0.1831 | 0.1221 |   4.21 KB |        2.75 |
| CompileJIT_Complex          | 345,885.8 ns | 167,556.4 ns |  9,184.33 ns | 591.35 |   20.61 |  0.4883 |      - |   9.71 KB |        6.34 |
| Execute_Interpreted_Simple  |  53,563.3 ns |  50,363.4 ns |  2,760.59 ns |  91.58 |    4.74 |  9.8267 |      - | 181.19 KB |      118.33 |
| Execute_JIT_Simple          |  32,320.9 ns |  79,479.0 ns |  4,356.51 ns |  55.26 |    6.61 |  0.1831 | 0.1221 |   4.46 KB |        2.91 |
| Execute_Interpreted_Complex | 264,632.8 ns | 213,789.4 ns | 11,718.52 ns | 452.43 |   21.01 | 21.9727 |      - | 405.13 KB |      264.57 |
| Execute_JIT_Complex         | 352,796.6 ns |  73,370.5 ns |  4,021.68 ns | 603.16 |   16.88 |  2.9297 | 1.4648 |  58.39 KB |       38.13 |
| BuildPredicate_Simple       |  31,332.3 ns |   9,289.2 ns |    509.17 ns |  53.57 |    1.59 |  0.3052 | 0.1831 |   5.75 KB |        3.75 |
| BuildPredicate_Medium       | 185,337.0 ns |  30,970.7 ns |  1,697.61 ns | 316.86 |    8.67 |  0.4883 | 0.2441 |   9.23 KB |        6.03 |
| BuildPredicate_Complex      | 339,019.4 ns |  29,744.1 ns |  1,630.37 ns | 579.61 |   15.37 |  0.4883 |      - |  17.09 KB |       11.16 |
| BuildPredicate_VeryComplex  | 543,583.0 ns |  73,341.9 ns |  4,020.12 ns | 929.35 |   25.05 |  0.9766 |      - |  24.43 KB |       15.96 |
