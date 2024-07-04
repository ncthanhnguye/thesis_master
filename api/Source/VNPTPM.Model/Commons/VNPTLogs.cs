using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNPTPM.Model.Core;

namespace VNPTPM.Model.Commons
{
    public class VNPTLogs
    {
        public Guid Write(IRepository repository, string content)
        {
            var id = Guid.NewGuid();
            repository.Add(new AD_LOG()
            {
                ID = id,
                ServiceName = VNPTHelper.GetServiceName(),
                UserName = VNPTHelper.GetUserName(),
                CreateAt = DateTime.Now,
                Data = content
            });

            //repository.UnitOfWork.SaveChanges();
            return id;
        }

        public void Write(IRepository repository, EAction action, string content)
        {
            repository.Add(new AD_LOG()
            {
                ID = Guid.NewGuid(),
                ServiceName = VNPTHelper.GetServiceName(),
                UserName = VNPTHelper.GetUserName(),
                ActionRec = (int)action,
                CreateAt = DateTime.Now,
                Data = content
            });
        }

        public void WriteBatch(IRepository repository, EAction action, string content)
        {
            repository.Add(new AD_LOG()
            {
                ID = Guid.NewGuid(),
                ServiceName = string.Empty,
                UserName = string.Empty,
                ActionRec = (int)action,
                CreateAt = DateTime.Now,
                Data = content
            });
        }


        public static void WriteFileLog(string strLog, string type = "INFO")
        {
            StreamWriter log;
            FileStream fileStream = null;
            DirectoryInfo logDirInfo = null;
            FileInfo logFileInfo;
            
            var logFilePath = $"{VNPTConfigs.DirDocumentUpload}/Upload/Logs/";
            logFilePath = logFilePath + "Log-" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
            logFileInfo = new FileInfo(logFilePath);
            logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists) logDirInfo.Create();
            if (!logFileInfo.Exists)
            {
                fileStream = logFileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(logFilePath, FileMode.Append);
            }
            log = new StreamWriter(fileStream);
            string logMain = $"[{DateTime.Now.ToString("yyyyMMddHHmmss")}]-[{type}]-[{strLog}]";
            log.WriteLine(logMain);
            log.Close();
        }
    }
}
