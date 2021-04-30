using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using ReactiveUI;
using Xamarin.Forms;

[assembly: XmlnsPrefix("https://schemas.rocketsurgeonsguild.com/xaml/airframe/viewmodels", "viewmodels")]
[assembly: XmlnsDefinition("https://schemas.rocketsurgeonsguild.com/xaml/airframe/viewmodels", "Rocket.Surgery.Airframe.ViewModels")]

[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Typed and untyped.")]
namespace Rocket.Surgery.Airframe.ViewModels
{
    /// <summary>
    /// Interface representation of a base ReactiveUI View Model.
    /// </summary>
    public interface IViewModel : IReactiveObject
    {
        /// <summary>
        /// Gets a value indicating whether the view model is doing work.
        /// </summary>
        bool IsLoading { get; }
    }

    /// <summary>
    /// Interface representing an view model that can be intialized.
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
    /// Interface representing an view model that can be intialized.
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
    /// Interface representing an view model that can be intialized.
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