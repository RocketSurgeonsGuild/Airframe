using Rocket.Surgery.Airframe.Defaults.Diagnostics;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using System.Diagnostics.CodeAnalysis;

namespace Rocket.Surgery.Airframe.Defaults.Tests.Data;

[SuppressMessage("Performance", "CA1823:Avoid unused private fields")]
public class MultipleConstructorData
{
    public static TheoryData<GeneratorTestContext> Data =>
    [
        GeneratorTestContextBuilder
           .Create()
           .WithGenerator<DefaultsGenerator>()
           .WithGenerator<MultipleConstructorDiagnostic>()
           .AddSources(MultipleConstructorsWithAttribute)
           .Build(),

        // GeneratorTestContextBuilder
        //    .Create()
        //    .WithGenerator<MultipleConstructorDiagnostic>()
        //    .AddSources(MultipleConstructorsWithoutAttribute)
        //    .Build()
    ];

    // lang=csharp
    private const string MultipleConstructorsWithAttribute = """
                                                             [DefaultsAttribute]
                                                             public partial class Junk
                                                             {
                                                                 public Junk(int value)
                                                                 {
                                                                 }
                                                                 public Junk(int value, Reference reference)
                                                                 {
                                                                     Value = value;
                                                                     Reference = reference;
                                                                 }
                                                                 public int Value { get; init; }
                                                                 public Reference? Reference { get; internal set; }
                                                                 public bool IsValid { get; set; }
                                                                 public bool IsNotValid { get; private set; }
                                                             }
                                                             """;

    // lang=csharp
    private const string MultipleConstructorsWithoutAttribute = """
                                                                public partial class Junk
                                                                {
                                                                    public Junk(int value)
                                                                    {
                                                                    }
                                                                    public Junk(int value, Reference reference)
                                                                    {
                                                                        Value = value;
                                                                        Reference = reference;
                                                                    }
                                                                    public int Value { get; init; }
                                                                    public Reference? Reference { get; internal set; }
                                                                    public bool IsValid { get; set; }
                                                                    public bool IsNotValid { get; private set; }
                                                                }
                                                                """;
}