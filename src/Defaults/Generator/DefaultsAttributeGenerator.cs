using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Airframe.Defaults.Generator;

/// <summary>
/// This class generates the attributes required to opt in to defaults.
/// </summary>
[Generator]
internal class DefaultsAttributeGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        RegisterDefaultAttribute(context);

        void RegisterDefaultAttribute(IncrementalGeneratorInitializationContext incrementalContext) => incrementalContext.RegisterPostInitializationOutput(
            initializationContext => initializationContext.AddSource($"{nameof(DefaultsAttribute)}.g.cs", DefaultsAttribute.Source));
    }
}