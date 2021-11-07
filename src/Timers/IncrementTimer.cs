using System;
using JetBrains.Annotations;

namespace Rocket.Surgery.Airframe.Timers
{
    /// <summary>
    /// Represents an <see cref="IDecrement"/> <see cref="ObservableTimer"/>.
    /// </summary>
    public class IncrementTimer : ObservableTimer, IIncrement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IncrementTimer"/> class.
        /// </summary>
        /// <param name="schedulerProvider">The scheduler provider.</param>
        public IncrementTimer([NotNull] ISchedulerProvider schedulerProvider)
            : base(schedulerProvider)
        {
        }

        /// <inheritdoc/>
        protected override TimeSpan TimeAccumulator(TimeSpan accumulated) => accumulated + TimeSpans.RefreshInterval;

        /// <inheritdoc/>
        protected override bool Elapse(TimeSpan elapsed) => false;
    }
}