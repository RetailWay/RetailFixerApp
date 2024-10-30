using Avalonia.Controls;
using RetailFixer.Enums;

namespace RetailFixer.Windows;

public partial class ReceiptsWindow : Window
{
    private Receipt[] Receipts { get; init; }
    public ReceiptsWindow()
    {
        Receipts = new Receipt[App.Receipts.Count];
        for(var i = 0; i < App.Receipts.Count; i++)
            Receipts[i] = new Receipt(App.Receipts[i]);
        InitializeComponent();
    }

    private void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private struct Receipt(Data.Receipt data)
    {
        public string Action { get; } = "I";
        public string Id { get; } = $"{data.Number}";
        public string DateTime { get; } = $"{data.DateTime:yyyy-MM-dd HH:mm}";
        public string Sum { get; } = $"{(data.TotalSum/100.0):F2} руб.";

        public string Source { get; } = data.Subject switch
        {
            PullSubjects.Operator => "ОФД",
            PullSubjects.FrontSystem => "Фронт-система",
            PullSubjects.Operator | PullSubjects.FrontSystem => "Фронт+ОФД",
            _ => ""
        };
    } 
}