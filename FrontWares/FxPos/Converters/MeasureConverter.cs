using System.Collections.Generic;
using RetailFixer.Enums;
using RetailFixer.Interfaces;

namespace RetailFixer.FrontWares.FxPos.Converters;

public class Measure : IConverter<int, MeasureUnit>
{
    private readonly List<MeasureUnit> _arr =
    [
        MeasureUnit.Piece,
        MeasureUnit.Gram,
        MeasureUnit.Kilogram,
        MeasureUnit.Ton,
        MeasureUnit.Centimeter,
        MeasureUnit.Decimeter,
        MeasureUnit.Meter,
        MeasureUnit.SquareCentimeter,
        MeasureUnit.SquareDecimeter,
        MeasureUnit.SquareMeter,
        MeasureUnit.Milliliter,
        MeasureUnit.Liter,
        MeasureUnit.CubicMeter,
        MeasureUnit.KilowattHour,
        MeasureUnit.GKal,
        MeasureUnit.Day,
        MeasureUnit.Hour,
        MeasureUnit.Minute,
        MeasureUnit.Second,
        MeasureUnit.Kilobyte,
        MeasureUnit.Megabyte,
        MeasureUnit.Gigabyte,
        MeasureUnit.Terabyte,
        MeasureUnit.Other
    ];

    public MeasureUnit Convert(int src) => _arr[src];

    public int ConvertBack(MeasureUnit src) => _arr.IndexOf(src);
}