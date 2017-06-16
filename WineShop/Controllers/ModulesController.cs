using WineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WineShop.Controllers
{
    public class ModulesController : Controller
    {
        ShopRuouDBEntities db = new ShopRuouDBEntities();

        // GET: Modules


        [ChildActionOnly]
        public PartialViewResult TopHeader()
        {
            return PartialView();
        }

        [ChildActionOnly]
        public PartialViewResult HomeSlideder()
        {
            return PartialView();
        }

        [ChildActionOnly]
        public PartialViewResult TopBanner()
        {
            return PartialView();
        }

        [ChildActionOnly]
        public PartialViewResult TabProduct()
        {            
            ViewBag.TenTab1 = "Xem Nhiều";
            ViewBag.TenTab2 = "Bán Chạy Nhất";
            ViewBag.TenTab3 = "Mới Nhất";
            return PartialView("TabProduct");
        }

        [ChildActionOnly]
        public PartialViewResult XemNhieu()
        {
            List<SanPham> lstSanPham = db.SanPhams.Where(s => s.BiXoa == 0).OrderByDescending(s => s.SoLuocXem).Take(4).ToList<SanPham>();
            return PartialView("TabProductSP", lstSanPham);
        }

        [ChildActionOnly]
        public PartialViewResult BanChayNhat()
        {
            List<SanPham> lstSanPham = db.SanPhams.Where(s => s.BiXoa == 0).OrderByDescending(s => s.SoLuongBan).Take(4).ToList<SanPham>();
            return PartialView("TabProductSP", lstSanPham);
        }

        [ChildActionOnly]
        public PartialViewResult MoiNhat()
        {
            List<SanPham> lstSanPham = db.SanPhams.Where(s => s.BiXoa == 0).OrderByDescending(s => s.NgayNhap).Take(4).ToList<SanPham>();
            return PartialView("TabProductSP", lstSanPham);
        }

        [ChildActionOnly]
        public PartialViewResult BlockTrending()
        {
            List<SanPham> lstSanPham = db.SanPhams.Where(s => s.BiXoa == 0).OrderByDescending(s => s.SoLuongBan).Take(4).ToList<SanPham>();
            ViewBag.Ten = "Sản Phẩm Mua Nhiều Nhất";
            return PartialView("BlockTrending", lstSanPham);
        }

        [ChildActionOnly]
        public PartialViewResult BlockLogo()
        {
            List<SanPham> lstSanPham = db.SanPhams.Where(s => s.BiXoa == 0).ToList<SanPham>();
            return PartialView("BlockLogo", lstSanPham);
        }

        [ChildActionOnly]
        public PartialViewResult Footer()
        {
            return PartialView();
        }
    }
}