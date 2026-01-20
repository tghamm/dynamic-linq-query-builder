```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.7623)
Intel Core i9-14900KF, 1 CPU, 32 logical and 24 physical cores
.NET SDK 9.0.306
  [Host]   : .NET 8.0.21 (8.0.2125.47513), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.21 (8.0.2125.47513), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                               | Mean       | Error     | StdDev   | Ratio | RatioSD | Gen0    | Gen1    | Allocated | Alloc Ratio |
|------------------------------------- |-----------:|----------:|---------:|------:|--------:|--------:|--------:|----------:|------------:|
| Type_Integer                         |   194.4 μs |  71.77 μs |  3.93 μs |  1.00 |    0.02 |  0.4883 |       - |  14.72 KB |        1.00 |
| Type_Long                            |   202.9 μs |  44.23 μs |  2.42 μs |  1.04 |    0.02 |  0.4883 |       - |  14.63 KB |        0.99 |
| Type_Double                          |   213.6 μs |  82.30 μs |  4.51 μs |  1.10 |    0.03 |  0.4883 |       - |   14.6 KB |        0.99 |
| Type_String                          |   523.1 μs | 246.02 μs | 13.48 μs |  2.69 |    0.08 |  5.8594 |  2.9297 | 121.68 KB |        8.27 |
| Type_Date                            |   329.2 μs | 198.01 μs | 10.85 μs |  1.69 |    0.06 |  0.4883 |       - |  16.07 KB |        1.09 |
| Type_DateTime_Local                  |   333.9 μs | 227.94 μs | 12.49 μs |  1.72 |    0.06 |  0.4883 |       - |  15.71 KB |        1.07 |
| Type_DateTime_UTC                    |   328.5 μs | 137.89 μs |  7.56 μs |  1.69 |    0.04 |  0.4883 |       - |  15.74 KB |        1.07 |
| Type_Boolean                         |   221.3 μs |  33.68 μs |  1.85 μs |  1.14 |    0.02 |  1.4648 |  0.9766 |  30.89 KB |        2.10 |
| Type_Guid                            |   340.4 μs |  70.10 μs |  3.84 μs |  1.75 |    0.03 |  0.4883 |       - |  15.97 KB |        1.09 |
| Type_NullableInteger                 |   255.5 μs |  52.56 μs |  2.88 μs |  1.32 |    0.03 |  0.4883 |       - |  15.52 KB |        1.05 |
| Type_NullableLong                    |   255.4 μs | 106.32 μs |  5.83 μs |  1.31 |    0.03 |  0.4883 |       - |  15.52 KB |        1.05 |
| Type_NullableDouble                  |   253.3 μs | 117.04 μs |  6.42 μs |  1.30 |    0.04 |  0.4883 |       - |  15.51 KB |        1.05 |
| Type_NullableDateTime                |   430.8 μs | 838.34 μs | 45.95 μs |  2.22 |    0.21 |  0.9766 |       - |  18.11 KB |        1.23 |
| Type_NullableBoolean                 |   258.8 μs |  72.17 μs |  3.96 μs |  1.33 |    0.03 |  0.9766 |  0.4883 |  23.66 KB |        1.61 |
| Type_NullableGuid                    |   467.9 μs | 620.79 μs | 34.03 μs |  2.41 |    0.16 |  0.9766 |       - |  18.23 KB |        1.24 |
| TypeCollection_Integer_10Values      |   315.7 μs | 318.96 μs | 17.48 μs |  1.62 |    0.08 |  0.9766 |  0.4883 |  23.61 KB |        1.60 |
| TypeCollection_String_10Values       | 1,621.3 μs | 714.20 μs | 39.15 μs |  8.34 |    0.23 | 46.8750 | 11.7188 | 906.25 KB |       61.58 |
| TypeCollection_DateTime_10Values     |   574.3 μs | 347.26 μs | 19.03 μs |  2.96 |    0.10 |  0.9766 |       - |  30.75 KB |        2.09 |
| TypeCollection_DateTime_10Values_UTC |   628.3 μs | 281.93 μs | 15.45 μs |  3.23 |    0.09 |  0.9766 |       - |  30.72 KB |        2.09 |
| TypeCollection_Guid_10Values         |   745.5 μs | 468.33 μs | 25.67 μs |  3.84 |    0.13 |  0.9766 |       - |  31.42 KB |        2.13 |
