using System;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;
using MASTERPM.Model;
using MASTERPM.Model.Commons;
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
        public IHttpActionResult Search(string LuatUUID, string ChuongUUID, string MucUUID, int? ArticalID)
        {
            try
            {
                var allLaw = LuatUUID == null;
                var allChapterID = ChuongUUID == null;
                var allChapterItemID = MucUUID == null;
                var allArticalID = ArticalID == null;

                Guid.TryParse(LuatUUID, out Guid parsedLuatUUID);
                Guid.TryParse(ChuongUUID, out Guid parsedChuongUUID);
                Guid.TryParse(MucUUID, out Guid parsedMucUUID);

                if (allLaw && allChapterID && allChapterItemID)
                {
                    var resultAll = this.Repository.GetQuery<DATA_1_Luat>()
                        .Join(this.Repository.GetQuery<DATA_2_Chuong>(),
                            law => law.LuatUUID,
                            chapter => chapter.LuatUUID,
                            (law, chapter) => new { law, chapter })
                        .Join(this.Repository.GetQuery<DATA_3_Muc>(),
                            combined => combined.chapter.ChuongUUID,
                            muc => muc.ChuongUUID,
                            (combined, muc) => new { combined.law, combined.chapter, muc })
                        .Join(this.Repository.GetQuery<DATA_4_Dieu>(),
                            combined => combined.muc.MucUUID,
                            dieu => dieu.MucUUID,
                            (combined, dieu) => new
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
                                    combined.muc.ID,
                                    combined.muc.Content,
                                    combined.muc.MucUUID
                                },
                                Dieu = new
                                {
                                    dieu.ID,
                                    dieu.Content,
                                    dieu.DieuUUID
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
                        .Where(r => allChapterItemID || r.MucUUID == parsedMucUUID),
                        combined => combined.chapter.ChuongUUID,
                        muc => muc.ChuongUUID,
                        (combined, muc) => new { combined.law, combined.chapter, muc })
                    .Join(this.Repository.GetQuery<DATA_4_Dieu>()
                        .Where(r => allChapterItemID || r.MucUUID == parsedMucUUID),
                        combined => combined.muc.MucUUID,
                        dieu => dieu.MucUUID,
                        (combined, dieu) => new
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
                                combined.muc.ID,
                                combined.muc.Content,
                                combined.muc.MucUUID
                            },
                            Dieu = new
                            {
                                dieu.ID,
                                dieu.Content,
                                dieu.DieuUUID
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
