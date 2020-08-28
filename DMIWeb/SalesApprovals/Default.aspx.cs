using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Data.SqlClient;

namespace DMIWeb.SalesApprovals
{
    public partial class Default : System.Web.UI.Page
    {
        /*******************************************************************************************************/
        #region SETTINGS

        #endregion SETTINGS
        /*******************************************************************************************************/
        #region PUBLIC VARIABLES

        protected const string PAGETOPIC = "Sales Approval";

        #endregion PUBLIC VARIABLES
        /*******************************************************************************************************/
        #region PRIVATE VARIABLES

        #endregion PRIVATE VARIABLES
        /*******************************************************************************************************/
        #region CONSTRUCTOR METHODS

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                populatePage();
        }

        #endregion CONSTRUCTOR METHODS
        /*******************************************************************************************************/
        #region METHODS

        private void populatePage()
        {
            string sql = string.Format(@"
                SELECT 
                    SalesApproval.*,
                    Customer.Name AS Customer_Name
                FROM [DWSystem].SalesApproval
                    LEFT OUTER JOIN DWSystem.Customer ON Customer.CustomerID = SalesApproval.CustomerID
                WHERE SalesApproval.[Approved] = 0 AND SalesApproval.[Canceled] = 0;
            ");
            DataTable datatable = DBUtil.getData(sql);
            
            if (datatable.Rows.Count == 0)
                Response.Redirect("~/");
            else
            {
                rptParent.DataSource = datatable;
                rptParent.DataBind();
            }
        }

        protected void update(string ID, bool isApproved)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(Settings.ConnectionString))
                {
                    string sql = "";
                    bool isNotValid = false;

                    //check row status. Abort if Cancelled or already approved
                    sql = @"
                        DECLARE @isNotValid bit = 0

                        SELECT @isNotValid = Canceled
                        FROM [DWSystem].[SalesApproval]                         
                        WHERE [ID] = @ID

                        IF @isNotValid = 0
                        BEGIN
                            SELECT @isNotValid = Approved
                            FROM [DWSystem].[SalesApproval]                         
                            WHERE [ID] = @ID
                        END

                        SELECT @isNotValid
                        ";
                    using (SqlCommand cmd = new SqlCommand(sql, sqlConnection))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@ID", SqlDbType.VarChar).Value = ID;

                        if (sqlConnection.State != ConnectionState.Open)
                            sqlConnection.Open();
                        isNotValid = Convert.ToBoolean(cmd.ExecuteScalar());
                    }

                    if(isNotValid)
                        message.display(PAGETOPIC + "Item tidak lagi valid untuk approval/rejection");
                    else
                    {
                        sql = string.Format(@"
                            UPDATE [DWSystem].[SalesApproval] 
                            SET 
                                [Approved] = {0},
                                [Canceled] = {1},
                                [ApprovedDate] = CURRENT_TIMESTAMP,
                                [ApprovedBy] = @UserName,
                                [WebApp] = 1
                            WHERE [ID] = @ID",
                            Convert.ToInt32(isApproved), Convert.ToInt32(!isApproved));
                        using (SqlCommand cmd = new SqlCommand(sql, sqlConnection))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.Add("@ID", SqlDbType.VarChar).Value = ID;
                            cmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = Tools.getCookieData<string>(GlobalVariables.COOKIEDATA_USERNAME);

                            if (sqlConnection.State != ConnectionState.Open)
                                sqlConnection.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex) { message.error(PAGETOPIC + " gagal di approve. Hubungi administrator. Error: " + ex.Message); }

