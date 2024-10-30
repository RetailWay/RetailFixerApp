using RetailFixer.Enums;
using RetailFixer.Interfaces;

namespace RetailFixer.Operators.OfdRu;

public class OperationConverter : IConverter<string, ReceiptOperation>
{
    public ReceiptOperation Convert(string src) =>
        (ReceiptOperation)((src.EndsWith("income") ? 1 : 3) + (src.StartsWith("refund") ? 1 : 0));

    public string ConvertBack(ReceiptOperation src) =>
        throw new System.NotImplementedException();
}