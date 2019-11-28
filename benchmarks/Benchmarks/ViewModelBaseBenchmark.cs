using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace Rocket.Surgery.Airframe.Benchmarks
{
    [CoreJob]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class ViewModelBaseBenchmark
    {
        private Test _base;

        [GlobalSetup]
        public void Setup()
        {
            _base = new Test();
        }

        [GlobalCleanup]
        public void Teardown()
        {
            _base = null;
        }

        /// <summary>
        /// Benchmarks the instance creation.
        /// </summary>
        /// <returns></returns>
        [Benchmark(Baseline = true)]
        public TestReactiveObject CreateReactiveObject() => new TestReactiveObject();

        /// <summary>
        /// Benchmarks the instance creation.
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public Test CreateViewModel() => new Test();

        /// <summary>
        /// Benchmarks the error registration.
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public IDisposable RegisterError() =>
            _base.ErrorInteraction.RegisterHandler(input => { });
    }
}
