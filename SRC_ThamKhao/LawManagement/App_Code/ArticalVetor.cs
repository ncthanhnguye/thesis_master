using BDSLaw;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class ArticalVetor
{
    public static List<ArticalVetor> _lstArticalVetors;
    public List<double> vector;

    public ArticalVetor() { }
    public ArticalVetor(ArticalVetor artical, double distance)
    {
        this.ArticalID = artical.ArticalID;
        this.distance = distance;
        this.LawID = artical.LawID;
        this.vector = artical.vector;
    }

    public int LawID { get; private set; }
    public int ArticalID { get; private set; }
    public double distance { get; set; }

    internal static List<ArticalVetor> LoadAllArtical()
    {
        if (_lstArticalVetors == null)
        {
            _lstArticalVetors = new List<ArticalVetor>();
            LuatProvider pd = new LuatProvider();
            DataSet ds = pd.ExecuteDataSet("exec LoadArticalVetor");
            for (int i = 0; i < Globals.DSCount(ds); i++)
            {
                _lstArticalVetors.Add(new ArticalVetor
                {
                    LawID = Globals.GetIDinDS(ds, i, "lawID"),
                    ArticalID = Globals.GetIDinDS(ds, i, "ArticalID"),
                    vector = ParseVector(Globals.GetinDS_String(ds, i, "vector"))
                });
            }
        }
        return _lstArticalVetors.ToList();
    }

    private static List<double> ParseVector(string v)
    {
        double v1 = 0;
        return v.Split(';').Select(x => double.TryParse(x, out v1) ? v1 : 0).ToList();
    }

  
}
