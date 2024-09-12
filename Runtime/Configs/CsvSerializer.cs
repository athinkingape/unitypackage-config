using System;
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

            List<T> data = new();
            Type type = typeof(T);
            string[] lines = dataset.text.Split("\n");
            string[] headers = lines[0].Split(',');
            Dictionary<string, FieldInfo> fields = new Dictionary<string, FieldInfo>();

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
                            field.SetValue(entry, rows[rowIndex]);
                            break;
                        case "Boolean":
                            field.SetValue(entry, rows[rowIndex] == "TRUE");
                            break;
                        case "Int32":
                            field.SetValue(entry, int.Parse(rows[rowIndex]));
                            break;
                        case "Single":
                            field.SetValue(entry, float.Parse(rows[rowIndex]));
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
