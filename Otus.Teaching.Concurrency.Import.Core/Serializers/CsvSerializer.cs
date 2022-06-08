using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Otus.Teaching.Concurrency.Import.Core.Serializers
{
    public class CsvSerializer
    {
        const string quotes = "&nbsp;";
        const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;

        public static string Serialize<T>(T obj)
        {
            StringBuilder sbSerializedString = new StringBuilder();

            FieldInfo[] fields = obj.GetType().GetFields(bindingAttr: bindingFlags);
            foreach (FieldInfo field in fields)
            {
                string name = field.Name;
                string value = field.GetValue(obj).ToString().Replace("\"", quotes);

                sbSerializedString.Append($",\"{name}\":\"{value}\"");
            }

            return sbSerializedString.ToString().TrimStart(',');
        }

        public static T Deserialize<T>(string csv)
        {
            var obj = Activator.CreateInstance<T>();

            string[] fields = csv.Split("\",\"", StringSplitOptions.RemoveEmptyEntries);
            if (fields.Length == 0) return obj;

            for (int i = 1; i < fields.Length; i++)
            {
                string[] parts = fields[i].Split("\":\"", StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2) continue;

                string fieldName = parts[0];
                string fieldValue = parts[1].Replace("\"", "").Replace(quotes, "\"");
                FieldInfo field = obj.GetType().GetField(name: fieldName, bindingAttr: bindingFlags);

                var value = Convert.ChangeType(fieldValue, field.FieldType);
                field.SetValue(obj: obj, value: value);
            }

            return (T)obj;

        }
    }
}
