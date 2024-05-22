using System.Windows;

namespace Meet.Views;

/// <summary>
///     MainWin.xaml 的交互逻辑
/// </summary>
public partial class MainWin : Window
{
    public MainWin()
    {
        InitializeComponent();
    }

    private void MinCmm(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaxCmm(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Maximized)
            WindowState = WindowState.Normal;
        else
            WindowState = WindowState.Maximized;
    }

    private void ExitCmm(object sender, RoutedEventArgs e)
    {
        Application.Current.MainWindow.Close(); // 关闭当前主窗口
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
    }
}