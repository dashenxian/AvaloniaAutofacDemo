using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging.Serilog;
using Avalonia.ReactiveUI;
using Host.Abp;
using Microsoft.Extensions.Configuration;
using ReactiveUI;
using Serilog;
using Serilog.Events;

namespace MyApp
{
    class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            //            Log.Logger = new LoggerConfiguration()
            //#if DEBUG
            //                .MinimumLevel.Debug()
            //#else
            //                .MinimumLevel.Information()
            //#endif
            //                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            //                .Enrich.FromLogContext()
            //                .WriteTo.Async(c => c.File("Logs/logs.txt"))
            //                .CreateLogger();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();
            try
            {
                //RxApp.DefaultExceptionHandler = new MyCoolObservableExceptionHandler();
                Log.Information("启动程序");
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
                BuildAvaloniaApp()
                   .StartWithClassicDesktopLifetime(args);
                Log.Information("关闭程序");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Log.Fatal(e.Exception, "Host terminated unexpectedly!");
        }

        private async static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Fatal(e.ExceptionObject as Exception, "Host terminated unexpectedly!");
            Window parentWindow = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow;
            await new Dialog().ShowDialog(parentWindow);
        }
        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UseReactiveUI()
                .UsePlatformDetect()
                .LogToDebug();
    }
}
