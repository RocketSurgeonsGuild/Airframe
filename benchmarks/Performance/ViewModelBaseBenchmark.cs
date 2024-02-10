using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Rocket.Surgery.Airframe.Performance;

[SimpleJob(RuntimeMoniker.NetCoreApp31)]
[MemoryDiagnoser]
[MarkdownExporterAttribute.GitHub]
public class ViewModelBaseBenchmark
{
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
}