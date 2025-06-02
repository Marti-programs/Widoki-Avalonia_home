using System;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia_home;

public class ZadaniaService
{
    private readonly string _connectionString;

    public ZadaniaService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<Zadanie>> PobierzZadaniaAsync()
    {
        var lista = new List<Zadanie>();

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var command = new SqlCommand("SELECT Id, Nazwa, Zrobione FROM ZadaniaDoZrobienia WHERE Zrobione = 0", connection);
        var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            lista.Add(new Zadanie
            {
                Id = reader.GetInt32(0),
                Nazwa = reader.GetString(1),
                Zrobione = reader.GetBoolean(2)
            });
        }

        return lista;
    }
}
