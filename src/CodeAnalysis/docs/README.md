# RSA analyzer rules

One page per rule, in the style of the [.NET compiler warning](https://github.com/dotnet/docs/blob/main/docs/csharp/language-reference/compiler-messages/cs0618.md)
and [style rule](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide0079)
docs: cause, rule description, how to fix, when to suppress, and the suppression syntax itself. For
severities, `.editorconfig` defaults, and the broader guidance these pages link back to, see the
[package README](../README.md).

## RSA1XXX — Usage

MVVM patterns and ReactiveUI best practices.

| Rule | Title | Default severity |
|------|-------|-------------------|
| [RSA1001](RSA1001.md) | Use expression lambda overload for `InvokeCommand` | Warning |
| [RSA1002](RSA1002.md) | Provide a well-formed lambda expression for `BindTo` | Error |
| [RSA1003](RSA1003.md) | Use the `out` parameter overload of `ToProperty` | Error |
| [RSA1004](RSA1004.md) | Provide a well-formed lambda expression for `WhenAnyValue` | Error |
| [RSA1005](RSA1005.md) | Specify a scheduler for better control over execution timing | Suggestion |
| [RSA1006](RSA1006.md) | `SubscribeOn` only supports a single use per pipeline | Suggestion |
| [RSA1007](RSA1007.md) | Use `Invoke()` instead of parentheses for function calls | Warning |

## RSA2XXX — Design

Member layout and file structure.

| Rule | Title | Default severity |
|------|-------|-------------------|
| [RSA2001](RSA2001.md) | Constructors should appear before other members | Warning |
| [RSA2002](RSA2002.md) | Private members should appear after non-private members | Warning |
| [RSA2003](RSA2003.md) | Members should be ordered by kind | Warning |
| [RSA2004](RSA2004.md) | Members should be ordered by access | Warning |
| [RSA2005](RSA2005.md) | Static members should appear before instance members | Warning |
| [RSA2006](RSA2006.md) | Constant fields should appear before non-constant fields | Warning |
| [RSA2007](RSA2007.md) | Readonly fields should appear before mutable fields | Warning |
| [RSA2008](RSA2008.md) | File should contain a single type | Warning |
| [RSA2009](RSA2009.md) | File name should match the first type name | Warning |
| [RSA2010](RSA2010.md) | Do not use regions | Warning |
| [RSA2011](RSA2011.md) | Declare accessibility explicitly | Warning |
| [RSA2012](RSA2012.md) | Conditional compilation should not span member declarations | Warning |
| [RSA2013](RSA2013.md) | Line exceeds the maximum length | Warning |

## RSA3XXX — Performance

Subscription lifetime and allocation.

| Rule | Title | Default severity |
|------|-------|-------------------|
| [RSA3001](RSA3001.md) | Subscription not disposed | Warning |
| [RSA3002](RSA3002.md) | Lambda expression can be made static | Warning |
