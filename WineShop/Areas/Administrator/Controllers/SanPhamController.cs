using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WineShop.Models;
using PagedList;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.Web.Helpers;
using iTextSharp.text.pdf;
using iTextSharp.text;
namespace WineShop.Areas.Administrator.Controllers
{
    
    public class SanPhamController : Controller
    {
        ShopRuouDBEntities db = new ShopRuouDBEntities();
        // GET: Administrator/SanPham
        
        public ActionResult Index(string sortOrder, string currentFilter, string TimSanPham, int? page)
        {
            if(Session["DangNhapAdmin"] == null)
            {
                return RedirectToAction("Index", "DangNhap");
            }
            ViewBag.CurrentFilter = sortOrder;
            Session["a"] = "SanPham";
            ViewBag.TenSanPhams = string.IsNullOrEmpty(sortOrder) ? "sanpham_desc" : "";
            ViewBag.SoLuocXems = sortOrder == "luocxem" ? "luocxem" : "luocxem_desc";
            ViewBag.SoLuongBans = sortOrder == "luongban_desc" ? "luongban_desc" : "luongban";

            var spList = db.SanPhams.Where(s =>  s.BiXoa.Value == 0);

            switch (sortOrder)
            {
                case "luocxem_desc":
                    spList = spList.OrderByDescending(s => s.SoLuocXem);
                    break;
                case "luocxem":
                    spList = spList.OrderBy(s => s.SoLuocXem);
                    break;
                case "luongban_desc":
                    spList = spList.OrderByDescending(s => s.SoLuongBan);
                    break;
                case "luongban":
                    spList = spList.OrderBy(s => s.SoLuongBan);
                    break;
                case "sanpham_desc":
                    spList = spList.OrderByDescending(s => s.TenSanPham);
                    break;
                default:
                    spList = spList.OrderBy(s => s.TenSanPham);
                    break;
            }

            if (TimSanPham != null)
            {
                page = 1;
            }
            else
            {
                TimSanPham = currentFilter;
            }

            ViewBag.CurrentFilter = TimSanPham;

            if (!string.IsNullOrEmpty(TimSanPham))
            {
                spList = spList.Where(s => s.TenSanPham.Contains(TimSanPham.Trim()));
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(spList.ToPagedList(pageNumber, pageSize));
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
            var sp = db.SanPhams.Where(s => s.MaSanPham == id && s.BiXoa == 0).First<SanPham>();
            return View(sp);
        }

        public ActionResult ThemMoi()
        {
            if (Session["DangNhapAdmin"] == null)
            {
                return RedirectToAction("Index", "DangNhap");
            }
            List<LoaiSanPham> listLoaiSanPham = db.LoaiSanPhams.ToList<LoaiSanPham>();
            List<HangSanXuat> listHangSanXuat = db.HangSanXuats.ToList<HangSanXuat>();
            ViewBag.LoaiSanPhams = new SelectList(listLoaiSanPham, "MaLoaiSanPham", "TenLoaiSanPham");
            ViewBag.HangSanXuats = new SelectList(listHangSanXuat, "MaHangSanXuat", "TenHangSanXuat");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemMoi([Bind(Include = "MaSanPham, TenSanPham, GiaSanPham, NgayNhap, MoTa,SoLuongTon, MaLoaiSanPham, MaHangSanXuat, Hinh")] SanPham sanpham, IEnumerable<HttpPostedFileBase> upload)
        {
            if (Session["DangNhapAdmin"] == null)
            {
                return RedirectToAction("Index", "DangNhap");
            }
            if (ModelState.IsValid)
            {
                if (upload != null)
                {
                    sanpham.HinhAnhs = new List<WineShop.Models.HinhAnh>();
                    foreach (var file in upload)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            string extension = Path.GetFileNameWithoutExtension(file.FileName);
                            string timeUTC = DateTime.Now.ToFileTimeUtc().ToString();
                            string a = file.FileName.Replace(extension, timeUTC);

                            String logoPath = Path.GetFileName(a);
                            var hinh = new WineShop.Models.HinhAnh
                            {
                                TenHinh = logoPath,
                                BiXoa = false
                            };
                            file.SaveAs(Path.Combine(Server.MapPath("~/Images/"), hinh.TenHinh));
                            sanpham.HinhAnhs.Add(hinh);
                        }
                    }
                }
                sanpham.SoLuongBan = 0;
                sanpham.SoLuocXem = 0;

                sanpham.BiXoa = 0;
                db.SanPhams.Add(sanpham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            List<LoaiSanPham> listLoaiSanPham = db.LoaiSanPhams.ToList<LoaiSanPham>();
            List<HangSanXuat> listHangSanXuat = db.HangSanXuats.ToList<HangSanXuat>();
            ViewBag.LoaiSanPhams = new SelectList(listLoaiSanPham, "MaLoaiSanPham", "TenLoaiSanPham");
            ViewBag.HangSanXuats = new SelectList(listHangSanXuat, "MaHangSanXuat", "TenHangSanXuat");

            return View(sanpham);
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

            SanPham sp = db.SanPhams.Single(s => s.MaSanPham == id.Value);
            int chiTietDonDatHangCount = sp.ChiTietDonDatHangs.Count;
            var hinhAnhList = sp.HinhAnhs;
            if (hinhAnhList.Count > 0)
            {
                foreach (var r in hinhAnhList)
                {
                    string hinhCu = Request.MapPath("~/Images/" + r.TenHinh);
                    if (System.IO.File.Exists(hinhCu))
                    {
                        System.IO.File.Delete(hinhCu);
                    }
                    //r.MaSanPham = null;
                    //r.BiXoa = true;
                }
                var hinhAnhDB = db.HinhAnhs;
                foreach(var r in hinhAnhList.ToList())
                {
                    var hinhTemp = hinhAnhDB.Single(h => h.Ma == r.Ma);
                    if(hinhTemp != null)
                    {
                        db.HinhAnhs.Remove(hinhTemp);
                    }
                }
            }


            if (chiTietDonDatHangCount == 0)
            {
                db.SanPhams.Remove(sp);
            }else
            {
                sp.BiXoa = 1;
            }
            db.SaveChanges();
            return RedirectToAction("Index");

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
            SanPham sp = db.SanPhams.Single(s => s.MaSanPham == id);

            List<LoaiSanPham> listLoaiSanPham = db.LoaiSanPhams.ToList<LoaiSanPham>();
            List<HangSanXuat> listHangSanXuat = db.HangSanXuats.ToList<HangSanXuat>();
            ViewBag.LoaiSanPhams = new SelectList(listLoaiSanPham, "MaLoaiSanPham", "TenLoaiSanPham");
            ViewBag.HangSanXuats = new SelectList(listHangSanXuat, "MaHangSanXuat", "TenHangSanXuat");

            return View(sp);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhat([Bind(Include = "MaSanPham, TenSanPham, GiaSanPham, NgayNhap, SoLuongTon, MoTa, MaLoaiSanPham, MaHangSanXuat")] SanPham sanpham, IEnumerable<HttpPostedFileBase> upload, IEnumerable<String> GiaTri)
        {
            if (Session["DangNhapAdmin"] == null)
            {
                return RedirectToAction("Index", "DangNhap");
            }
            if (ModelState.IsValid)
            {
                SanPham sp = db.SanPhams.Single<SanPham>(s => s.MaSanPham == sanpham.MaSanPham );
                sp.TenSanPham = sanpham.TenSanPham;
                sp.MoTa = sanpham.MoTa;
                sp.NgayNhap = sanpham.NgayNhap;
                sp.SoLuongTon = sanpham.SoLuongTon;
                sp.GiaSanPham = sanpham.GiaSanPham;
                sp.MaLoaiSanPham = sanpham.MaLoaiSanPham;
                sp.MaHangSanXuat = sanpham.MaHangSanXuat;
                if(GiaTri != null)
                {
                    var listHinhAnh = sp.HinhAnhs;
                    foreach (var r in GiaTri)
                    {
                        int maHinh = Int16.Parse(r);
                        HinhAnh ha = listHinhAnh.Single(h => h.Ma == maHinh);

                        string hinhCu = Request.MapPath("~/Images/" + ha.TenHinh);
                        if (System.IO.File.Exists(hinhCu))
                        {
                            System.IO.File.Delete(hinhCu);
                        }
                        //ha.BiXoa = true;
                        //ha.MaSanPham = null;
                    }
                    var hinhAndList = db.HinhAnhs.ToList();
                    foreach(var r in GiaTri)
                    {
                        int maHinh = Int16.Parse(r);
                        HinhAnh ha = hinhAndList.Single(h=>h.Ma == maHinh);
                        db.HinhAnhs.Remove(ha);
                    }
                }

                var hinhanh = new List<HinhAnh>();

                foreach (var file in upload)
                {
                    if (file != null && file.ContentLength > 0)
                    {

                        string extension = Path.GetFileNameWithoutExtension(file.FileName);
                        string timeUTC = DateTime.Now.ToFileTimeUtc().ToString();
                        string a = file.FileName.Replace(extension, timeUTC);

                        String logoPath = Path.GetFileName(a);
                        var hinh = new WineShop.Models.HinhAnh
                        {
                            TenHinh = logoPath,
                            BiXoa = false
                        };
                        file.SaveAs(Path.Combine(Server.MapPath("~/Images/"), hinh.TenHinh));
                        hinhanh.Add(hinh);
                    }
                }
                if (hinhanh.Count > 0)
                {
                    foreach(var r in hinhanh)
                    {
                        sp.HinhAnhs.Add(r);
                    }
                }

                int n=  db.SaveChanges();

                return RedirectToAction("Index");
            }
            List<LoaiSanPham> listLoaiSanPham = db.LoaiSanPhams.ToList<LoaiSanPham>();
            List<HangSanXuat> listHangSanXuat = db.HangSanXuats.ToList<HangSanXuat>();
            ViewBag.LoaiSanPhams = new SelectList(listLoaiSanPham, "MaLoaiSanPham", "TenLoaiSanPham");
            ViewBag.HangSanXuats = new SelectList(listHangSanXuat, "MaHangSanXuat", "TenHangSanXuat");
            return View(sanpham);
        }

        public FileStreamResult BaoCaoSanPham()
        {
            var sp = db.SanPhams.Where(s=>s.BiXoa == 0).ToList<SanPham>();
            WebGrid gird = new WebGrid(source: sp, canPage: false, canSort: false);
            string girdHtml = gird.GetHtml(
                columns: gird.Columns(
                            gird.Column("MaSanPham", "STT"),
                            gird.Column("TenSanPham", "Tên"),
                            gird.Column("SoLuongTon", "SL")
                            )
                            ).ToString();
            string exportData = string.Format("<html><head><meta charset="+"utf - 8"+"/>{0}</head><body>{1}</body></html>", "<style>table{border-spacing:10px; border-collapse:separate;}</style>", girdHtml);
            var bytes = System.Text.Encoding.UTF8.GetBytes(exportData);
            using (var input = new MemoryStream(bytes))
            {
                var output = new MemoryStream();
                var document = new iTextSharp.text.Document(PageSize.A4, 50, 50, 50, 50);
                var writer = PdfWriter.GetInstance(document, output);
                writer.CloseStream = false;

                document.Open();

                var xmlWorker = iTextSharp.tool.xml.XMLWorkerHelper.GetInstance();
                xmlWorker.ParseXHtml(writer, document, input, System.Text.Encoding.UTF8);
                document.Close();
                output.Position = 0;
                return new FileStreamResult(output, "application/pdf");
            }

        }
    }

}
