using ShoppingCart.Security;
using ShoppingCart.Services;
using ShoppingCart.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace ShoppingCart.Controllers
{
    public class MembersController : Controller
    {

        //宣告Members 資料表的Service物件
        private readonly MembersDBService membersService = new MembersDBService();

        //宣告寄信用的Service物件
        private readonly MailService mailService = new MailService();
        ////宣告Cart相關的Service物件
        //private readonly CartService cartservice = new CartService();

        // GET: Members
        public ActionResult Index()
        {
            return View();
        }

        #region 登入
        //登入一開始載入畫面
        public ActionResult Login()
        {
            //判斷使用者是否已經過登入驗證
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Item");//以登入則重新導向
            }
            return View();//否則進入登入畫面
        }

        //傳入登入的資料
        [HttpPost]
        public ActionResult Login(MembersLoginViewModel LoginMember)
        {
            //使用Service裡面的方法來驗證
            string ValidateStr = membersService.LoginCheck(LoginMember.Account, LoginMember.Password);
            //判斷驗證後的結果是否有錯誤訊息
            if (string.IsNullOrEmpty(ValidateStr))
            {
                ////無錯誤訊息，則登入
                ////先清空Session
                //HttpContext.Session.Clear();
                ////取得購物車保存
                //string Cart = cartService.GetCartSave(LoginMember.Account);
                ////判斷是否有保存，有的話則存入Session
                //if(Cart != null)
                //{
                //    HttpContext.Session["Cart"] = Cart;
                //}

                //藉由Service取得登入者腳色資料
                string RoleData = membersService.GetRole(LoginMember.Account);
                //設定JWT
                JwtService jwtService = new JwtService();
                //從web.config撈出資料
                //Cookie名稱

                string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();
                string Token = jwtService.GenerateToken(LoginMember.Account, RoleData);

                //產生一個Cookie
                HttpCookie cookie = new HttpCookie(cookieName);
                //設定單值
                cookie.Value = Server.UrlEncode(Token);
                //寫到用戶端
                Response.Cookies.Add(cookie);
                //設定Cookie期限
                Response.Cookies[cookieName].Expires = DateTime.Now.AddMinutes(Convert.ToInt32(WebConfigurationManager.AppSettings["ExpireMinutes"]));
                //重新導向頁面
                return RedirectToAction("Index", "Item");
            }
            else
            {
                //有驗證錯誤訊息，加入頁面模型中
                ModelState.AddModelError("", ValidateStr);
                //回傳資料至View中
                return View(LoginMember);
            }
        }

        #endregion

        #region 註冊
        //註冊一開始顯示葉面
        public ActionResult Register()
        {
            //判斷使用者是否已經過登入驗證
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Item");
            //已登入則重新導向
            //否則進入註冊畫面
            return View();
        }
        //傳入註冊資料的Action
        [HttpPost]
        //設定此Action只接受葉面POST資料傳入
        public ActionResult Register(MembersRegisterViewModel RegisterMember)
        {
            //判斷葉面資料是否都經過驗證
            if (ModelState.IsValid)
            {
                //將葉面資料中的密碼欄位填入
                RegisterMember.newMember.Password = RegisterMember.Password;
                //取得信箱驗證碼
                string AuthCode = mailService.GetValidateCode();
                //將信箱驗證碼填入
                RegisterMember.newMember.AuthCode = AuthCode;

                //呼叫Service註冊新會員
                membersService.Register(RegisterMember.newMember);
                //取得寫好的驗證信範本內容
                string TempMail = System.IO.File.ReadAllText(Server.MapPath("~/Views/Shared/RegisterEmailTemplate.html"));
                //宣告Email驗證用的Url
                UriBuilder ValidateUrl = new UriBuilder(Request.Url)
                {
                    Path = Url.Action("EmailValidate", "Members", new
                    {
                        Account = RegisterMember.newMember.Account,
                        AuthCode = AuthCode

                    })
                };
                //藉由Service將使用者資料填入驗證信範本中
                string MailBody = mailService.GetRegisterMailBady(TempMail, RegisterMember.newMember.Name, ValidateUrl.ToString().Replace("%3F", "?"));
                //呼叫Service寄出驗證信
                mailService.SendRegisterMail(MailBody, RegisterMember.newMember.Email);
                //用TempData儲存註冊訊息
                TempData["RegisterState"] = "註冊成功，請去收信以驗證Email";
                //重心導向葉面
                return RedirectToAction("RegisterResult");


            }
            //未經驗證清空密碼相關欄位
            RegisterMember.Password = null;
            RegisterMember.PasswordCheck = null;
            //將資料回填View中
            return View(RegisterMember);
        }

        #endregion

        #region 註冊結果顯示葉面
        public ActionResult RegisterResult()
        {
            return View();
        }

        //判斷註冊帳號是否已被註冊過Action
        public JsonResult AccountCheck(MembersRegisterViewModel RegisterMember)
        {
            //呼叫Service來判斷，並回傳結果
            return Json(membersService.AccountCheck(RegisterMember.newMember.Account), JsonRequestBehavior.AllowGet);
        }
        //接收驗證信連結傳進來的Action
        public ActionResult EmailValidate(string Account, string AuthCode)
        {
            //用ViewData儲存，使用Service進行信箱驗證後的結果訊息
            ViewData["EmailValidate"] = membersService.EmailValidate(Account, AuthCode);
            return View();
        }

        #endregion

        #region 修改密碼
        //修改密碼一開始仔入夜面
        [Authorize]//設定此Action需登入
        public ActionResult ChangePassword()
        {
            return View();
        }
        //修改密碼傳入資料Acttion
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(MembersChangePasswordViewModel ChangeData)
        {
            //判斷頁面是否都經過驗證
            if (ModelState.IsValid)
            {
                ViewData["ChangeState"] = membersService.ChangePassword(User.Identity.Name, ChangeData.Password, ChangeData.NewPassword);
            }
            return View();
        }

        #endregion

        #region 登出
        //登出Action
        public ActionResult Logout()
        {
            //使用者登出
            //Cookie名稱
            string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();
            //清除Cookie
            HttpCookie cookie = new HttpCookie(cookieName);
            cookie.Expires = DateTime.Now.AddDays(-1);
            cookie.Values.Clear();
            Response.Cookies.Set(cookie);
            //重新導向至登入Action
            return RedirectToAction("Login");
        }

        #endregion

    }
}