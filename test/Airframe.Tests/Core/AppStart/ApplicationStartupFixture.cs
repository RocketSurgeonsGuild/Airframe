using Rocket.Surgery.Airframe;
using Rocket.Surgery.Extensions.Testing.Fixtures;
using System.Collections.Generic;
using System.Linq;

namespace Airframe.Tests.Core.AppStart
{
    internal class ApplicationStartupFixture : ITestFixtureBuilder
    {
        private IEnumerable<IStartupOperation> _startupOperations = Enumerable.Empty<IStartupOperation>();
        public static implicit operator ApplicationStartup(ApplicationStartupFixture fixture) => fixture.Build();

        public ApplicationStartupFixture WithStartupOperations(params StartupOperationBase[] startupOperations)
            => this.With(ref _startupOperations, startupOperations);

        private ApplicationStartup Build() => new ApplicationStartup(_startupOperations);
    }
}