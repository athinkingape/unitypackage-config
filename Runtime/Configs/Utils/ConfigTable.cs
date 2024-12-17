using System.Collections.Generic;
using UnityEngine;

namespace Configs.Utils {
    public interface IConfigTable<T> {
        T GetById(string id);
        IEnumerable<T> Entries { get; }
    }

    public class ConfigTable<T> : IConfigTable<T>
        where T : IConfigTableEntry {
        protected readonly Dictionary<string, T> _entries = new();

        internal ConfigTable() { }
        
        public ConfigTable(IEnumerable<T> configTableEntries) {
            foreach (T tableEntry in configTableEntries) {
                AddTableEntry(tableEntry);
            }
        }

        internal void AddTableEntry(T tableEntry) {
            if (tableEntry.Id == null) {
                Debug.LogWarning($"ConfigTable of {typeof(T)}, entry id is null");
                return;
            }
            _entries.Add(tableEntry.Id, tableEntry);
        }

        public IEnumerable<T> Entries => _entries.Values;
        public T GetById(string id) => _entries.GetValueOrDefault(id);
    }
}
