using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Castle.DynamicLinqQueryBuilder
{
    /// <summary>
    /// Class to assist with building column definitions for the jQuery Query Builder
    /// </summary>
    public static class ColumnBuilder
    {
        /// <summary>
        /// Gets the default column definitions for a given type.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="camelCase">if set to <c>true</c> [camel case].</param>
        /// <returns></returns>
        public static List<ColumnDefinition> GetDefaultColumnDefinitionsForType(this Type dataType, bool camelCase = false)
        {
            List<ColumnDefinition> itemBankColumnDefinitions = new List<ColumnDefinition>();

            var id = 1;
            foreach (var prop in dataType.GetProperties())
            {
                if (prop.GetCustomAttribute(typeof (IgnoreDataMemberAttribute)) != null) continue;

                var name = camelCase ? prop.Name.ToCamelCase() : prop.Name;

                var title = prop.Name.ToFriendlySpacedString();

                var type = string.Empty;

                if ((prop.PropertyType == typeof (double)) || (prop.PropertyType == typeof (double?)))
                {
                    type = "double";
                }
                else if ((prop.PropertyType == typeof (int)) || (prop.PropertyType == typeof (int?)))
                {
                    type = "integer";
                }
                else if ((prop.PropertyType == typeof(DateTime)) || (prop.PropertyType == typeof(DateTime?)))
                {
                    type = "date";
                }
                else if ((prop.PropertyType == typeof(string)))
                {
                    type = "string";
                }

                switch (type)
                {
                    case "double":
                        itemBankColumnDefinitions.Add(new ColumnDefinition()
                        {
                            Label = title,
                            Field = name,
                            Type = type,
                            Id = id.ToString()
                        });
                        break;
                    case "integer":
                        itemBankColumnDefinitions.Add(new ColumnDefinition()
                        {
                            Label = title,
                            Field = name,
                            Type = type,
                            Id = id.ToString()
                        });
                        break;
                    case "string":
                        itemBankColumnDefinitions.Add(new ColumnDefinition()
                        {
                            Label = title,
                            Field = name,
                            Type = type,
                            Id = id.ToString()
                        });
                        break;
                    case "date":
                        itemBankColumnDefinitions.Add(new ColumnDefinition()
                        {
                            Label = title,
                            Field = name,
                            Type = type,
                            Id = id.ToString()
                        });
                        break;
                }

                id++;
            }
            return itemBankColumnDefinitions;
        }

        /// <summary>
        /// Camel cases a string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string ToCamelCase(this string input)
        {
            var s = input;
            if (!char.IsUpper(s[0])) return s;

            var cArr = s.ToCharArray();
            for (var i = 0; i < cArr.Length; i++)
            {
                if (i > 0 && i + 1 < cArr.Length && !char.IsUpper(cArr[i + 1])) break;
                cArr[i] = char.ToLowerInvariant(cArr[i]);
            }
            return new string(cArr);
        }

        /// <summary>
        /// Pretty-prints a property name.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string ToFriendlySpacedString(this string input)
        {
            var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);


            var res = r.Replace(input, " ");

            return res;
        }
    }
}
