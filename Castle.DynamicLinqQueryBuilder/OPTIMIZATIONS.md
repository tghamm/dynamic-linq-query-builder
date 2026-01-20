# Castle.DynamicLinqQueryBuilder Performance Optimizations

This document outlines a comprehensive performance optimization plan for Castle.DynamicLinqQueryBuilder, including implementation guidance, testing protocols, and baseline metrics.

## Table of Contents

1. [Baseline Metrics](#baseline-metrics)
2. [Optimization Roadmap](#optimization-roadmap)
3. [Implementation Details](#implementation-details)
4. [Testing Protocol](#testing-protocol)
5. [Post-Optimization Metrics](#post-optimization-metrics)

---

## Baseline Metrics

> **Benchmark Environment:** Windows 11 (10.0.26200.7623) - Intel Core i9-14900KF, 32 logical / 24 physical cores  
> **Date:** 2026-01-19  
> **Version:** 1.3.4  
> **.NET Runtime:** .NET 8.0.21 (8.0.2125.47513), X64 RyuJIT AVX2

### Expression Build Benchmarks

| Method                             | Mean           | Error            | StdDev          | Ratio      | Gen0   | Allocated   |
|----------------------------------- |---------------:|-----------------:|----------------:|-----------:|-------:|------------:|
| BuildExpression_SimpleProperty     |         596 ns |         198 ns   |         11 ns   |       1.00 | 0.0801 |     1.53 KB |
| BuildExpression_NestedProperty     |       7,120 ns |       1,771 ns   |         97 ns   |      11.94 | 0.5493 |    10.37 KB |
| BuildExpression_CollectionProperty |       1,981 ns |         319 ns   |         18 ns   |       3.32 | 0.1831 |     3.42 KB |
| BuildExpression_DictionaryAccess   |       2,137 ns |         206 ns   |         11 ns   |       3.58 | 0.1678 |     3.09 KB |
| BuildQuery_SimpleProperty          |       2,702 ns |         401 ns   |         22 ns   |       4.53 | 0.2518 |     4.73 KB |
| BuildQuery_NestedProperty          |       9,474 ns |       1,065 ns   |         58 ns   |      15.89 | 0.7324 |    13.68 KB |
| BuildAndExecute_SimpleProperty     |     190,441 ns |      14,348 ns   |        786 ns   |     319.46 | 0.4883 |    14.71 KB |
| BuildAndExecute_NestedProperty     | 313,352,000 ns | 226,913,000 ns   | 12,438,000 ns   | 525,647.00 |      - | 10610.36 KB |

### Operator Benchmarks

| Method                       | Mean       | Error       | StdDev   | Ratio | Gen0    | Gen1   | Allocated |
|----------------------------- |-----------:|------------:|---------:|------:|--------:|-------:|----------:|
| Equal_Integer                |   193.6 μs |     7.41 μs |  0.41 μs |  1.00 |  0.4883 |      - |  14.71 KB |
| NotEqual_Integer             |   201.9 μs |    18.80 μs |  1.03 μs |  1.04 |  1.4648 | 0.9766 |  30.82 KB |
| Equal_String_CaseInsensitive |   488.4 μs |    12.64 μs |  0.69 μs |  2.52 |  5.8594 | 2.9297 | 121.68 KB |
| Equal_String_CaseSensitive   |   295.9 μs |    67.88 μs |  3.72 μs |  1.53 |  0.4883 |      - |  17.86 KB |
| NotEqual_String              |   498.5 μs |    91.21 μs |  5.00 μs |  2.58 |  6.8359 | 2.9297 | 135.81 KB |
| In_5Values                   |   234.3 μs |    13.68 μs |  0.75 μs |  1.21 |  0.9766 | 0.4883 |  19.02 KB |
| In_50Values                  |   447.0 μs |   203.93 μs | 11.18 μs |  2.31 |  2.9297 | 1.9531 |  62.08 KB |
| In_500Values                 | 3,525.2 μs | 1,074.17 μs | 58.88 μs | 18.21 | 15.6250 | 7.8125 | 405.41 KB |
| NotIn_5Values                |   245.3 μs |    65.54 μs |  3.59 μs |  1.27 |  1.4648 | 0.9766 |  34.18 KB |
| Less                         |   203.5 μs |    56.69 μs |  3.11 μs |  1.05 |  0.9766 | 0.4883 |  22.73 KB |
| LessOrEqual                  |   219.8 μs |    56.66 μs |  3.11 μs |  1.14 |  0.9766 | 0.4883 |  22.74 KB |
| Greater                      |   213.9 μs |   144.21 μs |  7.90 μs |  1.11 |  0.9766 | 0.4883 |  22.73 KB |
| GreaterOrEqual               |   243.3 μs |   298.06 μs | 16.34 μs |  1.26 |  0.9766 | 0.4883 |  22.74 KB |
| Between                      |   246.4 μs |   258.15 μs | 14.15 μs |  1.27 |  1.4648 | 0.9766 |  31.57 KB |
| NotBetween                   |   242.0 μs |   149.20 μs |  8.18 μs |  1.25 |  0.9766 | 0.4883 |  19.58 KB |
| BeginsWith_CaseInsensitive   |   413.2 μs |   612.69 μs | 33.58 μs |  2.13 |  2.9297 | 0.9766 |  68.69 KB |
| BeginsWith_CaseSensitive     |   300.1 μs |   143.91 μs |  7.89 μs |  1.55 |  0.9766 | 0.4883 |  19.95 KB |
| NotBeginsWith                |   415.8 μs |   169.33 μs |  9.28 μs |  2.15 |  4.3945 |      - |  80.92 KB |
| Contains_CaseInsensitive     |   448.8 μs |   472.09 μs | 25.88 μs |  2.32 |  2.9297 | 0.9766 |  66.68 KB |
| Contains_CaseSensitive       |   321.0 μs |   377.64 μs | 20.70 μs |  1.66 |  0.4883 |      - |  17.91 KB |
| NotContains                  |   402.5 μs |   476.14 μs | 26.10 μs |  2.08 |  3.9063 | 1.9531 |  81.08 KB |
| EndsWith_CaseInsensitive     |   394.9 μs |   313.50 μs | 17.18 μs |  2.04 |  2.9297 | 0.9766 |  66.67 KB |
| EndsWith_CaseSensitive       |   282.3 μs |    77.52 μs |  4.25 μs |  1.46 |  0.4883 |      - |  17.91 KB |
| NotEndsWith                  |   384.4 μs |   131.50 μs |  7.21 μs |  1.99 |  4.3945 | 1.9531 |  80.93 KB |
| IsNull                       |   225.2 μs |    67.95 μs |  3.72 μs |  1.16 |  0.9766 | 0.4883 |  18.76 KB |
| IsNotNull                    |   238.1 μs |    54.66 μs |  3.00 μs |  1.23 |  1.4648 | 0.9766 |  30.86 KB |
| IsEmpty                      |   256.6 μs |   178.28 μs |  9.77 μs |  1.33 |  0.4883 |      - |  15.48 KB |
| IsNotEmpty                   |   255.7 μs |    23.16 μs |  1.27 μs |  1.32 |  1.4648 | 0.9766 |  31.74 KB |

### Type Benchmarks

| Method                               | Mean       | Error     | StdDev   | Ratio | Gen0    | Gen1    | Allocated |
|------------------------------------- |-----------:|----------:|---------:|------:|--------:|--------:|----------:|
| Type_Integer                         |   194.4 μs |  71.77 μs |  3.93 μs |  1.00 |  0.4883 |       - |  14.72 KB |
| Type_Long                            |   202.9 μs |  44.23 μs |  2.42 μs |  1.04 |  0.4883 |       - |  14.63 KB |
| Type_Double                          |   213.6 μs |  82.30 μs |  4.51 μs |  1.10 |  0.4883 |       - |   14.6 KB |
| Type_String                          |   523.1 μs | 246.02 μs | 13.48 μs |  2.69 |  5.8594 |  2.9297 | 121.68 KB |
| Type_Date                            |   329.2 μs | 198.01 μs | 10.85 μs |  1.69 |  0.4883 |       - |  16.07 KB |
| Type_DateTime_Local                  |   333.9 μs | 227.94 μs | 12.49 μs |  1.72 |  0.4883 |       - |  15.71 KB |
| Type_DateTime_UTC                    |   328.5 μs | 137.89 μs |  7.56 μs |  1.69 |  0.4883 |       - |  15.74 KB |
| Type_Boolean                         |   221.3 μs |  33.68 μs |  1.85 μs |  1.14 |  1.4648 |  0.9766 |  30.89 KB |
| Type_Guid                            |   340.4 μs |  70.10 μs |  3.84 μs |  1.75 |  0.4883 |       - |  15.97 KB |
| Type_NullableInteger                 |   255.5 μs |  52.56 μs |  2.88 μs |  1.32 |  0.4883 |       - |  15.52 KB |
| Type_NullableLong                    |   255.4 μs | 106.32 μs |  5.83 μs |  1.31 |  0.4883 |       - |  15.52 KB |
| Type_NullableDouble                  |   253.3 μs | 117.04 μs |  6.42 μs |  1.30 |  0.4883 |       - |  15.51 KB |
| Type_NullableDateTime                |   430.8 μs | 838.34 μs | 45.95 μs |  2.22 |  0.9766 |       - |  18.11 KB |
| Type_NullableBoolean                 |   258.8 μs |  72.17 μs |  3.96 μs |  1.33 |  0.9766 |  0.4883 |  23.66 KB |
| Type_NullableGuid                    |   467.9 μs | 620.79 μs | 34.03 μs |  2.41 |  0.9766 |       - |  18.23 KB |
| TypeCollection_Integer_10Values      |   315.7 μs | 318.96 μs | 17.48 μs |  1.62 |  0.9766 |  0.4883 |  23.61 KB |
| TypeCollection_String_10Values       | 1,621.3 μs | 714.20 μs | 39.15 μs |  8.34 | 46.8750 | 11.7188 | 906.25 KB |
| TypeCollection_DateTime_10Values     |   574.3 μs | 347.26 μs | 19.03 μs |  2.96 |  0.9766 |       - |  30.75 KB |
| TypeCollection_DateTime_10Values_UTC |   628.3 μs | 281.93 μs | 15.45 μs |  3.23 |  0.9766 |       - |  30.72 KB |
| TypeCollection_Guid_10Values         |   745.5 μs | 468.33 μs | 25.67 μs |  3.84 |  0.9766 |       - |  31.42 KB |

### Complexity Benchmarks

| Method                     | Mean         | Error        | StdDev       | Ratio | Gen0   | Gen1   | Allocated |
|--------------------------- |-------------:|-------------:|-------------:|------:|-------:|-------:|----------:|
| Flat_1Rule                 |    215,167 ns |   421,570 ns |   23,108 ns  | 1.007 | 0.4883 |      - |  14.83 KB |
| Flat_5Rules                |    267,391 ns |   110,531 ns |    6,059 ns  | 1.252 | 0.9766 | 0.4883 |  18.42 KB |
| Flat_10Rules               |    271,566 ns |   221,538 ns |   12,143 ns  | 1.271 | 0.9766 | 0.4883 |  23.31 KB |
| Flat_20Rules               |    349,326 ns |   414,244 ns |   22,706 ns  | 1.635 | 1.4648 | 0.9766 |  35.01 KB |
| FlatOr_5Rules              |    252,465 ns |   144,544 ns |    7,923 ns  | 1.182 | 0.9766 | 0.4883 |   19.5 KB |
| FlatOr_10Rules             |    271,282 ns |    49,216 ns |    2,698 ns  | 1.270 | 0.9766 | 0.4883 |  25.39 KB |
| FlatOr_20Rules             |    326,554 ns |   150,206 ns |    8,233 ns  | 1.529 | 1.9531 | 1.4648 |  37.05 KB |
| Nested_2Levels             |    239,060 ns |   205,414 ns |   11,259 ns  | 1.119 | 0.4883 |      - |  15.97 KB |
| Nested_3Levels             |    231,603 ns |    71,177 ns |    3,901 ns  | 1.084 | 0.4883 |      - |  17.41 KB |
| Nested_5Levels             |    236,346 ns |    71,445 ns |    3,916 ns  | 1.107 | 0.9766 | 0.4883 |  19.65 KB |
| Nested_10Levels            |    267,758 ns |   249,353 ns |   13,668 ns  | 1.254 | 0.9766 | 0.4883 |  26.03 KB |
| Mixed_2x2                  |    480,153 ns |   262,195 ns |   14,372 ns  | 2.248 | 0.9766 |      - |  22.82 KB |
| Mixed_3x3                  |    554,316 ns |   112,392 ns |    6,161 ns  | 2.595 | 0.9766 |      - |  27.05 KB |
| Mixed_5x5                  |    720,207 ns |   595,077 ns |   32,618 ns  | 3.372 | 1.9531 | 0.9766 |  36.21 KB |
| ComplexRealWorld           |    622,816 ns |   178,034 ns |    9,759 ns  | 2.916 | 3.9063 | 1.9531 |  72.86 KB |
| BuildOnly_Flat_1Rule       |        633 ns |       242 ns |       13 ns  | 0.003 | 0.0830 |      - |   1.53 KB |
| BuildOnly_Flat_20Rules     |      7,902 ns |     4,531 ns |      248 ns  | 0.037 | 0.9155 | 0.0153 |  16.83 KB |
| BuildOnly_Nested_10Levels  |      4,849 ns |     2,434 ns |      133 ns  | 0.023 | 0.5722 |      - |  10.57 KB |
| BuildOnly_ComplexRealWorld |      4,897 ns |     3,663 ns |      201 ns  | 0.023 | 0.3967 |      - |    7.8 KB |

### Scale Benchmarks

| Method                        | Mean           | Error           | StdDev        | Ratio  | Gen0     | Gen1    | Allocated |
|------------------------------ |---------------:|----------------:|--------------:|-------:|---------:|--------:|----------:|
| BuildQuery_Simple_100         |     180,691 ns |       8,474 ns  |       464 ns  |  1.000 |   0.4883 |       - |   15.0 KB |
| BuildQuery_Simple_1000        |     190,008 ns |     115,254 ns  |     6,317 ns  |  1.052 |   0.4883 |       - |   14.8 KB |
| BuildQuery_Simple_10000       |     250,144 ns |     170,899 ns  |     9,368 ns  |  1.384 |   0.4883 |       - |   16.7 KB |
| BuildQuery_Simple_100000      |   1,313,854 ns |   3,174,128 ns  |   173,985 ns  |  7.271 |        - |       - |   30.8 KB |
| BuildQuery_Complex_100        |     446,108 ns |     150,754 ns  |     8,263 ns  |  2.469 |   0.9766 |       - |   26.4 KB |
| BuildQuery_Complex_1000       |     486,242 ns |     296,948 ns  |    16,277 ns  |  2.691 |   2.9297 |  0.9766 |   70.6 KB |
| BuildQuery_Complex_10000      |     808,705 ns |   1,279,409 ns  |    70,129 ns  |  4.476 |  27.3438 |  7.8125 |  522.2 KB |
| BuildQuery_Complex_100000     |   7,808,813 ns |   3,939,433 ns  |   215,934 ns  | 43.217 | 250.0000 | 31.2500 | 4974.0 KB |
| PreCompiled_Simple_100        |         122 ns |          31 ns  |         2 ns  |  0.001 |   0.0055 |       - |    0.1 KB |
| PreCompiled_Simple_1000       |       1,334 ns |         123 ns  |         7 ns  |  0.007 |   0.0114 |       - |    0.2 KB |
| PreCompiled_Simple_10000      |      16,591 ns |      19,828 ns  |     1,087 ns  |  0.092 |   0.0916 |       - |    2.2 KB |
| PreCompiled_Simple_100000     |     376,437 ns |     206,247 ns  |    11,305 ns  |  2.083 |   0.4883 |       - |   16.3 KB |
| PreCompiled_Complex_100       |       1,896 ns |          99 ns  |         5 ns  |  0.010 |   0.2785 |       - |    5.2 KB |
| PreCompiled_Complex_1000      |      20,983 ns |       9,847 ns  |       540 ns  |  0.116 |   2.6855 |       - |   49.4 KB |
| PreCompiled_Complex_10000     |     278,782 ns |      29,836 ns  |     1,635 ns  |  1.543 |  26.8555 |  1.4648 |  501.0 KB |
| PreCompiled_Complex_100000    |   7,321,482 ns |   1,274,547 ns  |    69,862 ns  | 40.519 | 257.8125 | 23.4375 | 4952.5 KB |
| BuildOnly_Simple_NoExecution  |         684 ns |          48 ns  |         3 ns  |  0.004 |   0.0887 |       - |    1.6 KB |
| BuildOnly_Complex_NoExecution |       3,427 ns |       1,616 ns  |        89 ns  |  0.019 |   0.3242 |       - |    6.0 KB |
| BuildAndCompile_Simple        |      32,324 ns |      32,472 ns  |     1,780 ns  |  0.179 |   0.3052 |  0.1831 |    5.7 KB |
| BuildAndCompile_Complex       |     244,646 ns |     145,402 ns  |     7,970 ns  |  1.354 |   0.4883 |       - |   13.0 KB |

### Caching Benchmarks (Demonstrates Optimization Potential)

| Method                      | Mean              | Error             | StdDev          | Ratio      | Gen0    | Gen1    | Allocated |
|---------------------------- |------------------:|------------------:|----------------:|-----------:|--------:|--------:|----------:|
| Build_Simple_Once           |           588 ns  |           350 ns  |        19 ns    |      1.001 |  0.0830 |       - |    1568 B |
| UseCached_Simple_Once       |         0.008 ns  |         0.162 ns  |     0.009 ns    |      0.000 |       - |       - |         - |
| Build_Simple_10x            |         5,807 ns  |           702 ns  |        39 ns    |      9.876 |  0.8316 |       - |   15680 B |
| Build_Simple_100x           |        61,761 ns  |        49,264 ns  |     2,700 ns    |    105.027 |  8.3008 |       - |  156802 B |
| Build_Simple_1000x          |       608,186 ns  |       763,775 ns  |    41,865 ns    |  1,034.244 | 82.0313 |       - | 1568022 B |
| Build_Medium_Once           |         2,362 ns  |         1,133 ns  |        62 ns    |      4.016 |  0.1984 |       - |    3872 B |
| Build_Medium_10x            |        21,132 ns  |        20,235 ns  |     1,109 ns    |     35.935 |  1.9531 |       - |   38722 B |
| Build_Medium_100x           |       205,683 ns  |       155,615 ns  |     8,530 ns    |    349.772 | 20.5078 |       - |  387218 B |
| Build_Complex_Once          |         6,179 ns  |         7,362 ns  |       404 ns    |     10.507 |  0.5493 |       - |   10409 B |
| Build_Complex_10x           |        57,769 ns  |        53,016 ns  |     2,906 ns    |     98.238 |  5.3711 |       - |  104086 B |
| Build_Complex_100x          |       635,556 ns  |       628,102 ns  |    34,428 ns    |  1,080.788 | 54.6875 |       - | 1040865 B |
| BuildAndExecute_Simple      |       192,720 ns  |        75,833 ns  |     4,157 ns    |    327.727 |  0.4883 |       - |   15062 B |
| **CachedExecute_Simple**    |     **1,252 ns** |         **120 ns** |     **7 ns**   |  **2.130** |  0.0114 |       - |     248 B |
| BuildAndExecute_Complex     |       691,972 ns  |       102,629 ns  |     5,625 ns    |  1,176.725 |  3.9063 |  1.9531 |   79382 B |
| **CachedExecute_Complex**   |    **18,901 ns** |       **7,375 ns** |   **404 ns**   | **32.143** |  2.5635 |       - |   48800 B |
| BuildAndExecute_Simple_10x  |     1,954,312 ns  |       198,619 ns  |    10,887 ns    |  3,323.383 |  7.8125 |  3.9063 |  150720 B |
| CachedExecute_Simple_10x    |        13,091 ns  |           710 ns  |        39 ns    |     22.261 |  0.1221 |       - |    2480 B |
| BuildAndExecute_Complex_10x |     7,734,742 ns  |     1,564,442 ns  |    85,752 ns    | 13,153.228 | 31.2500 | 15.6250 |  794848 B |
| CachedExecute_Complex_10x   |       201,478 ns  |       196,439 ns  |    10,767 ns    |    342.621 | 25.8789 |       - |  488000 B |
| BuildAndCompile_Simple      |        40,371 ns  |        14,169 ns  |       777 ns    |     68.652 |  0.3052 |  0.1831 |    5886 B |
| BuildAndCompile_Complex     |       477,629 ns  |       635,880 ns  |    34,855 ns    |    812.226 |  0.9766 |       - |   21327 B |
| BuildAndCompile_Simple_10x  |       412,549 ns  |       599,096 ns  |    32,838 ns    |    701.555 |  2.9297 |  2.4414 |   58866 B |
| BuildAndCompile_Complex_10x |     4,704,455 ns  |     3,204,736 ns  |   175,662 ns    |  8,000.107 |  7.8125 |       - |  214105 B |

### Compilation Benchmarks

| Method                      | Mean         | Error        | StdDev       | Ratio  | Gen0    | Gen1   | Allocated |
|---------------------------- |-------------:|-------------:|-------------:|-------:|--------:|-------:|----------:|
| BuildOnly_Simple            |       585 ns |       328 ns |       18 ns  |   1.00 |  0.0830 |      - |   1.53 KB |
| BuildOnly_Medium            |     2,272 ns |     4,208 ns |      231 ns  |   3.88 |  0.1984 |      - |   3.78 KB |
| BuildOnly_Complex           |     5,203 ns |     2,665 ns |      146 ns  |   8.89 |  0.3967 |      - |   7.39 KB |
| BuildOnly_VeryComplex       |     8,074 ns |    13,464 ns |      738 ns  |  13.80 |  0.6409 |      - |  12.33 KB |
| CompileOnly_Simple          |    28,700 ns |    27,217 ns |    1,492 ns  |  49.07 |  0.1831 | 0.1221 |   4.21 KB |
| CompileOnly_Medium          |   170,769 ns |    36,834 ns |    2,019 ns  | 291.96 |  0.2441 |      - |   5.45 KB |
| CompileOnly_Complex         |   317,065 ns |    74,060 ns |    4,059 ns  | 542.08 |  0.4883 |      - |   9.71 KB |
| CompileOnly_VeryComplex     |   485,739 ns |   179,277 ns |    9,827 ns  | 830.45 |       - |      - |  11.97 KB |
| BuildAndCompile_Simple      |    32,506 ns |    22,484 ns |    1,232 ns  |  55.57 |  0.3052 | 0.1831 |   5.75 KB |
| BuildAndCompile_Medium      |   189,359 ns |   176,560 ns |    9,678 ns  | 323.74 |  0.4883 |      - |   9.23 KB |
| BuildAndCompile_Complex     |   360,451 ns |   102,347 ns |    5,610 ns  | 616.25 |  0.4883 |      - |  17.09 KB |
| BuildAndCompile_VeryComplex |   571,344 ns |   566,157 ns |   31,033 ns  | 976.81 |  0.9766 |      - |  24.32 KB |
| CompileInterpreted_Simple   |       744 ns |       534 ns |       29 ns  |   1.27 |  0.0677 |      - |   1.26 KB |
| CompileInterpreted_Complex  |     4,925 ns |     3,026 ns |      166 ns  |   8.42 |  0.3815 |      - |   7.11 KB |
| CompileJIT_Simple           |    30,384 ns |    40,612 ns |    2,226 ns  |  51.95 |  0.1831 | 0.1221 |   4.21 KB |
| CompileJIT_Complex          |   345,886 ns |   167,556 ns |    9,184 ns  | 591.35 |  0.4883 |      - |   9.71 KB |
| Execute_Interpreted_Simple  |    53,563 ns |    50,363 ns |    2,761 ns  |  91.58 |  9.8267 |      - | 181.19 KB |
| Execute_JIT_Simple          |    32,321 ns |    79,479 ns |    4,357 ns  |  55.26 |  0.1831 | 0.1221 |   4.46 KB |
| Execute_Interpreted_Complex |   264,633 ns |   213,789 ns |   11,719 ns  | 452.43 | 21.9727 |      - | 405.13 KB |
| Execute_JIT_Complex         |   352,797 ns |    73,371 ns |    4,022 ns  | 603.16 |  2.9297 | 1.4648 |  58.39 KB |
| BuildPredicate_Simple       |    31,332 ns |     9,289 ns |      509 ns  |  53.57 |  0.3052 | 0.1831 |   5.75 KB |
| BuildPredicate_Medium       |   185,337 ns |    30,971 ns |    1,698 ns  | 316.86 |  0.4883 | 0.2441 |   9.23 KB |
| BuildPredicate_Complex      |   339,019 ns |    29,744 ns |    1,630 ns  | 579.61 |  0.4883 |      - |  17.09 KB |
| BuildPredicate_VeryComplex  |   543,583 ns |    73,342 ns |    4,020 ns  | 929.35 |  0.9766 |      - |  24.43 KB |

---

## Optimization Roadmap

### Priority 1: High Impact / Low-Medium Effort

#### 1.1 Expression Caching
**Impact:** 🔥🔥🔥 | **Effort:** Medium | **Risk:** Low

Cache built expression trees to avoid rebuilding for identical filter patterns.

**Implementation:**
- Add `ConcurrentDictionary<string, Expression>` cache
- Generate cache key from filter structure (field/operator/type, not values)
- Implement configurable cache size with LRU eviction
- Add `BuildExpressionOptions.EnableCaching` flag

**Files to modify:**
- `QueryBuilder.cs` - Add caching layer in `BuildExpressionLambda<T>()`
- `BuildExpressionOptions.cs` - Add caching configuration options

**Expected improvement:** 50-90% reduction in repeated query build time

---

#### 1.2 PropertyInfo/MethodInfo Caching
**Impact:** 🔥🔥 | **Effort:** Low | **Risk:** Low

Cache reflection lookups for property and method access.

**Implementation:**
- Add `ConcurrentDictionary<(Type, string), PropertyInfo>` for property lookups
- Add `ConcurrentDictionary<(Type, string, Type[]), MethodInfo>` for method lookups
- Replace direct `Type.GetProperty()` / `Type.GetMethod()` calls

**Files to modify:**
- `ReflectionHelpers.cs` - Add cached lookup methods
- `QueryBuilder.cs` - Replace reflection calls with cached versions

**Expected improvement:** 10-20% reduction in expression build time

---

#### 1.3 TypeConverter Caching
**Impact:** 🔥 | **Effort:** Low | **Risk:** Low

Cache `TypeDescriptor.GetConverter()` results.

**Implementation:**
- Add `ConcurrentDictionary<Type, TypeConverter>` cache
- Create helper method `GetCachedConverter(Type type)`

**Files to modify:**
- `QueryBuilder.cs` - Add cache and replace calls in `GetConstants()`

**Expected improvement:** 5-10% reduction in type conversion overhead

---

#### 1.4 Operator Dispatch Optimization
**Impact:** 🔥 | **Effort:** Low | **Risk:** Low

Replace switch statement with dictionary dispatch; eliminate `.ToLower()` allocation.

**Implementation:**
- Create `FrozenDictionary<string, Func<...>>` for operator dispatch (net8+)
- Store operators in lowercase, normalize input once
- Use `StringComparer.OrdinalIgnoreCase` for custom operator lookup

**Files to modify:**
- `QueryBuilder.cs` - Refactor `BuildOperatorExpression()`

**Expected improvement:** 5-15% reduction in operator dispatch overhead

---

### Priority 2: Medium Impact / Medium Effort

#### 2.1 String Comparison Optimization
**Impact:** 🔥🔥 | **Effort:** Low | **Risk:** Low

Eliminate `.ToLower()` allocations by using `StringComparison.OrdinalIgnoreCase`.

**Implementation:**
- Generate `String.Equals(a, b, StringComparison.OrdinalIgnoreCase)` expressions
- Replace `.ToLower()` call expressions with comparison method calls
- Update `GetExpressionsOperands()` method

**Files to modify:**
- `QueryBuilder.cs` - Modify string comparison expression generation

**Expected improvement:** Reduced allocations for string operations

---

#### 2.2 HashSet for Large "In" Operations
**Impact:** 🔥 | **Effort:** Medium | **Risk:** Low

For large value lists in `in` operator, generate `HashSet<T>.Contains()` instead of chained `Or`.

**Implementation:**
- Add threshold check (e.g., >10 values)
- Generate `new HashSet<T>(values).Contains(property)` expression
- Keep existing behavior for small lists (Or chain may be faster)

**Files to modify:**
- `QueryBuilder.cs` - Modify `In()` method

**Expected improvement:** O(1) vs O(n) for large value lists at runtime

---

#### 2.3 Compiled Predicate Caching
**Impact:** 🔥🔥 | **Effort:** Medium | **Risk:** Low

Cache compiled delegates alongside expressions.

**Implementation:**
- Extend expression cache to store `Func<T, bool>` delegates
- Add option for auto-compilation on cache miss
- Consider warm-up compilation for known patterns

**Files to modify:**
- `QueryBuilder.cs` - Extend caching layer

**Expected improvement:** Eliminate ~100μs+ compilation cost on repeated use

---

### Priority 3: Lower Impact / Higher Effort

#### 3.1 Span-Based String Parsing
**Impact:** 🔥 | **Effort:** Medium | **Risk:** Medium

Use `ReadOnlySpan<char>` for value parsing to reduce allocations.

**Implementation:**
- Replace `.Split()` with `MemoryExtensions.Split()` (net8+)
- Use span-based parsing for numeric types
- Requires conditional compilation for older TFMs

**Files to modify:**
- `QueryBuilder.cs` - Refactor `GetConstants()` with `#if NET8_0_OR_GREATER`

**Expected improvement:** Reduced allocations in value parsing

---

#### 3.2 Static Lambda Caching
**Impact:** 🔥 (minor) | **Effort:** Low | **Risk:** Low

Cache static lambdas to avoid delegate allocations.

**Implementation:**
- Replace `_ => true` with static field
- Identify other static lambdas that can be cached

**Files to modify:**
- `QueryBuilder.cs` - Add static fields for common lambdas

**Expected improvement:** Minor allocation reduction

---

#### 3.3 List Pooling
**Impact:** 🔥 (minor) | **Effort:** Low | **Risk:** Low

Use `ArrayPool<T>` or object pooling for frequently allocated lists.

**Implementation:**
- Pool `List<ConstantExpression>` in `GetConstants()`
- Pool intermediate collections in expression building

**Files to modify:**
- `QueryBuilder.cs` - Add pooling for hot path allocations

**Expected improvement:** Reduced GC pressure

---

### Priority 4: Strategic / Breaking Changes

#### 4.1 Drop Legacy TFMs (v2.0)
**Impact:** 🔥🔥 | **Effort:** Low (but breaking) | **Risk:** High (compatibility)

Create v2.0 targeting only `net8` and `net9` to unlock modern APIs.

**Implementation:**
- Create v2.0 branch
- Remove `net45`, `netstandard2.0`, `net6` targets
- Leverage `FrozenDictionary`, `SearchValues<T>`, native AOT

**Files to modify:**
- `Castle.DynamicLinqQueryBuilder.csproj` - Update TFMs
- All source files - Remove conditional compilation guards

**Expected improvement:** Access to all modern .NET performance features

---

## Implementation Details

### Recommended Implementation Order

```
Phase 1: Quick Wins (Est. 1-2 days)
├── 1.3 TypeConverter Caching
├── 1.4 Operator Dispatch Optimization
├── 3.2 Static Lambda Caching
└── 3.3 List Pooling (partial)

Phase 2: Core Caching (Est. 2-3 days)
├── 1.2 PropertyInfo/MethodInfo Caching
├── 1.1 Expression Caching (basic)
└── 2.3 Compiled Predicate Caching

Phase 3: String Optimizations (Est. 1-2 days)
├── 2.1 String Comparison Optimization
└── 3.1 Span-Based String Parsing (net8+ only)

Phase 4: Algorithm Improvements (Est. 1 day)
└── 2.2 HashSet for Large "In" Operations

Phase 5: v2.0 Planning (Future)
└── 4.1 Drop Legacy TFMs
```

### Code Patterns

#### Expression Cache Key Generation

```csharp
// Generate cache key from filter structure (without values)
private static string GenerateCacheKey<T>(IFilterRule rule)
{
    var sb = new StringBuilder();
    sb.Append(typeof(T).FullName);
    AppendRuleStructure(sb, rule);
    return sb.ToString();
}

private static void AppendRuleStructure(StringBuilder sb, IFilterRule rule)
{
    if (rule.Rules != null)
    {
        sb.Append($"({rule.Condition}:");
        foreach (var child in rule.Rules)
            AppendRuleStructure(sb, child);
        sb.Append(')');
    }
    else
    {
        sb.Append($"[{rule.Field}|{rule.Operator}|{rule.Type}]");
    }
}
```

#### Cached Reflection Helpers

```csharp
private static readonly ConcurrentDictionary<(Type, string), PropertyInfo?> _propertyCache = new();
private static readonly ConcurrentDictionary<Type, TypeConverter> _converterCache = new();

public static PropertyInfo? GetCachedProperty(Type type, string name)
    => _propertyCache.GetOrAdd((type, name), key => key.Item1.GetProperty(key.Item2));

public static TypeConverter GetCachedConverter(Type type)
    => _converterCache.GetOrAdd(type, TypeDescriptor.GetConverter);
```

#### StringComparison Expression Generation

```csharp
// Instead of:
// property.ToLower() == value.ToLower()

// Generate:
// String.Equals(property, value, StringComparison.OrdinalIgnoreCase)

private static readonly MethodInfo StringEqualsMethod = typeof(string).GetMethod(
    nameof(string.Equals), 
    new[] { typeof(string), typeof(string), typeof(StringComparison) })!;

var comparison = Expression.Constant(StringComparison.OrdinalIgnoreCase);
var equalsCall = Expression.Call(StringEqualsMethod, propertyExp, valueExp, comparison);
```

---

## Testing Protocol

### Pre-Implementation Checklist

- [ ] Run full test suite - all tests pass
- [ ] Run full benchmark suite - capture baseline metrics
- [ ] Document current memory allocation patterns
- [ ] Create git branch for optimization work

### Per-Optimization Testing

For each optimization:

1. **Unit Tests**
   - Run existing tests: `dotnet test Castle.DynamicLinqQueryBuilder.Tests`
   - Add new tests for caching behavior (if applicable)
   - Verify edge cases (null filters, empty rules, etc.)

2. **Benchmark Comparison**
   - Run relevant benchmark category
   - Compare against baseline
   - Document improvement percentage

3. **Regression Check**
   - Run full test suite
   - Run quick benchmark sanity check
   - Verify no functionality changes

### Post-Implementation Protocol

1. **Full Test Suite**
   ```bash
   dotnet test Castle.DynamicLinqQueryBuilder.Tests -c Release
   ```

2. **Full Benchmark Suite**
   ```bash
   cd Castle.DynamicLinqQueryBuilder.Benchmarks
   dotnet run -c Release -- all
   ```

3. **Memory Analysis**
   - Compare Gen0/Gen1/Gen2 collections
   - Compare total allocations
   - Identify any memory regressions

4. **Update Documentation**
   - Update this file with post-optimization metrics
   - Document any API changes
   - Update README if needed

### Benchmark Commands Reference

```bash
# Full suite (takes ~30-60 minutes)
dotnet run -c Release -- all

# Individual categories
dotnet run -c Release -- expression
dotnet run -c Release -- operator
dotnet run -c Release -- type
dotnet run -c Release -- complexity
dotnet run -c Release -- scale
dotnet run -c Release -- caching
dotnet run -c Release -- compilation

# Quick sanity check
dotnet run -c Release -- quick

# Filter specific benchmarks
dotnet run -c Release -- --filter *Caching*
dotnet run -c Release -- --filter *Simple*
```

---

## Post-Optimization Metrics

> To be populated after optimizations are implemented

### Phase 1 Results
```
| Optimization | Baseline | After | Improvement | Notes |
|--------------|----------|-------|-------------|-------|
| TBD |
```

### Phase 2 Results
```
| Optimization | Baseline | After | Improvement | Notes |
|--------------|----------|-------|-------------|-------|
| TBD |
```

### Final Comparison

```
| Benchmark Category | Baseline Mean | Optimized Mean | Improvement |
|--------------------|---------------|----------------|-------------|
| Expression Build   | TBD           | TBD            | TBD         |
| Operators          | TBD           | TBD            | TBD         |
| Type Conversion    | TBD           | TBD            | TBD         |
| Complexity         | TBD           | TBD            | TBD         |
| Scale              | TBD           | TBD            | TBD         |
| Caching ROI        | TBD           | TBD            | TBD         |
| Compilation        | TBD           | TBD            | TBD         |
```

---

## Appendix

### Environment Details

**Benchmark Machine:**
- OS: Windows 11 (10.0.26200.7623)
- CPU: Intel Core i9-14900KF, 1 CPU, 32 logical and 24 physical cores
- .NET SDK: 9.0.306
- .NET Runtime: .NET 8.0.21 (8.0.2125.47513), X64 RyuJIT AVX2
- Hardware Intrinsics: AVX2, AES, BMI1, BMI2, FMA, LZCNT, PCLMUL, POPCNT, AvxVnni, SERIALIZE (VectorSize=256)

**Benchmark Configuration:**
- BenchmarkDotNet v0.14.0
- Job: ShortRun (IterationCount=3, LaunchCount=1, WarmupCount=3)
- Memory Diagnoser enabled

### Related Files

- Benchmark project: `Castle.DynamicLinqQueryBuilder.Benchmarks/`
- Test project: `Castle.DynamicLinqQueryBuilder.Tests/`
- Main source: `Castle.DynamicLinqQueryBuilder/QueryBuilder.cs`
