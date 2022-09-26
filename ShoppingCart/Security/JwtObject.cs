using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCart.Security
{
    public class JwtObject
    {
        //內容隨意設計，但不要放太重要的資料，且依定要記得設定到時期間

        public string Account { get; set; }

        public string Role { get; set; }
        //到期時間

        public string Expire { get; set; }
    }
}