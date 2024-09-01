using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BDSLaw
{
    public class LawDoc
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public List<Chapter> lstChapters { get; set; }

        public void ParseContent(bool isGenerateNewID, string content, string name)
        {
            if (content == "") return;
            this.Name = name;
            LuatProvider pd = new LuatProvider();
            ID = Globals.GetIDinDS(pd.ExecuteDataSet("exec GetLaw N'" + Name + "'"), 0, 0);

            if (ID > 0)
            {
                int chapterNumber = 1;
                lstChapters = new List<Chapter>();
                Chapter item = new Chapter();
                while (item.ParseContent(isGenerateNewID, content, chapterNumber++, ID))
                {
                    lstChapters.Add(item); item = new Chapter();
                }
            }
        }

       
        //public void ViewDetailArticalByID(int articalID)
        //{
        //    LuatProvider pd = new LuatProvider();
        //    DataSet ds = pd.ExecuteDataSet("exec [GetArticalDetail] " + articalID);
        //    if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0) return;
        //    Name = Globals.GetinDS_String(ds, 0, "LawName");
        //    lstChapters = new List<Chapter> { new Chapter {
        //        Name = Globals.GetinDS_String(ds, 0, "ChapterName"),
        //        Number = Globals.GetIDinDS(ds, 0, "ChapterNumber"),
        //        lstChapterItem= new List<ChapterItem>(){
        //            new ChapterItem
        //            {
        //                Name = Globals.GetinDS_String(ds, 0, "ChapterItemName"),
        //                lstArtical = new List<Artical>()
        //                {
        //                    new Artical{
        //                        ID = articalID,
        //                        Name = Globals.GetinDS_String(ds, 0, "Name"),
        //                          Title = Globals.GetinDS_String(ds, 0, "Title"),
        //                          ArticalContent =  Globals.GetinDS_String(ds, 0, "Content"),
        //                          lstClause = GetListClause(ds)
        //                        }
        //                }
        //            }
        //        }
        //        } };
        //}

        private List<Clause> GetListClause(DataSet ds)
        {
            List<Clause> lst = new List<Clause>();
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                if (item["Content"].ToString() == "")
                    lst.Add(new Clause { Content = item["ClauseContent"].ToString(), Number = int.Parse(item["ClauseNumber"].ToString()) });

            }
            return lst;
        }

     
       public void Load(int _LawID, int ChapterID = -1, int ChapterItemID = -1,  int articalID = -1 )
        {
           
            LuatProvider pd = new LuatProvider();
            DataSet ds = pd.ExecuteDataSet("select top 1 ID, Name from Law where ID = " + _LawID);
            ID = Globals.GetIDinDS(ds, 0, 0);
            if (ID < 0) return;
            Name = Globals.GetinDS_String(ds, 0, 1);
            // load chapter
            ds = pd.ExecuteDataSet("select ID, name, title, number from Chapter c where LawID = " + _LawID + (ChapterID <0?"":" and ID = "+ ChapterID) +" order by Number");
            lstChapters = new List<Chapter>();
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Chapter item = new Chapter();
                    item.ID = Globals.GetIDinDS(ds, i, 0);
                    item.Name = Globals.GetinDS_String(ds, i, 1);
                    item.Title = Globals.GetinDS_String(ds, i, 2);
                    item.Number = Globals.GetIDinDS(ds, i, 3);
                    item.LawID = ID;
                    item.LoadChapterItem(ChapterItemID, articalID);

                    lstChapters.Add(item);
                }
            }
        }

        public void UpdateKeyPhrases()
        {
            foreach (Chapter item in lstChapters)
            {
                item.UpdateKeyPhrases();
            }
        }
    }
}
