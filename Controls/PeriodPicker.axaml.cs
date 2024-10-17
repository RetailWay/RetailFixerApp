using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;

namespace RetailFixer.Controls;

public sealed partial class PeriodPicker : UserControl, INotifyPropertyChanged
{
    public DateTime StartDate
    {
        get => _start;
        set
        {
            if (_start != value)
            {
                _start = value;
                OnPropertyChanged();
            }
        }
    }
    public DateTime EndDate
    {
        get => _end;
        set
        {
            if (_end != value)
            {
                _end = value;
                OnPropertyChanged();
            }
        }
    }

    private DateTime _start = DateTime.Today;
    private DateTime _end = DateTime.Today;
    
    public PeriodPicker()
    {
        InitializeComponent();
    }
    
    public new event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}