<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormImprimir.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormImprimir" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="../js/ui/jquery-3.4.1.min.js"></script>  
      <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script> 
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
     <link href="../Styles/Aplicaction.css" rel="stylesheet" />
     <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
  <link href="../Awesome/css/brands.css" rel="stylesheet">
  <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
  <script defer src="../Awesome/js/solid.js"></script>
  <script defer src="../Awesome/js/fontawesome.js"></script>
    <script src="../js/MyJavaScriptFile.js"></script>  
    <script src="../js/java_general/general_code_java.js"></script> 
</head>
<body>

    <form id="form1" runat="server">
         <input type="hidden" id="sid" name="sid" value="<%=Session.SessionID%>" />
        <input type="hidden" id="sid2" name="sid2" runat="server" value="1111111111" />
        <input type="hidden" id="Hidden_ruta_archivo" name="Hidden_ruta_archivo" runat="server" value="hhh" />
    <div>
        <div id="ContenidoImpresion" style="color: black; background-color: #FFFFFF; height: auto; width: auto" class=" p-4">
            <div class="row justify-content-center">
                <span class="font-weight-light h5" style="font-family: 'Segoe UI'"><i class="fal fa-print fa-2x"></i> Impresión </span>
            </div>
            <div class="row mt-4">
                <div class="col-6">
                    <span id="usuario" class="font-weight-light h6" style="font-family: 'Segoe UI'">Usuario radicador </span>
                </div>
                <div class="col-6">
                    <label id="usuario_datos" class="font-weight-light h6" style="font-family: 'Segoe UI'" runat="server"></label>
                </div>
            </div>
            <div class="row">
                <div class="col-6">
                    <label id="radicad" class="font-weight-light h6" style="font-family: 'Segoe UI'">Consecutivo radicado </label>
                </div>
                <div class="col-6">
                    <label id="radicad_datos" runat="server" class="font-weight-light h6" style="font-family: 'Segoe UI'"></label>
                </div>
            </div>
            <div class="row">
                <div class="col-6">
                    <label id="barr" class="font-weight-light h6" style="font-family: 'Segoe UI'">Código de barras</label>
                </div>
                <div class="col-6">
                    <label id="barr_datos" runat="server"></label>
                </div>
            </div>
            <div class="row">
                <div class="col-6">
                    <label id="fech" class="font-weight-light h6" style="font-family: 'Segoe UI'">Fecha radicado</label>
                </div>
                <div class="col-6">
                    <label id="fech_datos" runat="server" class="font-weight-light h6" style="font-family: 'Segoe UI'"></label>
                </div>
            </div>
            <div class="row">
                <div class="col-6">
                    <label id="Sel" class="font-weight-light h6" style="font-family: 'Segoe UI'">Modo impresión</label>
                </div>
                <div class="col-6">
                    <select id="pid" name="pid" style="width: 100%" class="form-control">
                        <option selected="selected" value="0">Impresora Predeterminada</option>
                        <option value="1">Dialogo Impresion</option>

                    </select>
                </div>
            </div>
            <div class="row mt-3">
                <div class="col-6">
                </div>
                <div class="col-6">
                    
                    <button type="button" onclick="printPdf()" class="btn btn-success mt-3" value="descargar"><i class="fal fa-long-arrow-down">  </i> Descargar </button>  
                    <button type="button" onclick="javascript: doClientPrinttext();" class="btn btn-success mt-3" value="Imprimir"><i class="fal fa-print"></i> Imprimir </button> 
                </div>
            </div>

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
               <iframe id="ifrma_load_" style="display:none"></iframe>           
         </div>
         
    
    </form>
    



<%-- Register the WebClientPrint script code --%>

    <script type="text/javascript" src="<%=MyUtils.GetWebsiteRoot() + "/scripts/DemoPrintCommands.js"%>"></script>
    
    <%=Neodynamic.SDK.Web.WebClientPrint.CreateScript(MyUtils.GetWebsiteRoot() & "radicador/WebClientPrintAPI.ashx", MyUtils.GetWebsiteRoot() & "radicador/PrinterTexto.ashx", HttpContext.Current.Session.SessionID)%>
   <script  accesskey="javascript" type="text/javascript">
       
       function printPdf() {
           try {
               var url;
               url = "../radicador/WebFormDescargaRadicado.aspx";
               document.getElementById("ifrma_load_").src = url;
           } catch (ex) { alert("funcion printPdf " + ex.message); }
       }
     </script>
     
</body>
</html>
