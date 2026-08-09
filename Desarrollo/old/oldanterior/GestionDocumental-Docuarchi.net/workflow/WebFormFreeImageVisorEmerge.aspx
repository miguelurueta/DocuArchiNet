<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormFreeImageVisorEmerge.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormFreeImageVisorEmerge" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Neodynamic.WebControls.ImageDraw" Namespace="Neodynamic.WebControls.ImageDraw" TagPrefix="neoimg" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Visor Documental</title>
     <style type="text/css"> 
  #draggable {
    width: 100px;
    height: 100px;
    background: #ccc;
    
  }
   .invisible { 
            visibility: hidden; 
        } 
        
          .cabecera2{
    height : 20px;
    /*position : static;*/
    margin: 0px;
    padding: 0px;
    background: #053061;
    /*width: 100%;*/
    color:White;
    text-align:left;
	top: 0px;
	left: 0px;
     }
     .cabecera{
    height : 7%;
    /*position : static;*/
    margin: 0px;
    padding: 0px;
    background: #053061;
    width: 100%;
    color:White;
    text-align:left;
	top: 0px;
	left: 0px;
}
  </style>
     <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
   <script src="../js/ScrollableGrid2.js" type="text/javascript"></script>
   <script src="../js/sizeimagejquery.js" type="text/javascript"></script>
    <link href="../js/jquery-ui-themes-1.10.3/themes/smoothness/jquery-ui.css" rel="stylesheet" type="text/css" />
   <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <script src="../js/workflow/WebFormFreeImageVisorEmerge.js"></script>
     <script  accesskey="javascript" type="text/javascript">
        
   </script>
