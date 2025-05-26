using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia_home.Models;
using Avalonia_home.Views;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using System.IO;
using System.Net.Http;
using System;
using System.Collections.ObjectModel;

namespace Avalonia_home.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private INumberRegister _numberRegister;
    private Widok1 _view1;
    private Widok2 _view2;
    private Widok3 _view3; 
    private Window? _mainWindow;
    [ObservableProperty] private int _number;
    [ObservableProperty] private string _weatherInfo = "Ładowanie pogody...";
    [ObservableProperty] private string _weatherIconUrl = "";
    [ObservableProperty] private Bitmap? _weatherIcon; //Bitmapa ponieważ URL jest szukany na kompie
    public ObservableCollection<Zadanie> Zadania { get; } = new();
    private readonly ZadaniaService _zadaniaService;

    private void ChangeToView(UserControl view)
    {
        if (_mainWindow != null)
        {
            _mainWindow.Content = view;
        }
    }
    
    public MainWindowViewModel(WidokCreator widokCreator, INumberRegister numberRegister)
    {
        string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Studia\\ProjektAvalonia.mdf;Integrated Security=True;Connect Timeout=30";

        _numberRegister = numberRegister;
        _view1 = widokCreator.CreateWidok1(this);
        _view2 = widokCreator.CreateWidok2(this);
        _view3 = widokCreator.CreateWidok3(this);
        Number = 0;

        _ = LoadWeatherAsync();

        _zadaniaService = new ZadaniaService(connectionString);
        _ = WczytajZadaniaAsync();
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
    public void OnClick3()
    {
        ChangeToView(_view3);
    }

    [RelayCommand]
    public void Zapisz()
    {
        _numberRegister.AddNumber(Number);
    }

    [RelayCommand]
    public async Task LoadWeatherAsync()
    {
        var weatherService = new Weather();
        var info = await weatherService.GetWeatherAsync("Pszczyna");
        if (info != null)
        {
            WeatherInfo = info.Description;

            try
            {
                using var httpClient = new HttpClient();
                var imageBytes = await httpClient.GetByteArrayAsync(info.IconUrl);
                using var ms = new MemoryStream(imageBytes);
                WeatherIcon = new Bitmap(ms);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd ładowania ikony: {ex.Message}");
                WeatherIcon = null;
            }
        }
        else
        {
            WeatherInfo = "Nie udało się pobrać pogody.";
            WeatherIcon = null;
        }
    }
    private async Task WczytajZadaniaAsync()
    {
        var lista = await _zadaniaService.PobierzZadaniaAsync();

        Zadania.Clear();
        foreach (var zadanie in lista)
            Zadania.Add(zadanie);

        if (Zadania.Count == 0)
        {
            Zadania.Add(new Zadanie { Nazwa = "TEST", Zrobione = false });
        }

    }
}