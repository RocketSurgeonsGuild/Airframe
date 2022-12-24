using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Rocket.Surgery.Airframe.Core.Tests
{
    internal class TestOperation : StartupOperationBase
    {
        private readonly bool _canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestOperation"/> class.
        /// </summary>
        /// <param name="canExecute">Whether this instance can execute.</param>
        public TestOperation(bool canExecute = true) => _canExecute = canExecute;

        public bool Executed { get; protected set; }

        /// <inheritdoc/>
        protected override IObservable<Unit> Start() => Observable.Return(Unit.Default).Finally(() => Executed = true);

        /// <inheritdoc/>
        protected override bool CanExecute() => _canExecute;
    }
}