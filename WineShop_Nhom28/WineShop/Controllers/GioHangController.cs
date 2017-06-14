using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WineShop.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace WineShop.Controllers
{
    [Authorize]
    public class GioHangController : Controller
    {
        ShopRuouDBEntities db = new ShopRuouDBEntities();
        private const string CartSession = "CartSession";

        // GET: GioHang
        public ActionResult Index()
        {
            Session["a"] = "GioHang";
            var cart = Session[CartSession];

            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }

            return View(list);
        }

        [HttpGet]
        public ActionResult Nhap(int? id, int? Soluong)
        {
            if (Soluong != null && Soluong > 0 && Soluong <= db.SanPhams.Single(s => s.MaSanPham == id && s.BiXoa == 0).SoLuongTon)
            {
                return RedirectToAction("AddItem", "GioHang", new { id = id, sl = Soluong });
            }
            return RedirectToAction("HienThiChiTietSanPham", "SanPhams", new { id = id });
        }

        public ActionResult AddItem(int id, int sl)
        {
            var cart = Session[CartSession];
            if (cart != null)
            {
                var list = (List<CartItem>)cart;
                if (list.Exists(x => x.sanpham.MaSanPham == id))
                {
                    foreach (var item in list)
                    {
                        if (item.sanpham.MaSanPham == id)
                        {
                            int bientam = 0;
                            bientam = item.soluong + sl;
                            if (item.sanpham.SoLuongTon >= bientam)
                            {
                                item.soluong += sl;
                            }
                            else
                            {
                                return RedirectToAction("HienThiChiTietSanPham", "SanPhams", new { id = id });
                            }
                        }
                    }
                }
                else
                {
                    var item = new CartItem();
                    item.sanpham = db.SanPhams.Single(h => h.MaSanPham == id);
                    item.soluong = sl;
                    list.Add(item);
                }
                //Gán vào session
                Session[CartSession] = list;
            }
            else
            {
                var item = new CartItem();
                item.sanpham = db.SanPhams.Single(h => h.MaSanPham == id);
                item.soluong = sl;
                var list = new List<CartItem>();
                list.Add(item);
                Session[CartSession] = list;
            }

            return RedirectToAction("Index");
        }

        public ActionResult LichSuGiaoHang()
        {
            string IDKH = null;
            IDKH = User.Identity.GetUserId();
            List<DonDatHang> lst = db.DonDatHangs.Where(i => i.UserID == IDKH).ToList<DonDatHang>();
            return View(lst);
        }

        public ActionResult HuyDonHang(string maddh)
        {

            List<ChiTietDonDatHang> lst = db.ChiTietDonDatHangs.Where(i => i.MaDonDatHang == maddh).ToList<ChiTietDonDatHang>();
            DonDatHang ddh = db.DonDatHangs.Single(d => d.MaDonDatHang == maddh);
            ddh.MaTinhTrang = 4;
            SanPham sp = null;
            foreach (var item in lst)
            {
                sp = db.SanPhams.Single(s => s.MaSanPham == item.MaSanPham);
                sp.SoLuongTon += item.SoLuong;
                sp.SoLuongBan -= item.SoLuong;
            }
            db.SaveChanges();
            return RedirectToAction("LichSuGiaoHang", "GioHang");
        }

        public ActionResult XoaSP(int id)
        {
            var sessionCart = (List<CartItem>)Session[CartSession];
            sessionCart.RemoveAll(x => x.sanpham.MaSanPham == id);
            Session[CartSession] = sessionCart;
            return RedirectToAction("Index");
        }

        public ActionResult TangSL(int id, int sl)
        {
            var sessionCart = (List<CartItem>)Session[CartSession];
            foreach (var item in sessionCart)
            {
                if (item.sanpham.MaSanPham == id && item.sanpham.SoLuongTon > item.soluong)
                {
                    item.soluong++;
                }
            }
            Session[CartSession] = sessionCart;
            return RedirectToAction("Index");
        }

        public ActionResult GiamSL(int id, int sl)
        {
            var sessionCart = (List<CartItem>)Session[CartSession];
            foreach (var item in sessionCart)
            {
                if (item.sanpham.MaSanPham == id && item.soluong > 1)
                {
                    item.soluong--;
                }
            }
            Session[CartSession] = sessionCart;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ThanhToan(int tongtien, string GhiChu)
        {
            //Tạo đơn đặt hàng
            string strID = null;
            strID = User.Identity.GetUserId();
            DonDatHang ddh = new DonDatHang();
            int slddh = db.DonDatHangs.Count();
            string Maddh = "DDH0" + slddh;
            ddh.MaDonDatHang = Maddh;
            ddh.NgayLap = DateTime.Now;
            ddh.TongThanhTien = tongtien;
            ddh.MaTinhTrang = 1;
            ddh.UserID = strID;
            ddh.GhiChu = GhiChu;
            //Lưu đơn đặt hàng xuống DB


            //Tạo chi tiết đơn đặt hàng
            ChiTietDonDatHang ct = null;
            SanPham sp = null;
            var cart = (List<CartItem>)Session[CartSession];
            int slct = 0;
            string Mactdh = null;
            int k = 1;
            foreach (var item in cart)
            {
                ct = new ChiTietDonDatHang();
                slct = db.ChiTietDonDatHangs.Count();
                slct += k;
                k++;
                //k = slct + 1 + k;
                Mactdh = "CTDH" + slct;
                ct.MaChiTietDonDatHang = Mactdh;
                ct.SoLuong = item.soluong;
                ct.GiaBan = item.sanpham.GiaSanPham * ct.SoLuong;
                ct.MaSanPham = item.sanpham.MaSanPham;
                sp = db.SanPhams.Single(s => s.MaSanPham == ct.MaSanPham);
                sp.SoLuongTon -= ct.SoLuong;
                sp.SoLuongBan += ct.SoLuong;
                ddh.ChiTietDonDatHangs.Add(ct);
            }
            db.DonDatHangs.Add(ddh);
            db.SaveChanges();
            Session[CartSession] = null;
            //--\\
            return View();
        }
    }
}