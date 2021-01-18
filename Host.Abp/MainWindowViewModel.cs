using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;

namespace MyApp
{
    public class MainWindowViewModel : ReactiveObject,Volo.Abp.DependencyInjection.ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;
        public Window View { get; set; }
        public ICommand ShowCommand { get; }
        public ICommand ShowCommand2 { get; }
        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            ShowCommand = ReactiveCommand.CreateFromTask(ShowAsync);
            ShowCommand2 = ReactiveCommand.CreateFromTask(ShowAsync2);
        }

        public async Task ShowAsync()
        {
            throw new UserFriendlyException("这是错误");
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
