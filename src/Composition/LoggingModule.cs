using DryIoc;
using Serilog;
using Splat;

namespace Rocket.Surgery.Airframe.Composition
{
    /// <summary>
    /// Logging registrations.
    /// </summary>
    public class LoggingModule : DryIocModule
    {
        /// <inheritdoc />
        public override void Load(IRegistrator registrar)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo
                .AppCenterCrashes()
                .CreateLogger();

            var funcLogManager = new FuncLogManager(type =>
            {
                var actualLogger = global::Serilog.Log.ForContext(type);
                return new SerilogFullLogger(actualLogger);
            });

            registrar.RegisterInstance<Serilog.ILogger>(Log.Logger);
            registrar.Register<IFullLogger, SerilogFullLogger>(Reuse.Singleton);
            registrar.RegisterInstance<FuncLogManager>(funcLogManager);
        }
    }
}