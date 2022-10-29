using System;
using System.Diagnostics.CodeAnalysis;
using DynamicData;

namespace Rocket.Surgery.Airframe.Data.DuckDuckGo
{
    /// <summary>
    /// Interface representing a service that queries the <see cref="IDuckDuckGoApiClient"/>.
    /// </summary>
    [SuppressMessage("Roslynator", "RCS1243:Duplicate word in a comment.", Justification = "Duck Duck Go")]
    public interface IDuckDuckGoService
    {
        /// <summary>
        /// Queries duck duck go.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>An observable of chang sets.</returns>
        IObservable<IChangeSet<RelatedTopic, string>> Query(string query);

        /// <summary>
        /// Queries duck duck go.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="clearCache">A value indicating whether to clear the cache.</param>
        /// <returns>An observable of chang sets.</returns>
        IObservable<IChangeSet<RelatedTopic, string>> Query(string query, bool clearCache);
    }
}