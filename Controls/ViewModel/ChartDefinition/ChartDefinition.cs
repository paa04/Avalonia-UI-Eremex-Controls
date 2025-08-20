using System.ComponentModel;
using Avalonia;
using Controls.View;
using Eremex.AvaloniaUI.Charts;

namespace Controls.ViewModel.ChartDefinition;

public partial class ChartDefinition : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private ChartPosition _position;
    private string _title = string.Empty;
    private bool _isVisible = true;

    private int _axisKeyCounter = 0;

    public ChartPosition Position
    {
        get => _position;
        set
        {
            if (_position != value)
            {
                _position = value;
                OnPropertyChanged(nameof(Position));
            }
        }
    }

    public string Title
    {
        get => _title;
        set
        {
            if (_title != value)
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
    }

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (_isVisible != value)
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }
    }
    public IEnumerable<CartesianSeries> FindSeriesIndex(string seriesName, string comment = "")
    {
        return Series.Where(s => s.SeriesName == seriesName);
    }
    

    public void RequestScroll(Point point1, Point point2)
    {
        var deltaX = point2.X - point1.X;
        var deltaY = point2.Y - point1.Y;
        OnScrollRequested?.Invoke(this, new ScrollRequestEventArgs(deltaX, deltaY));
    }
    public event EventHandler<ScrollRequestEventArgs>? OnScrollRequested;
    

    private string GetNewAxesKey()
    {
        var key = _axisKeyCounter.ToString();
        _axisKeyCounter++;
        return key;
    }

    private string GetKeyXByIndex(int index, bool isPrimary)
    {
        if (isPrimary)
            return AxesX[index].Key;

        return AxesX2[index].Key;
    }

    private string GetKeyYByIndex(int index, bool isPrimary)
    {
        if (isPrimary)
            return AxesY[index].Key;

        return AxesY2[index].Key;
    }

    public void Update()
    {
        OnPropertyChanged(nameof(Update));
    }
}

public class ScrollRequestEventArgs : EventArgs
{
    public double DeltaX { get; }
    public double DeltaY { get; }
    public IEnumerable<Axis>? Axes { get; }

    public ScrollRequestEventArgs(double deltaX, double deltaY, IEnumerable<Axis>? axes = null)
    {
        DeltaX = deltaX;
        DeltaY = deltaY;
        Axes = axes;
    }
}