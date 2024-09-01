
<%@ Page Language="C#" Async="true" AsyncTimeout="8800" AutoEventWireup="true" CodeFile="Transactions.aspx.cs" Inherits="Transactions" MasterPageFile="~/MasterPageLEARNIX.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:HiddenField ID="hdftext" runat="server" />
    <div style="display:none">
        <asp:Button ID="btnResearch" runat="server" Text="Button" OnClick="btnResearch_Click" />
    </div>
    <table width="100%" border="0" align="center" cellspacing="0" cellpadding="0">
        <tbody>
            <tr>
                
                <td style="width: 100%; background-color:#2163ad">
                    <div class="form-inline" style="color:white">
                     
                        <asp:DropDownList ID="cboTime" runat="server">
                         <asp:ListItem Text="Last 1 hour" Value="1"> </asp:ListItem>
                               <asp:ListItem Text="Last 12 hours" Value="2"> </asp:ListItem>
                               <asp:ListItem Text="Last 1 day" Value="3"> </asp:ListItem>
                              <asp:ListItem Text="Last 7 day" Value="4"> </asp:ListItem>
                              <asp:ListItem Text="Last 21 day" Value="5"> </asp:ListItem>
                              <asp:ListItem Text="Last 1 month" Value="6"> </asp:ListItem>
                             <asp:ListItem Text="Last 3 months" Value="7"> </asp:ListItem>
                             <asp:ListItem Text="Last 1 year" Value="8"> </asp:ListItem>
                              <asp:ListItem Text="All time" Value="9" Selected="True"> </asp:ListItem>
                        </asp:DropDownList>
                        <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />
                        </div>
                </td>
            </tr>

            <tr>
                <td style="width: 100%; height:550px; vertical-align:top">
                    <div style="border: 1px; border-color: black">
                        <asp:GridView CssClass="clsTblTransactions" ID="GridView1" runat="server" AutoGenerateColumns="true" Width="100%" ></asp:GridView>
                    </div>
                </td>

            </tr>
        </tbody>
    </table>

</asp:Content>
