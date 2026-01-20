```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.7623)
Intel Core i9-14900KF, 1 CPU, 32 logical and 24 physical cores
.NET SDK 9.0.306
  [Host]   : .NET 8.0.21 (8.0.2125.47513), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.21 (8.0.2125.47513), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                       | Mean     | Error    | StdDev  | Gen0   | Gen1   | Allocated |
|----------------------------- |---------:|---------:|--------:|-------:|-------:|----------:|
| Equal_String_CaseInsensitive | 494.2 μs | 31.13 μs | 1.71 μs | 5.8594 | 2.9297 | 121.68 KB |
| Equal_String_CaseSensitive   | 302.1 μs | 27.53 μs | 1.51 μs | 0.9766 |      - |  17.98 KB |
