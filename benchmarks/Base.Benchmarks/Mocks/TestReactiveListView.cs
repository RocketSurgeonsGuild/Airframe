using System;
using Rocket.Surgery.ReactiveUI.Forms;
using Xamarin.Forms;

namespace Rocket.Surgery.ReactiveUI.Benchmarks
{
    public class TestReactiveListView : ListViewBase<Test> {
        public TestReactiveListView(Type cellType, ListViewCachingStrategy listViewCachingStrategy = ListViewCachingStrategy.RecycleElement)
            : base(cellType, listViewCachingStrategy)
        {
        }
    }
}