using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace Rocket.Surgery.Airframe.Data.DuckDuckGo;

/// <summary>
/// Interface that defines a duck duck go api.
/// </summary>
/// <remarks>https://api.duckduckgo.com</remarks>
[SuppressMessage("Roslynator", "RCS1243:Duplicate word in a comment.", Justification = "Duck Duck Go")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1629:Documentation text should end with a period", Justification = "Url")]
public interface IDuckDuckGoApiClient
{
    /// <summary>
    /// Search the duck duck go api with the provided query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    [Get("/?q={query}&format=json")]
    IObservable<SearchResult> Search(string query);

    /// <summary>
    /// Search the duck duck go api with the provided query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    [Get("/?q={query}&format=json")]
    IObservable<SearchResult> Search(string query, CancellationToken cancellationToken);
}