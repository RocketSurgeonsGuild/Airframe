using ReactiveUI;
using Rocket.Surgery.Airframe.ViewModels;
using Sextant;
using System;
using System.Reactive;
using System.Reactive.Disposables;

namespace Airframe.Tests.ViewModels
{
    internal class TestNavigationViewModel : NavigableViewModelBase
    {
        private readonly ObservableAsPropertyHelper<INavigationParameter> _navigatedToParameter;
        private readonly ObservableAsPropertyHelper<INavigationParameter> _navigatedFromParameter;
        private readonly ObservableAsPropertyHelper<INavigationParameter> _navigatingToParameter;
        private bool _overriden;

        public TestNavigationViewModel()
        {
            NavigatedTo
               .ToProperty(this, nameof(NavigatedToParameter), out _navigatedToParameter)
               .DisposeWith(Garbage);

            NavigatedFrom
               .ToProperty(this, nameof(NavigatedFromParameter), out _navigatedFromParameter)
               .DisposeWith(Garbage);

            NavigatingTo
               .ToProperty(this, nameof(NavigatingToParameter), out _navigatingToParameter)
               .DisposeWith(Garbage);
        }

        public INavigationParameter NavigatedToParameter => _navigatedToParameter.Value;
        public INavigationParameter NavigatedFromParameter => _navigatedFromParameter.Value;
        public INavigationParameter NavigatingToParameter => _navigatingToParameter.Value;

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