<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormImprimirfilesfil.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormImprimirfiles_fil" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
   <script src="../js/ScrollableGrid.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
</head>
<body>
   
    <form id="form1" runat="server">
          <input type="hidden" id="sid" name="sid" value="<%=Session.SessionID%>" />
          <input type="hidden" id="sid2" name="sid2" runat="server" value="1111111111" />
    <div>
     <div id="ContenidoImpresion" style="border: thin double #000080; color:black; background-color: #FFFFFF; height:220px; width:450px">
                         <asp:Table ID="Table1" runat="server" Style="width:450px; height:200px ">
                             <asp:TableRow ID="impresion">
                                 <asp:TableCell>
                                      
                                 </asp:TableCell>
                                  <asp:TableCell>
                                      <label style="color: #0000FF; font-family: Arial"> Impresion Post </label>
                                 </asp:TableCell>

                             </asp:TableRow>
                            
                             <asp:TableRow ID="seleccion">
                                 <asp:TableCell>
                                      <label id="Sel" style="font-family:Arial;font-size:14px"> Modo impresion</label>
                                 </asp:TableCell>
                                 <asp:TableCell>
                                      <select id="pid" name="pid">
                                         <option selected="selected" value="0">Impresora Predeterminada</option>
                                         <option value="1">Dialogo Impresion</option>

                                     </select>
                                 </asp:TableCell>
                                 
                             </asp:TableRow>

                          
                             <asp:TableRow>
                                <asp:TableCell>
                                   
                                   
                                     
                                </asp:TableCell>
                                    <asp:TableCell>
                                         <input type="button" style="font-size:16px; color: #FFFFFF; background-color: #053061;" onclick="javascript: doClientPrinttext_post();" value="Imprimir" />
                                          
                                    </asp:TableCell>
                                    
                                 
                             </asp:TableRow>
                         </asp:Table>
                 <div id="netPrinter" style="display:none">
                <label for="netPrinterHost">Printer's DNS Name or IP Address:</label>
                <input type="text" name="netPrinterHost" id="netPrinterHost" />
                <label for="netPrinterPort">Printer's Port:</label>
                <input type="text" name="netPrinterPort" id="netPrinterPort" />
            </div>
           
            <div id="parallelPrinter" style="display:none">
                <label for="parallelPort">Parallel Port:</label>
                <input type="text" name="parallelPort" id="parallelPort" value="LPT1" />

                <textarea id="printerCommands" name="printerCommands" rows="10" cols="80" class="span9"> ojo impreion </textarea>
            </div>

            <asp:Label ID="Labelestado" runat="server" Text="Estado" style="font-size:9px"></asp:Label>
            
                       
             
            </div>       
                         
         </div>
         
    
    </form>
    

<%-- Register the WebClientPrint script code --%>
<%=Neodynamic.SDK.Web.WebClientPrint.CreateScript(MyUtils.GetWebsiteRoot() + "Gestion/PrinterFilefil.ashx")%>
    
   
     <script type="text/javascript" src="<%=MyUtils.GetWebsiteRoot() + "/scripts/DemoPrintCommands.js"%>"></script>

</body>
</html>