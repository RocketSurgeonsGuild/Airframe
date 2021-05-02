using Rocket.Surgery.Airframe.ViewModels;

namespace Airframe.Tests.ViewModels
{
    internal class TestViewModel : ViewModelBase
    {
        public override bool IsLoading { get; } = true;
    }
}