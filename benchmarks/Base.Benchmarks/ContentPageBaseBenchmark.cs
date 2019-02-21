using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using ReactiveUI.XamForms;
using Xamarin.Forms;

namespace Rocket.Surgery.ReactiveUI.Benchmarks
{
    [CoreJob]
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
