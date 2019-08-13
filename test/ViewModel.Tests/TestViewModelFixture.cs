using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Rocket.Surgery.ViewModel.Tests
{
    internal class TestViewModelFixture : ITestFixtureBuilder
    {
        public static implicit operator TestViewModel(TestViewModelFixture fixture) => fixture.Build();

        private TestViewModel Build()
        {
            var viewModel = new TestViewModel();
            viewModel.ErrorInteraction.RegisterHandler(_ => _.SetOutput(true));
            return viewModel;
        }
    }
}