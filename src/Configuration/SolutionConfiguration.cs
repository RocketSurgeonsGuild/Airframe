﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Rocket.Surgery.Airframe.Configuration
{
    /// <summary>
    /// Represents a default xamarin mobile configuration.
    /// </summary>
    [SuppressMessage("Microsoft.Usage", "CA2214", Justification = "Consumer is aware of virtual constructor call.", Scope = "member")]
    public abstract class SolutionConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionConfiguration"/> class.
        /// </summary>
        protected SolutionConfiguration()
        {
            ConfigureEnvironment();
        }

        /// <summary>
        /// Gets or sets the current environment configuration.
        /// </summary>
        public Configuration Current { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to user mock data.
        /// </summary>
        public bool UseMockData { get; set; }

        /// <summary>
        /// Gets or sets the appcenter configuration.
        /// </summary>
        public Dictionary<string, string> AppCenter { get; set; }

        /// <summary>
        /// Overrideable method to configure environment.
        /// </summary>
        public abstract void ConfigureEnvironment();
    }
}