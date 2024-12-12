using System;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;
using MASTERPM.Model;
using MASTERPM.Model.Core;
using MASTERPM.Web.Api.Base;

namespace MASTERPM.Web.Models.Chapter
{
    [RoutePrefix("api/Artical")]
    public class ArticalController : BaseController
    {

        [HttpGet]
        [Route("GetArtical")]
        public IHttpActionResult GetArtical()
        {
            try
            {

                var result = this.Repository.GetQuery<DATA_4_Dieu>()
                .ToList();

                return Json(new TResult()
                {
                    Status = 1,
                    Data = result
                });
            }
            catch (Exception e)
            {
                throw new Exception("", e);
            }
        }



        [HttpGet]
        [Route("Search")]
        public IHttpActionResult Search(int? LawID, int? ChapterID, int? ChapterItemID, int? ArticalID, int? ClaustID, int? PointID)
        {
            try
            {
                var allLuat = LawID == null;
                var allChapterID = ChapterID == null;
                var allChapterItemID = ChapterItemID == null;
                var allArticalID = ArticalID == null;
                var allClaustID = ClaustID == null;
                var allPointID = PointID == null;

                var result = this.Repository.GetQuery<DATA_1_Luat>()
                     .Where(r => (allLuat || r.ID == LawID))
                     .Join(this.Repository.GetQuery<DATA_2_Chuong>().Where(r => (allChapterID || r.ID == ChapterID)),
                        a => a.ID, 
                        b => b.LuatID, 
                        (a, b) => new
                        {
                            DATA_1_Luat = a,
                            DATA_2_Chuong = b
                        }
                     )
                     .Join(
                         this.Repository.GetQuery<DATA_3_Muc>().Where(r => (allChapterItemID || r.ID == ChapterItemID)),
                         ab => ab.DATA_2_Chuong.ID,
                         c => c.ChuongID,
                         (ab, c) => new
                         {
                             ab.DATA_1_Luat,
                             ab.DATA_2_Chuong,
                             DATA_3_Muc = c
                         }
                     )
                     .Join(
                         this.Repository.GetQuery<DATA_4_Dieu>().Where(r => (allChapterItemID || r.ID == ChapterItemID)),
                         ab => ab.DATA_3_Muc.ID,
                         c => c.MucID,
                         (ab, c) => new
                         {
                             ab.DATA_1_Luat,
                             ab.DATA_2_Chuong,
                             ab.DATA_3_Muc,
                             DATA_4_Dieu = c
                         }
                     )
                     .Select(r => new
                     {
                         r.DATA_1_Luat,
                         r.DATA_2_Chuong,
                         r.DATA_3_Muc
                     })
                     .ToList();

                return Json(new TResult()
                {
                    Status = 1,
                    Data = result
                });
            }
            catch (Exception e)
            {
                throw new Exception("", e);
            }
        }

        [HttpGet]
        [Route("GetDetail")]
        public IHttpActionResult GetDetail(int iD)
        {
            try
            {
                var dataItem = this.Repository.GetQuery<DATA_4_Dieu>()
                    .FirstOrDefault(r => r.ID == iD);

                if (dataItem is null)
                {
                    return Json(new TResult()
                    {
                        Status = 0,
                        Msg = "Error"
                    });
                }

                return Json(new TResult()
                {
                    Status = 1,
                    Data = dataItem
                });
            }
            catch (Exception e)
            {
                throw new Exception("", e);
            }
        }

        [HttpPost]
        [Route("Post")]
        public IHttpActionResult Post([FromBody] DATA_4_Dieu dataRequest)
        {
            try
            {
               
                var dataItem = dataRequest.Clone();

                this.Repository.Add(dataItem);
                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = 1,
                    Msg = "Save Ok",
                    Data = dataItem.ID
                });
            }
            catch (Exception e)
            {
                throw new Exception("", e);
            }
        }

        [HttpPut]
        [Route("Put")]
        public IHttpActionResult Put(int iD, [FromBody] DATA_4_Dieu dataRequest)
        {
            try
            {
                var dataItem = dataRequest.Clone();
                this.Repository.Update(dataItem);

                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = 1,
                    Msg = "Save Ok",
                    Data = dataItem.ID
                });
            }
            catch (Exception e)
            {
                throw new Exception("", e);
            }
        }

        [HttpDelete]
        [Route("Delete")]
        public IHttpActionResult Delete(int iD)
        {
            try
            {
            
                var dataItem = this.Repository.GetQuery<DATA_4_Dieu>().FirstOrDefault(r => iD == r.ID);
                this.Repository.Delete(dataItem);
                this.Repository.UnitOfWork.SaveChanges();
                return Json(new TResult()
                {
                    Status = 1,
                    Msg = "Delete Ok"
                });
            }
            catch (Exception e)
            {
                throw new Exception("", e);
            }
        }

       
    }
}
