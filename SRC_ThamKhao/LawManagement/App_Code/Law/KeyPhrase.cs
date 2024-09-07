using BDSLaw;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

/// <summary>
/// Summary description for KeyPhrase
/// </summary>
public class KeyPhrase
{

    public int ID { get; set; }
    public string Key { get; set; }
    public int NumberArtical { get; set; }
    public int Count { get; set; }
    public KeyPhraseSource Source { get; set; }
    public KeyPhrase()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static async Task<List<KeyPhrase>> GetKeyPharesFromText(string searchInput)
    {
        // data
        List<KeyPhrase> lst = GetKeyPhraseFromDB(searchInput);

        // get from db

      

        // get key from Web APi
        string[] keys = Globals.isUseKeyAPI() ? await GetKeyPhraseFromAPI(searchInput) : new string[] { };
        // update in db
        LuatProvider pd = new LuatProvider();
        List<string> lsNewKeys = keys.ToList().Where(x => !lst.Exists(y => y.Key.ToLower() == x.ToLower())).ToList();
        for (int i = 0; i < lsNewKeys.Count; i++)
        {
            if (!lst.Exists(x => string.Compare( x.Key ,lsNewKeys[i], true) == 0))
            {
                int keyID = Globals.GetIDinDS(pd.GenerateKeyPhrase(lsNewKeys[i]), 0, 0);
                if (keyID > 0)
                    lst.Add(new KeyPhrase { ID = keyID, Source = KeyPhraseSource.Search, Key = lsNewKeys[i] });
            }
        }
 
        return lst;
    }

    public static void DeletekeyPhrase(string key)
    {
        throw new NotImplementedException();
    }

    public static List<KeyPhrase> GetKeyPhraseFromDB(string searchInput)
    {
        List<KeyPhrase> lst = new List<KeyPhrase>();
        LuatProvider pd = new LuatProvider();
        String sKey = Globals.GetKeyJoin(searchInput);
        DataSet ds = pd.ExecuteDataSet("exec GetKeyPharesFromText N'" + sKey + "'");  
        if (ds != null && ds.Tables.Count > 0)
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                lst.Add(new KeyPhrase { ID = Globals.GetIDinDS(ds, i, "ID"),
                    Key = Globals.GetinDS_String(ds, i, "KeyPhrase"), 
                    Source = (KeyPhraseSource)(Globals.GetIDinDS(ds, i, "source")),
                    NumberArtical = Globals.GetIDinDS(ds, i, "NumberArtical")
                });
            }
        }
        return lst;
    }

    public async void UpdateKeyPharease(Artical artical)
    {
       
        if (string.IsNullOrEmpty(artical.ArticalContent))
        {
            LuatProvider pd = new LuatProvider();
            artical.ArticalContent = Globals.GetinDS_String(pd.ExecuteDataSet("select top 1 content from Artical with(nolock) where ID = "+ artical.ID), 0, "content");
        }
        List<KeyPhrase> lstKeys_DB = await GetKeyPharesFromText(artical.ArticalContent);      
        artical.UpdateKeyPharease(lstKeys_DB.Select(x=>x.Key).ToArray());
    }



    public async void UpdateKeyPhrase(Clause cls, int ChapterID, int ChapterItemID)
    {
       
        if (string.IsNullOrEmpty(cls.Content))
        {
            LuatProvider pd = new LuatProvider();
            cls.Content = Globals.GetinDS_String(pd.ExecuteDataSet("select top 1 content from Claust with(nolock) where ID = " + cls.ID), 0, "content");
        }
        List<KeyPhrase> lstKeys_DB = await GetKeyPharesFromText(cls.Content);
        cls.UpdateKeyPharease(lstKeys_DB.Select(x => x.Key).ToArray(), ChapterID, ChapterItemID);
    }

    public static async Task<string[]> GetKeyPhraseFromAPI(string postData)
    {
        var httpClient = new HttpClient();
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri(Globals.KeyPhareAPIURL);



        request.Content = new StringContent(postData);

        request.Method = HttpMethod.Post;
        var response = httpClient.SendAsync(request).Result;
        response.EnsureSuccessStatusCode();

        var buffer = await response.Content.ReadAsByteArrayAsync();
        var byteArray = buffer.ToArray();
        var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

        //JavaScriptSerializer serializer = new JavaScriptSerializer();
        // string[] rddata = serializer.Deserialize<string[]>(responseString);
        return responseString.Split(' ').ToList().Where(x => x.Contains("_")).Select(x => x.Replace("\"", "")).ToArray();

        //using (var sr = new StreamReader(response.GetResponseStream()))
        //{

        //    text = sr.ReadToEnd();
        //    string text2 = Globals.DecodeFromUtf8(text);


        //}
    }

    public static void ResetKeyPhrase(int lawID)
    {
        LuatProvider pd = new LuatProvider();


        LawDoc law = new LawDoc();
      //  law.Load(lawID);
       // law.UpdateKeyPhrases();

       // pd.ExecuteDataSet("exec UpdateKeyWeight");
        // update vector
        DataSet ds1 = pd.ExecuteDataSet("select *  from Artical where LawID = " + lawID + "order by id");
      //  DataSet ds2 = pd.ExecuteDataSet("select *  from KeyPhrase where LawID = " + lawID + "order by id");
        int articalID;
        List<string> lstWeights;
        for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
        {
            articalID = Globals.GetIDinDS(ds1, i, "ID");

            DataSet ds = pd.ExecuteDataSet("exec GetVector " + articalID);
          
            lstWeights = new List<string>();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                lstWeights.Add(r["weight"].ToString());
            }
            string svector = string.Join(";", lstWeights);
            pd.ExecuteDataSet("exec SetVector " + articalID + ",'" + svector + "',"+lawID);
        }
    }
}

public enum KeyPhraseSource
{
    Auto = 0,
    Manual = 1,
    Search = 2
}