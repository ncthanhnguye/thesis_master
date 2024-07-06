using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;
using MASTERPM.Model;
using MASTERPM.Model.Commons;
using MASTERPM.Model.Core;
using MASTERPM.Web.Api.Base;

namespace MASTERPM.Web.Api.Profile
{
    [RoutePrefix("api/ImportDataCrawl")]
    [AllowAnonymous]
    public class ImportDataCrawlController : BaseController
    {
        
        [HttpPost]
        [Route("Saves")]
        public IHttpActionResult Saves(IEnumerable<Data_Crawl_ImportVM> dataRequest)
        {
            try
            {

                if (dataRequest.Count() > 0)
                {


                    DATA_CRAWL dataItem = null;
                    foreach (var item in dataRequest)
                    {
                        

                        dataItem = new DATA_CRAWL()
                        {
                            ID = item.ID ,
                            TenCauHoi = item.TenCauHoi,
                            LinhVuc = item.LinhVuc,
                            NoiDungCauHoi = item.NoiDungCauHoi,
                            CauTraLoi = item.CauTraLoi,
                            Luat = item.Luat,
                            KeyWords = item.KeyWords,
                        };

                        this.Repository.Add(dataItem);

                    
                    }

                    this.Repository.UnitOfWork.SaveChanges();
                }

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgSaveOk),
                });
            }
            catch (Exception e)
            {
                throw new Exception("", e);
            }
        }

        [HttpGet]
        [Route("Search")]
        public IHttpActionResult Search()
        {
            try
            {
        
                var result = this.Repository.GetQuery<DATA_CRAWL>()
                    
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
