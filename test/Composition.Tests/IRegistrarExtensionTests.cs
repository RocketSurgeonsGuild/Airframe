using DryIoc;
using FluentAssertions;
using ReactiveUI;
using Xunit;

namespace Rocket.Surgery.Airframe.Composition.Tests;

public sealed class IRegistrarExtensionTests
{
    public class TheRegisterViewMethod
    {
        [Fact]
        public void Should_Register_View_As_IViewFor()
        {
                // Given
                IContainer sut = new Container();

                // When
                sut.RegisterView<TestView, TestViewModel>();

                // Then
                sut.Resolve<IViewFor<TestViewModel>>().Should().BeOfType<TestView>();
            }
    }

    public class TheRegisterViewModelMethod
    {
        [Fact]
        public void Should_Register_ViewModel()
        {
                // Given
                IContainer sut = new Container();

                // When
                sut.RegisterViewModel<TestViewModel>();

                // Then
                sut.Resolve<TestViewModel>().Should().NotBeNull();
            }
    }

    public class TheRegisterModuleMethod
    {
        [Fact]
        public void Should_Throw_When_Resolve_Module()
        {
                // Given
                IContainer sut = new Container();
                sut.RegisterModule(new TestModule());

                // When
                var result = Record.Exception(() => sut.Resolve<TestModule>().Should().BeNull());

                // Then
                result
                    .Should()
                    .BeOfType<ContainerException>()
                    .Which
                    .ErrorName
                    .Should()
                    .Be("UnableToResolveUnknownService");
            }

        [Fact]
        public void Should_Resolve_Module_Registrations()
        {
                // Given
                IContainer sut = new Container();

                // When
                sut.RegisterModule(new TestModule());

                // Then
                sut.Resolve<TestViewModel>().Should().NotBeNull();
                sut.Resolve<IViewFor<TestViewModel>>().Should().BeOfType<TestView>();
            }
    }

    public class TheRegisterModuleGenericMethod
    {
        [Fact]
        public void Should_Resolve_Module()
        {
                // Given
                IContainer sut = new Container();

                // When
                sut.RegisterModule<TestModule>();

                // Then
                sut.Resolve<TestModule>().Should().NotBeNull();
            }

        [Fact]
        public void Should_Resolve_Module_Registrations()
        {
                // Given
                IContainer sut = new Container();

                // When
                sut.RegisterModule<TestModule>();

                // Then
                sut.Resolve<TestViewModel>().Should().NotBeNull();
                sut.Resolve<IViewFor<TestViewModel>>().Should().BeOfType<TestView>();
            }
    }
}