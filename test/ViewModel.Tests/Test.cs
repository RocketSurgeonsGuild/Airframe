using Rocket.Surgery.ReactiveUI;

namespace Rocket.Surgery.ViewModel.Tests
{
    internal class Test : ViewModelBase
    {
        public Test()
        {
            var count = SubscriptionDisposables.Count;
        }
    }
}