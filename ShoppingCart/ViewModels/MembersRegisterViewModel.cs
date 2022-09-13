using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShoppingCart.ViewModels
{
    public class MembersRegisterViewModel
    {
        public Members NewMember { get; set; }

        [DisplayName("密碼")]
        [Required(ErrorMessage ="請輸入密碼")]
        public string Password { get; set; }


        [DisplayName("確認密碼")]
        [Compare("Password",ErrorMessage ="兩次輸入的密碼不一樣")]
        [Required(ErrorMessage ="請輸入確認密碼")]
        public string PasswordCheck { get; set; }
    }
}