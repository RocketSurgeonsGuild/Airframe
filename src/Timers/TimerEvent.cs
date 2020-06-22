namespace Rocket.Surgery.Airframe.Timers
{
    /// <summary>
    /// A timer changed notification.
    /// </summary>
    public class TimerEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TimerEvent" /> class.
        /// </summary>
        /// <param name="state">The timer state.</param>
        public TimerEvent(TimerState state)
        {
            State = state;
        }

        /// <summary>
        ///     Gets the timer state.
        /// </summary>
        public TimerState State { get; }
    }
}