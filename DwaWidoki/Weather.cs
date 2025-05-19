using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Avalonia_home;

internal class Weather
{
    private readonly HttpClient _httpClient = new HttpClient();
    private const string ApiKey = "8c4b9b95ddf0e715e4a900487d78265d"; // <-- zamień na swój klucz
    private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";

    public async Task<WeatherInfo?> GetWeatherAsync(string city = "Pszczyna")
    {
        var url = $"{BaseUrl}?q={city}&appid={ApiKey}&units=metric&lang=pl";
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var weatherElement = doc.RootElement.GetProperty("weather")[0];
        var description = weatherElement.GetProperty("description").GetString();
        var icon = weatherElement.GetProperty("icon").GetString();
        var temp = doc.RootElement.GetProperty("main").GetProperty("temp").GetDecimal();

        return new WeatherInfo
        {
            Description = $"{temp}°C \n {description}",
            Temperature = temp,
            IconUrl = $"https://openweathermap.org/img/wn/{icon}@2x.png"
        };
    }
}
