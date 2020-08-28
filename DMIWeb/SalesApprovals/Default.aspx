<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DMIWeb.SalesApprovals.Default" %>
<%@ Register src="~/UserControls/Message.ascx" tagname="Message" tagprefix="Custom" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="page-title"><%= PAGETOPIC %></div>

    <Custom:Message ID="message" runat="server" />
    
    <div class="sales-approval">
        <div class="container-daftar">
            <div class="daftar">
                <table border="0">
                    <asp:Repeater ID="rptParent" OnItemCommand="rptParent_ItemCommand" runat="server">
                        <ItemTemplate>
                            <tr><td>
                                <div class="item">
                                    <div class="info oneline">
                                        <div class="col">
                                            <a name='<%# Eval("ID") %>' />
                                            <div class="title"><%# Eval("Customer_Name") %></div>
                                            <asp:Label ID="ID" Text='<%# Eval("ID") %>' Visible="false" runat="server" />
                                        </div>
                                        <div class="col">
                                            <div>Date: <%# string.Format("{0:dd/MM/yy}",Eval("RequestDate")) %></div>
                                        </div>
                                        <div class="col">
                                            <div>Operator: <%# Eval("UserName") %></div>
                                        </div>
                                    </div>

                                    <div class="description">
                                        <%# Eval("Description").ToString().Replace(Environment.NewLine, "<BR>") %>
                                    </div>

                                    <div class="children-container">
                                        <asp:Panel ID="pnlSisaPiutangAndCheques" Visible="false" runat="server">
                                            <div class="title">
                                                SISA PIUTANG
                                            </div>
                                            <div>
                                                <asp:Repeater ID="rptSisaPiutang"  runat="server">
                                                    <HeaderTemplate>
                                                        <div class="child-header oneline">
                                                            <div class="sisapiutang-id">ID</div>
                                                            <div class="sisapiutang-date">Date</div>
                                                            <div class="sisapiutang-due">Due</div>
                                                            <div class="sisapiutang-age">Age</div>
                                                            <div class="sisapiutang-status">Status</div>
                                                            <div class="sisapiutang-total">Total</div>
                                                            <div class="sisapiutang-paid">Paid</div>
                                                            <div class="sisapiutang-remaining">Remaining</div>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <div class="child oneline">
                                                            <div class="sisapiutang-id"><%# Eval("['SalesInvoiceID']") %> </div>
                                                            <div class="sisapiutang-date"><%# string.Format("{0:dd/MM/yy}",Eval("['InvoiceDate']")) %> </div>
                                                            <div class="sisapiutang-due"><%# string.Format("{0:dd/MM/yy}",Eval("['DueDate']")) %> </div>
                                                            <div class="sisapiutang-age"><%# Eval("['Age']") %> </div>
                                                            <div class="sisapiutang-status"><%# Eval("['Status']") %> </div>
                                                            <div class="sisapiutang-total"><%# string.Format("{0:N2}", Eval("['GrandTotal']")) %> </div>
                                                            <div class="sisapiutang-paid"><%# string.Format("{0:N2}", Eval("['PaidAmount']")) %> </div>
                                                            <div class="sisapiutang-remaining"><%# string.Format("{0:N2}", Eval("['RemainingAmount']")) %> </div>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </div>
                                            
                                            <div class="oneline">
                                                <div class="title">
                                                    PENDING CHEQUES
                                                </div>
                                            </div>
                                            <div>
                                                <asp:Repeater ID="rptSisaCheques" runat="server">
                                                    <HeaderTemplate>
                                                        <div class="child-header oneline">
                                                            <div class="sisacheque-id">ID</div>
                                                            <div class="sisacheque-no">No</div>
                                                            <div class="sisacheque-in">In</div>
                                                            <div class="sisacheque-due">Due</div>
                                                            <div class="sisacheque-bank">Bank</div>
                                                            <div class="sisacheque-issueby">Issue By</div>
                                                            <div class="sisacheque-status">Status</div>
                                                            <div class="sisacheque-amount">Amount</div>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <div class="child oneline">
                                                            <div class="sisacheque-id"><%# Eval("['ChequeID']") %> </div>
                                                            <div class="sisacheque-no"><%# Eval("['ChequeNo']") %> </div>
                                                            <div class="sisacheque-in"><%# string.Format("{0:dd/MM/yy}",Eval("['InDate']")) %> </div>
                                                            <div class="sisacheque-due"><%# string.Format("{0:dd/MM/yy}",Eval("['DueDate']")) %> </div>
                                                            <div class="sisacheque-bank"><%# Eval("['Bank']") %> </div>
                                                            <div class="sisacheque-issueby"><%# Eval("['IssueBy']") %> </div>
                                                            <div class="sisacheque-status"><%# Eval("['Status']") %> </div>
                                                            <div class="sisacheque-amount"><%# string.Format("{0:N2}", Eval("['Amount']")) %> </div>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </div>
                                        </asp:Panel>
                                    </div>

                                    <div class="button-container">
                                        <asp:Button id="btnApprove" Text="Approve" CssClass="rounded-button" OnCommand="btnApprove_Command" CommandArgument='<%# Eval("ID") %>' runat="server" />
                                        <asp:Button id="btnReject" Text="Reject" CssClass="rounded-button" OnCommand="btnReject_Command" CommandArgument='<%# Eval("ID") %>' runat="server" />
                                        <asp:Button id="btnShowPiutangAndCheques" Text="Pending Piutang & Cheques" CssClass="rounded-button" CommandArgument='<%# Eval("CustomerID") %>' runat="server" />
                                        <asp:Button ID="btnClosePiutangAndCheques" Text="Hide Pending Piutang & Cheques" CssClass="rounded-button" Visible="false" runat="server" />
                                    </div>

                                </div>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>
        </div>
    </div>
</asp:Content>
