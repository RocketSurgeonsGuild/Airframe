using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Rocket.Surgery.Airframe.Core.Tests.Listeners;

internal class TestListenerFixture : ITestFixtureBuilder
{
    public static implicit operator TestListener(TestListenerFixture fixture) => fixture.Build();

    private TestListener Build() => new TestListener(_loggerFactory);

    private ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;
}