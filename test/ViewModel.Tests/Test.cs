using Rocket.Surgery.Airframe;
using Rocket.Surgery.Airframe.ViewModels;

namespace Rocket.Surgery.Airframe.ViewModel.Tests
{
    internal class TestViewModel : ViewModelBase
    {
        public TestViewModel()
        {
            var count = Subscriptions.Count;
        }

        public override string Id => nameof(TestViewModel);

        protected override void RegisterObservers()
        {
        }

        protected override void ComposeObservables()
        {
        }
    }
}