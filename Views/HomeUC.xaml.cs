using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;

namespace Meet.Views;

/// <summary>
///     HomeUC.xaml 的交互逻辑
/// </summary>
public partial class HomeUC : UserControl
{
    public HomeUC()
    {
          InitializeComponent();
         
                    // SeriesCollection = new SeriesCollection
                    // {
                    //     new LineSeries
                    //     {
                    //         Title = "502A",
                    //         Values = new ChartValues<double> { 4, 6, 5, 2 ,4 }
                    //     },
                    //     new LineSeries
                    //     {
                    //         Title = "502B TV",
                    //         Values = new ChartValues<double> { 6, 7, 3, 4 ,6 },
                    //         PointGeometry = null
                    //     },
                    //     new LineSeries
                    //     {
                    //         Title = "502C",
                    //         Values = new ChartValues<double> { 4,2,7,2,7 },
                    //         PointGeometry = DefaultGeometries.Square,
                    //         PointGeometrySize = 15
                    //     }
                    // };
         
                    // Labels = new[] {"Jan", "Feb", "Mar", "Apr", "May"};
                    // YFormatter = value => value.ToString("C");
         
                    //modifying the series collection will animate and update the chart
                    // SeriesCollection.Add(new LineSeries
                    // {
                    //     Title = "Series 4",
                    //     Values = new ChartValues<double> {5, 3, 2, 4},
                    //     LineSmoothness = 0, //0: straight lines, 1: really smooth lines
                    //     PointGeometry = Geometry.Parse("m 25 70.36218 20 -28 -20 22 -8 -6 z"),
                    //     PointGeometrySize = 50,
                    //     PointForeground = Brushes.Gray
                    // });
         
                    //modifying any series values will also animate and update the chart
                    //SeriesCollection[3].Values.Add(5d);
         
                    // DataContext = this;
                }
    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        
    }
                // public SeriesCollection SeriesCollection { get; set; }
                // public string[] Labels { get; set; }
                // public Func<double, string> YFormatter { get; set; }
}