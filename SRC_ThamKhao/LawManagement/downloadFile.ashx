<%@ WebHandler Language="C#" Class="downloadFile" %>

using System;
using System.Web;
using System.IO;
using System.Web.SessionState;

public class downloadFile : IHttpHandler,IReadOnlySessionState
{
    public void ProcessRequest(HttpContext context)
    {
        try
        {
            string filename = "";
            string sFolder = "";
            try
            {
                string returnfilename = context.Request.QueryString["filename"].ToString();
                filename = returnfilename;
                if (filename != "")
                {
                    if (HttpContext.Current.Session["PathTempFolder"] != null)
                        sFolder = HttpContext.Current.Session["PathTempFolder"].ToString();
                    else
                        sFolder = HttpContext.Current.Server.MapPath(@"~\Temp");

                    filename = sFolder +"\\"+ filename;
                    FileInfo fs = new FileInfo(filename);
                    if (fs.Exists)
                    {
                        var disposition = new System.Net.Mime.ContentDisposition()
                        {
                            FileName = returnfilename,
                            Size = fs.Length,
                            CreationDate = DateTime.UtcNow,
                            ModificationDate = DateTime.UtcNow,
                            ReadDate = DateTime.UtcNow
                        };
                        try
                        {
                            context.Response.Clear();
                            context.Response.ClearHeaders();
                            context.Response.ClearContent();
                            context.Response.ContentType = MOABSearch.Common.Globals.GetResponseContentType(fs.Extension);
                            context.Response.AddHeader("Connection", "Keep-Alive");
                            context.Response.AddHeader("Content-Disposition", disposition.ToString()); //xu li cho TH ten file co khoang trang                        
                            context.Response.AppendHeader("Content-Length", fs.Length.ToString());

                            context.Response.TransmitFile(fs.FullName);
                            context.Response.Flush();
                        }
                        finally
                        {
                            fs.Delete();
                            context.Response.SuppressContent = true;
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            HttpContext.Current.Response.Close();
                            context.Response.End();
                        }
                    }
                }

            }
            catch { }
            //Delete file
            //System.Threading.Thread.Sleep(10000);
            //try
            //{
            //    FileInfo f = new FileInfo(filename);
            //    if (sFolder.EndsWith(f.DirectoryName))
            //    {
            //        if (File.Exists(filename))
            //            File.Delete(filename);
            //    }
            //    else if (!filename.EndsWith(".zip"))
            //        Directory.Delete(f.Directory.FullName, true);
            //}
            //catch { }
        }
        catch { }
    }

    public bool IsReusable
    {
        get
        {
            return true;
        }
    }

}