using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace RetailFixer.Converters;

public class DateOnlyConverter : IValueConverter
{
    public object? Convert(object? v, Type t, object? p, CultureInfo c) =>
        v is DateOnly d ? d.ToDateTime(new TimeOnly()) : DateTime.Today;

    public object? ConvertBack(object? v, Type t, object? p, CultureInfo c) =>
        DateOnly.FromDateTime((DateTime?)v ?? DateTime.Today);
}