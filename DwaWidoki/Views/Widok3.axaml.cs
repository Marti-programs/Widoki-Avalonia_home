using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia_home.ViewModels;

namespace Avalonia_home.Views;

public partial class Widok3 : UserControl
{
    public Widok3(MainWindowViewModel mainViewModel)
    {
        InitializeComponent();
        DataContext = mainViewModel;
    }
}