using Avalonia.Controls;
using Avalonia.Interactivity;
using RetailFixer.Configurations;
using RetailFixer.Windows;

namespace RetailFixer;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OpenFiscalConfigurator(object? sender, RoutedEventArgs e) =>
        new FiscalWizard().ShowDialog(this);

    private void OpenOperatorConfigurator(object? sender, RoutedEventArgs e) =>
        new OperatorWizard().ShowDialog(this);
}