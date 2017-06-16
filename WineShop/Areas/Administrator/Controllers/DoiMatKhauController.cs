using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Globalization;
using System.Security.Cryptography;
using WineShop.Models;

namespace WineShop.Areas.Administrator.Controllers
{
    public class DoiMatKhauController : Controller
    {
        ShopRuouDBEntities db = new ShopRuouDBEntities();
        // GET: Administrator/DoiMatKhau
        public ActionResult Index()
        {
            if (Session["DangNhapAdmin"] == null || !Session["DangNhapAdmin"].ToString().Equals("boss"))
            {
                return RedirectToAction("Index", "DangNhap");
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ChangePasswordViewModel model)
        {
            if (Session["DangNhapAdmin"] == null || !Session["DangNhapAdmin"].ToString().Equals("boss"))
            {
                return RedirectToAction("Index", "DangNhap");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.NewPassword.Equals(model.ConfirmPassword))
            {
                // các xác định session hay 1 Identify.User() để lấy đc tên Id của khách hàng
                // biến temp
                string user_id = "minda-admin-min-ad"; 
                var admin = db.AspNetUsers.Where(h => h.Id.Equals(user_id)).Single();

                bool hash = VerifyHashedPassword(admin.PasswordHash, model.OldPassword);

                if (hash)
                {
                    admin.PasswordHash = HashPassword(model.NewPassword);
                    db.SaveChanges();
                    return RedirectToAction("Thoat", "DangNhap");
                }
            }     
            return View(model);
        }
        public static string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
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