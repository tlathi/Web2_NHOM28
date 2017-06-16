using System;

using System.Collections.Generic;

using System.Linq;

using System.Web;

using System.Web.Helpers;

using System.Web.Mvc;

using System.Web.UI.WebControls;

using WineShop.Models;

namespace WineShop.Areas.Administrator.Controllers

{

    public class BieuDoController : Controller

    {

        ShopRuouDBEntities db = new ShopRuouDBEntities();

        // GET: Administrator/BieuDo

        public ActionResult LoaiSanPham()

        {

            if (Session["DangNhapAdmin"] == null)

            {

                return RedirectToAction("Index", "DangNhap");

            }

            ViewBag.TenAction = "LoaiSanPham";

            ViewBag.titles = "BIỂU ĐỒ THỐNG KÊ THEO LOẠI SẢN PHẨM";

            var namList = db.DonDatHangs.Select(d => d.NgayLap.Value.Year).Distinct();

            ViewBag.NamLists = namList;

            ViewBag.imagePath = "lsp_chart.png";

            return PartialView("Index");

        }

        [HttpPost]

        public ActionResult LoaiSanPham(int? nam)

        {

            if (Session["DangNhapAdmin"] == null)

            {
                return RedirectToAction("Index", "DangNhap");
            }

            ViewBag.TenAction = "LoaiSanPham";

            var namList = db.DonDatHangs.Select(d => d.NgayLap.Value.Year).Distinct();

            ViewBag.NamLists = namList;

            Session["a"] = "BieuDo";

            ViewBag.titles = "BIỂU ĐỒ THỐNG KÊ THEO LOẠI SẢN PHẨM";

            ViewBag.imagePath = "lsp_chart.png";

            var _loaisanpham = from lsp in db.LoaiSanPhams

                               join sp in db.SanPhams on lsp.MaLoaiSanPham equals sp.MaLoaiSanPham

                               join ct in db.ChiTietDonDatHangs on sp.MaSanPham equals ct.MaSanPham

                               join ddh in db.DonDatHangs on ct.MaDonDatHang equals ddh.MaDonDatHang

                               where ddh.NgayLap.Value.Year == nam && ddh.MaTinhTrang == 3

                               group new { ct } by lsp.TenLoaiSanPham into ten

                               select new

                               {

                                   TenLoaiSanPham = ten.Key,

                                   tong = ten.Sum(t => t.ct.GiaBan.Value * t.ct.SoLuong.Value)

                               };

            createChart("loại sản phẩm", "loại sản phẩm", _loaisanpham, "TenLoaiSanPham", "lsp_chart.png", getBackColorForChart("blue"));

            return PartialView("Index");

        }



        public ActionResult HSX()

        {

            if (Session["DangNhapAdmin"] == null)

            {

                return RedirectToAction("Index", "DangNhap");

            }

            ViewBag.TenAction = "HSX";

            ViewBag.titles = "BIỂU ĐỒ THỐNG KÊ DOANH THU THEO HÃNG SẢN XUẤT";

            var namList = db.DonDatHangs.Select(d => d.NgayLap.Value.Year).Distinct();

            ViewBag.NamLists = namList;

            ViewBag.imagePath = "hsx_chart.png";

            return PartialView("Index");

        }

        [HttpPost]

        public ActionResult HSX(int? nam)

        {

            if (Session["DangNhapAdmin"] == null)

            {

                return RedirectToAction("Index", "DangNhap");

            }

            ViewBag.TenAction = "HSX";

            var namList = db.DonDatHangs.Select(d => d.NgayLap.Value.Year).Distinct();

            ViewBag.NamLists = namList;

            ViewBag.titles = "BIỂU ĐỒ THỐNG KÊ DOANH THU THEO HÃNG SẢN XUẤT";

            ViewBag.imagePath = "hsx_chart.png";

            var _hang = from h in db.HangSanXuats

                        join sp in db.SanPhams on h.MaHangSanXuat equals sp.MaHangSanXuat

                        join ct in db.ChiTietDonDatHangs on sp.MaSanPham equals ct.MaSanPham

                        join ddh in db.DonDatHangs on ct.MaDonDatHang equals ddh.MaDonDatHang

                        where ddh.NgayLap.Value.Year == nam && ddh.MaTinhTrang == 3

                        group new { ct } by h.TenHangSanXuat into ten

                        select new

                        {

                            TenHangSanXuat = ten.Key,

                            tong = ten.Sum(t => t.ct.GiaBan.Value * t.ct.SoLuong.Value)

                        };

            createChart("hãng sản xuất", "hãng sản xuất", _hang, "TenHangSanXuat", "hsx_chart.png", ChartTheme.Green);

            return PartialView("Index");

        }



