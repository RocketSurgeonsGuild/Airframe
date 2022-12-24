using Rocket.Surgery.Airframe.ViewModels;

namespace Airframe.ViewModels.Tests
{
    internal class TestViewModel : ViewModelBase
    {
        public override bool IsLoading { get; } = true;
    }
}