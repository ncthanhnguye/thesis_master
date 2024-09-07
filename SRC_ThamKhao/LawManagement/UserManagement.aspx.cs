using BDSLaw;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

public partial class UserManagement : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
       //abc();
        if (!IsPostBack)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "btnView_Click", "ViewAllUsers();", true);
        }
    }
    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static string Update_User(int type, string username, string pass)
    {
        try
        {
            SearchProvider pd = new SearchProvider();
            if (type == 0)
            {              
                pd.ExecuteDataSet("exec Update_User 0,'" + username + "',''");
            }
            else if(type == 1)
                pd.ExecuteDataSet("exec Update_User 1,'" + username + "',''");
            else
                pd.ExecuteDataSet("exec Update_User 2,'" + username + "','"+Globals.GetSHA512(pass)+"'");
            return "";
        }
        catch (Exception ex)
        {
            Globals.WriteLog(ex);
            return null;
        }
    }


    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static List<User> GetAllUsers_DB()
    {
        try
        {
            SearchProvider pd = new SearchProvider();
            List<User> lst = new List<User>();
           DataSet ds = pd.ExecuteDataSet("select * from [User] with(nolock) order by username");
            for (int i = 0; i < Globals.DSCount(ds); i++)
            {
                lst.Add(new User {  ID = Globals.GetIDinDS(ds,i,"ID"), username = Globals.GetinDS_String(ds, i, "username") });
            }
            return lst;


        }
        catch (Exception ex)
        {
            Globals.WriteLog(ex);
            return null;
        }
    }

  


   
}