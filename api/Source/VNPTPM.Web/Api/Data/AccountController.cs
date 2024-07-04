using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using VNPTPM.Model;
using VNPTPM.Model.Commons;
using VNPTPM.Model.Core;
using VNPTPM.Model.Validate;
using VNPTPM.Model.VM;
using VNPTPM.Web.Api.Base;
using System.Threading.Tasks;
using System.Web.Security;

namespace VNPTPM.Web.Api.Profile
{
    [RoutePrefix("api/Account")]
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        [HttpGet]
        [Route("Search")]
        public IHttpActionResult Search(string SearchText, string unitID = "")
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
                var isAllText = string.IsNullOrEmpty(SearchText);

                var commons = this.Repository.GetQuery<M_COMMON>().Where(r => r.DelFlg != true).ToList();

                var positions = commons.Where(r => r.Type == (int)ECommon.Position).AsEnumerable();
                var ethnics = commons.Where(r => r.Type == (int)ECommon.Ethnic).AsEnumerable();
                var politicalTheorys = commons.Where(r => r.Type == (int)ECommon.PoliticalTheory).AsEnumerable();
                var communistPartyPositions = commons.Where(r => r.Type == (int)ECommon.CommunistPartyPosition).AsEnumerable();
                var qualifications = commons.Where(r => r.Type == (int)ECommon.Qualification).AsEnumerable();

                var unitIDLogin = "";
                var unitByLoginsTemp = VNPTHelper.GetUnitByChilds(this.Repository, ref unitIDLogin);
                var unitByLogins = unitByLoginsTemp.Select(r => new
                {
                    r.ID,
                    r.Name
                })
                .Distinct();

                var result = this.Repository.GetQuery<DATA_ACCOUNT>()
                    .Where(r => r.DelFlg != true
                        && (isAllText
                        || r.Name.Contains(SearchText)
                        || r.Phone.Contains(SearchText))
                        && (string.IsNullOrEmpty(unitID) || unitID.Equals(r.UnitID)))
                    .GroupJoin(this.Repository.GetQuery<AD_USER>().Where(r => r.DelFlg != true),
                        a => a.ID, b => b.AccountID, (a, b) => new
                        {
                            DATA_ACCOUNT = a,
                            AD_USER = b.FirstOrDefault()
                        })
                    .AsEnumerable()
                    .Join(unitByLogins, a => a.DATA_ACCOUNT.UnitID, b => b.ID, (a, b) => new
                    {
                        a.DATA_ACCOUNT,
                        a.AD_USER,
                        M_UNIT = b
                    })
                    .AsEnumerable()
                    .Select(r => new
                    {
                        UserName = r.AD_USER?.UserName,
                        r.DATA_ACCOUNT.ID,
                        r.DATA_ACCOUNT.Name,
                        r.DATA_ACCOUNT.NameModify,
                        r.DATA_ACCOUNT.Phone,
                        r.DATA_ACCOUNT.Email,
                        UnitName = r.M_UNIT.Name,
                        UnitID = r.M_UNIT.ID,
                        r.DATA_ACCOUNT.BirthDate,
                        r.DATA_ACCOUNT.Gender,
                        PositionName = positions.FirstOrDefault(a => a.ID == r.DATA_ACCOUNT.PositionID)?.Name,
                        r.DATA_ACCOUNT.HomeTown,
                        EthnicName = ethnics.FirstOrDefault(a => a.ID == r.DATA_ACCOUNT.Ethnic)?.Name,
                        r.DATA_ACCOUNT.Specialized,
                        PoliticalTheoryName = politicalTheorys.FirstOrDefault(a => a.ID == r.DATA_ACCOUNT.PoliticalTheory)?.Name,
                        r.DATA_ACCOUNT.YouthGroupDate,
                        r.DATA_ACCOUNT.CommunistPartyDate,
                        CommunistPartyPositionName = communistPartyPositions.FirstOrDefault(a => a.ID == r.DATA_ACCOUNT.CommunistPartyPosition)?.Name,
                        QualificationName = qualifications.FirstOrDefault(a => a.ID == r.DATA_ACCOUNT.Qualification)?.Name,
                        NameChangedFlg = !string.IsNullOrEmpty(r.DATA_ACCOUNT.NameModify)
                    })
                    .OrderBy(r => r.Name);

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
        [Route("NoAccountFilter")]
        public IHttpActionResult NoAccountFilter()
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
                var commons = this.Repository.GetQuery<M_COMMON>().Where(r => r.DelFlg != true).ToList();

