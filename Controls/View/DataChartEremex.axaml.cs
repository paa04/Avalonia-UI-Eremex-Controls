using System.Collections.Specialized;
using Avalonia.Controls;
using Avalonia.Media;
using Controls.ViewModel;
using Eremex.AvaloniaUI.Charts;

namespace Controls.View;

public record ChartPosition(int Row, int Column);

public record SeriesAxisKeys(string? KeyX, string? KeyY);

public partial class DataChartEremex : UserControl
{
    public DataChartViewModel ViewModel => (DataChartViewModel)DataContext!;

    public DataChartEremex()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => SubscribeToViewModel();
    }

    public void SubscribeToViewModel()
    {
        if (ViewModel == null)
            return;

        ViewModel.Charts.CollectionChanged += OnChartsChanged;
    }

    private void OnChartsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is { } added)
        {
            foreach (ChartDefinition def in added)
            {
                AddChartVisual(def);
            }
        }
    }

    private void AddChartVisual(ChartDefinition def)
    {
        EnsureGridSize(def.Position.Row + 1, def.Position.Column + 1);

        var chart = new CartesianChart();

        foreach (var ax in def.AxesX)
            chart.AxesX.Add(ax);

        foreach (var ay in def.AxesY)
            chart.AxesY.Add(ay);

        foreach (var s in def.Series)
            chart.Series.Add(s);

        // ⬇️ Важно: подписываемся на изменения данных
        def.Series.CollectionChanged += (_, _) =>
        {
            chart.Series.Clear();
            foreach (var s in def.Series)
                chart.Series.Add(s);
        };

        def.AxesX.CollectionChanged += (_, _) =>
        {
            chart.AxesX.Clear();
            foreach (var ax in def.AxesX)
                chart.AxesX.Add(ax);
        };

        def.AxesY.CollectionChanged += (_, _) =>
        {
            chart.AxesY.Clear();
            foreach (var ay in def.AxesY)
                chart.AxesY.Add(ay);
        };

        Grid.Children.Add(chart);
        Grid.SetRow(chart, def.Position.Row);
        Grid.SetColumn(chart, def.Position.Column);
    }


    private void EnsureGridSize(int rows, int cols)
    {
        while (Grid.RowDefinitions.Count < rows)
            Grid.RowDefinitions.Add(new RowDefinition(GridLength.Star));

        while (Grid.ColumnDefinitions.Count < cols)
            Grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
    }
}