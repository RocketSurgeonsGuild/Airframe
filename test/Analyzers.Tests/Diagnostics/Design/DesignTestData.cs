namespace Rocket.Surgery.Airframe.Analyzers.Tests.Diagnostics.Design;

/// <summary>
/// Sources shared by the RSA2XXX tests. Each ordering sample violates exactly one rank component,
/// which is what lets every ordering rule assert that the other six stay silent on it.
/// </summary>
internal static class DesignTestData
{
    // lang=csharp
    public const string Correct =
        """
        namespace Sample
        {
            public class Example
            {
                static Example()
                {
                }

                public Example()
                {
                }

                private Example(int value)
                {
                }

                public const int Constant = 1;
                public static readonly int StaticReadonly = 2;
                public static int Static = 3;
                public readonly int Readonly = 4;
                public int Mutable = 5;
                internal int Internal = 6;
                protected int Protected = 7;

                public int Property { get; set; }

                internal int InternalProperty { get; set; }

                public static void StaticMethod()
                {
                }

                public void Method()
                {
                }

                protected virtual void Hook()
                {
                }

                private const int PrivateConstant = 8;
                private static readonly int PrivateStaticReadonly = 9;
                private static int _privateStatic = 10;
                private readonly int _privateReadonly = 11;
                private int _privateMutable = 12;

                private int PrivateProperty { get; set; }

                private void Helper()
                {
                }
            }
        }
        """;

    // lang=csharp
    public const string ConstructorAfterField =
        """
        namespace Sample
        {
            public class Example
            {
                private readonly int _value;

                public Example()
                {
                }
            }
        }
        """;

    // lang=csharp
    public const string PrivateBeforeNonPrivate =
        """
        namespace Sample
        {
            public class Example
            {
                private void Helper()
                {
                }

                public int Property { get; set; }
            }
        }
        """;

    // lang=csharp
    public const string MethodBeforeProperty =
        """
        namespace Sample
        {
            public class Example
            {
                public void Method()
                {
                }

                public int Property { get; set; }
            }
        }
        """;

    // lang=csharp
    public const string InternalBeforePublic =
        """
        namespace Sample
        {
            public class Example
            {
                internal int Internal = 1;
                public int Public = 2;
            }
        }
        """;

    // lang=csharp
    public const string InstanceBeforeStatic =
        """
        namespace Sample
        {
            public class Example
            {
                public void Instance()
                {
                }

                public static void Static()
                {
                }
            }
        }
        """;

    // lang=csharp
    public const string FieldBeforeConstant =
        """
        namespace Sample
        {
            public class Example
            {
                private static int _value = 1;
                private const int Constant = 2;
            }
        }
        """;

    // lang=csharp
    public const string MutableBeforeReadonly =
        """
        namespace Sample
        {
            public class Example
            {
                private int _mutable;
                private readonly int _readonly;
            }
        }
        """;

    // lang=csharp
    public const string MultipleTypes =
        """
        namespace Sample
        {
            public class First
            {
            }

            public class Second
            {
            }
        }
        """;

    // lang=csharp
    public const string Regions =
        """
        namespace Sample
        {
            public class Example
            {
                #region Helpers
                private void Helper()
                {
                }
                #endregion
            }
        }
        """;

    // lang=csharp
    public const string ImplicitAccessibility =
        """
        namespace Sample
        {
            class Example
            {
                void Method()
                {
                }
            }
        }
        """;

    /// <summary>
    /// The harness names the sources it compiles <c>Input0.cs</c>, so a type named Input0 is what
    /// a matching file name looks like from inside a test.
    /// </summary>
    // lang=csharp
    public const string FileNameMatchesType =
        """
        namespace Sample
        {
            public class Input0
            {
            }
        }
        """;

    // lang=csharp
    public const string FileNameDoesNotMatchType =
        """
        namespace Sample
        {
            public class Mismatched
            {
            }
        }
        """;

    /// <summary>
    /// Generic arity siblings are one concept, so sharing a file is the convention rather than a
    /// violation.
    /// </summary>
    // lang=csharp
    public const string GenericSiblings =
        """
        namespace Sample
        {
            public interface Input0 : Input0<int>
            {
            }

            public interface Input0<out T>
            {
            }
        }
        """;

    /// <summary>
    /// A partial type splits across files with a suffix, so Input0+Statics.cs still names Input0.
    /// </summary>
    // lang=csharp
    public const string PartialContinuation =
        """
        namespace Sample
        {
            public partial class Input0
            {
            }
        }
        """;

    /// <summary>
    /// Shapes other than a flat class. Each is correctly ordered, so no ordering rule may fire on
    /// any of them.
    /// </summary>
    // lang=csharp
    public const string CorrectRecord =
        """
        namespace Sample
        {
            public record Example
            {
                public Example(int value) => Value = value;

                public int Value { get; }

                public void Method()
                {
                }

                private int _cache;
            }
        }
        """;

