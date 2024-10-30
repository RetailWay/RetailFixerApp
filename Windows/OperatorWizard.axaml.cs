using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RetailFixer.Interfaces;

namespace RetailFixer.Windows;

public partial class OperatorWizard : Window
{
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

    private string _token = "";
    private string _login = "";
    private string _passwd = "";

    public OperatorWizard()
    {
        InstalledNames = Installed.Select(i => (string)i.GetProperty("Name")!.GetValue(null)!);
        InitializeComponent();
    }

    private static Type[] Installed = [];
    private Type Selected => Installed[_installedIndex];
    private protected int InstalledIndex
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
    
    private int _installedIndex = 0;
    
    protected new event PropertyChangedEventHandler? PropertyChanged;

    private protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    
    private protected void Close(object? s, RoutedEventArgs e) => Close(null);

    public static void SetInstalled(IEnumerable<Type> types)
    {
        if (Installed.Length > 0)
            throw new ReadOnlyException();
        Installed = types.ToArray();
    }
    
    private async void Apply(object? s, RoutedEventArgs e)
    {
        var ofd = (IOperator)Activator.CreateInstance(Selected)!;
        if (!await ofd.CheckInfo(_token)) return;
        var isAuth = ofd.GetType().GetInterface(nameof(IAuthorization)) is not null;
        Settings.SetAuthDataOperator(isAuth ? $"{_login}{_passwd}" : _token);
        Settings.UpdateInfo("", "", "");
        await ofd.PullReceipts();
        Close(ofd);
    }
}