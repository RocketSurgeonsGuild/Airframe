namespace Rocket.Surgery.Airframe.Analyzers.Diagnostics.Design;

/// <summary>
/// Identifies the component of a <see cref="MemberRank"/> comparison that produced a difference.
/// Each RSA2001-RSA2007 diagnostic owns exactly one component, so a single misordered member is
/// attributed to exactly one rule.
/// </summary>
internal enum RankComponent
{
    /// <summary>The ranks are equal.</summary>
    None,

    /// <summary>Constructor or destructor placement. RSA2001.</summary>
    Constructor,

    /// <summary>Non-private versus private grouping. RSA2002.</summary>
    Bucket,

    /// <summary>Member kind. RSA2003.</summary>
    Kind,

    /// <summary>Declared accessibility. RSA2004.</summary>
    Access,

    /// <summary>Static versus instance. RSA2005.</summary>
    Static,

    /// <summary>Constant versus non-constant field. RSA2006.</summary>
    Const,

    /// <summary>Readonly versus mutable field. RSA2007.</summary>
    Readonly
}
