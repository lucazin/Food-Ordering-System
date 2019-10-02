using System.Linq;
using System.Web.Mvc;

namespace Food_Order.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult SignUp()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"].ToString();
            }
            return View();
        }

        [HttpPost]
        public ActionResult SaveUser(User user)
        {
            using (FoodOrderEntities db = new FoodOrderEntities())
            {
                var uname = db.Users.Where(x => x.username == user.username).FirstOrDefault();
                var email = db.Users.Where(x => x.email == user.email).FirstOrDefault();
                if (uname == null && email == null)
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("SignUp");
                }
                else
                {
                    TempData["Message"] = "fail";
                    return RedirectToAction("SignUp");
                }
            }
        }

        public ActionResult Login()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"].ToString();
            }
            return View();
        }

        [HttpPost]
        public ActionResult CheckLogin(User user)
        {
            using (FoodOrderEntities db = new FoodOrderEntities())
            {

                var obj = db.Users.Where(a => a.username == user.username && a.password == user.password).FirstOrDefault();
                if (obj != null)
                {
                    if (obj.username == "admin")
                    {
                        Session["UserID"] = obj.userId;
                        Session["Username"] = obj.username;
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        Session["UserID"] = obj.userId;
                        Session["Username"] = obj.username;
                        return RedirectToAction("Index","Home");
                    }
                }
                else
                {
                    TempData["Message"] = "Invalid Login Credentials";
                    return RedirectToAction("Login");
                }        
            }
        }
        public ActionResult Logout()
        {
            Session["UserID"] = null;
            Session["Username"] = null;
            return RedirectToAction("Login");
        }
        
        public ActionResult LogoutUser()
        {
            Session["UserID"] = null;
            Session["Username"] = null;
            return Content("true");
        }


    }
}