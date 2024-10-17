using System;
using System.Collections.Generic;

namespace RetailFixer.Data;

public struct Receipt
{
    public List<Position> Items;
    public int OpCode;
    public DateTime DateTime;
    public (uint c, uint e, uint cr, uint pre) Payment;
}