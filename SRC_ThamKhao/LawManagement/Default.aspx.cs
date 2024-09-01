using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MOABSearch.Common;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Session.Abandon();
    }

    protected void ShowMessage(String content)
    {
        ScriptManager.RegisterStartupScript(this, this.GetType(), Guid.NewGuid().ToString(), content, true);
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid)
            return;

        try
        {
            BaseUser u = BaseUser.GetUser(txtUserName.Text.Trim(), txtPassword.Text.Trim(), UserType.Normal);
            String message = String.Empty;
            if (u == null)
            {
                message = String.Format("ShowMessageBox('{0}','Law Search - Warning',null);", "Username or password is invalid.");
                ShowMessage(message);
                return;
            }
            else
            {
                if (u.Closed)
                {
                    message = String.Format("ShowMessageBox('{0}','Law Search - Warning',null);", "The user is closed.");
                    ShowMessage(message);
                    return;
                }
                else
                {
                  

                    u.ProviderID = "0";

                    Session["UserLogin"] = u;
                    Session["UserID"] = u.ID;
                }
            }
        }
        catch (System.Exception ex)
        {
            Globals.WriteLog("Login.log", ex.Message);
            ShowMessage(String.Format("ShowMessageBox('{0}','Law Search - Warning',null);", "Username or password is invalid."));
        }
        Response.Redirect("index.aspx");
    }
}