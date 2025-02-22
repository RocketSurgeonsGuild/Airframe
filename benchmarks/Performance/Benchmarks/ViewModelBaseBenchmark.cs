using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Rocket.Surgery.Airframe.Performance.Benchmarks;

/// <summary>
/// View Model Benchmarks.
/// </summary>
[SimpleJob(RuntimeMoniker.NetCoreApp31)]
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class ViewModelBaseBenchmark
{
    /// <summary>
    /// Benchmarks the instance creation.
    /// </summary>
    /// <returns>A test reactive object.</returns>
    [Benchmark(Baseline = true)]
    public TestReactiveObject CreateReactiveObject() => new TestReactiveObject();

    /// <summary>
    /// Benchmarks the instance creation.
    /// </summary>
    /// <returns>A test view model.</returns>
    [Benchmark]
    public TestViewModel CreateViewModel() => new TestViewModel();
}