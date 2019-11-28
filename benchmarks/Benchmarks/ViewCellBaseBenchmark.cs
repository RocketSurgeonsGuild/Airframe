using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using ReactiveUI.XamForms;
using Xamarin.Forms;

namespace Rocket.Surgery.Airframe.Benchmarks
{
    [CoreJob]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class ViewCellBaseBenchmark
    {
        /// <summary>
        /// Creates the reactive content page.
        /// </summary>
        /// <returns></returns>
        [Benchmark(Baseline = true)]
        public ReactiveViewCell<Test> CreateReactiveViewCell() => new ReactiveViewCell<Test>();

        /// <summary>
        /// Creates the content view.
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public ViewCell CreateViewCell() => new ViewCell();

        /// <summary>
        /// Creates the test content page.
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public TestViewCell CreateTestViewCell() => new TestViewCell();
    }
}
