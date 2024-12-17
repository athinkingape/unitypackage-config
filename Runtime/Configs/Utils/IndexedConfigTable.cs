using System.Collections.Generic;

namespace Configs.Utils {
    public interface IIndexedConfigTable<T> : IConfigTable<T> where T : IIndexedConfigTableEntry {
        public ConfigTable<T> GetByIndex(string index);
        IEnumerable<string> IndexValues { get; }
    }

    public class IndexedConfigTable<T> : ConfigTable<T>, Utils.IIndexedConfigTable<T>
        where T : IIndexedConfigTableEntry {
        private readonly Dictionary<string, ConfigTable<T>> _index = new();

        public IndexedConfigTable(IEnumerable<T> entries) : base() {
            foreach (T tableEntry in entries) {
                AddTableEntry(tableEntry);

                if (!_index.TryGetValue(tableEntry.Index, out ConfigTable<T> innerTable)) {
                    innerTable = new ConfigTable<T>();
                    _index.Add(tableEntry.Index, innerTable);
                }

                innerTable.AddTableEntry(tableEntry);
            }
        }

        public ConfigTable<T> GetByIndex(string index) {
            return _index.GetValueOrDefault(index);
        }

        public IEnumerable<string> IndexValues => _index.Keys;
    }
}