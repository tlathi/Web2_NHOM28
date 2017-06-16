using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WineShop.Models;

namespace WineShop.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //Session["BiKhoa"] = null;
            if (Session["BiKhoa"] != null)
            {
                return RedirectToAction("LogOffs", "Account");
            }
            string MaTK = null;
            MaTK = User.Identity.GetUserId();
            if (MaTK != null)
            {
                Session["DangNhap"] = "1";
            }

            if(Session["DangKyRoles"] != null && Session["DangKyRoles"].ToString().Equals("DKRoles"))
            {
                String userIDnek = User.Identity.GetUserId();
                AspNetUserRoles userR = new AspNetUserRoles();
                var u = userR.Users.Find(userIDnek);
                var _quyen = userR.Roles.Find("1");
                u.AspNetRoles.Add(_quyen);
                userR.SaveChanges();
                Session["DangKyRoles"] = null;
            }

            return View();
        }

        public ActionResult HienThiThongTin()
        {
            if (Session["BiKhoa"] != null)
            {
                return RedirectToAction("LogOffs", "Account");
            }
            return View();
        }

        public ActionResult HienThiLienHe()
        {
            if (Session["BiKhoa"] != null)
            {
                return RedirectToAction("LogOffs", "Account");
            }
            return View();
        }
    }
}