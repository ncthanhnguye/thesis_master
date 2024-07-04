using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(VNPTPM.Web.Startup))]

namespace VNPTPM.Web
{
    public partial class Startup
    {
        /*NotifyController notifyCllRemindDoc = new NotifyController();*/

        //public bool started = false;
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            /*if (VNPTPM.Model.Commons.VNPTConfigs.RemindDocumentTimeInterval > 0)
            {
                Thread thread = new Thread(new ThreadStart(DoAsyncNotifyOffice));
                thread.Start();
            }*/
        }

       /* public void DoAsyncNotifyOffice()
        {
            var timeInterval = 1000 * 60 * VNPTPM.Model.Commons.VNPTConfigs.RemindDocumentTimeInterval;

            Thread.Sleep(timeInterval);
            notifyCllRemindDoc.sendNotiyDocDay();
            DoAsyncNotifyOffice();
        }*/
    }
}
