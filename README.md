# 🍽️ InSight — Data Visualization App

## 📊 Overview

**InSight** is a desktop data visualization app built with **Avalonia** and **LiveCharts2**, following the **MVVM** architecture and applying **SOLID principles**. The app reads a food waste dataset (2018–2024) and helps users gain insights by displaying data through customizable, interactive charts.

Designed for ease of use, the app features a modern UI with a dynamic dashboard, a set of pre-defined queries, and intuitive chart controls.

---

## Features

### Core Features

- Import CSV data (via CsvHelper)
- Analyze data using LINQ
- Multiple chart types:
  - Line charts
  - Bar charts
  - Pie charts
  - Geographical heatmaps
- Preset queries:
  - Total waste by food category (by year)
  - Waste trend by food category over time
  - Global food waste trend
  - Heat map of waste by country
  - Proportion of waste by food category (pie chart)
- Dynamically add/remove charts to the dashboard
- Styled UI for a modern dashboard look

### Additional Feature: Undo/Redo Functionality

- **Undo**: Revert the last added or removed chart
- **Redo**: Restore the previously undone chart
- Built with an internal action stack to track and clone chart states safely

---

## How to Use

- Press the Import button on the top left;

- Open the .csv file provided in the Assets folder.

- Choose a chart from the left side.

- You can remove the charts by pressing the X button.

- There are also undo/redo buttons on the top right.
  
- You can scroll up and down to see al the charts, try to keep the mouse pointer outside of the charts when you scroll, our use the scroll bar.

---

## Architecture

- MVVM pattern
- Separated concerns:
  - `Models`: data structures (e.g., `FoodWasteData`)
  - `ViewModels`: command and UI logic
  - `Services`: CSV parsing and LINQ analysis (`CsvHandler`, `DataAnalyzer`)
  - `Views`: Avalonia XAML-based UI

---

## Dataset

- **Global Food Wastage Dataset (2018–2024)**
  - Source: [Kaggle](https://www.kaggle.com/datasets/atharvasoundankar/global-food-wastage-dataset-2018-2024)

---

## Notes

- Charts are responsive and styled for clarity
- Users can freely stack, remove, and toggle between different queries
- Undo/redo stacks are deep-copy safe to prevent UI/control reuse issues

---

## Authors

- Victor Petrica
- Vadim Iov
- Vlad Tomulescu
