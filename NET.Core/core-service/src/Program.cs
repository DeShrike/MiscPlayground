namespace core.service
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NLog.Extensions.Logging;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    class Program 
    {
        public static AppSettings AppSettings;

        public static NLog.Logger logger;

        public static IServiceProvider _serviceProvider;

        public static IHostBuilder _hostBuilder;

        private static async Task Main(string[] args)
        {
            Configure();
            // RegisterServices();
            RegisterHostedServices();

            var isService = !(Debugger.IsAttached || args.Contains("--console"));
            if (isService)
            {
                await _hostBuilder.RunAsServiceAsync();
            }
            else
            {
                await _hostBuilder.RunConsoleAsync();
            }

            DisposeServices();
            DisposeHostedServices();
            NLog.LogManager.Shutdown();
        }

        private static void RegisterHostedServices()
        {
            // Windows Service
            Program._hostBuilder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddHostedService<TestService>()
                        .AddScoped<IDemoService, DemoService>();
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddNLog(new NLogProviderOptions
                    {
                        CaptureMessageTemplates = true,
                        CaptureMessageProperties = true
                    });
                });
        }

        private static void RegisterServices()
        {
            // DI
            var collection = new ServiceCollection();

            Program._serviceProvider = collection.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageTemplates = true,
                    CaptureMessageProperties = true
                });
            })
            .AddTransient<TestService>()
            .AddScoped<IDemoService, DemoService>()
            .BuildServiceProvider();
        }

        protected static void Configure()
        {
            Program.AppSettings = new AppSettings();

            string env = Environment.GetEnvironmentVariable("LOCATION").ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(env))
            {
                // TODO this could fall back to an environment, rather than exceptioning?
                throw new Exception("LOCATION env variable not set.");
            }

            Program.logger = NLog.LogManager.LoadConfiguration(env.Equals("local") ? "nlog.config" : $"nlog.{env}.config").GetCurrentClassLogger();

            var configbuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false);
            //             .AddEnvironmentVariables();

            IConfigurationRoot configuration = configbuilder.Build();
            configuration.GetSection(nameof(AppSettings)).Bind(Program.AppSettings);

            Program.logger.Info(Program.AppSettings.PingInterval);
        }

        public static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }

            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)Program._serviceProvider).Dispose();
            }
        }

        public static void DisposeHostedServices()
        {
            if (_hostBuilder == null)
            {
                return;
            }

            if (_hostBuilder is IDisposable)
            {
                ((IDisposable)Program._hostBuilder).Dispose();
            }
        }
    }
}
