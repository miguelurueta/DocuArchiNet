<%@ Page Language="vb" AutoEventWireup="false" EnableEventValidation="false"  CodeBehind="WebFormGaGestionExpediente.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormGaGestionExpediente_vb" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
   <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
     <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <script src="../js/gestion/WebFormGaGestionExpediente.js"></script>
    <script src="../js/java_general/general_code_java.js"></script>
    <script src="../js/validate_campos.js"></script> 
    <script src="../js/java_general/row_multiple_gred.js"></script>
     <script  src="../Awesome/js/all.js"></script>
     <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
  <link href="../Awesome/css/brands.css" rel="stylesheet"/>
  <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
  <script  src="../Awesome/js/solid.js"></script>
  <script  src="../Awesome/js/fontawesome.js"></script>
   
</head>
<body>
    
    <form id="formGaGestionExpediente" runat="server">   
     <asp:ScriptManager ID="ScriptManager1" runat="server">
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
                elment_postbak = args.get_postBackElement();
                posicion_update_pogres('progres_bar');
                var elmen = document.getElementById(elment_postbak.id)
                if (elmen.type == "button" || elmen.type == "image" || elmen.type == "submit") {
                    elmen.disabled = true;
                }
            }
            function CheckStatus(sender, args) {
                progres_hiden('progres_bar');
                //Button_Lista_Radicados Button_actualizar_guia Button_anular_guia
                var elmen = document.getElementById(elment_postbak.id)
                if (elmen.type == "button" || elmen.type == "image" || elmen.type == "submit") {
                    elmen.disabled = false;
                    //elmen.value = value_element;
                }
                if (elment_postbak.id == "Button_archivar_expediente_gestion") {
                    auto_zise_reasigna_expe_unidad();
                }
                if (elment_postbak.id == "Button_ubicacio_expediente_gestion") {
                    auto_zise_ubicacion_toponimica();
                }
                //Eliminar registro en java
                if (elment_postbak.id == "Button_eliminar_expediente_gestion") {
                    if (document.getElementById("Hidden_result").value == "YES") {
                        eliminar_fila_data_gred('data_grid');
                        document.getElementById("Hidden_result").value = "NO";
                    }
                    
                }
                if (elment_postbak.id == "image_ayuda_responsable") {
                    ayuda_compartir('image_ayuda_responsable');
                }
                if (elment_postbak.id == "Button_nuevo_expediente_gestion") {
                    //auto_zise_agregar_expediente();
                }
                if (elment_postbak.id == "Button_Editar_expediente_gestion") {
                    auto_zise_editar_expediente();
                }
               
                if (elment_postbak.id == "Button_volumen_expediente_gestion") {
                    auto_zise_editar_expediente();
                }
                if (elment_postbak.id == "Button_documentos_relacionado") {
                    tab_sow("home_documentos_expediente");
                   
                }
                if (elment_postbak.id == "ButtonConsulta" || elment_postbak.id == "ButtonConsultaLike") {
                    if (document.getElementById("Hidden_res_consulta").value = "YES") {
                        tab_sow("home-expediente");
                    }
                }
               
                if (elment_postbak.id == "Button_active_eli_rel" && document.getElementById("Hidden_eli_result").value == "YES") {
                    eliminar_fila_data_gred_lista("data_grid_volumenes_relacionados", "Hidden_eli_rel");
                    cambia_icono_gred('data_grid');
                    document.getElementById("Hidden_eli_result").value = "";
                }
                if (elment_postbak.id == "Button_active_eli_rel" && document.getElementById("Hidden_eli_result").value == "") {
                    eliminar_fila_data_gred_lista("data_grid_volumenes_relacionados", "Hidden_eli_rel");
                    document.getElementById("Hidden_eli_result").value = "";
                }
                if (elment_postbak.id == "Button_activa_ventana_rel_volumen") {
                    //auto_zise_popup_padres_relacionados();
                }
                if (elment_postbak.id == "Button_active_eli_rel_padres" && document.getElementById("Hidden_eli_result_padres").value == "YES") {
                    cambia_icono_gred_volumen();
                    document.getElementById("Hidden_eli_result_padres").value = "";
                }
                if (elment_postbak.id == "Button_indice_expediente") {
                    
                    auto_zise_popup_indice_expediente();

                }
                if (elment_postbak.id == "Button_listar_volumenes_relacionados") {
                   
                    auto_zise_popup_volumenes_relacionados();
                }
            }

        </script>
  <div id="contendor_principal" style="height: 100%; width:100%">
        <input id="Hiddenheigpagina" type="hidden" value="475" runat="server"/>
        <input id="Hiddennameasigna" type="hidden" value="" runat="server"/>
        <input id="Hidden_res_consulta" type="hidden" value="" runat="server"/>
        <input id="HiddenPROMP" type="hidden" value="0" runat="server"/>
        <asp:HiddenField ID="Hidden0003" runat="server" Value="" />
        <asp:HiddenField ID="Hidden0005" runat="server" Value="1" />
        <asp:HiddenField ID="Hidden0006" runat="server" Value="0" />
        <asp:HiddenField ID="Hidden0007" runat="server" Value="" />
        <asp:HiddenField ID="Hidden0008" runat="server" Value="0" />
         <nav id="menu_var" class="navbar navbar-expand-sm nav_botota_person_gray modal_content_no_back_inferior">
                <button id="nav_togle_display" class="navbar-toggler" type="button" style=" background-color:#6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                   <span class="navbar-toggler-icon_"><i style="color:white" class="fad fa-th-list"></i></span>
               </button>
                <div class="collapse navbar-collapse row" id="navbarNavDropdown">
                    <ul class="navbar-nav col-md-8"> 
                         <li class="nav-item dropdown active ml-2 active_">                  
                            <a class="nav-link dropdown-toggle bot_hover_person" style="color:#6d7fcc" href="#" id="navbarDropdownMenuLink" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"> <i style="color:#0062cc" class="fad fa-folders"></i>  Expedientes
                            </a>
                            <div class="dropdown-menu " aria-labelledby="navbarDropdownMenuLink">  
                                 <a href="#" class="dropdown-item"  onclick="activa_boton_client_server('Button_nuevo_expediente_gestion'); asig_add_edit_expediente('add')"><i class="fal fa-folder-plus"></i> Nuevo expediente</a>
                                 <a href="#" class="dropdown-item"  onclick="activa_boton_client_server('Button_Editar_expediente_gestion') ; asig_add_edit_expediente('edit')"><i class="fal fa-edit"></i> Editar expediente</a>
                                 <a href="#" class="dropdown-item" onclick="activa_boton_client_server('Button_eliminar_expediente_gestion')"><i class="fal fa-folder-times"></i> Eliminar expediente</a>
                                 <a href="#" class="dropdown-item" onclick="activa_boton_client_server('Button_estado_expediente_gestion')"><i class="fal fa-folder"></i> Estado expediente</a>  
                                 <a href="#" class="dropdown-item" onclick="activa_boton_client_server('Button_general_indice_expediente')"><i class="fal fa-info-square"></i> Crear indice de expediente</a>  
                                 <a href="#" class="dropdown-item" onclick="activa_boton_client_server('Button_indice_expediente')"><i class="far fa-list-alt"></i> Mostrar indice de expediente</a>  
                                 
                            </div>
                        </li>
                        <li class="nav-item dropdown active ml-2 mr-0 active_">
                           <a class="nav-link  dropdown-toggle" style="color: #6d7fcc" href="#" id="A3" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-folder-tree "></i> Ubicación 
                           </a>
                           <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                                <a href="#" class="dropdown-item" onclick="activa_boton_client_server('Button_archivar_expediente_gestion')"><i class="fad fa-archive"></i> Archivar unidad documental (Expediente)</a>  
                                <a href="#" class="dropdown-item" onclick="activa_boton_client_server('Button_desachivar_expediente_gestion')"><i class="fal fa-archive"></i> Desarchivar unidad documental (Expediente)</a>
                                <a href="#" class="dropdown-item" onclick="activa_boton_client_server('Button_ubicacio_expediente_gestion')"><i class="fal fa-folder-tree"></i> Ubicación unidad documental (Expediente)</a>                
                           </div>
                       </li>
                        <li class="nav-item dropdown active ml-2 mr-0 active_  ">
                            <a class="nav-link  dropdown-toggle" style="color: #6d7fcc" href="#" id="A1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-stamp"></i>  Rotulo
                           </a>
                            <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                                 <a href="#" class="dropdown-item" onclick="activa_boton_client_server('ButtonRotulo') " ><i class="fal fa-file-download"></i> Descargar rótulo unidad documental (Expediente)</a>
                                 <a href="#" class="dropdown-item" onclick="activa_boton_client_server('Button_rotulo_expediente_gestion')"><i class="fad fa-print"></i> Imprimir rótulo unidad documental (Expediente)</a>
                                 <a href="#" class="dropdown-item" onclick="activa_boton_client_server('Button_configura_rotulo')"><i class="fad fa-tools"></i> Configuración rótulo unidad documental (Expediente)</a>
                            </div>
                        </li> 
                         <li class="nav-item dropdown active ml-2 mr-0 active_">
                            <a class="nav-link  dropdown-toggle" style="color: #6d7fcc" href="#" id="A2" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-database"></i> Volumen
                           </a>
                            <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">                
                                   <a href="#" class="dropdown-item" onclick="activa_boton_client_server('Button_volumen_expediente_gestion') ; asig_add_edit_expediente('vol')"> <span class="fa-stack fa-1x" style="width:1.2em; height:1em" > <i class="fal fa-database fa-stack-1x" style="font-size: 1.2em" ></i> <i class="far fa-plus fa-stack-1x " style="font-size: 0.8em; color:red; margin-top:1px; margin-right:1px; font-weight:700; display:none" ></i> </span>  Crear y extender un volumen del expediente seleccionado</a>
                                   <a href="#" class="dropdown-item" onclick="activa_boton_client_server('Button_activa_ventana_rel_volumen')"><span class="fa-stack fa-1x" style="width:1.2em; height:1em" > <i class="fal fa-coins fa-stack-1x" style="font-size: 1.2em;" ></i> <i class="fas fa-union fa-stack-2x " style="font-size: 0.6em; margin-top:1px; margin-right:1px; display:none" ></i> </span> Relacionar un expediente como volumen del expediente seleccionado </a>
                                   
                           </div>
                        </li>
                    </ul>
                    <div class=" float-md-right col-md-3 float-sm-left mr-1">
                        <div class="input-group ">
                            <button id="td-boton" class="btn btn-outline-secondary border-right-2 " style="background-color:white" title="Restaura busqueda por campos" style="border-top-right-radius: 0px; border-bottom-right-radius: 0px" onclick="restore_acti_busq_general_archivo_boton(event,this)" type="button">
                                <i class="fal fa-long-arrow-left"></i>
                            </button>
                            <asp:TextBox ID="TextBox_buequeda_general" runat="server"  class="form-control form-control-sm complex  border-left-0" placeholder="Busqueda expedientes...." ></asp:TextBox>
                            <div class="input-group-append">
                                <button class="btn btn-outline-secondary" style="background-color:white" onclick="acti_busq_general_archivo_boton(event, this)" type="button">
                                    <i class="fal fa-search"></i>
                                </button>
                            </div>
                        </div>
                    </div>     
                </div>
            </nav>
            <a id="da_show-sidebar_" class="btn btn-sm   show_da_slide "  href="#" data-target="#sidebar_"  title="Despliega consulta avanzada">
                <i style="color: white" class="fas fa-bars"></i>
            </a>
      <div id="menu_var_" class="navbar_gray" style="overflow: auto; display: none">
          <asp:UpdatePanel ID="UpdatePanel_menu_var_event" runat="server" UpdateMode="Conditional">
              <ContentTemplate>
                  <input id="Hidden_menu_var_event_dive" type="hidden" value="" runat="server" />
                  <asp:Button ID="Button_me_active_men_dive" runat="server" Text="" Style="display: none; width: 1px; height: 1px" />
                  <asp:Button ID="Button_general_indice_expediente" runat="server" Text="" Style="display: none; width: 1px; height: 1px" />
                  <asp:Button ID="Button_indice_expediente" runat="server" Text="" CssClass="boton" Style="width: 0px; display: none" />
                  <input id="Button_elimina_relacion_volumen" style="display: none; width: 1px; height: 1px" type="button" value="button" />
              </ContentTemplate>
          </asp:UpdatePanel>
      </div>
      <div id="da_content_wraper" class="ml-0 mr-2  d-flex " style="padding-left: 1px; padding-right: 1px">      
          <div id="Contentizquierdo" style="width:25%; float:left;border-bottom: 1px solid #e9ecef;  border-right: 1px solid #e9ecef" class="active_content_left">
              <nav id="sidebar_" class="pl-0 pr-0 active_da_slider" style="width: 100%">
                  <div id="title_treview" class="modal-header_ modal_title_superior " style="background: #6d7fcc; border-top-left-radius: initial; border-top-right-radius: initial" >
                         <h6 class=" mt-2 mb-2 ml-2 font-weight-light" id="pit_" style="color: white; float:left; font-family:'Segoe UI'" > Consulta </h6>
                         <a id="sidebarCollapse"   class="close_ mr-2"  style=" float:right; color:white; height:10px" title="Ocultar consulta avanzada">  <i class="fal fa-times  fa-1x font-weight-light"></i></a>
                    </div>
                  <asp:UpdatePanel ID="UpdatePaneLconsulta" runat="server" UpdateMode="Conditional">
                      <ContentTemplate>
                          <asp:DropDownList ID="DropDownListEntidadEmpresa" CssClass="custom-select" runat="server" Style="display: none" Width="100%" onchange="cambio_empresa_gestion_consulta()"></asp:DropDownList>
                          <asp:Panel ID="Panelcampos" runat="server" ScrollBars="Both"
                              Height="90%" Width="100%" Style="background-color: white; position: inherit" CssClass="p-4">
                              <div class="row">
                                  <span>Código único</span>
                              </div>
                              <div class="row">
                                  <asp:TextBox ID="TextBoxID_EXPEDIENTE" runat="server" Width="100%" cap_name="ID_EXPEDIENTE" CssClass="form-control solo-numero event_auto_source_expe"></asp:TextBox>
                              </div>
                              <div class="row mt-3">
                                  <span>Nombre y/o Consecutivo</span>
                              </div>
                              <div class="row">
                                  <asp:TextBox ID="TextBoxCODIGO_UNICO" runat="server" Width="100%" cap_name="CODIGO_UNICO" CssClass="form-control event_auto_source_expe"></asp:TextBox>
                              </div>
                              <div class="row mt-3">
                                  <span>Fecha creación (yyyy-mm-dd)</span>
                              </div>
                              <div class="row p-0">
                                  <div class="col-4 p-0">
                                      <asp:TextBox ID="TextBoxFECHA_CREACION_INI" runat="server" cap_name="FECHA_CREACION" CssClass="event_auto_source_expe" Style="width: 95%" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                      <asp:CalendarExtender ID="TextBoxFECHA_CREACION_INI_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_CREACION_INI_CalendarExtender" TargetControlID="TextBoxFECHA_CREACION_INI" DaysModeTitleFormat='yyyy-MM-dd' Format='yyyy-MM-dd' PopupButtonID="ImageButtonCreacionIni" />

                                  </div>
                                  <div class="col-2 p-0">
                                      <button class="ml-1 btn border-0" id="ImageButtonCreacionIni" type="button">
                                          <i class="fad fa-calendar-alt fa-1x"></i>
                                      </button>
                                  </div>
                                  <div class="col-4 p-0">
                                      <asp:TextBox ID="TextBoxFECHA_CREACION_FINAL" runat="server" cap_name="FECHA_CREACION" CssClass="event_auto_source_expe" Style="width: 95%" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                      <asp:CalendarExtender ID="TextBoxFECHA_CREACION_FINAL_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_CREACION_FINAL_CalendarExtender" TargetControlID="TextBoxFECHA_CREACION_FINAL" DaysModeTitleFormat="yyyy-mm-dd" Format='yyyy-MM-dd' PopupButtonID="ImageButtonCreacionFin" />

                                  </div>
                                  <div class="col-2 p-0">
                                      <button class="ml-1 btn border-0" id="ImageButtonCreacionFin" type="button">
                                          <i class="fad fa-calendar-alt fa-1x"></i>
                                      </button>
                                  </div>

                              </div>
                              <div class="row mt-3">
                                  <span>Tema unidad documental</span>
                              </div>
                              <div class="row">
                                  <asp:TextBox ID="TextBoxTEMA_EXPEDIENTE_" runat="server" cap_name="TEMA_EXPEDIENTE"  CssClass="form-control event_auto_source_expe" Width="100%"></asp:TextBox>
                              </div>
                              <div class="row mt-3">
                                  <asp:CheckBox ID="CheckBox_Asunto_" runat="server" CssClass="custom-checkbox mr-1" />
                                  <span>Asunto unidad documental</span>
                              </div>
                              <div class="row">
                                  <asp:TextBox ID="TextBoxASUNTO_EXPEDIENTE_" runat="server" cap_name="ASUNTO_EXPEDIENTE" CssClass="form-control event_auto_source_expe" Width="100%"></asp:TextBox>
                              </div>
                              <div class="row mt-3">
                                  <asp:CheckBox ID="CheckBox_observacion" runat="server" CssClass="custom-checkbox mr-1" />
                                  <span>Observación unidad documental</span>
                              </div>
                              <div class="row">
                                  <asp:TextBox ID="TextBoxOBSERVACION_EXPEDIENTE_" runat="server" Width="100%" cap_name="ASUNTO_EXPEDIENTE" CssClass="form-control event_auto_source_expe"></asp:TextBox>
                              </div>
                              <div class="row mt-3">
                                  <span>Nombre Solicitante</span>
                                  <input type="image" id="image_ayuda_solicitante_" src="../workflow/imageneswf/ayuda.png" class="def" style="height: 20px" onclick="ayuda_compartir('image_ayuda_solicitante', 'Panelcampos');">
                              </div>
                              <div class="row">
                                  <asp:TextBox ID="TextBoxNOMBRE_PERSONA_EXPEDIENTE_" runat="server" Width="100%" cap_name="NOMBRE_PERSONA_EXPEDIENTE" CssClass="form-control event_auto_source_expe"></asp:TextBox>
                              </div>
                              <div class="row mt-3">
                                  <span>Identificación del solicitante</span>
                              </div>
                              <div class="row">
                                  <asp:TextBox ID="TextBoxIDENTIFICACION_PERSONA_EXPEDIENTE_" runat="server" Width="100%" cap_name="IDENTIFICACION_PERSONA_EXPEDIENTE" CssClass="form-control event_auto_source_expe"></asp:TextBox>
                              </div>
                              <div class="row mt-3">
                                  <span>Nombre Responsable</span>
                                  <input type="image" id="image_ayuda_responsable_" src="../workflow/imageneswf/ayuda.png" style="height: 20px" class="def" onclick="ayuda_compartir('image_ayuda_responsable', 'Panelcampos');">
                              </div>
                              <div class="row">
                                  <asp:TextBox ID="TextBoxNOMBRE_RESPONSABLE_EXPEDIENTE_" runat="server" cap_name="NOMBRE_RESPONSABLE_EXPEDIENTE" Width="100%" CssClass="form-control event_auto_source_expe"></asp:TextBox>
                              </div>
                              <div class="row mt-3">
                                  <span>Nit/Identificación responsable</span>
                              </div>
                              <div class="row">
                                  <asp:TextBox ID="TextBoxIDENFICACION_RESPONSABLE_EXPEDIENTE_" runat="server" Width="100%" cap_name="IDENFICACION_RESPONSABLE_EXPEDIENTE" CssClass="form-control event_auto_source_expe"></asp:TextBox>
                              </div>
                              <div class="row mt-3">
                                  <span>Fondo documental</span>
                              </div>
                              <div class="row">
                                  <asp:DropDownList ID="DropDownListNOMBRE_FONDO_" Width="100%" runat="server" EnableViewState="true" CssClass="custom-select"></asp:DropDownList>
                              </div>
                              <div class="row mt-3">
                                  <span>Ciclo expediente archivo(*)</span>
                              </div>
                              <div class="row">
                                  <asp:DropDownList ID="DropDownListNOMBRE_CICLO_ARCHIVO_" Width="100%" runat="server" EnableViewState="true" CssClass="custom-select"></asp:DropDownList>
                              </div>
                              <div class="row mt-3">
                                  <span>Nombre área gestión(sección)</span>
                              </div>
                              <div class="row">
                                  <asp:TextBox ID="TextBoxNOMBRE_AREA_TRD_" runat="server" Width="100%" cap_name="NOMBRE_AREA_TRD"  CssClass="form-control event_auto_source_expe"></asp:TextBox>
                              </div>
                              <div class="row mt-3">
                                  <span>Nombre sub área gestión(sub sección)</span>
                              </div>
                              <div class="row">
                                  <asp:TextBox ID="TextBoxNOMBRE_SUB_AREA_" runat="server" Width="100%" cap_name="NOMBRE_SUB_AREA" CssClass="form-control event_auto_source_expe"></asp:TextBox>
                              </div>
                              <div class="row mt-3">
                                  <span>Nombre serie documental</span>
                              </div>
                              <div class="row">
                                  <asp:TextBox ID="TextBoxNOMBRE_SERIE_TRD_" runat="server" Width="100%" cap_name="NOMBRE_SERIE_TRD" CssClass="form-control event_auto_source_expe"></asp:TextBox>
                              </div>
                              <div class="row mt-3">
                                  <span>Nombre sub serie documental</span>
                              </div>
                              <div class="row">
                                  <asp:TextBox ID="TextBoxNOMBRE_SUBSERIE_TRD_" runat="server" Width="100%" cap_name="NOMBRE_SUBSERIE_TRD" CssClass="form-control event_auto_source_expe"></asp:TextBox>
                              </div>
                              <div class="row mt-3">
                                  <span>Rangos fecha inicial (Expedición)</span>
                              </div>
                              <div class="row p-0">
                                  <div class="col-4 p-0">
                                      <asp:TextBox ID="TextBoxFECHA_EXTREMA_INICIAL_INICIAL_" runat="server" cap_name="FECHA_EXTREMA_INICIAL" CssClass="event_auto_source_expe" Style="width: 95%" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                      <asp:CalendarExtender ID="TextBoxFECHA_EXTREMA_INICIAL_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_EXTREMA_INICIAL__CalendarExtender" TargetControlID="TextBoxFECHA_EXTREMA_INICIAL_INICIAL_" Format='yyyy-MM-dd' PopupButtonID="ImageButtonfechaextremaini_" />
                                  </div>
                                  <div class="col-2 p-0">
                                      <button class="ml-1 btn border-0" id="ImageButtonfechaextremaini_" type="button">
                                          <i class="fad fa-calendar-alt fa-1x"></i>
                                      </button>
                                  </div>
                                  <div class="col-4 p-0">
                                      <asp:TextBox ID="TextBoxFECHA_EXTREMA_INICIAL_FINAL_" runat="server" cap_name="FECHA_EXTREMA_INICIAL" CssClass="event_auto_source_expe" Style="width: 95%" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                      <asp:CalendarExtender ID="TextBoxFECHA_EXTREMA_FINAL_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_EXTREMA_FINAL__CalendarExtender" TargetControlID="TextBoxFECHA_EXTREMA_INICIAL_FINAL_" Format='yyyy-MM-dd' PopupButtonID="ImageButtonfechaextremafin_" />
                                  </div>
                                  <div class="col-2 p-0">
                                      <button class="ml-1 btn border-0" id="ImageButtonfechaextremafin_" type="button">
                                          <i class="fad fa-calendar-alt fa-1x"></i>
                                      </button>
                                  </div>
                              </div>


                              <div class="row mt-3">
                                  <span>Rangos fecha final (Terminación)</span>
                              </div>
                              <div class="row p-0">
                                  <div class="col-4 p-0">
                                      <asp:TextBox ID="TextBoxFECHA_EXTREMA_FINAL_INICIAL_" runat="server" cap_name="FECHA_EXTREMA_FINAL" CssClass="event_auto_source_expe" Style="width: 95%" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                      <asp:CalendarExtender ID="TextBoxFECHA_FINAL_INICIAL_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_FINAL_INICIAL__CalendarExtender" TargetControlID="TextBoxFECHA_EXTREMA_FINAL_INICIAL_" Format='yyyy-MM-dd' PopupButtonID="ImageButton_FINAL_INICIAL_" />
                                  </div>
                                  <div class="col-2 p-0">
                                      <button class="ml-1 btn border-0" id="ImageButton_FINAL_INICIAL_" type="button">
                                          <i class="fad fa-calendar-alt fa-1x"></i>
                                      </button>
                                  </div>
                                  <div class="col-4 p-0">
                                      <asp:TextBox ID="TextBoxFECHA_EXTREMA_FINAL_FINAL_" runat="server" cap_name="FECHA_EXTREMA_FINAL" CssClass="event_auto_source_expe" Style="width: 95%" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                      <asp:CalendarExtender ID="TextBoxFECHA_EXTREMA_FINAL_FINAL_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_EXTREMA_FINAL_FINAL__CalendarExtender" TargetControlID="TextBoxFECHA_EXTREMA_FINAL_FINAL_" Format='yyyy-MM-dd' PopupButtonID="ImageButtonfechaextremafinfin_" />
                                  </div>
                                  <div class="col-2 p-0">
                                      <button class="ml-1 btn border-0" id="ImageButtonfechaextremafinfin_" type="button">
                                          <i class="fad fa-calendar-alt fa-1x"></i>
                                      </button>
                                  </div>
                              </div>

                              <div class="row mt-3">
                                  <span>Rangos extremos iniciales</span>
                              </div>
                              <div class="row p-0">
                                  <div class="col-5 p-0">
                                      <asp:TextBox ID="TextBoxRANGO_EXTREMO_INICIAL_INICIAL_" runat="server" cap_name="RANGO_EXTREMO_INICIAL" CssClass="event_auto_source_expe" Style="width: 95%" onkeypress="return validate_numero(event,this)"></asp:TextBox>
                                  </div>
                                  <div class="col-2 p-0">
                                      <span>Hasta</span>
                                  </div>
                                  <div class="col-5 p-0">
                                      <asp:TextBox ID="TextBoxRANGO_EXTREMO_INICIAL_FINAL_" runat="server" cap_name="RANGO_EXTREMO_INICIAL" CssClass="event_auto_source_expe" Style="width: 95%" onkeypress="return validate_numero(event,this)"></asp:TextBox>
                                  </div>

                              </div>

                              <div class="row mt-3">
                                  <span>Rangos extremos finales</span>
                              </div>
                              <div class="row p-0">
                                  <div class="col-5 p-0">
                                      <asp:TextBox ID="TextBoxRANGO_EXTREMO_FINAL_INICIAL_" runat="server" cap_name="RANGO_EXTREMO_FINAL" CssClass="event_auto_source_expe" Style="width: 95%" onkeypress="return validate_numero(event,this)"></asp:TextBox>
                                  </div>
                                  <div class="col-2 p-0">
                                      <span>Hasta</span>
                                  </div>
                                  <div class="col-5 p-0">
                                      <asp:TextBox ID="TextBoxRANGO_EXTREMO_FINAL_FINAL_" runat="server" cap_name="RANGO_EXTREMO_FINAL" CssClass="event_auto_source_expe" Style="width: 95%" onkeypress="return validate_numero(event,this)"></asp:TextBox>
                                  </div>

                              </div>

                              <div class="row mt-3">
                                  <span>Estado Archivo</span>
                              </div>
                              <div class="row">
                                  <asp:DropDownList ID="DropDownListEstado_Expediente_" runat="server" CssClass="custom-select" Width="100%">
                                      <asp:ListItem>Todas</asp:ListItem>
                                      <asp:ListItem>Archivados</asp:ListItem>
                                      <asp:ListItem>Sin archivar</asp:ListItem>
                                  </asp:DropDownList>
                              </div>

                              <div class="row mt-3">
                                  <span>Creador por el usuario</span>
                              </div>
                              <div class="row">
                                  <asp:DropDownList ID="DropDownListUusuariocreador_" runat="server" CssClass="custom-select" Width="100%"></asp:DropDownList>
                              </div>
                              <div class="row">
                                  <asp:Button ID="ButtonAgregar" runat="server" Text="A" ToolTip="Agregar usuario" CssClass="btn btn-success mr-1 mt-1" />
                                  <asp:Button ID="ButtonLimpiar" runat="server" Text="L" ToolTip="Limpiar todos los usuario" CssClass="btn btn-success mr-1 mt-1" />
                                  <asp:Button ID="ButtonRemover" runat="server" Text="R" ToolTip="Quitar usuario expecifico" CssClass="btn btn-success mr-1 mt-1" />
                              </div>

                              <div class="row mt-3">
                                  <span>Clase unidad documental</span>
                              </div>

                              <div class="row">
                                  <asp:DropDownList ID="DropDownListtipoexpediente_" runat="server" Width="100%" CssClass="custom-select"></asp:DropDownList>
                              </div>

                              <div class="row mt-3">
                                  <span>Estado unidad documental</span>
                              </div>

                              <div class="row">
                                  <asp:DropDownList ID="DropDownListEstadoExpedienteSierre_" runat="server" Width="100%" CssClass="custom-select">
                                      <asp:ListItem>Todas</asp:ListItem>
                                      <asp:ListItem>Cerrado</asp:ListItem>
                                      <asp:ListItem>Abierto</asp:ListItem>
                                  </asp:DropDownList>
                              </div>
                              <div class="row mt-3">
                                  <span>Tipo unidad documental</span>
                              </div>
                              <div class="row">
                                  <asp:DropDownList ID="DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL_" Width="100%" runat="server" CssClass="custom-select"></asp:DropDownList>
                              </div>

                          </asp:Panel>

                      </ContentTemplate>
                  </asp:UpdatePanel>
                  <asp:UpdatePanel ID="UpdatePanel_botones_val_radicacion" runat="server" UpdateMode="Conditional">
                      <ContentTemplate>
                          <asp:Panel ID="Panelbuton" runat="server" ScrollBars="None"
                              Width="99%" Style="background-color: white" CssClass="p-3 align-content-end">
                              <asp:Button ID="ButtonConsulta" runat="server" Text="Consulta" ToolTip="Consulta" CssClass="btn btn-success" />
                              <asp:Button ID="ButtonRestaurar" runat="server" Text="Restaurar" ToolTip="Restaurar" CssClass="btn btn-success" />
                              <asp:Button ID="ButtonConsultaLike" runat="server" Text="" Style="display: none" />
                          </asp:Panel>
                      </ContentTemplate>
                  </asp:UpdatePanel>
              </nav>
          </div>
          <div id="ocultaleft"
              style="position: inherit; width: 0.5%; float: left; height: 5%; background-color: #053061; margin-left: 5px; margin-right: 2px; margin-top: 2px; float: left; cursor: pointer; display:none">
          </div>
          <div id="Contenedorderecho" class=" mr-0 ml-0 pl-1 pr-1 pb-0 pt-0  active_content_rigth" style="width:75%; float:right">
              <div class="nav-person-da" id="item_nav_tab">
                  <ul class="nav nav-tabs mt-2" id="myTab" role="tablist">
                      <li class="nav-item">
                          <a class="nav-link nav-link-person " id="home-expediente" data-toggle="tab" href="#home_expediente" role="tab" aria-controls="home_radic" aria-selected="true"><i style="color: #0062cc" id="home-radicadori" class="fad fa-folders "></i> Expedientes</a>
                      </li>
                      <li class="nav-item">
                          <a class="nav-link nav-link-person " id="home_documentos_expediente" data-toggle="tab" href="#documentos_expediente" role="tab" aria-controls="profile" aria-selected="false"><i style="color: #0062cc" id="soporte-envio_navi" class="fad fa-copy"></i> Documentos </a>
                      </li>

                  </ul>
              </div>
              <div class="tab-content" id="item_tab_content">
                  <div class="tab-pane  p-2" id="home_expediente" role="tabpanel" aria-labelledby="home-tab">
                      <div id="Contenedorgrid" style="width: 100%; position: inherit; left: auto; float: right; height: 100%; min-height: 300px">
                          <asp:UpdatePanel ID="UpdatePanel_general_titulo" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <div id="contenido_titulo_val_radicacion" style="width: 99%;" class="p-2">
                                      <asp:CheckBox ID="CheckBoxsolo_expeidente_propio" runat="server" Text="Solo ver mis expedientes" Font-Size="9pt" Style="font-family: Arial; float: right" Checked="false" ForeColor="Red" CssClass="custom-checkbox font-weight-light" />
                                      <asp:Label ID="Label_estado" runat="server" ForeColor="Black" Font-Size="9px" Style="float: right" CssClass="font-weight-light  h6"></asp:Label>
                                      <asp:Label ID="titulo_label_expedientes" runat="server" CssClass="font-weight-light p">Resultados busqueda</asp:Label>
                                  </div>
                              </ContentTemplate>
                          </asp:UpdatePanel>
                          <asp:Panel ID="Panelactividad" runat="server" Wrap="False"
                              Height="95%" Width="99%" Style="overflow: auto">
                              <asp:UpdatePanel ID="UpdateGeneral" runat="server" UpdateMode="Conditional">
                                  <ContentTemplate>
                                      <asp:GridView ID="data_grid" runat="server" Style="position: inherit" GridLines="None" EnableViewState="true" AutoGenerateColumns="true" ShowHeaderWhenEmpty="true"
                                          AutoGenerateSelectButton="False" CssClass="table font-weight-light" AllowPaging="true" PageSize="4" pag_util="4" PagerSettings-Position="Top"
                                          AllowSorting="True">
                                          <RowStyle VerticalAlign="Middle" />
                                          <FooterStyle BackColor="White" ForeColor="#E7EDF5" />
                                          <PagerSettings />
                                          <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                                          <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                          <RowStyle CssClass="GridviewScrollItem_line_cort" />
                                          <PagerStyle CssClass="pagination-ys" />
                                          <Columns>
                                              <asp:BoundField HeaderText="OPCIONES    " />

                                          </Columns>
                                      </asp:GridView>
                                      <input id="hdnEmailID" type="hidden" value="0" runat="server" />
                                      <input id="hdnEmailID_VAL" type="hidden" value="0" runat="server" />
                                      <input id="HiddenEmailconsulta" type="hidden" value="" runat="server" />
                                      <input id="Hidden_0001" type="hidden" value="0" runat="server" />
                                  </ContentTemplate>

                                  <Triggers>
                                  </Triggers>
                              </asp:UpdatePanel>
                          </asp:Panel>



                      </div>
                      <div id="botones_accion_postback" style="display: none">
                          <asp:UpdatePanel ID="UpdatePanel_acciones_exp" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <asp:Button ID="Button_actualiza_expdientes_agregados" runat="server" />
                              </ContentTemplate>
                          </asp:UpdatePanel>

                      </div>
                      <asp:HiddenField ID="HiddenField_botones_respuesta" runat="server" Value="-1" />
                      <div id="contenido_botonoes" style="width: 99.8%; position: inherit; left: auto; float: right; height: 10%; background-color: white; overflow: auto; border: 1px solid #ccc; display: none">
                          <asp:UpdatePanel ID="UpdatePanel_botones_opcion" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <asp:Button ID="Button_asigna_expediente_gestion" runat="server" Text="Asignar " Style="margin-top: 2px; margin-left: 5px; font-family: Arial; height: 20px; font-size: 12px; background-color: #7098DD;" ToolTip="Asigna unidad documental" OnClientClick="importa_dato_expediente()" />
                                  <asp:Button ID="Button_nuevo_expediente_gestion" runat="server" Text="Nuevo " Style="margin-top: 2px; margin-left: 5px; font-family: Arial; height: 20px; font-size: 12px" ToolTip="Agregar nueva unidad documental" OnClientClick="tamano_ventana_agregar_expediente()" CssClass="boton_azul_normal" />
                                  <asp:Button ID="Button_Editar_expediente_gestion" runat="server" Text="Editar " Style="margin-top: 2px; margin-left: 5px; font-family: Arial; height: 20px; font-size: 12px" ToolTip="Editar unidad documental" OnClientClick="tamano_ventana_editar_expediente()" CssClass="boton_azul_normal" />
                                  <asp:Button ID="Button_eliminar_expediente_gestion" runat="server" Text="Eliminar " Style="margin-top: 2px; margin-left: 5px; font-family: Arial; height: 20px; font-size: 12px" ToolTip="Eliminar unidad documental" OnClientClick="ConfirmMensajeGeneral('Desea eliminar la unidad','HiddenPROMP')" CssClass="boton_azul_normal" />
                                  <asp:Button ID="Button_archivar_expediente_gestion" runat="server" Text="Archivar " Style="margin-top: 2px; margin-left: 5px; font-family: Arial; height: 20px; font-size: 12px" ToolTip="Archivar unidad documental" CssClass="boton_azul_normal" />
                                  <asp:Button ID="Button_desachivar_expediente_gestion" runat="server" Text="Desarrchivar " Style="margin-top: 2px; margin-left: 5px; font-family: Arial; height: 20px; font-size: 12px" ToolTip="Desarrchivar unidad documental" OnClientClick="pront_confirmacion('Desea desachivar ?');" CssClass="boton_azul_normal" />
                                  <asp:Button ID="Button_ubicacio_expediente_gestion" runat="server" Text="Ubicación " Style="margin-top: 2px; margin-left: 5px; font-family: Arial; height: 20px; font-size: 12px" ToolTip="Muestra Ubicación unidad documental" CssClass="boton_azul_normal" />
                                  <asp:Button ID="ButtonRotulo" runat="server" Text="Descarga" Style="font-family: Arial; height: 20px; font-size: 12px" ToolTip="Descarga rotulo unidad documental" CssClass="boton_azul_normal" />
                                  <asp:Button ID="Button_rotulo_expediente_gestion" runat="server" Text="Imprimir" Style="margin-top: 2px; margin-left: 5px; font-family: Arial; font-size: 12px; height: 20px" ToolTip="Imprimir rotulo unidad documental" CssClass="boton_azul_normal" />
                                  <asp:Button ID="Button_estado_expediente_gestion" runat="server" Text="Estado " Style="margin-top: 2px; margin-left: 5px; font-family: Arial; font-size: 12px" ToolTip="Cambia estado unidad documental" Height="20px" CssClass="boton_azul_normal" />
                                  <asp:Button ID="Button_volumen_expediente_gestion" runat="server" Text="Volumen " Style="margin-top: 2px; margin-left: 5px; font-family: Arial; width: 70px; height: 20px; font-size: 12px" ToolTip="Agregar nuevo volumen a la unidad documental seleccionada" OnClientClick="tamano_ventana_nuevo_volumen_expediente()" CssClass="boton_azul_normal" />
                                  <asp:Button ID="Button_documentos_relacionado" runat="server" Text="Documentos " Style="margin-top: 2px; margin-left: 5px; font-family: Arial; height: 20px; width: 100px; font-size: 12px" ToolTip="Ver documentos relacionados" CssClass="boton_azul_normal" />
                                  <asp:Button ID="Button_configura_rotulo" runat="server" Text="Configura rotulo " Style="margin-top: 2px; margin-left: 5px; font-family: Arial; height: 20px; width: 120px; font-size: 12px" ToolTip="Selecciona el rotulo de impresión" CssClass="boton_azul_normal" />
                                  <asp:Button ID="Button_listar_volumenes_relacionados" runat="server" Text="Volumenes relacionados " Style="margin-top: 2px; margin-left: 5px; font-family: Arial; height: 20px; width: 160px; font-size: 12px" ToolTip="Listar volumnes relacionados" CssClass="boton_azul_normal" />
                                  <asp:Button ID="Button_activa_ventana_rel_volumen" runat="server" Text="Relacionar volumen " Style="margin-top: 2px; margin-left: 5px; font-family: Arial; height: 20px; width: 160px; font-size: 12px" ToolTip="Listar volumnes relacionados" CssClass="boton_azul_normal" />
                                  <asp:HiddenField ID="HiddenField_estado_ubicacion" runat="server" Value="" />
                                  <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server"/>
                                  <input id="Hidden_result" type="hidden" value="NO" runat="server"/>
                              </ContentTemplate>
                          </asp:UpdatePanel>
                      </div>
                  </div>
                  <div class="tab-pane  p-2" id="documentos_expediente" role="tabpanel" aria-labelledby="profile-tab">
                      <div id="div_contenedor_titulo_documentos_relacionados_exp" style="width: 100%" class="p-2 row">                       
                          <div id="div_expediente_seleccionado_exp" style="" class="p-2 col-7">
                              <asp:UpdatePanel ID="UpdatePanel_expediente_seleccionado_exp" runat="server" UpdateMode="Conditional">
                                  <ContentTemplate>
                                      <asp:Label ID="Label_expediente_seleccionado_exp" runat="server" Text="Unidad relacionada" CssClass="h6"></asp:Label>
                                  </ContentTemplate>
                              </asp:UpdatePanel>
                          </div>
                          <div class="input-group  col-5">
                              <button id="Button_retaura_relacionados" class="btn btn-outline-secondary border-right-2 " title="Restaura expedientes relacionados" style="border-top-right-radius: 0px; border-bottom-right-radius: 0px" onclick="restore_lista_documentos_relacionados(event,this)" type="button">
                                  <i class="fal fa-long-arrow-left"></i>
                              </button>
                              <asp:TextBox ID="TextBox_busqueda_documento" runat="server" class="form-control form-control-sm complex  border-left-0" placeholder="Buscar documentos...." onkeypress="acti_busq_general_documento(event,this)"></asp:TextBox>
                              <div class="input-group-append">
                                  <button class="btn btn-outline-secondary" onclick="activa_boton_client_documento(event, this)" type="button">
                                      <i class="fal fa-search"></i>
                                  </button>
                              </div>
                          </div>
                      </div>           
                      <div id="Contenedorgrid_documentos_exp" style="width: 100%; min-height: 300px">
                          <asp:Panel ID="Panelactividad_documentos_exp" runat="server" Wrap="False"
                              Style="overflow: auto" Height="95%" Width="99%">
                              <asp:UpdatePanel ID="UpdateGeneral_documentos_exp" runat="server" UpdateMode="Conditional">
                                  <ContentTemplate>
                                      <asp:GridView ID="data_grid_documentos_exp" runat="server" AllowSorting="true" AllowPaging="true" AutoGenerateColumns="true" ShowHeaderWhenEmpty="true"
                                          PageSize="4" PagerSettings-Position="Top" GridLines="None" Style="width: 100%"
                                          AutoGenerateSelectButton="False" CssClass="table font-weight-light">
                                          <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                          <RowStyle CssClass="GridviewScrollItem_line_cort" />
                                          <PagerStyle CssClass="pagination-ys" />
                                          <Columns>
                                              <asp:BoundField HeaderText="OPCIONES   " />
                                              
                                          </Columns>
                                          <EmptyDataTemplate>
                                              <div align="center">No records found.</div>
                                          </EmptyDataTemplate>
                                      </asp:GridView>
                                      <input id="hdnEmailID_documentos" type="hidden" value="0" runat="server" />
                                      <input id="hdnEmailID_VAL_documentos" type="hidden" value="0" runat="server" />
                                      <input id="Hidden_gabienete" type="hidden" value="0" runat="server" />
                                      <input id="HiddenEmailconsulta_documentos" type="hidden" value="" runat="server" />
                                  </ContentTemplate>
                                  <Triggers>
                                  </Triggers>
                              </asp:UpdatePanel>
                          </asp:Panel>
                          <div id="contenido_titulo_val_radicacion_documentos_exp" style="height: auto; width: 100%" class="p-0">
                              <asp:UpdatePanel ID="UpdatePanel_documentos_exp_title" runat="server" UpdateMode="Conditional">
                                  <ContentTemplate>
                                      <asp:Label ID="titulo_label_expedientes_documentos" runat="server" Style="float: left" CssClass="h6 font-weight-light">Resultados busqueda</asp:Label>
                                  </ContentTemplate>      
                              </asp:UpdatePanel>
                          </div>
                      </div>  
                       <div style="display: none">
                          <asp:UpdatePanel ID="UpdatePanel_boton_documento" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <asp:Button ID="Button_ver_documento" runat="server" />
                                  <asp:Button ID="Button_busqueda_documento" runat="server" />
                              </ContentTemplate>
                          </asp:UpdatePanel>
                      </div>
                      <div id="Div3" style="display: none">
                          <asp:UpdatePanel ID="UpdatePanel_boton_Event" runat="server" UpdateMode="Conditional">
                              <ContentTemplate>
                                  <asp:Button ID="Button_ver_documentos_relacionados" runat="server" />
                                  <asp:Button ID="Button_busqueda_expediente" runat="server" />
                              </ContentTemplate>
                          </asp:UpdatePanel>

                      </div>
                  </div>
              </div>
          </div>
      </div>
    </div>
     
         <!--ubicacion toponimica-->
        <div id="modal_ubicacion_toponimica_expediente">
            <asp:Panel ID="Panel_ubicacion_toponimica_expediente_popup" runat="server" Style="display:none;  width: 50%; height: 99%">
                <asp:ModalPopupExtender ID="ModalPopupExtende_ubicacion_toponimica_expediente_popup" runat="Server" 
                    BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_ubicacion_toponimica_expediente_popup"
                    PopupControlID="Panel_ubicacion_toponimica_expediente_popup" CancelControlID="Buttoncerrar_ubicacion_toponimica_expediente_popup" Y="0">

                </asp:ModalPopupExtender>
                <div id="modal_content_ubicacion_toponimica_expediente_popup" class="modal-content">
                    <div id="divcabecer_ubicacion_toponimica_expediente_popup" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Ubicación topografica</h6>
                        <button type="button" value="Buttoncerrar_ubicacion_toponimica_expediente_popup" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="Contenido_ubicacion_toponimica_expediente" style="height: 97%; width: 100%; border-top: none; overflow: auto" class="modal_content_back pl-1 pr-1">
                        <div id="div_treview_archivo_u_b_t" style="height: 100%">
                            <asp:Panel ID="Paneltreview_u_b_t" runat="server" ScrollBars="Both"
                                Height="100%" Width="100%" Style="position: inherit">
                                <asp:UpdatePanel ID="UpdatePanelViewArchivo_u_b_t" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:TreeView ID="TreeViewArchivo_u_b_t" Style="text-align: left; padding-left: 1px; font-size: 10px; margin-top: 0px" runat="server" CssClass="TreeN" NodeWrap="true"
                                            PopulateNodesFromClient="False" EnableViewState="true"
                                            LeafNodeStyle-CssClass="LeafNodeStyle" Font-Size="11px" NodeIndent="10" ExpandDepth="0" SkipLinkText="">
                                            <HoverNodeStyle Font-Underline="False" />
                                            <LeafNodeStyle CssClass="LeafNodeStyle" HorizontalPadding="10px" NodeSpacing="0px" VerticalPadding="5px" />
                                            <NodeStyle ChildNodesPadding="5px" HorizontalPadding="0px" NodeSpacing="5px" VerticalPadding="5px" ForeColor="Black" />
                                            <ParentNodeStyle ChildNodesPadding="0px" ForeColor="#313131" Font-Bold="true" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="5px" />
                                            <RootNodeStyle ChildNodesPadding="0px" ForeColor="#313131" Font-Bold="true" NodeSpacing="0px" VerticalPadding="5px" HorizontalPadding="5px" />
                                            <SelectedNodeStyle ForeColor="White" CssClass="node_select_" Font-Size="10px" ImageUrl="../workflow/imageneswf/iten_list_select.png" />
                                        </asp:TreeView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>

                            </asp:Panel>
                        </div>

                    </div>
                    <div id="contendor_botones_unidad_u_b_t" class="border_inferior_radius_blanco modal-footer">
                        <asp:UpdatePanel ID="UpdatePanel_botones_unidad_u_b_t" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_exportar" runat="server" Text="Exportar" CssClass="btn btn-success" OnClientClick="fnExcelTre('TreeViewArchivo_u_b_t')" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_ubicacion_toponimica_expediente_popup" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                        <asp:Button ID="ButtonSalir_ubicacion_toponimica_expediente_popup" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                        <asp:Button ID="Buttoncerrar_ubicacion_toponimica_expediente_popup" runat="Server" Text="" CssClass="invisible" />
                    </div>
                </div>
            </asp:Panel>
        </div>        
        <div id="modal_agregar_expediente">
            <asp:Panel ID="Panel_agregar_expdiente_popup" runat="server" Style="display: none; width: 50%; height: 98%; overflow: hidden; margin: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtende_agregar_expdiente_popup" runat="Server" BackgroundCssClass="FondoAplicacion" Y="1" TargetControlID="ButtonSalir_agregar_expdiente_popup"
                    PopupControlID="Panel_agregar_expdiente_popup" CancelControlID="Buttoncerrar_agregar_expdiente_popup">
                </asp:ModalPopupExtender>
                <div id="divcabecer_agregar_expdiente_popup" class="modal_title_superior" style="width: 99%; display: none">

                    <asp:Label ID="Label_agregar_expdiente_popup" runat="server" Text="Gestión expedientes" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton_agregar_expdiente_popup" style="float: right">
                        <asp:Button ID="Buttoncerrar_agregar_expdiente_popup" runat="Server" Text="X" CssClass="modal_boton_hiden"
                            ToolTip="Cerrar ventana" OnClientClick="cerrar_popup_agregar_expedinte()" />

                    </div>
                </div>
                <div id="Contenido_agregar_expdiente_popup" style="background-color: transparent; height: auto; width: auto; overflow: hidden" class="modal_content_back">
                    <asp:UpdatePanel ID="UpdatePanel_agregar_expdiente_popup" runat="server" UpdateMode="Conditional" style="height: 100%" RenderMode="Inline">
                        <ContentTemplate>
                            <iframe id="Iframe_agregar_expdiente_popup_" runat="server" style="width: 100%; height: 100%; overflow: hidden" scrolling="no" frameborder="0"></iframe>
                            <input id="Hidden_estado_editar" type="hidden" value="YES" runat="server"/>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <asp:Button ID="Button_agregar_expdiente_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" Style="display: none" />
                <asp:Button ID="ButtonSalir_agregar_expdiente_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" Style="display: none" />
            </asp:Panel>
            
        </div>
        <div id="modal_agregar_expediente_trabajo">
            <asp:Panel ID="Panel_agregar_expdiente_popup_trabajo" runat="server"  Style="display:none; color: White; width: 50%; height: 100%; overflow:hidden; margin:auto; background-color:white" >          
                <asp:ModalPopupExtender ID="ModalPopupExtende_agregar_expdiente_popup_trabajo" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_agregar_expdiente_popup_trabajo"
                    PopupControlID="Panel_agregar_expdiente_popup_trabajo" CancelControlID="Buttoncerrar_agregar_expdiente_popup_trabajo" >
                </asp:ModalPopupExtender>
                <div id="divcabecer_agregar_expdiente_popup_trabajo" class="cabecera2" style="width:99%; display:none">           
                    <asp:Label ID="Label_agregar_expdiente_popup_trabajo" runat="server" Text="Gestión expedientes" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton_agregar_expdiente_popup_trabajo" style="float: right">
                        <asp:Button ID="Buttoncerrar_agregar_expdiente_popup_trabajo" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" OnClientClick="cerrar_popup_agregar_expedinte()" />
                    </div>
                </div>  
                <div id="Contenido_agregar_expdiente_popup_trabajo" style=" height:100%; width:100%; overflow: hidden; background-color:white" >
                    <asp:UpdatePanel ID="UpdatePanel_agregar_expdiente_popup_trabajo" runat="server" UpdateMode="Conditional" >
                        <ContentTemplate>
                            <iframe id="Iframe_agregar_expdiente_popup_trabajo_" runat="server" style="width:100%; height: 100%; overflow: hidden" scrolling="no" frameborder="0"></iframe>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>  
                <asp:Button ID="Button_agregar_expdiente_popup_trabajo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                <asp:Button ID="ButtonSalir_agregar_expdiente_popup_trabajo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
            
        </div>
        
            <asp:Panel ID="Panel_cambia_estado_expediente_popup" runat="server" Style="display:none; width:40%; height:auto; overflow:auto" CssClass="modal_content_general">               
                 <asp:ModalPopupExtender ID="ModalPopupExtender_cambia_estado_expediente_popup" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_cambia_estado_expediente_popup"
                    PopupControlID="Panel_cambia_estado_expediente_popup" CancelControlID="Buttoncerrar_cambia_estado_expediente_popup"></asp:ModalPopupExtender>
                <div id="modal_content_cambia_estado_expediente_popup" class="modal-content">
                    <div id="divcabecer_cambia_estado_expediente_popup" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Cambia estado unidad documental</h6>
                         <button type="button" value="Buttoncerrar_cambia_estado_expediente_popup" class="close da_event_captive ">&times;</button>   
                    </div>
                    <div id="Contenido_cambia_estado_expediente_popup" style=" height: auto; width: auto; border-top:none; overflow:auto" class="modal_content_back pl-3 pr-3">
                        <asp:UpdatePanel ID="UpdatePanel_cambia_estado_expediente_popup" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusive_anexo_radicado" runat="server" TargetControlID="Check_ButtonAbierto"
                                    Key="radicado"></asp:MutuallyExclusiveCheckBoxExtender>
                                <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusive_relacionado_radicado" runat="server" TargetControlID="CheckBox_ButtonSerrado"
                                    Key="radicado"></asp:MutuallyExclusiveCheckBoxExtender>
                                <div class="row mt-4 pl-2">
                                    <asp:CheckBox ID="Check_ButtonAbierto" runat="server" Text="" Checked="false"   CssClass="h7 custom-checkbox mr-1" />
                                    <span>Unidad documental en estado abierto</span>
                                </div>
                                <div class="row mt-1 pl-2">
                                    <asp:CheckBox ID="CheckBox_ButtonSerrado" runat="server" Text="" Checked="false" CssClass="h7 custom-checkbox mr-1" />
                                    <span>Unidad documental en estado cerrado</span>
                                </div>
                                 <div class="row mt-4 pl-2">
                                 <span>Motivo del cambio</span>
                                 </div>
                                <div class="row mt-1 mb-3 pl-2">
                                    <asp:TextBox ID="TextBox_cambia_estado_exp_popup" TextMode="MultiLine" runat="server" Style="width: 98%" CssClass="form-control"></asp:TextBox>
                                </div>
                                
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="Button_estado_expediente_gestion" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                       
                    </div>
                     <div id="contedor_botones_cambia_estado_expediente_popup" class="modal-footer ">
                            <asp:UpdatePanel ID="updatepanel_botones_cambia_estado_expediente_popup" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Button ID="Button_actualiza_estado_expediente_popup" runat="server" Text="Cambiar" ToolTip="Cambia estado unidad documental"  CssClass="btn btn-success" OnClientClick="ConfirmMensajeGeneral('Desea cambiar el estado de la unidad documental?','HiddenPROMP')" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_cambia_estado_expediente_popup" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                        <asp:Button ID="ButtonSalir_cambia_estado_expediente_popup" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" Style="display: none" />
                        <asp:Button ID="Buttoncerrar_cambia_estado_expediente_popup" runat="Server" Text="X" CssClass="invisible" Height="1px" Width="1px" Style="display: none" />
                    </div>
                </div>
            </asp:Panel>
       
        <!--archiva expediente-->
        <div id="modal_reubicar_unidad_expediente">
            <asp:Panel ID="Panel_reubicar_unidad_expediente_popup" runat="server"  Style="display:none; width: 100%; height: 99%" CssClass="modal_content_general">             
                <asp:ModalPopupExtender ID="ModalPopupExtende_reubicar_unidad_expediente_popup" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_reubicar_unidad_expediente_popup"
                    PopupControlID="Panel_reubicar_unidad_expediente_popup" CancelControlID="Buttoncerrar_reubicar_unidad_expediente_popup" ></asp:ModalPopupExtender>
                <div id="modal_content_reubicar_unidad_expediente_popup" class="modal-content_">
                    <div id="divcabecer_reubicar_unidad_expediente_popup" class="modal_title_superior_ modal-header">
                         <h6 class="modal-title d-inline ml-1">Archivar unidad documental</h6>
                         <button type="button" value="Buttoncerrar_reubicar_unidad_expediente_popup" class="close da_event_captive">&times;</button>       
                    </div>
                    <div id="Contenido_reubicar_unidad_expediente_popup" style=" height: 97%; width: 100%; border-top:none" class="modal_content_back pl-1 pr-1 pt-1">
                        <div id="drowlist_r_u_e" style="">
                            <asp:UpdatePanel ID="UpdatePanelEntidad_r_u_e" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:DropDownList ID="DropDownListEntidadEmpresa_r_u_e" runat="server" Width="100%" CssClass="custom-select" onchange="buton_click('Button_listar_edificio');"></asp:DropDownList>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                         <div id="div_treview_archivo_r_u_e" style="height: 80%; margin-top: 5px">
                            <asp:Panel ID="Paneltreview_r_u_e" runat="server" ScrollBars="Both"
                                Height="100%" Width="100%" Style="position: inherit">
                                <asp:UpdatePanel ID="UpdatePanelViewArchivo_r_u_e" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:TreeView ID="TreeViewArchivo_r_u_e" Style="text-align: left; padding-left: 1px; font-size: 10px; margin-top: 0px" runat="server" CssClass="TreeN" NodeWrap="true"
                                            PopulateNodesFromClient="False" EnableViewState="true"
                                            LeafNodeStyle-CssClass="LeafNodeStyle" Font-Size="11px" NodeIndent="10" ExpandDepth="0" SkipLinkText="">
                                            <HoverNodeStyle Font-Underline="False" />
                                            <LeafNodeStyle CssClass="LeafNodeStyle" HorizontalPadding="10px" NodeSpacing="0px" VerticalPadding="5px" />
                                            <NodeStyle ChildNodesPadding="5px" HorizontalPadding="0px" NodeSpacing="5px" VerticalPadding="5px" ForeColor="Black" />
                                            <ParentNodeStyle ChildNodesPadding="0px" ForeColor="#313131" Font-Bold="true" HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="5px" />
                                            <RootNodeStyle ChildNodesPadding="0px" ForeColor="#313131" Font-Bold="true" NodeSpacing="0px" VerticalPadding="5px" HorizontalPadding="5px" />
                                            <SelectedNodeStyle ForeColor="White" CssClass="node_select_" Font-Size="10px" ImageUrl="../workflow/imageneswf/iten_list_select.png" />
                                        </asp:TreeView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>

                            </asp:Panel>
                        </div>         
                    </div>
                    <div id="contendor_botones_unidad_r_u_e" style="border-top:none" class="border_inferior_radius_blanco_ modal-footer">
                            <asp:UpdatePanel ID="UpdatePanel_botones_unidad_r_u_e" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Button ID="Button_actualizar_unidad" runat="server" Text="Editar" CssClass="btn btn-success" Style="display: none" />
                                    <asp:Button ID="Button_agrega_unidad_conservacion_interface" runat="server" Text="Archivar" ToolTip="Archivar unidad documental en unidad contenedora" CssClass="btn btn-success" Style=" display: none" />
                                    <asp:Button ID="Button_archivar" runat="server" Text="Archivar" CssClass="btn btn-success" Style="margin-left: 10px" />
                                    <asp:Button ID="Button_agrega_unidad_contenedora" runat="server" Text="Agregar" ToolTip="Agregar una unidad contenedora de unidad documental" CssClass="btn btn-success" Style=" margin-left: 10px" />
                                    <asp:Button ID="ButtonButtonEditar" runat="server" Text="Editar" CssClass="btn btn-success" ToolTip="Editar unidad contendora de unidad documental" />
                                    <asp:Button ID="ButtonEliminar_unidad_contendora" runat="server" Text="Eliminar" ToolTip="Eliminar unidad contendora de unidad documental" CssClass="btn btn-success" OnClientClick="ConfirmMensajeGeneral('Desea eliminar la unidad contenedora ?','Hidden_result_eliminar');" />
                                    <input id="Hidden_result_eliminar" type="hidden" value="0" runat="server"/>
                                    <input id="Hidden1" type="hidden" value="0" runat="server"/>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button_reubicar_unidad_expediente_popup" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    <asp:Button ID="ButtonSalir_reubicar_unidad_expediente_popup" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    <asp:Button ID="Buttoncerrar_reubicar_unidad_expediente_popup" runat="Server" Text="" CssClass="invisible" />
                </div>
            </asp:Panel>
            
        </div>
        <!--agregar unidad de conservacion-->
        <div id="modal_agregar_unidad_conservacion">  
                <asp:Panel ID="Panel_agregar_unidad_conservacion_popup" runat="server"  Style="display:none; color: White; width:50%; height: 99%; margin:auto" >                
                    <asp:ModalPopupExtender ID="ModalPopupExtende_agregar_unidad_conservacion_popup" runat="Server" 
                        BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_agregar_unidad_conservacion_popup"
                        PopupControlID="Panel_agregar_unidad_conservacion_popup" CancelControlID="Buttoncerrar_agregar_unidad_conservacion_popup" Y="1">
                    </asp:ModalPopupExtender>
                    <div id="divcabecer_agregar_unidad_conservacion_popup" class="modal_title_superior" style="display:none">                        
                        <asp:Label ID="Label_agregar_unidad_conservacion_popup" runat="server" Text="Gestión unidad contendora" Font-Size="10" Style="float: left; display:none">
                        </asp:Label>
                        <div id="Divcerrarbuton_agregar_unidad_conservacion_popup" style="float: right">
                            <asp:Button ID="Buttoncerrar_agregar_unidad_conservacion_popup" runat="Server" Text="X" Style="display: none"
                                ToolTip="Cerrar ventana" />
                        </div>
                    </div>  
                     <div id="Contenido_agregar_unidad_conservacion_popup" style="color: black; background-color: #FFFFFF; height: 100%; width: 100%" class="modal_content_general">
                         <asp:UpdatePanel ID="UpdatePanel_agregar_unidad_conservacion_popup" runat="server" UpdateMode="Conditional" style="height:100%" RenderMode="Inline">
                             <ContentTemplate>
                            <iframe  id="Iframe_agregar_unidad_conservacion_popup"  runat="server"  style="width:100%; height:100%;padding-top:5px" frameborder="0"></iframe>                
                                 <asp:HiddenField ID="Hidden_tipo_unidad_seleccion" runat="server" value="0"/>
                                 </ContentTemplate>
                             </asp:UpdatePanel>
                        </div>  
                        <asp:Button ID="Button_agregar_unidad_conservacion_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                        <asp:Button ID="ButtonSalir_agregar_unidad_conservacion_popup" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                </asp:Panel>
                
        </div>
        <!--detalle indice-->
	               <asp:Panel ID="Panel_indice" runat="server" Style="display:none; overflow:hidden; width:95%; height:100%" CssClass="modal_content_general" >
	                      <asp:ModalPopupExtender ID="ModalPopupExtender_indice" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_indice_dos"
	                          PopupControlID="Panel_indice"  CancelControlID="ButtonSalir_indice">
	                      </asp:ModalPopupExtender>
	                      <div id="Cabecerapendiente_indice" class="modal_title_superior" >                     
	                         <button style="margin-right:10px" type="button" onclick="activa_boton_client_server('ButtonSalir_indice');" class="close">&times;</button>          
	                      </div>
	                      <div id="Cotenedorpendiente_indice" style="height: 90%; width: 100%; overflow:hidden" class="modal_content_back">                     
	                          <asp:UpdatePanel ID="UpdatePanel_indice" runat="server" UpdateMode="Conditional">
	                              <ContentTemplate>
	                                  <iframe id="Iframe_indice_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
	                              </ContentTemplate>
	                          </asp:UpdatePanel>           
	                      </div>
                       <div style="display: none; height: 1px">
                           <asp:Button ID="ButtonSalir_indice" runat="Server" Text=""  CssClass="invisible bg-transparent" />
	                       <asp:Button ID="Button_indice_dos" CssClass="invisible bg-transparent" runat="server" Text="" Height="1px" Width="1px" />
                       </div>           
              </asp:Panel>
         <!--Popup visor externo-->
        <asp:Panel ID="Panel_visor_externo" runat="server" Style="display: none; overflow: hidden; left: 5px" ForeColor="White" Width="99%" Height="100% " CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtender_visor_externo" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_visor_externo"
                PopupControlID="Panel_visor_externo" CancelControlID="ButtonSalir_visor_externo">
            </asp:ModalPopupExtender>
            <div id="Cabecerapendiente_visor_externo" class="modal_title_superiorr_ modal-header">
                <h6 class="modal-title d-inline ml-1">Visor de documentos</h6>
                <button type="button" value="ButtonSalir_visor_externo" class="close da_event_captive ">&times;</button>
            </div>
            <div id="Cotenedorpendiente_visor_externo" style="height: 99%; width: 100%; overflow: hidden; border-top: none" class="modal_content_back">

                <asp:UpdatePanel ID="UpdatePanel_visor_externo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <iframe id="Iframe_visor_externo_clasficacion_" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: hidden"></iframe>
                    </ContentTemplate>

                </asp:UpdatePanel>
            </div>
           
            <div style="display: none; height: 1px">
                <asp:Button ID="Button_cerrar_emergente" runat="Server" Text="X" CssClass="modal_boton_hiden"
                    ToolTip="Cerrar ventana" />
                <asp:Button ID="Button_visor_externo" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" Style="display: none" />
                <asp:Button ID="ButtonSalir_visor_externo" runat="Server" Text="X" CssClass="modal_boton_hiden" Style="display: none"
                    ToolTip="Cerrar ventana" />
            </div>

        </asp:Panel>
           <!--Editar agregar expediente-->
        <asp:Panel ID="Panel_add_edit_expediente" runat="server" Style="display: none; width: 70%; height: 90%" CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_add_edit_expediente" runat="server"
                TargetControlID="ButtonSalir_add_edit_expediente" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_add_edit_expediente" PopupControlID="Panel_add_edit_expediente">
            </asp:ModalPopupExtender>
            <div id="modal_content_add_edit_expediente" class="modal-content">
                <div id="diver_cabcera_add_edit_expediente" class="modal_title_superior_ modal-header">
                    <h6 id="title_aad_edit_expediente" class="modal-title d-inline "></h6>
                    <button type="button" value="Button_cerrar_add_edit_expediente" class="close da_event_captive ">&times;</button>
                </div>
                <div id="contenido_campos" style="overflow: scroll" class="modal_content_back">
                    <asp:UpdatePanel ID="update_panel_controles" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <input id="Hidden_id_empresa" type="hidden" value="0" runat="server" />
                            <asp:Panel ID="panel_controles" runat="server" CssClass="p-2">
                                <asp:Table ID="table_controles" runat="server">

                                    <asp:TableRow>
                                        <asp:TableCell>
                                            <asp:CheckBox ID="CheckBoxActivaCodigomanual" runat="server" Text="" ForeColor="Red" style="font-family: Arial; font-size: 10pt" onchange="changue_manual_consecutivo();" />
                                            <span style="color: red" class="ml-1">Consecutivo unidad</span>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:TextBox ID="TextBoxCodigoManual" CssClass="event_auto_source_expe" cap_name="CODIGO_UNICO" MaxLength="70" runat="server" Style="width: 220px; font-family: Arial" ReadOnly="false" BackColor="white" ></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                  <span style=" color:red">Fecha inicial (Expedición) *</span> 
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:TextBox ID="TextBoxFECHA_EXTREMA_INICIAL" runat="server"  cap_name="FECHA_EXTREMA_INICIAL" MaxLength="10" CssClass="mt-1 event_auto_source_expe" Width="26%" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                            <asp:CalendarExtender ID="CalendarExtender1" runat="server" BehaviorID="TextBoxFECHA_EXTREMA_INICIAL_CalendarExtender" TargetControlID="TextBoxFECHA_EXTREMA_INICIAL" Format='yyyy-MM-dd' PopupButtonID="ImageButtonfechaextremaini" />
                                            <button class="ml-1 btn border-0" id="ImageButtonfechaextremaini" type="button">
                                                <i class="fad fa-calendar-alt fa-1x"></i>
                                            </button>

                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                  <span >Fecha final (Terminación)</span>         
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:TextBox ID="TextBoxFECHA_EXTREMA_FINAL" runat="server" MaxLength="10" cap_name="FECHA_EXTREMA_FINAL" Width="26%" CssClass="mt-1 event_auto_source_expe" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                                            <asp:CalendarExtender ID="CalendarExtender2" runat="server" BehaviorID="TextBoxFECHA_EXTREMA_FINAL_CalendarExtender" TargetControlID="TextBoxFECHA_EXTREMA_FINAL" Format='yyyy-MM-dd' PopupButtonID="ImageButtonfechaextremafin" />
                                            <button class="ml-1 btn border-0" id="ImageButtonfechaextremafin" type="button">
                                                <i class="fad fa-calendar-alt fa-1x"></i>
                                            </button>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                   <span>Rangos extremos</span>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:TextBox ID="TextBoxRANGO_EXTREMO_INICIAL" MaxLength="9" runat="server" Width="27%" cap_name="RANGO_EXTREMO_INICIAL" CssClass="mt-1 event_auto_source_expe" placeholder="rango inicial"></asp:TextBox>
                                            &nbsp;&nbsp;
                                  <asp:TextBox ID="TextBoxRANGO_EXTREMO_FINAL" runat="server" MaxLength="9" Width="27%" cap_name="RANGO_EXTREMO_FINAL" CssClass="mt-1 event_auto_source_expe" placeholder="rango final"></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                   <span>Tema expediente</span>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:TextBox ID="TextBoxTEMA_EXPEDIENTE" runat="server"  cap_name="TEMA_EXPEDIENTE" CssClass="mt-1 event_auto_source_expe" Width="98%"></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                  <span>Asunto expediente</span>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:TextBox ID="TextBoxASUNTO_EXPEDIENTE" runat="server" Width="98%" cap_name="ASUNTO_EXPEDIENTE" CssClass="mt-1 event_auto_source_expe" TextMode="MultiLine"></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>            
                                  <span>Observación expediente</span>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:TextBox ID="TextBoxOBSERVACION_EXPEDIENTE" runat="server" cap_name="OBSERVACION_EXPEDIENTE" Width="98%" CssClass="mt-1 event_auto_source_expe" TextMode="MultiLine"></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                   <span>Nombre Solicitante</span>
                                  <input type="image" id="image_ayuda_solicitante" src="../workflow/imageneswf/ayuda.png" style="height:20px" class="def" onclick="ayuda_compartir('image_ayuda_solicitante', 'contenido_campos');" >
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:TextBox ID="TextBoxNOMBRE_PERSONA_EXPEDIENTE" runat="server" cap_name="NOMBRE_PERSONA_EXPEDIENTE" Width="98%" CssClass="mt-1 event_auto_source_expe"></asp:TextBox>

                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                  <span>Nit/Identificación solicitante</span>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:TextBox ID="TextBoxIDENTIFICACION_PERSONA_EXPEDIENTE" runat="server" cap_name="IDENTIFICACION_PERSONA_EXPEDIENTE" Width="98%" MaxLength="60" CssClass="mt-1 event_auto_source_expe"></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                  <span>Nombre Responsable</span>
                                  <input type="image" id="image_ayuda_responsable" src="../workflow/imageneswf/ayuda.png" style="height: 20px" class="def" onclick="ayuda_compartir('image_ayuda_responsable', 'contenido_campos');">
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:TextBox ID="TextBoxNOMBRE_RESPONSABLE_EXPEDIENTE" runat="server" cap_name="NOMBRE_RESPONSABLE_EXPEDIENTE" Width="98%"  CssClass="mt-1 event_auto_source_expe"></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                  <span>Nit/Identificación responsable</span>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:TextBox ID="TextBoxIDENFICACION_RESPONSABLE_EXPEDIENTE" runat="server" Width="98%" cap_name="NOMBRE_RESPONSABLE_EXPEDIENTE" MaxLength="60" CssClass="mt-1 event_auto_source_expe"></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell ColumnSpan="2" Style="text-align: center; width: 100%" CssClass="mt-2">
                                   <span class="h5">CLASIFICACION DOCUMENTAL</span>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                  <span style=" color:red">Tipo unidad documental(*)</span>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:DropDownList ID="DropDownListNOMBRE_TIPO_UNIDAD_DOCUMENTAL" CssClass="mt-1 custom-select" Width="99%" runat="server"></asp:DropDownList>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                  <span>Fondo documental</span>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:DropDownList ID="DropDownListNOMBRE_FONDO" Width="99%" CssClass="mt-1 custom-select" runat="server" onchange="selecion_change_organigrama();"></asp:DropDownList>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                   <span style=" color:red">"Organigrama (*)</span>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:DropDownList ID="DropDownListorganigrama" Width="99%" runat="server" CssClass="mt-1 custom-select" onchange="selecion_change_organigrama();"></asp:DropDownList>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                  <span style=" color:red">Area/departamento(*)</span>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:DropDownList ID="DropDownListArea" Width="99%" runat="server" CssClass="mt-1 custom-select" onchange="selecion_change_area();"></asp:DropDownList>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                  <span>Instrumento</span>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:DropDownList ID="DropDownList_instrumento" Width="99%" runat="server" CssClass="mt-1 custom-select" AutoPostBack="true"></asp:DropDownList>
                                            <asp:DropDownList ID="DropDownListsub_seccion" Width="99%" runat="server" onchange="selecion_change_sub_area();" Style="display: none"></asp:DropDownList>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                  <span>Ciclo Expediente archivo</span>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:DropDownList ID="DropDownListNOMBRE_CICLO_ARCHIVO" Width="99%" runat="server" CssClass="mt-1 custom-select"></asp:DropDownList>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                  <span>Serie documental</span>
                                        </asp:TableCell>
                                        <asp:TableCell>

                                            <asp:DropDownList ID="DropDownListSerie" Width="99%" runat="server" CssClass="mt-1 custom-select" onchange="selecion_change_serie();"></asp:DropDownList>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                  <span>Sub serie documental</span>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:DropDownList ID="DropDownListSubserie" Width="99%" runat="server" CssClass="mt-1 custom-select"></asp:DropDownList>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                  <span  style=" color:red">Tipo expediente(*)</span>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:DropDownList ID="DropDownListBoxtipoexpediente" Width="99%" runat="server" CssClass="mt-1 custom-select" onchange="lista_ayuda_expediente();"></asp:DropDownList>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                 
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:TextBox ID="TextBoxayuda" runat="server" Width="98%" Style="height: 30px" ReadOnly="True" CssClass="mt-1" TextMode="MultiLine" BackColor="Yellow"></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                  <span>Tipo unidad de expediente</span>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:DropDownList ID="DropDownList_tipo_unidad_conservacion" Width="98%" runat="server" CssClass="mt-1 custom-select"></asp:DropDownList>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                  <span>Paginas digitalizadas</span>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:TextBox ID="TextBoxNUMERO_DIGITALIZADO_CONTENIDO" runat="server" Width="50%" ReadOnly="true" Text="0" class="solo-numero mt-1 "></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                   <span>folios físicos</span>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:TextBox ID="TextBoxNUMERO_FOLIOS_CONTENIDOS" runat="server" Width="50%" ReadOnly="true" Text="0" class="solo-numero mt-1 "></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>
                                  <span>Documentos electrónicos</span>
                                        </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:TextBox ID="TextBoxNUMERO_ELECTRONICO_CONTENIDO" runat="server" Width="50%" ReadOnly="true" Text="0" class="solo-numero mt-1 "></asp:TextBox>
                                        </asp:TableCell>
                                    </asp:TableRow>

                                    <asp:TableRow Style="display:none">
                                        <asp:TableCell>
                                            <asp:CheckBox ID="CheckBox_unidad_contenedora" runat="server" Text="" ForeColor="Red" Checked="true" />
                                            <span style="color: red" class="ml-1">Obliga unidad contenedora</span>
                                        </asp:TableCell>
                                        <asp:TableCell Style="">
                                            <asp:TextBox ID="TextBox_id_archivo" runat="server" MaxLength="9" ReadOnly="true" BackColor="Gray" class="mt-1"></asp:TextBox>
                                            <asp:Button ID="Button_activa_archivar_unidad" runat="server" Text="Archivar" ToolTip="Archiva unidad documental en unidad contenedora" CssClass="btn btn-success m-1" Style="font-size: 10px" OnClientClick="auto_zise_reasigna_expe_unidad();" />
                                            <asp:Button ID="Button_des_archivar" runat="server" Text="Desarchivar" CssClass="btn btn-success m-1" Style="font-size: 10px" ToolTip="Desarchiva unidad documental de unidad contenedora" />
                                            <input id="Hidden2" type="hidden" value="0" runat="server" />
                                            <input id="Hidden_tipo_unidad" type="hidden" value="0" runat="server" />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>

                            </asp:Panel>

                            <div id="contenedor_botones_postbak" style="display: none">
                                <asp:Button ID="Button_lista_ayuda_expediente" runat="server" Text="" Style="height: 1%" />
                                <asp:Button ID="Button_selecion_organigrama" runat="server" Text="" Style="height: 1%" />
                                <asp:Button ID="Button_selecion_area" runat="server" Text="" Style="height: 1%" />
                                <asp:Button ID="Button_seleccion_sub_area" runat="server" Text="" Style="height: 1%" />
                                <asp:Button ID="Button_selecion_serie" runat="server" Text="" Style="height: 1%" />
                                <asp:Button ID="Button1_seleccion_expediente_manual" runat="server" Text="" Style="height: 1%" />
                            </div>

                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>
                    <asp:Label ID="Labelresultado" runat="server" Text="" Style="float: right; font-size: 11px"></asp:Label>
                </div>


                <div style="display: none; height: 1px">
                    <asp:Button ID="Button_add_edit_expediente" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                    <asp:Button ID="ButtonSalir_add_edit_expediente" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    <asp:Button ID="Button_cerrar_add_edit_expediente" runat="Server" Text="X" CssClass="invisible" />
                </div>

                <div id="content_boton_add_edit_expediente" class="modal-footer justify-content-end">        
                    <asp:UpdatePanel ID="Updatepanel_botones" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <span id="result" class="mr-4" style="color:red"> </span>
                            <input id="Button_aceptar_java" type="button" value="Aceptar" tip_event="" class="btn btn-success" onclick="event_element_clic(event, this);"/> 
                            <asp:Button ID="ButtonRestaurar_" runat="server" Text="Restaurar" Style="display:none" ToolTip="Restaurar unidad conservacion contenedora" CssClass="btn btn-success" />
                            <input id="Hidden_resultado" type="hidden" value="" runat="server" />
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
            </div>
        </asp:Panel>
         <asp:UpdatePanel ID="UpdatePanel_boton" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    
                    <asp:Button ID="Button_show_printer" runat="server" Text="Button" style="display:none"/>
                </ContentTemplate>
             
            </asp:UpdatePanel>
         <!-- Modal ayuda -->
        <div id="myModal" class="modal-ayuda" style="display: none">
            <div id="mytexto_" class="modal-content-ayuda" style="overflow:auto">
                <span class="close" onclick="hide_autonomo();">&times;</span>
                <p id="tex_modal" style="justify-content: center; font-family: Arial; font-size: 11px; text-align: justify">Some text in the Modal..</p>
            </div>

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
         <!--PROGRES-->
        <div id="Divpro_gres_bar">
            <asp:Panel ID="Panel_pro_gres_bar" runat="server" Style="display:none; color: White; width:30%; height:auto" CssClass="border_superior_inferior_radius_blanco">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_pro_gres_bar" runat="server"  TargetControlID="ButtonSalir_pro_gres_bar" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_pro_gres_bar" PopupControlID="Panel_pro_gres_bar"></asp:ModalPopupExtender>
                <div id="div1" class="border_superior_radius_blanco" style="display:none">                
                    <asp:Label ID="Label_pro_gres_bar" runat="server" Text=""  Style="">
                    </asp:Label>
                    <div id="Divcerrarbuton2_pro_gres_bar" style="float: right">
                        <asp:Button ID="Button_cerrar_pro_gres_bar" runat="Server" Text="X" style="display:none" 
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_pro_gres_bar" style="width:99%; height:99%" class="modal_content_back_no_radio" > 
                      <br />   
                    <div style="text-align:center">
                         <asp:Label ID="Label_progres_bar" runat="server" Text="Progreso de la tarea" style="font-family:Arial; text-align:center; font-size:20px"></asp:Label>
                    </div>
                    <br />  
                     <div id="myProgress_contador" style="text-align: center; font-family:Arial; font-size:14px">
                        
                             0 
                        </div>
                    <div id="myProgress_porcent" style="text-align: center; font-family:Arial; font-size:14px">
                            0 %
                        </div>                
                        <div style="margin-left:5%; margin-right:5%">
                            <div id="myProgress" >
                            <div id="myBar" ></div>
                        </div>
                        </div>         
                        <br/>
                        <div style="text-align: center">
                            <button class="boton_blanco" onclick="myStopFunction(event)" >Cancelar</button>
                        </div>
                              
                    <asp:UpdatePanel ID="UpdatePanel_pro_gres_bar" runat="server" UpdateMode="Conditional">
                        <ContentTemplate> 
                           
                            <asp:Button ID="Button_pogres_show" CssClass="invisible" runat="server" Text="Button" style="display:none" />   
                            
                        </ContentTemplate>
                    </asp:UpdatePanel>
                         
                    <asp:Button ID="ButtonSalir_pro_gres_bar" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />    
                </div>
           </asp:Panel>
        </div>  
        <!--Termina mensaje_personalizado-->
        <div id="Impresion_post">
            <asp:Panel ID="Panelimpresionpost" runat="server" Style="display: none; width: auto; height: auto" CssClass="modal_content_general">
                
                <asp:ModalPopupExtender ID="ModalPopupExtenderimpre_post" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="ButtonSalir_post"
                    PopupControlID="Panelimpresionpost" CancelControlID="Buttoncerrarimpre_post">
                </asp:ModalPopupExtender>
                <div id="modal_content_impre_post" class="modal-content">
                    <div id="divcabecer2_post" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Menú Impresión</h6>
                         <button type="button" value="Buttoncerrarimpre_post" class="close da_event_captive">&times;</button>   
                    </div>
                    <asp:UpdatePanel ID="UpdatePaneliframe_post" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="ContenidoImpresion_post" style="height: auto; width: auto ; border-top:none; overflow:auto" class="modal_content_back  pl-3 pr-3">
                                <iframe width="100%" height="100%" id="ifimpre_post_" runat="server" frameborder="0"></iframe>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button1_post" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" Style="display: none" />
                        <asp:Button ID="ButtonSalir_post" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" Style="display: none" />
                        <asp:Button ID="Buttoncerrarimpre_post" runat="Server" Text="X" CssClass="invisible" Style="display: none" />
                    </div>
                </div>
            </asp:Panel>
        </div>
       <!--configura_plantilla_rotulo-->
          <div id="configura_plantilla_rotulo">
            <asp:Panel ID="Panel_configura_plantilla_rotulo" runat="server" Style="display:none; width:40%; height:auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_configura_plantilla_rotulo" runat="server" BehaviorID="Panel_configura_plantilla_rotulo" TargetControlID="ButtonSalir_configura_plantilla_rotulo" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_configura_plantilla_rotulo" PopupControlID="Panel_configura_plantilla_rotulo" ></asp:ModalPopupExtender>
                <div id="modal_content_configura_plantilla_rotulo" class="modal-content">
                    <div id="divcabecer2_configura_plantilla_rotulo" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title d-inline ml-1">Seleccionar plantilla</h6>
                        <button type="button" value="Button_cerrar_configura_plantilla_rotulo" class="close da_event_captive">&times;</button>  
                    </div>
                    <div id="contenido_procesa_configura_plantilla_rotulo" style="border-top:none; overflow:auto" class="modal_content_back pl-3 pr-3">
                        <asp:UpdatePanel ID="UpdatePanel_configura_plantilla_rotulo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div style="text-align: center">
                                    <br />
                                    <span></span>                           
                                    <br />
                                    <asp:DropDownList ID="DropDownList_configura_plantilla_rotulo" runat="server" CssClass="mt-1 custom-select"></asp:DropDownList>
                                    <br />
                                    <br />
                                    <asp:Button ID="Button_aceptar_configura_plantilla_rotulo" runat="server" Text="Aceptar" CssClass="btn btn-success mb-4" />
                                 
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_configura_plantilla_rotulo" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                        <asp:Button ID="ButtonSalir_configura_plantilla_rotulo" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                        <asp:Button ID="Button_cerrar_configura_plantilla_rotulo" runat="Server" Text="X" CssClass="invisible" />
                    </div>
                </div>
            </asp:Panel>
        </div>
        <asp:Panel ID="Panel_volumenes_relacionados" runat="server" Style="display:none; width: 70%; height: auto" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_volumenes_relacionados" runat="server"
                TargetControlID="ButtonSalir_volumenes_relacionados" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_volumenes_relacionados" PopupControlID="Panel_volumenes_relacionados">
            </asp:ModalPopupExtender>
            <div id="modal_content_volumenes_relacionados" class="modal-content">
                <div id="div_mod_title_volumenes_relacionados" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline ml-1">Volúmenes relacionados</h6>
                    <button type="button" value="Button_cerrar_volumenes_relacionados"  class="close da_event_captive mr-2">&times;</button>
                </div>
                <div id="contenido_procesa_volumenes_relacionados" style="background-color: white; width: 100%; height: 100%" class="modal_content_back pl-3 pr-3">
                    <div class="nav-person-da" id="item_nav_tab_volumen">
                        <ul class="nav nav-tabs mt-2" id="myTab_volumen" role="tablist">
                            <li class="nav-item" >
                                <a class="nav-link nav-link-person active show" id="home_relacion_volumen"  data-toggle="tab" href="#home_relacion_volumen_content" role="tab" aria-controls="home_radic" aria-selected="true"><i style="color: #0062cc" id="home-relacion-volumen" class="fad fa-folders "></i> Volumes relacionados</a>
                            </li>
                            <li class="nav-item" onclick="auto_zise_popup_volumenes_relacionados();">
                                <a class="nav-link nav-link-person " id="home_relacionar_volumen"   data-toggle="tab" href="#home_relacionar_volumen_content" role="tab" aria-controls="profile" aria-selected="false"><i style="color: #0062cc" id="home-relacionar-volumen" class="fad fa-copy"></i> Relacionar volumen </a>
                            </li>

                        </ul>
                    </div>

                    <div class="tab-content" id="item_tab_content_">
                        <div class="tab-pane  p-2 active show" id="home_relacion_volumen_content" role="tabpanel" aria-labelledby="home-tab">
                            <div id="div_contenedor_titulo_documentos_relacionados" style="width: 100%" class=" text-center p-3">
                                <asp:Label ID="Label_title_volumenes_relacionados" runat="server" Text="Volúmenes relacionados" CssClass="h6 font-weight-bold"></asp:Label>
                            </div>
                            <asp:UpdatePanel ID="UpdatePanel_volumenes_relacionados_title" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div id="contenido_titulo_val_radicacion_documentos" style="height: auto; width: 99%" class="p-3">
                                        <asp:Label ID="titulo_volumenes_relacionados" runat="server" CssClass="h7 font-weight-light">Resultados busqueda</asp:Label>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div id="content_data_grid_volumenes_relacionados" class="conten_gred_border_">
                                <asp:Panel ID="Panel_grid_volumenes_relacionados" runat="server" Wrap="False"
                                    Style="height: 98%" ScrollBars="Both">
                                    <asp:UpdatePanel ID="UpdateGeneral_volumenes_relacionados" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:GridView ID="data_grid_volumenes_relacionados" runat="server" Style="position: inherit; margin-left: 2px; width: 99%"
                                                AutoGenerateSelectButton="False" CssClass="table font-weight-light" GridLines="None" EnableViewState="true">
                                                <SelectedRowStyle BackColor="LightSkyBlue" Font-Bold="True" ForeColor="Red" />
                                                <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                                <RowStyle CssClass="GridviewScrollItem_line" />
                                                <PagerStyle CssClass="GridviewScrollPager_line" />
                                                <Columns>
                                                    <asp:BoundField HeaderText="OPCIONES   " />
                                                </Columns>
                                            </asp:GridView>
                                            <input id="hdn_id_elment" type="hidden" value="0" runat="server" />
                                        </ContentTemplate>
                                        <Triggers>
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </asp:Panel>
                            </div>
                        </div>
                        <div class="tab-pane  p-2" id="home_relacionar_volumen_content" role="tabpanel" aria-labelledby="profile-tab">
                            <div id="div_rel_padre_volumen" class="border_general_blanco_ row justify-content-end mt-2" style="max-height: 50px">
                                <div class="col-4">
                                    <asp:CheckBox ID="CheckBox_optio_busq_volumen" runat="server" Style="float: right; margin-right: 1px" Text="Solo palabra completa" Checked="true" CssClass="custom-checkbox" />
                                </div>
                                <div class="input-group  col-8">
                                    <asp:TextBox ID="TextBox_busqueda_padres_volumen" runat="server" class="form-control form-control-sm complex  " placeholder="Busqueda...." Style="max-height: 40px"></asp:TextBox>
                                    <div class="input-group-append">
                                        <button class="btn btn-outline-secondary" onclick="activa_busqueda_volumenes_relacionados_padre(event, this)" type="button" style="max-height: 40px">
                                            <i class="fal fa-search"></i>
                                        </button>
                                    </div>
                                </div>

                            </div>
                            <div id="Div_title_resultados_relacionados_volumen" style="height: auto; width: 99%" class="p-2">
                                <asp:UpdatePanel ID="UpdatePanel_relacionar_volumen_title__volumen" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Label ID="titulo_relacionar_volumen" runat="server" CssClass="h6 font-weight-light">Resultados busqueda</asp:Label>
                                    </ContentTemplate>
                                    <Triggers>
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                            <div id="content_data_grid_relacionar_volumen" class="conten_gred_border_" style="width: 100%">
                                <asp:Panel ID="Panel_grid_relacionar_volumen" runat="server" Wrap="False"
                                    Style="height: 98%" ScrollBars="Both">
                                    <asp:UpdatePanel ID="UpdateGeneral_relacionar_volumen" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:GridView ID="data_grid_relacionar_volumen" runat="server" Style="position: inherit; margin-left: 2px; width: 99%"
                                                AutoGenerateSelectButton="False" CssClass="table font-weight-light" GridLines="None" EnableViewState="true">
                                                <SelectedRowStyle BackColor="LightSkyBlue" />
                                                <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                                <RowStyle CssClass="GridviewScrollItem_line_corte_tr" />
                                                <PagerStyle CssClass="GridviewScrollPager_line" />
                                                <Columns>
                                                    <asp:BoundField HeaderText="OPCIONES   " />
                                                </Columns>
                                            </asp:GridView>
                                            <input id="Hidden_relacion_volumen" type="hidden" value="-1" runat="server" />
                                        </ContentTemplate>
                                        <Triggers>
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </asp:Panel>
                            </div>
                            <div style="display: none; height: 1px">
                                <asp:Button ID="Button_relacionar_volumen" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                                <asp:Button ID="ButtonSalir_relacionar_volumen" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                                <asp:Button ID="Button_cerrar_relacionar_volumen" runat="Server" Text="" CssClass="invisible" />
                                <asp:UpdatePanel ID="UpdatePanel_boton_relacionar_volumen" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <asp:ImageButton ID="ImageButton_buscar_volumen" runat="server" Style="display: none" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>


                </div>
                <div style="display: none; height: 1px">
                    <asp:Button ID="Button_cerrar_volumenes_relacionados" runat="Server" Text="" CssClass="invisible" />
                    <asp:Button ID="Button_volumenes_relacionados" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                    <asp:Button ID="ButtonSalir_volumenes_relacionados" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                    <asp:UpdatePanel ID="UpdatePanel_boton_volumenes_relacionados" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <input id="Hidden_eli_rel" type="hidden" value="-1" runat="server" />
                            <input id="Hidden_eli_result" type="hidden" value="" runat="server" />
                            <asp:Button ID="Button_export_lista" runat="server" Text="Exportar lista" ToolTip="Exportar lista" CssClass="boton_azul" Style="float: right; margin: 3px 3px 3px 3px; display: none" OnClientClick="activa_export_lista('Hidden_colum_header','')" />
                            <asp:Button ID="Button_active_eli_rel" runat="server" Text="" ToolTip="" CssClass="boton_azul" Style="display: none" OnClientClick="ConfirmMensajeGeneral('Desea eliminar la relación como expediente volumen?','HiddenPROMP')" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="Panel_padres_relacionados" runat="server" Style="display:none; width: 70%; height: 100%" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_padres_relacionados" runat="server"
                TargetControlID="ButtonSalir_padres_relacionados" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_padres_relacionados" PopupControlID="Panel_padres_relacionados">
            </asp:ModalPopupExtender>
            <div id="modal_content_padres_relacionados" class="modal-content">
                <div id="div_mod_title_padres_relacionados" class="modal_title_superior_ modal-header">
                    <h6 class="modal-title d-inline">Relacionar expediente volumen</h6>
                    <button type="button" value="Button_cerrar_padres_relacionados" class="close da_event_captive">&times;</button>

                </div>
                <div id="contenido_procesa_padres_relacionados" style="background-color: white; width: 99%; height: auto; padding-left: 2px; border-top: none; overflow: auto" class="modal_content_back  pl-3 pr-3 pb-3">
                    <div id="div_label_title" style="text-align: center" class="p-3">
                        <asp:Label ID="Label_title_padres_relacionados" runat="server" Text="Volúmenes a relacionar" CssClass="h6 font-weight-light"></asp:Label>
                    </div>
                    <div id="div_rel_padre" class="border_general_blanco_ row justify-content-end mt-2" style="max-height: 50px">
                        <div class="col-4">
                            <asp:CheckBox ID="CheckBox_optio_busq" runat="server" Style="float: right; margin-right: 1px" Text="Solo palabra completa" Checked="true" CssClass="custom-checkbox" />
                        </div>
                        <div class="input-group  col-8">
                            <asp:TextBox ID="TextBox_busqueda_padres" runat="server" class="form-control form-control-sm complex  " placeholder="Busqueda...." Style="max-height: 40px"></asp:TextBox>
                            <div class="input-group-append">
                                <button class="btn btn-outline-secondary" onclick="activa_busqueda_volumenes_relacionados(event, this)" type="button" style="max-height: 40px">
                                    <i class="fal fa-search"></i>
                                </button>
                            </div>
                        </div>

                    </div>
                    <div id="Div_title_resultados_relacionados" style="height: auto; width: 99%" class="p-2">
                        <asp:UpdatePanel ID="UpdatePanel_padres_relacionados_title" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="titulo_padres_relacionados" runat="server" CssClass="h6 font-weight-light">Resultados busqueda</asp:Label>
                            </ContentTemplate>
                            <Triggers>
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                    <div id="content_data_grid_padres_relacionados" class="conten_gred_border_" style="width: 100%">
                        <asp:Panel ID="Panel_grid_padres_relacionados" runat="server" Wrap="False"
                            Style="height: 98%" ScrollBars="Both">
                            <asp:UpdatePanel ID="UpdateGeneral_padres_relacionados" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:GridView ID="data_grid_padres_relacionados" runat="server" Style="position: inherit; margin-left: 2px; width: 99%"
                                        AutoGenerateSelectButton="False" CssClass="table font-weight-light" GridLines="None" EnableViewState="true">
                                        <SelectedRowStyle BackColor="LightSkyBlue" />
                                        <HeaderStyle CssClass="GridviewScrollHeader_line_boot" BorderStyle="None" />
                                        <RowStyle CssClass="GridviewScrollItem_line_corte_tr" />
                                        <PagerStyle CssClass="GridviewScrollPager_line" />
                                        <Columns>
                                            <asp:BoundField HeaderText="OPCIONES   " />
                                        </Columns>
                                    </asp:GridView>
                                    <input id="Hidden_eli_rel_volumen" type="hidden" value="-1" runat="server" />
                                </ContentTemplate>
                                <Triggers>
                                </Triggers>
                            </asp:UpdatePanel>
                        </asp:Panel>
                    </div>
                    <div style="display: none; height: 1px">
                        <asp:Button ID="Button_padres_relacionados" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="ButtonSalir_padres_relacionados" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        <asp:Button ID="Button_cerrar_padres_relacionados" runat="Server" Text="" CssClass="invisible" />
                        <asp:UpdatePanel ID="UpdatePanel_boton_padres_relacionados" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <input id="Hidden_eli_result_padres" type="hidden" value="" runat="server" />
                                <asp:ImageButton ID="ImageButton_buscar" runat="server" Style="margin-right: 4px; float: right; margin-top: 3px; margin-bottom: 3px; display: none" ImageUrl="../radicador/imagenes/cbxs0-vnnbp.png" />
                                <asp:Button ID="Button_active_eli_rel_padres" runat="server" Text="" ToolTip="" CssClass="boton_azul" Style="display: none" OnClientClick="ConfirmMensajeGeneral('Desea relacionar como expediente volumen  al expediente seleccionado ?','HiddenPROMP')" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>


                </div>
            </div>
        </asp:Panel>
       
        <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
        </div>
        <div style="display:none">
            <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <iframe runat="server" style="float: left" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                        frameborder="0" />
                </ContentTemplate>

            </asp:UpdatePanel>
            </div>
    </form>
     <script  accesskey="javascript" type="text/javascript">
        
         $(document).ready(function () {
             $('#sidebarCollapse').on('click', function () {
                 $('#sidebar_').toggleClass('active_da_slider');
                 $('#Contenedorderecho').toggleClass('active_content_rigth');
                 $('#Contentizquierdo').toggleClass('active_content_left');
                 $(this).toggleClass('active_da_slider');
                 $('#da_show-sidebar_').toggleClass('show_da_slide');
                 $('#da_show-sidebar_').toggleClass('hide_da_sidebar');
                 auto_zise();
             });
             $('#da_show-sidebar_').on('click', function () {
                 $('#sidebar_').toggleClass('active_da_slider');
                 $('#Contenedorderecho').toggleClass('active_content_rigth');
                 $('#Contentizquierdo').toggleClass('active_content_left');
                 $(this).toggleClass('show_da_slide');
                 $(this).toggleClass('hide_da_sidebar');
                 auto_zise();

             });
         });
     </script>
</body>
   
</html>
