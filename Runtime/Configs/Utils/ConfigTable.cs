using System.Collections.Generic;

namespace Configs.Utils {
    public interface IConfigTable<T> {
        T GetById(string id);
        IEnumerable<T> Entires { get; }
    }

    public class ConfigTable<T> : IConfigTable<T>
        where T : IConfigTableEntry {
        private readonly Dictionary<string, T> _entries = new();

        public ConfigTable(IEnumerable<T> configTableEntries) {
            foreach (var tableEntry in configTableEntries) {
                _entries.Add(tableEntry.Id, tableEntry);
            }
        }

        public IEnumerable<T> Entires => _entries.Values;
        public T GetById(string id) => _entries.GetValueOrDefault(id);
    }
}
