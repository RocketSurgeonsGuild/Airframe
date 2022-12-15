using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection.Tests.ComplexOptions;

internal class AncestorOptionsTestData : IEnumerable<object[]>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AncestorOptionsTestData"/> class.
    /// </summary>
    public AncestorOptionsTestData()
    {
        var serviceCollection = new ServiceCollection();
        _buildServiceProvider = serviceCollection
           .ConfigureAppSettings(builder => builder.AddJsonFile("ComplexOptions/multilevelsettings.json", optional: false))
           .ConfigureOptions<LevelSettings>()
           .BuildServiceProvider();
    }

    /// <inheritdoc/>
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { _buildServiceProvider };
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private readonly IServiceProvider _buildServiceProvider;
}