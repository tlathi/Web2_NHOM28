using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using WineShop.Models;

namespace WineShop.Areas.Administrator.Controllers
{
    
    public class DangNhapController : Controller
    {
        // GET: Administrator/DangNhap
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public  ActionResult Index(string username , string password)
        {
            ShopRuouDBEntities db = new ShopRuouDBEntities ();
            
            var result = db.AspNetUsers.Single(r => r.Email.Equals(username.Trim()));
            AspNetUserRoles a = new AspNetUserRoles();
            var tk = a.Users.Find(result.Id);
            var quyen = tk.AspNetRoles.SingleOrDefault();
            bool check = VerifyHashedPassword(result.PasswordHash, password);
            if(!check || !quyen.Id.Equals("2"))
            {
                return RedirectToAction("Index", "DangNhap");
            }
            Session["DangNhapAdmin"] = "true";
            if (result.Id.Equals("minda-admin-min-ad"))
            {
                Session["DangNhapAdmin"] = "boss";
            }
            return RedirectToAction("Index", "SanPham");
        }
        public ActionResult Thoat()
        {
            Session["DangNhapAdmin"] = null;
            return RedirectToAction("Index", "DangNhap");
        }
        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] buffer4;
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] src = Convert.FromBase64String(hashedPassword);
            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }
            //return ByteArraysEqual(buffer3, buffer4);
            return buffer3.SequenceEqual(buffer4);
        }
    }
}