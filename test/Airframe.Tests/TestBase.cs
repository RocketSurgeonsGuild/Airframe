using ReactiveUI;

namespace Airframe.Tests
{
    public abstract class TestBase
    {
        protected TestBase() => Initialize();

        private static void Initialize() => RxApp.DefaultExceptionHandler = BaseExceptionHandlerStub.Instance;
    }
}