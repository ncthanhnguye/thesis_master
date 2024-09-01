using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for KeyPhrasConceptResult
/// </summary>
public class KeyPhraseRelateDetails
{
    public KeyPhraseRelateDetails()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public int ID { get; set; }
    public string KeyPhrase { get; set; }
    public string ChapterName { get; set; }
    public string ArticalName { get; set; }
    public int NumCount { get; set; }
}