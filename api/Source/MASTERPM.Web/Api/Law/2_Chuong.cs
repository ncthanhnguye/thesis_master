using System;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;
using MASTERPM.Model;
using MASTERPM.Model.Commons;
using MASTERPM.Model.Core;
using MASTERPM.Web.Api.Base;
using Microsoft.Ajax.Utilities;

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
        public IHttpActionResult Search(string LuatUUID, int? ChapterID)
        {
            try
            {
                var allLaw = LuatUUID == null;
                var allChapterID = ChapterID == null;

                // Parse string to Guid
                Guid.TryParse(LuatUUID, out Guid parsedLuatUUID);
                
                if (string.IsNullOrWhiteSpace(LuatUUID))
                {                   
                    var result = this.Repository.GetQuery<DATA_1_Luat>().Where(r => (allLaw || r.LuatUUID == parsedLuatUUID))
                        .Join(this.Repository.GetQuery<DATA_2_Chuong>().Where(r => (allChapterID || r.ID == ChapterID)),
                        a => a.LuatUUID, b => b.LuatUUID, (a, b) =>
                        new
                        {
                            DATA_1_Luat = a,
                            DATA_2_Chuong = b,
                        })
                        .Select(r => new
                        {
                            r.DATA_1_Luat,
                            r.DATA_2_Chuong
                        }).ToList();

                    var allChapters = this.Repository.GetQuery<DATA_2_Chuong>()
                        .Where(chapter => ChapterID == null || chapter.ID == ChapterID)
                        .ToList();         

                    return Json(new TResult()
                    {
                        Status = 1,
                        Data = result
                    });

                }

                var resultChapterByLaw = this.Repository.GetQuery<DATA_1_Luat>()
                    .Where(r => (allLaw || r.LuatUUID == parsedLuatUUID))
                    .Join(this.Repository.GetQuery<DATA_2_Chuong>()
                        .Where(r => (allChapterID || r.ID == ChapterID)),
                        a => a.LuatUUID,
                        b => b.LuatUUID,
                        (a, b) => new
                        {
                            DATA_1_Luat = a,
                            DATA_2_Chuong = b,
                        })
                    .Select(r => new
                    {
                        r.DATA_1_Luat,
                        r.DATA_2_Chuong
                    })
                    .ToList();

                return Json(new TResult()
                {
                    Status = 1,
                    Data = resultChapterByLaw
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
                    Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgDeleteDataSuccess),
                });
            }
            catch (Exception e)
            {
                throw new Exception("", e);
            }
        }

       
    }
}
