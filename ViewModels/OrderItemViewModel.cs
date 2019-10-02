using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Food_Order.ViewModels
{
    public class OrderItemViewModel
    {
        public OrderDetail orderDetail { get; set; }
        public FoodItem foodItem { get; set; }
    }
}