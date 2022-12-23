using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Rocket.Surgery.Airframe.Core.Tests.AppStart
{
    internal class TestOperation : StartupOperationBase
    {
        private readonly bool _canExecute;

        public TestOperation(bool canExecute = true) => _canExecute = canExecute;

        public bool Executed { get; protected set; }

        /// <inheritdoc/>
        protected override IObservable<Unit> Start() => Observable.Return(Unit.Default).Finally(() => Executed = true);

        /// <inheritdoc/>
        protected override bool CanExecute() => _canExecute;
    }
}