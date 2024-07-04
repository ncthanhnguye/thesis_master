using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VNPTPM.Model;
using VNPTPM.Model.Commons;
using VNPTPM.Model.Core;
using VNPTPM.Model.VM;
using VNPTPM.Web.Api.Base;

namespace VNPTPM.Web.Api.Login
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
