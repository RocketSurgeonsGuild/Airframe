using ReactiveUI;
using Rocket.Surgery.Airframe.Navigation;
using System;
using System.Reactive;
using System.Reactive.Disposables;

namespace Rocket.Surgery.Airframe.ViewModels.Tests
{
    internal class TestNavigationViewModel : NavigableViewModelBase
    {
        private readonly ObservableAsPropertyHelper<IArguments> _navigatedToParameter;
        private readonly ObservableAsPropertyHelper<IArguments> _navigatedFromParameter;
        private readonly ObservableAsPropertyHelper<IArguments> _navigatingToParameter;
        private bool _overriden;

        public TestNavigationViewModel()
        {
            NavigatedTo
               .ToProperty(this, nameof(NavigatedToParameter), out _navigatedToParameter)
               .DisposeWith(Garbage);

            NavigatedFrom
               .ToProperty(this, nameof(NavigatedFromParameter), out _navigatedFromParameter)
               .DisposeWith(Garbage);

            Initialize
               .ToProperty(this, nameof(NavigatingToParameter), out _navigatingToParameter)
               .DisposeWith(Garbage);
        }

        public IArguments NavigatedToParameter => _navigatedToParameter.Value;

        public IArguments NavigatedFromParameter => _navigatedFromParameter.Value;

        public IArguments NavigatingToParameter => _navigatingToParameter.Value;

        protected override IObservable<Unit> ExecuteInitialize()
        {
            Overriden = true;

            return base.ExecuteInitialize();
        }

        public bool Overriden
        {
            get => _overriden;
            set => this.RaiseAndSetIfChanged(ref _overriden, value);
        }
    }
}