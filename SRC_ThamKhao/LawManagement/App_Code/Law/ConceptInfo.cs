using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Concept
/// </summary>
/// 
namespace BDSLaw
{
	public class ConceptInfo

	{
        public List<KeyPhrase> lstKeyPhrases;

        public ConceptInfo()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        public int ID { get;  set; }
        public string Name { get;  set; }
        public string Content { get;  set; }
        public int type { get; set; }
        public double[] vector;
        public double distance;

        public static List<ConceptInfo> GetListConcept()
        {            
            return ListConCeptFromSQL("select * from [Concept]  with(nolock)");           
        }

        private static List<ConceptInfo> ListConCeptFromSQL(string sql)
        {
            LuatProvider pd = new LuatProvider();

            DataSet ds = pd.ExecuteDataSet(sql);
            List<ConceptInfo> lst = new List<ConceptInfo>();
            for (int i = 0; i < Globals.DSCount(ds); i++)
            {
                lst.Add(new ConceptInfo
                {
                    ID = Globals.GetIDinDS(ds, i, "ID"),
                    Name = Globals.GetinDS_String(ds, i, "Name"),
                    Content = Globals.GetinDS_String(ds, i, "Description"),
                });
            }
            return lst;
        }

      

        public static string GetConceptContentByID(int ID)
        {
            LuatProvider pd = new LuatProvider();

            DataSet ds = pd.ExecuteDataSet("select Description from [Concept]  with(nolock) where ID = " + ID);
            return Globals.GetinDS_String(ds, 0, "Description");
        }

        public void ChangeConcept()
        {
            LuatProvider pd = new LuatProvider();
            if (type == 1)
            {
                pd.ExecuteDataSet(string.Format("Exec GetConcept N'{0}',N'{1}'", Name, Content));
            }
            else if (type == 2)
            {
                pd.ExecuteDataSet(string.Format("update concept set name = N'{0}',Description =  N'{1}' where ID = {2}", Name, Content, ID));
            }
            else if (type == 3)
            {
                pd.ExecuteDataSet(string.Format("delete from concept  where ID = {0}", ID));
            }
        }

        internal static List<KeyPhrase> GetKeyPhrasesByConceptID(int conceptID)
        {
            List<KeyPhrase> lst = new List<KeyPhrase>(); LuatProvider pd = new LuatProvider();
            DataSet ds = pd.ExecuteDataSet("exec GetKeyPharesByConceptID " + conceptID);
            string key = "";
            for (int i = 0; i < Globals.DSCount(ds); i++)
            {
                key = Globals.GetinDS_String(ds, i, "KeyPhrase");
                lst.Add(new KeyPhrase
                {
                    ID = Globals.GetIDinDS(ds, i, "ID"),
                    Key = key,
                    Count = Globals.GetIDinDS(ds, i, "Count"),
                });
            }
            return lst;
        }

       
  

    }
}