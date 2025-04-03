using System.Runtime.CompilerServices;
using Avalonia.Controls;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;

namespace InSight.Models;

public class CartesianChartData : ChartData
{
    public CartesianChartData(string title, ISeries[] series, Axis[] xAxis, Axis[] yAxis)
    {
        Title = title;
        Chart = new CartesianChart
        {
            Width = 500,
            Height = 500,
            ZoomMode = ZoomAndPanMode.Both,
            Series = series,
            XAxes = xAxis,
            YAxes = yAxis
        };
    }
}