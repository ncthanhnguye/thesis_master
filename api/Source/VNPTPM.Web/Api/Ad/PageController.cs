using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VNPTPM.Model;
using VNPTPM.Model.Commons;
using VNPTPM.Model.Core;
using VNPTPM.Model.Validate;
using VNPTPM.Model.VM;
using VNPTPM.Web.Api.Base;
using System.Web.Configuration;

namespace VNPTPM.Web.Api.Ad
{
    [RoutePrefix("api/Page")]
    public class PageController : BaseController
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
                var result = this.Repository.GetQuery<AD_PAGE>().FirstOrDefault(r => r.ID == id);
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
                //check token expired
                if (this.isAuthenticatedUser != true)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorNotPermisionFunc)
                    });
                }
                var isAll = string.IsNullOrEmpty(searchText);

                var pages = this.Repository.GetQuery<AD_PAGE>()
                    .OrderByDescending(r => r.OrdinalNumber);
                var result = pages.GroupJoin(pages, a => a.ParentID, b => b.ID, (a, b) => new
                {
                    a.ID,
                    Name = a.Name,
                    a.ParentID,
                    a.OrdinalNumber,
                    a.MenuFlg,
                    a.ButtonFlg,
                    a.Icon,
                    ParentName = b.FirstOrDefault().Name,
                    a.FileUrls
                })
                      .Where(r => (isAll || r.Name.Contains(searchText)))
                .OrderBy(r => r.OrdinalNumber)
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

        [HttpPut]
        public IHttpActionResult Put(string id, [FromBody] AD_PAGE dataRequest)
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
                string errorMsg = null;

                var dataItem = this.Repository.GetQuery<AD_PAGE>().FirstOrDefault(r => r.ID == id);
                if (dataItem is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
                }
                string fileUrls = dataItem.FileUrls;
                dataItem = dataRequest.Clone();
                dataItem.ID = id;
                dataItem.UpdateAt = DateTime.Now;
                dataItem.UpdateBy = VNPTHelper.GetUserName();


                if (fileUrls != dataRequest.FileUrls)
                {
                    if (dataItem.FileUrls != "" && dataItem.FileUrls != "\"\"")
                    {
                        string links = dataItem.FileUrls;
                        string userCurrent = VNPTHelper.GetUserName();
                        try
                        {
                            string sourcePath = string.Format(Constants.WO_PATH_SAVE_FILE_TMP, userCurrent);
                            string targetPath = string.Format(Constants.WO_PATH_SAVE_PAGE, dataRequest.ID.Replace("/", "_"));
                            string fileNm = Path.GetFileName(links);
                            fileNm = VNPTHelper.CoppyFile(sourcePath, targetPath, fileNm);
                            if (fileNm.Equals(EStatusFile.PathNotExist.ToString()))
                            {
                                errorMsg = String.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                                    VNPTResources.ID.MsgErrorNotFoundPath), sourcePath), ". ");
                            }
                            else
                            {
                                links = string.Concat(targetPath, fileNm);
                            }
                        }
                        catch (Exception e)
                        {
                            this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                            errorMsg = String.Concat(errorMsg, e.Message, ". ");
                        }

                        if (errorMsg != "" && errorMsg != null)
                        {
                            return Json(new TResult()
                            {
                                Status = (short)EStatus.Fail,
                                Msg = errorMsg
                            });
                        }

                        dataItem.FileUrls = links;
                    }
                    else
                    {
                        dataItem.FileUrls = null;
                    }
                }


                //-----------------------------------------------------------------
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

        [HttpGet]
        public IHttpActionResult Get(String iD, string langID)
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
                var dataItem = this.Repository.GetQuery<AD_PAGE>()
                    .FirstOrDefault(r => r.ID == iD);
                if (dataItem is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = $"{VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)}_DATA_ITEM"
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
        public IHttpActionResult Post([FromBody] AD_PAGE dataRequest)
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
                string errorMsg = null;
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
                if (this.Repository.GetQuery<AD_PAGE>().Any(r => r.ID.Equals(dataRequest.ID)))
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorIsExists), dataRequest.ID)
                    });
                }

                var dataItem = dataRequest.Clone();
                dataItem.CreateAt = DateTime.Now;
                dataItem.CreateBy = VNPTHelper.GetUserName();
                dataItem.UpdateAt = DateTime.Now;
                dataItem.UpdateBy = VNPTHelper.GetUserName();

                string userCurrent = VNPTHelper.GetUserName();
                //AvatarUpload
                if (dataItem.FileUrls != "" && dataItem.FileUrls != "\"\"")
                {
                    string links = dataItem.FileUrls;


                    if (!string.IsNullOrEmpty(links))
                    {
                        try
                        {
                            string sourcePath = string.Format(Constants.WO_PATH_SAVE_FILE_TMP, userCurrent);
                            string targetPath = string.Format(Constants.WO_PATH_SAVE_PAGE, dataRequest.ID.Replace("/","_") );
                            string fileNm = Path.GetFileName(links);
                            fileNm = VNPTHelper.CoppyFile(sourcePath, targetPath, fileNm);
                            if (fileNm.Equals(EStatusFile.PathNotExist.ToString()))
                            {
                                errorMsg = String.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                                    VNPTResources.ID.MsgErrorNotFoundPath), sourcePath), ". ");
                            }
                            else
                            {
                                links = string.Concat(targetPath, fileNm);
                            }
                        }
                        catch (Exception e)
                        {
                            this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                            errorMsg = String.Concat(errorMsg, e.Message, ". ");
                        }

                        if (errorMsg != "" && errorMsg != null)
                        {
                            return Json(new TResult()
                            {
                                Status = (short)EStatus.Fail,
                                Msg = errorMsg
                            });
                        }
                    }

                    dataItem.FileUrls = links;
                }
                else
                {
                    dataItem.FileUrls = null;
                }
                //-------------------------------------------------------------------AvatarUpload
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

        [HttpPost]
        [Route("Saves")]
        public IHttpActionResult Saves(IEnumerable<AD_PAGE> dataRequest)
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
                AD_PAGE dataItem = null;
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

                    dataItem = this.Repository.GetQuery<AD_PAGE>().FirstOrDefault(r => r.ID == item.ID);
                    if (dataItem is null)
                    {
                        dataItem = item.Clone();
                        dataItem.CreateAt = DateTime.Now;
                        dataItem.CreateBy = VNPTHelper.GetUserName();

                        this.Repository.Add(dataItem);
                        this.VNPTLogs.Write(this.Repository, EAction.Insert, JsonConvert.SerializeObject(dataItem));
                    }
                    else
                    {
                        return Json(new TResult()
                        {
                            Status = (short)EStatus.Fail,
                            Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorIsExists), item.ID)
                        });
                    }
                }

                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgUpdateDataSuccess),
                        VNPTResources.Instance.Get(VNPTResources.ID.Label_obj_page)),
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.Repository, e.Message);
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
                if (string.IsNullOrEmpty(dataRequest.IDList))
                {
                    return Json(new TResult()
                    {
                        Status = (int)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorRequiredDeleteID)
                    });
                }
                var temp = this.Repository.GetQuery<AD_PAGE>().FirstOrDefault(r => r.ID == dataRequest.IDList);
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

        [HttpGet]
        [Route("GetMenu")]
        public IHttpActionResult GetMenu(string roleID)
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
                var strRoleIDs = (List<string>)JsonConvert.DeserializeObject(roleID, typeof(List<string>));
                var roles = this.Repository.GetQuery<AD_ROLE>()
                    .Where(r => strRoleIDs.Any(p => r.ID.Equals(p)));

                //var role = this.Repository.GetQuery<AD_ROLE>().FirstOrDefault(r => r.ID.Equals(roleID));
                if (roles.Count() == 0)
                {
                    return Json(new TResult()
                    {
                        Status = (int)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
                }

                var pages = this.Repository.GetQuery<AD_PAGE>().Select(r =>
                       new
                       {
                           r.ID,
                           r.Name,
                           r.ButtonFlg,
                           r.Icon,
                           r.MenuFlg,
                           r.OrdinalNumber,
                           r.ParentID,
                       });
                
                var pageItem = pages;
                if (!roles.Any(r => r.DefaultFlg == true))
                {
                    pageItem = this.Repository.GetQuery<AD_ROLE_PAGE>().Where(r => r.ActiveFlg == true)
                        .Join(roles, a => a.RoleID, b => b.ID, (a, b) => a)
                        .Join(pages, a => a.PageID, b => b.ID, (a, b) => b);
                }

                var menus = pageItem.Where(r => r.ParentID != null && r.ButtonFlg != true && r.MenuFlg == true)
                    .GroupBy(r => r.ParentID)
                    .Select(r => new
                    {
                        ParentID = r.Key,
                        Childrens = r.OrderBy(p => p.OrdinalNumber).Select(p => new
                        {
                            p.Name,
                            p.ID,
                            p.OrdinalNumber
                        }).Distinct()
                    })
                    .Join(pages, a => a.ParentID, b => b.ID, (a, b) => new { a, b })
                    .ToList()
                    .Distinct()
                    .Select(r => new MenuVM()
                    {
                        Name = r.b.Name,
                        Url = r.b.ID,
                        OrderIndex = r.b.OrdinalNumber,
                        Childrens = r.a.Childrens.OrderBy(p => p.OrdinalNumber).Select(p => new MenuVM()
                            {
                                Name = p.Name,
                                Url = p.ID,
                            })
                            .ToList(),
                        IsButton = false,
                        Icon = r.b.Icon
                    });

                var menuButtons = pageItem.Where(r => r.ButtonFlg == true)
                    .ToList()
                    .Distinct()
                    .AsEnumerable().Select(r => new MenuVM()
                    {
                        Name = r.Name,
                        Url = r.ID,
                        OrderIndex = r.OrdinalNumber,
                        Childrens = new List<MenuVM>(),
                        IsButton = true,
                        Icon = r.Icon,
                    });

                var result = menus.Union(menuButtons).OrderBy(r => r.OrderIndex).ToList();

                if (result.Count == 0)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgAccessDenined)
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
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("Check")]
        public IHttpActionResult Check(string roleID, string url)
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
                var strRoleIDs = (List<string>)JsonConvert.DeserializeObject(roleID, typeof(List<string>));
                var roles = this.Repository.GetQuery<AD_ROLE>()
                    .Where(r => strRoleIDs.Any(p => r.ID.Equals(p)));

                //var role = this.Repository.GetQuery<AD_ROLE>().FirstOrDefault(r => r.ID.Equals(roleID));
                if (roles == null || roles.Count() == 0)
                {
                    return Json(new TResult()
                    {
                        Status = (int)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
                }

                if (roles.Any(r => r.DefaultFlg == true))
                {
                    return Json(new TResult()
                    {
                        Status = (int)EStatus.Ok
                    });
                }

                var rolePages = this.Repository.GetQuery<AD_ROLE_PAGE>().Where(r => r.PageID.Equals(url) && r.ActiveFlg == true)
                    .Join(roles, a => a.RoleID, b => b.ID, (a, b) => a)
                    .ToList();

                if (rolePages.Count > 0)
                {
                    return Json(new TResult()
                    {
                        Status = (int)EStatus.Ok
                    });
                }

                return Json(new TResult()
                {
                    Status = (short)EStatus.Fail,
                    Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgAccessDenined)
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                return BadRequest(e.Message);
            }
        }

        private void GetValidate(AD_PAGE item, ref string errorMsg)
        {
            if (string.IsNullOrEmpty(item.ID) || string.IsNullOrWhiteSpace(item.ID))
            {
                errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.Label_page_ID)), ". ");
            }
            else
            {
                if (!CustomValidation.maxLength(150, item.ID))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.Label_page_ID), 150), ". ");
                }
                if (CustomValidation.hasSpace(item.ID))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorHasSpace), VNPTResources.Instance.Get(VNPTResources.ID.Label_page_ID)), ". ");
                }
            }

            if (string.IsNullOrEmpty(item.Name) || string.IsNullOrEmpty(item.Name))
            {
                errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorRequire), VNPTResources.Instance.Get(VNPTResources.ID.Label_page_Nm)), ". ");
            }
            else
            {
                if (!CustomValidation.maxLength(150, item.Name))
                {
                    errorMsg = string.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                        VNPTResources.ID.MsgErrorMaximumLength), VNPTResources.Instance.Get(VNPTResources.ID.Label_page_Nm), 150), ". ");
                }
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetName")]
        public IHttpActionResult GetName(string id)
        {
            try
            {
                string name = this.Repository.GetQuery<AD_PAGE>().FirstOrDefault(r => r.ID == id)?.Name;
                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Data = !string.IsNullOrEmpty(name) ? name : string.Empty
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                throw new Exception("", e);
            }
        }

      


        [HttpDelete]
        public IHttpActionResult Delete(string iD)
        {
            try
            {
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
                var dataItem = this.Repository.GetQuery<AD_PAGE>().FirstOrDefault(r => iD == r.ID);
                if (dataItem == null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
                }

                this.Repository.Delete(dataItem);
                this.VNPTLogs.Write(this.Repository, EAction.Delete, JsonConvert.SerializeObject(dataItem));

                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgDeleteOk)
                });
            }
            catch (Exception e)
            {
                throw new Exception("", e);
            }
        }
        
    }
}
