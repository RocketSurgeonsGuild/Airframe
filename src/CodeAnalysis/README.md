# Rocket.Surgery.Airframe.CodeAnalysis

A set of analyzers and code fixes for common patterns found in Airframe based applications.

## Analyzers

- **RSA1XXX Usage** — MVVM patterns and ReactiveUI best practices
- **RSA2XXX Design** — member layout and file structure
- **RSA3XXX Performance** — subscription lifetime and allocation

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

Member layout and file structure. These rules describe a layout StyleCop cannot express, because
SA1201 hard-codes fields before constructors.

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

| Rule | Description | StyleCop equivalent |
|------|-------------|---------------------|
| RSA2001 | Constructors should appear before other members | none; SA1201 requires the opposite |
| RSA2002 | Private members should appear after non-private members | none |
| RSA2003 | Members should be ordered by kind | SA1201 |
| RSA2004 | Members should be ordered by access | SA1202 |
| RSA2005 | Static members should appear before instance members | SA1204 |
| RSA2006 | Constant fields should appear before non-constant fields | SA1203 |
| RSA2007 | Readonly fields should appear before mutable fields | SA1214 |

Each rule owns one component of the comparison, so a misplaced member reports against exactly one
rule and each concern gets its own severity.

**These rules never apply to interfaces or enumerations.** Their members declare no accessibility,
so ranking them by access would sort every member as though it were private and demand an order no
author could satisfy.

### File structure

| Rule | Description | StyleCop equivalent | Code fix |
|------|-------------|---------------------|----------|
| RSA2008 | File should contain a single type | SA1402 | none |
| RSA2009 | File name should match the first type name | SA1649 | none |
| RSA2010 | Do not use regions | SA1124 | yes |
| RSA2011 | Declare accessibility explicitly | SA1400 | yes |
| RSA2012 | Conditional compilation should not span member declarations | none | none |
| RSA2013 | Line exceeds the maximum length | none | yes |

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

| Construct | Result |
|-----------|--------|
| Parameter list | one parameter per line |
| Argument list | one argument per line |
| Collection expression and object initializer | one element per line |
| Expression body | break after the arrow |

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

The seven ordering rules share one code fix, because there is one sort. It supports Fix All, so
`dotnet format` will apply it:

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
