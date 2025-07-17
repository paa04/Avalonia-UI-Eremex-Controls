using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Controls.ViewModel;
using Eremex.AvaloniaUI.Charts;

namespace Controls.View;

public record ChartPosition(int Row, int Column);

public record SeriesAxisKeys(string? KeyX, string? KeyY);

public partial class DataChartEremex : UserControl
{
    private readonly Dictionary<ChartDefinition, CartesianChart> _chartVisuals = new();

    public DataChartViewModel ViewModel => (DataChartViewModel)DataContext!;

    public DataChartEremex()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => SubscribeToViewModel();
    }

    public void SubscribeToViewModel()
    {
        if (ViewModel != null)
        {
            ViewModel.Charts.CollectionChanged -= OnChartsChanged;
        }

        if (ViewModel != null)
        {
            ViewModel.Charts.CollectionChanged += OnChartsChanged;
        }
    }

    private void OnChartsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (ChartDefinition def in e.NewItems!)
                {
                    AddNewChart(def);
                }

                break;

            case NotifyCollectionChangedAction.Remove:
                foreach (ChartDefinition def in e.OldItems!)
                {
                    RemoveChart(def);
                }

                break;

            case NotifyCollectionChangedAction.Reset:
                ClearAllCharts();
                break;
        }
    }

    private void OnChartPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is ChartDefinition def && _chartVisuals.TryGetValue(def, out var chart))
        {
            if (e.PropertyName == nameof(ChartDefinition.Update))
            {
                RefreshChartAxes(chart, def);
                RefreshChartSeries(chart, def);
            }
        }
    }


    private void AddNewChart(ChartDefinition def)
    {
        var chart = CreateChartVisual(def);

        var position = CalculateChartPosition();

        AddChartToGrid(chart, position);

        _chartVisuals[def] = chart;

        SubscribeToUpdates(def);
        
        def.OnScrollRequested += (_, args) =>
        {
            chart.Scroll(args.DeltaX, args.DeltaY, args.Axes);
        };
        
        def.ScrollRequestedByValue += (_, args) =>
        {
            foreach (var axis in chart.AxesX)
            {
                if (axis.Range != null)
                {
                    axis.Range.VisualMin = 10;
                    axis.Range.VisualMax = 20;
                }
            }
        };

    }

    private void SubscribeToUpdates(ChartDefinition def)
    {
        def.PropertyChanged += OnChartPropertyChanged;
    }

    private CartesianChart CreateChartVisual(ChartDefinition def)
    {
        var chart = new CartesianChart();

        RefreshChartAxes(chart, def);
        RefreshChartSeries(chart, def);

        return chart;
    }

    private void SubscribeToChartChanges(ChartDefinition def, CartesianChart chart)
    {
        def.Series.CollectionChanged += (_, _) => RefreshChartSeries(chart, def);

        def.AxesX.CollectionChanged += (_, _) => RefreshChartAxes(chart, def);

        def.AxesY.CollectionChanged += (_, _) => RefreshChartAxes(chart, def);

        def.AxesX2.CollectionChanged += (_, _) => RefreshChartAxes(chart, def);
        def.AxesY2.CollectionChanged += (_, _) => RefreshChartAxes(chart, def);
    }

    private void RefreshChartSeries(CartesianChart chart, ChartDefinition def)
    {
        chart.Series.Clear();
        foreach (var series in def.Series)
        {
            chart.Series.Add(series);
        }
    }

    private void RefreshChartAxes(CartesianChart chart, ChartDefinition def)
    {
        chart.AxesX.Clear();
        chart.AxesY.Clear();

        foreach (var axis in def.AxesX)
        {
            chart.AxesX.Add(axis);
        }

        foreach (var axis in def.AxesX2)
        {
            chart.AxesX.Add(axis);
        }

        foreach (var axis in def.AxesY)
        {
            chart.AxesY.Add(axis);
        }

        foreach (var axis in def.AxesY2)
        {
            chart.AxesY.Add(axis);
        }
    }

    private void RemoveChart(ChartDefinition def)
    {
        if (_chartVisuals.TryGetValue(def, out var chart))
        {
            Grid.Children.Remove(chart);
            _chartVisuals.Remove(def);
        }
    }
    
    private void ClearAllCharts()
    {
        Grid.Children.Clear();
        _chartVisuals.Clear();

        Grid.RowDefinitions.Clear();
        Grid.ColumnDefinitions.Clear();
    }

    private ChartPosition CalculateChartPosition()
    {
        var index = Grid.Children.Count;
        int row, column;

        if (index < 4)
        {
            row = index / 2;
            column = index % 2;
        }
        else if (index == 4)
        {
            // Перемещаем 4-й чарт в позицию (0, 2)
            var curChart = Grid.Children[3];
            Grid.SetColumn(curChart, 2);
            Grid.SetRow(curChart, 0);

            row = 1;
            column = 1;

            EnsureGridSize(2, 3);
        }
        else
        {
            row = index / 3;
            column = index % 3;
        }

        return new ChartPosition(row, column);
    }

    private void AddChartToGrid(CartesianChart chart, ChartPosition position)
    {
        EnsureGridSize(position.Row + 1, position.Column + 1);

        Grid.Children.Add(chart);
        Grid.SetRow(chart, position.Row);
        Grid.SetColumn(chart, position.Column);
    }

    private void EnsureGridSize(int rows, int cols)
    {
        while (Grid.RowDefinitions.Count < rows)
            Grid.RowDefinitions.Add(new RowDefinition(GridLength.Star));

        while (Grid.ColumnDefinitions.Count < cols)
            Grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
    }

    public CartesianChart? GetChartVisual(ChartDefinition def)
    {
        return _chartVisuals.TryGetValue(def, out var chart) ? chart : null;
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        if (ViewModel != null)
        {
            ViewModel.Charts.CollectionChanged -= OnChartsChanged;
        }

        base.OnDetachedFromLogicalTree(e);
    }
}