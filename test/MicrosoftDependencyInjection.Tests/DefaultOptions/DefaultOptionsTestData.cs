using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection.Tests.DefaultOptions;

internal class DefaultOptionsTestData : IEnumerable<object[]>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultOptionsTestData"/> class.
    /// </summary>
    public DefaultOptionsTestData()
    {
        var serviceCollection = new ServiceCollection();
        _buildServiceProvider = serviceCollection
           .ConfigureAppSettings(builder => builder.AddJsonFile("DefaultOptions/defaultoptions.json", optional: false))
           .ConfigureSectionAsOptions<TransientFaultHandlingOptions>()
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