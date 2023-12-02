using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using Xunit.Abstractions;

[assembly: SuppressMessage("StyleCop.CSharp.NamingRules", "SA1312:Variable names should begin with lower-case letter", Justification = "Discarded variable.")]

namespace Rocket.Surgery.Airframe.Core.Tests
{
    internal class LoggerMock<T> : ILogger<T>
    {
        private readonly ITestOutputHelper _testOutputHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerMock{T}"/> class.
        /// </summary>
        /// <param name="testOutputHelper">The test output.</param>
        public LoggerMock(ITestOutputHelper testOutputHelper) => _testOutputHelper = testOutputHelper;

        /// <inheritdoc/>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
            LogToOutputHelper(eventId, state, exception, formatter);

        /// <inheritdoc/>
        public bool IsEnabled(LogLevel logLevel) => true;

        /// <inheritdoc/>
        public IDisposable BeginScope<TState>(TState state) => Disposable.Empty;

        private void LogToOutputHelper<TState>(EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) =>
            _testOutputHelper.WriteLine(formatter(state, exception));
    }
}