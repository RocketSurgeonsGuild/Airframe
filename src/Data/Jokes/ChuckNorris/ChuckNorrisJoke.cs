using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Rocket.Surgery.Airframe.Data.ChuckNorris
{
    /// <summary>
    /// Represents a joke from https://api.chucknorris.io/.
    /// </summary>
    public class ChuckNorrisJoke : IHaveIdentifier<string>
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the icon url.
        /// </summary>
        [JsonProperty("icon_url")]
        public Uri IconUrl { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        [JsonProperty("url")]
        public Uri Url { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        [JsonProperty("categories")]
        public IEnumerable<string> Categories { get; set; }

        /// <summary>
        /// Gets or sets the date of creation.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last updated date.
        /// </summary>
        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}