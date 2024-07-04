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
using VNPTPM.Model;
using VNPTPM.Model.Commons;
using VNPTPM.Model.Core;
using VNPTPM.Model.VM;
using VNPTPM.Web.Api.Base;

namespace VNPTPM.Web.Api.Home
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
                    Name = VNPTResources.Instance.Get("Label_" + r.ToString())
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
                    Name = VNPTResources.Instance.Get("Label_" + r.ToString())
                });

            return Json(new TResult()
            {
                Status = (int)EStatus.Ok,
                Data = result
            });
        }

        [Route("GetETypeImage")]
        [HttpGet]
        public IHttpActionResult GetETypeImage()
        {
            var result = Enum.GetNames(typeof(ETypeImage))
                .Select(r => new
                {
                    ID = (int)Enum.Parse(typeof(ETypeImage), r),
                    Name = VNPTResources.Instance.Get("Label_" + r.ToString())
                });

            return Json(new TResult()
            {
                Status = (int)EStatus.Ok,
                Data = result
            });
        }

        [Route("GetECommentStatus")]
        [HttpGet]
        public IHttpActionResult ECommentStatus()
        {
            var result = Enum.GetNames(typeof(ECommentStatus))
                .Select(r => new
                {
                    ID = (int)Enum.Parse(typeof(ECommentStatus), r),
                    Name = VNPTResources.Instance.Get("Label_PostComment_Status_" + r.ToString())
                })
                .ToList();

            result.Insert(0, new
            {
                ID = -1,
                Name = VNPTResources.Instance.Get("Label_PostComment_Status_All")
            });

            return Json(new TResult()
            {
                Status = (int)EStatus.Ok,
                Data = result
            });
        }

        [Route("GetESeatMapType")]
        [HttpGet]
        public IHttpActionResult ESeatMapType()
        {
            var result = Enum.GetNames(typeof(ESeatMapType))
                .Select(r => new
                {
                    ID = (int)Enum.Parse(typeof(ESeatMapType), r),
                    Name = VNPTResources.Instance.Get("Label_ESeatMapType_" + r.ToString())
                })
                .ToList();

            return Json(new TResult()
            {
                Status = (int)EStatus.Ok,
                Data = result
            });
        }

        [Route("GetERewardStatus")]
        [HttpGet]
        public IHttpActionResult GetERewardStatus()
        {
            var result = Enum.GetNames(typeof(ERewardStatus))
                .Select(r => new
                {
                    ID = (int)Enum.Parse(typeof(ERewardStatus), r),
                    Name = VNPTResources.Instance.Get("Label_" + r.ToString())
                });

            return Json(new TResult()
            {
                Status = (int)EStatus.Ok,
                Data = result
            });
        }

		[Route("GetERewardSignStatus")]
		[HttpGet]
		public IHttpActionResult GetERewardSignStatus()
		{
			var result = Enum.GetNames(typeof(ERewardSignStatus))
				.Select(r => new
				{
					ID = (int)Enum.Parse(typeof(ERewardSignStatus), r),
					Name = VNPTResources.Instance.Get("Label_" + r.ToString())
				});

			return Json(new TResult()
			{
				Status = (int)EStatus.Ok,
				Data = result
			});
		}
	}
}
