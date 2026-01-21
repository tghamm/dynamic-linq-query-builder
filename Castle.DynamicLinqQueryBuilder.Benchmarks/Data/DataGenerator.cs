using Castle.DynamicLinqQueryBuilder.Benchmarks.Models;

namespace Castle.DynamicLinqQueryBuilder.Benchmarks.Data;

/// <summary>
/// Generates test datasets of varying sizes for benchmarks.
/// </summary>
public static class DataGenerator
{
    private static readonly string[] FirstNames = 
    {
        "Emma", "Liam", "Olivia", "Noah", "Ava", "Oliver", "Isabella", "Elijah",
        "Sophia", "Lucas", "Mia", "Mason", "Charlotte", "Logan", "Amelia", "Alexander",
        "Harper", "Ethan", "Evelyn", "Jacob", "Abigail", "Michael", "Emily", "Daniel"
    };
    
    private static readonly string[] LastNames = 
    {
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis",
        "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson",
        "Thomas", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Thompson", "White", "Harris"
    };
    
    private static readonly string[] Cities = 
    {
        "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia",
        "San Antonio", "San Diego", "Dallas", "San Jose", "Austin", "Jacksonville",
        "Fort Worth", "Columbus", "Charlotte", "Seattle", "Denver", "Boston", "Nashville", "Portland"
    };
    
    private static readonly string[] States = 
    {
        "NY", "CA", "IL", "TX", "AZ", "PA", "FL", "OH", "NC", "WA", "CO", "MA", "TN", "OR",
        "GA", "MI", "NJ", "VA", "WI", "MN"
    };
    
    private static readonly string[] ContentTypeNames =
    {
        "Multiple-Choice", "Multiple-Select", "Drag-and-Drop Item", "Essay", "Fill-in-the-blank",
        "True/False", "Matching", "Short Answer", "Numeric Response", "Ordering"
    };

    private static readonly string[] LongerTexts =
    {
        "There is something interesting about this text",
        "The quick brown fox jumps over the lazy dog",
        "Lorem ipsum dolor sit amet consectetur adipiscing elit",
        "Performance testing reveals optimization opportunities",
        "Expression trees enable dynamic query generation",
        null!
    };

    /// <summary>
    /// Generates a list of BenchmarkEntity objects for testing.
    /// </summary>
    public static List<BenchmarkEntity> GenerateBenchmarkEntities(int count, int? seed = null)
    {
        var random = seed.HasValue ? new Random(seed.Value) : new Random(42);
        var result = new List<BenchmarkEntity>(count);

        for (int i = 0; i < count; i++)
        {
            var entity = new BenchmarkEntity
            {
                ContentTypeId = random.Next(1, 100),
                NullableContentTypeId = random.Next(0, 10) > 2 ? random.Next(1, 100) : null,
                ContentTypeLong = random.NextInt64(1, 1000000),
                NullableContentTypeLong = random.Next(0, 10) > 2 ? random.NextInt64(1, 1000000) : null,
                ContentTypeGuid = Guid.NewGuid(),
                NullableContentTypeGuid = random.Next(0, 10) > 3 ? Guid.NewGuid() : null,
                ContentTypeName = ContentTypeNames[random.Next(ContentTypeNames.Length)],
                LongerTextToFilter = LongerTexts[random.Next(LongerTexts.Length)],
                IsSelected = random.Next(0, 2) == 1,
                IsPossiblyNotSetBool = random.Next(0, 10) > 3 ? random.Next(0, 2) == 1 : null,
                LastModified = DateTime.UtcNow.AddDays(-random.Next(0, 365)),
                LastModifiedIfPresent = random.Next(0, 10) > 2 ? DateTime.UtcNow.AddDays(-random.Next(0, 365)) : null,
                NullableDateTime = random.Next(0, 10) > 5 ? DateTime.UtcNow.AddDays(-random.Next(0, 365)) : null,
                StatValue = random.NextDouble() * 100,
                PossiblyEmptyStatValue = random.Next(0, 10) > 3 ? random.NextDouble() * 100 : null,
                IntList = Enumerable.Range(0, random.Next(1, 10)).Select(_ => random.Next(1, 100)).ToList(),
                NullableIntList = Enumerable.Range(0, random.Next(1, 10))
                    .Select(_ => random.Next(0, 10) > 2 ? random.Next(1, 100) : (int?)null).ToList(),
                LongList = Enumerable.Range(0, random.Next(1, 5)).Select(_ => random.NextInt64(1, 1000)).ToList(),
                NullableLongList = Enumerable.Range(0, random.Next(1, 5))
                    .Select(_ => random.Next(0, 10) > 2 ? random.NextInt64(1, 1000) : (long?)null).ToList(),
                DateList = Enumerable.Range(0, random.Next(1, 5))
                    .Select(_ => DateTime.UtcNow.AddDays(-random.Next(0, 365))).ToList(),
                DoubleList = Enumerable.Range(0, random.Next(1, 5)).Select(_ => random.NextDouble() * 100).ToList(),
                StrList = Enumerable.Range(0, random.Next(1, 5))
                    .Select(_ => $"Str{random.Next(1, 100)}").ToList(),
                ChildClasses = GenerateChildClasses(random, random.Next(0, 4)),
                Dictionary = new Dictionary<string, object>
                {
                    ["first_name"] = FirstNames[random.Next(FirstNames.Length)],
                    ["last_name"] = LastNames[random.Next(LastNames.Length)],
                    ["score"] = random.Next(0, 100)
                }
            };
            
            // Ensure some entities have null Enemies list for null testing
            if (random.Next(0, 10) > 7)
            {
                entity.Enemies = null!;
            }
            else
            {
                entity.Enemies = Enumerable.Range(0, random.Next(0, 5))
                    .Select(_ => random.Next(1000, 9999)).ToList();
            }

            result.Add(entity);
        }

        return result;
    }

