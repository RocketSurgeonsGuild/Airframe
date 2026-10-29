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
        string IReader.Read(string key) => key;
    }
}