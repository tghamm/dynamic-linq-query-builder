using System;
using System.Collections.Generic;
using System.Globalization;


namespace Castle.DynamicLinqQueryBuilder.SystemTextJson
{

    using System.Text.Json;
    public class SystemTextJsonFilterRule : IFilterRule
    {
        /// <summary>
        /// Condition - acceptable values are "and" and "or".
        /// </summary>
        /// <value>
        /// The condition.
        /// </value>
        public string Condition { get; set; }
        /// <summary>
        /// The name of the field that the filter applies to.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        public string Field { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the input.
        /// </summary>
        /// <value>
        /// The input.
        /// </value>
        public string Input { get; set; }
        /// <summary>
        /// Gets or sets the operator.
        /// </summary>
        /// <value>
        /// The operator.
        /// </value>
        public string Operator { get; set; }
        /// <summary>
        /// Gets or sets nested filter rules.
        /// </summary>
        /// <value>
        /// The rules.
        /// </value>
        public List<SystemTextJsonFilterRule> Rules { get; set; }
        /// <summary>
        /// Gets or sets the type. Supported values are "integer", "double", "string", "date", "datetime", "guid", and "boolean".
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }

        private Object _Value { get; set; }
        /// <summary>
        /// Gets or sets the value of the filter.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        /// 
        public object Value
        {
            get
            {
                // See if this is a JsonElement
                if (_Value is JsonElement jsonValue)
                {
                    System.Type myType = QueryBuilder.GetCSharpType(Type);
                           
                    try
                    {
                        if (jsonValue.ValueKind == JsonValueKind.Array)
                        {
                            if (myType.Name == "DateOnly")
                                myType = typeof(DateTime);
                            var outList = Array.CreateInstance(myType, jsonValue.GetArrayLength());
                            var enumerator = 0;

                            foreach (var item in jsonValue.EnumerateArray())
                            {
                                object typedItem = GetJsonElementAsType(item);
                                outList.SetValue(typedItem, enumerator);
                                enumerator++;
                            }

                            return outList;
                        }
                        else
                        {
                            return GetJsonElementAsType(jsonValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidCastException("Error converting JsonElement to .Net Type", ex);
                    }
                }

                return _Value;
            }

            set 
            {
                _Value = value;
            }
        }
        

        IEnumerable<IFilterRule> IFilterRule.Rules => Rules;

        private object GetJsonElementAsType(JsonElement element)
        {
            Object o = null;
            switch (this.Type)
            {
                case "integer":
                    o = element.ValueKind == JsonValueKind.Number
                        ? element.GetInt32()
                        : Int32.Parse(element.GetString());
                    break;
                case "double":
                    o = element.ValueKind == JsonValueKind.Number
                        ? element.GetDouble()
                        : double.Parse(element.ToString());
                    break;
                case "string":
                    o = element.ToString();
                    break;
                case "date":
                case "datetime":
                    o = DateTime.Parse(element.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                    break;
                case "boolean":
                    o = ((element.ValueKind == JsonValueKind.True) || (element.ValueKind == JsonValueKind.False))
                        ? element.GetBoolean()
                        : bool.Parse(element.ToString());
                    break;
                case "guid":
                    o = element.GetGuid();
                    break;
            }

            return o;
        }
    }
}
