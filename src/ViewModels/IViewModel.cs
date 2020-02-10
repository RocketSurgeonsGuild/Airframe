using ReactiveUI;
using Xamarin.Forms;

[assembly: XmlnsPrefix("https://schemas.rocketsurgeonsguild.com/xaml/airframe/viewmodels", "viewmodels")]
[assembly: XmlnsDefinition("https://schemas.rocketsurgeonsguild.com/xaml/airframe/viewmodels", "Rocket.Surgery.Airframe.ViewModels")]

namespace Rocket.Surgery.Airframe.ViewModels
{
    /// <summary>
    /// Interface representation of a base ReactiveUI View Model.
    /// </summary>
    public interface IViewModel : IReactiveObject
    {
        /// <summary>
        /// Gets the view model id.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets a value indicating whether the view model is doing work.
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// Gets the error <see cref="Interaction{TInput,TOutput}"/>.
        /// </summary>
        Interaction<string, bool> ErrorInteraction { get; }
    }
}