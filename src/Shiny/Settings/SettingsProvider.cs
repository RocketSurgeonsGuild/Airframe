using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using Rocket.Surgery.Airframe.Settings;
using Shiny.Stores;

namespace Rocket.Surgery.Airframe.Shiny.Settings
{
    /// <summary>
    /// Represents a <see cref="ISettingsProvider"/> for <see cref="IKeyValueStore"/>.
    /// </summary>
    public sealed class SettingsProvider : ReactiveObject, ISettingsProvider, IDisposable
    {
        private readonly CompositeDisposable _garbage = new CompositeDisposable();

        [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Roslyn can't read DisposeWith")]
        private readonly SourceCache<ISetting, string> _settingsCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsProvider"/> class.
        /// </summary>
        /// <param name="shinySettings">The shiny settings interface.</param>
        public SettingsProvider(IKeyValueStore shinySettings)
        {
            static void PersistSetting(ISetting setting, IKeyValueStore settings) => settings.Set(setting.Key, setting.Value);

            static void RemoveSetting(ISetting setting, IKeyValueStore settings) => settings.Remove(setting.Key);

            _settingsCache = new SourceCache<ISetting, string>(x => x.Key).DisposeWith(_garbage);

            var settingsChanged = _settingsCache
               .DeferUntilLoaded()
               .RefCount();

            settingsChanged
               .OnItemAdded(setting => PersistSetting(setting, shinySettings))
               .OnItemRemoved(setting => RemoveSetting(setting, shinySettings))
               .WhenPropertyChanged(x => x.Value)
               .Where(x => x != null)
               .Subscribe(value => PersistSetting(value.Sender, shinySettings))
               .DisposeWith(_garbage);

            _settingsCache.DisposeWith(_garbage);
        }

        /// <inheritdoc />
        public ISetting<T> Get<T>(string key) => (ISetting<T>)_settingsCache.Lookup(key).Value;

        /// <inheritdoc />
        public ISetting<T> Get<T>(string key, T defaultValue)
        {
            var optional = _settingsCache.Lookup(key);
            if (optional.HasValue)
            {
                return (ISetting<T>)optional.Value;
            }

            _settingsCache.AddOrUpdate(new Setting<T>(key, defaultValue));
            return (ISetting<T>)_settingsCache.Lookup(key).Value;
        }

        /// <inheritdoc />
        public IObservable<ISetting<T>> Observe<T>(string key) =>
            _settingsCache
                .Watch(key)
                .Select(x => x.Current)
                .Cast<ISetting<T>>();

        /// <inheritdoc />
        public void Set<T>(ISetting<T> setting) => _settingsCache.AddOrUpdate(setting);

        /// <inheritdoc />
        public bool Remove(string key)
        {
            _settingsCache.Remove(key);
            return true;
        }

        /// <inheritdoc />
        public bool Contains(string key) => _settingsCache.Lookup(key).HasValue;

        /// <inheritdoc />
        public void Clear() => _settingsCache.Clear();

        /// <inheritdoc/>
        public void Dispose() => _garbage.Dispose();
    }
}