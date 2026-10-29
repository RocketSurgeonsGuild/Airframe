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
        [System.Obsolete]
        public override string Read(string key) => key;
    }
}