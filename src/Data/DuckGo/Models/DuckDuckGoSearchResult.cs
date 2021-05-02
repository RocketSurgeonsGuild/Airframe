using System.Collections.Generic;

namespace Rocket.Surgery.Airframe.Data
{
    /// <summary>
    /// Search results from query.
    /// </summary>
    public class DuckDuckGoSearchResult
    {
        /// <summary>
        /// Gets or sets topic summary containing HTML.
        /// </summary>
        public string Abstract { get; set; }

        /// <summary>
        /// Gets or sets topic summary containing no HTML.
        /// </summary>
        public string AbstractText { get; set; }

        /// <summary>
        /// Gets or sets type of Answer, e.g. calc, color, digest, info, ip, iploc, phone, pw, rand, regexp, unicode, upc, or zip (see goodies & tech pages for examples).
        /// </summary>
        public string AnswerType { get; set; }

        /// <summary>
        /// Gets or sets name of Abstract Source.
        /// </summary>
        public string AbstractSource { get; set; }

        /// <summary>
        /// Gets or sets dictionary definition (may differ from Abstract).
        /// </summary>
        public string Definition { get; set; }

        /// <summary>
        /// Gets or sets name of Definition source.
        /// </summary>
        public string DefinitionSource { get; set; }

        /// <summary>
        /// Gets or sets name of topic that goes with Abstract.
        /// </summary>
        public string Heading { get; set; }

        /// <summary>
        /// Gets or sets link to image that goes with Abstract.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Gets or sets array of internal links to related topics associated with Abstract.
        /// </summary>
        public List<DuckDuckGoQueryResult> RelatedTopics { get; set; }

        /// <summary>
        /// Gets or sets response category, i.e. A (article), D (disambiguation), C (category), N (name), E (exclusive), or nothing.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets !bang redirect URL.
        /// </summary>
        public string Redirect { get; set; }

        /// <summary>
        /// Gets or sets deep link to expanded definition page in DefinitionSource.
        /// </summary>
        public string DefinitionUrl { get; set; }

        /// <summary>
        /// Gets or sets instant answer.
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// Gets or sets array of external links associated with Abstract.
        /// </summary>
        public List<DuckDuckGoQueryResult> Results { get; set; }

        /// <summary>
        /// Gets or sets deep link to the expanded topic page in AbstractSource.
        /// </summary>
        public string AbstractUrl { get; set; }
    }
}