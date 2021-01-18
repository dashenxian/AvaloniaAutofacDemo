using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Microsoft.Extensions.Configuration;
using ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Serilog;

namespace MyApp
{
    public class MainWindowViewModel : ReactiveObject, Volo.Abp.DependencyInjection.ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MainWindow> _logger;
        private readonly IConfiguration _configuration;
        public Window View { get; set; }
        public ICommand ShowCommand { get; }
        public ICommand ShowCommand2 { get; }
        public MainWindowViewModel(IServiceProvider serviceProvider, ILogger<MainWindow> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
            ShowCommand = ReactiveCommand.CreateFromTask(ShowAsync);
            ShowCommand2 = ReactiveCommand.CreateFromTask(ShowAsync2);
        }

        public async Task ShowAsync()
        {
            var minimumLevel = _configuration["Serilog:MinimumLevel:Default"];
            _logger.LogInformation(minimumLevel);
            _logger.LogInformation("出错之前");
            Log.Logger.Information("出错之前2");
            //throw new UserFriendlyException("这是错误");
            //Error
            var child = _serviceProvider.GetService<MainWindow>();//new MainWindow();
            await child.ShowDialog(View);
        }
        public async Task ShowAsync2()
        {
            throw new Exception("这是错误");
            //Error
            var child = _serviceProvider.GetService<MainWindow>();//new MainWindow();
            await child.ShowDialog(View);
        }
    }
}
