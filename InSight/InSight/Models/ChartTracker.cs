namespace InSight.Models;

public class ChartTracker
{
    public enum ActionType{Add, Remove}
    public ActionType Type { get; set; }
    public ChartData Chart { get; set; }
    public int Index { get; set; }


    public ChartTracker(ActionType type, ChartData chart, int index)
    {
        Type = type;
        Chart = chart;
        Index = index;
    }
}