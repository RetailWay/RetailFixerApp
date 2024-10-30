using System;

namespace RetailFixer.Operators.OfdRu.Data;

internal struct Receipt
{
    public string OperationType { get; set; }
    public uint DocNumber { get; set; }
    public DateTime DocDateTime { get; set; }
    public string Operator { get; set; }
    public string DecimalFiscalSign { get; set; }
    public Item[] Items { get; set; }
    public uint TotalSumm { get; set; }
    public uint Depth { get; set; }
    public uint CashSumm { get; set; }
    public uint ECashSumm { get; set; }
    public uint PrepaidSumm { get; set; }
    public uint CreditSumm { get; set; }
    // public uint ProvisionSumm { get; set; }
    // public string FnsError { get; set; }
}