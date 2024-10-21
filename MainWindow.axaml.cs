using Avalonia.Controls;

namespace RetailFixer;

public partial class MainWindow : Window
{
    public static MainWindow Singleton { get; private set; } = null!;
        
    public MainWindow()
    {
        Singleton = this;
        InitializeComponent();
    }
}