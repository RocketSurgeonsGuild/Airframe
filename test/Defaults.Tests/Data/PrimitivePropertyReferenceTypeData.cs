// <copyright file="PrimitivePropertyReferenceTypeData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Rocket.Surgery.Airframe.Defaults.Tests.Data;

internal class PrimitivePropertyReferenceTypeData : DefaultsSourceData
{
    public static TheoryData<GeneratorTestContext> Data =>
    [
        DefaultBuilder()
           .AddSources(Constructor)
           .Build(),
        DefaultBuilder()
           .AddSources(ObjectInitialization)
           .Build(),
    ];

    // lang=csharp
    private const string Constructor = """
                                       namespace Rocket.Surgery.Airframe.Defaults
                                       {
                                           [DefaultsAttribute]
                                           public partial class Thing
                                           {
                                               public Thing(int one)
                                               {
                                                   One = one;
                                               }

                                               public int One { get; }
                                           }
                                       }
                                       """;

    // lang=csharp
    private const string ObjectInitialization = """
                                                namespace Rocket.Surgery.Airframe.Defaults
                                                {
                                                    [DefaultsAttribute]
                                                    public partial class Thing
                                                    {
                                                        public Thing()
                                                        {
                                                        }
    
                                                        public int One { get; set; }
                                                    }
                                                }
                                                """;
}