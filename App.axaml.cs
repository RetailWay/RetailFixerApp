using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using RetailFixer.Data;
using RetailFixer.Fiscals;
using RetailFixer.Interfaces;
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
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        AddFiscalServices(services);
        var provider = services.BuildServiceProvider();
        Settings.TrySetAvailableDrivers(provider.GetServices<IFiscal>());
        Settings.TrySetAvailableOperators(provider.GetServices<IOperator>());
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void AddFiscalServices(ServiceCollection collection)
    {
        foreach (var inter in new[] {typeof(IFiscal), typeof(IOperator)})
        {
            foreach (var t in Assembly.GetTypes().Where(i => i.GetInterface(nameof(inter)) is not null))
            {
                try
                {
                    collection.AddSingleton(inter, t);
                }
                catch (Exception e)
                {
                    Logger.Error($"Не удалось добавить сервис {t.FullName}");
                }
            }
        }
    }
}