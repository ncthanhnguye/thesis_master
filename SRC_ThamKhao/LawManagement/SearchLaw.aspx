<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageLEARNIX.master" AutoEventWireup="true" CodeFile="SearchLaw.aspx.cs" Inherits="SearchLaw" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="divcontainer">
        <div class="form-inline">
            <asp:TextBox runat="server" Width="766px" placeholder="<Tìm Văn Bản Pháp Luật>" ID="txtSearch"></asp:TextBox>

            <button onclick="SearchLaw(); return false;">Tìm Kiếm</button>
        </div>
    </div>
    <table width="1000" border="0" align="center" cellspacing="0" cellpadding="0">
        <tbody>
            <tr>
                <td style="width: 100%">
                    <div class="LHJvCe">
                        <div id="result-stats"  > <span class="sresult"></span><nobr> &nbsp;</nobr></div>
                    </div>

                </td>
            </tr>

            <tr>
                <td>
                    <div >
                        <table class="tblcontent">
                            <tbody>                            
                                <tr>
                                    <td>
                                        <div style="height: auto; width: 900px; overflow: hidden;" class="divContentHTMLLaw">
                                          <%--  <div class="g " lang="vi" style="width: 100%"  >
                                                <div>
                                                    <div  >
                                                        <div ><a >
                                                          
                                                            <h3 class="LC20lb MBeuO DKV0Md">sdfdsGiá, biểu đồ, vốn hóa thị trường của dForce (DF)</h3>
                                                        </a>                                                          
                                                        </div>
                                                    </div>
                                                    <div  >
                                                        <div ><span>Thứ hạng hiện tại trên CoinMarketCap là #594,g tối&nbsp;...</span></div>
                                                    </div>
                                                 
                                                </div>
                                            </div>--%>

                                        

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


