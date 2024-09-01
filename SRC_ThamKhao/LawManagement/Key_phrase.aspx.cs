using BDSLaw;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

public partial class Key_phrase : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
       //abc();
        if (!IsPostBack)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "btnView_Click", "ViewAllKeyphrases();", true);
        }
    }
    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static string Update_KeyPhrase(int type, string key)
    {
        try
        {
            SearchProvider pd = new SearchProvider();
            if (type == 0)
            {
                pd.GenerateKeyPhrase(key);
            }
            else
                pd.ExecuteDataSet("exec DeleteKeyPhrase N'" + key + "'");
            return "";
        }
        catch (Exception ex)
        {
            Globals.WriteLog(ex);
            return null;
        }
    }


    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static List<KeyPhrase> GetAllKeyphrases_DB()
    {
        try
        {
            SearchProvider pd = new SearchProvider();
            List<KeyPhrase> lst = new List<KeyPhrase>();
           DataSet ds = pd.ExecuteDataSet("select * from [KeyPhrase] with(nolock) order by KeyPhrase");
            for (int i = 0; i < Globals.DSCount(ds); i++)
            {
                lst.Add(new KeyPhrase {  ID = Globals.GetIDinDS(ds,i,"ID"), Key = Globals.GetinDS_String(ds, i, "KeyPhrase") });
            }
            return lst;


        }
        catch (Exception ex)
        {
            Globals.WriteLog(ex);
            return null;
        }
    }

    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static List<KeyPhraseRelateDetails> ViewKeyPhraseContent_Detail(int keyphraseID)
    {
        try
        {
            List<KeyPhraseRelateDetails> lst = new List<KeyPhraseRelateDetails>();
           SearchProvider pd = new SearchProvider();
       
            HttpContext.Current.Session["LastkeyphraseID"] = keyphraseID;
            DataSet ds = pd.ExecuteDataSet("exec GetKeyPhraseRelateDetailsByID " + keyphraseID);
            for (int i = 0; i < Globals.DSCount(ds); i++)
            {
                lst.Add(new KeyPhraseRelateDetails { ID = keyphraseID, KeyPhrase = Globals.GetinDS_String(ds,i, "KeyPhrase"),
                    ArticalName = Globals.GetinDS_String(ds, i, "ArticalName"),
                    ChapterName = Globals.GetinDS_String(ds, i, "ChapterName"),
                    NumCount = Globals.GetIDinDS(ds, i, "NumCount"),
                });
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