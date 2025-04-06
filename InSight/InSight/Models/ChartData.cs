using Avalonia.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;

namespace InSight.Models;

public  class ChartData
{
    public string Title { get; set; }
    public  Control Chart { get; protected set; }
    
    // Store the data needed to recreate the chart
    protected virtual object GetChartState()
    {
        return null;
    }
    
    // Recreate the chart from state
    protected virtual Control CreateChartFromState(object state)
    {
        return null;
    }

    public virtual ChartData Clone()
    {
        // Create a completely new chart control instance
        var clone = new ChartData
        {
            Title = this.Title
        };
        
        // We don't directly copy the Chart property to avoid the visual parent issue
        return clone;
    }
}