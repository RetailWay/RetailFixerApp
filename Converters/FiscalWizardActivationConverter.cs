using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace RetailFixer.Converters;

public class FiscalWizardActivationConverter : IValueConverter
{
    public object? Convert(object? value, Type t, object? parameter, CultureInfo c)
    {
        return (byte)value! == byte.Parse((string?)parameter ?? string.Empty);
    }

    public object? ConvertBack(object? v, Type t, object? p, CultureInfo c) =>
        throw new NotImplementedException();
}