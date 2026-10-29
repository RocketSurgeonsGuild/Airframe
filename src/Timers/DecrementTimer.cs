using System;

namespace Rocket.Surgery.Airframe.Timers;

/// <summary>
/// Represents an <see cref="IDecrement"/> <see cref="ObservableTimer"/>.
/// </summary>
public class DecrementTimer : ObservableTimer, IDecrement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DecrementTimer"/> class.
    /// </summary>
    /// <param name="schedulerProvider">The scheduler provider.</param>
    public DecrementTimer(ISchedulerProvider schedulerProvider)
        : base(schedulerProvider)
    {
    }

    /// <inheritdoc/>
    protected override TimeSpan TimeAccumulator(TimeSpan accumulated) => accumulated - TimeSpans.RefreshInterval;

    /// <inheritdoc/>
    protected override bool Elapse(TimeSpan elapsed) => elapsed <= TimeSpan.Zero;
}