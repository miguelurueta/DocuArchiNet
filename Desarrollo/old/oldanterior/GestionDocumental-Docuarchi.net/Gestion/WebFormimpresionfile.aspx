<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormimpresionfile.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormimpresionfile" %>

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
     <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
  <link href="../Awesome/css/brands.css" rel="stylesheet">
  <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
  <script defer src="../Awesome/js/solid.js"></script>
  <script defer src="../Awesome/js/fontawesome.js"></script>
    <link href="../Styles/menu_lib.css" rel="stylesheet" />
    <script src="../js/MyJavaScriptFile.js"></script>  
    <script src="../js/java_general/general_code_java.js"></script> 
</head>
<body>
   
    <form id="form1" runat="server">
          <input type="hidden" id="sid" name="sid" value="<%=Session.SessionID%>" />
          <input type="hidden" id="sid2" name="sid2" runat="server" value="1111111111" />
   
     <div id="ContenidoImpresion" style=" color:black; background-color: #FFFFFF; height:auto; width:auto">
         <div style="margin-left: 10px; margin-right: 10px; margin-top:10px">
             
             <div class="form-group">
                 <label id="Sel" style="font-family: Arial; font-size: 14px"><i class="fal fa-print fa-1x"></i> Modo de impresión</label>
                 <select id="pid" name="pid" style="" class="form-control">
                     <option selected="selected" value="0">Impresora Predeterminada</option>
                     <option value="1">Dialogo Impresion</option>
                 </select>

             </div>

             <input type="button" style="float: right; margin-top: 3px" onclick="javascript: doClientPrinttext_post_text();" class="btn btn-primary" value="Imprimir" />

         </div>
                        
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
                         
         
         
    
    </form>
    

<%-- Register the WebClientPrint script code --%>

    
   <%=Neodynamic.SDK.Web.WebClientPrint.CreateScript(MyUtils.GetWebsiteRoot() & "radicador/WebClientPrintAPI.ashx", MyUtils.GetWebsiteRoot() & "Gestion/printerf.ashx", HttpContext.Current.Session.SessionID)%>
     <script type="text/javascript" src="<%=MyUtils.GetWebsiteRoot() + "/scripts/DemoPrintCommands.js"%>"></script>

</body>
</html>
