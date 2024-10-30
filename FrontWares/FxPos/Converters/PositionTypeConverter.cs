using System;
using RetailFixer.Interfaces;

namespace RetailFixer.FrontWares.FxPos.Converters;

public class PositionType : IConverter<int, Enums.PositionType>
{
    public Enums.PositionType Convert(int src) => 
        src switch
        {
            1 => Enums.PositionType.Product,
            2 => Enums.PositionType.Excise,
            3 => Enums.PositionType.Work,
            4 => Enums.PositionType.Service,
            10 or 16 => Enums.PositionType.Payment,
            12 => Enums.PositionType.Other,
            13 => Enums.PositionType.AgencyRemuneration,
            14 => Enums.PositionType.LotteryTicket,
            15 => Enums.PositionType.ResultIntellectual,
            17 => Enums.PositionType.ResortFee,
            18 => Enums.PositionType.IssueMoney,
            _ => throw new ArgumentOutOfRangeException()
        };

    public int ConvertBack(Enums.PositionType src) => 
        src switch
        {
            Enums.PositionType.Product => 1,
            Enums.PositionType.Excise => 2,
            Enums.PositionType.Work => 3,
            Enums.PositionType.Service => 4,
            Enums.PositionType.Payment => 10,
            Enums.PositionType.Other => 12,
            Enums.PositionType.AgencyRemuneration => 13,
            Enums.PositionType.LotteryTicket => 14,
            Enums.PositionType.ResultIntellectual => 15,
            Enums.PositionType.ResortFee => 17,
            Enums.PositionType.IssueMoney => 18,
            _ => throw new ArgumentOutOfRangeException()
        };
}