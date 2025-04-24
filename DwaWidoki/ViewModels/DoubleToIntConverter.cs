namespace Avalonia_home.ViewModels;

using Avalonia.Data.Converters;
using System;
using System.Globalization;

public class DoubleToIntConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Convert int to double for the Slider
        if (value is int intValue)
            return (double)intValue;

        return 0.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Convert double to int for the ViewModel
        if (value is double doubleValue)
            return (int)doubleValue;

        return 0;
    }
}