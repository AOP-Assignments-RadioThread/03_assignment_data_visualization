using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
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
    
    private CsvHandler csv;

    private List<FoodWasteData> foodWasteDataList;
    private DataAnalyzer analyzer;

    public MainWindowViewModel()
    {
        csv = new CsvHandler();
        analyzer = new DataAnalyzer();
        
        Charts = new ObservableCollection<ChartData>();
    }

    [RelayCommand]
    private async Task ImportData()
    {
       foodWasteDataList =  await csv.GetDataAsync();
    }

    [RelayCommand]
    private void RemoveChart(ChartData chart)
    {
        Charts.Remove(chart);
    }


    [RelayCommand]
    private void AddTotalWasteByFoodCategoryAndYear()
    {
        Charts.Add(analyzer.TotalWasteByFoodCategoryByYear(foodWasteDataList, 2021));

    }
}