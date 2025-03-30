using Avalonia.Controls;

namespace InSight.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Button ghena = new Button();
        ghena.Content = "Ghena";
        
        ChartContainer.Children.Add(ghena);
    }
}