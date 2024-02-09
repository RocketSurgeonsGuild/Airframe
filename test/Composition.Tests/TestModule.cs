using DryIoc;

namespace Rocket.Surgery.Airframe.Composition.Tests;

public class TestModule : DryIocModule
{
    /// <inheritdoc/>
    public override void Load(IRegistrator registrar)
    {
        registrar.RegisterViewModel<TestViewModel>();
        registrar.RegisterView<TestView, TestViewModel>();
    }
}