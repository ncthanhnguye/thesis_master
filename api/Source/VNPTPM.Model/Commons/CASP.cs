using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Xml;
using System.Xml.XPath;
namespace VNPTPM.Model.Commons
{
    public class CASP
    {
        /// <summary>
        /// The base URL for the CAS server.
        /// </summary>
        protected string baseCasUrl;

        /// <summary>
        /// The service URL for the client, used as the "service" parameter in all CAS calls.
        /// </summary>
        protected string ServiceUrl;

        /// <summary>
        /// The page we're on
        /// </summary>
        protected string currentPage;

        /// <summary>
        /// Determines if we renew logins.  If true, CAS sessions from other browsing can be utilized.  If false, user will need to enter credentials every time.
        /// </summary>
        /// <remarks>
        /// See http://www.ja-sig.org/products/cas/overview/protocol/index.html, section 2.1.1
        /// </remarks>
        protected bool AlwaysRenew;

        /// <summary>
        /// The CAS ticket on the current page.
        /// </summary>
        public string CASTicket
        {
            get { return HttpContext.Current.Request.QueryString["ticket"]; }
        }

        /// <summary>
        /// Create a new CASP object, setting some initial values
        /// </summary>
        /// <param name="baseCasUrl">eg: "https://login.case.edu/cas/"</param>
        /// <param name="currentPage">usually this.Page or this</param>
        /// <param name="alwaysRenew">true to always renew CAS logins (prompting for credentials every time)</param>
        public CASP(string baseCasUrl, string currentPage, bool alwaysRenew)
        {
            //if (currentPage == null)
            //    throw new ArgumentNullException("currentPage cannot be null");
            //if (baseCasUrl == null)
            //    throw new ArgumentNullException("baseCasUrl cannot be null");

            this.baseCasUrl = baseCasUrl;
            this.currentPage = currentPage;
            this.AlwaysRenew = alwaysRenew;
            ServiceUrl = HttpUtility.UrlEncode(currentPage.Split('?')[0]);
            //ServiceUrl = HttpUtility.UrlEncode("http://localhost:4591/upload");
        }

        /// <summary>
        /// Create a new CASP object, setting some initial values
        /// </summary>
        /// <param name="baseCasUrl">eg: "https://login.case.edu/cas/"</param>
        /// <param name="currentPage">usually this.Page or this</param>
        public CASP(string baseCasUrl, string currentPage)
            : this(baseCasUrl, currentPage, false)
        { }

        /// <summary>
        /// Validates using CAS2, returning the value of the node given in the xpath expression.
        /// </summary>
        /// <remarks>
        /// See http://www.ja-sig.org/products/cas/overview/protocol/index.html, section 2.5
        /// </remarks>
        /// <param name="xpath"></param>
        /// <exception cref="ValidateException">If an error occurs</exception>
        /// <returns></returns>
        public string ServiceValidate(string xpath)
        {
            string result;
            //get the CAS2 xml into a string
            string xml =
                GetResponse(
                    string.Format("{0}?ticket={1}&service={2}", Path.Combine(baseCasUrl, "serviceValidate"), CASTicket,
                                  ServiceUrl));
            try
            {
                //use an army of objects to run an xpath on that xml string
                using (TextReader tx = new StringReader(xml))
                {
                    XPathNavigator nav = new XPathDocument(tx).CreateNavigator();
                    XPathExpression xpe = nav.Compile(xpath);

                    //recognize xmlns:cas
                    XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());
                    namespaceManager.AddNamespace("cas", "http://www.yale.edu/tp/cas");
                    xpe.SetContext(namespaceManager);

                    //get the contents of the <cas:user> element
                    XPathNavigator node = nav.SelectSingleNode(xpe);
                    result = node.Value;
                }
            }
            catch (Exception ex)
            {
                //if we had a problem somewhere above, throw up with some helpful data
                throw new ValidateException(CASTicket, xml, ex);
            }

            return result;
        }

        /// <summary>
        /// Validates using CAS2, returning the cas:user
        /// </summary>
        /// <returns>returns the value of the cas:user</returns>
        public string ServiceValidate()
        {
            return ServiceValidate("/cas:serviceResponse/cas:authenticationSuccess/cas:user");
        }

