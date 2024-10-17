using System;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RetailFixer.Configurations;
using RetailFixer.Windows;
using Serilog;
using Serilog.Core;

namespace RetailFixer;

public partial class App : Application
{
    public static readonly Logger Logger = new LoggerConfiguration().WriteTo.File(".log").CreateLogger();
    public static readonly HttpClient Http = new() { Timeout = new TimeSpan(0, 0, 3) };
    
    public static FiscalConfiguration Fiscal;
    public static OperatorConfiguration Operator;
    
    public override void Initialize()
    {
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