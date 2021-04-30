using ReactiveUI;
using Rocket.Surgery.Airframe.Exceptions;

namespace Rocket.Surgery.Airframe.Forms
{
    /// <summary>
    /// Represents the default exception handler.
    /// </summary>
    public class ExceptionHandler : ExceptionHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandler"/> class.
        /// </summary>
        public ExceptionHandler()
            : base(RxApp.MainThreadScheduler)
        {
        }
    }
}