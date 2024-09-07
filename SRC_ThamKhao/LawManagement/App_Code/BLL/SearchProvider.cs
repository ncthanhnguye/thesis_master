using BDSLaw;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for SearchProvider
/// </summary>
public class SearchProvider : LuatProvider
{
    public SearchProvider()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public SearchArticalResult SearchArtical(string searchInput)
    {
        SearchArticalResult rs = new SearchArticalResult();
        List<KeyPhrase> lstKeyPhrases_Searched = KeyPhrase.GetKeyPhraseFromDB(searchInput);
        foreach (var item in lstKeyPhrases_Searched)
        {
            item.Count = Globals.CountTerm(searchInput, item.Key.Replace("_", " "));
        }
        rs.KeyPhrases = lstKeyPhrases_Searched;

        List<ConceptInfo> lstAllConcept = ConceptInfo.GetListConcept();
        List<KeyPhraseResult> lstCandidate_Concepts = new List<KeyPhraseResult>();
        for (int i = 0; i < lstAllConcept.Count; i++)
        {
            lstCandidate_Concepts.Add(new KeyPhraseResult { ID = lstAllConcept[i].ID, keys = ConceptInfo.GetKeyPhrasesByConceptID(lstAllConcept[i].ID) });
        }
        int itopConcept = int.Parse(Globals.GetConfig("TopConceptSearch", "3"));
        int itopArticals = int.Parse(Globals.GetConfig("TopArticalSearch", "100"));
        double minScoreConcept = double.Parse(Globals.GetConfig("minScoreConcept", "0.01"));
        double minScoreArtical = double.Parse(Globals.GetConfig("TopArticalSearch", "0.01"));
        rs.lstConcepts = TFIDFMeasure.FindNearestNeighbors(lstCandidate_Concepts, lstKeyPhrases_Searched, minScoreConcept, itopConcept);

        List<KeyPhraseResult> lstCandidates = GetListArticalByConceptID(rs.lstConcepts.Select(x => x.ID).ToList());

        rs.lstArticals = TFIDFMeasure.FindNearestNeighbors(lstCandidates, lstKeyPhrases_Searched, minScoreArtical, itopArticals);

        //List<List<float>> lstvectors = new List<List<float>>();
        //List<double> inputVector = new List<double>();
        //int totalArtical = Globals.GetIDinDS(ExecuteDataSet("select count(*) from artical with(nolock) "), 0, 0);
        //DataSet dsKey_DB = ExecuteDataSet("select *  from KeyPhrase order by id");
        //for (int i = 0; i < Globals.DSCount(dsKey_DB); i++)
        //{
        //    int keyphraseDB_ID = Globals.GetIDinDS(dsKey_DB, i, "ID");
        //    KeyPhrase key = lstKeyPhrases_Searched.Where(x => x.ID == keyphraseDB_ID).FirstOrDefault();
        //    if (key != null)
        //    {
        //        double tf = (1+ Math.Log( (double) Globals.CountTerm(searchInput, key.Key.Replace("_", " ")) ))/
        //            Math.Log( (double)lstKeyPhrases_Searched.Count);
        //        double idf = Math.Log( 1+ ((double)totalArtical / (double)key.NumberArtical));
        //        inputVector.Add(tf * idf);
        //    }
        //    else
        //        inputVector.Add(0);
        //}
        //rs.lstArtical = FindNearestNeighbor(inputVector, -1);
        return rs;
        // distincae cosimn

        //DataSet ds = ExecuteDataSet("exec GetQuestionType N'" + searchInput.Replace("'", "''") + "'");
        //string QuestionType = Globals.GetinDS_String(ds, 0, 0);
        //// List<KeyPhrase> lstKeyPhrases = await KeyPhrase.GetKeyPharesFromText(searchInput);
        //ds = ExecuteDataSet("exec Search N'" + Globals.GetKeyJoin(searchInput) + "'");

        //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //{
        //  //  lst.Add(Globals.GetIDinDS(ds, i, 0));
        //}
        //return null; ;
    }

    public List<KeyPhraseResult> GetListArticalByConceptID(List<int> lst)
    {
        List<KeyPhraseResult> lstResult = new List<KeyPhraseResult>();
        DataSet ds = ExecuteDataSet("Exec GetListArticalKeyPhraseByConceptID '" + string.Join(",", lst) + "'");
        SortedDictionary<int, List<KeyPhrase>> dic = new SortedDictionary<int, List<KeyPhrase>>();
        int articalID = 0; KeyPhrase ph;
        for (int i = 0; i < Globals.DSCount(ds); i++)
        {
            articalID = Globals.GetIDinDS(ds, i, "ArticalID");
            ph = new KeyPhrase
            {
                ID = Globals.GetIDinDS(ds, i, "KeyPhraseID"),
                Key = Globals.GetinDS_String(ds, i, "KeyPhrase"),
                Count = Math.Max(1, Globals.GetIDinDS(ds, i, "NumCount")),
            };
            if (!dic.ContainsKey(articalID))
            {
                dic.Add(articalID, new List<KeyPhrase>
                {
                  ph
                });

            }
            else
                dic[articalID].Add(ph);

        }
        foreach (var item in dic.Keys)
        {
            lstResult.Add(new KeyPhraseResult { ID = item, keys = dic[item] });
        }
        return lstResult;
    }

