namespace Castle.DynamicLinqQueryBuilder.Benchmarks.Models;

/// <summary>
/// Primary benchmark entity modeled after ExpressionTreeBuilderTestClass from the test suite.
/// Covers all supported types and edge cases.
/// </summary>
public class BenchmarkEntity
{
    public int ContentTypeId { get; set; }
    public int? NullableContentTypeId { get; set; }
    public long ContentTypeLong { get; set; }
    public long? NullableContentTypeLong { get; set; }
    public Guid ContentTypeGuid { get; set; }
    public Guid? NullableContentTypeGuid { get; set; }
    public List<int> Enemies { get; set; } = new();
    public List<string> Flags { get; set; } = new();
    public string? ContentTypeName { get; set; }
    public string? LongerTextToFilter { get; set; }
    public bool IsSelected { get; set; }
    public bool? IsPossiblyNotSetBool { get; set; }
    public DateTime LastModified { get; set; }
    public DateTime? LastModifiedIfPresent { get; set; }
    public DateTime? NullableDateTime { get; set; }
    public double StatValue { get; set; }
    public double? PossiblyEmptyStatValue { get; set; }
    public List<int> IntList { get; set; } = new();
    public List<int?> NullableIntList { get; set; } = new();
    public List<long> LongList { get; set; } = new();
    public List<long?> NullableLongList { get; set; } = new();
    public List<DateTime> DateList { get; set; } = new();
    public List<double> DoubleList { get; set; } = new();
    public List<string> StrList { get; set; } = new();
    public List<ChildClass> ChildClasses { get; set; } = new();
    public Dictionary<string, object> Dictionary { get; set; } = new();
}

public class ChildClass
{
    public string? ClassName { get; set; }
    public ChildSubClass? ChildSubClass { get; set; }
}

public class ChildSubClass
{
    public string? ClassName { get; set; }
}
