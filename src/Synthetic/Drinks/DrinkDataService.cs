using Rocket.Surgery.Airframe.Data;

namespace Rocket.Surgery.Airframe.Synthetic
{
    /// <summary>
    /// Represents a coffee data service.
    /// </summary>
    public class DrinkDataService : DataServiceBase<DrinkDto>, IDrinkService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrinkDataService"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        public DrinkDataService(IClient client)
            : base(client)
        {
        }
    }
}