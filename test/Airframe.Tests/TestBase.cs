using ReactiveUI;

namespace Rocket.Surgery.Airframe.Tests
{
    public abstract class TestBase
    {
        protected TestBase() => Initialize();

        private static void Initialize() => RxApp.DefaultExceptionHandler = NoExceptionHandlerStub.Instance;
    }
}