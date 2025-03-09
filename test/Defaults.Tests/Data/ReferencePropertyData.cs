// <copyright file="ReferencePropertyData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Rocket.Surgery.Airframe.Defaults.Tests.Data;

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
                                       
                                               public Stuff One { get; } = Stuff.Default;
                                           }
                                       }
                                       """;

    // lang=csharp
    private const string Stuff = """
                                 [DefaultsAttribute]
                                 public partial class Stuff
                                 {
                                     public static Stuff Default { get; } = new Stuff();
                                     
                                     public Junk Junk { get; }
                                 }
                                 """;

    // lang=csharp
    private const string Junk = """
                                [DefaultsAttribute]
                                public partial class Junk
                                {
                                    public static Junk Default { get; } = new Junk(default, Reference.Default);
                                }
                                """;

    // lang=csharp
    private const string MultipleConstructors = """
                                [DefaultsAttribute]
                                public partial class Junk
                                {
                                    [DefaultsConstructorAttribute]
                                    public Junk(int value);
                                    public Junk(int value, Reference reference);
                                    public static Junk Default { get; } = new Junk(default, Reference.Default);
                                    
                                [DefaultsEqualityAttribute]
                                    public int Value { get; init; }
                                    [DefaultsNotNullAttribute] // only if it's nullable.
                                    public Reference? Reference { get; internal set; }
                                [DefaultsEqualityAttribute]
                                    public bool IsValid { get; set; }
                                    public bool IsNotValid { get; private set; }
                                }
                                
                                //if (new Junk(0, Reference.Default) == Junk.Default)
                                //<ApplyDefaultsTo>MyNamespace.Junk;MyNamespace.Reference</ApplyDefaultsTo>
                                """;
}