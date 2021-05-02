using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rocket.Surgery.Airframe.Data;

namespace Rocket.Surgery.Airframe.Synthetic
{
    /// <summary>
    /// Represents a Mock <see cref="IClient"/> implementation.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public abstract class ClientMock<T> : IClient
        where T : IDto
    {
        /// <summary>
        /// Gets or sets the list of items.
        /// </summary>
        protected List<T> Items { get; set; } = new List<T>();

        /// <inheritdoc/>
        public virtual Task<T> Get<T>(Guid id)
            where T : IDto => Task.FromResult((T)(object)Items.FirstOrDefault(x => x.Id == id));

        /// <inheritdoc/>
        public virtual Task<IEnumerable<T>> GetAll<T>()
            where T : IDto => Task.FromResult((IEnumerable<T>)Items);

        /// <inheritdoc/>
        public virtual Task<T> Post<T>(T entity)
            where T : IDto => Task.FromResult(entity);

        /// <inheritdoc/>
        public virtual Task Delete<T>(Guid id)
            where T : IDto => Task.FromResult((T)(object)Items.FirstOrDefault(x => x.Id == id));

        /// <inheritdoc/>
        public virtual Task Delete<T>(T entity)
            where T : IDto => Task.FromResult(entity);

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}