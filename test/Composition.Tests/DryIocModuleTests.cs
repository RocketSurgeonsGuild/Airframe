using DryIoc;
using FluentAssertions;
using Xunit;

namespace Rocket.Surgery.Airframe.Composition.Tests;

public sealed class DryIocModuleTests
{
    [Fact]
    public void Should_Resolve_Module()
    {
            // Given
            IContainer sut = new Container();

            // When
            var result = sut.RegisterModule<TestModule>();

            result.Resolve<TestModule>().Should().NotBeNull();
        }

        [Fact]
        public void Should_Resolve_Module_Registrations()
        {
            // Given
            IContainer sut = new Container();

            // When
            var result = sut.RegisterModule<TestModule>();

            result.Resolve<TestViewModel>().Should().NotBeNull();
        }
}