namespace core_testing
{
    using System;
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NLog.Extensions.Logging;

    class Program
    {
        private static AppSettings appSettings = new AppSettings();

        private static NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static IServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            string env = Environment.GetEnvironmentVariable("LOCATION").ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(env))
            {
                // TODO this could fall back to an environment, rather than exceptioning?
                throw new Exception("LOCATION env variable not set.");
            }

            NLog.LogManager.LoadConfiguration(env.Equals("local") ? "nlog.config" : $"nlog.{env}.config");
            // log = NLog.LogManager.LoadConfiguration(env.Equals("local") ? "nlog.config" : $"nlog.{env}.config").GetCurrentClassLogger();
            log = NLog.LogManager.GetCurrentClassLogger();

            log.Info("Hello World !");

            log.Trace(Directory.GetCurrentDirectory());

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.{env}.json", optional: false, reloadOnChange: false);
            // .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();
            configuration.GetSection(nameof(AppSettings)).Bind(appSettings);
            log.Info(appSettings.ScanFolder);
            log.Info(appSettings.ErrorEmailTo);
            log.Info(appSettings.SmtpServer);

            /*var target = NLog.LogManager.Configuration.FindTargetByName<NLog.Targets.MailTarget>("errormail-real-html");
            if (target != null)
            {
                target.SmtpServer = appSettings.SmtpServer;
            }*/

            RegisterServices();

            log.Info("Services Registered");

            var service = _serviceProvider.GetService<IDemoService>();

            log.Info("Do SomeThing");

            service.DoSomething();

            var runner = _serviceProvider.GetRequiredService<Runner>();
            runner.DoAction("Yo !");

            // log.Warn("Oops Warning");
            // log.Error("Oops Error");

            log.Info("Disposing Services");

            DisposeServices();
            NLog.LogManager.Shutdown();
        }

        private static void RegisterServices()
        {
            var collection = new ServiceCollection();
            _serviceProvider = collection.AddLogging(builder =>
            {
               builder.SetMinimumLevel(LogLevel.Trace);
               builder.AddNLog(new NLogProviderOptions
               {
                   CaptureMessageTemplates = true,
                   CaptureMessageProperties = true
               });
            })
            .AddTransient<Runner>()
            .AddScoped<IDemoService, DemoService>()
            .BuildServiceProvider();
        }

        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }

            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
    }
}
