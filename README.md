# Dynamic Linq Query Builder
[![.NET](https://github.com/tghamm/dynamic-linq-query-builder/actions/workflows/dotnet.yml/badge.svg)](https://github.com/tghamm/dynamic-linq-query-builder/actions/workflows/dotnet.yml) [![Coverage Status](https://coveralls.io/repos/github/tghamm/dynamic-linq-query-builder/badge.svg?branch=master)](https://coveralls.io/github/tghamm/dynamic-linq-query-builder?branch=master) [![Nuget](https://img.shields.io/nuget/dt/Castle.DynamicLinqQueryBuilder)](https://www.nuget.org/packages/Castle.DynamicLinqQueryBuilder/)

`dynamic-linq-query-builder` is a small library that allows any .NET collection to be filtered dynamically at runtime.

## What's New in v2.0

Version 2.0 is a performance-focused release that targets **.NET 8+** exclusively. By dropping support for legacy frameworks (.NET 4.5, .NET Standard 2.0, .NET 6), we were able to leverage modern .NET APIs that simply weren't available before:

- **`FrozenDictionary`** for faster operator dispatch
- **Native `System.DateOnly`** instead of a custom workaround
- **Modern caching primitives** for expression and reflection caching

The result is a faster, leaner library with new opt-in features for scenarios where performance really matters.

> **Still on an older framework?** Version 1.3.4 remains available on NuGet and continues to support .NET 4.5, .NET Standard 2.0, .NET 6, and .NET 8.

## Features

- Generates an `IQueryable` from any collection and filter combination
- Capable of complex, grouped queries against as many fields as you want
- Supports nested objects and collections via dot notation
- Supports ORMs like `EF6`, `EF Core`, and `MongoDB Client >=2.19`
- Compatible with [jQuery QueryBuilder](https://querybuilder.js.org) (see samples for an example)
- Supports a number of operators for each type:
  - in
  - not in
  - equal
  - not equal
  - between
  - not between
  - less
  - less or equal
  - greater
  - greater or equal
  - begins with
  - not begins with
  - contains
  - not contains
  - ends with
  - not ends with
  - is empty
  - is not empty
  - is null
  - is not null
  - custom operators via `IFilterOperator` interface and options

### New in v2.0

- **Expression & predicate caching** — opt-in caching for repeated queries (up to 150x faster)
- **Optimized string comparison** — opt-in `StringComparison.OrdinalIgnoreCase` mode for in-memory scenarios
- **HashSet optimization** — automatic O(1) lookups for large `in` operations
- **Reduced allocations** — internal caching of reflection lookups and static lambdas

## Performance at a Glance

| Scenario | v1.3.4 | v2.0 | Improvement |
|----------|-------:|-----:|-------------|
| Large `in` (500 values) | 3,525 μs | 421 μs | **88% faster** |
| Repeated queries (cached) | 210 μs | 1.4 μs | **150x faster** |
| String filter (ordinal mode) | 490 μs | 309 μs | **37% faster**, 85% less memory |


## Quick Start: New v2.0 Options

All new features are **opt-in** — your existing code works without changes.

### Expression Caching

If you're running the same filter structure repeatedly (e.g., in a loop or API endpoint), enable caching to skip redundant expression building:

```csharp
var options = new BuildExpressionOptions 
{ 
    EnableExpressionCaching = true,  // Cache built expression trees
    EnablePredicateCaching = true    // Cache compiled delegates
};

// First call builds and caches; subsequent calls are ~150x faster
var results = myCollection.BuildQuery(filter, options).ToList();
```

### Optimized String Comparison

For **in-memory** scenarios (not EF Core or other ORMs), enable ordinal string comparison to avoid `ToLower()` allocations:

```csharp
var options = new BuildExpressionOptions 
{ 
    UseOrdinalStringComparison = true  // Uses StringComparison.OrdinalIgnoreCase
};

var results = myCollection.BuildQuery(filter, options).ToList();
```

> **Note:** Keep this `false` (default) when using with EF Core, EF6, or other ORMs that translate expressions to SQL. The `ToLower()` approach is required for database compatibility.

## Installation

Install via NuGet UI or Package Manager Console:

```
PM> Install-Package Castle.DynamicLinqQueryBuilder
```

For System.Text.Json support:

```
PM> Install-Package Castle.DynamicLinqQueryBuilder.SystemTextJson
```

**Targets:** .NET 8+

## Upgrading from v1.x

Migrating to v2.0 is straightforward:

1. **Update your target framework** to `net8.0` or later
2. **Update the NuGet package** to v2.0
3. **That's it!** All new options are opt-in; existing code works unchanged

If you can't upgrade to .NET 8, stay on v1.3.4 — it's still fully functional and available on NuGet.

## Getting Started

The easiest way to get started is to install the NuGet package and take a look at the MVC sample application included in the source code. It contains a working example of both `dynamic-linq-query-builder` and `jQuery-QueryBuilder`.

For more details, see the [Wiki](https://github.com/tghamm/dynamic-linq-query-builder/wiki).

## Contributions

Contributions and pull requests are welcome with associated unit tests.
