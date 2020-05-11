using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocket.Surgery.Airframe.Data
{
    /// <summary>
    /// Represents a Mock <see cref="IClient"/> implementation.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public abstract class ClientMock<T> : IClient
        where T : Dto
    {
        /// <summary>
        /// Gets or sets the list of items.
        /// </summary>
        protected List<T> Items { get; set; } = new List<T>();

        /// <inheritdoc/>
        public virtual Task<T> Get<T>(Guid id)
            where T : Dto => Task.FromResult((T)(object)Items.FirstOrDefault(x => x.Id == id));

        /// <inheritdoc/>
        public virtual Task<IEnumerable<T>> GetAll<T>()
            where T : Dto => Task.FromResult((IEnumerable<T>)Items);

        /// <inheritdoc/>
        public virtual Task<T> Post<T>(T entity)
            where T : Dto => Task.FromResult(entity);

        /// <inheritdoc/>
        public virtual Task Delete<T>(Guid id)
            where T : Dto => Task.FromResult((T)(object)Items.FirstOrDefault(x => x.Id == id));

        /// <inheritdoc/>
        public virtual Task Delete<T>(T entity)
            where T : Dto => Task.FromResult(entity);
    }
}