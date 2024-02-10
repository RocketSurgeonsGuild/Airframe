using System;
using DynamicData;

namespace Rocket.Surgery.Airframe.Data;

/// <summary>
/// Interface representing a <see cref="ChuckNorrisJoke"/> service.
/// </summary>
public interface IChuckNorrisJokeService
{
    /// <summary>
    /// Gets a random chuck norris joke.
    /// </summary>
    /// <returns>An observable of chang sets.</returns>
    IObservable<ChuckNorrisJoke> Random();

    /// <summary>
    /// Gets a random chuck norris joke from the provided categories.
    /// </summary>
    /// <param name="categories">The categories.</param>
    /// <returns>An observable of chang sets.</returns>
    IObservable<ChuckNorrisJoke> Random(params string[] categories);

    /// <summary>
    /// Queries chuck norris jokes.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>An observable of chang sets.</returns>
    IObservable<IChangeSet<ChuckNorrisJoke, string>> Query(string query);

    /// <summary>
    /// Queries chuck norris jokes.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="clearCache">A value indicating whether to clear the cache.</param>
    /// <returns>An observable of chang sets.</returns>
    IObservable<IChangeSet<ChuckNorrisJoke, string>> Query(string query, bool clearCache);
}