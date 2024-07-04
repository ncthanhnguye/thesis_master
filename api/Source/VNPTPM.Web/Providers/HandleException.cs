using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace VNPTPM.Web.Providers
{
    public class HandleException : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            string exceptionMessage = "Lỗi hệ thống. Hãy liên hệ admin.";
            //We can log this exception message to the file or database.
            context.Response = new HttpResponseMessage()
            {
                Content = new StringContent(exceptionMessage),
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }
}