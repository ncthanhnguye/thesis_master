using System;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;
using MASTERPM.Model;
using MASTERPM.Model.Core;
using MASTERPM.Web.Api.Base;

namespace MASTERPM.Web.Models.Chapter
{
    [RoutePrefix("api/Dieu")]
    public class ChapterController : BaseController
    {

        [HttpGet]
        [Route("GetChapter")]
        public IHttpActionResult GetChapter()
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
        public IHttpActionResult Search(int? LuatID, int? ChuongID, int? MucID, int? DieuID, int? KhoanID, int? DiemID)
        {
            try
            {

                var allLuat = LuatID == null;
                var allChuongID = ChuongID == null;
                var allMucID = MucID == null;
                var allDieuID = DieuID == null;
                var allKhoanID = KhoanID == null;
                var allDiemID = DiemID == null;

                var result = this.Repository.GetQuery<DATA_1_Luat>().Where(r => (allLuat || r.ID == LuatID))
                     .Join(this.Repository.GetQuery<DATA_2_Chuong>().Where(r => (allChuongID || r.ID == ChuongID)),
                        a => a.ID, b => b.LuatID, (a, b) => new
                        {
                            DATA_1_Luat = a,
                            DATA_2_Chuong = b
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
