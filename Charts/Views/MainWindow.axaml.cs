using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Controls.View;
using Controls.ViewModel;
using Eremex.AvaloniaUI.Charts;
using Eremex.AvaloniaUI.Controls.Common;

namespace Charts.Views;

public partial class MainWindow : MxWindow
{
    public DataChartViewModel vm = new DataChartViewModel();
    public MainWindow()
    {
        InitializeComponent();
        InitChart();
    }
    
    private DataChartViewModel _viewModel;
    private DataChartEremex _view;

    public void InitializeExample()
    {
        _viewModel = new DataChartViewModel();
        _view = new DataChartEremex { DataContext = _viewModel };
    }

    public void InitChart()
    {
         chartControl.DataContext = vm;
        
        vm.AddChartArea();
        vm.AddChartArea();
        vm.AddChartArea();
        vm.AddChartArea();
        vm.AddChartArea();

// добавим оси


        vm[0].AddAxisX(new DateTimeScaleOptions{MeasureUnit = DateTimeUnit.Day});
        vm[0].AddAxisY(new NumericScaleOptions());

        Random rand = new Random(4);
        
        SortedDateTimeDataAdapter adapter = new SortedDateTimeDataAdapter();
        SortedDateTimeDataAdapter adapter2 = new SortedDateTimeDataAdapter();
        
        for (int i = 0; i < 12; i++)
        {
            adapter.Add(DateTime.Now.AddMonths(i), i);
            adapter2.Add(DateTime.Now.AddMonths(i), rand.NextDouble() * 10);
        }
        
        vm[0].AddSeries<CartesianLineSeriesView>(adapter, Colors.Blue, new AxesKey("0", "1"));
        // vm[0].AddSeries<CartesianSideBySideBarSeriesView>(adapter, Colors.Red, new AxesKey("0", "1"));
        
        vm[0].AddSeries(adapter, Colors.Red, "Lol", SeriesChartType.Column, true, true, 0, 0);
        
        // vm[0].RemoveSeries(vm[0].Series[0]);
        
        vm[1].AddAxisX(new DateTimeScaleOptions{MeasureUnit = DateTimeUnit.Day});
        vm[1].AddAxisY(new NumericScaleOptions());
        
        
        vm[1].AddSeries<CartesianSideBySideBarSeriesView>(adapter, Colors.Green, new AxesKey("0", "1"));
        
        vm[2].AddAxisX(new DateTimeScaleOptions{MeasureUnit = DateTimeUnit.Day});
        vm[2].AddAxisY(new NumericScaleOptions());

        foreach (var chart in vm.Charts)
        {
            chart.Update();
        }
        
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
                vm[2].LoadData<CartesianSideBySideBarSeriesView>(lines, Colors.Gold, new AxesKey("0", "1"));
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