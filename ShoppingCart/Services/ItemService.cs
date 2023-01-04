using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace ShoppingCart.Services
{
    public class ItemService
    {
        //建立與資料庫的連線字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP.NET MVC"].ConnectionString;

        //建立資料庫的連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 取得單一商品資料
        //藉由取得的單筆商品資料的方法
        public Item GetDataById(int Id)
        {
            //回傳根據編號所取得的資料
            Item Data = new Item();
            //sql語法Item
            string sql = $@" SELECT * FROM Item WHERE Id ={Id};";
            try
            {
                conn.Open();
                //執行sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Data資料
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();//獲得下一筆資料直到沒有資料
                Data.Id = Convert.ToInt32(dr["Id"]);
                Data.Image = dr["Image"].ToString();
                Data.Name = dr["Name"].ToString();
                Data.Price = Convert.ToInt32(dr["Price"]);
            }
            catch (Exception e)
            {
                //沒有資料回傳null
                Data = null;
            }
            finally
            {
                conn.Close();
            }
            return Data;
        }

        #endregion

        #region 取得商品編號陣列
        public List<int> GetIdList(ForPaging Paging)
        {
            //計算所需的總頁面
            SetMaxPaging(Paging);
            //取得資料庫中的Item資料表
            List<int> IdList = new List<int>();
            //sql語法 //desc為反向排序 越晚新增的越前面
            string sql = $@" SELECT Id FROM (SELECT row_number() OVER(order by Id desc) AS sort,* FROM Item) m WHERE m.sort BETWEEN {(Paging.NowPage - 1) * Paging.ItemNum + 1} AND {Paging.NowPage * Paging.ItemNum};";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql,conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) //獲得下一筆資料直到沒有資料
                {
                    IdList.Add(Convert.ToInt32(dr["Id"]));
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
            return IdList;
        }


        #endregion

        #region 設定最大頁數方法
        public void SetMaxPaging(ForPaging Paging)
        {
            //計算列數
            int Row = 0;
            string sql = $@" SELECT * FROM Item;";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())//獲得下一筆資料直到沒有資料
                {
                    Row++;
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
            //計算所需的總頁面數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            //重新設定正確的頁數，避免有不正確的直傳入
            Paging.SetRightPage();
        }

        #endregion

        #region 新增商品
        //新增商品方法
        public void Insert(Item newData)
        {
            //sql新增語法Item
            string sql = $@" INSERT INTO Item(Name,Price,Image) VALUES ('{newData.Name}','{newData.Price}','{newData.Image}') ";
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

        #region 刪除商品
        public void Delete(int Id)
        {
            //SQL語法CartBuy
            //根據商品編號刪除資料
            //先將CartBuy的資料刪除才刪Item
            //使用StringBuilder方法依次建立SQL使用
            StringBuilder sql = new StringBuilder();
            sql.AppendLine($@" DELETE FROM CartBuy WHERE Item_Id = {Id}; ");
            sql.AppendLine($@" DELETE FROM Item WHERE Id = {Id}; ");
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
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