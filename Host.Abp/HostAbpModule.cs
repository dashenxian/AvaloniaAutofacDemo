﻿using System;
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
            context.Services.AddTransient<MainWindow>();
        }
    }
}