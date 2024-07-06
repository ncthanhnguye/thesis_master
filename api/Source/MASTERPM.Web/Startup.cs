using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MASTERPM.Web.Startup))]

namespace MASTERPM.Web
{
    public partial class Startup
    {
        /*NotifyController notifyCllRemindDoc = new NotifyController();*/

        //public bool started = false;
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            /*if (MASTERPM.Model.Commons.MASTERConfigs.RemindDocumentTimeInterval > 0)
            {
                Thread thread = new Thread(new ThreadStart(DoAsyncNotifyOffice));
                thread.Start();
            }*/
        }

       /* public void DoAsyncNotifyOffice()
        {
            var timeInterval = 1000 * 60 * MASTERPM.Model.Commons.MASTERConfigs.RemindDocumentTimeInterval;

            Thread.Sleep(timeInterval);
            notifyCllRemindDoc.sendNotiyDocDay();
            DoAsyncNotifyOffice();
        }*/
    }
}
