using BDSLaw;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

public partial class Transactions : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
       //abc();
        if (!IsPostBack)
        {
            
        }
    }

 

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        int itime = int.Parse(cboTime.SelectedValue);

        DateTime[] arr = new DateTime[] {
            DateTime.Now .AddHours(-1),
             DateTime.Now .AddHours(-12),
              DateTime.Now .AddDays(-1),
              DateTime.Now .AddDays(-7),
            DateTime.Now .AddDays(-21),
             new DateTime( DateTime.Now.Year,DateTime.Now.Month,1).AddMonths(-1),
              new DateTime( DateTime.Now.Year,DateTime.Now.Month,1).AddMonths(-3),
                new DateTime( DateTime.Now.Year,1,1) .AddYears(-1),
                 new DateTime(2000,1,1)
        };
        DateTime dtStart = arr[itime - 1];

        DataSet ds = Globals.GetDataset("exec GetQueryByDate '" +  dtStart.ToString() + "'");
      
        if (ds != null)
        {
            for (int i = 0; i < Globals.DSCount(ds); i++)
            {
                ds.Tables[0].Rows[i]["Number"] = i + 1;
            }
            ds.Tables[0].AcceptChanges();
            GridView1.DataSource = ds.Tables[0];
            GridView1.DataBind();
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "btnSearchTrans_Click", "TranRegister();", true);
        }

    }

    protected void btnResearch_Click(object sender, EventArgs e)
    {
        HttpContext.Current.Session["ResearchByLog"] = hdftext.Value;
        Response.Redirect("SearchLaw.aspx?r=1");
    }
}