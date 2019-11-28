using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using AppKit;
using CoreGraphics;
using Foundation;
using ReactiveUI;
using Rocket.Surgery.ReactiveUI;
using IReactiveObjectExtensions = Rocket.Surgery.ReactiveUI.IReactiveObjectExtensions;
using PropertyChangingEventArgs = ReactiveUI.PropertyChangingEventArgs;
using PropertyChangingEventHandler = ReactiveUI.PropertyChangingEventHandler;

namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// An <see cref="NSPopover"/> with the power of an <see cref="IReactiveObject"/>.
    /// </summary>
    public abstract class ReactivePopover : NSPopover,
        IViewFor,
        IReactiveNotifyPropertyChanged<ReactivePopover>,
        IHandleObservableErrors,
        IReactiveObject,
        ICanActivate
    {
        private readonly Subject<Unit> _activated = new Subject<Unit>();
        private readonly Subject<Unit> _deactivated = new Subject<Unit>();
        private Lazy<IObservable<Exception>> _thrownExceptions;
        private IObservable<EventPattern<PropertyChangedEventArgs>> _propertyChanged;
        private IObservable<EventPattern<PropertyChangingEventArgs>> _propertyChanging;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePopover"/> class.
        /// </summary>
        protected ReactivePopover()
        {
            RegisterReactiveObject();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePopover"/> class.
        /// </summary>
        /// <param name="coder">The coder.</param>
        protected ReactivePopover(NSCoder coder)
            : base(coder)
        {
            RegisterReactiveObject();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePopover"/> class.
        /// </summary>
        /// <param name="t">The t.</param>
        protected ReactivePopover(NSObjectFlag t)
            : base(t)
        {
            RegisterReactiveObject();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePopover"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        protected ReactivePopover(IntPtr handle)
            : base(handle)
        {
            RegisterReactiveObject();
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
        public IObservable<IReactivePropertyChangedEventArgs<ReactivePopover>> Changing => this.GetChangingObservable();

        /// <inheritdoc/>
        public IObservable<IReactivePropertyChangedEventArgs<ReactivePopover>> Changed => this.GetChangedObservable();

        /// <inheritdoc/>
        public IObservable<Exception> ThrownExceptions => this.GetThrownExceptionsObservable();

        /// <inheritdoc/>
        public IDisposable SuppressChangeNotifications() => IReactiveObjectExtensions.SuppressChangeNotifications(this);

        /// <inheritdoc/>
        public void RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);

        /// <inheritdoc/>
        public void RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _activated?.Dispose();
                _deactivated?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void RegisterReactiveObject()
        {
            _propertyChanged = Observable
                .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    handler => PropertyChanged += handler,
                    handler => PropertyChanged -= handler);

            _propertyChanging = Observable
                .FromEventPattern<PropertyChangingEventHandler, PropertyChangingEventArgs>(
                    handler => PropertyChanging += handler,
                    handler => PropertyChanging -= handler);

            _thrownExceptions =
                new Lazy<IObservable<Exception>>(this.GetThrownExceptionsObservable, LazyThreadSafetyMode.PublicationOnly);
        }
    }
}