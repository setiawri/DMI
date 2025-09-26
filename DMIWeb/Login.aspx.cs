using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DMIWeb
{
	public partial class Test : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			setupControls();

			if (!Page.IsPostBack)
			{

			}
		}

		private void setupControls()
		{
			txtUsername.MaxLength = 30;
			txtPassword.MaxLength = 30;
		}

		protected void btnLogin_OnClick(object sender, EventArgs e)
		{
			DBUtil.sanitize(txtUsername, txtPassword);

			if (Settings.isDevEnvironment())
			{
				if (txtUsername.Text == "liveserver")
				{
					Settings.ConnectToLiveRemoteServer = true;
					txtUsername.Text = "admin";
				}

				if (string.IsNullOrWhiteSpace(txtUsername.Text))
					txtUsername.Text = "admin";

				if (string.IsNullOrWhiteSpace(txtPassword.Text))
					txtPassword.Text = UserAccount.ADMINPASSWORD;
			}

			if (string.IsNullOrWhiteSpace(txtUsername.Text))
			{
				Page.SetFocus(txtUsername);
				message.error("Silahkan lengkapi username");
				return;
			}
			else if (string.IsNullOrWhiteSpace(txtPassword.Text))
			{
				Page.SetFocus(txtPassword);
				message.error("Silahkan lengkapi password");
				return;
			}

			UserAccount userAccount = new UserAccount(txtUsername.Text);
			if (string.IsNullOrEmpty(userAccount.Username))
			{
				message.error("Invalid username atau password");
				Page.SetFocus(txtUsername);
			}
			else if (!userAccount.Active)
			{
				message.error("Account " + userAccount.Username + " non-aktif");
				txtUsername.Text = "";
				txtPassword.Text = "";
				Page.SetFocus(txtUsername);
			}
			else if (!userAccount.isCorrectPassword(txtPassword.Text))
			{
				message.error("Invalid username atau password");
				Page.SetFocus(txtPassword);
			}
			else
			{
				userAccount.redirectToOriginalPage();
				if (!string.IsNullOrWhiteSpace(Request.QueryString["returnUrl"]))
					Response.Redirect(Request.QueryString["returnUrl"]);
				else
					Response.Redirect("~/");
			}
		}
	}
}