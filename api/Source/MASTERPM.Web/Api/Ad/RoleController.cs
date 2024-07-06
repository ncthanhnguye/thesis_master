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
using MASTERPM.Model.Validate;
using MASTERPM.Model.VM;
using MASTERPM.Web.Api.Base;

namespace MASTERPM.Web.Api.Ad
{
    [RoutePrefix("api/Role")]
    public class RoleController : BaseController
    {
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
                var result = MASTERHelper.GetRoleByChilds(this.Repository);

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
        [Route("GetPage")]
        public IHttpActionResult GetPage()
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
                var result = this.Repository.GetQuery<AD_PAGE>()
                .OrderBy(r => r.ID)
                .ToList();

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
        [Route("Search")]
        public IHttpActionResult Search(string searchText)
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
                var isAll = string.IsNullOrEmpty(searchText);
                var data = MASTERHelper.GetRoleByChilds(this.Repository);

                var result = data
                    .Where(r => (isAll
                        || r.Name.Contains(searchText)
                        || r.ID.Contains(searchText)) && r.DelFlg != true)
                    .OrderBy(r => r.ID)
                    .ToList();

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
        public IHttpActionResult Put(string id, [FromBody]AD_ROLE dataRequest)
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

                var dataItem = this.Repository.GetQuery<AD_ROLE>().FirstOrDefault(r => r.ID == id);
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
        public IHttpActionResult Post([FromBody] AD_ROLE dataRequest)
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

                if (this.Repository.GetQuery<AD_ROLE>().Any(r => r.ID.Equals(dataRequest.ID)))
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
        public IHttpActionResult Saves(IEnumerable<AD_ROLE> dataRequest)
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

                AD_ROLE dataItem = null;
                foreach (var item in dataRequest)
                {
                    string errorMsg = null;
                    GetValidate(item, ref errorMsg);
                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        return Json(new TResult()
                        {
                            Status = (int)EStatus.Fail,
                            Msg = errorMsg
                        });
                    }

                    dataItem = this.Repository.GetQuery<AD_ROLE>().FirstOrDefault(r => r.ID == item.ID);
                    if (dataItem is null)
                    {
                        dataItem = item.Clone();

                        this.Repository.Add(dataItem);
                        this.MASTERLogs.Write(this.Repository, EAction.Insert, JsonConvert.SerializeObject(dataItem));
                    }
                    else
                    {
                        return Json(new TResult()
                        {
                            Status = (short)EStatus.Fail,
                            Msg = string.Format(MASTERResources.Instance.Get(MASTERResources.ID.MsgErrorIsExists), item.ID)
                        });
                    }
                }

                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgSaveOk),
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
                var temp = this.Repository.GetQuery<AD_ROLE>().FirstOrDefault(r => r.ID == dataRequest.IDList);

                if (temp is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgNotFound)
                    });
                }

                var condition = this.Repository.GetQuery<AD_USER>()
                    .AsEnumerable()
                    .FirstOrDefault(r => ((List<string>)JsonConvert.DeserializeObject(r.RoleID, typeof(List<string>))).Any(p => p.Equals(temp.ID)))?.UserName;

                if (!string.IsNullOrEmpty(condition))
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = string.Format(MASTERResources.Instance.Get(MASTERResources.ID.MsgErrorUnitDeleteUser), temp.Name, condition)
                    });
                }

                temp.DelFlg = true;
                temp.UpdateAt = DateTime.Now;
                this.Repository.Update(temp);
                this.MASTERLogs.Write(this.Repository, EAction.Update, JsonConvert.SerializeObject(temp));
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

        private void GetValidate(AD_ROLE item, ref string errorMsg)
        {
            if (string.IsNullOrEmpty(item.ID) || string.IsNullOrWhiteSpace(item.ID))
            {
                errorMsg = string.Concat(errorMsg, string.Format(MASTERResources.Instance.Get(
                        MASTERResources.ID.MsgErrorRequire), MASTERResources.Instance.Get(MASTERResources.ID.Label_Role_ID)), ". ");
            }
            else
            {
                if (!CustomValidation.maxLength(50, item.ID))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(MASTERResources.Instance.Get(
                        MASTERResources.ID.MsgErrorMaximumLength), MASTERResources.Instance.Get(MASTERResources.ID.Label_Role_ID), 50), ". ");
                }
            }

            if (string.IsNullOrEmpty(item.Name) || string.IsNullOrWhiteSpace(item.Name))
            {
                errorMsg = string.Concat(errorMsg, string.Format(MASTERResources.Instance.Get(
                        MASTERResources.ID.MsgErrorRequire), MASTERResources.Instance.Get(MASTERResources.ID.Label_Role_Name)), ". ");
            }
            else
            {
                if (!CustomValidation.maxLength(250, item.Name))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(MASTERResources.Instance.Get(
                        MASTERResources.ID.MsgErrorMaximumLength), MASTERResources.Instance.Get(MASTERResources.ID.Label_Role_Name), 250), ". ");
                }
            }
        }
    }
}
