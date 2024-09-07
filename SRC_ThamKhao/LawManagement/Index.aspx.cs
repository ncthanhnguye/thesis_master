using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MOABSearch.BLL;
using System.Data;
using MOABSearch.Common;

using System.Web.Script.Serialization;

public partial class Index : System.Web.UI.Page
{
    public CurrentAppConfig app
    {
        get
        {
            if (Application[Globals.sAppConfig] == null)
                Application[Globals.sAppConfig] = new CurrentAppConfig();
            return (CurrentAppConfig)Application[Globals.sAppConfig];
        }
        set
        {
            Application[Globals.sAppConfig] = value;
        }
    }
    public ReportInfo rp
    {
        get
        {
            object o = Session["MOABReportInfo"];
            if (o != null)
                return (ReportInfo)o;
            return null;
        }
        set
        {
            Session["MOABReportInfo"] = value;
        }
    }
    public MOABSearchInfo searchInfo
    {
        get
        {
            object o = Session["MOABSearchInfo"];
            if (o != null)
                return (MOABSearchInfo)o;
            return null;
        }
        set
        {
            Session["MOABSearchInfo"] = value;
        }
    }
    protected void Page_Init(object sender, EventArgs e)
    {
        Globals.RegisterFilesOnHeader(Page, new String[] { "NewLanding" }, false, "");// CSS
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        Globals.ValidateSession();
        if (!IsPostBack)
        {
           
          
        }

    }


}