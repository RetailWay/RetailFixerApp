using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace RetailFixer.Windows;

public partial class OperatorWizard : Window, INotifyPropertyChanged
{
    public byte OperatorId
    {
        get => _ofdId;
        set
        {
            if (_ofdId == value) return;
            _ofdId = value;
            OnPropertyChanged();
        }
    }
    public string Token
    {
        get => _token;
        set
        {
            if (_token == value) return;
            _token = value;
            OnPropertyChanged();
        }
    }
    public string Login
    {
        get => _login;
        set
        {
            if (_login == value) return;
            _login = value;
            OnPropertyChanged();
        }
    }
    public string Password
    {
        get => _passwd;
        set
        {
            if (_passwd == value) return;
            _passwd = value;
            OnPropertyChanged();
        }
    }

    private byte _ofdId = 0;
    private string _token = "";
    private string _login = "";
    private string _passwd = "";
    
    public string[] Operators { get; init; } 
    
    public OperatorWizard()
    {
        Operators = Settings.AvailableOperators.Select(i => i.Name).ToArray();
        InitializeComponent();
    }
    
    public new event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private async void Apply(object? sender, RoutedEventArgs e)
    {
        var ofd = Settings.AvailableOperators[_ofdId];
        if (!await ofd.CheckInfo(_token)) return;
        Settings.UpdateOfd(ofd, _token, $"{_login}\n{_passwd}");
        Close();
    }
    private void Cancel(object? sender, RoutedEventArgs e) => Close();
}