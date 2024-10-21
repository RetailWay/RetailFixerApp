using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RetailFixer.Enums;
using RetailFixer.Exceptions;

namespace RetailFixer.Windows;

public sealed partial class FiscalWizard : Window, INotifyPropertyChanged
{
    public byte DriverId
    {
        get => (byte)_driverId;
        set
        {
            if (_driverId == value) return;
            _driverId = value;
            OnPropertyChanged();
        }
    }
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

    private int _driverId = 0;
    private FiscalConnectionType _connType = FiscalConnectionType.Usb;
    private string _serialPort = "";
    private string _ipAddress = "";
    private string _macAddress = "";
    private string _usbPath = "";
    private ushort? _tcpPort;

    internal string[] SerialPorts => System.IO.Ports.SerialPort.GetPortNames();
    internal string[] BluetoothMacs => [];
    internal string[] Drivers { get; init; }


    private string Address => _connType switch
    {
        FiscalConnectionType.SerialPort => _serialPort,
        FiscalConnectionType.Usb => _usbPath,
        FiscalConnectionType.TcpIp => _ipAddress,
        FiscalConnectionType.Bluetooth => _macAddress,
        _ => ""
    };
    
    public new event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public FiscalWizard()
    {
        Drivers = Settings.AvailableDrivers.Select(i => i.Name).ToArray();
        InitializeComponent();
    }
    
    private void Apply(object? s, RoutedEventArgs e)
    {
        var driver = Settings.AvailableDrivers[_driverId];
        try
        {
            var prepConf = Settings.FiscalConnect;
            Settings.UpdateFiscal(_connType, Address, _tcpPort);
            if (driver.Connect())
            {
                Settings.FiscalDriver = driver;
                Close();
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
                InvalidEnumArgumentException => "Произошёл баг! Сообщите разработчику! (Код: 001)",
                NetworkAddressFormatException => "Введённый IP-адрес не соотвествует формату",
                ControlAddressFormatException => "Произошёл баг! Сообщите разработчику! (Код: 002)",
                _ => "Произошла неизвестная ошибка"
            };
            new Alert("Не удалось применить настройки", message, AlertLevel.Error).ShowDialog(this);
            App.Logger.Error($"При применении настроек произошла ошибка: {ex.Message}");
        }
    }

    private void Close(object? s, RoutedEventArgs e) => Close();
}
