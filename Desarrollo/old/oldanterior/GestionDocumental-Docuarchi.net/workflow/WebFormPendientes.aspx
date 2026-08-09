<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormPendientes.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormPendientes" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../js/ui/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
   <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script defer src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet">
  <link href="../Awesome/css/brands.css" rel="stylesheet">
  <link href="../Awesome/css/solid.css" rel="stylesheet">
    <script defer src="../Awesome/js/brands.js"></script>
  <script defer src="../Awesome/js/solid.js"></script>
  <script defer src="../Awesome/js/fontawesome.js"></script>
    <script src="../js/Filtrar.js"></script>
    <script src="../js/workflow/WebFormPendientes.js"></script>
    <script src="../Fixed-Header-Table-master/gridviewScroll.min.js"></script>
   <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/java_general/general_code_java.js"></script>
   <style type="text/css">
  
        .invisible { 
            visibility: hidden; 
        } 
    </style>
    <script accesskey="javascript" type="text/javascript">
           
    </script>
</head>
<body style="overflow:hidden">
    <form id="form1" runat="server">
     
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
                elment_postbak = args.get_postBackElement();
                posicion_update_pogres('progres_bar');
            }
            function CheckStatus(sender, args) {
                try {
                //progres_hiden('progres_bar');
                    //ELIMINA REGISTRO PENDIENTE DEL GREDVIEW ImageButtonterminar btnOkpagina
                if (elment_postbak.id == "btnOkpagina") {
                    elimina_registro_gred_pendiente();
                    document.getElementById("HiddenPROMP").value = "0";
                   // if (document.getElementById("Hidden_vi_reasigna").value == "1") {
                    //    document.getElementById("btnOkpagina").style.visibility = 'hidden';
                    //    document.getElementById("ButtonReasignarTerminar").style.visibility = 'visible';
                    //} else {
                    //    document.getElementById("btnOkpagina").style.visibility = 'visible';
                    //    document.getElementById("ButtonReasignarTerminar").style.visibility = 'hidden';
                    //}
                    //auto_zise_popup_pendinetes();
                    if (document.getElementById("Hidden_vi_reasigna").value == "1") {

                        if (document.getElementById("btnOkpagina")) {
                            document.getElementById("btnOkpagina").style.visibility = 'hidden';
                        }
                        if (document.getElementById("ButtonReasignarTerminar")) {
                            document.getElementById("ButtonReasignarTerminar").style.visibility = 'visible';
                        }


                    } else {
                        
                        if (document.getElementById("btnOkpagina")) { document.getElementById("btnOkpagina").style.visibility = 'visible'; }
                        if (document.getElementById("ButtonReasignarTerminar")) { document.getElementById("ButtonReasignarTerminar").style.visibility = 'hidden'; }

                    }
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
                if (elment_postbak.id == "ImageButtonEnviarUsuario") {
                    if (document.getElementById("Hidden_vi_reasigna").value == "1") {
                        if (document.getElementById("btnOkpagina")) {
                            document.getElementById("btnOkpagina").style.visibility = 'hidden';
                        }
                        if (document.getElementById("ButtonReasignarTerminar")) {
                            document.getElementById("ButtonReasignarTerminar").style.visibility = 'visible';
                        }
    
                    } else {
                        if (document.getElementById("btnOkpagina")) { document.getElementById("btnOkpagina").style.visibility = 'visible'; }
                        if (document.getElementById("ButtonReasignarTerminar")) { document.getElementById("ButtonReasignarTerminar").style.visibility = 'hidden'; }
                        
                    }
                    document.getElementById("Labeletiqueta").innerHTML = "Enviar la tarea seleccionada al usuario a seleccionar de la lista";
                    auto_zise_popup_envia_usuario_grupo();
                }
                if (elment_postbak.id == "ImageButtonEnviaActividad") {
                    document.getElementById("Labeletiqueta").innerHTML = "Enviar la tarea seleccionada al grupo o actividad a seleccionar de la lista";
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
                //Button_actualiza_pendiente
                if (elment_postbak.id == "Button_actualiza_pendiente" && document.getElementById("Hidden_Resultado_actualiza").value == "YES") {
                    actualiza_gred('DETALLEPENDIENTE', document.getElementById("contenidobusqueda_actualiza").value);
                    document.getElementById("Hidden_Resultado_actualiza").value == "";
                }
                //ENVIAR ACTIVIDAD FLUJO 
                if (elment_postbak.id == "ImageButtonterminar") {
                    if (document.getElementById("Hidden_lista_ruta_flujo").value == "F") {
                        auto_zise_popup_lista_tareas("1");
                    }
                    if (document.getElementById("Hidden_lista_ruta_flujo").value == "R") {
                        auto_zise_popup_lista_tareas_ruta("1");
                    }
                }
                if (elment_postbak.id == "Button_buscar_lista") {
                    busqueda_gred('Hidden_id_actividad_flujo', 'data_grid', 'TextBox_busqueda_', 'CheckBox_busqueda');
                }
                //
                if (elment_postbak.id == "Button_busqueda_actividad") {
                    busqueda_gred('Hidden_id_actividad_ruta', 'data_grid_actividad', 'TextBox_busqueda_actividad', 'CheckBox_busqueda_actividad');
                }
                if (elment_postbak.id == "Button_activa_enviar_actividad_ruta") {
                    if (document.getElementById("Hidden_res_evi").value == "YES") {
                        document.getElementById("Hidden_res_evi").value = "";
                        eliminar_fila_data_gred('GridViewlista', 'Hidden_id');
                        document.getElementById("Hidden_id_actividad_ruta").value = "";
                        document.getElementById("Hidden_id").value = "-1";
                        
                    }
                }
                //btnOkpagina
                if (elment_postbak.id == "btnOkpagina") {
                    if (document.getElementById("Hidden_res_envi").value == "YES") {
                        document.getElementById("Hidden_res_envi").value = "";
                        eliminar_fila_data_gred('GridViewlista', 'Hidden_id');
                        document.getElementById("Hidden_id_actividad_ruta").value = "";
                        document.getElementById("Hidden_id").value = "-1";

                    }
                }
                    //ButtonReasignarTerminar
                if (elment_postbak.id == "ButtonReasignarTerminar") {
                    if (document.getElementById("Hidden_res_envi").value == "YES") {
                        document.getElementById("Hidden_res_envi").value = "";
                        eliminar_fila_data_gred('GridViewlista', 'Hidden_id');
                        document.getElementById("Hidden_id_actividad_ruta").value = "";
                        document.getElementById("Hidden_id").value = "-1";

                    }
                }
                if (elment_postbak.id == "Button_autoriza_reasignacion") {
                    if (document.getElementById("Hidden_resp_envio").value == "YES") {
                        document.getElementById("Hidden_resp_envio").value = "";
                        eliminar_fila_data_gred('GridViewlista', 'Hidden_id');
                        document.getElementById("Hidden_id_actividad_ruta").value = "";
                        document.getElementById("Hidden_id").value = "-1";

                    }
                }
                    //Button_activa_enviar_actividad_flujo_trabajo
                if (elment_postbak.id == "Button_activa_enviar_actividad_flujo_trabajo") {
                    if (document.getElementById("Hidden_result_envi_flujo").value == "YES") {
                        document.getElementById("Hidden_result_envi_flujo").value = "";
                        eliminar_fila_data_gred('GridViewlista', 'Hidden_id');
                        document.getElementById("Hidden_id_actividad_ruta").value = "";
                        document.getElementById("Hidden_id").value = "-1";

                    }
                }
                    //Reasigna actividad flujo trabajo 
                if (elment_postbak.id == "Button_autoriza_reasignacion_flujo") {
                    if (document.getElementById("Hidden_resp_envio_flujo").value == "YES") {
                        document.getElementById("Hidden_resp_envio_flujo").value = "";
                        eliminar_fila_data_gred('GridViewlista', 'Hidden_id');
                        document.getElementById("Hidden_id_actividad_ruta").value = "";
                        document.getElementById("Hidden_id").value = "-1";

                    }
                }

                }
                catch (err) {
                    alert(" Funcion CheckStatus asincrona " + err.message);
                }
                finally {
                    progres_hiden('progres_bar');
                }
            }

            </script>
        
       
             <div id="contenido_desicion" style="width: 99%; height: 10%; overflow:auto" class="border_superior_radius">
            
                 <asp:UpdatePanel ID="updata_panel_pendiente" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:ImageButton ID="ImageButtonEnviarUsuario" runat="server" ImageUrl="../workflow/imageneswf/envia_usuario.jpg"
                        ToolTip="Enviar tareas a usuario" AlternateText="Renviar a Usuario"
                        ImageAlign="Left" Width="120px" Height="25px" Style="margin-left: 0px; margin-top: 0px; display: none" CssClass="alterna_image" OnClientClick="asigna_usuario_grupos_cheked(); auto_zise_popup_envia_usuario_grupo();" />
                    <asp:ImageButton ID="ImageButtonEnviaActividad" runat="server" ImageUrl="../workflow/imageneswf/enviar_actividad.jpg"
                        ToolTip="Enviar tareas a Grupos" AlternateText="Reenviar a actividad de grupo"
                        ImageAlign="Left" Width="130px" Height="25px" Style="margin-left: 0px; margin-top: 0px; display: none" CssClass="alterna_image" OnClientClick="asigna_usuario_grupos_cheked(); auto_zise_popup_envia_usuario_grupo();" />
                    <asp:ImageButton ID="ImageButtonterminar" runat="server" ImageUrl="../workflow/imageneswf/terminar.jpg"
                        ToolTip="Terminar tarea" AlternateText="Enviar tarea a"
                        ImageAlign="Left" Width="100px" Height="25px" Style="margin-left: 0px; margin-top: 0px; display:none" CssClass="alterna_image" />
                    <input id="Hidden_vi_reasigna" type="hidden" value="" runat="server">
                    <input id="Hidden_lista_ruta_flujo" type="hidden" value="" runat="server">
                    <a class="boton_gres_image" style="margin-left: 10px; margin: 5px; width: 200px; float: left" title="Reenviar tarea a usuario especifico" href="#" onclick="activa_boton_client_server('ImageButtonEnviarUsuario')"><i class="fas fa-user"></i> Enviar tarea a usuario </a>
                    <a class="boton_gres_image" style="margin-left: 10px; margin: 5px; width: 200px; float: left" title="Enviar tarea a actividad de usuario" href="#" onclick="activa_boton_client_server('ImageButtonEnviaActividad')"><i class="fas fa-users"></i> Enviar tarea a actividad </a>
                    <a id="boton_heig" class="boton_gres_image" style="margin-left: 10px; margin: 5px; width: 200px; float: left" title="Terminar tarea por flujo o ruta de trabajo" href="#" onclick="activa_boton_client_server('ImageButtonterminar')"><i class="fas fa-project-diagram"></i> Enviar tarea a </a>
                    <asp:DropDownList ID="DropDownActividades" Style="margin-left: 0px; margin-top: 0px; display: none"
                        runat="server" Height="25px"
                        Width="226px" ImageAlign="Left">
                    </asp:DropDownList>
                    <asp:Button ID="Button_visor_emergente" runat="server" Text="Button" Style="display: none" />
                </ContentTemplate>
            </asp:UpdatePanel>
            
           

        </div>
        <div id="Lista" style="width:99%; float: left; height: 70%; margin: 0px 2px 1px 1px; margin-top: 1px; border-color: #b0c4de; border-style: ridge; border-width: 1px; position: relative">
            <asp:UpdatePanel ID="UpdatePanelmensaje" runat="server" UpdateMode="Conditional" RenderMode="Inline"  >
                <ContentTemplate>               
                        <asp:GridView ID="GridViewlista" runat="server" Font-Size="7.9pt" EnableViewState="false"
                            AutoGenerateSelectButton="False" CssClass="filtrar" style="width:99.5%" GridLines="None">
                              <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                              <HeaderStyle CssClass="GridviewScrollHeader_line_blanco" />
                              <RowStyle CssClass="GridviewScrollItem_line" />
                              <PagerStyle CssClass="GridviewScrollPager_line" />
                                
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                           
                        </asp:GridView>
                        
                </ContentTemplate>
                <Triggers>
       
                </Triggers>
            </asp:UpdatePanel>

        </div>                  
        <div id="buton" style="width: 99%; float: left; height: 20%; margin: 2px 1px 1px 1px;  overflow:auto" >           
            <asp:UpdatePanel ID="Upadatepanel_botnoes" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                     <asp:Button ID="ButtonFiltrar" runat="server" Text="Fltrar " ToolTip="Filtrar tareas" CssClass="boton_blanco" Style="border-color: #b0c4de;margin-left: 5px; font-size:11px;width:50px;height:30px; display:none" />
                    <asp:Button ID="Button_actualiza_datos" runat="server" Text="Actualiza" ToolTip="Actualizar datos identificacion pendiente" CssClass="boton_blanco" Style=" border-color: #b0c4de;margin-left: 3px; font-size:11px; display:none" Height="30px" Width="80px" />
                     <asp:Button ID="Button_sacar_pendiente" runat="server" Text="Asignar tarea desde pendiente " ToolTip="Asignar tarea desde pendientes" CssClass="boton_blanco" Style=" border-color: #b0c4de;margin-left: 3px; font-size:11px;height:30px; width:200px; color:black; display:none" OnClientClick="asignar_tarea_pendiente();" />
                    <asp:Button ID="ButtonSubir" CssClass="boton_blanco" runat="server" Text="Enviar tarea a pendiente  " ToolTip="Subir tareas a pendientes" Style="font-size:11px;height:30px; width:200px; display:none"  />
                    <asp:Button ID="ButtonUnir" runat="server" Text="Unir Tareas " ToolTip="Buscar tareas en pendiente" CssClass="boton" Style="margin-left: 5px; display: none" />
                    <asp:TextBox ID="contenidobusqueda" runat="server" style="left: 10px; width: 200px; margin-left: 5px; margin-top:5px"   ></asp:TextBox>
                    <asp:Button ID="Buttonbuscar" runat="server" Text="Buscar" style="height:30px;  border-color: #b0c4de; display:none" class="boton" ToolTip="Buscar tareas en la lista" CssClass="boton_blanco"  OnClientClick="busqueda_gred('Hidden_id','GridViewlista','contenidobusqueda','CheckboxBusqueda');" />
                      <a class="boton_azul" style="width:70px"  title="Buscar tareas en la lista" href="#"  onclick="activa_boton_client_server('Buttonbuscar')"><i class="fas fa-search"></i> Buscar </a> 
                    <asp:CheckBox ID="CheckboxBusqueda" runat="server" Font-Names="arial" Font-Size="10"  ToolTip="Sólo palabras completas"/>
                    <label style="font-family: Arial; font-size: 10px" >SLP</label>
                    &nbsp
           
                     <a class="boton_azul" Style="border-color: #b0c4de;margin-left: 5px; font-size:11px;width:70px"  title="Filtrar tareas" href="#"  onclick="activa_boton_client_server('ButtonFiltrar')"><i class="fas fa-filter"></i> Fltrar </a>     
                    &nbsp
            
                    <a class="boton_azul" Style="margin-left: 3px; font-size:11px; width:80px"  title="Actualizar datos de identificación pendiente" href="#"  onclick="activa_boton_client_server('Button_actualiza_datos')"><i class="fas fa-edit"></i> Actualiza </a>     
                    &nbsp
                   <a class="boton_azul" Style="font-size:11px; width:200px" title="Asignar tarea desde pendientes" href="#"  onclick="activa_boton_client_server('Button_sacar_pendiente')"><i class="fas fa-arrow-circle-down"></i> Asignar tarea desde pendiente </a>     
                    &nbsp
                  <a id="heig_bot" class="boton_azul" Style="font-size:11px; width:200px" title="Subir tareas a pendientes" href="#"  onclick="activa_boton_client_server('ButtonSubir')"><i class="fas fa-arrow-circle-up"></i> Enviar tarea a pendiente </a>     
                </ContentTemplate>
            </asp:UpdatePanel>
           
        </div>
    
       <INPUT id="hdnEmailID" type="hidden" value="0" runat="server" >
        <INPUT id="hdnEmailID_sel" type="hidden" value="0" runat="server" >
        <INPUT id="Hidden_id" type="hidden" value="-1" runat="server" >
        <INPUT id="HiddenPROMP" type="hidden" value="0" runat="server" >
        <input id="Hidden_id_tarea_sel" type="hidden" value="-1" runat="server">
        <input id="Hidden_tipo_visor" type="hidden" value="" runat="server">
        <input id="Hidden_seleccion" type="hidden" value="YES" runat="server">
         <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 50px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>
    <!--codigo cuadro de dialogo-->
   <input id="HiddenFiltro" type="hidden" value="" runat="server">
     <div id="framemensaje">          
                <asp:Panel ID="Panelmensaj" runat="server"  style = "display:none; width:40%; height:auto" ForeColor="White"   CssClass="modal_content_general" >
                    <asp:ModalPopupExtender ID="ModalPopupTexto" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button4"
                        PopupControlID="Panelmensaj" >
                    </asp:ModalPopupExtender> 
                    <div id="Div2" class="modal_title_superior">             
                        <asp:Label ID="Label2" runat="server" Text="Enviar tarea a pendientes" Font-Size="10" Style="float: left; font-family:Arial">
                        </asp:Label>
                        <div id="Divcerrarbuton2_reversa_respuesta" style="float: right">
                           
                        </div>
                       
                    </div> 
                                
                    <div id="container" style=" color: White; background-color: #FFFFFF; height:auto; width:auto" class="modal_content_back">
                        <div id="Contenido" style="height:auto; text-align:center">
                            <br />
                            
                            <label id="Lableme" style="font-size: 14px; color: #000000" title="" />
                            <i class="fas fa-tasks"></i>
                            <asp:Label ID="LabelMensaje" runat="server" Text="Identificacion de la tarea en pendientes" ForeColor="Black"  Visible="True" style="font-family:Arial; margin-bottom:10px; font-size:14px; font-weight:bold" />
                            
                            <br />
                            <asp:TextBox ID="TextBoxdatos" runat="server" Width="70%" Style="margin-top:10px">
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
                                <div id="Contenidbuton" style="height:auto; color: White; background-color: #FFFFFF; text-align:center">
                                    <br />
                                    <asp:Button ID="btnOkay" runat="server" Text="Aceptar "  CssClass="boton_azul" />
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancelar " CssClass="boton_azul"/>
                                    <br />
                                   <br />
                                    <input id="Hidden_resultado" type="hidden" value="" runat="server">
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                        <div id="Div3" style="height: 10%; color: White; background-color: #FFFFFF;">
                             <asp:Button ID="Button4" runat="Server" Text="X" style="display:none"
                                 />
                        </div>
                    </div>
                
                    
                
                </asp:Panel> 
       </div>
       
     <div id="Filtro">
              <asp:Panel ID="Panel_filtro" runat="server" Style="display:none; color: White; width: auto; height: auto" CssClass="modal_content_general">
                 
                  <asp:ModalPopupExtender ID="ModalPopupExtender_Filtro" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Buttond_Filtro"
                      PopupControlID="Panel_filtro" CancelControlID="Button_Filtro_Cerrar">
                  </asp:ModalPopupExtender>
                  <div id="divcabecer_filtro" class="modal_title_superior">
                     
                      <asp:Label ID="Label1" runat="server" Text="Filtrar Lista" Font-Size="10" Style="float: left; font-family:Arial">
                      </asp:Label>
                      <div id="Divcerrarbuton_filtro" style="float: right">
                          <asp:Button ID="Button_Filtro_Cerrar" runat="Server" Text="X"  CssClass="modal_boton_hiden"
                               ToolTip="Cerrar ventana" />
                      </div>
                  </div>
                
                  <div id="Diupdate_filtro" style=" color: White; background-color: #FFFFFF; height: auto; width: auto" class="modal_content_back">
                      <div id="Contenidopagina_filtro" style="height: 140px; width: 450px; overflow: no-display; color: black; margin-left: 15px">
                          <asp:UpdatePanel ID="updata_panel_filtro" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <br />
                                
                                  <br />
                                  
                                  <asp:TextBox ID="contenidobusqueda_filtro" runat="server" placeholder="Didigita filtro" style="margin-left:5px; width:240px"></asp:TextBox>
                                  <asp:Button ID="ButtonFiltro"  Text="Aceptar" runat="server" class="boton_azul" />
                                  <asp:CheckBox ID="CheckboxBusqueda_f" runat="server" Text="SLP" ToolTip="Sólo palabras completas" Font-Size="10" Font-Names="arial"  />
                                  <br />
                              </ContentTemplate>
                          </asp:UpdatePanel>
                           <asp:Button ID="Buttond_Filtro" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                      </div>

                  </div>
                 
                 
              </asp:Panel>
          </div>
        
      <!--Mensaje popup para paginas externas enviar tarea usuario, enviar tarea a grupo-->
      <div id="envioactividad"   >    
              <asp:Panel ID="Panelpagina" runat="server" Style="display:none; color: White; width: 100%; height: 100%;  margin-top:1px" CssClass="modal_content_general">
               
                  <div id="Divcab" class="modal_title_superior">
                      <asp:ModalPopupExtender ID="ModalPopupExtendermesjpagina" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Buttonhide_full"
                          PopupControlID="Panelpagina" CancelControlID="Button_Cerrar" ></asp:ModalPopupExtender>
                       
                      <asp:Label ID="Labeletiqueta" runat="server" Text="Enviar tarea" Font-Size="10"></asp:Label>
                     
                      <div id="Divlabel" style="float: right">
                          
                          <asp:Button ID="Button_Cerrar" runat="Server" ToolTip="Cerrar ventana" Text="X" CssClass="modal_boton_hiden"/>
                          
                      </div>
                      <br />
                  </div>

                  <div id="DivColorPagina" style="color: White; background-color: #FFFFFF; height: 80%; width: 100%" class="modal_content_back">

                          <asp:UpdatePanel ID="UpdatePanelpagina" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                              <ContentTemplate>

                                  <iframe id="frameeditexpanse_" runat="server" frameborder="0"  scrolling="no" style="width:100%; height:100%"></iframe>

                              </ContentTemplate>
                          </asp:UpdatePanel>
                             <asp:Button ID="Buttonhide_full" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                  </div>
                  <div id="DivBotones" style="height:20%; margin-top:1px; background-color:white; color:black; background-color: #FFFFFF" >
                      <asp:UpdatePanel ID="Updatecondiciona" runat="Server" UpdateMode="Conditional" RenderMode="Inline">
                          <ContentTemplate>
                           <asp:Button ID="btnCancelpagina" runat="server" Text="Cancelar " Style="float: right; margin-right: 10px; margin-left: 10px; margin-top: 1px" CssClass="boton_blanco" />
                               &nbsp
                              <asp:Button ID="ButtonReasignarTerminar" runat="server" Text="Reasignar " Style="float: right; margin-left: 10px; margin-top: 1px" OnClientClick="confirma_respuesta(&quot;Desea terminar y reasignar la tarea&quot;);" CssClass="boton_azul" ToolTip="Reasigna y envia la tarea al usuario seleccionado" />           
                               &nbsp
                          <asp:Button ID="btnOkpagina" runat="server" Text="Aceptar " Style="float: right; margin-left: 10px; margin-top: 1px" OnClientClick="confirma_respuesta('Desea enviar las tareas seleccionadas');" CssClass="boton_azul" />
                          &nbsp <asp:CheckBox ID="CheckBox_noti_envio" runat="server" Text="Notifica envío a correo electrónico" Style="float: right; margin-right: 10px; margin-left: 10px; margin-top: 1px; font-family:Arial;font-size:11px" />
                              <input id="Hiddenseltareas" type="hidden" value="0" runat="server">
                               <input id="Hidden_res_envi" type="hidden" value="" runat="server">
                              <input id="Hidden_lista_eliminar_tarea" type="hidden" value="0" runat="server">
                          </ContentTemplate>
                      </asp:UpdatePanel>
                     
                     
                    </div>
                   
              </asp:Panel>
      
       </div>
        <!--Popup visor externo-->
               <asp:Panel ID="Panel_visor_externo" runat="server" Style="display:none; overflow:hidden" ForeColor="White" Width="99%" Height="100% " CssClass="modal_content_general">
                  <asp:ModalPopupExtender ID="ModalPopupExtender_visor_externo" runat="Server" Y="1" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_visor_externo"
                      PopupControlID="Panel_visor_externo"  CancelControlID="ButtonSalir_visor_externo">
                  </asp:ModalPopupExtender>
                  <div id="Cabecerapendiente_visor_externo" class="modal_title_superior">
                      
                      <asp:Label ID="Label9" runat="server" Text="Visor documentos Workflow" Font-Size="10" style="font-family:Arial"></asp:Label>
                      <div id="Div_visor_externo" style="float: right">
                          <asp:Button ID="ButtonSalir_visor_externo" runat="Server" Text="X" CssClass="modal_boton_hiden"
                               ToolTip="Cerrar ventana" />

                      </div>
                  </div>
                  <div id="Cotenedorpendiente_visor_externo" style="color: Black; background-color: #FFFFFF; height: 90%; width: 100%; overflow:hidden" class="modal_content_back">
                  
                      <asp:UpdatePanel ID="UpdatePanel_visor_externo" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_visor_externo_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>

                      </asp:UpdatePanel>
                           <asp:Button ID="Button_visor_externo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" style="display:none" />
                  </div>
                  
              </asp:Panel>
      <!--Actualizar dato pendiente-->
        <asp:Panel ID="panel_actualiza_datos" runat="server" Style="display:none; color: White; width:auto; height:auto; margin-top: 1px" CssClass="modal_content_general">     
                <asp:ModalPopupExtender ID="ModalPopupExtenderactualiza" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_targ_actualiza"
                    PopupControlID="panel_actualiza_datos" CancelControlID="ButtonCerrarpendiente" >
                </asp:ModalPopupExtender>
             <div id="Divcabutil" class="modal_title_superior">
                <asp:Label ID="Label3" runat="server" Text="Actualiza" Font-Size="10"></asp:Label>
                <div id="Div1" style="float: right">
                    <asp:Button ID="ButtonCerrarpendiente" runat="Server" Text="X"  CssClass="modal_boton_hiden"
                         />

                </div>
                <div id="Divlabel_actualiza" style="float: right">
                    <asp:Button ID="Button_targ_actualiza" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                </div>
                <br />

            </div>
            <div id="Diupdate_actualiza" style="color: White; background-color: #FFFFFF; height: auto; width: auto" class="modal_content_back">
                <div id="Contenidopagina_actualiza" style="height:auto; width:auto; overflow: no-display; color: black; margin-left: 1px">
                    <asp:UpdatePanel ID="Updatepanael_Actualiza" runat="server" UpdateMode="Conditional" >
                        <ContentTemplate>
                            <br />
                            <label style="font-family: Arial; font-size: 12px; margin-left:7px">Digita nueva identificación de la tarea pendiente </label>
                            <br />
                            <br />
                            <asp:TextBox ID="contenidobusqueda_actualiza" runat="server" Style="min-width:250px; margin-left:5px" />
                            <asp:AutoCompleteExtender ID="auto_complete" runat="server"
                                TargetControlID="contenidobusqueda_actualiza" MinimumPrefixLength="2"
                                EnableCaching="true" CompletionSetCount="10" CompletionInterval="50"
                                ServiceMethod="GetPosiblesDatos" ServicePath="../webservice/WebServiceWorkflow.asmx"
                                ContextKey="datos_pendiente|tarea_pendiente" UseContextKey="True"
                                CompletionListCssClass="completionList" CompletionListHighlightedItemCssClass="itemHighlighted"
                                CompletionListItemCssClass="listItem" OnClientShown="onDataShown">
                            </asp:AutoCompleteExtender>
                            <asp:Button ID="Button_actualiza_pendiente" type="button" Text="Aceptar" runat="server" class="boton_azul" />
                            <input id="Hidden_Resultado_actualiza" type="hidden" value="" runat="server">
                            <br />
                             <br />
                            <br />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                   

                </div>
            </div>
        </asp:Panel>
         <!--autoriza reasignacion-->
          <div id="autoriza_reasignacion_tarea">
            <asp:Panel ID="Panel_autoriza_reasignacion_tarea" runat="server" Style="display:none; color: White; width: auto; height: auto" CssClass="modal_content_general_border">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_autoriza_reasignacion_tarea" runat="server" BehaviorID="Panel_autoriza_reasignacion_tarea" TargetControlID="ButtonSalir_autoriza_reasignacion_tarea" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_autoriza_reasignacion_tarea" PopupControlID="Panel_autoriza_reasignacion_tarea" ></asp:ModalPopupExtender>
                <div id="divcabecer2_autoriza_reasignacion_tarea" class="modal_title_superior">
                  
                    <asp:Label ID="Label_autoriza_reasignacion_tarea" runat="server" Text="Autoriza reasignación" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_autoriza_reasignacion_tarea" style="float: right">
                        <asp:Button ID="Button_cerrar_autoriza_reasignacion_tarea" runat="Server" Text="X" CssClass="modal_boton_hiden"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_autoriza_reasignacion_tarea" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF;" class="modal_content_back">
                                
                    
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
                                        <td style="text-align:right">
                                            <asp:Button ID="Button_autoriza_reasignacion" runat="server" Text="Reasignar" Style="margin-right:2px"  CssClass="boton_azul" /> 
                                                     <input id="Hidden_resp_envio" type="hidden" value="" runat="server">    
                                        </td>
                                    </tr>
                                    
                                    
                                </table>
                                                         
                            </ContentTemplate>
                        </asp:UpdatePanel>
                           <asp:Button ID="Button_autoriza_reasignacion_tarea" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                           <asp:Button ID="ButtonSalir_autoriza_reasignacion_tarea" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                </div>
            </asp:Panel>
        </div>
         <!--lista_actividades_ruta-->
          <div id="lista_actividades_ruta">
            <asp:Panel ID="Panel_lista_actividades_ruta" runat="server" Style="display:none; color: White; width: 70%; height:50%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_actividades_ruta"  runat="server" BehaviorID="Panel_lista_actividades_ruta" TargetControlID="ButtonSalir_lista_actividades_ruta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_lista_actividades_ruta" PopupControlID="Panel_lista_actividades_ruta" ></asp:ModalPopupExtender>
                <div id="divcabecer2_lista_actividades_ruta"  class="modal_title_superior"> 
                    <asp:Label ID="Label_lista_actividades_ruta" runat="server" Text="Envío de tareas por medio de ruta de trabajo" Font-Size="10" Style="float: left; font-family:Arial">                    
                    </asp:Label>
                    <div id="Divcerrarbuton2_lista_actividades_ruta" style="float: right">
                        <asp:Button ID="Button_cerrar_lista_actividades_ruta" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_lista_actividades_ruta" style="background-color: white; width: 100%; height:99%; color: black; background-color: #FFFFFF;" class="modal_content_back">                
                    <asp:UpdatePanel ID="UpdateGeneral_documentos_actividad" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <div id="contenido_titulo_data_grid_dos_title_actividad" style="width: auto; height: auto; margin-left: 3px; margin-right: 3px; margin-top: 3px; margin-bottom: 3px" class="border_superior_radius">
                                <asp:Label ID="titulo_label_grid_actividad" runat="server" ForeColor="Black" Font-Size="13px" Style="width: 100%; height: 31px; font-family: Arial; font-weight:bold">Resultados busqueda</asp:Label>
                                &nbsp &nbsp &nbsp &nbsp
                                <asp:Label ID="Label_title_nombre_ruta" runat="server" ForeColor="Black" Font-Size="13px" Style="width: 100%; height: 31px; font-family: Arial; font-weight:bold">La lista a continuación representa las actividades disponibles para envío de la tarea seleccionada a través de la ruta de trabajo</asp:Label>
                            </div>
                            <input id="Hidden_id_tar_sel" type="hidden" value="0" runat="server">
                            <input id="Hidden_id_actividad_ruta" type="hidden" value="" runat="server">
                            <input id="Hidden_nombre_actividad" type="hidden" value="" runat="server">
                            <input id="Hiddenestadoactividad" type="hidden" value="1" runat="server">
                            <div id="div_actividades_diponibles" style="text-align: center; display:none">
                                <asp:Label ID="Label_actividades_disponibles_envio" runat="server" Text="ACTIVIDADES DISPONIBLES PARA ENVIAR LA TAREA" Style="display:none"></asp:Label>
                            </div>
                            <div id="div_gred_actividad" style="margin-top: 1px; border-color: rgb(176, 196, 222); border-style: ridge; border-width: 1px; height: 320px; position: relative; overflow: auto; margin-left: 3px; margin-right: 3px; margin-top: 3px; margin-bottom: 3px">

                                <asp:GridView ID="data_grid_actividad" runat="server" style="font-family:Arial"
                                    AutoGenerateSelectButton="False" CssClass="filtrar" GridLines="None" Font-Size="12px">
                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <HeaderStyle CssClass="GridviewScrollHeader_line" />
                                    <RowStyle CssClass="GridviewScrollItem_line" />
                                    <PagerStyle CssClass="GridviewScrollPager_line" />
                                    <Columns>
                                        <asp:BoundField HeaderText="ITEN" />

                                    </Columns>
                                </asp:GridView>

                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                            <div id="div_contenido_procesa_lista_actividades_ruta_botones_desicion_actividad" style="width: auto; margin-top: 1px; position: relative; height:auto; display:block; margin-left:3px; margin-right:3px; margin-top:3px; margin-bottom:3px">
                        <asp:UpdatePanel ID="UpdatePanel_contendor_botones_desicion_actividad" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                
                                 <asp:CheckBox ID="CheckBox_busqueda_actividad" runat="server"  Text="Sólo palabras completas" Font-Names="arial"  Font-Size="6" />
                            &nbsp &nbsp <asp:TextBox ID="TextBox_busqueda_actividad" runat="server" Width="200px" Height="16px" style="margin-bottom:2px" ></asp:TextBox>
                             &nbsp <asp:Button ID="Button_busqueda_actividad" runat="server"   Text="Buscar" style=" font-size:11px"  CssClass="boton_azul"  />     
                            
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    </div>
   
                 <asp:Button ID="Button_lista_actividades_ruta" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_lista_actividades_ruta" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
              <asp:UpdatePanel ID="UpdatePanel_lista_actividades_ruta" runat="server" UpdateMode="Conditional">
                  <ContentTemplate>
                       <input id="Hidden_res_evi" type="hidden" value="" runat="server">
                      <asp:Button ID="Button_activa_enviar_actividad_ruta" runat="server" Text="" style="display:none"   />
                      <asp:Button ID="Button_detalle_enviar_actividad_ruta" runat="server" Text="" style="display:none"   />
                  </ContentTemplate>
              </asp:UpdatePanel>
        </div>
            <!--lista_actividades_worflow_ruta-->
          <div id="lista_actividades_worflow_ruta">
            <asp:Panel ID="Panel_lista_actividades_worflow_ruta" runat="server" Style="display:none; color: White; width: 70%; height:50%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_actividades_worflow_ruta"  runat="server" BehaviorID="Panel_lista_actividades_worflow_ruta" TargetControlID="ButtonSalir_lista_actividades_worflow_ruta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_lista_actividades_worflow_ruta" PopupControlID="Panel_lista_actividades_worflow_ruta" ></asp:ModalPopupExtender>
                <div id="divcabecer2_lista_actividades_worflow_ruta"  class="modal_title_superior"> 
                    <asp:Label ID="Label_lista_actividades_worflow_ruta" runat="server" Text="Envío de tareas por medio de flujo de trabajo" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_lista_actividades_worflow_ruta" style="float: right">
                        <asp:Button ID="Button_cerrar_lista_actividades_worflow_ruta" runat="Server" Text="X" CssClass="modal_boton_hiden"
                            ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_lista_actividades_workflow" style="background-color: white; width: 100%; height:99%; color: black; background-color: #FFFFFF;">                
                    <asp:UpdatePanel ID="UpdateGeneral_documentos" runat="server" UpdateMode="Conditional"  RenderMode="Inline">
                        <ContentTemplate>
                           <div id="contenido_titulo_data_grid_dos_title" style="width:auto; height:auto;margin-left: 3px; margin-right: 3px; margin-top: 3px; margin-bottom: 3px" class="border_superior_radius">
                                <asp:Label ID="titulo_label_grid" runat="server" ForeColor="Black" Font-Size="13px" style="width: 100%; height: 31px; text-align: center; font-weight:bold; font-family:Arial">Resultados busqueda</asp:Label>
                                &nbsp &nbsp &nbsp &nbsp
                                <asp:Label ID="Label_nombre_flujo" runat="server" ForeColor="Black" Font-Size="13px" Style="width: 100%; height: 31px; font-family: Arial; font-weight:bold">La lista a continuación representa las actividades disponibles para envío de la tarea seleccionada a través de la ruta de trabajo</asp:Label>
                            </div>
                            
                            <input id="HiddenEstado" type="hidden" value="1" runat="server">
                             <div id="div_actividades_disponibles_flujo" style="text-align: center; display:none" >
                                <asp:Label ID="Label_actividades_disponibles_flujo" runat="server" Text="ACTIVIDADES DISPONIBLES PARA ENVIAR LA TAREA"></asp:Label>
                            </div>
                            <div id="div_gred" Style="margin-top: 1px; border-color: rgb(176, 196, 222); border-style: ridge; border-width: 1px;height:320px; position:relative; overflow:auto; margin-left: 3px; margin-right: 3px; margin-top: 3px; margin-bottom: 3px">                                
                                    <asp:GridView ID="data_grid" runat="server"  style="font-family:Arial"
                                        AutoGenerateSelectButton="False" CssClass="filtrar" GridLines="None" Font-Size="12px">
                                        <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                        <HeaderStyle CssClass="GridviewScrollHeader_line" />
                                        <RowStyle CssClass="GridviewScrollItem_line" />
                                        <PagerStyle CssClass="GridviewScrollPager_line" />
                                        <Columns>
                                            <asp:BoundField HeaderText="ITEN" />
                                        </Columns>
                                    </asp:GridView>        
                            </div>                                     
                        </ContentTemplate>

                    </asp:UpdatePanel>
                     <div id="div_contenido_procesa_lista_actividades_worflow_ruta_botones_desicion" style="width: auto; margin-top: 1px; border-color: #b0c4de; border-width: 1px; border-style: ridge; position: relative; height:auto;  margin-left: 3px; margin-right: 3px; margin-top: 3px; margin-bottom: 3px">
                        <asp:UpdatePanel ID="UpdatePanel_contendor_botones_desicion" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_agrega_actividad_flujo_trabajo" runat="server" Text="Agregar actividad" CssClass="boton_azul" Style="margin-left: 5px; display:none" />
                                 <asp:CheckBox ID="CheckBox_busqueda" runat="server"  Text="Sólo palabras completas" Font-Names="arial"  Font-Size="6" />
                            &nbsp &nbsp <asp:TextBox ID="TextBox_busqueda_" runat="server" Width="200px" Height="16px" ></asp:TextBox>
                             &nbsp <asp:Button ID="Button_buscar_lista" runat="server" Width="65px"  Text="Buscar" style=" font-size:11px"  CssClass="boton_azul"  />     
                            <input id="Hidden_id_actividad_flujo" type="hidden" value="0" runat="server">
                            <input id="Hidden_id_flujo_trabjo" type="hidden" value="0" runat="server">
                            <input id="Hidden_id_actividad_destino" type="hidden" value="0" runat="server">
                            <input id="Hidden_id_usuario_workflow" type="hidden" value="0" runat="server">
                            <input id="Hidden_id_conector" type="hidden" value="0" runat="server">
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    </div>
   
                 <asp:Button ID="Button_lista_actividades_worflow_ruta" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_lista_actividades_worflow_ruta" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
              <asp:UpdatePanel ID="UpdatePanel_enviar_actividad" runat="server" UpdateMode="Conditional">
                  <ContentTemplate>
                      <input id="Hidden_result_envi_flujo" type="hidden" value="" runat="server">
                      <asp:Button ID="Button_activa_enviar_actividad_flujo_trabajo" runat="server" Text="" style="display:none"   />
                      <asp:Button ID="Button_detalle_enviar_actividad_flujo_trabajo" runat="server" Text="" style="display:none"   />
                  </ContentTemplate>
              </asp:UpdatePanel>
        </div>
            <!--envia_actividad_flujo_trabajo-->
          <div id="envia_actividad_flujo_trabjo">
            <asp:Panel ID="Panel_envia_actividad_flujo_trabjo" runat="server" Style="display:none; color: White; width:300px; height: 130px" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_envia_actividad_flujo_trabjo"  runat="server" BehaviorID="Panel_envia_actividad_flujo_trabjo" TargetControlID="ButtonSalir_envia_actividad_flujo_trabjo" BackgroundCssClass="FondoAplicacion" 
                    CancelControlID="Button_cerrar_envia_actividad_flujo_trabjo" PopupControlID="Panel_envia_actividad_flujo_trabjo"   ></asp:ModalPopupExtender>
                <div id="divcabecer2_envia_actividad_flujo_trabjo"  class="cabecera2">               
                    <asp:Label ID="Label_envia_actividad_flujo_trabjo" runat="server" Text="Mensaje" Font-Size="10" Style="float: left; font-family:Arial; margin-left:10px">
                    </asp:Label>
                    <div id="Divcerrarbuton2_envia_actividad_flujo_trabjo" style="float: right">
                        <asp:Button ID="Button_cerrar_envia_actividad_flujo_trabjo" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_envia_actividad_flujo_trabjo" style="background-color: white; width: 100%; height: 99%;border: thin double #000080; color: black; background-color: #FFFFFF;">                  
                        <asp:UpdatePanel ID="UpdatePanel_envia_actividad_flujo_trabjo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <input id="Hidden_estado_eliminar" type="hidden" value="" runat="server">
                                <div style="text-align: center">
                                    <br />
                                    <asp:Label ID="Label_title_comfirma_eliminar" runat="server" Text="Desea enviar la tarea a la actividad selecionada ?" style="font-family:Arial; font-size:14px"></asp:Label>
                                    <br />
                                    <br />
                                    <asp:Button ID="Button_aceptar_confirmacion" runat="server" Text="Aceptar" CssClass="boton_azul" /> &nbsp
                                    <asp:Button ID="Button_cancelar_confirmacion" runat="server" Text="Cancelar" CssClass="boton_azul" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
                 <asp:Button ID="Button_envia_actividad_flujo_trabjo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_envia_actividad_flujo_trabjo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
        </div>

          <!--detalle_actividad_flujo_user-->
          <div id="detalle_actividad_flujo_user">
            <asp:Panel ID="Panel_detalle_actividad_flujo_user" runat="server" Style="display:none; color: White; width:auto; height:auto" CssClass="modal_content_general_border"> 
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_detalle_actividad_flujo_user"  runat="server" BehaviorID="Panel_detalle_actividad_flujo_user" TargetControlID="ButtonSalir_detalle_actividad_flujo_user" BackgroundCssClass="FondoAplicacion" 
                    CancelControlID="Button_cerrar_detalle_actividad_flujo_user" PopupControlID="Panel_detalle_actividad_flujo_user"   ></asp:ModalPopupExtender>
                <div id="divcabecer2_detalle_actividad_flujo_user"  class="modal_title_superior">               
                    <asp:Label ID="Label_detalle_actividad_flujo_user" runat="server" Text="Detalle" Font-Size="10" Style="float: left; font-family:Arial; margin-left:1px">
                    </asp:Label>
                    <div id="Divcerrarbuton2_detalle_actividad_flujo_user" style="float: right">
                        <asp:Button ID="Button_cerrar_detalle_actividad_flujo_user" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_detalle_actividad_flujo_user" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF;" Class="modal_content_back"/>                  
                        <asp:UpdatePanel ID="UpdatePanel_detalle_actividad_flujo_user" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>                               
                                <div style="">
                                    <table style="width: 100%;">
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label_title_nombre_usuario" runat="server" Text="Nombre Usuario" style="font-family:Arial; font-size:12px"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label_nombre_usuario" runat="server" Text="" style="font-family:Arial; font-size:12px"></asp:Label>
                                            </td>
                                           
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label_title_cargo" runat="server" Text="Cargo Usuario" style="font-family:Arial; font-size:12px"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label_cargo" runat="server" Text="" style="font-family:Arial; font-size:12px"></asp:Label>
                                            </td>
                                            
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label_title_correo" runat="server" Text="Correo Electrónico" style="font-family:Arial; font-size:12px"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label_correo" runat="server" Text="" style="font-family:Arial; font-size:12px"></asp:Label>
                                            </td>
                                            
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label_title_nombre_grupo" runat="server" Text="Grupo usuario" style="font-family:Arial; font-size:12px"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label_nombre_grupo" runat="server" Text="" style="font-family:Arial; font-size:12px"></asp:Label>
                                            </td>
                                            
                                        </tr>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
               
                    <asp:Button ID="Button_detalle_actividad_flujo_user" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_detalle_actividad_flujo_user" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
        </div>
           <!--detalle_actividad_flujo-->
          <div id="detalle_actividad_flujo">
            <asp:Panel ID="Panel_detalle_actividad_flujo" runat="server" Style="display:none; color: White; width:auto; height:auto" CssClass="modal_content_general_border">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_detalle_actividad_flujo"  runat="server" BehaviorID="Panel_detalle_actividad_flujo" TargetControlID="ButtonSalir_detalle_actividad_flujo" BackgroundCssClass="FondoAplicacion" 
                    CancelControlID="Button_cerrar_detalle_actividad_flujo" PopupControlID="Panel_detalle_actividad_flujo"   ></asp:ModalPopupExtender>
                <div id="divcabecer2_detalle_actividad_flujo"  class="modal_title_superior">               
                    <asp:Label ID="Label_detalle_actividad_flujo" runat="server" Text="Detalle" Font-Size="10" Style="float: left; font-family:Arial; margin-left:1px">
                    </asp:Label>
                    <div id="Divcerrarbuton2_detalle_actividad_flujo" style="float: right">
                        <asp:Button ID="Button_cerrar_detalle_actividad_flujo" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_detalle_actividad_flujo" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF;" class="modal_content_back">                  
                        <asp:UpdatePanel ID="UpdatePanel_detalle_actividad_flujo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                                           
                                <div style="">
                                    <table style="width: 100%;">
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label_title_nombre_actividad" runat="server" Text="Nombre Actividad" style="font-family:Arial; font-size:12px"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label_nombre_actividad" runat="server" Text="" style="font-family:Arial; font-size:12px"></asp:Label>
                                            </td>
                                           
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label_title_descripcion" runat="server" Text="Descripción Actividad" style="font-family:Arial; font-size:12px"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label_descripcion" runat="server" Text="" style="font-family:Arial; font-size:12px"></asp:Label>
                                            </td>
                                            
                                        </tr>
                                         <tr>
                                            <td>
                                                <asp:Label ID="Label_title_tipo_actividad" runat="server" Text="Tipo Actividad" style="font-family:Arial; font-size:12px"></asp:Label>
                                            </td>
                                            <td>
                                                
                                                    <asp:Label ID="Label_tipo_actividad" runat="server" Text="" style="font-family:Arial; font-size:12px"></asp:Label>
                                                

                                                
                                            </td>
                                            
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label_title_usuario_relacionados" runat="server" Text="Usuarios Relacionados" style="font-family:Arial; font-size:12px"></asp:Label>
                                            </td>
                                            <td>
                                                <div style="overflow:auto">
                                                    <asp:Label ID="Label_usuario_relacionados" runat="server" Text="" style="font-family:Arial; font-size:12px"></asp:Label>
                                                </div>

                                                
                                            </td>
                                            
                                        </tr>
                                        
                                    </table>
                                </div>
                            
                                
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
                 <asp:Button ID="Button_detalle_actividad_flujo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_detalle_actividad_flujo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
        </div>
        <!--autoriza reasignacion flujo-->
          <div id="lista_actividades_ruta_flujo">
            <asp:Panel ID="Panel_lista_actividades_ruta_flujo" runat="server" Style="display:none; color: White; width: 600px; height: 200px">

                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_actividades_ruta_flujo" runat="server" BehaviorID="Panel_lista_actividades_ruta_flujo" TargetControlID="ButtonSalir_lista_actividades_ruta_flujo" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_lista_actividades_ruta_flujo" PopupControlID="Panel_lista_actividades_ruta_flujo" ></asp:ModalPopupExtender>
                <div id="divcabecer2_lista_actividades_ruta_flujo" class="cabecera2">
                    <asp:Button ID="Button_lista_actividades_ruta_flujo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_lista_actividades_ruta_flujo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label_lista_actividades_ruta_flujo" runat="server" Text="Autoriza reasignación" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_lista_actividades_ruta_flujo" style="float: right">
                        <asp:Button ID="Button_cerrar_lista_actividades_ruta_flujo" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_lista_actividades_ruta_flujo" style="background-color: white; width: 100%; height: 99%;border: thin double #000080; color: black; background-color: #FFFFFF;">
                                
                    
                        <asp:UpdatePanel ID="UpdatePanel_lista_actividades_ruta_flujo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                               <br />
                                <table style="width: 100%;">
                                   
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_user_lista_actividades_ruta_flujo" runat="server" Text="Usuario autorizado*" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label>
                                        </td>
                                        <td><asp:TextBox ID="TextBox_login_lista_actividades_ruta_flujo" runat="server" Style="width:300px"></asp:TextBox></td>
                                       
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_dest_lista_actividades_ruta_flujo" runat="server" Text="Contraseña usuario*" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label>

                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_pasw_lista_actividades_ruta_flujo" runat="server" Style="width:300px"  TextMode="Password"></asp:TextBox> 
                                           

                                        </td>                           
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    
                                    <tr>
                                        <td>

                                        </td>
                                        <td style="float:left"><asp:Button ID="Button_autoriza_reasignacion_flujo" runat="server" Text="Reasignar" Style="background-color: white; border-color: #b0c4de; height: 30px; width: 200px; height: 25px; text-align: center" CssClass="boton" /> &nbsp &nbsp
                                                     <input id="Hidden_resp_envio_flujo" type="hidden" value="" runat="server">    
                                        </td>
                                    </tr>
                                    
                                    
                                </table>
                                                         
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
            </asp:Panel>
        </div>
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
    </form>
</body>
</html>
