using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace RetailFixer.Converters;

public class ReceiptActionToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string text)
        {
            return text switch
            {
                "S" => Brushes.SpringGreen,
                "C" => Brushes.IndianRed,
                _ => Brushes.White
            };
        }
        return Brushes.White;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}