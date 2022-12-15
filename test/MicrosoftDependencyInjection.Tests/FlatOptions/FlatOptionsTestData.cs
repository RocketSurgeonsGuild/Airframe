using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection.Tests.FlatOptions;

internal class FlatOptionsTestData : TestClassData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlatOptionsTestData"/> class.
    /// </summary>
    public FlatOptionsTestData() => _buildServiceProvider = new ServiceCollection()
       .ConfigureAppSettings(builder => builder.AddJsonFile("FlatOptions/flatsettings.json", optional: false))
       .ConfigureOptions<FlatSettings>()
       .BuildServiceProvider();

    /// <inheritdoc/>
    protected override IEnumerator<object[]> Enumerator()
    {
        yield return new object[] { _buildServiceProvider };
    }

    private readonly IServiceProvider _buildServiceProvider;
}