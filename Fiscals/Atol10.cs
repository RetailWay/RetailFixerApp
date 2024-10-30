/*using System;
using System.Linq;
using System.Text;
using Fptr10Lib;
using RetailFixer.Attributes;
using RetailFixer.Data;
using RetailFixer.Enums;
using RetailFixer.Exceptions;
using RetailFixer.Interfaces;

namespace RetailFixer.Fiscals;

[DevelopStatus(DevelopStatus.Testing)]
public sealed class Atol10 : IFiscal
{
    #region Публичные методы

    public static string Name => "FPTR (Atol v.10)";
    public bool IsConnected => _driver.isOpened();
    private readonly Fptr _driver = new();
    public static InstalledServices IsInstalled
    {
        get
        {
            try
            {
                _ = new Fptr();
                return InstalledServices.Fptr;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }

    public void Dispose()
    {
        _driver.close();
        _driver.destroy();
    }

    public SessionStage? Session
    {
        get
        {
            _driver.setParam(65587, 14);
            if (_driver.queryData() != 0) return null;
            return _driver.getParamInt(65592) switch
            {
                0 => SessionStage.Closed,
                1 => SessionStage.Opened,
                2 => SessionStage.Expired,
                _ => null
            };
        }
    }

    private int Payment(uint type, uint sum)
    {
        var dSum = sum / 100.0;
        App.Logger.Information($"Оплата {{\"type\": {type}, \"sum\": {dSum}}}");
        _driver.setParam(65564,type);
        _driver.setParam(65565,dSum);
        return _driver.payment();
    }
    public void PullInfo()
    {
        _driver.setParam(65622, 9);
        if(_driver.fnQueryData() != 0) return;
        _driver.setParam(65622, 2);
        if(_driver.fnQueryData() != 0) return;
        var vatin = _driver.getParamStr(1018);
        var regId = _driver.getParamStr(1037);
        var storageId = _driver.getParamStr(65559);
        Settings.UpdateInfo(vatin, storageId, regId);
    }

    public void SetTypeReceipt(bool isElectronically) =>
        _driver.setParam(65572, isElectronically);

    public bool OpenSession() =>
        _driver.openShift() != 0;

    public bool CloseSession()
    {
        _driver.setParam(65546, 0);
        return _driver.report() != 0;
    }

    public bool CancelReceipt() => _driver.cancelReceipt() != 0;

    public bool Connect()
    {
        #region Настройка драйвера

        _driver.setSingleSetting("Model", "500");
        _driver.setSingleSetting("Port", $"{Settings.FiscalConnect.Type}");
        switch (Settings.FiscalConnect.Type)
        {
            case FiscalConnectionType.SerialPort:
                _driver.setSingleSetting("BaudRate", "115200");
                _driver.setSingleSetting("ComFile", Settings.FiscalConnect.Address);
                break;
            case FiscalConnectionType.Usb:
                if (!string.IsNullOrWhiteSpace(Settings.FiscalConnect.Address))
                    _driver.setSingleSetting("UsbDevicePath", Settings.FiscalConnect.Address);
                break;
            case FiscalConnectionType.TcpIp:
                _driver.setSingleSetting("IPAddress", Settings.FiscalConnect.Address);
                _driver.setSingleSetting("IPPort", $"{Settings.FiscalConnect.Port!}");
                break;
            case FiscalConnectionType.Bluetooth:
                _driver.setSingleSetting("MACAddress", Settings.FiscalConnect.Address);
                break;
            default:
                throw new TypeConnectionNotSupportedException();
        }

        #endregion

        return _driver.open() != 0;
    }

    public bool OpenReceipt(uint opcode, DateTime dt)
    {
        #region Создание основания коррекции

        _driver.setParam(1178, dt);
        _driver.setParam(1179, Encoding.Unicode.GetBytes(" "));
        if (_driver.utilFormTlv() != 0) return false;
        var corrInfo = _driver.getParamByteArray(65624);

        #endregion

        _driver.setParam(65545, opcode + 6);
        _driver.setParam(1173, 0);
        _driver.setParam(1174, corrInfo);
        return _driver.openReceipt() == 0;
    }

    private static uint ConvertTaxRate(byte raw) =>
        raw switch
        {
            255 => 6,
            0 => 5,
            10 => 2,
            20 => 7,
            110 => 4,
            120 => 8,
            _ => throw new Exception()
        };

    public bool Registration(Position position)
    {
        _driver.setParam(65631, Encoding.UTF8.GetBytes(position.Name));
        _driver.setParam(65632, position.Price / 100.0);
        _driver.setParam(65633, position.Quantity / 1000.0);
        _driver.setParam(65569, ConvertTaxRate(position.TaxRate));
        _driver.setParam(65654, position.Quantity % 1000 == 0);
        _driver.setParam(2108, (uint)position.Measure);
        _driver.setParam(1212, (uint)position.Type);
        _driver.setParam(1214, (uint)position.PaymentType);
        _driver.setParam(65634, position.Total / 100.0);
        return _driver.registration() == 0;
    }

    public bool CloseReceipt(Payment payment)
    {
        if (payment.Cash > 0 && Payment(0, payment.Cash) != 0) return false;
        if (payment.ECash > 0 && Payment(1, payment.ECash) != 0) return false;
        if (payment.Credit > 0 && Payment(3, payment.Credit) != 0) return false;
        if (payment.Prepaid > 0 && Payment(2, payment.Prepaid) != 0) return false;
        if (payment.Other.Any(other => other.Sum > 0 && Payment(other.Type, other.Sum) != 0))
            return false;
        return _driver.closeReceipt() == 0;
    }

    #endregion
}*/