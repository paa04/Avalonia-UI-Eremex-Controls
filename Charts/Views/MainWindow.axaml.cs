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
        // InitializeExample();

        InitChart();
    }
    
    private DataChartViewModel _viewModel;
    private DataChartEremex _view;

    public void InitializeExample()
    {
        _viewModel = new DataChartViewModel();
        _view = new DataChartEremex { DataContext = _viewModel };
    }
    public void Example1_AddNewChart()
    {
        // Добавляем новый чарт - это вызовет AddNewChart() в View
        var chartIndex = _viewModel.AddChartArea();
        
        // Получаем ссылку на созданный чарт
        var chart = _viewModel[chartIndex];
        
        // Настраиваем чарт
        chart.Title = "Мой первый чарт";
        
        // Добавляем данные - это вызовет RefreshChartSeries() в View
        var data = new[] 
        {
            "2024-01-01 10:00:00 100.5",
            "2024-01-01 11:00:00 105.2",
            "2024-01-01 12:00:00 98.7"
        };
        
        chart.LoadData<CartesianLineSeriesView>(data, Colors.Blue);
    }

    public void InitChart()
    {
        chartControl.DataContext = vm;
        chartControl.SubscribeToViewModel();
        
        vm.AddChartArea();
        vm.AddChartArea();
        vm.AddChartArea();
        vm.AddChartArea();
        vm.AddChartArea();

// добавим оси


        vm[0].AddAxisX(new AxisX
        {
            Title = "Day",
            ScaleOptions = new DateTimeScaleOptions
            {
                MeasureUnit = DateTimeUnit.Day,
            }
        });
        vm[0].AddAxisY(new AxisY { Title = "Value" });

        Random rand = new Random(4);
        
        SortedDateTimeDataAdapter adapter = new SortedDateTimeDataAdapter();
        
        for (int i = 0; i < 12; i++)
        {
            adapter.Add(DateTime.Now.AddMonths(i), i);
        }
        
        vm[0].AddSeries<CartesianLineSeriesView>(adapter, Colors.Blue);
        vm[0].AddSeries<CartesianSideBySideBarSeriesView>(adapter, Colors.Red);
        
        vm[1].AddAxisX(new AxisX
        {
            Title = "Value",
            ScaleOptions = new DateTimeScaleOptions
            {
                MeasureUnit = DateTimeUnit.Day,
            }
        });
        
        vm[1].AddSeries<CartesianSideBySideBarSeriesView>(adapter, Colors.Green);
        
        vm[2].AddAxisX(new AxisX
        {
            Title = "Value",
            ScaleOptions = new DateTimeScaleOptions
            {
                MeasureUnit = DateTimeUnit.Day,
            }
        });
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
                vm[2].LoadData<CartesianSideBySideBarSeriesView>(lines, Colors.Gold);
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