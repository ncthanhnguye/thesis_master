using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using MASTERPM.Model;
using MASTERPM.Model.Commons;
using MASTERPM.Model.Core;
using MASTERPM.Model.VM;
using MASTERPM.Web.Api.Base;

namespace MASTERPM.Web.Api.Home
{
    public class clsECommon
    {
        public byte Id { get; set; }
        public string Name { get; set; }
    }

    [RoutePrefix("api/Enum")]
    [AllowAnonymous]
    public class EnumController : BaseController
    {
        [Route("GetEGender")]
        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult GetEGender()
        {
            var result = Enum.GetNames(typeof(EGender))
                .Select(r => new
                {
                    ID = (int)Enum.Parse(typeof(EGender), r),
                    Name = MASTERResources.Instance.Get("Label_" + r.ToString())
                });

            return Json(new TResult()
            {
                Status = (int)EStatus.Ok,
                Data = result
            });
        }
        

        [Route("GetEConfig")]
        [HttpGet]
        public IHttpActionResult GetEConfig()
        {
            var result = Enum.GetNames(typeof(EConfig))
                .Select(r => new
                {

                    ID = r.ToString(),
                    Name = r.ToString()
                }).ToList();

            return Json(new TResult()
            {
                Status = (int)EStatus.Ok,
                Data = result
            });
        }

        [Route("GetECommon")]
        [HttpGet]
        public IHttpActionResult GetECommon()
        {
            var result = Enum.GetNames(typeof(ECommon))
                .Select(r => new
                {
                    ID = (int)Enum.Parse(typeof(ECommon), r),
                    Name = MASTERResources.Instance.Get("Label_" + r.ToString())
                });

            return Json(new TResult()
            {
                Status = (int)EStatus.Ok,
                Data = result
            });
        }




	}
}
