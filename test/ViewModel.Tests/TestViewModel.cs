using Rocket.Surgery.Airframe.ViewModels;

namespace Rocket.Surgery.Airframe.ViewModel.Tests
{
    internal class TestViewModel : ViewModelBase
    {
        public override bool IsLoading { get; } = true;
    }
}