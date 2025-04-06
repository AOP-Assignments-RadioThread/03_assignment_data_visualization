using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InSight.Models;
using InSight.Services;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;

namespace InSight.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<ChartData> Charts { get; set; }
    
    private Stack<ChartAction> _undoStack { get; set; }
    private Stack<ChartAction> _redoStack { get; set; }
    
    private CsvHandler csv;
    [ObservableProperty] private string _dataStatus = "No data loaded";
    
    private List<FoodWasteData> foodWasteDataList;
    private DataAnalyzer analyzer;

    // A new class to track chart operations with factory functions
    private class ChartAction
    {
        public enum ActionType { Add, Remove }
        
        public ActionType Type { get; }
        public int Index { get; }
        public string ChartType { get; }
        public Dictionary<string, object> State { get; }
        
        public ChartAction(ActionType type, int index, ChartData chart)
        {
            Type = type;
            Index = index;
            
            // Store the type of chart and its state
            ChartType = chart.GetType().FullName;
            State = new Dictionary<string, object>
            {
                ["Title"] = chart.Title
            };
            
            // Store chart-specific state
            if (chart is CartesianChartData cartesian)
            {
                State["Series"] = cartesian.GetSeries();
                State["XAxes"] = cartesian.GetXAxes();
                State["YAxes"] = cartesian.GetYAxes();
            } 
            else if (chart is PieChartData pie)
            {
                State["Series"] = pie.GetSeries();
            }
            else if (chart is HeatMapData heatMap)
            {
                State["Series"] = heatMap.GetSeries();
            }
        }
        
        public ChartData RecreateChart(DataAnalyzer analyzer, List<FoodWasteData> data)
        {
            // Simple recreation logic - in a real app, you might need more sophisticated chart recreation
            if (ChartType.Contains("CartesianChartData"))
            {
                if (State.TryGetValue("Series", out var series) && 
                    State.TryGetValue("XAxes", out var xAxes) && 
                    State.TryGetValue("YAxes", out var yAxes))
                {
                    return new CartesianChartData(
                        State["Title"] as string, 
                        series as ISeries[], 
                        xAxes as Axis[],
                        yAxes as Axis[]);
                }
            }
            else if (ChartType.Contains("PieChartData"))
            {
                if (State.TryGetValue("Series", out var series))
                {
                    return new PieChartData(
                        State["Title"] as string,
                        series as ISeries[]);
                }
            }
            else if (ChartType.Contains("HeatMapData"))
            {
                if (State.TryGetValue("Series", out var series))
                {
                    return new HeatMapData(
                        State["Title"] as string,
                        series as HeatLandSeries[]);
                }
            }
            
            // Fallback: recreate the chart from saved data based on title
            string title = State["Title"] as string;
            
            if (title.Contains("Total Waste By Food Category"))
                return analyzer.TotalWasteByFoodCategoryByYear(data, 2021);
            else if (title.Contains("Total Global Food Waste Trend"))
                return analyzer.GlobalTotalWasteTrend(data);
            else if (title.Contains("Heat Map"))
                return analyzer.GlobalTotalWasteHeatMap2024(data);
            else if (title.Contains("Food Category Waste"))
                return analyzer.ProportionOfWasteByCategoryAndYear(data);
            else if (title.Contains("Waste By Food Category Trend"))
                return analyzer.WasteByFoodCategoryTrend(data);
                
            return null;
        }
    }

    public MainWindowViewModel()
    {
        csv = new CsvHandler();
        analyzer = new DataAnalyzer();
        
        Charts = new ObservableCollection<ChartData>();
        _undoStack = new Stack<ChartAction>();
        _redoStack = new Stack<ChartAction>();
    }

    [RelayCommand]
    private async Task ImportData()
    {
       foodWasteDataList =  await csv.GetDataAsync();
       if(foodWasteDataList != null && foodWasteDataList.Count != 0) DataStatus = "Data loaded";
    }

    [RelayCommand]
    private void RemoveChart(ChartData chart)
    {
        int index = Charts.IndexOf(chart);
        if (index >= 0)
        {
            _undoStack.Push(new ChartAction(ChartAction.ActionType.Remove, index, chart));
            Charts.RemoveAt(index);
            _redoStack.Clear();
        }
    }

    [RelayCommand]
    private void TotalWasteByFoodCategoryAndYear()
    {
        if (foodWasteDataList != null)
        {
            var newChart = analyzer.TotalWasteByFoodCategoryByYear(foodWasteDataList, 2021);
            AddChart(newChart);
        }
    }

    [RelayCommand]
    private void GlobalTotalWasteTrend()
    {
        if (foodWasteDataList != null)
        {
            var newChart = analyzer.GlobalTotalWasteTrend(foodWasteDataList);
            AddChart(newChart);
        }
    }

    [RelayCommand]
    private void GlobalTotalWasteHeat()
    {
        if (foodWasteDataList != null)
        {
            var newChart = analyzer.GlobalTotalWasteHeatMap2024(foodWasteDataList);
            AddChart(newChart);
        }
    }

    [RelayCommand]
    private void Undo()
    {
        if (_undoStack.Count > 0 && foodWasteDataList != null)
        {
            var action = _undoStack.Pop();
            
            if (action.Type == ChartAction.ActionType.Add)
            {
                // Undo an add = remove the chart at the saved index
                if (action.Index < Charts.Count)
                {
                    var chart = Charts[action.Index];
                    Charts.RemoveAt(action.Index);
                    _redoStack.Push(new ChartAction(ChartAction.ActionType.Add, action.Index, chart));
                }
            }
            else if (action.Type == ChartAction.ActionType.Remove)
            {
                // Undo a remove = add the chart back at the saved index
                ChartData newChart = action.RecreateChart(analyzer, foodWasteDataList);
                if (newChart != null)
                {
                    if (action.Index <= Charts.Count)
                        Charts.Insert(action.Index, newChart);
                    else
                        Charts.Add(newChart);
                        
                    _redoStack.Push(new ChartAction(ChartAction.ActionType.Remove, action.Index, newChart));
                }
            }
        }
    }

    [RelayCommand]
    private void Redo()
    {
        if (_redoStack.Count > 0 && foodWasteDataList != null)
        {
            var action = _redoStack.Pop();
            
            if (action.Type == ChartAction.ActionType.Add)
            {
                // Redo an add = add the chart back
                ChartData newChart = action.RecreateChart(analyzer, foodWasteDataList);
                if (newChart != null)
                {
                    if (action.Index <= Charts.Count)
                        Charts.Insert(action.Index, newChart);
                    else
                        Charts.Add(newChart);
                        
                    _undoStack.Push(new ChartAction(ChartAction.ActionType.Add, action.Index, newChart));
                }
            }
            else if (action.Type == ChartAction.ActionType.Remove)
            {
                // Redo a remove = remove the chart again
                if (action.Index < Charts.Count)
                {
                    var chart = Charts[action.Index];
                    Charts.RemoveAt(action.Index);
                    _undoStack.Push(new ChartAction(ChartAction.ActionType.Remove, action.Index, chart));
                }
            }
        }
    }

    [RelayCommand]
    private void YearlyTrendByFoodCategory()
    {
        if (foodWasteDataList != null)
        {
            var newChart = analyzer.WasteByFoodCategoryTrend(foodWasteDataList);
            AddChart(newChart);
        }
    }

    [RelayCommand]
    private void ProportionOfWasteByCategoryAndYear()
    {
        if (foodWasteDataList != null)
        {
            var newChart = analyzer.ProportionOfWasteByCategoryAndYear(foodWasteDataList);
            AddChart(newChart);
        }
    }

    public void AddChart(ChartData chart)
    {
        Charts.Add(chart);
        _undoStack.Push(new ChartAction(ChartAction.ActionType.Add, Charts.Count - 1, chart));
        _redoStack.Clear();
    }
}