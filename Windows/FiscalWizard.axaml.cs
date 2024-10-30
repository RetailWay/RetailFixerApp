using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RetailFixer.Enums;
using RetailFixer.Exceptions;
using RetailFixer.Interfaces;

namespace RetailFixer.Windows;

public sealed partial class FiscalWizard : Window
{
    public byte ConnType
    {
        get => (byte)_connType;
        set
        {
            if ((byte)_connType == value) return;
            _connType = (FiscalConnectionType)value;
            OnPropertyChanged();
        }
    }
    public string SerialPort
    {
        get => _serialPort ?? string.Empty;
        set
        {
            if (_serialPort == value) return;
            _serialPort = value;
            OnPropertyChanged();
        }
    }
    public string IpAddress
    {
        get => _ipAddress ?? string.Empty;
        set
        {
            if (_ipAddress == value) return;
            _ipAddress = value;
            OnPropertyChanged();
        }
    }
    public ushort? TcpPort
    {
        get => _tcpPort;
        set
        {
            if (_tcpPort == value) return;
            _tcpPort = value;
            OnPropertyChanged();
        }
    }
    public string MacAddress
    {
        get => _macAddress ?? string.Empty;
        set
        {
            if (_macAddress == value) return;
            _macAddress = value;
            OnPropertyChanged();
        }
    }
    public string UsbPath
    {
        get => _usbPath ?? string.Empty;
        set
        {
            if (_usbPath == value) return;
            _usbPath = value;
            OnPropertyChanged();
        }
    }
    public int InstalledIndex
    {
        get => _installedIndex;
        set
        {
            if (_installedIndex == value) return;
            _installedIndex = value;
            OnPropertyChanged();
        }
    }
    private protected IEnumerable<string> InstalledNames { get; init; }

    private FiscalConnectionType _connType = FiscalConnectionType.Usb;
    private string _serialPort = "";
    private string _ipAddress = "";
    private string _macAddress = "";
    private string _usbPath = "";
    private ushort? _tcpPort;
    private static Type[] Installed = [];
    private Type Selected => Installed[_installedIndex];
    private int _installedIndex = 0;
    
    protected new event PropertyChangedEventHandler? PropertyChanged;

    private protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    internal string[] SerialPorts { get; init; }
    internal string[] BtAddresses { get; init; }


    private string Address => _connType switch
    {
        FiscalConnectionType.SerialPort => _serialPort,
        FiscalConnectionType.Usb => _usbPath,
        FiscalConnectionType.TcpIp => _ipAddress,
        FiscalConnectionType.Bluetooth => _macAddress,
        _ => ""
    };
    public FiscalWizard()
    {
        InstalledNames = Installed.Select(i => (string)i.GetProperty("Name")!.GetValue(null)!);
        SerialPorts = System.IO.Ports.SerialPort.GetPortNames();
        BtAddresses = [];
        InitializeComponent();
    }
    
    private void Apply(object? s, RoutedEventArgs e)
    {
        var driver = (IFiscal)Activator.CreateInstance(Selected)!;
        try
        {
            var prepConf = Settings.FiscalConnect;
            Settings.UpdateFiscal(_connType, Address, _tcpPort);
            if (driver.Connect())
            {
                Close(driver);
            }
            else
            {
                App.Logger.Warning("Не удалось подключиться к ККМ");
                Settings.UpdateFiscal(prepConf.Type, prepConf.Address, prepConf.Port);
            }
        }
        catch (Exception ex)
        {
            var message = ex switch
            {
                ArgumentNullException ane => $"Отсутствует обязательное значение: {ane.ParamName}",
                TypeConnectionNotSupportedException => "Данный тип подключения не поддерживается выбранным драйвером.",
                NetworkAddressFormatException => "Введённый IP-адрес не соотвествует формату",
                ControlAddressFormatException => "Произошёл баг! Сообщите разработчику! (Код: 002)",
                KktException => $"Возникла ошибка ККТ: {ex.Message}",
                _ => "Произошла неизвестная ошибка"
            };
            Alert.Show("Не удалось применить настройки", message, AlertLevel.Error);
            App.Logger.Error($"При применении настроек произошла ошибка: {ex.Message}");
        }
    }
    
    private protected void Close(object? s, RoutedEventArgs e) => Close(null);

    public static void SetInstalled(IEnumerable<Type> types)
    {
        if (Installed.Length > 0)
            throw new ReadOnlyException();
        Installed = types.ToArray();
    }
}
