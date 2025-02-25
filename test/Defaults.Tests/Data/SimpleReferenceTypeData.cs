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
    private const string DefaultConstructor = """
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
    private const string PropertyNameConstructor = """
                                  namespace Rocket.Surgery.Airframe.Defaults
                                  {
                                      [DefaultsAttribute(PropertyName = "None")]
                                      public partial class Thing
                                      {
                                          public Thing()
                                          {
                                          }
                                      }
                                  }
                                  """;
}