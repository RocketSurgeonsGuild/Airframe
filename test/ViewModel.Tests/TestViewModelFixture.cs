using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Rocket.Surgery.ViewModel.Tests
{
    internal class TestViewModelFixture : ITestFixtureBuilder
    {
        public static implicit operator Test(TestViewModelFixture fixture) => fixture.Build();

        private Test Build()
        {
            var viewModel = new Test();
            viewModel.AlertInteraction.RegisterHandler(_ => _.SetOutput(true));
            viewModel.ConfirmationInteraction.RegisterHandler(_ => _.SetOutput(true));
            viewModel.ErrorInteraction.RegisterHandler(_ => _.SetOutput(true));
            return viewModel;
        }
    }
}