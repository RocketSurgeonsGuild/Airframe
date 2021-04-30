using System;
using System.Diagnostics.CodeAnalysis;

[assembly:SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification = "Typed and untyped.")]
namespace Rocket.Surgery.Airframe.Settings
{
    /// <summary>
    /// Interface representing a way to access <see cref="ISetting{T}"/>.
    /// </summary>
    public interface ISettingsProvider
    {
        /// <summary>
        /// Gets the <see cref="ISetting{T}"/> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <typeparam name="T">The setting type.</typeparam>
        /// <returns>The setting.</returns>
        // ReSharper disable once CA1716
        ISetting<T> Get<T>(string key);

        /// <summary>
        /// Gets the <see cref="ISetting{T}"/> with the specified key and provides a default value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <typeparam name="T">The setting type.</typeparam>
        /// <returns>The setting.</returns>
        ISetting<T> Get<T>(string key, T defaultValue);

        /// <summary>
        /// Gets an observable sequence of changes.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <typeparam name="T">The setting type.</typeparam>
        /// <returns>A stream of settings.</returns>
        IObservable<ISetting<T>> Observe<T>(string key);

        /// <summary>
        /// Sets the <see cref="ISetting{T}"/>.
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <typeparam name="T">The setting type.</typeparam>
        void Set<T>(ISetting<T> setting);

        /// <summary>
        /// Remove the setting by key.
        /// </summary>
        /// <param name="key">The setting key.</param>
        /// <returns>A value indicating whether the setting was removed.</returns>
        bool Remove(string key);

        /// <summary>
        /// Checks if the setting key is set.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>A value indicating whether the setting is in the provider.</returns>
        bool Contains(string key);

        /// <summary>
        /// Clears all setting values.
        /// </summary>
        void Clear();
    }
}