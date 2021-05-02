using Rocket.Surgery.Airframe.Data;

namespace Rocket.Surgery.Airframe.Synthetic
{
    /// <summary>
    /// A coffee drink data transfer object.
    /// </summary>
    public class DrinkDto : Dto
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public DrinkType Type { get; set; }
    }
}