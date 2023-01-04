using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCart.ViewModels
{
    //商品資料頁面用Model
    public class ItemDetailViewModel
    {
        //新增商品內容
        public Item Data { get; set; }
        //是否在購物車中
        public bool InCart { get; set; }
    }
}