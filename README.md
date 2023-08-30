# Dynamic Linq Query Builder
[![.NET](https://github.com/tghamm/dynamic-linq-query-builder/actions/workflows/dotnet.yml/badge.svg)](https://github.com/tghamm/dynamic-linq-query-builder/actions/workflows/dotnet.yml) [![Coverage Status](https://coveralls.io/repos/github/tghamm/dynamic-linq-query-builder/badge.svg?branch=master)](https://coveralls.io/github/tghamm/dynamic-linq-query-builder?branch=master) [![Nuget](https://img.shields.io/nuget/dt/Castle.DynamicLinqQueryBuilder)](https://www.nuget.org/packages/Castle.DynamicLinqQueryBuilder/)

`dynamic-linq-query-builder` is a small library that allows any `.Net` framework class collection to be filtered dynamically at runtime.  

Features (v1.3.3)
--
* Generates an `IQueryable` from any collection and filter combination
* Capable of complex, grouped queries against as many fields as you want
* Supports nested objects and collections via dot notation
* Supports ORMs like `EF6`, `EFCore`, and `MongoDB Client >=2.19`
* Supports a number of operators for each type
  * in
  * not in
  * equal
  * not equal
  * between
  * not between
  * less
  * less or equal
  * greater
  * greater or equal
  * begins with
  * not begins with
  * contains
  * not contains
  * ends with
  * not ends with
  * is empty
  * is not empty
  * is null
  * is not null
  * custom operators via interface and options
* Compatible with [jQuery QueryBuilder](https://querybuilder.js.org) (see samples for an example)

* Targets .NET 4.5, .NET Standard 2.0, and .NET 6

Installation
--
`dynamic-linq-query-builder` can be installed via the nuget UI (as Castle.DynamicLinqQueryBuilder), or via the nuget package manager console:
```
PM> Install-Package Castle.DynamicLinqQueryBuilder
```
To Install the System.Text.Json extension:
--
`dynamic-linq-query-builder-system-text-json` can be installed via the nuget UI (as Castle.DynamicLinqQueryBuilder.SystemTextJson), or via the nuget package manager console:
```
PM> Install-Package Castle.DynamicLinqQueryBuilder.SystemTextJson
```

Getting Started
--
The easiest way to get started is to install the NuGet package and take a look at the MVC sample application included in the source code.  It contains a working example of both `dynamic-linq-query-builder` and `jQuery-QueryBuilder`.

Additionally, see the [Wiki](https://github.com/tghamm/dynamic-linq-query-builder/wiki)

Contributions
--
Contributions and pull requests are welcome with associated unit tests.