                var positions = commons.Where(r => r.Type == (int)ECommon.Position).AsEnumerable();
                var ethnics = commons.Where(r => r.Type == (int)ECommon.Ethnic).AsEnumerable();
                var politicalTheorys = commons.Where(r => r.Type == (int)ECommon.PoliticalTheory).AsEnumerable();
                var communistPartyPositions = commons.Where(r => r.Type == (int)ECommon.CommunistPartyPosition).AsEnumerable();
                var qualifications = commons.Where(r => r.Type == (int)ECommon.Qualification).AsEnumerable();

                var unitIDLogin = "";
                var unitByLoginsTemp = VNPTHelper.GetUnitByChilds(this.Repository, ref unitIDLogin);
                var unitByLogins = unitByLoginsTemp.Select(r => new
                {
                    r.ID,
                    r.Name
                })
                .Distinct();

                var result = this.Repository.GetQuery<DATA_ACCOUNT>()
                    .GroupJoin(this.Repository.GetQuery<AD_USER>().Where(r => r.DelFlg != true),
                        a => a.ID, b => b.AccountID, (a, b) => new
                        {
                            DATA_ACCOUNT = a,
                            AD_USER = b.FirstOrDefault()
                        })

                    .AsEnumerable()
                    .Join(unitByLogins, a => a.DATA_ACCOUNT.UnitID, b => b.ID, (a, b) => new
                    {
                        a.DATA_ACCOUNT,
                        a.AD_USER,
                        M_UNIT = b
                    })

                    //.Where(r => r.AD_USER == null)
                    .AsEnumerable()
                    .Select(r => new
                    {
                        r.DATA_ACCOUNT.ID,
                        Name = r.DATA_ACCOUNT.Name + " (" + r.DATA_ACCOUNT.Phone + ")"
                    })
                    .OrderBy(r => r.Name);

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
        public IHttpActionResult Get(Guid iD)
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
                var dataItem = this.Repository.GetQuery<DATA_ACCOUNT>()
                    .FirstOrDefault(r => r.ID == iD);

