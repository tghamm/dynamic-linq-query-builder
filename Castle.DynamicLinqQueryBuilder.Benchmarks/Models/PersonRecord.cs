namespace Castle.DynamicLinqQueryBuilder.Benchmarks.Models;

/// <summary>
/// Secondary benchmark entity for simpler flat record benchmarks.
/// </summary>
public class PersonRecord
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime Birthday { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public bool Deceased { get; set; }
    public int Age { get; set; }
    public double Salary { get; set; }
    public Guid PersonId { get; set; }
}
