// <copyright file="InaccessibleConstructorData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Rocket.Surgery.Airframe.Defaults.Tests.Data;

internal class InaccessibleConstructorData : DefaultsSourceData
{
    public static TheoryData<GeneratorTestContext> Data =>
    [
        DefaultBuilder()
           .AddSources(InternalConstructor)
           .Build(),
        DefaultBuilder()
           .AddSources(PrivateConstructor)
           .Build(),
        DefaultBuilder()
           .AddSources(ProtectedConstructor)
           .Build(),
    ];

    // lang=csharp
    public const string InternalConstructor = """
                                              namespace Rocket.Surgery.Airframe.Defaults
                                              {
                                                  [Defaults]
                                                  public partial class Thing
                                                  {
                                                      internal Thing()
                                                      {
                                                      }
                                                  }
                                              }
                                              """;

    // lang=csharp
    public const string PrivateConstructor = """
                                             namespace Rocket.Surgery.Airframe.Defaults
                                             {
                                                 [Defaults]
                                                 public partial class Thing
                                                 {
                                                     private Thing()
                                                     {
                                                     }
                                                 }
                                             }
                                             """;

    // lang=csharp
    public const string ProtectedConstructor = """
                                               namespace Rocket.Surgery.Airframe.Defaults
                                               {
                                                   [Defaults]
                                                   public partial class Thing
                                                   {
                                                       protected Thing()
                                                       {
                                                       }
                                                   }
                                               }
                                               """;
}