using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;
using MASTERPM.Model;
using MASTERPM.Model.Commons;
using MASTERPM.Model.Core;
using MASTERPM.Model.VM;
using MASTERPM.Web.Api.Base;

namespace MASTERPM.Web.Api.Profile
{
    [RoutePrefix("api/ImportWordLaw")]
    [AllowAnonymous]
    public class ImportWordLawController : BaseController
    {
        [HttpPost]
        [Route("Saves")]
        public IHttpActionResult SavesWord(SavesWordRequest payload)
        {
            try
            {
                // Lưu thông tin Luật
                var law = new DATA_1_Luat()
                {
                    Content = payload.Content,
                    ContentHTML = payload.ContentHTML,
                    LawDate = payload.LawDate,
                    LawNumber = payload.LawNumber,
                    TotalChapter = payload.Chapters.Count,
                    Status = payload.Status,
                };
                this.Repository.Add(law);
                this.Repository.UnitOfWork.SaveChanges();

                // Chương
                if (payload.Chapters != null && payload.Chapters.Count > 0)
                {
                    foreach (var chapter in payload.Chapters)
                    {
                        var _chapter = new DATA_2_Chuong()
                        {
                            ChuongUUID = chapter.ChuongUUID,
                            Content = chapter.Content,
                            LuatUUID = chapter.LuatUUID,
                        };
                        this.Repository.Add(chapter);
                    }
                }

                // Mục
                if (payload.ChapterItems != null && payload.ChapterItems.Count > 0)
                {
                    foreach (var item in payload.ChapterItems)
                    {
                        var chapterItem = new DATA_3_Muc()
                        {
                            MucUUID = item.MucUUID,
                            LuatUUID = item.LuatUUID,
                            ChuongUUID = item.ChuongUUID,
                            Content = item.Content,
                        };
                        this.Repository.Add(chapterItem);
                    }
                }

                // Điều
                if (payload.Articals != null && payload.Articals.Count > 0)
                {
                    foreach (var artical in payload.Articals)
                    {
                        var articalItem = new DATA_4_Dieu()
                        {
                            DieuUUID = artical.DieuUUID,
                            LuatUUID = artical.LuatUUID,
                            ChuongUUID = artical.ChuongUUID,
                            MucUUID = artical.MucUUID,
                            Content = artical.Content,
                        };
                        this.Repository.Add(articalItem);
                    }
                }

                // Khoản
                if (payload.Clausts != null && payload.Clausts.Count > 0)
                {
                    foreach (var claust in payload.Clausts)
                    {
                        var claustItem = new DATA_5_Khoan()
                        {
                            KhoanUUID = claust.KhoanUUID,
                            LuatUUID = claust.LuatUUID,
                            ChuongUUID = claust.ChuongUUID,
                            MucUUID = claust.MucUUID,
                            DieuUUID = claust.DieuUUID,
                            Content = claust.Content,
                        };
                        this.Repository.Add(claustItem);
                    }
                }

                // Điểm
                if (payload.Points != null && payload.Points.Count > 0)
                {
                    foreach (var point in payload.Points)
                    {
                        var pointItem = new DATA_6_Diem()
                        {
                            DiemUUID = point.DiemUUID,
                            LuatUUID = point.LuatUUID,
                            ChuongUUID = point.ChuongUUID,
                            MucUUID = point.MucUUID,
                            DieuUUID = point.DieuUUID,
                            KhoanUUID = point.KhoanUUID,
                            Content = point.Content,
                        };
                        this.Repository.Add(pointItem);
                    }
                }

                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgSaveOk),
                });
            }
            catch (Exception e)
            {
                this.MASTERLogs.Write(this.RepositoryLog, e.Message); // Write error logs into Logs File
                throw new Exception("", e);
            }
        }

        [HttpGet]
        [Route("Search")]
        public IHttpActionResult Search()
        {
            try
            {
        
                var result = this.Repository.GetQuery<DATA_1_Luat>()
                    
                    .OrderBy(r => r.ID).ToList();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Data = result
                });
            }
            catch (Exception e)
            {
                this.MASTERLogs.Write(this.RepositoryLog, e.Message);
                throw new Exception("", e);
            }
        }

    }
}
