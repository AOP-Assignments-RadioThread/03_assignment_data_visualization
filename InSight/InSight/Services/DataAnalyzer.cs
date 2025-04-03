using System;
using System.Collections.Generic;
using System.Linq;
using InSight.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;

namespace InSight.Services;

public class DataAnalyzer
{
    public ChartData TotalWasteByFoodCategoryByYear(List<FoodWasteData> foodWasteData, int year)
    {
        var latestYearDataPerCountry = foodWasteData
            .GroupBy(d => d.Country)
            .Select(g =>
                g.Where(x => x.Year == year)
            )
            .SelectMany(x => x);

        var totalWasteByFoodCategory = latestYearDataPerCountry
            .GroupBy(d => d.FoodCategory)
            .Select(g => new
            {
                Category = g.Key,
                TotalWaste = Math.Round(g.Sum(x => x.TotalWaste), 2, MidpointRounding.AwayFromZero)
            })
            .OrderByDescending(x => x.TotalWaste)
            .ToList();

        var categoryList = totalWasteByFoodCategory.Select(x => x.Category).ToList();
        var totalList = totalWasteByFoodCategory.Select(x => x.TotalWaste).ToList();
     

        var chartData = new CartesianChartData(
            $"Total Waste By Food Category in {year}",
            new ISeries[] { new ColumnSeries<double>() { Values = totalList }  },
            new Axis[]{ new Axis { Labels = categoryList, LabelsRotation = 90, Name = "Category", TextSize = 10} },
            new Axis[] { new Axis { Name = "Total Food Waste (tons)" } }
        );
        
    return chartData;
    }

    // public ChartData TotalWasteTrend(List<FoodWasteData> foodWasteData, int year)
    // {
    //     var groupedFoodCategory = foodWasteData.GroupBy(d => d.FoodCategory);
    //     
    // }
}