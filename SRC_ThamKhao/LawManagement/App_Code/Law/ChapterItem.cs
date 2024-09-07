using System;
using System.Collections.Generic;
using System.Data;

namespace BDSLaw
{
    public class ChapterItem
    {
        public ChapterItem()
        {
            lstArtical = new List<Artical>();
        }

        public ChapterItem(SelectedLawItemType selectedType)
        {
            LawID = selectedType.LawID;
            ChapterID = selectedType.chapterID;
            Number = selectedType.Number;
            Name = selectedType.Name;
            Title = selectedType.Title;
            ID = selectedType.chapteritemID;
        }

        public int LawID { get; set; }
        public int ID { get; set; }
        public int ChapterID { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int ChapterNumber { get; set; }
        public List<Artical> lstArtical { get; set; }

        public bool ParseContent(bool isGenerateNewID, string chapterContent, int number, int ChapterID, int ChapterNumber)
        {
            try
            {
                string tail = ".";
               this.ChapterID = ChapterID; string ChapterItemContent = "";
                this.ChapterNumber = ChapterNumber;
               
                if (chapterContent.IndexOf("MỤC " + ChapterNumber, StringComparison.InvariantCultureIgnoreCase) < 0 
                    && chapterContent.IndexOf("MỤC 1\r\n", StringComparison.InvariantCultureIgnoreCase) < 0
                    && chapterContent.IndexOf("MỤC 1.", StringComparison.InvariantCultureIgnoreCase) < 0 
                    && chapterContent.IndexOf("MỤC 1:", StringComparison.InvariantCultureIgnoreCase) < 0)
                {
                    if (number > 1)
                        return false;
                    else
                    {
                        Number = 0; // muc 0
                        Name = "";
                        ChapterItemContent = chapterContent;
                    }
                }
                else
                {
                    Name = "MỤC " + number;
                  
                    Number = number;
                    //if (chapterContent.IndexOf(Name + ".", StringComparison.InvariantCultureIgnoreCase) < 0 && chapterContent.IndexOf(Name + ":", StringComparison.InvariantCultureIgnoreCase) > 0)
                    tail = Globals.GetTail(chapterContent, Name);
                    //if (string.IsNullOrEmpty(ChapterItemContent))
                    {
                        ChapterItemContent = Globals.NextItem(chapterContent, Name);
                        ChapterItemContent = Globals.GetContentBetween(chapterContent, Name, "MỤC " + (number + 1), true, true);
                    }

                }
                if (ChapterItemContent == "") return false;
                LuatProvider pd = new LuatProvider();
                ID = !isGenerateNewID ? -1 : Globals.GetIDinDS(pd.ExecuteDataSet("exec GetChapterItem N'" + Name + "'," + Number + ", " + ChapterID), 0, 0);
                if (ID > 0 || !isGenerateNewID)
                {
                    lstArtical = new List<Artical>();
                    Artical item = new Artical();
                    int tempArticalnumber = GetFirstArticalNumber(ChapterItemContent, Number);
                    if (tempArticalnumber > 0 && Number > 0)
                    {
                        Title = Globals.GetContentBetween(chapterContent, "\nMỤC " + (Number), "\nĐiều " + tempArticalnumber + Globals.GetTail(chapterContent, "\nĐiều " + tempArticalnumber), false, true);
                    }
                    else Title = Globals.GetContentBetween(chapterContent, "\nMỤC " + (Number), "\r\n", false, true);Title = Title.Trim('.').Trim(':').Trim();
                    while (tempArticalnumber > 0 && item.ParseContent(isGenerateNewID, ChapterItemContent, tempArticalnumber, ID, ChapterID, ChapterNumber))
                    {                      
                        tempArticalnumber++;
                        lstArtical.Add(item); item = new Artical();
                    }
                  if(lstArtical.Count == 0)
                    {
                        string s = "";
                    }
                    return true;
                }

            }
            catch (Exception ex)
            {

                Globals.WriteLog(ex);
            }

            return false;
        }

        private int GetFirstArticalNumber(int lastArticalnumber, string chapterContent)
        {

            return chapterContent.IndexOf("Điều " + (lastArticalnumber + 1) + ".") > 0 ? lastArticalnumber + 1 : -1;
        }
        private int GetFirstArticalNumber( string chapterItemContent, int chapterItemnumber)
        {
            
            if (chapterItemnumber == 0)
            {           
                if(chapterItemContent.IndexOf("Điều ",StringComparison.OrdinalIgnoreCase) < 0)               
                    return -1;
            }
            else if (chapterItemContent.IndexOf("MỤC " + chapterItemnumber + ". ", StringComparison.OrdinalIgnoreCase) < 0 && chapterItemContent.IndexOf("MỤC " + chapterItemnumber + ":", StringComparison.OrdinalIgnoreCase) < 0)
                    return -1;
            
            string sNumber = Globals.GetContentBetween(chapterItemContent, "\nĐiều ", ".", true, true);
            if(sNumber=="")
                sNumber = Globals.GetContentBetween(chapterItemContent, "\nĐiều ", ":", true, true);
            if (sNumber != "")
            {
                if (sNumber.Contains("\n"))
                    sNumber = sNumber.Substring(sNumber.LastIndexOf("\n"));
                sNumber = sNumber.Replace("Điều ", "").Replace(".", "").Replace(":", "").Trim();
                int iNumber = 0;
                return int.TryParse(sNumber, out iNumber) ? iNumber : -1;
            }
            return -1;
        }
        internal void LoadArtical(int articalID)
        {
            LuatProvider pd = new LuatProvider();
            //string sContent = "Content";
            DataSet ds = pd.ExecuteDataSet("select ID, name,[Title], number, [ChapterID], [ChapterItemID],Content, LawID from [Artical] where [ChapterID] = " + ChapterID + " and [ChapterItemID] = " + ID + (articalID < 0 ? "" : " and ID = " + articalID) + " order by Number");
            lstArtical = new List<Artical>();
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Artical item = new Artical();
                    item.ID = Globals.GetIDinDS(ds, i, 0);
                    item.Name = Globals.GetinDS_String(ds, i, 1);
                    item.Title = Globals.GetinDS_String(ds, i, 2);
                    item.Number = Globals.GetIDinDS(ds, i, 3);
                    item.ChapterID = Globals.GetIDinDS(ds, i, 4);
                    item.ChapterItemID = Globals.GetIDinDS(ds, i, 5);
                    item.LawID = Globals.GetIDinDS(ds, i, "LawID");
                    item.ChapterNumber = ChapterNumber;
                    item.LoadClause(articalID);
                    if (item.lstClause.Count == 0)
                        item.ArticalContent = Globals.GetinDS_String(ds, i, "Content");
                    else item.ArticalContent = "";

                    lstArtical.Add(item);
                }
            }
        }

