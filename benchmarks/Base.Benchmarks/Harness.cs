using System;
using BenchmarkDotNet.Running;
using Xunit;

namespace Rocket.Surgery.ReactiveUI.Benchmarks
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
        /// BaseContentPage benchmarks.
        /// </summary>
        [Fact]
        public void ContentPageBase() => BenchmarkRunner.Run<ContentPageBaseBenchmark>();

        /// <summary>
        /// BaseContentPage benchmarks.
        /// </summary>
        [Fact]
        public void ContentViewBase() => BenchmarkRunner.Run<ContentViewBaseBenchmark>();

        /// <summary>
        /// BaseContentPage benchmarks.
        /// </summary>
        [Fact]
        public void ListViewBase() => BenchmarkRunner.Run<ListViewBaseBenchmark>();
    }
}
