using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WineShop.Models;
using PagedList;
using System.Net;
using System.Data.Entity;

namespace WineShop.Areas.Administrator.Controllers
{
    public class KhachHangController : Controller
    {
        ShopRuouDBEntities db = new ShopRuouDBEntities();
        // GET: Administrator/KhachHang
        public ActionResult Index(string sortOrder, string currentFilter, string TimKhachHang, int? page)
        {
            if (Session["DangNhapAdmin"] == null ||  !Session["DangNhapAdmin"].ToString().Equals("boss"))
            {
                return RedirectToAction("Index", "DangNhap");
            }
            Session["a"] = "KhachHang";
            ViewBag.CurrentFilter = sortOrder;
            ViewBag.TenKhachHangs = string.IsNullOrEmpty(sortOrder) ? "khachhang" : "khachhang_desc";

            var kh = db.AspNetUsers.Where(h=>!h.Id.Equals("minda-admin-min-ad") );
      
            if (sortOrder != null && sortOrder.Equals("khachhang_desc"))
            {
                kh = kh.OrderByDescending(h => h.UserName);
            }
            else
            {
                kh = kh.OrderBy(h => h.UserName);
            }
            if (TimKhachHang != null)
            {
                page = 1;
            }
            else
            {
                TimKhachHang = currentFilter;
            }

            ViewBag.CurrentFilter = TimKhachHang;
            if (TimKhachHang != null)
            {
                kh = kh.Where(h => h.UserName.Contains(TimKhachHang.Trim()));
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(kh.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult CapNhat([Bind(Include = "Id")]AspNetUser user)
        {
            if (Session["DangNhapAdmin"] == null ||  !Session["DangNhapAdmin"].ToString().Equals("boss"))
            {
                return RedirectToAction("Index", "DangNhap");
            }
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            if (user.Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<AspNetRole> listQuyen = db.AspNetRoles.ToList<AspNetRole>();
            ViewBag.Quyens = new SelectList(listQuyen, "Id", "Name");
            var kh = db.AspNetUsers.Single(u => u.Id.Equals(user.Id));
            return View(kh);
        }
        [HttpPost]
        public ActionResult CapNhat([Bind(Include = "Id")]AspNetUser user, string RoleID)
        {
            if (Session["DangNhapAdmin"] == null ||  !Session["DangNhapAdmin"].ToString().Equals("boss"))
            {
                return RedirectToAction("Index", "DangNhap");
            }
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            // New class vừa tạo
            AspNetUserRoles a = new AspNetUserRoles();
            //kh.ID có thể truyền trực tiếp bằng mã/id không cần load từ Db Lên
            var khachhang = a.Users.Find(user.Id); //<<== Chỗ này
           
            var quyenHienTai = khachhang.AspNetRoles.Single();
            if (quyenHienTai.Id.Equals(RoleID))
            {
                return View(user);
            }

            var quyen = a.Roles.Find(RoleID);//<<== Chỗ này
            khachhang.AspNetRoles.Add(quyen);
            khachhang.AspNetRoles.Remove(quyenHienTai);
            //class vừa tạo SaveChanges()
            if (a.SaveChanges() > 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                List<AspNetRole> listQuyen = db.AspNetRoles.ToList<AspNetRole>();
                ViewBag.Quyens = new SelectList(listQuyen, "Id", "Name");
                return View(user);
            }
        }
        public ActionResult HuyPhanQuyen(string id)
        {
            if (Session["DangNhapAdmin"] == null ||  !Session["DangNhapAdmin"].ToString().Equals("boss"))
            {
                return RedirectToAction("Index", "DangNhap");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var kh = db.AspNetUsers.Single(u => u.Id.Equals(id));
            List<AspNetRole> quyen = kh.AspNetRoles.ToList<AspNetRole>();
            ViewBag.Quyens = new SelectList(quyen, "Id", "Name");
            return View(kh);
        }
        [HttpPost]
        public ActionResult HuyPhanQuyen([Bind(Include = "Id")]AspNetUser user, string RoleID)
        {
            if (Session["DangNhapAdmin"] == null ||  !Session["DangNhapAdmin"].ToString().Equals("boss"))
            {
                return RedirectToAction("Index", "DangNhap");
            }
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var kh = db.AspNetUsers.Single(u => u.Id.Equals(user.Id));
            
            // nếu 1 khách hàng chỉ còn 1 quyên thì không được xóa quyền nữa
            if (kh.AspNetRoles.Count == 1)
            {
                return RedirectToAction("Index");
            }
            AspNetUserRoles a = new AspNetUserRoles();
            var khachhang = a.Users.Find(user.Id);
            var quyen = a.Roles.Find(RoleID);
            khachhang.AspNetRoles.Remove(quyen);
            if (a.SaveChanges() > 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                List<AspNetRole> listQuyen = kh.AspNetRoles.ToList<AspNetRole>();
                ViewBag.Quyens = new SelectList(listQuyen, "Id", "Name");
                return View(user);
            }
        }

        public ActionResult Xoa([Bind(Include = "Id")]AspNetUser user)
        {
            if (Session["DangNhapAdmin"] == null ||  !Session["DangNhapAdmin"].ToString().Equals("boss"))
            {
                return RedirectToAction("Index", "DangNhap");
            }
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            var kh = db.AspNetUsers.Single<AspNetUser>(h => h.Id.Equals(user.Id));

            var soLuongDDH = kh.DonDatHangs.Count();
            if(soLuongDDH == 0)
            {
                // do trong chỉ có 1 row trong bảng mối quan hệ nhiều - nhiều
                // nên có thể lấy bằng lệnh Single
                // mục đích lấy đc Id  mà ApsNetRole
                var xoaQuyen = kh.AspNetRoles.Single();

                AspNetUserRoles a = new AspNetUserRoles();
                //tìm trong bảng phụ khách hàng
                var _khachhang = a.Users.Find(kh.Id);
                // tìm 1 quyền duy nhất của khách hàng
                var _quyen = a.Roles.Find(xoaQuyen.Id);
                // xóa khách hàng ra khỏi bảng phụ
                _quyen.AspNetUsers.Remove(_khachhang);
                // save lại bảng phụ
                // lúc này khách hàng đã hết Role nên có thể xóa.
                db.AspNetUsers.Remove(kh);
            }else
            {
                // nếu khách hàng có đơn hàng -> xóa ẩn
                kh.LockoutEnabled = false;
            }
            int n = db.SaveChanges();
            if(n > 0)
            {
                Session["Bikhoa"] = "true";
            }
            return RedirectToAction("Index");
        }

        public ActionResult PhucHoi(string id)
        {
            if (Session["DangNhapAdmin"] == null || !Session["DangNhapAdmin"].ToString().Equals("boss"))
            {
                return RedirectToAction("Index", "DangNhap");
            }
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            var kh = db.AspNetUsers.Single<AspNetUser>(h => h.Id.Equals(id));
            kh.LockoutEnabled = true;
            Session["Bikhoa"] = null;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}