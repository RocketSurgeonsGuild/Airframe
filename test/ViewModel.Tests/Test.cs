using Rocket.Surgery.ReactiveUI;

namespace Rocket.Surgery.ViewModel.Tests
{
    internal class Test : ViewModelBase
    {
        public Test()
        {
            var count = Bindings.Count;
        }

        protected override void SetupObservables()
        {
        }

        protected override void RegisterObservers()
        {
        }
    }
}