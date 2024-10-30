using System;
using System.Text.RegularExpressions;

namespace RetailFixer.Exceptions;

/// <summary>Нарушение формата MAC-адреса</summary>
public partial class ControlAddressFormatException() : FormatException("Нарушен формат MAC-адреса.")
{
    public static void ThrowIfIncorrect(string text)
    {
        if (MacAddressRegex().IsMatch(text)) throw new ControlAddressFormatException();
    }

    [GeneratedRegex(@"^([0-9A-Fa-f]{2}:){5}([0-9A-Fa-f]{2})$")]
    private static partial Regex MacAddressRegex();
}