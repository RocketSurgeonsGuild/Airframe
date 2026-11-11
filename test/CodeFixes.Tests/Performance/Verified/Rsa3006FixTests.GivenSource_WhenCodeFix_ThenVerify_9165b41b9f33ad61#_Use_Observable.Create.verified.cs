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
        public IObservable<EventPattern<PropertyChangedEventArgs>> Changed(Source source) =>
            Observable.Create<EventPattern<PropertyChangedEventArgs>>(observer =>
            {
                void Handler(object sender, PropertyChangedEventArgs e) => observer.OnNext(new EventPattern<PropertyChangedEventArgs>(sender, e));
                PropertyChangedEventHandler handler = Handler;
                Register(source, handler);
                return Disposable.Create(() => Unregister(source, handler));
            });

        private static void Register(Source source, Delegate handler) =>
            source.PropertyChanged += (PropertyChangedEventHandler)handler;

        private static void Unregister(Source source, Delegate handler) =>
            source.PropertyChanged -= (PropertyChangedEventHandler)handler;
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