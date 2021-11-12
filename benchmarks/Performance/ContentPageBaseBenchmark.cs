using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ReactiveUI.XamForms;
using Xamarin.Forms;

namespace Rocket.Surgery.Airframe.Performance
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class ContentPageBaseBenchmark
    {
        /// <summary>
        /// Creates the reactive content page.
        /// </summary>
        /// <returns></returns>
        [Benchmark(Baseline = true)]
        public ReactiveContentPage<Test> CreateReactiveContentPage() => new ReactiveContentPage<Test>();

        /// <summary>
        /// Creates the content page.
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public ContentPage CreateContentPage() => new ContentPage();


        /// <summary>
        /// Creates the test content page.
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public TestPage CreateTestContentPage() => new TestPage();
    }
}
