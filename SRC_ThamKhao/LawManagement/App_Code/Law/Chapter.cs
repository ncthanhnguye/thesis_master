using System;
using System.Collections.Generic;
using System.Data;

namespace BDSLaw
{
    public class Chapter
    {

        public int ID { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public int LawID { get; set; }
        public Chapter() { lstChapterItem = new List<ChapterItem>(); }

        public Chapter(SelectedLawItemType selectedType)
        {
            Number = selectedType.Number;
            LawID = selectedType.LawID;
            Name = selectedType.Name;
            Title = selectedType.Title;
            lstChapterItem = new List<ChapterItem>();
            ID = selectedType.chapterID;
        }

        public List<ChapterItem> lstChapterItem { get; set; }

        public bool ParseContent(bool isGenerateNewID, string content, int number, int LawID)
        {
            try
            {
                string nameLama = "CHƯƠNG " + Globals.GetLama(number);
                string nameNormal = "CHƯƠNG " + number + ".";
                Name = nameLama;
                this.Number = number;
                this.LawID = LawID;
                string chapterContent = Globals.NextItem(content, nameLama);
                bool isChuongNormal = false;
                if (chapterContent == "")
                {
                    chapterContent = Globals.NextItem(content, nameNormal);
                    if (chapterContent != "")
                    {
                        isChuongNormal = true;
                        Name = nameNormal;
                    }
                }
                chapterContent = Globals.GetContentBetween(content, Name, "CHƯƠNG " + (isChuongNormal ? (number + 1).ToString() : Globals.GetLama(number + 1)), true, true);
                if (chapterContent == "" && number == 1)
                {
                    if (content.Contains("Điều 1.") || content.Contains("Điều 1:"))
                        chapterContent = content;
                    Number = 0; Name = ""; number = 0; Title = "";
                }
                if (chapterContent == "") return false;
                string dieu = GetTextSignBeginArtical(chapterContent);
                if (string.IsNullOrEmpty(dieu))
                {
                    if (chapterContent.Contains("Điều 1."))
                        dieu = "Điều 1.";
                    else if (chapterContent.Contains("Điều 1:"))
                        dieu = "Điều 1:";
                    else dieu = "\r\n";
                }

                string muc = chapterContent.IndexOf("MỤC 1.", StringComparison.InvariantCultureIgnoreCase) > 0 ? "MỤC 1." : "MỤC 1:";
                if (number > 0)
                    Title = Globals.GetContentBetween(chapterContent, Name, muc, false, true, dieu, "\r\n");

                LuatProvider pd = new LuatProvider();
                ID = !isGenerateNewID ? -1 : Globals.GetIDinDS(pd.ExecuteDataSet("exec GetChapter N'" + Name + "', N'" + Title + "'," + number + "," + LawID), 0, 0);
                if (ID > 0 || !isGenerateNewID)
                {
                    lstChapterItem = new List<ChapterItem>();
                    ChapterItem item = new ChapterItem();
                    int ChapterItemNumber = 1;
                    while (item.ParseContent(isGenerateNewID, chapterContent, ChapterItemNumber, ID, Number))
                    {
                        lstChapterItem.Add(item); item = new ChapterItem();
                        ChapterItemNumber++;
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

        private string GetTextSignBeginArtical(string chapterContent)
        {
            string content = chapterContent;
            while (content.Length > 0)
            {
                int idex = content.IndexOf("Điều ");
                if (idex > 0)
                {
                    content = content.Substring(idex + 5);
                    string sign = ".";
                    int idex2 = content.IndexOf(sign);
                    if (idex2 < 0)
                    {
                        sign = ":";
                        idex2 = content.IndexOf(sign);
                    }
                    if (idex2 > 0)
                    {
                        string number = content.Substring(0, idex2);
                        int iNumber = 0;
                        if (int.TryParse(number, out iNumber))
                        {
                            return "Điều " + iNumber + sign;
                        }
                        content = content.Substring(idex2 + 1);
                    }
                }
                else return null;
            }
            return null;
        }

        internal void LoadChapterItem(int ChapterItemID = -1, int articalID = -1)
        {
            LuatProvider pd = new LuatProvider();
            DataSet ds = pd.ExecuteDataSet("select ID, name, number, LawID, Title from [ChapterItem] where [ChapterID] = " + ID + (ChapterItemID < 0 ? "" : " and ID = " + ChapterItemID) + " order by Number");
            lstChapterItem = new List<ChapterItem>();
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ChapterItem item = new ChapterItem();
                    item.ID = Globals.GetIDinDS(ds, i, 0);
                    item.LawID = Globals.GetIDinDS(ds, i, "LawID");
                    item.ChapterID = ID;
                    item.Name = Globals.GetinDS_String(ds, i, 1);
                    item.Number = Globals.GetIDinDS(ds, i, 2);
                    item.Title = Globals.GetinDS_String(ds, i, "Title");
                    item.ChapterNumber = Number;
                    item.LoadArtical(articalID);
                    lstChapterItem.Add(item);
                }
            }
        }

        public void Update()
        {
            LuatProvider pd = new LuatProvider();
            pd.ExecuteDataSet("update chapter set Name = N'" + Name + "', Title = N'" + Title + "', Number = " + Number + " where ID = " + ID + " and lawID =" + LawID);
        }

        public void ConfirmedEdit()
        {
            LuatProvider pd = new LuatProvider();
            if (ID < 0)
                ID = Globals.GetIDinDS(pd.ExecuteDataSet("exec GetChapter N'" + Name + "', N'" + Title + "'," + Number + "," + LawID), 0, 0);
            foreach (var item in lstChapterItem)
                item.ConfirmedEdit();


        }

        public  bool Exists(int number)
        {
            LuatProvider pd = new LuatProvider();
            return Globals.GetIDinDS(pd.ExecuteDataSet("select top 1 1 from chapter with(nolock) where lawid = "+ LawID +" and number = "+ number), 0, 0) > 0;
        }

        public string IsValid()
        {
            return string.IsNullOrEmpty(Title) || Number <= 0 || string.IsNullOrEmpty(Name) ? "Tiêu đề / Số Chương không hợp lệ" : "";
        }

        public string CleanStart(int number, string content)
        {
            string key = "Chương " + number+".", key1 = "Chương " + (number+1) + ".";
            content = content.IndexOf(key, StringComparison.CurrentCultureIgnoreCase) < 0 ? "" : content.Substring(content.IndexOf(key, StringComparison.CurrentCultureIgnoreCase));
            if (content.IndexOf(key1, StringComparison.CurrentCultureIgnoreCase) > 0)
            {
                content = content.Substring(0, content.IndexOf(key1, StringComparison.CurrentCultureIgnoreCase));
            }
            return content;
        }

        public Chapter CloneWithoutData()
        {
            Chapter ci = new Chapter();
            ci.Name = Name;
            ci.Title = Title;
            ci.Number = Number;
            ci.ID = ID;
            ci.LawID = LawID;
            ci.lstChapterItem = new List<ChapterItem>();
            return ci;
        }

        public  int GetChapterNumber(string content)
        {
          
            if (content.IndexOf("Chương ", StringComparison.CurrentCultureIgnoreCase) < 0)
                return -1;
            string snumber = Globals.GetContentBetween(content, "Chương ", ".", false, true);
            int number = 0;
            if (!int.TryParse(snumber, out number)) return -2;
           
            // check exists
            //if (Exists(number))
            //{
            //    return -1;
            //}
            return number;

           
        }

        public int ManualInsertNew()
        {
            LuatProvider pd = new LuatProvider();
            return Globals.GetIDinDS(pd.ExecuteDataSet("exec ManualInsertNewChapter " + Number + ",N'" + Name.Replace("'", "''") + "',N'" + Title.Replace("'", "''") + "',"+LawID), 0, 0);
        }

        internal void UpdateKeyPhrases()
        {
            foreach (var item in lstChapterItem)
            {
                item.UpdateKeyPhrases();
            }
        }

        public void Delete()
        {
            LuatProvider pd = new LuatProvider();
            pd.ExecuteDataSet("Delete from Chapter where LawID = "+ LawID +" and ID = "+ ID);

        }
    }
}