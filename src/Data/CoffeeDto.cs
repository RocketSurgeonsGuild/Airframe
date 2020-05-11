using System.Collections.Generic;

namespace Rocket.Surgery.Airframe.Data
{
    public class CoffeeDto : Dto
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the spicies.
        /// </summary>
        public string Species { get; set; }

        /// <summary>
        /// Gets or sets the regions.
        /// </summary>
        public IEnumerable<string> Regions { get; set; }
    }
}