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
    
    private Stack<ChartTracker> _undoStack { get; set; }
    private Stack<ChartTracker> _redoStack { get; set; }
    
    private CsvHandler csv;
    [ObservableProperty] private string _dataStatus = "No data loaded";
    
    

    private List<FoodWasteData> foodWasteDataList;
    private DataAnalyzer analyzer;

    public MainWindowViewModel()
    {
        csv = new CsvHandler();
        analyzer = new DataAnalyzer();
        
        
        Charts = new ObservableCollection<ChartData>();
        _undoStack = new Stack<ChartTracker>();
        _redoStack = new Stack<ChartTracker>();
    }

    [RelayCommand]
    private async Task ImportData()
    {
       foodWasteDataList =  await csv.GetDataAsync();
       if( foodWasteDataList.Count != 0) DataStatus = "Data loaded";
    }

    [RelayCommand]
    private void RemoveChart(ChartData chart)
    {
        var clone = chart.Clone();
        
        _undoStack.Push(new ChartTracker(ChartTracker.ActionType.Remove, clone, Charts.IndexOf(chart)));
        Charts.Remove(chart);
        
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
        if (_undoStack.Count > 0)
        {
            var action = _undoStack.Pop();
            var clone = action.Chart.Clone();
            if (action.Type == ChartTracker.ActionType.Add)
            {
                Charts.Remove(action.Chart);
                _redoStack.Push(new ChartTracker(action.Type, clone, action.Index));
            }
            else if (action.Type == ChartTracker.ActionType.Remove)
            {
                try
                {
                    Charts.Insert(action.Index, action.Chart);
                    _redoStack.Push(new ChartTracker(action.Type, clone, action.Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to undo insert: {action.Chart.Title}, {ex.Message}");
                }
            }
        }
    }

    [RelayCommand]
    private void Redo()
    {
        if (_redoStack.Count > 0)
        {
            var action = _redoStack.Pop();
            var clone = action.Chart.Clone();
            if (action.Type == ChartTracker.ActionType.Add)
            {
                Charts.Insert(action.Index, action.Chart);
                _undoStack.Push(new ChartTracker(action.Type, clone, action.Index));
            }
            else if (action.Type == ChartTracker.ActionType.Remove)
            {
                Charts.Remove(action.Chart);
                _undoStack.Push(new ChartTracker(action.Type, clone, action.Index));
            }
        }
    }

    public void AddChart(ChartData chart)
    {
        Charts.Add(chart);
        _undoStack.Push(new ChartTracker(ChartTracker.ActionType.Add, chart, Charts.IndexOf(chart)));
        _redoStack.Clear();
    }
}