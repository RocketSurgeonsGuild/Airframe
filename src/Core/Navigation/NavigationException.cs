using System;

namespace Rocket.Surgery.Airframe.Navigation;

/// <summary>
/// Represents a navigation error.
/// </summary>
public sealed class NavigationException : Exception
{
    /// <inheritdoc />
    public NavigationException()
    {
    }

    /// <inheritdoc />
    public NavigationException(string? message)
        : base(message)
    {
    }

    /// <inheritdoc />
    public NavigationException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}