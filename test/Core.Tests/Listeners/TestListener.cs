using Airframe.Testing;
using Microsoft.Extensions.Logging;
using ReactiveMarbles.Extensions;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Rocket.Surgery.Airframe.Core.Tests.Listeners;

internal class TestListener : Listener, INotifier<Unit>
{
    public TestListener(ILoggerFactory loggerFactory)
        : base(loggerFactory)
    {
        _subject = new Subject<Unit>().DisposeWith(Garbage);
        Listening = _subject.AsObservable().Publish();
    }

    /// <inheritdoc/>
    public void Notify(Unit item) => _subject.OnNext(item);

    private readonly ISubject<Unit> _subject;
}