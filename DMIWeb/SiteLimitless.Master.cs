using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DMIWeb
{
	public partial class SiteLimitless : System.Web.UI.MasterPage
	{
		public string username = "";

		protected void Page_Load(object sender, EventArgs e)
		{
			if (UserAccount.IsAuthenticated)
			{
				username = Tools.getCookieData<string>(GlobalVariables.COOKIEDATA_USERNAME);

				if (Tools.getCookieData<bool>(GlobalVariables.COOKIEDATA_CANVIEWCUSTOMERORDER))
				{
					lnkOrders.Visible = true;
					if (!Tools.getCookieData<bool>(GlobalVariables.COOKIEDATA_CANAPPROVESALES) && !Tools.getCookieData<bool>(GlobalVariables.COOKIEDATA_CANVIEWPROFITLOSSSTATEMENT))
						lnkHome.Visible = false;
				}
			}
		}

		protected void lbtnLogout_Click(object sender, EventArgs e)
		{
			FormsAuthentication.SignOut();
			Response.Redirect("~/");
		}
	}
}