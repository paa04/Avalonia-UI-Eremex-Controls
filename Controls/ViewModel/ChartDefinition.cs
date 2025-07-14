using System.Collections.ObjectModel;
using Controls.View;
using Eremex.AvaloniaUI.Charts;

namespace Controls.ViewModel;

public class ChartDefinition
{
    public ChartPosition Position { get; init; }

    public ObservableCollection<CartesianSeries> Series { get; } = new();
    public ObservableCollection<AxisX> AxesX { get; } = new();
    public ObservableCollection<AxisX> AxesX2 { get; } = new();
    public ObservableCollection<AxisY> AxesY { get; } = new();
    public ObservableCollection<AxisY> AxesY2 { get; } = new();
}