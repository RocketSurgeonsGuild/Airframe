using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Rocket.Surgery.Airframe.Forms;
using Xamarin.Forms;

namespace Rocket.Surgery.Airframe.Performance
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class ListViewBenchmark
    {
        /// <summary>
        /// Creates the reactive list view.
        /// </summary>
        /// <returns></returns>
        [Benchmark(Baseline = true)]
        public ReactiveListView CreateReactiveListView() => new ReactiveListView();

        /// <summary>
        /// Creates the reactive content page.
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public ReactiveListView CreateReactiveListViewWithDataTemplate() => new ReactiveListView(typeof(TestContentView));

        /// <summary>
        /// Creates the list view.
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public ListView CreateListView() => new ListView();
    }
}
