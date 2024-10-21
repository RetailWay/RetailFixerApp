using System;
using System.Collections.Generic;
using RetailFixer.Enums;

namespace RetailFixer.Data;

public class Receipt
{
    public uint Number = 0;
    public List<Position> Items = [];
    public byte OpCode = 0;
    public DateTime DateTime = DateTime.MinValue;
    public (uint c, uint e, uint cr, uint pre) Payment = (0, 0, 0, 0);
    public Dictionary<string, string> InnerProperties = [];
    public PullSubjects Subject = 0;

    public string this[string param] => InnerProperties[param];

/*
    public void CloseOnKkm()
    {
        if (!Settings.FiscalDriver.OpenReceipt(OpCode, DateTime)) return;
        foreach (var item in Items)
        {
            if (!Settings.FiscalDriver.Registration(item))
            {
                Settings.FiscalDriver.CancelReceipt();
                return;
            }
        }

        if (Settings.FiscalDriver.CloseReceipt(Payment.c, Payment.e, Payment.cr, Payment.pre))
            Settings.FiscalDriver.CancelReceipt();
    }*/
}