using AuthorizeNet.Api.Contracts.V1;
using Food_Order.Models;
using Food_Order.ViewModels;
using net.authorize.sample;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Controllers.Bases;
namespace Food_Order.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            using (FoodOrderEntities db = new FoodOrderEntities())
            {
                List<Category> categories = new List<Category>();
                var data = db.Categories.Join(
                    db.FoodItems.Where(x => x.isEnabled == true
                                && x.isDeleted == false
                                && x.quantity != "0"),
                    c => c.catId,
                    i => i.catId,
                    (c,i) => new // result selector
                    {
                       c.catId,
                       c.name
                    }).ToList().Distinct();
                foreach (var item in data)
                {
                    Category cat = new Category();
                    cat.catId = item.catId;
                    cat.name = item.name;
                    categories.Add(cat);
                }
                return View(categories);
            }

        }

        public ActionResult ShowCategoryItems(int id)
        {
            using (FoodOrderEntities db = new FoodOrderEntities())
            {
                CatItemViewModel viewModel = new CatItemViewModel();
                var itemData = db.FoodItems.Where(x => x.catId == id
                                && x.isEnabled == true
                                && x.isDeleted == false
                                && x.quantity != "0").ToList();
                var catData = db.Categories.Where(x => x.catId == id).ToList();
                viewModel.category = catData;
                viewModel.foodItem = itemData;
                return View(viewModel);
            }
        }
        [HttpPost]
        public ActionResult Checkout(List<CartItemArray> arr, int total)
        {
            using (FoodOrderEntities db = new FoodOrderEntities())
            {
                var response = ChargeCreditCard.Run(total);
                if (response != null && response.check == true)
                {
                    Order order = new Order();
                    order.transId = response.response;
                    order.userId = Convert.ToInt32(Session["UserID"].ToString());
                    order.status = "Pending";
                    order.dateTime = DateTime.Now.ToString("HH:mm tt dd/MM/yyyy");
                    order.totalPrice = total.ToString();
                    db.Orders.Add(order);
                    db.SaveChanges();
                    int id = Convert.ToInt32(order.orderId);
                    foreach (var item in arr)
                    {
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.orderId = id;
                        orderDetail.itemId = item.id;
                        orderDetail.quantity = item.qty.ToString();
                        db.OrderDetails.Add(orderDetail);
                        db.SaveChanges();
                        var foodItem = db.FoodItems.Where(x => x.itemId == item.id).FirstOrDefault();
                        foodItem.quantity = (Convert.ToInt32(foodItem.quantity) - item.qty).ToString();
                        db.SaveChanges();

                    }
                    return Content("true");
                }
                else
                {
                    return Content("false");
                }

            }
            
        }

    }
}