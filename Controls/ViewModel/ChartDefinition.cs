using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Controls.View;
using DynamicData;
using Eremex.AvaloniaUI.Charts;

namespace Controls.ViewModel;

public record AxesKey(string KeyX, string KeyY);

public class ChartDefinition : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private ChartPosition _position;
    private string _title = string.Empty;
    private bool _isVisible = true;

    private int _axisKey = 0;

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

    public ObservableCollection<CartesianSeries> Series { get; } = new();
    public ObservableCollection<AxisX> AxesX { get; } = new();
    public ObservableCollection<AxisX> AxesX2 { get; } = new();
    public ObservableCollection<AxisY> AxesY { get; } = new();
    public ObservableCollection<AxisY> AxesY2 { get; } = new();

    public void AddSeries<TView>(ISeriesDataAdapter adapter, Color color, AxesKey? key = null)
        where TView : CartesianSeriesView, new()
    {
        var view = new TView();
        SetColor(view, color);

        var series = new CartesianSeries
        {
            DataAdapter = adapter,
            View = view
        };

        if (key != null)
        {
            series.AxisXKey = key.KeyX;
            series.AxisYKey = key.KeyY;
        }

        Series.Add(series);
    }

    public void DeleteSeries(string seriesName)
    {
        var series = Series.Where(s => s.SeriesName == seriesName);
        Series.RemoveMany(series);
    }

    public void DeleteSeries(int seriesIndex)
    {
        Series.RemoveAt(seriesIndex);
    }

    public IEnumerable<CartesianSeries> FindSeriesIndex(string seriesName, string comment = "")
    {
        return Series.Where(s => s.SeriesName == seriesName);
    }

    public void AddSeries(ISeriesDataAdapter dataAdapter, Color color, string Name, SeriesChartType Type,
        bool XAxisPrimary,
        bool YAxisPrimary,
        int XAxisIndex, int YAxisIndex, string Unit = "", int GroupIndex = -1, CartesianSeries? seriesSettings = null)
    {
        CartesianSeries series;

        if (seriesSettings is not null)
            series = seriesSettings;
        else
        {
            var keyX = GetKeyXByIndex(XAxisIndex, XAxisPrimary);
            var keyY = GetKeyYByIndex(YAxisIndex, YAxisPrimary);

            var view = MapSeriesType(Type);
            SetColor(view, color);

            series = new CartesianSeries
            {
                DataAdapter = dataAdapter, Name = Name, View = view,
                AxisXKey = keyX, AxisYKey = keyY
            };
        }

        Series.Add(series);
    }

    public void AddAxisX()
    {
        var axis = new AxisX
        {
            Key = GetNewAxesKey(),
            ScaleOptions = new DateTimeScaleOptions
            {
                MeasureUnit = DateTimeUnit.Day,
            }
        };
        AxesX.Add(axis);
    }

    public void RemoveAxisX()
    {
        AxesX.RemoveAt(0);
    }

    public void AddAxisY()
    {
        var axis = new AxisY
        {
            Key = GetNewAxesKey()
        };
        AxesY.Add(axis);
    }

    public void RemoveAxisY()
    {
        AxesY.RemoveAt(0);
    }

    public void RemoveAxisY(int index)
    {
        AxesY.RemoveAt(index);
    }

    public void AddAxisX2()
    {
        var axis = new AxisX
        {
            Key = GetNewAxesKey(), Position = AxisPosition.Far,
            ScaleOptions = new DateTimeScaleOptions
            {
                MeasureUnit = DateTimeUnit.Day,
            }
        };
        AxesX2.Add(axis);
    }

    public void AddAxisY2()
    {
        var axis = new AxisY { Key = GetNewAxesKey(), Position = AxisPosition.Far };
        AxesY2.Add(axis);
    }

    public void RemoveAxisY2()
    {
        AxesY2.RemoveAt(AxesY2.Count - 1);
    }

    public void RemoveAxisY2(int index)
    {
        AxesY2.RemoveAt(index);
    }

    public void RemoveSeries(CartesianSeries series)
    {
        Series.Remove(series);
    }

    public AxisY? FindAxisY(string title)
    {
        return AxesY.FirstOrDefault(axis => axis.Title == title);
    }

    public AxisX? FindAxisX(string title)
    {
        return AxesX.FirstOrDefault(axis => axis.Title == title);
    }

    public void ClearSeries()
    {
        Series.Clear();
    }

    public void ClearAxes()
    {
        AxesX.Clear();
        AxesX2.Clear();
        AxesY.Clear();
        AxesY2.Clear();
    }

    private void SetColor(CartesianSeriesView view, Color color)
    {
        var property = view.GetType().GetProperty("Color");
        if (property != null && property.PropertyType == typeof(Color))
        {
            property.SetValue(view, color);
        }
    }

    public void LoadData<TView>(string[] data, Color color)
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

        AddSeries<TView>(adapter, color);
    }

    private CartesianSeriesView MapSeriesType(SeriesChartType chartType)
    {
        switch (chartType)
        {
            case SeriesChartType.Point:
                return new CartesianPointSeriesView();
            case SeriesChartType.Line:
                return new CartesianLineSeriesView();
            case SeriesChartType.Area:
                return new CartesianAreaSeriesView();
            case SeriesChartType.Column:
                return new CartesianSideBySideBarSeriesView();
            case SeriesChartType.BrokenLine:
                return new CartesianScatterLineSeriesView();
            default:
                throw new ArgumentOutOfRangeException(nameof(chartType), chartType, null);
        }
    }

    private string GetNewAxesKey()
    {
        var key = _axisKey.ToString();
        _axisKey++;
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

    // Метод для пакетного обновления (чтобы избежать множественных уведомлений)
    public void BeginUpdate()
    {
        _isUpdating = true;
    }

    public void EndUpdate()
    {
        _isUpdating = false;
        OnPropertyChanged(string.Empty); // Уведомляем о том, что все свойства могли измениться
    }

    private bool _isUpdating;
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