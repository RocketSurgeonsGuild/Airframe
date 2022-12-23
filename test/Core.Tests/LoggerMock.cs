using Microsoft.Extensions.Logging;
using System.Reactive.Disposables;
using Xunit.Abstractions;

namespace Rocket.Surgery.Airframe.Core.Tests;

internal class LoggerMock<T> : ILogger<T>
{
    private readonly ITestOutputHelper _testOutputHelper;

    public LoggerMock(ITestOutputHelper testOutputHelper) => _testOutputHelper = testOutputHelper;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) => LogToOutputHelper(eventId, state, exception, formatter);

    private void LogToOutputHelper<TState>(EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) =>
        _testOutputHelper.WriteLine(formatter(state, exception));

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable BeginScope<TState>(TState state) => Disposable.Empty;
}