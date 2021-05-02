using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using DynamicData;

namespace Rocket.Surgery.Airframe.Data
{
    /// <summary>
    /// Interface representing a typed data service.
    /// </summary>
    /// <typeparam name="T">The dto type.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Typed and untyped.")]
    public interface IDataService<T> : IDataService
        where T : IDto
    {
        /// <summary>
        /// Gets the observable of change sets.
        /// </summary>
        IObservable<IChangeSet<T, Guid>> ChangeSet { get; }

        /// <summary>
        /// Creates a new entity.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns>A completion notification.</returns>
        IObservable<Unit> Create(T dto);

        /// <summary>
        /// Reads all available entities.
        /// </summary>
        /// <returns>A completion notification.</returns>
        new IObservable<T> Read();

        /// <summary>
        /// Reads an entity with the specified id.
        /// </summary>
        /// <param name="id">The identifier..</param>
        /// <returns>A completion notification.</returns>
        new IObservable<T> Read(Guid id);

        /// <summary>
        /// Updates the specified dto.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns>A completion notification.</returns>
        IObservable<Unit> Update(T dto);

        /// <summary>
        /// Deletes the specified dto.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns>A completion notification.</returns>
        IObservable<Unit> Delete(T dto);
    }

    /// <summary>
    /// Interface representing a non typed data service.
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Creates a new dto.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns>A completion notification.</returns>
        IObservable<Unit> Create(object dto);

        /// <summary>
        /// Reads all available dto's.
        /// </summary>
        /// <returns>A completion notification.</returns>
        IObservable<object> Read();

        /// <summary>
        /// Reads an entity with the specified id.
        /// </summary>
        /// <param name="id">The identifier..</param>
        /// <returns>A completion notification.</returns>
        IObservable<object> Read(Guid id);

        /// <summary>
        /// Updates the specified dto.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns>A completion notification.</returns>
        IObservable<Unit> Update(object dto);

        /// <summary>
        /// Deletes the specified dto.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns>A completion notification.</returns>
        IObservable<Unit> Delete(object dto);

        /// <summary>
        /// Deletes the dto with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A completion notification.</returns>
        IObservable<Unit> Delete(Guid id);
    }
}