                if (dataItem is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = $"{VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)}_DATA_ACCOUNT"
                    });
                }

                dataItem.PositionName = this.Repository.GetQuery<M_COMMON>()
                    .FirstOrDefault(r => r.ID == dataItem.PositionID)
                    ?.Name;

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
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
        public IHttpActionResult Post([FromBody] DATA_ACCOUNT dataRequest)
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

                if (dataRequest.BirthDate.GetValueOrDefault() > DateTime.Today)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgAccountBirthdayToday)
                    });
                }

                if (!string.IsNullOrEmpty(dataRequest.Email)
                    && this.Repository.GetQuery<DATA_ACCOUNT>().Any(r =>r.DelFlg != true && r.Email == dataRequest.Email))
                {
                    Dictionary<string, string> errColumns = new Dictionary<string, string>();
                    errColumns.Add("Email", VNPTResources.Instance.Get(VNPTResources.ID.MsgEmailIsExists));
                    var dtErr = VNPTResources.Instance.GetDataTable(errColumns);

                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Data = dtErr,
                        Msg = ""
                    });
                }

                string userName = VNPTHelper.GetUserName();

                var dataItem = dataRequest.Clone();
                dataItem.ID = Guid.NewGuid();
                dataItem.CreateAt = DateTime.Now;
                dataItem.CreateBy = userName;

                //AvatarUpload
                if (!string.IsNullOrEmpty(dataItem.AvatarUrl))
                {
                    string link = dataItem.AvatarUrl;
                    var errorMsg = "";

                    try
                    {
                        string sourcePath = string.Format(Constants.WO_PATH_SAVE_FILE_TMP, userName);
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

                    dataItem.AvatarUrl = link;
                }
                else
                {
                    dataItem.AvatarUrl = null;
                }

                this.Repository.Add(dataItem);
                this.VNPTLogs.Write(this.Repository, EAction.Insert, JsonConvert.SerializeObject(dataItem));

                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgSaveOk),
                    Data = dataItem.ID
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                throw new Exception("", e);
            }
        }

        [HttpPut]
        public IHttpActionResult Put(Guid iD, [FromBody] DATA_ACCOUNT dataRequest)
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
                //start check BIRTHDAY
                if (dataRequest.BirthDate.GetValueOrDefault() > DateTime.Today)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgAccountBirthdayToday)
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
                if (!string.IsNullOrEmpty(dataRequest.Email)
                    && this.Repository.GetQuery<DATA_ACCOUNT>()
                        .Any(r => r.DelFlg != true && r.Email == dataRequest.Email
                            && r.ID != iD))
                {
                    Dictionary<string, string> errColumns = new Dictionary<string, string>();
                    errColumns.Add("Email", VNPTResources.Instance.Get(VNPTResources.ID.MsgEmailIsExists));
                    var dtErr = VNPTResources.Instance.GetDataTable(errColumns);

                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Data = dtErr,
                        Msg = ""
                    });
                }

                string userName = VNPTHelper.GetUserName();

                var dataItem = this.Repository.GetQuery<DATA_ACCOUNT>()
                    .FirstOrDefault(r => r.DelFlg != true && r.ID == dataRequest.ID);

                if (dataItem is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
                }

                dataItem.Name = dataRequest.Name;
                dataItem.BirthDate = dataRequest.BirthDate;
                dataItem.Gender = dataRequest.Gender;
                dataItem.Email = dataRequest.Email;
                dataItem.PositionID = dataRequest.PositionID;
                dataItem.UnitID = dataRequest.UnitID;
                dataItem.Phone = dataRequest.Phone;

                dataItem.HomeTown = dataRequest.HomeTown;//quê quán
                dataItem.Ethnic = dataRequest.Ethnic;//dân tộc
                dataItem.Specialized = dataRequest.Specialized;//chuyên môn nghiệp vụ
                dataItem.PoliticalTheory = dataRequest.PoliticalTheory;//lý luận chính trị
                dataItem.YouthGroupDate = dataRequest.YouthGroupDate;// ngày vào đoàn
                dataItem.CommunistPartyDate = dataRequest.CommunistPartyDate;// ngày vào đảng
                dataItem.CommunistPartyPosition = dataRequest.CommunistPartyPosition;//chức vụ đảng
                dataItem.Qualification = dataRequest.Qualification;//trình độ chuyên môn
                dataItem.Note = dataRequest.Note;

                dataItem.UpdateAt = DateTime.Now;
                dataItem.UpdateBy = userName;

                //AvatarUpload
                if (!string.IsNullOrEmpty(dataRequest.AvatarUrl) && !dataRequest.AvatarUrl.Equals(dataItem.AvatarUrl))
                {
                    string link = dataRequest.AvatarUrl;
                    var errorMsg = "";

                    try
                    {
                        string sourcePath = string.Format(Constants.WO_PATH_SAVE_FILE_TMP, userName);
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

                    dataItem.AvatarUrl = link;
                }


                this.Repository.Update(dataItem);
                this.VNPTLogs.Write(this.Repository, EAction.Update, JsonConvert.SerializeObject(dataItem));

                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgSaveOk),
                    Data = dataItem.ID
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.RepositoryLog, e.Message);
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
                var dataItem = this.Repository.GetQuery<DATA_ACCOUNT>().FirstOrDefault(r => iD == r.ID);
                if (dataItem == null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
                }

                var UserName = this.Repository.GetQuery<AD_USER>()
                    .FirstOrDefault(r => r.AccountID == iD && r.DelFlg != true)
                    ?.UserName;

                if (!string.IsNullOrEmpty(UserName))
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgUserAccountIsExist), UserName, dataItem.Name)
                    });
                }

                dataItem.DelFlg = true;
                dataItem.DeleteAt = DateTime.Now;
                dataItem.DeleteBy = VNPTHelper.GetUserName();

                this.Repository.Update(dataItem);
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



        [HttpGet]
        [Route("GetConfigRegist")]
        public IHttpActionResult GetConfigRegist()
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
                var result = this.Repository.GetQuery<AD_CONFIG>()
                     .FirstOrDefault(r => EConfig.GetConfigRegist.ToString().Equals(r.ID))
                     ?.Value;

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

    }
}
