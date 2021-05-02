using System;

namespace Rocket.Surgery.Airframe.Data
{
    /// <summary>
    /// Represents a data transfer object.
    /// </summary>
    public abstract class Dto : IDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dto"/> class.
        /// </summary>
        protected Dto()
            : this(Guid.NewGuid())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dto"/> class.
        /// </summary>
        /// <param name="id">The global unique identifier.</param>
        protected Dto(Guid id) => Id = id;

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public Guid Id { get; set; }
    }
}