using Microsoft.Extensions.Logging;
using NSubstitute;
using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Rocket.Surgery.Airframe.Core.Tests.AppStart
{
    internal class ApplicationStartupFixture : ITestFixtureBuilder
    {
        private IEnumerable<IStartupOperation> _startupOperations = Enumerable.Empty<IStartupOperation>();
        private ILoggerFactory _loggerFactory = Substitute.For<ILoggerFactory>();

        public static implicit operator ApplicationStartup(ApplicationStartupFixture fixture) => fixture.Build();

        public ApplicationStartupFixture WithStartupOperations(params StartupOperationBase[] startupOperations)
            => this.With(ref _startupOperations, startupOperations);

        private ApplicationStartup Build() => new ApplicationStartup(_loggerFactory, _startupOperations);
    }
}