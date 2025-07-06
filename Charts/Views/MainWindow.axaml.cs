using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Controls;
using Eremex.AvaloniaUI.Charts;
using Eremex.AvaloniaUI.Controls.Common;

namespace Charts.Views;

public partial class MainWindow : MxWindow
{
    public MainWindow()
    {
        InitializeComponent();
        InitChart();
    }

    private void InitChart()
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

        chartControl.AddRow(GridLength.Star);
        chartControl.AddColumn(GridLength.Star);

        chartControl.AddNewChart(new ChartPosition(0, 0));

        chartControl.AddSeries<CartesianSideBySideBarSeriesView>(adapter1, new ChartPosition(0, 0),
            null, view => view.Color = Colors.Red);

        chartControl.AddColumn(GridLength.Star);
        chartControl.AddNewChart(new ChartPosition(0, 1));
        chartControl.AddXAxis(new ChartPosition(0, 0),
            new AxisX
            {
                Key = "Day",
                Title = "Day",
                ScaleOptions = new DateTimeScaleOptions
                {
                    MeasureUnit = DateTimeUnit.Day,
                }
            });
        chartControl.AddSeries<CartesianSideBySideBarSeriesView>(adapter2, new ChartPosition(0, 0),
            new SeriesAxisKeys("Day", null), view => view.Color = Colors.Red);

        chartControl.AddSeries<CartesianSideBySideBarSeriesView>(adapter3, new ChartPosition(0, 1),
            new SeriesAxisKeys("Day", null), view => view.Color = Colors.Red);
        var axis = new AxisX
        {
            Key = "Day",
            Title = "Day",
            ScaleOptions = new DateTimeScaleOptions
            {
                MeasureUnit = DateTimeUnit.Day,
            }
        };

        chartControl.AddXAxis(new ChartPosition(0, 1), axis);

        chartControl.AddXAxis(new ChartPosition(0, 0), new AxisX
        {
            Key = "Month",
            Title = "Month",
            ScaleOptions = new DateTimeScaleOptions
            {
                MeasureUnit = DateTimeUnit.Month,
            }
        });

        chartControl.AddRow(GridLength.Star);
    }

    private async void OnLoadButtonClick(object? sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Выберите файл с данными",
            AllowMultiple = false
        };

        var result = await dialog.ShowAsync(this);

        if (result != null && result.Length > 0)
        {
            var filePath = result[0];
            try
            {
                string[] lines = await File.ReadAllLinesAsync(filePath);
                chartControl.LoadData<CartesianSideBySideBarSeriesView>(lines, Colors.Gold, new ChartPosition(0, 0), new SeriesAxisKeys("Month", null));
            }
            catch (Exception ex)
            {
                await MessageBox("Ошибка", $"Не удалось загрузить данные: {ex.Message}");
            }
        }
    }

    private async Task MessageBox(string title, string message)
    {
        var dialog = new Window
        {
            Title = title,
            Width = 300,
            Height = 150,
            Content = new TextBlock { Text = message, Margin = new Thickness(10) }
        };
        await dialog.ShowDialog(this);
    }
}