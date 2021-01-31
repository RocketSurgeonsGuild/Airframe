namespace Rocket.Surgery.Airframe.ViewModels
{
    /// <summary>
    /// Base ReactiveUI View Model.
    /// </summary>
    public abstract class ViewModelBase : ReactiveDisposable, IViewModel
    {
        /// <inheritdoc />
        public virtual bool IsLoading { get; }
    }
}