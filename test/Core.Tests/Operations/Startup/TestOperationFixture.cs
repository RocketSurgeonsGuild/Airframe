using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Rocket.Surgery.Airframe.Core.Tests
{
    internal sealed class TestOperationFixture : ITestFixtureBuilder
    {
        private bool _canExecute = true;

        public static implicit operator TestOperation(TestOperationFixture fixture) => fixture.Build();

        public TestOperationFixture WithCanExecute(bool canExecute) => this.With(ref _canExecute, canExecute);

        public IOperation AsOperation() => Build();

        public IStartupOperation AsStartupOperation() => Build();

        private TestOperation Build() => new(_canExecute);
    }
}