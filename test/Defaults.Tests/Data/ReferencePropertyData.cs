// <copyright file="ReferencePropertyData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Diagnostics.CodeAnalysis;

namespace Rocket.Surgery.Airframe.Defaults.Tests.Data;

[SuppressMessage("Performance", "CA1823:Avoid unused private fields")]
internal class ReferencePropertyData : DefaultsSourceData
{
    public static TheoryData<GeneratorTestContext> Data =>
    [
        DefaultBuilder()
           .AddSources(Constructor, Stuff)
           .Build(),
    ];

    // lang=csharp
    private const string Constructor = """
                                       namespace Rocket.Surgery.Airframe.Defaults
                                       {
                                           [DefaultsAttribute]
                                           public partial class Thing
                                           {
                                               public Thing(Stuff one)
                                               {
                                                   One = one;
                                               }
                                       
                                               public Stuff One { get; }
                                           }
                                       }
                                       """;

    // lang=csharp
    private const string Stuff = """
                                 public class Stuff
                                 {
                                     public static Stuff Default { get; } = new Stuff();
                                 }
                                 """;
}