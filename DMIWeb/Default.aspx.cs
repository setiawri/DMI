using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Globalization;
using System.Data;

using LIBUtil;

namespace DMIWeb
{
    public partial class Default : System.Web.UI.Page
    {
        /*******************************************************************************************************/
        #region SETTINGS

        #endregion SETTINGS
        /*******************************************************************************************************/
        #region PUBLIC VARIABLES

        #endregion PUBLIC VARIABLES
        /*******************************************************************************************************/
        #region PRIVATE VARIABLES

        private const string ERROR_NOPRIVILEGE = "Anda tidak mempunya akses. Harap hubungi administrator.";
        private const string DATEFORMAT = "dd/MM/yyyy";

        #endregion PRIVATE VARIABLES
        /*******************************************************************************************************/
        #region INITIATION METHODS

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!UserAccount.IsAuthenticated)
            {
                Response.Redirect("~/Login.aspx?returnUrl=" + Request.RawUrl);
            }
            else
            {
                if (!Tools.getCookieData<bool>(GlobalVariables.COOKIEDATA_CANAPPROVESALES)
                    && !Tools.getCookieData<bool>(GlobalVariables.COOKIEDATA_CANVIEWPROFITLOSSSTATEMENT)
                    && Tools.getCookieData<bool>(GlobalVariables.COOKIEDATA_CANVIEWCUSTOMERORDER))
                    Response.Redirect("~/Orders");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                //textbox set to readonly causes value not retained during postback. This is the fix
                txtDateStart.Text = Request[txtDateStart.UniqueID];
                txtDateEnd.Text = Request[txtDateEnd.UniqueID];
            }
            else
            {
                if (Tools.getCookieData<bool>(GlobalVariables.COOKIEDATA_CANAPPROVESALES))
                {
                    pnlSalesApprovalLink.Visible = true;
                    populateSalesApprovalCount();
                }
                if (Tools.getCookieData<bool>(GlobalVariables.COOKIEDATA_CANVIEWPROFITLOSSSTATEMENT))
                {
                    pnlProfitLossStatement.Visible = true;

                    //by default set date range to previous month
                    txtDateStart.Text = string.Format("{0:dd/MM/yyyy}", Util.addMonths(-1, true, false));
                    txtDateEnd.Text = string.Format("{0:dd/MM/yyyy}", Util.addMonths(-1, false, true));
                }
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            if (!Tools.getCookieData<bool>(GlobalVariables.COOKIEDATA_CANAPPROVESALES) 
                && !Tools.getCookieData<bool>(GlobalVariables.COOKIEDATA_CANVIEWCUSTOMERORDER) 
                && !Tools.getCookieData<bool>(GlobalVariables.COOKIEDATA_CANVIEWPROFITLOSSSTATEMENT))
                
                message.error(ERROR_NOPRIVILEGE);
        }

        #endregion
        /*******************************************************************************************************/
        #region METHODS 

        private const string TABLENAME_CombinedValueQueries = "TABLENAME_CombinedValueQueries";


        private void populateSalesApprovalCount()
        {
            string sqlCalls = "";
            List<string> tableNames = new List<string>();

            //build sql for combined values
            string sqlResultTable = "";
            tableNames.Add(TABLENAME_CombinedValueQueries);
            insertSQL_SalesApprovalCount(ref sqlCalls, ref sqlResultTable);
            sqlCalls += sqlResultTable + ";";
            
            //run query
            DataSet dataset = DBUtil.getDataSet(sqlCalls);
            DataRow row = dataset.Tables[tableNames.IndexOf(TABLENAME_CombinedValueQueries)].Rows[0];

            //update sales approval count
            lblSalesApprovalCount.Text = Util.wrapNullable<string>(row, VAR_SalesApprovalCount);
            if (lbtnSalesApproval.Text == "0") lbtnSalesApproval.Enabled = false;            
        }

