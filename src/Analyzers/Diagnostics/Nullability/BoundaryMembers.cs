using System.Linq;
using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;

/// <summary>
/// Identifies members that sit on a public boundary and the declared shape of their return type.
/// Shared by RSA0002 and RSA0003.
/// </summary>
internal static class BoundaryMembers
{
    /// <summary>
    /// Determines whether <paramref name="symbol"/> is reachable from outside its declaring type:
    /// public, protected, or protected internal. RSA0002, RSA0003.
    /// </summary>
    /// <param name="symbol">The method or property symbol.</param>
    /// <returns>A value indicating whether the symbol is publicly or protectedly visible.</returns>
    public static bool IsPubliclyVisible(ISymbol symbol) =>
        symbol.DeclaredAccessibility is
            Accessibility.Public or
            Accessibility.Protected or
            Accessibility.ProtectedOrInternal or
            Accessibility.ProtectedAndInternal;

    /// <summary>
    /// Determines whether <paramref name="symbol"/>'s signature is dictated by something other
    /// than its own declaration: an override, or an explicit interface implementation. Neither
    /// author chose the nullability of the contract they are fulfilling. RSA0002, RSA0003.
    /// </summary>
    /// <param name="symbol">The method or property symbol.</param>
    /// <returns>A value indicating whether the contract is inherited rather than authored.</returns>
    public static bool IsInheritedContract(ISymbol symbol) => symbol switch
    {
        IMethodSymbol method => method.IsOverride || method.ExplicitInterfaceImplementations.Length > 0,
        IPropertySymbol property => property.IsOverride || property.ExplicitInterfaceImplementations.Length > 0,
        var _ => false
    };

    /// <summary>
    /// Determines whether <paramref name="type"/> is a nullable-annotated collection type: an
    /// array, or a type assignable to the non-generic <see cref="System.Collections.IEnumerable"/>,
    /// excluding <see cref="string"/> itself. RSA0002.
    /// </summary>
    /// <param name="type">The declared return or property type.</param>
    /// <returns>A value indicating whether the type is a nullable collection.</returns>
    public static bool IsNullableCollectionType(ITypeSymbol type) =>
        type.NullableAnnotation == NullableAnnotation.Annotated && IsCollectionType(type);

    /// <summary>
    /// Determines whether <paramref name="type"/> is <c>Task</c>, <c>Task&lt;T&gt;</c>,
    /// <c>ValueTask</c>, or <c>ValueTask&lt;T&gt;</c> from <c>System.Threading.Tasks</c>, itself
    /// nullable. <c>Task</c> and <c>Task&lt;T&gt;</c> are classes, so their nullable form is a
    /// <see cref="NullableAnnotation.Annotated"/> reference type; <c>ValueTask</c> and
    /// <c>ValueTask&lt;T&gt;</c> are structs, so their nullable form is
    /// <see cref="System.Nullable{T}"/> instead and is unwrapped before the same name check. A
    /// nullable type argument, e.g. <c>Task&lt;T?&gt;</c>, is a nullable result and not a nullable
    /// task, and does not match. RSA0003.
    /// </summary>
    /// <param name="type">The declared return type.</param>
    /// <param name="taskName">The unqualified name of the task type when matched.</param>
    /// <returns>A value indicating whether the type is a nullable task.</returns>
    public static bool IsNullableTaskType(ITypeSymbol type, out string taskName)
    {
        if (type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullableValueType)
        {
            return IsTaskNamedType(nullableValueType.TypeArguments[0], out taskName);
        }

        taskName = type.Name;
        return type.NullableAnnotation == NullableAnnotation.Annotated && IsTaskNamedType(type, out taskName);
    }

    private static bool IsTaskNamedType(ITypeSymbol type, out string taskName)
    {
        taskName = type.Name;

        if (type is not INamedTypeSymbol { ContainingNamespace: { } containingNamespace } namedType ||
            containingNamespace.ToDisplayString() != "System.Threading.Tasks")
        {
            return false;
        }

        taskName = namedType.Name;
        return namedType.Name is "Task" or "ValueTask";
    }

    private static bool IsCollectionType(ITypeSymbol type)
    {
        if (type.SpecialType == SpecialType.System_String)
        {
            return false;
        }

        if (type.TypeKind == TypeKind.Array)
        {
            return true;
        }

        if (IsEnumerableSpecialType(type.OriginalDefinition.SpecialType))
        {
            return true;
        }

        return type.AllInterfaces.Any(i => IsEnumerableSpecialType(i.OriginalDefinition.SpecialType));
    }

    private static bool IsEnumerableSpecialType(SpecialType specialType) => specialType is
        SpecialType.System_Collections_IEnumerable or
        SpecialType.System_Collections_Generic_IEnumerable_T or
        SpecialType.System_Collections_Generic_ICollection_T or
        SpecialType.System_Collections_Generic_IList_T or
        SpecialType.System_Collections_Generic_IReadOnlyCollection_T or
        SpecialType.System_Collections_Generic_IReadOnlyList_T;
}
