using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RetailFixer.Windows;

namespace RetailFixer.Controls;

public partial class SearchProperties : UserControl, INotifyPropertyChanged
{
    public DateOnly BeginPeriod
    {
        get => begin;
        set
        {
            if (begin == value) return;
            begin = value;
            OnPropertyChanged();
        }
    }
    public DateOnly EndPeriod
    {
        get => end;
        set
        {
            if (end == value) return;
            end = value;
            OnPropertyChanged();
        }
    }

    private DateOnly begin = DateOnly.FromDateTime(DateTime.Today);
    private DateOnly end = DateOnly.FromDateTime(DateTime.Today);
    
    public SearchProperties()
    {
        InitializeComponent();
    }
    
    public new event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void OpenFiscalWizard(object? sender, RoutedEventArgs e) =>
        new FiscalWizard().ShowDialog(MainWindow.Singleton);

    private void OpenOperatorWizard(object? sender, RoutedEventArgs e) =>
        new OperatorWizard().ShowDialog(MainWindow.Singleton);
    
    private void OpenFrontolWizard(object? sender, RoutedEventArgs e) =>
        new FrontolWizard().ShowDialog(MainWindow.Singleton);
}