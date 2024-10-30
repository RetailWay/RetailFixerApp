using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RetailFixer.Enums;
using RetailFixer.Interfaces;
using RetailFixer.Windows;

namespace RetailFixer;

public partial class MainWindow : Window
{
    public static MainWindow Singleton { get; private set; } = null!;
    private IFiscal? _driverFiscal;
    private IOperator? _fiscalOperator;
    private IFrontWare? _frontWare;
        
    public MainWindow()
    {
        Singleton = this;
        InitializeComponent();
    }

    private async Task GetInstalledFrontWares()
    {
        var arr = App.Assembly.GetTypes().Where(i =>
            i.GetInterface(nameof(IFrontWare)) is not null &&
            (bool)i.GetProperty("IsInstalled")?.GetValue(null)!).ToArray();
        if (arr.Length == 0)
            await CriticalError("Найдено ни одной фронт-системы.");
        FrontWareWizard.SetInstalled(arr);
    }

    private async Task GetInstalledFiscals()
    {
        var arr = App.Assembly.GetTypes().Where(i =>
            i.GetInterface(nameof(IFiscal)) is not null &&
            (bool)i.GetProperty("IsInstalled")?.GetValue(null)!).ToArray();
        if (arr.Length == 0)
            await CriticalError("Найдено ни одного драйвера ККМ.");
        FiscalWizard.SetInstalled(arr);
    }

    private void GetInstalledOperators() =>
        OperatorWizard.SetInstalled(
            App.Assembly.GetTypes().Where(i => 
                i.GetInterface(nameof(IOperator)) is not null));

    private async Task CriticalError(string message)
    {
        App.Logger.Error(message);
        await Alert.Show("КРИТИЧЕСКАЯ ОШИБКА!!", message, AlertLevel.Fatal);
#if !DEBUG
        Environment.Exit(0);
#endif
    }

    private async void ShowDeviceSettings(object? sender, RoutedEventArgs e) =>
        _driverFiscal = await new FiscalWizard().ShowDialog<IFiscal>(this);

    private async void ShowOperatorSettings(object? sender, RoutedEventArgs e) =>
        _fiscalOperator = await new OperatorWizard().ShowDialog<IOperator>(this);

    private async void ShowFrontWareSettings(object? sender, RoutedEventArgs e) =>
        _frontWare = await new FrontWareWizard().ShowDialog<IFrontWare>(this);

    private void Read(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_fiscalOperator is null)
                throw new Exception("Отсутствует подключение к ОФД");
            if (_frontWare is null)
                throw new Exception("Отсутствует подключение к фронт-системе");
            _fiscalOperator.PullReceipts();
            foreach (var r in _frontWare.PullReceipts())
            {
                var i = App.Receipts.FindIndex(j => j.Number == r.Number);
                if(i == -1) App.Receipts.Add(r);
                App.Receipts[i].Subject |= PullSubjects.FrontSystem;
            }
        }
        catch (Exception ex)
        {
            App.Logger.Error(ex.Message);
            Alert.Show(ex.Message, "Выполните предыдущие пункты!", AlertLevel.Error);
        }

    }

    private void ShowMissings(object? sender, RoutedEventArgs e)
    {
        new ReceiptsWindow().ShowDialog(this);
    }

    private void Write(object? sender, RoutedEventArgs e)
    {
        
    }

    private async void Loaded(object? sender, RoutedEventArgs e)
    {
        await GetInstalledFrontWares();
        await GetInstalledFiscals();
        GetInstalledOperators();
    }
}