using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RetailFixer.Enums;

namespace RetailFixer.Windows;

public partial class Alert : Window
{
    private string Text { get; }
    private string TagType { get; }

    public static Task Show(string title, string message, AlertLevel level) =>
        new Alert(title, message, level).ShowDialog(MainWindow.Singleton);

    private Alert(string title, string message, AlertLevel level)
    {
        Title = title;
        Text = message;
        TagType = level switch
        {
            AlertLevel.Info => "info",
            AlertLevel.Warn => "warning",
            AlertLevel.Error => "error",
            AlertLevel.Fatal => "critical",
            _ => "debug"
        };
        InitializeComponent();
    }

    private void Close(object? s, RoutedEventArgs e) => Close();
}