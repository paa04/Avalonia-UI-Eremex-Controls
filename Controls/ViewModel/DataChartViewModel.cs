using System.Collections.ObjectModel;
using System.ComponentModel;
using Controls.View;

namespace Controls.ViewModel;

public class DataChartViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public ObservableCollection<ChartDefinition> Charts { get; } = new();
    
    public int AddChartArea()
    {
        var chart = new ChartDefinition();
        Charts.Add(chart);
        return Charts.Count - 1;
    }

    public int AddChartArea(ChartPosition position)
    {
        var chart = new ChartDefinition { Position = position };
        Charts.Add(chart);
        return Charts.Count - 1;
    }

    public void RemoveChartArea(int index)
    {
        if (index >= 0 && index < Charts.Count)
        {
            Charts.RemoveAt(index);
        }
    }

    public void RemoveChartArea(ChartDefinition chart)
    {
        Charts.Remove(chart);
    }

    public void ClearCharts()
    {
        Charts.Clear();
    }
    
    public ChartDefinition this[int index] => Charts[index];

    public ChartDefinition? GetChartByPosition(ChartPosition position)
    {
        return Charts.FirstOrDefault(c => c.Position == position);
    }

    private ChartDefinition GetDefinition(ChartPosition pos) =>
        Charts.FirstOrDefault(c => c.Position == pos)
        ?? throw new InvalidOperationException($"Chart at {pos} not found");

    public void AddMultipleCharts(params ChartDefinition[] charts)
    {
        foreach (var chart in charts)
        {
            Charts.Add(chart);
        }
    }

    public int ChartsCount => Charts.Count;

    public bool HasCharts => Charts.Count > 0;

    public IEnumerable<ChartDefinition> VisibleCharts => Charts.Where(c => c.IsVisible);
}