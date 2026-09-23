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
}
