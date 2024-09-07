
<%@ Page Language="C#" Async="true" AsyncTimeout="8800" AutoEventWireup="true" CodeFile="UserManagement.aspx.cs" Inherits="UserManagement" MasterPageFile="~/MasterPageLEARNIX.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:HiddenField ID="hdfIsEdit" runat="server" />
    <table width="1000" border="0" align="center" cellspacing="0" cellpadding="0">
        <tbody>
            <tr>
                
                <td style="width: 100%; background-color:#2163ad">
                    <div class="form-inline" style="color:white">
                     
                         <asp:Button ID="btnAdd" runat="server" Text="Add" OnClientClick="ChangeUser(0); return false;"  Width="100px" Style="margin: 10px; font-weight: bold" />
                          <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClientClick="ChangeUser(1); return false;"  Width="100px" Style="margin: 10px; font-weight: bold" />
                         <asp:Button ID="btnResetPass" runat="server" Text="Reset Password" OnClientClick="ChangeUser(2); return false;"  Width="150px" Style="margin: 10px; font-weight: bold" />
                     
                        </div>
                </td>
            </tr>

            <tr>
                <td>
                    <div style="border: 1px; border-color: black">
                        <table class="tblcontent" border="1">
                            <tbody>                           
                                <tr>
                                    <td style="width:25%">
                                        <div class="clsConceptlist dvleft" >
                                            

                                        </div>
                                    </td>
                                    <td style="vertical-align:text-top">
                                        <div class="divlawUserDetail" >
                                            
                                        </div>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </td>

            </tr>
        </tbody>
    </table>

</asp:Content>
