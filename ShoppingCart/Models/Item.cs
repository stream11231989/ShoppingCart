using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCart.Models
{
    public class Item
    {
        //編號
        public  int Id{ get; set; }
        //名稱
        public string Name { get; set; }
        //價錢
        public int Price { get; set; }
        //商品圖片
        public string Image { get; set; }
    }
}