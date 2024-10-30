/*using System;
using DrvFRLib;
using RetailFixer.Attributes;
using RetailFixer.Data;
using RetailFixer.Enums;
using RetailFixer.Exceptions;
using RetailFixer.Interfaces;

namespace RetailFixer.Fiscals;

[DevelopStatus(DevelopStatus.InProcess)]
public sealed class ShtrihM: IFiscal
{
    public string Name => "DrvFR (Штрих-М)";
    public bool IsConnected => _driver.Connected;
    private readonly DrvFR _driver = new();
    
    
    public static InstalledServices IsInstalled
    {
        get
        {
            try
            {
                _ = new DrvFR();
                return InstalledServices.DrvFR;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }

    public SessionStage? Session
    {
        get
        {
            if(_driver.FNGetCurrentSessionParams() != 0) return null;
            return _driver.FNSessionState == 1?SessionStage.Opened:SessionStage.Closed;
            // todo add Expired
        }
    }

    public void SetTypeReceipt(bool isElectronically)
    {
        throw new NotImplementedException();
    }

    public bool OpenSession()
    {
        throw new NotImplementedException();
    }

    public bool CloseSession()
    {
        throw new NotImplementedException();
    }

    public bool CancelReceipt()
    {
        throw new NotImplementedException();
    }

    public uint ConvertTaxRate(int raw)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public bool Connect()
    {
        _driver.Timeout = 155;
        _driver.ProtocolType = 0;
        switch (Settings.FiscalConnect.Type)
        {
            case FiscalConnectionType.SerialPort:
                _driver.ConnectionType = 0;
                if (!Settings.FiscalConnect.Address.StartsWith("COM")) return false;
                _driver.ComNumber = int.Parse(Settings.FiscalConnect.Address[3..]) - 1;
                _driver.ComputerName = "RetailFixer";
                _driver.UseIPAddress = false;
                _driver.BaudRate = 6;
                break;
            case FiscalConnectionType.TcpIp:
                _driver.ConnectionType = 6;
                _driver.UseIPAddress = true;
                _driver.IPAddress = Settings.FiscalConnect.Address;
                _driver.TCPPort = (int)Settings.FiscalConnect.Port!;
                break;
            case FiscalConnectionType.Bluetooth:
            case FiscalConnectionType.Usb:
            default:
                throw new TypeConnectionNotSupportedException();
        }
        if (_driver.Connect() == 0) return true;
        throw new KktException(_driver.ResultCodeDescription);
    }

    public void PullInfo()
    {
        throw new NotImplementedException();
    }

    public bool OpenReceipt(uint opcode, DateTime dt)
    {
        throw new NotImplementedException();
    }

    public bool Registration(Position position)
    {
        throw new NotImplementedException();
    }

    public bool CloseReceipt(Payment payment)
    {
        throw new NotImplementedException();
    }
}*/