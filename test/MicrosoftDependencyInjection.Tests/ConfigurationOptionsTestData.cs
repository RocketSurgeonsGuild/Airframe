using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection.Tests.ComplexOptions;
using System.Collections.Generic;

namespace Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection.Tests;

internal class ConfigurationOptionsTestData : TestClassData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationOptionsTestData"/> class.
    /// </summary>
    public ConfigurationOptionsTestData() => _buildServiceProvider = new ServiceCollection()
       .ConfigureSettings(
            builder => builder.AddJsonFile("ComplexOptions/multilevelsettings.json", optional: false),
            option => option
               .ConfigureOption<LevelSettings>())
       .BuildServiceProvider();

    /// <inheritdoc/>
    protected override IEnumerator<object[]> Enumerator()
    {
        yield return new object[] { _buildServiceProvider };
    }

    private readonly ServiceProvider _buildServiceProvider;
}