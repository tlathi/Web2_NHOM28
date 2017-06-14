using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WineShop.Models;
using PagedList;
namespace WineShop.Controllers
{
    public class SanPhamsController : Controller
    {
        ShopRuouDBEntities db = new ShopRuouDBEntities();

        [ChildActionOnly]
        public PartialViewResult Search()
        {
            var result = db.LoaiSanPhams.ToList();
            ViewBag.LSPs = new SelectList(result, "MaLoaiSanPham", "TenLoaiSanPham");

            var result2 = db.HangSanXuats.ToList();
            ViewBag.HSXs = new SelectList(result2, "MaHangSanXuat", "TenHangSanXuat");

            return PartialView();
        }

        [ChildActionOnly]
        public PartialViewResult SanPhamCungLoai(int id)
        {
            ViewBag.TieuDe = "Sản Phẩm Cùng Loại";
            SanPham sp = db.SanPhams.Single(s => s.MaSanPham == id);
            List<SanPham> lst = db.SanPhams.Where(s => s.BiXoa == 0 && s.MaLoaiSanPham == sp.MaLoaiSanPham && s.MaSanPham != id).Take(8).ToList<SanPham>();

            return PartialView( lst);
        }

        [ChildActionOnly]
        public ActionResult MenuLoai()
        {
            List<LoaiSanPham> lst = db.LoaiSanPhams.Where(s => s.BiXoa == 0).ToList<LoaiSanPham>();
            return PartialView(lst);
        }

        [ChildActionOnly]
        public ActionResult MenuHang()
        {
            List<HangSanXuat> lst = db.HangSanXuats.Where(s => s.BiXoa == 0).ToList<HangSanXuat>();
            return PartialView(lst);
        }


        public ActionResult HienThiSanPhamTheoLoai(int id, int? page)
        {
            if (Session["BiKhoa"] != null)
            {
                return RedirectToAction("LogOffs", "Account");
            }
            ViewBag.TenAction = "HienThiSanPhamTheoLoai";

            int pageNumber = (page ?? 1);
            int pageSize = 4;

            var lst = db.SanPhams.Where(s => s.BiXoa == 0 && s.MaLoaiSanPham == id).OrderBy(s=>s.MaSanPham);
            LoaiSanPham loai = db.LoaiSanPhams.Single(l => l.MaLoaiSanPham == id);
            ViewBag.TieuDe = "Danh sách sản phẩm theo Loại " + loai.TenLoaiSanPham;

            return View("DanhSachSanPham",lst.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult HienThiSanPhamTheoHang(int id, int? page)
        {
            if (Session["BiKhoa"] != null)
            {
                return RedirectToAction("LogOffs", "Account");
            }
            var lst = db.SanPhams.Where(s => s.BiXoa == 0 && s.MaHangSanXuat == id).OrderBy(s=>s.MaSanPham);
            HangSanXuat hang = db.HangSanXuats.Single(l => l.MaHangSanXuat == id);
            ViewBag.TieuDe = "Danh sách sản phẩm theo Hãng " + hang.TenHangSanXuat;
            int pageNumber = (page ?? 1);
            int pageSize = 4;
            return View("DanhSachSanPham", lst.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult HienThiChiTietSanPham(int id)
        {
            if (Session["BiKhoa"] != null)
            {
                return RedirectToAction("LogOffs", "Account");
            }
            //List<SanPham> lst = db.SanPhams.Where(s => s.BiXoa == 0 && s.MaSanPham == id).ToList<SanPham>();
            SanPham hang = db.SanPhams.Single(s=> s.BiXoa == 0 && s.MaSanPham == id);
            hang.SoLuocXem = hang.SoLuocXem + 1;
            db.SaveChanges();
            return View("ChiTiets", hang);
        }

        public ActionResult TimKiemSP(int? page)
        {
            if (Session["BiKhoa"] != null)
            {
                return RedirectToAction("LogOffs", "Account");
            }
            ViewBag.TieuDe = "Sản Phẩm Tìm Được";
            int pageNumber = (page ?? 1);
            int pageSize = 4;
            var sp = Session["DSTimKiemSanPham"] as IOrderedQueryable<SanPham>;
            if(sp == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View("DanhSachSanPham", sp.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult TimKiemSP(string txtTimkiem, int? MaLoaiSanPham, int? MaHangSanXuat, int? txtGia1, int? txtGia2, int? page)
        {
            if (Session["BiKhoa"] != null)
            {
                return RedirectToAction("LogOffs", "Account");
            }
            ViewBag.TieuDe = "Sản Phẩm Tìm Được";
            var sp = db.SanPhams.Where(s => s.BiXoa == 0).OrderBy(s => s.MaSanPham);
            int soLuong = sp.Count();
            int pageNumber = (page ?? 1);
            int pageSize = 4;

           
            if (txtTimkiem != null)
            {
                sp = sp.Where(s => s.TenSanPham.Contains(txtTimkiem)).OrderBy(s => s.MaSanPham);
            }
            if (MaLoaiSanPham != null)
            {
                sp = sp.Where(s => s.MaLoaiSanPham == MaLoaiSanPham.Value).OrderBy(s => s.MaSanPham);
            }
            if (MaHangSanXuat != null)
            {
                sp = sp.Where(s => s.MaHangSanXuat == MaHangSanXuat.Value).OrderBy(s => s.MaSanPham);
            }

            if (txtGia1 != null && txtGia1.Value >= 0)
            {
                sp = sp.Where(s => s.GiaSanPham >= txtGia1.Value).OrderBy(s => s.MaSanPham);
            }
            if (txtGia2 != null && txtGia2.Value >= 0)
            {
                sp = sp.Where(s => s.GiaSanPham <= txtGia2.Value).OrderBy(s => s.MaSanPham);
            }
            if(sp.Count() == soLuong)
            {                
                return RedirectToAction("Index","Home");
            }
            Session["DSTimKiemSanPham"] = sp;
            return View("DanhSachSanPham", sp.ToPagedList(pageNumber, pageSize));
        }

    }
}