        /// <summary>
        /// Validates a ticket using CAS1, returing the username
        /// </summary>
        /// <remarks>
        /// See http://www.ja-sig.org/products/cas/overview/protocol/index.html, section 2.4
        /// </remarks>
        /// <exception cref="ValidateException">If an error occurs</exception>
        /// <returns></returns>
        public string Validate()
        {
            string result;
            //get the CAS1 response into a string
            string resp =
                GetResponse(
                    string.Format("{0}?ticket={1}&service={2}", Path.Combine(baseCasUrl, "validate"), CASTicket,
                                  ServiceUrl));
            try
            {
                result = resp.Split('\n')[1];
            }
            catch (Exception ex)
            {
                //if we had a problem somewhere above, throw up with some helpful data
                throw new ValidateException(CASTicket, resp, ex);
            }
            return result;
        }

        /// <summary>
        /// Logs in the user, redirecting if needed.
        /// </summary>
        /// <remarks>
        /// See http://www.ja-sig.org/products/cas/overview/protocol/index.html, section 2.1
        /// </remarks>
        /// <param name="serviceUrl"></param>
        /// <param name="force">force the redirect, whether we have a ticket or not</param>
        /// <returns>Returns true if we're logged in and ready to be validated.</returns>
        public bool Login(string serviceUrl, bool force)
        {
            string loginUrl = string.Format("{0}?service={1}",
                                            Path.Combine(this.baseCasUrl, "login"), serviceUrl);
            if (AlwaysRenew)
                loginUrl += "&renew=true";

            if (force || string.IsNullOrEmpty(CASTicket))
                //currentPage.Response.Redirect(loginUrl, true);
                HttpContext.Current.Response.Redirect(loginUrl, true);
            bool a = force || string.IsNullOrEmpty(CASTicket);
            return !(a);
        }

        /// <summary>
        /// Logs in the user, redirecting if needed.  
        /// </summary>
        /// <param name="force">force the redirect, whether we have a ticket or not</param>
        /// <returns>Returns true if we're logged in and ready to be validated.</returns>
        public bool Login(bool force)
        {
            return Login(ServiceUrl, force);
        }

        /// <summary>
        /// Logs in the user, redirecting if needed.  
        /// </summary>
        /// <returns>Returns true if we're logged in and ready to be validated.</returns>
        public bool Login()
        {
            return Login(ServiceUrl, false);
        }

        /// <summary>
        /// Helper to get a web response as text
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected static string GetResponse(string url)
        {
            //split out IDisposables into seperate using blocks to ensure everything gets disposed
            using (WebClient c = new WebClient())
            using (Stream response = c.OpenRead(url))
            using (StreamReader reader = new StreamReader(response))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Authenticates, getting the username.  Will redirect as needed.
        /// </summary>
        /// <param name="baseCasUrl">eg: "https://login.case.edu/cas/"</param>
        /// <param name="page">usually this.Page or this</param>
        /// <param name="alwaysRenew">true to always renew CAS logins (prompting for credentials every time)</param>
        /// <param name="useCas2">if set to <c>true</c> then use CAS2 ServiceValidate, otherwises uses CAS1 Validate</param>        
        /// <returns>username</returns>
        public static string Authenticate(string baseCasUrl, string page, bool alwaysRenew, bool useCas2)
        {
            string username = null;
            CASP casp = new CASP(baseCasUrl, page, alwaysRenew);
            if (casp.Login())
            {
                try
                {
                    username = useCas2 ? casp.ServiceValidate() : casp.Validate();
                }
                catch (ValidateException)
                {
                    //try again, something was messed up
                    casp.Login(true);
                }
            }
            return username;
        }
        /// <summary>
        /// Authenticates using CAS2, getting the username.  Will redirect as needed.
        /// </summary>
        /// <param name="baseCasUrl"></param>
        /// <param name="page"></param>
        /// <returns>cas:user</returns>
        public static string Authenticate(string baseCasUrl, string page)
        {
            return Authenticate(baseCasUrl, page, false, true);
        }

        /// <summary>
        /// Represents errors when validating a CAS ticket
        /// </summary>
        public class ValidateException : Exception
        {
            /// <summary>
            /// The actual response from the server
            /// </summary>
            public string ValidationResponse;

            /// <summary>
            /// Throws a new one, crafting a decent exception message.
            /// </summary>
            /// <param name="ticket">The CAS ticket.</param>
            /// <param name="validationResponse">The validation response.</param>
            /// <param name="innerException">The inner exception.</param>
            public ValidateException(string ticket, string validationResponse, Exception innerException)
                : base(
                    string.Format("Error validating ticket {0}, validation response:\n{1}", ticket, validationResponse),
                    innerException)
            {
                this.ValidationResponse = validationResponse;
            }
        }
    }
}