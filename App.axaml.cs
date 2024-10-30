using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RetailFixer.Data;
using RetailFixer.Enums;
using RetailFixer.FrontWares.FxPos;
using RetailFixer.Interfaces;
using RetailFixer.Windows;
using Serilog;
using Serilog.Core;

namespace RetailFixer;

public partial class App : Application
{
    public static readonly Logger Logger = new LoggerConfiguration()
        .WriteTo.File(".log", flushToDiskInterval: TimeSpan.FromMilliseconds(100))
        .CreateLogger();
    public static readonly HttpClient Http = new() { Timeout = new TimeSpan(0, 0, 3) };
    public static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
    public static readonly List<Receipt> Receipts = [];
    public static App Singleton { get; private set; }
    
    
    public override void Initialize()
    {
        Singleton = this;
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }
        base.OnFrameworkInitializationCompleted();
    }
}