using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Media;
using Eremex.AvaloniaUI.Charts;
using Controls.View;

namespace Controls.ViewModel;

public class DataChartViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public ObservableCollection<ChartDefinition> Charts { get; } = new();

    public DataChartViewModel()
    {
        Charts.CollectionChanged += OnChartsCollectionChanged;
    }

    private void OnChartsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (ChartDefinition chart in e.NewItems!)
                {
                    OnChartAdded(chart);
                }
                break;
                
            case NotifyCollectionChangedAction.Remove:
                foreach (ChartDefinition chart in e.OldItems!)
                {
                    OnChartRemoved(chart);
                }
                break;
        }
    }

    private void OnChartAdded(ChartDefinition chart)
    {
        if (chart.Position == default)
        {
            chart.Position = new ChartPosition(Charts.Count - 1, 0);
        }
    }

    private void OnChartRemoved(ChartDefinition chart)
    {
        //TODO
    }

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