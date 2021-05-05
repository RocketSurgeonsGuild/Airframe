using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Linq;

[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Typed and untyped.")]
namespace Rocket.Surgery.Airframe.ViewModels
{
    /// <summary>
    /// Represents an view model that can be initialized.
    /// </summary>
    public abstract class InitializableViewModel : ViewModelBase, IInitializable
    {
        /// <inheritdoc />
        IObservable<Unit> IInitializable.Initialize() => Initialize();

        /// <summary>
        /// Template method for initialization.
        /// </summary>
        /// <returns>A completion notification.</returns>
        protected virtual IObservable<Unit> ExecuteInitialize() => Observable.Return(Unit.Default);

        private IObservable<Unit> Initialize() => ExecuteInitialize();
    }

    /// <summary>
    /// Represents an view model that can be intialized.
    /// </summary>
    /// <typeparam name="TParam">The parameter type.</typeparam>
    // ReSharper disable once SA1402
    public abstract class InitializableViewModel<TParam> : InitializableViewModel, IInitializable<TParam>
    {
        /// <inheritdoc/>
        IObservable<Unit> IInitializable<TParam>.Initialize(TParam param) => Initialize(param);

        /// <summary>
        /// Template method for initialization.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns>A completion notification.</returns>
        protected virtual IObservable<Unit> ExecuteInitialize(TParam param) => Observable.Return(Unit.Default);

        private IObservable<Unit> Initialize(TParam param) => ExecuteInitialize(param);
    }

    /// <summary>
    /// Represents an view model that can be intialized.
    /// </summary>
    /// <typeparam name="TParam">The parameter type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    public abstract class InitializableViewModel<TParam, TResult> : InitializableViewModel, IInitializable<TParam, TResult>
    {
        /// <inheritdoc/>
        IObservable<TResult> IInitializable<TParam, TResult>.Initialize(TParam param) => Initialize(param);

        /// <summary>
        /// Template method for initialization.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns>A result.</returns>
        protected virtual IObservable<TResult> ExecuteInitialize(TParam param) => Observable.Empty<TResult>();

        private IObservable<TResult> Initialize(TParam param) => ExecuteInitialize(param);
    }
}