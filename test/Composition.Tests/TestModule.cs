using DryIoc;
using Rocket.Surgery.Airframe.Composition;

namespace Composition.Tests
{
    public class TestModule : DryIocModule
    {
        public override void Load(IRegistrator registrar)
        {
            registrar.RegisterViewModel<TestViewModel>();
            registrar.RegisterView<TestView, TestViewModel>();
        }
    }
}