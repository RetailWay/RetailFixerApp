using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace RetailFixer.Converters;

public class ReceiptSourceToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string text && text.Contains("Фронт") ^ text.Contains("ОФД"))
            return Brushes.Goldenrod;
        return Brushes.White;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}