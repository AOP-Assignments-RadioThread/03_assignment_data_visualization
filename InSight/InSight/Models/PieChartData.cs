using Avalonia.Controls;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;

namespace InSight.Models;

public class PieChartData : ChartData
{
    private ISeries[] _series;
    
    public PieChartData(string title, ISeries[] series)
    {
        Title = title;
        _series = series;
        Chart = CreateChart();
    }
    
    private Control CreateChart()
    {
        return new PieChart()
        {
           Height = 500,
           Width = 500,
           Series = _series,
        };
    }
    
    protected override object GetChartState()
    {
        return _series;
    }
    
    protected override Control CreateChartFromState(object state)
    {
        if (state is ISeries[] series)
        {
            return new PieChart()
            {
                Height = 500,
                Width = 500,
                Series = series,
            };
        }
        return null;
    }
    
    // Method to access internal data for undo/redo system
    public ISeries[] GetSeries() => _series;
    
    public override ChartData Clone()
    {
        return new PieChartData(Title, _series);
    }
}