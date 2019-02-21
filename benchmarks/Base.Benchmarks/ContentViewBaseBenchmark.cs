using BenchmarkDotNet.Attributes;
using ReactiveUI.XamForms;
using Xamarin.Forms;

namespace Rocket.Surgery.ReactiveUI.Benchmarks
{
    [CoreJob]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class ContentViewBaseBenchmark
    {
        /// <summary>
        /// Creates the reactive content page.
        /// </summary>
        /// <returns></returns>
        [Benchmark(Baseline = true)]
        public ReactiveContentView<Test> CreateReactiveContentPage() => new ReactiveContentView<Test>();

        /// <summary>
        /// Creates the content view.
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public ContentView CreateContentView() => new ContentView();

        /// <summary>
        /// Creates the test content page.
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public TestContentView CreateTestContentPage() => new TestContentView();
    }
}