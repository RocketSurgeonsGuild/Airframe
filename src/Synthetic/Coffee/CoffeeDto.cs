using System.Collections.Generic;
using Rocket.Surgery.Airframe.Data;

namespace Rocket.Surgery.Airframe.Synthetic
{
    /// <summary>
    /// A coffee data transfer object.
    /// </summary>
    public class CoffeeDto : Dto
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the species.
        /// </summary>
        public string Species { get; set; }

        /// <summary>
        /// Gets or sets the regions.
        /// </summary>
        public IEnumerable<string> Regions { get; set; }
    }
}