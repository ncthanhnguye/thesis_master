using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



public partial class MasterPageLEARN : System.Web.UI.MasterPage
{

    protected void Page_Init(object sender, EventArgs e)
    {

       MOABSearch.Common. Globals.RegisterFilesOnHeader(Page, new String[] { "Site", "MOABSearch", "datepicker", "firmstrip", "master" }, false, "");// CSS
        MOABSearch.Common.Globals.RegisterFilesOnHeader(Page, new String[] { "Site", "DragPopup", "GTCWindow", "ButtonText", "bootstrap-datepicker", "AuditPopup" }, true, "");// JS
    }
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {
            Page.Title = Resources.Resource.MasterHeader;
        }
        
    }

   

    protected void btnHiddenBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("index.aspx");
    }
    protected void btnHiddenHome_Click(object sender, EventArgs e)
    {
        Response.Redirect("index.aspx");
    }
    protected void btnHiddenLogout_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }
  
}
