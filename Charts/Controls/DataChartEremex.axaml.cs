using System;
using Avalonia.Controls;
using Avalonia.Media;
using DynamicData;
using Eremex.AvaloniaUI.Charts;

namespace Charts.Controls;

public partial class DataChartEremex : UserControl
{
    public DataChartEremex()
    {
        InitializeComponent();
    }

    public void AddSeries(CartesianSeries series)
    {
        Chart.Series.Add(series);
    }

    public void AddSeries<TView>(ISeriesDataAdapter adapter, Color color, string? keyX = null, string? keyY = null)
        where TView : CartesianSeriesView, new()
    {
        var view = new TView();
        SetColor(view, color);

        var series = new CartesianSeries
        {
            DataAdapter = adapter,
            View = view,
        };

        if (keyX is not null)
            series.AxisXKey = keyX;
        if (keyY is not null)
            series.AxisYKey = keyY;

        Chart.Series.Add(series);
    }

    private void SetColor(CartesianSeriesView view, Color color)
    {
        var type = view.GetType();
        var property = type.GetProperty("Color");

        if (property != null && property.PropertyType == typeof(Color))
        {
            property.SetValue(view, color);
        }
        else
        {
            throw new InvalidOperationException(
                $"Тип {type.Name} не содержит свойство Color типа Color");
        }
    }

    public void AddLineSeries(ISeriesDataAdapter adapter, Color color, string? keyX = null, string? keyY = null)
        => AddSeries<CartesianLineSeriesView>(adapter, color, keyX, keyY);

    public void AddBarSeries(ISeriesDataAdapter adapter, Color color, string? keyX = null, string? keyY = null) =>
        AddSeries<CartesianSideBySideBarSeriesView>(adapter, color, keyX, keyY);

    public void AddPointSeries(ISeriesDataAdapter adapter, Color color, string? keyX = null, string? keyY = null) =>
        AddSeries<CartesianPointSeriesView>(adapter, color, keyX, keyY);

    public void AddAreaSeries(ISeriesDataAdapter adapter, Color color, string? keyX = null, string? keyY = null) =>
        AddSeries<CartesianAreaSeriesView>(adapter, color, keyX, keyY);

    public void AddLegend()
    {
        throw new NotImplementedException();
    }

    public void AddXAxis(AxisX axis)
    {
        Chart.AxesX.Add(axis);
    }

    public void AddYAxis(AxisY axis)
    {
        Chart.AxesY.Add(axis);
    }

    public void LoadData<TView>(string[] data, Color color, string? keyX = null, string? keyY = null)
        where TView : CartesianSeriesView, new()
    {
        var adapter = new SortedDateTimeDataAdapter();

        foreach (var line in data)
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 3)
            {
                var dateTimeString = $"{parts[0]} {parts[1]}";
                if (DateTime.TryParse(dateTimeString, out var dateTime) &&
                    double.TryParse(parts[2], out var value))
                {
                    adapter.Add(dateTime, value);
                }
            }
        }

        AddSeries<TView>(adapter, color, keyX, keyY);
    }
}