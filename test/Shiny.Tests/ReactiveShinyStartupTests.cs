using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shiny;
using Xunit;

namespace Airframe.Shiny.Tests
{
    public class ReactiveShinyStartupTests
    {
        [Fact]
        public void Should_Register_Configuration()
        {
            // Given
            var configuration = Substitute.For<IConfiguration>();
            var serviceCollection = new ServiceCollection();
            var sut = new TestStartup(serviceCollection, configuration);

            // When
            sut.ConfigureServices(serviceCollection, Arg.Any<IPlatform>());

            // Then
            serviceCollection.Should().ContainSingle(x => x.ServiceType == typeof(IConfiguration));
        }

    }
}
