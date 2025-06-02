using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia_home.ViewModels;

namespace Avalonia_home.Views;

public partial class Formularz : Window
{
    private readonly MainWindowViewModel _viewModel;
    public Formularz(MainWindowViewModel vm)
    {
        InitializeComponent();
        _viewModel = vm;
        DataContext = _viewModel;
    }

    private async void Dodaj_rekord(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await _viewModel.DodajZadanieAsync();
        Close();
    }
}