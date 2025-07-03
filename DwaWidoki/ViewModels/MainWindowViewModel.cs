using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia_home.Models;
using Avalonia_home.Views;

namespace Avalonia_home.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly INumberRegister _numberRegister;
        private readonly Widok1 _view1;
        private readonly Widok2 _view2;
        private readonly Widok3 _view3;
        private Window? _mainWindow;

        [ObservableProperty] private int _number;
        [ObservableProperty] private string _weatherInfo = "Ładowanie pogody...";
        [ObservableProperty] private string _weatherIconUrl = "";
        [ObservableProperty] private Bitmap? _weatherIcon;
        [ObservableProperty] private string _nowaNazwaZadania;

        public ObservableCollection<Zadanie> Zadania { get; } = new();
        public ObservableCollection<ObservableCollection<DzienWidoku>> Tygodnie { get; }
            = new ObservableCollection<ObservableCollection<DzienWidoku>>();

        private const string ConnectionString =
            "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Studia\\ProjektAvalonia.mdf;" +
            "Integrated Security=True;Connect Timeout=30";

        private readonly ZadaniaService _zadaniaService;

        public MainWindowViewModel(WidokCreator widokCreator, INumberRegister numberRegister)
        {
            _numberRegister = numberRegister;
            _zadaniaService = new ZadaniaService(ConnectionString);

            _view1 = widokCreator.CreateWidok1(this);
            _view2 = widokCreator.CreateWidok2(this);
            _view3 = widokCreator.CreateWidok3(this);

            Number = 0;
            _ = LoadWeatherAsync();
            _ = WczytajZadaniaAsync();
            _ = WczytajDniKalendarzaAsync();
        }

        public void SetMainWindow(Window mainWindow) => _mainWindow = mainWindow;

        public void Init() => ChangeToView(_view1);

        private void ChangeToView(UserControl view)
        {
            if (_mainWindow != null)
                _mainWindow.Content = view;
        }

        [RelayCommand]
        public void OnClick1() => ChangeToView(_view1);

        [RelayCommand]
        public void OnClick2() => ChangeToView(_view2);

        [RelayCommand]
        public void OnClick3() => ChangeToView(_view3);

        [RelayCommand]
        public void Zapisz() => _numberRegister.AddNumber(Number);

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
                    var bytes = await httpClient.GetByteArrayAsync(info.IconUrl);
                    using var ms = new MemoryStream(bytes);
                    WeatherIcon = new Bitmap(ms);
                }
                catch
                {
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
            foreach (var z in lista)
                Zadania.Add(z);

            if (!Zadania.Any())
                Zadania.Add(new Zadanie { Nazwa = "TEST", Zrobione = false });
        }

        [RelayCommand]
        public async Task DodajZadanieAsync()
        {
            if (string.IsNullOrWhiteSpace(NowaNazwaZadania))
                return;

            var nowe = new Zadanie { Nazwa = NowaNazwaZadania, Zrobione = false };

            try
            {
                using var conn = new SqlConnection(ConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqlCommand(
                    "INSERT INTO ZadaniaDoZrobienia (Nazwa, Zrobione) VALUES (@nazwa, @zrobione)",
                    conn);
                cmd.Parameters.AddWithValue("@nazwa", nowe.Nazwa);
                cmd.Parameters.AddWithValue("@zrobione", nowe.Zrobione);
                await cmd.ExecuteNonQueryAsync();

                Zadania.Add(nowe);
                NowaNazwaZadania = string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd dodawania zadania: {ex.Message}");
            }
        }

        public async Task WczytajDniKalendarzaAsync()
        {
            try
            {
                // Pobierz wpisy z bazy danych
                var wpisy = await new KalendarzService(ConnectionString)
                    .PobierzWpisyZBazyAsync(DateTime.Now);

                // Wygeneruj siatkę dni
                GenerateMonthGrid(DateTime.Now.Year, DateTime.Now.Month);

                // Pogrupuj wpisy według dnia
                var grouped = wpisy
                    .GroupBy(w => w.Data_wyd.Day)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Przypisz wydarzenia i kolory do dni
                foreach (var tydzien in Tygodnie)
                {
                    foreach (var dzien in tydzien)
                    {
                        if (string.IsNullOrWhiteSpace(dzien.DzienTekst))
                        {
                            dzien.Kolor = "#E0E0E0"; // szary dla pustych pól
                            continue;
                        }

                        if (int.TryParse(dzien.DzienTekst, out int dayNum)
                            && grouped.TryGetValue(dayNum, out var ev))
                        {
                            dzien.Wydarzenia = ev.Select(e => e.Nazwa).ToList();
                            dzien.Kolor = "#D9EAFB"; // niebieski jeśli są wydarzenia
                        }
                        else
                        {
                            dzien.Kolor = "White"; // domyślny biały dzień bez wydarzeń
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd ładowania wydarzeń: {ex.Message}");
            }
        }

        private void GenerateMonthGrid(int year, int month)
        {
            Tygodnie.Clear();
            var firstDay = new DateTime(year, month, 1);
            int offset = ((int)firstDay.DayOfWeek + 6) % 7; // poniedziałek = 0
            int days = DateTime.DaysInMonth(year, month);

            var buffer = new List<DzienWidoku>();

            // pustki przed 1.
            for (int i = 0; i < offset; i++)
                buffer.Add(new DzienWidoku { DzienTekst = "", Kolor = "#E0E0E0" });

            // dni miesiąca
            for (int d = 1; d <= days; d++)
                buffer.Add(new DzienWidoku { DzienTekst = d.ToString(), Kolor = "White" });

            // dopełnij do pełnych tygodni
            while (buffer.Count % 7 != 0)
                buffer.Add(new DzienWidoku { DzienTekst = "", Kolor = "#E0E0E0" });

            // podziel na tygodnie
            for (int i = 0; i < buffer.Count; i += 7)
            {
                var tydzien = new ObservableCollection<DzienWidoku>(
                    buffer.Skip(i).Take(7));
                Tygodnie.Add(tydzien);
            }
        }
    }
}