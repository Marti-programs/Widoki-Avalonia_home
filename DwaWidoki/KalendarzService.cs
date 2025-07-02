using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Avalonia_home;

public class KalendarzService
{
    private readonly string _connectionString;

    public KalendarzService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<WpisKalendarza>> PobierzWpisyZBazyAsync(DateTime miesiac)
    {
        var lista = new List<WpisKalendarza>();
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        string query = "SELECT * FROM Kalendarz WHERE MONTH(Data_wyd) = @Miesiac AND YEAR(Data_wyd) = @Rok";
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Miesiac", miesiac.Month);
        command.Parameters.AddWithValue("@Rok", miesiac.Year);

        var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(new WpisKalendarza
            {
                ID_kalendarz = reader.GetInt32(0),
                Nazwa = reader.GetString(1),
                Opis = reader.GetString(2),
                Kolor = reader.GetString(3),
                Data_wyd = reader.GetDateTime(4)
            });
        }

        return lista;
    }
}
