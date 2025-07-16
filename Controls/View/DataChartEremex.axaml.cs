using System.Collections.Specialized;
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
    // Словарь для отслеживания визуальных элементов чартов
    private readonly Dictionary<ChartDefinition, CartesianChart> _chartVisuals = new();

    public DataChartViewModel ViewModel => (DataChartViewModel)DataContext!;

    public DataChartEremex()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => SubscribeToViewModel();
    }

    public void SubscribeToViewModel()
    {
        // Отписываемся от предыдущих событий если они были
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

    // Метод для добавления НОВОГО чарта
    private void AddNewChart(ChartDefinition def)
    {
        // Создаем визуальный элемент чарта
        var chart = CreateChartVisual(def);
        
        // Определяем позицию
        var position = CalculateChartPosition();
        
        // Добавляем в Grid
        AddChartToGrid(chart, position);
        
        // Сохраняем связь между определением и визуальным элементом
        _chartVisuals[def] = chart;
        
        // Подписываемся на изменения конкретного чарта
        SubscribeToChartChanges(def, chart);
    }

    // Метод для создания визуального элемента чарта
    private CartesianChart CreateChartVisual(ChartDefinition def)
    {
        var chart = new CartesianChart();

        // Инициализируем оси и серии
        RefreshChartAxes(chart, def);
        RefreshChartSeries(chart, def);

        return chart;
    }

    // Подписка на изменения конкретного чарта
    private void SubscribeToChartChanges(ChartDefinition def, CartesianChart chart)
    {
        // Подписываемся на изменения серий
        def.Series.CollectionChanged += (_, _) => RefreshChartSeries(chart, def);
        
        // Подписываемся на изменения осей X
        def.AxesX.CollectionChanged += (_, _) => RefreshChartAxes(chart, def);
        
        // Подписываемся на изменения осей Y  
        def.AxesY.CollectionChanged += (_, _) => RefreshChartAxes(chart, def);
        
        // Подписываемся на изменения дополнительных осей если они есть
        def.AxesX2.CollectionChanged += (_, _) => RefreshChartAxes(chart, def);
        def.AxesY2.CollectionChanged += (_, _) => RefreshChartAxes(chart, def);
    }

    // Обновление серий чарта
    private void RefreshChartSeries(CartesianChart chart, ChartDefinition def)
    {
        chart.Series.Clear();
        foreach (var series in def.Series)
        {
            chart.Series.Add(series);
        }
    }

    // Обновление осей чарта
    private void RefreshChartAxes(CartesianChart chart, ChartDefinition def)
    {
        // Очищаем существующие оси
        chart.AxesX.Clear();
        chart.AxesY.Clear();

        // Добавляем оси X
        foreach (var axis in def.AxesX)
        {
            chart.AxesX.Add(axis);
        }

        // Добавляем дополнительные оси X
        foreach (var axis in def.AxesX2)
        {
            chart.AxesX.Add(axis);
        }

        // Добавляем оси Y
        foreach (var axis in def.AxesY)
        {
            chart.AxesY.Add(axis);
        }

        // Добавляем дополнительные оси Y
        foreach (var axis in def.AxesY2)
        {
            chart.AxesY.Add(axis);
        }
    }

    // Удаление чарта
    private void RemoveChart(ChartDefinition def)
    {
        if (_chartVisuals.TryGetValue(def, out var chart))
        {
            Grid.Children.Remove(chart);
            _chartVisuals.Remove(def);
        }
    }

    // Очистка всех чартов
    private void ClearAllCharts()
    {
        Grid.Children.Clear();
        _chartVisuals.Clear();
        
        // Очищаем определения Grid
        Grid.RowDefinitions.Clear();
        Grid.ColumnDefinitions.Clear();
    }

    // Вычисление позиции для нового чарта
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

    // Добавление чарта в Grid
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

    // Метод для получения визуального элемента чарта по определению
    public CartesianChart? GetChartVisual(ChartDefinition def)
    {
        return _chartVisuals.TryGetValue(def, out var chart) ? chart : null;
    }

    // Освобождение ресурсов
    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        // Отписываемся от всех событий
        if (ViewModel != null)
        {
            ViewModel.Charts.CollectionChanged -= OnChartsChanged;
        }

        base.OnDetachedFromLogicalTree(e);
    }
}