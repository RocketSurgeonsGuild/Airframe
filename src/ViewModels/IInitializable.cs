using System;
using System.Reactive;

namespace Rocket.Surgery.Airframe.ViewModels
{
    /// <summary>
    /// Interface representing an view model that can be initialized.
    /// </summary>
    /// <typeparam name="TParam">The parameter type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    public interface IInitializable<in TParam, out TResult>
    {
        /// <summary>
        /// Initializes the view model with the provided parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns>The result.</returns>
        IObservable<TResult> Initialize(TParam param);
    }

    /// <summary>
    /// Interface representing an view model that can be initialized.
    /// </summary>
    public interface IInitializable
    {
        /// <summary>
        /// Initializes the view model with the provided parameter.
        /// </summary>
        /// <returns>A completion notification.</returns>
        IObservable<Unit> Initialize();
    }

    /// <summary>
    /// Interface representing an view model that can be initialized.
    /// </summary>
    /// <typeparam name="TParam">The parameter type.</typeparam>
    public interface IInitializable<in TParam>
    {
        /// <summary>
        /// Initializes the view model with the provided parameter.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns>The result.</returns>
        IObservable<Unit> Initialize(TParam param);
    }
}