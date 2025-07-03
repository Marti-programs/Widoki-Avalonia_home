using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia_home.Models;

public class DzienWidoku
{
    public string DzienTekst { get; set; } = "";
    public string Kolor { get; set; } = "White"; // domyślnie biały
    public List<string> Wydarzenia { get; set; } = new();
}

