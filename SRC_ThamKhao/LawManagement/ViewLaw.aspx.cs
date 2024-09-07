using BDSLaw;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ViewLaw : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LuatProvider pd = new LuatProvider();
            DataSet ds = pd.ExecuteDataSet("select ID, name from Law where status = 1 order by Name");
            if (ds != null)
            {
                cboLuat.Items.Clear();
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    cboLuat.Items.Add(new ListItem(r["name"].ToString(), r["ID"].ToString()));
                }
                cboLuat.Items[cboLuat.Items.Count -1 ].Selected = true;
            }
        }
    }

    protected void btnView_Click(object sender, EventArgs e)
    {
        int ID = int.Parse(cboLuat.SelectedValue);
      
        string ss = Globals.GetinDS_String("select top 1 ContentHTML from [LawHTML] where LawID ="+ ID, 0);
        UpdateChuong(ID);
        UpdateMuc(ID, -1);
        UpdateDieu(ID,-1, -1);
       
        divContentHTMLLaw.InnerHtml = ss;
        ScriptManager.RegisterStartupScript(this, Page.GetType(), "btnView_Click", "ViewLaw("+ ID + ")", true);//OnClientClick="ViewLaw();return false;"

    }

    private void UpdateDieu(int LawID, int ChapterID, int ChapterItemID)
    {  LuatProvider pd = new LuatProvider();
        cboDieu.Items.Clear();
        cboDieu.Items.Add(new ListItem("-Tất cả-", "-1"));
        DataSet ds = pd.ExecuteDataSet("Exec GetArticalFilter "+LawID+","+ ChapterID +","+ ChapterItemID);

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
        LawDoc law = new LawDoc();
        law.Load(ID);
     
        return law;
    }
    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static LawDoc ViewDetailArticalByView(List<int> lstArticalID, int chapterID, int LawID)
    {
        try
        {
            SearchProvider pd = new SearchProvider();
            if( lstArticalID==null || lstArticalID.Count == 0)
                return pd.ViewChapterDetail(chapterID);
            return pd.ViewArticalDetail(lstArticalID, LawID);
          

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
        UpdateDieu(int.Parse(cboLuat.SelectedValue), -1, - 1);
        ScriptManager.RegisterStartupScript(this, Page.GetType(), "btnView_Click", "ViewLaw(" + cboLuat.SelectedValue + ")", true);//OnClientClick="ViewLaw();return false;"
    }
    protected void cboChuong_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateMuc(int.Parse(cboLuat.SelectedValue), int.Parse(cboChuong.SelectedValue));
        UpdateDieu(int.Parse( cboLuat.SelectedValue), int.Parse(cboChuong.SelectedValue) , - 1);
        ScriptManager.RegisterStartupScript(this, Page.GetType(), "btnView_Click", "ViewLaw(" + cboLuat.SelectedValue + ")", true);//OnClientClick="ViewLaw();return false;"
    }

    protected void cboMuc_SelectedIndexChanged(object sender, EventArgs e)
    {
        int MucID = int.Parse(cboMuc.SelectedValue);
        LuatProvider pd = new LuatProvider();
        int chuongID = Globals.GetIDinDS(pd.ExecuteDataSet("select top 1 chapterID from chapteritem where ID = "+ MucID), 0, 0);
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