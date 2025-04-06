using System;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;

namespace InSight.Models;

public class CartesianChartData : ChartData
{
    private ISeries[] _series;
    private Axis[] _xAxes;
    private Axis[] _yAxes;
    private ZoomAndPanMode _zoomMode;

    public CartesianChartData(string title, ISeries[] series, Axis[] xAxis, Axis[] yAxis)
    {
        Title = title;
        _series = series;
        _xAxes = xAxis;
        _yAxes = yAxis;
        _zoomMode = ZoomAndPanMode.PanY;
        
        Chart = CreateChart();
    }
    
    private Control CreateChart()
    {
        return new CartesianChart
        {
            Width = 500,
            Height = 500,
            ZoomMode = _zoomMode,
            Series = _series,
            XAxes = _xAxes,
            YAxes = _yAxes
        };
    }
    
    protected override object GetChartState()
    {
        // Return a tuple with all the data needed to recreate the chart
        return Tuple.Create(_series, _xAxes, _yAxes, _zoomMode);
    }
    
    protected override Control CreateChartFromState(object state)
    {
        if (state is Tuple<ISeries[], Axis[], Axis[], ZoomAndPanMode> chartState)
        {
            var series = chartState.Item1;
            var xAxes = chartState.Item2;
            var yAxes = chartState.Item3;
            var zoomMode = chartState.Item4;
            
            return new CartesianChart
            {
                Width = 500,
                Height = 500,
                ZoomMode = zoomMode,
                Series = series,
                XAxes = xAxes,
                YAxes = yAxes
            };
        }
        
        return null;
    }
    
    // Methods to access internal data for undo/redo system
    public ISeries[] GetSeries() => _series; 
    public Axis[] GetXAxes() => _xAxes;
    public Axis[] GetYAxes() => _yAxes;
    
    public override ChartData Clone()
    {
        // Create a new instance with the same data
        return new CartesianChartData(Title, _series, _xAxes, _yAxes);
    }
}