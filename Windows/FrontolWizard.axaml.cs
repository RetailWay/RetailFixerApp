using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using RetailFixer.Enums;
using RetailFixer.Exceptions;
using RetailFixer.Soft;

namespace RetailFixer.Windows;

public sealed partial class FrontolWizard : Window, INotifyPropertyChanged
{
    public byte SoftId
    {
        get => _softId;
        set
        {
            if (_softId == value) return;
            _softId = value;
            OnPropertyChanged();
        }
    }
    public string PathMain
    {
        get => _main;
        set
        {
            if (_main == value) return;
            _main = value;
            OnPropertyChanged();
        }
    }
    public string PathLog
    {
        get => _log;
        set
        {
            if (_log == value) return;
            _log = value;
            OnPropertyChanged();
        }
    }

    private byte _softId;
    private string _main;
    private string _log;
    
    public new event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public FrontolWizard()
    {
        _softId = Settings.Soft.SoftId;
        _main = Settings.Soft.PathMainBase;
        _log = Settings.Soft.PathLogBase;
        InitializeComponent();
    }

    private void Apply(object? s, RoutedEventArgs e)
    {
        Settings.UpdateSoft(0, PathMain, PathLog);
        FxPOS pos = new(PathMain);
        pos.Start(new DateOnly(2024, 07, 16), new DateOnly(2024, 07, 20));
        Close();
    }

    private void Close(object? s, RoutedEventArgs e) => Close();

    private async void BrowseMain(object? sender, RoutedEventArgs e)
    {
  /*      var opts = new FilePickerOpenOptions
        {
            FileTypeFilter = [new FilePickerFileType("*.db"), new FilePickerFileType("*.gdb")],
            AllowMultiple = false
        };
        var files = await StorageProvider.OpenFilePickerAsync(opts);
        PathMain = files[0].Path.ToString();
*/
        PathMain = (await new OpenFileDialog
        {
            Filters = [new FileDialogFilter { Extensions = ["db", "gdb"] }]
        }.ShowAsync(this))[0];
    }
    private async void BrowseLog(object? sender, RoutedEventArgs e)
    {/*
        var opts = new FilePickerOpenOptions
        {
            FileTypeFilter = [new FilePickerFileType("*.db"), new FilePickerFileType("*.gdb")],
            AllowMultiple = false
        };
        var files = await StorageProvider.OpenFilePickerAsync(opts);
        PathLog = files[0].Path.ToString();*/
        PathLog = (await new OpenFileDialog
        {
            Filters = [new FileDialogFilter { Extensions = ["db", "gdb"] }]
        }.ShowAsync(this))[0];
    }
}