        private void populateProfitLossStatement()
        {
            string sqlCalls = "";
            List<string> tableNames = new List<string>();

            //build sql for combined values
            string sqlResultTable = "";
            tableNames.Add(TABLENAME_CombinedValueQueries);
            insertSQL_SisaPiutangUsahaAmount(ref sqlCalls, ref sqlResultTable);
            insertSQL_SisaPiutangLewatJTAmount(ref sqlCalls, ref sqlResultTable);
            if (pnlLaporanKeuangan.Visible)
                insertSQL_PenjualanAmount(ref sqlCalls, ref sqlResultTable);
            sqlCalls += sqlResultTable + ";";

            //build sql for table calls
            insertSQL_Table_Persediaan(ref sqlCalls, ref tableNames);
            insertSQL_Table_HutangJatuhTempo(ref sqlCalls, ref tableNames);
            insertSQL_Table_HutangJatuhTempoDalam7Hari(ref sqlCalls, ref tableNames);

            //run query
            DataSet dataset = DBUtil.getDataSet(sqlCalls);
            DataRow row = dataset.Tables[tableNames.IndexOf(TABLENAME_CombinedValueQueries)].Rows[0];
            
            lblPiutangUsahaAmount.Text = string.Format("{0:N2}", Util.wrapNullable<decimal>(row, VAR_SisaPiutangUsahaAmount));
            lblPiutangLewatJTAmount.Text = string.Format("{0:N2}", Util.wrapNullable<decimal>(row, VAR_SisaPiutangLewatJTAmount));

            //update laporan keuangan
            if (pnlLaporanKeuangan.Visible)
            {
                lblPenjualanAmount.Text = string.Format("{0:N2}", Util.wrapNullable<decimal>(row, VAR_PenjualanAmount));
                lblCOGSAmount.Text = string.Format("{0:N2}", Util.wrapNullable<decimal>(row, VAR_COGSAmount));
                lblLabaKotorAmount.Text = string.Format("{0:N2}", Util.wrapNullable<decimal>(row, VAR_LabaKotorAmount));
            }

            lblPersediaanAmount.Text = string.Format("{0:N2}", Util.compute(dataset.Tables[tableNames.IndexOf(TABLENAME_Persediaan)], "SUM", "TotalValue", ""));
            rptPersediaan.DataSource = dataset.Tables[tableNames.IndexOf(TABLENAME_Persediaan)];
            rptPersediaan.DataBind();

            lblHutangJatuhTempoAmount.Text = string.Format("{0:N2}", Util.compute(dataset.Tables[tableNames.IndexOf(TABLENAME_HutangJatuhTempo)], "SUM", "TotalUnpaidDue", ""));
            rptHutangJatuhTempo.DataSource = dataset.Tables[tableNames.IndexOf(TABLENAME_HutangJatuhTempo)];
            rptHutangJatuhTempo.DataBind();

            lblHutangJatuhTempoDalam7Hari.Text = string.Format("{0:N2}", Util.compute(dataset.Tables[tableNames.IndexOf(TABLENAME_HutangJatuhTempoDalam7Hari)], "SUM", "TotalUnpaidDueIn7Day", ""));
            rptHutangJatuhTempoDalam7Hari.DataSource = dataset.Tables[tableNames.IndexOf(TABLENAME_HutangJatuhTempoDalam7Hari)];
            rptHutangJatuhTempoDalam7Hari.DataBind();
        }

