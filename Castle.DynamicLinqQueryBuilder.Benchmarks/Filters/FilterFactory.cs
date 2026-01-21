namespace Castle.DynamicLinqQueryBuilder.Benchmarks.Filters;

/// <summary>
/// Factory for creating filter rules used in benchmarks.
/// Provides reusable methods for all operator/type combinations.
/// </summary>
public static class FilterFactory
{
    /// <summary>
    /// All supported operators.
    /// </summary>
    public static readonly string[] AllOperators =
    {
        "equal", "not_equal",
        "in", "not_in",
        "less", "less_or_equal", "greater", "greater_or_equal",
        "between", "not_between",
        "begins_with", "not_begins_with",
        "contains", "not_contains",
        "ends_with", "not_ends_with",
        "is_null", "is_not_null",
        "is_empty", "is_not_empty"
    };

    /// <summary>
    /// All supported types.
    /// </summary>
    public static readonly string[] AllTypes =
    {
        "integer", "long", "double", "string", "date", "datetime", "boolean", "guid"
    };

    /// <summary>
    /// Creates a simple single-rule filter.
    /// </summary>
    public static QueryBuilderFilterRule CreateSingleRule(
        string op, 
        string type, 
        string field, 
        params string[] values)
    {
        return new QueryBuilderFilterRule
        {
            Condition = "and",
            Rules = new List<QueryBuilderFilterRule>
            {
                new QueryBuilderFilterRule
                {
                    Condition = "and",
                    Field = field,
                    Id = field,
                    Input = "NA",
                    Operator = op,
                    Type = type,
                    Value = values.Length > 0 ? values : new[] { GetDefaultValue(type) }
                }
            }
        };
    }

    /// <summary>
    /// Creates a filter with multiple flat rules (all ANDed together).
    /// </summary>
    public static QueryBuilderFilterRule CreateFlatFilter(int ruleCount, string op = "equal", string type = "integer")
    {
        var rules = new List<QueryBuilderFilterRule>();
        for (int i = 0; i < ruleCount; i++)
        {
            rules.Add(new QueryBuilderFilterRule
            {
                Condition = "and",
                Field = "ContentTypeId",
                Id = $"ContentTypeId_{i}",
                Input = "NA",
                Operator = op,
                Type = type,
                Value = new[] { (i + 1).ToString() }
            });
        }

        return new QueryBuilderFilterRule
        {
            Condition = "and",
            Rules = rules
        };
    }

    /// <summary>
    /// Creates a filter with multiple flat rules using OR condition.
    /// </summary>
    public static QueryBuilderFilterRule CreateFlatOrFilter(int ruleCount, string op = "equal", string type = "integer")
    {
        var rules = new List<QueryBuilderFilterRule>();
        for (int i = 0; i < ruleCount; i++)
        {
            rules.Add(new QueryBuilderFilterRule
            {
                Condition = "or",
                Field = "ContentTypeId",
                Id = $"ContentTypeId_{i}",
                Input = "NA",
                Operator = op,
                Type = type,
                Value = new[] { (i + 1).ToString() }
            });
        }

        return new QueryBuilderFilterRule
        {
            Condition = "or",
            Rules = rules
        };
    }

    /// <summary>
    /// Creates a nested filter with specified depth.
    /// </summary>
    public static QueryBuilderFilterRule CreateNestedFilter(int depth, string op = "equal", string type = "integer")
    {
        if (depth <= 1)
        {
            return CreateSingleRule(op, type, "ContentTypeId", "1");
        }

        return new QueryBuilderFilterRule
        {
            Condition = "and",
            Rules = new List<QueryBuilderFilterRule>
            {
                CreateNestedFilter(depth - 1, op, type),
                new QueryBuilderFilterRule
                {
                    Condition = "and",
                    Field = "ContentTypeId",
                    Id = $"ContentTypeId_depth_{depth}",
                    Input = "NA",
                    Operator = op,
                    Type = type,
                    Value = new[] { depth.ToString() }
                }
            }
        };
    }

    /// <summary>
    /// Creates a mixed AND/OR nested filter.
    /// </summary>
    public static QueryBuilderFilterRule CreateMixedFilter(int depth, int breadth)
    {
        var rules = new List<QueryBuilderFilterRule>();
        
        for (int i = 0; i < breadth; i++)
        {
            if (depth > 1)
            {
                rules.Add(new QueryBuilderFilterRule
                {
                    Condition = i % 2 == 0 ? "and" : "or",
                    Rules = new List<QueryBuilderFilterRule>
                    {
                        CreateSingleRule("equal", "integer", "ContentTypeId", (i + 1).ToString()),
                        CreateSingleRule("contains", "string", "ContentTypeName", "Choice")
                    }
                });
            }
            else
            {
                rules.Add(CreateSingleRule("equal", "integer", "ContentTypeId", (i + 1).ToString()));
            }
        }

        return new QueryBuilderFilterRule
        {
            Condition = "and",
            Rules = rules
        };
    }

