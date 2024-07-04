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
using VNPTPM.Model.Validate;
using VNPTPM.Model.VM;
using VNPTPM.Web.Api.Base;

namespace VNPTPM.Web.Api.Ad
{
    [RoutePrefix("api/Control")]
    public class ControlController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get(int iD)
        {
            try
            {
                var result = this.Repository.GetQuery<AD_CONTROL>().FirstOrDefault(r => r.ID == iD);
                if (result is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
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
                this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                throw new Exception("", e);
            }
        }

        [HttpGet]
        [Route("Search")]
        public IHttpActionResult Search(string searchText)
        {
            try
            {
                var isAllControl = string.IsNullOrEmpty(searchText);
                var result = this.Repository.GetQuery<AD_CONTROL>()
                    .Where(r => isAllControl
                    || r.Name.Contains(searchText))
                .OrderBy(r => r.Name)
                .ToList();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Data = result
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                throw new Exception("", e);
            }
        }

        [HttpPut]
        public IHttpActionResult Put(int iD, [FromBody]AD_CONTROL dataRequest)
        {
            try
            {
                //check token expired
                if (this.isAuthenticatedUser != true)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorNotPermisionFunc)
                    });
                }
                //start check permisssion
                string usernameFromHeader = Request.Headers.GetValues("UserName").First();
                if (string.IsNullOrEmpty(usernameFromHeader))
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgUserNameInHeaderNullOrEmpty)
                    });
                }
                bool checkPermission = VNPTHelper.CheckUserPermissionByRoleIDs(this.Repository, usernameFromHeader, true);
                if (!checkPermission)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorNotPermisionFunc)
                    });
                }
                //end
                if (!ModelState.IsValid)
                {
                    var model = ModelState;
                    var dtErr = VNPTResources.Instance.GetDataTable(model);

                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Data = dtErr,
                        Msg = ""
                    });
                }

                var dataItem = this.Repository.GetQuery<AD_CONTROL>().FirstOrDefault(r => r.ID == iD);
                if (dataItem is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
                }

                dataItem.ControlID = dataRequest.ControlID;
                dataItem.PageID = dataRequest.PageID;
                dataItem.Name = dataRequest.Name;
                dataItem.AbsoluteUrl = dataRequest.AbsoluteUrl;
                dataItem.MethodRequest = dataRequest.MethodRequest;

                this.Repository.Update(dataItem);
                this.VNPTLogs.Write(this.Repository, EAction.Update, JsonConvert.SerializeObject(dataItem));

                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgUpdateDataSuccess),
                        VNPTResources.Instance.Get(VNPTResources.ID.Label_obj_page)),
                    Data = dataItem
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                throw new Exception("", e);
            }
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] AD_CONTROL dataRequest)
        {
            try
            {
                //check token expired
                if (this.isAuthenticatedUser != true)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorNotPermisionFunc)
                    });
                }
                //start check permisssion
                string usernameFromHeader = Request.Headers.GetValues("UserName").First();
                if (string.IsNullOrEmpty(usernameFromHeader))
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgUserNameInHeaderNullOrEmpty)
                    });
                }
                bool checkPermission = VNPTHelper.CheckUserPermissionByRoleIDs(this.Repository, usernameFromHeader, true);
                if (!checkPermission)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorNotPermisionFunc)
                    });
                }
                //end
                if (!ModelState.IsValid)
                {
                    var model = ModelState;
                    var dtErr = VNPTResources.Instance.GetDataTable(model);

                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Data = dtErr,
                        Msg = ""
                    });
                }

                if (this.Repository.GetQuery<AD_CONTROL>().Any(r => 
                    r.ControlID.Equals(dataRequest.ControlID)
                    && r.PageID.Equals(dataRequest.PageID)))
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorIsExists), dataRequest.ControlID)
                    });
                }

                var dataItem = dataRequest.Clone();

                this.Repository.Add(dataItem);
                this.VNPTLogs.Write(this.Repository, EAction.Insert, JsonConvert.SerializeObject(dataItem));

                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgCreateDataSuccess),
                        VNPTResources.Instance.Get(VNPTResources.ID.Label_obj_page)),
                    Data = dataItem
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                throw new Exception("", e);
            }
        }
        
        [HttpDelete]
        public IHttpActionResult Delete(int iD)
        {
            try
            {
                //check token expired
                if (this.isAuthenticatedUser != true)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorNotPermisionFunc)
                    });
                }
                //start check permisssion
                string usernameFromHeader = Request.Headers.GetValues("UserName").First();
                if (string.IsNullOrEmpty(usernameFromHeader))
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgUserNameInHeaderNullOrEmpty)
                    });
                }
                bool checkPermission = VNPTHelper.CheckUserPermissionByRoleIDs(this.Repository, usernameFromHeader, true);
                if (!checkPermission)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorNotPermisionFunc)
                    });
                }
                //end
                var temp = this.Repository.GetQuery<AD_CONTROL>().FirstOrDefault(r => r.ID == iD);
                if (temp is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
                }


                this.Repository.Delete(temp);
                this.VNPTLogs.Write(this.Repository, EAction.Delete, JsonConvert.SerializeObject(temp));
                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgDeleteOk)
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                throw new Exception("", e);
            }
        }

        [HttpPost]
        [Route("Check")]
        public IHttpActionResult Check(CheckControlVM dataRequest)
        {
            try
            {
                //check token expired
                if (this.isAuthenticatedUser != true)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorNotPermisionFunc)
                    });
                }
                var strRoleIDs = (List<string>)JsonConvert.DeserializeObject(dataRequest.RoleID, typeof(List<string>));
                var roles = this.Repository.GetQuery<AD_ROLE>()
                    .Where(r => strRoleIDs.Any(p => r.ID.Equals(p)));

                var page = !string.IsNullOrEmpty(dataRequest.PageUrl)
                    ? this.Repository.GetQuery<AD_PAGE>().FirstOrDefault(r => r.ID.Equals(dataRequest.PageUrl))
                    : null;

                if (page is null || roles is null || roles.Count() == 0)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail
                    });
                }

                //SaveRoleControl(controlSQL, dataRequest.Controls, page, roles.ToList());

                var controlByPages = this.Repository.GetQuery<AD_CONTROL>()
                        .Where(r => r.PageID == dataRequest.PageUrl)
                        .ToList();

                if (roles.Any(r => r.DefaultFlg == true))
                {
                    var result = controlByPages
                        .Select(r => new
                        {
                            ID = r.ControlID,
                            ActiveFlg = true
                        })
                        .ToList();

                    if (!result.Any(r => r.ID == "AddNew")) { result.Add(new { ID = "AddNew", ActiveFlg = true }); }
                    if (!result.Any(r => r.ID == "Edit")) { result.Add(new { ID = "Edit", ActiveFlg = true }); }
                    if (!result.Any(r => r.ID == "Save")) { result.Add(new { ID = "Save", ActiveFlg = true }); }
                    if (!result.Any(r => r.ID == "Delete")) { result.Add(new { ID = "Delete", ActiveFlg = true }); }

                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Ok,
                        Data = result
                    });
                }

                var rolePages = this.Repository.GetQuery<AD_ROLE_PAGE>().Where(r => r.PageID.Equals(page.ID))
                    .Join(roles, a => a.RoleID, b => b.ID, (a, b) => a)
                    .ToList();

                List<int> controlIDs = new List<int>();
                if (rolePages != null && rolePages.Count > 0)
                {
                    List<int> _tempControlIDs = null;
                    foreach (var rolePage in rolePages)
                    {
                        if (!string.IsNullOrEmpty(rolePage.Value))
                        {
                            _tempControlIDs = (List<int>)JsonConvert.DeserializeObject(rolePage.Value, typeof(List<int>));
                            if (_tempControlIDs != null)
                            {
                                foreach (var _tempControleIDItem in _tempControlIDs)
                                {
                                    if (!controlIDs.Any(r => r.Equals(_tempControleIDItem)))
                                    {
                                        controlIDs.Add(_tempControleIDItem);
                                    }
                                }
                            }
                        }
                    }
                }

                if (controlIDs != null && controlIDs.Count > 0)
                {
                    var result = controlByPages
                        .Select(r => new
                        {
                            ID = r.ControlID,
                            ActiveFlg = controlIDs.Any(a => r.ID == a)
                        })
                        .ToList();

                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Ok,
                        Data = result
                    });
                }

                return Json(new TResult()
                {
                    Status = (short)EStatus.Fail
                   
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                return BadRequest(e.Message);
            }
        }

        //private void SaveRoleControl(IQueryable<AD_CONTROL> controlSQL, List<string> controls, AD_PAGE page, List<AD_ROLE> roles)
        //{
        //    var flag = false;
        //    var pageID = page.ID;
        //    var rolePageValue = JsonConvert.SerializeObject(controls);
        //    if (string.IsNullOrEmpty(page.Value) || !page.Value.Equals(rolePageValue))
        //    {
        //        flag = true;
        //        page.Value = rolePageValue;
        //        this.Repository.Update(page);
        //    }
        //    var controlDBs = controlSQL.ToList();
            
        //    foreach (var item in controls)
        //    {
        //        if (!controlDBs.Any(r => r.ID.Equals(item)))
        //        {
        //            flag = true;
        //            this.Repository.Add(new AD_CONTROL()
        //            {
        //                ID = item,
        //                Name = item
        //            });
        //        }
        //    }

        //    AD_ROLE_PAGE rolePage = null;
        //    foreach (var role in roles)
        //    {
        //        rolePage = this.Repository.GetQuery<AD_ROLE_PAGE>().FirstOrDefault(r => r.RoleID.Equals(role.ID) && r.PageID.Equals(pageID));
        //        if (rolePage == null)
        //        {
        //            rolePage = new AD_ROLE_PAGE()
        //            {
        //                RoleID = role.ID,
        //                PageID = pageID,
        //                Value = rolePageValue,
        //                CreateAt = DateTime.Now,
        //                UpdateAt = DateTime.Now,
        //                ActiveFlg = true
        //            };

        //            flag = true;
        //            this.Repository.Add(rolePage);
        //        }
        //        else if (rolePage.ActiveFlg == true && string.IsNullOrEmpty(rolePage.Value))
        //        {
        //            flag = true;
        //            rolePage.Value = rolePageValue;
        //            this.Repository.Update(rolePage);
        //        }
        //    }

        //    if (flag)
        //        this.Repository.UnitOfWork.SaveChanges();
        //}
    }
}
