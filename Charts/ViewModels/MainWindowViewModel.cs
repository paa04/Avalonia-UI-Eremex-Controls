using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Eremex.AvaloniaUI.Charts;
using System;
using Charts.ViewModels;

namespace ChartBarSeriesView.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] SeriesViewModel barSeries1;
    [ObservableProperty] SeriesViewModel linearSeries1;
    [ObservableProperty] FuncLabelFormatter monthFormatter = new(o => String.Format("{0:MMM} {0:yy}", o));

    public MainWindowViewModel()
    {
        var random = new Random(11);
        var startDate = new DateTime(DateTime.Now.Year, 1, 1);
        SortedDateTimeDataAdapter barSeries1DataAdapter = new();
        SortedDateTimeDataAdapter barSeries2DataAdapter = new();
        SortedDateTimeDataAdapter barSeries3DataAdapter = new();
        SortedDateTimeDataAdapter linearSeries1DataAdapter = new();
        for (int i = 0; i < 12; i++)
        {
            var argument = startDate.AddMonths(i);
            barSeries1DataAdapter.Add(argument, random.NextDouble() * 100 - 30);
            barSeries2DataAdapter.Add(argument, random.NextDouble() * 100 - 30);
            barSeries3DataAdapter.Add(argument, random.NextDouble() * 100 - 30);
            linearSeries1DataAdapter.Add(argument, random.NextDouble() * 100 - 30);
        }
        // Create data series
        BarSeries1 = new() { Color = Color.FromUInt32(0xffe07a5f), DataAdapter = barSeries1DataAdapter };
        LinearSeries1 = new (){ Color = Color.FromArgb(255, 255, 0, 0), DataAdapter = linearSeries1DataAdapter };
    }
}

public partial class SeriesViewModel : ObservableObject
{
    [ObservableProperty] Color color;
    [ObservableProperty] ISeriesDataAdapter dataAdapter;
}