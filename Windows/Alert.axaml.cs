using Avalonia.Controls;
using Avalonia.Interactivity;
using RetailFixer.Enums;

namespace RetailFixer.Windows;

public partial class Alert : Window
{
    internal string Text { get; set; }
    internal string TagType { get; set; }

#if DEBUG
    public Alert()
    {
        Title = "Проблема с соединением";
        Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed scelerisque quis metus eget iaculis. Nulla facilisi. Curabitur rutrum libero in nunc consequat, nec venenatis lorem tincidunt. Donec nec lorem quis urna posuere volutpat vitae eu lorem. In malesuada fermentum turpis. Cras consequat neque sit amet gravida malesuada. Donec hendrerit purus";
        TagType = "debug";
        InitializeComponent();
    }
#endif

    public Alert(string title, string message, AlertLevel level)
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

    private void btnClose(object? s, RoutedEventArgs e) => Close();
}