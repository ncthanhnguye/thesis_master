using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MOABSearch.Common;
using MOABSearch.BLL;

public partial class MasterPageLEARNIX : System.Web.UI.MasterPage
{

    protected void Page_Init(object sender, EventArgs e)
    {
        Globals.RegisterFilesOnHeader(Page, new String[] { "Site", "MOABSearch", "master" }, false, "");// CSS
        Globals.RegisterFilesOnHeader(Page, new String[] { "Site", "DragPopup", "GTCWindow", "ButtonText"}, true, "");// JS
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["keepalive"] != null)
            return;
        if (!Page.IsPostBack)
        {
            Page.Title = Resources.Resource.MasterHeader;
           BaseUser u = Session["UserLogin"] as BaseUser;
            if (u != null)
                lblUserLogin.Text = u.UserName;
           


        }
        
    }
  
   

    protected void btnHiddenBack_Click(object sender, EventArgs e)
    {
        if (Session[Globals.g_UserLogin] != null)
            RedirectPageToLearn("BackLearnNewLanding");
        else btnHiddenLogout_Click(sender, e);
    }
    protected void btnHiddenHome_Click(object sender, EventArgs e)
    {
        if (Session[Globals.g_UserLogin] != null)
            RedirectPageToLearn("LearnNewLanding");
        else btnHiddenLogout_Click(sender, e);
    }
    protected void btnHiddenLogout_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx"); return;
    }
    private void RedirectPageToLearn(string targetWeb)
    {       
        try
        {
           
            if (targetWeb == "LearnNewLanding" || targetWeb == "LogOutLearn" || targetWeb == "BackLearnNewLanding")
            {
                //logout
                NormalUser curUser = HttpContext.Current.Session[Globals.g_UserLogin] as NormalUser;
                if (targetWeb == "LogOutLearn" || curUser == null)
                { Response.Redirect( "Default.aspx"); return; }
                // Back
                if (targetWeb == "BackLearnNewLanding")
                {
                    Response.Redirect("index.aspx");
                }
                //Home Click
                else if (targetWeb == "LearnNewLanding")
                {
                    Response.Redirect("index.aspx");
                }
            }
        }
        catch (Exception ex)
        {
            Globals.WriteLog("MOABError.log", "RedirectPageToLearn: " + targetWeb + ";" + ex.ToString());
        }
    }

   
   
}
