namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Interface representing whether a thing™ can execute.
    /// </summary>
    public interface ICanExecute
    {
        /// <summary>
        /// Gets a value indicating whether this instance will execute the operation.
        /// </summary>
        /// <returns>Whether the instance will execute.</returns>
        bool CanExecute();
    }
}