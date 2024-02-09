using System;

namespace Rocket.Surgery.Airframe.Tests
{
    internal sealed class NoExceptionHandlerStub : IObserver<Exception>
    {
        public static readonly NoExceptionHandlerStub Instance = new NoExceptionHandlerStub();

        /// <inheritdoc/>
        public void OnCompleted()
        {
        }

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
        }

        /// <inheritdoc />
        public void OnNext(Exception exception)
        {
        }
    }
}