// <copyright file="DefaultGeneratorTests+Diagnostics.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using FluentAssertions;
using Rocket.Surgery.Airframe.Defaults.Tests.Data;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Linq;
using System.Threading.Tasks;

namespace Rocket.Surgery.Airframe.Defaults.Tests;

public partial class DefaultsGeneratorTests
{
    [Theory]
    [MemberData(nameof(InaccessibleConstructorData.Data), MemberType = typeof(InaccessibleConstructorData))]
    public async Task GivenClassWithInaccessibleConstructor_WhenGenerate_ThenDiagnosticReported(GeneratorTestContext context)
    {
        // Given, When
        var result = await context.GenerateAsync();

        // Then
        result
           .Results
           .Should()
           .Contain(pair => pair.Value.Diagnostics.All(diagnostic => diagnostic.Id == Diagnostics.Defaults0001.Id));
    }
}