//HintName: 
namespace Sample
{
    public interface IReader
    {
        string Read(string key);
    }

    public class Reader : IReader
    {
        /// <inheritdoc />
        public string Read(string key) => key;
    }
}