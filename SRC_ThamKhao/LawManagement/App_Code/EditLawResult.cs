using System;
using System.Collections.Generic;
using System.Linq;
using BDSLaw;

public class EditLawResult
{
    public LawDoc law { get; set; }
    public int icode { get; set; }
    public Chapter chapter { get; set; }
    public ChapterItem chapterItem { get; set; }
    public Artical artical { get; set; }
    public Clause clause { get; set; }
    public string mess { get; set; }

    public void Failed()
    {
        icode = -1;
        mess = "Nội dung không hợp lệ";
    }
    public EditLawResult()
    {
        mess = "";
    }

    public void SetItem(SelectedLawItemType selectedType)
    {
        LawDoc mylaw = Globals.GetLaw(selectedType.LawID);
        law = new LawDoc { ID = selectedType.LawID, Name = mylaw.Name };
        Chapter mychapter = mylaw.lstChapters.Where(x => x.ID == selectedType.chapterID).FirstOrDefault();
        ChapterItem mychapterItem = null; Artical myArtical = null;
        if (mychapter != null)
        {
            law.lstChapters = new List<Chapter>() { mychapter.CloneWithoutData() };
            mychapterItem = mychapter == null ? null : mychapter.lstChapterItem.Where(x => x.ID == selectedType.chapteritemID).FirstOrDefault();
            if (mychapterItem != null)
            {
                law.lstChapters[0].lstChapterItem = new List<ChapterItem>() { mychapterItem.CloneWithoutData() };
                myArtical = mychapterItem == null ? null : mychapterItem.lstArtical.Where(x => x.ID == selectedType.ArticalID).FirstOrDefault();
                if (myArtical != null)
                    law.lstChapters[0].lstChapterItem[0].lstArtical = new List<Artical>() { myArtical.CloneWithoutData() };
            }
        }
    }
}