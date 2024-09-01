using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Web.Caching;
using System.IO;
/// <summary>
/// Summary description for DataAccess
/// </summary>
/// 
namespace MOABSearch.Common
{
    public abstract class DataAccess
    {
        public DataAccess()
        {
            m_ConnectionString = ConfigurationManager.ConnectionStrings["CSConnString"].ConnectionString;
            if (ConfigurationManager.ConnectionStrings["CSConnString_MOAB"] != null)
                m_MOABDBConnectionString = ConfigurationManager.ConnectionStrings["CSConnString_MOAB"].ConnectionString;
            m_Timeout = Globals.g_CommandTimeOut;

        }
        private string m_ConnectionString = "";
        private string m_MOABDBConnectionString = "";
        //private string m_MOABConnectionString = "";
        protected string ConnectionString
        {
            get { return m_ConnectionString; }
            set { m_ConnectionString = value; }
        }

        private int m_Timeout;
        protected int TimeOut
        {
            get { return m_Timeout; }
        }
        private void CheckDBConnectFailed(Exception ex)
        {
            try
            {
                if (ex != null && (ex.Message.Contains("Could not open a connection")
                    || ex.Message.Contains("Cannot open database")
                    || ex.Message.Contains("permission was denied")
                    || ex.Message.Contains("Login failed")
                    || ex.Message.Contains("Thread was being aborted")))
                {
                    try
                    {
                        if (HttpContext.Current != null && HttpContext.Current.Response != null)
                        {
                            string p = Globals.ContactPage;
                            if (!string.IsNullOrEmpty(p))
                                HttpContext.Current.Response.Redirect(p);
                        }
                    }
                    catch (Exception ex1){ }
                }
            }
            catch { }
        }
        public int ExecuteNonQuery(SqlCommand cmd)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(m_ConnectionString))
                {
                    cn.Open();
                    cmd.Connection = cn;
                    cmd.CommandTimeout = TimeOut;
                    return cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                CheckDBConnectFailed(ex);
                Globals.WriteLog("ErrorSQL.log", DateTime.UtcNow.ToString() + " - " + cmd.CommandText + ": " + ex.Message);
                return -1;
            }
        }

        public DataSet ExecuteDataset(string strSQL)
        {
            SqlConnection cn = null;
            try
            {
                if (!string.IsNullOrEmpty(strSQL))
                {
                    using (cn = new SqlConnection(m_ConnectionString))
                    {
                        cn.Open();
                        SqlCommand cmd = new SqlCommand(strSQL, cn);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = Globals.g_CommandTimeOut;
                        SqlDataAdapter adap = new SqlDataAdapter(cmd);
                        DataSet dst = new DataSet();
                        adap.Fill(dst);
                        return dst;
                    }
                }
                else
                {
                    Globals.WriteLog("ErrorSQL.log", DateTime.UtcNow.ToString() + " - SQL is Empty" + "\r\n");
                    return null;
                }

            }
            catch (System.Exception ex)
            {
                CheckDBConnectFailed(ex);
                Globals.WriteLog("ErrorSQL.log", DateTime.UtcNow.ToString() + " - " + strSQL + ": " + ex.Message);
                return null;
            }
        }
        public DataSet ExecuteDatasetMOAB(string strSQL)
        {
            SqlConnection cn = null;
            try
            {
                if (!string.IsNullOrEmpty(strSQL))
                {
                    using (cn = new SqlConnection(m_MOABDBConnectionString))
                    {
                        cn.Open();
                        SqlCommand cmd = new SqlCommand(strSQL, cn);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = Globals.g_CommandTimeOut;
                        SqlDataAdapter adap = new SqlDataAdapter(cmd);
                        DataSet dst = new DataSet();
                        adap.Fill(dst);
                        return dst;
                    }
                }
                else
                {
                    Globals.WriteLog("ErrorSQL.log", DateTime.UtcNow.ToString() + " - SQL is Empty" + "\r\n");
                    return null;
                }

            }
            catch (System.Exception ex)
            {
                CheckDBConnectFailed(ex);
                Globals.WriteLog("ErrorSQL.log", DateTime.UtcNow.ToString() + " - " + strSQL + ": " + ex.Message);
                return null;
            }
        }
        public DataSet ExecuteDataset(string[] strSQL)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(m_ConnectionString))
                {
                    cn.Open();
                    DataSet dst = new DataSet();
                    foreach (string sql in strSQL)
                    {
                        SqlCommand cmd = new SqlCommand(sql, cn);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = Globals.g_CommandTimeOut;
                        SqlDataAdapter adap = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adap.Fill(dt);
                        dst.Tables.Add(dt);
                    }
                    cn.Close();
                    return dst;
                }
            }
            catch (System.Exception ex)
            {
                CheckDBConnectFailed(ex);
                Globals.WriteLog("ErrorSQL.log", DateTime.UtcNow.ToString() + " - " + strSQL + ": " + ex.Message);
                return null;
            }
        }

        public DataSet ExecuteDataset_Store(string strSQL, SqlParameter[] sqlParams)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(m_ConnectionString))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand(strSQL, cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = TimeOut;

                    foreach (SqlParameter param in sqlParams)
                        cmd.Parameters.Add(param);

                    SqlDataAdapter adap = new SqlDataAdapter(cmd);
                    DataSet dst = new DataSet();
                    adap.Fill(dst);

                    cmd.Parameters.Clear();
                    cmd.Dispose();
                    return dst;
                }
            }
            catch (Exception ex)
            {
                CheckDBConnectFailed(ex);
                Globals.WriteLog("ErrorSQL.log", DateTime.UtcNow.ToString() + " - " + strSQL + ": " + ex.Message);
            }
            return null;

        }

        public bool ExecuteNonQuery(string strSQL)
        {
            try
            {
                if (!string.IsNullOrEmpty(strSQL))
                {
                    using (SqlConnection cn = new SqlConnection(m_ConnectionString))
                    {
                        cn.Open();
                        SqlCommand cmd = new SqlCommand(strSQL, cn);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = TimeOut;
                        return cmd.ExecuteNonQuery() >= 0;
                    }
                }
                else
                {
                    Globals.WriteLog("ErrorSQL.log", DateTime.UtcNow.ToString() + " - Sql is Empty" + "\r\n");
                    return false;
                }

            }
            catch (Exception ex)
            {
                CheckDBConnectFailed(ex);
                Globals.WriteLog("ErrorSQL.log", DateTime.UtcNow.ToString() + " - " + strSQL + ": " + ex.Message);
                return false;
            }
        }
        public bool ExecuteNonQuery(string strSQL, SqlParameter[] sqlParams)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(m_ConnectionString))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand(strSQL, cn);
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = TimeOut;
                    foreach (SqlParameter param in sqlParams)
                        cmd.Parameters.Add(param);

                    int obj = cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    cmd.Dispose();

                    return obj > 0;
                }
            }
            catch (Exception ex)
            {
                CheckDBConnectFailed(ex);
                Globals.WriteLog("ErrorSQL.log", DateTime.UtcNow.ToString() + " - " + strSQL + ": " + ex.Message);
                return false;
            }
        }

        public SqlDataReader ExecuteReader(string strSQL)
        {
            if (strSQL != "")
            {
                SqlConnection cn = null;
                try
                {
                    SqlDataReader reader = null;
                    cn = new SqlConnection(m_ConnectionString);
                    cn.Open();
                    SqlCommand cmd = new SqlCommand(strSQL, cn);
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = TimeOut;


                    reader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);

                    return reader;

                }
                catch (System.Exception ex)
                {

                    try
                    {
                        if (cn != null && cn.State != ConnectionState.Closed)
                            cn.Close();
                    }
                    catch { }
                    CheckDBConnectFailed(ex);
                    Globals.WriteLog("ErrorSQL.log", DateTime.UtcNow.ToString() + " - " + strSQL + ": " + ex.Message);
                    return null;
                }
            }
            return null;
        }

        public int ExecuteScalar(string strSQL, SqlParameter[] sqlParams)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(m_ConnectionString))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand(strSQL, cn);
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = TimeOut;
                    foreach (SqlParameter param in sqlParams)
                        cmd.Parameters.Add(param);

                    object obj = cmd.ExecuteScalar();

                    cmd.Parameters.Clear();
                    cmd.Dispose();

                    if (obj == null)
                        return 1;

                    return int.Parse(obj.ToString());
                }
            }
            catch (Exception ex)
            {
                CheckDBConnectFailed(ex);
                Globals.WriteLog("ErrorSQL.log", DateTime.UtcNow.ToString() + " - " + strSQL + ": " + ex.Message);
                return -1;
            }
        }
        public object ExecuteScalar(string strSQL)
        {
            try
            {
                if (!string.IsNullOrEmpty(strSQL))
                {
                    using (SqlConnection cn = new SqlConnection(m_ConnectionString))
                    {
                        cn.Open();
                        SqlCommand cmd = new SqlCommand(strSQL, cn);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = TimeOut;
                        return cmd.ExecuteScalar();
                    }
                }
                else
                {
                    Globals.WriteLog("ErrorSQL.log", DateTime.UtcNow.ToString() + " - SQL is Empty" + "\r\n");
                    return null;
                }

            }
            catch (System.Exception ex)
            {
                CheckDBConnectFailed(ex);
                Globals.WriteLog("ErrorSQL.log", DateTime.UtcNow.ToString() + " - " + strSQL + ": " + ex.Message);
                return null;
            }
        }

        public object ExecuteScalar_Store(string strSQL, SqlParameter[] cmdParams)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(m_ConnectionString))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand(strSQL, cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = TimeOut;

                    for (int i = 0; i < cmdParams.Length; i++)
                        cmd.Parameters.Add(cmdParams[i]);

                    object obj = cmd.ExecuteScalar();

                    cmd.Parameters.Clear();
                    cmd.Dispose();
                    return obj;
                }
            }
            catch (System.Exception ex)
            {
                CheckDBConnectFailed(ex);
                Globals.WriteLog("ErrorSQL.log", DateTime.UtcNow.ToString() + " - " + strSQL + ": " + ex.Message);
                return null;
            }
        }

        public bool ExecuteProcedure(string procName, SqlParameter[] cmdParams)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(m_ConnectionString))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand(procName, cn);
                    cmd.CommandTimeout = TimeOut;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter myParm = cmd.Parameters.Add("@RetValue", SqlDbType.Int);
                    myParm.Direction = ParameterDirection.ReturnValue;
                    for (int i = 0; i < cmdParams.Length; i++)
                        cmd.Parameters.Add(cmdParams[i]);

                    int obj = cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    cmd.Dispose();

                    return obj > 0;
                }
            }
            catch (Exception ex)
            {
                CheckDBConnectFailed(ex);
                Globals.WriteLog("ErrorSQL.log", DateTime.UtcNow.ToString() + " - Procedure: " + ex.Message);
                return false;
            }
        }
    }
}
