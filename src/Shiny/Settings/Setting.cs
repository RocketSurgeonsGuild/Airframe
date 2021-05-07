using System;
using System.Reactive.Disposables;
using ReactiveUI;
using Rocket.Surgery.Airframe.Settings;

namespace Rocket.Surgery.Airframe.Shiny.Settings
{
    /// <summary>
    /// Class that represents an <see cref="ISetting{T}"/>.
    /// </summary>
    /// <typeparam name="T">The setting type.</typeparam>
    public sealed class Setting<T> : ReactiveObject, ISetting<T>, IDisposable
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private T _value = default!;

        /// <summary>
        /// Initializes a new instance of the <see cref="Setting{T}"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public Setting(string key, T value)
        {
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Gets the default <see cref="Setting{T}"/>.
        /// </summary>
        public static ISetting<T> Default => new Setting<T>(nameof(Default), default!);

        /// <inheritdoc/>
        public string Key { get; }

        /// <inheritdoc/>
        public T Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        /// <inheritdoc/>
        object ISetting.Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, (T)value);
        }

        /// <inheritdoc/>
        public void Dispose() => _disposable.Dispose();
    }
}