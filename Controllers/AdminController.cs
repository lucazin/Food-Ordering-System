using Food_Order.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Food_Order.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult ViewUsers()
        {
            using (FoodOrderEntities db = new FoodOrderEntities())
            {
                var data = db.Users.Where(x => x.username != "admin").ToList();
                return View(data);
            }
                
        }

        public ActionResult Index()
        {
            using (FoodOrderEntities db = new FoodOrderEntities())
            {
                ViewBag.OrderComplete = db.Orders.Where(x => x.status == "Complete").Count();
                ViewBag.OrderPending = db.Orders.Where(x => x.status == "Pending").Count();
                ViewBag.FoodItems = db.FoodItems.Where(x => x.isDeleted == false).Count();
                ViewBag.Users = db.Users.Where(x => x.username != "admin").Count();
                return View(GetTodayOrders());
            }
        }


        IEnumerable<UserOrderViewModel> GetTodayOrders()
        {
            var viewModels = new List<UserOrderViewModel>();

            using (FoodOrderEntities db = new FoodOrderEntities())
            {
                var today = DateTime.Now.ToString("dd/MM/yyyy");
                var innerjoin = db.Orders.Where(x => x.dateTime.Contains(today)).Join(
                    db.Users,
                    o => o.userId,
                    u => u.userId,
                    (o, u) => new
                    {
                        username = u.username,
                        orderId = o.orderId,
                        status = o.status,
                        totalPrice = o.totalPrice,
                        dateTime = o.dateTime
                    }).ToList();
                foreach (var item in innerjoin)
                {
                    UserOrderViewModel viewModel = new UserOrderViewModel
                    {
                        user = new User(),
                        order = new Order()
                    };
                    viewModel.user.username = item.username;
                    viewModel.order.orderId = item.orderId;
                    viewModel.order.status = item.status;
                    viewModel.order.dateTime = item.dateTime;
                    viewModel.order.totalPrice = item.totalPrice;
                    viewModels.Add(viewModel);
                }
                return viewModels;
            }
        }




        public ActionResult AddItem()
        {
            return View(GetAllCatItems());
        }

        CatItemViewModel GetAllCatItems()
        {
            using (FoodOrderEntities db = new FoodOrderEntities())
            {
                var catData = db.Categories.ToList();
                var itemData = db.FoodItems.Where(x => x.isDeleted == false).ToList();
                
                var viewModel = new CatItemViewModel() {
                    category = catData,
                    foodItem = itemData
                };
                return viewModel;
            }
        }

        public ActionResult AddCategory()
        {
            using (FoodOrderEntities db = new FoodOrderEntities())
            {
                var data = db.Categories.ToList();
                return View(data);
            }
        }
        [HttpPost]
        public ActionResult SaveCategory(Category category)
        {
            using (FoodOrderEntities db = new FoodOrderEntities())
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("AddCategory");
            }
        }

        [HttpPost]
        public ActionResult SaveItem(FoodItem foodItem)
        {
            var fileName = "";
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(file.FileName);
                    foodItem.image = fileName;
                    var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    file.SaveAs(path);
                }
            }

            using (FoodOrderEntities db = new FoodOrderEntities())
            {
                foodItem.isEnabled = true;
                db.FoodItems.Add(foodItem);
                db.SaveChanges();
                return RedirectToAction("AddItem"); 
            }
        }

        public ActionResult Order()
        {
            return View(GetAllUserOrders());
        }
        IEnumerable<UserOrderViewModel> GetAllUserOrders()
        {
            var viewModels = new List<UserOrderViewModel>();
           
            using (FoodOrderEntities db = new FoodOrderEntities())
            {
                var innerjoin = db.Orders.Join(
                    db.Users,
                    o => o.userId,
                    u => u.userId,
                    (o, u) => new
                    {
                       username = u.username,
                       orderId = o.orderId,
                       status = o.status,
                       totalPrice = o.totalPrice,
                       dateTime = o.dateTime
                    }).ToList();
                foreach (var item in innerjoin)
                {
                    UserOrderViewModel viewModel = new UserOrderViewModel
                    {
                        user = new User(),
                        order = new Order()
                    };
                    viewModel.user.username = item.username;
                    viewModel.order.orderId = item.orderId;
                    viewModel.order.status = item.status;
                    viewModel.order.dateTime = item.dateTime;
                    viewModel.order.totalPrice = item.totalPrice;
                    viewModels.Add(viewModel);
                }
                return viewModels;
            }
        }
        public ActionResult OrderDetails(int id)
        {
            return PartialView(GetUserOrderDetails(id));
        }
        IEnumerable<OrderItemViewModel> GetUserOrderDetails(int id)
        {
            var viewModels = new List<OrderItemViewModel>();

            using (FoodOrderEntities db = new FoodOrderEntities())
            {
                var innerjoin = db.OrderDetails.Where(x => x.orderId == id).Join(
                    db.FoodItems,
                    o => o.itemId,
                    u => u.itemId,
                    (o, u) => new
                    {
                        quantity = o.quantity,
                        name = u.name,
                        price = u.price,
                        description = u.description,
                        image = u.image
                    }).ToList();
                foreach (var item in innerjoin)
                {
                    OrderItemViewModel viewModel = new OrderItemViewModel
                    {
                        orderDetail = new OrderDetail(),
                        foodItem = new FoodItem()
                    };
                    viewModel.orderDetail.quantity = item.quantity;
                    viewModel.foodItem.name = item.name;
                    viewModel.foodItem.price = item.price;
                    viewModel.foodItem.image = item.image;
                    viewModel.foodItem.description = item.description;
                    viewModels.Add(viewModel);
                }
                return viewModels;
            }
        }
        public ActionResult ChangeStatus(int id)
        {
            using (FoodOrderEntities db = new FoodOrderEntities())
            {
                var obj = db.Orders.Where(x => x.orderId == id).FirstOrDefault();
                obj.status = "Complete";
                db.SaveChanges();
                return RedirectToAction("Order");
            }
                
        }
        public ActionResult DeleteItem(int id)
        {
            using (FoodOrderEntities db = new FoodOrderEntities())
            {
                var obj = db.FoodItems.Where(x => x.itemId == id).FirstOrDefault();
                obj.isDeleted = true;
                db.SaveChanges();
                return RedirectToAction("AddItem");
            }

        }
        public ActionResult DisableItem(int id)
        {
            using (FoodOrderEntities db = new FoodOrderEntities())
            {
                var obj = db.FoodItems.Where(x => x.itemId == id).FirstOrDefault();
                obj.isEnabled = false;
                db.SaveChanges();
                return RedirectToAction("AddItem");
            }

        }
        public ActionResult EnableItem(int id)
        {
            using (FoodOrderEntities db = new FoodOrderEntities())
            {
                var obj = db.FoodItems.Where(x => x.itemId == id).FirstOrDefault();
                obj.isEnabled = true;
                db.SaveChanges();
                return RedirectToAction("AddItem");
            }

        }
        public ActionResult Transactions()
        {
            return View(GetAllUserTransactions());
        }
        IEnumerable<UserOrderViewModel> GetAllUserTransactions()
        {
            var viewModels = new List<UserOrderViewModel>();

            using (FoodOrderEntities db = new FoodOrderEntities())
            {
                var innerjoin = db.Orders.Join(
                    db.Users.Where(x => x.username != "admin"),
                    o => o.userId,
                    u => u.userId,
                    (o, u) => new
                    {
                        username = u.username,
                        transId = o.transId,
                        totalPrice = o.totalPrice,
                        dateTime = o.dateTime
                    }).ToList();
                foreach (var item in innerjoin)
                {
                    UserOrderViewModel viewModel = new UserOrderViewModel
                    {
                        user = new User(),
                        order = new Order()
                    };
                    viewModel.user.username = item.username;
                    viewModel.order.transId = item.transId;
                    viewModel.order.dateTime = item.dateTime;
                    viewModel.order.totalPrice = item.totalPrice;
                    viewModels.Add(viewModel);
                }
                return viewModels;
            }
        }
    }

    
}