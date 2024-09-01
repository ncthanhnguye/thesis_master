using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace BDSLaw
{
    public class LuatProvider
    {
        public LuatProvider()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public DataSet ExecuteDataSet(string v)
        {
            try
            {
                Globals.WriteLog("SQL = " + v);
                DataSet ds = new DataSet();
                using (SqlConnection cn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["CSConnString"].ConnectionString))
                {
                    cn.Open();
                    SqlDataAdapter dap = new SqlDataAdapter(v, cn);
                    dap.Fill(ds);
                    cn.Close();
                }
                return ds;
            }
            catch (Exception ex)
            {

                Globals.WriteLog(ex);
            }
            return null;
        }

      

        public DataTable LoadLaw()
        {
            DataSet ds = ExecuteDataSet("select * from Law");
            return ds.Tables[0];
        }



        public DataTable LoadChuong()
        {
            DataSet ds = ExecuteDataSet("select * from Chapter order by number ");
            return ds.Tables[0];
        }

        public DataTable LoadDieu(string selectedValue)
        {
            DataSet ds = ExecuteDataSet("select * from Artical where ChapterID = " + selectedValue + " order by number");
            return ds.Tables[0];
        }

        public DataSet GenerateKeyPhrase(string key, int CID = 0)
        {
            return ExecuteDataSet("exec GetKeyPhrase N'" + key + "',"+CID);
        }
    }
}