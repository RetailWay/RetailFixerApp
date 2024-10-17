using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RetailFixer.Configurations;

namespace RetailFixer.Windows;

public sealed partial class FiscalWizard : Window, INotifyPropertyChanged
{
    public byte ConnType
    {
        get => _connType;
        set
        {
            if (_connType != value)
            {
                _connType = value;
                OnPropertyChanged();
            }
        }
    }
    public string SerialPort
    {
        get => _serialPort;
        set
        {
            if (_serialPort != value)
            {
                _serialPort = value;
                OnPropertyChanged();
            }
        }
    }
    public string IpAddress
    {
        get => _ipAddress;
        set
        {
            if (_ipAddress != value)
            {
                _ipAddress = value;
                OnPropertyChanged();
            }
        }
    }
    public ushort? TcpPort
    {
        get => _tcpPort;
        set
        {
            if (_tcpPort != (value ?? 0))
            {
                _tcpPort = value ?? 0;
                OnPropertyChanged();
            }
        }
    }
    public string MacAddress
    {
        get => _macAddress;
        set
        {
            if (_macAddress != value)
            {
                _macAddress = value;
                OnPropertyChanged();
            }
        }
    }
    public string UsbPath
    {
        get => _usbPath;
        set
        {
            if (_usbPath != value)
            {
                _usbPath = value;
                OnPropertyChanged();
            }
        }
    }
    
    private byte _connType = 1;
    private string _serialPort = "";
    private string _ipAddress = "";
    private string _macAddress = "";
    private string _usbPath = "";
    private ushort _tcpPort;

    public string[] SerialPorts => System.IO.Ports.SerialPort.GetPortNames();
    public string[] BluetoothMacs => [];
    
    public new event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    
    public FiscalWizard()
    {
        InitializeComponent();
    }

    private void Apply(object? s, RoutedEventArgs e)
    {
        FiscalConfiguration.Apply(_connType, _serialPort, _usbPath, _ipAddress, _tcpPort, _macAddress);
        Close();
    }

    private void Close(object? s, RoutedEventArgs e) => Close();
}
