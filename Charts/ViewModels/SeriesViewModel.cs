using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Eremex.AvaloniaUI.Charts;

namespace Charts.ViewModels;

public partial class SeriesViewModel :ObservableObject
{
    [ObservableProperty]
        Color color;
        
    [ObservableProperty] 
        ISeriesDataAdapter dataAdapter;
}