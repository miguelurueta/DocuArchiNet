<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormRaImprimirFul.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormRaImprimirFul" %>
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
       
        <div id="ContenidoImpresion" style="border: thin double #000080; color: black; background-color: #FFFFFF; height: 220px; width: 450px">
           
           
            <asp:Table ID="Table1" runat="server" Style="width: 450px; height: 150px">
                <asp:TableRow ID="impresion">
                    <asp:TableCell>
                                      <label style="color: #0000FF; font-family: Arial; display:none"> Datos de Radicación</label>
                    </asp:TableCell>
                    <asp:TableCell>
                                      <label style="color: #0000FF; font-family: Arial"> Impresión File </label>
                    </asp:TableCell>

                </asp:TableRow>
                <asp:TableRow ID="user">
                    <asp:TableCell>
                                      <label id="usuario" style="display:none"> Usuario Radicador </label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <label id="usuario_datos" style="display: none" runat="server"></label>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="radicado">
                    <asp:TableCell>
                                      <label id="radicad" style="color: #FF0000; font-family: Arial; font-size: large; display:none"> Consecutivo Radicado </label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <label id="radicad_datos" runat="server" style="color: #FF0000; font-family: Arial; font-size: large"></label>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="barra">
                    <asp:TableCell>
                                      <label id="barr" style="display:none">Codigo Barras</label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <label id="barr_datos" runat="server"></label>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="fecha">
                    <asp:TableCell>
                                      <label id="fech" style=" display:none"> Fecha Radicado</label>
                    </asp:TableCell>
                    <asp:TableCell>
                        <label id="fech_datos" runat="server"></label>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow ID="seleccion">
                    <asp:TableCell>
                                      <label id="Sel"> Modo impresión</label>
                    </asp:TableCell>
                    <asp:TableCell>
                                      <select id="pid" name="pid">
                                         <option selected="selected" value="1">Dialogo Impresion</option>
                                         <option value="0">Impresora Predeterminada</option>

                                     </select>
                    </asp:TableCell>

                </asp:TableRow>


                <asp:TableRow>
                    <asp:TableCell>
                                   
                                   
                                     
                    </asp:TableCell>
                    <asp:TableCell>
                                         <input type="button" style="font-size:16px; color: #FFFFFF; background-color: #053061;" onclick="javascript: doClientPrinttext(); " value="Imprimir" />
                                          
                    </asp:TableCell>


                </asp:TableRow>
            </asp:Table>
            <div id="netPrinter" style="display: none">
                <label for="netPrinterHost">Printer's DNS Name or IP Address:</label>
                <input type="text" name="netPrinterHost" id="netPrinterHost" />
                <label for="netPrinterPort">Printer's Port:</label>
                <input type="text" name="netPrinterPort" id="netPrinterPort" />
            </div>

            <div id="parallelPrinter" style="display: none">
                <label for="parallelPort">Parallel Port:</label>
                <input type="text" name="parallelPort" id="parallelPort" value="LPT1" />

                <textarea id="printerCommands" name="printerCommands" rows="10" cols="80" class="span9"> ojo impreion </textarea>
            </div>





        </div>       
                         
         </div>
         
    
    </form>
    
    
<%-- Register the WebClientPrint script code --%>

    <%=Neodynamic.SDK.Web.WebClientPrint.CreateScript(MyUtils.GetWebsiteRoot() & "radicador/WebClientPrintAPI.ashx", MyUtils.GetWebsiteRoot() & "radicador/PrinterFile.ashx", HttpContext.Current.Session.SessionID)%>
     <script type="text/javascript" src="<%=MyUtils.GetWebsiteRoot() + "/scripts/DemoPrintCommands.js"%>"></script>
    
</body>
</html>

