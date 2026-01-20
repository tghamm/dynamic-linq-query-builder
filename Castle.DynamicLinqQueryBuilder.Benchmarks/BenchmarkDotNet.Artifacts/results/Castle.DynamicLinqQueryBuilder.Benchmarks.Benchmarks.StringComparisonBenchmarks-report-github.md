```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.7623)
Intel Core i9-14900KF, 1 CPU, 32 logical and 24 physical cores
.NET SDK 9.0.306
  [Host]   : .NET 8.0.21 (8.0.2125.47513), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.21 (8.0.2125.47513), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                        | Mean       | Error     | StdDev   | Ratio | RatioSD | Gen0    | Gen1    | Allocated  | Alloc Ratio |
|------------------------------ |-----------:|----------:|---------:|------:|--------:|--------:|--------:|-----------:|------------:|
| Equal_ToLower                 |   490.3 μs | 111.05 μs |  6.09 μs |  1.00 |    0.02 |  5.8594 |  2.9297 |  121.68 KB |        1.00 |
| Equal_OrdinalIgnoreCase       |   309.3 μs |  39.97 μs |  2.19 μs |  0.63 |    0.01 |  0.4883 |       - |   17.88 KB |        0.15 |
| Equal_CaseSensitive           |   301.1 μs |  51.51 μs |  2.82 μs |  0.61 |    0.01 |  0.4883 |       - |   17.86 KB |        0.15 |
| Contains_ToLower              |   377.4 μs | 142.99 μs |  7.84 μs |  0.77 |    0.02 |  3.4180 |  1.4648 |   66.52 KB |        0.55 |
| Contains_OrdinalIgnoreCase    |   313.8 μs |   8.02 μs |  0.44 μs |  0.64 |    0.01 |  0.4883 |       - |   17.67 KB |        0.15 |
| Contains_CaseSensitive        |   300.6 μs |  74.05 μs |  4.06 μs |  0.61 |    0.01 |  0.9766 |       - |   18.08 KB |        0.15 |
| BeginsWith_ToLower            |   368.6 μs |  67.11 μs |  3.68 μs |  0.75 |    0.01 |  3.4180 |  1.4648 |   68.54 KB |        0.56 |
| BeginsWith_OrdinalIgnoreCase  |   290.5 μs |  62.74 μs |  3.44 μs |  0.59 |    0.01 |  0.9766 |  0.4883 |   19.76 KB |        0.16 |
| BeginsWith_CaseSensitive      |   291.7 μs |  19.91 μs |  1.09 μs |  0.60 |    0.01 |  0.9766 |  0.4883 |   19.91 KB |        0.16 |
| EndsWith_ToLower              |   361.0 μs |  42.72 μs |  2.34 μs |  0.74 |    0.01 |  3.4180 |  1.4648 |    66.6 KB |        0.55 |
| EndsWith_OrdinalIgnoreCase    |   267.4 μs |  82.10 μs |  4.50 μs |  0.55 |    0.01 |  0.4883 |       - |   17.82 KB |        0.15 |
| EndsWith_CaseSensitive        |   282.1 μs |  39.23 μs |  2.15 μs |  0.58 |    0.01 |  0.4883 |       - |   17.87 KB |        0.15 |
| In5Strings_ToLower            |   935.5 μs | 155.24 μs |  8.51 μs |  1.91 |    0.03 | 25.3906 |  7.8125 |  478.48 KB |        3.93 |
| In5Strings_OrdinalIgnoreCase  |   404.8 μs |  34.16 μs |  1.87 μs |  0.83 |    0.01 |  0.9766 |  0.4883 |   22.25 KB |        0.18 |
| In20Strings_ToLower           | 2,155.6 μs | 761.73 μs | 41.75 μs |  4.40 |    0.09 | 97.6563 | 19.5313 | 1815.57 KB |       14.92 |
| In20Strings_OrdinalIgnoreCase |   691.8 μs |  75.22 μs |  4.12 μs |  1.41 |    0.02 |  1.9531 |  0.9766 |   37.47 KB |        0.31 |
| Complex_ToLower               |   607.4 μs |  35.60 μs |  1.95 μs |  1.24 |    0.01 |  8.7891 |  3.9063 |  175.77 KB |        1.44 |
| Complex_OrdinalIgnoreCase     |   450.6 μs | 136.92 μs |  7.50 μs |  0.92 |    0.02 |  0.9766 |       - |   25.44 KB |        0.21 |
| Complex_CaseSensitive         |   442.8 μs | 258.23 μs | 14.15 μs |  0.90 |    0.03 |  0.9766 |       - |   25.68 KB |        0.21 |
