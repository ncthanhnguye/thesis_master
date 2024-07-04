using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Web.Http;
using VNPTPM.Model;
using VNPTPM.Model.Commons;
using VNPTPM.Model.Core;
using VNPTPM.Model.Validate;
using VNPTPM.Model.VM;
using VNPTPM.Web.Api.Base;
using VNPTPM.Web.Providers;

namespace VNPTPM.Web.Api.Ad
{
    [RoutePrefix("api/User")]
    public class UserController : BaseController
    {
        public UserInformationVM Login(string userName, string password, string tokenDevice)
        {
            var passWord = new VNPTCrypto().Encrypt(password);

            var userItem = this.Repository.GetQuery<AD_USER>()
                .FirstOrDefault(r => r.DelFlg != true
                    && r.UserName.Equals(userName));

            // Đăng nhập không thành công
            if (userItem == null || string.IsNullOrEmpty(userItem.UserName))
            {
                return new UserInformationVM()
                {
                    Msg = VNPTResources.ID.MsgLoginFail
                };
            }

            // Kiểm tra mật khẩu
            if (!userItem.Password.Equals(passWord))
            {
                // Khoá tài khoản khi đăng nhập sai quá 3 lần
                if (userItem.LoginFailedCount == null)
                {
                    userItem.LoginFailedCount = 1;
                }
                else
                {
                    switch (userItem.LoginFailedCount)
                    {
                        case 3:
                            userItem.LockFlg = true;
                            break;
                        default:
                            userItem.LoginFailedCount += 1;
                            break;
                    }
                }
                this.Repository.Update(userItem);
                this.Repository.UnitOfWork.SaveChanges();
                return new UserInformationVM()
                {
                    Msg = VNPTResources.ID.MsgLoginFail
                };
            }


            if (userItem.LockFlg.GetValueOrDefault())
            {
                return new UserInformationVM()
                {
                    Msg = VNPTResources.ID.MsgUserLock
                };
            }

            if (string.IsNullOrEmpty(userItem.RoleID))
            {
                return new UserInformationVM()
                {
                    Msg = VNPTResources.ID.MsgNotFound
                };
            }
            var strRoleIDs = (List<string>)JsonConvert.DeserializeObject(userItem.RoleID, typeof(List<string>));

            var roles = this.Repository.GetQuery<AD_ROLE>()
                .Where(r => strRoleIDs.Any(p => r.ID.Equals(p)))
                .Select(r => r.Name)
                .ToList();

            var accountItem = this.Repository.GetQuery<DATA_ACCOUNT>()
                .FirstOrDefault(r => r.ID == userItem.AccountID);
            userItem.LoginFailedCount = 0;
            this.Repository.Update(userItem);
            accountItem.TokenDevice = tokenDevice;
            this.Repository.Update(accountItem);
            this.Repository.UnitOfWork.SaveChanges();

            return new UserInformationVM()
            {
                UserName = userName,
                FullName = accountItem.Name,
                UnitID = string.IsNullOrEmpty(accountItem.UnitID) ? string.Empty : accountItem.UnitID,
                AccountID = accountItem.ID,
                RoleID = userItem.RoleID,
                RoleName = string.Join(";", roles),
                Phone = string.IsNullOrEmpty(accountItem.Phone) ? string.Empty : accountItem.Phone,
                WebPortalUrl = VNPTConfigs.WebPortalUrl,
                WebClientUrl = VNPTConfigs.WebClientUrl,
            };
        }

        [HttpGet]
        [AllowAnonymous]
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
                var isAllSearchText = string.IsNullOrEmpty(searchText);

