using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia_home.ViewModels;

namespace Avalonia_home.Views;

public partial class Widok3 : UserControl
{
    private readonly MainWindowViewModel _viewModel;

    public Widok3(MainWindowViewModel mainViewModel)
    {
        InitializeComponent();
        _viewModel = mainViewModel;
        DataContext = _viewModel;
    }

    private void Formularz_Click(object? sender, RoutedEventArgs e)
    {
        var NowyFormularz = new Formularz(_viewModel);
        NowyFormularz.Show(); // albo ShowDialog(), jeœli chcesz zablokowaæ widok pod spodem
    }
}