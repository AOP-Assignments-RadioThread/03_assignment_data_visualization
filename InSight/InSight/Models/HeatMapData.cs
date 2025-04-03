using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;

namespace InSight.Models;

public class HeatMapData : ChartData
{
    public HeatMapData(string title, HeatLandSeries[] series)
    {
        Title = title;
        Chart = new GeoMap
        {
            Width = 1000,
            Height = 500,
            Series = series,
            MapProjection = MapProjection.Mercator,
        };
    }
}