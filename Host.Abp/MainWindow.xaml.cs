using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;

namespace MyApp
{
    public class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;

        public MainWindow()
        {
        }

        public MainWindow(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider; 
            InitializeComponent();
            var btn = this.FindControl<Button>("ShowChild");
            btn.Click += Btn_Click;
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            var model = _serviceProvider.GetService<MainWindowViewModel>();
            model.View = this;
            this.DataContext = model;
        }

        private void Btn_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var child = new MainWindow();
            child.ShowDialog(this);
        }
    }
}