        public void Update()
        {
            LuatProvider pd = new LuatProvider();
            pd.ExecuteDataSet("update chapterItem set Name = N'" + Name + "', Title = N'" + Title + "', Number = " + Number + " where ID = " + ID + " and lawID =" + LawID +" and chapterID = "+ ChapterID);

        }

        internal void ConfirmedEdit()
        {
            LuatProvider pd = new LuatProvider();
            if (ID < 0)
                ID = Globals.GetIDinDS(pd.ExecuteDataSet("exec GetChapterItem N'" + Name + "'," + Number + ", " + ChapterID), 0, 0);
            foreach (var item in lstArtical)
                item.ConfirmedEdit();

        }

        public ChapterItem CloneWithoutData()
        {
            ChapterItem ci = new ChapterItem
            {
                Name = this.Name,
                Title = this.Title,
                Number = this.Number,
                ID = this.ID,
                LawID = this.LawID,
                ChapterID = this.ChapterID,
                lstArtical = new List<Artical>()
            };
            return ci;
        }

        public string IsValid()
        {
            return string.IsNullOrEmpty(Title) || Number <= 0 || string.IsNullOrEmpty(Name) ? "Tiêu đề / Số Mục không hợp lệ" : "";
        }

        internal void UpdateKeyPhrases()
        {
            foreach (var item in lstArtical)
            {
                item.UpdateKeyPhrases();
            }
        }

        public int ManualInsertNew()
        {
            LuatProvider pd = new LuatProvider();
            return Globals.GetIDinDS(pd.ExecuteDataSet("exec ManualInsertNewChapterItem N'" + Name.Replace("'", "''") + "," + Number + ","+ChapterID+",N'" + Title.Replace("'", "''") + "'," + LawID), 0, 0);

        }

        public  int GetChapterItemNumber(string content)
        {
            content = content.TrimStart();
            if (content.IndexOf("Mục", StringComparison.InvariantCultureIgnoreCase) < 0) return -1;
            string snumber = Globals.GetContentBetween(content, "MỤC ", ".", false, true);
            if(snumber == "") snumber = Globals.GetContentBetween(content, "MỤC ", ":", false, true);
            int number = 0;
            if (!int.TryParse(snumber, out number)) return -2;
            return number;
          
        }

        private  bool Exists(int number)
        {
            LuatProvider pd = new LuatProvider();
            return Globals.GetIDinDS(pd.ExecuteDataSet("select top 1 1 from chapterItem with(nolock) where ChapterID = " + ChapterID + " and number = " + number), 0, 0) > 0;

        }

        public void Delete()
        {
            LuatProvider pd = new LuatProvider();
            pd.ExecuteDataSet("Delete from ChapterItem where LawID = " + LawID + " and ID = " + ID);

        }
    }
}