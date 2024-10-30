using System;
using RetailFixer.Enums;
using RetailFixer.Interfaces;

namespace RetailFixer.FrontWares.FxPos.Converters;

public class Operation : IConverter<int, ReceiptOperation>
{
    public ReceiptOperation Convert(int src) =>
        src switch
        {
            0 => ReceiptOperation.Income,
            1 => ReceiptOperation.RefundIncome,
            25 => ReceiptOperation.Outcome,
            26 => ReceiptOperation.RefundOutcome,
            _ => throw new ArgumentOutOfRangeException()
        };

    public int ConvertBack(ReceiptOperation src) =>
        src switch
        {
            ReceiptOperation.Income => 0,
            ReceiptOperation.RefundIncome => 1,
            ReceiptOperation.Outcome => 25,
            ReceiptOperation.RefundOutcome => 26,
            _ => throw new ArgumentOutOfRangeException()
        };
}