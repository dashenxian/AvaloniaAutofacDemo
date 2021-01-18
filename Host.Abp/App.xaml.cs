using System;
using System.Reactive;
using System.Reactive.Concurrency;
using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Host.Abp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Serilog;
using Serilog.Events;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;
using Volo.Abp;

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
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.AddSplat();
                })
                .UseEnvironment(Environments.Development)
                .Build();
            _application = _host.Services.GetService<IAbpApplicationWithExternalServiceProvider>();
            // Since MS DI container is a different type,
            // we need to re-register the built container with Splat again
            var container = _host.Services;
            container.UseMicrosoftDependencyResolver();
        }

        public override void Initialize()
        {
            Initialize(_host.Services);
            AvaloniaXamlLoader.Load(this);
            RxApp.DefaultExceptionHandler = Observer.Create<Exception>(ex =>
            {
                Log.Fatal(ex, "onNext!");
                if (!(ex is UserFriendlyException))
                {
                    MessageBus.Current.SendMessage(new ShutdownApp(),"1");
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
    public class ShutdownApp{}
}