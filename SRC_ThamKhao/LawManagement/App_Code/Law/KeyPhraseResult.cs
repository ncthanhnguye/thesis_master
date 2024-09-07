using System.Collections.Generic;
using BDSLaw;

public class KeyPhraseResult
{
    public List<KeyPhrase> keys;


    public int ID { get; set; }
    public double distance { get;  set; }
    public double[] vector { get;  set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public KeyPhraseResult() { keys = new List<KeyPhrase>(); }
}