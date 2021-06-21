namespace Rocket.Surgery.Airframe.Data.DuckDuckGo
{
    /// <summary>
    /// Represents a topic.
    /// </summary>
    public class Topic
    {
        /// <summary>
        /// Gets or sets the first url.
        /// </summary>
        public string FirstUrl { get; set; }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public Icon Icon { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get; set; }
    }
}