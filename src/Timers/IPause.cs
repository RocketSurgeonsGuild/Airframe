namespace Rocket.Surgery.Airframe.Timers
{
    /// <summary>
    /// Interface that defines a timer that can pause.
    /// </summary>
    public interface IPause
    {
        /// <summary>
        /// Pause a timer.
        /// </summary>
        void Pause();
    }
}