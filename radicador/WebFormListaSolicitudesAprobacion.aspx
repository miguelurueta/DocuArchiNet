<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormListaSolicitudesAprobacion.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormListaSolicitudesAprobacion" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
     <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
   <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/Filtrar.js"></script>
    <script src="../js/radicacion/WebFormListaSolicitudesAprobacion.js"></script>
    <script src="../Fixed-Header-Table-master/gridviewScroll.min.js"></script>
   <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/validate_campos.js"></script>
    <script src="../js/java_general/general_code_java.js"></script>

   <style type="text/css">
  
        .invisible { 
            visibility: hidden; 
        } 
    </style>
    <script accesskey="javascript" type="text/javascript">
           
    </script>
</head>
<body style="margin-top:2px">
     <form id="form1" runat="server" onkeypress="return caracter_especial(event,this)">
     
        <asp:ScriptManager ID="ScriptManager1" runat="server"
            EnableScriptGlobalization="True" EnablePageMethods="True" AsyncPostBackTimeout="900">
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
                elment_postbak = args.get_postBackElement();
                posicion_update_pogres('progres_bar');
            }
            function CheckStatus(sender, args) {
                progres_hiden('progres_bar');
                //ELIMINA REGISTRO PENDIENTE DEL GREDVIEW ImageButtonterminar
                if (elment_postbak.id == "btnOkpagina") {
                    elimina_registro_gred_pendiente();
                    document.getElementById("HiddenPROMP").value = "0";
                    //auto_zise_popup_pendinetes();
                }
                if (elment_postbak.id == "ImageButtonterminar") {
                    elimina_registro_gred_pendiente();
                    document.getElementById("HiddenPROMP").value = "0";
                    auto_zise_popup_pendinetes();
                }
                //ButtonReasignarTerminar
                if (elment_postbak.id == "ButtonReasignarTerminar") {
                    elimina_registro_gred_pendiente();
                    document.getElementById("HiddenPROMP").value = "0";
                    auto_zise_popup_pendinetes();
                }
                if (elment_postbak.id == "Button_autoriza_reasignacion") {
                    elimina_registro_gred_pendiente();
                    document.getElementById("HiddenPROMP").value = "0";
                    auto_zise_popup_pendinetes();
                }

                if (elment_postbak.id == "Buttonbuscar") {
                    busqueda_gred('Hidden_id', 'GridViewlista', 'contenidobusqueda', 'CheckboxBusqueda');
                }
                //btnOkay
                if (elment_postbak.id == "btnOkay") {
                    sube_tarea_a_pendiente();
                    auto_zise_popup_pendinetes();

                }
                //Button_lista_filtro
                if (elment_postbak.id == "Button_lista_filtro") {
                    auto_zise_popup_pendinetes();
                }
                if (elment_postbak.id == "ImageButtonEnviarUsuario") {
                    document.getElementById("Labeletiqueta").innerHTML = "Enviar la tareas seleccionadas al usuario a seleccionar";
                    auto_zise_popup_envia_usuario_grupo();
                }
                if (elment_postbak.id == "ImageButtonEnviaActividad") {
                    document.getElementById("Labeletiqueta").innerHTML = "Enviar la tareas seleccionadas al grupo o activdad a seleccionar";
                    auto_zise_popup_envia_usuario_grupo();
                }
                //ButtonFiltro
                if (elment_postbak.id == "ButtonFiltro") {

                    filtro_gred('Hidden_id', 'GridViewlista', 'contenidobusqueda_filtro', 'CheckboxBusqueda_f');
                    auto_zise_popup_pendinetes();
                }
                //Button_actualiza_pendiente  
                if (elment_postbak.id == "Button_actualiza_pendiente") {
                    auto_zise_popup_pendinetes();
                }
                if (elment_postbak.id == "Button_activa_ver_nota_general") {
                    auto_zise_popup_paginas_externas_libres();

                }
                if (elment_postbak.id == "Button_listar_usuarios_relacionados") {
                    auto_zise_popup_usuarios_relacionados();
                }
                //Button_actualiza_pendiente
                if (elment_postbak.id == "Button_actualiza_pendiente" && document.getElementById("Hidden_Resultado_actualiza").value == "YES") {
                    actualiza_gred('DETALLEPENDIENTE', document.getElementById("contenidobusqueda_actualiza").value);
                    document.getElementById("Hidden_Resultado_actualiza").value == "";
                }
                //auto_zise_popup_pendinetes();
                if ($("#" + 'GridViewlista' + " tr:visible").length > 0) {
                    //mueve_scroll_data_gred('GridViewlista', 'Hidden_id');
                }
                //Button_sacar_pendiente
                if (elment_postbak.id == "Button_sacar_pendiente") {
                   
                    if (document.getElementById("Hidden_resultado_pendiente").value == "YES") {
                        document.getElementById("Hidden_resultado_pendiente").value = "";
                        var id_pendiente = document.getElementById("Hidden_id_pendiente").value + "-" + document.getElementById("Hidden_id_tarea").value;
                        asignar_tarea_pendiente(id_pendiente);
                    }
                }
            }

            </script>
        <INPUT id="hdnEmailID" type="hidden" value="0" runat="server" >
        <INPUT id="hdnEmailID_sel" type="hidden" value="0" runat="server" >
        <INPUT id="Hidden_id" type="hidden" value="0" runat="server" >
         <INPUT id="Hidden_rad" type="hidden" value="0" runat="server" >
        <INPUT id="HiddenPROMP" type="hidden" value="0" runat="server" >
        <input id="Hidden_id_tarea_sel" type="hidden" value="-1" runat="server">
        <input id="Hidden_tipo_visor" type="hidden" value="" runat="server">
        <input id="Hidden_seleccion" type="hidden" value="YES" runat="server">
         <div id="div_contendor_principal">
              <div id="div_titulo_listado" style="width: 100%; height: 15%; margin: 0px 2px 1px 1px" class="border_superior_radius">
            <asp:UpdatePanel ID="updata_panel_pendiente" runat="server" UpdateMode="Conditional">
                <ContentTemplate>  
                    <asp:Button ID="ButtonUnir" runat="server" Text="Unir Tareas " ToolTip="Buscar tareas en pendiente" CssClass="boton" Style="margin-left: 5px; display: none" />
                    <asp:TextBox ID="contenidobusqueda" runat="server" placeholder="Buscar..." Style="Width: 270px; float: right; margin-right: 1px; margin-top: 0px"></asp:TextBox>
                    <asp:Button ID="Buttonbuscar" runat="server" Text="Buscar" Style="height: 30px; border-color: #b0c4de; float: right; display:none" ToolTip="Buscar tareas en la lista" CssClass="boton_azul" OnClientClick="busqueda_gred('Hidden_id','GridViewlista','contenidobusqueda','CheckboxBusqueda');" />
                    <asp:ImageButton ID="ImageButton_buscar" runat="server" Style="margin-top: 4px; float: right; margin-right: 4px" ImageUrl="../radicador/imagenes/cbxs0-vnnbp.png" />
                    <asp:Button ID="Button_visor_emergente" runat="server" Text="Button" Style="display: none" />
                </ContentTemplate>
            </asp:UpdatePanel>
           
        </div>
         <div id="div_contendor_filtro_listado" class="border_general_blanco" style="width:100%; height:auto" >   
                <asp:Label ID="Label_anunciado_filtro" runat="server" Text="✓ Solicitudes Aprobadas" style="font-size:12px; font-family:Arial; float:left; margin-top:5px; float:right; margin-right:2px"></asp:Label> 
                 <div id="div_filtro__fil" class="dropdown_filter" >
                    <button id="boton__filtro_ver" onclick="myFunction(event,this)" class="dropbtn_filter">Filtrar</button>
                    <div id="myDropdown" class="dropdown-content_filter" onkeyup="hiden_keys(event, thiss)">
                        <input type="text" placeholder="Search.." id="myInput" onkeyup="filterFunction()">
                        <a href="#about" onclick="event_elemento(event,'1',this)" class="e_list_marc"> ✓ Solicitudes Aprobadas </a>
                        <a href="#base" onclick="event_elemento(event,'2',this)" class="e_list_marc">Solicitudes Pendientes por aprobar</a>
                        <a href="#blog" onclick="event_elemento(event,'3',this)" class="e_list_marc">Solicitudes Archivadas</a>
                        <a href="#blog" onclick="event_elemento(event,'4',this)" class="e_list_marc">Solicitudes Desaprobadas</a>
                    </div>
                </div>
                
            </div>
           
            <div style="display:none">
                <asp:UpdatePanel ID="UpdatePanel_menu_boton" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                            <ContentTemplate>
                                 <asp:Button ID="Button_lik_service_boton" runat="server" Text="Button"  />
                                <input id="Hidden_lik_service_boton" type="hidden" value="0" runat="server">
                                </ContentTemplate>
                    </asp:UpdatePanel>
            </div>
        <div id="content_grid" style="width:100%; float: left; height: 70%; margin: 0px 2px 1px 1px; margin-top: 1px; border-color: #b0c4de; border-style: ridge; border-width: 1px; position: relative">
            <asp:UpdatePanel ID="UpdatePanelmensaje" runat="server" UpdateMode="Conditional" RenderMode="Inline"  >
                <ContentTemplate>
                        
                     <asp:Panel ID="Panel_principal" runat="server"  ScrollBars="Auto"
                            Width="100%" Style="">
                         <asp:GridView ID="GridViewlista" runat="server" Font-Names="arial" Font-Size="12px" AllowSorting="true" 
                            PagerSettings-Position="Top"  AllowPaging="true"  EnableViewState="true"
                            AutoGenerateSelectButton="False" CssClass="filtrar"  GridLines="None" style="margin-left:1px; width:100%">
                              <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True"  ForeColor="Red"  />
                              <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <HeaderStyle CssClass="GridviewScrollHeader_line_blanco_cort" BorderStyle="None" />
                                    <RowStyle CssClass="GridviewScrollItem_line_cort"  />
                                    <PagerStyle CssClass="pagination-ys" />
                                    <Columns>
                                        <asp:BoundField HeaderText="OPCIONES   " />
                                    </Columns>

                        </asp:GridView>
                         </asp:Panel>             
                         <div id="contenido_titulo_listado_solicitudes" style="width: 100%; float: inherit" class="border_inferior_radius">
                            <asp:Label ID="Label_titulo_listado_solicitudes" runat="server" ForeColor="Black" Font-Size="12px" Style="margin-left: 3px; font-family:Arial">Resultados busqueda</asp:Label>
                             <asp:Label ID="Label_estado" runat="server" Text="" Style="font-size: 10px; font-family: Arial; float: right"></asp:Label>
                        </div>  
                        
                </ContentTemplate>
                <Triggers>
       
                </Triggers>
            </asp:UpdatePanel>

        </div>                  
        <div id="buton" style="width: 100%; float: left; height:auto; overflow:auto; padding-top:5px; padding-bottom:5px; display:none">           
            <asp:UpdatePanel ID="Upadatepanel_botnoes" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="ButtonFiltrar" runat="server" Text="Fltrar " ToolTip="Filtrar tareas" CssClass="boton_blanco" Style="border-color: #b0c4de; margin-left: 5px; font-size: 11px; width: 50px; height: 30px; display: none" />
                    <asp:Button ID="Button_actualiza_datos" runat="server" Text="Actualiza" ToolTip="Actualizar datos identificacion pendiente" CssClass="boton_blanco" Style="border-color: #b0c4de; margin-left: 3px; font-size: 11px; display: none" Height="30px" Width="80px" />
                    <asp:Button ID="Button_sacar_pendiente" runat="server" Text="Completar la gestión " ToolTip="Asignar tarea desde pendientes para completar la gestión de la respuesta" CssClass="boton_azul" Style="margin-left: 3px; height: 30px; width: 200px" />
                    &nbsp
            <asp:Button ID="ButtonSubir" CssClass="boton_blanco" runat="server" Text="Enviar tarea a pendiente  " ToolTip="Subri tareas a pendientes" Style="font-size: 11px; height: 30px; width: 200px; display: none" />
                    &nbsp
                    <asp:Button ID="Button_ver_documento_solicitud" runat="server" Text="Ver documento solicitud  " ToolTip="Ver el documento solicitud" CssClass="boton_azul" Style="margin-left: 3px; height: 30px; width: 200px" />
                    &nbsp
                    <asp:Button ID="Button_ver_documento_respuesta_solicitud" runat="server" Text="Ver respuesta solicitud  " ToolTip="Ver el documento respuesta solicitud" CssClass="boton_azul" Style="margin-left: 3px; height: 30px; width: 200px" />
                    &nbsp
                    <asp:Button ID="ButtonActiva_solicitud_aprobacion" runat="server" Text="Solicitud de aprobación" ToolTip="Gestiona las solicitudes de aprobación" CssClass="boton_azul" Style="margin-left: 3px; height: 30px; width: 200px" />
                    &nbsp
                    <asp:Button ID="Button_activa_ver_nota_general" runat="server" Text="Ver todas las notas" ToolTip="Ver todas las notas de la solicitud" CssClass="boton_azul" Style="margin-left: 3px; height: 30px; width: 200px" OnClientClick="auto_zise_popup_paginas_externas_libres();" />
                   
                    <asp:Button ID="Button_lista_filtro" runat="server" Text="Button" Style="display: none" />
                    <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server">
                    <input id="Hidden_resultado_pendiente" type="hidden" value="" runat="server">
                    <input id="Hidden_id_pendiente" type="hidden" value="0" runat="server">
                    <input id="Hidden_id_tarea" type="hidden" value="0" runat="server">
                </ContentTemplate>
            </asp:UpdatePanel>
           
        </div>
         </div>
       
         <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 50px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>
    <!--codigo cuadro de dialogo-->
   <input id="HiddenFiltro" type="hidden" value="" runat="server">
     <div id="framemensaje">          
                <asp:Panel ID="Panelmensaj" runat="server"  style = "display:none" ForeColor="White" Width="348px" Height="160px" HorizontalAlign="Center" >
                    <asp:ModalPopupExtender ID="ModalPopupTexto" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button4"
                        PopupControlID="Panelmensaj" >
                    </asp:ModalPopupExtender> 
                    <div id="Div2" class="cabecera2">
                         
                        <asp:Label ID="Label2" runat="server" Text="Mensaje" Font-Size="10">
                        </asp:Label>
                    </div> 
                                
                    <div id="container" style="border: thin double #000080; color: White; background-color: #FFFFFF; height: 87%; width: 347px">
                        <div id="Contenido" style="height: 60%">
                            <br />
                            <label id="Lableme" style="font-size: 14px; color: #000000" title="" />
                            <asp:Label ID="LabelMensaje" runat="server" Text="Identificacion de pendiente" ForeColor="Black" Font-Size="9" Visible="True" />
                            <asp:TextBox ID="TextBoxdatos" runat="server" Width="300px">
                            </asp:TextBox>
                            <asp:AutoCompleteExtender ID="AutoCompleteExtender_pendiente" runat="server"
                                TargetControlID="TextBoxdatos" MinimumPrefixLength="2"
                                EnableCaching="true" CompletionSetCount="10" CompletionInterval="50"
                                ServiceMethod="GetPosiblesDatos" ServicePath="../webservice/WebServiceWorkflow.asmx"
                                ContextKey="datos_pendiente|tarea_pendiente" UseContextKey="True"
                                CompletionListCssClass="completionList" CompletionListHighlightedItemCssClass="itemHighlighted"
                                CompletionListItemCssClass="listItem" OnClientShown="onDataShown">
                            </asp:AutoCompleteExtender>
                        </div>
                        <asp:UpdatePanel ID="UpdatePnael_botones_subir_pendiente" runat="server"  UpdateMode="Conditional">
                            <ContentTemplate>
                                <div id="Contenidbuton" style="height: 29%; color: White; background-color: #FFFFFF;">
                                    <asp:Button ID="btnOkay" runat="server" Text="Aceptar "  CssClass="boton" />
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancelar " CssClass="boton"/>
                                    <br />
                                   
                                    <input id="Hidden_resultado" type="hidden" value="" runat="server">
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                        <div id="Div3" style="height: 10%; color: White; background-color: #FFFFFF;">
                            <asp:Button ID="Button4" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                        </div>
                    </div>
                
                </asp:Panel> 
       </div>
      
     <div id="Filtro">
              <asp:Panel ID="Panel_filtro" runat="server" Style="display: none; color: White; width: auto; height: auto">
                 
                  <asp:ModalPopupExtender ID="ModalPopupExtender_Filtro" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Buttond_Filtro"
                      PopupControlID="Panel_filtro" CancelControlID="Button_Filtro_Cerrar">
                  </asp:ModalPopupExtender>
                  <div id="divcabecer_filtro" class="cabecera2">
                      <asp:Button ID="Buttond_Filtro" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                      <asp:Label ID="Label1" runat="server" Text="Filtrar Lista" Font-Size="10" Style="float: left">
                      </asp:Label>
                      <div id="Divcerrarbuton_filtro" style="float: right">
                          <asp:Button ID="Button_Filtro_Cerrar" runat="Server" Text="X"
                              ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                      </div>
                  </div>
                
                  <div id="Diupdate_filtro" style="border: thin double #000080; color: White; background-color: #FFFFFF; height: auto; width: auto">
                      <div id="Contenidopagina_filtro" style="height: 140px; width: 450px; overflow: no-display; color: black; margin-left: 15px">
                          <asp:UpdatePanel ID="updata_panel_filtro" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <br />
                                  <label style="font-family: Arial; font-size: 11px">Busqueda Tareas a filtrar </label>
                                  <br />
                                  <label style="font-family: Arial; font-size: 11px">Digita Texto </label>
                                  <asp:TextBox ID="contenidobusqueda_filtro" runat="server"></asp:TextBox>
                                  <asp:Button ID="ButtonFiltro"  Text="Aceptar" runat="server" class="boton" />
                                  <asp:CheckBox ID="CheckboxBusqueda_f" runat="server" Text="Sólo palabras completas" Font-Size="10" Font-Names="arial"  />
                                  <br />
                              </ContentTemplate>
                          </asp:UpdatePanel>

                      </div>

                  </div>
                  <div id="border_filtro" style="color: white; font-size: small; background-color: #053061; width: 470px; height: 10px">
                  </div>
                 
              </asp:Panel>
          </div>
        
      <!--Mensaje popup para paginas externas enviar tarea usuario, enviar tarea a grupo-->
      <div id="envioactividad"   >    
              <asp:Panel ID="Panelpagina" runat="server" Style="display:none; color: White; width: 100%; height: 100%;  margin-top:1px">
               
                  <div id="Divcab" class="cabecera2">
                      <asp:ModalPopupExtender ID="ModalPopupExtendermesjpagina" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Buttonhide_full"
                          PopupControlID="Panelpagina" CancelControlID="Button_Cerrar" ></asp:ModalPopupExtender>
                       
                      <asp:Label ID="Labeletiqueta" runat="server" Text="Enviar tarea" Font-Size="10"></asp:Label>
                     
                      <div id="Divlabel" style="float: right">
                          <asp:Button ID="Buttonhide_full" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                          <asp:Button ID="Button_Cerrar" runat="Server" Text="X" />
                          
                      </div>
                      <br />
                  </div>

                  <div id="DivColorPagina" style="border: thin double #000080; color: White; background-color: #FFFFFF; height: 80%; width: 100%">

                          <asp:UpdatePanel ID="UpdatePanelpagina" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                              <ContentTemplate>

                                  <iframe id="frameeditexpanse_" runat="server" frameborder="0"  scrolling="no" style="width:100%; height:100%"></iframe>

                              </ContentTemplate>
                          </asp:UpdatePanel>
                 
                  </div>
                  <div id="DivBotones" style="height:20%; margin-top:1px; background-color:white; color:black; background-color: #FFFFFF" >
                      <asp:UpdatePanel ID="Updatecondiciona" runat="Server" UpdateMode="Conditional" RenderMode="Inline">
                          <ContentTemplate>
                           <asp:Button ID="btnCancelpagina" runat="server" Text="Cancelar " Style="float: right; margin-right: 10px; margin-left: 10px; margin-top: 1px" CssClass="boton_blanco" />
                               &nbsp
                              <asp:Button ID="ButtonReasignarTerminar" runat="server" Text="Reasignar " Style="float: right; margin-left: 10px; margin-top: 1px; background-color:yellow" OnClientClick="confirma_respuesta(&quot;Desea terminar y reasignar la tarea&quot;);" CssClass="boton" ToolTip="Reasigna y envia la tarea al usuario seleccionado" />           
                               &nbsp
                          <asp:Button ID="btnOkpagina" runat="server" Text="Aceptar " Style="float: right; margin-left: 10px; margin-top: 1px" OnClientClick="confirma_respuesta('Desea enviar las tareas seleccionadas');" CssClass="boton_blanco" />
                          <input id="Hiddenseltareas" type="hidden" value="0" runat="server">
                              <input id="Hidden_lista_eliminar_tarea" type="hidden" value="0" runat="server">
                          </ContentTemplate>
                      </asp:UpdatePanel>
                     
                     
                    </div>
                   
              </asp:Panel>
      
       </div>
         <input id="Hidden1" type="hidden" value="-1" runat="server">
           <input id="Hidden2" type="hidden" value="" runat="server">
               <!--Popup visor externo-->
               <asp:Panel ID="Panel_visor_externo" runat="server" Style="display:none; overflow:hidden" ForeColor="White" Width="100%" Height="100% " CssClass="modal_content_general" >
                  <asp:ModalPopupExtender ID="ModalPopupExtender_visor_externo" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_visor_externo"
                      PopupControlID="Panel_visor_externo"  CancelControlID="ButtonSalir_visor_externo">
                  </asp:ModalPopupExtender>
                  <div id="Cabecerapendiente_visor_externo" class="modal_title_superior" style="height:25px">
                      
                      <asp:Label ID="Label12" runat="server" Text="Documentos Relacionados" Font-Size="10"></asp:Label>
                      <div id="Div_visor_externo" style="float: right">
                          <asp:Button ID="ButtonSalir_visor_externo" runat="Server" Text="X" CssClass="modal_boton_hiden"
                               ToolTip="Cerrar ventana" />

                      </div>
                  </div>
                  <div id="Cotenedorpendiente_visor_externo" style=" background-color: #FFFFFF; height: 100%; width: 100%; overflow:hidden" class="modal_content_back">
                  
                      <asp:UpdatePanel ID="UpdatePanel_visor_externo" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_visor_externo_wf_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>

                      </asp:UpdatePanel>
                           
                  </div>
                  <asp:Button ID="Button_visor_externo" style="display:none" runat="server" Text="Button" Height="20px" Width="20px" />
              </asp:Panel>
      <!--Actualizar dato pendiente-->
        <asp:Panel ID="panel_actualiza_datos" runat="server" Style="display:none; color: White; width: 470px; height: 350px; margin-top: 1px">
            <div id="Divcabutil" class="cabecera2">
                <asp:ModalPopupExtender ID="ModalPopupExtenderactualiza" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_targ_actualiza"
                    PopupControlID="panel_actualiza_datos" CancelControlID="ButtonCerrarpendiente" Y="1">
                </asp:ModalPopupExtender>
                <asp:Label ID="Label3" runat="server" Text="Actualiza" Font-Size="10"></asp:Label>
                <div id="Div1" style="float: right">
                    <asp:Button ID="ButtonCerrarpendiente" runat="Server" Text="X"
                        ForeColor="#000066" Height="21px" />

                </div>
                <div id="Divlabel_actualiza" style="float: right">
                    <asp:Button ID="Button_targ_actualiza" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                </div>
                <br />

            </div>
            <div id="Diupdate_actualiza" style="border: thin double #000080; color: White; background-color: #FFFFFF; height: auto; width: auto">
                <div id="Contenidopagina_actualiza" style="height: 140px; width: 450px; overflow: no-display; color: black; margin-left: 15px">
                    <asp:UpdatePanel ID="Updatepanael_Actualiza" runat="server" UpdateMode="Conditional" >
                        <ContentTemplate>
                            <br />
                            <label style="font-family: Arial; font-size: 12px">Digita nuevo id del pendiente </label>
                            <br />
                            <br />
                            <label style="font-family: Arial; font-size: 12px; margin-top: -1px">Digita Texto </label>
                            <asp:TextBox ID="contenidobusqueda_actualiza" runat="server" Style="width: 250px" />
                            <asp:AutoCompleteExtender ID="auto_complete" runat="server"
                                TargetControlID="contenidobusqueda_actualiza" MinimumPrefixLength="2"
                                EnableCaching="true" CompletionSetCount="10" CompletionInterval="50"
                                ServiceMethod="GetPosiblesDatos" ServicePath="../webservice/WebServiceWorkflow.asmx"
                                ContextKey="datos_pendiente|tarea_pendiente" UseContextKey="True"
                                CompletionListCssClass="completionList" CompletionListHighlightedItemCssClass="itemHighlighted"
                                CompletionListItemCssClass="listItem" OnClientShown="onDataShown">
                            </asp:AutoCompleteExtender>
                            <asp:Button ID="Button_actualiza_pendiente" type="button" Text="Aceptar" runat="server" class="boton" />
                            <input id="Hidden_Resultado_actualiza" type="hidden" value="" runat="server">
                            <br />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                   

                </div>
            </div>
        </asp:Panel>
         <!--autoriza reasignacion-->
          <div id="autoriza_reasignacion_tarea">
            <asp:Panel ID="Panel_autoriza_reasignacion_tarea" runat="server" Style="display:none; color: White; width: 600px; height: 200px">

                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_autoriza_reasignacion_tarea" runat="server" BehaviorID="Panel_autoriza_reasignacion_tarea" TargetControlID="ButtonSalir_autoriza_reasignacion_tarea" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_autoriza_reasignacion_tarea" PopupControlID="Panel_autoriza_reasignacion_tarea" ></asp:ModalPopupExtender>
                <div id="divcabecer2_autoriza_reasignacion_tarea" class="cabecera2">
                    <asp:Button ID="Button_autoriza_reasignacion_tarea" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_autoriza_reasignacion_tarea" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label_autoriza_reasignacion_tarea" runat="server" Text="Autoriza reasignación" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_autoriza_reasignacion_tarea" style="float: right">
                        <asp:Button ID="Button_cerrar_autoriza_reasignacion_tarea" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_autoriza_reasignacion_tarea" style="background-color: white; width: 100%; height: 99%;border: thin double #000080; color: black; background-color: #FFFFFF;">
                                
                    
                        <asp:UpdatePanel ID="UpdatePanel_autoriza_reasignacion_tarea" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                               <br />
                                <table style="width: 100%;">
                                   
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_user_autoriza_reasignacion_tarea" runat="server" Text="Usuario autorizado*" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label>
                                        </td>
                                        <td><asp:TextBox ID="TextBox_login_autoriza_reasignacion_tarea" runat="server" Style="width:300px"></asp:TextBox></td>
                                       
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_dest_autoriza_reasignacion_tarea" runat="server" Text="Contraseña usuario*" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label>

                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_pasw_autoriza_reasignacion_tarea" runat="server" Style="width:300px"  TextMode="Password"></asp:TextBox> 
                                           

                                        </td>                           
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    
                                    <tr>
                                        <td>

                                        </td>
                                        <td style="float:left"><asp:Button ID="Button_autoriza_reasignacion" runat="server" Text="Reasignar" Style="background-color: white; border-color: #b0c4de; height: 30px; width: 200px; height: 25px; text-align: center" CssClass="boton" /> &nbsp &nbsp
                                                         
                                        </td>
                                    </tr>
                                    
                                    
                                </table>
                                                         
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
            </asp:Panel>
        </div>
          <!--modal solicitud aprobación documentos-->
         <div id="solicitud_aprobacion">
             <asp:Panel ID="Panel_solicitud_aprobacion" runat="server" Style="display:none; color: White; width: 100%; height: 100%" CssClass="modal_content_general">
 
                 <asp:ModalPopupExtender ID="ModalPopupExtender_solicitud_aprobacion" runat="server" BehaviorID="Panel_solicitud_aprobacion_ModalPopupExtender" TargetControlID="ButtonSalir_solicitud_aprobacion"
                     CancelControlID="Button_cerrar_solicitud_aprobacion" PopupControlID="Panel_solicitud_aprobacion"  >
                 </asp:ModalPopupExtender>
                 <div id="divcabecer2_solicitud_aprobacion" class="modal_title_superior">
                    
                     <asp:Label ID="Label_solicitud_aprobacion" runat="server" Text="Solicitud aprobación documentos de respuesta" Font-Size="10" Style="float: left">
                     </asp:Label>
                     <div id="Divcerrarbuton2_solicitud_aprobacion" style="float: right">
                         <asp:Button ID="Button_cerrar_solicitud_aprobacion" runat="Server" Text="X" CssClass="modal_boton_hiden"
                              ToolTip="Cerrar ventana" />
                     </div>
                 </div>
                 <div id="contenido_procesa_solicitud_aprobacion" style="background-color:white; width:100%; height: 100%" class="modal_content_back">
                     <asp:UpdatePanel ID="UpdatePanel_solicitud_aprobacion" runat="server" UpdateMode="Conditional">
                         <ContentTemplate>
 
                             <iframe Style="color: White; width: 100%; background-color:white; height: 100%; overflow:hidden" id="Iframe_solicitud_aprobacion"  frameborder="0" runat="server"  ></iframe>
                              
                         </ContentTemplate>
                     </asp:UpdatePanel>
                      <asp:Button ID="Button_solicitud_aprobacion" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                     <asp:Button ID="ButtonSalir_solicitud_aprobacion" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                 </div>
             </asp:Panel>
        </div>
          <!--Mensaje popup para paginas externas libres-->
          <div style="">
              <asp:Panel ID="PanelLibre" runat="server" Style="display:none; color: White; width: 70%; height: auto" CssClass="modal_content_general">
                  <asp:ModalPopupExtender ID="ModalPopupExtenderLibre" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonD"
                      PopupControlID="PanelLibre" CancelControlID="Buttoncabcel">
                  </asp:ModalPopupExtender>
                  <div id="Div6" class="modal_title_superior">        
                      <asp:Label ID="Labeladver" runat="server" Text="Notas solicitud" Font-Size="10" Style="float: left">         
                      </asp:Label>
                      <div id="Div7" style="float: right">
                          <asp:Button ID="Buttoncabcel" runat="Server" Text="X" CssClass="modal_boton_hiden"
                               ToolTip="Cerrar ventana" />
                      </div>
                  </div>
                  <div id="Div9" style=" background-color: #FFFFFF; height:100%; width: 100%" class="modal_content_back">
                      <asp:UpdatePanel ID="UpdatePanelLibre" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframelibre_notas_general_" runat="server" frameborder="0"  scrolling="no" style="width:100%" ></iframe>
                          </ContentTemplate>
                      </asp:UpdatePanel>
                  </div>
                  <asp:Button ID="ButtonD"  runat="server" Text="Button" Height="20px" Width="20px" style="display:none" />
              </asp:Panel>
          </div>
         <asp:Panel ID="Panel_usu_rel_solicitud" runat="server" Style="display:none; color: White; width:70%; height:auto" CssClass="modal_content_general" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_usu_rel_solicitud" runat="server" BehaviorID="Panel_usu_rel_solicitud_ModalPopupExtender" 
                     TargetControlID="ButtonSalir_usu_rel_solicitud" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_usu_rel_solicitud" PopupControlID="Panel_usu_rel_solicitud" ></asp:ModalPopupExtender>
                <div id="div4" class="modal_title_superior">              
                    <asp:Label ID="Label_usu_rel_solicitud" runat="server" Text="Reversa respuesta" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_usu_rel_solicitud" style="float: right">
                        <asp:Button ID="Button_cerrar_usu_rel_solicitud" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_usu_rel_solicitud" style="background-color: white; width: 100%; height:100%" class="modal_content_back">         
                    <div id="div_contenedor_titulo_documentos_relacionados" style="width: 100%; position: inherit; left: auto; 
                     text-align: left; font-family: Arial; font-size: 16px; font-weight: 600; text-align: center; background-color: #f5f5f5">
                        <asp:Label ID="Label_relacion_solicitudes" runat="server" Text="Usuarios relacionados a la solicitud de aprobación"></asp:Label>
                    </div>
                    <input id="HiddenEmailconsulta" type="hidden" value="" runat="server">
                    <asp:UpdatePanel ID="UpdateGeneral_documentos" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                             <asp:Button ID="Button_listar_usuarios_relacionados_solicitud" runat="server" Text="Button" Style="display:none" />
                            <input id="hdnEmailID_documentos" type="hidden" value="0" runat="server">
                            <input id="hdnEmailID_VAL_documentos" type="hidden" value="0" runat="server">
                            <input id="Hidden_id_usuarios_sel" type="hidden" value="0" runat="server">
                            <input id="HiddenEmailconsulta_documentos" type="hidden" value="" runat="server">
                            <div id="contenido_titulo_val_radicacion_documentos" style="height:auto; width: 99%;margin-top:1px; margin-left:2px">
                                <asp:Label ID="Label_estado_documentos" runat="server" ForeColor="Black" Font-Size="9px" Style="float: right; display:none"></asp:Label>
                                <asp:Label ID="titulo_label_expedientes_documentos" runat="server" ForeColor="Black" Font-Size="12px">Resultados busqueda</asp:Label>
                            </div>
                            <div id="content_data_grid" class="conten_gred_border">
                                <asp:Panel ID="Panelactividad_documentos" runat="server" Wrap="False"
                                  style="height:98%" >
                                <asp:GridView ID="data_grid_documentos" runat="server" Style="position: inherit;  margin-left:2px; width:99%"
                                       AutoGenerateSelectButton ="False" CssClass="filtrar" GridLines="None" Font-Size="12px">
				                                <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
				                                 <HeaderStyle CssClass="GridviewScrollHeader_line_blanco" />
                                                <RowStyle CssClass="GridviewScrollItem_line" />
                                                 <PagerStyle CssClass="GridviewScrollPager_line" />
                                     <Columns>
                                        <asp:BoundField HeaderText="" />                               
                                    </Columns>                  
                                </asp:GridView>
                            </asp:Panel>
                            </div>
                            

                        </ContentTemplate>

                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>    
                <div id="div_expediente_seleccionado" style="width: 100%; font-family: Arial; font-size: 11px;margin-top:1px; height:10%">
                    <asp:UpdatePanel ID="UpdatePanel_expediente_seleccionado" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>                      
                            <input id="Hidden_resultado_aprobacion" type="hidden" value="" runat="server">
                            <input id="Hidden_actualizacion_general" type="hidden" value="" runat="server">
                            <input id="Hidden_actualizacion_usuario" type="hidden" value="" runat="server">
                            <asp:Button ID="Button_estado_solicitud" runat="server" Text="Listar notas del usuario" ToolTip="listar las notas a la solicitud de aprobación del usuario seleccionado en la lista"  CssClass="boton_azul" style="margin-top:5px; display:none"/> 
                            <asp:Button ID="Button_documentos_correccion" runat="server" Text="Listar anexos de corrección" CssClass="boton_azul" ToolTip="Listar los anexos de corrección del usuario seleccionado en la lista" style="margin-top:5px ; display:none" />  
                            <asp:Button ID="Button_archiva_solicitud" runat="server" Text="Archiva la solictud" ToolTip="Archiva la solictud de aprobación del usuario que seleccione en la lista"  CssClass="boton_azul" style="margin-top:5px ; display:none"/> 
                            <asp:Button ID="Button_notifica_solicitud_usuario_correo" runat="server" Text="Notificar al correo electrónico" ToolTip="Notificar al correo electrónico la solcitud de aprobación al usuario que seleccione en la lista"  CssClass="boton_azul" style="display:none" />
                            <asp:Button ID="Button_nuevo_integrante" runat="server" Text="Nuevo usuario" ToolTip="Relacionar un nuevo integrante a la solicitud de aprobación" CssClass="boton_azul" style="float:right; margin: 3px 3px 3px 3px; display:none"/>
                           
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
                          <asp:Button ID="Button_usu_rel_solicitud" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                          <asp:Button ID="ButtonSalir_usu_rel_solicitud" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                </div>
            </asp:Panel>
          <!--mensaje_personalizado-->
	            <asp:Panel ID="Panel_mensaje_personalizado" runat="server" Style="display:none; color: White; width: 400px; height: 150px">
                    <asp:ModalPopupExtender ID="ModalPopupExtender_mensaje_personalizado" runat="server"
                        TargetControlID="Button_mensaje_personalizado" BackgroundCssClass="FondoAplicacion"
                        CancelControlID="Button_cerrar_mensaje_personalizado" PopupControlID="Panel_mensaje_personalizado">
                    </asp:ModalPopupExtender>
                    <div id="div_persoanlizado" class="cabecera2">
                        <asp:Label ID="Label_mensaje_personalizado_" runat="server" Text="Mensaje de servidor" Font-Size="10" Style="float: left; font-family: Arial; margin-left: 5px; margin-top: 2px">
                        </asp:Label>
                        <div id="Divcerrarbuton2_mensaje_personalizado" style="float: right">
                            <asp:Button ID="Button_cerrar_mensaje_personalizado" runat="Server" Text="X"
                                ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                        </div>
                    </div>
                    <div id="contenido_procesa_mensaje_personalizado" style="background-color: white; width: 100%; height: 99%; border: thin double #000080; color: black; background-color: #FFFFFF">
                        <br />
                        <div style="height: 60%; float: left; width: 50px">
                            <asp:Label ID="Label_estil" runat="server" Text="&#9888;" Style="font-family: Arial; font-size: 40px; color: black; margin-top: 60px; margin-left: 10px"></asp:Label>
                        </div>
                        <div style="height: 60%; overflow: auto; float: right; width: 330px; margin-right: 10px; text-align: center">
                            <br />
                            <asp:Label ID="Label_mensaje_personalizado" runat="server" Text="Detalle" Style="font-family: Arial; font-size: 11px; color: black; padding-top: 30px; padding-left: 1px; padding-right: 10px; margin-right: 30px; font-weight: 500"></asp:Label>
                        </div>
                        <asp:Button ID="Button_mensaje_personalizado" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                        <asp:Button ID="ButtonSalir_mensaje_personalizado" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    </div>
	            </asp:Panel>
        <!--Termina mensaje_personalizado-->
                <div id="tol_pie" style=" float:right;  background-color:#E7EDF5; width:100%; height:3%;border-style: ridge; border-bottom-width: 0.5px; border-left-width: 1px; border-right-width: 1px; border-top-width: 1px;text-align:center; display:none"">
                 <asp:Label ID="Label5" runat="server" Text="Estado" style="font-family:Arial;font-size:11px; display:none"></asp:Label>
                    <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional" >
                            <ContentTemplate>
                                  <iframe runat="server" style="float:left" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                            frameborder="0" />
                            </ContentTemplate>
                           
                 </asp:UpdatePanel>
             </div>
    </form>
</body>
</html>
