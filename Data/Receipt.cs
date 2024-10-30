using System;
using System.Collections.Generic;
using RetailFixer.Enums;

namespace RetailFixer.Data;

public class Receipt
{
    public uint Number = 0;
    public List<Position> Items = [];
    public ReceiptOperation OpCode = 0;
    public DateTime DateTime = DateTime.MinValue;
    public Payment Payment = new ();
    public Dictionary<string, string> InnerProperties = [];
    public PullSubjects Subject = 0;
    public uint TotalSum = 0;
    public string Operator = "";
    public string FiscalSign = "";

    public string this[string param] => InnerProperties[param];
}