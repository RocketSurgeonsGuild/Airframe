//HintName: 
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Sample
{
    public class Example
    {
        public event EventHandler Changed;

        public IObservable<EventPattern<object>> Changes() => Observable.Create<EventPattern<object>>(observer =>
        {
            void Handler(object sender, EventArgs e) => observer.OnNext(new EventPattern<object>(sender, e));
            this.Changed += Handler;
            return Disposable.Create(() => this.Changed -= Handler);
        });
    }
}

#pragma warning disable CS0067
namespace Sample
{
    public class Source : System.ComponentModel.INotifyPropertyChanged, System.ComponentModel.INotifyPropertyChanging
    {
        public static event System.EventHandler StaticChanged;

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;

        public event System.EventHandler Changed;

        public event System.EventHandler<string> Named;

        public event System.Action<int> Counted;

        public event System.Action Pinged;
    }
}