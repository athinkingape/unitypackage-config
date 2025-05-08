using System.Collections.Generic;

namespace Configs.Utils {
    public interface IMultiConfigTable<T> {
        IEnumerable<string> Ids { get; }
        bool HasId(string id);
        IEnumerable<T> GetById(string id);
    }

    public class MultiConfigTable<T> : IMultiConfigTable<T>
        where T : IConfigTableEntry {
        private readonly Dictionary<string, List<T>> _entries = new();

        public MultiConfigTable(IEnumerable<T> entires) {
            foreach (var tableEntry in entires) {
                var currentEntires = _entries.GetValueOrDefault(tableEntry.Id, new List<T>());
                currentEntires.Add(tableEntry);
                _entries[tableEntry.Id] = currentEntires;
            }
        }

        public IEnumerable<string> Ids => _entries.Keys;
        public bool HasId(string id) => _entries.ContainsKey(id);
        public IEnumerable<T> GetById(string id) => string.IsNullOrEmpty(id) ? default : _entries.GetValueOrDefault(id);
    }
}
