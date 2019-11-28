using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using Rocket.Surgery.Airframe.Forms;
using Xamarin.Forms;

namespace Rocket.Surgery.Airframe.Benchmarks
{
    [CoreJob]
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub]
    public class ApplicationBaseBenchmark
    {
        /// <summary>
        /// Creates the reactive content page.
        /// </summary>
        /// <returns></returns>
        [Benchmark(Baseline = true)]
        public Application Application() => new Application();

        /// <summary>
        /// Creates the content page.
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public TestApplication TestApplication() => new TestApplication();


        /// <summary>
        /// Creates the test content page.
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public TestApplication TestApplicationWithConfigurator() => new TestApplication(locator => { });
    }
}
