using System;
using System.Collections.Generic;
using System.Data;

namespace BDSLaw
{
    public class Clause
    {
       

        public Clause()
        {
            lstPoints = new List<LawPoint>();
        }

        public Clause(SelectedLawItemType selectedType)
        {
            ArticalID = selectedType.ArticalID;
            Number = selectedType.Number;
            Title = selectedType.Title;
            Content = selectedType.content;
            ID = selectedType.ClauseID;
        }

        public int ID { get; set; }
        public int ArticalID { get; set; }
        public int Number { get; set; }
        public string Content { get; set; }

        public string Title { get; set; }
        public System.Collections.Generic.List<LawPoint> lstPoints { get; set; }
        public bool ParseContent(bool isGenerateNewID, string articalContent, int number, int ArticalID)
        {
            try
            {
                this.ArticalID = ArticalID;
                this.Number = number;
                string Name = number + ".";
               // if (string.IsNullOrEmpty(Content))
                {
                    Content = Globals.NextItem(articalContent, "\n" + Name);
                    Content = Globals.GetContentBetween(Content, "\n" + Name, (number + 1) + ".", true, true);
                }
                if (Content == "") return false;
                bool hasLawPoint = Content.IndexOf("a) ") > 0 && Content.IndexOf("b) ") > 0;

                Title = hasLawPoint ? Globals.GetContentBetween(Content, "\n" + Name, "a)", true, true) : "";
                LuatProvider pd = new LuatProvider();
                ID = !isGenerateNewID ? -1 : Globals.GetIDinDS(pd.ExecuteDataSet("exec GetClaust N'" + Content + "', N'" + Title + "'," + number + "," + ArticalID), 0, 0);

                if (ID > 0 || !isGenerateNewID)
                {
                    lstPoints = new List<LawPoint>();
                    string TrimLawPointContent = Content.Substring(Math.Min(Content.Length - 1, Content.IndexOf(Name + ".") + Name.Length + 1));
                    if (hasLawPoint)
                    {
                        LawPoint item = new LawPoint();
                        int LawPointNumber = 1;
                        while (item.ParseContent(isGenerateNewID, TrimLawPointContent, LawPointNumber++, ID))
                        {
                            lstPoints.Add(item); item = new LawPoint();
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


        internal void UpdateKeyPharease(string[] keys, int ChapterID, int ChapterItemID)
        {
            LuatProvider pd = new LuatProvider(); int keyID = 0;
            foreach (var key in keys)
            {
                keyID = Globals.GetIDinDS(pd.GenerateKeyPhrase(key), 0, 0);
                if (keyID > 0)
                {
                    int Count = Globals.CountTerm(Content, key.Replace("_", " "));
                    pd.ExecuteDataSet("exec UpdateKeyPhraseByClause " + keyID + "," + ArticalID + "," + ChapterID + "," + ChapterItemID + "," + ID + "," + Count);
                }
            }
        }

        internal void LoadPoint(bool isLoadContent = false)
        {
            LuatProvider pd = new LuatProvider();
            DataSet ds = pd.ExecuteDataSet("select ID, [ClauseID], number, Name, Content  from [Point] where ClauseID = " + ID + " order by Number");
            lstPoints = new List<LawPoint>();
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    LawPoint item = new LawPoint();
                    item.ID = Globals.GetIDinDS(ds, i, 0);
                    item.ClauseID = Globals.GetIDinDS(ds, i, 1);
                    item.Number = Globals.GetIDinDS(ds, i, 2);
                    item.Name = Globals.GetinDS_String(ds, i, "Name");
                    item.Content = isLoadContent ? Globals.GetinDS_String(ds, i, "Content") : "";
                    lstPoints.Add(item);
                }
            }
        }
        public void UpdateManual()
        {
            LuatProvider pd = new LuatProvider();
            pd.ExecuteDataSet("update claust set  Title = N'" + Title + "', Number = " + Number + " where ID = " + ID + "  and ArticalID = " + ArticalID);

        }
        internal void Update()
        {


            string Name = Number + ".";

            bool hasLawPoint = Content.IndexOf("a) ") > 0 && Content.IndexOf("b) ") > 0;

            Title = hasLawPoint ? Globals.GetContentBetween(Content, Name, "a)", true, true) : "";
            if (Title != "")
            {
                LuatProvider pd = new LuatProvider();
                ID = Globals.GetIDinDS(pd.ExecuteDataSet("exec GetClaust N'" + Content + "', N'" + Title + "'," + Number + "," + ArticalID), 0, 0);

            }

        }

        public int ManualInsertNew()
        {
            LuatProvider pd = new LuatProvider();
            int code = Globals.GetIDinDS(pd.ExecuteDataSet("exec ManualInsertNewClause N'" + Title.Replace("'", "''") + "',N'" + Content.Replace("'", "''") + "'," + Number + "," + ArticalID), 0, 0);
            if (code < 0) return code;
            ParsePoint();
            return code;

        }
        internal bool ParsePoint()
        {
            try
            {
              
                string Name = this.Number + ".";
                bool hasLawPoint = Content.IndexOf("a) ") > 0 && Content.IndexOf("b) ") > 0;
                LuatProvider pd = new LuatProvider();
                ID =  Globals.GetIDinDS(pd.ExecuteDataSet("exec GetClaust N'" + Content + "', N'" + Title + "'," + Number + "," + ArticalID), 0, 0);

                if (ID > 0 )
                {
                    lstPoints = new List<LawPoint>();
                    string TrimLawPointContent = Content.Substring(Math.Min(Content.Length - 1, Content.IndexOf(Name + ".") + Name.Length + 1));
                    if (hasLawPoint)
                    {
                        LawPoint item = new LawPoint();
                        int LawPointNumber = 1;
                        while (item.ParseContent(true, TrimLawPointContent, LawPointNumber++, ID))
                        {
                            lstPoints.Add(item); item = new LawPoint();
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

    

        public int GetChapterClauseNumber(string content)
        {
            content = content.TrimStart();
            if(!content.Contains("."))
                return -1;
            content = content.Substring(0, content.IndexOf(".") + 1);
            if (content.Contains("\r"))
                content.Substring(content.IndexOf("\r"));
            if (content.Contains("\n"))
                content.Substring(content.IndexOf("\n"));
            string snumber = Globals.GetContentBetween(content, "", ".", false, true);
            int number = 0;

            return int.TryParse(snumber, out number) ? number : -1;
        
        }

        internal void ConfirmedEdit()
        {
            LuatProvider pd = new LuatProvider();
            if (ID < 0)
                ID = Globals.GetIDinDS(pd.ExecuteDataSet("exec GetClaust N'" + Content + "', N'" + Title + "'," + Number + "," + ArticalID), 0, 0);
            foreach (var item in lstPoints)
                item.ConfirmedEdit();
        }

        private bool Exists(int number)
        {
            LuatProvider pd = new LuatProvider();
            return Globals.GetIDinDS(pd.ExecuteDataSet("select top 1 1 from Cluase with(nolock) where ArticalID = " + ArticalID + " and number = " + number), 0, 0) > 0;

        }

        public string IsValid()
        {
            return Number <= 0  ? "Tiêu đề / Số Khoản không hợp lệ" : "";
        }

        public void Delete()
        {
            LuatProvider pd = new LuatProvider();
            pd.ExecuteDataSet("Delete from Claust where ArticalID = " + ArticalID + " and ID = " + ID);

        }
    }
}