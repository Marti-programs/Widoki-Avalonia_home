using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia_home.ViewModels;

namespace Avalonia_home.Views;

public partial class Widok1 : UserControl
{
    public Widok1(MainWindowViewModel mainViewModel)
    {
        InitializeComponent();
        DataContext = mainViewModel;
    }
}