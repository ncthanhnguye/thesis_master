using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using VNPTPM.Model;
using VNPTPM.Model.Commons;
using VNPTPM.Model.Core;
using VNPTPM.Model.Validate;
using VNPTPM.Model.VM;
using VNPTPM.Web.Api.Base;

namespace VNPTPM.Web.Api.M
{
    [RoutePrefix("api/Unit")]
    public class UnitController : BaseController
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
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorNotPermisionFunc)
                    });
                }
                var unit = this.Repository.GetQuery<M_UNIT>().AsEnumerable()
                    .Where(r => r.DelFlg != true)
                    .AsEnumerable();

                var unitIDLogin = "";
                var unitByLogins = VNPTHelper.GetUnitByChilds(this.Repository, ref unitIDLogin).AsEnumerable();

                var result = unitByLogins
                    .OrderBy(r => r.Name)
                    .AsEnumerable()
                    .Select(r => new
                    {
                        r.ID,
                        r.Name,
                        ParentID = r.ID == unitIDLogin ? null : r.ParentID,
                        r.DelFlg,
                        ParentName = unit.FirstOrDefault(x => x.ID == r.ParentID)?.Name,
                        r.Email,
                        r.Phone,
                        r.ImageUrl
                    })
                    //.GroupJoin(this.Repository.GetQuery<DATA_ACCOUNT>().Where(r => r.DelFlg != true),
                    //    a => a.ID, b => b.UnitID, (a, b) => new
                    //    {
                    //        a,
                    //        b
                    //    })
                    //.SelectMany(temp => temp.b.DefaultIfEmpty(), (temp, b) => new
                    //{
                    //    M_UNIT = temp.a,
                    //    DATA_ACCOUNT = b
                    //})
                    //.GroupBy(r => r.M_UNIT)
                    //.Select(r => new
                    //{
                    //    r.Key.ID,
                    //    r.Key.Name,
                    //    r.Key.ParentID,
                    //    r.Key.DelFlg,
                    //    r.Key.ParentName,
                    //    r.Key.Email,
                    //    r.Key.Phone,
                    //    r.Key.ImageUrl,
                    //    AccountCnt = r.Where(a => a.DATA_ACCOUNT != null && !string.IsNullOrEmpty(a.DATA_ACCOUNT.Name)).Count()
                    //})
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
                this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                throw new Exception("", e);
            }
        }

        [HttpGet]
        public IHttpActionResult Get(string iD)
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
                var dataItem = this.Repository.GetQuery<M_UNIT>()
                    .FirstOrDefault(r => r.ID == iD);
                if (dataItem is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = $"{VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)}_M_UNIT"
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
        public IHttpActionResult Post([FromBody] M_UNIT dataRequest)
        {
            try
            {
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
                if (this.Repository.GetQuery<M_UNIT>().Any(r => r.ID.Equals(dataRequest.ID) && r.DelFlg != true))
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorIsIDUnitExists), dataRequest.ID)
                    });
                }

                //if (this.Repository.GetQuery<M_UNIT>().Any(r => (r.Name.Equals(dataRequest.Name) && r.ParentID == dataRequest.ParentID) && r.DelFlg != true))
                    if (this.Repository.GetQuery<M_UNIT>().Any(r => r.Name.Equals(dataRequest.Name) && r.DelFlg != true))
                    {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgErrUnitNameIsExixts), dataRequest.Name)
                    });
                }

                var dataItem = dataRequest.Clone();
                dataItem.CreateAt = DateTime.Now;
                dataItem.Name = dataRequest.Name.Trim();

                // Ảnh đơn vị
                if (!string.IsNullOrEmpty(dataItem.ImageUrl))
                {
                    string link = dataItem.ImageUrl;
                    var errorMsg = "";

                    try
                    {
                        string sourcePath = string.Format(Constants.WO_PATH_SAVE_FILE_TMP, usernameFromHeader);
                        string targetPath = string.Format(Constants.WO_PATH_SAVE_AVATAR, dataItem.ID);
                        string fileNm = Path.GetFileName(link);
                        fileNm = VNPTHelper.CoppyFile(sourcePath, targetPath, fileNm);
                        if (fileNm.Equals(EStatusFile.PathNotExist.ToString()))
                        {
                            errorMsg = String.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                                VNPTResources.ID.MsgErrorNotFoundPath), sourcePath), ". ");
                        }
                        else
                        {
                            link = string.Concat(targetPath, fileNm);
                        }
                    }
                    catch (Exception e)
                    {
                        this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                        errorMsg = String.Concat(errorMsg, e.Message, ". ");
                    }

                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        return Json(new TResult()
                        {
                            Status = (short)EStatus.Fail,
                            Msg = errorMsg
                        });
                    }

                    dataItem.ImageUrl = link;
                }
                else
                {
                    dataItem.ImageUrl = null;
                }

                this.Repository.Add(dataItem);
                this.VNPTLogs.Write(this.Repository, EAction.Insert, JsonConvert.SerializeObject(dataItem));

                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgSaveOk),
                    Data = dataItem
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                throw new Exception("", e);
            }
        }

        [HttpPut]
        public IHttpActionResult Put(string iD, [FromBody] M_UNIT dataRequest)
        {
            try
            {
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

                //if (this.Repository.GetQuery<M_UNIT>().Where(r => r.ID != iD).Any(r => (r.Name.Equals(dataRequest.Name) && r.ParentID == dataRequest.ParentID) && r.DelFlg != true))
                if (this.Repository.GetQuery<M_UNIT>().Where(r => r.ID != iD).Any(r => r.Name.Equals(dataRequest.Name) && r.DelFlg != true))
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgErrUnitNameIsExixts), dataRequest.Name)
                    });
                }

                var dataItem = this.Repository.GetQuery<M_UNIT>().FirstOrDefault(r => r.ID.Equals(iD));
                if (dataItem is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = $"{VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)}_M_UNIT"
                    });
                }


                dataItem.Name = dataRequest.Name.Trim();
                dataItem.ParentID = dataRequest.ParentID;
                dataItem.Type = dataRequest.Type;
                dataItem.Email = dataRequest.Email;
                dataItem.Phone = dataRequest.Phone;

                //AvatarUpload
                if (!string.IsNullOrEmpty(dataRequest.ImageUrl) && !dataRequest.ImageUrl.Equals(dataItem.ImageUrl))
                {
                    string link = dataRequest.ImageUrl;
                    var errorMsg = "";

                    try
                    {
                        string sourcePath = string.Format(Constants.WO_PATH_SAVE_FILE_TMP, usernameFromHeader);
                        string targetPath = string.Format(Constants.WO_PATH_SAVE_AVATAR, dataItem.ID);
                        string fileNm = Path.GetFileName(link);
                        fileNm = VNPTHelper.CoppyFile(sourcePath, targetPath, fileNm);
                        if (fileNm.Equals(EStatusFile.PathNotExist.ToString()))
                        {
                            errorMsg = String.Concat(errorMsg, string.Format(VNPTResources.Instance.Get(
                                VNPTResources.ID.MsgErrorNotFoundPath), sourcePath), ". ");
                        }
                        else
                        {
                            link = string.Concat(targetPath, fileNm);
                        }
                    }
                    catch (Exception e)
                    {
                        this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                        errorMsg = String.Concat(errorMsg, e.Message, ". ");
                    }

                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        return Json(new TResult()
                        {
                            Status = (short)EStatus.Fail,
                            Msg = errorMsg
                        });
                    }

                    dataItem.ImageUrl = link;
                }
                else
                {
                    dataItem.ImageUrl = null;
                }

                dataItem.UpdateAt = DateTime.Now;

                this.Repository.Update(dataItem);
                this.VNPTLogs.Write(this.Repository, EAction.Update, JsonConvert.SerializeObject(dataItem));

                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgSaveOk),
                    Data = dataItem
                });
            }
            catch (Exception e)
            {
                throw new Exception("", e);
            }
        }

        [HttpDelete]
        public IHttpActionResult Delete(string iD)
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
                var dataItem = this.Repository.GetQuery<M_UNIT>().FirstOrDefault(r => iD == r.ID);
                if (dataItem == null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
                }
                var condition = this.Repository.GetQuery<DATA_ACCOUNT>().FirstOrDefault(r => r.DelFlg != true && r.UnitID == dataItem.ID);
                var unitChild = this.Repository.GetQuery<M_UNIT>().FirstOrDefault(r => r.DelFlg != true && r.ParentID == dataItem.ID);
                if (condition != null || unitChild != null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorUnitDeleteUser), dataItem.Name)
                    });
                }

                //dataItem.UpdateAt = DateTime.Now;
                //dataItem.DelFlg = true;
                this.Repository.Delete(dataItem);
                this.VNPTLogs.Write(this.Repository, EAction.Update, JsonConvert.SerializeObject(dataItem));

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

        [HttpPost]
        [Route("Saves")]
        public IHttpActionResult Saves(IEnumerable<UnitImportExcelVM> dataRequest)
        {
            try
            {
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
                //check token expired
                if (this.isAuthenticatedUser != true)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorNotPermisionFunc)
                    });
                }

                var userName = VNPTHelper.GetUserName();

                if (dataRequest.Count() > 0)
                {
                    var reqUnitID = dataRequest.Select(r => r.ID).AsEnumerable();
                    var reqParentUnitID = dataRequest.Select(r => r.ParentID).AsEnumerable();
                    var reqUnitName = dataRequest.Select(r => r.Name).AsEnumerable();

                    var nameExists = this.Repository.GetQuery<M_UNIT>()
                        .Where(r => r.DelFlg != true)
                        .Join(reqUnitName, a => a.Name, b => b, (a, b) => a.Name)
                        .ToList();

                    if (nameExists.Count > 0)
                    {
                        var errMsg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgErrUnitNameIsExixts), string.Join("; ", nameExists));

                        return Json(new TResult()
                        {
                            Status = (short)EStatus.Fail,
                            Msg = errMsg
                        });
                    }

                    if (reqUnitID.Distinct().Count() != dataRequest.Count())
                    {
                        var reqUnitIDDuplicates = reqUnitID
                            .GroupBy(r => r)
                            .Select(r => new
                            {
                                ID = r.Key,
                                Cnt = r.Count()
                            })
                            .Where(r => r.Cnt > 1)
                            .Select(r => r.ID)
                            .ToList();

                        var msg = string.Join("; ", reqUnitIDDuplicates);

                        return Json(new TResult()
                        {
                            Status = (short)EStatus.Fail,
                            Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgOrganizationImportExcelDuplicate), msg)
                        });
                    }

                    var unitExists = this.Repository.GetQuery<M_UNIT>()
                        .Join(reqUnitID, a => a.ID, b => b, (a, b) => a.ID)
                        .ToList();

                    if (unitExists.Count > 0)
                    {
                        var errMsg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgErrUnitIDIsExixts), string.Join("; ", unitExists));

                        return Json(new TResult()
                        {
                            Status = (short)EStatus.Fail,
                            Msg = errMsg
                        });
                    }

                    var parentUnitExists = this.Repository.GetQuery<M_UNIT>()
                        .Where(r => r.DelFlg != true)
                        .Join(reqParentUnitID, a => a.ID, b => b, (a, b) => a.ID)
                        .ToList();

                    var parentUnitNotExists = reqParentUnitID.ToList().Except(parentUnitExists).ToList();

                    if (parentUnitNotExists.Count > 0)
                    {
                        var errMsg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgErrUnitIDNotExixts), string.Join("; ", parentUnitNotExists));

                        return Json(new TResult()
                        {
                            Status = (short)EStatus.Fail,
                            Msg = errMsg
                        });
                    }

                    //var unitTypes = this.Repository.GetQuery<M_COMMON>()
                    //    .Where(r => r.DelFlg != true
                    //        && r.Type == (int)ECommon.UnitType)
                    //    .Select(r => r.ID)
                    //    .ToList();

                    //if (dataRequest.Any(r => !unitTypes.Any(a => r.Type == a)))
                    //{
                    //    return Json(new TResult()
                    //    {
                    //        Status = (short)EStatus.Fail,
                    //        Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgUnitInUnitType), string.Join(";", unitTypes))
                    //    });
                    //}

                    M_UNIT dataItem = null;
                    foreach (var item in dataRequest)
                    {
                        dataItem = new M_UNIT()
                        {
                            ID = item.ID,
                            Name = item.Name,
                            ParentID = item.ParentID,
                            Email = item.Email,
                            Phone = item.Phone,
                            Type = item.Type,

                            CreateAt = DateTime.Now,
                        };

                        this.Repository.Add(dataItem);
                        this.VNPTLogs.Write(this.Repository, EAction.Insert, JsonConvert.SerializeObject(dataItem));
                    }

                    this.Repository.UnitOfWork.SaveChanges();
                }

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgSaveOk),
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                throw new Exception("", e);
            }
        }

        [HttpGet]
        [Route("GetByType")]
        public IHttpActionResult GetByType(int unitTypeID)
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
                var result = this.Repository.GetQuery<M_UNIT>()
                        .Where(r => r.DelFlg != true && r.Type == unitTypeID)
                    .Select(r => new
                    {
                        r.ID,
                        r.Name
                    })
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

        //[HttpGet]
        //[Route("GetByParentID")]
        //public IHttpActionResult GetByParentID(string unitParentID)
        //{
        //    try
        //    {
        //        //check token expired
        //        if (this.isAuthenticatedUser != true)
        //        {
        //            return Json(new TResult()
        //            {
        //                Status = (short)EStatus.Fail,
        //                Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgErrorNotPermisionFunc)
        //            });
        //        }
        //        var unitChilds = new List<M_UNIT>();
        //        var units = this.Repository.GetQuery<M_UNIT>().Where(r => r.DelFlg != true).ToList();
        //        var unitTemp = units.Where(r => r.DelFlg != true && r.ID == unitParentID).ToList();
        //        if (unitTemp == null || unitTemp.Count == 0)
        //        {
        //            return null;
        //        }

        //        var listUnitChildTemp = VNPTHelper.TakeListUnitRecursive2(unitTemp, unitTemp, units.ToList());

        //        foreach (var item in listUnitChildTemp)
        //        {
        //            if (unitChilds.Any(r => r.ID == item.ID)) continue;

        //            unitChilds.Add(item);
        //        }
        //        var result = unitChilds.Distinct();

        //        return Json(new TResult()
        //        {
        //            Status = (short)EStatus.Ok,
        //            Data = result
        //        });
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("", e);
        //    }
        //}

        }
    }
