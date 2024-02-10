using System.Diagnostics.CodeAnalysis;

namespace Rocket.Surgery.Airframe.Synthetic.Drinks;

/// <summary>
/// Enumeration representing espresso drink types.
/// </summary>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1629:Documentation text should end with a period", Justification = "enum")]
public enum DrinkType
{
    /// <summary>
    /// Caffe Americano.
    /// </summary>
    Americano,

    /// <summary>
    /// Drip Coffee.
    /// </summary>
    Brewed,

    /// <summary>
    /// Cappuccino.
    /// </summary>
    Cappuccino,

    /// <summary>
    /// Espresso Shots.
    /// </summary>
    Espresso,

    /// <summary>
    /// Flat White.
    /// </summary>
    FlatWhite,

    /// <summary>
    /// Caffe Latte.
    /// </summary>
    Latte,

    /// <summary>
    /// Macchiato.
    /// </summary>
    Macchiato
}