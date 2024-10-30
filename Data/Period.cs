using System;

namespace RetailFixer.Data;

public class Period
{
    public DateTimeOffset Start { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset End { get; set; } = DateTimeOffset.Now;
}