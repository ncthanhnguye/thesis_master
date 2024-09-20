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
        public IHttpActionResult SavesWord(IEnumerable<Data_LawVM> dataRequest)
        {
            try
            {
                if (dataRequest.Count() > 0)
                {
                    DATA_1_Luat dataItem = null;
                    foreach (var item in dataRequest)
                    {
                        dataItem = new DATA_1_Luat()
                        {
                            Title = item.Title,
                            Content = item.Content,
                            ContentHTML = item.ContentHTML,
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
