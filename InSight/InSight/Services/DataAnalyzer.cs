using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using InSight.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace InSight.Services;

public class DataAnalyzer
{
    
    private Dictionary<string,string> countryToIso3 = new Dictionary<string, string>
    {
        { "Australia", "aus" },
        { "Indonesia", "idn" },
        { "Germany", "deu" },
        { "France", "fra" },
        { "India", "ind" },
        { "China", "chn" },
        { "UK", "gbr" },
        { "South Africa", "zaf" },
        { "Japan", "jpn" },
        { "USA", "usa" },
        { "Brazil", "bra" },
        { "Saudi Arabia", "sau" },
        { "Italy", "ita" },
        { "Spain", "esp" },
        { "Mexico", "mex" },
        { "Argentina", "arg" },
        { "Canada", "can" },
        { "South Korea", "kor" },
        { "Russia", "rus" },
        { "Turkey", "tur" }
    };
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

    public ChartData GlobalTotalWasteTrend(List<FoodWasteData> foodWasteData)
    {
        var groupedGlobalTotalWasteByYear = foodWasteData.GroupBy(d => d.Year).Select(g => new
        {
            Year = g.Key,
            TotalWaste = Math.Round(g.Sum(x => x.TotalWaste), 2, MidpointRounding.AwayFromZero)
        }).OrderBy(x => x.Year).ToList();
        
        var years = groupedGlobalTotalWasteByYear.Select(x => x.Year.ToString()).ToList();
        var totals = groupedGlobalTotalWasteByYear.Select(x => x.TotalWaste).ToList();
        
        var chartData = new CartesianChartData(
            "Total Global Food Waste Trend Over Years",
            new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = totals,
                    Name = "Total Waste"
                }
            },
            new Axis[] { new Axis { Labels = years, Name = "Year", TextSize = 10 } },
            new Axis[] { new Axis { Name = "Total Waste (tons)" } }
        );
        
        return chartData;
    }

    public ChartData GlobalTotalWasteHeatMap2024(List<FoodWasteData> foodWasteData)
    {
        var data = foodWasteData
            .Where(g => g.Year == 2024)                // filter by year
            .GroupBy(g => g.Country)                   //  group by country
            .Select(g => new {
                Country = g.Key,                       // country name
                TotalWaste = g.Sum(x => x.TotalWaste)  // total waste for that country
            });
        
        var lands = data
            .Where(d => countryToIso3.ContainsKey(d.Country))
            .Select(d => new HeatLand
            {
                Name = countryToIso3[d.Country].ToLower(),
                Value = d.TotalWaste
            })
            .ToArray();
       
        var chartData = new HeatMapData("Total Waste Heat Map 2024", new HeatLandSeries[]{new HeatLandSeries{Lands = lands}});
        return chartData;
    }
}