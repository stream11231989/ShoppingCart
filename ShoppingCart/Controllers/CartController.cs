using ShoppingCart.Services;
using ShoppingCart.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppingCart.Controllers
{
    public class CartController : Controller
    {
        //宣告Cart相關的Service物件
        private readonly CartService cartService = new CartService();
        // GET: Cart
        [Authorize]//設定此Action需登入
        public ActionResult Index()
        {
            //宣告一個新的頁面模型
            CartBuyViewModel Data = new CartBuyViewModel();
            //取得Session內購物資料
            //A?B:C的寫法，代表A如果是true，得到B；A如果是False，得到C
            //所以此行代表，如果Secction["Caer"]不是Null，則返回Secction["Caer"]的內容字串，否則返回Null值
            string Cart = (HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : null;

            //藉由Service並根據Session內儲存購物車編號
            //取得以放入購物車的商品資料陣列
            Data.DataList = cartService.GetItemFromCart(Cart);
            //解由Service來確認購物車是否以保存
            Data.isCartSave = cartService.CheckCartService(User.Identity.Name, Cart);
            return View(Data);
        }

        #region 保存購物車
        //保存使用者購物車資料Action
        [Authorize]
        public ActionResult CartSave()
        {

            //宣告接收購物車Service資料物件
            string Cart;

            //判斷Sessions內是否有值
            if(HttpContext.Session["Cart"] != null)
            {
                //設定購物車值
                Cart = HttpContext.Session["Cart"].ToString();
            }
            else
            {
                //重新定義購物車值
                Cart = DateTime.Now.ToString() + User.Identity.Name;
                //填入Session中
                HttpContext.Session["Cart"] = Cart;
            }
            //藉由Service來儲存購物車資料
            cartService.SaveCart(User.Identity.Name, Cart);
            //重心導向頁面
            return RedirectToAction("Index");
        }

        #endregion

        #region 取消保存購物車
        [Authorize]
        public ActionResult CartSaveRemove()
        {
            //藉由Service取消儲存購物車資料
            cartService.SaveCartRemove(User.Identity.Name);
            //重心導向頁面
            return RedirectToAction("Index");
        }

        #endregion

        #region 從購物車中取出
        [Authorize]
        public ActionResult Pop(int Id,string toPage)
        {
            //取得Session內資料
            string Cart = (HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : null;
            //藉由Service來將商品從購物車取出
            cartService.RemoveForCart(Cart, Id);
            if(toPage == "Item")//判斷傳入的Page來決定導向
            {
                return RedirectToAction("Item", "Item", new { Id = Id });
            }
            else if (toPage == "ItemBlock")
            {
                return RedirectToAction("ItemBlock", "Item", new { Id = Id });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region 放入購物車
        [Authorize]
        public ActionResult Put(int Id,string toPage)
        {
            //若Session中沒購物車資料，以使用者名稱與時間，新增一筆購物車資料
            if (HttpContext.Session["Cart"] == null)
            {
                HttpContext.Session["Cart"] = DateTime.Now.ToString() + User.Identity.Name;
            }
            //藉由Service來將商品放入購物車中
            cartService.AddtoCart(HttpContext.Session["Cart"].ToString(), Id);
            //判斷傳入的toPage來決定導向
            if (toPage == "Item")
            {
               return RedirectToAction("Item", "Item", new { Id = Id });
            }
            else if (toPage == "ItemBlock")
            {
               return RedirectToAction("ItemBlock", "Item", new { Id = Id });
            }
            else
            {
             return   RedirectToAction("Index", "Index", new { Id = Id });
            }
        }

        #endregion


    }
}