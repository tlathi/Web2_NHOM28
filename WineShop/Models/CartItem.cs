using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WineShop.Models
{
    [Serializable]
    public class CartItem
    {
        public SanPham sanpham { get; set; }
        public int soluong { get; set; }
        public int tongtien { get; set; }
    }
}