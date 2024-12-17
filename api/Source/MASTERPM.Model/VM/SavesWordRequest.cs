using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MASTERPM.Model.VM
{
    public class SavesWordRequest
    {
        public SavesWordRequest() { }

        public System.Guid LuatUUID { get; set; }
        public string Content { get; set; }
        public string ContentHTML { get; set; }
        public string LawDate { get; set; }
        public string LawNumber { get; set; }
        public int Status { get; set; }
        public int TotalChapter { get; set; }
        public List<DATA_2_Chuong> Chapters { get; set; }
        public List<DATA_3_Muc> ChapterItems { get; set; }
        public List<DATA_4_Dieu> Articals { get; set; }
        public List<DATA_5_Khoan> Clausts { get; set; }
        public List<DATA_6_Diem> Points { get; set; }

        public SavesWordRequest(System.Guid luatUUID, List<DATA_2_Chuong> chapters, List<DATA_3_Muc> chapterItems, List<DATA_4_Dieu> articals, List<DATA_5_Khoan> clausts, List<DATA_6_Diem> points)
        {
            LuatUUID = luatUUID;
            Chapters = chapters ?? new List<DATA_2_Chuong>();
            ChapterItems = chapterItems ?? new List<DATA_3_Muc>();
            Articals = articals ?? new List<DATA_4_Dieu>();
            Clausts = clausts ?? new List<DATA_5_Khoan>();
            Points = points ?? new List<DATA_6_Diem>();
        }

        public override string ToString()
        {
            return $"SavesWordRequest [LuatID={LuatUUID}, Chapters={Chapters?.Count}, ChapterItems={ChapterItems?.Count}, Articals={Articals?.Count}, Clausts={Clausts?.Count}, Points={Points?.Count}]";
        }
    }
}
