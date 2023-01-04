using ShoppingCart.Services;
using ShoppingCart.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ShoppingCart.Controllers
{
    public class ItemController : Controller
    {
        //宣告Cart相關的Service物件
        private readonly CartService cartService = new CartService();

        //宣告Item相關的Service物件
        private readonly ItemService itemService = new ItemService();

        public ActionResult Index(int page = 1)
        {
            //宣告一個新頁面模型
            ItemViewModel Data = new ItemViewModel();
            //新增頁面模型中的分頁
            Data.Paging = new ForPaging(page);
            //從Service中取得頁面所需的陣列資料
            Data.IdList = itemService.GetIdList(Data.Paging);
            Data.ItemBlock = new List<ItemDetailViewModel>();
            foreach(var Id in Data.IdList)
            {
                //宣告一個新陣列內物件
                ItemDetailViewModel newBlock = new ItemDetailViewModel();
                //藉由Service取得商品資料
                newBlock.Data = itemService.GetDataById(Id);
                //取得Session內購物車資料(檢查是否有資料，有則回傳字串，沒有則回傳空值)
                string Cart = (HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : null;
                //藉由Service確認是否於購物車中
                newBlock.InCart = cartService.CheckInCart(Cart, Id);
                Data.ItemBlock.Add(newBlock);

                
            }
            return View(Data);
        }

        #region 商品頁面

        //商品頁面要根據傳入編號來定要決定要顯示的資料
        public ActionResult Item(int Id)
        {
            //宣告一個新的頁面模型
            ItemDetailViewModel ViewData = new ItemDetailViewModel();
            //節由Service取得商品資料
            ViewData.Data = itemService.GetDataById(Id);
            //取得Session內資料
            string Cart = (HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : null;
            //藉由Service確認是否存於購物車中
            ViewData.InCart = cartService.CheckInCart(Cart, Id);
            //將資料回傳View中
            return View(ViewData);

        }

        #endregion

        #region 商品列表區塊
        //商品列表中每一個區塊Action
        public ActionResult ItemBlock(int Id)
        {
            //宣告一個新頁面模型
            ItemDetailViewModel ViewData = new ItemDetailViewModel();
            //節由Service取得商品
            ViewData.Data = itemService.GetDataById(Id);
            //取得Session內購物車資料
            string Cart = (HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : null;
            //藉由Service確認是否存於購物車中
            ViewData.InCart = cartService.CheckInCart(Cart, Id);

            return PartialView(ViewData);
        }

        #endregion

        #region 新增商品
        //新增商品一開始載入頁面
        [Authorize(Roles ="Admin")]//設定此頁面只有Admin腳色才可使用
        public ActionResult Create()
        {
            return View();
        }

        //新增商品傳入資料時的Action
        [Authorize(Roles ="Admin")]
        [HttpPost]//設定此頁面僅接受頁面POST傳入
        public ActionResult Add(ItemCreateViewModel Data)
        {
            if (Data.ItemImage != null)
            {
                //取得檔名
                string filename = Path.GetFileName(Data.ItemImage.FileName);
                //將檔案和伺服器上路徑合併
                string Url = Path.Combine(Server.MapPath("~/Upload/"), filename);
                //將檔案存於伺服器上
                Data.ItemImage.SaveAs(Url);
                //設定路徑
                Data.NewData.Image = filename;
                //用Service來新增一筆商品資料
                itemService.Insert(Data.NewData);
                //重心導向頁面致開始頁面
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("ItemImage", "請選擇上傳檔案");
                return View(Data);                 
            }
        }

        #endregion

        #region 刪除商品
      [Authorize(Roles ="Admin")]
      public ActionResult Delete(int Id)
        {
            //使用Service來刪除資料
            itemService.Delete(Id);

            //重心導向頁面致開始頁面
            return RedirectToAction("Index");
        }

        #endregion

    }
}