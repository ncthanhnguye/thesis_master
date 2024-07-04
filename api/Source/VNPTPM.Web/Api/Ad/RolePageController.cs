using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    [RoutePrefix("api/RolePage")]
    public class RolePageController : BaseController
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
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorNotPermisionFunc)
                    });
                }
                var role = this.Repository.GetQuery<AD_ROLE>().FirstOrDefault(r => r.ID.Equals(id));
                if (role is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
                }

                var controls = this.Repository.GetQuery<AD_CONTROL>().ToList();

                var rolePages = this.Repository.GetQuery<AD_PAGE>()
                    .GroupJoin(this.Repository.GetQuery<AD_ROLE_PAGE>().Where(r => r.RoleID.Equals(id)),
                    a => a.ID, b => b.PageID, (a, b) => new
                    {
                        AD_PAGE = a,
                        AD_ROLE_PAGE = b.FirstOrDefault()
                    })
                    .ToList();

                var result = new List<object>();
                List<AD_CONTROL> controlByPages = null;
                List<int> controlIDs = null;
                var activeFlg = false;
                var flag = false;
                foreach (var rolePageItem in rolePages)
                {
                    result.Add(new
                    {
                        ID = rolePageItem.AD_PAGE.ID,
                        Name = rolePageItem.AD_PAGE.Name,
                        ActiveFlg = rolePageItem.AD_ROLE_PAGE != null && rolePageItem.AD_ROLE_PAGE.ActiveFlg == true
                    });

                    controlByPages = controls.Where(r => rolePageItem.AD_PAGE.ID.Equals(r.PageID))
                        .ToList();
                    controlIDs = null;

                    if (rolePageItem.AD_ROLE_PAGE != null && !string.IsNullOrEmpty(rolePageItem.AD_ROLE_PAGE.Value))
                    {
                        controlIDs = (List<int>)JsonConvert.DeserializeObject(rolePageItem.AD_ROLE_PAGE.Value, typeof(List<int>));
                    }

                    flag = false;
                    foreach (var controlItem in controlByPages)
                    {
                        activeFlg = controlIDs != null && controlIDs.Count > 0
                                ? controlIDs.Any(r => controlItem.ID == r)
                                : false;

                        if (!flag && activeFlg)
                        {
                            flag = true;
                        }

                        result.Add(new
                        {
                            ParentID = rolePageItem.AD_PAGE.ID,
                            ParentName = rolePageItem.AD_PAGE.Name,
                            ID = controlItem.ID,
                            Name = controlItem.Name,
                            ActiveFlg = activeFlg
                        });
                    }

                    //if (flag)
                    //{
                    //    result.Add(new
                    //    {
                    //        ID = rolePageItem.AD_PAGE.ID,
                    //        Name = rolePageItem.AD_PAGE.Name,
                    //        ActiveFlg = true
                    //    });
                    //}
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

        [HttpPost]
        public IHttpActionResult Post([FromBody] AD_ROLE_PAGE dataRequest)
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
                    string msg = VNPTResources.Instance.Get(model);

                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = msg
                    });
                }

                var dataItem = this.Repository.GetQuery<AD_ROLE_PAGE>()
                    .FirstOrDefault(r => r.PageID.Equals(dataRequest.PageID) && r.RoleID.Equals(dataRequest.RoleID));
                var controlSelectedIDs = new List<int>();
                if (string.IsNullOrEmpty(dataRequest.Value))
                {
                    // là chọn tất cả các nút trong trang
                    controlSelectedIDs = this.Repository.GetQuery<AD_CONTROL>()
                        .Where(r => r.PageID.Equals(dataRequest.PageID))
                        .Select(r => r.ID)
                        .ToList();
                }
                else
                {
                    controlSelectedIDs.Add(int.Parse(dataRequest.Value));
                }

                if (dataRequest.ActiveFlg == true)
                {
                    // Chọn
                    if (dataItem == null)
                    {
                        dataItem = new AD_ROLE_PAGE()
                        {
                            RoleID = dataRequest.RoleID,
                            PageID = dataRequest.PageID,
                            CreateAt = DateTime.Now,
                            ActiveFlg = true,
                            Value = JsonConvert.SerializeObject(controlSelectedIDs)
                        };

                        this.Repository.Add(dataItem);
                        this.VNPTLogs.Write(this.Repository, EAction.Insert, JsonConvert.SerializeObject(dataItem));
                    }
                    else
                    {
                        List<int> controlIDs = null;
                        if (!string.IsNullOrEmpty(dataItem.Value))
                        {
                            controlIDs = (List<int>)JsonConvert.DeserializeObject(dataItem.Value, typeof(List<int>));
                        }

                        // nếu cột value chưa có nút nào
                        if (controlIDs == null || controlIDs.Count() == 0)
                        {
                            dataItem.Value = JsonConvert.SerializeObject(controlSelectedIDs);
                        }
                        else
                        {
                            foreach (var item in controlSelectedIDs)
                            {
                                if (!controlIDs.Any(r => r == item))
                                {
                                    controlIDs.Add(item);
                                }
                            }

                            dataItem.UpdateAt = DateTime.Now;
                            dataItem.Value = JsonConvert.SerializeObject(controlIDs);
                            this.Repository.Update(dataItem);
                            this.VNPTLogs.Write(this.Repository, EAction.Update, JsonConvert.SerializeObject(dataItem));
                        }
                    }

                    this.Repository.UnitOfWork.SaveChanges();
                }
                else
                {
                    // Gỡ
                    if (dataItem != null)
                    {
                        List<int> controlIDs = new List<int>();
                        if (!string.IsNullOrEmpty(dataItem.Value))
                        {
                            controlIDs = (List<int>)JsonConvert.DeserializeObject(dataItem.Value, typeof(List<int>));
                        }

                        foreach (var item in controlSelectedIDs)
                        {
                            if (controlIDs.Any(r => r == item))
                            {
                                controlIDs.Remove(item);
                            }
                        }

                        if (controlIDs.Count == 0)
                        {
                            this.Repository.Delete(dataItem);
                            this.VNPTLogs.Write(this.Repository, EAction.Delete, JsonConvert.SerializeObject(dataItem));
                        }
                        else
                        {
                            dataItem.UpdateAt = DateTime.Now;
                            dataItem.Value = JsonConvert.SerializeObject(controlIDs);
                            this.Repository.Update(dataItem);
                            this.VNPTLogs.Write(this.Repository, EAction.Update, JsonConvert.SerializeObject(dataItem));
                        }

                        this.Repository.UnitOfWork.SaveChanges();
                    }
                }

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgSaveOk),
                    Data = dataItem
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.Repository, e.Message);
                throw new Exception("", e);
            }
        }

        [HttpPost]
        [Route("GetRolePage")]
        public IHttpActionResult GetRolePage(RoleReqVM dataRequest)
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
                //validate custom data input
                string errorMsg = null;
                //RoleID
                if (string.IsNullOrEmpty(dataRequest.RoleID) || string.IsNullOrWhiteSpace(dataRequest.RoleID))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorRequire),
                        VNPTResources.Instance.Get(VNPTResources.ID.Label_RolePage_RoleID)), ". ");
                }
                //RoleID
                if (string.IsNullOrEmpty(dataRequest.PageID) || string.IsNullOrWhiteSpace(dataRequest.PageID))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorRequire),
                        VNPTResources.Instance.Get(VNPTResources.ID.Label_RolePage_PageID)), ". ");

                }

                //validate custom data input
                if (errorMsg != "" && errorMsg != null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = errorMsg
                    });
                }

                var rolePages = this.Repository.GetQuery<AD_ROLE_PAGE>()
                    .FirstOrDefault(r => r.RoleID == dataRequest.RoleID && r.PageID == dataRequest.PageID && r.ActiveFlg == true);

                string roleStr = "";
                if (rolePages != null)
                {
                    roleStr = rolePages.Value;
                }

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Data = roleStr
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.Repository, e.Message);
                throw new Exception("", e);
            }
        }

        //[HttpPost]
        //public IHttpActionResult Saves(AD_ROLE_PAGE item)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            var model = ModelState;
        //            string msg = VNPTResources.Instance.Get(model);

        //            return Json(new TResult()
        //            {
        //                Status = (short)EStatus.Fail,
        //                Msg = msg
        //            });
        //        }
        //        var aDRolePage = this.Repository.GetQuery<AD_ROLE_PAGE>();
        //        AD_ROLE_PAGE dataItem = null;
        //        string errorMsg = null;
        //        GetValidate(item, ref errorMsg);
        //        if (!string.IsNullOrEmpty(errorMsg))
        //        {
        //            return Json(new TResult()
        //            {
        //                Status = (int)EStatus.Fail,
        //                Msg = errorMsg
        //            });
        //        }
        //        var roleCount = this.Repository.GetQuery<AD_ROLE>().Where(x => x.ID.Equals(item.RoleID))                    
        //        .Select(r => new
        //        {
        //            RoleID = r.ID,
        //            PageID = item.PageID,
        //        }).Join(this.Repository.GetQuery<AD_PAGE>(), a => a.PageID, b => b.ID,
        //        (a, b) => new
        //        {
        //            AD_ROLE = a,
        //            AD_PAGE = b
        //        })
        //        .Select(r => new
        //        {
        //            r.AD_ROLE.RoleID,
        //            r.AD_ROLE.PageID,
        //            PageValue = r.AD_PAGE.Value
        //        })
        //        .Where(x => x.PageID.Equals(item.PageID))
        //        .FirstOrDefault();

        //        if (roleCount != null)
        //        {
        //            dataItem = aDRolePage.FirstOrDefault(r => r.RoleID.Equals(item.RoleID) && r.PageID.Equals(item.PageID));
        //            if (dataItem is null)
        //            {
        //                if (!string.IsNullOrEmpty(item.Value))
        //                {
        //                    var value = new string[] { item.Value };
        //                    item.Value = (string)JsonConvert.SerializeObject(value);
        //                }                            
        //                if (string.IsNullOrEmpty(item.Value) && !string.IsNullOrEmpty(roleCount.PageValue))
        //                {
        //                    item.Value = roleCount.PageValue;
        //                }
        //                dataItem = item.Clone();
        //                this.Repository.Add(dataItem);
        //                this.VNPTLogs.Write(this.Repository, EAction.Insert, JsonConvert.SerializeObject(dataItem));
        //            }
        //            else
        //            {
        //                var valueNew = (string[])JsonConvert.DeserializeObject(dataItem.Value, typeof(string[]));

        //                if (item.ActiveFlg == true)
        //                {
        //                    if (valueNew != null)
        //                    {                                
        //                        if (!string.IsNullOrEmpty(item.Value))
        //                        {
        //                            var value = new string[valueNew.Length + 1];
        //                            for (int i = 0; i <= valueNew.Length; i++)
        //                            {
        //                                if (i == valueNew.Length)
        //                                {
        //                                    value[i] = item.Value;
        //                                }
        //                                else
        //                                {
        //                                    value[i] = valueNew[i];
        //                                }
        //                            }
        //                            item.Value = (string)JsonConvert.SerializeObject(value);
        //                        }
        //                        if (string.IsNullOrEmpty(item.Value) && !string.IsNullOrEmpty(roleCount.PageValue))
        //                        {
        //                            item.Value = roleCount.PageValue;
        //                        }

        //                    }
        //                    else
        //                    {
        //                        if (!string.IsNullOrEmpty(item.Value))
        //                        {
        //                            var value = new string[] { item.Value };
        //                            item.Value = (string)JsonConvert.SerializeObject(value);
        //                        }
        //                        if (string.IsNullOrEmpty(item.Value) && !string.IsNullOrEmpty(roleCount.PageValue))
        //                        {
        //                            item.Value = roleCount.PageValue;
        //                        }                                
        //                    }                            
        //                }
        //                else
        //                {

        //                    var value = new string[0];

        //                    if (!string.IsNullOrEmpty(item.Value))
        //                    {
        //                        value = new string[valueNew.Length - 1];
        //                        var j = 0;
        //                        for (int i = 0; i < valueNew.Length; i++)
        //                        {
        //                            if (valueNew[i] != item.Value)
        //                            {
        //                                value[j] = valueNew[i];
        //                                j++;
        //                            }
        //                        }
        //                    }
        //                    if (string.IsNullOrEmpty(item.Value) && !string.IsNullOrEmpty(roleCount.PageValue))
        //                    {
        //                        item.Value = roleCount.PageValue;
        //                    }

        //                    item.Value = (string)JsonConvert.SerializeObject(value);
        //                    if (value.Length > 0)
        //                    {
        //                        item.ActiveFlg = dataItem.ActiveFlg;
        //                    }
        //                }

        //                dataItem = item.Clone();

        //                this.Repository.Update(dataItem);
        //                this.VNPTLogs.Write(this.Repository, EAction.Update, JsonConvert.SerializeObject(dataItem));
        //            }

        //            this.Repository.UnitOfWork.SaveChanges();
        //        }


        //        return Json(new TResult()
        //        {
        //            Status = (short)EStatus.Ok,
        //            Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgSaveOk),
        //        });
        //    }
        //    catch (Exception e)
        //    {
        //        this.VNPTLogs.Write(this.Repository, e.Message);
        //        return Json(new TResult()
        //        {
        //            Status = (short)EStatus.Fail,
        //            Msg = e.Message
        //        });
        //    }
        //}

        //private void GetValidate(AD_ROLE_PAGE item, ref string errMsg)
        //{
        //    if (string.IsNullOrEmpty(item.RoleID) || string.IsNullOrWhiteSpace(item.RoleID))
        //    {
        //        errMsg = string.Concat(errMsg, string.Format(VNPTResources.Instance.Get(
        //                VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.Label_RolePage_RoleID)), ". ");
        //    }
        //    else
        //    {
        //        if (!CustomValidation.maxLength(50, item.RoleID))
        //        {
        //            errMsg = string.Concat(errMsg, string.Format(VNPTResources.Instance.Get(
        //                VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.Label_RolePage_RoleID), 50), ". ");
        //        }
        //    }

        //    if (string.IsNullOrEmpty(item.PageID) || string.IsNullOrWhiteSpace(item.RoleID))
        //    {
        //        errMsg = string.Concat(errMsg, string.Format(VNPTResources.Instance.Get(
        //                VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.Label_RolePage_PageID)), ". ");
        //    }
        //    else
        //    {
        //        if (!CustomValidation.maxLength(50, item.PageID))
        //        {
        //            errMsg = string.Concat(errMsg, string.Format(VNPTResources.Instance.Get(
        //                VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.Label_RolePage_PageID), 50), ". ");
        //        }
        //    }
        //}
        
        //FOR-APP
        //[HttpPost]
        //[Route("CheckRolePage")]
        //public IHttpActionResult CheckRolePage(RoleReqVM dataRequest)
        //{
        //    try
        //    {
        //        //validate custom data input
        //        string errorMsg = null;
        //        //PageID
        //        if (string.IsNullOrEmpty(dataRequest.PageID) || string.IsNullOrWhiteSpace(dataRequest.PageID))
        //        {
        //            errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorRequire),
        //                VNPTResources.Instance.Get(VNPTResources.ID.Label_RolePage_PageID)), ". ");
        //        }

        //        //ControlStr
        //        if (string.IsNullOrEmpty(dataRequest.ControlStr) || string.IsNullOrWhiteSpace(dataRequest.ControlStr))
        //        {
        //            errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorRequire),
        //                VNPTResources.Instance.Get(VNPTResources.ID.Label_RolePage_ControlStr)), ". ");
        //        }

        //        //validate custom data input
        //        if (errorMsg != "" && errorMsg != null)
        //        {
        //            return Json(new TResult()
        //            {
        //                Status = (short)EStatus.Fail,
        //                Msg = errorMsg
        //            });
        //        }

        //        //Add control
        //        List<string> listControl = (List<string>)JsonConvert.DeserializeObject(dataRequest.ControlStr, typeof(List<string>));
        //        List<string> listControlID = new List<string>();
        //        foreach (string item in listControl)
        //        {
        //            string[] controlTmp = (!string.IsNullOrEmpty(item)) ? item.Split(';') : null;
        //            if (controlTmp != null && controlTmp.Count() == 2)
        //            {
        //                string controlTmpID = controlTmp[0].Trim();
        //                string controlTmpNm = controlTmp[1].Trim();

        //                var controlItem = this.Repository.GetQuery<AD_CONTROL>().FirstOrDefault(r => r.ID == controlTmpID);
        //                if (controlItem == null)
        //                {
        //                    AD_CONTROL controlNew = new AD_CONTROL();
        //                    controlNew.ID = controlTmpID;
        //                    controlNew.Name = controlTmpNm;

        //                    this.Repository.Add<AD_CONTROL>(controlNew);
        //                    this.VNPTLogs.Write(this.Repository, EAction.Insert, JsonConvert.SerializeObject(controlNew));
        //                }
        //                listControlID.Add(controlTmpID);
        //            }
        //        }

        //        if (listControlID.Count > 0)
        //        {
        //            dataRequest.ControlStr = JsonConvert.SerializeObject(listControlID);
        //        }
        //        else
        //        {
        //            dataRequest.ControlStr = "";
        //        }

        //        var page = this.Repository.GetQuery<AD_PAGE>().FirstOrDefault(r => r.ID == dataRequest.PageID);

        //        if (page != null)
        //        {
        //            page.Value = (!string.IsNullOrEmpty(dataRequest.ControlStr) && !string.IsNullOrWhiteSpace(dataRequest.ControlStr)) ? dataRequest.ControlStr : null;
        //            this.Repository.Update<AD_PAGE>(page);
        //            this.VNPTLogs.Write(this.Repository, EAction.Update, JsonConvert.SerializeObject(page));
        //        }
        //        else
        //        {
        //            AD_PAGE pageNew = new AD_PAGE();
        //            pageNew.ID = dataRequest.PageID;
        //            pageNew.Name = dataRequest.PageNm;
        //            pageNew.MenuFlg = true;
        //            pageNew.Value = dataRequest.ControlStr;

        //            this.Repository.Add<AD_PAGE>(pageNew);
        //            this.VNPTLogs.Write(this.Repository, EAction.Insert, JsonConvert.SerializeObject(pageNew));
        //        }

        //        this.Repository.UnitOfWork.SaveChanges();

        //        return Json(new TResult()
        //        {
        //            Status = (short)EStatus.Ok,
        //            Data = null
        //        });
        //    }
        //    catch (Exception e)
        //    {
        //        this.VNPTLogs.Write(this.Repository, e.Message);
        //        return Json(new TResult()
        //        {
        //            Status = (short)EStatus.Fail,
        //            Msg = e.Message
        //        });
        //    }
        //}

    }
}
