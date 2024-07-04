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

namespace VNPTPM.Web.Api.M
{
    [RoutePrefix("api/CommonMenu")]
    public class CommonMenuController : BaseController
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
                //var eCommon = Enum.GetNames(typeof(ETypeImage))
                //.Select(r => new
                //{
                //    ID = (int)Enum.Parse(typeof(ETypeImage), r),
                //    Name = VNPTResources.Instance.Get("Label_" + r.ToString())
                //});

                var result = this.Repository.GetQuery<M_COMMON>()
                    .Where(r => r.DelFlg != true)
                    .Select(r => new {
                        ID = r.ID,
                        Type = r.Type,
                        Name = r.Name,
                        OrderIndex = r.OrderIndex,
                        CreateAt = r.CreateAt
                    })
                    .OrderByDescending(r => r.CreateAt);

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
        public IHttpActionResult Get(int iD)
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
                var dataItem = this.Repository.GetQuery<M_COMMON>()
                    .FirstOrDefault(r => r.ID == iD);
                if (dataItem is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = $"{VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)}_M_UNIT_CONTACT"
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
        [Route("Search")]
        public IHttpActionResult Search(CommonMenuVM dataRequest)
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
                var eCommon = Enum.GetNames(typeof(ECommon))
                 .Select(r => new
                 {
                     ID = (int)Enum.Parse(typeof(ECommon), r)
                 }).ToList();

                var isAll = string.IsNullOrEmpty(dataRequest.searchText);

                var result = this.Repository.GetQuery<M_COMMON>().Where(r => r.DelFlg != true &&
                    (isAll || r.Name.Contains(dataRequest.searchText)) && (dataRequest.type == null ? r.Type != null : r.Type == dataRequest.type)).AsEnumerable()
                        .Join(eCommon, r => (int)r.Type, e => e.ID, (r, e) => new {
                            ID = r.ID,
                            OrderIndex = r.OrderIndex,
                            Name = r.Name,
                            CreateAt = r.CreateAt,
                            Type = r.Type
                        })
                    .OrderBy(r => r.Type)
                    .ThenBy(r => r.OrderIndex)
                    .ToList();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Data = result
                });
            }
            catch (Exception e)
            {
                var t = e.ToString();
                throw new Exception("", e);
            }
        }


        [HttpPost]
        public IHttpActionResult Post([FromBody] M_COMMON dataRequest)
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

                if(dataRequest.OrderIndex < 0)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.OrderIndex_Common)
                    });
                }

                var dataItem = dataRequest.Clone();
                dataItem.CreateAt = DateTime.Now;
                dataItem.CreateAt = DateTime.Now;
                //dataItem.CreateBy = VNPTHelper.GetUserName();

                this.Repository.Add(dataItem);

                this.Repository.UnitOfWork.SaveChanges();

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgSaveOk)
                });
            }
            catch (Exception e)
            {
                throw new Exception("", e);
            }
        }

        [HttpPut]
        public IHttpActionResult Put(int iD, [FromBody]M_COMMON dataRequest)
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

                if (dataRequest.OrderIndex < 0)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.OrderIndex_Common)
                    });
                }

                var dataItem = this.Repository.GetQuery<M_COMMON>().FirstOrDefault(r => r.ID == iD);
                if (dataItem is null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = $"{VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)}_M_UNIT_CONTACT"
                    });
                }

                dataItem.Name = dataRequest.Name;
                dataItem.UpdateAt = DateTime.Now;
                dataItem.OrderIndex = dataRequest.OrderIndex;
                dataItem.Type = dataRequest.Type;

                this.Repository.Update(dataItem);

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
        public IHttpActionResult Delete(int iD, string langID)
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
                var dataItem = this.Repository.GetQuery<M_COMMON>().FirstOrDefault(r => iD == r.ID);
                if (dataItem == null)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgNotFound)
                    });
                }

                dataItem.DelFlg = true;
                this.Repository.Update(dataItem);

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
        [Route("GetPositions")]
        public IHttpActionResult GetPositions()
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
                var result = this.Repository.GetQuery<M_COMMON>()
                    .Where(r => r.DelFlg != true && r.Type == (int)ECommon.Position)
                    .Select(r => new
                    {
                        r.ID,
                        r.Name
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
                throw new Exception("", e);
            }
        }

        //
        [HttpGet]
        [Route("GetByUnitType")]
        public IHttpActionResult GetByUnitType()
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
                var result = this.Repository.GetQuery<M_COMMON>()
                    .Where(r => r.DelFlg != true && r.Type == (int)ECommon.UnitType)
                    .Select(r => new
                    {
                        ID = r.ID,
                        Name = r.Name,
                    });

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
    }
}