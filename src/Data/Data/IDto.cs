using System;

namespace Data
{
    /// <summary>
    /// Interface representing a data transfer object.
    /// </summary>
    public interface IDto
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        Guid Id { get; set; }
    }
}