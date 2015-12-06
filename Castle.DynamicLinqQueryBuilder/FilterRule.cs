using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castle.DynamicLinqQueryBuilder
{
    [ExcludeFromCodeCoverage]
    public class FilterRule
    {
        public string Condition { get; set; }
        public string Field { get; set; }
        public string Id { get; set; }
        public string Input { get; set; }
        public string Operator { get; set; }
        public List<FilterRule> Rules { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
