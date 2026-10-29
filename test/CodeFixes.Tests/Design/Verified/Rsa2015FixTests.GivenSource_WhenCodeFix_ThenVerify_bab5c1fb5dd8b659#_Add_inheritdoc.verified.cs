//HintName: 
namespace Sample
{
    public abstract class ReaderBase
    {
        public abstract string Read(string key);
    }

    public class Reader : ReaderBase
    {
        /// <inheritdoc />
        public override string Read(string key) => key;
    }
}