using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCart.ViewModels
{
    //購物車頁面用ViewModel
    public class CartBuyViewModel
    {

        //購物車內商品陣列
        public List<CartBuy> DataList { get; set; }

        //購物車是否以保存
        public bool isCartSave { get; set; }
    }
}