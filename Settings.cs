using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using RetailFixer.Data;
using RetailFixer.Enums;
using RetailFixer.Exceptions;
using RetailFixer.Interfaces;
using RetailFixer.Soft;
using RetailFixer.Utils;

namespace RetailFixer;

public static class Settings
{
    public static FiscalConnectionData FiscalConnect { get; private set; }
    public static OperatorSettings Ofd { get; private set; }
    public static FiscalSoftSettings Soft { get; private set; }
    public static IFiscal[] AvailableDrivers { get; private set; } = [];
    public static IOperator[] AvailableOperators { get; private set; } = [];
    public static IFiscal FiscalDriver { get; set; } = null!;
    public static (DateTime from, DateTime to) SearchPeriod { get; private set; } = (DateTime.Today, DateTime.Today);
    public static FiscalInfo Info { get; private set; } = new("", "", "");
    public static FxPOS SoftWare { get; private set; } = null!;

    /// <summary>
    /// Обновление настройки подключения ККТ
    /// </summary>
    /// <param name="type">Тип подключения</param>
    /// <param name="address">Адрес или путь</param>
    /// <param name="port">TCP-порт</param>
    /// <exception cref="SerialPortFormatException">Некорректный формат номера последовательного порта</exception>
    /// <exception cref="NetworkAddressFormatException">Некорректный формат IP-адреса</exception>
    /// <exception cref="ControlAddressFormatException">Некорректный формат MAC-адреса</exception>
    /// <exception cref="ArgumentException">Отсутствует адрес/путь</exception>
    /// <exception cref="ArgumentNullException">Отсутствует номер TCP-порта при TCP/IP-подключении</exception>
    /// <exception cref="InvalidEnumArgumentException">Неизвестный тип подключения</exception>
    public static void UpdateFiscal(FiscalConnectionType type, string address, ushort? port) =>
        FiscalConnect = CheckData.Check(new FiscalConnectionData(type, address, port));

    public static void UpdateOfd(IOperator ofd, string? token = null, string? authData = null)
    {/*
        if (token is null && authData is null)
            throw new AllOptionalArgumentsNullException(nameof(token), nameof(authData));*/
        Ofd = new OperatorSettings(ofd, token, authData);
    }

    public static void UpdateSoft(byte softId, string mainPath, string logPath) =>
        Soft = new FiscalSoftSettings(softId, mainPath, logPath);

    public static void UpdateInfo(string vatin, string storageId, string regId) =>
        Info = new FiscalInfo(vatin, storageId, regId);

    public static bool TrySetAvailableDrivers(IEnumerable<IFiscal> drivers)
    {
        if (AvailableDrivers.Length > 0) return false;
        AvailableDrivers = drivers.ToArray();
        return true;
    }
    public static bool TrySetAvailableOperators(IEnumerable<IOperator> operators)
    {
        if (AvailableOperators.Length > 0) return false;
        AvailableOperators = operators.ToArray();
        return true;
    }
    public static bool TrySetSoftWare(FxPOS? ware)
    {
        if (ware is null || SoftWare != null!) return false; 
        SoftWare = ware;
        return true;
    }
}

public record struct OperatorSettings(IOperator Operator, string? AuthToken = null, string? AuthData = null);
public record struct FiscalSoftSettings(byte SoftId, string PathMainBase, string PathLogBase);
public record struct FiscalInfo(string Vatin, string StorageId, string RegId);