            populatePage();
        }

        #endregion METHODS
        /*******************************************************************************************************/
        #region EVENT HANDLERS
            
        protected void btnApprove_Command(object sender, CommandEventArgs e)
        {
            update(e.CommandArgument.ToString(), true);
        }

        protected void btnReject_Command(object sender, CommandEventArgs e)
        {
            update(e.CommandArgument.ToString(), false);
        }

        protected void rptParent_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            if (((Button)e.CommandSource).ID == "btnClosePiutangAndCheques")
            {
                ((Panel)e.Item.FindControl("pnlSisaPiutangAndCheques")).Visible = false;
                ((Button)e.Item.FindControl("btnClosePiutangAndCheques")).Visible = false;
                ((Button)e.Item.FindControl("btnShowPiutangAndCheques")).Visible = true;
            }
            else if (((Button)e.CommandSource).ID == "btnShowPiutangAndCheques")
            {
                ((Panel)e.Item.FindControl("pnlSisaPiutangAndCheques")).Visible = true;
                ((Button)e.Item.FindControl("btnClosePiutangAndCheques")).Visible = true;
                ((Button)e.Item.FindControl("btnShowPiutangAndCheques")).Visible = false;
                string sql;

                //SISA PIUTANG
                sql = string.Format(@"
                    SELECT 
                        I.[SalesInvoiceID] as SalesInvoiceID, 
                            I.[InvoiceDate] as InvoiceDate,  
                            I.[DueDate] as DueDate,
                            CAST(DATEDIFF(DAY, I.[InvoiceDate], GETDATE()) as varchar(50)) as Age,
                            I.[GrandTotal], 
                            ISNULL(SUM(IP.[TotalPaidAmount]),0) as PaidAmount, 
                            I.[GrandTotal] - ISNULL(SUM(IP.[TotalPaidAmount]),0) as RemainingAmount, 
                            S.[Status] as [Status]
                    FROM [DWSystem].[SalesInvoice] I
                       LEFT OUTER JOIN([DWSystem].[SellingInvoicePayment] IP INNER JOIN[DWSystem].[ARPayment] P
                           ON IP.[PaymentID] = P.[PaymentID] AND P.[Canceled] = 0)
                                    ON I.[SalesInvoiceID] = IP.[SalesInvoiceID], [DWSystem].[InvoiceStatus] S
                    WHERE 
                        I.[StatusID] = S.[StatusID]
                        AND I.[StatusID] <> 1
                        AND I.[StatusID] <> 4
                        AND I.[StatusID] <> 3
                        AND I.[CustomerID] = {0}
                    GROUP BY I.[CustomerID], I.[SalesInvoiceID], I.[InvoiceDate], I.[DueDate], I.[GrandTotal], S.[Status]

                    UNION

                    SELECT
                        [SalesOrderID] as SalesInvoiceID,
                        [Date] as InvoiceDate,
                        [DeliveryDate] as [DueDate],
                        CAST(DATEDIFF(DAY,[Date], GETDATE()) as varchar(50)) as Age,
                        [GrandTotal],
                        0 as PaidAmount,
                        [GrandTotal] as RemainingAmount,
                        'DELIVERY INVOICE' as [Status]
                    FROM [DWSystem].[SalesOrder]
                    WHERE 
                        [CustomerID] = {0}
                        AND [Rejected] = 0 
                        AND [SalesOrderID] NOT IN (
                              SELECT [SalesOrderID]
                              FROM [DWSystem].[SalesOrderDone]
                          )
                    ORDER BY InvoiceDate ASC
                ", e.CommandArgument.ToString());
                DataTable datatable = DBUtil.getData(sql);

                Repeater rtpSisaPiutang = ((Repeater)e.Item.FindControl("rptSisaPiutang"));
                rtpSisaPiutang.DataSource = datatable;
                rtpSisaPiutang.DataBind();

                //SISA CHEQUES
                sql = string.Format(@"
                    DECLARE @ChequeOwn TABLE(ChequeID varchar(50))

                    INSERT @ChequeOwn
                        SELECT DISTINCT [ChequeID] 
                        FROM [DWSystem].[ARPaymentCheque] C INNER JOIN [DWSystem].[ARPayment] P 
	                        ON C.[PaymentID] = C.[PaymentID] 
		                        AND P.[Canceled] = 0 
		                        AND P.[CustomerID] = {0}

                        UNION

                        SELECT DISTINCT [ChequeID]  
                        FROM [DWSystem].[CreditNoteDownpaymentCheque] C2 INNER JOIN [DWSystem].[CreditNote] CN 
	                        ON C2.[CreditNoteNo] = CN.[CreditNoteNo] 
		                        AND CN.[Approved] = 1 
		                        AND CN.[Canceled] = 0 
		                        AND CN.[CustomerID] = {0}

                    SELECT
                        C.[ChequeID],
                        C.[ChequeNo],
                        C.[InDate],
                        C.[DueDate],
                        C.[Bank],
                        C.[IssueBy],
                        C.[Amount],
                        C.[StatusID],
	                    CASE WHEN C.[DepositID] IS NULL THEN S.[Status] + '(Undeposisted)' ELSE S.[Status] END as [Status] 
                    FROM [DWSystem].[ChequeDeposit]  C,[DWSystem].[ChequeStatus] S 
                    WHERE 
	                    C.[StatusID] = S.[StatusID]
	                    AND C.[StatusID] <> 0 
	                    AND C.[StatusID] <>  0  
	                    AND C.[StatusID] <>  2  
	                    AND C.[StatusID] <>  4 
	                    AND (C.[ChequeID] IN (
					                    SELECT [ChequeID] 
					                    FROM @ChequeOwn
				                    )
			                    OR C.[ChequeID] IN ( 
					                    SELECT [ReplacementChequeID] 
					                    FROM [DWSystem].[ChequeDepositReplacementCheque]
					                    WHERE [ChequeID] IN (
							                    SELECT [ChequeID] 
							                    FROM @ChequeOwn
						                    )
				                    )
		                    )
                ", e.CommandArgument.ToString());
                DataTable dtSisaCheques = DBUtil.getData(sql);
                Repeater rptSisaCheques = ((Repeater)e.Item.FindControl("rptSisaCheques"));
                rptSisaCheques.DataSource = dtSisaCheques;
                rptSisaCheques.DataBind();
            }

            ClientScript.RegisterStartupScript(this.GetType(), "hash", "location.hash = '#" + ((Label)e.Item.FindControl("ID")).Text + "';", true);
        }

        #endregion EVENT HANDLERS
        /*******************************************************************************************************/

    }
}