        public ActionResult SanPham()

        {

            if (Session["DangNhapAdmin"] == null)

            {

                return RedirectToAction("Index", "DangNhap");

            }

            ViewBag.TenAction = "SanPham";

            ViewBag.titles = "BIỂU ĐỒ THỐNG KÊ DOANH THU THEO SẢN PHẨM";

            var namList = db.DonDatHangs.Select(d => d.NgayLap.Value.Year).Distinct();

            ViewBag.NamLists = namList;

            ViewBag.imagePath = "sp_chart.png";

            return PartialView("Index");

        }



        [HttpPost]

        public ActionResult SanPham(int? nam)

        {

            if (Session["DangNhapAdmin"] == null)

            {

                return RedirectToAction("Index", "DangNhap");

            }

            ViewBag.TenAction = "SanPham";

            var namList = db.DonDatHangs.Select(d => d.NgayLap.Value.Year).Distinct();

            ViewBag.NamLists = namList;

            ViewBag.titles = "BIỂU ĐỒ THỐNG KÊ DOANH THU THEO SẢN PHẨM";

            ViewBag.imagePath = "sp_chart.png";

            var _sanpham = from sp in db.SanPhams

                           join ct in db.ChiTietDonDatHangs on sp.MaSanPham equals ct.MaSanPham

                           join ddh in db.DonDatHangs on ct.MaDonDatHang equals ddh.MaDonDatHang

                           where ddh.NgayLap.Value.Year == nam && ddh.MaTinhTrang == 3

                           group new { ct } by sp.TenSanPham into ten

                           select new

                           {

                               TenSanPham = ten.Key,

                               tong = ten.Sum(t => t.ct.GiaBan.Value * t.ct.SoLuong.Value)

                           };



            foreach (var s in _sanpham)

            {

                int kn = s.tong;

                var kk = s.TenSanPham;

            }

            createChart("sản phẩm", "sản phẩm", _sanpham, "TenSanPham", "sp_chart.png", ChartTheme.Vanilla);

            return PartialView("Index");

        }



        public ActionResult Thang()

        {

            if (Session["DangNhapAdmin"] == null)

            {

                return RedirectToAction("Index", "DangNhap");

            }

            ViewBag.TenAction = "Thang";

            ViewBag.titles = "BIỂU ĐỒ THỐNG KÊ DOANH THU THEO THÁNG";

            var namList = db.DonDatHangs.Select(d => d.NgayLap.Value.Year).Distinct();

            ViewBag.NamLists = namList;

            ViewBag.imagePath = "thang_chart.png";

            return PartialView("Index");

        }

        [HttpPost]

        public ActionResult Thang(int? nam)

        {

            if (Session["DangNhapAdmin"] == null)

            {

                return RedirectToAction("Index", "DangNhap");

            }

            ViewBag.TenAction = "Thang";

            var namList = db.DonDatHangs.Select(d => d.NgayLap.Value.Year).Distinct();

            ViewBag.NamLists = namList;

            ViewBag.titles = "BIỂU ĐỒ THỐNG KÊ DOANH THU THEO THÁNG";

            ViewBag.imagePath = "thang_chart.png";

            var _thang = from ct in db.ChiTietDonDatHangs

                         join ddh in db.DonDatHangs on ct.MaDonDatHang equals ddh.MaDonDatHang

                         where ddh.TinhTrang.MaTinhTrang == 3 && ddh.NgayLap.Value.Year == nam

                         group ct by ddh.NgayLap.Value.Month into g

                         select new

                         {

                             Thang = g.Key.ToString(),

                             tong = g.Sum(t => t.GiaBan.Value * t.SoLuong.Value)

                         };

            createChart("tháng", "tháng", _thang, "Thang", "thang_chart.png", ChartTheme.Yellow);



            return PartialView("Index");

        }

