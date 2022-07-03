using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using AvaloniaImageCompress.ViewModels;

namespace AvaloniaImageCompress.Views
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel.WindowInstance = this;
        }
       


    }
}