    private static List<ChildClass> GenerateChildClasses(Random random, int count)
    {
        var result = new List<ChildClass>(count);
        for (int i = 0; i < count; i++)
        {
            result.Add(new ChildClass
            {
                ClassName = $"ChildClass_{random.Next(1, 100)}",
                ChildSubClass = random.Next(0, 10) > 3 ? new ChildSubClass
                {
                    ClassName = $"SubClass_{random.Next(1, 100)}"
                } : null
            });
        }
        return result;
    }

    /// <summary>
    /// Generates a list of PersonRecord objects for testing.
    /// </summary>
    public static List<PersonRecord> GeneratePersonRecords(int count, int? seed = null)
    {
        var random = seed.HasValue ? new Random(seed.Value) : new Random(42);
        var result = new List<PersonRecord>(count);

        for (int i = 0; i < count; i++)
        {
            result.Add(new PersonRecord
            {
                FirstName = FirstNames[random.Next(FirstNames.Length)],
                LastName = LastNames[random.Next(LastNames.Length)],
                Birthday = DateTime.UtcNow.AddYears(-random.Next(18, 80)).AddDays(-random.Next(0, 365)),
                Address = $"{random.Next(1, 9999)} {LastNames[random.Next(LastNames.Length)]} Street",
                City = Cities[random.Next(Cities.Length)],
                State = States[random.Next(States.Length)],
                ZipCode = random.Next(10000, 99999).ToString(),
                Deceased = random.Next(0, 10) < 2,
                Age = random.Next(18, 80),
                Salary = random.NextDouble() * 150000 + 30000,
                PersonId = Guid.NewGuid()
            });
        }

        return result;
    }

    /// <summary>
    /// Gets a sample value for testing a specific type.
    /// </summary>
    public static object GetSampleValue(string type, Random? random = null)
    {
        random ??= new Random(42);
        return type switch
        {
            "integer" => random.Next(1, 100),
            "long" => random.NextInt64(1, 100000),
            "double" => Math.Round(random.NextDouble() * 100, 2),
            "string" => "Test String Value",
            "date" => DateTime.UtcNow.Date.ToString("yyyy-MM-dd"),
            "datetime" => DateTime.UtcNow.ToString("o"),
            "boolean" => true,
            "guid" => Guid.NewGuid().ToString(),
            _ => "default"
        };
    }

    /// <summary>
    /// Gets multiple sample values for 'in' operator testing.
    /// </summary>
    public static string[] GetSampleValues(string type, int count, Random? random = null)
    {
        random ??= new Random(42);
        var values = new string[count];
        for (int i = 0; i < count; i++)
        {
            values[i] = GetSampleValue(type, random).ToString()!;
        }
        return values;
    }
}
