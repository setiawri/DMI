using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace DMIWeb
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (UserAccount.IsAuthenticated)
            {
                FormsIdentity objUserIdentity = (FormsIdentity)Context.User.Identity;

                List<string> cookiedata = objUserIdentity.Ticket.UserData.Split(new char[] { GlobalVariables.COOKIEDATE_DELIMITER }).ToList<string>();
                Context.Items[GlobalVariables.COOKIEDATA_USERNAME] = cookiedata[0];
                Context.Items[GlobalVariables.COOKIEDATA_CANAPPROVESALES] = cookiedata[1];
                Context.Items[GlobalVariables.COOKIEDATA_CANVIEWPROFITLOSSSTATEMENT] = cookiedata[2];
                Context.Items[GlobalVariables.COOKIEDATA_CANVIEWCUSTOMERORDER] = cookiedata[3];
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}