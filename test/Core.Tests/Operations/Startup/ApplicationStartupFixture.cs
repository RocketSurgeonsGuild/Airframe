using Microsoft.Extensions.Logging;
using NSubstitute;
using Rocket.Surgery.Extensions.Testing.Fixtures;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Surgery.Airframe.Core.Tests;

internal class ApplicationStartupFixture : ITestFixtureBuilder
{
    private IEnumerable<IStartupOperation> _startupOperations = Enumerable.Empty<IStartupOperation>();
    private ILoggerFactory _loggerFactory = Substitute.For<ILoggerFactory>();

    public static implicit operator ApplicationStartup(ApplicationStartupFixture fixture) => fixture.Build();

    public ApplicationStartupFixture WithStartupOperations(params StartupOperationBase[] startupOperations)
        => this.With(ref _startupOperations, startupOperations);

    public IApplicationStartup AsInterface() => Build();

    private ApplicationStartup Build() => new(_loggerFactory, _startupOperations);
}