using Rocket.Surgery.Airframe;
using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Airframe.Tests.Core.AppStart
{
    internal class TestOperation : StartupOperationBase
    {
        private readonly IScheduler _scheduler;
        private readonly TimeSpan _delay;
        private readonly bool _canExecute;

        public TestOperation(IScheduler scheduler, TimeSpan delay, bool canExecute = true)
        {
            _scheduler = scheduler;
            _delay = delay;
            _canExecute = canExecute;
        }

        protected override IObservable<Unit> Start() => Observable.Return(Unit.Default).Delay(_delay, _scheduler);
        protected override bool CanExecute() => _canExecute;
    }
}