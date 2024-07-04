using System;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;
using VNPTPM.Model;
using VNPTPM.Model.Core;
using VNPTPM.Web.Api.Base;

namespace VNPTPM.Web.Models.Point
{
    [RoutePrefix("api/TimKiem")]
    public class SearchController : BaseController
    {

        [HttpGet]
        [Route("Search")]
        [AllowAnonymous]
        public IHttpActionResult Search(string searchText)
        {
            try
            {
                var isAllSearchText = string.IsNullOrEmpty(searchText);


                var result = this.Repository.GetQuery<DATA_CRAWL>().Where(r=> isAllSearchText || (r.NoiDungCauHoi.ToLower().Contains(searchText.ToLower()) || r.TenCauHoi.ToLower().Contains(searchText.ToLower())) ).ToList();


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
        [Route("GetByID")]
        [AllowAnonymous]
        public IHttpActionResult Get(int iD)
        {

            var result = this.Repository.GetQuery<DATA_CRAWL>().FirstOrDefault(r=>r.ID == iD);

            return Json(new TResult()
                {
                    Status = 1,
                    Data = result
            });
            
        }




    }
}
