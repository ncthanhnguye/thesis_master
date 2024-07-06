using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using MASTERPM.Model.Commons;
using MASTERPM.Web.Providers;

namespace MASTERPM.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Add Handler Exception
            config.Filters.Add(new HandleException());
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            //var cors = new System.Web.Http.Cors.EnableCorsAttribute("*", "*", "*") { SupportsCredentials = true };
            //config.EnableCors(cors);
            //config.MessageHandlers.Add(new PreflightRequestsHandler());

            var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.MessageHandlers.Add(new LanguageMessageHandler());

            LoadConfigs();
        }

        public static void LoadConfigs()
        {
            try
            {
                var configValue = "";
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["TokenExpireTime"]))
                {
                    configValue = ConfigurationManager.AppSettings["TokenExpireTime"].ToString();
                    int value = 0;
                    int.TryParse(configValue, out value);
                    MASTERConfigs.TokenExpireTime = value;
                }
                
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DirDocumentUpload"]))
                {
                    configValue = ConfigurationManager.AppSettings["DirDocumentUpload"].ToString();
                    MASTERConfigs.DirDocumentUpload = !string.IsNullOrEmpty(configValue) ? configValue : string.Empty;
                }

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AccoutSMS_UserName"]))
                {
                    configValue = ConfigurationManager.AppSettings["AccoutSMS_UserName"].ToString();
                    MASTERConfigs.AccoutSMS_UserName = !string.IsNullOrEmpty(configValue) ? configValue : string.Empty;
                }

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AccoutSMS_Password"]))
                {
                    configValue = ConfigurationManager.AppSettings["AccoutSMS_Password"].ToString();
                    MASTERConfigs.AccoutSMS_Password = !string.IsNullOrEmpty(configValue) ? configValue : string.Empty;
                }

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["OTP_TEST"]))
                {
                    configValue = ConfigurationManager.AppSettings["OTP_TEST"].ToString();
                    MASTERConfigs.OTP_TEST = !string.IsNullOrEmpty(configValue) ? configValue : string.Empty;
                }

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["PathAvatar"]))
                {
                    configValue = ConfigurationManager.AppSettings["PathAvatar"].ToString();
                    MASTERConfigs.PathAvatar = !string.IsNullOrEmpty(configValue) ? configValue : string.Empty;
                }

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["DirDocumentUpload"]))
                {
                    configValue = ConfigurationManager.AppSettings["DirDocumentUpload"].ToString();
                    MASTERConfigs.DirDocumentUpload = !string.IsNullOrEmpty(configValue) ? configValue : string.Empty;
                }
                
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SMS_ForceSendFlg"]))
                {
                    configValue = ConfigurationManager.AppSettings["SMS_ForceSendFlg"].ToString();
                    MASTERConfigs.SMS_ForceSendFlg = !string.IsNullOrEmpty(configValue) ? configValue : string.Empty;
                }

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["PortalUrl"]))
                {
                    configValue = ConfigurationManager.AppSettings["PortalUrl"].ToString();
                    MASTERConfigs.PortalUrl = !string.IsNullOrEmpty(configValue) ? configValue : string.Empty;
                }

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Firebase_URL"]))
                {
                    configValue = ConfigurationManager.AppSettings["Firebase_URL"].ToString();
                    MASTERConfigs.Firebase_URL = !string.IsNullOrEmpty(configValue) ? configValue : string.Empty;
                }

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Firebase_ServerKey"]))
                {
                    configValue = ConfigurationManager.AppSettings["Firebase_ServerKey"].ToString();
                    MASTERConfigs.Firebase_ServerKey = !string.IsNullOrEmpty(configValue) ? configValue : string.Empty;
                }

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Firebase_SenderID"]))
                {
                    configValue = ConfigurationManager.AppSettings["Firebase_SenderID"].ToString();
                    MASTERConfigs.Firebase_SenderID = !string.IsNullOrEmpty(configValue) ? configValue : string.Empty;
                }

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Firebase_ForceSendFlg"]))
                {
                    configValue = ConfigurationManager.AppSettings["Firebase_ForceSendFlg"].ToString();
                    MASTERConfigs.Firebase_ForceSendFlg = !string.IsNullOrEmpty(configValue) ? configValue : string.Empty;
                }

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["RemindMeetingTimeInterval"]))
                {
                    configValue = ConfigurationManager.AppSettings["RemindMeetingTimeInterval"].ToString();
                    int value = 0;
                    int.TryParse(configValue, out value);
                    MASTERConfigs.RemindMeetingTimeInterval = value;
                }
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["MailServerInfo"]))
                {
                    configValue = ConfigurationManager.AppSettings["MailServerInfo"].ToString();
                    MASTERConfigs.MailServerInfo = !string.IsNullOrEmpty(configValue) ? configValue : string.Empty;
                }
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["WebPortalUrl"]))
                {
                    configValue = ConfigurationManager.AppSettings["WebPortalUrl"].ToString();
                    MASTERConfigs.WebPortalUrl = !string.IsNullOrEmpty(configValue) ? configValue : string.Empty;
                }
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["WebClientUrl"]))
                {
                    configValue = ConfigurationManager.AppSettings["WebClientUrl"].ToString();
                    MASTERConfigs.WebClientUrl = !string.IsNullOrEmpty(configValue) ? configValue : string.Empty;
                }
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ApiDomainOCR"]))
                {
                    configValue = ConfigurationManager.AppSettings["ApiDomainOCR"].ToString();
                    MASTERConfigs.ApiDomainOCR = !string.IsNullOrEmpty(configValue) ? configValue : string.Empty;
                }

            }
            catch
            {
            }
        }
    }

    public class PreflightRequestsHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.Contains("Origin") && request.Method.Method == "OPTIONS")
            {
                var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, Authorization");
                response.Headers.Add("Access-Control-Allow-Methods", "*");
                var tsc = new TaskCompletionSource<HttpResponseMessage>();
                tsc.SetResult(response);
                return tsc.Task;
            }
            return base.SendAsync(request, cancellationToken);
        }
    }

    public class LanguageMessageHandler : DelegatingHandler
    {
        private const string LangviVN = "vi-VN";
        private const string LangenUS = "en-US";

        private readonly List<string> _supportedLanguages = new List<string> { LangviVN, LangenUS };

        private bool SetHeaderIfAcceptLanguageMatchesSupportedLanguage(HttpRequestMessage request)
        {
            foreach (var lang in request.Headers.AcceptLanguage)
            {
                if (_supportedLanguages.Contains(lang.Value))
                {
                    SetCulture(request, lang.Value);
                    return true;
                }
            }

            return false;
        }

        private bool SetHeaderIfGlobalAcceptLanguageMatchesSupportedLanguage(HttpRequestMessage request)
        {
            foreach (var lang in request.Headers.AcceptLanguage)
            {
                var globalLang = lang.Value.Substring(0, 2);
                if (_supportedLanguages.Any(t => t.StartsWith(globalLang)))
                {
                    SetCulture(request, _supportedLanguages.FirstOrDefault(i => i.StartsWith(globalLang)));
                    return true;
                }
            }

            return false;
        }

        private void SetCulture(HttpRequestMessage request, string lang)
        {
            request.Headers.AcceptLanguage.Clear();
            request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(lang));
            Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!SetHeaderIfAcceptLanguageMatchesSupportedLanguage(request))
            {
                // Whoops no localization found. Lets try Globalisation
                if (!SetHeaderIfGlobalAcceptLanguageMatchesSupportedLanguage(request))
                {
                    // no global or localization found
                    SetCulture(request, LangviVN);
                }
            }

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}