        private void updateSQLResultTable(ref string sqlResultTable, string select)
        {
            if (string.IsNullOrWhiteSpace(sqlResultTable))
                sqlResultTable += string.Format(@"
                    SELECT {0}", select); //add extra line at the top
            else
                sqlResultTable += string.Format(", {0}", select);
        }

        #endregion
        /*******************************************************************************************************/
        #region PERSEDIAAN

        private const string TABLENAME_Persediaan = "TABLENAME_Persediaan";
        private void insertSQL_Table_Persediaan(ref string sqlCalls, ref List<string> tableNames)
        {
            tableNames.Add(TABLENAME_Persediaan);

            sqlCalls += string.Format(@"

                DECLARE @CountPending as BIGINT 
                DECLARE @TotalValue as float,@TotalQty as bigint 
                DECLARE @InventoryID as varchar(50),@Stock as bigint,@COG as Float 
                DECLARE @TableStock TABLE(InventoryID varchar(50),Stock bigint,COG float)

                DECLARE CPOINT CURSOR FOR
                SELECT InventoryID,SUM(Stock)
                FROM DWSystem.WarehouseStock 
                GROUP BY InventoryID 
                HAVING SUM(Stock) > 0 

                OPEN CPOINT 
                FETCH NEXT FROM CPOINT INTO @InventoryID,@Stock 
                WHILE (@@FETCH_STATUS = 0) 
                BEGIN 

                SELECT @TotalQty = ISNULL(SUM(ML.[Quantity]),0),@TotalValue = ISNULL(SUM(ML.[Quantity] * ML.[COG]),0) 
                FROM [DWSystem].[BBM] M,[DWSystem].[BBMLog] ML 
                WHERE M.[BBMID] = ML.[BBMID] 
                AND M.[BBMTypeID] <> 4 
                AND M.[Canceled] = 0 AND ML.[COG] IS NOT NULL 
                AND ML.[InventoryID] = @InventoryID 
         
                SELECT @TotalQty = @TotalQty - ISNULL(SUM(KL.[Quantity]),0),@TotalValue = @TotalValue - ISNULL(SUM(KL.[Quantity] * KL.[COG]),0) 
                FROM [DWSystem].[BBK] K,[DWSystem].[BBKLog] KL 
                WHERE K.[BBKID] = KL.[BBKID] 
                AND K.[BBKTypeID] <> 4 
                AND K.[Canceled] = 0 AND KL.[COG] IS NOT NULL 
                AND KL.[InventoryID] = @InventoryID 
           
                SELECT @COG = CASE WHEN @TotalQty <> 0 THEN @TotalValue/@TotalQty ELSE 0 END
            
                INSERT INTO @TableStock(InventoryID,Stock,COG) VALUES (@InventoryID,@Stock,@COG) 
                FETCH NEXT FROM CPOINT INTO @InventoryID,@Stock 
                END 
                CLOSE CPOINT 
                DEALLOCATE CPOINT 

                SELECT I.[CategoryID],C.[Name] as Brand,SUM([Stock]) as TotalStok,SUM([Stock]*[COG]) as TotalValue
                FROM @TableStock S,[DWSystem].[Inventory] I,[DWSystem].[InventoryCategory] C, [DWSystem].[InventoryType] T 
                WHERE S.[InventoryID] = I.[InventoryID] AND I.[CategoryID] = C.[CategoryID] 
                AND I.[TypeID] = T.[TypeID]
                GROUP BY I.[CategoryID],C.[Name] 
                ORDER BY Brand

                    ");
        }

        #endregion
        /*******************************************************************************************************/
        #region HUTANG JATUH TEMPO

        private const string TABLENAME_HutangJatuhTempo = "TABLENAME_HutangJatuhTempo";
        private void insertSQL_Table_HutangJatuhTempo(ref string sqlCalls, ref List<string> tableNames)
        {
            tableNames.Add(TABLENAME_HutangJatuhTempo);

            sqlCalls += string.Format(@"

                DECLARE @TEMPTABLE4 TABLE([SupplierID] varchar(50),[Supplier] varchar(50),[TotalUnpaid] Float) 
                INSERT INTO @TEMPTABLE4 
                SELECT PO.[SupplierID],C.[Name] as Supplier,
                CAST(CASE WHEN (PO.[GrandTotalConverted] IS NOT NULL) THEN PO.[GrandTotalConverted] - ISNULL(SUM(IP.[TotalPaidAmount]),0) ELSE 
                (ISNULL(PO.[GrandTotal],0) - ISNULL(SUM(IP.[TotalPaidAmount]),0)) END as decimal(18, 3)) as TotalUnpaid 

                FROM (([DWSystem].[PurchaseInvoice] PO LEFT OUTER JOIN ([DWSystem].[AccountPayable] AR 
                INNER JOIN [DWSystem].[AccountPayableType] AT ON AR.[TypeID] = AT.[TypeID]) 
                ON PO.[PurchaseInvoiceID] = AR.[PurchaseInvoiceID]) LEFT OUTER JOIN 
                ([DWSystem].[PurchasingInvoicePayment] IP INNER JOIN [DWSystem].[APPayment] P 
                ON IP.[PaymentID] = P.[PaymentID] AND P.[Canceled] = 0) 
                ON PO.[PurchaseInvoiceID] = IP.[PurchaseInvoiceID]), 
                [DWSystem].[Supplier] C,[DWSystem].[Currency] Y,[DWSystem].[InvoiceStatus] ST 
                WHERE PO.[SupplierID] = C.[SupplierID] AND 
                PO.[StatusID] = ST.[StatusID] AND PO.[CurrencyID] = Y.[CurrencyID] AND PO.[Approved] = 1 
                AND PO.[StatusID] <> 1 --InvoiceStatus.PAID   
                AND PO.[StatusID] <> 4 --InvoiceStatus.PAID_PENDING  
                AND PO.[StatusID] <> 3 --InvoiceStatus.CANCELED  
                AND PO.[DueDate] < GETDATE()
                GROUP BY PO.[PurchaseInvoiceID],PO.[TaxInvoiceNo],PO.[InvoiceNo],PO.[Date],PO.[InvoiceDate],
                PO.[DueDate],PO.[SupplierID],C.[Name],PO.[CurrencyID], Y.[CurrencyName],Y.[CurrencySymbol],
                Y.[CurrencyText],PO.[Rate],PO.[SubTotal],PO.[Discount],PO.[DPercent],PO.[TaxPercent],PO.[Tax],
                PO.[PPHPercent],PO.[PPH], PO.[MiscTotal],PO.[GrandTotal],PO.[GrandTotalConverted],PO.[Note],
                PO.[UserName],PO.[Approved],PO.[Rejected],PO.[ValidatedDate], PO.[ValidatedBy],PO.[LastUpdated],PO.[UpdatedBy],
                PO.[StatusID], ST.[Status], AR.[TypeID], AT.[Type], AT.[Description],PO.[TotalBM],PO.[TotalBMIDR] 

                SELECT [SupplierID],[Supplier],SUM([TotalUnpaid]) as TotalUnpaidDue
                FROM @TEMPTABLE4 
                GROUP BY [SupplierID],[Supplier] 
                HAVING SUM([TotalUnpaid]) > 0
                ORDER BY [Supplier] 

                    ");
        }

        #endregion
        /*******************************************************************************************************/
        #region HUTANG JATUH TEMPO DALAM 7 HARI

        private const string TABLENAME_HutangJatuhTempoDalam7Hari = "TABLENAME_HutangJatuhTempoDalam7Hari";
        private void insertSQL_Table_HutangJatuhTempoDalam7Hari(ref string sqlCalls, ref List<string> tableNames)
        {
            tableNames.Add(TABLENAME_HutangJatuhTempoDalam7Hari);

            sqlCalls += string.Format(@"

                DECLARE @TEMPTABLE5 TABLE([SupplierID] varchar(50),[Supplier] varchar(50),[TotalUnpaid] Float) 
                INSERT INTO @TEMPTABLE5 
                SELECT PO.[SupplierID],C.[Name] as Supplier,
                CAST(CASE WHEN (PO.[GrandTotalConverted] IS NOT NULL) THEN PO.[GrandTotalConverted] - ISNULL(SUM(IP.[TotalPaidAmount]),0) ELSE 
                (ISNULL(PO.[GrandTotal],0) - ISNULL(SUM(IP.[TotalPaidAmount]),0)) END as decimal(18, 3)) as TotalUnpaid 

                FROM (([DWSystem].[PurchaseInvoice] PO LEFT OUTER JOIN ([DWSystem].[AccountPayable] AR 
                INNER JOIN [DWSystem].[AccountPayableType] AT ON AR.[TypeID] = AT.[TypeID]) 
                ON PO.[PurchaseInvoiceID] = AR.[PurchaseInvoiceID]) LEFT OUTER JOIN 
                ([DWSystem].[PurchasingInvoicePayment] IP INNER JOIN [DWSystem].[APPayment] P 
                ON IP.[PaymentID] = P.[PaymentID] AND P.[Canceled] = 0) 
                ON PO.[PurchaseInvoiceID] = IP.[PurchaseInvoiceID]), 
                [DWSystem].[Supplier] C,[DWSystem].[Currency] Y,[DWSystem].[InvoiceStatus] ST 
                WHERE PO.[SupplierID] = C.[SupplierID] AND 
                PO.[StatusID] = ST.[StatusID] AND PO.[CurrencyID] = Y.[CurrencyID] AND PO.[Approved] = 1 
                AND PO.[StatusID] <> 1 --InvoiceStatus.PAID   
                AND PO.[StatusID] <> 4 --InvoiceStatus.PAID_PENDING  
                AND PO.[StatusID] <> 3 --InvoiceStatus.CANCELED  
                AND PO.[DueDate] < DATEADD(DAY,7,GETDATE())
                GROUP BY PO.[PurchaseInvoiceID],PO.[TaxInvoiceNo],PO.[InvoiceNo],PO.[Date],PO.[InvoiceDate],
                PO.[DueDate],PO.[SupplierID],C.[Name],PO.[CurrencyID], Y.[CurrencyName],Y.[CurrencySymbol],
                Y.[CurrencyText],PO.[Rate],PO.[SubTotal],PO.[Discount],PO.[DPercent],PO.[TaxPercent],PO.[Tax],
                PO.[PPHPercent],PO.[PPH], PO.[MiscTotal],PO.[GrandTotal],PO.[GrandTotalConverted],PO.[Note],
                PO.[UserName],PO.[Approved],PO.[Rejected],PO.[ValidatedDate], PO.[ValidatedBy],PO.[LastUpdated],PO.[UpdatedBy],
                PO.[StatusID], ST.[Status], AR.[TypeID], AT.[Type], AT.[Description],PO.[TotalBM],PO.[TotalBMIDR] 

                SELECT [SupplierID],[Supplier],SUM([TotalUnpaid]) as TotalUnpaidDueIn7Day
                FROM @TEMPTABLE5 
                GROUP BY [SupplierID],[Supplier] 
                HAVING SUM([TotalUnpaid]) > 0
                ORDER BY [Supplier] 

                    ");
        }

        #endregion
        /*******************************************************************************************************/
        #region SALES APPROVAL COUNT

        private const string VAR_SalesApprovalCount = "VAR_SalesApprovalCount";
        private void insertSQL_SalesApprovalCount(ref string sqlCalls, ref string sqlResultTable)
        {
            sqlCalls += string.Format(@"

                    DECLARE @{0} int;

                    SELECT @{0} = ISNULL(COUNT(SalesApproval.ID),0)
                    FROM [DWSystem].SalesApproval
                    WHERE SalesApproval.[Approved] = 0 AND SalesApproval.[Canceled] = 0;

                    ", VAR_SalesApprovalCount);

            updateSQLResultTable(ref sqlResultTable, string.Format("@{0} AS {0}", VAR_SalesApprovalCount));
        }

        #endregion  
        /*******************************************************************************************************/
        #region PENJUALAN

        private const string VAR_PenjualanAmount = "VAR_PenjualanAmount";
        private const string VAR_COGSAmount = "VAR_COGSAmount";
        private const string VAR_LabaKotorAmount = "VAR_LabaKotorAmount";
        private void insertSQL_PenjualanAmount(ref string sqlCalls, ref string sqlResultTable)
        {
            sqlCalls += string.Format(@"

                    DECLARE @DateFrom as Date,@DateTo as Date,@AccountID as varchar(50)
                    SET @DateFrom = '{3}'
                    SET @DateTo = '{4}'
                    SET @AccountID = '4%'

                    DECLARE @TEMPTABLE TABLE([Quantity] Float,[Total] Float,[COGS] Float,[Profit] Float)
                    INSERT INTO @TEMPTABLE 
                    SELECT L.[Quantity],L.[Total],(L.[Quantity] * ISNULL(KL.[COG],0)),L.[Total]-(L.[Quantity] * ISNULL(KL.[COG],0)) 
                    FROM [DWSystem].[SalesOrder] SI LEFT OUTER JOIN [DWSystem].[SalesOrderDone] O ON SI.[SalesOrderID] = O.[SalesOrderID],
                    [DWSystem].[SalesOrderLog] L,[DWSystem].[Inventory] I,[DWSystem].[InventoryCategory] C,
                    [DWSystem].[InventoryType] T,[DWSystem].[SalesOrderLogDelivered] D,[DWSystem].[BBKLog] KL,
                    [DWSystem].[SalesPerson] P 
                    WHERE SI.[SalesOrderID] = L.[SalesOrderID] AND L.[InventoryID] = I.[InventoryID] 
                    AND I.[CategoryID] = C.[CategoryID] AND I.[TypeID] = T.[TypeID] AND L.[No] = D.[SOLogNo] 
                    AND D.[BBKLogNo] = KL.[No] AND SI.[SalesID] = P.[SalesID] 
                    AND SI.[Date] >= @DateFrom AND SI.[Date] < @DateTo AND SI.[Rejected] = 0 AND L.[Deleted] = 0 AND L.[Quantity] > 0 
                    AND O.[SalesInvoiceID] IS NULL 
           
                    INSERT INTO @TEMPTABLE 
                    SELECT L.[Quantity],L.[Total],(L.[Quantity] * ISNULL(KL.[COG],0)),L.[Total]-(L.[Quantity] * ISNULL(KL.[COG],0)) 
                    FROM [DWSystem].[SalesInvoice] SI,[DWSystem].[SalesInvoiceLog] L,[DWSystem].[Inventory] I,
                    [DWSystem].[InventoryCategory] C,[DWSystem].[InventoryType] T,[DWSystem].[BBKLogSalesSettlement] KS,[DWSystem].[BBKLog] KL,
                    [DWSystem].[SalesPerson] P 
                    WHERE SI.[SalesInvoiceID] = L.[SalesInvoiceID] AND L.[InventoryID] = I.[InventoryID] 
                    AND I.[CategoryID] = C.[CategoryID] AND I.[TypeID] = T.[TypeID] AND L.[No] = KS.[SILogNo] 
                    AND KS.[BBKLogNo] = KL.[No]  AND SI.[SalesID] = P.[SalesID] 
                    AND SI.[InvoiceDate] >= @DateFrom AND SI.[InvoiceDate] < @DateTo 
                    AND SI.[Rejected] = 0 AND L.[Deleted] = 0 AND L.[Quantity] > 0 
            
                    INSERT INTO @TEMPTABLE 
                    SELECT -1 * L.[Quantity],(-1 * L.[Total]),-1 * L.[Total],L.[Total]-L.[Total]
                    FROM [DWSystem].[RTNSalesInvoice] SI,[DWSystem].[RTNSalesInvoiceLog] L,[DWSystem].[Inventory] I,
                    [DWSystem].[InventoryCategory] C,[DWSystem].[InventoryType] T,[DWSystem].[BBMLogRTNSalesSettlement] SS,[DWSystem].[BBMLog] KL,
                    [DWSystem].[SalesPerson] P 
                    WHERE SI.[RTNSalesInvoiceID] = L.[RTNSalesInvoiceID] AND L.[InventoryID] = I.[InventoryID] 
                    AND I.[CategoryID] = C.[CategoryID] AND I.[TypeID] = T.[TypeID] AND L.[No] = SS.[RTNSILogNo] 
                    AND SS.[BBMLogNo] = KL.[No]  AND SI.[SalesID] = P.[SalesID] 
                    AND SI.[InvoiceDate] >= @DateFrom AND SI.[InvoiceDate] < @DateTo 
                    AND SI.[Rejected] = 0 AND L.[Quantity] > 0 

                    --SELECT SUM([Quantity]) as TotalQuantity,SUM([Total]) as Total,SUM([COGS]) as TotalCOGS,SUM([Profit]) as TotalProfit 
                    --FROM @TEMPTABLE 

                    DECLARE @TotalDebit as float
                    DECLARE @TotalCredit as float
                    DECLARE @TotalCOGS as float

                    SELECT @TotalDebit = ISNULL(SUM([Amount]),0)
                    FROM [DWSystem].[COA] O LEFT OUTER JOIN [DWSystem].[COABalanceLog] S ON O.[AccountNo] = S.[AccountNo],
                    [DWSystem].[COABaseType] B,[DWSystem].[COAPosition] P,[DWSystem].[Currency] C 
                    WHERE O.[BaseTypeID] = B.[BaseTypeID] AND O.[POSID] = P.[POSID] AND O.[CurrencyID] = C.[CurrencyID] 
                    AND O.[Branch] = 0 AND O.[AccountNo] LIKE(@AccountID) AND O.[Balance] <> 0 
                    AND O.POSID = 1
                    AND S.[StatusID] <> 2 
                    AND S.[Date] < @DateFrom 
                    AND S.[AccountNo] LIKE(@AccountID)

                    SELECT @TotalCredit = ISNULL(SUM([Amount]),0)
                    FROM [DWSystem].[COA] O LEFT OUTER JOIN [DWSystem].[COABalanceLog] S ON O.[AccountNo] = S.[AccountNo],
                    [DWSystem].[COABaseType] B,[DWSystem].[COAPosition] P,[DWSystem].[Currency] C 
                    WHERE O.[BaseTypeID] = B.[BaseTypeID] AND O.[POSID] = P.[POSID] AND O.[CurrencyID] = C.[CurrencyID] 
                    AND O.[Branch] = 0 AND O.[AccountNo] LIKE(@AccountID) AND O.[Balance] <> 0 
                    AND O.POSID = 2
                    AND S.[StatusID] <> 2 
                    AND S.[Date] < @DateFrom 
                    AND S.[AccountNo] LIKE(@AccountID)


                    SELECT @TotalDebit = ISNULL(SUM([Amount]),0) - @TotalDebit
                    FROM [DWSystem].[COA] O LEFT OUTER JOIN [DWSystem].[COABalanceLog] S ON O.[AccountNo] = S.[AccountNo],
                    [DWSystem].[COABaseType] B,[DWSystem].[COAPosition] P,[DWSystem].[Currency] C 
                    WHERE O.[BaseTypeID] = B.[BaseTypeID] AND O.[POSID] = P.[POSID] AND O.[CurrencyID] = C.[CurrencyID] 
                    AND O.[Branch] = 0 AND O.[AccountNo] LIKE(@AccountID) AND O.[Balance] <> 0 
                    AND O.POSID = 1
                    AND S.[StatusID] <> 2 
                    AND S.[Date] < @DateTo 
                    AND S.[AccountNo] LIKE(@AccountID)

                    SELECT @TotalCredit = ISNULL(SUM([Amount]),0) - @TotalCredit
                    FROM [DWSystem].[COA] O LEFT OUTER JOIN [DWSystem].[COABalanceLog] S ON O.[AccountNo] = S.[AccountNo],
                    [DWSystem].[COABaseType] B,[DWSystem].[COAPosition] P,[DWSystem].[Currency] C 
                    WHERE O.[BaseTypeID] = B.[BaseTypeID] AND O.[POSID] = P.[POSID] AND O.[CurrencyID] = C.[CurrencyID] 
                    AND O.[Branch] = 0 AND O.[AccountNo] LIKE(@AccountID) AND O.[Balance] <> 0 
                    AND O.POSID = 2
                    AND S.[StatusID] <> 2 
                    AND S.[Date] < @DateTo 
                    AND S.[AccountNo] LIKE(@AccountID)

                    SELECT @TotalCOGS = @TotalDebit - @TotalCredit 

                    --SELECT SUM([Quantity]) as TotalQuantity,SUM([Total]) as Total,@TotalCOGS as TotalCOGS,SUM([Total]) - @TotalCOGS as TotalProfit 
                    --FROM @TEMPTABLE

                    DECLARE @{0} decimal(15,2);
                    DECLARE @{1} decimal(15,2);
                    DECLARE @{2} decimal(15,2);

                    SELECT
                        @{0} = SUM([Total]),
                        @{1} = @TotalCOGS,
                        @{2} = SUM([Total]) - @TotalCOGS 
                    FROM @TEMPTABLE 

                    ", VAR_PenjualanAmount, VAR_COGSAmount, VAR_LabaKotorAmount, Tools.getStartDate(txtDateStart.Text, DATEFORMAT), Tools.getEndDate(txtDateEnd.Text, DATEFORMAT));

            updateSQLResultTable(ref sqlResultTable, string.Format("@{0} AS {0}", VAR_PenjualanAmount));
            updateSQLResultTable(ref sqlResultTable, string.Format("@{0} AS {0}", VAR_COGSAmount));
            updateSQLResultTable(ref sqlResultTable, string.Format("@{0} AS {0}", VAR_LabaKotorAmount));
        }

        #endregion
        /*******************************************************************************************************/
        #region SISA PIUTANG

        private const string VAR_SisaPiutangUsahaAmount = "VAR_SisaPiutangUsahaAmount";
        private void insertSQL_SisaPiutangUsahaAmount(ref string sqlCalls, ref string sqlResultTable)
        {
            sqlCalls += string.Format(@"

                    DECLARE @TEMPTABLE2 TABLE([TotalUnpaid] Float)
                    INSERT INTO @TEMPTABLE2 
                    SELECT CAST(((PO.[SubTotal] * CASE WHEN (PO.[GrandTotalConverted] IS NULL) THEN 1 ELSE PO.[Rate] END) - ISNULL(SUM(IP.[DPPPaid]),0)) as decimal(18, 3)) + 
                    ((CASE WHEN (PO.[GrandTotalConverted] IS NOT NULL) THEN (((PO.[SubTotal] + PO.[Tax] + PO.[PPH]) * PO.[Rate]) - PO.[GrandTotalConverted]) ELSE 0 END) + 
                    (ISNULL(PO.[Tax],0) * CASE WHEN (PO.[GrandTotalConverted] IS NULL) THEN 1 ELSE PO.[Rate] END) - ISNULL(SUM(IP.[PPNPaid]),0)) + 
                    ((ISNULL(PO.[PPH],0) * CASE WHEN (PO.[GrandTotalConverted] IS NULL) THEN 1 ELSE PO.[Rate] END) - ISNULL(SUM(IP.[PPHPaid]),0)) as TotalUnpaid 
                    FROM [DWSystem].[SalesInvoice] PO LEFT OUTER JOIN 
                    ([DWSystem].[SellingInvoicePayment] IP INNER JOIN [DWSystem].[ARPayment] P 
                    ON IP.[PaymentID] = P.[PaymentID] AND P.[Canceled] = 0) 
                    ON PO.[SalesInvoiceID] = IP.[SalesInvoiceID], 
                    [DWSystem].[Customer] C,[DWSystem].[SalesPerson] S,
                    [DWSystem].[Currency] Y,[DWSystem].[InvoiceStatus] ST,[DWSystem].[TaxInvoiceType] TI 
                    WHERE PO.[CustomerID] = C.[CustomerID] AND PO.[SalesID] = S.[SalesID] AND 
                    PO.[StatusID] = ST.[StatusID] AND PO.[CurrencyID] = Y.[CurrencyID] AND 
                    PO.[TaxInvoiceTypeID] = TI.[TaxInvoiceTypeID] AND PO.[Approved] = 1 
                    AND PO.[StatusID] <> 1 --InvoiceStatus.PAID 
                    AND PO.[StatusID] <> 4 --InvoiceStatus.PAID_PENDING  
                    AND PO.[StatusID] <> 3 --InvoiceStatus.CANCELED 
                    --AND PO.[CustomerID] = @CustomerID 
                    GROUP BY PO.[SalesInvoiceID],PO.[GrandTotal] ,PO.[SubTotal],PO.[GrandTotalConverted],PO.[Tax],PO.[PPH],PO.[Rate]

                    DECLARE @{0} decimal;

                    SELECT @{0} = ISNULL(SUM([TotalUnpaid]),0)
                    FROM @TEMPTABLE2 

                    ", VAR_SisaPiutangUsahaAmount);

            updateSQLResultTable(ref sqlResultTable, string.Format("@{0} AS {0}", VAR_SisaPiutangUsahaAmount));
        }

        #endregion
        /*******************************************************************************************************/
        #region SISA PIUTANG LEWAT JATUH TEMPO

        private const string VAR_SisaPiutangLewatJTAmount = "VAR_SisaPiutangLewatJTAmount";
        private void insertSQL_SisaPiutangLewatJTAmount(ref string sqlCalls, ref string sqlResultTable)
        {
            sqlCalls += string.Format(@"

                    DECLARE @TEMPTABLE3 TABLE([TotalUnpaid] Float)
                    INSERT INTO @TEMPTABLE3 
                    SELECT CAST(((PO.[SubTotal] * CASE WHEN (PO.[GrandTotalConverted] IS NULL) THEN 1 ELSE PO.[Rate] END) - ISNULL(SUM(IP.[DPPPaid]),0)) as decimal(18, 3)) + 
                    ((CASE WHEN (PO.[GrandTotalConverted] IS NOT NULL) THEN (((PO.[SubTotal] + PO.[Tax] + PO.[PPH]) * PO.[Rate]) - PO.[GrandTotalConverted]) ELSE 0 END) + 
                    (ISNULL(PO.[Tax],0) * CASE WHEN (PO.[GrandTotalConverted] IS NULL) THEN 1 ELSE PO.[Rate] END) - ISNULL(SUM(IP.[PPNPaid]),0)) + 
                    ((ISNULL(PO.[PPH],0) * CASE WHEN (PO.[GrandTotalConverted] IS NULL) THEN 1 ELSE PO.[Rate] END) - ISNULL(SUM(IP.[PPHPaid]),0)) as TotalUnpaid 
                    FROM [DWSystem].[SalesInvoice] PO LEFT OUTER JOIN 
                    ([DWSystem].[SellingInvoicePayment] IP INNER JOIN [DWSystem].[ARPayment] P 
                    ON IP.[PaymentID] = P.[PaymentID] AND P.[Canceled] = 0) 
                    ON PO.[SalesInvoiceID] = IP.[SalesInvoiceID], 
                    [DWSystem].[Customer] C,[DWSystem].[SalesPerson] S,
                    [DWSystem].[Currency] Y,[DWSystem].[InvoiceStatus] ST,[DWSystem].[TaxInvoiceType] TI 
                    WHERE PO.[CustomerID] = C.[CustomerID] AND PO.[SalesID] = S.[SalesID] AND 
                    PO.[StatusID] = ST.[StatusID] AND PO.[CurrencyID] = Y.[CurrencyID] AND 
                    PO.[TaxInvoiceTypeID] = TI.[TaxInvoiceTypeID] AND PO.[Approved] = 1 
                    AND PO.[StatusID] <> 1 --InvoiceStatus.PAID 
                    AND PO.[StatusID] <> 4 --InvoiceStatus.PAID_PENDING  
                    AND PO.[StatusID] <> 3 --InvoiceStatus.CANCELED 
                    --AND PO.[CustomerID] = @CustomerID 
                    AND PO.[DueDate] < GETDATE()
                    GROUP BY PO.[SalesInvoiceID],PO.[GrandTotal] ,PO.[SubTotal],PO.[GrandTotalConverted],PO.[Tax],PO.[PPH],PO.[Rate]

                    DECLARE @{0} decimal;

                    SELECT @{0} = ISNULL(SUM([TotalUnpaid]),0)
                    FROM @TEMPTABLE3

                    ", VAR_SisaPiutangLewatJTAmount);

            updateSQLResultTable(ref sqlResultTable, string.Format("@{0} AS {0}", VAR_SisaPiutangLewatJTAmount));
        }

        #endregion
        /*******************************************************************************************************/
        #region EVENT HANDLERS

        protected void lbtnSalesApproval_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(lblSalesApprovalCount.Text) > 0)
            {
                if (Tools.getCookieData<bool>(GlobalVariables.COOKIEDATA_CANAPPROVESALES))
                    Response.Redirect("~/SalesApprovals");
                else
                    message.error(ERROR_NOPRIVILEGE);
            }
        }

        protected void btnGenerateLaporanKeuangan_Click(object sender, EventArgs e)
        {
            pnlLaporanKeuangan.Visible = false;

            DateTime dtLaporanKeuanganStart, dtLaporanKeuanganEnd;
            if (!DateTime.TryParseExact(txtDateStart.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtLaporanKeuanganStart))
                message.error("Invalid tanggal awal");
            else if (!DateTime.TryParseExact(txtDateEnd.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtLaporanKeuanganEnd))
                message.error("Invalid tanggal akhir");
            else if (dtLaporanKeuanganStart > dtLaporanKeuanganEnd)
                message.error("Tanggal akhir tidak bisa lebih awal dari tanggal awal.");
            else
                pnlLaporanKeuangan.Visible = true;

            if(pnlLaporanKeuangan.Visible)
                populateProfitLossStatement();
        }

        #endregion
        /*******************************************************************************************************/

    }
}