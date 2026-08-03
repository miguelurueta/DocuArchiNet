<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormRespuestaRadicado.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormRespuestaRadicado" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %><%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %><!DOCTYPE html><html xmlns="http://www.w3.org/1999/xhtml"><head runat="server"><meta http-equiv="Content-Type" content="text/html; charset=utf-8"/><link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" /><link href="../Styles/Aplicaction.css" rel="stylesheet" /><style type="text/css">
     .auto-style1 {
         height: 29px;
     }
     .auto-style2 {
         height: 34px;
     }
     .auto-style3 {
         height: 24px;
     }
 </style><title></title><script src="../js/ui/jquery-3.4.1.min.js"></script><script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script><script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script><link href="../js/ui/jquery-ui.css" rel="stylesheet" /><link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" /><script src="../bootstrap/js/bootstrap.min.js"></script><link href="../Styles/bootra-person.css" rel="stylesheet" /><link href="../tokenzize2/tokenize2.min.css" rel="stylesheet" /><script src="../tokenzize2/tokenize2.1.min.js"></script><link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" /><script src="../js/jquery.contextMenu.js" type="text/javascript"></script><link href="../Styles/tumb.css" rel="stylesheet" /><link href="../js/jquery-ui-1.12.1.custom/style.css" rel="stylesheet" /><script defer src="../Awesome/js/all.js"></script><link href="../Awesome/css/fontawesome.css" rel="stylesheet"><link href="../Awesome/css/brands.css" rel="stylesheet"><link href="../Awesome/css/solid.css" rel="stylesheet"><script defer src="../Awesome/js/brands.js"></script><script defer src="../Awesome/js/solid.js"></script><script defer src="../Awesome/js/fontawesome.js"></script><script src="../js/MyJavaScriptFile.js"></script><script src="../js/java_general/general_code_java.js"></script><script src="../js/radicacion/WebFormRespuestaRadicado.js"></script><script src="../js/validate_campos.js"></script></head><body style="margin-top:1px; width:100%; height:100%"><form id="form1" runat="server" onkeypress="return caracter_especial(event,this)">
          <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True" AsyncPostBackTimeout="900" EnableScriptGlobalization="true"  EnableScriptLocalization="true">
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
                if (elmen.type == "button" ||  elmen.type == "submit") {
                    value_element = elmen.value;
                    elmen.value = "Espere..."
                    elmen.disabled = true;
                }
            }
            function CheckStatus(sender, args) {
                try {
                    progres_hiden('progres_bar');
                    //$("#Menu1").show();
                    if (elment_postbak.type == "button" || elment_postbak.type == "submit") {
                        elment_postbak.value = value_element;
                        elment_postbak.disabled = false;
                    }
                    if (elment_postbak.id == "Button_confirmar") {
                        if (document.getElementById("Hidden_resultado_ventana").value == "YES") {
                            document.getElementById("Hidden_resultado_ventana").value = "";
                        
                            document.getElementById("Button_cerrar_radica_documento_respuesta").click();            
                        }
                    }
                    if (elment_postbak.id == "Button_confirmar_envio_respuesta") {
                        if (document.getElementById("Hidden_resultado_envio").value == "YES") {
                            document.getElementById("Hidden_resultado_envio").value = "";
                            var boton = window.parent.document.getElementById('Button_actualiza_trevie_seleccion');
                            if (boton != undefined) {
                                window.parent.document.getElementById('Button_actualiza_trevie_seleccion').click();
                            }
                            boton = window.parent.document.getElementById('ContentPlacenter_Button_actualiza_trevie_seleccion');
                            if (boton != undefined) {
                                //Hidden_estado_actualizacion
                                window.parent.document.getElementById('ContentPlacenter_Hidden_estado_actualizacion').value == "YES";
                                window.parent.document.getElementById('ContentPlacenter_Button_actualiza_trevie_seleccion').click();
                            }
                        }
                    }
                    if (elment_postbak.id == "ImageButton_desplegar_formal") {
                        if (document.getElementById("Panel1_CollapsiblePanelExtender_ClientState").value == "false") {

                            //size_colla_panel_formal();
                            size_colla_panel_formal_radicado();
                            size_colla_panel_confirma_radicado();
                        }
                    }
               
                    if (elment_postbak.id == "ImageButton_confirmar_firma") {
                        if (document.getElementById("panel2_CollapsiblePanelExtender_ClientState").value == "false") {

                            //size_colla_panel_confirma();
                            size_colla_panel_confirma_radicado();
                            size_colla_panel_formal_radicado();
                        }
                    }
                    if (elment_postbak.id == "Button_anexo_cargar") {
                        document.getElementById("Label_sube_documento_respuesta").innerHTML = "Carga anexos de la respuesta";
                    }
                    if (elment_postbak.id == "Button_anexo_cargar_simple") {
                        document.getElementById("Label_sube_documento_respuesta").innerHTML = "Carga anexos de la respuesta";
                    }
                    if (elment_postbak.id == "Button_carga_plantilla") {
                        document.getElementById("Label_sube_documento_respuesta").innerHTML = "Adjunto respuesta";
                    }
                    if (elment_postbak.id == "Button_sube_documento" || elment_postbak.id == "Button_radicar_tramite") {
                        asig_correo_token('tokenize-callable-demo_respuesta_');
                    }
                    if (elment_postbak.id == "Button_me_active_men_dive" ) {
                        asig_correo_token('tokenize-callable-demo_respuesta__');
                    }
                } catch (e) { alert("Fucion CheckStatus " + e.message) }
            }

            </script>
    <div id="contenedor_general_respuesta" style="left: auto; width: 100%; height: 99%; background-color: White; float:none; margin-top:0px ">     
        <div id="center" style="float:right; width:100%; height:100%; left:auto">
            <div id="opciones_seleccion" >
                <div id="menu_var" class="navbar_gray_ navbar navbar-expand-sm nav_botota_person modal_content_no_back_inferior" >
                    <button class="navbar-toggler" type="button" style=" background-color:#6d7fcc" data-toggle="collapse" data-target="#navbarNavDropdown">
                         <span class="navbar-toggler-icon_"><i style="color:white" class="fad fa-th-list"></i></span>
                    </button>
                    <div class="collapse navbar-collapse row" id="navbarNavDropdown">        
                        <ul class="navbar-nav col-md-8">
                            <li class="nav-item dropdown active ml-2 active_">
                                <a class="nav-link dropdown-toggle bot_hover_person" style="color: #6d7fcc" href="#" id="navbarDropdownMenuLink" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: #0062cc" class="fad fa-line-columns"></i> Detalle
                                </a>
                                <div class="dropdown-menu " aria-labelledby="navbarDropdownMenuLink">
                                      <a href="#" class="dropdown-item" onclick="activa_menu_general_diference(event,this,'D-D-R-R')"><i class="fal fa-table"></i> Detalle respuesta radicado</a>
                                      <a href="#" class="dropdown-item" onclick="activa_menu_general_diference(event,this,'D-V-D-T')"><i class="fal fa-list-ol"></i> Transacciones de la respuesta</a>
                                </div>
                            </li>
                            <li class="nav-item dropdown active ml-2 mr-0 active_">
                                <a class="nav-link  dropdown-toggle" style="color:#6d7fcc" href="#" id="navbarDropdownMenuLink_" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color:#0062cc" class="fad fa-th-list  "></i> Opciones
                                </a>
                                <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                                    <a href="#" class="dropdown-item" onclick="activa_menu_general_diference(event,this,'N-R-C')"><i class="fal fa-envelope-square"></i> Notificar respuesta al correo electrónico</a>
                                    <a href="#" class="dropdown-item" onclick="activa_menu_general_diference(event,this,'R-R-D')"><i class="fal fa-undo"></i> Reversar gestión tramite</a>
                                    <a href="#" class="dropdown-item" onclick="activa_menu_general_diference(event,this,'R-P-I')"><i class="fal fa-user"></i> Reasignar peticionario</a>
                                    
                                </div>
                            </li> 
                              
                        </ul>
                    </div>                 
                </div>
            </div>           
            <asp:Panel ID="Panel_seleccion" runat="server" Style="height:98%; display:block; overflow:auto;  background-color:white; border-top: inset 1px solid #ccc;" >
                <asp:UpdatePanel ID="UpdatePanel_respuesta" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>                        
                        <div id="div_title" style="text-align:center">
                            <asp:UpdatePanel ID="UpdatePanel_titulo_respuesta" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Label ID="label_title" runat="server" Text="Respuesta tramite" Style="font-size: 12px; font-family: Arial; color:black; font-weight:700"></asp:Label>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            
                        </div>
                        <div id="div_tab_content_resp_general" style="height: auto; margin-left: 15px; margin-right: 15px">
                            <ul class="tab" style="background-color: white; height: auto">
                                <li><a style="font-family: Arial" href="javascript:void(0)" class="tablinks__" onclick="openCity__(event, 'div_tab_resp_formal')" id="default_formal"><i class="fad fa-file"></i> Respuesta formal </a></li>
                                <li><a style="font-family: Arial" href="javascript:void(0)" class="tablinks__" onclick="openCity__(event, 'div_tab_solo_confirmar')" id="default_confirmar"><i class="fad fa-reply"></i> Confirmar o darse por enterado</a></li>
                            </ul>
                        </div>
                        <div id="div_tab_resp_formal" class="tabcontent___boot_da" style="margin-left:15px; margin-right:15px; border:none">    
                            <div id="div_resp_formal" style="margin-top: 1px; border-color: #b0c4de; border-style: ridge; border-width: 1px; background-color: #f5f5f5; display:none">
                                <asp:ImageButton ID="ImageButton_desplegar_formal" runat="server" src="../Radicador/imagenes/mas.png" Style="float: right; height: 15px; margin-right: 5px" OnClientClick="prevent_hident(event)" />
                                <asp:Label ID="label_text_title" runat="server" Text="(1) Elaborar una respuesta a la petición o solicitud " Style="font-size: 13px; font-family: Arial; font-weight: 600"></asp:Label>
                                <asp:Image ID="Image_formal_visto" runat="server" src="../Gestion/imagenes/visto_bueno.png" Style="height: 15px; width: 40px; margin-left: 5px; display: none; float: left" />
                            </div>
                            <asp:Panel ID="Panel_respuesta_formal" runat="server" Style="width: 100%; height: auto">
                                <div id="Content" style="height: auto; display:none">
                                    <ul class="tab_" style="background-color: white; height: auto">
                                        <li><a style="font-family: Arial" href="javascript:void(0)" class="tablinks" onclick="openCity(event, 'conten_general_respuesta_formal')" id="defaultOpen"><i class="fal fa-file-edit"></i> Elabora respuesta</a></li>
                                        <li><a style="font-family: Arial" href="javascript:void(0)" class="tablinks" onclick="openCity(event, 'content_anexos')"><i class="fal fa-paperclip"></i> Anexos</a></li>
                                    </ul>
                                </div>
                                <div id="div_image_semaforo" style="width: 100%;" class="modal_content_back_   col-12  d-flex   d-sm-flex d-md-inline d-lg-inline ">
                                    <asp:UpdatePanel ID="UpdatePanel_image_semaforo" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                        <ContentTemplate>
                                            <asp:Image ID="Image_estado_resp" CssClass="img-responsive" runat="server" Style="height:50px; width:inherit; float: right" ImageUrl="../radicador/imagenes/electronica_resp_estado_V0.png" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                                <div id="title_resp" style="margin-top: 1px; border-color: #b0c4de; border-width: 1px; border-style: ridge; text-align: center; width: 100%; float: right; display:none">
                                    <asp:Label ID="Label_desc" runat="server" Text="" Style="text-align: center; font-family: Arial; font-size: 16px"></asp:Label>
                                </div>
                                <div id="conten_general_respuesta_formal" style="float: right; width: 100%; height: 100%; border: none" class="tabcontent_boot_da_">
                                    <div id="Div2" style="float: left; width: 2%; height: 100%; left: auto; position: static; height: 150px; display: none">
                                        <input id="Hidden_estado_update" type="hidden" value="-1" runat="server"/>
                                        <input id="Hidden_height" type="hidden" value="0" runat="server"/>
                                        <input id="Hidden_width" type="hidden" value="0" runat="server"/>
                                        <asp:UpdatePanel ID="UpdatePanel_hiden_resp" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <input id="Hidden_tipo_respuesta" type="hidden" value="-1" runat="server"/>
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
                                                    <input type="image" id="image_ayuda_descarga" src="../workflow/imageneswf/ayuda.png" style="height: 20px; display: none" onclick="ayuda_general('DFP', 'image_ayuda_descarga');">
                                                </div>
                                                <div class="">       
                                                    <button type="button"  class="btn btn-success da_event_captive mt-1" title="descarga protocolo de respuesta" value="Button_activa_descarga_formato"><i class="fad fa-arrow-to-bottom"></i></button>
                                                </div>
                                            </div>
                                            <div class="container-fluid pt-2 pb-2" style="margin-top: 10px; background-color: #e8e8f7">
                                                <div class="d-none">
                                                    <h5>Soportes para la respuesta</h5>
                                                </div>
                                                <div class="row">
                                                    <div class="col-2 ">
                                                        <h6 class="h6" style="text-overflow:ellipsis">Documento respuesta*</h6>            
                                                        <input type="image" id="image_ayuda_adjunta" src="../workflow/imageneswf/ayuda.png" style="height: 20px; display: none" onclick="ayuda_general('ADR', 'image_ayuda_adjunta');">
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
                                                        <button type="button" title="Cargar documento respuesta" value="Button_carga_plantilla" class="btn  btn-warning da_event_captive mt-1"><i class="fad fa-arrow-from-bottom"></i></button> 
                                                         <button type="button" title="Descargar documento respuesta" value="Button_descarga" class="btn btn-success  da_event_captive mt-1"><i class="fad fa-arrow-to-bottom"></i></button>                                
                                                         <button type="button" title="Eliminar documento respuesta" value="Button_eliminar" style="color:white" class="btn  btn-danger da_event_captive mt-1"><i class="fal fa-times"></i></button>
                                                                              
                                                    </div>
                                                     <div class="col-3 d-md-inline">
                                                           
                                                          <button type="button" title="Solicitar aprobación de la respuesta" value="Button_activa_registro_solicitud"  class="btn  btn-info da_event_captive mt-1"><i class="fal fa-file-check"></i></button>
                                                          <button type="button" title="Listar solictudes de aprobación de la respuesta" onclick="activa_menu_general_diference(event,this,'S-A-R-G-RD')"   class="btn  btn-secondary mt-1"><i class="fal fa-list-ul"></i></button>
                                                    </div>
                                                    <div class="col-7 d-md-inline">
                                                        <asp:UpdatePanel ID="UpdatePanel_anexos_respuesta" UpdateMode="Conditional" runat="server" RenderMode="Inline">
                                                            <ContentTemplate>
                                                                <asp:DropDownList ID="DropDownList_anexos_respuesta"  runat="server" Style="margin-left: 1px; max-width: 250px" CssClass="dropdown_ form-control d-sm-inline mt-1"></asp:DropDownList>

                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                        <button type="button" title="Cargar anexo" value="Button_anexo_cargar"  class="btn  btn-warning da_event_captive mt-1"><i class="fad fa-arrow-from-bottom"></i></button>
                                                        <button type="button" title="Eliminar anexo" value="Button_anexo_eliminar" style="color:white" class="btn  btn-danger da_event_captive mt-1"><i class="fal fa-times"></i></button>
                                                        <button type="button" title="Descargar anexo" value="Button_descargar_anexo" class="btn btn-success  da_event_captive mt-1"><i class="fad fa-arrow-to-bottom"></i></button>            
                                                          
                                                    </div>
                                                </div>
                                               
                                            </div>
                                            <div class="container-fluid pt-2 pb-2 " style="margin-top: 10px; background-color: #e8e8f7"> 
                                                 <div class="d-none">
                                                    <h5>Confirma respuesta para el tramite</h5>
                                                </div> 
                                                <div class="row">                    
                                                    <div class="col-6">
                                                        <h6 >Confirmar respuesta *</h6>
                                                     
                                                    </div>
                                                    <div class="col-6">
                                                         <h6>Opciones de respuesta final</h6>                      
                                                    </div>
                                                </div>
                                                <div class="row">
                                                   
                                                    <div class="col-6">
                                                        <button type="button" title="Confirmar respuesta" value="Button_radicar_tramite"  class="btn  btn-primary da_event_captive mt-1"><i class="fad fa-external-link-alt"></i></button>  
                                                    </div>
                                                    <div class="col-6">
                                                        <button type="button" title="Descargar respuesta final" value="Button_descarga_respuesta"  class="btn btn-success da_event_captive mt-1" ><i class="fad fa-arrow-to-bottom"></i></button>
                                                        <button type="button" title="Imprimir respuesta final" value="Button_imprimir" class="btn  btn-secondary da_event_captive mt-1" ><i class="fal fa-print"></i></button>
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
                                                <asp:Button ID="Button_activa_descarga_formato" runat="server" Style="width: 1px;  display: none"  />
                                                <asp:Button ID="Button_carga_plantilla" runat="server" Style="width: 1px;  display: none"  OnClientClick="eliminar_ajaxtolkit();" />
                                                <asp:Button ID="Button_descarga" runat="server" Style="width: 1px;  display: none"  />
                                                <asp:Button ID="Button_eliminar" runat="server" Style="width: 1px;  display: none"  OnClientClick="promp_respuesta('Desea eliminar el soporte documental de la respuesta?');" />
                                                <asp:Button ID="Button_activa_registro_solicitud" runat="server" Style="width: 1px; display: none" />
                                                <asp:Button ID="Button_radicar_tramite" runat="server" Style="width: 1px; display: none"  />
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <asp:UpdatePanel ID="UpdatePanel_combo_plantillas" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                        <ContentTemplate>
                                            <asp:Button ID="Button_descarga_respuesta" runat="server" Text="Descargar" Style="font-size: 12px; font-family: Arial; margin-left: 5px; font-family: Arial; font-size: 12px" CssClass="btn btn-primary" />
                                            <asp:Button ID="Button_imprimir" runat="server" Text="Impresión" Style="font-size: 12px; font-family: Arial; margin-left: 5px; font-family: Arial; font-size: 12px" CssClass="btn btn-primary" />
                                            <input id="Hidden_resp" type="hidden" value="0" runat="server"/>
                                        </ContentTemplate>

                                    </asp:UpdatePanel>
                                    </div>            
                                </div>
                                
                            </asp:Panel>
                            
                        </div>
                                     
                        <div id="div_tab_solo_confirmar" class="tabcontent___boot_da" style="height:auto; margin-left:15px; margin-right:15px; border:none">

                            <div id="div_respuesta_confirmar" style="margin-top: 1px; border-color: #b0c4de; border-width: 1px; border-style: ridge; background-color: #f5f5f5; display:none">
                                <asp:ImageButton ID="ImageButton_confirmar_firma" runat="server" src="../Radicador/imagenes/mas.png" Style="float: right; height: 15px; margin-right: 5px; display:none" OnClientClick="prevent_hident(event)" />
                                <asp:Label ID="label_text_confirma" runat="server" Style="font-size: 13px; text-align: center; font-family: Arial; font-weight: 600; display:none" Text="(2) Solo confirmar o darse por enterado"></asp:Label>
                                <asp:Image ID="Image_formal_visto_simple" runat="server" src="../Gestion/imagenes/visto_bueno.png" Style="height: 15px; width: 40px; margin-left: 5px; display: none; float: left" />

                            </div>
                             <br />
                            <asp:Panel ID="panel_respuesta_confirmar" runat="server" style=" ">
                            <div id="tab" style="height: auto; margin-left:15px; margin-right:15px; display:none">                
                                <ul class="tab" style="background-color: white; height: auto">
                                    <li><a style="font-family: Arial" href="javascript:void(0)" class="tablinks_" onclick="openCity_(event, 'div_confirma')" id="aopendef"><i class="fal fa-reply"></i> Confirmar</a></li>
                                    <li><a style="font-family: Arial" href="javascript:void(0)" class="tablinks_" onclick="openCity_(event, 'div_enexos_confirma')"><i class="fal fa-paperclip"></i> Anexos</a></li>
                                </ul>
                            </div>
                            <div class="row">
                                 <div class="col-6">
                                            <div class="custom-control custom-checkbox mt-3">
                                                 <asp:CheckBox ID="CheckBox_envio_correo_solo_confirmar" CssClass="checkbox" runat="server" Checked="true" Style="font-family: Arial; font-size: 15px; color: black; float:left; margin-right:15px" Text="Enviar al correo electrónico la confirmación" />
                                            </div>
                                           
                                 </div>
                                <div id="id_semaforo_confirma"  class="col-6" >
                                    <asp:UpdatePanel ID="UpdatePanel_image_semaforo_resp" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:Image ID="Image_estado_resp_solo_confirm" runat="server" Style="height: 50px; width: auto; float: right; margin-right: 30px" ImageUrl="../radicador/imagenes/resp_solo_elctronica_conf_estado_V0.png" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                                
                            </div>
                                
                            <div id="div_confirma" class="tabcontent__boot_da" style="height:auto; border:none">         
                                   
                                <div class="rows" >
                                    <div class="col-sm-12">
                                        <asp:Label ID="Label8" runat="server" CssClass="control-label" Text="Correos electrónicos a notificar" Style=""></asp:Label>
                                    </div>
                                    <div class="col-sm-12">
                                    <select class="tokenize-callable-demo_respuesta " style="width: 100%" multiple>
                                      </div>  
                                 </div>
                                <div class="rows">
                                     <asp:Label ID="Label18" runat="server" Text="Anexos de la confirmación" Style="font-family: Arial; font-size: 16px"></asp:Label>
                                </div>
                                 <div class="rows" >
                                       <div class="col-sm-12">
                                           <asp:Label ID="Label_nota_" runat="server" Text="Nota para la confirmación" Style="font-family: Arial; font-size: 14px"></asp:Label>
                                       </div>
                                        <div class="col-sm-12">
                                            <asp:TextBox ID="TextBox_nota_confirma" rows="5" CssClass="form-control" placeholder="Digita la nota de confirmación" runat="server" TextMode="MultiLine" ></asp:TextBox>
                                        </div>
                                 </div>
                                 <div class="row container_ mt-2 ml-3 mr-3" style=" background-color: #e8e8f7">
                                     <div class="col-sm-6 pt-1 pb-1 pl-2">
                                         <asp:UpdatePanel ID="UpdatePanel_anexos_respuesta_simple" UpdateMode="Conditional" runat="server" RenderMode="Inline">
                                             <ContentTemplate>
                                                 <div style="display: none">
                                                     <asp:Button ID="Button_anexo_cargar_simple" runat="server" Text="Adjuntar anexo" Style="margin-left: 5px" CssClass="btn btn-primary" />
                                                     <asp:Button ID="Button_descargar_anexo_simple" runat="server" Text="Descargar anexo" Style="margin-left: 5px" CssClass="btn btn-primary" />
                                                     <asp:Button ID="Button_anexo_eliminar_simple" runat="server" Text="Eliminar anexo" Style="margin-left: 5px" CssClass="btn btn-primary" />
                                                     <asp:Button ID="Button_sube_documento_adjunto_respuesta_simple" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                                                 </div>
                                                 <asp:DropDownList ID="DropDownList_anexos_respuesta_simple" runat="server" class="custom-select mr-sm-4 form-control" Style="margin-left: -3px; max-width: 250px"></asp:DropDownList>
                                             </ContentTemplate>
                                         </asp:UpdatePanel>
                                         <button type="button" title="Cargar anexo" value="Button_anexo_cargar_simple" class="btn  btn-warning da_event_captive mt-1"><i class="fad fa-arrow-from-bottom"></i></button>
                                         <button type="button" title="Eliminar anexo" value="Button_anexo_eliminar_simple" style="color: white" class="btn  btn-danger da_event_captive mt-1"><i class="fal fa-times"></i></button>
                                         <button type="button" title="Descargar anexo" value="Button_descargar_anexo_simple" class="btn btn-success  da_event_captive mt-1"><i class="fad fa-arrow-to-bottom"></i></button>
                                     </div>
                                     <div class="col-sm-6 pr-1 pt-1 pb-1  float-right">
                                         <asp:UpdatePanel ID="UpdatePanel_solo_confirmar" runat="server" UpdateMode="Conditional">
                                         <ContentTemplate>
                                                <asp:Button ID="Button_confirmar_solo_confirmar" runat="server"  Text="Aceptar" Style="" CssClass="btn btn-primary  float-right" ToolTip="Confirmar la solicitud" OnClientClick="asig_array_tokenize('tokenize-callable-demo_respuesta')" />
                                         </ContentTemplate>
                                       </asp:UpdatePanel>  
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
                                         
                    </ContentTemplate>
                </asp:UpdatePanel>
               <div id="div_pie" style="background-color: #b0c4de;text-align:center; display:none">
                   <asp:Label ID="label_result" runat="server" Text="" style="font-size:15px; font-family:Arial">

                   </asp:Label>
               </div>
            </asp:Panel>
            
        </div>
        
    </div>
        <asp:UpdatePanel ID="UpdatePanel_menu_var_event" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <input id="Hidden_menu_var_event_dive" type="hidden" value="" runat="server" />
                            <asp:Button ID="Button_me_active_men_dive" runat="server" Text="" Style="display: none; width: 10px;" />
                            <asp:Button ID="Button_me_retur_mem_device" runat="server" Text="" Style="display: none; width: 10px;" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
        
        <div style="display: none">
            <asp:UpdatePanel ID="UpdatePanel_descarga_hml_dowload" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <input id="Hidden_descarga_hml_dowload" type="hidden" value="1" runat="server" />
                    <div id="derecho" style="float: right; width: 1%; height: 100%; position: static; height: 150px; display: none">
                        <input id="Hidden_radicado" type="hidden" value="" runat="server" />
                        <input id="Hidden_id_respuesta" type="hidden" value="-1" runat="server" />
                        <input id="Hidden_id_propietario_resp" type="hidden" value="-1" runat="server" />
                        <input id="Hidden_obliga_rep" type="hidden" value="-1" runat="server" />
                        <input id="Hidden_tipo_resp" type="hidden" value="-1" runat="server" />
                        <input id="Hidden_estado_evento" type="hidden" value="-1" runat="server" />
                        <input id="Hidden_select_tab" type="hidden" value="" runat="server" />
                        <input id="Hidden_select_tab_" type="hidden" value="" runat="server" />
                        <input id="Hidden_text_user_correo" type="hidden" value="" runat="server" />
                        <input id="Hidden_text_user" type="hidden" value="" runat="server" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <!--modal sube documento respuesta-->
        <div style="display:none">
            <asp:UpdatePanel ID="UpdatePanel_sube_documento_respuesta" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                     <asp:Button ID="Button_sube_documento" runat="server" Text="Button" style="display:none" />
                </ContentTemplate>
            </asp:UpdatePanel>
           
        </div>
        <div id="sube_documento_respuesta">
            <asp:Panel ID="Panel_sube_documento_respuesta" runat="server" Style="display:none;  width: 50%; height: auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_sube_documento_respuesta" runat="server"  TargetControlID="ButtonSalir_sube_documento_respuesta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_sube_documento_respuesta" PopupControlID="Panel_sube_documento_respuesta"></asp:ModalPopupExtender>
                <div class="modal-content">
                    <div id="divcabecer2_sube_documento" class="modal_title_superior_ modal-header">
                        <h6 id="Label_sube_documento_respuesta" class="modal-title"></h6>
                        <button type="button" value="Button_cerrar_sube_documento_respuesta" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="contenido_procesa_sube_documento_respuesta" style="width: 100%; height: 100%; border-top:none" class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdatePane_opcion_adjunta" runat="server" UpdateMode="Conditional" Visible="true">
                            <ContentTemplate>
                                <asp:Panel ID="Panel_opcion_adjunta" runat="server">
                                    <div id="Contenido_opcion_adjunta" style="height: auto; width: 100%; background-color: white" class="content_option_selecion">
                                        <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender1" runat="server" TargetControlID="Check_adjunta_formato"
                                            Key="radicado"></asp:MutuallyExclusiveCheckBoxExtender>
                                        <asp:MutuallyExclusiveCheckBoxExtender ID="Mutuallyexclusivecheckboxextender2" runat="server" TargetControlID="CheckBox_adjunta_documento_libre"
                                            Key="radicado"></asp:MutuallyExclusiveCheckBoxExtender>
                                        <asp:CheckBox ID="Check_adjunta_formato" runat="server" Text="Adjunta documento formato respuesta" Checked="false" ForeColor="Black" Font-Size="10" Font-Names="Arial" Style="margin-left: 5px" AutoPostBack="true" />
                                        <asp:CheckBox ID="CheckBox_adjunta_documento_libre" runat="server" Text="Adjunta documento formato libre " Checked="true" ForeColor="Black" Font-Size="10" Font-Names="Arial" Style="margin-left: 5px" AutoPostBack="true" />
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

                        <asp:UpdatePanel ID="UpdatePanel_Panel_descarga_ajax" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="Panel_descarga_ajax" runat="server">
                                    <div id="drop_zone_2" style="width: 100%; height: auto; overflow: auto">
                                        <asp:UpdatePanel ID="UpdatePanel_descarga" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:AjaxFileUpload ID="AjaxFileUpload_dowload" runat="server" ThrobberID="drop_zone_2"
                                                    ContextKeys="fred_1"
                                                    AllowedFileTypes="docx"
                                                    MaximumNumberOfFiles="1" OnClientUploadComplete="activa_boton_dowload" />
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="Panel_descarga_externo" runat="server">
                                </asp:Panel>
                                &nbsp  
                               <asp:Label ID="Label_estado_carga" runat="server" Text="Estado" Style="font-family: Arial; font-size: 10px; color: red"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button_cerrar_sube_documento_respuesta" runat="Server" Text="" CssClass="invisible" />
                            <asp:Button ID="ButtonSalir_sube_documento_respuesta" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                        </div>

                    </div>
                    <div class="modal-footer">

                    </div>
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
                                                            <asp:CalendarExtender ID="TextBoxFECHA_EXTREMA_INICIAL_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_EXTREMA_INICIAL_CalendarExtender" TargetControlID="TextBox_fecha_limite_solicitud" Format='yyyy-MM-dd' PopupButtonID="A1" />
                                                            <a id="A1" class="" style="" title="Examinar el calendario" href="#"><i style="margin-left: 1px" class="fal fa-calendar-alt fa-2x"></i></a>
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
                                                <asp:Button ID="Button_registrar_solicitud_aprobacion" runat="server" Text="Aceptar" CssClass="btn  btn-success" Style="float: right; margin-bottom: 10px; margin-top: 10px; margin-right: 5px" OnClientClick="Solicitud_aprobacion_tokenize();" />
                                                <asp:Button ID="Button_cancelar_registro" runat="server" Text="Cancelar" CssClass="btn btn-default" Style="float: right; margin-bottom: 10px; margin-top: 10px; margin-right: 5px;" />
                                                <input id="Hidden_resultado_actualizar" runat="server" type="hidden" value=""/>
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
        <!--modal notifica respuesta formal sin funcionamiento-->
        <div id="notifica_respuesta_gestion_formal">
            <asp:Panel ID="Panel_gestion_formal" runat="server" Style="display:none; color: White; width: 100%; height: 100%">

                <asp:ModalPopupExtender ID="ModalPopupExtender_gestion_formal" runat="server" BehaviorID="Panel_gestion_formal_ModalPopupExtender" TargetControlID="ButtonSalir_gestion_formal"
                    CancelControlID="Button_cerrar_gestion_formal" PopupControlID="Panel_gestion_formal"  y="5">
                </asp:ModalPopupExtender>
                <div id="divcabecer2_gestion_formal" class="cabecera2">
                    <asp:Button ID="Button_gestion_formal" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_gestion_formal" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Label ID="Label_gestion_formal" runat="server" Text="Gestión respuesta formal" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_gestion_formal" style="float: right">
                        <asp:Button ID="Button_cerrar_gestion_formal" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_gestion_formal" style="background-color:white; width:100%; height: 100%">
                    <asp:UpdatePanel ID="UpdatePanel_gestion_formal" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <iframe style="color: White; width: 100%; background-color:white; height: 100%; overflow:hidden" id="ifimpre" runat="server"  ></iframe>                      
                        </ContentTemplate>
                    </asp:UpdatePanel>         
                </div>
            </asp:Panel>
        </div>
        <!--modal solicitud aprobación documentos-->
        <div id="solicitud_aprobacion">
            <asp:Panel ID="Panel_solicitud_aprobacion" runat="server" Style="display:none; color: White; width: 90%; height: 90%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_solicitud_aprobacion" runat="server"  TargetControlID="ButtonSalir_solicitud_aprobacion" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_solicitud_aprobacion" PopupControlID="Panel_solicitud_aprobacion"  >
                </asp:ModalPopupExtender>
                <div id="divcabecer2_solicitud_aprobacion" class="modal_title_superior">  
                     <h6 class="modal-title ml-2 d-inline">Solicitudes de aprobación</h6>         
                     <button type="button" value="Button_cerrar_solicitud_aprobacion" class="close da_event_captive mr-2">&times;</button>
                </div>
                <div id="contenido_procesa_solicitud_aprobacion" style="background-color:white; width:100%; height: 100%" class="modal_content_back" >
                    <asp:UpdatePanel ID="UpdatePanel_solicitud_aprobacion" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <iframe style="color: White; width: 100%;  background-color:white; height: 100%; overflow:hidden" frameborder="0" id="Iframe_solicitud_aprobacion" runat="server"  ></iframe>                          
                        </ContentTemplate>
                    </asp:UpdatePanel>
                     <div style="display:none; height:1px">
                           <asp:Button ID="Button_solicitud_aprobacion" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                           <asp:Button ID="ButtonSalir_solicitud_aprobacion" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                           <asp:Button ID="Button_cerrar_solicitud_aprobacion" runat="Server" Text="X" CssClass="invisible" />
                     </div>
                   
                </div>
            </asp:Panel>
        </div>
       <!--Reversa respuesta-->
        <div id="reversa_respuesta">
            <asp:Panel ID="Panel_reversa_respuesta" runat="server" Style="display:none;  width:auto; height:auto" CssClass="modal_content_general" >
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_reversa_respuesta" runat="server"  TargetControlID="ButtonSalir_reversa_respuesta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_reversa_respuesta" PopupControlID="Panel_reversa_respuesta" ></asp:ModalPopupExtender>
                <div class="modal-content">
                    <div id="div10" class="modal_title_superior_ modal-header">
                        <h6 class="modal-title">Autorización</h6>
                        <button type="button" value="Button_cerrar_reversa_respuesta" class="close da_event_captive">&times;</button>
                    </div>
                    <div id="contenido_procesa_reversa_respuesta" style="background-color: white; width: 100%; height: 99%" class="modal_content_back_ modal-body">
                        <div class=" col-12">
                            <span>Usuario autorizado*
                            </span>
                        </div> 
                        <div  class=" col-6">
                            <asp:TextBox ID="TextBox_login_usuario_val" runat="server" Style="width: 300px"  CssClass="form-control"></asp:TextBox>
                        </div>   
                        <div class=" col-12">
                              <span>Contraseña usuario*
                            </span>
                        </div>    
                        <div  class=" col-6">
                              <asp:TextBox ID="TextBox_pasw_usuario_val" runat="server" Style="width: 300px" TextMode="Password"></asp:TextBox>
                        </div> 
                        <div style="display: none; height: 1px">
                            <asp:Button ID="Button_cerrar_reversa_respuesta" runat="Server" Text="" CssClass="invisible" />
                            <asp:Button ID="Button_reversa_respuesta" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                            <asp:Button ID="ButtonSalir_reversa_respuesta" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                        </div>

                    </div>
                     <div class="modal-footer">
                          <asp:UpdatePanel ID="UpdatePanel_contenido_radica_documento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>  
                                <asp:Button ID="Button_reversar" runat="server" Text="Aceptar" Style="float: right; margin-right: 5px; margin-top: 10px; margin-bottom: 10px" CssClass="btn btn-success" />
                                <asp:Button ID="Button__reversar_cancelar" runat="server" Text="Cancelar" Style="float: right; margin-right: 5px; margin-top: 10px; margin-bottom: 10px" CssClass="btn btn-light" />
                                <input id="Hidden_user_rever" type="hidden" value="" runat="server"/>
                            </ContentTemplate>
                        </asp:UpdatePanel>
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
                     <div id="div8" class="modal_title_superior_ modal-header">
                         
                         <button type="button" onclick="activa_boton_client_server('Button_cerrar_confirma_reversa_respuesta');" class="close">&times;</button>
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
                          <asp:UpdatePanel ID="UpdatePanel_confirma_reversa" runat="server" UpdateMode="Conditional">
                             <ContentTemplate>
                                 <asp:Button ID="Button_confirma_reversar" runat="server" Text="Aceptar" Style="float: right; margin-right: 5px; margin-top: 10px; margin-bottom: 10px" CssClass="btn btn-success" />
                                 <asp:Button ID="Button_cancel_confirma_reversar" runat="server" Text="Cancelar" Style="float: right; margin-right: 5px; margin-top: 10px; margin-bottom: 10px" CssClass="btn btn-light" />
                                 <input id="Hidden_con_ref" type="hidden" value="" runat="server"/>
                             </ContentTemplate>
                         </asp:UpdatePanel>
                     </div>
                 </div>
              
            </asp:Panel>
           
        </div>
       <div id="asigna_dest_externo">
            <asp:Panel ID="Panel_asigna_dest_externo" runat="server" Style="display:none; width:35%; height:auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_asigna_dest_externo" runat="server" BehaviorID="Panel_asigna_dest_externo_ModalPopupExtender" TargetControlID="ButtonSalir_asigna_dest_externo" BackgroundCssClass="FondoAplicacion"
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
        <!--detalle transacciones-->
        <asp:Panel ID="Panel_transacciones" runat="server" Style="display:none; overflow: hidden; width: 90%; height: 100%" CssClass="modal_content_general">
            <asp:ModalPopupExtender ID="ModalPopupExtender_transacciones" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_transacciones_dos"
                PopupControlID="Panel_transacciones" CancelControlID="ButtonSalir_transacciones">
            </asp:ModalPopupExtender>
            <div id="Cabecerapendiente_transacciones" class="modal_title_superior">
                <h6 class="modal-title d-inline mt-1 mb-1 ml-2">Detalle de transacciones</h6>
                <button type="button" value="ButtonSalir_transacciones" class="close da_event_captive mr-2 mt-1 mb-1">&times;</button>
            </div>
            <div id="Cotenedorpendiente_transacciones" style="height: 90%; width: 100%; overflow: hidden" class="modal_content_back">

                <asp:UpdatePanel ID="UpdatePanel_transacciones" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <iframe id="Iframe_transacciones_" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: hidden"></iframe>
                    </ContentTemplate>

                </asp:UpdatePanel>

            </div>
            <div style="display:none; height:1px">
                 <asp:Button ID="Button_transacciones_dos" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                  <asp:Button ID="ButtonSalir_transacciones" runat="Server" Text=""  CssClass="invisible" />
            </div>             
        </asp:Panel>

        <asp:Panel ID="Panel_detalle_respuesta" runat="server" Style="display:none; width:95%; overflow:hidden; height:auto" ForeColor="White"   CssClass="modal_content_general" >
                  <asp:ModalPopupExtender ID="ModalPopupExtender_detalle_respuesta" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_detalle_respuesta"
                      PopupControlID="Panel_detalle_respuesta"  CancelControlID="ButtonSalir_detalle_respuesta">
                  </asp:ModalPopupExtender>
                  <div id="Cabecerapendiente_detalle_respuesta" class="modal_title_superior">   
                        <button type="button" value="ButtonSalir_detalle_respuesta"  class="close da_event_captive mr-2">&times;</button>
                  </div>
                  <div id="Cotenedorpendiente_detalle_respuesta" style="color: Black;  height: 99%; width: 100%; overflow:hidden" class="modal_content_back">
                  
                      <asp:UpdatePanel ID="UpdatePanel_detalle_respuesta" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                              <iframe id="Iframe_visor_externo_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>
                          </ContentTemplate>

                      </asp:UpdatePanel>
                           
                  </div>
                  <div style="display:none; height:1px">
                      <asp:Button ID="Button_detalle_respuesta" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                      <asp:Button ID="ButtonSalir_detalle_respuesta" runat="Server" Text="" CssClass="invisible"/> 
                 </div>
                  
              </asp:Panel>
        
     <asp:Panel ID="Panel_nota_respuesta" runat="server" Style="display:none; overflow: hidden; width:50%; height:auto" CssClass="modal_content_general" >
            <asp:ModalPopupExtender ID="ModalPopupExtender_nota_respuesta" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_nota_respuesta"
                PopupControlID="Panel_nota_respuesta"  CancelControlID="ButtonSalir_nota_respuesta"></asp:ModalPopupExtender>
            <div id="Cabecerapendiente_nota_respuesta" class="modal_title_superior">    
                <asp:Label ID="Label_not" runat="server" Text="No de la confirmación"></asp:Label>
                <div id="Div_nota_respuesta" style="float: right">
                    <asp:Button ID="ButtonSalir_nota_respuesta" runat="Server" Text="X" ToolTip="Cerrar ventana" CssClass="modal_boton_hiden"
                          />

                </div>
            </div>
            <div id="Cotenedorpendiente_nota_respuesta" style=" height: 100%; width: 100%; overflow: hidden" class="modal_content_back">         
                    <div style="margin-top:10px; margin-left:10px; margin-right:10px; margin-bottom:10px">
                         <asp:UpdatePanel ID="UpdatePanel_nota_respuesta" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:TextBox ID="TextBox_NOTA_RESPUESTA" runat="server" CssClass="form-control" Style="font-family: arial; font-size: 12px; width:100%; margin-right: 7px; height:100%; " TextMode="MultiLine"  CausesValidation="False" Rows="10"></asp:TextBox>
                        </ContentTemplate>

                    </asp:UpdatePanel>
                    <asp:UpdatePanel ID="UpdatePanel_botones_registro" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>

                            <asp:Panel ID="Panel_botones" runat="server">
                                <input id="Hidden_estado_nota" type="hidden" value="" runat="server"/>
                                <asp:Button ID="Button_guardar" Style="margin-top:10px; float:right; margin-right:5px; margin-bottom:10px" runat="server" Text="Aceptar" ToolTip="Actualizar o guadar nota" CssClass="btn btn-primary" />

                            </asp:Panel>
                            
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    </div>
                    <div style="display:none; height:1px">
                         
                         <asp:Button ID="Button_nota_respuesta" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    </div>
                   
            </div>

        </asp:Panel>
         <!--Notifca respuesta al correo electronico-->
        <div id="notifica_correo_respuesta">
            <asp:Panel ID="Panel_notifica_correo_respuesta" runat="server" Style="display:none;  width:50%; height:auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_notifica_correo_respuesta" runat="server" BehaviorID="Panel_notifica_correo_respuesta_ModalPopupExtender" TargetControlID="ButtonSalir_notifica_correo_respuesta" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_notifica_correo_respuesta" PopupControlID="Panel_notifica_correo_respuesta" ></asp:ModalPopupExtender>
                
                    <div id="div1" class="modal_title_superior_ modal-header">
                           <h6 class="modal-title">Notifica respuesta al correo electrónico</h6>
                           <button type="button" value="Button_cerrar_notifica_correo_respuesta" class="close da_event_captive">&times;</button>
                      
                    </div>
                    <div id="contenido_procesa_notifica_correo_respuesta" style="background-color: white; width: 100%; height: auto ; border-top:none" class="modal_content_back modal-body">
                        <div style="margin-left: 10px; margin-right: 10px">
                            <div class="row" style="margin-top: 10px">
                                <div class="col-sm-2">
                                    <asp:Label ID="Label2" runat="server" Text="Correos electrónicos*" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label>
                                </div>
                                <div class="col-sm-10">
                                    <select class="tokenize-callable-demo_respuesta__" multiple style="width: 98%;">
                                    </select>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-12">
                                    <asp:CheckBox ID="CheckBox_anexa_anexos" runat="server" Checked="true" Text="Adjunta al correo los anexos de la respuesta" Style="font-family: Arial; font-size: 12px; margin-left: 5px" />
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
                          <asp:UpdatePanel ID="UpdatePanel_notifica_correo" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:Button ID="Button_notificar_correo" runat="server" Text="Notificar al correo" Style="margin: 2px; float: right; margin-right: 5px" CssClass="btn  btn-success" OnClientClick="asig_array_tokenize('tokenize-callable-demo_respuesta__');" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                          </div>
                
            </asp:Panel>
        </div>
        <div id="notifica_respuesta_edition_html">
            <asp:Panel ID="Panel_edition_html" runat="server" Style="display:none; color: White; width: 90%; height: 90%">

                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_html" runat="server" BehaviorID="Panel_edition_html_ModalPopupExtender" TargetControlID="ButtonSalir_edition_html"
                    CancelControlID="Button_cerrar_edition_html" PopupControlID="Panel_edition_html" Y="1"></asp:ModalPopupExtender>
                <div id="divcabecer2_edition_html" class="cabecera2">
                    
                    
                    <div id="Divcerrarbuton2_edition_html" style="float: right">
                
                        <asp:Button ID="Button_cerrar_edition_html" runat="Server" Text="X"
                            ForeColor="#000066" Height="19px" ToolTip="Cerrar ventana" />
                    </div>
                </div>
               
                <div id="contenido_procesa_edition_html" style="background-color: white; width: 100%; height: 100%">   
                          
                          <asp:UpdatePanel ID="UpdatePanel_html_editor" runat="server"  UpdateMode="Conditional">
                              <ContentTemplate>
                                  <iframe runat="server" id="ifm_html_editor" style="width:100%; height:100%" ></iframe>
                              </ContentTemplate>
                          </asp:UpdatePanel>
                    
               </div>
                
               <div style="display:none">
                      <asp:Button ID="Button_edition_html" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                    <asp:Button ID="ButtonSalir_edition_html" CssClass="invisible" runat="server" Text="Button" Height="0px" Width="0px" />
                   
               </div>
            </asp:Panel>
        </div>
       <!--Radica documento respuesta no funcional-->
        <div id="radica_documento_respuesta">
            <asp:Panel ID="Panel_radica_documento_respuesta" runat="server" Style="display:none; color: White; width: 800px; height: 300px">

                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_radica_documento_respuesta" runat="server" BehaviorID="Panel_radica_documento_respuesta_ModalPopupExtender" TargetControlID="ButtonSalir_radica_documento_respuesta" BackgroundCssClass="FondoAplicacion"
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
                                            <asp:Label ID="Label9" runat="server" Text="Tramite*" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label>

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
                                            <asp:Label ID="Label11" runat="server" Text="Destinatario*" Style="text-align: center; font-family: Arial; font-size: 14px"></asp:Label>

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
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_confirma_envio_respuesta" runat="server" BehaviorID="Panel_confirma_envio_respuesta_ModalPopupExtender" TargetControlID="ButtonSalir_confirma_envio_respuesta" BackgroundCssClass="FondoAplicacion"
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
                                <h6>Correos electrónicos</h6>
                                <select class="tokenize-callable-demo_respuesta_ form-control" multiple>
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
                        <asp:UpdatePanel ID="update_boton_confirma" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Button ID="Button_cancel_envio_respuesta" runat="server" Text="Cancelar"  CssClass="btn btn-default" />
                                <asp:Button ID="Button_confirmar_envio_respuesta" runat="server" Text="Aceptar"   CssClass="btn  btn-success mr-1" ToolTip="Confirmar y enviar el documento respuesta" OnClientClick="asig_array_tokenize('tokenize-callable-demo_respuesta_');" />             
                                <input id="Hidden_resultado_envio" type="hidden" value="" runat="server"/>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </asp:Panel>
        </div>
         <!--opcion_descarga_respuesta!-->
         <div id="opcion_descarga_respuesta">
            <asp:Panel ID="Panel_opcion_descarga_respuesta" runat="server"  Style="display:none;  height:auto; width:40%" CssClass="modal_content_general">              
                <asp:ModalPopupExtender ID="ModalPopupExtender_opcion_descarga_respuesta" runat="Server" BackgroundCssClass="FondoAplicacion" 
                     TargetControlID="ButtonSalir_opcion_descarga_respuesta"
                    PopupControlID="Panel_opcion_descarga_respuesta" CancelControlID="Buttoncerrarimpre_opcion_descarga_respuesta">
                </asp:ModalPopupExtender>
                
                    <div id="Divcerrarbuton2_opcion_descarga_respuesta"  class="modal_title_superior_ modal-header">             
                           <button type="button" value="Buttoncerrarimpre_opcion_descarga_respuesta" class="close da_event_captive">&times;</button>
                    </div>
                
                   
                        <div id="Contenido_opcion_descarga_respuesta" style="height:auto; width: 100%; color:black; border-top:none" class="modal_content_back modal-body">
                           
                            <div id="div_title_opcion_descarga_respuesta"  class="mt-1 " >
                                <h5>Opciones para descargar el documento</h5>            
                            </div>
                             
                            <div id="div_opcion_descarga_respuesta" class="border mt-3 mb-3"  >
                                <asp:CheckBox ID="CheckBox_opcion_descarga_respuesta_con_firma" runat="server" Text="Guardar documento con firma " Checked="true" ForeColor="Black" Font-Size="10" Font-Names="Arial" Style="margin-left: 5px" Enabled="true" />
                                <asp:CheckBox ID="Check_opcion_descarga_respuesta_sin_firma" runat="server" Text="Guardar documento sin firma" Checked="false" ForeColor="Black" Font-Size="10" Font-Names="Arial" Style="margin-left: 5px; display:block" Enabled="true" />                
                            </div>          
                            <div  class="mr-3">
                                <span>Formato de descarga</span>
                               
                                <asp:DropDownList ID="DropDownList_tipo_archivo" runat="server"  style="width:auto" CssClass="form-control">
                                    <asp:ListItem Selected="True">PDF</asp:ListItem>
                                    <asp:ListItem>DOCX</asp:ListItem>
                                </asp:DropDownList>
                                
                            </div>
                                            
                           
                        </div>
                <div class="modal-footer">
                      <asp:UpdatePanel ID="UpdatePane_opcion_descarga_respuesta" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                         <asp:Button ID="Button_descarga_docmento_respuesta" runat="server" Text="Descargar"  CssClass="btn btn-success" Style="margin-right:10px; margin-bottom:15px; float:right" />
                              <asp:Button ID="Button_cancelar_documento_respuesta" runat="server" Text="Cancelar"  CssClass="btn btn-default" Style="margin-right:10px; margin-bottom:15px; float:right" />
                              <asp:HiddenField ID="HiddenField_descarga_docmento_respuesta" runat="server" Value="" />
                         </ContentTemplate>
                </asp:UpdatePanel>  
                </div>
                   
                <div style="display: none; height: 0px">
                    <asp:Button ID="ButtonSalir_opcion_descarga_respuesta" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                    <asp:Button ID="Buttoncerrarimpre_opcion_descarga_respuesta" runat="Server" Text=""
                        CssClass="invisible" />
                       <asp:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender3" runat="server" TargetControlID="Check_opcion_descarga_respuesta_sin_firma"
                            Key="radicado"></asp:MutuallyExclusiveCheckBoxExtender>
                        <asp:mutuallyexclusivecheckboxextender id="Mutuallyexclusivecheckboxextender4" runat="server" targetcontrolid="CheckBox_opcion_descarga_respuesta_con_firma"
                            key="radicado"></asp:mutuallyexclusivecheckboxextender>
                </div>
                   
            </asp:Panel>
        </div>
        <!--Descarga plantilla radicado-->
        <div id="descarga_plantilla_radicada">
            <asp:Panel ID="Panel_descarga_plantilla_radicada" runat="server" Style="display:none;  min-width:400px; height:auto" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_descarga_plantilla_radicada" runat="server"  TargetControlID="ButtonSalir_descarga_plantilla_radicada" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_descarga_plantilla_radicada" PopupControlID="Panel_descarga_plantilla_radicada" ></asp:ModalPopupExtender>
                <div class="modal-content">
                    <div id="div5" class="modal_title_superior_ modal-header" >
                            <h6 class="modal-title">Descargar protocolo de respuesta</h6>
                          <button type="button" value="Button_cerrar_descarga_plantilla_radicada"  class="close da_event_captive">&times;</button>            
                    </div>
                    <div id="contenido_procesa_descarga_plantilla_radicada" style="width: 100%; height: 99%; color: black; border-top:none" class="modal_content_back modal-body">
                        <asp:UpdatePanel ID="UpdatePanel_descarga_plantilla_radicada" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div style="margin-left: 15px; margin-right: 15px; margin-bottom: 15px; margin-top: 5px">
                                    <div class="form-group-sm">
                                        <asp:Label ID="Label20" runat="server" Style="text-align: center; font-family: Arial; font-size: 14px" Text="Tramite a responder*"></asp:Label>
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
                                    <div style="display:none; height:1px">
                                         <asp:Button ID="Button_descarga_plantilla_radicado" runat="server"  Style="width:1px; height:1px; display:none"  />
                                    </div>
                                   
                                    
                                </div>
                            </ContentTemplate>

                        </asp:UpdatePanel>
                         <div class="modal-footer align-content-end">
                             <button type="button" value="Button_cerrar_descarga_plantilla_radicada" class="btn btn-light da_event_captive"> Cancelar </button>
                             <button type="button" value="Button_descarga_plantilla_radicado" class="btn btn-success da_event_captive"> Aceptar </button>
                         </div>
                        <div style="display:none; height:1px">
                             <asp:Button ID="Button_descarga_plantilla_radicada" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />
                             <asp:Button ID="ButtonSalir_descarga_plantilla_radicada" CssClass="invisible" runat="server" Text="Button" Height="1px" Width="1px" />         
                             <asp:Button ID="Button_cerrar_descarga_plantilla_radicada" runat="Server" Text="X" CssClass="invisible"/>
                        </div>      
                    </div>
                </div>

            </asp:Panel>
        </div>
         <!--detalle descarga_formato-->
           <asp:Panel ID="Panel_descarga_formato" runat="server" Style="display:none; overflow:hidden; width:50%; height:auto"  CssClass="modal_content_general" >
                  <asp:ModalPopupExtender ID="ModalPopupExtender_descarga_formato" runat="Server" BackgroundCssClass="FondoAplicacion" TargetControlID="Button_descarga_formato_dos"
                      PopupControlID="Panel_descarga_formato"  CancelControlID="ButtonSalir_descarga_formato"></asp:ModalPopupExtender>
                  <div id="Cabecerapendiente_descarga_formato" class="modal_title_superior">                     
                      <asp:Label ID="Label_desc_" runat="server" Text="Descarga formato respuesta"  Style=""></asp:Label>
                      <div id="Div_descarga_formato" style="float: right">
                          <asp:Button ID="ButtonSalir_descarga_formato" runat="Server" Text="X" CssClass="modal_boton_hiden"
                               ToolTip="Cerrar ventana descarga formato"  />

                      </div>
                  </div>
                  <div id="Cotenedorpendiente_descarga_formato" style="height: 100%; width: 100%" class="modal_content_back">  
                      <div style="margin:15px">
                             <asp:UpdatePanel ID="UpdatePanel_descarga_formato" runat="server" UpdateMode="Conditional">
                          <ContentTemplate>
                             
                              <asp:Label ID="Label_title_descarga" runat="server" Text="Selecciona el usuario que firma el formato de respuesta" style="font-family:Arial; font-size:12px; margin-left:5px"></asp:Label> <br />
                              <asp:DropDownList ID="DropDownList_lista_firmas" runat="server" style="" CssClass="form-control"></asp:DropDownList>
                               
                          </ContentTemplate>
                         
                      </asp:UpdatePanel>
                      <br />
                       <asp:UpdatePanel ID="UpdatePanel_boton_descarga_formato" runat="server" UpdateMode="Conditional">
                               <ContentTemplate>
                                   <asp:Button ID="Button_descarga_plantilla" runat="server" Text="Aceptar" Style="float:right; margin-right:5px; margin-bottom:10px" CssClass="btn btn-primary" />
                                   <asp:Button ID="Button_cancela_descarga_plantilla" runat="server" Text="Cancelar" Style="float:right; margin-right:5px; margin-bottom:10px; margin-right:5px" CssClass="btn btn-default" />
                                   
                              </ContentTemplate>
                       </asp:UpdatePanel>   
                      </div>        
                      
                  </div>
                   <asp:Button ID="Button_descarga_formato_dos" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px"  />
              </asp:Panel>
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

     <div id="inferior_bajo_boton" style="width: 100%; height: 20%; background-color: #E7EDF5; display: none">
         <asp:UpdatePanel ID="updatapanel_iframe" runat="server" UpdateMode="Conditional">
             <ContentTemplate>

                 <asp:Label ID="Label17" runat="server" Text="Estado" Style="font-size: 8px; font-family: Arial; float: right"></asp:Label>
                 <iframe runat="server" id="ifmExcel_" width="0" height="0" marginheight="0" marginwidth="0"
                     frameborder="0" />
                 <input id="Hidden_ruta_archivo" type="hidden" value="" runat="server" />
             </ContentTemplate>

         </asp:UpdatePanel>
     </div>
         <!--compartir documento-->
          <div id="autoriza_compartir_documento">
            <asp:Panel ID="Panel_autoriza_compartir_documento" runat="server" Style="display:none; color: White; width: 100%; height: 100%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_autoriza_compartir_documento" runat="server" BehaviorID="Panel_autoriza_compartir_documento" TargetControlID="ButtonSalir_autoriza_compartir_documento" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_autoriza_compartir_documento" PopupControlID="Panel_autoriza_compartir_documento" ></asp:ModalPopupExtender>
                <div id="divcabecer2_autoriza_compartir_documento"  class="modal_title_superior">
                    
                    <asp:Label ID="Label_autoriza_compartir_documento" runat="server" Text="Compartir documento" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_autoriza_compartir_documento" style="float: right">
                        <asp:Button ID="Button_cerrar_autoriza_compartir_documento" runat="Server" Text="X" CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_autoriza_compartir_documento" style="background-color: white; width: 100%; height: 99%">
                                
                    
                        <asp:UpdatePanel ID="UpdatePanel_autoriza_compartir_documento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                              <iframe id="Iframe_compartir_documento_" runat="server" frameborder="0"  style="width:100%; height:100%; overflow:hidden"></iframe>                           
                            </ContentTemplate>
                        </asp:UpdatePanel>
                         
                </div>
                <asp:Button ID="Button_autoriza_compartir_documento" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                    <asp:Button ID="ButtonSalir_autoriza_compartir_documento" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
        </div>
       
        <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
                <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
                Processing ...
            </div>
        <div id="nove" style="display:none" >
            <asp:UpdatePanel ID="UpdatePanel_actualiza_guardar_documento" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button ID="Button_antualiza_semaforo_chkeditor" runat="server" Text="Button" />

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <!--registro_colaboracion-->
        <div id="registro_colaboracion">
            <asp:Panel ID="Panel_registro_colaboracion" runat="server" Style="display: none; color: White; width: 100%; height: 100%" CssClass="modal_content_general">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_registro_colaboracion" runat="server" BehaviorID="Panel_registro_colaboracion" TargetControlID="ButtonSalir_registro_colaboracion" BackgroundCssClass="FondoAplicacion"
                    CancelControlID="Button_cerrar_registro_colaboracion" PopupControlID="Panel_registro_colaboracion">
                </asp:ModalPopupExtender>
                <div id="divcabecer2_registro_colaboracion" class="modal_title_superior">
                    <asp:Label ID="Label_registro_colaboracion" runat="server" Text="Registros de colaboración" Font-Size="10" Style="float: left">
                    </asp:Label>
                    <div id="Divcerrarbuton2_registro_colaboracion" style="float: right">
                        <asp:Button ID="Button_cerrar_registro_colaboracion" runat="Server" Text="X"  CssClass="modal_boton_hiden"
                             ToolTip="Cerrar ventana" />
                    </div>
                </div>
                <div id="contenido_procesa_registro_colaboracion" style="background-color: white; width: 100%; height: 100%" >
                    <asp:UpdatePanel ID="UpdatePanel_registro_colaboracion" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <iframe id="Iframe_registro_colaboracion_" runat="server" frameborder="0" style="width: 100%; height: 100%; overflow: hidden"></iframe>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
                <asp:Button ID="Button_registro_colaboracion" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
                <asp:Button ID="ButtonSalir_registro_colaboracion" CssClass="invisible" runat="server" Text="Button" Height="20px" Width="20px" />
            </asp:Panel>
        </div>
         <div id="myModal" class="modal-ayuda" style="display:none">

            <!-- Modal content -->
            <div id="mytexto_" class="modal-content-ayuda" style="overflow:auto">
                <span  class="close" onclick="hide_autonomo();"> &times; 
                    ontent:center; font-family:Arial; font-size:11px" >Some text in the Modal..2 in the Modal..content-ayuda" style="overflow:auto">
                <span  class="close" onclick="hide_autonomo();"> &times; 
                    ontent:center; font-family:Arial; font-size:11px" >Some text in the Modal..2 in the Modal..</p>
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
        <!--Termina mensaje_personalizado-->
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

          </script>
   
</body>
   
</html>
