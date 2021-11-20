using System;
using System.Collections.Generic;


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
        /// Gets or sets the type. Supported values are "integer", "double", "string", "date", "datetime", and "boolean".
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
                        IEnumerable<JsonElement> array = jsonValue.EnumerateArray();

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
                    catch (Exception ex)
                    {

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
            switch (this.Type)
            {
                case "integer":
                    return Int32.Parse(element.GetString());
                    break;
                case "double":
                    return element.GetDouble();
                    break;
                case "string":
                    return element.ToString();
                    break;
                case "date":
                case "datetime":
                    return DateTime.Parse(element.GetString());
                    break;
                case "boolean":
                    return element.GetBoolean();
                    break;
                default:
                    throw new Exception($"Unexpected data type {this.Type}");
            }
        }
    }
}
