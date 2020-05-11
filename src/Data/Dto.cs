using System;

namespace Rocket.Surgery.Airframe.Data
{
    /// <summary>
    /// Represents a data transfer object.
    /// </summary>
    public abstract class Dto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dto"/> class.
        /// </summary>
        protected Dto()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public Guid Id { get; set; }
    }
}