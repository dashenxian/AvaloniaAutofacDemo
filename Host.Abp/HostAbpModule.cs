using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using MyApp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Host.Abp
{
    [DependsOn(typeof(AbpAutofacModule))]
    public class HostAbpModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            services.AddTransient<MainWindow>();
            services.AddTransient<Splat.ILogger, LoggingService>();

            var configuration = services.GetConfiguration();
            services.Configure<SerilogOption>(configuration.GetSection(
                "Serilog"));
        }
    }
}