</head>
    <body style=" height:480px">
    <form id="form1" runat="server" >
    
     <div id="ContentGeneral" style="width:100%; height:100%; position:absolute">
         
            <asp:ScriptManager ID="ScriptManager1" runat="server"
            EnableScriptGlobalization="True" EnablePageMethods="True">
        </asp:ScriptManager> 
        <script accesskey="javascript" type="text/javascript">
            Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeRequest);
            Sys.Application.add_load(ApplicationLoadHandler)
            var elment_postbak;
            function ApplicationLoadHandler(sender, args) {

                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckStatus);

            }
            function InitializeRequest(sender, args) {
                //
                try {
                elment_postbak = args.get_postBackElement();
                posicion_update_pogres('progres_bar');
                }
                catch (err) {
                    alert(err.message + " Funcion CheckStatus");
                }
            }
            function CheckStatus(sender, args) {
                try {
                progres_hiden('progres_bar');
               
                $("#noaming").bind("contextmenu", function (e) {
                    e.preventDefault();
                });
                }
                catch (err) {
                    alert(err.message + " Funcion CheckStatus");
                }
            }

        </script>
             <div id="tollimage" style="height: 100%; width: 100%;  position: relative; margin-bottom: 1px">

                 <asp:ImageButton ID="ImageButtonInicio" runat="server" ToolTip="Primera  imagen" ImageUrl="../imagewf/inicio14.png" />
                 <asp:ImageButton ID="ImageButtonAnterior" runat="server" ToolTip="Anterior imagen" ImageUrl="../imagewf/anterior15.png" />

                 <asp:UpdatePanel ID="UpdatePanelButon" runat="server"
                     UpdateMode="Conditional" RenderMode="Inline">
                     <ContentTemplate>

                         <asp:Label ID="LabelConteo" runat="server" Text="0/0" Style="margin-left: 10px; margin-right: 10px; margin-bottom: 20px"></asp:Label>

                     </ContentTemplate>
                     <Triggers>
                         <asp:AsyncPostBackTrigger ControlID="ImageFirma" EventName="Click" />
                         <asp:AsyncPostBackTrigger ControlID="ImageButtonInicio" EventName="Click" />
                         <asp:AsyncPostBackTrigger ControlID="ImageButtonAnterior" EventName="Click" />
                         <asp:AsyncPostBackTrigger ControlID="ImageButtonSiguiente" EventName="Click" />
                         <asp:AsyncPostBackTrigger ControlID="ImageButtonFinal" EventName="Click" />
                         <asp:AsyncPostBackTrigger ControlID="ImageMenos" EventName="Click" />
                         <asp:AsyncPostBackTrigger ControlID="ImageMas" EventName="Click" />
                         <asp:AsyncPostBackTrigger ControlID="ImageButtonguardar" EventName="Click" />
                         <asp:AsyncPostBackTrigger ControlID="ImageButtonadjunta" EventName="Click" />
                     </Triggers>
                 </asp:UpdatePanel>
                 <asp:ImageButton ID="ImageButtonSiguiente" runat="server" ToolTip="Siguiente imagen" ImageUrl="../imagewf/siguiente15.png" ImageAlign="NotSet" />
                 <asp:ImageButton ID="ImageButtonFinal" runat="server" ToolTip="Ultima  imagen" ImageUrl="../imagewf/final15.png" />
                 <asp:ImageButton ID="ImageMenos" runat="server" ToolTip="Alejar Imagen" ImageUrl="../imagewf/alejarimagen.png" />
                 <asp:ImageButton ID="ImageMas" runat="server" ToolTip="Acercar Imagen" ImageUrl="../imagewf/acercarimagen.png" />
                 <asp:DropDownList ID="DropDownList_zom" runat="server" AutoPostBack="True">
                         <asp:ListItem Value="10"></asp:ListItem>
                         <asp:ListItem>20</asp:ListItem>
                         <asp:ListItem>30</asp:ListItem>
                         <asp:ListItem>40</asp:ListItem>
                         <asp:ListItem>50</asp:ListItem>
                         <asp:ListItem>60</asp:ListItem>
                         <asp:ListItem>70</asp:ListItem>
                         <asp:ListItem>80</asp:ListItem>
                         <asp:ListItem>90</asp:ListItem>
                         <asp:ListItem>100</asp:ListItem>
                     </asp:DropDownList>
                 <asp:ImageButton ID="ImageFirma" runat="server" ToolTip="Firma Imagen" Visible="false" ImageUrl="../imagewf/firma.png" />
                 <asp:ImageButton ID="ImageButtonadjunta" runat="server" ToolTip="Adjunta imagen a documento" Visible="false" />



             </div>
         <div id="content" style="width: 97%; height: 380px; position: absolute; background-color: Gray; filter: alpha(opacity=70); opacity: 50; overflow: scroll; border-style: ridge; border-bottom-width: 0.5px; border-left-width: 1px; border-right-width: 1px; border-top-width: 1px">
             <div id="zona" style="width: auto; height: auto; position: absolute;">
                 <asp:UpdatePanel ID="UpdatePanelvisor" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                     <ContentTemplate>

                         <neoimg:ImageDraw ID="noaming" runat="server" Style="position: relative" RenderingMethod="HttpHandler" HttpHandlerName="ImageGenerator.axd">
                         </neoimg:ImageDraw>

                     </ContentTemplate>
                     <Triggers>
                     </Triggers>
                 </asp:UpdatePanel>
             </div>
             <div id="draggable" class="ui-widget-content" style="background-color: Gray; display: none; position: absolute">

                 <img id="img" alt="Firma Mecanica"
                     align="bottom" style="border-style: none" />
             </div>


             <input id="Hiddenintercambio" type="hidden" value="0" runat="server">
             <input id="Hiddenintercambio2" type="hidden" value="0" runat="server">
         </div>
         
         <div id="Pietolbar" style="width:99%; height:10px; position:absolute"></div>
         <div id="oculto" style=" visibility:hidden; position:absolute">
              <asp:ImageButton ID="ImageButtonguardar" runat="server"  ToolTip="Guardar firma" Visible="true" />
         </div>

        
    </div>
         <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 50px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>
    <asp:Panel ID="PanelLibre" runat="server"  style = "display:none" ForeColor="White" Width="400px" Height="100px" 
                              HorizontalAlign="Center" >
                             <asp:DragPanelExtender ID="DragPanelExtenderLibre" runat="server" TargetControlID="PanelLibre" /> 
                              <asp:ModalPopupExtender ID="ModalPopupExtenderLibre" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonD"  
                                          PopupControlID="PanelLibre" CancelControlID="Buttoncabcel"   >
                                          </asp:ModalPopupExtender>
                           <div id="Div6" class="cabecera2" >
                              <asp:Button ID="ButtonD"  CssClass="invisible"  runat="server" Text="Button"  Height="20px" Width="20px" />  
                             <asp:Label ID="Labeladver" runat="server" Text="Adjuntar" Font-Size="10" style="float:left">
                             </asp:Label>
                               <div id="Div7" style=" float: right ">
                             
                                      
                                  <asp:Button ID="Buttoncabcel" runat="Server"  Text="X" 
                                       ForeColor="#000066" Height="19px"  ToolTip="Cerrar ventana" />
                                       
                            
                               </div>   
                                           
                           </div>  
                           <div id="Div9"  style=" border:thin double #000080; color:White;  background-color: #FFFFFF; 
                              height:50px; width:400px">
                           
                            <div id="Fileup" style=" height:10% ;  width:100%">
                                <asp:Button ID="ButtonAceptar" runat="server" Text="Aceptar" ViewStateMode="Enabled" style="float:right; width:20%; height:30px" />
                                <asp:FileUpload ID="FileUpload1" runat="server" style="float:left;width:80%;height:30px"  />
                                
                                 
                            </div>
                            
                            </div>      
                                 
                           
                       </asp:Panel> 
    </form>
</body>
</html>
