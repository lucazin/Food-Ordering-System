using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Food_Order.ViewModels
{
    public class CatItemViewModel
    {
        public List<Category> category { get; set; }
        public List<FoodItem> foodItem { get; set; }
    }
}