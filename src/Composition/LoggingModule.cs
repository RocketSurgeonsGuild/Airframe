using DryIoc;
using Serilog;
using Splat;

namespace Rocket.Surgery.Airframe.Composition;

/// <summary>
/// Logging registrations.
/// </summary>
public class LoggingModule : DryIocModule
{
    /// <inheritdoc />
    public override void Load(IRegistrator registrar)
    {
        var funcLogManager = new FuncLogManager(
            type =>
            {
                var actualLogger = Log.ForContext(type);
                return new SerilogFullLogger(actualLogger);
            });

        registrar.RegisterInstance(Log.Logger);
        registrar.Register<IFullLogger, SerilogFullLogger>(Reuse.Singleton);
        registrar.RegisterInstance(funcLogManager);
    }
}