using System;
using BenchmarkDotNet.Running;
using Xunit;

namespace Rocket.Surgery.Airframe.Benchmarks
{
    public class Harness
    {
        [Fact]
        public void ApplicationBase() => BenchmarkRunner.Run<ApplicationBaseBenchmark>();

        /// <summary>
        /// ViewModelBase benchmarks.
        /// </summary>
        [Fact]
        public void ViewModelBase() => BenchmarkRunner.Run<ViewModelBaseBenchmark>();

        /// <summary>
        /// ContentPageBase benchmarks.
        /// </summary>
        [Fact]
        public void ContentPageBase() => BenchmarkRunner.Run<ContentPageBaseBenchmark>();

        /// <summary>
        /// ContentViewBase benchmarks.
        /// </summary>
        [Fact]
        public void ContentViewBase() => BenchmarkRunner.Run<ContentViewBaseBenchmark>();

        /// <summary>
        /// ReactiveListView benchmarks.
        /// </summary>
        [Fact]
        public void ListViewBase() => BenchmarkRunner.Run<ListViewBenchmark>();

        /// <summary>
        /// ViewCellBase benchmarks.
        /// </summary>
        [Fact]
        public void ViewCellBase() => BenchmarkRunner.Run< ViewCellBaseBenchmark>();}
}
