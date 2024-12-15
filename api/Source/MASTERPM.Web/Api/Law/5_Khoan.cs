using System;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;
using MASTERPM.Model;
using MASTERPM.Model.Commons;
using MASTERPM.Model.Core;
using MASTERPM.Web.Api.Base;

namespace MASTERPM.Web.Models.ChapterItem
{
    [RoutePrefix("api/Claust")]
    public class ClaustController : BaseController
    {
        [HttpGet]
        [Route("GetClaust")]
        public IHttpActionResult GetClaust()
        {
            try
            {
                var result = this.Repository.GetQuery<DATA_5_Khoan>()
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
        public IHttpActionResult Search(string LuatUUID, string ChuongUUID, string MucUUID, string DieuUUID, int? ClaustID)
        {
            try
            {
                var allLaw = LuatUUID == null;
                var allChapterID = ChuongUUID == null;
                var allChapterItemID = MucUUID == null;
                var allArticalID = DieuUUID == null;
                var allClaustID = ClaustID == null;

                Guid.TryParse(LuatUUID, out Guid parsedLuatUUID);
                Guid.TryParse(ChuongUUID, out Guid parsedChuongUUID);
                Guid.TryParse(MucUUID, out Guid parsedMucUUID);
                Guid.TryParse(DieuUUID, out Guid parsedDieuUUID);

                if (allLaw && allChapterID && allChapterItemID && allArticalID)
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
                            (combined, dieu) => new { combined.law, combined.chapter, combined.muc, dieu })
                        .Join(this.Repository.GetQuery<DATA_5_Khoan>(),
                            combined => combined.dieu.DieuUUID,
                            khoan => khoan.DieuUUID,
                            (combined, khoan) => new
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
                                    combined.dieu.ID,
                                    combined.dieu.Content,
                                    combined.dieu.DieuUUID
                                },
                                Khoan = new
                                {
                                    khoan.ID,
                                    khoan.Content,
                                    khoan.KhoanUUID
                                }
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
                        .Where(r => allArticalID || r.DieuUUID == parsedDieuUUID),
                        combined => combined.muc.MucUUID,
                        dieu => dieu.MucUUID,
                        (combined, dieu) => new { combined.law, combined.chapter, combined.muc, dieu })
                    .Join(this.Repository.GetQuery<DATA_5_Khoan>()
                        .Where(r => allArticalID || r.DieuUUID == parsedDieuUUID),
                        combined => combined.dieu.DieuUUID,
                        khoan => khoan.DieuUUID,
                        (combined, khoan) => new
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
                                combined.dieu.ID,
                                combined.dieu.Content,
                                combined.dieu.DieuUUID
                            },
                            Khoan = new
                            {
                                khoan.ID,
                                khoan.Content,
                                khoan.KhoanUUID
                            }
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
                var dataItem = this.Repository.GetQuery<DATA_5_Khoan>()
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
        public IHttpActionResult Post([FromBody] DATA_5_Khoan dataRequest)
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
        public IHttpActionResult Put(int iD, [FromBody] DATA_5_Khoan dataRequest)
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
            
                var dataItem = this.Repository.GetQuery<DATA_5_Khoan>().FirstOrDefault(r => iD == r.ID);
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
