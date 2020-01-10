using DryIoc;
using Rocket.Surgery.Airframe.Composition;
using Rocket.Surgery.Airframe.Forms;
using Rocket.Surgery.Extensions.Testing.Fixtures;

namespace Composition.Tests.Mock
{
    public class ApplicationMock : ApplicationBase
    {
        private readonly IPlatformRegistrar _platformRegistrar;

        public ApplicationMock(IPlatformRegistrar platformRegistrar)
            : base(platformRegistrar)
        {
            _platformRegistrar = platformRegistrar;
        }

        protected override void RegisterServices(IContainer container)
        {
            container
                .RegisterView<TestView, TestViewModel>()
                .RegisterViewModel<TestViewModel>();
        }
    }

    public class ApplicationFixture : ITestFixtureBuilder
    {
        private IPlatformRegistrar _platformRegistrar;

        public ApplicationFixture()
        {
            Xamarin.Forms.Mocks.MockForms.Init();
        }
        
        public ApplicationFixture WithPlatformRegistration(IPlatformRegistrar registrar) =>
            this.With(ref _platformRegistrar, registrar);

        public static implicit operator ApplicationMock(ApplicationFixture fixture) => fixture.Build();

        private ApplicationMock Build() => new ApplicationMock(_platformRegistrar);
    }
}