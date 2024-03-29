using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Rocket.Surgery.Airframe.ViewModels.Tests;

internal class TestViewModelFixture : ITestFixtureBuilder
{
    public static implicit operator TestViewModel(TestViewModelFixture fixture) => fixture.Build();

    private TestViewModel Build() => new TestViewModel();
}