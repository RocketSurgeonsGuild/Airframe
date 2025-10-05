//HintName: 
using System.Threading;

namespace Sample
{
    public class FunctionWithParametersInvocation
    {
        public FunctionWithParametersInvocation()
        {
            var func = (string stuff) => string.Empty;
    
            var result = func.Invoke(string.Empty);
        }
    }
}