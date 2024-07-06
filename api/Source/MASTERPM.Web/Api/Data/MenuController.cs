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

namespace MASTERPM.Web.Api.M
{
    [RoutePrefix("api/Menu")]
    public class MenuController : BaseController
    {
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
                var isAll = string.IsNullOrEmpty(searchText);
                var pages = this.Repository.GetQuery<DATA_MENU>()
                        .Where(r => r.DelFlg != true && (isAll || r.Name.Contains(searchText)))
                        .OrderByDescending(r => r.CreateAt);
                var result = pages.GroupJoin(pages, a => a.ParentID, b => b.ID, (a, b) => new
                {
                    a.ID,
                    a.Name,
                    a.ParentID,
                    a.PageID,
                    a.OrderIdx,
                    ParentName = b.FirstOrDefault().Name,
                    a.UrlPath
                })
                .OrderBy(r => r.OrderIdx)
                .ToList();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Data = result
                });
            }
            catch (Exception e)
            {
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
                var result = this.Repository.GetQuery<DATA_MENU>()
                    .Where(r => r.DelFlg != true )
                    .OrderBy(r => r.OrderIdx)
                    .ThenByDescending(r => r.CreateAt);
                
                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Data = result
                });
            }
            catch (Exception e)
            {
                throw new Exception("", e);
            }
        }

        [HttpGet]
        public IHttpActionResult Get(Guid ID)
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
                var dataItem = this.Repository.GetQuery<DATA_MENU>()
                    .FirstOrDefault(r => r.ID == ID);
                if (dataItem is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = $"{MASTERResources.Instance.Get(MASTERResources.ID.MsgNotFound)}_DATA_ITEM"
                    });
                }

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Data = dataItem
                });
            }
            catch (Exception e)
            {
                throw new Exception("", e);
            }
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] DATA_MENU dataRequest)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    var model = ModelState;
                    var dtErr = MASTERResources.Instance.GetDataTable(model);

                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Data = dtErr,
                        Msg = ""
                    });
                }
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
                var dataItem = dataRequest.Clone();
                dataItem.ID = Guid.NewGuid();
                dataItem.CreateAt = DateTime.Now;
                dataItem.CreateAt = DateTime.Now;

                this.Repository.Add(dataItem);
                
                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgSaveOk)
                });
            }
            catch (Exception e)
            {
                throw new Exception("", e);
            }
        }

        [HttpPut]
        public IHttpActionResult Put(Guid iD, [FromBody]DATA_MENU dataRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var model = ModelState;
                    var dtErr = MASTERResources.Instance.GetDataTable(model);

                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Data = dtErr,
                        Msg = ""
                    });
                }
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
                var dataItem = this.Repository.GetQuery<DATA_MENU>().FirstOrDefault(r => r.ID == iD);
                if (dataItem is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = $"{MASTERResources.Instance.Get(MASTERResources.ID.MsgNotFound)}_DATA_MENU"
                    });
                }

                dataItem.Name = dataRequest.Name;
                dataItem.PageID = dataRequest.PageID;
                dataItem.ParentID = dataRequest.ParentID;
                dataItem.OrderIdx = dataRequest.OrderIdx;
                dataItem.UrlPath = dataRequest.UrlPath;
                dataItem.Roles = dataRequest.Roles;

                dataItem.UpdateAt = DateTime.Now;

                this.Repository.Update(dataItem);

                this.Repository.UnitOfWork.SaveChanges();

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

        [HttpDelete]
        public IHttpActionResult Delete(Guid iD)
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
                var dataItem = this.Repository.GetQuery<DATA_MENU>().FirstOrDefault(r => iD == r.ID );
                if(dataItem == null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgNotFound)
                    });
                }
                var unitChild = this.Repository.GetQuery<DATA_MENU>().FirstOrDefault(r => r.DelFlg != true && r.ParentID == dataItem.ID);
                if (unitChild != null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = string.Format(MASTERResources.Instance.Get(MASTERResources.ID.MsgErrorUnitDeleteMenu), dataItem.Name)
                    });
                }
                dataItem.DelFlg = true;
                this.Repository.Update(dataItem);

                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgDeleteOk)
                });
            }
            catch (Exception e)
            {
                throw new Exception("", e);
            }
        }
    }
}