using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Controls.View;
using Controls.ViewModel;
using Controls.ViewModel.ChartDefinition;
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

    // Создаём 6 областей
    for (int i = 0; i < 6; i++)
        vm.AddChartArea();

    var rnd = new Random();
    var now = DateTime.Now;

    for (int i = 0; i < 6; i++)
    {
        // Оси
        vm[i].AddAxisX(new DateTimeScaleOptions { MeasureUnit = DateTimeUnit.Month });
        vm[i].AddAxisY(new NumericScaleOptions());

        // Адаптер
        var adapter = new SortedDateTimeDataAdapter();
        for (int m = 0; m < 50; m++)
        {
            var dt = now.AddMonths(m);
            double value;

            // Разные диапазоны для разных чартов
            switch (i)
            {
                case 0: // Линия: 50 ± 20
                    value = 50 + (rnd.NextDouble() - 0.5) * 40;
                    break;
                case 1: // Bar: 0–100
                    value = rnd.NextDouble() * 100;
                    break;
                case 2: // Area: 20 ± 10
                    value = 20 + (rnd.NextDouble() - 0.5) * 20;
                    break;
                case 3: // Point: 10 ± 5
                    value = 10 + (rnd.NextDouble() - 0.5) * 10;
                    break;
                case 4: // Line + Bar: линия 40±15, столбцы 80±20 (добавим две серии)
                    if (m % 2 == 0)
                        value = 40 + (rnd.NextDouble() - 0.5) * 30;
                    else
                        value = 80 + (rnd.NextDouble() - 0.5) * 40;
                    break;
                default: // Area + Point: area 30±10, point 15±5
                    if (m % 2 == 0)
                        value = 30 + (rnd.NextDouble() - 0.5) * 20;
                    else
                        value = 15 + (rnd.NextDouble() - 0.5) * 10;
                    break;
            }

            adapter.Add(dt, value);
        }

        // Серии
        switch (i)
        {
            case 0:
                vm[i].AddSeries<CartesianLineSeriesView>(adapter, Colors.Blue, new AxesKey("0", "1"));
                break;
            case 1:
                vm[i].AddSeries<CartesianSideBySideBarSeriesView>(adapter, Colors.Red, new AxesKey("0", "1"));
                break;
            case 2:
                vm[i].AddSeries<CartesianAreaSeriesView>(adapter, Colors.Green, new AxesKey("0", "1"));
                break;
            case 3:
                vm[i].AddSeries<CartesianPointSeriesView>(adapter, Colors.Orange, new AxesKey("0", "1"));
                break;
            case 4:
                // две серии в одну область
                var adapterLine = new SortedDateTimeDataAdapter();
                var adapterBar  = new SortedDateTimeDataAdapter();
                for (int m = 0; m < 50; m++)
                {
                    var dt = now.AddMonths(m);
                    adapterLine.Add(dt, 40 + (rnd.NextDouble() - 0.5) * 30);
                    adapterBar .Add(dt, 80 + (rnd.NextDouble() - 0.5) * 40);
                }
                vm[i].AddSeries<CartesianLineSeriesView>(adapterLine, Colors.Purple, new AxesKey("0", "1"));
                vm[i].AddSeries<CartesianSideBySideBarSeriesView>(adapterBar, Colors.Pink, new AxesKey("0", "1"));
                break;
            case 5:
                // area + points
                var adapterArea  = new SortedDateTimeDataAdapter();
                var adapterPoint = new SortedDateTimeDataAdapter();
                for (int m = 0; m < 50; m++)
                {
                    var dt = now.AddMonths(m);
                    adapterArea .Add(dt, 30 + (rnd.NextDouble() - 0.5) * 20);
                    adapterPoint.Add(dt, 15 + (rnd.NextDouble() - 0.5) * 10);
                }
                vm[i].AddSeries<CartesianAreaSeriesView>(adapterArea, Colors.Brown, new AxesKey("0", "1"));
                vm[i].AddSeries<CartesianPointSeriesView>(adapterPoint, Colors.Yellow, new AxesKey("0", "1"));
                break;
        }

        vm[i].Update();
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
        
        vm[2].Update();
        vm[0].RequestScroll(new Point(0, 0), new Point(50000, 50000));
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