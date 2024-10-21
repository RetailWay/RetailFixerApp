using System;
using System.Text.RegularExpressions;

namespace RetailFixer.Exceptions;

/// <summary>Нарушение формата IP-адреса</summary>
public partial class NetworkAddressFormatException() : FormatException("Нарушен формат IP-адреса.")
{
    public static void ThrowIfIncorrect(string text)
    {
        if (!IpAddressRegex().IsMatch(text)) throw new NetworkAddressFormatException();
    }

    [GeneratedRegex(@"^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)\.?\b){4}$")]
    private static partial Regex IpAddressRegex();
}