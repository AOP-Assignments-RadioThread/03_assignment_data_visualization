using System;
using System.Linq;
using Avalonia.Controls;
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace InSight.Models;

public class HeatMapData : ChartData
{
    private HeatLandSeries[] _series;
    private MapProjection _mapProjection;

    public HeatMapData(string title, HeatLandSeries[] series)
    {
        Title = title;
        _series = series;
        _mapProjection = MapProjection.Mercator;

        Chart = CreateChart();
    }
    
    private Control CreateChart()
    {
        return new GeoMap
        {
            Width = 1000,
            Height = 500,
            Series = _series,
            MapProjection = _mapProjection
        };
    }

    protected override object GetChartState()
    {
        return Tuple.Create(_series, _mapProjection);
    }
    
    protected override Control CreateChartFromState(object state)
    {
        if (state is Tuple<HeatLandSeries[], MapProjection> chartState)
        {
            var series = chartState.Item1;
            var projection = chartState.Item2;
            
            return new GeoMap
            {
                Width = 1000,
                Height = 500,
                Series = series,
                MapProjection = projection
            };
        }
        return null;
    }
    
    // Method to access internal data for undo/redo system
    public HeatLandSeries[] GetSeries() => _series;

    public override ChartData Clone()
    {
        // Deep copy each HeatLand
        var newSeries = _series?.Select(s =>
        {
            if (s == null || s.Lands == null)
                return new HeatLandSeries { Lands = Array.Empty<HeatLand>() };
                
            var clonedLands = s.Lands
                .Select(l => new HeatLand { Name = l.Name, Value = l.Value })
                .ToArray();

            return new HeatLandSeries
            {
                Lands = clonedLands
            };
        }).ToArray() ?? Array.Empty<HeatLandSeries>();

        return new HeatMapData(this.Title, newSeries);
    }
}