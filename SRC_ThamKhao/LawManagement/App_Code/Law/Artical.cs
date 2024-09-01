using System;
using System.Collections.Generic;
using System.Data;

namespace BDSLaw
{
    public class Artical
    {
        public Artical()
        {
            lstClause = new List<Clause>();
        }

        public Artical(SelectedLawItemType selectedType, int chapterNumber)
        {
            LawID = selectedType.LawID;
            ChapterID = selectedType.chapterID;
            ChapterItemID = selectedType.chapteritemID;
            ChapterNumber = chapterNumber;
            Number = selectedType.Number;
            Name = selectedType.Name;
            Title = selectedType.Title;
            ArticalContent = selectedType.content;
            ID = selectedType.ArticalID;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int Number { get; set; }
        public string ArticalContent { get; set; }
        public int ChapterItemID { get; set; }
        public int ChapterID { get; set; }
        public int LawID { get; set; }
        public int ChapterNumber { get; set; }
        public List<Clause> lstClause { get; set; }
        public double score { get;  set; }

        public bool ParseContent(bool isGenerateNewID, string content, int number, int chapterItemID, int chapterID, int ChapterNumber)
        {
            try
            {
                
                Name = "Điều " + number;
                string tail = Globals.GetTail(content, Name);// content.IndexOf(Name + ".", StringComparison.InvariantCultureIgnoreCase) < 0 && content.IndexOf(Name + ":", StringComparison.InvariantCultureIgnoreCase) > 0 ? ":" : ".";
                string pre = "\n";
                this.Number = number;
                this.ChapterID = chapterID;
                this.ChapterItemID = chapterItemID;
                this.ChapterNumber = ChapterNumber;
                //if (string.IsNullOrEmpty(ArticalContent))
                {
                    ArticalContent = Globals.NextItem(content, pre+Name + tail);
                    ArticalContent = Globals.GetContentBetween(content, pre + Name + tail, "Điều " + (number + 1) + tail, true, true);
                }
                if (ArticalContent == "") return false;

                Title = Globals.GetContentBetween(ArticalContent, Name + tail, pre + "1" + tail, false, true, "\r\n");



                LuatProvider pd = new LuatProvider();
                ID = !isGenerateNewID?-1: Globals.GetIDinDS(pd.ExecuteDataSet("exec GetArtical N'" + Name + "', N'" + Title + "',N'" + ArticalContent + "'," + number + "," + chapterItemID + "," + chapterID), 0, 0);

                if (ID > 0 || !isGenerateNewID)
                {
                    lstClause = new List<Clause>();
                    string TrimArticalContent = ArticalContent.Substring(Math.Min(ArticalContent.Length - 1, ArticalContent.IndexOf(Name + tail) + Name.Length + 1));
                    if (TrimArticalContent.IndexOf("\n1. ") > 0 && TrimArticalContent.IndexOf("\n2. ") > 0)
                    {
                        Clause item = new Clause();
                        int ClauseNumber = 1;
                        while (item.ParseContent(isGenerateNewID, TrimArticalContent, ClauseNumber++, ID))
                        {
                            lstClause.Add(item); item = new Clause();
                        }
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

        public void UpdateKeyPharease(string[] keys)
        {

            LuatProvider pd = new LuatProvider(); int keyID = 0;
            foreach (var key in keys)
            {
               
                int Count = Globals.CountTerm(ArticalContent, key.Replace("_"," "));
                keyID = Globals.GetIDinDS(pd.GenerateKeyPhrase(key) , 0,0);
                if (keyID > 0)
                {                   
                    pd.ExecuteDataSet("exec UpdateKeyPhraseByArtical " + keyID + "," + ID + "," + ChapterID + "," + ChapterItemID+","+ Count);
                }
            }
        }

        public void LoadClause(int ArticalID)
        {
            LuatProvider pd = new LuatProvider();
            DataSet ds = pd.ExecuteDataSet("select ID, [ArticalID], number, Content, Title  from [Claust] where ArticalID = " + ID + " order by Number");
            lstClause = new List<Clause>();
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    Clause item = new Clause(); 
                    item.ID = Globals.GetIDinDS(ds,i, 0);
                    item.ArticalID = Globals.GetIDinDS(ds, i,1);
                    item.Number = Globals.GetIDinDS(ds,i, 2);
                    item.LoadPoint(true);
                    item.Title = Globals.GetinDS_String(ds, i, "Title");
                    if(item.Title!=""|| item.ID == 1318)
                    {
                        string d = "";
                    }
                    item.Content = item.lstPoints.Count > 0 ? "" : Globals.GetinDS_String(ds, i, "Content");
                   // item.Content = Globals.GetinDS_String(ds, i, "Content");
                    //item.Update();
                    lstClause.Add(item);
                }
            }
        }

        internal void UpdateKeyPhrases()
        {
            KeyPhrase key = new KeyPhrase();
            key.UpdateKeyPharease(this);
            foreach (Clause item in lstClause)
            {
                key.UpdateKeyPhrase(item, this.ChapterID, this.ChapterItemID);
            }
           
        }

        public int ManualInsertNew()
        {
            LuatProvider pd = new LuatProvider();
            return Globals.GetIDinDS(pd.ExecuteDataSet("exec ManualInsertNewArtical N'" + Name.Replace("'", "''") + "',N'" + Title.Replace("'", "''") + "',N'"+ArticalContent.Replace("'","''")+"'," + Number + "," + ChapterItemID+","+ChapterID+","  + LawID), 0, 0);

        }

        public int GetArticalNumber(string content)
        {
            content = content.TrimStart();
            if (content.StartsWith("Điều "))
            {
                string snumber = Globals.GetContentBetween(content, "Điều ", ".", false, true);
                if(snumber=="") snumber = Globals.GetContentBetween(content, "Điều ", ":", false, true);
                int number = 0;
                if (!int.TryParse(snumber, out number)) return -2;

                // check exists
                //if (Exists(number))
                //{
                //    return -1;
                //}
                return number;
            }
            return -1;
        }

        public void Update()
        {
            LuatProvider pd = new LuatProvider();
            pd.ExecuteDataSet("update Artical set Name = N'" + Name + "', Title = N'" + Title + "', Number = " + Number + " where ID = " + ID + " and lawID =" + LawID + " and chapterID = " + ChapterID +" and chapterItemID = "+ ChapterItemID);

        }

        public Artical CloneWithoutData()
        {
            Artical ci = new Artical
            {
                Name = this.Name,
                Title = this.Title,
                ID = this.ID,
                LawID = this.LawID,
                Number= this.Number,
                ChapterID = this.ChapterID,
                ChapterItemID = this.ChapterItemID,
                lstClause = new List<Clause>()
            };
            return ci;
        }

        internal void ConfirmedEdit()
        {
            LuatProvider pd = new LuatProvider();
            if (ID < 0)
                ID = Globals.GetIDinDS(pd.ExecuteDataSet("exec GetArtical N'" + Name + "', N'" + Title + "',N'" + ArticalContent + "'," + Number + "," + ChapterItemID + "," + ChapterID), 0, 0);
            foreach (var item in lstClause)
                item.ConfirmedEdit();
        }

        private bool Exists(int number)
        {
            LuatProvider pd = new LuatProvider();
            return Globals.GetIDinDS(pd.ExecuteDataSet("select top 1 1 from Artical with(nolock) where ChapterItemID = " + ChapterItemID + " and number = " + number), 0, 0) > 0;

        }

        public string IsValid()
        {
            return string.IsNullOrEmpty(Title) || Number <= 0 || string.IsNullOrEmpty(Name) ? "Tiêu đề / Số Điều không hợp lệ" : "";
        }

        public void Delete()
        {
            LuatProvider pd = new LuatProvider();
            pd.ExecuteDataSet("Delete from Artical where LawID = " + LawID + " and ID = " + ID);

        }
    }
}