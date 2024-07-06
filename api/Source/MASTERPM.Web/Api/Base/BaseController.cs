using System;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using MASTERPM.Model.Commons;
using MASTERPM.Model.Core;
using MASTERPM.Web.Api.Home;
using MASTERPM.Model;
using Newtonsoft.Json;
using System.Net.Http.Formatting;


namespace MASTERPM.Web.Api.Base
{
    [Authorize]
    public class BaseController : ApiController, IDisposable
    {
        private IDALContainer DALContainer = null;
        private IDALContainer DALContainerLog = null;
        private MASTERLogs mASTERLogs = null;
        public bool isAuthenticatedUser = false;

        public BaseController()
        {
            DALContainer = new EFDALContainer();
            DALContainerLog = new EFDALContainer();
            var httpCurrent = HttpContext.Current;
            if (httpCurrent != null)
            {
                var request = httpCurrent.Request;
                if (request.HttpMethod != "OPTIONS" && request.Path != "/Token")
                {
                    var token = "";
                    var userNameFromHeader = "";
                    if (request.Headers != null)
                    {
                        token = request.Headers.GetValues("Authorization").FirstOrDefault();
                    }
                    if (request.Headers.GetValues("UserName") != null)
                    {
                        userNameFromHeader = request.Headers.GetValues("UserName").FirstOrDefault();
                    }
                    this.isAuthenticatedUser = MASTERHelper.checkToken(this.Repository, token, userNameFromHeader);
                }
            }
        }

        public IRepository Repository
        {
            get
            {
                return DALContainer.Repository;
            }
        }

        public IRepository RepositoryLog
        {
            get
            {
                return DALContainerLog.Repository;
            }
        }
        public MASTERLogs MASTERLogs
        {
            get
            {
                if (mASTERLogs == null)
                {
                    mASTERLogs = new MASTERLogs();
                }

                return mASTERLogs;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (DALContainer != null)
            {
                DALContainer.Close();
                DALContainer.Dispose();
                DALContainer = null;
            }
            base.Dispose(disposing);
        }
    
    }
}