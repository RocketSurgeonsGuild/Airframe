using System;

namespace Airframe.Tests
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
}