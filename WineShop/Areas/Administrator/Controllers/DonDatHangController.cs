using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WineShop.Models;
using PagedList;
using System.Net;

namespace WineShop.Areas.Administrator.Controllers
{
    public class DonDatHangController : Controller
    {
        ShopRuouDBEntities db = new ShopRuouDBEntities();
        // GET: Administrator/DonDatHang
        public ActionResult Index(string sortOrder, string currentFilter, string TimDonDatHang, int? page)
        {
            if (Session["DangNhapAdmin"] == null)
            {
                return RedirectToAction("Index", "DangNhap");
            }
            Session["a"] = "DonDatHang";
            ViewBag.CurrentFilter = sortOrder;
            ViewBag.NgayLaps = string.IsNullOrEmpty(sortOrder) ? "dondathang" : "dondathang_desc";

            var listDonDatHang = db.DonDatHangs.Select(l => l);

            if(sortOrder != null && sortOrder.Equals("dondathang_desc"))
            {
                listDonDatHang = listDonDatHang.OrderByDescending(d => d.NgayLap);
            }else
            {
                listDonDatHang = listDonDatHang.OrderBy(d => d.NgayLap);
            }
            
            if(TimDonDatHang != null)
            {
                page = 1;
            }else
            {
                TimDonDatHang = currentFilter;
            }
            ViewBag.CurrentFilter = TimDonDatHang;

            if(TimDonDatHang != null)
            {
                listDonDatHang = listDonDatHang.Where(d => d.AspNetUser.UserName.Contains(TimDonDatHang.Trim()));
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(listDonDatHang.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ChiTiet(string id)
        {
            if (Session["DangNhapAdmin"] == null)
            {
                return RedirectToAction("Index", "DangNhap");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var dondathang = db.DonDatHangs.Single(d => d.MaDonDatHang.Equals(id));
            return View(dondathang);
        }

        public ActionResult CapNhat(string id)
        {
            if (Session["DangNhapAdmin"] == null)
            {
                return RedirectToAction("Index", "DangNhap");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tinhtrang = db.TinhTrangs.Select(t => t);
            ViewBag.TinhTrangs = new SelectList(tinhtrang, "MaTinhTrang", "TenTinhTrang");

            var dondathang = db.DonDatHangs.Single(d => d.MaDonDatHang.Equals(id));
            return View(dondathang);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhat([Bind(Include ="MaDonDatHang, GhiChu, MaTinhTrang")]DonDatHang dondathang)
        {
            if (Session["DangNhapAdmin"] == null)
            {
                return RedirectToAction("Index", "DangNhap");
            }
            if (ModelState.IsValid)
            {
                var ddh = db.DonDatHangs.Single(d => d.MaDonDatHang.Equals(dondathang.MaDonDatHang));
                ddh.GhiChu = string.IsNullOrEmpty(dondathang.GhiChu) ? "" : dondathang.GhiChu;
                ddh.MaTinhTrang = dondathang.MaTinhTrang;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dondathang);
        }

    }
}