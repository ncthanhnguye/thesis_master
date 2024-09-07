using BDSLaw;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

public partial class Concept : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
       //abc();
        if (!IsPostBack)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "btnView_Click", "ViewConcept();", true);
        }
    }
    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static string KeyPhrase_Changed(KeyPhraseChanged keyPhraseChanged)
    {
        try
        {
            SearchProvider pd = new SearchProvider();
            int conceptID = (int)HttpContext.Current.Session["LastKeyPhrase_conceptID"];
            foreach (var item in keyPhraseChanged.lstAdd)
            {
                pd.GenerateKeyPhrase(item, conceptID);
            }


            pd.DeletekeyphraseMapping(conceptID, keyPhraseChanged.lstDelete);
            return "";
        }
        catch (Exception ex)
        {
            Globals.WriteLog(ex);
            return null;
        }
    }
    //ChangeConcept
    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static string ChangeConcept(ConceptInfo concept)
    {
        try
        {
            SearchProvider pd = new SearchProvider();
            concept.ChangeConcept();
            return "";

        }
        catch (Exception ex)
        {
            Globals.WriteLog(ex);
            return null;
        }
    }
    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static List<ConceptInfo> ViewConcept()
    {
        try
        {
            SearchProvider pd = new SearchProvider();          
            return ConceptInfo.GetListConcept();


        }
        catch (Exception ex)
        {
            Globals.WriteLog(ex);
            return null;
        }
    }

    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static KeyPhraseConceptResult ViewDetailConcept_KeyPhrase(int conceptID)
    {
        try
        {
            SearchProvider pd = new SearchProvider();
       
            HttpContext.Current.Session["LastKeyPhrase_conceptID"] = conceptID;
            return pd.GetKeyPhraseLawResult(conceptID);

        }
        catch (Exception ex)
        {
            Globals.WriteLog(ex);
            return null;
        }
    }



    public void abc() {

        int ID = 149;
        SearchProvider pd1 = new SearchProvider();
        LawDoc law = null; // pd.ViewArticalDetail(new List<int>() { ID });
        LuatProvider pd = new LuatProvider();
  
        //foreach (Clause clause in law.lstChapters[0].lstChapterItem[0].lstArtical[0].lstClause)
        //{
        //    string content = clause.Content;
        //    content = content.Substring(clause.Number.ToString().Length + 2);
        //    bool isCHiphi = content.ToLower().StartsWith("chi phí");
        //    string linkstring = isCHiphi || clause.Content.StartsWith("27. Tổ chức kinh tế") ? " bao gồm " : " là ";
        //    string concept = content.Substring(0, content.IndexOf(linkstring));
        //    string conceptContent = content;
        //    pd.ExecuteDataSet(string.Format("Exec GetConcept N'{0}',N'{1}'", concept, content));
        //}
        //return;
        DataSet ds = pd.ExecuteDataSet("select name, ID,Description from concept");
    
        //law.Load(2);
        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        {
            string concept = ds.Tables[0].Rows[i][0].ToString().ToLower();
            string Description = ds.Tables[0].Rows[i]["Description"].ToString().ToLower();
            int cID = Globals.GetIDinDS(ds, i, "ID");
            int keyPhraseID = Globals.GetIDinDS(pd.GenerateKeyPhrase(Globals.GetKeyJoin(concept), cID), 0, 0);
            List<KeyPhrase> lst = KeyPhrase.GetKeyPhraseFromDB(Description);
            foreach (KeyPhrase item in lst)
            {
                int Count = Globals.CountTerm(Description, item.Key.Replace("_"," "));
                pd.ExecuteDataSet("UpdateConcept_KeyPhrase " + cID + "," + item.ID + ",2,"+ Count);
            }
            continue;
            foreach (var ch in law.lstChapters)
            {
                foreach (var chi in ch.lstChapterItem)
                {
                    foreach (var ar in chi.lstArtical)
                    {
                        if (ar.ArticalContent != "")
                        {
                            if (ar.ArticalContent.ToLower().Contains(concept))
                            {
                                pd.ExecuteDataSet("exec UpdateConcept " + cID + "," + ar.ChapterID + "," + ar.ChapterItemID + "," + ar.ID + ",0,0," + law.ID);
                                pd.ExecuteDataSet("exec UpdateKeyPhraseByArtical " + keyPhraseID + "," + ar.ID + "," + ar.ChapterID + "," + ar.ChapterItemID);
                            }
                        }
                        else
                        {
                            foreach (var cl in ar.lstClause)
                            {
                                if (cl.Content != "")
                                {
                                    if (cl.Content.ToLower().Contains(concept))
                                    {
                                        pd.ExecuteDataSet("exec UpdateConcept " + cID + "," + ar.ChapterID + "," + ar.ChapterItemID + "," + ar.ID + "," + cl.ID + ",0," + law.ID);
                                        pd.ExecuteDataSet("exec UpdateKeyPhraseByClause " + keyPhraseID + "," + ar.ID + "," + ar.ChapterID + "," + ar.ChapterItemID + "," + cl.ID);
                                    }
                                }
                                else
                                {
                                    bool isExist = false;
                                    foreach (var pi in cl.lstPoints)
                                    {
                                        if (pi.Content.ToLower().Contains(concept))
                                        {
                                            pd.ExecuteDataSet("exec UpdateConcept " + cID + "," + ar.ChapterID + "," + ar.ChapterItemID + "," + ar.ID + "," + cl.ID + "," + pi.ID + "," + law.ID);
                                            isExist = true;
                                        }
                                    }
                                    if (isExist)
                                    {
                                        pd.ExecuteDataSet("exec UpdateKeyPhraseByClause " + keyPhraseID + "," + ar.ID + "," + ar.ChapterID + "," + ar.ChapterItemID + "," + cl.ID);

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}