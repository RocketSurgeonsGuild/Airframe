// <copyright file="SimpleReferenceTypeData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Rocket.Surgery.Airframe.Defaults.Tests.Data;

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
                                  namespace Rocket.Surgery.Airframe.Defaults
                                  {
                                      [DefaultsAttribute]
                                      public partial class Thing
                                      {
                                          public Thing()
                                          {
                                          }
                                      }
                                  }
                                  """;

    // lang=csharp
    public const string PropertyNameConstructor = """
                                  namespace Rocket.Surgery.Airframe.Defaults
                                  {
                                      [DefaultsAttribute(PropertyName = "None"]
                                      public partial class Thing
                                      {
                                          public Thing()
                                          {
                                          }
                                      }
                                  }
                                  """;
}

public partial class Thing
{
    public Thing()
    {
    }
}