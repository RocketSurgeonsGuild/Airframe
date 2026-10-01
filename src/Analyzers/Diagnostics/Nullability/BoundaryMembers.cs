using System.Linq;
using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Nullability;

/// <summary>
/// Identifies members that sit on a public boundary and the declared shape of their return type.
/// Shared by RSA0001, RSA0002, RSA0003, RSA0006, and RSA0009.
/// </summary>
internal static class BoundaryMembers
{
    /// <summary>
    /// Determines whether <paramref name="symbol"/> is reachable from outside the declaring
    /// assembly: public, protected, or protected internal, all the way up its containing-type
    /// chain. A member's own accessibility only ever narrows what its containing type already
    /// allows - a <c>public</c> method on an <c>internal</c>, a <c>private</c>-nested, or a
    /// <c>file</c>-scoped type is not reachable from outside the assembly (a <c>file</c> type is
    /// narrower still: not even reachable from another file in the same assembly) no matter what
    /// the member itself declares, so every containing type is checked as well as the member.
    /// RSA0001, RSA0002, RSA0003, RSA0006, RSA0009.
    /// </summary>
    /// <param name="symbol">The method or property symbol.</param>
    /// <returns>A value indicating whether the symbol is publicly or protectedly visible.</returns>
    public static bool IsPubliclyVisible(ISymbol symbol)
    {
        if (!IsAccessibleAccessibility(symbol.DeclaredAccessibility))
        {
            return false;
        }

        for (var containingType = symbol.ContainingType; containingType is not null; containingType = containingType.ContainingType)
        {
            if (containingType.IsFileLocal || !IsAccessibleAccessibility(containingType.DeclaredAccessibility))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Determines whether <paramref name="symbol"/>'s signature is dictated by something other
    /// than its own declaration: an override, or an explicit interface implementation. Neither
    /// author chose the nullability of the contract they are fulfilling. RSA0001, RSA0002, RSA0003,
    /// RSA0006, RSA0009.
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
    /// Determines whether <paramref name="symbol"/> originates a return-type contract that an
    /// override or an implementation must match: an abstract member (including a
    /// <c>static abstract</c> interface member), an interface member with no default body (which
    /// Roslyn also reports as abstract), or a <c>virtual</c> member, including a default interface
    /// member with a body, since it too can be overridden and its nullability inherited. An
    /// override is deliberately not virtual by this definition - <see cref="IMethodSymbol.IsVirtual"/>
    /// and <see cref="IPropertySymbol.IsVirtual"/> are true only for a fresh, non-overriding
    /// declaration - so an override is excluded without a separate check. RSA0009.
    /// </summary>
    /// <param name="symbol">The method or property symbol.</param>
    /// <returns>A value indicating whether the symbol originates the contract.</returns>
    public static bool IsAbstractionOrigin(ISymbol symbol) => symbol switch
    {
        IMethodSymbol method => method.IsAbstract || method.IsVirtual,
        IPropertySymbol property => property.IsAbstract || property.IsVirtual,
        var _ => false
    };

    /// <summary>
    /// Determines whether <paramref name="property"/> is a bindable view-model member: its
    /// containing type implements <see cref="System.ComponentModel.INotifyPropertyChanged"/>
    /// (directly or through a base type, e.g. <c>ReactiveObject</c>), or the property itself
    /// carries an attribute named <c>Reactive</c> (unqualified, so any <c>[Reactive]</c> - the
    /// ReactiveUI.Fody and ReactiveUI.SourceGenerators one included - matches regardless of its
    /// namespace). Either is expected to be externally mutable by design: the binding
    /// infrastructure, not just the type's own constructor, is what assigns it. RSA0001.
    /// </summary>
    /// <param name="property">The property symbol.</param>
    /// <returns>A value indicating whether the property is exempt as a bindable member.</returns>
    public static bool IsBindableViewModelMember(IPropertySymbol property) =>
        property.ContainingType.AllInterfaces.Any(
            i => i.ToDisplayString() == "System.ComponentModel.INotifyPropertyChanged") ||
        property.GetAttributes().Any(attribute => attribute.AttributeClass?.Name == "ReactiveAttribute");

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

    /// <summary>
    /// Determines whether <paramref name="type"/> is <c>bool?</c> - <see cref="System.Nullable{T}"/>
    /// wrapping <see cref="bool"/>. RSA0006.
    /// </summary>
    /// <param name="type">The declared property type.</param>
    /// <returns>A value indicating whether the type is a nullable boolean.</returns>
    public static bool IsNullableBoolean(ITypeSymbol type) =>
        type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullableValueType &&
        nullableValueType.TypeArguments[0].SpecialType == SpecialType.System_Boolean;

    private static bool IsAccessibleAccessibility(Accessibility accessibility) => accessibility is
        Accessibility.Public or
        Accessibility.Protected or
        Accessibility.ProtectedOrInternal;

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
