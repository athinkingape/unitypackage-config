using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
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

            for (int i = 0; i < headers.Length; i++) {
                headers[i] = "_" + headers[i].Trim();
            }
            
            var fields = new Dictionary<string, FieldInfo>();

            foreach (var field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                fields[field.Name] = field;
            }

            for (var lineIndex = 1; lineIndex < lines.Length; lineIndex++)
            {
                var rows = Regex.Split(lines[lineIndex], "[,]{1}(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                var entry = new T();

                if (headers.Length != rows.Length) {
                    Debug.LogError($"headers {headers.Length} != rows {rows.Length}. Rows {string.Join(";", rows)}");
                    return null;
                }
                
                for (var rowIndex = 0; rowIndex < rows.Length; rowIndex++) {
                    string fieldKey = headers[rowIndex];
                    
                    if (!fields.TryGetValue(fieldKey, out FieldInfo field)) {
                        Debug.LogWarning($"GameConfig: field {fieldKey} is not found at class {typeof(T)}, filename {resourceUrl}");
                        continue;
                    }

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
