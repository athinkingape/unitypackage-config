using System.Collections.Generic;

namespace Configs.Utils {
    public interface IMultiConfigTable<T> {
        IEnumerable<T> GetById(string id);
    }

    public class MutliConfigTable<T> : IMultiConfigTable<T>
        where T : IConfigTableEntry {
        private readonly Dictionary<string, List<T>> _entries = new();

        public MutliConfigTable(IEnumerable<T> entires) {
            foreach (var tableEntry in entires) {
                var currentEntires = _entries.GetValueOrDefault(tableEntry.Id, new List<T>());
                currentEntires.Add(tableEntry);
                _entries[tableEntry.Id] = currentEntires;
            }
        }

        public IEnumerable<T> GetById(string id) => _entries.GetValueOrDefault(id);
    }
}
