# Rocket.Surgery.Airframe.CodeAnalysis

A set of analyzers and code fixes for common patterns found in Airframe based applications.

## Analyzers

- **RSA1XXX Usage** — MVVM patterns and ReactiveUI best practices
- **RSA2XXX Design** — member layout, file structure, and documentation
- **RSA3XXX Performance** — subscription lifetime and allocation

Every rule has its own reference page — cause, how to fix, when to suppress — under
[`docs/`](docs/README.md), styled after the [.NET compiler warning](https://github.com/dotnet/docs/blob/main/docs/csharp/language-reference/compiler-messages/cs0618.md)
and [style rule](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide0079)
docs.

## .editorconfig options

Every rule at its default severity, ready to paste into `.editorconfig` — delete the lines you
don't want to override:

```ini
[*.cs]
# RSA1XXX — Usage
dotnet_diagnostic.RSA1001.severity = warning      # Use expression lambda overload for a property
dotnet_diagnostic.RSA1002.severity = error        # Provide a well-formed lambda expression
dotnet_diagnostic.RSA1003.severity = error        # Use the out parameter overload
dotnet_diagnostic.RSA1004.severity = error        # Provide a well-formed lambda expression (missing member access prefix)
dotnet_diagnostic.RSA1005.severity = suggestion   # Specify a scheduler for better control over execution timing
dotnet_diagnostic.RSA1006.severity = suggestion   # SubscribeOn only supports a single use per pipeline
dotnet_diagnostic.RSA1007.severity = warning      # Use Invoke() instead of parentheses for function calls

# RSA2XXX — Design
dotnet_diagnostic.RSA2001.severity = warning      # Constructors should appear before other members
dotnet_diagnostic.RSA2002.severity = warning      # Private members should appear after non-private members
dotnet_diagnostic.RSA2003.severity = warning      # Members should be ordered by kind
dotnet_diagnostic.RSA2004.severity = warning      # Members should be ordered by access
dotnet_diagnostic.RSA2005.severity = warning      # Static members should appear before instance members
dotnet_diagnostic.RSA2006.severity = warning      # Constant fields should appear before non-constant fields
dotnet_diagnostic.RSA2007.severity = warning      # Readonly fields should appear before mutable fields
dotnet_diagnostic.RSA2008.severity = warning      # File should contain a single type
dotnet_diagnostic.RSA2009.severity = warning      # File name should match the first type name
dotnet_diagnostic.RSA2010.severity = warning      # Do not use regions
dotnet_diagnostic.RSA2011.severity = warning      # Declare accessibility explicitly
dotnet_diagnostic.RSA2012.severity = warning      # Conditional compilation should not span member declarations
dotnet_diagnostic.RSA2013.severity = warning      # Line exceeds the maximum length
dotnet_diagnostic.RSA2014.severity = warning      # Provide a summary for public abstractions
dotnet_diagnostic.RSA2015.severity = warning      # Provide <inheritdoc/> on members that implement or override an abstraction

# RSA3XXX — Performance
dotnet_diagnostic.RSA3001.severity = warning      # Subscription not disposed; consider DisposeWith
dotnet_diagnostic.RSA3002.severity = warning      # Lambda expression can be made static
dotnet_diagnostic.RSA3003.severity = suggestion   # Lambda expression captures state and allocates a closure
dotnet_diagnostic.RSA3004.severity = warning      # Provide an explicit scheduler for AutoRefresh
dotnet_diagnostic.RSA3005.severity = warning      # AutoRefresh already applied for this property

# RSA2013's line-length margin — see "RSA2013 and chop line"
max_line_length = 160
```

Valid severities: `error` | `warning` | `suggestion` | `silent` | `none`.

## Usage

This package is a development dependency and does not add any runtime dependencies to your project.

## Upgrading from 0.12.0 or earlier: rules that were silent now report

Every rule from RSA1001 to RSA3002 was previously inert. `DiagnosticAnalyzerAttribute` sat on the
abstract base classes, and Roslyn does not inherit that attribute when it discovers analyzers, so
the compiler loaded none of the concrete rules. RSA1007 was the only rule unaffected.

That is fixed, which means **upgrading turns on rules that have never reported before**. Across the
Airframe source tree the fix surfaces 109 RSA3002 and 6 RSA1007 diagnostics that were always true
and never reportable. Expect a comparable jump in your own build, and expect it to break the build
outright if you run with `TreatWarningsAsErrors`.

To stage the adoption, set the rules you are not ready for to `none` or `suggestion` in
`.editorconfig` and raise them as you clear them:

```ini
[*.cs]
dotnet_diagnostic.RSA3002.severity = suggestion
```

## RSA2XXX — Design

Member layout, file structure, and documentation. The layout rules describe an order StyleCop
cannot express, because SA1201 hard-codes fields before constructors.

### Member ordering

```
constructors / destructor            (any access; static constructor first)

non-private fields                   (public -> internal -> protected)
                                     (static -> instance)
                                     (const -> readonly -> mutable)
non-private events, properties, indexers, methods, operators, nested types

private fields
private events, properties, indexers, methods, nested types
```

| Rule    | Description                                              | StyleCop equivalent                |
|---------|----------------------------------------------------------|------------------------------------|
| RSA2001 | Constructors should appear before other members          | none; SA1201 requires the opposite |
| RSA2002 | Private members should appear after non-private members  | none                               |
| RSA2003 | Members should be ordered by kind                        | SA1201                             |
| RSA2004 | Members should be ordered by access                      | SA1202                             |
| RSA2005 | Static members should appear before instance members     | SA1204                             |
| RSA2006 | Constant fields should appear before non-constant fields | SA1203                             |
| RSA2007 | Readonly fields should appear before mutable fields      | SA1214                             |

Each rule owns one component of the comparison, so a misplaced member reports against exactly one
rule and each concern gets its own severity.

**These rules never apply to interfaces or enumerations.** Their members declare no accessibility,
so ranking them by access would sort every member as though it were private and demand an order no
author could satisfy.

### File structure

| Rule    | Description                                                 | StyleCop equivalent | Code fix |
|---------|-------------------------------------------------------------|---------------------|----------|
| RSA2008 | File should contain a single type                           | SA1402              | none     |
| RSA2009 | File name should match the first type name                  | SA1649              | none     |
| RSA2010 | Do not use regions                                          | SA1124              | yes      |
| RSA2011 | Declare accessibility explicitly                            | SA1400              | yes      |
| RSA2012 | Conditional compilation should not span member declarations | none                | none     |
| RSA2013 | Line exceeds the maximum length                             | none                | yes      |

RSA2008 accepts types that share a name, so `IListener` beside `IListener<T>` is not a violation.
RSA2009 accepts a generic arity or a partial suffix, so `Thing{T}.cs`, ``Thing`1.cs`` and
`Thing+Statics.cs` all name `Thing`.

RSA2012 is about conditional compilation that wraps whole member declarations:

```csharp
#if XAMARIN_IOS
    public void StartRangingBeacons(CLBeaconRegion region) => ...
#else
    public void StartRangingBeacons(CLBeaconIdentityConstraint constraint) => ...
#endif
```

A block like this hides half a type's surface from anyone reading one configuration, leaves the
layout rules unable to see the members it excludes, and stops the reorder, which cannot move a
member past it without changing which symbols compile it. Move the directive inside the members it
guards, or split the type across per-target files.

**Conditional compilation inside a member body is not reported**, because that is how multi
targeting is written:

```csharp
public int Start() =>
#if XAMARIN_IOS
    _manager.Value.StartMonitoring(region, accuracy);
#else
    _manager.Value.StartMonitoring(region);
#endif
```

### RSA2013 and chop line

RSA2013 reports any line past the margin. The limit comes from the `max_line_length` editorconfig
key, the same key Rider and ReSharper read, so the margin the editor draws and the margin the build
enforces are one number. **Where that key is absent, or set to `off`, the rule reports nothing**
rather than inventing a limit.

```ini
[*.cs]
max_line_length = 160
```

Its code fix is the equivalent of Rider's chop line. It breaks the outermost construct on the line
that can carry a break:

| Construct                                    | Result                 |
|----------------------------------------------|------------------------|
| Parameter list                               | one parameter per line |
| Argument list                                | one argument per line  |
| Collection expression and object initializer | one element per line   |
| Expression body                              | break after the arrow  |

```csharp
public static string Combine(string first, string second, string third, string fourth)
```

becomes

```csharp
public static string Combine(
    string first,
    string second,
    string third,
    string fourth)
```

Two things to know.

**Roslyn's formatter does not wrap.** It normalises whitespace it is given but has no notion of a
right margin, so `dotnet format` will never shorten a line by itself. This fix is that wrapping,
and it is why RSA2013 is worth having rather than leaving the margin to the formatter.

**One chop per pass.** A line long enough to need two breaks, such as a long signature whose body
is also long, shortens on the first pass and is reported again on the next, so repeated runs
converge. Nothing is offered for a line whose length is a string literal or a comment, because
there is no break to insert that would not change the text.

### Using the ordering fix as a formatter

The seven ordering rules share one code fix, because there is one sort. Applying it also sorts the
file's `using` directives — every regular using alphabetically, `using static` directives next,
alias directives last — since a using list left out of order is the same kind of layout drift the
member reorder already corrects. Whether a `System` namespace leads the regular group, and whether
a blank line separates that group from the rest, come from the standard
`dotnet_sort_system_directives_first` and `dotnet_separate_import_directive_groups` editorconfig
keys — the same two keys Visual Studio and Rider read for the same purpose:

```ini
[*.cs]
dotnet_sort_system_directives_first = true    # default; System.* leads the regular group
dotnet_separate_import_directive_groups = false   # default; no blank line between groups
```

The static-then-alias tail is not governed by either key and stays fixed regardless. It supports
Fix All, so `dotnet format` will apply it:

```bash
dotnet format analyzers --diagnostics RSA2001,RSA2002,RSA2003,RSA2004,RSA2005,RSA2006,RSA2007
```

Two things to know about it.

**The fix always applies the whole layout.** Turning off RSA2004 stops the diagnostic being
reported; it does not stop the fix ordering members by access when you invoke it for one of the
other six. Tune these rules by severity individually, but enable or disable them as a group.

**The fix declines when a directive's meaning depends on where it sits among the members.** That
covers a conditional block wrapping member declarations, which RSA2012 reports in its own right,
and a `#pragma warning disable` written between two members, which RSA2012 does not report but
which still starts suppressing from wherever it lands. In both cases the diagnostic stands and the
edit is left to you. A directive pair contained within one member, or a pragma inside a member
body, travels with that member and does not block the fix.

### Documentation

| Rule    | Description                                                                  | StyleCop equivalent    | Code fix |
|---------|------------------------------------------------------------------------------|------------------------|----------|
| RSA2014 | Provide a summary for public abstractions                                    | SA1600 (broader scope) | none     |
| RSA2015 | Provide `<inheritdoc/>` on members that implement or override an abstraction | none                   | none     |

RSA2014 reports on an interface or an abstract type declaration, and on every member either of them
declares without a body: an interface member with no default implementation — a `static abstract`
member (the generic-math pattern) included, since it has no body either — or a member marked
`abstract` in an abstract class. Each one is a contract other code is written against, so it needs
a `<summary>` a reader can act on without opening an implementation.

```csharp
/// <summary>Reads a value by key.</summary>
public interface IReader
{
    /// <summary>Gets the value for <paramref name="key"/>.</summary>
    string Read(string key);

    /// <summary>Parses <paramref name="value"/>.</summary>
    static abstract IReader Parse(string value);
}
```

**A default interface member and a concrete member of an abstract class are both excluded.**
Neither is part of the contract an implementer has to fulfil sight unseen, so RSA2014 leaves them
to whatever general documentation policy a project already runs. A default interface member stays
excluded whether or not it is `static`, since what excludes it is having a body, not being static.

RSA2015 reports on a member that implements an interface member or overrides an abstract member,
on whichever type directly declares it:

```csharp
public class Reader : IReader
{
    /// <inheritdoc/>
    public string Read(string key) => key;
}
```

**Inheriting an implementation from a base class does not itself require `<inheritdoc/>` at the
derived class** — only the type that actually writes the member out needs the tag. A member that
is itself `abstract`, re-declaring the contract rather than fulfilling it, is excluded; RSA2014
owns documenting that one instead. RSA2015 does not currently check an event declared with the
field-like `event Handler Changed;` shorthand; declare it with `event Handler Changed { add; remove; }`
if it implements an interface event and needs the check.

See [RSA2014](docs/RSA2014.md) and [RSA2015](docs/RSA2015.md) in the per-rule reference for the
full cause/fix/suppress writeup.

## Suppress a warning

Three ways to suppress an RSA diagnostic, in order of scope: attribute (one member), pragma (one
block), `.editorconfig` (everywhere). Prefer the narrowest scope that fits, and always justify it —
an unexplained suppression is indistinguishable from one nobody meant to keep.

### In source, with `SuppressMessageAttribute`

```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RSA2001:Constructors should appear before other members", Justification = "Generated by a source generator; the emitted order is not under our control.")]
public class GeneratedViewModel
{
    private readonly string _name;

    public GeneratedViewModel(string name) => _name = name;
}
```

`Category` is the RSA band (`"Design"` for RSA2XXX, `"Usage"` for RSA1XXX, `"Performance"` for
RSA3XXX). `CheckId` is `"<rule id>:<title>"` — the title after the colon is documentation only and
is not matched; Visual Studio's Quick Actions add it for you. Always fill in `Justification`: it is
the one thing that turns a suppression into a decision someone else can review, the same discipline
[CS0618](https://github.com/dotnet/docs/blob/main/docs/csharp/language-reference/compiler-messages/cs0618.md)
warnings call for when you must keep calling an `[Obsolete]` member.

### In source, with `#pragma warning`

```csharp
#pragma warning disable RSA2010 // Do not use regions
#region Legacy interop
    // ...
#endregion
#pragma warning restore RSA2010
```

Scope the `disable`/`restore` pair as tightly as the code that needs it — see
["The fix declines when a directive's meaning depends on where it sits among the members"](#using-the-ordering-fix-as-a-formatter)
above for how an unmatched pair interacts with the RSA2XXX ordering fix.

### Project-wide, with `.editorconfig`

```ini
[*.cs]
dotnet_diagnostic.RSA2001.severity = none
```

This is the same mechanism [IDE0079](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide0079)
governs: Roslyn reports IDE0079 when a `SuppressMessageAttribute` or `#pragma` suppresses a
diagnostic that is already `none` in `.editorconfig`, because the source-level suppression is then
dead weight — the rule was never going to fire. Turn a rule off at this level only when no file in
the project should ever report it; see [.editorconfig options](#editorconfig-options) above for
every rule at its default severity.

## Running alongside StyleCop

RSA2003 through RSA2011 reimplement nine StyleCop rules. If you use both packages you will get two
diagnostics for the same code. Adopting the RSA2XXX band means retiring its StyleCop counterparts:

```ini
[*.cs]
dotnet_diagnostic.SA1201.severity = none   # RSA2003
dotnet_diagnostic.SA1202.severity = none   # RSA2004
dotnet_diagnostic.SA1203.severity = none   # RSA2006
dotnet_diagnostic.SA1204.severity = none   # RSA2005
dotnet_diagnostic.SA1214.severity = none   # RSA2007
dotnet_diagnostic.SA1402.severity = none   # RSA2008
dotnet_diagnostic.SA1649.severity = none   # RSA2009
dotnet_diagnostic.SA1124.severity = none   # RSA2010
dotnet_diagnostic.SA1400.severity = none   # RSA2011
```

RSA2001 and RSA2002 have no StyleCop counterpart, and RSA2001 requires the opposite of SA1201, so
SA1201 must be off for the layout to be satisfiable at all.

This package does not replace StyleCop. It absorbs nine of its rules; the rest of StyleCop still
has a job.
