using BDSLaw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ImportLaw : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnImport_Click(object sender, EventArgs e)
    {

        LawDoc l = new LawDoc();
        l.ParseContent(true, txtContent.Text, txtName.Text);
        //if(txtContent_HTML.Text.Trim()!="")
        //{
        //    LuatProvider pd = new LuatProvider();
        //    string sql = string.Format("Insert into [LawHTML](LawID, ContentHTML) values ({0},N'{1}')", l.ID, txtContent_HTML.Text.Trim());
        //    pd.ExecuteDataSet(sql);
        //}
        Response.Redirect("EditLaw.aspx?ID=" + l.ID);
    }
}