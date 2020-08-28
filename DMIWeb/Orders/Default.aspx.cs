using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace DMIWeb.Orders
{
    public partial class Default : System.Web.UI.Page
    {
        protected const string PAGETOPIC = "ORDERS";
        protected const string RELATIONNAME = "relation";

        protected string _CategoryName = "";
        protected string _TypeName = "";

        private PagedDataSource _PDS_FilteredInventory = new PagedDataSource();
        public int NowViewing
        {
            get
            {
                object obj = ViewState["_NowViewing"];
                if (obj == null)
                    return 0;
                else
                    return (int)obj;
            }
            set
            {
                this.ViewState["_NowViewing"] = value;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            populateDDLs();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                populateOrdersRepeater();
            }
        }
        
        private void populateDDLs()
        {
            string sqlCalls = "";
            List<string> tableNames = new List<string>()
            {
                "DWSystem.InventoryGroup",
                "DWSystem.Customer",
                "DWSystem.InventoryCategory",
                "DWSystem.InventoryType"
            };
            sqlCalls += string.Format(@"
                SELECT CategoryName, TypeName 
                FROM DWSystem.InventoryGroup 
                WHERE GroupID = 1; 

                SELECT Customer.*
                FROM DWSystem.Customer
                WHERE 
                    (SELECT SalesID FROM DWSystem.Operator WHERE UserName='{0}') IS NULL
                    OR Customer.SalesID = (SELECT SalesID FROM DWSystem.Operator WHERE UserName='{0}')
                ORDER BY Name ASC;

                SELECT InventoryCategory.*
                FROM DWSystem.InventoryCategory
                ORDER BY Name ASC;

                SELECT InventoryType.*
                FROM DWSystem.InventoryType
                ORDER BY Name ASC;
            ", Tools.getCookieData<string>(GlobalVariables.COOKIEDATA_USERNAME));
            DataSet dataset = DBUtil.getDataSet(sqlCalls);

            DataRow row = dataset.Tables[tableNames.IndexOf("DWSystem.InventoryGroup")].Rows[0];

            _CategoryName = row["CategoryName"].ToString();
            _TypeName = row["TypeName"].ToString();

            ddlCustomers.DataSource = dataset.Tables[tableNames.IndexOf("DWSystem.Customer")];
            ddlCustomers.DataValueField = "CustomerID";
            ddlCustomers.DataTextField = "Name";
            ddlCustomers.DataBind();

            ddlCategory.DataSource = dataset.Tables[tableNames.IndexOf("DWSystem.InventoryCategory")];
            ddlCategory.DataValueField = "CategoryID";
            ddlCategory.DataTextField = "Name";
            ddlCategory.DataBind();

            ddlType.DataSource = dataset.Tables[tableNames.IndexOf("DWSystem.InventoryType")];
            ddlType.DataValueField = "TypeID";
            ddlType.DataTextField = "Name";
            ddlType.DataBind();
        }

        protected void lbtnClearKeyword_Click(object sender, EventArgs e)
        {
            txtKeyword.Text = "";
        }

        protected void chkCategory_CheckedChanged(object sender, EventArgs e)
        {
            ddlCategory.Enabled = chkCategory.Checked;
        }

        protected void chkType_CheckedChanged(object sender, EventArgs e)
        {
            ddlType.Enabled = chkType.Checked;
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            NowViewing = 0;
            populateInventoryRepeater(false);
        }

        private void populateInventoryRepeater(bool showLast)
        {
            string filter = "1=1";
            if (!string.IsNullOrWhiteSpace(txtKeyword.Text.Trim()))
                filter = Tools.append(filter, string.Format("({0})", LIBUtil.Util.compileQuickSearchFilter(txtKeyword.Text, new string[] { "Inventory.InventoryName", "Inventory.InventoryID" })), " AND ");
            if (chkCategory.Checked)
                filter = Tools.append(filter, string.Format("Inventory.CategoryID='{0}'", ddlCategory.SelectedValue), " AND ");
            if (chkType.Checked)
                filter = Tools.append(filter, string.Format("Inventory.TypeID='{0}'", ddlType.SelectedValue), " AND ");

            string sql = string.Format(@"

	            -- drop table if already exists
	            IF(SELECT object_id('TempDB..#TEMP_PendingOrders')) IS NOT NULL
		            DROP TABLE #TEMP_PendingOrders
		
	            SELECT * INTO #TEMP_PendingOrders 
                FROM (
                    SELECT CustomerOrder.*
                    FROM DWSystem.CustomerOrder
                        LEFT OUTER JOIN DWSystem.Inventory ON Inventory.InventoryID = CustomerOrder.InventoryID
                    WHERE CustomerOrder.CustomerID='{1}' 
                        AND (CustomerOrder.Stop = 0 OR CustomerOrder.Stop IS NULL)
                        AND CustomerOrder.DeliveredQuantity < CustomerOrder.Quantity
                ) AS x

                SELECT Inventory.*,
                    InventoryCategory.Name AS CategoryName,
                    InventoryType.Name AS TypeName,
                    #TEMP_PendingOrders.Quantity - #TEMP_PendingOrders.DeliveredQuantity AS PendingOrderQuantity,
                    #TEMP_PendingOrders.Discount1,
                    #TEMP_PendingOrders.Discount2,
                    #TEMP_PendingOrders.ExpiredDate,
					ISNULL(InventoryStock.Stock,0) AS Quantity,
					InventoryUnit.Unit AS Unit
                FROM DWSystem.Inventory
                    LEFT OUTER JOIN DWSystem.InventoryCategory ON InventoryCategory.CategoryID = Inventory.CategoryID
                    LEFT OUTER JOIN DWSystem.InventoryType ON InventoryType.TypeID = Inventory.TypeID
                    LEFT OUTER JOIN #TEMP_PendingOrders ON #TEMP_PendingOrders.InventoryID = Inventory.InventoryID
					LEFT OUTER JOIN DWSystem.InventoryUnit ON InventoryUnit.UnitID = Inventory.UnitID
					LEFT OUTER JOIN (
							SELECT InventoryID, SUM(Stock) AS Stock
							FROM DWSystem.WarehouseStock
                            WHERE WarehouseStock.WarehouseID IN (SELECT WarehouseID FROM DWSystem.WarehouseAccess WHERE UserName = '{2}')
							GROUP BY InventoryID					
						) InventoryStock ON InventoryStock.InventoryID = Inventory.InventoryID
                WHERE {0}
                ORDER BY InventoryName ASC;
	
	            -- clean up
	            DROP TABLE #TEMP_PendingOrders
            ", filter, ddlCustomers.SelectedValue, ((Site)this.Master).username);
            _PDS_FilteredInventory.DataSource = DBUtil.getData(sql).DefaultView;
            _PDS_FilteredInventory.PageSize = Convert.ToInt16(ddlPageSizes.SelectedValue);
            _PDS_FilteredInventory.AllowPaging = true;

            if (showLast)
                NowViewing = _PDS_FilteredInventory.PageCount - 1;

            _PDS_FilteredInventory.CurrentPageIndex = NowViewing;

            rptInventory.DataSource = _PDS_FilteredInventory;
            rptInventory.DataBind();

            lblInventoryHasNoData.Visible = rptInventory.Items.Count == 0;
            rptInventory.Visible = pnlInventoryPaging1.Visible = pnlInventoryPaging2.Visible = rptInventory.Items.Count > 0;            
            lblPageIndex1.Text = lblPageIndex2.Text = (NowViewing + 1).ToString();
            lblPageCount1.Text = lblPageCount2.Text = _PDS_FilteredInventory.PageCount.ToString();
            btnPrevious1.Enabled = btnFirst1.Enabled = btnPrevious2.Enabled = btnFirst2.Enabled = !_PDS_FilteredInventory.IsFirstPage;
            btnNext1.Enabled = btnLast1.Enabled = btnNext2.Enabled = btnLast2.Enabled = !_PDS_FilteredInventory.IsLastPage;
        }

        private void populateOrdersRepeater()
        {
            string sql = string.Format(@"
                SELECT CustomerOrder.*,
                    Inventory.InventoryName AS InventoryName,
                    InventoryCategory.Name AS CategoryName,
                    InventoryType.Name AS TypeName,
					InventoryUnit.Unit AS Unit,
                    CustomerOrder.Quantity - CustomerOrder.DeliveredQuantity AS PendingOrderQuantity,
                    CustomerOrder.Price * (100-Discount1)/100 * (100-Discount2)/100 AS AdjustedPrice,
                    CustomerOrder.Quantity * (CustomerOrder.Price * (100-Discount1)/100 * (100-Discount2)/100) AS OrderAmount,
                    ROW_NUMBER() OVER (ORDER BY InventoryName ASC) AS RowNumber
                FROM DWSystem.CustomerOrder
                    LEFT OUTER JOIN DWSystem.Inventory ON Inventory.InventoryID = CustomerOrder.InventoryID
                    LEFT OUTER JOIN DWSystem.InventoryCategory ON InventoryCategory.CategoryID = Inventory.CategoryID
                    LEFT OUTER JOIN DWSystem.InventoryType ON InventoryType.TypeID = Inventory.TypeID
					LEFT OUTER JOIN DWSystem.InventoryUnit ON InventoryUnit.UnitID = Inventory.UnitID
                WHERE CustomerOrder.CustomerID='{0}' 
                    AND (Stop = 0 OR Stop IS NULL)
                    AND CustomerOrder.Quantity - CustomerOrder.DeliveredQuantity > 0 
                ORDER BY InventoryName ASC;
            ", ddlCustomers.SelectedValue);
            DataTable data = DBUtil.getData(sql);
            rptOrders.DataSource = data;
            rptOrders.DataBind();

            lblOrdersHasNoData.Visible = rptOrders.Items.Count == 0;
            rptOrders.Visible = rptOrders.Items.Count > 0;
            lblOrderTotalAmount.Text = string.Format("({0:N2})", LIBUtil.Util.compute(data, "SUM", "OrderAmount", ""));
        }

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            NowViewing--;
            populateInventoryRepeater(false);
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            NowViewing++;
            populateInventoryRepeater(false);
        }

        protected void btnFirst_Click(object sender, EventArgs e)
        {
            NowViewing = 0;
            populateInventoryRepeater(false);
        }

        protected void btnLast_Click(object sender, EventArgs e)
        {
            populateInventoryRepeater(true);
        }

        protected void rptInventory_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            if (((Button)e.CommandSource).ID == "btnAdd")
            {
                decimal price = 0;
                if (!Decimal.TryParse(((HtmlGenericControl)e.Item.FindControl("spanPrice")).InnerText, out price))
                {
                    message.error("Price is Invalid");
                    return;
                }
                int qty;
                if(!Int32.TryParse(((TextBox)e.Item.FindControl("txtQty")).Text, out qty))
                {
                    message.error("Invalid QTY");
                    SetFocus(e.Item.FindControl("txtQty"));
                    return;
                }
                //DateTime expiredate = DateTime.Now;
                //if (!DateTime.TryParseExact(((TextBox)e.Item.FindControl("txtExpireDate")).Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out expiredate))
                //{
                //    message.error("Select expire date");
                //    SetFocus(e.Item.FindControl("txtExpireDate"));
                //    return;
                //}
                decimal discount1 = 0;
                if (!string.IsNullOrWhiteSpace(((TextBox)e.Item.FindControl("txtDiscount1")).Text) && !Decimal.TryParse(((TextBox)e.Item.FindControl("txtDiscount1")).Text, out discount1))
                {
                    message.error("Discount 1 is Invalid");
                    SetFocus(e.Item.FindControl("txtDiscount1"));
                    return;
                }
                decimal discount2 = 0;
                if (!string.IsNullOrWhiteSpace(((TextBox)e.Item.FindControl("txtDiscount2")).Text) && !Decimal.TryParse(((TextBox)e.Item.FindControl("txtDiscount2")).Text, out discount2))
                {
                    message.error("Discount 2 is Invalid");
                    SetFocus(e.Item.FindControl("txtDiscount2"));
                    return;
                }

                try
                {
                    string sql = string.Format(@"
                            DECLARE @timestamp datetime
                            SELECT @timestamp=CURRENT_TIMESTAMP

                            INSERT INTO [DWSystem].[CustomerOrder] (
                                CustomerID,
                                InventoryID,
                                Quantity,
                                DeliveredQuantity,
                                Price,
                                Discount1,
                                Discount2,
                                Total,
                                OrderDate,
                                ExpiredDate,
                                LastUpdated,
                                UserID
                            )
                            VALUES (
                                @CustomerID,
                                @InventoryID,
                                @Quantity,
                                0,
                                @Price,
                                @Discount1,
                                @Discount2,
                                (@Quantity * @Price * (100-@Discount1)/100 * (100-@Discount2)/100),
                                @timestamp,
                                null,
                                @timestamp,
                                (SELECT ID FROM DWSystem.Operator WHERE UserName = @UserName)
                            )
                    ");

                    using (SqlConnection sqlConnection = new SqlConnection(Settings.ConnectionString))
                    using (SqlCommand cmd = new SqlCommand(sql, sqlConnection))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@CustomerID", SqlDbType.VarChar).Value = ddlCustomers.SelectedValue;
                        cmd.Parameters.Add("@InventoryID", SqlDbType.VarChar).Value = e.CommandArgument.ToString();
                        cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = qty;
                        cmd.Parameters.Add("@Price", SqlDbType.Int).Value = price;
                        cmd.Parameters.Add("@Discount1", SqlDbType.Decimal).Value = discount1;
                        cmd.Parameters.Add("@Discount2", SqlDbType.Decimal).Value = discount2;
                        //cmd.Parameters.Add("@ExpiredDate", SqlDbType.DateTime).Value = expiredate;
                        cmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = Tools.getCookieData<string>(GlobalVariables.COOKIEDATA_USERNAME);

                        if (sqlConnection.State != ConnectionState.Open)
                            sqlConnection.Open();
                        cmd.ExecuteNonQuery();
                    }

                    populateOrdersRepeater();
                    populateInventoryRepeater(false);
                    message.display("Submit berhasil");
                }
                catch (Exception ex) { message.error("Submit gagal. Hubungi administrator. Error: " + ex.Message); }
            }
        }

        protected void ddlCustomers_SelectedIndexChanged(object sender, EventArgs e)
        {
            populateOrdersRepeater();
        }

        protected void chkShowQty_OnCheckedChanged(object sender, EventArgs e)
        {
            populateInventoryRepeater(false);
        }
        
        protected void rptOrders_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            if (((Button)e.CommandSource).ID == "btnStop")
            {
                try
                {
                    string sql = string.Format(@"
                            UPDATE [DWSystem].[CustomerOrder] 
                            SET Stop = 1,
                                StopDate = CURRENT_TIMESTAMP,
                                StopBy = (SELECT ID FROM DWSystem.Operator WHERE UserName = @UserName)
                            WHERE CustomerOrderID = @CustomerOrderID
                    ");

                    using (SqlConnection sqlConnection = new SqlConnection(Settings.ConnectionString))
                    using (SqlCommand cmd = new SqlCommand(sql, sqlConnection))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@CustomerOrderID", SqlDbType.VarChar).Value = e.CommandArgument.ToString();
                        cmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = Tools.getCookieData<string>(GlobalVariables.COOKIEDATA_USERNAME);

                        if (sqlConnection.State != ConnectionState.Open)
                            sqlConnection.Open();
                        cmd.ExecuteNonQuery();
                    }

                    populateOrdersRepeater();
                    if (rptInventory.Visible)
                        populateInventoryRepeater(false);
                    message.display("Stop berhasil");
                }
                catch (Exception ex) { message.error("Stop gagal. Hubungi administrator. Error: " + ex.Message); }
            }
            else if (((Button)e.CommandSource).ID == "btnUpdate")
            {
                decimal price = 0;
                if (!Decimal.TryParse(((HtmlGenericControl)e.Item.FindControl("spanPrice")).InnerText, out price))
                {
                    message.error("Price is Invalid");
                    return;
                }
                int pendingQty;
                if (!Int32.TryParse(((TextBox)e.Item.FindControl("txtPendingQty")).Text, out pendingQty))
                {
                    message.error("Invalid Pending QTY");
                    SetFocus(e.Item.FindControl("txtPendingQty"));
                    return;
                }
                //DateTime expiredate = DateTime.Now;
                //if (!DateTime.TryParseExact(((TextBox)e.Item.FindControl("txtExpireDate")).Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out expiredate))
                //{
                //    message.error("Select expire date");
                //    SetFocus(e.Item.FindControl("txtExpireDate"));
                //    return;
                //}
                decimal discount1 = 0;
                if (!string.IsNullOrWhiteSpace(((TextBox)e.Item.FindControl("txtDiscount1")).Text) && !Decimal.TryParse(((TextBox)e.Item.FindControl("txtDiscount1")).Text, out discount1))
                {
                    message.error("Discount 1 is Invalid");
                    SetFocus(e.Item.FindControl("txtDiscount1"));
                    return;
                }
                decimal discount2 = 0;
                if (!string.IsNullOrWhiteSpace(((TextBox)e.Item.FindControl("txtDiscount2")).Text) && !Decimal.TryParse(((TextBox)e.Item.FindControl("txtDiscount2")).Text, out discount2))
                {
                    message.error("Discount 2 is Invalid");
                    SetFocus(e.Item.FindControl("txtDiscount2"));
                    return;
                }

                try
                {
                    string sql = string.Format(@"
                            UPDATE [DWSystem].[CustomerOrder] 
                            SET Quantity = @PendingQuantity + DeliveredQuantity,
                                Discount1 = @Discount1,
                                Discount2 = @Discount2,
                                Total = ((@PendingQuantity + DeliveredQuantity) * Price * (100-@Discount1)/100 * (100-Discount2)/100),
                                ExpiredDate = null,
                                LastUpdated = CURRENT_TIMESTAMP,
                                UserID = (SELECT ID FROM DWSystem.Operator WHERE UserName = @UserName)
                            WHERE CustomerOrderID = @CustomerOrderID
                    ");

                    using (SqlConnection sqlConnection = new SqlConnection(Settings.ConnectionString))
                    using (SqlCommand cmd = new SqlCommand(sql, sqlConnection))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@CustomerOrderID", SqlDbType.VarChar).Value = e.CommandArgument.ToString();
                        cmd.Parameters.Add("@PendingQuantity", SqlDbType.Int).Value = pendingQty;
                        cmd.Parameters.Add("@Discount1", SqlDbType.Decimal).Value = discount1;
                        cmd.Parameters.Add("@Discount2", SqlDbType.Decimal).Value = discount2;
                        //cmd.Parameters.Add("@ExpiredDate", SqlDbType.DateTime).Value = expiredate;
                        cmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = Tools.getCookieData<string>(GlobalVariables.COOKIEDATA_USERNAME);

                        if (sqlConnection.State != ConnectionState.Open)
                            sqlConnection.Open();
                        cmd.ExecuteNonQuery();
                    }

                    populateOrdersRepeater();
                    if(rptInventory.Visible)
                        populateInventoryRepeater(false);
                    message.display("Update berhasil");
                }
                catch (Exception ex) { message.error("Update gagal. Hubungi administrator. Error: " + ex.Message); }
            }
        }

        protected void chkHasPendingOrders_CheckedChanged(object sender, EventArgs e)
        {
            DataTable data = DBUtil.getData(string.Format(@"
                DECLARE @HasPendingOrdersOnly bit = '{1}'

                SELECT Customer.*
                FROM DWSystem.Customer
                    LEFT OUTER JOIN (
                            SELECT CustomerID, COUNT(CustomerOrder.CustomerID) AS PendingOrderCount
                            FROM DWSystem.CustomerOrder
                            WHERE 
                                (Stop = 0 OR Stop IS NULL)
                                AND CustomerOrder.Quantity - CustomerOrder.DeliveredQuantity > 0 
                            GROUP BY CustomerOrder.CustomerID
                        ) CustomerOrders ON CustomerOrders.CustomerID = Customer.CustomerID
                WHERE
                    ((SELECT SalesID FROM DWSystem.Operator WHERE UserName='{0}') IS NULL
                        OR Customer.SalesID = (SELECT SalesID FROM DWSystem.Operator WHERE UserName='{0}')
                    )
                    AND (@HasPendingOrdersOnly = 0 OR CustomerOrders.PendingOrderCount > 0)
                ORDER BY Name ASC;
            ", Tools.getCookieData<string>(GlobalVariables.COOKIEDATA_USERNAME), chkHasPendingOrders.Checked));
            

            ddlCustomers.DataSource = data;
            ddlCustomers.DataValueField = "CustomerID";
            ddlCustomers.DataTextField = "Name";
            ddlCustomers.DataBind();
        }
    }
}