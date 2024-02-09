using ReactiveMarbles.Mvvm;
using ReactiveUI;
using Rocket.Surgery.Airframe.Navigation;
using System.Reactive.Disposables;

namespace Rocket.Surgery.Airframe.ViewModels.Tests
{
    internal class TestNavigationViewModel : NavigableViewModelBase
    {
        public TestNavigationViewModel()
        {
            Initialize
               .AsValue(_ => RaisePropertyChanged(nameof(InitializeParameter)))
               .DisposeWith(Garbage);

            NavigatedTo
               .AsValue(_ => RaisePropertyChanged(nameof(NavigatedToParameter)))
               .DisposeWith(Garbage);

            NavigatedFrom
               .AsValue(_ => RaisePropertyChanged(nameof(NavigatedFromParameter)))
               .DisposeWith(Garbage);
        }

        public IArguments NavigatedToParameter => _navigatedToParameter.Value;

        public IArguments NavigatedFromParameter => _navigatedFromParameter.Value;

        public IArguments InitializeParameter => _navigatingToParameter.Value;

        public bool Overriden
        {
            get => _overriden;
            set => this.RaiseAndSetIfChanged(ref _overriden, value);
        }

        private readonly ObservableAsPropertyHelper<IArguments> _navigatedToParameter;
        private readonly ObservableAsPropertyHelper<IArguments> _navigatedFromParameter;
        private readonly ObservableAsPropertyHelper<IArguments> _navigatingToParameter;
        private bool _overriden;
    }
}