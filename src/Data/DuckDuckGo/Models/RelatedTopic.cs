using System.Collections.Generic;

namespace Rocket.Surgery.Airframe.Data.DuckDuckGo;

/// <summary>
/// Represents related topics.
/// </summary>
public class RelatedTopic
{
    /// <summary>
    /// Gets or sets the first url.
    /// </summary>
    public required string FirstUrl { get; set; }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    public Icon? Icon { get; set; }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    public required string Result { get; set; }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    public required string Text { get; set; }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    public IEnumerable<Topic> Topics { get; set; } = [];
}