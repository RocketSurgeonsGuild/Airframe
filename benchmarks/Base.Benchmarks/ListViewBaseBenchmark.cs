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
