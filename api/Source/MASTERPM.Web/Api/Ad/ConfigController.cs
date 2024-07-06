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
using MASTERPM.Model.Validate;
using MASTERPM.Web.Api.Base;

namespace MASTERPM.Web.Api.Ad
{
    [RoutePrefix("api/Config")]
    public class ConfigController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get(string id)
        {
            try
            {
                //check token expired
                if (this.isAuthenticatedUser != true)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgErrorNotPermisionFunc)
                    });
                }
                var result = this.Repository.GetQuery<AD_CONFIG>().FirstOrDefault(r => r.ID == id);
                if (result is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgNotFound)
                    });
                }

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

        [HttpGet]
        [Route("GetAppVersion")]
        [AllowAnonymous]
        public IHttpActionResult GetAppVersion()
        {
            try
            {
                //check token expired
                if (this.isAuthenticatedUser != true)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgErrorNotPermisionFunc)
                    });
                }
                var result = this.Repository.GetQuery<AD_CONFIG>().FirstOrDefault(r => r.ID == Constants.PROJECT_APP_VERSION);
                if (result is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgNotFound)
                    });
                }

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Data = new
                    {
                        AppVersion = result.Value,
                        Url = "https://appstore.hcmtelecom.vn/ttcntt/qldanguy.html"
                    }
                });
            }
            catch (Exception e)
            {
                this.MASTERLogs.Write(this.RepositoryLog, e.Message);
                throw new Exception("", e);
            }
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                //check token expired
                if (this.isAuthenticatedUser != true)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgErrorNotPermisionFunc)
                    });
                }
                var result = this.Repository.GetQuery<AD_CONFIG>().ToList();
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

        [HttpPut]
        public IHttpActionResult Put(string id, [FromBody]AD_CONFIG dataRequest)
        {
            try
            {
                //check token expired
                if (this.isAuthenticatedUser != true)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgErrorNotPermisionFunc)
                    });
                }
                //start check permisssion
                string usernameFromHeader = Request.Headers.GetValues("UserName").First();
                if (string.IsNullOrEmpty(usernameFromHeader))
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgUserNameInHeaderNullOrEmpty)
                    });
                }
                bool checkPermission = MASTERHelper.CheckUserPermissionByRoleIDs(this.Repository, usernameFromHeader, true);
                if (!checkPermission)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgErrorNotPermisionFunc)
                    });
                }
                //end
                //validate custom data input
                string errorMsg = null;
                GetValidate(dataRequest, ref errorMsg);

                //validate custom data input
                if (errorMsg != "" && errorMsg != null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = errorMsg
                    });
                }

                if (!ModelState.IsValid)
                {
                    var model = ModelState;
                    string msg = MASTERResources.Instance.Get(model);

                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = msg
                    });
                }

                var dataItem = this.Repository.GetQuery<AD_CONFIG>().FirstOrDefault(r => r.ID == id);
                if (dataItem is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgNotFound)
                    });
                }

                dataItem = dataRequest.Clone();
                dataItem.ID = id;

                this.Repository.Update(dataItem);
                this.MASTERLogs.Write(this.Repository, EAction.Update, JsonConvert.SerializeObject(dataItem));
                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgSaveOk),
                    Data = dataItem
                });
            }
            catch (Exception e)
            {
                this.MASTERLogs.Write(this.RepositoryLog, e.Message);
                throw new Exception("", e);
            }
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] AD_CONFIG dataRequest)
        {
            try
            {
                //check token expired
                if (this.isAuthenticatedUser != true)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgErrorNotPermisionFunc)
                    });
                }
                //start check permisssion
                string usernameFromHeader = Request.Headers.GetValues("UserName").First();
                if (string.IsNullOrEmpty(usernameFromHeader))
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgUserNameInHeaderNullOrEmpty)
                    });
                }
                bool checkPermission = MASTERHelper.CheckUserPermissionByRoleIDs(this.Repository, usernameFromHeader, true);
                if (!checkPermission)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgErrorNotPermisionFunc)
                    });
                }
                //end
                //validate custom data input
                string errorMsg = null;
                GetValidate(dataRequest, ref errorMsg);

                //validate custom data input
                if (errorMsg != "" && errorMsg != null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = errorMsg
                    });
                }

                if (!ModelState.IsValid)
                {
                    var model = ModelState;
                    string msg = MASTERResources.Instance.Get(model);

                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = msg
                    });
                }

                if (this.Repository.GetQuery<AD_CONFIG>().Any(r => r.ID.Equals(dataRequest.ID)))
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = string.Format(MASTERResources.Instance.Get(MASTERResources.ID.MsgErrorIsExists), dataRequest.ID)
                    });
                }

                var dataItem = dataRequest.Clone();

                this.Repository.Add(dataItem);
                this.MASTERLogs.Write(this.Repository, EAction.Insert, JsonConvert.SerializeObject(dataItem));
                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgSaveOk),
                    Data = dataItem
                });
            }
            catch (Exception e)
            {
                this.MASTERLogs.Write(this.RepositoryLog, e.Message);
                throw new Exception("", e);
            }
        }

        [HttpPost]
        [Route("Saves")]
        public IHttpActionResult Saves(IEnumerable<AD_CONFIG> dataRequest)
        {
            try
            {
                //check token expired
                if (this.isAuthenticatedUser != true)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgErrorNotPermisionFunc)
                    });
                }
                //start check permisssion
                string usernameFromHeader = Request.Headers.GetValues("UserName").First();
                if (string.IsNullOrEmpty(usernameFromHeader))
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgUserNameInHeaderNullOrEmpty)
                    });
                }
                bool checkPermission = MASTERHelper.CheckUserPermissionByRoleIDs(this.Repository, usernameFromHeader, true);
                if (!checkPermission)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgErrorNotPermisionFunc)
                    });
                }
                //end
                if (!ModelState.IsValid)
                {
                    var model = ModelState;
                    string msg = MASTERResources.Instance.Get(model);

                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = msg
                    });
                }

                AD_CONFIG dataItem = null;
                foreach (var item in dataRequest)
                {
                    dataItem = this.Repository.GetQuery<AD_CONFIG>().FirstOrDefault(r => r.ID == item.ID);
                    if (dataItem is null)
                    {
                        if (this.Repository.GetQuery<AD_CONFIG>().Any(r => r.ID.Equals(item.ID)))
                        {
                            return Json(new TResult()
                            {
                                Status = (short)EStatus.Fail,
                                Msg = string.Format(MASTERResources.Instance.Get(MASTERResources.ID.MsgErrorIsExists), item.ID)
                            });
                        }

                        dataItem = item.Clone();
                        this.Repository.Add(dataItem);
                        this.MASTERLogs.Write(this.Repository, EAction.Insert, JsonConvert.SerializeObject(dataItem));
                    }
                }

                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok
                });
            }
            catch (Exception e)
            {
                this.MASTERLogs.Write(this.Repository, e.Message);
                throw new Exception("", e);
            }
        }

        [HttpPost]
        [Route("Deletes")]
        public IHttpActionResult Deletes([FromBody] DeleteVM dataRequest)
        {
            try
            {
                //check token expired
                if (this.isAuthenticatedUser != true)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgErrorNotPermisionFunc)
                    });
                }
                //start check permisssion
                string usernameFromHeader = Request.Headers.GetValues("UserName").First();
                if (string.IsNullOrEmpty(usernameFromHeader))
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgUserNameInHeaderNullOrEmpty)
                    });
                }
                bool checkPermission = MASTERHelper.CheckUserPermissionByRoleIDs(this.Repository, usernameFromHeader, true);
                if (!checkPermission)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgErrorNotPermisionFunc)
                    });
                }
                //end
                if (string.IsNullOrEmpty(dataRequest.IDList))
                {
                    return Json(new TResult()
                    {
                        Status = (int)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgErrorRequiredDeleteID)
                    });
                }
                var temp = this.Repository.GetQuery<AD_CONFIG>().FirstOrDefault(r => r.ID == dataRequest.IDList);
                if (temp is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgNotFound)
                    });
                }
                this.Repository.Delete(temp);
                this.MASTERLogs.Write(this.Repository, EAction.Delete, JsonConvert.SerializeObject(temp));

                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgDeleteOk)
                });
            }
            catch (Exception e)
            {
                this.MASTERLogs.Write(this.RepositoryLog, e.Message);
                throw new Exception("", e);
            }
        }

        private void GetValidate(AD_CONFIG item, ref string errorMsg)
        {
            if (string.IsNullOrEmpty(item.ID) || string.IsNullOrWhiteSpace(item.ID))
            {
                errorMsg = string.Concat(errorMsg, string.Format(MASTERResources.Instance.Get(
                        MASTERResources.ID.MsgErrorRequire), MASTERResources.Instance.Get(MASTERResources.ID.Label_Config_ID)), ". ");
            }
            else
            {
                if (!CustomValidation.maxLength(50, item.ID))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(MASTERResources.Instance.Get(
                        MASTERResources.ID.MsgErrorMaximumLength), MASTERResources.Instance.Get(MASTERResources.ID.Label_Config_ID), 50), ". ");
                }
            }

            if (string.IsNullOrEmpty(item.Value) || string.IsNullOrWhiteSpace(item.Value))
            {
                errorMsg = string.Concat(errorMsg, string.Format(MASTERResources.Instance.Get(
                        MASTERResources.ID.MsgErrorRequire), MASTERResources.Instance.Get(MASTERResources.ID.Label_Config_Value)), ". ");
            }
            else
            {
                if (!CustomValidation.maxLength(500, item.Value))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(MASTERResources.Instance.Get(
                        MASTERResources.ID.MsgErrorMaximumLength), MASTERResources.Instance.Get(MASTERResources.ID.Label_Config_Value), 250), ". ");
                }
            }

            if (string.IsNullOrEmpty(item.Description) || string.IsNullOrWhiteSpace(item.Description))
            {
                errorMsg = string.Concat(errorMsg, string.Format(MASTERResources.Instance.Get(
                        MASTERResources.ID.MsgErrorRequire), MASTERResources.Instance.Get(MASTERResources.ID.Label_Config_Description)), ". ");
            }

        }
    }
}
