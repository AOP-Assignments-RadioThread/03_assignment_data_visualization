using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging;
using Avalonia.Platform.Storage;
using InSight.Models;
using CsvHelper;
using CsvHelper.Configuration;

namespace InSight.Services;

public class CsvHandler
{
    public async Task<List<FoodWasteData>> GetDataAsync()
    {
        var application = Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

        // Get the top-level window
        var topLevel = TopLevel.GetTopLevel(application.MainWindow);
        if (topLevel == null)
        {
            return null;
        }

        // Open the file picker from top level
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open CSV File",
            AllowMultiple = false
        });

        if (files == null || files.Count == 0)
        {
            return null;
        }
        
        using var reader = new StreamReader(files[0].Path.AbsolutePath);
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture)){
            csv.Context.RegisterClassMap<FoodWasteDataMap>();
            var records = csv.GetRecords<FoodWasteData>().ToList();
            Console.WriteLine("Successfully loaded CSV file");
            return records;
        }
    }
    
}