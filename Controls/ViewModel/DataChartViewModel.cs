using System.Collections.ObjectModel;
using Avalonia.Media;
using Eremex.AvaloniaUI.Charts;
using Controls.View;

namespace Controls.ViewModel;

public class DataChartViewModel
{
    public ObservableCollection<ChartDefinition> Charts { get; } = new();

    public void AddChartArea(ChartPosition pos)
    {
        Charts.Add(new ChartDefinition { Position = pos });
    }

    public void AddSeries<TView>(ChartPosition pos, ISeriesDataAdapter adapter, Color color)
        where TView : CartesianSeriesView, new()
    {
        var def = GetDefinition(pos);
        var view = new TView();
        SetColor(view, color);

        var series = new CartesianSeries
        {
            DataAdapter = adapter,
            View = view
        };

        def.Series.Add(series);
    }

    public void AddAxisX(ChartPosition pos, AxisX axis)
    {
        GetDefinition(pos).AxesX.Add(axis);
    }

    public void AddAxisY(ChartPosition pos, AxisY axis)
    {
        GetDefinition(pos).AxesY.Add(axis);
    }

    private ChartDefinition GetDefinition(ChartPosition pos) =>
        Charts.FirstOrDefault(c => c.Position == pos)
        ?? throw new InvalidOperationException($"Chart at {pos} not found");

    private void SetColor(CartesianSeriesView view, Color color)
    {
        var property = view.GetType().GetProperty("Color");
        if (property != null && property.PropertyType == typeof(Color))
        {
            property.SetValue(view, color);
        }
    }
}