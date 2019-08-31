using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AppKit;
using ReactiveUI;
using PropertyChangingEventArgs = ReactiveUI.PropertyChangingEventArgs;
using PropertyChangingEventHandler = ReactiveUI.PropertyChangingEventHandler;

namespace Rocket.Surgery.ReactiveUI
{
    /// <summary>
    /// An <see cref="NSPopover"/> with the power of an <see cref="IReactiveObject"/>.
    /// </summary>
    public abstract class ReactivePopover : NSPopover, IViewFor, IReactiveNotifyPropertyChanged<ReactivePopover>,
        IHandleObservableErrors, IReactiveObject, ICanActivate
    {
        private Subject<Unit> _activated = new Subject<Unit>();
        private Subject<Unit> _deactivated = new Subject<Unit>();

        private IObservable<EventPattern<PropertyChangedEventArgs>> _propertyChanged;
        private IObservable<EventPattern<PropertyChangingEventArgs>> _propertyChanging;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePopover"/> class.
        /// </summary>
        protected ReactivePopover()
        {
            _propertyChanged = Observable
                .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    handler => PropertyChanged += handler,
                    handler => PropertyChanged -= handler);

            _propertyChanging = Observable
                .FromEventPattern<PropertyChangingEventHandler, PropertyChangingEventArgs>(
                    handler => PropertyChanging += handler,
                    handler => PropertyChanging -= handler);
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <inheritdoc/>
        public IObservable<Unit> Activated => _activated.AsObservable();

        /// <inheritdoc/>
        public IObservable<Unit> Deactivated => _deactivated.AsObservable();

        /// <inheritdoc/>
        public object ViewModel { get; set; }

        /// <inheritdoc/>
        public IObservable<IReactivePropertyChangedEventArgs<ReactivePopover>> Changing { get; }

        /// <inheritdoc/>
        public IObservable<IReactivePropertyChangedEventArgs<ReactivePopover>> Changed { get; }

        /// <inheritdoc/>
        public IObservable<Exception> ThrownExceptions { get; }

        /// <inheritdoc/>
        public IDisposable SuppressChangeNotifications() => throw new NotImplementedException();

        /// <inheritdoc/>
        public void RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}