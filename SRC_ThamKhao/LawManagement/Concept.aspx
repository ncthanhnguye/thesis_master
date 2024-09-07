
<%@ Page Language="C#" Async="true" AsyncTimeout="8800" AutoEventWireup="true" CodeFile="Concept.aspx.cs" Inherits="Concept" MasterPageFile="~/MasterPageLEARNIX.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:HiddenField ID="hdfIsEdit" runat="server" />
    <table width="1000" border="0" align="center" cellspacing="0" cellpadding="0">
        <tbody>
            <tr>
                
                <td style="width: 100%; background-color:#2163ad">
                    <div class="form-inline" style="color:white">
                     
                         <asp:Button ID="btnAdd" runat="server" Text="Add" OnClientClick="ChangeConcept(1); return false;"  Width="100px" Style="margin: 10px; font-weight: bold" />
                         <asp:Button ID="btnEdit" runat="server" Text="Edit" OnClientClick="ChangeConcept(2); return false;"  Width="100px" Style="margin: 10px; font-weight: bold" />
                        <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClientClick="ChangeConcept(3); return false;"  Width="100px" Style="margin: 10px; font-weight: bold" />
                          <asp:Button ID="btnKeyPhrase" runat="server" Text="Key Phrases" OnClientClick="ViewKeyPhrases_Concept();return false;" Style="margin: 10px; font-weight: bold" />
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
                                        <div class="clsConceptlist" style="width: 400px; height: 900px; text-align: left; padding-right: 20px;overflow:auto">
                                            

                                        </div>
                                    </td>
                                    <td style="vertical-align:text-top">
                                        <div class="divlawConceptDetail" >
                                            
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
