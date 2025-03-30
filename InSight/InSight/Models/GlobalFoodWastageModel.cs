using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InSight.Models
{
    class GlobalFoodWastageModel
    {
        public string Country { get; set; }
        public int Year { get; set; }
        public string FoodCategory { get; set; }
        public double TotalWaste { get; set; }
        public double EconomicLoss { get; set; }
        public double AvgWaste { get; set; }
        public double Population { get; set; }
        public double HouseholdWaste { get; set; }
    }
}
