using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using VNPTPM.Model;
using VNPTPM.Model.Commons;
using VNPTPM.Model.VM;
using VNPTPM.Web.Api.Ad;
using VNPTPM.Web.Api.Base;

namespace VNPTPM.Web.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (string.IsNullOrEmpty(publicClientId))
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //local use below else comment
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            

            var lang = context.Request.Headers["Accept-Language"];
            if (string.IsNullOrEmpty(lang))
            {
                lang = "vi-VN";
            }
            Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);

            UserInformationVM userInformation = null;
            Task task = Task.Factory.StartNew<UserInformationVM>(() =>
            {
                //bool isMobileApp = context.OwinContext.Get<bool>("IsMobileApp");
                //string device = (!string.IsNullOrEmpty(context.OwinContext.Get<string>("device"))) ? context.OwinContext.Get<string>("device") : "";
                string tokenDevice = (!string.IsNullOrEmpty(context.OwinContext.Get<string>("tokenDevice"))) ? context.OwinContext.Get<string>("tokenDevice") : "";
                //string iMEICode = (!string.IsNullOrEmpty(context.OwinContext.Get<string>("iMEICode"))) ? context.OwinContext.Get<string>("iMEICode") : "";
                //string type = (!string.IsNullOrEmpty(context.OwinContext.Get<string>("type"))) ? context.OwinContext.Get<string>("type") : "";
                return (new UserController().Login(context.UserName, context.Password, tokenDevice));
            })
            .ContinueWith((result) =>
            {
                userInformation = result.Result;
            });

            await task;

            if (!string.IsNullOrEmpty(userInformation.Msg))
            {
                context.SetError("invalid_grant", VNPTResources.Instance.Get(userInformation.Msg));
                return;
            }

            ClaimsIdentity oAuthIdentity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);
            oAuthIdentity.AddClaim(new Claim("sub", context.UserName));
            oAuthIdentity.AddClaim(new Claim("roleName", userInformation.RoleName));
            oAuthIdentity.AddClaim(new Claim("roleId", userInformation.RoleID));
            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));

            ClaimsIdentity cookiesIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationType);
            cookiesIdentity.AddClaim(new Claim("sub", context.UserName));
            cookiesIdentity.AddClaim(new Claim("roleName", userInformation.RoleName));
            cookiesIdentity.AddClaim(new Claim("roleId", userInformation.RoleID));
            cookiesIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            


            AuthenticationProperties properties = new AuthenticationProperties(new Dictionary<string, string>
            {
                {"UserName", userInformation.UserName },
                {"FullName", userInformation.FullName },
                {"AccountID", userInformation.AccountID.ToString()},
                {"UnitID", userInformation.UnitID??string.Empty },
                {"RoleID", userInformation.RoleID },
                {"RoleName", userInformation.RoleName },
                {"Phone", userInformation.Phone },
                {"WebPortalUrl", userInformation.WebPortalUrl },
                {"WebClientUrl", userInformation.WebClientUrl }
            });

            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            bool isMobileApp;
            try
            {
                isMobileApp = Convert.ToBoolean(context.Parameters.Where(f => f.Key == "isMobileApp").Select(f => f.Value).SingleOrDefault()?[0]);
            }
            catch
            {
                isMobileApp = false;
            }
            context.OwinContext.Set<bool>("IsMobileApp", isMobileApp);

            //device
            string device = "";
            if (context.Parameters.Where(f => f.Key == "device").Select(f => f.Value).SingleOrDefault() != null)
            {
                device = context.Parameters.Where(f => f.Key == "device").Select(f => f.Value).SingleOrDefault()[0];
                if (!string.IsNullOrEmpty(device))
                {
                    context.OwinContext.Set<string>("device", device);
                }
            }

            string IMEICode = "";
            if (context.Parameters.Where(f => f.Key == "IMEICode").Select(f => f.Value).SingleOrDefault() != null)
            {
                IMEICode = context.Parameters.Where(f => f.Key == "IMEICode").Select(f => f.Value).SingleOrDefault()[0];
                if (!string.IsNullOrEmpty(IMEICode))
                {
                    context.OwinContext.Set<string>("IMEICode", IMEICode);
                }
            }

            string tokenDevice = "";
            if (context.Parameters.Where(f => f.Key == "tokenDevice").Select(f => f.Value).SingleOrDefault() != null)
            {
                tokenDevice = context.Parameters.Where(f => f.Key == "tokenDevice").Select(f => f.Value).SingleOrDefault()[0];
                if (!string.IsNullOrEmpty(tokenDevice))
                {
                    context.OwinContext.Set<string>("tokenDevice", tokenDevice);
                }
            }

            //type
            string type = "";
            if (context.Parameters.Where(f => f.Key == "type").Select(f => f.Value).SingleOrDefault() != null)
            {
                type = context.Parameters.Where(f => f.Key == "type").Select(f => f.Value).SingleOrDefault()[0];
                if (!string.IsNullOrEmpty(type))
                {
                    context.OwinContext.Set<string>("type", type);
                }
            }

            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }


        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            
            
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
        public override Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
        {
            var token = context.AccessToken;
            var userName = context.OwinContext.Authentication.AuthenticationResponseGrant.Identity.Name;

            BaseController baseController = new BaseController();
            var accountID = baseController.Repository.GetQuery<AD_USER>()
                .Where(r => r.UserName == userName)
                .Select(r => r.AccountID)
                .FirstOrDefault();
            USER_TOKEN dataItem = new USER_TOKEN();
            dataItem.Token = token;
            dataItem.Username = userName;
            dataItem.AccountID = accountID;
            baseController.Repository.Add(dataItem);
            baseController.VNPTLogs.Write(baseController.Repository, EAction.Insert, JsonConvert.SerializeObject(dataItem));

            baseController.Repository.UnitOfWork.SaveChanges();
            return base.TokenEndpointResponse(context);
        }
    }
    public class UserToken
    {
        public string Token { get; set; }
        public string Username { get; set; }
    }
}