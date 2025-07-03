using System;
using Avalonia.Controls;
using Avalonia.Media;
using DynamicData;
using Eremex.AvaloniaUI.Charts;

namespace Charts.Controls;

public partial class DataChartEremex: UserControl
{
    public DataChartEremex()
    {
        InitializeComponent();
        InitChart();
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
    
        var series = new CartesianSeries { 
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
        
        AddSeries<CartesianLineSeriesView>(adapter1, Colors.Blue, "Month");
        AddBarSeries(adapter2, Colors.Green, "Day");
        AddAreaSeries(adapter3, Colors.Red, "Day");

        var axis = new AxisX
        {
            Key = "Day",
            Title = "Day",
            ScaleOptions = new DateTimeScaleOptions
            {
                MeasureUnit = DateTimeUnit.Day,
            }
        };
        
        AddXAxis(axis);
        
        AddXAxis(new AxisX{
            Key = "Month",
            Title = "Month",
            ScaleOptions = new DateTimeScaleOptions
        {
            MeasureUnit = DateTimeUnit.Month,
        }});
        
    }

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
}
