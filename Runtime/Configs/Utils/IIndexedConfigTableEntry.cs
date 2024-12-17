namespace Configs.Utils {
    public interface IIndexedConfigTableEntry : IConfigTableEntry {
        public string Index { get; }
    }
}