using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using Rocket.Surgery.ReactiveUI.Forms;
using Xamarin.Forms;

namespace Rocket.Surgery.ReactiveUI.Benchmarks
{
    [CoreJob]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class ListViewBaseBenchmark
    {
        /// <summary>
        /// Creates the reactive content page.
        /// </summary>
        /// <returns></returns>
        [Benchmark(Baseline = true)]
        public ReactiveListView<Test> CreateReactiveContentPage() => new ReactiveListView<Test>(typeof(TestContentView));

        /// <summary>
        /// Creates the content view.
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public ListView CreateListView() => new ListView();

        /// <summary>
        /// Creates the test content page.
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public TestReactiveListView CreateTestReactiveListView() => new TestReactiveListView(typeof(TestContentView));
    }
}
