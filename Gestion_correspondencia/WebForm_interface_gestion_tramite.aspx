<%@ Page Language="vb" AutoEventWireup="false"  EnableEventValidation="false" CodeBehind="WebForm_interface_gestion_tramite.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebForm_interface_gestion_tramite" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
   
    <title>Gestion tramites</title>
    <script src="../js/ui/jquery-3.4.1.min.js" type="text/javascript"></script>
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
     <script src="../js/jquery-ui-1.12.1.custom/jquery-ui.min.js" type="text/javascript"></script>
    <link href="../js/jquery-ui-1.12.1.custom/jquery-ui.min.css" rel="stylesheet" />
    <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <script src="../js/sizeimagejquery.js" type="text/javascript"></script> 
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.11.0/umd/popper.min.js" type="text/javascript"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js" type="text/javascript"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />  
    <script src="../js/table_boo/table_boot_config.js" type="text/javascript"></script>
    <script src="../js/java_general/BootstrapTable.js" type="text/javascript"></script>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table.min.css"/>
    <script src="https://cdn.jsdelivr.net/npm/tableexport.jquery.plugin@1.29.0/tableExport.min.js" type="text/javascript"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table.min.js" type="text/javascript"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/bootstrap-table-locale-all.min.js" type="text/javascript"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-table@1.23.1/dist/extensions/export/bootstrap-table-export.min.js" type="text/javascript"></script> 
    <link href="../tokenzize2/tokenize2.min.css" rel="stylesheet" />
    <script src="../tokenzize2/tokenize2.1.min.js"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>  
    <link href="../Styles/Menu3.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/java_general/gestion_meta_dato.js" type="text/javascript"></script>
    <script src="../js/gestion_correspondencia/WebForm_interface_gestion_tramite.js"></script>
    <script src="../js/java_general/general_code_java.js"></script>
    <script src="../js/java_general/ubicacion_code_java.js"></script>
    <script src="../js/java_general/general_control_java.js"></script>
    <script src="../generic_control/FileUploadHandler.js"></script>
    <link href="../generic_control/UploadFile.css" rel="stylesheet" />
    <script  src="../Awesome/js/all.js"></script>
    <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
    <link href="../Awesome/css/brands.css" rel="stylesheet"/>
    <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
    <script  src="../Awesome/js/solid.js"></script>
     <script  src="../Awesome/js/fontawesome.js"></script>  
     <script src="../js/ScrollableGridViewPlugin_ASP.NetAJAXmin.js" type="text/javascript"></script>
     <script src="../Fixed-Header-Table-master/gridviewScroll.min.js"></script>
    <script src="../js/Filtrar.js"></script>
    <script src="../js/validate_campos.js"></script>  
