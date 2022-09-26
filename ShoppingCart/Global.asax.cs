using Jose;
using ShoppingCart.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ShoppingCart
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        
        //撰寫驗證前執行的動作
        //在此用於設定腳色(role)
        protected void Application_OnPostAuthenticateRequest(object sender,EventArgs e)
        {
            //接收請求資料
            HttpRequest httpRequest = HttpContext.Current.Request;
            //設定JWT密鑰
            string SecretKey = WebConfigurationManager.AppSettings["SecretKey"].ToString();
            //設定Cookie名稱
            string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();

            //檢查Cookie內是否存放Token
            if (httpRequest.Cookies[cookieName] != null)
            {
                //將Token還原
                JwtObject jwtObject = JWT.Decode<JwtObject>(Convert.ToString(httpRequest.Cookies[cookieName].Value), Encoding.UTF8.GetBytes(SecretKey), JwsAlgorithm.HS512);

                //將使用者腳色資料取出，併分割成陣列
                string[] roles = jwtObject.Role.Split(new char[] { ',' });
                //自行建立Identity 取代HttpContext.Current.User的Identity
                //將資料塞進Claim內做設計
                Claim[] claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, jwtObject.Account),
                    new Claim(ClaimTypes.NameIdentifier,jwtObject.Account)
                };
                var claimsIdentity = new ClaimsIdentity(claims, cookieName);
                //加入 identityprovider這個claim 史的反仿冒語彙@html.AntiForgeryToken()能通過
                claimsIdentity.AddClaim(
                    new Claim(@"http://schemas.micrrosoft.com/accesscontrolservice/2010/07/claims/identityprovider", "My Identity", @"http://www.w3.org/2001/XMLSchema#string"));
                //指派腳色到目前這個HttpContext的User物件去
                HttpContext.Current.User = new GenericPrincipal(claimsIdentity, roles);
                Thread.CurrentPrincipal = HttpContext.Current.User;
            }

        }
    }
}
