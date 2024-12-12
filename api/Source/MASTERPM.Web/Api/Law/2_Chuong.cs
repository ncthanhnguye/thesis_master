using System;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;
using MASTERPM.Model;
using MASTERPM.Model.Core;
using MASTERPM.Web.Api.Base;

namespace MASTERPM.Web.Models.Claust
{
    [RoutePrefix("api/Chapter")]
    public class ChapterController : BaseController
    {
        [HttpGet]
        [Route("GetChapter")]
        public IHttpActionResult GetChapter()
        {
            try
            {

                var result = this.Repository.GetQuery<DATA_2_Chuong>()
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
                var allLaw = LawID == null;
                var allChapterID = ChapterID == null;
                var allChapterItemID = ChapterItemID == null;
                var allArticalID = ArticalID == null;
                var allClaustID = ClaustID == null;
                var allPointID = PointID == null;

                var result = this.Repository.GetQuery<DATA_1_Luat>().Where(r => (allLaw || r.ID == LawID))
                     .Join(this.Repository.GetQuery<DATA_2_Chuong>().Where(r => (allChapterID || r.ID == ChapterID)),
                        a => a.ID, b => b.LuatID, (a, b) => new
                        {
                            DATA_1_Luat = a,
                            DATA_2_Chuong = b
                        })
                     .Select(r => new
                     {
                         r.DATA_1_Luat , 
                         r.DATA_2_Chuong
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
                var dataItem = this.Repository.GetQuery<DATA_2_Chuong>()
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
        public IHttpActionResult Post([FromBody] DATA_2_Chuong dataRequest)
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
        public IHttpActionResult Put(int iD, [FromBody] DATA_2_Chuong dataRequest)
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
            
                var dataItem = this.Repository.GetQuery<DATA_2_Chuong>().FirstOrDefault(r => iD == r.ID);
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
