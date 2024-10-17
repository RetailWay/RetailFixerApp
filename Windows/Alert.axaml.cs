using Avalonia.Controls;
using Avalonia.Interactivity;

namespace RetailFixer.Windows;

public partial class Alert : Window
{
    internal string Text { get; set; }
    
    public Alert(string title, string message)
    {
        InitializeComponent(); 
        Title = title;
        Text = message;
    }

    private void btnClose(object? s, RoutedEventArgs e) => Close();
}