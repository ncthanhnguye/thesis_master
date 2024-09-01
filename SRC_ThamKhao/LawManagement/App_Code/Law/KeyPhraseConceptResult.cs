using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for KeyPhrasConceptResult
/// </summary>
public class KeyPhraseConceptResult
{
    public KeyPhraseConceptResult()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public int conceptID { get;  set; }
    public List<KeyPhrase> keys { get;  set; }
    public string Content { get; internal set; }
}