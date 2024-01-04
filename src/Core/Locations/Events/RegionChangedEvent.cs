namespace Rocket.Surgery.Airframe
{
    /// <summary>
    /// Represents a change to a <see cref="GeoRegion"/>.
    /// </summary>
    public class RegionChangedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegionChangedEvent"/> class.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="state">The region state.</param>
        public RegionChangedEvent(GeoRegion region, RegionState? state = RegionState.Unknown)
        {
            Region = region;
            State = state;
        }

        /// <summary>
        /// Gets the region.
        /// </summary>
        public GeoRegion Region { get; }

        /// <summary>
        /// Gets the region state.
        /// </summary>
        public RegionState? State { get; }
    }
}