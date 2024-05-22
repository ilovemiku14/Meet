using System.Windows;
using System.Windows.Controls;
using Meet.ViewModels;

namespace Meet.Views;

/// <summary>
///     ScheduleMeetingUC.xaml 的交互逻辑
/// </summary>
public partial class ScheduleMeetingUC : UserControl
{
    public ScheduleMeetingUC()
    {
        InitializeComponent();
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        
    }
    private void PopupBox_OnOpened(object sender, RoutedEventArgs e)
    {
        // ComboBox combobox = sender as ComboBox;
        // var index = combobox.SelectedItem as string;
         // Console.WriteLine(sender);

        // 处理 PopupBox 打开时的逻辑
    }

    private void PopupBox_OnClosed(object sender, RoutedEventArgs e)
    {
        // ComboBox combobox = sender as ComboBox;
        // var index = combobox.SelectedItem as string;
        // Console.WriteLine(sender);
        // 处理 PopupBox 关闭时的逻辑
    }
    
}