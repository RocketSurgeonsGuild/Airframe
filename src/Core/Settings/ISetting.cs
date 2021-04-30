using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

[assembly:SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Typed and untyped.")]
namespace Rocket.Surgery.Airframe.Settings
{
    /// <summary>
    /// Interface representing a setting.
    /// </summary>
    public interface ISetting : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        object Value { get; set; }
    }

    /// <summary>
    /// Interface representing a setting.
    /// </summary>
    /// <typeparam name="T">The setting type.</typeparam>
    public interface ISetting<T> : ISetting
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        new T Value { get; set; }
    }
}