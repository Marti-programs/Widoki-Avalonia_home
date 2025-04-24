using System.Collections.Generic;

namespace Avalonia_home.Models;

public interface INumberRegister
{
    void AddNumber(int n);
    ICollection<int> GetNumbers();
}
