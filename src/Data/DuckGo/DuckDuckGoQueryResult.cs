namespace Rocket.Surgery.Airframe.Data
{
    /// <summary>
    /// Result returned from the query.
    /// </summary>
    public class DuckDuckGoQueryResult
    {
        /// <summary>
        /// Gets or sets hTML link(s) to related topic(s) or external site(s).
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// Gets or sets icon associated with related topic(s) or FirstUrl.
        /// </summary>
        public DuckDuckGoIcon Icon { get; set; }

        /// <summary>
        /// Gets or sets first URL in Result.
        /// </summary>
        public string FirstUrl { get; set; }

        /// <summary>
        /// Gets or sets text from first URL.
        /// </summary>
        public string Text { get; set; }
    }
}