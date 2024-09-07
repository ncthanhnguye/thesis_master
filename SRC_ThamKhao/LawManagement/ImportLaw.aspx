<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ImportLaw.aspx.cs" Inherits="ImportLaw" MasterPageFile="~/MasterPageLEARNIX.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="clsImport">
       
        <table>
            <tbody>
                <tr>
                    <td>Tên</td>  <td><asp:TextBox ID="txtName" runat="server" Width="1000px"></asp:TextBox></td></tr>
                 <tr>
                    <td>  Nội Dung</td>  <td> <asp:TextBox ID="txtContent" runat="server" TextMode="MultiLine" Height="570px" Width="1000px"></asp:TextBox></td></tr>

            </tbody>
        </table>
     
        <asp:Button ID="btnImport" runat="server" Text="Import" OnClick="btnImport_Click" CssClass="button150" />
            
</div>
</asp:Content>

