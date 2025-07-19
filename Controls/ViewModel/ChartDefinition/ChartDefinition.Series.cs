using System.Collections.ObjectModel;
using Avalonia.Media;
using DynamicData;
using Eremex.AvaloniaUI.Charts;

namespace Controls.ViewModel.ChartDefinition;

public record AxesKey(string KeyX, string KeyY);

public partial class ChartDefinition
{
    public ObservableCollection<CartesianSeries> Series { get; } = new();

    public void AddSeries<TView>(ISeriesDataAdapter adapter, Color color, AxesKey? key = null)
        where TView : CartesianSeriesView, new()
    {
        var view = new TView();
        SetColor(view, color);

        var series = new CartesianSeries
        {
            DataAdapter = adapter,
            View = view,
            AxisXKey = key?.KeyX,
            AxisYKey = key?.KeyY
        };

        Series.Add(series);
    }

    public void AddSeries(ISeriesDataAdapter adapter, Color color, string name, SeriesChartType type,
        bool xPrimary, bool yPrimary, int xIndex, int yIndex, string unit = "", int group = -1,
        CartesianSeries? preset = null)
    {
        var view = MapSeriesType(type);
        SetColor(view, color);

        var series = preset ?? new CartesianSeries
        {
            DataAdapter = adapter,
            SeriesName = name,
            View = view,
            AxisXKey = GetKeyXByIndex(xIndex, xPrimary),
            AxisYKey = GetKeyYByIndex(yIndex, yPrimary)
        };

        Series.Add(series);
    }

    public void DeleteSeries(string name) =>
        ListEx.RemoveMany(Series, Enumerable.Where<CartesianSeries>(Series, s => s.SeriesName == name));

    public void DeleteSeries(int index) => Series.RemoveAt(index);

    public void RemoveSeries(CartesianSeries series) => Series.Remove(series);

    public void ClearSeries() => Series.Clear();

    public IEnumerable<CartesianSeries> FindSeriesIndex(string name) =>
        Enumerable.Where<CartesianSeries>(Series, s => s.SeriesName == name);

    public void LoadData<TView>(string[] data, Color color, AxesKey? key = null)
        where TView : CartesianSeriesView, new()
    {
        var adapter = new SortedDateTimeDataAdapter();

        foreach (var line in data)
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 3 &&
                DateTime.TryParse($"{parts[0]} {parts[1]}", out var dateTime) &&
                double.TryParse(parts[2], out var value))
            {
                adapter.Add(dateTime, value);
            }
        }

        AddSeries<TView>(adapter, color, key);
        Update();
    }

    private void SetColor(CartesianSeriesView view, Color color)
    {
        var property = view.GetType().GetProperty("Color");
        if (property?.PropertyType == typeof(Color))
            property.SetValue(view, color);
    }

    private CartesianSeriesView MapSeriesType(SeriesChartType chartType) =>
        chartType switch
        {
            SeriesChartType.Point => new CartesianPointSeriesView(),
            SeriesChartType.Line => new CartesianLineSeriesView(),
            SeriesChartType.StepLine => new CartesianStepLineSeriesView(),
            SeriesChartType.Column => new CartesianSideBySideBarSeriesView(),
            SeriesChartType.Area => new CartesianAreaSeriesView(),
            SeriesChartType.BrokenLine => new CartesianScatterLineSeriesView(),
            _ => throw new NotSupportedException()
        };
}

public enum SeriesChartType : byte
{
    Point = 0,
    Line = 3,
    StepLine = 5,
    Column = 10,
    Area = 13,
    StackedArea = 15,
    BrokenLine = 35
}