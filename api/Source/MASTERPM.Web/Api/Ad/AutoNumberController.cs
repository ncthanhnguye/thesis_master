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
using MASTERPM.Web.Api.Base;

namespace MASTERPM.Web.Api.Ad
{
    [RoutePrefix("api/AutoNumber")]
    public class AutoNumberController : BaseController
    {
        [HttpGet]
        public IHttpActionResult Get(string id)
        {
            try
            {
                var result = this.Repository.GetQuery<AD_AUTONUMBER>().FirstOrDefault(r => r.ID == id);
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
        public IHttpActionResult Get()
        {
            try
            {
                var result = this.Repository.GetQuery<AD_AUTONUMBER>().ToList();
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
        public IHttpActionResult Put(string id, [FromBody]AD_AUTONUMBER dataRequest)
        {
            try
            {
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

                var dataItem = this.Repository.GetQuery<AD_AUTONUMBER>().FirstOrDefault(r => r.ID == id);
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
        public IHttpActionResult Post([FromBody] AD_AUTONUMBER dataRequest)
        {
            try
            {
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

                if (this.Repository.GetQuery<AD_AUTONUMBER>().Any(r => r.ID.Equals(dataRequest.ID)))
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
        public IHttpActionResult Saves(IEnumerable<AD_AUTONUMBER> dataRequest)
        {
            try
            {
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

                AD_AUTONUMBER dataItem = null;
                foreach (var item in dataRequest)
                {
                    dataItem = this.Repository.GetQuery<AD_AUTONUMBER>().FirstOrDefault(r => r.ID == item.ID);
                    if (dataItem is null)
                    {
                        if (this.Repository.GetQuery<AD_AUTONUMBER>().Any(r => r.ID.Equals(item.ID)))
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
                if (string.IsNullOrEmpty(dataRequest.IDList))
                {
                    return Json(new TResult()
                    {
                        Status = (int)EStatus.Fail,
                        Msg = MASTERResources.Instance.Get(MASTERResources.ID.MsgErrorRequiredDeleteID)
                    });
                }
                string[] ids = (string[])JsonConvert.DeserializeObject(dataRequest.IDList, typeof(string[]));

                var list = this.Repository.GetQuery<AD_AUTONUMBER>().Where(r => ids.Any(p => p == r.ID)).ToList();
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var autoNumber = list[i];

                    this.Repository.Delete(autoNumber);
                    this.MASTERLogs.Write(this.Repository, EAction.Delete, JsonConvert.SerializeObject(autoNumber));
                }

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
    }
}
