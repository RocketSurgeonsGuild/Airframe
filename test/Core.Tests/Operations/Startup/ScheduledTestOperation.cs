using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Rocket.Surgery.Airframe.Core.Tests;

internal class ScheduledTestOperation : TestOperation
{
    private readonly IScheduler _scheduler;
    private readonly TimeSpan _delay;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduledTestOperation"/> class.
    /// </summary>
    /// <param name="scheduler">The scheduler.</param>
    /// <param name="delay">The delay.</param>
    /// <param name="canExecute">Whether the instance can execute.</param>
    public ScheduledTestOperation(IScheduler scheduler, TimeSpan delay, bool canExecute = true)
        : base(canExecute)
    {
        _scheduler = scheduler;
        _delay = delay;
    }

    /// <inheritdoc/>
    protected override IObservable<Unit> Start() => Observable.Return(Unit.Default).Delay(_delay, _scheduler).Finally(() => Executed = true);
}