        private void createChart(string name, string axis, IQueryable<Object> values, string xField, string typeChart, string theme)

        {

            var mChart = new Chart(Int16.Parse(Unit.Pixel(1200).Value.ToString()),

                                            Int16.Parse(Unit.Pixel(730).Value.ToString()),

                                            theme);



            mChart

                .AddSeries(

                        name: name,

                        axisLabel: axis,

                        xValue: values, xField: xField,

                        yValues: values, yFields: "tong"

                        )

                        .Save(path: "~/Images/Charts/" + typeChart);

        }



        private string getBackColorForChart(string color)

        {

            string Blue =

                "<Chart " +

                    "BackColor=\"#D3DFF0\"  " +

                    "BackGradientStyle=\"TopBottom\" " +

                    "BackSecondaryColor=\"Azure\" " +

                    "BorderColor=\"26, 59, 105\" " +

                    "BorderlineDashStyle=\"Solid\" " +

                    "BorderWidth=\"2\" " +

                    "Palette=\"BrightPastel\">\r\n    <ChartAreas>\r\n        <ChartArea Name=\"Default\" _Template_=\"All\" " +

                    "BackColor=\"64, 165, 191, 228\" BackGradientStyle=\"TopBottom\" BackSecondaryColor=\"Azure\" BorderColor=\"64, 64, 64, 64\" BorderDashStyle=\"Solid\" ShadowColor=\"Transparent\" /> \r\n    </ChartAreas>\r\n    <Legends>\r\n        <Legend _Template_=\"All\" BackColor=\"Red\" Font=\"Trebuchet MS, 8.25pt, style=Bold\" IsTextAutoFit=\"False\" /> \r\n    </Legends>\r\n    <BorderSkin SkinStyle=\"Emboss\" /> \r\n " +

                "</Chart>";

            //

            // Summary:

            //     A theme for 2D charting that features a visual container with a green gradient,

            //     rounded edges, drop-shadowing, and low-contrast gridlines.

            string Green = "<Chart BackColor=\"#C9DC87\" BackGradientStyle=\"TopBottom\" BorderColor=\"181, 64, 1\" BorderWidth=\"2\" BorderlineDashStyle=\"Solid\" Palette=\"BrightPastel\">\r\n  <ChartAreas>\r\n    <ChartArea Name=\"Default\" _Template_=\"All\" BackColor=\"Transparent\" BackSecondaryColor=\"White\" BorderColor=\"64, 64, 64, 64\" BorderDashStyle=\"Solid\" ShadowColor=\"Transparent\">\r\n      <AxisY LineColor=\"64, 64, 64, 64\">\r\n        <MajorGrid Interval=\"Auto\" LineColor=\"64, 64, 64, 64\" />\r\n        <LabelStyle Font=\"Trebuchet MS, 8.25pt, style=Bold\" />\r\n      </AxisY>\r\n      <AxisX LineColor=\"64, 64, 64, 64\">\r\n        <MajorGrid LineColor=\"64, 64, 64, 64\" />\r\n        <LabelStyle Font=\"Trebuchet MS, 8.25pt, style=Bold\" />\r\n      </AxisX>\r\n      <Area3DStyle Inclination=\"15\" IsClustered=\"False\" IsRightAngleAxes=\"False\" Perspective=\"10\" Rotation=\"10\" WallWidth=\"0\" />\r\n    </ChartArea>\r\n  </ChartAreas>\r\n  <Legends>\r\n    <Legend _Template_=\"All\" Alignment=\"Center\" BackColor=\"Transparent\" Docking=\"Bottom\" Font=\"Trebuchet MS, 8.25pt, style=Bold\" IsTextAutoFit =\"False\" LegendStyle=\"Row\">\r\n    </Legend>\r\n  </Legends>\r\n  <BorderSkin SkinStyle=\"Emboss\" />\r\n</Chart>";

            //

            // Summary:

            //     A theme for 2D charting that features no visual container and no gridlines.

            string Vanilla = "<Chart Palette=\"SemiTransparent\" BorderColor=\"#000\" BorderWidth=\"2\" BorderlineDashStyle=\"Solid\">\r\n<ChartAreas>\r\n    <ChartArea _Template_=\"All\" Name=\"Default\">\r\n            <AxisX>\r\n                <MinorGrid Enabled=\"False\" />\r\n                <MajorGrid Enabled=\"False\" />\r\n            </AxisX>\r\n            <AxisY>\r\n                <MajorGrid Enabled=\"False\" />\r\n                <MinorGrid Enabled=\"False\" />\r\n            </AxisY>\r\n    </ChartArea>\r\n</ChartAreas>\r\n</Chart>";

            //

            // Summary:

            //     A theme for 3D charting that features no visual container, limited labeling and,

            //     sparse, high-contrast gridlines.

            string Vanilla3D = "<Chart BackColor=\"#555\" BackGradientStyle=\"TopBottom\" BorderColor=\"181, 64, 1\" BorderWidth=\"2\" BorderlineDashStyle=\"Solid\" Palette=\"SemiTransparent\" AntiAliasing=\"All\">\r\n    <ChartAreas>\r\n        <ChartArea Name=\"Default\" _Template_=\"All\" BackColor=\"Transparent\" BackSecondaryColor=\"White\" BorderColor=\"64, 64, 64, 64\" BorderDashStyle=\"Solid\" ShadowColor=\"Transparent\">\r\n            <Area3DStyle LightStyle=\"Simplistic\" Enable3D=\"True\" Inclination=\"30\" IsClustered=\"False\" IsRightAngleAxes=\"False\" Perspective=\"10\" Rotation=\"-30\" WallWidth=\"0\" />\r\n        </ChartArea>\r\n    </ChartAreas>\r\n</Chart>";

            //

            // Summary:

            //     A theme for 2D charting that features a visual container that has a yellow gradient,

            //     rounded edges, drop-shadowing, and high-contrast gridlines.

            string Yellow = "<Chart BackColor=\"#FADA5E\" BackGradientStyle=\"TopBottom\" BorderColor=\"#B8860B\" BorderWidth=\"2\" BorderlineDashStyle=\"Solid\" Palette=\"EarthTones\">\r\n  <ChartAreas>\r\n    <ChartArea Name=\"Default\" _Template_=\"All\" BackColor=\"Transparent\" BackSecondaryColor=\"White\" BorderColor=\"64, 64, 64, 64\" BorderDashStyle=\"Solid\" ShadowColor=\"Transparent\">\r\n      <AxisY>\r\n        <LabelStyle Font=\"Trebuchet MS, 8.25pt, style=Bold\" />\r\n      </AxisY>\r\n      <AxisX LineColor=\"64, 64, 64, 64\">\r\n        <LabelStyle Font=\"Trebuchet MS, 8.25pt, style=Bold\" />\r\n      </AxisX>\r\n    </ChartArea>\r\n  </ChartAreas>\r\n  <Legends>\r\n    <Legend _Template_=\"All\" BackColor=\"Transparent\" Docking=\"Bottom\" Font=\"Trebuchet MS, 8.25pt, style=Bold\" LegendStyle=\"Row\">\r\n    </Legend>\r\n  </Legends>\r\n  <BorderSkin SkinStyle=\"Emboss\" />\r\n</Chart>";

            if (color.Equals("blue"))

            {

                return Blue;

            }

            else if (color.Equals("yellow"))

            {

                return Yellow;

            }
            else if (color.Equals("vanilla3D"))

            {

                return Vanilla3D;

            }
            else if (color.Equals("green"))

            {

                return Green;

            }
            else

            {

                return Vanilla;

            }

        }



    }

}