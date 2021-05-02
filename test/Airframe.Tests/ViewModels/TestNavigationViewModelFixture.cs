namespace Airframe.Tests.ViewModels
{
    internal class TestNavigationViewModelFixture
    {
        public static implicit operator TestNavigationViewModel(TestNavigationViewModelFixture fixture) => fixture.Build();

        private TestNavigationViewModel Build() => new TestNavigationViewModel();
    }
}