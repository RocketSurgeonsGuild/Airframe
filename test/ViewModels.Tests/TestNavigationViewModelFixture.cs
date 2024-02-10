namespace Rocket.Surgery.Airframe.ViewModels.Tests;

internal class TestNavigationViewModelFixture
{
    public static implicit operator TestNavigationViewModel(TestNavigationViewModelFixture fixture) => fixture.Build();

    private TestNavigationViewModel Build() => new TestNavigationViewModel();
}