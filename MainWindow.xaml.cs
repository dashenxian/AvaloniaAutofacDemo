using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MyApp
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var btn = this.FindControl<Button>("ShowChild");
            btn.Click += Btn_Click;
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            var model = new MainWindowViewModel();
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