    private List<int> FindNearestNeighbor(List<double> inputVector, double minDistance = 0)
    {
        List<ArticalVetor> lst = ArticalVetor.LoadAllArtical();

        List<ArticalVetor> lstResult = new List<ArticalVetor>();
        double distance = 0;
        foreach (ArticalVetor artical in lst)
        {
            distance = GetDistanceCosin(artical.vector, inputVector);
            if (distance > minDistance)
                lstResult.Add(new ArticalVetor(artical, distance));
        }
        lstResult.Sort((x, y) => y.distance.CompareTo(x.distance));
        return lstResult.Select(x => x.ArticalID).ToList();
    }

    private double GetDistanceCosin(List<double> V1, List<double> V2)
    {
        int N = 0;
        N = ((V2.Count < V1.Count) ? V2.Count : V1.Count);
        double dot = 0.0d;
        double mag1 = 0.0d;
        double mag2 = 0.0d;
        for (int n = 0; n < N; n++)
        {
            dot += V1[n] * V2[n];
            mag1 += Math.Pow(V1[n], 2);
            mag2 += Math.Pow(V2[n], 2);
        }

        return dot / (Math.Sqrt(mag1) * Math.Sqrt(mag2));
    }
    public LawDoc ViewArticalDetail(List<int> lstArticalID, int LawID)
    {
        List<LawDoc> lst = ViewArticalDetails(lstArticalID);
        return lst.Where(x => x.ID == LawID).FirstOrDefault();
    }

    public List<LawDoc> ViewArticalDetails(List<int> lstArticalID)
    {
        LawDoc law = null;
        List<LawDoc> lst = new List<LawDoc>();
        Dictionary<int, LawDoc> dicLaws = Globals.GetAllLaws();

        Chapter mychapter = null; ChapterItem mychapterItem = null;
        for (int idx = 0; idx < lstArticalID.Count; idx++)
        {
            var articalID = lstArticalID[idx];

            DataSet ds = ExecuteDataSet("exec [GetArticalDetail] " + articalID);
            int _LawID = Globals.GetIDinDS(ds, 0, "LawID");
            int ChapterID = Globals.GetIDinDS(ds, 0, "ChapterID");
            int ChapterItemID = Globals.GetIDinDS(ds, 0, "ChapterItemID");


            foreach (int LawID in dicLaws.Keys)
            {
                if (LawID != _LawID) continue;
                law = lst.Where(x => x.ID == LawID).FirstOrDefault();
                if (law == null)
                {
                    law = new LawDoc { ID = LawID, Name = dicLaws[LawID].Name, lstChapters = new List<Chapter>() };
                    lst.Add(law);
                }
                foreach (Chapter ch in dicLaws[LawID].lstChapters)
                {
                    if (ch.ID != ChapterID) continue;
                    mychapter = law.lstChapters.Where(x => x.ID == ChapterID).FirstOrDefault();
                    if (mychapter == null)
                    {
                        mychapter = ch.CloneWithoutData();
                        law.lstChapters.Add(mychapter);
                    }

                    foreach (var chi in ch.lstChapterItem)
                    {
                        if (chi.ID != ChapterItemID) continue;
                        mychapterItem = mychapter.lstChapterItem.Where(x => x.ID == ChapterItemID).FirstOrDefault();
                        if (mychapterItem == null)
                        {
                            mychapterItem = chi.CloneWithoutData();
                            mychapter.lstChapterItem = new List<ChapterItem>() { mychapterItem };
                        }

                        foreach (var artical in chi.lstArtical)
                        {
                            if (artical.ID != articalID) continue;                         
                            mychapterItem.lstArtical.Add(artical);
                        }
                    }
                }
            }

            //LawDoc law2 = Globals.GetLaw(_LawID, ChapterID, ChapterItemID, articalID);
            //if (lawTotal == null)
            //    lawTotal = law2;
            //else
            //    Globals.MergeLaw1IntoLaw2(law, lawTotal);
        }
   
        return lst;
    }

