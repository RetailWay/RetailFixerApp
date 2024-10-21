using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using RetailFixer.Data;
using RetailFixer.Enums;
using RetailFixer.Exceptions;

namespace RetailFixer.Utils;

public static partial class CheckData
{
    /// <summary>
    /// Проверка объекта FiscalConnectionData
    /// </summary>
    /// <param name="data">Созданный объект</param>
    /// <exception cref="SerialPortFormatException">Некорректный формат номера последовательного порта</exception>
    /// <exception cref="NetworkAddressFormatException">Некорректный формат IP-адреса</exception>
    /// <exception cref="ControlAddressFormatException">Некорректный формат MAC-адреса</exception>
    /// <exception cref="ArgumentException">Отсутствует адрес/путь</exception>
    /// <exception cref="ArgumentNullException">Отсутствует номер TCP-порта при TCP/IP-подключении</exception>
    /// <exception cref="InvalidEnumArgumentException">Неизвестный тип подключения</exception>
    public static FiscalConnectionData Check(FiscalConnectionData data)
    {
        switch (data.Type)
        {
            case FiscalConnectionType.SerialPort:
                ArgumentException.ThrowIfNullOrWhiteSpace(data.Address);
                if (!SerialPortRegex().IsMatch(data.Address))
                    throw new SerialPortFormatException();
                break;
            case FiscalConnectionType.Usb:
                break;
            case FiscalConnectionType.TcpIp:
                ArgumentException.ThrowIfNullOrWhiteSpace(data.Address);
                if (!NetworkAddressRegex().IsMatch(data.Address))
                    throw new NetworkAddressFormatException();
                ArgumentNullException.ThrowIfNull(data.Port);
                break;
            case FiscalConnectionType.Bluetooth:
                ArgumentException.ThrowIfNullOrWhiteSpace(data.Address);
                if (!ControlAddressRegex().IsMatch(data.Address))
                    throw new ControlAddressFormatException();
                break;
            default:
                throw new InvalidEnumArgumentException();
        }

        return data;
    }

    #region RegExp
    [GeneratedRegex(@"^([0-9A-F]{2}:){5}[0-9A-F]{2}$")]
    private static partial Regex ControlAddressRegex();
    [GeneratedRegex(@"^([0-9]{1,3}\.){3}[0-9]{1,3}$")]
    private static partial Regex NetworkAddressRegex();
    [GeneratedRegex(@"^COM\d{1,3}$")]
    private static partial Regex SerialPortRegex();
    #endregion
}