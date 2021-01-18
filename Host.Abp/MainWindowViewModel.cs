using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;
using Microsoft.Extensions.DependencyInjection;

namespace MyApp
{
    public class MainWindowViewModel : ReactiveObject,Volo.Abp.DependencyInjection.ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;
        public Window View { get; set; }
        public ICommand ShowCommand { get; }
        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            ShowCommand = ReactiveCommand.CreateFromTask(ShowAsync);
        }

        public async Task ShowAsync()
        {
            //Error
            var child = _serviceProvider.GetService<MainWindow>();//new MainWindow();
            await child.ShowDialog(View);
        }
    }
}
