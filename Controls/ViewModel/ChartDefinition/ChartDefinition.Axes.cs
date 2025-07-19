using System.Collections.ObjectModel;
using Eremex.AvaloniaUI.Charts;

namespace Controls.ViewModel.ChartDefinition;

public partial class ChartDefinition
{
    public ObservableCollection<AxisX> AxesX { get; } = new();
    public ObservableCollection<AxisX> AxesX2 { get; } = new();
    public ObservableCollection<AxisY> AxesY { get; } = new();
    public ObservableCollection<AxisY> AxesY2 { get; } = new();

    public void AddAxisX(ScaleOptions options) =>
        AxesX.Add(new AxisX { Key = GetNewAxesKey(), ScaleOptions = options });

    public void AddAxisX2(ScaleOptions options) =>
        AxesX2.Add(new AxisX { Key = GetNewAxesKey(), Position = AxisPosition.Far, ScaleOptions = options });

    public void AddAxisY(NumericScaleOptions options) =>
        AxesY.Add(new AxisY { Key = GetNewAxesKey(), ScaleOptions = options });

    public void AddAxisY2(NumericScaleOptions options) =>
        AxesY2.Add(new AxisY { Key = GetNewAxesKey(), Position = AxisPosition.Far, ScaleOptions = options });

    public void RemoveAxisX() => AxesX.RemoveAt(0);
    public void RemoveAxisY() => AxesY.RemoveAt(0);
    public void RemoveAxisY(int index) => AxesY.RemoveAt(index);
    public void RemoveAxisY2() => AxesY2.RemoveAt(AxesY2.Count - 1);
    public void RemoveAxisY2(int index) => AxesY2.RemoveAt(index);

    public void ClearAxes()
    {
        AxesX.Clear();
        AxesX2.Clear();
        AxesY.Clear();
        AxesY2.Clear();
    }

    public AxisX? FindAxisX(string title) =>
        AxesX.FirstOrDefault(a => a.Title == title) ?? AxesX2.FirstOrDefault(a => a.Title == title);

    public AxisY? FindAxisY(string title) =>
        AxesY.FirstOrDefault(a => a.Title == title) ?? AxesY2.FirstOrDefault(a => a.Title == title);
}