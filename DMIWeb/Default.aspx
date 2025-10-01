<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DMIWeb.Default" %>
<%@ Register src="~/UserControls/Message.ascx" tagname="Message" tagprefix="Custom" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="AJAX" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <Custom:Message ID="message" runat="server" />
    
<%--    <form id="form1" defaultbutton="" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                --%>

    <div class="page-title">
        DASHBOARD
    </div>

    <asp:Panel ID="pnlContent" runat="server">
        <div class="main-links-container">
            <asp:Panel ID="pnlSalesApprovalLink" Visible="false" runat="server">
                <div class="section-title">Pending Items</div>
                <div class="main-links"><asp:LinkButton ID="lbtnSalesApproval" Text="Sales Approval" OnClick="lbtnSalesApproval_Click" runat="server" /> (<asp:Label ID="lblSalesApprovalCount" runat="server" />)</div>
            </asp:Panel>
        </div>

        <asp:Panel ID="pnlProfitLossStatement" Visible="false" runat="server">
            <div class="laporan-keuangan">
                <div class="section-title">Laporan Keuangan</div>
                <div>
                    <table border="0">
                        <tr><td>Periode:&nbsp;</td>
                            <td><asp:TextBox ID="txtDateStart" width="70" style="font:8pt Verdana;" ReadOnly="true" runat="server" /></td>
                            <td><asp:ImageButton ID="IbtnDateStart" ImageUrl="~/Images/Icons/Calendar_Button.png" CssClass="AJAXCalendarExtender" runat="server" />
                                <AJAX:CalendarExtender ID="ceDateStart" runat="server" 
                                        Enabled="True" TargetControlID="txtDateStart" Format="dd/MM/yyyy"
                                    PopupButtonID="IbtnDateStart">
                                </AJAX:CalendarExtender>
                            </td>
                            <td>&nbsp;-&nbsp;</td>
                            <td><asp:TextBox ID="txtDateEnd" width="70" style="font:8pt Verdana;" ReadOnly="true" runat="server" /></td>
                            <td><asp:ImageButton ID="IbtnDateEnd" ImageUrl="~/Images/Icons/Calendar_Button.png" CssClass="AJAXCalendarExtender" runat="server" />
                                <AJAX:CalendarExtender ID="ceDateEnd" runat="server" 
                                        Enabled="True" TargetControlID="txtDateEnd" Format="dd/MM/yyyy"
                                    PopupButtonID="IbtnDateEnd">
                                </AJAX:CalendarExtender>
                            </td>
                            <td>&nbsp;<asp:Button ID="btnGenerateLaporanKeuangan" Text="SUBMIT" OnClick="btnGenerateLaporanKeuangan_Click" runat="server" /></td>
                        </tr>
                    </table>
                </div>
            </div>
        
            <%-- <asp:UpdatePanel ID="upLaporanPenjualan" runat="server">
                <ContentTemplate>--%>
                    <asp:Panel ID="pnlLaporanKeuangan" Visible="false" runat="server">
                        <div class="laporan-keuangan">
                                <div style="padding-top:10px;"></div>
                                <table runat="server">
                                    <tr><td class="label">Penjualan</td><td class="value"><asp:Label ID="lblPenjualanAmount" runat="server" /></td></tr>
                                    <tr><td class="label">COGS</td><td class="value"><asp:Label ID="lblCOGSAmount" runat="server" /></td></tr>
                                    <tr><td class="label">Laba Kotor</td><td class="value" style="border-top: 1px solid #D3D3D3;"><asp:Label ID="lblLabaKotorAmount" runat="server" /></td></tr>
                                    <tr><td>&nbsp</td></tr>
                                    <tr><td class="label">Piutang Usaha</td><td class="value"><asp:Label ID="lblPiutangUsahaAmount" runat="server" /></td></tr>
                                    <tr><td class="label">Piutang lewat JT</td><td class="value"><asp:Label ID="lblPiutangLewatJTAmount" runat="server" /></td></tr>
                                    <tr><td>&nbsp</td></tr>
                                </table>
                        </div>

                        <div class="persediaan">  
                            <div class="section-title">Persediaan: <asp:Label ID="lblPersediaanAmount" runat="server" /></div>  
                            <div>
                                <asp:Repeater ID="rptPersediaan"  runat="server">
                                    <HeaderTemplate>
                                        <div class="child-header oneline">
                                            <div class="persediaan-brand">Brand</div>
                                            <div class="persediaan-totalstok">Stok</div>
                                            <div class="persediaan-totalvalue">Value</div>
                                        </div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <div class="child oneline">
                                            <div class="persediaan-brand"><%# Eval("['Brand']") %> </div>
                                            <div class="persediaan-totalstok"><%# string.Format("{0:N0}",Eval("['TotalStok']")) %> </div>
                                            <div class="persediaan-totalvalue"><%# string.Format("{0:N2}",Eval("['TotalValue']")) %> </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
    
                        <div class="hutang-jatuh-tempo">  
                            <div class="section-title">Hutang Jatuh Tempo: <asp:Label ID="lblHutangJatuhTempoAmount" runat="server" /></div>  
                            <div>
                                <asp:Repeater ID="rptHutangJatuhTempo"  runat="server">
                                    <HeaderTemplate>
                                        <div class="child-header oneline">
                                            <div class="hutang-jatuh-tempo-supplier">Supplier</div>
                                            <div class="hutang-jatuh-tempo-totalunpaiddue">Due</div>
                                        </div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <div class="child oneline">
                                            <div class="hutang-jatuh-tempo-supplier"><%# Eval("['Supplier']") %> </div>
                                            <div class="hutang-jatuh-tempo-totalunpaiddue"><%# string.Format("{0:N2}",Eval("['TotalUnpaidDue']")) %> </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
    
                        <div class="hutang-jatuh-tempo-dalam-7-hari">    
                            <div class="section-title">Hutang JT 7 hari ke depan: <asp:Label ID="lblHutangJatuhTempoDalam7Hari" runat="server" /></div>      
                            <div>
                                <asp:Repeater ID="rptHutangJatuhTempoDalam7Hari"  runat="server">
                                    <HeaderTemplate>
                                        <div class="child-header oneline">
                                            <div class="hutang-jatuh-tempo-dalam-7-hari-supplier">Supplier</div>
                                            <div class="hutang-jatuh-tempo-dalam-7-hari-totalunpaidduein7day">Due</div>
                                        </div>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <div class="child oneline">
                                            <div class="hutang-jatuh-tempo-dalam-7-hari-supplier"><%# Eval("['Supplier']") %> </div>
                                            <div class="hutang-jatuh-tempo-dalam-7-hari-totalunpaidduein7day"><%# string.Format("{0:N2}",Eval("['TotalUnpaidDueIn7Day']")) %> </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </asp:Panel>
                <%--</ContentTemplate>
            </asp:UpdatePanel>--%>
        </asp:Panel>
    </asp:Panel>

<%--    </form>--%>
</asp:Content>
