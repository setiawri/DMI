<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DMIWeb.Orders.Default" %>
<%@ Register src="~/UserControls/Message.ascx" tagname="Message" tagprefix="Custom" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="AJAX" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="page-title"><%= PAGETOPIC %></div>
            <Custom:Message ID="message" runat="server" />
    
            <div class="oneline">
                <div class="section-title">
                    Customer:&nbsp;<asp:DropDownList ID="ddlCustomers" OnSelectedIndexChanged="ddlCustomers_SelectedIndexChanged" AutoPostBack="true" runat="server" />
                </div>
                <div>
                &nbsp<asp:CheckBox ID="chkHasPendingOrders" OnCheckedChanged="chkHasPendingOrders_CheckedChanged" AutoPostBack="true" Text="ada order" runat="server" />
                </div>
            </div>
    
    
    <asp:UpdatePanel ID="upFilter" runat="server">
        <ContentTemplate>

            <table>
                <tr><td>            
                    <div class="filter">
                        <div>            
                            <table border="0">
                                    <tr><td class="label">Keyword:&nbsp;</td>
                                        <td>
                                            <asp:TextBox ID="txtKeyword" CssClass="rounded-textbox input-textbox" runat="server" />
                                            <asp:LinkButton ID="lbtnClearKeyword" OnClick="lbtnClearKeyword_Click" Text="X" CssClass="clear-textbox" runat="server" />
                                        </td>
                                    </tr>
                                    <tr><td class="label"><%= _CategoryName %>:&nbsp;</td>
                                        <td><asp:CheckBox ID="chkCategory" OnCheckedChanged="chkCategory_CheckedChanged" AutoPostBack="true" runat="server" />
                                            <asp:DropDownList ID="ddlCategory" Enabled="false" runat="server" />
                                        </td>
                                    </tr>
                                    <tr><td class="label"><%= _TypeName %>:&nbsp;</td>
                                        <td><asp:CheckBox ID="chkType" OnCheckedChanged="chkType_CheckedChanged" AutoPostBack="true" runat="server" />
                                            <asp:DropDownList ID="ddlType" Enabled="false" runat="server" />
                                        </td>
                                    <tr><td class="label">Show:&nbsp;</td>
                                        <td>
                                            <asp:DropDownList ID="ddlPageSizes" runat="server">
                                                <asp:ListItem Text="5" Value="5" />
                                                <asp:ListItem Text="10" Value="10" />
                                                <asp:ListItem Text="25" Value="25" />
                                                <asp:ListItem Text="50" Value="50" />
                                            </asp:DropDownList>
                                            per page
                                        </td>
                                    </tr>
                                    </tr>
                            </table>
                        </div>
                        <div class="submitbutton">
                           <asp:Button ID="btnFilter" Text="FILTER" OnClick="btnFilter_Click" CssClass="rounded-button" runat="server" />
                        </div>
                    </div>
                    </td>
                </tr>
            </table>

            <table>
                <tr><td>
                    <asp:Panel ID="pnlInventoryPaging1" Visible="false" runat="server">
                        <asp:Button ID="btnFirst1" OnClick="btnFirst_Click" Text="<<" CssClass="rounded-button" style="background-color:whitesmoke;" runat="server" /> 
                        <asp:Button ID="btnPrevious1" OnClick="btnPrevious_Click" Text="<" CssClass="rounded-button" style="background-color:whitesmoke;" runat="server" />
                        Page <asp:Label ID="lblPageIndex1" runat="server" /> of <asp:Label ID="lblPageCount1" runat="server" />
                        <asp:Button ID="btnNext1" OnClick="btnNext_Click" Text=">" CssClass="rounded-button" style="background-color:whitesmoke;" runat="server" /> 
                        <asp:Button ID="btnLast1" OnClick="btnLast_Click" Text=">>" CssClass="rounded-button" style="background-color:whitesmoke;" runat="server" />
                    </asp:Panel>
                    </td>
                </tr>
                <tr><td>
                    <div class="orders-inventory">
                        <asp:Label ID="lblInventoryHasNoData" Text="Tidak ada data. Silahkan rubah filter." CssClass="rounded-div" style="background-color:blanchedalmond;" Visible="false" runat="server" />
                        <asp:Repeater ID="rptInventory" OnItemCommand="rptInventory_ItemCommand" runat="server">                    
                            <HeaderTemplate>
                                <div style="border-bottom:1px solid lightgrey;"></div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div style="padding-top:5px; padding-bottom:5px;border-bottom:1px solid lightgray;">
                                    <a name='<%# Eval("['InventoryID']") %>' />
                                    <asp:Panel ID="pnlInventoryItem" Enabled='<%# Eval("PendingOrderQuantity") == DBNull.Value %>' runat="server">
                                        <div class="info oneline">
                                            <div class="text-bold">Kode:</div><%# Eval("['InventoryID']") %>
                                            &nbsp; &nbsp;<div class="text-bold"><%= _CategoryName %>:</div> <%# Eval("['CategoryName']") %>
                                            &nbsp; &nbsp;<div class="text-bold"><%= _TypeName %>:</div> <%# Eval("['TypeName']") %>
                                            &nbsp; &nbsp;<div class="text-bold">Price:</div> <span id="spanPrice" runat="server"><%# string.Format("{0:N2}",Eval("['Price']")) %></span><%# "/"+Eval("['Unit']") %>
                                        </div>
                                        <div class="info oneline">
                                            <asp:Label Text='<%# "["+Eval("Quantity")+"]" %>' Visible='<%# chkShowQty.Checked %>' runat="server" />
                                            <%# Eval("['InventoryName']") %>
                                        </div>
                                        <div>
                                            Qty:<asp:TextBox ID="txtQty" Text='<%# Eval("PendingOrderQuantity") %>' CssClass="rounded-textbox" Width="45" MaxLength="5" runat="server" />

                                            <%--&nbsp;Expire:<asp:TextBox ID="txtExpireDate" Text='<%# string.Format("{0:dd/MM/yyyy}", Eval("ExpiredDate")) %>' Enabled="false" CssClass="rounded-textbox" Width="85" runat="server" />
                                            <asp:ImageButton ID="ibtnDate" ImageUrl="~/Images/Icons/Calendar_Button.png" CssClass="AJAXCalendarExtender" runat="server" />
                                            <AJAX:CalendarExtender ID="calDate" runat="server" 
                                                    Enabled="True" TargetControlID="txtExpireDate" Format="dd/MM/yyyy"
                                                PopupButtonID="ibtnDate">
                                            </AJAX:CalendarExtender>--%>

                                            &nbsp;% Disc 1:<asp:TextBox ID="txtDiscount1" Text='<%# Eval("Discount1") %>' CssClass="rounded-textbox" MaxLength="6" Width="50" runat="server" />
                                            &nbsp;% Disc 2:<asp:TextBox ID="txtDiscount2" Text='<%# Eval("Discount2") %>' CssClass="rounded-textbox" MaxLength="6" Width="50" runat="server" />
                                            &nbsp;<asp:Button ID="btnAdd" Text="ADD" CommandArgument='<%# Eval("InventoryID") %>' CssClass="rounded-button" style="background-color:whitesmoke;" runat="server" />
                                        </div>  
                                    </asp:Panel>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    </td>
                </tr>
                <tr><td>
                    <asp:Panel ID="pnlInventoryPaging2" Visible="false" runat="server">
                        <asp:Button ID="btnFirst2" OnClick="btnFirst_Click" Text="<<" CssClass="rounded-button" style="background-color:whitesmoke;" runat="server" /> 
                        <asp:Button ID="btnPrevious2" OnClick="btnPrevious_Click" Text="<" CssClass="rounded-button" style="background-color:whitesmoke;" runat="server" />
                        Page <asp:Label ID="lblPageIndex2" runat="server" /> of <asp:Label ID="lblPageCount2" runat="server" />
                        <asp:Button ID="btnNext2" OnClick="btnNext_Click" Text=">" CssClass="rounded-button" style="background-color:whitesmoke;" runat="server" /> 
                        <asp:Button ID="btnLast2" OnClick="btnLast_Click" Text=">>" CssClass="rounded-button" style="background-color:whitesmoke;" runat="server" />
                        <asp:CheckBox ID="chkShowQty" Checked="false" OnCheckedChanged="chkShowQty_OnCheckedChanged" AutoPostBack="true" runat="server" />
                    </asp:Panel>
                    </td>
                </tr>        
            </table>

            <table>        
                <tr><td>&nbsp;</td></tr>

                <tr><td class="section-title" style="padding-bottom:5px;">
                    Daftar Order <asp:Label ID="lblOrderTotalAmount" runat="server" />
                    </td>
                </tr>                            
                <tr><td>
                    <div class="orders">
                        <asp:Label ID="lblOrdersHasNoData" Text="Tidak ada pending orders." CssClass="rounded-div" style="background-color:blanchedalmond;" Visible="false" runat="server" />
                        <asp:Repeater ID="rptOrders" OnItemCommand="rptOrders_ItemCommand" runat="server">                    
                            <HeaderTemplate>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div style="padding-top:5px; padding-bottom:5px;border-bottom:1px solid lightgray;">
                                    <a name='<%# Eval("['InventoryID']") %>' />
                                    <div class="info oneline">
                                        <div class="text-bold"><%# Eval("['RowNumber']") %>.</div>
                                        <div class="text-bold">Kode:</div> <%# Eval("['InventoryID']") %>
                                        &nbsp; &nbsp;<div class="text-bold"><%= _CategoryName %>:</div> <%# Eval("['CategoryName']") %>
                                        &nbsp; &nbsp;<div class="text-bold"><%= _TypeName %>:</div> <%# Eval("['TypeName']") %>
                                        &nbsp; &nbsp;<div class="text-bold">Base Price:</div> <span id="spanPrice" runat="server"><%# string.Format("{0:N2}",Eval("['Price']")) %></span><%# "/"+Eval("['Unit']") %>
                                    </div>
                                    <div class="info oneline">
                                        <asp:Label Text='<%# "["+Eval("Quantity")+"]" %>' Visible='<%# chkShowQty.Checked %>' runat="server" />
                                        <%# Eval("['InventoryName']") %>
                                    </div>
                                    <div class="info oneline">
                                        <div class="text-bold">Order Date:</div> <%# string.Format("{0:dd/MM/yyyy}", Eval("['OrderDate']")) %>
                                        &nbsp; &nbsp;<div class="text-bold">Discounted:</div> <span id="spanAdjustedPrice" runat="server"><%# string.Format("{0:N2}",Eval("['AdjustedPrice']")) %></span><%# "/"+Eval("['Unit']") %>
                                        &nbsp; &nbsp;<div class="text-bold">Total:</div> <span id="spanOrderAmount" runat="server"><%# string.Format("{0:N2}",Eval("['OrderAmount']")) %></span>
                                    </div>
                                    <div>
                                        Pending:<asp:TextBox ID="txtPendingQty" Text='<%# Eval("PendingOrderQuantity") %>' CssClass="rounded-textbox" Width="45" MaxLength="5" runat="server" />

                                        <%--&nbsp;Expire:<asp:TextBox ID="txtExpireDate" Text='<%# string.Format("{0:dd/MM/yyyy}", Eval("ExpiredDate")) %>' Enabled="false" CssClass="rounded-textbox" Width="85" runat="server" />
                                        <asp:ImageButton ID="ibtnDate" ImageUrl="~/Images/Icons/Calendar_Button.png" CssClass="AJAXCalendarExtender" runat="server" />
                                        <AJAX:CalendarExtender ID="calDate" runat="server" 
                                                Enabled="True" TargetControlID="txtExpireDate" Format="dd/MM/yyyy"
                                            PopupButtonID="ibtnDate">
                                        </AJAX:CalendarExtender>--%>

                                        &nbsp;% Disc 1:<asp:TextBox ID="txtDiscount1" Text='<%# Eval("Discount1") %>' CssClass="rounded-textbox" MaxLength="6" Width="50" runat="server" />
                                        &nbsp;% Disc 2:<asp:TextBox ID="txtDiscount2" Text='<%# Eval("Discount2") %>' CssClass="rounded-textbox" MaxLength="6" Width="50" runat="server" />
                                        &nbsp;<asp:Button ID="btnUpdate" Text="UPDATE" CommandArgument='<%# Eval("CustomerOrderID") %>' CssClass="rounded-button" style="background-color:whitesmoke;" runat="server" />
                                        <asp:Button ID="btnStop" Text="STOP" CommandArgument='<%# Eval("CustomerOrderID") %>' CssClass="rounded-button" style="background-color:whitesmoke;" runat="server" />
                                    </div>                            
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    </td>
                </tr>
            </table>
            
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
