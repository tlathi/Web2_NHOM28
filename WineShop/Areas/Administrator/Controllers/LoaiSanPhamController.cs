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
    public class LoaiSanPhamController : Controller
    {
        // GET: Administrator/LoaiSanPham
        ShopRuouDBEntities db = new ShopRuouDBEntities();
        public ActionResult Index(string sortOrder, string currentFilter, string TimLoaiSanPham, int? page)
        {
            if (Session["DangNhapAdmin"] == null)
            {
                return RedirectToAction("Index", "DangNhap");
            }
            Session["a"] = "LoaiSanPham";
            ViewBag.CurrentFilter = sortOrder;
            ViewBag.TenLoaiSanPhams = string.IsNullOrEmpty(sortOrder) ? "ten_desc" : "ten";

            var listLoaiSanPham = db.LoaiSanPhams.Where(l => l.BiXoa == 0);

            if (sortOrder != null && sortOrder.Equals("ten_desc"))
            {
                listLoaiSanPham = listLoaiSanPham.OrderByDescending(l => l.TenLoaiSanPham);
            }
            else
            {
                listLoaiSanPham = listLoaiSanPham.OrderBy(l => l.TenLoaiSanPham);
            }

            if (!string.IsNullOrEmpty(TimLoaiSanPham))
            {
                page = 1;
            }
            else
            {
                TimLoaiSanPham = currentFilter;
            }

            ViewBag.CurrentFilter = TimLoaiSanPham;

            if (!string.IsNullOrEmpty(TimLoaiSanPham))
            {
                listLoaiSanPham = listLoaiSanPham.Where(l => l.TenLoaiSanPham.Contains(TimLoaiSanPham.Trim()));
            }


            int pageSize = 5;
            int pageNumber = (page ?? 1);


            return View(listLoaiSanPham.ToPagedList(pageNumber, pageSize));
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

            var lsp = db.LoaiSanPhams.Single(l => l.MaLoaiSanPham == id && l.BiXoa == 0);
            return View(lsp);
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

            var lsp = db.LoaiSanPhams.Single(l => l.MaLoaiSanPham == id && l.BiXoa == 0);
            return View(lsp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhat([Bind(Include = "MaLoaiSanPham, TenLoaiSanPham")]LoaiSanPham lsp)
        {
            if (Session["DangNhapAdmin"] == null)
            {
                return RedirectToAction("Index", "DangNhap");
            }
            if (ModelState.IsValid)
            {
                db.LoaiSanPhams.Single(l => l.MaLoaiSanPham == lsp.MaLoaiSanPham && l.BiXoa == 0).TenLoaiSanPham = lsp.TenLoaiSanPham;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(lsp);
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
        public ActionResult ThemMoi([Bind(Include = "TenLoaiSanPham")]LoaiSanPham lsp)
        {
            if (Session["DangNhapAdmin"] == null)
            {
                return RedirectToAction("Index", "DangNhap");
            }
            if (ModelState.IsValid)
            {
                lsp.BiXoa = 0;
                db.LoaiSanPhams.Add(lsp);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(lsp);
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
            var lsp = db.LoaiSanPhams.Single(l => l.MaLoaiSanPham == id );
            var count = lsp.SanPhams.Count;
            var countXoaAn = lsp.SanPhams.Where(s => s.BiXoa == 1).Count();
            if (count == 0)
            {
                db.LoaiSanPhams.Remove(lsp);
            }
            if(count == countXoaAn)
            {
                lsp.BiXoa = 1;
            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}