</head>
<body >
    <form id="form1"  runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True">
        </asp:ScriptManager>
        <script type="text/javascript" language="javascript">
            Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeRequest);
            Sys.Application.add_load(ApplicationLoadHandler)
            var elment_postbak;
            var value_element;
            function ApplicationLoadHandler(sender, args) {

                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(CheckStatus);

            }
            function InitializeRequest(sender, args) {
                //
                posicion_update_pogres('progres_bar');
                elment_postbak = args.get_postBackElement();
                var elmen = document.getElementById(elment_postbak.id)
                if (elmen.type == "button" || elmen.type == "submit") {
                    value_element = elmen.value;
                    elmen.value = "Espere..."
                    elmen.disabled = true;
                }
            }
            function CheckStatus(sender, args) {
                try {    
                    if (elment_postbak.type == "button" || elment_postbak.type == "submit") {
                        elment_postbak.value = value_element;
                        elment_postbak.disabled = false;
                    }
                    if (elment_postbak.id == "Button_valida_Cerrar_respuesta_radicado") {
                        actualiza_estado_tramite();
                    }
                    if (elment_postbak.id == "ImageButtonterminar") {
                        if (document.getElementById("Hidden_men_result").value == 1) {
                            eliminar_fila_data_gred('data_grid_listado_solicitudes', 'hdnEmailID');
                            Decrementa_contador_tramites();
                            document.getElementById("Hidden_men_result").value = 0;
                        }
                    }
                   
                    if (elment_postbak.id == "Button_reasignar_tramite") {
                        if (document.getElementById("Hidden_result_reasignar").value == "1") {
                            eliminar_fila_data_gred('data_grid_listado_solicitudes', 'hdnEmailID');
                            Decrementa_contador_tramites();
                            Actualiza_cantidad_barr_estado();
                            document.getElementById("Hidden_result_reasignar").value = "0";
                        }
                    }
                    if (elment_postbak.id == "Button_autoriza_reasignacion") {
                        if (document.getElementById("Hidden_resp_envio").value == "1") {
                            eliminar_fila_data_gred('data_grid_listado_solicitudes', 'hdnEmailID');
                            Decrementa_contador_tramites();
                            Actualiza_cantidad_barr_estado();
                            document.getElementById("Hidden_resp_envio").value = "0";
                        }
                    }
                    if (elment_postbak.id == "Button_visor_emergente") {
                        auto_zise_popup_visor_externo();
                    }
                    if (elment_postbak.id == "Button_activa_lista_imagenes_gestion_corresponencia") {
                        auto_zise_popup_lista_imagenes_gestion();
                    }
                   
                    if (elment_postbak.id == "Button_confirma_reversar") {
                        if (document.getElementById("Hidden_con_ref").value == "YES") {
                            document.getElementById('Hidden_estado_tramite').value = "Por tramitar";
                            changue_row_font_weigh_bold('data_grid_listado_solicitudes', document.getElementById("hdnEmailID").value);
                            actualiza_estado_tramite();
                            document.getElementById("Hidden_con_ref").value = "";
                        }
                    }
                    if (elment_postbak.id == "Button_reversar") {
                        if (document.getElementById("Hidden_user_rever").value == "YES") {
                            document.getElementById('Hidden_estado_tramite').value = "Por tramitar";
                            changue_row_font_weigh_bold('data_grid_listado_solicitudes', document.getElementById("hdnEmailID").value);
                            actualiza_estado_tramite();
                            document.getElementById("Hidden_user_rever").value = "";
                        }
                    }
                   
                   
                    
                    if (elment_postbak.id == "Button_guardar_desicion" || elment_postbak.id == "Button_acepta_sube_documento_integra_sii") {
                        if (document.getElementById("Hidden_result_load").value == "YES") {
                            document.getElementById("Hidden_result_load").value = "";
                            insert_row_documento_relacionado(document.getElementById("Hidden_date_row").value, document.getElementById("Hidden_tip_adjunt").value);
                            document.getElementById("Hidden_date_row").value = "";
                            document.getElementById("Hidden_tip_adjunt").value = "";
                        }
                    }
                    if (elment_postbak.id == "Button_eliminar_documento") {
                        if (document.getElementById("Hidden_confir_elimina").value == "YES") {
                            document.getElementById("Hidden_confir_elimina").value = "";
                            var seter = "-1";
                            if (document.getElementById("Hidden_selccion_documento_eliminar_wf").value == document.getElementById("hiden_seleccion_documento_id_wf").value) {
                                seter = "";
                            }
                            eliminar_fila_data_gred_simple_wf('GridView_list_documento_relacion_wf', 'Hidden_selccion_documento_eliminar_wf', 'Hidden_selccion_documento_eliminar_split_wf', seter, seter)
                        }
                    }
                    if (elment_postbak.id == "Button_actualiza_tipologia_documental_workflow") {
                        if (document.getElementById("Hidden_resulta_botno_tipologia_documental_workflow").value != "") {
                            update_Cell_AspNetGred('GridView_list_documento_relacion_wf', document.getElementById("Hidden_selccion_documento_cambia_tipo_wf").value, document.getElementById("Hidden_resulta_botno_tipologia_documental_workflow").value, 'DOCUMENTO', 'id_wf');
                            document.getElementById("Hidden_resulta_botno_tipologia_documental_workflow").value = "";
                        }
                    }
                    //ZONA RESPUESTA RADICADO

                    //Acualiza estado EN TRAMITE
                    if (elment_postbak.id == "Button_descarga_plantilla_radicado_resp") {
                        if (document.getElementById("Hidden_rest_resp").value == "YES") {
                            document.getElementById("Hidden_rest_resp").value = "";
                            changue_row_font_weigh_light('data_grid_listado_solicitudes', document.getElementById("hdnEmailID").value);
                            actualiza_gre_campo('data_grid_listado_solicitudes', document.getElementById("hdnEmailID").value, 'En tramite', 'ESTADO');
                        }
                    }
                   
                    Actualiza_cantidad_barr_estado();
                }
                catch (err) {
                    alert(" Funcion CheckStatus asincrona workflow.aspx" + err.message);
                }
                finally {
                    progres_hiden('progres_bar');
                }
            }
        </script>
     <div id="div_contendor_principal">
             <nav id="navar_barra" class="navbar navbar-expand-sm nav_botota_person_gray modal_content_no_back_inferior"  > 
               <button class="navbar-toggler" type="button" style=" background-color:#6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                   <span class="navbar-toggler-icon_"><i style="color:white" class="fad fa-th-list"></i></span>
               </button>
                <div class="collapse navbar-collapse row" id="navbarNavDropdown">               
                    <ul class="navbar-nav col-md-8"> 
                       
                        <li class="nav-item dropdown active ml-2 active_"> 
                            
                            <a class="nav-link dropdown-toggle bot_hover_person" style="color:#6d7fcc" href="#" id="navbarDropdownMenuLink" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"> <i style="color:#0062cc" class="fad fa-line-columns"></i> Detalle
                            </a>
                            <div class="dropdown-menu " aria-labelledby="navbarDropdownMenuLink">
                                <a class="dropdown-item" href="#" onclick="activa_menu_general_diference(event,this,'B-HT')">  Busqueda historico de tramites</a>
                                <a class="dropdown-item" href="#" onclick="activa_boton_client_server('ImageButton_guarda_lista');"> Descargar lista de resultados</a>
                                <a class="dropdown-item" href="#" onclick="activa_menu_general_diference(event,this,'D-RT')"> Detalle respuesta del tramite</a>
                                <a class="dropdown-item" href="#" onclick="activa_boton_client_server('Button_tool_activa_detalle_radicado_seleccion');"> Detalle del radicado</a>        
                                <a class="dropdown-item" href="#" onclick="activa_menu_general_diference(event,this,'T-DT')"> Transacciones del tramite </a>
                                <a class="dropdown-item" href="#" onclick="activa_menu_general_diference(event,this,'G-TDW')"> Trazabilidad del tramite</a> 
                                
                            </div>
                        </li>        
                         <li class="nav-item dropdown active ml-0 mr-0 active_">
                                <a class="nav-link  dropdown-toggle" style="color:#6d7fcc" href="#" id="navbarDropdownMenuLink_" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color:#0062cc" class="fad fa-th-list  "></i> Opciones
                                </a>
                                <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                                    <a class="dropdown-item" href="#" onclick="activa_menu_general_diference(event,this,'R-T-D')"> Asumir el tramite</a>
                                    
                                    <a class="dropdown-item" href="#" onclick="activa_menu_general_diference(event,this,'S-A-R')"> Lista solicitudes de aprobación documento respuesta del tramite</a>
                                </div>
                            </li>                
                         <li class="nav-item active ml-2 active_">
                           <a  class="nav-link" style="color:#6d7fcc" title="Actualizar lista trámites pendientes" href="#" onclick="activa_boton_client_server('ImageButtonactualizar');"><i  style="margin-left: 1px; margin-top: 7px; color:#0062cc " class="fad fa-sync-alt"></i>  Actualizar  </a>
                        </li>
                                                 
                     </ul>
                         
                    <div class=" float-md-right col-md-4 float-sm-left">
                        <div class="input-group ">
                            <button id="td-boton" class="btn btn-outline-secondary border-right-2 " style="border-top-right-radius: 0px; border-bottom-right-radius: 0px" onclick="activa_boton_client_server('ImageButtonactualizar');" type="button">
                                <i class="fal fa-long-arrow-left"></i>
                            </button>
                            <asp:TextBox ID="auto_complex" runat="server" class="form-control form-control-sm complex " placeholder="Busqueda...." onkeypress="preven_event_search_keypres_enter(event,this);"></asp:TextBox>
                            <div class="input-group-append">
                                <button class="btn btn-outline-secondary" onclick="preven_event_search(event,this)" type="button">
                                    <i class="fal fa-search"></i>
                                </button>
                            </div>
                        </div>
                    </div>          
             </div>                                  
            </nav>
            <div id="Menutol" class="navbar_gray" style="overflow: auto; background-color: #f5f5f5; display:none">
                <asp:UpdatePanel ID="updatemenu" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:ImageButton ID="ImageButtonactualizar" runat="server" ImageUrl="../workflow/imageneswf/actualizar.jpg"
                            ToolTip="Actualizar para nuevas Tareas" AlternateText="Actualizar lista"
                            ImageAlign="Left" Width="1px" Height="1px" Style="margin-left: 3px; display: none" CssClass="alterna_image" OnClientClick="actualiza_selecion()" />
                        <asp:ImageButton ID="ImageButton_responder" runat="server" ImageUrl="../workflow/imageneswf/autoterminar.jpg"
                            ToolTip="El sistema decide el envío por usted" AlternateText="El sistema decide el envío por usted" CssClass="alterna_image"
                            ImageAlign="Left" Width="1px" Height="1px" Style="display: none" />
                        <asp:ImageButton ID="ImageButtonEnviarUsuario" runat="server" CssClass="alterna_image"
                            AlternateText="Renviar a Usuario"
                            ImageAlign="Left" Width="1" Height="1px" Style="display: none" />
                        <asp:ImageButton ID="ImageButtonConfirmar" runat="server" ImageAlign="Left" Width="1px" Height="1px" Style="display: none" CssClass="alterna_image" />
                        <asp:ImageButton ID="ImageButtonterminar" runat="server" OnClientClick="ConfirmMensajeGeneral('Desea terminar el tramite relacionado a la tarea?', 'Hidden_men_result')"
                            ImageAlign="Left" Width="1px" Height="1px" Style="display: none" />
                        <asp:UpdatePanel ID="UpdatePanel_busqueda" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="box">
                                    <asp:ImageButton ID="ImageButton_buscar" runat="server" Style="height: 1px; height: 0px; display: none" />
                                    <asp:ImageButton ID="ImageButton_filter" runat="server" Style="height: 1px; width: 1px; display: none" />
                                    <asp:ImageButton ID="ImageButton_guarda_lista" runat="server" OnClientClick="activa_export_lista('Hidden_colum_header','data_grid_listado_solicitudes')" Style="height: 1px; margin-top: 5px; display: none" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:Button ID="Button_visor_emergente" runat="server" Text="Button" Style="display: none" />
                        <asp:Button ID="Button_activa_lista_imagenes_gestion_corresponencia" runat="server" Text="Button" Style="display: none"/>
                        <asp:Button ID="Button_activa_lista_solicitudes_aprobacion" runat="server" Text="Button" Style="display: none"/>
                        <input id="Hidden_estado_tareas_pendiente" type="hidden" value="NO" runat="server"/>
                        <input id="Hidden_estado_anotacion" type="hidden" value="NO" runat="server"/>
                        <input id="Hidden_estado_pendiente_aprobacion" type="hidden" value="NO" runat="server"/>
                        <input id="Hidden_activa_popup" type="hidden" value="" runat="server"/>
                        <input id="Hidden_lista_ruta_flujo" type="hidden" value="" runat="server"/>
                        <input id="Hidden_vi_reasigna" type="hidden" value="" runat="server"/>
                        <input id="Hidden_men_result" type="hidden" value="" runat="server"/>
                        <input id="Hidden_id_respuesta_" type="hidden" value="-1" runat="server"/>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="div_contendor_filtro_listado" class="modal_content_no_back_inferior " style="width:100%; height:auto" >   
                <div class="row">
                    <div class="col-4">
                        <div id="div_filtro__fil" class="dropdown_filter" style="margin-left: 10px">
                            <button id="boton__filtro_ver" onclick="myFunction(event,this)" data-toggle="dropdown" class="dropbtn_filter dropdown-toggle" style="color: #0062cc">Filtrar</button>
                            <div id="myDropdown" class="dropdown-content_filter" onkeyup="hiden_keys(event, thiss)">
                                <input type="text" placeholder="Search.." id="myInput" onkeyup="filterFunction()" />
                                <a href="#about" onclick="event_elemento(event,'1',this)" class="e_list_marc">Todos</a>
                                <a href="#base" onclick="event_elemento(event,'2',this)" class="e_list_marc">Por tramitar</a>
                                <a href="#blog" onclick="event_elemento(event,'3',this)" class="e_list_marc">En tramite</a>
                                <a href="#blog" onclick="event_elemento(event,'4',this)" class="e_list_marc">Tramitado</a>
                                <a href="#blog" onclick="event_elemento(event,'10',this)" class="e_list_marc">Tramitado archivado</a>
                                <a href="#blog" onclick="event_elemento(event,'5',this)" class="e_list_marc">Solicitud por aprobación</a>
                                <a href="#blog" onclick="event_elemento(event,'6',this)" class="e_list_marc">Solicitud aprobada</a>
                                <a href="#blog" onclick="event_elemento(event,'7',this)" class="e_list_marc">Solicitud desaprobada</a>
                                <a href="#blog" onclick="event_elemento(event,'8',this)" class="e_list_marc">Solicitud archivada</a>
                                <a href="#blog" onclick="event_elemento(event,'9',this)" class="e_list_marc">Solicitud anulada</a>
                            </div>
                        </div>
                    </div>
                    <div class="col-6">
                        <asp:UpdatePanel ID="UpdatePanel_title" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <div id="contenido_titulo_listado_solicitudes" style="width: 100%; position: inherit; height: 100%" class="p-2  modal_content_no_back_superior nav_botota_person_gray__">
                                    <div class="row">
                                        <div class="col-8">
                                            <asp:Label ID="Label_titulo_listado_solicitudes" runat="server" Style="color: #6d7fcc; float: left" CssClass="h6  font-weight-normal">Resultados busqueda</asp:Label>
                                        </div>
                                        <div class="col-4">
                                            <asp:Label ID="Label_estado" runat="server" ForeColor="Black" Font-Size="9px" Style="float: left"></asp:Label>
                                        </div>
                                       
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col-2">
                        <asp:Label ID="Label_anunciado_filtro"  runat="server" Text="Todos" CssClass="p2" style="font-size:12px; font-family:Arial; float:left; margin-top:5px; float:right; margin-right:2px; margin-right: 15px; color:#0062cc"></asp:Label> 
                    </div>
                </div>             
            </div>
         <div id="content_grid" class="border_general_blanco_" style="width: auto; position: inherit; left: 0px; top: 0px; margin-top: 1px; margin-right: 10px; margin-left: 10px">
             <asp:Panel ID="Panel_principal" runat="server" ScrollBars="Auto" Width="100%" Style="height: 600px">
                 <asp:UpdatePanel ID="UpdateGeneral" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                     <ContentTemplate>
                         <input id="hdnEmailID" type="hidden" value="0" runat="server" />
                         <input id="hdnEmailID_VAL" type="hidden" value="0" runat="server" />
                         <input id="Hidden_content" type="hidden" value="0" runat="server" />
                         <input id="HiddenEmailconsulta" type="hidden" value="" runat="server" />
                         <input id="Hidden_control_lista" type="hidden" value="" runat="server" />
                         <asp:GridView ID="data_grid_listado_solicitudes" runat="server" AllowSorting="true" AllowPaging="true" EnableViewState="true"
                             PageSize="7" PagerSettings-Position="Top" Style="width: 100%; font-family: Segoe UI"
                             AutoGenerateSelectButton="False" CssClass="table font-weight-light" GridLines="None" Font-Size="14px">
                             <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                             <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                             <RowStyle CssClass="" />
                             <PagerStyle CssClass="pagination-ys" />
                             <Columns>
                                 <asp:BoundField HeaderText="OPCIONES" />
                             </Columns>
                         </asp:GridView>
                     </ContentTemplate>
                     <Triggers>
                     </Triggers>
                 </asp:UpdatePanel>
             </asp:Panel>
         </div>
            <div id="contenedor_opciones_solictitud_general" style="width: 100%; text-align: left; font-family: Arial; background-color: #E7EDF5; margin-top: 1px; border-color: #b0c4de; border-style: ridge; border-width: 1px; display: none">
                <asp:UpdatePanel ID="update_botonoes_opciones_solicitud_general" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server"/>
                        <input id="Hidden_solicitud_compartido" type="hidden" value="-1" runat="server"/>
                        <input id="Hidden_result_eliminar" type="hidden" value="" runat="server"/>
                        <input id="Hidden_resultado_ver_documento" type="hidden" value="" runat="server"/>
                        <input id="Hiddenxxxxxx" type="hidden" value="" runat="server"/>
                          <input id="Hidden_rest_resp" type="hidden" value="" runat="server"/>  
                        <asp:Button ID="Button_ver_documento_solicitud" runat="server" Enabled="false" Text="Ver documento solicitud" ToolTip="Visualiza el documento de solicitud de aprobación" CssClass="boton" Style="display: none" />
                        <asp:Button ID="Button_ver_documento_respuesta_solicitud" runat="server" Text="Ver documento respuesta" ToolTip="Visualiza el documento respuesta de solicitud de aprobación" CssClass="boton" Style="display: none" />
                        <asp:Button ID="Button_activa_desicion_aprobacion" runat="server" Text="Gestionar solicitud" ToolTip="Agrega una nueva solicitud de aprobación" CssClass="boton" Style="display: none" />
                        <asp:Button ID="Button_lista_filtro" runat="server" Text="Gestionar solicitud" CssClass="boton" Style="display: none" />
                        <asp:Button ID="Button_ver_documentos_relacionados" runat="server" Text="Ver documentos" ToolTip="Lista los documentos compartidos relacionados con el registro seleccionado" CssClass="boton" />
                        <asp:Button ID="Button_eliminar_registro" runat="server" Text="Eliminar" CssClass="boton" ToolTip="Elimina el registro seleccionado" OnClientClick="ConfirmMensajeGeneral('Desea eliminar el registro','Hiddenxxxxxx')" />
                        <asp:Button ID="Button_activa_visto" runat="server" Text="Eliminar" CssClass="boton" Style="display: none" />
                        <asp:Button ID="Button_ver_registro_colaboracion" runat="server" Text="Ver registro colaboración" CssClass="boton" ToolTip="Ver los registro de colaboración asociados al registro" />
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="UpdatePanel_boton_tool" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                <ContentTemplate>
                    <asp:Button ID="Button_tool_activa_sube_documento" runat="server" Text="" />
                    <asp:Button ID="Button_tool_activa_sube_documento_lista" runat="server" Text="" />
                    <asp:Button ID="Button_tool_adjunta_documento_relacionado" runat="server" Text="" />
                    <asp:Button ID="Button_tool_activa_detalle_radicado_seleccion" runat="server" Text="" /> 
                     <asp:ImageButton ID="ImageButtonanotacion" runat="server"   Width="0px" Height="0px" style="margin-left:0px; display:none" />
                </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
         <div style="display: none">
                <asp:UpdatePanel ID="UpdatePanel_menu_boton" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Button ID="Button_lik_service_boton" runat="server" Text="Button" />
                        <input id="Hidden_lik_service_boton" type="hidden" value="0" runat="server"/>
                    </ContentTemplate>
                </asp:UpdatePanel>
                  <asp:UpdatePanel ID="UpdatePanel_menu_var_event" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <input id="Hidden_menu_var_event_dive" type="hidden" value="" runat="server" />
                        <asp:Button ID="Button_me_active_men_dive" runat="server" Text="" Style="display: none; width: 1px; height: 1px" />
                        
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
         <!--nota_flujo-->  
           <asp:Panel ID="Panel_content_anotacion" runat="server" Style="display: none; width: 90%; height: 100%" CssClass="modal_content_general_">
                  <asp:ModalPopupExtender ID="ModalPopupExtender_edition_content_anotacion" runat="server"
                      TargetControlID="ButtonSalir_content_anotacion_" BackgroundCssClass="FondoAplicacion"
                      CancelControlID="Button_cerrar_content_anotacion" PopupControlID="Panel_content_anotacion">
                  </asp:ModalPopupExtender>
                  <div id="modal_content_anotacion" class="modal-content">
                      <div id="diver_cabcera_content_anotacion" class="modal_title_superior_ modal-header">
                          <asp:Label ID="LabelTitulo" runat="server" Text="Anotaciones de la Tarea" CssClass="h6 font-weight-light"> </asp:Label>
                          <button type="button" value="Button_cerrar_content_anotacion" class="close da_event_captive ">&times;</button>
                      </div>
                      <div id="contenido_procesa_content_anotacion" style="height: auto; width: 100%; overflow: auto; border-top:none" class="pl-3 pr-3">
                          <asp:UpdatePanel ID="UpdatePanelanotacion" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <asp:Panel ID="Panel_content_anotacion_gred" runat="server"
                                      Enabled="true" Style="margin-bottom: 5px">
                                      <asp:GridView ID="GridView_lista_notas" runat="server" Style="width: 100%"
                                          AutoGenerateSelectButton="False" CssClass="filtrar table  font-weight-light" GridLines="None">
                                          <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                          <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                          <Columns>
                                              <asp:BoundField HeaderText="OPCIONES" />
                                          </Columns>
                                      </asp:GridView>
                                  </asp:Panel>
                                  <input id="hdnidlista" type="hidden" value="-1" runat="server"/>
                              </ContentTemplate>
                              <Triggers>
                              </Triggers>
                          </asp:UpdatePanel>
                      </div>
                       <div id="content_boton" class="modal-footer justify-content-end">
                           <input id="Button_Show_Guardar" type="button" class="btn btn-success" value="Nueva nota"  />        
                  </div>
                  </div>
                 
                  <div style="display: none; height: 1px">
                      <asp:Button ID="Button_content_anotacion" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                      <asp:Button ID="ButtonSalir_content_anotacion_" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                      <asp:Button ID="Button_cerrar_content_anotacion" runat="Server" Text="X" CssClass="invisible" />
                  </div>
              </asp:Panel>
             <div id="nota_respuesta">
                  <asp:Panel ID="Panel_nota_respuesta" runat="server" Style="display: none; width: 66%; height: auto" CssClass="modal_content_general_">
                      <asp:ModalPopupExtender ID="ModalPopupExtender_edition_nota_respuesta" runat="server" TargetControlID="ButtonSalir_nota_respuesta" BackgroundCssClass="FondoAplicacion"
                          CancelControlID="Button_cerrar_nota_respuesta" PopupControlID="Panel_nota_respuesta">
                      </asp:ModalPopupExtender>
                      <div id="modal_content_nota_respuesta" class="modal-content">
                          <div id="divcabecer_nota_respuesta" class="modal_title_superior_ modal-header">
                              <asp:Label ID="Label_nota_respuesta" class="modal-title d-inline ml-1 h6" runat="server" Text="Nota">
                              </asp:Label>
                              <button type="button" value="Button_cerrar_nota_respuesta" class="close da_event_captive">&times;</button>
                          </div>
                         
                          <div id="contenido_procesa_nota_respuesta" style="border-top: none; overflow: hidden" class="p-1">    
                               <textarea id="TextBox_nota" rows="10" style="width: 100%; height: 100%" cols="50">Write something here</textarea>
                          </div>
                          <div id="content_boton_nota" class="modal-footer">
                              <input id="Button_actualizar_nota" type="button" class="btn btn-success" value="Aceptar" style="display:none" />
                              <input id="Button_duardar_nota" type="button" class="btn btn-success" value="Aceptar" style="display:none"/>
                          </div>
                          <div style="display: none; height: 1px">
                              <asp:Button ID="Button_nota_respuesta" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" Style="display: none" />
                              <asp:Button ID="ButtonSalir_nota_respuesta" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" Style="display: none" />
                              <asp:Button ID="Button_cerrar_nota_respuesta" runat="Server" Text="X" CssClass="modal_boton_hiden" />
                          </div>

                      </div>
                  </asp:Panel>
              </div>  
       
        <!--Popup visor externo-->
         <input id="Hidden_tipo_visor" type="hidden" value="" runat="server"/>
        <asp:Panel ID="Panel_visor_externo" runat="server" Style="display:none; overflow: hidden" ForeColor="White" Width="100%" Height="100% " CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_visor_externo" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_visor_externo"
                PopupControlID="Panel_visor_externo" CancelControlID="ButtonSalir_visor_externo">
            </asp:ModalPopupExtender>            
            <div id="Cotenedorpendiente_visor_externo" style="color: Black; background-color: #FFFFFF; height: 100%; width: 100%; overflow:auto" class="modal_content_back_">
                <nav id="nav_visor" class="navbar  navbar-light  bg-transparent ">
                    <button class="navbar-toggler d-none" type="button" data-toggle="collapse" data-target="#navbarNavDropdown_">
                        <span class="navbar-toggler-icon"></span>
                    </button>
                    <div class="container-floid col-12  justify-content-end pl-0 pr-0">
                               <button type="button" onclick="activa_boton_client_server('ButtonSalir_visor_externo');" class="close">&times;</button>
                         </div>  
                    <div class="collapse navbar-collapse row d-none" id="navbarNavDropdown_">
                            
                        <ul class="navbar-nav col-md-10 ">
                            <a href="#" class="navbar-brand ml-1"  style="color:#0062cc" > Gestión de tramites </a> 
                            <li class="nav-item dropdown active ml-0 mr-0 active_">     
                                <a class="nav-link  dropdown-toggle bot_hover_person" href="#" id="A1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color:#0062cc" class="fad fa-line-columns"></i> Detalle
                                </a>
                                <div class="dropdown-menu " aria-labelledby="navbarDropdownMenuLink">      
                                    <a class="dropdown-item" href="#" onclick="activa_menu_general_diference(event,this,'D-RT')">Detalle respuesta del tramite</a>
                                    <a class="dropdown-item" href="#" onclick="activa_menu_general_diference(event,this,'T-DT')">Transacciones del tramite </a>
                                    <a class="dropdown-item" href="#" onclick="activa_menu_general_diference(event,this,'G-TDW')">Trazabilidad del tramite</a>         
                                </div>
                            </li>
                            <li class="nav-item dropdown active ml-0 mr-0 active_">
                                <a class="nav-link  dropdown-toggle" href="#" id="A2" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color:#0062cc" class="fad fa-th-list  "></i> Opciones
                                </a>
                                <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                                    <a class="dropdown-item" href="#" onclick="activa_menu_general_diference(event,this,'R-T-D')">Asumir el tramite</a>
                                    <a class="dropdown-item" href="#" onclick="activa_menu_general_diference(event,this,'R-R-D')">Reversar estión tramite</a>
                                    <a class="dropdown-item" href="#" onclick="activa_menu_general_diference(event,this,'S-A-R')">Solicitudes de aprobación del tramite</a>
                                </div>
                            </li>

                            <li class="nav-item active ml-0 mr-0 active_">
                                <a class="nav-link "  title="Responder "  href="#" onclick="activa_boton_client_server('Button_activa_respuesta_radicado');"><i style="margin-left: 2px; margin-top: 7px; color:#0062cc; margin-right:3px" class="fad fa-reply"></i>Responder </a>
                            </li>
                            <li class="nav-item active ml-0 mr-0 active_">
                                <a class="nav-link " title="Resignar a otro usuario"  href="#" onclick="activa_boton_client_server('ImageButtonEnviarUsuario');"><i style="margin-left: 2px; margin-top: 7px; color: #0062cc; margin-right:3px" class="fad fa-user"></i>Reasignar </a>
                            </li>
                            <li class="nav-item active bot_hover_person ml-0 mr-0 active_">
                                <a class="nav-link  " title="Archivar el tramite"  href="#" onclick="activa_boton_client_server('ImageButtonConfirmar');"><i style="margin-left: 2px; margin-top: 7px; color: #0062cc; margin-right:3px" class="fad fa-archive"></i>Archivar </a>
                            </li>
                            <li class="nav-item active ml-0 mr-0 active active_">
                                <a id="A4" class="nav-link " title="Finalizar el tramite"  href="#" onclick="activa_boton_client_server('ImageButtonterminar');"><i style="margin-left: 2px; margin-top: 7px; color: #0062cc; margin-right:3px" class="fad fa-check"></i>Finalizar </a>
                            </li>
                        </ul>
                           

                    </div>
                   
                </nav>
                
                <asp:UpdatePanel ID="UpdatePanel_visor_externo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <iframe id="Iframe_visor_externo_" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: hidden; display:none"></iframe>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div style="display:none; height:1px">
                  <asp:Button ID="ButtonSalir_visor_externo" Style="color:white; display:none" runat="Server" Text="X" CssClass="modal_boton_hiden"
                         Height="21px" />
                   <asp:Button ID="Button_visor_externo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" Style="display: none" />
             </div>
           
        </asp:Panel>
        <asp:Panel ID="Panel_lista_imagenes_gestion" runat="server" Style="display: none; width: 100%; height: 100%" CssClass="modal_content_general">
                   <asp:ModalPopupExtender ID="ModalPopupExtender_edition_lista_imagenes_gestion" runat="server"
                       TargetControlID="ButtonSalir_lista_imagenes_gestion_" BackgroundCssClass="FondoAplicacion"
                       CancelControlID="Button_cerrar_lista_imagenes_gestion" PopupControlID="Panel_lista_imagenes_gestion">
                   </asp:ModalPopupExtender>
                   <div id="modal_lista_imagenes_gestion" class="modal-content_">
                       <div id="diver_cabcera_lista_imagenes_gestion" class="modal_title_superior_ modal-header">
                           <asp:Label ID="Label_lista_imagenes_gestion" runat="server" Text="" CssClass="h6 font-weight-light" Style="color: #6d7fcc" > </asp:Label>
                           <button type="button" value="Button_cerrar_lista_imagenes_gestion" class="close da_event_captive ">&times;</button>
                       </div>
                       <div id="contenido_procesa_lista_imagenes_gestion" style="height: auto; width: 100%;  border-top:none" >           
                           <div id="content_selecion_tarea" style="width: 100%">
                               <div id="content_seleccion_documentos" style="width: 25%; position: relative; left: auto; float: left; height: 100%;" class="modal_content_no_back_rigth modal_content_no_back_inferior">
                                   <div id="div_label" style="width: 100%; border-top-left-radius: initial; border-top-right-radius: initial" class="modal-header_ modal_title_superior  p-2 modal_content_no_back_inferior ">
                                       <div class="row">
                                           <div class="col-8">
                                               <asp:UpdatePanel ID="UpdatePanel_label_seleccion" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                                   <ContentTemplate>
                                                       <asp:Label ID="Label_docu_relacionado_wf" runat="server" Text="Documentos (0)" Style="color: #0062cc; float: left; font-family: 'Segoe UI'" CssClass="h6 mt-1  font-weight-normal"></asp:Label>
                                                   </ContentTemplate>
                                               </asp:UpdatePanel>
                                           </div>
                                           <div class="col-4">
                                               <div class="nav-item_ active active_">
                                                   <a class="nav-link " style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600; float: right" title="Adjuntar documento" href="#" onclick="inicializa_tipo_adjunto_documento(event,this,'C-DW-LISTA')"><i style="" class="fas fa-upload "></i></a>
                                                  
                                               </div>
                                           </div>
                                       </div>       
                                   </div>
                                   <div id="seleccion" style="width: 100%; float: left; height: 15%; position: inherit" class="bg-light">
                                       <asp:Panel ID="Panel_scroll" runat="server" ScrollBars="Auto" Style="height: 150px; background-color: white" class="modal_content_no_back_inferior">
                                           <asp:UpdatePanel ID="UpdatePanelseleccion" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                               <ContentTemplate>
                                                   <input id="Hidden_selccion_documento_eliminar_wf" type="hidden" value="" runat="server" />
                                                   <input id="Hidden_selccion_documento_eliminar_split_wf" type="hidden" value="" runat="server" />
                                                   <input id="Hidden_selccion_documento_cambia_tipo_wf" type="hidden" value="" runat="server" />
                                                   <input id="Hidden_selccion_documento_cambia_tipo_split_wf" type="hidden" value="" runat="server" />
                                                   <input id="hiden_seleccion_documento_wf" type="hidden" value="" runat="server" />
                                                   <input id="hiden_seleccion_documento_id_wf" type="hidden" value="" runat="server" />
                                                   <input id="Hidden_numero_doc_rel_wf" type="hidden" value="0" runat="server" />
                                                   <asp:GridView ID="GridView_list_documento_relacion_wf" runat="server" Style="position: inherit; width: 100%; font-size: 14px"
                                                       AutoGenerateSelectButton="False" AllowSorting="false" AllowPaging="false" PageSize="6" PagerSettings-Position="Top" CssClass="table  font-weight-light" GridLines="None"
                                                       EnableViewState="true">
                                                       <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                                       <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                                       <PagerStyle CssClass="pagination-ys" />
                                                       <Columns>
                                                           <asp:TemplateField>
                                                               <HeaderTemplate>
                                                                   <asp:Panel ID="Panel_che_box_aling" runat="server" Style="text-align: left; max-width: 5px">
                                                                       <asp:CheckBox ID="chk_selec" Text="" CssClass="btn   btn-light btn-sm border-0 bg-transparent" runat="server" onclick="table_gred_on_click_check(this,'GridView_list_documento_relacion_wf','chek_selecion_list_wf');" />

                                                                   </asp:Panel>
                                                               </HeaderTemplate>
                                                           </asp:TemplateField>
                                                       </Columns>
                                                   </asp:GridView>

                                                   <input id="Hiddenint" type="hidden" value="0" runat="server" />
                                               </ContentTemplate>
                                           </asp:UpdatePanel>
                                       </asp:Panel>
                                   </div>
                               </div>

                               <a id="da_show-sidebar_" class="btn btn-sm   show_da_slide_rigth  " title="Visualiza indice" style="top: 50%" href="#" data-target="#sidebar__">
                                   <i style="color: white" class="fas fa-bars"></i>
                               </a>
                               <div id="contenido_indice"
                                   style="width: 20%; position: inherit; left: auto; height: 100%; margin: 0px 0px 0px 0px; float: right; background-color: white; display: none" class="modal_content_no_back_left">
                                   <div id="div_conent_indice">
                                       <div id="title_indice" class="modal-header_ modal_title_superior  p-2 modal_content_no_back_inferior" style="border-top-left-radius: initial; border-top-right-radius: initial">
                                           <h6 class=" mt-2 mb-2 ml-2 font-weight-normal" id="pit_" style="color: #0062cc; float: left; font-family: 'Segoe UI'">Indice </h6>
                                           <a id="sidebarCollapse" class="close_ mr-2" title="Oculta indice" style="float: right; height: 10px; color: #0062cc"><i class="fal fa-times   font-weight-light"></i></a>
                                       </div>
                                       <asp:UpdatePanel ID="UpdatePanelindice" runat="server" UpdateMode="Conditional"
                                           RenderMode="Inline">
                                           <ContentTemplate>
                                               <asp:Panel ID="Panel_indice" runat="server" ScrollBars="Auto" CssClass="pl-1"
                                                   Height="98%" EnableViewState="true">
                                               </asp:Panel>
                                               <asp:Panel ID="div_buton" runat="server" Style="text-align: center; background-color: white" Visible="false" CssClass="modal_content_no_back_inferior">
                                                   <ul class="navbar-nav">
                                                       <li class="nav-item active  active_">
                                                           <a class="nav-link" id="a_lement_actualiza_index" href="#" onclick="event_element_clic(event,this);">
                                                               <i style="margin-left: 1px; margin-top: 7px; color: #0062cc" class="fad fa-save"></i>
                                                               <span id="Span1" style="color: #6d7fcc" title="Guardar los cambios"> Guardar  </span>
                                                           </a>
                                                       </li>
                                                   </ul>
                                               </asp:Panel>
                                               <input id="Hiddenheih" type="hidden" value="0" runat="server" class="dec_000_21_000" />
                                               <input id="Hidden_image_gabinete" type="hidden" value="0" runat="server" class="dec_000_21_000" />
                                               <input id="Hidden_id_inventario" type="hidden" value="0" runat="server" class="dec_000_21_000" />
                                               <input id="Hidden_id_serie" type="hidden" value="0" runat="server" class="dec_000_21_000" />
                                               <input id="Hidden_id_sub_serie" type="hidden" value="0" runat="server" class="dec_000_21_000" />
                                               <input id="Hidden_id_documento" type="hidden" value="0" runat="server" class="dec_000_21_000" />
                                               <input id="Hidden_id_area" type="hidden" value="0" runat="server" class="dec_000_21_000" />
                                               <input id="Hidden_id_tipo" type="hidden" value="0" runat="server" class="dec_000_21_000" />
                                               <input id="Hidden_id_expediente" type="hidden" value="0" runat="server" class="dec_000_21_000" />
                                               <input id="Hidden_id_tipo_expediente" type="hidden" value="0" runat="server" class="dec_000_21_000" />
                                               <input id="Hidden_id_unidad_conservacion" type="hidden" value="0" runat="server" class="dec_000_21_000" />
                                               <input id="Hidden_id_tipo_unidad_conservacion" type="hidden" value="0" runat="server" class="dec_000_21_000" />
                                           </ContentTemplate>
                                           <Triggers>
                                           </Triggers>
                                       </asp:UpdatePanel>
                                       <asp:UpdatePanel ID="Updatepanel_actualiza" runat="server" UpdateMode="Conditional">
                                           <ContentTemplate>
                                               <asp:Button ID="Button_actualiza_indice_imagen" runat="server" Text="Actualiza Indice" CssClass="btn btn-success" ToolTip="Actualiza Indice documento" Style="display: none" />
                                           </ContentTemplate>
                                       </asp:UpdatePanel>

                                   </div>
                               </div>
                               <div id="contenido_imagen"
                                   style="width: 75%; position: relative; left: auto; float: left; height: 100%; margin: 0px 0px 0px 0px">
                                   <div id="div_cerrar" class="modal-header_ modal_title_superior " style="border-top-left-radius: initial; border-top-right-radius: initial; border-bottom: 1px solid #e9ecef; display:none">
                                          <h6 id="titel_visor" class="mt-2 mb-2 ml-2  h6 font-weight-light" style="color: #6d7fcc; font-family: 'Segoe UI'; float: left">Visor externo</h6>
                                          <button type="button" title="Cerrar ventana visualizador" onclick="prevent_cerrar(event,this);" class="close mr-1" style="float: right">&times;</button>
                                      </div>
                                   <asp:UpdatePanel ID="UpdatePanel_content_iframe" runat="server" UpdateMode="Conditional"
                                       RenderMode="Inline">
                                       <ContentTemplate>
                                           <asp:Panel ID="panel_content_iframe" runat="server" Visible="false" style="height:100%">
                                               <asp:UpdatePanel ID="UpdatePanel_panel_toll" runat="server" UpdateMode="Conditional"
                                                   RenderMode="Inline">
                                                   <ContentTemplate>
                                                       <asp:Panel ID="Panel_tolbar_pdf" runat="server" Visible="false" Style="display: inline-flexbox; width: 100%; height: auto" class="navbar navbar-expand-sm  p-2  pb-1 pl-0 pt-1 modal_content_no_back_inferior">
                                                           <div class="nav  ml-1">
                                                              
                                                               <div class="nav-item_ active active_">
                                                                   <a id="id_indice_wf_pdf" class="nav-link" style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Ver meta datos de documento" href="#" onclick="event_element_clic(event,this);"><i style="" class="fas fa-file-invoice"></i></a>
                                                               </div>
                                                           </div>
                                                       </asp:Panel>
                                                       <asp:ImageButton ID="ImageButtonadjunta" runat="server" Style="display: none" />
                                                       <asp:ImageButton ID="ImagenAdjuntaAutomatico" runat="server" Style="display: none" />
                                                   </ContentTemplate>
                                               </asp:UpdatePanel>
                                               <asp:UpdatePanel ID="UpdatePanelVisor" runat="server" UpdateMode="Conditional"
                                                   RenderMode="Inline">
                                                   <ContentTemplate>
                                                       <iframe id="ifrm_visor_" runat="server" style="border-style: none; left: 0px; width: 100%; height: 100%; position: relative; top: 1px; z-index: auto; right: 1px"
                                                           frameborder="0" scrolling="no"></iframe>

                                                   </ContentTemplate>
                                                   <Triggers>
                                                   </Triggers>
                                               </asp:UpdatePanel>
                                               <!--guarda el alto del ifrmviso-->
                                               <input id="HiddenHeigth" type="hidden" value="560" runat="server" />
                                               <div id="oculto" style="display: none; width: 5px;">
                                                   <asp:UpdatePanel ID="UpdatePanel_actualiza_tab" runat="server" UpdateMode="Conditional"
                                                       RenderMode="Inline">
                                                       <ContentTemplate>
                                                           <asp:Button ID="Buttonuser" runat="server" Height="0px" Visible="True"
                                                               Width="0px" />
                                                           <asp:Button ID="Buttonactividad" runat="server" Height="0px" Visible="True"
                                                               Width="0px" />
                                                           <asp:Button ID="ButtonTreeviewSeleccion" runat="server" Height="0px" Visible="True"
                                                               Width="0px" />
                                                       </ContentTemplate>
                                                   </asp:UpdatePanel>
                                               </div>
                                           </asp:Panel>
                                       </ContentTemplate>
                                   </asp:UpdatePanel>
                                   <asp:UpdatePanel ID="UpdatePanel_content_image_draw" runat="server" UpdateMode="Conditional"
                                       RenderMode="Inline">
                                       <ContentTemplate>
                                           <asp:Panel ID="panel_content_image_draw" runat="server" Visible="false" Style="height: 100%">
                                               <div id="tollimage" style="border-bottom: 1px solid #ddd; display: inline-flexbox; width: 100%" class="navbar navbar-expand-sm   pb-0 pl-0 pt-1">
                                                   <button class="navbar-toggler btn btn-light " style="padding-bottom: 2px" type="button" data-toggle="collapse" data-target="#navbarNavDropdown">
                                                       <span class="pb-1"><i style="color: white" class="fas fa-bars"></i></span>
                                                   </button>
                                                   <div class="collapse navbar-collapse   pb-1 pt-0" id="navbarNavDropdown__">
                                                       <div class="nav  ml-1">
                                                           <div class="nav-item active active_ ">
                                                               <a class="nav-link" style="color: #6d7fcc" title="Primera  imagen" href="#" onclick="activa_boton_client_server('ImageButtonInicio')"><i style="font-size: 20px" class="fad fa-arrow-alt-to-left  "></i></a>
                                                           </div>
                                                           <div class="nav-item active active_ ">
                                                               <a class="nav-link  " style="margin-left: 2px; margin: 2px; width: auto; color: #6d7fcc" title="Anterior imagen" href="#" onclick="activa_boton_client_server('ImageButtonAnterior')"><i style="font-size: 20px" class="fad fa-arrow-alt-left "></i></a>
                                                           </div>
                                                           <asp:UpdatePanel ID="UpdatePanel_conte_bot" runat="server"
                                                               UpdateMode="Conditional" RenderMode="Inline">
                                                               <ContentTemplate>
                                                                   <div class="nav-item active ">
                                                                       
                                                                       <asp:TextBox ID="LabelConteo" runat="server" Style="margin-left: 5px; margin-right: 5px; text-align: center; margin-top: 3px; font-size: 12px; width: 50px; font-family: 'Segoe UI Emoji'" onkeypress="preven_event_search_keypres_enter(event,this);"></asp:TextBox>
                                                                   </div>
                                                               </ContentTemplate>
                                                               <Triggers>
                                                                     <asp:AsyncPostBackTrigger ControlID="Button_update_update_adjunto_doc_visor" EventName="Click" />
                                                              </Triggers>
                                                           </asp:UpdatePanel>
                                                           <div class="nav-item active  active_">
                                                               <a class="nav-link " style="color: #6d7fcc" title="Siguiente imagen" href="#" onclick="activa_boton_client_server('ImageButtonSiguiente')"><i style="font-size: 20px" class="fad fa-arrow-alt-right "></i></a>
                                                           </div>

                                                           <div class="nav-item active active">
                                                               <a class="nav-link " style="color: #6d7fcc" title="Ultima  imagen" href="#" onclick="activa_boton_client_server('ImageButtonFinal')"><i style="font-size: 20px" class="fad fa-arrow-alt-to-right "></i></a>
                                                           </div>

                                                           <div class="nav-item active active_">
                                                               <a class="nav-link " style="color: #6d7fcc" title="Alejar Imagen" href="#" onclick="activa_boton_client_server('ImageMenos')"><i style="font-size: 20px" class="fad fa-minus-circle "></i></a>
                                                           </div>

                                                           <div class="nav-item active active_">
                                                               <a class="nav-link " style="color: #6d7fcc" title="Acercar Imagen" href="#" onclick="activa_boton_client_server('ImageMas') "><i style="font-size: 20px" class="fad fa-plus-circle "></i></a>
                                                           </div>
                                                           <asp:UpdatePanel ID="UpdatePanel_drows_bot" runat="server"
                                                               UpdateMode="Conditional" RenderMode="Inline">
                                                               <ContentTemplate>
                                                                   <asp:DropDownList ID="DropDownList_zom" runat="server" AutoPostBack="True" class=" mr-1 ml-1">
                                                                       <asp:ListItem Value="50"></asp:ListItem>
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
                                                               </ContentTemplate>
                                                               <Triggers>
                                                                   <asp:AsyncPostBackTrigger ControlID="ImageMenos" EventName="Click" />
                                                                   <asp:AsyncPostBackTrigger ControlID="ImageMenos" EventName="Click" />
                                                               </Triggers>
                                                           </asp:UpdatePanel>

                                                           <div class="nav-item_ active active_">
                                                               <a class="nav-link " style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Rotar 45 grados a la izquierda" href="#" onclick="activa_boton_client_server('ImageRotate45')"><i style="" class="fad fa-undo "></i></a>
                                                           </div>
                                                           <asp:TextBox ID="TextBox2" Style="display: none" class=" mr-0 ml-0" Width="74px" runat="server" placeholder="" onkeypress="preven_event_search_keypres_enter(event,this);"></asp:TextBox>
                                                           <div class="nav-item_ active active_" style="display: none">
                                                               <a class="nav-link " style="color: #6d7fcc" title="Ir a imagen" href="#" onclick="activa_boton_client_server('ImageButton_ir_pagina')"><i style="color: white; font-size: 20px" class="fas fa-search "></i></a>
                                                           </div>
                                                           <div class="nav-item_ active active_">
                                                               <a class="nav-link " id="ImageButtonguardardocumento_" style="color: #6d7fcc" title="Descargar imagenes" href="#" onclick="activa_boton_client_server('ImageButtonguardardocumento')"><i style="font-size: 20px" class="fad fa-arrow-to-bottom "></i></a>
                                                           </div>
                                                           <div class="nav-item_ active active_">
                                                               <a class="nav-link " style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Imprimir documento" href="#" onclick="activa_boton_client_server('ImageButtonimprimir')"><i style="" class="fad fa-print "></i></a>
                                                           </div>
                                                           <div id="ImageFirma_" style="display:none" class="nav-item_ active active_">
                                                               <a class="nav-link " style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Firma Imagen" href="#" onclick="firma_mecanica();"><i style="" class="fas fa-file-signature "></i></a>
                                                           </div>
                                                           <div id="ImageButtonadjunta_" class="nav-item_ active active_">
                                                               <a class="nav-link " style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Adjuntar documento" href="#" onclick="inicializa_tipo_adjunto_documento(event,this,'C-DW-VIS')"><i style="" class="fas fa-upload "></i></a>
                                                           </div>
                                                           
                                                           <div class="nav-item_ active active_" style="display:none">
                                                               <a id="id_indice_wf" class="nav-link" style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Adjuntar documento desde servicio web" href="#" onclick="inicializa_tipo_adjunto_documento(event,this,'C-DW-AUTO')"><i style="" class="fas fa-page-break"></i></a>
                                                           </div>
                                                           <div class="nav-item_ active active_">
                                                               <a id="id_indice_wf_pdf_draw" class="nav-link" style="color: #6d7fcc; font-family: Arial; text-decoration: none; font-weight: 600" title="Ver meta datos de documento" href="#" onclick="event_element_clic(event,this);"><i style="" class="fas fa-file-invoice"></i></a>
                                                           </div>
                                                       </div>
                                                       <input id="Hidden2" type="hidden" value="" runat="server" />
                                                   </div>
                                               </div>
                                               <asp:UpdatePanel ID="Updatepanel_boton_content" runat="server" UpdateMode="Conditional">
                                                   <ContentTemplate>
                                                       <asp:ImageButton ID="ImageButtonInicio" runat="server" ToolTip="Primera  imagen" ImageUrl="../imagewf/inicio14.png" Style="display: none" />
                                                       <asp:ImageButton ID="ImageButtonAnterior" runat="server" ToolTip="Anterior imagen" ImageUrl="../imagewf/anterior15.png" Style="display: none" />
                                                       <asp:ImageButton ID="ImageButtonguardar" runat="server" ToolTip="Guardar firma" Style="display: none" />
                                                       <asp:ImageButton ID="ImageButtonSiguiente" runat="server" ToolTip="Siguiente imagen" ImageUrl="../imagewf/siguiente15.png" ImageAlign="NotSet" Style="display: none" />
                                                       <asp:ImageButton ID="ImageButtonFinal" runat="server" ToolTip="Ultima  imagen" ImageUrl="../imagewf/final15.png" Style="display: none" />
                                                       <asp:ImageButton ID="ImageButton_ir_pagina" runat="server" ToolTip="Ir a imagen" ImageUrl="../Docuarchi/imagenes/busca_pagina.png" Visible="true" Style="display: none" />
                                                       <asp:ImageButton ID="ImageMenos" runat="server" ToolTip="Alejar Imagen" ImageUrl="../imagewf/alejarimagen.png" Style="display: none" />
                                                       <asp:ImageButton ID="ImageMas" runat="server" ToolTip="Acercar Imagen" ImageUrl="../imagewf/acercarimagen.png" Style="display: none" />
                                                       <asp:ImageButton ID="ImageRotate45" runat="server" ToolTip="Rotar 90 grados" ImageUrl="../Docuarchi/imagenes/rotar90.png" Style="display: none" />
                                                       <asp:ImageButton ID="ImageRotate180" runat="server" ToolTip="Rotar 180 grados" ImageUrl="../Docuarchi/imagenes/rotar180.png" Style="display: none" />
                                                       <asp:ImageButton ID="ImageRotate270" runat="server" ToolTip="Rotar 270 grados" ImageUrl="../Docuarchi/imagenes/rotar270.png" Style="display: none" />
                                                       <asp:ImageButton ID="ImageButtonimprimir" runat="server" ToolTip="Imprimir documento" ImageUrl="../Docuarchi/imagenes/imprimir30.png" Visible="true" Style="display: none" />
                                                       <asp:ImageButton ID="ImageButtonguardardocumento" runat="server" ToolTip="Guadar documento" Style="display: none" ImageUrl="../Docuarchi/imagenes/guardarimagen.png" Visible="true" />
                                                       <asp:ImageButton ID="ImageButtoninfo" runat="server" ToolTip="Información Documento" ImageUrl="../Docuarchi/imagenes/infoimagen.png" Style="display: none" Visible="true" />
                                                       <asp:ImageButton ID="ImageFirma" runat="server" ToolTip="Firma Imagen" ImageUrl="../imagewf/firma.png" Style="display: none" OnClientClick="firma_mecanica();" />
                                                       <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="../imagewf/adjunta_image.png" ToolTip="Adjunta imagen a documento" Style="display: none" />

                                                   </ContentTemplate>
                                               </asp:UpdatePanel>
                                               <div id="content" style="width: 100%; height: 88%; position: absolute; background-color: Gray; filter: alpha(opacity=70); opacity: 50; overflow: scroll; border-bottom-width: 0.5px; border-left-width: 1px; border-right-width: 1px; border-top-width: 2px; left: 0px; display: block">
                                                   <div id="zona" style="width: auto; height: auto; position: absolute;">
                                                       <asp:UpdatePanel ID="UpdatePanel_noaming" runat="server" UpdateMode="Conditional">
                                                           <ContentTemplate>
                                                               <neoimg:ImageDraw ID="noaming" runat="server" Style="position: relative" RenderingMethod="HttpHandler" HttpHandlerName="ImageGenerator.axd">
                                                               </neoimg:ImageDraw>
                                                               <div id="draggable" class="ui-widget-content" style="background-color: Gray; display: none; position: absolute">
                                                                   <img id="img" alt="Firma Mecanica"
                                                                       align="bottom" style="border-style: none" />
                                                               </div>
                                                           </ContentTemplate>
                                                           <Triggers>
                                                           </Triggers>
                                                       </asp:UpdatePanel>
                                                   </div>

                                               </div>
                                           </asp:Panel>
                                       </ContentTemplate>
                                   </asp:UpdatePanel>
                                   <input id="Hiddenintercambio" type="hidden" value="0" runat="server" />
                                   <input id="Hiddenintercambio2" type="hidden" value="0" runat="server" />
                               </div>
                               <div id="content_pie_seleccion_tarea" style="width: 100%; clear: both" class="p-2  modal_content_no_back_superior nav_botota_person_gray_">
                                   <asp:UpdatePanel ID="UpdatePanel_estado_tarea" runat="server" UpdateMode="Conditional"
                                       RenderMode="Inline">
                                       <ContentTemplate>
                                           <div class="row">
                                               <div class="col-8">
                                                   <asp:Label ID="Label_estado_tarea_selecion" runat="server" Text="Estado" Style="color: #6d7fcc" CssClass="h6  font-weight-light"></asp:Label>
                                               </div>
                                               <div class="col-4">
                                                   <asp:Label ID="Label_estado_selecion" runat="server" Text="" Style="color: #6d7fcc; float: right" CssClass="font-weight-light h6"></asp:Label>
                                               </div>
                                           </div>
                                       </ContentTemplate>
                                   </asp:UpdatePanel>
                               </div>
                           </div>              
                   </div>
                   </div> 
                   <div style="display: none; height: 1px">
                       <asp:Button ID="Button_lista_imagenes_gestion" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                       <asp:Button ID="ButtonSalir_lista_imagenes_gestion_" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                       <asp:Button ID="Button_cerrar_lista_imagenes_gestion" runat="Server" Text="X" CssClass="invisible" />
                   </div>
              </asp:Panel>
        <input id="HiddenPROMP" type="hidden" value="0" runat="server"/>
         <asp:UpdatePanel ID="UpdatePanel_seleccion_treview" runat="server" UpdateMode="Conditional" RenderMode="Inline">
              <ContentTemplate>
                  <input id="Hidden_0004" type="hidden" value="1" runat="server" />
                  <asp:Button ID="Button_selecion_treview_documento" runat="server" Text="Button" Style="display: none" />
                  <input id="hidden_selecion_documento_treview" type="hidden" value="" runat="server"/>
                  <input id="hidden_selecion_actualiza_treview" type="hidden" value="" runat="server"/>
                  <input id="hidden_estado_seleccion" type="hidden" value="" runat="server"/>
                  <input id="Hidden_confir_elimina" type="hidden" value="" runat="server" />
                  <asp:Button ID="Button_eliminar_documento" runat="server" Text="" Style="display: none"  />
                  <asp:Button ID="Button_clasficar_documento" runat="server" Text="" Style="display: none" />
                  <asp:Button ID="Button_Actualizar_seleccion_indice_wf" runat="server" Text="Actualizar" Style="display: none" OnClientClick="actualiza_treview_seleccion();" />
                  <asp:Button ID="Button_buton_actualiza_seleccion" runat="server" Text="Button" Style="display: none" OnClientClick="actualiza_treview_seleccion();" />
                  <input id="Button_buton_actualiza_seleccion__" type="button" value="button" style="display: none" onclick="actualiza_treview_seleccion();" />
              </ContentTemplate>
          </asp:UpdatePanel>
           <!--cargar documento anexo!-->
          <div id="contenido_procesa_sube_documento_adjunto" >
            <asp:Panel ID="Panel_sube_documento_adjunto" runat="server" Style="display:none;  width: 70%; height: 100%" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_sube_documento_adjunto" runat="Server" BackgroundCssClass="FondoAplicacion" 
                    TargetControlID="Button_sube_documento_adjunto"
                    PopupControlID="Panel_sube_documento_adjunto" CancelControlID="Button3_cerrar_adjunta" ></asp:ModalPopupExtender>
                <div id="modal_content_sube_documento_adjunto" class="modal-content">  
                    <div id="Div_cabecera" class="modal_title_superior_ modal-header"> 
                        <h6 class="modal-title d-inline ml-1">Adjuntar</h6>  
                        <button type="button" value="Button3_cerrar_adjunta_" onclick="hide_upload_content('ModalPopupExtender_sube_documento_adjunto');" class="close da_event_captive_ ">&times;</button>                   
                   </div>            
                    <div id="Div_contenido_adjunta" style="height: auto; width: 100%; border-top: none" class="modal_content_back p-2">
                        <div id="content_option_chek_adjunto_doc_visor"> 
                        <asp:UpdatePanel ID="Update_actualiza_adjunta_documento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender1_" runat="server" TargetControlID="Check_anexo_radicado_adj"
                                     Key ="radicado_"></asp:MutuallyExclusiveCheckBoxExtender>
                                    <asp:mutuallyexclusivecheckboxextender id="Mutuallyexclusivecheckboxextender2_" runat="server" targetcontrolid="CheckBox_relacionado_radicado_adj"
                                     Key ="radicado_"></asp:mutuallyexclusivecheckboxextender>  
                                <div class=" row pl-1 pr-1">         
                                    <div class="col-6">
                                       <div class="row">
                                            <div class="col-0 pl-3 pr-2 pb-2 pt-2">
                                                <asp:CheckBox ID="Check_anexo_radicado_adj" runat="server" Text="" Checked="false"  Font-Size="11"  onchange="upload_adjunto_doc_visor_event_cheked_adjunto(event)"  Enabled="true" CssClass="h6 font-weight-light"  />
                                            </div>
                                           <div class="col-8 pl-0  pr-0 pb-2 pt-2">
                                               <asp:Label ID="h_adjunto_adjunto_doc_visor" class="pl-0 font-weight-light h6"  runat="server" Text="Adjuntar como parte del documento"></asp:Label>
                                                   
                                            </div>
                                       </div>
                                        
                                    </div>
                                     <div class="col-6">
                                          <div class="row">
                                              <div class="col-0 pl-0 pr-2 pb-2 pt-2">
                                                   <asp:CheckBox ID="CheckBox_relacionado_radicado_adj" runat="server" Text="" Checked="true" onchange="upload_adjunto_doc_visor_event_cheked_relacion(event)"  Font-Size="11" CssClass="h6 font-weight-light" Enabled="true" />
                                              </div>
                                              <div class="col-8 pl-0  pr-0 pb-2 pt-2">
                                                  <asp:Label ID="h_relacion_adjunto_doc_visor" class="pl-0 font-weight-light h6"  runat="server" Text="Adjuntar como documento relacionado"></asp:Label>
                                                 
                                              </div>
                                          </div>
                                        
                                    </div>
                                </div>                  
                                <div id="content_data_grid_adjunta_documento" class="conten_gred_border_" style="width: 100%">
                                    <asp:DropDownList ID="DropDownList_adjunta_documento" Style="width: 100%" CssClass="custom-select mr-sm-2" runat="server"></asp:DropDownList>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                            </Triggers>
                        </asp:UpdatePanel>
                        </div>
                        <div class="p-2">
                            <div class="row p-2" id="content_boton_adjunto_doc_visor">
                                <div class="col-12 p-0">
                                    <div class="file-select " id="src-file">
                                        <input id="file_element_adjunto_doc_visor" type="file" multiple="multiple"  accept=""  style="width: 100px; height: 40px" name="src-file" class="p-1" contente_file="ModalPopupExtender_sube_documento_adjunto" aria-label="Archivo"  />
                                    </div>
                                    <a id="save_file_element_adjunto_doc_visor" title="Guardar todos los archivos"  class="btn  btn-success" style="opacity: 0; color:white"><i style="color: white" class="fas fa-save "></i> Guardar </a>   
                                    <a id="delete_file_element_adjunto_doc_visor" title="Elminar todos los archivos cargados"  class="btn  btn-danger " style="opacity: 0; color:white"><i style="color: white" class="fal fa-trash-alt "></i> Eliminar </a>
                                     <a id="cancel_file_element_adjunto_doc_visor" title="Cancelar guardar archivos"  class="btn  btn-warning" style="opacity: 0; color:white"><i style="color: white" class="fas fa-window-close "></i> Cancelar </a>
                                </div>
                            </div>            
                            <div class="paren_element background_upload" id="conten_file_element_adjunto_doc_visor" style="overflow: auto; height: 100%">
                                
                                  <div id="content_drop_element_adjunto_doc_visor" claas="">
                                       
                                     
                                 </div>
                                 <table id="table_file_element_adjunto_doc_visor" class="table table-striped">
                                 </table>
                            </div>
                            <div class="row border pt-2" id="content_pie_title_adjunto_doc_visor">
                                <div class="col-8">
                                    <div class="row p-2">
                                        <div class="col-4 p-0">
                                            <div >
                                                <asp:Label ID="Label_progres_bar_file_element_adjunto_doc_visor" runat="server" Text="" Style="font-family: Arial; text-align: center; font-size: 20px"></asp:Label>
                                            </div>
                                            <div id="pogres_file_element_contador_adjunto_doc_visor" style="text-align: center; font-family: Arial; font-size: 14px">
                                            </div>
                                            <div id="pogres_file_element_porcent_adjunto_doc_visor" style="text-align: center; font-family: Arial; font-size: 14px">
                                            </div>
                                        </div>
                                        <div class="col-5 p-0">
                                            <div>
                                                <div id="myProgress_file_element_adjunto_doc_visor">
                                                    <div id="myBar_file_element_adjunto_doc_visor" class="file-select-bar"></div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-3 p-0 pl-3">
                                            <p id="count_byte_file_element_adjunto_doc_visor"></p>    
                                        </div>
                                    </div>
                                    
                                </div>
                                <div class="col-4 justify-content-end pt-2">
                                    <p id="count_file_element_adjunto_doc_visor" class="font-weight-light" style="float: right"> Estado </p>
                                </div>
                            </div>
                        </div>
                        <asp:Panel ID="Panel_descarga_ajax" runat="server">               
                            <div id="drop_zone_" style="width: 100%; height: auto; display: none">
                                <asp:UpdatePanel ID="UpdatePanel_descarga" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                         <asp:Button ID="Button_update_update_adjunto_doc_visor" runat="server" Text="Button" />
                                        <asp:AjaxFileUpload ID="AjaxFileUpload_dowload" runat="server" ThrobberID="drop_zone_"
                                            ContextKeys="fred"
                                            AllowedFileTypes="tif,jpg,tiff,bmp,pdf"
                                            MaximumNumberOfFiles="1" OnClientUploadComplete="activa_boton_dowload" />
                                        <asp:Button ID="Button_guardar_desicion" runat="server" Text="Button" Style="display: none" />
                                        &nbsp  
                                        <asp:Label ID="Label_estado_carga" runat="server" Text="Estado" Style="font-size: 10px" CssClass="font-weight-light h6"></asp:Label>
                                        <input id="Hidden_result_load" type="hidden" value="" runat="server" />
                                        <input id="Hidden_tip_adjunt" type="hidden" value="" runat="server" />
                                        <input id="Hidden_date_row" type="hidden" value="" runat="server" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </asp:Panel>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button_sube_documento_adjunto" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                            <asp:Button ID="Button3_cerrar_adjunta" runat="Server" Text="" CssClass="invisible" />
                        </div>

                    </div>
                </div>
            </asp:Panel>     
        </div>
         <div id="Impresion_post">
            <asp:Panel ID="Panelimpresionpost" runat="server"  Style="display:none;  width:80%; height: auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtenderimpre_post" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post"
                    PopupControlID="Panelimpresionpost" CancelControlID="Buttoncerrarimpre_post">
                </asp:ModalPopupExtender>
                <div id="modal_content_Panelimpresionpost" class="modal-content">
                    <div id="divcabecer2_post" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Menú Impresión</h6>
                        <button type="button" value="Buttoncerrarimpre_post" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="ContenidoImpresion_post" style="border-top:none; overflow:auto; height: auto; width: 100%" class="modal_content_back_">
                        <asp:UpdatePanel ID="UpdatePaneliframe_post" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                                <iframe width="100%" height="100%" id="ifimpre_post_" frameborder="0" runat="server" src="../Radicador/WebFormDaImprimir.aspx"></iframe>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </asp:Panel>
            <div style="display:none; height:0px">
                 <asp:Button ID="Button1_post" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                 <asp:Button ID="ButtonSalir_post" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                 <asp:Button ID="Buttoncerrarimpre_post" runat="Server" Text="" CssClass="invisible"/>
            </div>
            
        </div>
         <div id="guardar_post">
            <asp:Panel ID="Panel_guardar" runat="server"  Style="display:none; width:80%; height: auto" CssClass="modal_content_general_">    
                <asp:ModalPopupExtender ID="ModalPopupExtender_guardar" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post_guardar"
                    PopupControlID="Panel_guardar" CancelControlID="Buttoncerrarimpre_post_guardar">
                </asp:ModalPopupExtender>
                <div id="modal_content_Panel_guardar" class="modal-content">
                    <div id="divcabecer2_post_guardar" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Guardar documento</h6>
                        <button type="button" value="Buttoncerrarimpre_post_guardar" class="close da_event_captive">&times;</button>   
                    </div>
                    <div id="Content_guardar_documento" style=" width: 100% ; border-top:none; overflow:auto" class="modal_content_back">
                        <asp:UpdatePanel ID="UpdatePane_guardar" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <iframe width="100%" height="100%" id="Iframe_guardar" runat="server" frameborder="0"></iframe>
                                
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button1_guardar" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                            <asp:Button ID="ButtonSalir_post_guardar" CssClass="invisible" runat="server" Text="" Height="0px" Width="0px" />
                            <asp:Button ID="Buttoncerrarimpre_post_guardar" runat="Server" Text="X" CssClass="modal_boton_hiden" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
        <asp:Panel ID="Panel_actualiza_tipologia_documental_workflow" runat="server" Style="display:none; width: 40%; height: auto" CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_actualiza_tipologia_documental_workflow" runat="server"
                TargetControlID="ButtonSalir_actualiza_tipologia_documental_workflow" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_actualiza_tipologia_documental_workflow" PopupControlID="Panel_actualiza_tipologia_documental_workflow">
            </asp:ModalPopupExtender>     
            <div id="modal_content_actualiza_tipologia_documental_workflow" class="modal-content">
                <div id="diver_cabcera_actualiza_tipologia_documental_workflow" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ">Tipologia documental</h6>
                    <button type="button" value="Button_cerrar_actualiza_tipologia_documental_workflow" class="close da_event_captive ">&times;</button>
                </div>
                <div id="contenido_procesa_actualiza_tipologia_documental_workflow" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                    <asp:UpdatePanel ID="Update_actualiza_tipologia_documental_workflow" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>                          
                            <div id="content_data_grid_actualiza_tipologia_documental_workflow" class="conten_gred_border_" style="width: 100%">
                                <asp:DropDownList ID="DropDownList_tipologia_documental_workflow" style="width:100%" CssClass="custom-select mr-sm-2" runat="server"></asp:DropDownList>
                            </div>                           
                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>

                    <div style="display: none; height: 1px">
                        <asp:Button ID="ButtonSalir_actualiza_tipologia_documental_workflow" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="Button_cerrar_actualiza_tipologia_documental_workflow" runat="Server" Text="X" CssClass="invisible" />
                    </div>
                </div>
                <div class="modal-footer justify-content-end" id="modal-footer_tipologia_documental_workflow">
                    <asp:UpdatePanel ID="UpdatePanel_boton_tipologia_documental_workflow" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Button ID="Button_actualiza_tipologia_documental_workflow" runat="server" Text="Aceptar" CssClass="btn btn-success" />
                            <input id="Hidden_resulta_botno_tipologia_documental_workflow" type="hidden" value="" runat="server"/>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
              
            </div>
        </asp:Panel>
         
       
         <!--Descarga plantilla radicado-->
        <div id="descarga_plantilla_radicada">
            <asp:Panel ID="Panel_descarga_plantilla_radicada" runat="server" Style="display: none; min-width: 400px; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_descarga_plantilla_radicada" runat="server" TargetControlID="ButtonSalir_descarga_plantilla_radicada" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_descarga_plantilla_radicada" PopupControlID="Panel_descarga_plantilla_radicada">
                </asp:ModalPopupExtender>
                <div class="modal-content">
                    <div id="div5" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title">Descargar protocolo de respuesta</h6>
                        <button type="button" value="Button_cerrar_descarga_plantilla_radicada" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="contenido_procesa_descarga_plantilla_radicada" style="width: 100%; height: 99%; color: black; border-top: none" class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_descarga_plantilla_radicada" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div style="margin-left: 15px; margin-right: 15px; margin-bottom: 15px; margin-top: 5px">
                                    <div class="form-group-sm">
                                        <asp:Label ID="Label2" runat="server" Style="text-align: center; font-family: Arial; font-size: 14px" Text="Tramite a responder*"></asp:Label>
                                        <asp:DropDownList ID="RE_Descripcion_Documento_ra" runat="server" CssClass="form-control" Style="width: auto">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group-sm" style="margin-top: 10px">
                                        <asp:Label ID="Label21" runat="server" Style="text-align: center; font-family: Arial; font-size: 14px" Text="A quien se dirige la respuesta"></asp:Label>
                                        <asp:TextBox ID="RE_REMITENTE_COR_REMITENTE_COR_VARCHAR_RA" runat="server" Enabled="False" Style="background-color: #F2F2F2" CssClass="form-control"></asp:TextBox>
                                        <asp:UpdatePanel ID="UpdatePanel_bton_ra" runat="server" RenderMode="Inline">
                                            <ContentTemplate>
                                                <asp:Button ID="Button_examinar_destinatario_ra" runat="server" CssClass="boton" OnClientClick="asigna_datos_heig_with()" Style="font-size: 12px; display: none" Text="gestion" />
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                    <div class="form-group-sm" style="margin-top: 10px">
                                        <asp:Label ID="Label_selec_firma" runat="server" Text="Usuario que firma el formato" Style="font-family: Arial; font-size: 14px"></asp:Label>
                                        <asp:UpdatePanel ID="UpdatePanel_descarga_formato_interface" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:DropDownList ID="DropDownList_lista_firma_interface" runat="server" Style="width: auto" CssClass="form-control"></asp:DropDownList>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                   
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="modal-footer align-content-end">
                        <button type="button" value="Button_cerrar_descarga_plantilla_radicada" class="btn btn-light da_event_captive">Cancelar </button>
                        <asp:UpdatePanel ID="UpdatePanel_boton_descarga_plantilla" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                               <asp:Button ID="Button_descarga_plantilla_radicado_resp" class="btn btn-success" runat="server"  Text="Aceptar" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_descarga_plantilla_radicada" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                        <asp:Button ID="ButtonSalir_descarga_plantilla_radicada" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                        <asp:Button ID="Button_cerrar_descarga_plantilla_radicada" runat="Server" Text="X" CssClass="invisible" />
                    </div>
                </div>

            </asp:Panel>
        </div>
         <!--detalle descarga_formato-->
        <asp:Panel ID="Panel_descarga_formato" runat="server" Style="display: none; overflow: hidden; width: auto; height: auto" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtender_descarga_formato" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_descarga_formato_dos"
                PopupControlID="Panel_descarga_formato" CancelControlID="ButtonSalir_descarga_formato">
            </asp:ModalPopupExtender>
            <div class="modal-content">
                <div id="Cabecerapendiente_descarga_formato" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title">Descarga formato respuesta</h6>
                    <button type="button" value="ButtonSalir_descarga_formato" class="close da_event_captive">&times;</button>
                </div>
                <div id="Cotenedorpendiente_descarga_formato" style="height: 100%; width: 100%" class="modal_content_back modal-body">
                    <asp:UpdatePanel ID="UpdatePanel_descarga_formato" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="row pl-1 pt-2">
                                <div class="col-6">
                                    <h6 class="modal-title  font-weight-normal">El usuario que firma</h6>
                                </div>
                                <div class="col-6">
                                    <asp:DropDownList ID="DropDownList_lista_firmas" runat="server" Style="" CssClass="form-control"></asp:DropDownList>
                                </div>      
                            </div>  
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer" id="modal_footer_regitra_formato">
                    <asp:UpdatePanel ID="UpdatePanel_boton_descarga_formato" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Button ID="Button_descarga_plantilla" runat="server" Text="Aceptar" Style="float: right; margin-right: 5px; margin-bottom: 10px" CssClass="btn btn-primary" />
                            <asp:Button ID="Button_cancela_descarga_plantilla" runat="server" Text="Cancelar" Style="float: right; margin-right: 5px; margin-bottom: 10px; margin-right: 5px" CssClass="btn btn-default" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="ButtonSalir_descarga_formato" runat="Server" Text="X" CssClass="invisible" Height="0px" Width="0px" />
                    <asp:Button ID="Button_descarga_formato_dos" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                </div>
            </div>

        </asp:Panel>
         <!--modal sube documento respuesta-->
        <div style="display:none">
            <asp:UpdatePanel ID="UpdatePanel_sube_documento_respuesta" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                     <asp:Button ID="Button_sube_documento" runat="server" Text="Button" style="display:none" />
                </ContentTemplate>
            </asp:UpdatePanel>          
        </div>
         <!--cargar documento respuesta!-->
            <asp:Panel ID="Panel_sube_documento_respuesta" runat="server" Style="display:none;  width: 60%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_sube_documento_respuesta" runat="server"  TargetControlID="ButtonSalir_sube_documento_respuesta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_sube_documento_respuesta" PopupControlID="Panel_sube_documento_respuesta"></asp:ModalPopupExtender>
                <div class="modal_content" id="modal_content_sube_documento_respuesta">
                    <div id="divcabecer2_sube_documento_respuesta" class="modal_title_superior_ modal-header">
                        <h6 id="Label_sube_documento_respuesta" class="modal-title">Adjunta documento respuesta</h6>
                        <button type="button" value="Button_cerrar_sube_documento_respuesta_" onclick="hide_upload_content('ModalPopupExtender_edition_sube_documento_respuesta');" class="close da_event_captive_">&times;</button>
                    </div>
                    <div id="contenido_procesa_sube_documento_respuesta" style="width: 100%; height: 100%; border-top:none" class="modal_content_back p-2">
                        <asp:UpdatePanel ID="UpdatePane_opcion_adjunta" runat="server" UpdateMode="Conditional" Visible="true">
                            <ContentTemplate>
                                <asp:Panel ID="Panel_opcion_adjunta" runat="server">
                                    <div id="Contenido_opcion_adjunta_respuesta" style="height: auto; width: 100%; background-color: white" class="content_option_selecion row">
                                        <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender1" runat="server" TargetControlID="Check_adjunta_formato"
                                            Key="radicado"></asp:MutuallyExclusiveCheckBoxExtender>
                                        <asp:MutuallyExclusiveCheckBoxExtender ID="Mutuallyexclusivecheckboxextender2" runat="server" TargetControlID="CheckBox_adjunta_documento_libre"
                                            Key="radicado"></asp:MutuallyExclusiveCheckBoxExtender>

                                        <div class="col-0 p-2">
                                            <asp:CheckBox ID="Check_adjunta_formato" runat="server" Text="" Checked="true" onchange="upload_adjunto_doc_respuesta_event_cheked_adjunto(event)" ForeColor="Black" Font-Size="10" Font-Names="Arial" Style="" AutoPostBack="true" />
                                        </div>
                                        <div class="col-5 p-2">
                                            <h6 class="pl-0 font-weight-light"> Adjunta documento formato respuesta</h6>
                                        </div>
                                        <div class="col-0 p-2">
                                            <asp:CheckBox ID="CheckBox_adjunta_documento_libre" runat="server" Text=" " onchange="upload_adjunto_doc_respuesta_libre_event_cheked_adjunto(event)"  Checked="false" ForeColor="Black" Font-Size="10" Font-Names="Arial" Style="" AutoPostBack="true" />
                                        </div>
                                        <div class="col-5 p-2">
                                            <h6 class="pl-0 font-weight-light "> Adjunta documento formato libre</h6>
                                        </div>
                                    </div>
                                </asp:Panel>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel ID="UpdatePanel_descarga_formato_adjunto_archivo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="Panel_descarga_formato_adjunto_archivo" runat="server">
                                    <asp:Label ID="Label_title_descarga_adjunto_archivo" runat="server" Text="Selecciona el usuario que firma el formato de respuesta" Style="font-family: Arial; font-size: 12px; margin-left: 5px; display: none"></asp:Label>
                                    <asp:DropDownList ID="DropDownList_lista_firmas_adjunto_archivo" runat="server" Style="width: 98%; margin-left: 5px; margin-top: 5px; margin-bottom: 5px"></asp:DropDownList>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div class="p-2">
                        <div class="row p-2" id="content_boton_adjunto_doc_respuesta">
                                <div class="col-12 p-0">
                                    <div class="file-select " id="src-file_">
                                        <input id="file_element_adjunto_doc_respuesta" type="file" multiple="multiple" accept="" style="width: 100px; height: 40px" name="src-file" class="p-1" contente_file="ModalPopupExtender_sube_documento_adjunto" aria-label="Archivo" />
                                    </div>
                                    <a id="save_file_element_adjunto_doc_respuesta" title="Guardar todos los archivos" class="btn  btn-success" style="opacity: 0; color:white"><i style="color: white" class="fas fa-save "></i> Guardar </a>
                                    <a id="delete_file_element_adjunto_doc_respuesta" title="Elminar todos los archivos cargados" class="btn  btn-danger " style="opacity: 0; color:white"><i style="color: white" class="fal fa-trash-alt "></i> Eliminar </a>
                                    <a id="cancel_file_element_adjunto_doc_respuesta" title="Cancelar guardar archivos" class="btn  btn-warning" style="opacity: 0; color:white"><i style="color: white" class="fas fa-window-close "></i> Cancelar </a>
                                </div>
                        </div>
                        <div class="paren_element background_upload" id="conten_file_element_adjunto_doc_respuesta" style="overflow: auto; height: 100%">

                                <div id="content_drop_element_adjunto_doc_respuesta" claas="">
                                </div>
                                <table id="table_file_element_adjunto_doc_respuesta" class="table table-striped">
                                </table>
                            </div>
                        <div class="row border pt-2" id="content_pie_title_adjunto_doc_respuesta">
                                <div class="col-8">
                                    <div class="row p-2">
                                        <div class="col-4 p-0">
                                            <div>
                                                <asp:Label ID="Label_progres_bar_file_element_adjunto_doc_respuesta" runat="server" Text="" Style="font-family: Arial; text-align: center; font-size: 20px"></asp:Label>
                                            </div>
                                            <div id="pogres_file_element_contador_adjunto_doc_respuesta" style="text-align: center; font-family: Arial; font-size: 14px">
                                            </div>
                                            <div id="pogres_file_element_porcent_adjunto_doc_respuesta" style="text-align: center; font-family: Arial; font-size: 14px">
                                            </div>
                                        </div>
                                        <div class="col-5 p-0">
                                            <div>
                                                <div id="myProgress_file_element_adjunto_doc_respuesta">
                                                    <div id="myBar_file_element_adjunto_doc_respuesta" class="file-select-bar"></div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-3 p-0 pl-2">
                                             <p id="count_byte_file_element_adjunto_doc_respuesta"></p>
                                        </div>
                                    </div>
                                   
                                </div>
                                <div class="col-4 justify-content-end pt-2">
                                    <p id="count_file_element_adjunto_doc_respuesta" class="font-weight-light" style="float: right">Estado </p>
                                </div>
                            </div>
                        <asp:UpdatePanel ID="UpdatePanel_Panel_descarga_ajax" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="Panel_sube_plantilla_respuesta" runat="server" style="display:none">
                                    <div id="drop_zone_22" style="width: 100%; height: auto; overflow: auto">
                                        <asp:UpdatePanel ID="UpdatePanel_sube_plantilla_respuesta" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:AjaxFileUpload ID="AjaxFileUpload_sube_plantilla_respuesta" runat="server" ThrobberID="drop_zone_22"
                                                    ContextKeys="fred_1_"
                                                    AllowedFileTypes="docx"
                                                    MaximumNumberOfFiles="1" OnClientUploadComplete="activa_boton_dowload_sube_plantilla" />
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                    
                                </asp:Panel>
                                <asp:Panel ID="Panel_descarga_externo" runat="server">
                                    <asp:Label ID="Label_estado_sube_plantilla_respuesta" runat="server" Text="Estado" Style="font-family: Arial; font-size: 10px; color: red"></asp:Label>
                                </asp:Panel>
                                &nbsp  
                               
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        </div>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button_cerrar_sube_documento_respuesta" runat="Server" Text="" CssClass="invisible" />
                            <asp:Button ID="ButtonSalir_sube_documento_respuesta" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        </div>

                    </div>
                    <div class="modal-footer">

                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="Panel_sube_anexo_respuesta" runat="server" Style="display:none;  width: 60%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_sube_anexo_respuesta" runat="server"  TargetControlID="ButtonSalir_sube_anexo_respuesta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_sube_anexo_respuesta" PopupControlID="Panel_sube_anexo_respuesta"></asp:ModalPopupExtender>
                <div class="modal_content" id="modal_content_sube_anexo_respuesta">
                    <div id="divcabecer2_sube_anexo_respuesta" class="modal_title_superior_ modal-header">
                        <h6 id="Label_sube_anexo_respuesta" class="modal-title"> Adjuntar enexos </h6>
                        <button type="button" value="Button_cerrar_sube_anexo_respuesta_" onclick="hide_upload_content('ModalPopupExtender_edition_sube_anexo_respuesta');" class="close da_event_captive_">&times;</button>
                    </div>
                    <div id="contenido_procesa_sube_anexo_respuesta" style="width: 100%; height: 100%; border-top:none" class="modal_content_back p-2">
                        <div class="p-2">
                        <div class="row p-2" id="content_boton_adjunto_anexo_respuesta">
                                <div class="col-12 p-0">
                                    <div class="file-select " id="src-file__">
                                        <input id="file_element_adjunto_anexo_respuesta" type="file" multiple="multiple" accept="" style="width: 100px; height: 40px" name="src-file" class="p-1" contente_file="ModalPopupExtender_sube_documento_adjunto" aria-label="Archivo" />
                                    </div>
                                    <a id="save_file_element_adjunto_anexo_respuesta" title="Guardar todos los archivos" class="btn  btn-success" style="opacity: 0"><i style="color: white" class="fas fa-save "></i> Guardar </a>
                                    <a id="delete_file_element_adjunto_anexo_respuesta" title="Elminar todos los archivos cargados" class="btn  btn-danger " style="opacity: 0"><i style="color: white" class="fal fa-trash-alt "></i> Eliminar </a>
                                    <a id="cancel_file_element_adjunto_anexo_respuesta" title="Cancelar guardar archivos" class="btn  btn-warning" style="opacity: 0"><i style="color: white" class="fas fa-window-close "></i> Cancelar </a>
                                </div>
                        </div>
                        <div class="paren_element background_upload" id="conten_file_element_adjunto_anexo_respuesta" style="overflow: auto; height: 100%">

                                <div id="content_drop_element_adjunto_anexo_respuesta" claas="">
                                </div>
                                <table id="table_file_element_adjunto_anexo_respuesta" class="table table-striped">
                                </table>
                            </div>
                        <div class="row border pt-2" id="content_pie_title_adjunto_anexo_respuesta">
                                <div class="col-8">
                                    <div class="row p-2">
                                        <div class="col-4 p-0">
                                            <div>
                                                <asp:Label ID="Label_progres_bar_file_element_adjunto_anexo_respuesta" runat="server" Text="" Style="font-family: Arial; text-align: center; font-size: 20px"></asp:Label>
                                            </div>
                                            <div id="pogres_file_element_contador_adjunto_anexo_respuesta" style="text-align: center; font-family: Arial; font-size: 14px">
                                            </div>
                                            <div id="pogres_file_element_porcent_adjunto_anexo_respuesta" style="text-align: center; font-family: Arial; font-size: 14px">
                                            </div>
                                        </div>
                                        <div class="col-5 p-0">
                                            <div>
                                                <div id="myProgress_file_element_adjunto_anexo_respuesta">
                                                    <div id="myBar_file_element_adjunto_anexo_respuesta" class="file-select-bar"></div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-3 p-0 pl-1">
                                             <p id="count_byte_file_element_adjunto_anexo_respuesta"></p>
                                        </div>

                                    </div>
                                   
                                </div>
                                <div class="col-4 justify-content-end pt-2">
                                    <p id="count_file_element_adjunto_anexo_respuesta" class="font-weight-light" style="float: right">Estado </p>
                                </div>
                            </div>
                       
                        </div>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button_cerrar_sube_anexo_respuesta" runat="Server" Text="" CssClass="invisible" />
                            <asp:Button ID="ButtonSalir_sube_anexo_respuesta" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        </div>

                    </div>
                   
                </div>
            </asp:Panel>
         <!--Radica documento respuesta no funcional-->
        <div id="radica_documento_respuesta">
            <asp:Panel ID="Panel_radica_documento_respuesta" runat="server" Style="display:none; color: White; width: 800px; height: 300px">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_radica_documento_respuesta" runat="server"  TargetControlID="ButtonSalir_radica_documento_respuesta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_radica_documento_respuesta" PopupControlID="Panel_radica_documento_respuesta" ></asp:ModalPopupExtender>
                <div id="div6" class="cabecera2">
                    <asp:Button ID="Button_radica_documento_respuesta" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_radica_documento_respuesta" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label_radica_documento_respuesta" runat="server" Text="Confirma y radica respuesta" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_radica_documento_respuesta" style="float: right">
                        <asp:Button ID="Button_cerrar_radica_documento_respuesta" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_radica_documento_respuesta" style="background-color: white; width: 100%; height: 99%;border: thin double #000080; color: black; background-color: #FFFFFF;">                                
                        <asp:UpdatePanel ID="UpdatePanel_radic_documento_respuesta" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                
                                <table style="width: 100%;">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label5" runat="server" Text="Tramite*" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label>

                                        </td>
                                         <td>
                                             <asp:DropDownList ID="RE_Descripcion_Documento" runat="server" style="width:600px"></asp:DropDownList>
                                         </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label10" runat="server" Text="Anexo*" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label>
                                        </td>
                                        <td><asp:TextBox ID="TextBoxanexo" runat="server" Style="width:600px">NO</asp:TextBox></td>
                                       
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label7" runat="server" Text="Destinatario*" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label>

                                        </td>
                                        <td>
                                            <asp:TextBox ID="RE_REMITENTE_COR_REMITENTE_COR_VARCHAR" runat="server" Style="background-color:yellow;width:600px" Enabled="False"></asp:TextBox> 
                                            <asp:UpdatePanel ID="UpdatePanel_bton" runat="server" RenderMode="Inline">
                                                <ContentTemplate>
                                                    <asp:Button ID="Button_examinar_destinatario" runat="server" Text="gestion" style="font-size:12px; display:none" CssClass="boton" OnClientClick="asigna_datos_heig_with()"/> 
                                                </ContentTemplate>
                                            </asp:UpdatePanel>

                                        </td>                           
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style=" align-content: center; margin-left:30px">
                                            <asp:CheckBox ID="CheckBox_envio_correo" runat="server" Style="text-align: center; font-family: Arial; font-size: 15px; margin-left:20px;color:red" Text="Confirmar respuesta al correo electrónico del peticionario" />
                                        </td>
                                      
                                    </tr>
                                    <tr>
                                        <td colspan="2" style=" align-content: center; margin-left:30px">
                                            <asp:CheckBox ID="CheckBox_confirma_envio_enexos" runat="server" Style="text-align: center; font-family: Arial; font-size: 15px; margin-left:20px;color:red" Text="Adjunta los anexos de la respuesta al correo eletrónico" Checked="true" />
                                        </td>
                                      
                                    </tr>
                                    <tr style="align-content:center">
                                          <td colspan="2" style=" align-content: center;">
                                            <asp:CheckBox ID="CheckBox_envia_ventanilla" runat="server" Style="text-align: center; font-family: Arial; font-size: 15px; margin-left:20px;color:red" Text="Solicita al centro de envío de correspondencia el envío de la respuesta" Checked="True" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td><asp:Label ID="Label13" runat="server" Text="Por favor separe por comas(,) los correos electronicos, ejemplo pepito@gmail.com,juan@hotmail.com" Style="text-align: center; font-family: Arial; font-size: 12px"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td><asp:Label ID="Label14" runat="server" Text="Correos electronicos" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label></td>
                                         <td>
                                            <asp:TextBox ID="TextBox_correo_electronico_interf" runat="server" Style="width:600px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    
                                    <tr>
                                        <td>

                                        </td>
                                        <td style="float:left"><asp:Button ID="Button_confirmar" runat="server" Text="Aceptar" Style="background-color: white; border-color: #b0c4de; height: 30px; width: 200px; height: 25px; text-align: center" CssClass="boton" /> &nbsp &nbsp
                                               <input id="Hidden_resultado_ventana" type="hidden" value="" runat="server"/>           
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>
                                             <asp:Label ID="Label15" runat="server" Text="Si produjo algún error y no se pudo notificar al correo electrónico intenta aquí" style=" font-family: Arial; font-size: 14px; color:red"></asp:Label> &nbsp &nbsp
                                            <asp:Button ID="Button_reintenta_notificar_correo" runat="server" Text="Reintentar" Style="background-color: white; border-color: #b0c4de; height: 30px; width: 100px; height: 25px; text-align: center" CssClass="boton" />
                                            
                                        </td>
                                    </tr>
                                    
                                </table>
                                                         
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
            </asp:Panel>
        </div>
        <!--Confirma envio respuesta-->
        <div id="confirma_envio_respuesta">
            <asp:Panel ID="Panel_confirma_envio_respuesta" runat="server" Style="display:none;  width: 50%; height:auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_confirma_envio_respuesta" runat="server"  TargetControlID="ButtonSalir_confirma_envio_respuesta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_confirma_envio_respuesta" PopupControlID="Panel_confirma_envio_respuesta" ></asp:ModalPopupExtender>
                <div class="modal-content">
                    <div id="div7" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title">Confirmar</h6>
                        <button type="button" value="Button_cerrar_confirma_envio_respuesta"  class="close da_event_captive">&times;</button>
                    </div>
                    <div id="contenido_procesa_confirma_envio_respuesta" style="width: auto; height: auto; margin-right: 5px; border-top:none" class="modal_content_back modal-body">
                        <div style="margin-left: 15px; margin-right: 15px">
                            <asp:UpdatePanel ID="UpdatePanel_confirma_envio_respuesta" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row" style="margin-top: 5px">

                                        <asp:CheckBox ID="CheckBox_envio_correo_ra" runat="server"  Text="" />
                                        <span class="ml-2" style="font-family: 'Segoe UI Emoji'; font-size: 13px"> Confirmar respuesta al correo electrónico del peticionario</span>
                                    </div>
                                    <div class="row">

                                        <asp:CheckBox ID="CheckBox_envia_ventanilla_ra" runat="server"  Checked="True" />
                                         <span class="ml-2" style="font-family: 'Segoe UI Emoji'; font-size: 13px"> Solicita al centro de envío de correspondencia el envío de la respuesta</span>
                                    </div>
                                    <div class="row">
                                        <asp:CheckBox ID="CheckBox_firma_digital" runat="server" Text=""  />
                                        <span class="ml-2" style="font-family: 'Segoe UI Emoji'; font-size: 13px"> Certificar digitalmente el documento de respuesta</span>
                                    </div>
                                    <div class="form-group-sm" style="margin-top: 13px">
                                        <h6 style="margin-left: 5px">Firma respuesta</h6>
                                        <asp:DropDownList ID="DropDownList_lista_firmas_confirma_respuesta" runat="server" Style="" CssClass=" custom-select"></asp:DropDownList>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                             <div class="form-group" style="margin-top: 13px">
                                <h6>Tipo respuesta</h6>
                                <asp:DropDownList ID="DropDownList_tipo_respuesta" runat="server"  CssClass=" custom-select"></asp:DropDownList>
                                
                            </div>
                            <div class="form-group" style="margin-top: 14px">
                                <h6>Correos electrónicos</h6> <span style="font-family: 'Segoe UI Emoji'; font-size: 13px; color:darkred"> (Para agregar una nueva dirección de correo electrónico por favor digite y  presione enter.)</span>
                                <select class="tokenize-callable-demo_respuesta_ form-control" multiple >
                                </select>
                            </div>
                            
                        </div>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button_cerrar_confirma_envio_respuesta" runat="Server" Text="X" CssClass="invisible" />
                            <asp:Button ID="Button_confirma_envio_respuesta" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                            <asp:Button ID="ButtonSalir_confirma_envio_respuesta" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                        </div>
                       
                    </div>
                    <div class="modal-footer align-content-end">  
                        <button type="button" title="Confirmar y enviar el documento respuesta" onclick="inicializa_tipo_adjunto_documento(event,this, 'R-R-R-F');"  class="btn btn-success   mt-1"> Aceptar</button>
                        <button type="button" title=""  value="Button_cerrar_confirma_envio_respuesta" class="btn btn-light da_event_captive  mt-1"> Cancelar </button>
                        <input id="Hidden_resultado_envio" type="hidden" value="" runat="server"/>
                       
                    </div>
                </div>
            </asp:Panel>
        </div>
        <!--Redirecciona entidad externa-->    
        <asp:Panel ID="Panel_redirecciona_entidad_externa" runat="server" Style="display: none; width: 70%; height: auto" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_redirecciona_entidad_externa" runat="server" TargetControlID="ButtonSalir_redirecciona_entidad_externa" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_redirecciona_entidad_externa" PopupControlID="Panel_redirecciona_entidad_externa">
            </asp:ModalPopupExtender>
            <div class="modal-content_">
                <div id="div_redireciona_externa" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title">Traslado de solicitud a entidad externa</h6>
                    <button type="button" value="Button_cerrar_redirecciona_entidad_externa" class="close da_event_captive">&times;</button>
                </div>
                <div id="contenido_procesa_redirecciona_entidad_externa" style="width: auto; height: auto; margin-right: 5px; border-top: none" class="modal_content_back modal-body">
                    <div style="margin-left: 15px; margin-right: 15px">
                        <div class="row w-100 pt-2">
                            <div class="col-6">
                                <h6>Nombre entidad *</h6>
                            </div>
                            <div class="col-6">
                                <asp:TextBox ID="TextBox_nombre_externo"    atrib_aleas_c="Nombre entidad" atrib_campo_n="Nombre_externo" atrib_campo_O="1" atrib_campo_v="1" atrib_campo_nl="0" atrib_campo_tip="na" CssClass="form-control form_control_traslado" MaxLength="100" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        
                        <div class="row w-100 pt-2">
                            <div class="col-6">
                                <h6>Nit identificación *</h6>
                            </div>
                            <div class="col-6">
                                <asp:TextBox ID="TextBox_identificacion" atrib_aleas_c="Nit identificación" atrib_campo_n="Identificacion" atrib_campo_O="1" atrib_campo_v="1" atrib_campo_nl="0" atrib_campo_tip="na" CssClass="form-control form_control_traslado" MaxLength="20" runat="server"></asp:TextBox>
                            </div>
                        </div>
                         <div class="row w-100 pt-2">
                            <div class="col-6">
                                <h6>Correo electrónico *</h6>
                            </div>
                            <div class="col-6">
                                <asp:TextBox ID="TextBox_correo_electronico" atrib_aleas_c="Correo electrónico" atrib_campo_n="correo_electronico" atrib_campo_o="1" atrib_campo_v="1" atrib_campo_nl="0" atrib_campo_tip="cor" CssClass="form-control form_control_traslado" MaxLength="200" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row w-100 pt-2">
                            <div class="col-6">
                                <h6>Dirección *</h6>
                            </div>
                            <div class="col-6">
                                <asp:TextBox ID="TextBox_direccion" atrib_aleas_c="Dirección" atrib_campo_n="Direccion" atrib_campo_o="1" atrib_campo_v="1" atrib_campo_nl="0" atrib_campo_tip="na" CssClass="form-control form_control_traslado" MaxLength="200" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row w-100 pt-2">
                            <div class="col-6">
                                <h6>Pais *</h6>
                            </div>
                            <div class="col-6">
                               <select id="pais" atrib_aleas_c="País" atrib_campo_n="Pais" atrib_campo_o="1" atrib_campo_v="0" atrib_campo_nl="0" atrib_campo_tip="na" class="form-control w-100 form_control_traslado"  onchange="event_add_departanmento('pais');"> </select>
                            </div>
                        </div>
                        <div class="row w-100 pt-2">
                            <div class="col-6">
                                <h6>Departamento *</h6>
                            </div>
                            <div class="col-6">
                               <select id="departamento" atrib_aleas_c="Departamento" atrib_campo_n="Departamento" atrib_campo_o="1" atrib_campo_v="0" atrib_campo_nl="0" atrib_campo_tip="na" class="form-control w-100 form_control_traslado" onchange="event_add_municipio('departamento');"> </select>
                            </div>
                        </div>
                         <div class="row w-100 pt-2">
                            <div class="col-6">
                                <h6>Municipio *</h6>
                            </div>
                            <div class="col-6">
                               <select id="municipio" atrib_aleas_c="Municipio" atrib_campo_n="Municipio" atrib_campo_o="1" atrib_campo_v="0" atrib_campo_nl="0" atrib_campo_tip="na" class="form-control w-100 form_control_traslado" > </select>
                            </div>
                        </div>
                         <div class="row w-100 pt-2">
                            <div class="col-6">
                                <h6>Nota traslado *</h6>
                            </div>
                            <div class="col-6">
                                <asp:TextBox ID="TextBox_nota_traslado" TextMode="MultiLine" atrib_aleas_c="Nota_traslado" atrib_campo_n="nota_traslado" atrib_campo_o="1" atrib_campo_v="1" atrib_campo_nl="0" atrib_campo_tip="na" CssClass="form-control form_control_traslado" MaxLength="200" runat="server"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
            <div class="modal-footer align-content-end">
                <button type="button" title="Confirmar y enviar el documento respuesta" onclick="inicializa_tipo_adjunto_documento(event,this, 'E-ENTIDAD-EXTERNA');" class="btn btn-success   mt-1">Aceptar</button>
                <button type="button" title="" value="Button_cerrar_redirecciona_entidad_externa" class="btn btn-light da_event_captive  mt-1">Cancelar </button>
                <input id="Hidden1" type="hidden" value="" runat="server" />

            </div>
            <div style="display: none; height: 1px">
                <asp:Button ID="Button_cerrar_redirecciona_entidad_externa" runat="Server" Text="X" CssClass="invisible" />
                <asp:Button ID="Button_redirecciona_entidad_externa" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                <asp:Button ID="ButtonSalir_redirecciona_entidad_externa" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
            </div>
        </asp:Panel>
        <!--Redirecciona gestion solicitud-->    
        <asp:Panel ID="Panel_gestion_respuesta_solicitud" runat="server" Style="display: none; width: 80%; height: auto" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_gestion_respuesta_solicitud" runat="server" TargetControlID="ButtonSalir_gestion_respuesta_solicitud" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_gestion_respuesta_solicitud" PopupControlID="Panel_gestion_respuesta_solicitud">
            </asp:ModalPopupExtender>
            <div class="modal-content_">
                <div id="div_gestion_respuesta_solicitud" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title">Registrar gestión a la solicitud</h6>
                    <button type="button" value="Button_cerrar_gestion_respuesta_solicitud" class="close da_event_captive">&times;</button>
                </div>
                <div id="contenido_procesa_gestion_respuesta_solicitud" style="width: auto; height: auto; border-top: none" class="modal_content_back modal-body">
                    <div>
                        
                         <div class="row w-100 pt-2">
                            <div class="col-3">
                                <h6>Descripción de la gestión *</h6>
                            </div>
                            <div class="col-9">
                                <asp:TextBox ID="TextBox_nota_gestion" TextMode="MultiLine" atrib_aleas_c="Descripción de la gestión" atrib_campo_n="CAMPOS" atrib_campo_o="1" atrib_campo_v="1" atrib_campo_nl="0" atrib_campo_tip="na" CssClass="form-control form_control_gestion" MaxLength="300" runat="server"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
            <div class="modal-footer align-content-end">
                <button type="button" title="Registra gestión solicitud" onclick="event_element_menu('R-GESTION-SOLICITUD','');" class="btn btn-success   mt-1">Aceptar</button>
                <button type="button" title="" value="Button_cerrar_gestion_respuesta_solicitud" class="btn btn-light da_event_captive  mt-1">Cancelar </button>
                
            </div>
            <div style="display: none; height: 1px">
                <asp:Button ID="Button_cerrar_gestion_respuesta_solicitud" runat="Server" Text="X" CssClass="invisible" />
                <asp:Button ID="Button_gestion_respuesta_solicitud" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                <asp:Button ID="ButtonSalir_gestion_respuesta_solicitud" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
            </div>
        </asp:Panel>
        <!--Edita gestion solicitud-->    
        <asp:Panel ID="Panel_editar_gestion_solicitud" runat="server" Style="display: none; width: 80%; height: auto" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_editar_gestion_solicitud" runat="server" TargetControlID="ButtonSalir_editar_gestion_solicitud" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_editar_gestion_solicitud" PopupControlID="Panel_editar_gestion_solicitud">
            </asp:ModalPopupExtender>
            <div class="modal-content_">
                <div id="div_editar_gestion_solicitud" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title">Editar gestión de la solicitud</h6>
                    <button type="button" value="Button_cerrar_editar_gestion_solicitud" class="close da_event_captive">&times;</button>
                </div>
                <div id="contenido_procesa_editar_gestion_solicitud" style="width: auto; height: auto; border-top: none" class="modal_content_back modal-body">
                    <div>        
                         <div class="row w-100 pt-2">
                            <div class="col-3">
                                <h6>Descripción de la gestión *</h6>
                            </div>
                            <div class="col-9">
                                <asp:TextBox ID="TextBox_nota_gestion_edita" TextMode="MultiLine" atrib_aleas_c="Descripción de la gestión" atrib_campo_n="CAMPOS" atrib_campo_o="1" atrib_campo_v="1" atrib_campo_nl="0" atrib_campo_tip="na" CssClass="form-control form_control_gestion_edition" MaxLength="300" runat="server"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
            <div class="modal-footer align-content-end">
                <button id="edit_gestion_soclicitud" type="button" title="Editar gestion respuesta"  class="btn btn-success   mt-1">Aceptar</button>
                <button type="button" title="" value="Button_cerrar_editar_gestion_solicitud" class="btn btn-light da_event_captive  mt-1">Cancelar </button>
                
            </div>
            <div style="display: none; height: 1px">
                <asp:Button ID="Button_cerrar_editar_gestion_solicitud" runat="Server" Text="X" CssClass="invisible" />
                <asp:Button ID="Button_editar_gestion_solicitud" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                <asp:Button ID="ButtonSalir_editar_gestion_solicitud" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
            </div>
        </asp:Panel>
      
        <asp:Panel ID="Panel_list_gestion_solicitud" runat="server" Style="display:block; width: 70%; height: 100%" CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_list_gestion_solicitud" runat="server"
                TargetControlID="ButtonSalir_list_gestion_solicitud" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_list_gestion_solicitud" PopupControlID="Panel_list_gestion_solicitud">
            </asp:ModalPopupExtender>
            <div id="modal_content_list_gestion_solicitud" class="modal-content">
                <div id="diver_cabcera_list_gestion_solicitud" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ">Lista de gestión</h6>
                    <button type="button" value="Button_cerrar_list_gestion_solicitud" class="close da_event_captive ">&times;</button>
                </div>
                <div id="contenido_procesa_list_gestion_solicitud" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                    <asp:UpdatePanel ID="Update_list_gestion_solicitud" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>      
                            <div id="contenido_titulo_list_gestion_solicitud" class="mb-2">  
                                <input id="Hidden_list_gestion_solicitud" type="hidden" value="-1" runat="server"/>          
                                <asp:Label ID="titulo_label_list_gestion_solicitud" runat="server" class="h6 font-weight-light">Resultados busqueda</asp:Label>
                            </div>
                            <div id="content_data_grid_list_gestion_solicitud" class="conten_gred_border_" style="overflow: auto; width: 100%">
                                <asp:GridView ID="GridView_list_gestion_solicitud" runat="server"  Style="position: inherit; width: 100%; font-size: 14px"
                                    AutoGenerateSelectButton="False" AllowSorting="true"  AllowPaging="true" PageSize  ="6" PagerSettings-Position="Top"  CssClass="table  font-weight-light" GridLines="None"
                                     EnableViewState="true">
                                    <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                    <HeaderStyle CssClass="GridviewScrollHeader_line_boot" />
                                    <PagerStyle CssClass="pagination-ys" />
                                    <Columns>
                                         <asp:BoundField HeaderText="OPCIONES"   />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </ContentTemplate>

                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>

                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_list_gestion_solicitud" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                        <asp:Button ID="ButtonSalir_list_gestion_solicitud" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="Button_cerrar_list_gestion_solicitud" runat="Server" Text="X" CssClass="invisible" />
                    </div>
                </div>
              
            </div>
        </asp:Panel>
        <div style="display: none">
            <asp:UpdatePanel ID="UpdatePanel_boton_gestion_solicitud" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="Button_tool_activa_lista_gestion_solicitud" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
         <div id="asigna_dest_externo">
            <asp:Panel ID="Panel_asigna_dest_externo" runat="server" Style="display:none; width:35%; height:auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_asigna_dest_externo" runat="server"  TargetControlID="ButtonSalir_asigna_dest_externo" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_asigna_dest_externo" PopupControlID="Panel_asigna_dest_externo" ></asp:ModalPopupExtender>
                <div id="div4" class="modal_title_superior_ modal-header"> 
                       <h6 class="modal-title">Reasigna peticionario con autorización</h6>
                       <button type="button" value="Button_cerrar_asigna_dest_externo" class="close da_event_captive">&times;</button>                 
                   
                </div>
                <div id="contenido_procesa_asigna_dest_externo" style="background-color: white; width:auto; height:auto; border-top:none" class="modal_content_back modal-body">  
                       <div style="margin:10px">
                           <asp:UpdatePanel ID="UpdatePanel_dest_externo" runat="server" UpdateMode="Conditional">
                               <ContentTemplate>
                                   <input id="Hidden_remitente_destinatario" type="hidden" value="-1" runat="server" checked="checked"/>
                                   <input id="Hidden_remitente_nombre" type="hidden" value="-1" runat="server" checked="checked"/>
                                   <asp:Button ID="Button_Asigana_datos_validacion_edicion" runat="server" Text="Button" Style="display: none" />
                                    <asp:Button ID="Button_examinar_dest_externo" runat="server" Text="" Style="display: none" CssClass="btn btn-success" OnClientClick="asigna_datos_heig_with();" />
                                   
                                       <div class="form-group-sm row mr-0 ml-0">
                                           <div class="col-10 ml-0 pl-0">
                                                <asp:TextBox ID="TextBox_dext_externo" CssClass="bg-transparent" runat="server" Style="background-color: InactiveBorder; width: 100%" Enabled="false"></asp:TextBox>     
                                           </div>
                                             <div class="col-2 pl-0">
                                                  <button type="button" title="Examinar usuario remitente o peticionario" onclick="activa_boton_client_server('Button_examinar_dest_externo');" value="Button_examinar_dest_externo" class="btn btn-success"><i  class="fal fa-search"></i></button>
                                             </div>                                           
                                       </div>
                                        
                                   <div class="form-group-sm mt-2 ml-1">
                                       <asp:Label ID="Label_usario_externo" runat="server" Text="Usuario autorizado*" CssClass="h6"></asp:Label>
                                       <asp:TextBox ID="TextBox_login_usuario_val_externo" runat="server" Style="" CssClass="form-control"></asp:TextBox>
                                   </div>
                                   <div class="form-group-sm mt-2 ml-1">
                                       <asp:Label ID="Label_destinatario_externo" runat="server" Text="Contraseña usuario*" CssClass="h6"></asp:Label>
                                       <asp:TextBox ID="TextBox_pasw_usuario_val_externo" runat="server" CssClass="form-control" Style="" Type="password" TextMode="Password"></asp:TextBox>
                                      
                                   </div>


                               </ContentTemplate>
                           </asp:UpdatePanel>
                       </div>               
                            
                          <div style="display:none; height:1px">
                              <asp:Button ID="Button_cerrar_asigna_dest_externo" runat="Server" Text="X" CssClass="invisible" />
                              <asp:Button ID="Button_asigna_dest_externo" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                              <asp:Button ID="ButtonSalir_asigna_dest_externo" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                         </div>
                    <div class="modal-footer">
                        <asp:UpdatePanel ID="UpdatePanel_dest_externo_boton" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                 <asp:Button ID="Button_actualizar_peticionario" runat="server" Text="Aceptar" Style="float: right; margin-top: 10px; margin-bottom: 5px" CssClass="btn  btn-success" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </asp:Panel>
        </div>
        <div id="validacion_plantilla">
            <asp:Panel ID="Panel_valiacion_plantilla" runat="server" Style="display:none; color: White; width:100%; height:auto; margin-top:1px" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_valiacion_plantilla" Y="1" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_valiacion_plantilla"
                    PopupControlID="Panel_valiacion_plantilla" CancelControlID="Button_cerrar_validacion_plantilla">
                </asp:ModalPopupExtender>
                <div id="divcabecer2_validacion_plantilla" class="modal_title_superior"> 
                     <h6 class="modal-title d-inline ml-2">Gestion externos</h6>
                     <button type="button" value="Button_cerrar_validacion_plantilla" class="close da_event_captive mr-2">&times;</button>                                            
                </div>
                <asp:UpdatePanel ID="UpdatePanel_validacion_plantilla" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="Contenido_validacion_plantilla" style="height: 100%; width: 100%" class="modal_content_back">
                            <iframe width="100%" height="100%" frameborder="0" id="Iframe_validacion_plantilla_" runat="server"></iframe>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div style="display:none; height:1px">
                    <asp:Button ID="ButtonSalir_valiacion_plantilla" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                    <asp:Button ID="Button_cerrar_validacion_plantilla" runat="Server" Text="X" class="invisible"/>
                </div>
                    
            </asp:Panel>
        </div>
        <div id="reasigna_tramite_usuario">
            <asp:Panel ID="Panel_reasigna_tramite_usuario" runat="server" Style="display:none;  width:50%; height:auto" CssClass="modal_content_general" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_reasigna_tramite_usuario" runat="server"  
                    TargetControlID="ButtonSalir_reasigna_tramite_usuario" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_reasigna_tramite_usuario" PopupControlID="Panel_reasigna_tramite_usuario" ></asp:ModalPopupExtender>
                <div class="modal-content">
                    <div id="divcabecer2_radica_documento" class="modal_title_superior_ modal-header" >
                        <h6 class="modal-title">Reasignar</h6>
                        <button type="button" value="Button_cerrar_reasigna_tramite_usuario" class="close da_event_captive mr-2">&times;</button>
                    </div>
                    <div id="contenido_procesa_reasigna_tramite_usuario" style="background-color: white; width: auto; height: auto; border-top:none" class="modal_content_back modal-body">
                        <div style=" margin-top: 15px">
                            <select class="tokenize-callable-demo_respuesta form-control " multiple>
                            </select>                 
                        </div>          
                        <div style="display: none; height: 1px">
                            <input id="Hidden_token_tokenize2" type="hidden" value="-1" runat="server"/>
                            <asp:TextBox ID="TextBox_user_seleccionado" runat="server" Style="width: 1%;  display: none" ></asp:TextBox>
                            <asp:Button ID="Button_reasigna_tramite_usuario" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                            <asp:Button ID="ButtonSalir_reasigna_tramite_usuario" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                            <asp:Button ID="Button_cerrar_reasigna_tramite_usuario" runat="Server" Text="X"  />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" title="Reasigna tramite" onclick="inicializa_tipo_adjunto_documento(event,this, 'R-ASIG-LISTA');" class="btn btn-success"> Aceptar</button>
                        <button type="button" title="Cancelar" value="Button_cerrar_reasigna_tramite_usuario" class="btn  btn-light da_event_captive"> Cancelar</button>
                    </div>
                </div>
            </asp:Panel>
           
        </div>
         <!--autoriza reasignacion-->
          <div id="autoriza_reasignacion_tarea">
            <asp:Panel ID="Panel_autoriza_reasignacion_tarea" runat="server" Style="display:none; color: White; width: 30%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_autoriza_reasignacion_tarea" runat="server"  TargetControlID="ButtonSalir_autoriza_reasignacion_tarea" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_autoriza_reasignacion_tarea" PopupControlID="Panel_autoriza_reasignacion_tarea" ></asp:ModalPopupExtender>
                <div id="divcabecer2_autoriza_reasignacion_tarea" class="modal_title_superior">
                  
                    <asp:Label ID="Label_autoriza_reasignacion_tarea" runat="server" Text="Autoriza reasignación"  Style="float: left; margin-left:5px">
                    </asp:Label>
                    <div id="Divcerrarbuton2_autoriza_reasignacion_tarea" style="float: right">
                        <asp:Button ID="Button_cerrar_autoriza_reasignacion_tarea" runat="Server" Text="X" CssClass="modal_boton_hiden"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_autoriza_reasignacion_tarea" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF;" class="modal_content_back">   
                       <div style="margin-left:15px; margin-right:15px">
                              <asp:UpdatePanel ID="UpdatePanel_autoriza_reasignacion_tarea" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                               <br />
                                <div class="form-group">
                                     <asp:Label ID="Label_user_autoriza_reasignacion_tarea" runat="server" Text="Usuario autorizado*" Style=" font-family: Arial; font-size: 14px"></asp:Label>
                                     <asp:TextBox ID="TextBox_login_autoriza_reasignacion_tarea" runat="server" class="form-control"></asp:TextBox>
                                     <asp:Label ID="Label_dest_autoriza_reasignacion_tarea" runat="server" Text="Contraseña usuario*" Style="font-family: Arial; font-size: 14px"></asp:Label>
                                     <asp:TextBox ID="TextBox_pasw_autoriza_reasignacion_tarea" runat="server"  class="form-control" TextMode="Password"></asp:TextBox> 
                                     <input id="Hidden_resp_envio" type="hidden" value="" runat="server"/>    
                                </div>
                                 <asp:Button ID="Button_autoriza_reasignacion" runat="server" Text="Aceptar" Style="margin-top:10px; margin-bottom:10px; float:right"  CssClass="btn  btn-primary" /> 
                                     <asp:Button ID="Button_cancela_autoriza_reasignacion" runat="server" Text="Cancelar" Style="margin-top:10px; margin-bottom:10px; margin-right:10px; float:right"  CssClass="btn  btn-light" />                      
                            </ContentTemplate>
                        </asp:UpdatePanel>
                       </div>  
                        
                           <asp:Button ID="Button_autoriza_reasignacion_tarea" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                           <asp:Button ID="ButtonSalir_autoriza_reasignacion_tarea" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                </div>
            </asp:Panel>
        </div>
         <!--Confirma reasigna respuesta-->
        <div id="confirma_reasigna_responsable_tramite">
            <asp:Panel ID="Panel_confirma_reasigna_responsable_tramite" runat="server" Style="display:none; color: White; width: 400px; height: 130px" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_confirma_reasigna_responsable_tramite" runat="server" 
                     TargetControlID="ButtonSalir_confirma_reasigna_responsable_tramite" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_confirma_reasigna_responsable_tramite" PopupControlID="Panel_confirma_reasigna_responsable_tramite" ></asp:ModalPopupExtender>
                <div id="div8" class="modal_title_superior">                 
                    <asp:Label ID="Label_confirma_reasigna_responsable_tramite" runat="server" Text="Reasigna respuesta tramite" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_confirma_reasigna_responsable_tramite" style="float: right">
                        <asp:Button ID="Button_cerrar_confirma_reasigna_responsable_tramite" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_confirma_reasigna_responsable_tramite" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF;" class="modal_content_back">        
                        <asp:UpdatePanel ID="UpdatePanel_contenido_confirma_reasigna_responsable_tramite" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                               <br />
                                <table style="width: 100%;">                    
                                    <tr >
                                        <td style=" width:300px; padding-left:30px; padding-bottom:20px">
                                            <asp:Label ID="Label_destinatario_confirma_reasgina" runat="server" Text="Desea reasignarse la responsabilidad de responder esta solicitud?" Style="text-align: center; font-family: Arial; font-size: 13px; font-weight:600"></asp:Label>

                                        </td>
                                                                  
                                    </tr>
                                    <tr>
                                        <td> </td>
                                    </tr>
                                    
                                    <tr>
                                        
                                        <td style=" text-align:right">
                                            <asp:Button ID="Button_autoriza_confirma_reasigna" runat="server" Text="Aceptar" Style="width: 100px" CssClass="boton_azul" /> 
                                             <asp:Button ID="Button_cancela_confirma_reasigna" runat="server" Text="Cancelar" Style="width: 100px" CssClass="boton_blanco" /> 
                                                         
                                        </td>
                                    </tr>
                                    
                                    
                                </table>
                                                         
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
                <asp:Button ID="Button_confirma_reasigna_responsable_tramite" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_confirma_reasigna_responsable_tramite" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
           
        </div>
         <!--Modal reasigna respuesta-->
            <div id="reasigna_responsable_tramite">
                <asp:Panel ID="Panel_reasigna_responsable_tramite" runat="server" Style="display:none;  width: auto; height: auto" CssClass="modal_content_general_ ">
                    <asp:ModalPopupExtender ID="ModalPopupExtender_edition_reasigna_responsable_tramite" runat="server"  TargetControlID="ButtonSalir_reasigna_responsable_tramite" BackgroundCssClass="FondoAplicacion"
                        CancelControlID="Button_cerrar_reasigna_responsable_tramite" PopupControlID="Panel_reasigna_responsable_tramite" ></asp:ModalPopupExtender>
                    <div class="modal-content">
                        <div id="div10" class="modal_title_superior_ modal-header">
                             <h6 class="modal-title">Reasigna respuesta tramite</h6>
                            <button type="button" onclick="activa_boton_client_server('Button_cerrar_reasigna_responsable_tramite');" class="close">&times;</button>
                        </div>
                        <div id="contenido_procesa_reasigna_responsable_tramite" style="background-color: white; width: auto; height: auto; color: black; background-color: #FFFFFF;" class="modal_content_back modal-body">
                            <div  class=" col-12">
                                <span>
                                    Usuario autorizado*
                                </span>       
                            </div>
                             <div  class=" col-6">
                                  <asp:TextBox ID="TextBox_login_autoriza_reasigna" runat="server" Style="width: 300px" CssClass="form-control"></asp:TextBox>
                            </div>
                            <br />
                            <div  class=" col-12">
                                  
                                <span>
                                    Contraseña usuario*
                                </span>
                            </div>
                             <div  class=" col-6">
                                 <asp:TextBox ID="TextBox_pasw_autoriza_reasigna" runat="server" Style="width: 300px" TextMode="Password"></asp:TextBox>
                            </div>
                            
                        </div>
                         <div class="modal-footer">
                             <asp:UpdatePanel ID="UpdatePanel_contenido_reasigna_responsable_tramite" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                     <asp:Button ID="Button_autoriza_reasigna" runat="server" Text="Aceptar" Style="float:right" CssClass="btn btn-success" />
                                    <asp:Button ID="Button_cancela_autoriza_reasigna" runat="server" Text="Cancelar" Style="float:right; margin-right:10px" CssClass="btn  btn-light" />
                                     </ContentTemplate>
                            </asp:UpdatePanel>
                             </div>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button_reasigna_responsable_tramite" CssClass="invisible bg-transparent" runat="server" Text="" Height="1px" Width="1px" />
                            <asp:Button ID="ButtonSalir_reasigna_responsable_tramite" CssClass="invisible bg-transparent" runat="server" Text="" Height="1px" Width="1px" />
                            <asp:Button ID="Button_cerrar_reasigna_responsable_tramite" runat="Server" Text="" />
                        </div>         
                    </div>
                </asp:Panel>
               
        </div>
        <!--detalle respuesta-->
           <asp:Panel ID="Panel_detalle_respuesta" runat="server" Style="display:none; overflow:hidden"  Width="95%" Height="100% " CssClass="modal_content_general">
                  <asp:ModalPopupExtender ID="ModalPopupExtender_detalle_respuesta" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_detalle_respuesta"
                      PopupControlID="Panel_detalle_respuesta"  CancelControlID="ButtonSalir_detalle_respuesta">
                  </asp:ModalPopupExtender>
                  <div id="Cabecerapendiente_detalle_respuesta" class="modal_title_superior">    
                       <button type="button" style="margin-right:10px" onclick="activa_boton_client_server('ButtonSalir_detalle_respuesta');" class="close">&times;</button>           
                  </div>
                  <div id="Cotenedorpendiente_detalle_respuesta" style="color: Black; background-color: #FFFFFF; height: 90%; width: 100%; overflow:hidden" class="modal_content_back">       
                      <asp:UpdatePanel ID="UpdatePanel_detalle_respuesta" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_visor_externo__" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>

                      </asp:UpdatePanel>
                           
                  </div>
               <div style="display: none; height: 1px">
                   <asp:Button ID="Button_detalle_respuesta" CssClass="invisible bg-transparent" runat="server" Text="Button" Height="1px" Width="1px" />
                   <asp:Button ID="ButtonSalir_detalle_respuesta" runat="Server" Text="" CssClass="invisible bg-transparent" />
               </div>
                  
                    
              </asp:Panel>
         <!--detalle transacciones-->
	               <asp:Panel ID="Panel_transacciones" runat="server" Style="display:none; overflow:hidden; width:95%; height:100%" CssClass="modal_content_general" >
	                      <asp:ModalPopupExtender ID="ModalPopupExtender_transacciones" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_transacciones_dos"
	                          PopupControlID="Panel_transacciones"  CancelControlID="ButtonSalir_transacciones">
	                      </asp:ModalPopupExtender>
	                      <div id="Cabecerapendiente_transacciones" class="modal_title_superior" >                     
	                         <button style="margin-right:10px" type="button" onclick="activa_boton_client_server('ButtonSalir_transacciones');" class="close">&times;</button>          
	                      </div>
	                      <div id="Cotenedorpendiente_transacciones" style="height: 90%; width: 100%; overflow:hidden" class="modal_content_back">                     
	                          <asp:UpdatePanel ID="UpdatePanel_transacciones" runat="server" UpdateMode="Conditional">
	                              <ContentTemplate>
	                                  <iframe id="Iframe_transacciones_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
	                              </ContentTemplate>
	    
	                          </asp:UpdatePanel>
	                               
	                      </div>
                       <div style="display: none; height: 1px">
                           <asp:Button ID="ButtonSalir_transacciones" runat="Server" Text=""  CssClass="invisible bg-transparent" />
	                       <asp:Button ID="Button_transacciones_dos" CssClass="invisible bg-transparent" runat="server" Text="" Height="1px" Width="1px" />
                       </div>  
	                       
              </asp:Panel>
         <!--detalle trazabilidad-->
	               <asp:Panel ID="Panel_trazabilidad" runat="server" Style="display:none; overflow:hidden; width:70%; height:100%"  CssClass="modal_content_general" >
	                      <asp:ModalPopupExtender ID="ModalPopupExtender_trazabilidad" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_trazabilidad_dos"
	                          PopupControlID="Panel_trazabilidad"  CancelControlID="ButtonSalir_trazabilidad">
	                      </asp:ModalPopupExtender>
	                      <div id="Cabecerapendiente_trazabilidad" class="modal_title_superior"> 
                                                 
	                          <button type="button" style="margin-right:10px" onclick="activa_boton_client_server('ButtonSalir_trazabilidad');" class="close">&times;</button>
	                      </div>
	                      <div id="Cotenedorpendiente_trazabilidad" style="height: 90%; width: 100%" class="modal_content_back">          
	                          <asp:UpdatePanel ID="UpdatePanel_trazabilidad" runat="server" UpdateMode="Conditional">
	                              <ContentTemplate>
	                                  <iframe id="Iframe_trazabilidad_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
	                              </ContentTemplate>
	    
	                          </asp:UpdatePanel>
	                               
	                      </div>
                          <div style="display: none; height: 1px">
                           <asp:Button ID="Button_trazabilidad_dos" CssClass="invisible bg-transparent" runat="server" Text="" Height="1px" Width="1px" />
                           <asp:Button ID="ButtonSalir_trazabilidad" runat="Server" Text="" CssClass="invisible bg-transparent" />
                         </div>
	                       
	    
	                        
              </asp:Panel>
         
         <!--Reversa respuesta-->
        <div id="reversa_respuesta">
            <asp:Panel ID="Panel_reversa_respuesta" runat="server" Style="display:none;  width:auto; height:auto" CssClass="modal_content_general" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_reversa_respuesta" runat="server"  TargetControlID="ButtonSalir_reversa_respuesta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_reversa_respuesta" PopupControlID="Panel_reversa_respuesta" ></asp:ModalPopupExtender>
                <div class="modal-content">
                    <div id="div1" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title">Autorización</h6>
                        <button type="button" value="Button_cerrar_reversa_respuesta" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="contenido_procesa_reversa_respuesta" style="background-color: white; width: 100%; height: 99%" class="modal_content_back_ modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_contenido_radica_documento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class=" col-12">
                                    <span>Usuario autorizado*
                                    </span>
                                </div>
                                <div class=" col-6">
                                    <asp:TextBox ID="TextBox_login_usuario_val" runat="server" Style="width: 300px" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class=" col-12">
                                    <span>Contraseña usuario*
                                    </span>
                                </div>
                                <div class=" col-6">
                                    <asp:TextBox ID="TextBox_pasw_usuario_val" runat="server" Style="width: 300px" TextMode="Password"></asp:TextBox>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button_cerrar_reversa_respuesta" runat="Server" Text="" CssClass="invisible" />
                            <asp:Button ID="Button_reversa_respuesta" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                            <asp:Button ID="ButtonSalir_reversa_respuesta" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                        </div>

                    </div>
                     <div class="modal-footer">
                         <button type="button" title="Reversa gestión respuesta"  onclick="inicializa_tipo_adjunto_documento(event,this, 'E-R-R-A')" class="btn btn-success   mt-1"> Aceptar</button>
                         <button type="button" title=""  value="Button_cerrar_reversa_respuesta" class="btn btn-light da_event_captive  mt-1"> Cancelar </button>
                         <input id="Hidden_user_rever" type="hidden" value="" runat="server"/>
                          
                    </div>
                </div>
            </asp:Panel>
           
        </div>
        <!--Confirma Reversa respuesta-->
        <div id="confirma_reversa_respuesta">
            <asp:Panel ID="Panel_confirma_reversa_respuesta" runat="server" Style="display:none;  width:auto; height:auto" CssClass="modal_content_general_" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_confirma_reversa_respuesta" runat="server"  TargetControlID="ButtonSalir_confirma_reversa_respuesta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_confirma_reversa_respuesta" PopupControlID="Panel_confirma_reversa_respuesta" ></asp:ModalPopupExtender>
                 <div class="modal-content">  
                     <div id="div2" class="modal_title_superior_ modal-header">    
                         <button type="button" value="Button_cerrar_confirma_reversa_respuesta"  class="close da_event_captive">&times;</button>
                     </div>
                     <div id="contenido_procesa_confirma_reversa_respuesta" style="background-color: white; width: auto; height: auto; border-top:none" class="modal_content_back modal-body">
                         <div class="container">
                            
                              <p class="p-3  " style="font-family:'Segoe UI'">  Desea reversar la gestion del tramite <i class="fad fa-question"></i></p>
                         </div>
                        
                         <div style="display: none; height: 1px">
                             <asp:Button ID="Button_confirma_reversa_respuesta" CssClass="invisible bg-transparent" runat="server" Text="" Height="1px" Width="1px" />
                             <asp:Button ID="ButtonSalir_confirma_reversa_respuesta" CssClass="invisible bg-transparent" runat="server" Text="" Height="20px" Width="1px" />
                             <asp:Button ID="Button_cerrar_confirma_reversa_respuesta" CssClass="invisible bg-transparent" runat="Server" Text="X" />
                         </div>

                     </div>
                     <div class="modal-footer">
                         <button type="button" title="Reversa gestión respuesta"  onclick="inicializa_tipo_adjunto_documento(event,this, 'E-R-R-C')" class="btn btn-success   mt-1"> Aceptar</button>
                         <button type="button" title=""  value="Button_cerrar_confirma_reversa_respuesta" class="btn btn-light da_event_captive  mt-1"> Cancelar </button>
                          <input id="Hidden_con_ref" type="hidden" value="" runat="server"/>    
                     </div>
                 </div>
              
            </asp:Panel>
           
        </div>
         <div id="ventanaimpreion">     
            <asp:Panel ID="Panelimpresion" runat="server"  Style="display:none; width: auto; height: auto" CssClass="modal_content_general">
                 <asp:DragPanelExtender ID="DragPanelExtenderimpre" runat="server" TargetControlID="Panelimpresion" />
                 <asp:ModalPopupExtender ID="ModalPopupExtenderimpre" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir"
                     PopupControlID="Panelimpresion" CancelControlID="Buttoncerrarimpre">
                 </asp:ModalPopupExtender>
                <div class="modal-content">        
                    <div id="divcabecer2" class="modal_title_superior_ modal-header">
                        <button type="button" value="Buttoncerrarimpre" class="close da_event_captive">&times;</button>
                    </div>
               
                <asp:UpdatePanel ID="UpdatePaneliframe" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div id="ContenidoImpresion" style="color: black; background-color: #FFFFFF; height:auto; width:auto" class="modal_content_back">
                            <iframe width="100%" height="100%" id="Iframe1" frameborder="0" runat="server" src="../Gestion/WebFormimpresionfile.aspx" ></iframe>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                      <div style="display:none; height:1px">
                             <asp:Button ID="ButtonSalir" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                             <asp:Button ID="Buttoncerrarimpre" runat="Server" Text="X" CssClass="invisible"
                               />  
                      </div>
                   
                </div>
               </asp:Panel>
            
        </div>
         <!--opcion_descarga_respuesta!-->
        <div id="opcion_descarga_respuesta">
            <asp:Panel ID="Panel_opcion_descarga_respuesta" runat="server" Style="display: none; height: auto; width: 40%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_opcion_descarga_respuesta" runat="Server" BackgroundCssClass="FondoAplicacion"
                    TargetControlID="ButtonSalir_opcion_descarga_respuesta"
                    PopupControlID="Panel_opcion_descarga_respuesta" CancelControlID="Buttoncerrarimpre_opcion_descarga_respuesta">
                </asp:ModalPopupExtender>
                <div id="Divcerrarbuton2_opcion_descarga_respuesta" class="modal_title_superior_ modal-header">
                    <button type="button" value="Buttoncerrarimpre_opcion_descarga_respuesta" class="close da_event_captive">&times;</button>
                </div>
                <div id="Contenido_opcion_descarga_respuesta" style="height: auto; width: 100%; color: black; border-top: none" class="modal_content_back modal-body">
                    <div id="div_title_opcion_descarga_respuesta" class="mt-1 ">
                        <h5>Opciones para descargar el documento</h5>
                    </div>
                    <div id="div_opcion_descarga_respuesta" class="border mt-3 mb-3">
                        <asp:CheckBox ID="CheckBox_opcion_descarga_respuesta_con_firma" runat="server" Text="Guardar documento con firma " Checked="true" ForeColor="Black" Font-Size="10" Font-Names="Arial" Style="margin-left: 5px" Enabled="true" />
                        <asp:CheckBox ID="Check_opcion_descarga_respuesta_sin_firma" runat="server" Text="Guardar documento sin firma" Checked="false" ForeColor="Black" Font-Size="10" Font-Names="Arial" Style="margin-left: 5px; display: block" Enabled="true" />
                    </div>
                    <div class="mr-3">
                        <span>Formato de descarga</span>
                        <asp:DropDownList ID="DropDownList_tipo_archivo" runat="server" Style="width: auto" CssClass="form-control">
                            <asp:ListItem Selected="True">PDF</asp:ListItem>
                            <asp:ListItem>DOCX</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" title="Descargar respuesta" onclick="inicializa_tipo_adjunto_documento(event,this, 'D-D-R-S');" class="btn btn-success">Descargar</button>
                    <button type="button" title="Cancelar" value="Buttoncerrarimpre_opcion_descarga_respuesta" class="btn btn-default da_event_captive">Cancelar</button>
                    <asp:HiddenField ID="HiddenField_descarga_docmento_respuesta" runat="server" Value="" />

                </div>

                <div style="display: none; height: 0px">
                    <asp:Button ID="ButtonSalir_opcion_descarga_respuesta" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    <asp:Button ID="Buttoncerrarimpre_opcion_descarga_respuesta" runat="Server" Text=""
                        CssClass="invisible" />
                    <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender3" runat="server" TargetControlID="Check_opcion_descarga_respuesta_sin_firma"
                        Key="radicado"></asp:MutuallyExclusiveCheckBoxExtender>
                    <asp:MutuallyExclusiveCheckBoxExtender ID="Mutuallyexclusivecheckboxextender4" runat="server" TargetControlID="CheckBox_opcion_descarga_respuesta_con_firma"
                        Key="radicado"></asp:MutuallyExclusiveCheckBoxExtender>
                </div>

            </asp:Panel>
        </div>
        <!--modal solicitud aprobación documentos-->
        <div id="solicitud_aprobacion">
            <asp:Panel ID="Panel_solicitud_aprobacion" runat="server" Style="display:none;  width: 90%; height: 90%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_solicitud_aprobacion" runat="server" Y="1"  TargetControlID="ButtonSalir_solicitud_aprobacion" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_solicitud_aprobacion" PopupControlID="Panel_solicitud_aprobacion"  >
                </asp:ModalPopupExtender>
                <div id="divcabecer2_solicitud_aprobacion" class="modal_title_superior">                   
                        <button type="button" style="margin-right:10px" onclick="inicializa_tipo_adjunto_documento(event,this,'A-E-S-A');" class="close">&times;</button>           
                </div>
                <div id="contenido_procesa_solicitud_aprobacion" style="background-color:white; width:100%; height: 100%" class="modal_content_back" >
                    <asp:UpdatePanel ID="UpdatePanel_solicitud_aprobacion" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>

                            <iframe style="color: White; width: 100%;  height: 100%; overflow:hidden" frameborder="0" id="Iframe_solicitud_aprobacion" runat="server"  ></iframe>
                             
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div style="display: none; height: 0px">
                        <asp:Button ID="Button_solicitud_aprobacion" CssClass="invisible bg-transparent" runat="server" Text="" Height="20px" Width="1px" />
                        <asp:Button ID="ButtonSalir_solicitud_aprobacion" CssClass="invisible bg-transparent" runat="server" Text="" Height="20px" Width="1px" />
                        <asp:Button ID="Button_cerrar_solicitud_aprobacion" runat="Server" Text="" CssClass=" bg-transparent"
                             />
                    </div>
                  
                </div>
            </asp:Panel>
        </div>
        
              <!--Popup historico tramite-->            
                   <asp:Panel ID="Panel_historico_tramite" runat="server" Style="display:none; overflow:hidden; width:100%; height:100%; border-radius:inherit"   CssClass="modal_content_general">
                      <asp:ModalPopupExtender ID="ModalPopupExtender_historico_tramite" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_historico_tramite"
                          PopupControlID="Panel_historico_tramite"  CancelControlID="ButtonSalir_historico_tramite">
                      </asp:ModalPopupExtender>
                      <div id="Cabecerapendiente_historico_tramite" class="modal_title_superior" style="border-bottom:none; padding-top:initial; padding-bottom:initial; background-color:#f8f9fa">  
                                   
                          <button type="button" style="margin-right:10px" onclick="activa_boton_client_server('ButtonSalir_historico_tramite');" class="close">&times;</button>
                      </div>
                      <div id="Cotenedorpendiente_historico_tramite" style="color: Black; background-color: #FFFFFF; height:auto; width: auto; overflow:auto; border-top:none" class="modal_content_back">        
                          <asp:UpdatePanel ID="UpdatePanel_historico_tramite" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <iframe id="Iframe_historico_tramite_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:auto"></iframe>
                              </ContentTemplate>
                          </asp:UpdatePanel>                          
                      </div>
                           <div style="display:none; height:1px">
                                 <asp:Button ID="ButtonSalir_historico_tramite" runat="Server" Text="X" CssClass="invisible bg-light"
                                  Height="1px" />
                                  <asp:Button ID="Button_historico_tramite" CssClass="invisible bg-light" runat="server" Text="" Height="1px" Width="1px" style="display:none" />
                           </div>
                              
              </asp:Panel>
              <input id="Hidden_name_event" type="hidden" value="" runat="server"/>     
        <input id="Hidden_colum_header" type="hidden" value="" runat="server"/>        
        <div id="inferior_bajo_boton" style="width: 0%; height: 0%; background-color: #E7EDF5; display: none">
             
            <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
                <ContentTemplate>   
                     <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                        frameborder="0" />
                   <asp:Button ID="Button_export_lista_event" runat="server" Text="Exportar" style="margin-top:5px; display:none" CssClass="boton_azul"   /> 
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>   
        <!--Confirma resuesta-->
          <div id="confirma_respuesta">
            <asp:Panel ID="Panel_confirma_respuesta" runat="server" Style="display:none;  width:50%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_confirma_respuesta"  runat="server" BackgroundCssClass="FondoAplicacion"  TargetControlID="ButtonSalir_confirma_respuesta" 
                    CancelControlID="Button_cerrar_confirma_respuesta" PopupControlID="Panel_confirma_respuesta" ></asp:ModalPopupExtender>
                <div class="modal-content">
                    <div id="divcabecer2_confirma_respuesta_" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title">Archivar</h6>
                        <button type="button" value="Button_cerrar_confirma_respuesta" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="contenido_procesa_confirma_respuesta" style="background-color: white; width: auto; height: auto; color: black; background-color: #FFFFFF; border-top:none" class="modal_content_back modal-body"> 
                            <div class="row">
                                <div class="pl-3">
                                    <asp:CheckBox ID="CheckBox_envio_correo_solo_confirmar" CssClass="p2" runat="server" Checked="true"  Text="" />
                                </div>
                                <div class="col-11">
                                    <p> Enviar al correo electrónico la confirmación del archivado</p>
                                </div>
                            </div>  
                            <div style="margin-top: 10px">
                                <select class="tokenize-callable-demo_respuesta_k form-control " style="max-width: 300px" multiple>
                                </select>
                            </div>
                            <input id="Hidden_token_tokenize_k" type="hidden" value="" runat="server"/>      
                           
                    </div>
                    <div class="modal-footer">
                        <button type="button" title="Archivar tramite"  onclick="inicializa_tipo_adjunto_documento(event,this, 'A-AH-GES');" class="btn btn-success"> Aceptar</button>
                        <button type="button" title="Cancelar" value="Button_cerrar_confirma_respuesta" class="btn  btn-light da_event_captive"> Cancelar</button>

                    </div>
                     <div style="display:none; height:1px">
                            <asp:Button ID="Button_cerrar_confirma_respuesta" runat="Server" Text="X" 
                            ToolTip="Cerrar ventana" />
                            <asp:Button ID="Button_confirma_respuesta"  runat="server" Text="Button" Height="1px" Width="1px" />
                            <asp:Button ID="ButtonSalir_confirma_respuesta" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                        </div> 
                </div>
            </asp:Panel>
        </div>
        <!--filtro-->
          <div id="filtro_historico">
            <asp:Panel ID="Panel_filtro_historico" runat="server" Style="display:none; color: White; width: 510px; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_filtro_historico" Y="36" runat="server" BackgroundCssClass="FondoAplicacion"  TargetControlID="ButtonSalir_filtro_historico" 
                    CancelControlID="Button_cerrar_filtro_historico" PopupControlID="Panel_filtro_historico" ></asp:ModalPopupExtender>
                <div id="divcabecer2_filtro_historico" class="modal_title_superior">
                  
                    <asp:Label ID="Label_filtro_historico" runat="server" Text="" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_filtro_historico" style="float: right">
                        <asp:Button ID="Button_cerrar_filtro_historico" runat="Server" Text="X" CssClass="modal_boton_hiden"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana"  />
                    </div>
                </div>
                <div id="contenido_procesa_filtro_historico" style="background-color: white; width: 100%; height: 99%; color: black; background-color: #FFFFFF;" class="modal_content_back">              
                        <asp:UpdatePanel ID="UpdatePanel_filtro_historico" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                               <br />
                                <table style="width: 100%;">
                                   
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_fecha_asig_filtro_historico" runat="server" Text="Fecha asignación tramite" Style="text-align: center; font-family: Arial; font-size: 14px; font-weight:700"></asp:Label>
                                        </td>
                                        <td>                  
                                            <asp:TextBox ID="TextBox_fecha_ini_asigna" runat="server" Style="width:100px; border:none; border-bottom:solid; border-bottom-width:1px; border-bottom-right-radius:0px; border-bottom-left-radius:0px" placeholder="yyyy-mm-dd"></asp:TextBox>
                                             <asp:ImageButton ID="ImageButton_fecha_ini_asigna" runat="server"  ImageUrl="../imagera/Calendar.png"/> &nbsp
                                             <asp:CalendarExtender ID="fecha_ini_asigna_CalendarExtender" runat="server" TargetControlID="TextBox_fecha_ini_asigna"  PopupButtonID="ImageButton_fecha_ini_asigna" Format = "yyyy-MM-dd"/>
                                            <asp:Label ID="Label_fecha_ini_hasta" runat="server" Text="Hasta" Style="text-align: center; font-family: Arial; font-size: 14px; font-weight:700"></asp:Label>
                                            <asp:TextBox ID="TextBox_fecha_fin_asigna" runat="server" Style="width:100px; margin-left:10px; border:none; border-bottom:solid; border-bottom-width:1px; border-bottom-right-radius:0px; border-bottom-left-radius:0px" placeholder="yyyy-mm-dd"></asp:TextBox>
                                            <asp:ImageButton ID="ImageButton_fecha_fin_asigna" runat="server"  ImageUrl="../imagera/Calendar.png"/>
                                            <asp:CalendarExtender ID="fecha_fin_asigna_CalendarExtender" runat="server" TargetControlID="TextBox_fecha_fin_asigna"  PopupButtonID="ImageButton_fecha_fin_asigna" Format = "yyyy-MM-dd"/>
                                        </td>
                                       
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label_dest_filtro_historico" runat="server" Text="Fecha finalización tramite" Style="text-align: center; font-family: Arial; font-size: 14px; font-weight:700"></asp:Label>

                                        </td>
                                        <td>
                                            <asp:TextBox ID="TextBox_fecha_ini_final_tramite" runat="server" Style="width:100px; border:none; border-bottom:solid; border-bottom-width:1px; border-bottom-right-radius:0px; border-bottom-left-radius:0px" placeholder="yyyy-mm-dd" ></asp:TextBox> 
                                            <asp:ImageButton ID="ImageButton_fecha_ini_final_tramite" runat="server"  ImageUrl="../imagera/Calendar.png"/> &nbsp
                                             <asp:CalendarExtender ID="CalendarExtender_fecha_ini_final_tramite" runat="server" TargetControlID="TextBox_fecha_ini_final_tramite"  PopupButtonID="ImageButton_fecha_ini_final_tramite" Format = "yyyy-MM-dd"/>
                                            <asp:Label ID="Label_fecha_ini_final_tramite" runat="server" Text="Hasta" Style="text-align: center; font-family: Arial; font-size: 14px; font-weight:700"></asp:Label> 
                                            <asp:TextBox ID="TextBox_fecha_fin_final_tramite" runat="server" Style="width:100px; margin-left:10px; border:none; border-bottom:solid; border-bottom-width:1px; border-bottom-right-radius:0px; border-bottom-left-radius:0px" placeholder="yyyy-mm-dd"></asp:TextBox>
                                            <asp:ImageButton ID="ImageButton_fecha_fin_final_tramite" runat="server"  ImageUrl="../imagera/Calendar.png"/>
                                            <asp:CalendarExtender ID="CalendarExtender_fecha_fin_final_tramite" runat="server" TargetControlID="TextBox_fecha_fin_final_tramite"  PopupButtonID="ImageButton_fecha_fin_final_tramite" Format = "yyyy-MM-dd"/>
                                        </td>                           
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    
                                    <tr>
                                        <td>

                                        </td>
                                        <td style="text-align:right">
                                            <br />
                                            <asp:Button ID="Button_consultar" runat="server" Text="Consultar" Style="margin-right:10px"  CssClass="boton_azul" /> 
                                             <asp:Button ID="Button_cancelar" runat="server" Text="Cancelar" Style="margin-right:20px; display:none"  CssClass="boton_blanco" /> 
                                                     
                                        </td>
                                    </tr>
                                    
                                    
                                </table>
                                                         
                            </ContentTemplate>
                        </asp:UpdatePanel>
                           <asp:Button ID="Button_filtro_historico" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                           <asp:Button ID="ButtonSalir_filtro_historico" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                </div>
            </asp:Panel>
        </div>
        <asp:Panel ID="Panel_detalle_radicado" runat="server" Style="display:none; width: auto; height: auto" CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_detalle_radicado" runat="server"
                TargetControlID="ButtonSalir_detalle_radicado" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_detalle_radicado" PopupControlID="Panel_detalle_radicado">
            </asp:ModalPopupExtender>     
            <div id="modal_content_detalle_radicado" class="modal-content">
                <div id="diver_cabcera_detalle_radicado" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ">Detalle radicado</h6>
                    <button type="button" value="Button_cerrar_detalle_radicado" class="close da_event_captive ">&times;</button>
                </div>
                <div id="contenido_procesa_detalle_radicado" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                    <asp:UpdatePanel ID="Update_detalle_radicado" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Panel ID="Panel_detalle_radicado_user" runat="server" Style="height: 100%; width: 99.8%; margin-bottom: 1px; overflow:auto" CssClass="container">
                                <asp:Table ID="Table_detalle_radicado" runat="server" Style="height: 100%; width: 100%; font-size:12px" CssClass="table table-hover" >
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label1" runat="server" Text="RADICADO DEL TRAMITE"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_RADICADO_TRAMITE" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label19_" runat="server" Text="PETICIONARIO O SOLICITANTE"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="LabelDESTINATARIO" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label3" runat="server" Text="TIPO TRÁMITE RADICADO"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_TIPO_TRAMITE" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label6" runat="server" Text="FECHA DE RADICACION DEL TRAMITE"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="LabelFECHA_REGISTRO" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label17" runat="server" Text="FECHA VENCIMIENTO DEL TRAMITE"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_FECHA_VENCE" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_asunto_radicado" runat="server" Text="ASUNTO"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_ASUNTO_RADICADO_" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label4" runat="server" Text="FLUJO RADICADO"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_FLUJO_RADICADO" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label20" runat="server" Text="USUARIO RADICADOR "></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_radicador_usuario" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label11" runat="server" Text="CARGO USUARIO RADICADOR"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_CARGO_USUARIO_RADICADOR" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:Label ID="Label9" runat="server" Text="SEDE USUARIO RADICADOR"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:Label ID="Label_SEDE_USUARIO" runat="server" Text=""></asp:Label>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                            </asp:Panel>
                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>

                    <div style="display: none; height: 1px">
                        <asp:Button ID="ButtonSalir_detalle_radicado" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="Button_cerrar_detalle_radicado" runat="Server" Text="X" CssClass="invisible" />
                    </div>
                </div>
                <div class="modal-footer justify-content-end" id="modal-footer_detalle_radicado">
                    <asp:UpdatePanel ID="UpdatePanel_boton_detalle_radicado" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                           
                            
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
              
            </div>
        </asp:Panel>
       <asp:Panel ID="Panel_interface_consulta_meta_dato" runat="server" Style="display: none; width: 100%; height: auto" CssClass="modal_content_general_">
              <asp:ModalPopupExtender ID="ModalPopupExtender_edition_interface_consulta_meta_dato" runat="server" BackgroundCssClass="FondoAplicacion" 
                  TargetControlID="ButtonSalir_interface_consulta_meta_dato"
                  CancelControlID="Button_cerrar_interface_consulta_meta_dato" PopupControlID="Panel_interface_consulta_meta_dato">
              </asp:ModalPopupExtender>
              <div id="modal_content_consulta_meta_dato" class="modal-content">
                  <div id="divcabecer2_interface_consulta_meta_dato" class="modal_title_superior_ modal-header">
                      <h6 id="label_interface_consulta_meta_dato" class="modal-title  ">Meta datos</h6>
                      <button type="button" value="Button_cerrar_interface_consulta_meta_dato" class="close da_event_captive">&times;</button>
                  </div>
                  <div id="contenido_procesa_interface_consulta_meta_dato" style="background-color: white; width: auto; height: auto; color: black; background-color: #FFFFFF; border-top: none; overflout: auto" class="modal_content_back modal-body">
                      <div id="div_content_tabla">
                          <table
                              id="table_meta_row"
                              data-height="400"
                              data-pagination="false"
                              data-page-list="[10, 25, 50, 100, all]"
                              data-show-export="true"
                              data-toggle="table"
                              data-id-field="ra_m_id"
                              data-search="true"
                              data-locale="es-SP">
                              <thead>
                                  <tr>
                                      <th data-field="ra_m_id" data-visible="false" style="display: none">id_meta_dato</th>
                                      <th data-field="Meta_dato" data-sortable="true" data-sort-name="Meta_dato" data-sort-order="desc">CAMPO</th>
                                      <th data-field="Valor_meta_dato">VALOR</th>
                                      <th data-field="Estado_obligatorio">OBLIGATORIEDAD</th>
                                      <th data-field="Estandar_meta_dato" data-sortable="true" data-sort-name="Estandar_meta_dato" data-sort-order="desc">ESTANDAR</th>
                                      <th data-field="descripcion">DESCRIPCION</th>
                                      <th data-field="Tipo">CONTEXTO</th>
                                  </tr>
                              </thead>
                          </table>
                      </div>
                  </div>
                 
                  <div style="display: none; height: 1px">
                      <asp:Button ID="Button_cerrar_interface_consulta_meta_dato" runat="Server" Text="" />
                      <asp:Button ID="Button_interface_consulta_meta_dato" runat="server" Text="" Height="1px" Width="1px" />
                      <asp:Button ID="ButtonSalir_interface_consulta_meta_dato" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                  </div>
              </div>
          </asp:Panel>
         <asp:Panel ID="Panel_interface_regitra_meta_dato" runat="server" Style="display:none;  width:50%; height: auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_interface_regitra_meta_dato"  runat="server" BackgroundCssClass="FondoAplicacion"  TargetControlID="ButtonSalir_interface_regitra_meta_dato" 
                    CancelControlID="Button_cerrar_interface_regitra_meta_dato" PopupControlID="Panel_interface_regitra_meta_dato" ></asp:ModalPopupExtender>
                <div class="modal-content">
                    <div id="divcabecer2_interface_regitra_meta_dato" class="modal_title_superior_ modal-header">
                        <h6 id="label_interface_regitra_meta_dato" class="modal-title  ">Registra meta dato</h6>
                        <button type="button" value="Button_cerrar_interface_regitra_meta_dato" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="contenido_procesa_interface_regitra_meta_dato" style="background-color: white; width: auto; height: auto; color: black; background-color: #FFFFFF; border-top: none; overflow: auto" class="modal_content_back modal-body">
                        <div id="conte_regitra_meta_dato_control" >
                         
                        </div>
                    </div>
                    <div class="modal-footer" id="modal_footer_regitra_meta_dato">
                        <input id="Button_registra_meta" type="button" value="Aceptar" onclick="event_element_clic(event,this);" class="btn btn-success"/>
                        <input id="Button_file_firma" type="button" value="Aceptar" style="display:none" class="btn btn-success"/>
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_cerrar_interface_regitra_meta_dato" runat="Server" Text="" />
                        <asp:Button ID="Button_interface_regitra_meta_dato" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="ButtonSalir_interface_regitra_meta_dato" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    </div>
                </div>
            </asp:Panel>
        <!--Popup respuesta radicado-->              
              <asp:Panel ID="Panel_respuesta_radicado" runat="server" Style="display:none; width:98%; height:99%" ForeColor="White"   CssClass="modal_content_general">
                  <asp:ModalPopupExtender ID="ModalPopup_respuesta_radicado" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_respuesta_radicado"
                      PopupControlID="Panel_respuesta_radicado"  CancelControlID="ButtonCerrar_respuesta_radicado"></asp:ModalPopupExtender>
                  <div id="Cabecera_respuesta_radicado" class="modal_title_superior" style="">               
                      <div id="Div4_" style="float: right">
                          <button type="button" style="margin-right:10px" value="ButtonCerrar_respuesta_radicado" class="close da_event_captive" title="Cerrar" >&times;</button>                          
                      </div>
                  </div>
                  <div id="div_respuesta_radicado" style="height: auto; width: auto" class="modal_content_back">
                      <div id="center_contenedor_respuesta" style="float: right; width: 100%; height: 100%; left: auto">
                          <div id="opciones_seleccion">
                              <div id="menu_var" class="navbar_gray_ navbar navbar-expand-sm nav_botota_person modal_content_no_back_inferior">
                                  <button class="navbar-toggler" type="button" style="background-color: #6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                                      <span class="navbar-toggler-icon_"><i style="color: white" class="fad fa-th-list"></i></span>
                                  </button>
                                  <div class="collapse navbar-collapse row" id="navbarNavDropdown_rep">
                                      <ul class="navbar-nav col-md-8">
                                          <li class="nav-item dropdown active ml-2 active_">
                                              <a class="nav-link dropdown-toggle bot_hover_person" style="color: #6d7fcc" href="#" id="navbarDropdownMenuLink_rep" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-line-columns"></i> Detalle
                                              </a>
                                              <div class="dropdown-menu " aria-labelledby="navbarDropdownMenuLink_rep">
                                                  <a href="#" class="dropdown-item" onclick="activa_menu_general_diference(event,this,'D-D-R-R')"><i class="fal fa-table"></i> Detalle respuesta radicado</a>
                                                  <a href="#" class="dropdown-item" onclick="activa_menu_general_diference(event,this,'D-V-D-T')"><i class="fal fa-list-ol"></i> Transacciones de la respuesta</a>
                                              </div>
                                          </li>
                                          <li class="nav-item dropdown active ml-2 mr-0 active_">
                                              <a class="nav-link  dropdown-toggle" style="color: #6d7fcc" href="#" id="navbarDropdownMenuLink__rep" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-th-list  "></i> Opciones
                                              </a>
                                              <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink__rep">
                                                  <a href="#" class="dropdown-item" onclick="inicializa_tipo_adjunto_documento(event,this,'A-RENVIA-COR_TR');"><i class="fal fa-envelope-square"></i> Notificar respuesta al correo electrónico</a>
                                                  <a href="#" class="dropdown-item" onclick="activa_menu_general_diference(event,this,'R-R-D')"><i class="fal fa-undo"></i>Reversar gestión tramite</a>
                                                  <a href="#" class="dropdown-item" onclick="inicializa_tipo_adjunto_documento(event,this,'R-ENTIDAD-EXTERNA');"><i class="fal fa-external-link-square"></i> Redirecciona a entidad externa</a>
                                                  <div class="dropdown-submenu ">
                                                      <a class="dropdown-item font-weight-light  dropdown-toggle" style="color: #6d7fcc" href="#" id="A7" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="far fa-pen"></i> Gestión de la solicitud
                                                      </a>
                                                      <div class="dropdown-menu">
                                                          <a href="#" class="dropdown-item" onclick="inicializa_tipo_adjunto_documento(event,this,'R-GESTION-SOLICITUD');"><i class="far fa-pen"></i> Registrar gestión a la solicitud</a>
                                                          <a href="#" class="dropdown-item" onclick="inicializa_tipo_adjunto_documento(event,this,'R-LISTA-GESTION');"><i class="fal fa-list-ul"></i> Lista gestión de la solicitud</a>
                                                         
                                                      </div>
                                                  </div>
                                              </div>
                                          </li>

                                      </ul>
                                  </div>
                              </div>
                          </div>
                          <asp:Panel ID="Panel_seleccion" runat="server" Style="height: 98%; display: block; overflow: auto; background-color: white; border-top: inset 1px solid #ccc">   
                                      <div id="div_title" style="text-align: center">
                                           <asp:Label ID="label_title" runat="server" Text="Respuesta tramite" Style="font-size: 12px; font-family: Arial; color: black; font-weight: 700"></asp:Label>      
                                      </div>
                                      <div id="div_tab_content_resp_general" style="height: auto; margin-left: 15px; margin-right: 15px">
                                          <ul class="tab" style="background-color: white; height: auto">
                                              <li><a style="font-family: Arial" href="javascript:void(0)" class="tablinks__" onclick="openCity__(event, 'div_tab_resp_formal')" id="default_formal"><i class="fad fa-file"></i> Respuesta formal </a></li>
                                              <li><a style="font-family: Arial" href="javascript:void(0)" class="tablinks__" onclick="openCity__(event, 'div_tab_solo_confirmar')" id="default_confirmar"><i class="fad fa-reply"></i> Confirmar o darse por enterado</a></li>
                                          </ul>
                                      </div>
                                      <div id="div_tab_resp_formal" class="tabcontent___boot_da" style="margin-left: 15px; margin-right: 15px; border: none">
                                          <div id="div_resp_formal" style="margin-top: 1px; border-color: #b0c4de; border-style: ridge; border-width: 1px; background-color: #f5f5f5; display: none">
                                              <asp:ImageButton ID="ImageButton_desplegar_formal" runat="server" src="../Radicador/imagenes/mas.png" Style="float: right; height: 15px; margin-right: 5px" OnClientClick="prevent_hident(event)" />
                                              <asp:Label ID="label_text_title" runat="server" Text="(1) Elaborar una respuesta a la petición o solicitud " Style="font-size: 13px; font-family: Arial; font-weight: 600"></asp:Label>
                                              <asp:Image ID="Image_formal_visto" runat="server" src="../Gestion/imagenes/visto_bueno.png" Style="height: 15px; width: 40px; margin-left: 5px; display: none; float: left" />
                                          </div>
                                          <asp:Panel ID="Panel_respuesta_formal" runat="server" Style="width: 100%; height: auto">
                                              <div id="Content" style="height: auto; display: none">
                                                  <ul class="tab_" style="background-color: white; height: auto">
                                                      <li><a style="font-family: Arial" href="javascript:void(0)" class="tablinks" onclick="openCity(event, 'conten_general_respuesta_formal')" id="defaultOpen"><i class="fal fa-file-edit"></i> Elabora respuesta</a></li>
                                                      <li><a style="font-family: Arial" href="javascript:void(0)" class="tablinks" onclick="openCity(event, 'content_anexos')"><i class="fal fa-paperclip"></i> Anexos</a></li>
                                                  </ul>
                                              </div>
                                              <div id="div_image_semaforo" style="width: 100%;" class="modal_content_back_   col-12  d-flex   d-sm-flex d-md-inline d-lg-inline ">
                                                  
                                                  <asp:UpdatePanel ID="UpdatePanel_image_semaforo" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                                      <ContentTemplate>
                                                           <asp:Button ID="Button_update_semaforo" runat="server" Text="" style="display:none"/>
                                                           <asp:Image ID="Image_estado_resp" CssClass="img-responsive" runat="server" Style="height: 50px; width: inherit; float: right" ImageUrl="../radicador/imagenes/electronica_resp_estado_V0.png" />
                                                      </ContentTemplate>
                                                      <Triggers>
                                                          <asp:AsyncPostBackTrigger ControlID="Button_update_semaforo" EventName="Click" />
                                                      </Triggers>
                                                  </asp:UpdatePanel>
                                              </div>
                                              <div id="title_resp" style="margin-top: 1px; border-color: #b0c4de; border-width: 1px; border-style: ridge; text-align: center; width: 100%; float: right; display: none">
                                                  <asp:Label ID="Label_desc" runat="server" Text="" Style="text-align: center; font-family: Arial; font-size: 16px"></asp:Label>
                                              </div>
                                              <div id="conten_general_respuesta_formal" style="float: right; width: 100%; height: 100%; border: none" class="tabcontent_boot_da">
                                                  <div id="Div2" style="float: left; width: 2%; height: 100%; left: auto; position: static; height: 150px; display: none">
                                                      <input id="Hidden_estado_update" type="hidden" value="-1" runat="server" />
                                                      <input id="Hidden_height" type="hidden" value="0" runat="server" />
                                                      <input id="Hidden_width" type="hidden" value="0" runat="server" />
                                                      <asp:UpdatePanel ID="UpdatePanel_hiden_resp" runat="server" UpdateMode="Conditional">
                                                          <ContentTemplate>
                                                              <input id="Hidden_tipo_respuesta" type="hidden" value="-1" runat="server" />
                                                              <asp:Button ID="Button_hident" runat="server" Text="" Style="display: none" />
                                                              <asp:Button ID="Button_inicio_respuesta" runat="server" Text="" Style="display: none" />
                                                          </ContentTemplate>
                                                      </asp:UpdatePanel>
                                                  </div>
                                                  <br />
                                                  <div class="container-fluid pt-2 pb-2 " style="background-color: #e8e8f7">
                                                      <div class="justify-content-center d-none">
                                                          <h5>Descarga formato</h5>
                                                      </div>
                                                      <div class="">
                                                          <h6>Descargar formato o procotolo de respuesta (Opcional)</h6>
                                                          <input type="image" id="image_ayuda_descarga" src="../workflow/imageneswf/ayuda.png" style="height: 20px; display: none" onclick="ayuda_general('DFP', 'image_ayuda_descarga');"/>
                                                      </div>
                                                      <div class="">
                                                          <button type="button" class="btn btn-success da_event_captive mt-1" title="descarga protocolo de respuesta" value="Button_activa_descarga_formato"><i class="fad fa-arrow-to-bottom"></i></button>
                                                      </div>
                                                  </div>
                                                  <div class="container-fluid pt-2 pb-2" style="margin-top: 10px; background-color: #e8e8f7">
                                                      <div class="d-none">
                                                          <h5>Soportes para la respuesta</h5>
                                                      </div>
                                                      <div class="row">
                                                          <div class="col-2 ">
                                                              <h6 class="h6" style="text-overflow: ellipsis">Documento respuesta*</h6>
                                                              <input type="image" id="image_ayuda_adjunta" src="../workflow/imageneswf/ayuda.png" style="height: 20px; display: none" onclick="ayuda_general('ADR', 'image_ayuda_adjunta');"/>
                                                          </div>
                                                          <div class="col-3">
                                                              <h6>Solicitud de aprobación respuesta</h6>

                                                          </div>
                                                          <div class="col-7">
                                                              <h6>Anexos</h6>
                                                          </div>

                                                      </div>
                                                      <div class="row ">
                                                          <div class="col-2">
                                                              <button type="button" title="Cargar documento respuesta" id="boton_carga_documento_respuesta"  onclick="inicializa_tipo_adjunto_documento(event,this, 'S-D-R');" class="btn  btn-warning  mt-1"><i class="fad fa-arrow-from-bottom"></i></button>
                                                              <button type="button" title="Descargar documento respuesta" value="Button_descarga" class="btn btn-success  da_event_captive mt-1"><i class="fad fa-arrow-to-bottom"></i></button>
                                                              <button type="button" title="Eliminar documento respuesta" value="Button_eliminar" style="color: white" onclick="inicializa_tipo_adjunto_documento(event,this, 'E-D-R');" class="btn  btn-danger  mt-1"><i class="fal fa-times"></i></button>
                                                          </div>
                                                          <div class="col-3 d-md-inline">

                                                              <button type="button" title="Solicitar aprobación de la respuesta" onclick="inicializa_tipo_adjunto_documento(event,this,'A-P-S-A')" class="btn  btn-info  mt-1"><i class="fal fa-file-check"></i></button>
                                                              <button type="button" title="Listar solictudes de aprobación de la respuesta" onclick="activa_menu_general_diference(event,this,'S-A-R-G-RD')" class="btn  btn-secondary mt-1"><i class="fal fa-list-ul"></i></button>
                                                          </div>
                                                          <div class="col-7 d-md-inline">
                                                              <asp:UpdatePanel ID="UpdatePanel_anexos_respuesta" UpdateMode="Conditional" runat="server" RenderMode="Inline">
                                                                  <ContentTemplate>
                                                                      <asp:DropDownList ID="DropDownList_anexos_respuesta" runat="server" Style="margin-left: 1px; max-width: 250px" CssClass="dropdown_ form-control d-sm-inline mt-1"></asp:DropDownList>

                                                                  </ContentTemplate>
                                                              </asp:UpdatePanel>
                                                              <button type="button" title="Cargar anexo" onclick="inicializa_tipo_adjunto_documento(event,this, 'S-D-A');" value="Button_anexo_cargar" class="btn  btn-warning  mt-1" ><i class="fad fa-arrow-from-bottom"></i></button>
                                                              <button type="button" title="Eliminar anexo" value="Button_anexo_eliminar" onclick="inicializa_tipo_adjunto_documento(event,this, 'E-A-R');" style="color: white" class="btn  btn-danger  mt-1"><i class="fal fa-times"></i></button>
                                                              <button type="button" title="Descargar anexo" value="Button_descargar_anexo" onclick="inicializa_tipo_adjunto_documento(event,this, 'D-A-R-F');" class="btn btn-success  da_event_captive_ mt-1"><i class="fad fa-arrow-to-bottom"></i></button>

                                                          </div>
                                                      </div>

                                                  </div>
                                                  <div class="container-fluid pt-2 pb-2 " style="margin-top: 10px; background-color: #e8e8f7">
                                                      <div class="d-none">
                                                          <h5>Confirma respuesta para el tramite</h5>
                                                      </div>
                                                      <div class="row">
                                                          <div class="col-6">
                                                              <h6>Confirmar respuesta *</h6>

                                                          </div>
                                                          <div class="col-6">
                                                              <h6>Opciones de respuesta final</h6>
                                                          </div>
                                                      </div>
                                                      <div class="row">

                                                          <div class="col-6">
                                                              <button type="button" title="Confirmar respuesta" value="Button_radicar_tramite" class="btn  btn-primary da_event_captive mt-1"><i class="fad fa-external-link-alt"></i></button>
                                                          </div>
                                                          <div class="col-6">
                                                              <button type="button" title="Descargar respuesta final" value="Button_descarga_respuesta" class="btn btn-success da_event_captive mt-1"><i class="fad fa-arrow-to-bottom"></i></button>
                                                              <button type="button" title="Imprimir respuesta final" value="Button_imprimir" class="btn  btn-secondary da_event_captive mt-1"><i class="fal fa-print"></i></button>
                                                          </div>
                                                      </div>
                                                  </div>

                                                  <div style="display: none">
                                                      <asp:UpdatePanel ID="UpdatePanel_anexos_respuesta_boton" UpdateMode="Conditional" runat="server" RenderMode="Inline">
                                                          <ContentTemplate>
                                                              <asp:Button ID="Button_descargar_anexo" runat="server" Text="Descargar" Style="width: 1px; text-align: center; margin-left: 5px; font-family: Arial; font-size: 12px" CssClass="btn btn-primary" />
                                                              <asp:Button ID="Button_anexo_eliminar" runat="server" Text="Eliminar" Style="width: 1px; text-align: center; margin-left: 5px; font-family: Arial; font-size: 12px" CssClass="btn btn-primary" OnClientClick="prom_respuesta_personalizado('Desea eliminar el anexo seleccionado','Hidden_resp_elimina_anexo')" />
                                                              <asp:Button ID="Button_anexo_cargar" runat="server" Text="Adjuntar" Style="width: 1px; text-align: center; margin-left: 5px; font-family: Arial; font-size: 12px" CssClass="btn btn-primary" />
                                                              <asp:Button ID="Button_sube_documento_adjunto_respuesta" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                                                              <input id="Hidden_resp_elimina_anexo" type="hidden" value="0" runat="server" />
                                                          </ContentTemplate>
                                                      </asp:UpdatePanel>

                                                      <asp:UpdatePanel ID="UpdatePanel_gestion_respuesta" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                                          <ContentTemplate>
                                                              <asp:Label ID="Label_solicitudes" runat="server" Text="Gestión con otros usuarios " Style="text-align: center; font-family: Arial; font-size: 16px; margin-left: 5px; font-weight: 500"></asp:Label>
                                                              <asp:Button ID="ButtonActiva_solicitud_aprobacion" runat="server" Text="Solicitudes de aprobación de la respuesta" ToolTip="Solicitud de aprobación de documento de la respuesta" Style="background-color: white; border-color: #b0c4de; height: 25px; font-size: 12px; font-family: Arial" CssClass="boton" />
                                                              <asp:Button ID="Button_activa_solicitudes_colaboracion" runat="server" Text="Solicitudes de colaboración para elaborar la respuesta" ToolTip="Lista registros de colaboración relacionados al radicado" Style="background-color: white; border-color: #b0c4de; height: 25px; font-size: 12px; font-family: Arial" CssClass="boton" />
                                                          </ContentTemplate>
                                                      </asp:UpdatePanel>

                                                      <asp:UpdatePanel ID="UpdatePanel_respuesta_documento" runat="server" UpdateMode="Conditional">
                                                          <ContentTemplate>
                                                              <asp:Button ID="Button_activa_descarga_formato" runat="server" Style="width: 1px; display: none" />
                                                              <asp:Button ID="Button_carga_plantilla" runat="server" Style="width: 1px; display: none" OnClientClick="eliminar_ajaxtolkit();" />
                                                              <asp:Button ID="Button_descarga" runat="server" Style="width: 1px; display: none" />
                                                              <asp:Button ID="Button_eliminar" runat="server" Style="width: 1px; display: none" OnClientClick="promp_respuesta('Desea eliminar el soporte documental de la respuesta?');" />
                                                              <asp:Button ID="Button_activa_registro_solicitud" runat="server" Style="width: 1px; display: none" />
                                                              <asp:Button ID="Button_radicar_tramite" runat="server" Style="width: 1px; display: none" OnClientClick="asig_correo_token_respuesta('tokenize-callable-demo_respuesta_');lista_tipos_respuesta();" />
                                                          </ContentTemplate>
                                                      </asp:UpdatePanel>
                                                      <asp:UpdatePanel ID="UpdatePanel_combo_plantillas" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                                          <ContentTemplate>
                                                              <asp:Button ID="Button_descarga_respuesta" runat="server" Text="Descargar" Style="font-size: 12px; font-family: Arial; margin-left: 5px; font-family: Arial; font-size: 12px" CssClass="btn btn-primary" />
                                                              <asp:Button ID="Button_imprimir" runat="server" Text="Impresión" Style="font-size: 12px; font-family: Arial; margin-left: 5px; font-family: Arial; font-size: 12px" CssClass="btn btn-primary" />
                                                              <input id="Hidden_resp" type="hidden" value="0" runat="server" />
                                                          </ContentTemplate>

                                                      </asp:UpdatePanel>
                                                  </div>
                                              </div>

                                          </asp:Panel>

                                      </div>

                                      <div id="div_tab_solo_confirmar" class="tabcontent___boot_da" style="height: auto; margin-left: 15px; margin-right: 15px; border: none">

                                          <div id="div_respuesta_confirmar" style="margin-top: 1px; border-color: #b0c4de; border-width: 1px; border-style: ridge; background-color: #f5f5f5; display: none">
                                              <asp:ImageButton ID="ImageButton_confirmar_firma" runat="server" src="../Radicador/imagenes/mas.png" Style="float: right; height: 15px; margin-right: 5px; display: none" OnClientClick="prevent_hident(event)" />
                                              <asp:Label ID="label_text_confirma" runat="server" Style="font-size: 13px; text-align: center; font-family: Arial; font-weight: 600; display: none" Text="(2) Solo confirmar o darse por enterado"></asp:Label>
                                              <asp:Image ID="Image_formal_visto_simple" runat="server" src="../Gestion/imagenes/visto_bueno.png" Style="height: 15px; width: 40px; margin-left: 5px; display: none; float: left" />

                                          </div>
                                          <br />
                                          <asp:Panel ID="panel_respuesta_confirmar" runat="server" Style="">
                                              <div id="tab" style="height: auto; margin-left: 15px; margin-right: 15px; display: none">
                                                  <ul class="tab" style="background-color: white; height: auto">
                                                      <li><a style="font-family: Arial" href="javascript:void(0)" class="tablinks_" onclick="openCity_(event, 'div_confirma')" id="aopendef"><i class="fal fa-reply"></i> Confirmar</a></li>
                                                      <li><a style="font-family: Arial" href="javascript:void(0)" class="tablinks_" onclick="openCity_(event, 'div_enexos_confirma')"><i class="fal fa-paperclip"></i> Anexos</a></li>
                                                  </ul>
                                              </div>
                                              <div class="row">
                                                  <div class="col-6">
                                                      <div class="custom-control custom-checkbox mt-3">
                                                          <asp:CheckBox ID="CheckBox_respuesta_confirmar" CssClass="checkbox" runat="server" Checked="true" Style="font-family: Arial; font-size: 15px; color: black; float: left; margin-right: 15px" Text="Enviar al correo electrónico la confirmación" />
                                                      </div>

                                                  </div>
                                                  <div id="id_semaforo_confirma" class="col-6">
                                                      <asp:UpdatePanel ID="UpdatePanel_image_semaforo_resp" runat="server" UpdateMode="Conditional">
                                                          <ContentTemplate>
                                                              <asp:Image ID="Image_estado_resp_solo_confirm" runat="server" Style="height: 50px; width: auto; float: right; margin-right: 30px" ImageUrl="../radicador/imagenes/resp_solo_elctronica_conf_estado_V0.png" />
                                                          </ContentTemplate>
                                                          <Triggers>
                                                             <asp:AsyncPostBackTrigger ControlID="Button_update_semaforo" EventName="Click" />
                                                          </Triggers>
                                                      </asp:UpdatePanel>
                                                  </div>

                                              </div>

                                              <div id="div_confirma" class="tabcontent__boot_da" style="height: auto; border: none">
                                                  <div class="rows" style="display:flex">
                                                       <div class="col-6" style="display:flex">
                                                          <h6> Correos electrónicos a notificar</h6> 
                                                      </div>
                                                      <div class="col-6" style="display:flex">
                                                          <span style="font-family: 'Segoe UI Emoji'; font-size: 13px; color:darkred"> (Para agregar una nueva dirección de correo electrónico por favor digite y  presione enter.)</span>
                                                     </div>
                                                 </div>
                                                  <div class=" ">     
                                                      <div class="col-12">
                                                          <select class="tokenize-callable-demo_respuesta_simple " style="width: 100%" multiple>
                                                      </div>
                                                  </div>
                                                   <div class="rows pt-3">
                                                      <div class="col-12">
                                                          <span class="font-weight-light">  Nota confirmación </span>
                                                      </div>
                                                      <div class="col-12">
                                                         <asp:TextBox ID="TextBox_nota_confirma" Rows="5" CssClass="form-control" placeholder="Digita nota de confirmación" runat="server" ></asp:TextBox>         
                                                         
                                                      </div>
                                                  </div>
                                  
                                                  <div class="rows pt-3">
                                                      <div class="col-12">
                                                          <span class="font-weight-light">  Seleciona tipo de confirmación *</span>
                                                      </div>
                                                      <div class="col-12">
                                                           <select id="Drop_tipo_respuesta" class="form-control">
                                                          <option></option>

                                                           </select>
                                                      </div>
                                                  </div>
                                                 
                                                  <div class="row container_ mt-2 ml-3 mr-3" style="background-color: #e8e8f7">
                                                      <div class="col-sm-6 pt-1 pb-1 pl-2">
                                                          <asp:UpdatePanel ID="UpdatePanel_anexos_respuesta_simple" UpdateMode="Conditional" runat="server" RenderMode="Inline">
                                                              <ContentTemplate>
                                                                  <div style="display: none">
                                                                      <asp:Button ID="Button_anexo_cargar_simple" runat="server" Text="Adjuntar anexo" Style="margin-left: 5px" CssClass="btn btn-primary" />
                                                                      <asp:Button ID="Button_descargar_anexo_simple" runat="server" Text="Descargar anexo" Style="margin-left: 5px" CssClass="btn btn-primary" />
                                                                      <asp:Button ID="Button_anexo_eliminar_simple" runat="server"  Text="Eliminar anexo" Style="margin-left: 5px" CssClass="btn btn-primary" />
                                                                      <asp:Button ID="Button_sube_documento_adjunto_respuesta_simple" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                                                                  </div>
                                                                  <asp:DropDownList ID="DropDownList_anexos_respuesta_simple" runat="server" class="custom-select mr-sm-4 form-control" Style="margin-left: -3px; max-width: 250px"></asp:DropDownList>
                                                              </ContentTemplate>
                                                          </asp:UpdatePanel>
                                                          <button type="button" title="Cargar anexo" value="Button_anexo_cargar_simple" onclick="inicializa_tipo_adjunto_documento(event,this, 'S-D-A');" class="btn  btn-warning  mt-1"><i class="fad fa-arrow-from-bottom"></i></button>
                                                          <button type="button" title="Eliminar anexo" value="Button_anexo_eliminar_simple" onclick="inicializa_tipo_adjunto_documento(event,this, 'E-A-R-S');" style="color: white" class="btn  btn-danger  mt-1"><i class="fal fa-times"></i></button>
                                                          <button type="button" title="Descargar anexo" value="Button_descargar_anexo_simple" onclick="inicializa_tipo_adjunto_documento(event,this, 'D-A-R-S');" class="btn btn-success  da_event_captive_ mt-1"><i class="fad fa-arrow-to-bottom"></i></button>
                                                      </div>
                                                      <div class="col-sm-6 pr-1 pt-1 pb-1  float-right">
                                                          <button type="button" title="Confirmar la solicitud"  onclick="inicializa_tipo_adjunto_documento(event,this, 'R-R-R-S');" class="btn btn-primary  float-right  da_event_captive_ mt-1"> Aceptar</button>  
                                                      </div>
                                                  </div>
                                                  <asp:UpdatePanel ID="UpdatePanel_boton_nota" runat="server" UpdateMode="Conditional">
                                                      <ContentTemplate>
                                                          <asp:Button ID="Button_nota_solo_confirmar" runat="server" Text="Nota" ToolTip="Agregar una nota a la confirmación" Style="float: right; display: none" CssClass="btn btn-primary" />
                                                      </ContentTemplate>
                                                  </asp:UpdatePanel>
                                              </div>



                                          </asp:Panel>
                                      </div>

                               
                              <div id="div_pie" style="background-color: #b0c4de; text-align: center; display: none">
                                  <asp:Label ID="label_result" runat="server" Text="" Style="font-size: 15px; font-family: Arial">

                                  </asp:Label>
                              </div>
                          </asp:Panel>

                      </div>
                      <asp:UpdatePanel ID="UpdatePanel_respuesta_radicado" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_respuesta_radicado_" runat="server" frameborder="0" style="width: 100%; height: 100%;"></iframe>
                              <asp:Button ID="Button_activa_respuesta_radicado" runat="server" Text="Button" Style="display: none" OnClientClick="auto_zise_popup_respuesta();" />
                              <asp:Button ID="Button_activa_respuesta_radicado_tag" runat="server" Text="Button" Style="display: none" />
                              <input id="Hidden_radicado" type="hidden" value="" runat="server" />
                              <input id="Hidden_id_respuesta" type="hidden" value="-1" runat="server" />
                          </ContentTemplate>

                      </asp:UpdatePanel>
                  </div>
                  <asp:Button ID="ButtonSalir_respuesta_radicado" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                  <asp:Button ID="ButtonCerrar_respuesta_radicado" runat="Server" Text="X"
                              ForeColor="#000066" Height="21px" style="display:none" />
                  <asp:UpdatePanel ID="UpdatePanel_valida_cerra_respuesta_radicado" runat="server" UpdateMode="Conditional">
                      <ContentTemplate>
                          <asp:Button ID="Button_valida_Cerrar_respuesta_radicado" runat="Server" Text="hiden"
                              Style="display: none" />
                          <input id="Hidden_estado_tramite" type="hidden" value="" runat="server"/>
                      </ContentTemplate>
                  </asp:UpdatePanel>
                   

              </asp:Panel>
         <!--Notifca respuesta al correo electronico-->
        <div id="notifica_correo_respuesta">
            <asp:Panel ID="Panel_notifica_correo_respuesta" runat="server" Style="display:none;  width:50%; height:auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_notifica_correo_respuesta" runat="server" TargetControlID="ButtonSalir_notifica_correo_respuesta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_notifica_correo_respuesta" PopupControlID="Panel_notifica_correo_respuesta" ></asp:ModalPopupExtender>      
                    <div id="div_cap_notifica_correo_respuesta" class="modal_title_superior_ modal-header">
                           <h6 class="modal-title">Notifica respuesta al correo electrónico</h6>
                           <button type="button" value="Button_cerrar_notifica_correo_respuesta" class="close da_event_captive">&times;</button>    
                    </div>
                    <div id="contenido_procesa_notifica_correo_respuesta" style="background-color: white; width: 100%; height: auto ; border-top:none" class="modal_content_back modal-body">
                        <div style="margin-left: 10px; margin-right: 10px">
                            <div class="row">
                                <div class="pl-3">
                                    <asp:CheckBox ID="CheckBox_anexa_anexos" runat="server" Checked="true" Text=" "  />
                                </div>
                                  <div class="col-11">
                                    <p>Adjunta al correo los anexos de la respuesta</p>
                                 </div>
                            </div>   
                            <div class="row" style="margin-top: 10px">
                                <div class="col-sm-2">
                                    <asp:Label ID="Label12" runat="server" Text="Correos electrónicos*" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label>
                                </div>
                                <div class="col-sm-10">
                                    <select class="tokenize-callable-demo_respuesta__" multiple style="width: 98%;">
                                    </select>
                                </div>
                            </div>                       
                        </div>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button_notifica_correo_respuesta" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                            <asp:Button ID="ButtonSalir_notifica_correo_respuesta" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                            <asp:Button ID="Button_cerrar_notifica_correo_respuesta" runat="Server" Text="X" CssClass="invisible" />
                        </div>
                                   
                    </div>
                      <div class="modal-footer">
                          <button type="button" title="Noficar la gestión al correo electrónico"  onclick="inicializa_tipo_adjunto_documento(event,this, 'N-R-C-E');" class="btn  btn-success  float-right  da_event_captive_ mt-1"> Aceptar</button>  
                     </div>
                
            </asp:Panel>
        </div>
         <!--Popup registrar nueva solicitud-->
            <asp:Panel ID="Panel_actualizacion_anualidad" runat="server"   Style=" width: 90%; height:auto; display:none" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_actualizacion_anualidad" runat="Server" BackgroundCssClass="FondoAplicacion" 
                     TargetControlID="ButtonSalir_actualizacion_anualidad"
                    PopupControlID="Panel_actualizacion_anualidad" CancelControlID="ButtonCerrar_actualizacion_anualidad"></asp:ModalPopupExtender>     
                <div class="modal-content">
                    <div id="div3" class="modal_title_superior_ modal-header">
                           <h6 class="modal-title">Solicitud de aprobación</h6>
                           <button type="button" value="ButtonCerrar_actualizacion_anualidad" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="Cotenedor_actualizacion_anualidad" style="color: Black; background-color: #FFFFFF; height: 100%; width: 100%; border-top:none" class="modal_content_back modal-body">
                        <div style="margin-left: 10px; margin-right: 10px; margin-top: 10px">
                            <div class="row">
                                <div class="col-sm-12">
                                    <asp:UpdatePanel ID="UpdatePanel_registro_solicitud" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div style="margin-top: 1px; background-color: white">
                                                <div class="form-group-sm">
                                                    <h6 class="h6">Prioridad de la solicitud</h6>
                                                    <asp:DropDownList ID="DropDownList_prioridad_solicitud" CssClass="form-control" runat="server" Style="width: 200px"></asp:DropDownList>
                                                </div>
                                                <div class="form-group-sm">
                                                    <h6 class="h6 mt-2">Fecha límite de aprobación</h6>
                                                    <div class="row">
                                                        <div class="col-sm-2">
                                                            <asp:TextBox ID="TextBox_fecha_limite_solicitud" CssClass="form-control" runat="server" Width="100px"></asp:TextBox>
                                                        </div>
                                                        <div class="col-sm-1">
                                                            <asp:CalendarExtender ID="TextBoxFECHA_EXTREMA_INICIAL_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_EXTREMA_INICIAL_CalendarExtender" TargetControlID="TextBox_fecha_limite_solicitud" Format='yyyy-MM-dd' PopupButtonID="acalendar" />
                                                            <a id="acalendar" class="" style="" title="Examinar el calendario" href="#"><i style="margin-left: 1px" class="fal fa-calendar-alt fa-2x"></i></a>
                                                        </div>
                                                    </div>

                                                </div>

                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                            <div class="row "  style="margin-top: 10px">
                                <div   class="col-sm-12" style="overflow:auto">
                                    <select class="tokenize-callable-demo_respuesta___" style="width: 99%;" multiple>
                                    </select>
                                </div>
                            </div>
                            <div>
                                <asp:UpdatePanel ID="UpdatePanel_boton_nota_active" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <div class="col-sm-12">
                                                <asp:TextBox ID="TextBox_nota_aprobacion" runat="server" CssClass="form-control" TextMode="MultiLine" Style="width: 99.5%; font-size: 12px; margin-top: 3px; margin-left: 3px" Rows="2" placeholder="Digita nota solicitud.."></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-sm-12">
                                                <div class="modal-footer">
                                                    <button type="button" value="ButtonCerrar_actualizacion_anualidad" class="da_event_captive btn btn-default">Cancelar</button>
                                                    <button type="button" class="btn  btn-success" onclick="inicializa_tipo_adjunto_documento(event,this, 'R-S-A-D')">Aceptar</button>
                                                </div>
                                                <input id="Hidden_resultado_actualizar" runat="server" type="hidden" value="" />
                                            </div>
                                        </div>
                                        <asp:Label ID="Label30" runat="server" Text="Usuarios relacionados a la solicitud de aprobación" Style="font-size: 14px; text-wrap: normal; display: none"></asp:Label>
                                        <asp:Button ID="Button_activa_usuario_relacion" runat="server" Text="Ver usuarios relacionados" CssClass="boton" Style="width: 96%; display: none" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                        <div style="display: none; height: 1px">
                            <asp:Button ID="ButtonCerrar_actualizacion_anualidad" runat="Server" Text="" Height="1px" Width="1px" Style="display: none" />
                            <asp:Button ID="ButtonSalir_actualizacion_anualidad" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                        </div>

                    </div>
                </div>
            </asp:Panel>
        <!--mensaje_progreso evento-->
        <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>
         <div id="derecho" style="float:right; width:1%; height:100%; position:static; height:150px; display:none">
             <input id="Hidden_id_propietario_resp" type="hidden" value="-1" runat="server"/>
             <input id="Hidden_obliga_rep" type="hidden" value="-1" runat="server"/>
             <input id="Hidden_tipo_resp" type="hidden" value="-1" runat="server"/>
             <input id="Hidden_estado_evento" type="hidden" value="-1" runat="server"/>
             <input id="Hidden_select_tab" type="hidden" value="" runat="server"/>
            <input id="Hidden_select_tab_" type="hidden" value="" runat="server"/>  
            <input id="Hidden_text_user_correo" type="hidden" value="" runat="server"/> 
            <input id="Hidden_text_user" type="hidden" value="" runat="server"/>    
            
        </div>
         <div style="display: none">
            <asp:UpdatePanel ID="UpdatePanel_descarga_hml_dowload" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <input id="Hidden_descarga_hml_dowload" type="hidden" value="1" runat="server"/>
                   
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
       <!--mensaje_personalizado-->
        <asp:Panel ID="Panel_mensaje_personalizado" runat="server" Style="display: none; color: black; width: auto; height: auto; z-index: 99999999999">
            <asp:ModalPopupExtender ID="ModalPopupExtender_mensaje_personalizado" runat="server"
                TargetControlID="Button_mensaje_personalizado" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_mensaje_personalizado" PopupControlID="Panel_mensaje_personalizado">
            </asp:ModalPopupExtender>
            <div class="modal-content">
                <div id="div_persoanlizado" class="modal-header">
                    <a class="modal-title h6 " href="#" style="color: orange"><i class="fas fa-exclamation-triangle"></i></a>
                    <br />
                    <button type="button" onclick="document.getElementById('Button_cerrar_mensaje_personalizado').click();" class="close">&times;</button>
                </div>
                <div id="contenido_procesa_mensaje_personalizado" style="max-width: 450px; max-height: 350px; background-color: white; color: black; overflow: auto" class="modal-body  text-justify">
                    <asp:Label ID="Label_mensaje_personalizado" runat="server" Text="Detalle" Style=""></asp:Label>
                    <asp:Button ID="Button_mensaje_personalizado" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_mensaje_personalizado" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="Button_cerrar_mensaje_personalizado" CssClass="invisible" runat="Server" />
                </div>
                <div class="modal-footer ">
                    <button type="button" class="btn  btn-light  float-right" style="margin-right: 5px; color: orange" onclick="document.getElementById('Button_cerrar_mensaje_personalizado').click();">Aceptar </button>

                </div>
            </div>
        </asp:Panel>
        <!--Termina mensaje_personalizado-->
        <!--POPUP EXPORTAR DOCUMENTO-->
        <div id="Divdescarga_anexo_respuesta">
            <asp:Panel ID="Panel_descarga_anexo_respuesta" runat="server" Style="display:none; width:40%; height:auto" CssClass="modal_content_general_">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_descarga_anexo_respuesta" runat="server" BehaviorID="Panel_descarga_anexo_respuesta_ModalPopupExtender" TargetControlID="ButtonSalir_descarga_anexo_respuesta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_descarga_anexo_respuesta" PopupControlID="Panel_descarga_anexo_respuesta"></asp:ModalPopupExtender>
                <div class="modal-content">
                    <div id="div9" class="modal_title_superior_ modal-header">
                             <h6 class="modal-title">Descarga</h6>
                             <button type="button" value="Button_cerrar_descarga_anexo_respuesta" class="close da_event_captive">&times;</button>                  
                    </div>
                    <div id="contenido_procesa_descarga_anexo_respuesta" style="width: 100%; height: 100%; border-top:none" class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_descarga_anexo_respuesta" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <iframe id="ifimpre_descarga_anexo_respuesta_" runat="server" style="border: none; width: 100%; min-height:100px"></iframe>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div style="display:none; height:1px">
                             <asp:Button ID="Button_cerrar_descarga_anexo_respuesta" runat="Server" Text="" CssClass="invisible"
                                />
                              <asp:Button ID="ButtonSalir_descarga_anexo_respuesta" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        </div>
                        
                    </div>
                </div>
                
           </asp:Panel>
        </div>    

    </form>
      <script  accesskey="javascript" type="text/javascript"> 
          function openCity(evt, cityName) {
              var i, tabcontent, tablinks;
              tabcontent = document.getElementsByClassName("tabcontent_boot_da");
              for (i = 0; i < tabcontent.length; i++) {
                  tabcontent[i].style.display = "none";
              }
              tablinks = document.getElementsByClassName("tablinks");
              for (i = 0; i < tablinks.length; i++) {
                  tablinks[i].className = tablinks[i].className.replace(" active_boot_da", "");
              }
              document.getElementById(cityName).style.display = "block";
              document.getElementById("Hidden_select_tab").value = cityName;
              evt.currentTarget.className += " active_boot_da";

          }

          function openCity_(evt, cityName) {
              var i, tabcontent, tablinks;
              tabcontent = document.getElementsByClassName("tabcontent__boot_da");
              for (i = 0; i < tabcontent.length; i++) {
                  tabcontent[i].style.display = "none";
              }
              tablinks = document.getElementsByClassName("tablinks_");
              for (i = 0; i < tablinks.length; i++) {
                  tablinks[i].className = tablinks[i].className.replace(" active_vis_boot_da", "");
              }
              document.getElementById(cityName).style.display = "block";
              document.getElementById("Hidden_select_tab_").value = cityName;
              evt.currentTarget.className += " active_vis_boot_da";

          }
          function openCity__(evt, cityName) {
              var i, tabcontent, tablinks;
              tabcontent = document.getElementsByClassName("tabcontent___boot_da");
              for (i = 0; i < tabcontent.length; i++) {
                  tabcontent[i].style.display = "none";
              }
              tablinks = document.getElementsByClassName("tablinks__");
              for (i = 0; i < tablinks.length; i++) {
                  tablinks[i].className = tablinks[i].className.replace(" active_vis__boot_da", "");
              }
              document.getElementById(cityName).style.display = "block";

              evt.currentTarget.className += " active_vis__boot_da";

          }
          // Get the element with id="defaultOpen" and click 

          asig_tab_respuesta();
          function asig_tab_respuesta() {
              document.getElementById("defaultOpen").click();
              document.getElementById("aopendef").click();
              if (document.getElementById("Hidden_obliga_rep").value == 1) {
                  document.getElementById("default_formal").click();
                  document.getElementById("default_confirmar").style.display = "none";
              } else {
                  document.getElementById("default_confirmar").click();
              }
          }
          AjaxFileUpload_change_text();

          $(document).ready(function () {
              $('#sidebarCollapse_').on('click', function () {
                  $('#sidebar__').toggleClass('active_da_slider');
                  $('#Contenedorderecho_').toggleClass('active_content_rigth');
                  $('#Contentizquierdo_').toggleClass('active_content_left');
                  $(this).toggleClass('active_da_slider');
                  $('#da_show-sidebar__').toggleClass('show_da_slide');
                  $('#da_show-sidebar__').toggleClass('hide_da_sidebar');
              });
              $('#da_show-sidebar__').on('click', function () {
                  $('#sidebar__').toggleClass('active_da_slider');
                  $('#Contenedorderecho_').toggleClass('active_content_rigth');
                  $('#Contentizquierdo_').toggleClass('active_content_left');
                  $(this).toggleClass('show_da_slide');
                  $(this).toggleClass('hide_da_sidebar');
              });
              $('#sidebarCollapse').on('click', function () {
                  $(this).toggleClass('active_da_slider_rigth');
                  $('#da_show-sidebar_').toggleClass('show_da_slide_rigth');
                  $('#da_show-sidebar_').toggleClass('hide_da_sidebar_rigth');
                  $("#contenido_indice").css("display", "none");
                  $("#contenido_imagen").css("width", "75%");
                  auto_zise_popup_lista_imagenes_gestion();
              });
              $('#da_show-sidebar_').on('click', function () {
                  $(this).toggleClass('show_da_slide_rigth');
                  $(this).toggleClass('hide_da_sidebar_rigth');
                  $("#contenido_indice").css("display", "block");
                  $("#contenido_indice").css("width", "20%");
                  $("#contenido_imagen").css("width", "55%");
                  auto_zise_popup_lista_imagenes_gestion();
              });
          });
          
      </script>
</body>
</html>
