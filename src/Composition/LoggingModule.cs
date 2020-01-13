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
        private readonly string _appCenterSecret;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingModule"/> class.
        /// </summary>
        public LoggingModule()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingModule"/> class.
        /// </summary>
        /// <param name="appCenterSecret">The app center secret.</param>
        public LoggingModule(string appCenterSecret)
        {
            _appCenterSecret = appCenterSecret;
        }

        /// <inheritdoc />
        public override void Load(IRegistrator registrar)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo
                .AppCenterCrashes(_appCenterSecret)
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