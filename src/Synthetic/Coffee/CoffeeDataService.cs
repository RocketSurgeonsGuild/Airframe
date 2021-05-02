using Rocket.Surgery.Airframe.Data;

namespace Rocket.Surgery.Airframe.Synthetic
{
    /// <summary>
    /// Represents a coffee data service.
    /// </summary>
    public class CoffeeDataService : DataServiceBase<CoffeeDto>, ICoffeeDataService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoffeeDataService"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        public CoffeeDataService(IClient client)
            : base(client)
        {
        }
    }
}