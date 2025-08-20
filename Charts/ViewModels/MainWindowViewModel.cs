using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Eremex.AvaloniaUI.Charts;
using System;
using Charts.ViewModels;

namespace ChartBarSeriesView.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        Random rand = new Random(4);

        SortedDateTimeDataAdapter adapter1 = new SortedDateTimeDataAdapter();
        SortedDateTimeDataAdapter adapter2 = new SortedDateTimeDataAdapter();
        SortedDateTimeDataAdapter adapter3 = new SortedDateTimeDataAdapter();

        for (int i = 0; i < 12; i++)
        {
            adapter1.Add(DateTime.Now.AddMonths(i), i);
            adapter2.Add(DateTime.Now.AddMonths(i), i * 2);
        }

        for (int i = 0; i < 360; i++)
        {
            adapter3.Add(DateTime.Now.AddDays(i), rand.NextDouble() * 7);
        }

        var axis = new AxisX
        {
            Key = "Day",
            Title = "Day",
            ScaleOptions = new DateTimeScaleOptions
            {
                MeasureUnit = DateTimeUnit.Day,
            }
        };
        
    }
}