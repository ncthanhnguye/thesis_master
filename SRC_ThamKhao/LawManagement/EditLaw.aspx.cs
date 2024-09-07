using BDSLaw;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class EditLaw : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            MOABSearch.Common.Globals.ValidateSession();
            HttpContext.Current.Session["LastEditlaw"] = null;
            LuatProvider pd = new LuatProvider();
            DataSet ds = pd.ExecuteDataSet("select ID, name from Law where status = 1 order by Name");
            if (ds != null)
            {
                cboLuat.Items.Clear();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    cboLuat.Items.Add(new ListItem(r["name"].ToString(), r["ID"].ToString()));
                }
                cboLuat.Items[cboLuat.Items.Count - 1].Selected = true;
                hdfIsEdit.Value = "1";
            }
        }
    }

    protected void btnView_Click(object sender, EventArgs e)
    {
        int ID = int.Parse(cboLuat.SelectedValue);

        string ss = Globals.GetinDS_String("select top 1 ContentHTML from [LawHTML] where LawID =" + ID, 0);
        UpdateChuong(ID);
        UpdateMuc(ID, -1);
        UpdateDieu(ID, -1, -1);

        //divContentHTMLLaw.InnerHtml = ss;
        ScriptManager.RegisterStartupScript(this, Page.GetType(), "btnView_Click", "ViewLaw(" + ID + ")", true);//OnClientClick="ViewLaw();return false;"

    }

    private void UpdateDieu(int LawID, int ChapterID, int ChapterItemID)
    {
        LuatProvider pd = new LuatProvider();
        cboDieu.Items.Clear();
        cboDieu.Items.Add(new ListItem("-Tất cả-", "-1"));
        DataSet ds = pd.ExecuteDataSet("Exec GetArticalFilter " + LawID + "," + ChapterID + "," + ChapterItemID);

        if (ds != null)
        {

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                cboDieu.Items.Add(new ListItem(r["name"].ToString(), r["ID"].ToString()));
            }
        }
        cboDieu.Items[0].Selected = true;
    }

    private void UpdateMuc(int lawID, int chapterID = -1, int ChapterItemID = -1)
    {
        LuatProvider pd = new LuatProvider();
        cboMuc.Items.Clear();
        cboMuc.Items.Add(new ListItem("-Tất cả-", "-1"));
        DataSet ds = pd.ExecuteDataSet("select ci.ID, c.name +' - '+ ci.name Name from ChapterItem ci left join chapter c on ci.chapterID = c.ID where "
            + "c.lawid =" + lawID + (chapterID < 0 ? "" : " and ci.chapterID =" + chapterID)
            + (ChapterItemID < 0 ? "" : " and ci.ID =" + ChapterItemID)
            + " and ci.name is not null order by Name");

        if (ds != null)
        {

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                cboMuc.Items.Add(new ListItem(r["name"].ToString(), r["ID"].ToString()));
            }
        }
        cboMuc.Items[0].Selected = true;
    }

    private void UpdateChuong(int LawID)
    {

        cboChuong.Items.Clear();
        cboChuong.Items.Add(new ListItem("-Tất cả-", "-1"));
        LuatProvider pd = new LuatProvider();
        DataSet ds = pd.ExecuteDataSet("select ID, name from Chapter where lawid =" + LawID + " order by Number");

        if (ds != null)
        {

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                cboChuong.Items.Add(new ListItem(r["name"].ToString(), r["ID"].ToString()));
            }
        }
        cboChuong.Items[0].Selected = true;
    }

    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static LawDoc LoadLawData(int ID)
    {
        LawDoc law = Globals.GetLaw(ID);
        return law;
    }
    //GetLawInfoToEdit
    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]  
    public static EditLawResult EditLawItem(SelectedLawItemType selectedType)
    {

        EditLawResult result = new EditLawResult();
        result.SetItem(selectedType);
        
        if (selectedType.type == 0)// chuong
        {
            Chapter item = new Chapter { Number = selectedType.Number, LawID = selectedType.LawID, Name = selectedType.Name, Title = selectedType.Title, ID = selectedType.chapterID };
            item.Update();
        }
        else if (selectedType.type == 1)
        {
            ChapterItem item = new ChapterItem(selectedType);
            item.Update();
        }
        else if (selectedType.type == 2)
        {
            Artical item = new Artical(selectedType, result.chapter.Number);
            item.Update();
        }
        else if (selectedType.type == 3)
        {
            Clause item = new Clause(selectedType);
            item.UpdateManual();
        }
        Globals.ReloadLaw(selectedType.LawID);
        return result;
    }

   

    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static string AddNewLaw_Confirm()
    {
        try
        {
            EditLawResult result = Globals.GetLastEditLaw();
            if (result == null || result.law == null) return "error";
            foreach (var item in result.law.lstChapters)
            {
                item.ConfirmedEdit();
            }
            Globals.ReloadLaw(result.law.ID);
            return "";
        }
        catch (Exception ex)
        {
            Globals.WriteLog(ex);
            return "error";
        }

    }
    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static EditLawResult AddNewLaw(SelectedLawItemType selectedType)
    {

        EditLawResult result = new EditLawResult();        
        result.SetItem(selectedType);
        LawDoc mylaw = Globals.GetLaw(selectedType.LawID);
        Chapter mychapter = result.law.lstChapters[0];
        ChapterItem mychapterItem = result.chapterItem; Artical myArtical = result.artical;
  
        if (selectedType.isAuto)
        {
            if (selectedType.type == 0)// chuong
            {
                Chapter item = new Chapter();
                int Number = item.GetChapterNumber(selectedType.content);
                if (Number < 0)
                {
                    result.icode = Number;
                    result.mess = item.IsValid();
                    return result;
                }
                // rewrite
                selectedType.content = item.CleanStart(Number, selectedType.content);
                item.ParseContent(false, selectedType.content, Number, selectedType.LawID);
                result.mess = item.IsValid();
                result.chapter = item;
                result.law.lstChapters = new List<Chapter>() { item };
            }
            else if (selectedType.type == 1)
            {
                ChapterItem item = new ChapterItem (selectedType);

                if (mychapter == null)
                {
                    result.mess = "Chương không tìm thấy.";
                    return result;
                }
                int Number = item.GetChapterItemNumber(selectedType.content);

                if (Number < 0)
                {
                    result.mess = item.IsValid();
                    return result;
                }

                item.ParseContent(false, selectedType.content, Number, item.ChapterID, selectedType.chapterNumber);
                result.mess = item.IsValid();
                result.chapterItem = item;
                result.law.lstChapters[0].lstChapterItem = new List<ChapterItem>() { item };
            }
            else if (selectedType.type == 2)
            {
                Artical item = new Artical(selectedType, mychapter.Number);
                int Number = item.GetArticalNumber(selectedType.content);
                if (Number < 0)
                {
                    result.mess = item.IsValid();
                    return result;
                }
                if (mychapter == null || mychapterItem == null)
                {
                    result.mess = "Chương / Mục không tìm thấy.";
                    return result;
                }
                item.ParseContent(false, selectedType.content, Number, item.ChapterItemID, item.ChapterID, selectedType.chapterNumber);
                result.mess = item.IsValid();
                result.artical = item;
                result.law.lstChapters[0].lstChapterItem[0].lstArtical.Add(item);

            }
            else if (selectedType.type == 3)
            {
                Clause item = new Clause(selectedType);
                int Number = item.GetChapterClauseNumber(selectedType.content);
                if (Number < 0)
                {
                    result.mess = item.IsValid();
                    return result;
                }

                item.ParseContent(false, selectedType.content, Number, item.ArticalID);
                result.mess = item.IsValid();
                result.clause = item;
                result.law.lstChapters[0].lstChapterItem[0].lstArtical[0].lstClause.Add(item);
            }
        }
        else
        {
            if (selectedType.type == 0)// chuong
            {
                Chapter item = new Chapter(selectedType);                
                result.icode = item.ManualInsertNew();
                result.mess = item.IsValid();
                result.chapter = item;
                result.law.lstChapters = new List<Chapter> { item };
            }
            else if (selectedType.type == 1)
            {
                ChapterItem item = new ChapterItem(selectedType);             
                result.icode = item.ManualInsertNew();
                result.mess = item.IsValid();
                result.chapterItem = item;
                result.law.lstChapters[0].lstChapterItem = new List<ChapterItem>() { item };
            }
            else if (selectedType.type == 2)
            {
                Artical item = new Artical(selectedType, mychapter.Number);               
                result.icode = item.ManualInsertNew();
                result.mess = item.IsValid();
                result.artical = item;
                result.law.lstChapters[0].lstChapterItem[0].lstArtical = new List<Artical> { item };
            }
            else if (selectedType.type == 3)
            {
                Clause item = new Clause(selectedType);
                
                result.icode = item.ManualInsertNew();
                result.mess = item.IsValid();
                result.clause = item;
                result.law.lstChapters[0].lstChapterItem[0].lstArtical[0].lstClause = new List<Clause>() { item };
            }

            // remove law from sesstion to reload
            Globals.ReloadLaw(mylaw.ID);
        }

        Globals.SetLastEditLaw(result);
        return result;
    }
    //DeleteLaw
    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static int DeleteLaw(int LawID, int type, int parentID, int ID)
    {
        LawDoc law = Globals.GetLaw(LawID);
        if (type == 0)// chuong
        {
            int chapterID = ID;
            Chapter chapter = law.lstChapters.Where(x => x.ID == chapterID).FirstOrDefault();
            if (chapter == null) return -2;
           
            if (chapter.lstChapterItem.Count > 1 || chapter.lstChapterItem.Count == 1 && chapter.lstChapterItem[0].lstArtical.Count > 0)
                return -1;
            chapter.Delete();
        }
        else if (type == 1)// muc
        {
            int chapterID = parentID;
            Chapter chapter = law.lstChapters.Where(x => x.ID == chapterID).FirstOrDefault();
            if (chapter == null) return -2;
            ChapterItem chapterItem = chapter.lstChapterItem.Where(x => x.ID == ID).FirstOrDefault();
            if (chapterItem == null) return -2;

            if (chapterItem.lstArtical.Count > 0)
                return -1;
            chapterItem.Delete();
        }
        else if (type == 2)// Artical
        {
            int chapterID = parentID;
            Chapter chapter = law.lstChapters.Where(x => x.lstChapterItem.Exists(y => y.ID == parentID)).FirstOrDefault();
            if (chapter == null) return -2;
      
            Artical artical = chapter.lstChapterItem.Where(x=>x.ID == parentID).First().lstArtical.Where(x => x.ID == ID).FirstOrDefault();
            if (artical == null) return -2;
            if (artical.lstClause.Count > 0)
                return -1;
            artical.Delete();
            if (chapter.lstChapterItem.Count == 1 && chapter.lstChapterItem[0].Number == 0 && chapter.lstChapterItem[0].lstArtical.Count(x => x.ID != artical.ID) == 0)
                chapter.lstChapterItem[0].Delete();
        }
        else if (type == 3)// Artical
        {
            int chapterID = parentID;
            Chapter chapter = law.lstChapters.Where(x => x.lstChapterItem.Exists(y => y.lstArtical.Exists(z=>z.ID == parentID))).FirstOrDefault();
            if (chapter == null) return -2;
            Clause clause = chapter.lstChapterItem.Where(x => x.lstArtical.Exists(z => z.ID == parentID)).First().lstArtical.Where(x => x.ID == parentID).First().lstClause.Where(x=>x.ID == ID).FirstOrDefault();
            if (clause == null) return -2;
            if (clause.lstPoints.Count > 0)
            {
                LuatProvider pd = new LuatProvider();
                pd.ExecuteDataSet("Exec DeletePointByClause " + clause.ID);
            }
            clause.Delete();
        }
        Globals.ReloadLaw(LawID);
        return 1;
    }
   [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static LawDoc ViewDetailArticalByView(List<int> lstArticalID, int chapterID, int LawID)
    {
        try
        {
            SearchProvider pd = new SearchProvider();
            if (lstArticalID == null || lstArticalID.Count == 0)
                return pd.ViewChapterDetail(chapterID);
            return pd.ViewArticalDetail(lstArticalID, LawID);
       
        }
        catch (Exception ex)
        {
            Globals.WriteLog(ex);
            return null;
        }
    }

    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static KeyPhraseLawResult ViewDetailArtical_KeyPhrase(List<int> lstArticalID, int LawID)
    {
        try
        {
            SearchProvider pd = new SearchProvider();
            if (lstArticalID == null || lstArticalID.Count == 0)
                return null;
            HttpContext.Current.Session["LastKeyPhrase_lstArticalID"] = lstArticalID;
            return pd.GetKeyPhraseLawResult(lstArticalID, LawID);

        }
        catch (Exception ex)
        {
            Globals.WriteLog(ex);
            return null;
        }
    }
    //KeyPhrase_Changed
    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static string KeyPhrase_Changed(KeyPhraseChanged keyPhraseChanged)
    {
        try
        {
            SearchProvider pd = new SearchProvider();
            List<int> lstArticalID = HttpContext.Current.Session["LastKeyPhrase_lstArticalID"] as List<int>;
            if (lstArticalID == null || lstArticalID.Count == 0)
                return null;
            foreach (var item in keyPhraseChanged.lstAdd)
            {
                pd.GenerateKeyPhrase(item);
            }
            

            pd.DeletekeyphraseMapping(lstArticalID, keyPhraseChanged.lstDelete);
            return "";
        }
        catch (Exception ex)
        {
            Globals.WriteLog(ex);
            return null;
        }
    }
    protected void cboLuat_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateChuong(-1);
        UpdateMuc(int.Parse(cboLuat.SelectedValue), -1);
        UpdateDieu(int.Parse(cboLuat.SelectedValue), -1, -1);
        ScriptManager.RegisterStartupScript(this, Page.GetType(), "btnView_Click", "ViewLaw(" + cboLuat.SelectedValue + ")", true);//OnClientClick="ViewLaw();return false;"
    }
    protected void cboChuong_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateMuc(int.Parse(cboLuat.SelectedValue), int.Parse(cboChuong.SelectedValue));
        UpdateDieu(int.Parse(cboLuat.SelectedValue), int.Parse(cboChuong.SelectedValue), -1);
        ScriptManager.RegisterStartupScript(this, Page.GetType(), "btnView_Click", "ViewLaw(" + cboLuat.SelectedValue + ")", true);//OnClientClick="ViewLaw();return false;"
    }

    protected void cboMuc_SelectedIndexChanged(object sender, EventArgs e)
    {
        int MucID = int.Parse(cboMuc.SelectedValue);
        LuatProvider pd = new LuatProvider();
        int chuongID = Globals.GetIDinDS(pd.ExecuteDataSet("select top 1 chapterID from chapteritem where ID = " + MucID), 0, 0);
        SelectChuong(chuongID);



        UpdateDieu(int.Parse(cboLuat.SelectedValue), chuongID, int.Parse(cboMuc.SelectedValue));
        ScriptManager.RegisterStartupScript(this, Page.GetType(), "btnView_Click", "ViewLaw(" + cboLuat.SelectedValue + ")", true);//OnClientClick="ViewLaw();return false;"
    }

    private void SelectChuong(int chuongID)
    {
        cboChuong.SelectedItem.Selected = false;
        for (int i = 0; i < cboChuong.Items.Count; i++)
        {
            if (cboChuong.Items[i].Value == chuongID.ToString())
            {
                cboChuong.Items[i].Selected = true;
                break;
            }
        }
    }
    private void SelectMuc(int chapterItemID)
    {
        cboMuc.SelectedItem.Selected = false;
        for (int i = 0; i < cboChuong.Items.Count; i++)
        {
            if (cboMuc.Items[i].Value == chapterItemID.ToString())
            {
                cboMuc.Items[i].Selected = true;
                break;
            }
        }
    }
    protected void cboDieu_SelectedIndexChanged(object sender, EventArgs e)
    {
        int ArticalID = int.Parse(cboDieu.SelectedValue);
        LuatProvider pd = new LuatProvider();
        DataSet ds = pd.ExecuteDataSet("exec [GetArticalDetail] " + ArticalID);
        int _LawID = Globals.GetIDinDS(ds, 0, "LawID");
        int ChapterID = Globals.GetIDinDS(ds, 0, "ChapterID");
        int ChapterItemID = Globals.GetIDinDS(ds, 0, "ChapterItemID");
        SelectChuong(ChapterID);
        SelectMuc(ChapterItemID);
        ScriptManager.RegisterStartupScript(this, Page.GetType(), "btnView_Click", "ViewLaw(" + cboLuat.SelectedValue + ")", true);//OnClientClick="ViewLaw();return false;"
    }



    protected void btnUpdateKeyPhrase_Click(object sender, EventArgs e)
    {

        RegisterAsyncTask(new PageAsyncTask(UpdateKeyPhrases));

    }

    private async Task UpdateKeyPhrases()
    {
        int lawID = int.Parse(cboLuat.SelectedValue);

        KeyPhrase.ResetKeyPhrase(lawID);

        // update Vetor
    }

    protected void btnView_Click1(object sender, EventArgs e)
    {

    }


}