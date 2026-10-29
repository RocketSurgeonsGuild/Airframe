//HintName: 
using System.Threading;

namespace Sample
{
    public class FunctionInvocation
    {
        public FunctionInvocation()
        {
            var func = () => string.Empty;
                
            var result = func.Invoke();
        }
    }
}