using System.Linq;
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace InSight.Models;

public class HeatMapData : ChartData
{
    private HeatLandSeries[] _series;

    
    public HeatMapData(string title, HeatLandSeries[] series)
    {
        Title = title;

        _series = series;

        Chart = new GeoMap
        {
            Width = 1000,
            Height = 500,
            Series = series,
            MapProjection = MapProjection.Mercator
        };
    }

    // Deep copy

    public override ChartData Clone()
    {
        // Deep copy each HeatLand
        var newSeries = _series.Select(s =>
        {
            var clonedLands = s.Lands
                .Select(l => new HeatLand { Name = l.Name, Value = l.Value })
                .ToArray();

            return new HeatLandSeries
            {
                Lands = clonedLands
            };
        }).ToArray();

        return new HeatMapData(this.Title, newSeries);
    }
}