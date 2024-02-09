using Rocket.Surgery.Airframe.Navigation;

namespace Rocket.Surgery.Airframe.ViewModels.Tests
{
    internal class TestNavigationViewModelFixture
    {
        public static implicit operator TestNavigationViewModel(TestNavigationViewModelFixture fixture) => fixture.Build();

        public INavigated AsNavigated() => Build();

        private TestNavigationViewModel Build() => new TestNavigationViewModel();
    }
}