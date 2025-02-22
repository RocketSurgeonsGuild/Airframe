using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Airframe.Defaults;

[Generator]
public class DefaultsGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        RegisterDefaultAttribute(context);

        void RegisterDefaultAttribute(
            IncrementalGeneratorInitializationContext incrementalContext) => incrementalContext.RegisterPostInitializationOutput(
            initializationContext => initializationContext.AddSource(
                "DefaultAttribute.g.cs",
                DefaultAttribute.Source()));
    }
}