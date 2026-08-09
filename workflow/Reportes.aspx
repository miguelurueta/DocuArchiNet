<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Reportes.aspx.vb" Inherits="Reportes" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Página sin título</title>
<script language="javascript" type="text/javascript">
// <!CDATA[

function TABLE1_onclick() {

}

// ]]>
</script>
</head>
<body>
    <form id="form1" runat="server">
       
      


                    <div style="width: 100%; margin: 0px auto; height: 100%">
                        <div id="derecha" style="width:29%; height:100%; float:left;">
                            <div id="reportes" style="width: 100%; height: 30%; position: relative">
                                <asp:Panel ID="Panel2" runat="server" BackColor="WhiteSmoke" ScrollBars="Auto"
                                    Style="width: 100%; height: 100%">

                                    <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                        Font-Size="X-Large" ForeColor="Navy" Style="left: 6px; top: 5px"
                                        Text="Reportes Workflow" Width="100%"></asp:Label>
                                    <asp:TreeView ID="TreeView1" runat="server" ImageSet="Faq" Style="left: 13px; top: 38px; float: left" Width="100%" Height="100%" ShowLines="True">
                                        <ParentNodeStyle Font-Bold="False" />
                                        <HoverNodeStyle Font-Underline="True" ForeColor="Purple" />
                                        <SelectedNodeStyle Font-Underline="True" HorizontalPadding="0px" VerticalPadding="0px" />
                                        <NodeStyle Font-Names="Tahoma" Font-Size="8pt" ForeColor="DarkBlue" HorizontalPadding="5px"
                                            NodeSpacing="0px" VerticalPadding="0px" />
                                    </asp:TreeView>
                                </asp:Panel>
                            </div>
                            <div id="parametros" style="width: 100%; position: relative; height: 30%; margin: 1px 1px 1px 1px;">
                                <asp:Panel ID="Panel3" runat="server" ScrollBars="Auto"
                                    Style="width: 100%; float: left; height: 100%;" BackColor="WhiteSmoke">
                                    <asp:Table ID="Tableparametro" runat="server" Style="left: 2px; top: 5px">
                                        <asp:TableRow runat="server">
                                        </asp:TableRow>
                                    </asp:Table>
                                </asp:Panel>
                            </div>
                        </div>
              <div id="resultados" style="width:69%; height:100%; float:right;">
            <asp:Panel ID="Panel1" runat="server" ScrollBars="Both"
                            Style="width:100%; height:550px" BackColor="WhiteSmoke">
                            <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Italic="False" Font-Overline="False"
                                Font-Size="Large" ForeColor="Navy" Style="left: 13px;"
                                Text="Resultados Workflow" Width="100%"></asp:Label>
                            <asp:Table ID="Tableconsulta" runat="server" Style="left: 5px; top: 33px"
                                Width="70%">
                            </asp:Table>


                        </asp:Panel>
                  </div>
                    </div>
            
        
    </form>
</body>
</html>
