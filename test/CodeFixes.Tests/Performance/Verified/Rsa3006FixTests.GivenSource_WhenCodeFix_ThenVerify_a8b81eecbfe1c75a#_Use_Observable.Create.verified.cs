//HintName: 
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Sample
{
    public class Example
    {
        public IDisposable Changed(Source source, object sender, EventArgs e)
        {
            var observer = Observer.Create<EventPattern<object>>(_ => { });
            var Handler = 1;

            return Observable.Create<EventPattern<object>>(observer1 =>
            {
                void Handler1(object sender, EventArgs e) => observer1.OnNext(new EventPattern<object>(sender, e));
                source.Changed += Handler1;
                return Disposable.Create(() => source.Changed -= Handler1);
            })
               .Subscribe(observer);
        }
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