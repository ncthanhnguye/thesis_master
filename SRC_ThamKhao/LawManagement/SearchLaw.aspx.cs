using BDSLaw;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SearchLaw : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string research = HttpContext.Current.Session["ResearchByLog"] as string;
        if (!string.IsNullOrEmpty(research)) {
            HttpContext.Current.Session["ResearchByLog"] = null;
            txtSearch.Text = research;
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "btnSearchTrans_Click_Perform", " SearchLaw();", true);
           
        }
    }

    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static SearchArticalResult SearchLawByText(string searchInput)
    {
        SearchProvider pd = new SearchProvider();
        DataSet ds = Globals.GetDataset("exec QueryLog N'"+searchInput.Replace("'","''")+"',"+Globals.GetUerIDLogin());
        int queryId = Globals.GetIDinDS(ds,0,0);
        SearchArticalResult rs = pd.SearchArtical(searchInput);
        foreach (var artical in rs.lstArticals)
        {
            DataSet dsDetail = pd.ExecuteDataSet("exec GetArticalDetail2 " + artical.ID);
            artical.Title = Globals.GetinDS_String(dsDetail, 0, "ChapterName") + " - " + Globals.GetinDS_String(dsDetail, 0, "Name");
            artical.Content = Globals.GetinDS_String(dsDetail, 0, "Title");
        }
        return rs;
    }

    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static List<LawDoc> ViewDetailArticalByID_Result(List<int> lstArticalID )
    {
        try
        {
            SearchProvider pd = new SearchProvider();
            return pd.ViewArticalDetails(lstArticalID);
            
        }
        catch (Exception ex)
        {
            Globals.WriteLog(ex);
            return null;
        }
   
    }
}