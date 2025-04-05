using Avalonia.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;

namespace InSight.Models;

public  class ChartData
{
    public string Title { get; set; }
    public  Control Chart { get; protected set; }



    public virtual ChartData Clone()
    {
        return new ChartData
        {
            Title = this.Title,
            Chart = this.Chart
        };
    }

}