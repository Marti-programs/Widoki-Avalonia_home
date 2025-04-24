using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia_home.Models;
using Avalonia_home.Views;

namespace Avalonia_home.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private INumberRegister _numberRegister;
    private Widok1 _view1;
    private Widok2 _view2;
    private Window? _mainWindow;
    [ObservableProperty] private int _number;

    private void ChangeToView(UserControl view)
    {
        if (_mainWindow != null)
        {
            _mainWindow.Content = view;
        }
    }
    
    public MainWindowViewModel(WidokCreator widokCreator, INumberRegister numberRegister)
    {
        _numberRegister = numberRegister;
        _view1 = widokCreator.CreateWidok1(this);
        _view2 = widokCreator.CreateWidok2(this);
        Number = 0;
    }

    public void SetMainWindow(Window mainWindow)
    {
        _mainWindow = mainWindow;
    }

    public void Init()
    {
        ChangeToView(_view1);
    }

    [RelayCommand]
    public void OnClick1()
    {
        ChangeToView(_view1);
    }
    
    [RelayCommand]
    public void OnClick2()
    {
        ChangeToView(_view2);
    }
    
    [RelayCommand]
    public void Zapisz()
    {
        _numberRegister.AddNumber(Number);
    }
}