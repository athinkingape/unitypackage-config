using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Configs
{
    public class CsvSerializer
    {
        public static IEnumerable<T> Deserialize<T>(string resourceUrl)
            where T : new()
        {
            TextAsset dataset = Resources.Load<TextAsset>(resourceUrl);
            if (dataset == null)
            {
                Debug.LogError($"GameConfig dataset not found {resourceUrl}");
                return null;
            }

            var data = new List<T>();
            var type = typeof(T);
            var lines = dataset.text.Split("\n");
            var headers = lines[0].Split(',');
            var fields = new Dictionary<string, FieldInfo>();

            foreach (var field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                fields[field.Name] = field;
            }

            for (var lineIndex = 1; lineIndex < lines.Length; lineIndex++)
            {
                var rows = lines[lineIndex].Split(',');
                var entry = new T();

                for (var rowIndex = 0; rowIndex < rows.Length; rowIndex++)
                {
                    var field = fields["_" + headers[rowIndex].Trim()];
                    var dataType = field.FieldType.Name;

                    switch (dataType)
                    {
                        case "String":
                            field.SetValue(entry, rows[rowIndex].Trim());
                            break;
                        case "Boolean":
                            field.SetValue(entry, rows[rowIndex] == "TRUE");
                            break;
                        case "Int32":
                            field.SetValue(entry, string.IsNullOrEmpty(rows[rowIndex]) ? 0 : int.Parse(rows[rowIndex]));
                            break;
                        case "Single":
                            field.SetValue(entry, string.IsNullOrEmpty(rows[rowIndex]) ? 0f : float.Parse(rows[rowIndex]));
                            break;
                        default:
                            Debug.LogWarning($"GameConfig: unknown data type {dataType} at {headers[rowIndex]}, filename {resourceUrl}");
                            break;
                    }
                }

                data.Add(entry);
            }

            return data;
        }
    }
}
