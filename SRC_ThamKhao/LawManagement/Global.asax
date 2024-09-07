<%@ Application Language="C#" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        // Code that runs on application startup
        MOABSearch.Common.CurrentAppConfig app = new MOABSearch.Common.CurrentAppConfig();
        if (app != null && !string.IsNullOrEmpty(app.LinkLearnNewLanding))
            Application[MOABSearch.Common.Globals.sAppConfig] = app;
    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs
        try
        {
            // DSL-348
            Exception exception = Server.GetLastError();
            HttpException httpException = (HttpException)exception;
            int httpCode = httpException.GetHttpCode();

            MOABSearch.Common.Globals.WriteLog("Application_Error.log", "\t httpCode: " + httpCode.ToString());

            switch (httpCode)
            {

                case 401:   // Not authorized
                case 403:   // No Access / Forbidden
                case 404:   // page not found
                case 405:   // Method not allowed
                case 406:   // Not Acceptable    
                case 412:   // Precondition failed
                case 500:   // Internal server error
                case 501:   // Not implemented
                case 502:   // Bad gateway
                case 503:   // Service unavailable

                    if (!string.IsNullOrEmpty(MOABSearch.Common.Globals.ContactPage))
                        HttpContext.Current.Response.Redirect(MOABSearch.Common.Globals.ContactPage);
                    else
                        HttpContext.Current.Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["ContactPage"]);
                    break;

                default: break;
            }
        }
        catch
        {
        }
    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e) 
    {
       
    }
       
</script>