                var roles = this.Repository.GetQuery<AD_ROLE>().Where(r => r.DelFlg != true).ToList();
                var roleByLogins = VNPTHelper.GetRoleByChilds(this.Repository);
                var unitIDLogin = "";
                var unitByLoginsTemp = VNPTHelper.GetUnitByChilds(this.Repository, ref unitIDLogin);
                if (unitByLoginsTemp == null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Data = "NULL"
                    });
                }

                var result = this.Repository.GetQuery<AD_USER>().Where(r => r.DelFlg != true && !string.IsNullOrEmpty(r.RoleID))
                    .Join(this.Repository.GetQuery<DATA_ACCOUNT>(), a => a.AccountID, b => b.ID,
                    (a, b) => new { AD_USER = a, DATA_ACCOUNT = b})
                    .OrderBy(r => r.DATA_ACCOUNT.Name)
                    .AsEnumerable()
                    .Where(r => unitByLoginsTemp.Any(a => a.ID == r.DATA_ACCOUNT.UnitID))
                    .Select(r => new
                    {
                        UserName = r.AD_USER.UserName,
                        AccountName = r.DATA_ACCOUNT.Name + (r.DATA_ACCOUNT.DelFlg == true ? " (ĐÃ XÓA)" : ""),
                        r.AD_USER.LockFlg,
                        RoleIDs = (List<string>)JsonConvert.DeserializeObject(r.AD_USER.RoleID, typeof(List<string>)),
                        r.AD_USER.RoleID,
                        AccountID = r.DATA_ACCOUNT.ID,
                        Password = Guid.Empty.ToString()
                    })
                    .AsEnumerable()
                    .Select(r => new
                    {
                        r.UserName,
                        r.AccountName,
                        r.LockFlg,
                        r.RoleID,
                        r.AccountID,
                        r.Password,
                        r.RoleIDs,
                        RoleCnt = roleByLogins.Where(a => r.RoleIDs.Any(b => b == a.ID)).Count(),
                        RoleName = string.Join(";", roles.Where(a => r.RoleIDs
                            .Any(b => b.Equals(a.ID))).Select(a => a.Name).ToList()),
                    })
                    .Where(r => 
                        r.RoleCnt > 0 
                        && r.RoleCnt == r.RoleIDs.Count()
                        && !string.IsNullOrEmpty(r.AccountName)
                        && !string.IsNullOrEmpty(r.UserName)
                        && (isAllSearchText
                            || r.UserName.ToLower().Contains(searchText.ToLower())
                            || r.AccountName.ToLower().Contains(searchText.ToLower())))
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
        public IHttpActionResult Get(string userName)
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
                var result = this.Repository.GetQuery<AD_USER>().FirstOrDefault(r => r.UserName.Equals(userName));
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

        [HttpPost]
        public IHttpActionResult Post([FromBody] AD_USER dataRequest)
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


                var isExistUser = this.Repository.GetQuery<AD_USER>().Where(r => r.AccountID == dataRequest.AccountID && r.DelFlg != true).ToList();
                if (isExistUser.Count() > 0)
                {

                    var errMsg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgErrAccountHaveUserExixts));

                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = errMsg
                    });

                }

                var dataItem = this.Repository.GetQuery<AD_USER>()
                    .FirstOrDefault(r => r.UserName.Equals(dataRequest.UserName));

                if (dataItem == null)
                {
                    dataItem = dataRequest.Clone();
                    dataItem.CreateAt = DateTime.Now;
                    dataItem.UpdateAt = DateTime.Now;

                    if (!string.IsNullOrEmpty(dataRequest.Password))
                    {
                        dataItem.Password = (new VNPTCrypto()).Encrypt(dataRequest.Password);
                    }

                    this.Repository.Add(dataItem);
                    this.VNPTLogs.Write(this.Repository, EAction.Insert, JsonConvert.SerializeObject(dataItem));
                }
                else
                {
                    if (!string.IsNullOrEmpty(dataRequest.Password)
                    && !dataRequest.Password.Equals(dataItem.Password)
                    && !Guid.Empty.ToString().Equals(dataRequest.Password))
                    {
                        dataItem.Password = (new VNPTCrypto()).Encrypt(dataRequest.Password);
                    }

                    dataItem.RoleID = dataRequest.RoleID;
                    dataItem.AccountID = dataRequest.AccountID;
                    dataItem.LockFlg = dataRequest.LockFlg;
                    dataItem.DelFlg = false;

                    dataItem.UpdateAt = DateTime.Now;

                    this.Repository.Update(dataItem);
                    this.VNPTLogs.Write(this.Repository, EAction.Update, JsonConvert.SerializeObject(dataItem));
                }

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
        public IHttpActionResult Put(string userName, [FromBody]AD_USER dataRequest)
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

                var dataItem = this.Repository.GetQuery<AD_USER>().FirstOrDefault(r => r.UserName.Equals(userName));
                if (dataItem is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
                }

                if (!string.IsNullOrEmpty(dataRequest.Password) 
                    && !dataRequest.Password.Equals(dataItem.Password) 
                    && !Guid.Empty.ToString().Equals(dataRequest.Password))
                {
                    dataItem.Password = (new VNPTCrypto()).Encrypt(dataRequest.Password);
                }

                dataItem.RoleID = dataRequest.RoleID;
                dataItem.AccountID = dataRequest.AccountID;
                dataItem.LockFlg = dataRequest.LockFlg;
               
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
                this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                throw new Exception("", e);
            }
        }

        [HttpDelete]
        public IHttpActionResult Delete(string userName)
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
                var dataItem = this.Repository.GetQuery<AD_USER>().FirstOrDefault(r => r.UserName.Equals(userName));
                if (dataItem is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
                }
                dataItem.DelFlg = true;
                dataItem.UpdateAt = DateTime.Now;

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
                this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                throw new Exception("", e);
            }
        }

        [HttpGet]
        [Route("GetUserLoginInfo")]
        public IHttpActionResult GetUserLoginInfo(string userName)
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
                var temp = this.Repository.GetQuery<AD_USER>().Where(r => r.UserName.Equals(userName))
                    .Join(this.Repository.GetQuery<DATA_ACCOUNT>(), a => a.AccountID, b => b.ID, (a, b) => new
                    {
                        AD_USER = a,
                        DATA_ACCOUNT = b
                    })
                    .FirstOrDefault();

                var strRoleIDs = (List<string>)JsonConvert.DeserializeObject(temp.AD_USER.RoleID, typeof(List<string>));
                var roles = this.Repository.GetQuery<AD_ROLE>()
                    .Where(r => strRoleIDs.Any(p => r.ID.Equals(p)))
                    .Select(r => r.Name)
                    .ToList();

                var result = new UserInformationVM()
                {
                    UserName = temp.AD_USER.UserName,
                    FullName = temp.DATA_ACCOUNT.Name,
                    FilePath = temp.DATA_ACCOUNT.AvatarUrl,
                    AccountID = temp.AD_USER.AccountID.GetValueOrDefault(),
                    UnitID = string.IsNullOrEmpty(temp.DATA_ACCOUNT.UnitID) ? string.Empty : temp.DATA_ACCOUNT.UnitID,
                    RoleID = temp.AD_USER.RoleID,
                    RoleName = string.Join(";", roles),
                };

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

      



        [HttpPost]
        [Route("ChangePassword")]
        public IHttpActionResult ChangePassword(ChangePasswordVM dataRequest)
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

                var dataItem = this.Repository.GetQuery<AD_USER>()
                    .FirstOrDefault(r => r.UserName.Equals(dataRequest.UserName) && r.DelFlg != true);

                if (dataItem == null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
                }

                if (!(new VNPTCrypto()).Encrypt(dataRequest.PasswordOld).Equals(dataItem.Password))
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgLoginFail)
                    });
                }

                dataItem.Password = (new VNPTCrypto()).Encrypt(dataRequest.PasswordNew);

                dataItem.UpdateAt = DateTime.Now;

                this.Repository.Update(dataItem);
                this.VNPTLogs.Write(this.Repository, EAction.Update, JsonConvert.SerializeObject(dataItem));

                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgSaveOk)
                });
            }
            catch (Exception e)
            {
                this.VNPTLogs.Write(this.RepositoryLog, e.Message);
                throw new Exception("", e);
            }
        }
        [HttpPost]
        [Route("Logout")]
        public IHttpActionResult Logout()
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
                var AuthenticationManager = Request.GetOwinContext().Authentication;
                AuthenticationManager.SignOut(OAuthDefaults.AuthenticationType);
                var bearerToken = Request.Headers.GetValues("Authorization").FirstOrDefault();
                var token = bearerToken.Replace("Bearer ", "");
                var username = VNPTHelper.GetUserName();
                var removeItem = this.Repository.GetQuery<USER_TOKEN>()
                    .Where(r => r.Token == token && r.Username == username).FirstOrDefault();
                if (removeItem == null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
                }
                this.Repository.Delete(removeItem);
                this.VNPTLogs.Write(this.Repository, EAction.Insert, JsonConvert.SerializeObject(removeItem));

                this.Repository.UnitOfWork.SaveChanges();
                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = "Logout success"
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