    /// <summary>
    /// Creates a filter for nested property access (e.g., ChildClasses.ClassName).
    /// </summary>
    public static QueryBuilderFilterRule CreateNestedPropertyFilter(string nestedPath, string op, string type, params string[] values)
    {
        return new QueryBuilderFilterRule
        {
            Condition = "and",
            Rules = new List<QueryBuilderFilterRule>
            {
                new QueryBuilderFilterRule
                {
                    Condition = "and",
                    Field = nestedPath,
                    Id = nestedPath.Replace(".", "_"),
                    Input = "NA",
                    Operator = op,
                    Type = type,
                    Value = values.Length > 0 ? values : new[] { GetDefaultValue(type) }
                }
            }
        };
    }

    /// <summary>
    /// Creates a filter for 'in' operator with a specified number of values.
    /// </summary>
    public static QueryBuilderFilterRule CreateInFilter(string field, string type, int valueCount)
    {
        var values = new string[valueCount];
        for (int i = 0; i < valueCount; i++)
        {
            values[i] = type switch
            {
                "integer" => (i + 1).ToString(),
                "long" => ((long)(i + 1) * 1000).ToString(),
                "double" => (i + 0.5).ToString(),
                "string" => $"Value{i + 1}",
                "guid" => Guid.NewGuid().ToString(),
                _ => i.ToString()
            };
        }

        return CreateSingleRule("in", type, field, values);
    }

    /// <summary>
    /// Creates a filter for between operator.
    /// </summary>
    public static QueryBuilderFilterRule CreateBetweenFilter(string field, string type, string low, string high)
    {
        return new QueryBuilderFilterRule
        {
            Condition = "and",
            Rules = new List<QueryBuilderFilterRule>
            {
                new QueryBuilderFilterRule
                {
                    Condition = "and",
                    Field = field,
                    Id = field,
                    Input = "NA",
                    Operator = "between",
                    Type = type,
                    Value = new[] { low, high }
                }
            }
        };
    }

    /// <summary>
    /// Creates string operator filters with case sensitivity option.
    /// </summary>
    public static (QueryBuilderFilterRule Filter, BuildExpressionOptions Options) CreateStringOperatorFilter(
        string op, 
        string field, 
        string value, 
        bool caseSensitive)
    {
        var filter = CreateSingleRule(op, "string", field, value);
        var options = new BuildExpressionOptions
        {
            StringCaseSensitiveComparison = caseSensitive
        };
        return (filter, options);
    }

    /// <summary>
    /// Creates datetime filter with UTC parsing option.
    /// </summary>
    public static (QueryBuilderFilterRule Filter, BuildExpressionOptions Options) CreateDateTimeFilter(
        string op,
        string field,
        string value,
        bool parseAsUtc)
    {
        var filter = CreateSingleRule(op, "datetime", field, value);
        var options = new BuildExpressionOptions
        {
            ParseDatesAsUtc = parseAsUtc
        };
        return (filter, options);
    }

    /// <summary>
    /// Gets a default value string for a given type.
    /// </summary>
    private static string GetDefaultValue(string type)
    {
        return type switch
        {
            "integer" => "1",
            "long" => "1000",
            "double" => "1.5",
            "string" => "test",
            "date" => DateTime.UtcNow.Date.ToString("yyyy-MM-dd"),
            "datetime" => DateTime.UtcNow.ToString("o"),
            "boolean" => "true",
            "guid" => Guid.Empty.ToString(),
            _ => "default"
        };
    }

    /// <summary>
    /// Gets the appropriate field name for a given type.
    /// </summary>
    public static string GetFieldForType(string type)
    {
        return type switch
        {
            "integer" => "ContentTypeId",
            "long" => "ContentTypeLong",
            "double" => "StatValue",
            "string" => "ContentTypeName",
            "date" => "LastModified",
            "datetime" => "LastModified",
            "boolean" => "IsSelected",
            "guid" => "ContentTypeGuid",
            _ => "ContentTypeId"
        };
    }

    /// <summary>
    /// Gets the appropriate nullable field name for a given type.
    /// </summary>
    public static string GetNullableFieldForType(string type)
    {
        return type switch
        {
            "integer" => "NullableContentTypeId",
            "long" => "NullableContentTypeLong",
            "double" => "PossiblyEmptyStatValue",
            "string" => "LongerTextToFilter",
            "date" => "LastModifiedIfPresent",
            "datetime" => "NullableDateTime",
            "boolean" => "IsPossiblyNotSetBool",
            "guid" => "NullableContentTypeGuid",
            _ => "NullableContentTypeId"
        };
    }
}
