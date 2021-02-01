using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Host.Abp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Serilog;
using Serilog.Events;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;
using Volo.Abp;
using LogLevel = Splat.LogLevel;

namespace MyApp
{
    public class App : Application
    {
        private IHost _host;
        private IAbpApplicationWithExternalServiceProvider _application;
        public IServiceProvider Container { get; private set; }
        public App()
        {
            Init();
        }
        private void Init()
        {
            _host = Microsoft.Extensions.Hosting.Host
                .CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    IHostEnvironment env = hostingContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                })
                //.UseEnvironment(Environments.Development)
                .ConfigureServices(services =>
                {
                    services.UseMicrosoftDependencyResolver();
                    var resolver = Locator.CurrentMutable;
                    resolver.InitializeSplat();
                    resolver.InitializeReactiveUI();

                    RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
                    Locator.CurrentMutable.RegisterConstant(new AvaloniaActivationForViewFetcher(), typeof(IActivationForViewFetcher));
                    Locator.CurrentMutable.RegisterConstant(new AutoDataTemplateBindingHook(), typeof(IPropertyBindingHook));
                    // Configure our local services and access the host configuration
                    services.AddApplication<HostAbpModule>();
                })
                .UseSerilog()
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.AddSplat();
                })
                .Build();
            _application = _host.Services.GetService<IAbpApplicationWithExternalServiceProvider>();
            // Since MS DI container is a different type,
            // we need to re-register the built container with Splat again
            var container = _host.Services;
            container.UseMicrosoftDependencyResolver();
        }

        public override void Initialize()
        {
            Interaction<string, bool> interaction = new Interaction<string, bool>();
            Initialize(_host.Services);
            AvaloniaXamlLoader.Load(this);
            RxApp.DefaultExceptionHandler = Observer.Create<Exception>(ex =>
            {
                Log.Error(ex, "onNext!");
                if (!(ex is UserFriendlyException))
                {
                    MessageBus.Current.SendMessage(new ShutdownApp(), "1");
                }
                else
                {

                    //interaction.RegisterHandler( interaction =>
                    //{
                    //    var deleteIt = this.DisplayAlert(
                    //        "Confirm Delete",
                    //        $"Are you sure you want to delete '{interaction.Input}'?",
                    //        "YES",
                    //        "NO");

                    //    interaction.SetOutput(deleteIt);
                    //});
                    //interaction.Handle(ex.Message);
                }
            });
        }

        private void Initialize(IServiceProvider serviceProvider)
        {
            _application.Initialize(serviceProvider);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = _host.Services.GetRequiredService<MainWindow>();
                MessageBus.Current.Listen<ShutdownApp>("1")
                    .Subscribe(x =>
                    {
                        desktop.Shutdown();
                    });
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
    public class ShutdownApp { }
    public class LoggingService : Splat.ILogger
    {
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger;
            Level = LogLevel.Debug;
        }
        public LogLevel Level { get; set; }

        public void Write([Localizable(false)] string message, LogLevel logLevel)
        {
            if (logLevel >= Level)
                _logger.LogInformation(message);
        }

        public void Write(Exception exception, [Localizable(false)] string message, LogLevel logLevel)
        {
            if (logLevel >= Level)
                _logger.LogInformation(message);
        }

        public void Write([Localizable(false)] string message, [Localizable(false)] Type type, LogLevel logLevel)
        {
            if (logLevel >= Level)
                _logger.LogInformation(message);
        }

        public void Write(Exception exception, [Localizable(false)] string message, [Localizable(false)] Type type, LogLevel logLevel)
        {
            if (logLevel >= Level)
                System.Diagnostics.Debug.WriteLine(message);
        }
    }
}