    public LawDoc ViewChapterDetail(int chapterID)
    {
        int LawID = Globals.GetIDinDS(ExecuteDataSet("select top 1 LawID from chapter with(nolock) where ID = " + chapterID), 0, 0);
        LawDoc mylaw = Globals.GetLaw(LawID);
        LawDoc returnLAW = new LawDoc { ID = LawID, Name = mylaw.Name };
        Chapter mychapter = mylaw.lstChapters.Where(x => x.ID == chapterID).FirstOrDefault();
        returnLAW.lstChapters = new List<Chapter>() { mylaw.lstChapters.Where(x => x.ID == chapterID).FirstOrDefault() };
        return returnLAW;
    }
    public KeyPhraseConceptResult GetKeyPhraseLawResult(int _conceptID)
    {
        KeyPhraseConceptResult rs = new KeyPhraseConceptResult
        {
            conceptID = _conceptID
        };

        LuatProvider pd = new LuatProvider();
        rs.Content = ConceptInfo.GetConceptContentByID(_conceptID);
        rs.keys = ConceptInfo.GetKeyPhrasesByConceptID(_conceptID);


        rs.Content = Globals.HighlightKeyphrase(rs.Content, rs.keys);
        return rs;
    }
    public KeyPhraseLawResult GetKeyPhraseLawResult(List<int> lstArticalID, int LawID)
    {
        KeyPhraseLawResult rs = new KeyPhraseLawResult
        {
            law = ViewArticalDetail(lstArticalID, LawID)
        };
        rs.keys = new List<KeyPhrase>();
        LuatProvider pd = new LuatProvider();
        DataSet ds = pd.ExecuteDataSet("exec GetKeyPharesByArticalIDs '" + string.Join(",", lstArticalID.ToArray()) + "'");
        List<string> lstkeys_title = new List<string>();
        SortedDictionary<string, int> dic = new SortedDictionary<string, int>(); string key;
        for (int i = 0; i < Globals.DSCount(ds); i++)
        {
            key = Globals.GetinDS_String(ds, i, "KeyPhrase");
            if (!dic.ContainsKey(key))
            {
                rs.keys.Add(new KeyPhrase
                {
                    ID = Globals.GetIDinDS(ds, i, "ID"),
                    Key = key,
                    NumberArtical = Globals.GetIDinDS(ds, i, "total"),
                });
                dic.Add(key, 0);
            }
            if (Globals.GetIDinDS(ds, 0, "type") > 0)
                lstkeys_title.Add(key.Replace("_", " "));
        }
        // replace content highlight


        List<string> lstAllkeys = dic.Keys.ToList();
        for (int i = 0; i < rs.law.lstChapters.Count; i++)
        {
            Chapter chapter = rs.law.lstChapters[i];
            chapter.Title = Globals.HighlightKeyphrase(chapter.Title, rs.keys);
            for (int j = 0; j < chapter.lstChapterItem.Count; j++)
            {
                ChapterItem item = chapter.lstChapterItem[j];
                item.Title = Globals.HighlightKeyphrase(item.Title, rs.keys);
                for (int k = 0; k < item.lstArtical.Count; k++)
                {
                    Artical artical = item.lstArtical[k];
                    artical.Title = Globals.HighlightKeyphrase(artical.Title, rs.keys);
                    artical.ArticalContent = Globals.HighlightKeyphrase(artical.ArticalContent, rs.keys);
                    for (int z = 0; z < artical.lstClause.Count; z++)
                    {
                        Clause clause = artical.lstClause[z];
                        clause.Title = Globals.HighlightKeyphrase(clause.Title, rs.keys);
                        clause.Content = Globals.HighlightKeyphrase(clause.Content, rs.keys);
                        for (int n = 0; n < clause.lstPoints.Count; n++)
                        {
                            LawPoint point = clause.lstPoints[n];
                            point.Content = Globals.HighlightKeyphrase(point.Content, rs.keys);
                        }

                    }
                }
            }
        }
        return rs;
    }

    public void DeletekeyphraseMapping(List<int> lstArticalID, List<string> lstkeyDelete)
    {
        string sql = "exec DeleteKeyMappingByArticalIDs '" + string.Join(",", lstArticalID.ToArray()) + "',N'" +
            string.Join("','", lstkeyDelete.Select(x => x.Replace("'", "''")).ToArray())
            + "'";
        ExecuteDataSet(sql);
    }

    public void DeletekeyphraseMapping(int conceptID, List<string> lstkeyDelete)
    {
        string sql = "exec DeleteKeyMappingByConceptID " + conceptID + ",N'" +
        string.Join("','", lstkeyDelete.Select(x => x.Replace("'", "''")).ToArray())
        + "'";
        ExecuteDataSet(sql);
    }
}