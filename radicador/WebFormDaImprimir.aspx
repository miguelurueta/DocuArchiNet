<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormDaImprimir.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormDaImprimir" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
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
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <script src="../js/Docuarchi/WebFormDaImprimir.js"></script>
     <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
  <link href="../Awesome/css/brands.css" rel="stylesheet">
  <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
  <script defer src="../Awesome/js/solid.js"></script>
  <script defer src="../Awesome/js/fontawesome.js"></script>
 <style>
 
.waterMark
{  height: 16px;
           width: 168px;
           padding: 2px 2px 2px 2px;
           border: 1px solid #BEBEBE;
           background-color: #F0F8FF;
           color: gray;
           font-size: 8pt;
           text-align: center;
}
</style>
</head>
<body>

   <form id="form1" runat="server">
        <input type="hidden" id="sid" name="sid" value="<%=Session.SessionID%>" />
       <asp:ScriptManager ID="ScriptManager1" runat="server"
            EnableScriptGlobalization="True" EnablePageMethods="True">
        </asp:ScriptManager> 
       <script accesskey="javascript" type="text/javascript">
          
        </script>
           <script accesskey="javascript" type="text/javascript">

            Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeRequest);
            Sys.Application.add_load(ApplicationLoadHandler)
            var elment_postbak;
            function ApplicationLoadHandler(sender, args) {

                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckStatus);

            }
            function InitializeRequest(sender, args) {
                //

                elment_postbak = args.get_postBackElement();
                if (args.get_postBackElement().id == 'Button_imprimir') {
                    document.getElementById("Button_imprimir").disabled = true;
                    document.getElementById("Button_imprimir").value = "Espere...."
                }
                posicion_update_pogres('progres_bar');
            }
            function CheckStatus(sender, args) {
                progres_hiden('progres_bar');
                if (elment_postbak.id == 'Button_imprimir') {
                    document.getElementById("Button_imprimir").disabled = false;
                    document.getElementById("Button_imprimir").value = "Imprimir"

                }
                if (elment_postbak.id == "Button_imprimir") {
                    if (document.getElementById("Hidden_respuesta").value == "YES" && document.getElementById("sid2").value !== "") {
                        javascript: doClientPrinttext();
                        document.getElementById("Hidden_respuesta").value == "NO";
                        document.getElementById("sid2").value = "";
                    }
                    
                }

            }

        </script>
        
    <div>
      
     <div id="ContenidoImpresion" style=" color:black; background-color: #FFFFFF; height:250px; width:auto" class="p-1">
        
         <asp:Table ID="Table_selecion" runat="server" Style="width:100%">
             <asp:TableRow>
                 <asp:TableCell>
                      <label id="Label2" style="" class="h6 font-weight-light" > Seleccione el modo de impresión</label>
                 </asp:TableCell>
                 <asp:TableCell>
                     <select id="pid" style="width:100%; float:left" name="pid" > <option  value="1">Dialogo de Impresion</option>
                                      <option selected="selected" value="0">Impresora Predeterminada</option>
                     </select>
                 </asp:TableCell>
             </asp:TableRow>
         </asp:Table>

         <asp:Table ID="Table_selecion_pagina" runat="server" Style="width: 100%; height:auto; margin-top:5px; margin-bottom:5px; background:#6d7fcc"   CssClass="content_option_selecion" >
       
             <asp:TableRow ID="todo" Style="border-bottom: 1px ridge white">
                 <asp:TableCell>
                     <asp:RadioButton ID="RadioButton_todo" runat="server" Checked="true" GroupName="data" CssClass="ml-1"/>
                     <label id="tod_1" style="font-family: Arial; font-size: 11px; color:white">Todas las páginas </label>
                     
                 </asp:TableCell>
                 <asp:TableCell>
                     <label id="Label1" runat="server"></label>
                 </asp:TableCell>
             </asp:TableRow>
             <asp:TableRow ID="barra" CssClass="_seleccion_campo_adjunta_  bor" style="background:#6d7fcc; border-bottom: 1px ridge white"  >
                 <asp:TableCell>
                     <asp:RadioButton ID="RadioButton_rango" runat="server" GroupName="data" CssClass="ml-1" />
                     <label id="barr" style="font-family: Arial; font-size: 11px; color:white">Rangos de páginas</label>
                 </asp:TableCell>
                 <asp:TableCell>
                     <asp:TextBoxWatermarkExtender ID="TextBox1_TextBoxWatermarkExtender" runat="server" BehaviorID="TextBox1_TextBoxWatermarkExtender" TargetControlID="TextBox_ini" WatermarkCssClass="waterMark_ mt-1" WatermarkText="Seleción 1,2,3.. Rangos 1-5" />
                     <asp:TextBox ID="TextBox_ini" runat="server" Style="width: 200px" CssClass="solo-numero form-control mt-1"></asp:TextBox>
                     <br />
                     <label id="barr1_2" style="color: red; font-family: Arial; font-size: 10px; color:white">Las paginas individuales se separan con  comas (Ej; 1,5..) </label>
                     <br />
                     <label id="barr1_3" style="color: red; font-family: Arial; font-size: 10px; color:white">los rangos de paginas se definen separados por un guion (ej; 1-5) </label>

                 </asp:TableCell>
             </asp:TableRow>
             <asp:TableRow ID="fecha">
                 <asp:TableCell>
                     <asp:RadioButton ID="RadioButton_seleccion" runat="server" GroupName="data" CssClass="ml-1"/>
                     <label id="fech" style="font-family: Arial; font-size: 11px; color:white" >Página actual</label>
                 </asp:TableCell>
                 <asp:TableCell>
                                    
                 </asp:TableCell>
             </asp:TableRow>
            
             <asp:TableRow>
                 <asp:TableCell ColumnSpan="2">
                   
                 </asp:TableCell>
             </asp:TableRow>
         </asp:Table>
         <asp:Table ID="Table_boton" runat="server" Style="width:100%" CssClass="mt-2">
             <asp:TableRow>
                 <asp:TableCell ID="cel_option">
                       <asp:CheckBox ID="CheckBox_pdf_convert" runat="server" Text="Convertir e imprimir pdf" Checked="true" Style="" class="h6 font-weight-light" />
                     &nbsp
                     <asp:CheckBox ID="CheckBox_marca_agua" runat="server" Text="Imprimir con marca de agua" Style="" class="h6 font-weight-light" />
                 </asp:TableCell>
                  <asp:TableCell >
                      <asp:UpdatePanel ID="updatepanel_imprimir" runat="server" UpdateMode="Conditional">
                         <ContentTemplate>
                             <asp:Button ID="Button_imprimir" runat="server" Text="Imprimir" class="btn btn-primary" Style="float: right; margin-bottom:1px; background:#6d7fcc" />
                             <input type="hidden" id="Hidden_respuesta" name="Hidden_respuesta" runat="server" value="NO" />
                             <input type="hidden" id="sid2" name="sid2" runat="server" value="" />
                             <input type="hidden" id="hideextension" name="hideextension" runat="server" value="" />
                         </ContentTemplate>
                     </asp:UpdatePanel>
                 </asp:TableCell>
             </asp:TableRow>
            
         </asp:Table>
                 <div id="netPrinter" style="display:none">
                <label for="netPrinterHost">Printer's DNS Name or IP Address:</label>
                <input type="text" name="netPrinterHost" id="netPrinterHost" />
                     <input type="text" name="extension_file" id="extension_file" />
                <label for="netPrinterPort">Printer's Port:</label>
                <input type="text" name="netPrinterPort" id="netPrinterPort" />
            </div>

            <div id="parallelPrinter" style="display:none">
                <label for="parallelPort">Parallel Port:</label>
                <input type="text" name="parallelPort" id="parallelPort" value="LPT1" />

                <textarea id="printerCommands" name="printerCommands" rows="10" cols="80" class="span9"> ojo impreion </textarea>
            </div>
    
            </div>       
                         
         </div>
          <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
                <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
                Processing ...
            </div>
    
    </form>
    

    
    <%=Neodynamic.SDK.Web.WebClientPrint.CreateScript(MyUtils.GetWebsiteRoot() & "radicador/WebClientPrintAPI.ashx", MyUtils.GetWebsiteRoot() & "radicador/PrinterFileDa.ashx", HttpContext.Current.Session.SessionID)%>
   
     <script type="text/javascript" src="<%=MyUtils.GetWebsiteRoot() + "/scripts/DemoPrintCommands.js"%>"></script>

</body>
</html>

