using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WineShop.Models;
using PagedList;
using System.Net;
using System.IO;

namespace WineShop.Areas.Administrator.Controllers
{
    public class HangSanXuatController : Controller
    {
        ShopRuouDBEntities db = new ShopRuouDBEntities();
        // GET: Administrator/HangSanXuat
        public ActionResult Index(string sortOrder, string currentFilter, string TimHangSanXuat, int? page)
        {
            if (Session["DangNhapAdmin"] == null)
            {
                return RedirectToAction("Index", "DangNhap");
            }
            Session["a"] = "HangSanXuat";

            ViewBag.CurrentFilter = sortOrder;
            ViewBag.TenHangSanXuats = string.IsNullOrEmpty(sortOrder) ? "ten" : "ten_desc";

            var listHangSanXuat = db.HangSanXuats.Where(hsx => hsx.BiXoa == 0);


            if (sortOrder != null && sortOrder.Equals("ten"))
            {
                listHangSanXuat = listHangSanXuat.OrderBy(h => h.TenHangSanXuat);
            }
            else
            {
                listHangSanXuat = listHangSanXuat.OrderByDescending(h => h.TenHangSanXuat);
            }

            if (string.IsNullOrEmpty(TimHangSanXuat))
            {
                TimHangSanXuat = currentFilter;
            }
            else
            {
                page = 1;
            }

            ViewBag.CurrentFilter = TimHangSanXuat;

            if (!string.IsNullOrEmpty(TimHangSanXuat))
            {
                listHangSanXuat = listHangSanXuat.Where(h => h.TenHangSanXuat.Contains(TimHangSanXuat.Trim()));
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(listHangSanXuat.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ChiTiet(int? id)
        {
            if (Session["DangNhapAdmin"] == null)
            {
                return RedirectToAction("Index", "DangNhap");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var HSX = db.HangSanXuats.Where(h => h.MaHangSanXuat == id && h.BiXoa == 0).First<HangSanXuat>();
            return View(HSX);
        }

        public ActionResult ThemMoi()
        {
            if (Session["DangNhapAdmin"] == null)
            {
                return RedirectToAction("Index", "DangNhap");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemMoi([Bind(Include = "TenHangSanXuat")]HangSanXuat hsxuat, HttpPostedFileBase upload)
        {
            if (Session["DangNhapAdmin"] == null)
            {
                return RedirectToAction("Index", "DangNhap");
            }
            if (ModelState.IsValid)
            {
                string logoPath = "";
                if (upload != null && upload.ContentLength > 0)
                {
                    string extension = Path.GetFileNameWithoutExtension(upload.FileName);
                    string timeUTC = DateTime.Now.ToFileTimeUtc().ToString();
                    string a = upload.FileName.Replace(extension, timeUTC);
                    logoPath = Path.GetFileName(a);
                    upload.SaveAs(Path.Combine(Server.MapPath("~/Images/Logo/") + logoPath));
                }
                hsxuat.BiXoa = 0;
                hsxuat.LogoURL = logoPath;
                db.HangSanXuats.Add(hsxuat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hsxuat);
        }

        public ActionResult CapNhat(int? id)
        {
            if (Session["DangNhapAdmin"] == null)
            {
                return RedirectToAction("Index", "DangNhap");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var HSX = db.HangSanXuats.Where(h => h.MaHangSanXuat == id && h.BiXoa == 0).First<HangSanXuat>();
            return View(HSX);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhat([Bind(Include = "MaHangSanXuat, TenHangSanXuat")]HangSanXuat hsxuat, HttpPostedFileBase upload)
        {
            if (Session["DangNhapAdmin"] == null)
            {
                return RedirectToAction("Index", "DangNhap");
            }
            if (!ModelState.IsValid)
            {
                return View(hsxuat);
            }
            string logoPath = "";
            if (upload != null && upload.ContentLength > 0)
            {
                
                string extension = Path.GetFileNameWithoutExtension(upload.FileName);
                string timeUTC = DateTime.Now.ToFileTimeUtc().ToString();
                string a = upload.FileName.Replace(extension, timeUTC);
                
                logoPath =  Path.GetFileName(a);
                
                upload.SaveAs(Path.Combine(Server.MapPath("~/Images/Logo/") + logoPath));
            }

            var hsx = db.HangSanXuats.Single<HangSanXuat>(h => h.MaHangSanXuat == hsxuat.MaHangSanXuat);
            hsx.TenHangSanXuat = hsxuat.TenHangSanXuat;

            if (!string.IsNullOrEmpty(logoPath))
            {
                hsxuat.LogoURL = logoPath;

                string logoCu = Request.MapPath("~/Images/Logo/" + hsx.LogoURL);
                if (System.IO.File.Exists(logoCu))
                {
                    System.IO.File.Delete(logoCu);
                }

                hsx.LogoURL = hsxuat.LogoURL;
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Xoa(int? id)
        {
            if (Session["DangNhapAdmin"] == null)
            {
                return RedirectToAction("Index", "DangNhap");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var xoaHangSanXuat = db.HangSanXuats.Where(h => h.MaHangSanXuat == id).First<HangSanXuat>();
            int count = xoaHangSanXuat.SanPhams.Count;
            int countXoaAn = xoaHangSanXuat.SanPhams.Where(s => s.BiXoa == 1).Count();
            // hãng sản xuất không có sản phẩm
            if (count == 0)
            {
                db.HangSanXuats.Remove(xoaHangSanXuat);   
            }
            if(count == countXoaAn)
            {
                xoaHangSanXuat.BiXoa = 1;
            }
            int n = db.SaveChanges();
            if(n > 0)
            {
                string logoCu = Request.MapPath("~/Images/Logo/" + xoaHangSanXuat.LogoURL);
                if (System.IO.File.Exists(logoCu))
                {
                    System.IO.File.Delete(logoCu);
                }
            }


            return RedirectToAction("Index");

        }
    }
}