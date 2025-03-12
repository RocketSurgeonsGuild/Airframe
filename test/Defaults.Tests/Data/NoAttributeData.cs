using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Rocket.Surgery.Airframe.Defaults.Tests.Data;

internal class NoAttributeData : DefaultsSourceData
{
    public static TheoryData<GeneratorTestContext> Data =>
    [
        DefaultBuilder()
           .AddSources(DefaultConstructor)
           .Build(),
    ];

    // lang=csharp
    private const string DefaultConstructor = """
                                              namespace Rocket.Surgery.Airframe.Defaults
                                              {
                                                  public partial class Thing
                                                  {
                                                      public Thing()
                                                      {
                                                      }
                                                  }
                                              }
                                              """;
}