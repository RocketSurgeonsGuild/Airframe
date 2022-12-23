using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Rocket.Surgery.Airframe.Core.Tests.AppStart
{
    internal class ScheduledTestOperation : TestOperation
    {
        private readonly IScheduler _scheduler;
        private readonly TimeSpan _delay;

        public ScheduledTestOperation(IScheduler scheduler, TimeSpan delay, bool willExecute = true)
            : base(willExecute)
        {
            _scheduler = scheduler;
            _delay = delay;
        }

        /// <inheritdoc/>
        protected override IObservable<Unit> Start() => Observable.Return(Unit.Default).Delay(_delay, _scheduler).Finally(() => Executed = true);
    }
}