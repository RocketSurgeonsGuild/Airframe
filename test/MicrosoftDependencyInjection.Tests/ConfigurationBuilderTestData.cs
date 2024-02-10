using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection.Tests.ComplexOptions;
using System.Collections.Generic;

namespace Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection.Tests;

internal class ConfigurationBuilderTestData : TestClassData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationBuilderTestData"/> class.
    /// </summary>
    public ConfigurationBuilderTestData() => _buildServiceProvider = new ServiceCollection()
       .ConfigureBuilder(
            builder => builder.AddJsonFile("ComplexOptions/multilevelsettings.json", optional: false))
       .ConfigureOptions(option => option
           .ConfigureOption<LevelSettings>())
       .ConfigureAppSettings()
       .BuildServiceProvider();

    /// <inheritdoc/>
    protected override IEnumerator<object[]> Enumerator()
    {
        yield return new object[] { _buildServiceProvider };
    }

    private readonly ServiceProvider _buildServiceProvider;
}