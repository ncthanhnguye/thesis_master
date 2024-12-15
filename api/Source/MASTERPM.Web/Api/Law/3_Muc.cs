using System;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;
using MASTERPM.Model;
using MASTERPM.Model.Commons;
using MASTERPM.Model.Core;
using MASTERPM.Web.Api.Base;

namespace MASTERPM.Web.Models.Artical
{
    [RoutePrefix("api/ChapterItem")]
    public class ChapterItemController : BaseController
    {
        [HttpGet]
        [Route("GetChapterItem")]
        public IHttpActionResult GetChapterItem()
        {
            try
            {

                var result = this.Repository.GetQuery<DATA_3_Muc>()
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
        public IHttpActionResult Search(string LuatUUID, string ChuongUUID, int? ChapterItemID)
        {
            try
            {
                var allLaw = LuatUUID == null;
                var allChapterID = ChuongUUID == null;
                var allChapterItemID = ChapterItemID == null;

                Guid.TryParse(LuatUUID, out Guid parsedLuatUUID);
                Guid.TryParse(ChuongUUID, out Guid parsedChuongUUID);

                if (allLaw && allChapterID)
                {
                    var resultAll = this.Repository.GetQuery<DATA_1_Luat>()
                        .Join(this.Repository.GetQuery<DATA_2_Chuong>(),
                            law => law.LuatUUID,
                            chapter => chapter.LuatUUID,
                            (law, chapter) => new { law, chapter })
                        .Join(this.Repository.GetQuery<DATA_3_Muc>(),
                            combined => combined.chapter.ChuongUUID,
                            muc => muc.ChuongUUID,
                            (combined, muc) => new
                            {
                                Luat = new
                                {
                                    combined.law.ID,
                                    combined.law.Content,
                                    combined.law.LuatUUID
                                },
                                Chuong = new
                                {
                                    combined.chapter.ID,
                                    combined.chapter.Content,
                                    combined.chapter.ChuongUUID
                                },
                                Muc = new
                                {
                                    muc.ID,
                                    muc.Content,
                                    muc.MucUUID
                                },
                            })                       
                        .ToList();

                    return Json(new TResult()
                    {
                        Status = 1,
                        Data = resultAll
                    });
                }

                var resultFiltered = this.Repository.GetQuery<DATA_1_Luat>()
                    .Where(r => allLaw || r.LuatUUID == parsedLuatUUID)
                    .Join(this.Repository.GetQuery<DATA_2_Chuong>()
                        .Where(r => allChapterID || r.ChuongUUID == parsedChuongUUID),
                        law => law.LuatUUID,
                        chapter => chapter.LuatUUID,
                        (law, chapter) => new { law, chapter })
                    .Join(this.Repository.GetQuery<DATA_3_Muc>()
                        .Where(r => allChapterID || r.ChuongUUID == parsedChuongUUID),
                        combined => combined.chapter.ChuongUUID,
                        muc => muc.ChuongUUID,
                        (combined, muc) => new
                        {
                            Luat = new
                            {
                                combined.law.ID,
                                combined.law.Content,
                                combined.law.LuatUUID
                            },
                            Chuong = new
                            {
                                combined.chapter.ID,
                                combined.chapter.Content,
                                combined.chapter.ChuongUUID
                            },
                            Muc = new
                            {
                                muc.ID,
                                muc.Content,
                                muc.MucUUID
                            },
                        })                    
                    .ToList();

                return Json(new TResult()
                {
                    Status = 1,
                    Data = resultFiltered
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
                var dataItem = this.Repository.GetQuery<DATA_3_Muc>()
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
        public IHttpActionResult Post([FromBody] DATA_3_Muc dataRequest)
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
        public IHttpActionResult Put(int iD, [FromBody] DATA_3_Muc dataRequest)
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
            
                var dataItem = this.Repository.GetQuery<DATA_3_Muc>().FirstOrDefault(r => iD == r.ID);
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
