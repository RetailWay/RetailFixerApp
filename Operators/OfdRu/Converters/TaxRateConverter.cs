using System;
using RetailFixer.Interfaces;

namespace RetailFixer.Operators.OfdRu;

public class TaxRateConverter: IConverter<int, byte>
{
    public byte Convert(int src) =>
        src switch
        {
            1 => 20,
            2 => 10,
            3 => 120,
            4 => 110,
            5 => 0,
            6 => 255,
            _ => throw new ArgumentOutOfRangeException(nameof(src)) 
        };
        

    public int ConvertBack(byte src) => -1;
}