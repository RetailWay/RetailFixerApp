using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using RetailFixer.Interfaces;

namespace RetailFixer.Windows;

public sealed partial class FrontWareWizard: Window
{
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
    private string _main;
    private string _log;
    private readonly TopLevel _top;

    public FrontWareWizard()
    {
        InstalledIndex = Settings.Soft.SoftId;
        _main = Settings.Soft.PathMainBase;
        _log = Settings.Soft.PathLogBase;
        _top = GetTopLevel(this)!;
        InitializeComponent();
    }

    private void Apply(object? s, RoutedEventArgs e)
    {
        Settings.UpdateSoft(0, PathMain, PathLog);
        Close((IFrontWare)Activator.CreateInstance(Installed[InstalledIndex])!);
    }


    private async void BrowseMain(object? sender, RoutedEventArgs e)
    {
      var opts = new FilePickerOpenOptions()
      {
          AllowMultiple = false,
          Title = "Выбор главной базы FxPOS 3",
          FileTypeFilter = [
              new FilePickerFileType("FxPOS"){ Patterns = ["main.db"] },
              new FilePickerFileType("SQLite"){ Patterns = ["*.db"] },
              new FilePickerFileType("Firebird"){ Patterns = ["*.gdb"] },
          ]
      };
      var files = await _top.StorageProvider.OpenFilePickerAsync(opts);
      if(files.Count == 1) PathMain = files[0].Path.AbsolutePath;
    }
    private async void BrowseLog(object? sender, RoutedEventArgs e)
    {
        var opts = new FilePickerOpenOptions()
        {
            AllowMultiple = false,
            Title = "Выбор базы логирования FxPOS 3",
            FileTypeFilter = [
                new FilePickerFileType("FxPOS"){ Patterns = ["log.db"] },
                new FilePickerFileType("SQLite"){ Patterns = ["*.db"] },
                new FilePickerFileType("Firebird"){ Patterns = ["*.gdb"] },
            ]
        };
        var files = await _top.StorageProvider.OpenFilePickerAsync(opts);
        if(files.Count == 1) PathLog = files[0].Path.AbsolutePath;
    }
    private static Type[] Installed = [];
    private Type Selected => Installed[_installedIndex];
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
    private IEnumerable<string> InstalledNames { get; init; }
    
    private int _installedIndex = 0;
    
    private new event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    
    private void Close(object? s, RoutedEventArgs e) => Close(null);

    public static void SetInstalled(IEnumerable<Type> types)
    {
        if (Installed.Length > 0)
            throw new ReadOnlyException();
        Installed = types.ToArray();
    }
}
