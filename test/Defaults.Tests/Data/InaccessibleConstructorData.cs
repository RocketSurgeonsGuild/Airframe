// <copyright file="InaccessibleConstructorData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Rocket.Surgery.Airframe.Defaults.Tests.Data;

internal class InaccessibleConstructorData : DefaultsSourceData
{
    public static TheoryData<GeneratorTestContext> Data =>
    [
        Rsad0001()
           .AddSources(InternalConstructor)
           .Build(),
        Rsad0001()
           .AddSources(PrivateConstructor)
           .Build(),
        Rsad0001()
           .AddSources(ProtectedConstructor)
           .Build(),
        Rsad0001()
           .AddSources(ProtectedAndPrivateConstructor)
           .Build(),
        Rsad0001()
           .AddSources(InternalAndProtectedAndPrivateConstructor)
           .Build(),
        Rsad0001()
           .AddSources(InternalAndProtectedConstructor)
           .Build(),
        Rsad0001()
           .AddSources(InternalAndPrivateConstructor)
           .Build(),
    ];

    // lang=csharp
    private const string InternalConstructor = """
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
    private const string PrivateConstructor = """
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
    private const string ProtectedConstructor = """
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

    // lang=csharp
    private const string InternalAndProtectedAndPrivateConstructor = """
                                                                     namespace Rocket.Surgery.Airframe.Defaults
                                                                     {
                                                                         [Defaults]
                                                                         public partial class Thing
                                                                         {
                                                                             internal Thing()
                                                                             {
                                                                             }
                                                                             protected Thing()
                                                                             {
                                                                             }
                                                                             private Thing()
                                                                             {
                                                                             }
                                                                         }
                                                                     }
                                                                     """;

    // lang=csharp
    private const string InternalAndProtectedConstructor = """
                                                           namespace Rocket.Surgery.Airframe.Defaults
                                                           {
                                                               [Defaults]
                                                               public partial class Thing
                                                               {
                                                                   internal Thing()
                                                                   {
                                                                   }
                                                                   protected Thing()
                                                                   {
                                                                   }
                                                                   private Thing()
                                                                   {
                                                                   }
                                                               }
                                                           }
                                                           """;

    // lang=csharp
    private const string InternalAndPrivateConstructor = """
                                                         namespace Rocket.Surgery.Airframe.Defaults
                                                         {
                                                             [Defaults]
                                                             public partial class Thing
                                                             {
                                                                 internal Thing()
                                                                 {
                                                                 }
                                                                 private Thing()
                                                                 {
                                                                 }
                                                             }
                                                         }
                                                         """;

    // lang=csharp
    private const string ProtectedAndPrivateConstructor = """
                                                          namespace Rocket.Surgery.Airframe.Defaults
                                                          {
                                                              [Defaults]
                                                              public partial class Thing
                                                              {
                                                                  protected Thing()
                                                                  {
                                                                  }
                                                                  private Thing()
                                                                  {
                                                                  }
                                                              }
                                                          }
                                                          """;
}