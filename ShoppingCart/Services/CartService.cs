using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ShoppingCart.Services
{
    public class CartService
    {
        //建立與資料庫連結字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP.NET MVC"].ConnectionString;

        //建立與資料庫的連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 取得購物車內商品陣列
        public List<CartBuy> GetItemFromCart(string Cart)
        {
            //宣告要回傳的搜尋資料為資料庫中的CartBuy表單
            List<CartBuy> DataList = new List<CartBuy>();
            //sql語法 CartBuy
            //根據購物車編號取得以放入購物車的商品陣列
            string sql = $@" SELECT * FROM CartBuy m INNER JOIN Item d ON m.Item_id = d.Id WHERE Cart_Id = '{Cart}'; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得SQL資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())//獲得下一筆資料直到沒資料
                {
                    CartBuy Data = new CartBuy();
                    Data.Cart_Id = dr["Cart_Id"].ToString();
                    Data.Item_Id = Convert.ToInt32(dr["Item_Id"]);
                    Data.Item.Id = Convert.ToInt32(dr["Id"]);
                    Data.Item.Image = dr["Image"].ToString();
                    Data.Item.Name = dr["Name"].ToString();
                    Data.Item.Price = Convert.ToInt32(dr["Price"]);
                    DataList.Add(Data);

                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return DataList;
        }

        #endregion

        #region 確認商品是否於購物車中
        //確認商品是否於購物車中的方法
        public bool CheckInCart(string Cart, int Item_Id)
        {
            //宣告要回傳的收尋資料為資料庫中的CartBuy資料
            CartBuy Data = new CartBuy();
            //Sql語法CartBuy
            //根據購物車與商品編號取得CartBut 資料表內資料
            string sql = $@" SELECT * FROM CartBuy m INNER JOIN Item d ON m.Item_Id = d.Id WHERE Cart_Id = '{Cart}' AND Item_Id = {Item_Id}; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Cart_Id = dr["Cart_Id"].ToString();
                Data.Item_Id = Convert.ToInt32(dr["Item_Id"]);
                Data.Item.Id = Convert.ToInt32(dr["Id"]);
                Data.Item.Image = dr["Image"].ToString();
                Data.Item.Name = dr["Name"].ToString();
                Data.Item.Price = Convert.ToInt32(dr["Price"]);
            }
            catch (Exception e)
            {
                //沒有資料傳回null
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            //判斷是否有資料，以確認是否於購物車中
            return (Data != null);
        }

        #endregion

        #region 放入購物車
        //放入購物車方法
        public void AddtoCart(string Cart, int Item_Id) 
        {
            //sql新增語法，新增CartBuy
            string sql = $@" INSERT INTO CartBuy(Cart_Id,Item_Id) VALUES('{Cart}','{Item_Id}'); ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        
        }

        #endregion

        #region 取出購物車
        //將商品從購物車中取出的方法
        public void RemoveForCart(string Cart, int Item_Id)
        {
            //sql刪除語法CartBuy
            //根據購物車與商品編號取得要刪除的資料
            string sql = $@" DELETE FROM CartBuy WHERE Cart_Id = '{Cart}' AND Item_Id = '{Item_Id}'; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 確認購物車是否有保存
        //確認商品是否於購物車中方法
        public bool CheckCartService(string Account,string Cart)
        {
            //根據會員帳號與購物車編號取得CartSave資料表內資料
            CartSave Data = new CartSave();
            //Sql語法 CartSave
            string sql = $@" SELECT * FROM CartSave m INNER JOIN Members d ON m.Account = d.Account WHERE m.Account = '{Account}' AND Cart_Id = '{Cart}'; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();//獲得下一筆資料直到沒有資料
                Data.Account = dr["Account"].ToString();
                Data.Cart_Id = dr["Cart_Id"].ToString();
                Data.Members.Name = dr["Name"].ToString();            
            }
            catch (Exception e)
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            return (Data != null);
        }

        #endregion

        #region 取得購物車保存方法
        public string GetCartSave(string Account)
        {
            //根據會員資料取得CartSave資料表內資校
            CartSave Data = new CartSave();
            string sql = $@" SELECT * FROM CartSave m INNER JOIN Members d ON m.Account = d.Account WHERE m.Account = '{Account}'; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Account = dr["Account"].ToString();
                Data.Cart_Id = dr["Cart_Id"].ToString();
                Data.Members.Name = dr["Name"].ToString();
            }
            catch
            {
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            //判斷是否有資料，以確認是否有在購物車中
            if (Data!= null)
            {
                return Data.Cart_Id;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region 保存購物車
        public void SaveCart(string Account, string Cart)
        {
            //宣告一筆新的CartSave資料表資料物件
            //sql新增語法 CartSave
            string sql = $@" INSERT INTO CartSave(Account, Cart_Id) VALUES('{Account}','{Cart}');";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

            }
            catch (Exception e) 
            {
                throw new Exception(e.Message.ToString());            
            }
            finally
            {
                conn.Close();
            }
        }

        #endregion

        #region 取消保存購物車
        public void SaveCartRemove(string Account)
        {
            string sql = $@" DELETE FROM CartSave WHERE Account = '{Account}'; ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }        
        }

        #endregion
    }
}