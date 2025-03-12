// <copyright file="AccessibleConstructorData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Rocket.Surgery.Airframe.Defaults.Tests.Data;

internal class AccessibleConstructorData : DefaultsSourceData
{
    public static TheoryData<GeneratorTestContext> Data =>
    [
        Rsda0001()
           .AddSources(InternalConstructor)
           .Build(),
        Rsda0001()
           .AddSources(PrivateConstructor)
           .Build(),
        Rsda0001()
           .AddSources(ProtectedConstructor)
           .Build(),
        Rsda0001()
           .AddSources(ProtectedAndPrivateConstructor)
           .Build(),
        Rsda0001()
           .AddSources(InternalAndProtectedAndPrivateConstructor)
           .Build(),
        Rsda0001()
           .AddSources(InternalAndProtectedConstructor)
           .Build(),
        Rsda0001()
           .AddSources(InternalAndPrivateConstructor)
           .Build(),
    ];

    // lang=csharp
    public const string InternalConstructor = """
                                              namespace Rocket.Surgery.Airframe.Defaults
                                              {
                                                  [Defaults]
                                                  public partial class Thing
                                                  {
                                                      public Thing()
                                                      {
                                                      }
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
                                                     public Thing()
                                                     {
                                                     }
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
                                                       public Thing()
                                                       {
                                                       }
                                                       protected Thing()
                                                       {
                                                       }
                                                   }
                                               }
                                               """;

    // lang=csharp
    public const string InternalAndProtectedAndPrivateConstructor = """
                                                                    namespace Rocket.Surgery.Airframe.Defaults
                                                                    {
                                                                        [Defaults]
                                                                        public partial class Thing
                                                                        {
                                                                            public Thing()
                                                                            {
                                                                            }
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
    public const string InternalAndProtectedConstructor = """
                                                          namespace Rocket.Surgery.Airframe.Defaults
                                                          {
                                                              [Defaults]
                                                              public partial class Thing
                                                              {
                                                                  public Thing()
                                                                  {
                                                                  }
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
    public const string InternalAndPrivateConstructor = """
                                                        namespace Rocket.Surgery.Airframe.Defaults
                                                        {
                                                            [Defaults]
                                                            public partial class Thing
                                                            {
                                                                public Thing()
                                                                {
                                                                }
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
    public const string ProtectedAndPrivateConstructor = """
                                                         namespace Rocket.Surgery.Airframe.Defaults
                                                         {
                                                             [Defaults]
                                                             public partial class Thing
                                                             {
                                                                 public Thing()
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
}