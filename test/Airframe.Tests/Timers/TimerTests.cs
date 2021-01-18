using System;
using ReactiveUI;

namespace Airframe.Tests.Timers
{
    internal sealed class BaseExceptionHandlerStub : IObserver<Exception>
    {
        public static readonly BaseExceptionHandlerStub Instance = new BaseExceptionHandlerStub();

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        /// <inheritdoc />
        public void OnNext(Exception exception) {}
    }

    public abstract class TestBase
    {
        protected TestBase()
        {
            Initialize();
        }

        private static void Initialize()
        {
            RxApp.DefaultExceptionHandler = BaseExceptionHandlerStub.Instance;
        }
    }
}