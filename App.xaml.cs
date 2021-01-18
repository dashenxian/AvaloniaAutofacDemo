using System;
using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using ReactiveUI;
using Splat;
using Splat.Autofac;

namespace MyApp
{
    public class App : Application
    {
        public override void Initialize()
        {
            InitAutofac();
            AvaloniaXamlLoader.Load(this);
        }
        private void InitAutofac()
        {
            // Build a new Autofac container.
            var builder = new ContainerBuilder();

            //builder.RegisterType<MainWindowViewModel>();
            //builder.RegisterType<Student>().As<IStudent>();
            // Creates and sets the Autofac resolver as the Locator
            var autofacResolver = builder.UseAutofacDependencyResolver();
            //var autofacResolver = new AutofacDependencyResolver(builder);
            Locator.SetLocator(autofacResolver);
            // Register the resolver in Autofac so it can be later resolved
            builder.RegisterInstance(autofacResolver);

            // Initialize ReactiveUI components
            autofacResolver.InitializeSplat();
            autofacResolver.InitializeReactiveUI();
            //Container = autofacResolver.Container;

            RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
            Locator.CurrentMutable.RegisterConstant(new AvaloniaActivationForViewFetcher(), typeof(IActivationForViewFetcher));
            Locator.CurrentMutable.RegisterConstant(new AutoDataTemplateBindingHook(), typeof(IPropertyBindingHook));
        }


        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}