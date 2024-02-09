using ReactiveMarbles.Mvvm;
using Rocket.Surgery.Airframe.Navigation;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;

namespace Rocket.Surgery.Airframe.ViewModels.Tests
{
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed")]
    internal class TestNavigationViewModel : NavigableViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestNavigationViewModel"/> class.
        /// </summary>
        public TestNavigationViewModel()
        {
            _initializeParameter =
                Initialize
                   .AsValue(_ => RaisePropertyChanged(nameof(InitializeParameter)))
                   .DisposeWith(Garbage)!;

            _navigatedToParameter =
                NavigatedTo
                   .AsValue(_ => RaisePropertyChanged(nameof(NavigatedToParameter)))
                   .DisposeWith(Garbage)!;

            _navigatedFromParameter =
                NavigatedFrom
                   .AsValue(_ => RaisePropertyChanged(nameof(NavigatedFromParameter)))
                   .DisposeWith(Garbage)!;
        }

        public IArguments NavigatedToParameter => _navigatedToParameter.Value;

        public IArguments NavigatedFromParameter => _navigatedFromParameter.Value;

        public IArguments InitializeParameter => _initializeParameter.Value;

        private readonly ValueBinder<IArguments> _initializeParameter;
        private readonly ValueBinder<IArguments> _navigatedToParameter;
        private readonly ValueBinder<IArguments> _navigatedFromParameter;
    }
}