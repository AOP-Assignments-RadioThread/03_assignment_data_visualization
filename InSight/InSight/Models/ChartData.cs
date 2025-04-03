using Avalonia.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;

namespace InSight.Models;

public abstract class ChartData
{
    public string Title { get; set; }
    public  Control Chart { get; protected set; }
}