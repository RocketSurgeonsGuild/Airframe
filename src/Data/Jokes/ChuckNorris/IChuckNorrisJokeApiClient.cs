using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Refit;

namespace Rocket.Surgery.Airframe.Data
{
    /// <summary>
    /// Interface that defines an api client for <c>https://api.chucknorris.io/</c>.
    /// </summary>
    /// <remarks>https://api.chucknorris.io</remarks>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1629:Documentation text should end with a period", Justification = "Url")]
    public interface IChuckNorrisJokeApiClient
    {
        /// <summary>
        /// Gets a random <see cref="ChuckNorrisJoke"/>.
        /// </summary>
        /// <returns>An <see cref="IObservable{T}"/> representing the result of the asynchronous operation.</returns>
        [Get("/jokes/random")]
        IObservable<ChuckNorrisJoke> Random();

        /// <summary>
        /// Gets a random <see cref="ChuckNorrisJoke"/>.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>An <see cref="IObservable{T}"/> representing the result of the asynchronous operation.</returns>
        [Get("/jokes/random?category={category}")]
        IObservable<IEnumerable<ChuckNorrisJoke>> RandomFromCategory(string category);

        /// <summary>
        /// Gets a random <see cref="ChuckNorrisJoke"/>.
        /// </summary>
        /// <returns>An <see cref="IObservable{T}"/> representing the result of the asynchronous operation.</returns>
        [Get("/jokes/categories")]
        IObservable<IEnumerable<string>> Categories();

        /// <summary>
        /// Gets a random <see cref="ChuckNorrisJoke"/>.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>An <see cref="IObservable{T}"/> representing the result of the asynchronous operation.</returns>
        [Get("/jokes/search?query={query}")]
        IObservable<IEnumerable<ChuckNorrisJoke>> Search(string query);
    }
}