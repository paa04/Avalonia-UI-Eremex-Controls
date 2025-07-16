using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Media;
using Controls.View;
using Eremex.AvaloniaUI.Charts;

namespace Controls.ViewModel;

public class ChartDefinition : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    
    private ChartPosition _position;
    private string _title = string.Empty;
    private bool _isVisible = true;

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

    public ChartDefinition()
    {
        // Подписываемся на изменения коллекций для уведомления View
        Series.CollectionChanged += (_, e) => OnCollectionChanged(nameof(Series), e);
        AxesX.CollectionChanged += (_, e) => OnCollectionChanged(nameof(AxesX), e);
        AxesX2.CollectionChanged += (_, e) => OnCollectionChanged(nameof(AxesX2), e);
        AxesY.CollectionChanged += (_, e) => OnCollectionChanged(nameof(AxesY), e);
        AxesY2.CollectionChanged += (_, e) => OnCollectionChanged(nameof(AxesY2), e);
    }

    private void OnCollectionChanged(string collectionName, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(collectionName);
    }
    
    public void AddSeries<TView>(ISeriesDataAdapter adapter, Color color)
        where TView : CartesianSeriesView, new()
    {
        var view = new TView();
        SetColor(view, color);

        var series = new CartesianSeries
        {
            DataAdapter = adapter,
            View = view
        };

        Series.Add(series);
    }
    
    public void AddAxisX(AxisX axis)
    {
        AxesX.Add(axis);
    }

    public void AddAxisY(AxisY axis)
    {
        AxesY.Add(axis);
    }

    public void AddAxisX2(AxisX axis)
    {
        AxesX2.Add(axis);
    }

    public void AddAxisY2(AxisY axis)
    {
        AxesY2.Add(axis);
    }

    public void RemoveSeries(CartesianSeries series)
    {
        Series.Remove(series);
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