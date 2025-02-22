// <copyright file="SimpleReferenceTypeData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Rocket.Surgery.Airframe.Defaults.Tests;

internal class SimpleReferenceTypeData : DefaultsSourceData
{
    public static TheoryData<GeneratorTestContext> Data =>
    [
        DefaultBuilder()
           .AddSources(DefaultConstructor)
           .Build(),
        DefaultBuilder()
           .AddSources(PropertyNameConstructor)
           .Build(),
    ];

    // lang=csharp
    public const string DefaultConstructor = """
                                             using Rocket.Surgery.Airframe.Defaults;

                                             [ReferenceDefault]
                                             public partial class Thing;
                                             """;

    // lang=csharp
    public const string PropertyNameConstructor = """
                                                  using Rocket.Surgery.Airframe.Defaults;

                                                  [ReferenceDefault(PropertyName = "None"]
                                                  public partial class Thing;
                                                  """;
}