    // lang=csharp
    public const string CorrectStruct =
        """
        namespace Sample
        {
            public struct Example
            {
                public Example(int value) => _value = value;

                public static readonly int Shared = 1;

                public int Value => _value;

                private readonly int _value;
            }
        }
        """;

    // lang=csharp
    public const string CorrectGeneric =
        """
        namespace Sample
        {
            public class Example<T>
            {
                public Example(T value) => _value = value;

                public T Value => _value;

                private readonly T _value;
            }
        }
        """;

    // lang=csharp
    public const string CorrectNestedType =
        """
        namespace Sample
        {
            public class Example
            {
                public Example()
                {
                }

                public int Value { get; set; }

                public class Nested
                {
                    public Nested()
                    {
                    }

                    public int NestedValue { get; set; }

                    private int _nested;
                }

                private int _value;
            }
        }
        """;

    // lang=csharp
    public const string CorrectPartial =
        """
        namespace Sample
        {
            public partial class Example
            {
                public Example()
                {
                }

                public int Value { get; set; }

                private int _value;
            }
        }
        """;

    // lang=csharp
    public const string CorrectAttributedMembers =
        """
        using System;

        namespace Sample
        {
            public class Example
            {
                public Example()
                {
                }

                [Obsolete("Use Value instead.")]
                public int Legacy { get; set; }

                public int Value { get; set; }

                [Obsolete("Internal.")]
                private int _value;
            }
        }
        """;

    /// <summary>
    /// An interface whose members are in no particular order. RSA2001 through RSA2007 never apply
    /// to interfaces, because interface members declare no accessibility.
    /// </summary>
    // lang=csharp
    public const string UnorderedInterface =
        """
        namespace Sample
        {
            public interface IExample
            {
                void Method();

                int Property { get; set; }
            }
        }
        """;

    /// <summary>
    /// A nested type declared before the members it sits beside, which RSA2003 owns.
    /// </summary>
    // lang=csharp
    public const string NestedTypeBeforeMethod =
        """
        namespace Sample
        {
            public class Example
            {
                public class Nested
                {
                }

                public void Method()
                {
                }
            }
        }
        """;

    /// <summary>
    /// An explicit interface implementation, which declares no accessibility modifier of its own
    /// but ranks as public. <c>MemberRank.AccessOf</c>'s explicit-interface branch is otherwise
    /// never reached, since every other sample's members declare their accessibility directly.
    /// </summary>
    // lang=csharp
    public const string CorrectExplicitInterfaceImplementation =
        """
        using System;

        namespace Sample
        {
            public class Example : IDisposable
            {
                public Example()
                {
                }

                void IDisposable.Dispose()
                {
                }
            }
        }
        """;

    /// <summary>
    /// Gets the shapes every ordering rule must stay silent on.
    /// </summary>
    public static readonly string[] CorrectShapes =
    [
        Correct,
        CorrectRecord,
        CorrectStruct,
        CorrectGeneric,
        CorrectNestedType,
        CorrectPartial,
        CorrectAttributedMembers,
        UnorderedInterface,
        CorrectExplicitInterfaceImplementation
    ];

    /// <summary>
    /// A protected field declared before a public field, which RSA2004 owns. <c>Correct</c>
    /// declares a protected member, but <c>MemberRank.Describe</c> only stringifies access for
    /// members caught in a violation, so its "protected" arm is otherwise unreached.
    /// </summary>
    // lang=csharp
    public const string ProtectedBeforePublic =
        """
        namespace Sample
        {
            public class Example
            {
                protected int Protected = 1;
                public int Public = 2;
            }
        }
        """;

    /// <summary>
    /// A protected internal field declared before a public field, which RSA2004 owns. Covers the
    /// protected-and-internal branch of <c>MemberRank.AccessOf</c> and Describe's matching arm.
    /// </summary>
    // lang=csharp
    public const string ProtectedInternalBeforePublic =
        """
        namespace Sample
        {
            public class Example
            {
                protected internal int ProtectedInternal = 1;
                public int Public = 2;
            }
        }
        """;

    /// <summary>
    /// A private protected field declared before a public field, which RSA2004 owns. Covers the
    /// private-and-protected branch of <c>MemberRank.AccessOf</c> and Describe's matching arm.
    /// </summary>
    // lang=csharp
    public const string PrivateProtectedBeforePublic =
        """
        namespace Sample
        {
            public class Example
            {
                private protected int PrivateProtected = 1;
                public int Public = 2;
            }
        }
        """;

    /// <summary>
    /// A conditional block wrapping whole member declarations. RSA2012 owns this.
    /// </summary>
    // lang=csharp
    public const string DirectiveSpanningMembers =
        """
        namespace Sample
        {
            public class Example
            {
        #if XAMARIN_IOS
                public void Start(int region)
                {
                }
        #else
                public void Start(string region)
                {
                }
        #endif
            }
        }
        """;

