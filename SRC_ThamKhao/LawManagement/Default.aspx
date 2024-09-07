<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" MasterPageFile="~/MasterPageLEARN.master" %>

<asp:Content ID="content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="Jscripts/JSLogIn.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <script>
        $(document).ready(function () {
            $('[id$=txtUserName]').focus();         
            $('.HeaderIconRightLearn').hide();
        });
    </script>
    <style>
        .main
        {
            height: 500px;
        }
    </style>
    <center>
        <div class="HeadTitle">Law Search</div>
        <div id="loginbox" class="BackgroundLoginBox">
            <table cellpadding="10" cellspacing="15">
                <tr>
                    <td>
                        <asp:Label ID="lblUserName" runat="server" Text="Username:"></asp:Label>
                    </td>
                    <td align="left">
                        <asp:TextBox ID="txtUserName" runat="server" CssClass="txtLogin" TabIndex="2"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblPassword" runat="server" Text="Password:"></asp:Label>
                    </td>
                    <td align="left">
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="txtLogin" TextMode="Password"
                            TabIndex="3"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td align="left">
                        <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="button100" TabIndex="4"
                            OnClientClick="return submitForm();" OnClick="btnLogin_Click" />
                    </td>
                </tr>
            </table>
        </div>
    </center>
    <asp:HiddenField ID="hdfTimeZone" runat="server" />
    <asp:HiddenField ID="hdfDayLightSaving" runat="server" />
</asp:Content>

