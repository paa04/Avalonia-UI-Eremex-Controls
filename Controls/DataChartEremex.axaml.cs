using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Eremex.AvaloniaUI.Charts;

namespace Controls;

public record ChartPosition(int Row, int Column);

public record SeriesAxisKeys(string? KeyX, string? KeyY);

public partial class DataChartEremex : UserControl
{
    private readonly Dictionary<ChartPosition, CartesianChart> _charts =
        new Dictionary<ChartPosition, CartesianChart>();

    public DataChartEremex()
    {
        InitializeComponent();
    }

    private CartesianChart GetChart(ChartPosition position)
    {
        return _charts[position];
    }

    public void AddSeries(ChartPosition pos, CartesianSeries series)
    {
        var chart = GetChart(pos);
        chart.Series.Add(series);
    }

    public CartesianSeries AddSeries<TView>(
        ISeriesDataAdapter adapter,
        ChartPosition pos,
        SeriesAxisKeys? keys = null,
        Action<TView>? configureView = null)
        where TView : CartesianSeriesView, new()
    {
        var view = new TView();
        configureView?.Invoke(view);

        var series = new CartesianSeries
        {
            DataAdapter = adapter,
            View = view,
        };

        if (keys is not null)
        {
            if (keys.KeyX != null)
                series.AxisXKey = keys.KeyX;
            if (keys.KeyY != null)
                series.AxisYKey = keys.KeyY;
        }


        GetChart(pos).Series.Add(series);
        return series;
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

    public void AddLegend()
    {
        throw new NotImplementedException();
    }

    public void AddXAxis(ChartPosition pos, AxisX axis)
    {
        var chart = GetChart(pos);
        chart.AxesX.Add(axis);
    }

    public void AddYAxis(ChartPosition pos, AxisY axis)
    {
        var chart = GetChart(pos);
        chart.AxesY.Add(axis);
    }

    public void LoadData<TView>(string[] data, Color color, ChartPosition pos, SeriesAxisKeys keys)
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

        AddSeries<TView>(adapter, pos, keys, view => SetColor(view, color));
    }

    public void AddRow(GridLength height)
    {
        Grid.RowDefinitions.Add(new RowDefinition(height));
    }

    public void AddColumn(GridLength width)
    {
        Grid.ColumnDefinitions.Add(new ColumnDefinition(width));
    }

    public void AddNewChart(ChartPosition pos)
    {
        if (Grid.RowDefinitions.Count <= pos.Row || Grid.ColumnDefinitions.Count <= pos.Column)
            throw new InvalidDataException();

        var chart = new CartesianChart();
        _charts.Add(pos, chart);

        Grid.Children.Add(chart);
        Grid.SetRow(chart, pos.Row);
        Grid.SetColumn(chart, pos.Column);
    }
}