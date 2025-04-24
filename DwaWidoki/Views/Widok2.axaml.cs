using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia_home.ViewModels;

namespace Avalonia_home.Views;

public partial class Widok2 : UserControl
{
    public Widok2(MainWindowViewModel mainViewModel)
    {
        InitializeComponent();
        DataContext = mainViewModel;
    }
}