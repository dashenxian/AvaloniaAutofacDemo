using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;

namespace MyApp
{
    public class MainWindowViewModel : ReactiveObject
    {
        public Window View { get; set; }
        public ICommand ShowCommand { get; }
        public MainWindowViewModel()
        {
            ShowCommand = ReactiveCommand.CreateFromTask(ShowAsync);
        }

        public async Task ShowAsync()
        {
            //Error
            var child = new MainWindow();
            child.ShowDialog(View);
        }
    }
}