    /// <summary>
    /// A pragma pair wrapping fields. It straddles members, so the reorder declines across it, but
    /// it is not conditional compilation and RSA2012 leaves it alone.
    /// </summary>
    // lang=csharp
    public const string PragmaSpanningMembers =
        """
        namespace Sample
        {
            public class Example
            {
                public Example()
                {
                }

        #pragma warning disable CA2213
                private int _first;
                private int _second;
        #pragma warning restore CA2213
            }
        }
        """;

    /// <summary>
    /// A conditional block inside one member body, which is how multi targeting is written and is
    /// deliberately allowed.
    /// </summary>
    // lang=csharp
    public const string DirectiveWithinMemberBody =
        """
        namespace Sample
        {
            public class Example
            {
                public int Start()
                {
        #if XAMARIN_IOS
                    return 1;
        #else
                    return 2;
        #endif
                }
            }
        }
        """;

    /// <summary>
    /// Gets every ordering sample. A rule under test asserts it stays silent on the six that
    /// belong to its siblings.
    /// </summary>
    public static readonly string[] Ordering =
    [
        ConstructorAfterField,
        PrivateBeforeNonPrivate,
        MethodBeforeProperty,
        InternalBeforePublic,
        InstanceBeforeStatic,
        FieldBeforeConstant,
        MutableBeforeReadonly
    ];

    /// <summary>
    /// A destructor declared after a field, which RSA2001 owns just as it does a misplaced
    /// constructor. <c>MemberRank.NameOf</c> and <c>LocationOf</c> switch on the concrete
    /// declaration syntax, and only <see cref="Ordering"/>'s constructor, field, property and
    /// method shapes are otherwise exercised, so a destructor never reaches either switch without
    /// this sample.
    /// </summary>
    // lang=csharp
    public const string FieldBeforeDestructor =
        """
        namespace Sample
        {
            public class Example
            {
                private readonly int _value;

                ~Example()
                {
                }
            }
        }
        """;

    /// <summary>
    /// An event field declared after a property, which RSA2003 owns. Covers
    /// <c>EventFieldDeclarationSyntax</c> in <c>MemberRank.NameOf</c>/<c>LocationOf</c>.
    /// </summary>
    // lang=csharp
    public const string PropertyBeforeEventField =
        """
        using System;

        namespace Sample
        {
            public class Example
            {
                public int Value { get; set; }

                public event EventHandler Changed;
            }
        }
        """;

    /// <summary>
    /// A custom add/remove event declared after a property, which RSA2003 owns. Covers
    /// <c>EventDeclarationSyntax</c>, distinct from the event field shape above.
    /// </summary>
    // lang=csharp
    public const string PropertyBeforeEvent =
        """
        using System;

        namespace Sample
        {
            public class Example
            {
                public int Value { get; set; }

                public event EventHandler Changed
                {
                    add { }
                    remove { }
                }
            }
        }
        """;

    /// <summary>
    /// An indexer declared after a method, which RSA2003 owns. Covers
    /// <c>IndexerDeclarationSyntax</c>.
    /// </summary>
    // lang=csharp
    public const string MethodBeforeIndexer =
        """
        namespace Sample
        {
            public class Example
            {
                public void Method()
                {
                }

                public int this[int index] => index;
            }
        }
        """;

    /// <summary>
    /// An operator overload declared after a delegate, which RSA2003 owns. Covers
    /// <c>OperatorDeclarationSyntax</c>.
    /// </summary>
    // lang=csharp
    public const string DelegateBeforeOperator =
        """
        namespace Sample
        {
            public class Example
            {
                public delegate void Handler();

                public static Example operator +(Example left, Example right) => left;
            }
        }
        """;

    /// <summary>
    /// A conversion operator declared after a delegate, which RSA2003 owns. Covers
    /// <c>ConversionOperatorDeclarationSyntax</c>, distinct from the operator shape above even
    /// though both rank as the same member kind.
    /// </summary>
    // lang=csharp
    public const string DelegateBeforeConversionOperator =
        """
        namespace Sample
        {
            public class Example
            {
                public delegate void Handler();

                public static implicit operator int(Example value) => 0;
            }
        }
        """;

    /// <summary>
    /// A delegate declared after a nested type, which RSA2003 owns. <see cref="DelegateBeforeOperator"/>
    /// and <see cref="DelegateBeforeConversionOperator"/> only ever place a delegate as the earlier,
    /// correctly-ranked member, so neither reaches the <c>DelegateDeclarationSyntax</c> arm of
    /// <c>MemberRank.LocationOf</c>; this sample puts the delegate on the violating side instead.
    /// </summary>
    // lang=csharp
    public const string NestedTypeBeforeDelegate =
        """
        namespace Sample
        {
            public class Example
            {
                public class Nested
                {
                }

                public delegate void Handler();
            }
        }
        """;
}
