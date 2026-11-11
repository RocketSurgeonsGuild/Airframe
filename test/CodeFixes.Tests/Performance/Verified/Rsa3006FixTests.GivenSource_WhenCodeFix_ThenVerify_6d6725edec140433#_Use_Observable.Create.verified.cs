//HintName: 
using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Sample
{
    public class Example
    {
        public IObservable<EventPattern<Source, PropertyChangedEventArgs>> Changed(Source source) =>
            Observable.Create<EventPattern<Source, PropertyChangedEventArgs>>(observer =>
            {
                void Handler(object sender, PropertyChangedEventArgs e) => observer.OnNext(new EventPattern<Source, PropertyChangedEventArgs>((Source)sender, e));
                source.PropertyChanged += Handler;
                return Disposable.Create(() => source.PropertyChanged -= Handler);
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