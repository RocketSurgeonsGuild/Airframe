namespace Rocket.Surgery.Airframe.Timers
{
    /// <summary>
    /// Interface that defines a timer that can be reset.
    /// </summary>
    public interface IReset
    {
        /// <summary>
        /// Resets the timer.
        /// </summary>
        void Reset();
    }
}