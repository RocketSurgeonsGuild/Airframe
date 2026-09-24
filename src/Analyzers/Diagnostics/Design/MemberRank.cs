using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

/// <summary>
/// The single source of truth for member ordering. Lower sorts first. Compared component by
/// component: constructor, bucket, kind, access, static, const, readonly.
///
/// Rules encoded:
///   1. Constructors and destructors always at the top, regardless of access.
///   2. Everything private goes to the bottom.
///   3. Within a bucket: fields, events, properties, indexers, methods, operators, nested types.
///   4. Within a kind: public, internal, protected internal, protected, private protected, private.
///   5. Static before instance.
///   6. Constant fields before non-constant fields.
///   7. Readonly fields before mutable fields.
///   8. Within the private bucket, fields sink below every other kind instead of leading it: a
///      private field is implementation detail backing the private members above it, not part of
///      a surface those members are declared in service of, so it reads last rather than first.
///
/// Both the analyzers and the reorder code fix compare through this type, so a reported violation
/// and the fix that resolves it can never disagree.
/// </summary>
internal readonly struct MemberRank : IComparable<MemberRank>
{
    private MemberRank(int constructor, int bucket, int kind, int access, int @static, int @const, int @readonly)
    {
        _constructor = constructor;
        _bucket = bucket;
        _kind = kind;
        _access = access;
        _static = @static;
        _const = @const;
        _readonly = @readonly;
    }

    /// <summary>
    /// Ranks a member declaration.
    /// </summary>
    /// <param name="member">The member to rank.</param>
    /// <returns>The rank.</returns>
    public static MemberRank Of(MemberDeclarationSyntax member)
    {
        var kind = KindOf(member);
        var access = AccessOf(member);
        var isConstructor = kind == KindConstructor || kind == KindDestructor;

        // Constructors never sit in the private bucket; everything else private does.
        var bucket = !isConstructor && access == AccessPrivate ? 1 : 0;

        var modifiers = member.Modifiers;
        var isConst = modifiers.Any(SyntaxKind.ConstKeyword);
        var isStatic = isConst || modifiers.Any(SyntaxKind.StaticKeyword);
        var isReadonly = isConst || modifiers.Any(SyntaxKind.ReadOnlyKeyword);
        var isField = kind == KindField;

        // A private field backs the private members declared above it; it is implementation
        // detail, not a kind those members share equal footing with, so it sinks below all of
        // them instead of leading the private bucket the way a non-private field leads its own.
        if (bucket == 1 && isField)
        {
            kind = KindPrivateField;
        }

        return new MemberRank(
            isConstructor ? 0 : 1,
            bucket,
            kind,
            access,
            isStatic ? 0 : 1,
            isField && isConst ? 0 : isField ? 1 : 0,
            isField && isReadonly ? 0 : isField ? 1 : 0);
    }

    /// <summary>
    /// Gets a display name for a member.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <returns>The name.</returns>
    public static string NameOf(MemberDeclarationSyntax member) => member switch
    {
        ConstructorDeclarationSyntax declaration => declaration.Identifier.Text + "()",
        DestructorDeclarationSyntax declaration => "~" + declaration.Identifier.Text + "()",
        FieldDeclarationSyntax declaration => declaration.Declaration.Variables.FirstOrDefault()?.Identifier.Text ?? "field",
        EventFieldDeclarationSyntax declaration => declaration.Declaration.Variables.FirstOrDefault()?.Identifier.Text ?? "event",
        EventDeclarationSyntax declaration => declaration.Identifier.Text,
        PropertyDeclarationSyntax declaration => declaration.Identifier.Text,
        IndexerDeclarationSyntax => "this[]",
        MethodDeclarationSyntax declaration => declaration.Identifier.Text + "()",
        OperatorDeclarationSyntax declaration => "operator " + declaration.OperatorToken.Text,
        ConversionOperatorDeclarationSyntax declaration => declaration.ImplicitOrExplicitKeyword.Text + " operator",
        DelegateDeclarationSyntax declaration => declaration.Identifier.Text,
        BaseTypeDeclarationSyntax declaration => declaration.Identifier.Text,
        _ => member.Kind().ToString()
    };

    /// <summary>
    /// Gets the location to squiggle: the identifier where one exists, otherwise the whole member.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <returns>The location.</returns>
    public static Location LocationOf(MemberDeclarationSyntax member)
    {
        SyntaxToken? token = member switch
        {
            ConstructorDeclarationSyntax declaration => declaration.Identifier,
            DestructorDeclarationSyntax declaration => declaration.Identifier,
            FieldDeclarationSyntax declaration => declaration.Declaration.Variables.FirstOrDefault()?.Identifier,
            EventFieldDeclarationSyntax declaration => declaration.Declaration.Variables.FirstOrDefault()?.Identifier,
            EventDeclarationSyntax declaration => declaration.Identifier,
            PropertyDeclarationSyntax declaration => declaration.Identifier,
            IndexerDeclarationSyntax declaration => declaration.ThisKeyword,
            MethodDeclarationSyntax declaration => declaration.Identifier,
            OperatorDeclarationSyntax declaration => declaration.OperatorToken,
            ConversionOperatorDeclarationSyntax declaration => declaration.ImplicitOrExplicitKeyword,
            DelegateDeclarationSyntax declaration => declaration.Identifier,
            BaseTypeDeclarationSyntax declaration => declaration.Identifier,
            _ => null
        };

        return token.HasValue ? token.Value.GetLocation() : member.GetLocation();
    }

    /// <inheritdoc/>
    public int CompareTo(MemberRank other)
    {
        int comparison;
        if ((comparison = _constructor.CompareTo(other._constructor)) != 0)
        {
            return comparison;
        }

        if ((comparison = _bucket.CompareTo(other._bucket)) != 0)
        {
            return comparison;
        }

        if ((comparison = _kind.CompareTo(other._kind)) != 0)
        {
            return comparison;
        }

        if ((comparison = _access.CompareTo(other._access)) != 0)
        {
            return comparison;
        }

        if ((comparison = _static.CompareTo(other._static)) != 0)
        {
            return comparison;
        }

        if ((comparison = _const.CompareTo(other._const)) != 0)
        {
            return comparison;
        }

        return _readonly.CompareTo(other._readonly);
    }

    /// <summary>
    /// Gets the first component on which this rank differs from <paramref name="other"/>. This is
    /// what attributes an ordering violation to exactly one diagnostic.
    /// </summary>
    /// <param name="other">The rank to compare against.</param>
    /// <returns>The first differing component.</returns>
    public RankComponent FirstDifference(MemberRank other)
    {
        if (_constructor != other._constructor)
        {
            return RankComponent.Constructor;
        }

        if (_bucket != other._bucket)
        {
            return RankComponent.Bucket;
        }

        if (_kind != other._kind)
        {
            return RankComponent.Kind;
        }

        if (_access != other._access)
        {
            return RankComponent.Access;
        }

        if (_static != other._static)
        {
            return RankComponent.Static;
        }

        if (_const != other._const)
        {
            return RankComponent.Const;
        }

        return _readonly != other._readonly ? RankComponent.Readonly : RankComponent.None;
    }

    /// <summary>
    /// Gets a human readable description used in diagnostic messages, such as
    /// "private static readonly field".
    /// </summary>
    /// <returns>The description.</returns>
    public string Describe()
    {
        var access = _access switch
        {
            AccessPublic => "public",
            AccessInternal => "internal",
            AccessProtectedInternal => "protected internal",
            AccessProtected => "protected",
            AccessPrivateProtected => "private protected",
            _ => "private"
        };

        var kind = _kind switch
        {
            KindConstructor => "constructor",
            KindDestructor => "destructor",
            KindField or KindPrivateField => "field",
            KindEvent => "event",
            KindProperty => "property",
            KindIndexer => "indexer",
            KindMethod => "method",
            KindOperator => "operator",
            KindDelegate => "delegate",
            KindNestedType => "nested type",
            _ => "member"
        };

        var isField = _kind is KindField or KindPrivateField;
        var description = access;
        if (isField && _const == 0)
        {
            // const implies static, so naming both would be noise.
            return description + " const " + kind;
        }

        if (_static == 0)
        {
            description += " static";
        }

        if (isField && _readonly == 0)
        {
            description += " readonly";
        }

        return description + " " + kind;
    }

    private static int KindOf(MemberDeclarationSyntax member) => member switch
    {
        ConstructorDeclarationSyntax => KindConstructor,
        DestructorDeclarationSyntax => KindDestructor,
        FieldDeclarationSyntax => KindField,
        EventFieldDeclarationSyntax => KindEvent,
        EventDeclarationSyntax => KindEvent,
        PropertyDeclarationSyntax => KindProperty,
        IndexerDeclarationSyntax => KindIndexer,
        MethodDeclarationSyntax => KindMethod,
        OperatorDeclarationSyntax => KindOperator,
        ConversionOperatorDeclarationSyntax => KindOperator,
        DelegateDeclarationSyntax => KindDelegate,
        BaseTypeDeclarationSyntax => KindNestedType,
        _ => KindOther
    };

    private static int AccessOf(MemberDeclarationSyntax member)
    {
        // Explicit interface implementations carry no modifier but are effectively public.
        if (HasExplicitInterfaceSpecifier(member))
        {
            return AccessPublic;
        }

        // A static constructor and a destructor cannot declare accessibility, so ranking them by
        // the C# default would bury them under members that merely chose to be public. Rank them
        // first and let the static component decide, which puts a static constructor ahead of the
        // instance constructors as intended.
        if (member is DestructorDeclarationSyntax ||
            (member is ConstructorDeclarationSyntax && member.Modifiers.Any(SyntaxKind.StaticKeyword)))
        {
            return AccessPublic;
        }

        var modifiers = member.Modifiers;
        var isPublic = modifiers.Any(SyntaxKind.PublicKeyword);
        var isInternal = modifiers.Any(SyntaxKind.InternalKeyword);
        var isProtected = modifiers.Any(SyntaxKind.ProtectedKeyword);
        var isPrivate = modifiers.Any(SyntaxKind.PrivateKeyword);

        if (isPublic)
        {
            return AccessPublic;
        }

        if (isProtected && isInternal)
        {
            return AccessProtectedInternal;
        }

        if (isPrivate && isProtected)
        {
            return AccessPrivateProtected;
        }

        if (isInternal)
        {
            return AccessInternal;
        }

        if (isProtected)
        {
            return AccessProtected;
        }

        // No modifier: members of classes, structs and records default to private.
        return AccessPrivate;
    }

    private static bool HasExplicitInterfaceSpecifier(MemberDeclarationSyntax member) => member switch
    {
        MethodDeclarationSyntax declaration => declaration.ExplicitInterfaceSpecifier != null,
        PropertyDeclarationSyntax declaration => declaration.ExplicitInterfaceSpecifier != null,
        IndexerDeclarationSyntax declaration => declaration.ExplicitInterfaceSpecifier != null,
        EventDeclarationSyntax declaration => declaration.ExplicitInterfaceSpecifier != null,
        _ => false
    };

    private const int KindConstructor = 0;
    private const int KindDestructor = 1;
    private const int KindField = 2;
    private const int KindEvent = 3;
    private const int KindProperty = 4;
    private const int KindIndexer = 5;
    private const int KindMethod = 6;
    private const int KindOperator = 7;
    private const int KindDelegate = 8;
    private const int KindNestedType = 9;
    private const int KindOther = 10;

    /// <summary>
    /// The display kind <see cref="Of"/> assigns a field declared in the private bucket, so it
    /// sorts below every other private kind instead of leading them the way a non-private field
    /// leads its own bucket. Deliberately greater than every other kind constant.
    /// </summary>
    private const int KindPrivateField = 11;

    private const int AccessPublic = 0;
    private const int AccessInternal = 1;
    private const int AccessProtectedInternal = 2;
    private const int AccessProtected = 3;
    private const int AccessPrivateProtected = 4;
    private const int AccessPrivate = 5;

    private readonly int _constructor;
    private readonly int _bucket;
    private readonly int _kind;
    private readonly int _access;
    private readonly int _static;
    private readonly int _const;
    private readonly int _readonly;
}
