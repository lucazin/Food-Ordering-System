using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Food_Order.ViewModels
{
    public class UserOrderViewModel
    {
        public User user { get; set; }
        public Order order { get; set; }
    }
}