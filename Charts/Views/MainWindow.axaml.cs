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
        InitChart2();
    }
    
    private DataChartViewModel _viewModel;
    private DataChartEremex _view;

    public void InitializeExample()
    {
        _viewModel = new DataChartViewModel();
        _view = new DataChartEremex { DataContext = _viewModel };
    }

    public void InitChart2()
{
    chartControl.DataContext = vm;

    for (int i = 0; i < 6; i++)
        vm.AddChartArea();

    var rnd = new Random();
    var now = DateTime.Now;

    for (int i = 0; i < 6; i++)
    {
        vm[i].AddAxisX(new DateTimeScaleOptions { MeasureUnit = DateTimeUnit.Month });
        vm[i].AddAxisY(new NumericScaleOptions());

        var adapter = new SortedDateTimeDataAdapter();
        for (int m = 0; m < 12; m++)
        {
            var dt = now.AddMonths(m);
            double y = rnd.NextDouble() * (i + 1) * 20;
            adapter.Add(dt, y);
        }

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
                vm[i].AddSeries<CartesianLineSeriesView>(adapter, Colors.Purple, new AxesKey("0", "1"));
                vm[i].AddSeries<CartesianSideBySideBarSeriesView>(adapter, Colors.Pink, new AxesKey("0", "1"));
                break;
            case 5:
                vm[i].AddSeries<CartesianAreaSeriesView>(adapter, Colors.Brown, new AxesKey("0", "1"));
                vm[i].AddSeries<CartesianPointSeriesView>(adapter, Colors.Yellow, new AxesKey("0", "1"));
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