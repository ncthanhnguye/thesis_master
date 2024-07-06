using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MASTERPM.Model;
using MASTERPM.Model.Commons;
using MASTERPM.Model.Core;
using MASTERPM.Model.VM;
using MASTERPM.Web.Api.Base;

namespace MASTERPM.Web.Api.Login
{
    [RoutePrefix("api/Login")]
    public class LoginController : BaseController
    {
        [HttpGet]
        [Route("GetInformation")]
        [AllowAnonymous]
        public IHttpActionResult GetInformation(String accountID)
        {
            try
            {
                Guid account = Guid.Parse(accountID);
                var result = this.Repository.GetQuery<DATA_ACCOUNT>().Where(r => r.DelFlg != true && r.ID == account).First();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Data = result
                });
            }
            catch (Exception e)
            {
                return Json(new TResult()
                {
                    Status = (int)EStatus.Fail,
                    Data = null,
                    Msg = e.Message
                });
            }
        }
                
    }
}
