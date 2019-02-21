using ReactiveUI;

namespace Rocket.Surgery.ReactiveUI
{
    /// <summary>
    /// Interface representation of a base ReactiveUI View Model.
    /// </summary>
    public interface IViewModelBase : IReactiveObject
    {
        /// <summary>
        /// Gets the alert <see cref="Interaction{TInput,TOutput}"/>.
        /// </summary>
        Interaction<string, bool> AlertInteraction { get; }

        /// <summary>
        /// Gets the confirmation <see cref="Interaction{TInput,TOutput}"/>.
        /// </summary>
        Interaction<string, bool> ConfirmationInteraction { get; }

        /// <summary>
        /// Gets the error <see cref="Interaction{TInput,TOutput}"/>.
        /// </summary>
        Interaction<string, bool> ErrorInteraction { get; }

        /// <summary>
        /// Gets a value indicating whether the view model is doing work.
        /// </summary>
        bool IsBusy { get; }
    }
}