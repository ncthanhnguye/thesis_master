using System;
using System.Collections.Generic;

namespace BDSLaw
{
    public class LawPoint
    {
        public int ID { get; set; }
        public int ClauseID { get; set; }
        public string Content { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }


        internal bool ParseContent(bool isGenerateNewID, string clauseContent, int number, int ClauseID)
        {
            try
            {
                this.ClauseID = ClauseID;
                this.Number = number;
                string Name = GetPointName(number);
                if (Name == "") return false;
                string pre = "\n";
                //if (string.IsNullOrEmpty(Content))
                {
                    Content = Globals.NextItem(clauseContent, pre + Name);
                    Content = Globals.GetContentBetween(Content, pre + Name, pre + GetPointName(number + 1), true, true);
                }
                if (Content == "") return false;


                LuatProvider pd = new LuatProvider();
                ID = !isGenerateNewID ? -1 : Globals.GetIDinDS(pd.ExecuteDataSet("exec GetPoint N'" + Content + "', N'" + Name + "'," + number + "," + ClauseID), 0, 0);

                if (ID > 0 || !isGenerateNewID)
                {


                    return true;
                }
            }
            catch (System.Exception ex)
            {

                Globals.WriteLog(ex);
            }

            return false;
        }

        public static string GetPointName(int number)
        {
            string[] arr = new string[] { "", "a)", "b)", "c)", "d)", "đ)", "e)", "g)", "h)", "i)", "k)", "l)", "m)", "n)", "o)", "p)", "q)", "r)", "s)", "t)", "u)", "v)", "x)", "y)" };
            return arr.Length > number ? arr[number] : "";
        }
        public static int GetPointnumber(string v)
        {
            List<string> arr = new List<string>() { "", "a)", "b)", "c)", "d)", "đ)", "e)", "g)", "h)", "i)", "k)", "l)", "m)", "n)", "o)", "p)", "q)", "r)", "s)", "t)", "u)", "v)", "x)", "y)" };
            return arr.Contains(v) ? arr.IndexOf(v) : -1;
        }

        internal void ConfirmedEdit()
        {
            LuatProvider pd = new LuatProvider();
            if (ID < 0)
                ID = Globals.GetIDinDS(pd.ExecuteDataSet("exec GetPoint N'" + Content + "', N'" + Name + "'," + Number + "," + ClauseID), 0, 0);
        }
    }
}