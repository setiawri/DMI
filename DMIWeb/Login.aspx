<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="DMIWeb.Test" %>
<%@ Register src="UserControls/Message.ascx" tagname="Message" tagprefix="Custom" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <title></title>

    <!-- #Include virtual="~/Scripts.aspx" -->
</head>

<body class="bg-slate-800">
    <!-- Page content -->
    <div class="page-content">
        <!-- Main content -->
        <div class="content-wrapper">
            <!-- Content area -->
            <div class="content d-flex justify-content-center align-items-center">
                <!-- Login card -->
                    <form id="form1" defaultbutton="btnSubmit" runat="server">
                    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        
                        <div class="card mb-0">
                            <div class="card-body">
                                <div>
                                    <span class="form-text text-center display-4 text-muted mb-2"><%: DMIWeb.Tools.getAppSettingsValue("CompanyName").ToUpper() %></span>
                                </div>
                                
                                <div class="d-flex flex-column justify-content-center align-items-center mb-5">         
                                    <div class="flex-grow-1 align-items-center">
                                        
                                        <div class="mb-2 text-danger form-group form-group-feedback form-group-feedback-left mx-4" style="min-height:20px;"> 
                                            <Custom:Message ID="message" runat="server" />
                                        </div>

                                        <div class="form-group form-group-feedback form-group-feedback-left mx-4">
                                            <asp:TextBox runat="server" ID="txtUsername" CssClass="form-control" placeholder="username" />
                                            <%--<input type="text" id="Username" name="Username" value="" class="form-control" placeholder="Username">--%>
                                            <div class="form-control-feedback">
                                                <i class="icon-user text-muted"></i>
                                            </div>
                                        </div>

                                        <div class="form-group form-group-feedback form-group-feedback-left mx-4">
                                            <asp:TextBox runat="server" ID="txtPassword" CssClass="form-control" TextMode="Password" placeholder="password" />
                                            <div class="form-control-feedback">
                                                <i class="icon-lock2 text-muted"></i>
                                            </div>
                                        </div>
                                       <%-- <div class="form-group mx-4">
                                            <asp:Button runat="server" OnClick="btnLogin_OnClick" Text="LOGIN" CssClass="btn btn-primary btn-block" />
                                            <button type="submit" onclick="showLoadingSpinner()" class="btn btn-primary btn-block"><i class="icon-circle-right2 mr-1"></i>LOGIN</button>
                                        </div>--%>
                                        
                                        <div class="form-group mx-4">
                                            <asp:LinkButton runat="server" ID="btnSubmit" OnClick="btnLogin_OnClick" OnClientClick="showLoadingSpinner()" CssClass="btn btn-primary btn-block">
                                                <i class="icon-circle-right2 mr-1"></i>LOGIN
                                            </asp:LinkButton>
                                        </div>

                                        <span class="form-text text-center text-muted">version <%: DMIWeb.Settings.version %></span>

                                    </div>
                                </div>

                            </div>
                        </div>
                        
                    </form>
                <!-- /login card -->
            </div>
            <!-- /content area -->
        </div>
        <!-- /main content -->
    </div>
    <!-- /page content -->
</body>
</html>

<script type="text/javascript">
    $(document).ready(function () {
        if ($('#txtUsername').val() == '')
            $('#txtUsername').select();
        else
            $('#Password').select();

    });
</script>

<!-- #Include virtual="~/Javascripts.aspx" -->