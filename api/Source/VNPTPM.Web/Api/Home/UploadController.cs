using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using VNPTPM.Model.Commons;
using VNPTPM.Model.Core;
using VNPTPM.Web.Api.Base;

namespace VNPTPM.Web.Api.Home
{
    [RoutePrefix("api/Upload")]
    public class UploadController : BaseController
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Post(string dirName, string typeData, string acceptExtensions)
        {
            try
            {
                
                if (!Request.Content.IsMimeMultipartContent() || string.IsNullOrEmpty(acceptExtensions))
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Data = StatusCode(HttpStatusCode.UnsupportedMediaType),
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgFileUploadNotValid)
                    });
                }

                var _AcceptExtensions = (List<string>)JsonConvert.DeserializeObject(acceptExtensions, typeof(List<string>));
                if (_AcceptExtensions == null || acceptExtensions.Count() == 0)
                {
                    return Json(new TResult()
                    {
                        Status = (short)EStatus.Fail,
                        Data = StatusCode(HttpStatusCode.UnsupportedMediaType),
                        Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgFileUploadNotValid)
                    });
                }

                var filesReadToProvider = await Request.Content.ReadAsMultipartAsync();
                List<string> listNameMedia = new List<string>();
                var extension = "";

                foreach (var stream in filesReadToProvider.Contents)
                {
                    // Getting of content as byte[], picture name and picture type
                    var fileBytes = await stream.ReadAsByteArrayAsync();
                    var fileNameReq = stream.Headers.ContentDisposition.FileName;
                    if (fileBytes != null && !string.IsNullOrEmpty(fileNameReq))
                    {
                        extension = Path.GetExtension((string)JsonConvert.DeserializeObject(fileNameReq, typeof(string)));
                        if (string.IsNullOrEmpty(extension) || !_AcceptExtensions.Any(r => r.ToLower().Equals(extension.ToLower())))
                        {
                            return Json(new TResult()
                            {
                                Status = (short)EStatus.Fail,
                                Msg = string.Format(VNPTResources.Instance.Get(VNPTResources.ID.MsgAcceptExtension), string.Join(";", _AcceptExtensions))
                            });
                        }

                        fileNameReq = Path.GetFileName((string)JsonConvert.DeserializeObject(fileNameReq, typeof(string)));
                        string errorMsg = "";
                        string fileNameRes = VNPTHelper.CreateFileFromBytes(fileBytes, typeData, dirName, fileNameReq, ref errorMsg);
                        if (!string.IsNullOrEmpty(errorMsg))
                        {
                            return Json(new TResult()
                            {
                                Status = (short)EStatus.Fail,
                                Msg = errorMsg
                            });
                        }

                        // Kiểm tra file có cho phép ko?
                        bool isExtensionFileAccepted = VNPTHelper.IsExtensionFileAccepted(new List<string> { fileNameRes });
                        
                        if (!isExtensionFileAccepted)
                        {
                            return Json(new TResult()
                            {
                                Status = (short)EStatus.Fail,
                                Data = null,
                                Msg = VNPTResources.Instance.Get(VNPTResources.ID.MsgFileUploadNotAccept)
                            });
                        }
                        listNameMedia.Add(fileNameRes);
                    }
                }

                return Json(new TResult()
                {
                    Status = (short)EStatus.Ok,
                    Data = new
                    {
                        MediaNm = listNameMedia,
                        DirMedia = string.Format(Constants.WO_PATH_SAVE_FILE_TMP, dirName)
                    }
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
