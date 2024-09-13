using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MOABSearch.Common;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace MOABSearch.BLL
{
    public class UserProvider : MOABSearch.Common.DataAccess
    {
        public UserProvider() { }
        public UserDetails GetUserDetails(string strSQl)
        {
            UserDetails details = null;
            try
            {
                SqlDataReader reader = ExecuteReader(strSQl);

                if (reader != null)
                {
                    if (reader.Read())
                        details = GetUserDetailsFromReader(reader);

                    reader.Close();
                }
            }
            catch (SqlException ex)
            {
                Globals.WriteLog("Login.log", ex.Message);
            }

            return details;
        }
        public UserDetails GetUserDetails(string UserName, string RawPass, int type)
        {
            string strSQl = string.Format("select top 1 * from [user] WITH (NOLOCK) where username='{0}' and password='{1}' and Type={2}"
               , UserName, RawPass, type);
            return GetUserDetails(strSQl);
          
        }

        public UserDetails GetUserDetailsFromReader(SqlDataReader reader)
        {
            UserDetails details = new UserDetails();

            try
            {
                details.ID = reader["ID"].ToString();
          
                details.Name = Globals.ConvertString(reader["Name"]);
                details.UserName = Globals.ConvertString(reader["UserName"]);
                details.Description = Globals.ConvertString(reader["Description"]);
            
                details.Email = Globals.ConvertString(reader["Email"]);
            

                if (Globals.ConvertString(reader["Type"]) != "")
                    details.Type = int.Parse(Globals.ConvertString(reader["Type"]));
              

            }
            catch (Exception ex)
            {
                Globals.WriteLog("Login.log", ex.Message);

                return null;
            }

            return details;
        }

        public bool IsClosedUser(string username, string type, ref string sProviderid)
        {
            sProviderid = "";
            try
            {

                string strSQL = "select top 1 closed,ID from [user] WITH (NOLOCK) where username='" + username + "' and type=" + type;
                SqlDataReader reader = ExecuteReader(strSQL);
                bool bClosed = true;
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            try
                            {
                                bClosed = bool.Parse(reader["closed"].ToString());
                                sProviderid = Globals.ConvertString(reader["ID"]);
                            }
                            catch { }

                        }
                    }

                    reader.Close();
                    return bClosed;
                }
            }
            catch
            {
            }

            return true;
        }

        public long GetUserRight(String userId)
        {
            long p = 0;
            try
            {

                String strSQL = String.Format("select top 1 Privilege from [user] WITH (NOLOCK) where ID = {0}", userId);
                p = (long)ExecuteScalar(strSQL);
            }
            catch
            {
            }

            return p;
        }



        // lay danh sach userid cua nhung agency co quyen shared NVLS data

       

        public UserDetails GetUserDetailsNoPass(string UserName, int type)
        {
            UserDetails details = null;
            string strSQl = string.Format("select top 1 * from [user] WITH (NOLOCK) where UserName='{0}' and type={1}"
                , UserName, type);
            try
            {
                SqlDataReader reader = ExecuteReader(strSQl);

                if (reader != null)
                {
                    if (reader.Read())
                        details = GetUserDetailsFromReader(reader);

                    reader.Close();
                }
            }
            catch (SqlException ex)
            {
                Globals.WriteLog("Login.log", ex.Message);
            }

            return details;
        }
        public string GetlinkForRedirect(string namePage)
        {
            try
            {
                string strDSL = "_DSL";
                string strSQL = "select value from setting where name='" + namePage + strDSL + "'";
                object obj = ExecuteScalar(strSQL);
                if (obj != null)
                    return obj.ToString();
            }
            catch { }

            return "";
        }

        internal long GetPrivilege(string strUser, int Type)
        {
            try
            {
                string strSQL = "select Privilege from [user] WITH (NOLOCK) where username ='" + strUser + "' and Type=" + Type.ToString();
                object o = ExecuteScalar(strSQL);
                if (o == null)
                    return 0;
                long n = long.Parse(Globals.ConvertString(o));
                return n;
            }
            catch
            {
                return 0;
            }
        }


    }
}