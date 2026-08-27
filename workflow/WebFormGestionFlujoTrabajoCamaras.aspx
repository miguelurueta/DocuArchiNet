<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebFormGestionFlujoTrabajoCamaras.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebFormGestionFlujoTrabajoCamaras" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
  <title>Administración flujos camara</title>
  <script src="../js/ui/jquery-3.4.1.min.js"></script>  
  <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
  <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
  <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
  <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
  <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
  <link href="../Styles/Aplicaction.css" rel="stylesheet" />
  <link href="../Styles/tabccs.css" rel="stylesheet" />
  <link href="../Styles/style.css" rel="stylesheet" />
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.11.0/umd/popper.min.js"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../bootstrap/table/dist/bootstrap-table.min.css" rel="stylesheet" />
    <script src="../bootstrap/table/dist/bootstrap-table.min.js"></script>
    <script src="../bootstrap/table/dist/bootstrap-table-locale-all.js"></script>   
     <script src="../bootstrap/table/dist/extensions/export/bootstrap-table-export.min.js"></script>
    <script src="../bootstrap/table/dist/extensions/export/bootstrap-table-export.js"></script>     
    <script src="../js/table_boo/table_boot_config.js"></script>
    <script src="../js/java_general/BootstrapTable.js"></script>
    <script src="https://unpkg.com/tableexport.jquery.plugin/tableExport.min.js"></script>
    <script  src="../Awesome/js/all.js"></script>    
    <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
    <link href="../Awesome/css/brands.css" rel="stylesheet"/>
    <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
    <script  src="../Awesome/js/solid.js"></script>
    <script  src="../Awesome/js/fontawesome.js"></script>
    <script src="../js/workflow/WebFormGestionFlujoTrabajoCamaras.js"></script>
    <script src="../js/java_general/general_code_java.js?v=20260827-compatible-events5"></script>
    <script src="../js/java_general/general_config.js"></script>
    <script src="../js/java_general/general_control_java.js"></script>
    <script src="../generic_control/FileUploadHandler.js" type="text/javascript"></script>
    <link href="../generic_control/UploadFile.css" rel="stylesheet" />
    <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/styleMenu.css" rel="stylesheet" type="text/css" /> 
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/Menu3.css" rel="stylesheet" />
    <style type="text/css">
        .auto-style1 {
            height: 29px;
        }
    </style>
</head>
<body style="background-color: #b8b9bc; width:100%">
    <form id="form1" style="background-color: #b8b9bc; width:100%" runat="server">
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
                }
                catch (err) {
                    alert(" Funcion CheckStatus asincrona workflow.aspx" + err.message);
                }
                finally {
                    progres_hiden('progres_bar');
                }
            }
            </script>
        <div id="pag_plantilla" style="text-align: left; width: 100%; background-color: #FAFAFA; border-radius: 1px; background: white; height: 100%; margin-top: 1px">  
            <div class="pt-3" id="tab_option_integracion">
                <ul class="tab" style="background-color: white">
                    <li id="util_sii_registro_tarea_flujo" class="p-1 d-none" style="border-radius: 50px 20px;"><a href="javascript:void(0)" style="border-radius: 50px 20px;color: #678098" class="tablinks" onclick="openCity(event, 'registro_flujo')" id="defaultOpen"><i class="fas fa-project-diagram"></i> Registrar tarea para flujo</a> </li>
                    <li id="util_sii_registro_tarea_ruta" class="p-1 d-none" style="border-radius: 50px 20px;"><a href="javascript:void(0)" style="border-radius: 50px 20px;color: #678098" class="tablinks" onclick="openCity(event, 'registro_ruta')"><i class="fad fa-project-diagram"></i> Registrar tarea para ruta</a></li>
                    <li id="util_sii_getion_tarea" class="p-1 d-none" style="border-radius: 50px 20px;"><a href="javascript:void(0)" style="border-radius: 50px 20px;color: #678098" class="tablinks" onclick="openCity(event, 'elimina_flujo')"><i class="fad fa-trash"></i> Eliminar tarea workflow</a></li>
                    <li id="util_sii_getion_tarea_"class="p-1 d-none" style="border-radius: 50px 20px;"><a href="javascript:void(0)" style="border-radius: 50px 20px;color: #678098" class="tablinks" onclick="openCity(event, 'edita_flujo')"><i class="fas fa-wrench"></i> Actualizar documento tarea</a></li>
                    <li id="util_reasigna_tarea_workflow_sii"class="p-1 d-none" style="border-radius: 50px 20px;"><a href="javascript:void(0)" style="border-radius: 50px 20px;color: #678098" class="tablinks" onclick="openCity(event, 'tab_reasigna_sii')"><i class="fas fa-user-check"></i> Reasignar tarea workflow</a></li>
                    <li id="util_gestion_reasing_user" class="p-1 d-none" style="border-radius: 50px 20px;"><a href="javascript:void(0)" style="border-radius: 50px 20px;color: #678098" class="tablinks" onclick="openCity(event, 'tab_gestion_balance')"><i class="fas fa-user-cog"></i> Gestión de usuarios</a></li>
                    <li id="util_sii_gestion_tarea_rue" class="p-1 d-none" style="border-radius: 50px 20px;"><a href="javascript:void(0)" style="border-radius: 50px 20px;color: #678098" class="tablinks" onclick="openCity(event, 'tab_gestion_rue')"><i class="fad fa-cogs"></i> Gestión de trámites rues</a></li>
                    <li id="util_sii_gestion_tarea_virtual"class="p-1 d-none" style="border-radius: 50px 20px;"><a href="javascript:void(0)" style="border-radius: 50px 20px;color: #678098" class="tablinks" onclick="openCity(event, 'tab_gestion_virtual')"><i class="fad fa-cogs"></i> Gestión de tramites virtuales</a></li>
                </ul>
            </div>
            <div id="error_div_error_general" style="position: relative; width: 100%"></div>
            <div id="registro_ruta" class="tabcontent d-none border-0" style="border-radius: 0px 0px 5px 5px"> 
                <div id="title_registro_ruta" class="navbar navbar-expand-lg navbar-light bg-light_  pl-0">
                    <div class="row w-100">
                        <div class="col-6">
                            <span class="navbar-text h6" style="color: #678098">REGISTRAR TAREA PARA RUTA
                            </span>   
                        </div>
                        <div class="col-6 justify-content-end">
                           
                        </div>     
                    </div>
                </div>
                <div id="conten_registro_ruta"  style="overflow: auto">
                    <div id="error_div_registro_ruta" style="position: relative; width: 100%"></div>
                    <div class="row pt-2 pb-2 ml-0 mr-0">
                        <div class="col-4 pl-0 pr-0">
                            <span class="h6 font-weight-light">Recibo SII</span>
                        </div>
                        <div class="col-8 input-group">
                            <div class="input-group-append">
                                <select id="DropDownList_ante_pone_rut" class="form-select form-select-lg  form-control-drow" atrib_aleas_c="DropDownList_tramites_rut" atrib_campo_o="1"
                                    atrib_campo_n="DropDownList_tramites_rut" atrib_campo_v="1"
                                    atrib_campo_tip="0" atrib_campo_nl="1" atrib_campo_id="0" atrib_name_campo_id="null" atrib_campo_t="VARCHAR" atrib_campo_tbl=""
                                    atrib_campo_drow_destino="" atrib_name_espace_control="conten_registro_ruta" atrib_control_tip_correo="0"
                                    atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null">
                                    <option>S</option>
                                    <option>R</option>
                                </select>
                            </div>
                            <input class="form-control  conten_registro_ruta form-controls" id="TextBox_recibo_caja_rut" type="text" maxlength="30" atrib_aleas_c="recibo" atrib_campo_o="1"
                                atrib_campo_n="recibo" atrib_campo_v="1" atrib_campo_tip="1" atrib_campo_nl="0" atrib_campo_id="0" atrib_name_campo_id="null"
                                atrib_campo_t="VARCHAR" atrib_campo_tbl="" atrib_campo_drow_destino="null" atrib_name_espace_control="conten_registro_ruta"
                                atrib_control_tip_correo="0" atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null" />

                            <div class="input-group-append">
                                <a id="Button_consultar_recibo_sii_rut" title="consultar recibo" class="btn btn-success" href="#"><i class="far fa-search"></i></a>
                            </div>
                        </div>
                    </div>
                    <div class="row pb-2 ml-0 mr-0">
                        <div class="col-4 pl-0 pr-0">
                            <span class="h6 font-weight-light">Código de Barras / Radicado *</span>
                        </div>
                        <div class="col-8">
                            <input id="TextBox_codigo_barras_ruta" disabled="disabled" type="text" class="form-control w-100 conten_registro_ruta form-controls" maxlength="30" atrib_aleas_c="codigo barras" atrib_campo_o="1"
                                atrib_campo_n="codigo_barras" atrib_campo_v="1" atrib_campo_tip="1" atrib_campo_nl="0" atrib_campo_id="0" atrib_name_campo_id="null"
                                atrib_campo_t="VARCHAR" atrib_campo_tbl="" atrib_campo_drow_destino="null" atrib_name_espace_control="conten_registro_ruta"
                                atrib_control_tip_correo="0" atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null" />
                        </div>
                    </div>
                    <div class="row pb-2 ml-0 mr-0">
                        <div class="col-4 pl-0 pr-0">
                            <span class="h6 font-weight-light">Matricula *</span>
                        </div>
                        <div class="col-8">
                            <input id="TextBox_matricula_rut" disabled="disabled" type="text" class="form-control w-100 conten_registro_ruta form-controls" maxlength="30" atrib_aleas_c="matricula" atrib_campo_o="1"
                                atrib_campo_n="matricula" atrib_campo_v="1" atrib_campo_tip="1" atrib_campo_nl="0" atrib_campo_id="0" atrib_name_campo_id="null"
                                atrib_campo_t="VARCHAR" atrib_campo_tbl="" atrib_campo_drow_destino="null" atrib_name_espace_control="conten_registro_ruta"
                                atrib_control_tip_correo="0" atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null" />
                        </div>
                    </div>
                    <div class="row pb-2 ml-0 mr-0">
                        <div class="col-4 pl-0 pr-0">
                            <span class="h6 font-weight-light">Razón social / Nombre  *</span>
                        </div>
                        <div class="col-8">
                            <input id="TextBox_razon_social_ruta" disabled="disabled" type="text" class="form-control w-100 conten_registro_ruta form-controls" maxlength="120" atrib_aleas_c="Razón Social" atrib_campo_o="1"
                                atrib_campo_n="rscocial" atrib_campo_v="1" atrib_campo_tip="1" atrib_campo_nl="0" atrib_campo_id="0" atrib_name_campo_id="null"
                                atrib_campo_t="VARCHAR" atrib_campo_tbl="" atrib_campo_drow_destino="null" atrib_name_espace_control="conten_registro_ruta"
                                atrib_control_tip_correo="0" atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null" />
                        </div>
                    </div>
                    <div class="row pb-2 ml-0 mr-0">
                        <div class="col-4 pl-0 pr-0">
                            <span class="h6 font-weight-light">Trámites disponibles * </span>
                        </div>
                        <div class="col-8">
                            <select id="DropDownList_tramites_rut" class="form-select form-select-lg mb-3 w-100 conten_registro_ruta form-control-drow" atrib_aleas_c="Tramite " atrib_campo_o="1"
                                atrib_campo_n="id_tramite" atrib_campo_v="1"
                                atrib_campo_tip="0" atrib_campo_nl="1" atrib_campo_id="0" atrib_name_campo_id="null" atrib_campo_t="VARCHAR" atrib_campo_tbl="RAD_GESTION"
                                atrib_campo_drow_destino="" atrib_name_espace_control="conten_registro_ruta" atrib_control_tip_correo="0"
                                atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null">
                            </select>
                        </div>
                    </div>
                    <div class="row pb-2 ml-0 mr-0">
                        <div class="col-4 pl-0 pr-0">
                            <span class="h6 font-weight-light">Actividades disponibles * </span>
                        </div>
                        <div class="col-8">
                            <select id="DropDownList_actividades_ruta" class="form-select form-select-lg mb-3 w-100 conten_registro_ruta form-control-drow" atrib_aleas_c="Actividad ruta" atrib_campo_o="1"
                                atrib_campo_n="id_actividad" atrib_campo_v="1"
                                atrib_campo_tip="0" atrib_campo_nl="1" atrib_campo_id="0" atrib_name_campo_id="null" atrib_campo_t="VARCHAR" atrib_campo_tbl="RAD_GESTION"
                                atrib_campo_drow_destino="" atrib_name_espace_control="conten_registro_ruta" atrib_control_tip_correo="0"
                                atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null">
                            </select>
                        </div>

                    </div>
                    <div class="row pb-2 d-none ml-0 mr-0">
                        <div class="col-4 pl-0 pr-0">
                            <span class="h6 font-weight-light">Usuarios disponibles * </span>
                        </div>
                        <div class="col-8">
                            <select id="DropDownList_usurios_ruta" class="form-select form-select-lg mb-3 w-100 conten_registro_ruta_ d-none" atrib_aleas_c="Usuario de la  tarea" atrib_campo_o="1"
                                atrib_campo_n="id_usuario" atrib_campo_v="1"
                                atrib_campo_tip="0" atrib_campo_nl="1" atrib_campo_id="0" atrib_name_campo_id="null" atrib_campo_t="VARCHAR" atrib_campo_tbl="RAD_GESTION"
                                atrib_campo_drow_destino="" atrib_name_espace_control="conten_registro_ruta" atrib_control_tip_correo="0"
                                atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null">
                            </select>
                        </div>
                    </div>
                </div>
                 
                <div class="modal-footer align-content-end" id="modal_foter_registro_ruta">
                    <button type="button" id="Button_registro_actividad_ruta" class="btn  btn-success" title="Registro actividad ruta ">Aceptar</button>
                </div>
            </div>
            <div id="registro_flujo" class="tabcontent d-none border-0" style="border-radius: 0px 0px 0px 0px ">
                
                 <div id="title_registro_flujo" class="navbar navbar-expand-lg navbar-light bg-light_ pl-0">
                    <div class="row w-100">
                        <div class="col-6">
                            <span class="navbar-text h6" style="color: #678098">REGISTRAR TAREA PARA FLUJO
                            </span>   
                        </div>
                        <div class="col-6 justify-content-end">
                           
                        </div>     
                    </div>
                </div>
                 <div id="conten_registro_flujo"  style="overflow: auto">
                     <div id="error_div_registro_flujo" class="pl-0 pr-0 ml-0 mr-0" style="position: relative; width: 100%"></div>
                    <div class="row pt-2 pb-2 ml-0 mr-0">
                        <div class="col-4 pl-0 pr-0 btn-group_">
                            <span class="h6 font-weight-light control-label">Recibo SII *</span>
                        </div>
                        <div class="col-8 input-group">
                            <div class="input-group-append">
                                <select id="DropDownList_ante_pone_flujo" class="form-select form-select-lg w-100 form-control-drow" atrib_aleas_c="DropDownList_tramites_flujo" atrib_campo_o="0"
                                    atrib_campo_n="DropDownList_tramites_flujo" atrib_campo_v="1"
                                    atrib_campo_tip="0" atrib_campo_nl="1" atrib_campo_id="0" atrib_name_campo_id="null" atrib_campo_t="VARCHAR" atrib_campo_tbl=""
                                    atrib_campo_drow_destino="" atrib_name_espace_control="conten_registro_flujo_" atrib_control_tip_correo="0"
                                    atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null">
                                    <option>S</option>
                                    <option>R</option>
                                </select>
                            </div>
                            <input class="form-control  conten_registro_flujo form-controls" id="TextBox_recibo_caja_flujo" type="text" maxlength="30" atrib_aleas_c="recibo" atrib_campo_o="1"
                                atrib_campo_n="recibo" atrib_campo_v="1" atrib_campo_tip="1" atrib_campo_nl="0" atrib_campo_id="0" atrib_name_campo_id="null"
                                atrib_campo_t="VARCHAR" atrib_campo_tbl="" atrib_campo_drow_destino="null" atrib_name_espace_control="conten_registro_flujo"
                                atrib_control_tip_correo="0" atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null" />
                            <div class="input-group-append">
                                <a id="Button_consultar_recibo_sii_flujo" title="consultar recibo" class="btn btn-success" href="#"><i class="far fa-search"></i></a>
                            </div>
                        </div>
                    </div>
                    <div class="row pb-2 ml-0 mr-0">
                        <div class="col-4 pl-0 pr-0">
                            <span class="h6 font-weight-light">Código de Barras / Radicado *</span>
                        </div>
                        <div class="col-8">
                            <input id="TextBox_codigo_barras_flujo" disabled="disabled" type="text" class="form-control w-100 conten_registro_flujo form-controls" maxlength="30" atrib_aleas_c="codigo barras" atrib_campo_o="1"
                                atrib_campo_n="codigo_barras" atrib_campo_v="1" atrib_campo_tip="1" atrib_campo_nl="0" atrib_campo_id="0" atrib_name_campo_id="null"
                                atrib_campo_t="VARCHAR" atrib_campo_tbl="" atrib_campo_drow_destino="null" atrib_name_espace_control="conten_registro_flujo"
                                atrib_control_tip_correo="0" atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null" />
                        </div>
                    </div>
                    <div class="row pb-2 ml-0 mr-0">
                        <div class="col-4 pl-0 pr-0">
                            <span class="h6 font-weight-light">Matricula *</span>
                        </div>
                        <div class="col-8">
                            <input id="TextBox_matricula_flujo" type="text" disabled="disabled" class="form-control w-100 conten_registro_flujo form-controls" maxlength="30" atrib_aleas_c="matricula" atrib_campo_o="1"
                                atrib_campo_n="matricula" atrib_campo_v="1" atrib_campo_tip="1" atrib_campo_nl="0" atrib_campo_id="0" atrib_name_campo_id="null"
                                atrib_campo_t="VARCHAR" atrib_campo_tbl="" atrib_campo_drow_destino="null" atrib_name_espace_control="conten_registro_flujo"
                                atrib_control_tip_correo="0" atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null"  />
                        </div>
                    </div>
                    <div class="row pb-2 ml-0 mr-0">
                        <div class="col-4 pl-0 pr-0">
                            <span class="h6 font-weight-light">Razón social / Nombre  *</span>
                        </div>
                        <div class="col-8">
                            <input id="TextBox_razon_social_flujo" disabled="disabled" type="text" class="form-control w-100 conten_registro_flujo form-controls" maxlength="120" atrib_aleas_c="Razón Social" atrib_campo_o="1"
                                atrib_campo_n="rscocial" atrib_campo_v="1" atrib_campo_tip="1" atrib_campo_nl="0" atrib_campo_id="0" atrib_name_campo_id="null"
                                atrib_campo_t="VARCHAR" atrib_campo_tbl="" atrib_campo_drow_destino="null" atrib_name_espace_control="conten_registro_flujo"
                                atrib_control_tip_correo="0" atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null" />
                        </div>
                    </div>
                    <div class="row pb-2 ml-0 mr-0">
                        <div class="col-4 pl-0 pr-0">
                            <span class="h6 font-weight-light">Trámites disponibles * </span>
                        </div>
                        <div class="col-8">
                            <select id="DropDownList_tramites_flujo" disabled="disabled" class="form-select form-select-lg mb-3 w-100 conten_registro_flujo form-control-drow" atrib_aleas_c="Tramite " atrib_campo_o="1"
                                atrib_campo_n="id_tramite" atrib_campo_v="1"
                                atrib_campo_tip="0" atrib_campo_nl="1" atrib_campo_id="0" atrib_name_campo_id="null" atrib_campo_t="VARCHAR" atrib_campo_tbl=""
                                atrib_campo_drow_destino="" atrib_name_espace_control="conten_registro_flujo" atrib_control_tip_correo="0"
                                atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null">
                            </select>
                        </div>
                    </div>
                    <div class="row pb-2 ml-0 mr-0">
                        <div class="col-4 pl-0 pr-0">
                            <span class="h6 font-weight-light">Flujos disponibles * </span>
                        </div>
                        <div class="col-8">
                            <select id="DropDownList_flujos" class="form-select form-select-lg mb-3 w-100 conten_registro_flujo form-control-drow" atrib_aleas_c="Flujo trabajo" atrib_campo_o="1"
                                atrib_campo_n="id_flujo" atrib_campo_v="1"
                                atrib_campo_tip="0" atrib_campo_nl="1" atrib_campo_id="0" atrib_name_campo_id="null" atrib_campo_t="VARCHAR" atrib_campo_tbl=""
                                atrib_campo_drow_destino="" atrib_name_espace_control="conten_registro_flujo" atrib_control_tip_correo="0"
                                atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null">
                            </select>
                        </div>

                    </div>
                    <div class="row pb-2 ml-0 mr-0">
                        <div class="col-4 pl-0 pr-0">
                            <span class="h6 font-weight-light">Actividades disponibles * </span>
                        </div>
                        <div class="col-8">
                            <select id="DropDownList_actividades_flujo" class="form-select form-select-lg mb-3 w-100 conten_registro_flujo form-control-drow" atrib_aleas_c="Actividad flujo" atrib_campo_o="1"
                                atrib_campo_n="id_actividad" atrib_campo_v="1"
                                atrib_campo_tip="0" atrib_campo_nl="1" atrib_campo_id="0" atrib_name_campo_id="null" atrib_campo_t="VARCHAR" atrib_campo_tbl=""
                                atrib_campo_drow_destino="" atrib_name_espace_control="conten_registro_flujo" atrib_control_tip_correo="0"
                                atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null">
                            </select>
                        </div>

                    </div>
                    <div class="row pb-2  ml-0 mr-0">
                        <div class="col-4 pl-0 pr-0">
                            <span class="h6 font-weight-light">Usuarios disponibles * </span>
                        </div>
                        <div class="col-8">
                            <select id="DropDownList_usurios_flujo" class="form-select form-select-lg mb-3 w-100 conten_registro_flujo form-control-drow" atrib_aleas_c="Usuario de la  tarea" atrib_campo_o="1"
                                atrib_campo_n="id_usuario" atrib_campo_v="1"
                                atrib_campo_tip="0" atrib_campo_nl="1" atrib_campo_id="0" atrib_name_campo_id="null" atrib_campo_t="VARCHAR" atrib_campo_tbl=""
                                atrib_campo_drow_destino="" atrib_name_espace_control="conten_registro_flujo" atrib_control_tip_correo="0"
                                atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null">
                            </select>
                        </div>
                    </div>
                </div>
                
                 <div class="modal-footer align-content-end" id="modal_foter_registro_flujo">
                    <button type="button" id="Button_registro_actividad_flujo" class="btn  btn-success" title="Registro actividad flujo ">Aceptar</button>
                 </div>
            </div>
            <div id="registro_flujo_" class="tabcontent" style="overflow: auto; border-radius: 0px 0px 0px 0px">
                <table style="width: auto">
                    <tr>
                        <td>
                            <asp:Label ID="Label_recibo" runat="server" Text="Recibo sii * " CssClass="h6 font-weight-light"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="DropDownList_ante_pone" runat="server" Style="width: 60px">
                                <asp:ListItem>S</asp:ListItem>
                                <asp:ListItem>R</asp:ListItem>
                            </asp:DropDownList>
                            <asp:TextBox ID="TextBox_recibo_caja" runat="server" Style="width: 200px"></asp:TextBox>
                            <asp:UpdatePanel ID="UpdatePanel_buton_consulta" UpdateMode="Conditional" runat="server" RenderMode="Inline">
                                <ContentTemplate>
                                    <asp:Button ID="Button_consulta_recibo" runat="server" Text="Consulta" Style="float: right" CssClass="btn btn-success" ToolTip="Consulta los datos del recibo en el sii" />
                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label_codigo_barraras" runat="server" Text="Códogo de Barras / Radicado *" CssClass="h6 font-weight-light"></asp:Label>
                        </td>
                        <td>
                            <asp:UpdatePanel ID="UpdatePanel_text_codigo_barras" UpdateMode="Conditional" runat="server" RenderMode="Inline">
                                <ContentTemplate>
                                    <asp:TextBox ID="TextBox_codigo_barras" runat="server" Style="width: 200px"></asp:TextBox>
                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label_matricula" runat="server" Text="Matricula  " CssClass="h6 font-weight-light"></asp:Label>
                        </td>
                        <td>

                            <asp:UpdatePanel ID="UpdatePanel_text_matricula" UpdateMode="Conditional" runat="server" RenderMode="Inline">
                                <ContentTemplate>
                                    <asp:TextBox ID="TextBox_matricula" runat="server" Style="width: 200px"></asp:TextBox>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label_razon_social" runat="server" Text="Razón social / Nombre  *" CssClass="h6 font-weight-light"></asp:Label>
                        </td>
                        <td>
                            <asp:UpdatePanel ID="UpdatePanel_text_Razon_social" UpdateMode="Conditional" runat="server" RenderMode="Inline">
                                <ContentTemplate>
                                    <asp:TextBox ID="TextBox_razon_social" runat="server" Style="width: 400px"></asp:TextBox>
                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label_tramite" runat="server" Text="Trámites disponibles para el flujo * " CssClass="h6 font-weight-light"></asp:Label>
                        </td>
                        <td>
                            <asp:UpdatePanel ID="UpdatePanel_drow_tramites" UpdateMode="Conditional" runat="server" RenderMode="Inline">
                                <ContentTemplate>
                                    <asp:DropDownList ID="DropDownList_tramites" runat="server" Style="width: 400px" AutoPostBack="true">
                                    </asp:DropDownList>
                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label_rutas" runat="server" Text="Rutas disponibles para el flujo * " CssClass="h6 font-weight-light"></asp:Label>
                        </td>
                        <td>
                            <asp:UpdatePanel ID="UpdatePanel_drow_rutas" UpdateMode="Conditional" runat="server" RenderMode="Inline">
                                <ContentTemplate>
                                    <asp:DropDownList ID="DropDownList_rutas" runat="server" Style="width: 400px" AutoPostBack="true">
                                    </asp:DropDownList>
                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label_actividades" runat="server" Text="Actividades disponibles para el flujo * " CssClass="h6 font-weight-light"></asp:Label>
                        </td>
                        <td class="auto-style1">
                            <asp:UpdatePanel ID="UpdatePanel_drow_actividades" UpdateMode="Conditional" runat="server" RenderMode="Inline">
                                <ContentTemplate>
                                    <asp:DropDownList ID="DropDownList_actividades" runat="server" Style="width: 400px" AutoPostBack="true">
                                    </asp:DropDownList>
                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label_usuarios" runat="server" Text="Usuarios disponibles para el flujo * " CssClass="h6 font-weight-light"></asp:Label>
                        </td>
                        <td class="auto-style1">
                            <asp:UpdatePanel ID="UpdatePanel_drow_usuarios" UpdateMode="Conditional" runat="server" RenderMode="Inline">
                                <ContentTemplate>
                                    <asp:DropDownList ID="DropDownList_usurios" runat="server" Style="width: 400px">
                                    </asp:DropDownList>
                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </td>
                    </tr>



                    <tr>
                        <td colspan="2">
                            <asp:UpdatePanel ID="UpdatePanel_botones_registro" UpdateMode="Conditional" runat="server" RenderMode="Inline">
                                <ContentTemplate>
                                    <asp:Button ID="Button_reistrar_flujo" runat="server" Text="Aceptar" Style="float: right; margin-top: 5px" CssClass=" btn btn-success" />

                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>

                </table>

            </div>
            <div id="elimina_flujo" class="tabcontent d-none border-0" style="border-radius: 0px 0px 0px 0px">
                <div id="title_elimina_flujo" class="navbar navbar-expand-lg navbar-light bg-light_ pl-0">
                    <div class="row w-100">
                        <div class="col-6">
                            <span class="navbar-text h6" style="color: #678098">ELIMINAR TAREA WORKFLOW
                            </span>   
                        </div>
                        <div class="col-6 justify-content-end">
                           
                        </div>     
                    </div>
                </div>
                <div id="conten_elimina_flujo"  class="border-0" style="overflow: auto">
                    <div id="error_div_elimina_flujo" class="pl-0 pr-0 ml-0 mr-0" style="position: relative; width: 100%"></div>
                    <div class="row pt-2 pb-2 ml-0 mr-0">
                        <div class="col-4 pl-0 pr-0">
                            <span class="h6 font-weight-light">Recibo SII</span>
                        </div>
                        <div class="col-8 input-group">
                            <div class="input-group-append">
                                <select id="DropDownList_ante_pone_elimina_flujo" class="form-select form-select-lg w-100 form-control-drow" atrib_aleas_c="DropDownList_tramites_elimina_flujo" atrib_campo_o="1"
                                    atrib_campo_n="DropDownList_tramites_flujo" atrib_campo_v="1"
                                    atrib_campo_tip="0" atrib_campo_nl="1" atrib_campo_id="0" atrib_name_campo_id="null" atrib_campo_t="VARCHAR" atrib_campo_tbl=""
                                    atrib_campo_drow_destino="" atrib_name_espace_control="conten_registro_ruta" atrib_control_tip_correo="0"
                                    atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null">
                                    <option>S</option>
                                    <option>R</option>
                                </select>
                            </div>
                            <input class="form-control  conten_registro_flujo_ form-controls" id="TextBox_recibo_caja_elimina_flujo" type="text" maxlength="30" atrib_aleas_c="recibo" atrib_campo_o="1"
                                atrib_campo_n="recibo" atrib_campo_v="1" atrib_campo_tip="1" atrib_campo_nl="0" atrib_campo_id="0" atrib_name_campo_id="null"
                                atrib_campo_t="VARCHAR" atrib_campo_tbl="" atrib_campo_drow_destino="null" atrib_name_espace_control="conten_registro_flujo_"
                                atrib_control_tip_correo="0" atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null" />
                            <div class="input-group-append">
                                <button class="btn btn-success" id="Button_elimina_tarea" title="Elimina tarea worflow SII" type="button">
                                    <i class="fas fa-trash-alt"></i>
                                </button>

                            </div>
                        </div>
                    </div>
                     
                </div>
                <div class="modal-footer align-content-end" id="modal_foter_elimina_flujo">
                 </div>
            </div>
            <div id="elimina_flujo_" class="tabcontent" style="overflow: auto; border-radius: 0px 0px 0px 0px">
                <table style="width: auto">
                    <tr style="height: 40px">
                        <td style="height: 40px">
                            <asp:Label ID="Label1" runat="server" Text="Recibo sii * " CssClass="h6 font-weight-light"></asp:Label>
                        </td>
                        <td style="height: 40px">
                            <asp:DropDownList ID="DropDownList_antepone_elimina" runat="server" Style="width: 60px; height: 30px">
                                <asp:ListItem>S</asp:ListItem>
                                <asp:ListItem>R</asp:ListItem>
                            </asp:DropDownList>
                            <asp:TextBox ID="TextBox_recibo_elimina" runat="server" Style="width: 200px"></asp:TextBox>
                            <asp:UpdatePanel ID="UpdatePanel_buton_elimina" UpdateMode="Conditional" runat="server" RenderMode="Inline">
                                <ContentTemplate>
                                    <asp:Button ID="Button_elimina_flujo" runat="server" Text="Eliminar" Style="float: right; margin-left: 5px" CssClass="btn btn-success" ToolTip="Elimina un flujo de trabajo" />
                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </td>
                    </tr>
                </table>
            </div>
            <div id="edita_flujo" class="tabcontent d-none border-0" style="border-radius: 0px 0px 0px 0px">
                <div id="title_edita_flujo" class="navbar navbar-expand-lg navbar-light bg-light_ pl-0">
                    <div class="row w-100">
                        <div class="col-6">
                            <span class="navbar-text h6" style="color: #678098">ACTUALIZAR DOCUMENTO TAREA
                            </span>   
                        </div>
                        <div class="col-6 justify-content-end">
                           
                        </div>     
                    </div>
                </div>
                <div id="conten_edita_flujo" class="pt-3" style="overflow: auto">
                    <div id="error_div_edita_flujo" class="pl-0 pr-0 ml-0 mr-0" style="position: relative; width: 100%"></div>
                    <div class="row pt-2 pb-2 ml-0 mr-0">
                        <div class="col-4 pl-0 pr-0">
                            <span class="h6 font-weight-light">Recibo SII</span>
                        </div>
                        <div class="col-8 input-group">
                            <div class="input-group-append">
                                <select id="DropDownList_ante_pone_edita_flujo" class="form-select form-select-lg w-100 form-control-drow" atrib_aleas_c="DropDownList_tramites_edita_flujo" atrib_campo_o="1"
                                    atrib_campo_n="DropDownList_tramites_flujo" atrib_campo_v="1"
                                    atrib_campo_tip="0" atrib_campo_nl="1" atrib_campo_id="0" atrib_name_campo_id="null" atrib_campo_t="VARCHAR" atrib_campo_tbl=""
                                    atrib_campo_drow_destino="" atrib_name_espace_control="conten_registro_ruta" atrib_control_tip_correo="0"
                                    atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null">
                                    <option>S</option>
                                    <option>R</option>
                                </select>
                            </div>
                            <input class="form-control  conten_registro_flujo_ form-controls" id="TextBox_recibo_caja_edita_flujo" type="text" maxlength="30" atrib_aleas_c="recibo" atrib_campo_o="1"
                                atrib_campo_n="recibo" atrib_campo_v="1" atrib_campo_tip="1" atrib_campo_nl="0" atrib_campo_id="0" atrib_name_campo_id="null"
                                atrib_campo_t="VARCHAR" atrib_campo_tbl="" atrib_campo_drow_destino="null" atrib_name_espace_control="conten_registro_flujo_"
                                atrib_control_tip_correo="0" atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null" />
                            <div class="input-group-append">
                                <button class="btn btn-success" id="Button_edita_flujo" title="Actualiza imagen tarea ruta" type="button">
                                    <i class="fad fa-file-check"></i>
                                </button>

                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer align-content-end" id="modal_foter_edita_flujo">
                </div>
               </div>
            
            <div id="edita_flujo_" class="tabcontent">
                <table style="width: auto">
                    <tr>
                        <td>
                            <asp:Label ID="Label_rutas_flujo_edita" runat="server" Text="Rutas disponibles para el flujo * " CssClass="h6 font-weight-light"></asp:Label>
                        </td>
                        <td>
                            <asp:UpdatePanel ID="UpdatePanel_drow_rutas_edita" UpdateMode="Conditional" runat="server" RenderMode="Inline">
                                <ContentTemplate>
                                    <asp:DropDownList ID="DropDownList_drow_rutas_edita" runat="server" Style="width: 400px" AutoPostBack="true">
                                    </asp:DropDownList>
                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </td>
                    </tr>
                    <tr style="height: 40px">
                        <td style="height: 40px">
                            <asp:Label ID="Label3" runat="server" Text="Recibo sii * " CssClass="h6 font-weight-light"></asp:Label>
                        </td>
                        <td style="height: 40px">
                            <asp:DropDownList ID="DropDownList_antepone_actuliza" runat="server" Style="width: 60px; height: 30px">
                                <asp:ListItem>S</asp:ListItem>
                                <asp:ListItem>R</asp:ListItem>
                            </asp:DropDownList>
                            <asp:TextBox ID="TextBox_recibo_actualiza" runat="server" Style="width: 200px"></asp:TextBox>
                            <asp:UpdatePanel ID="UpdatePanel_buton_actualiza" UpdateMode="Conditional" runat="server" RenderMode="Inline">
                                <ContentTemplate>
                                    <asp:Button ID="ButtonEdita_actualiza" runat="server" Text="Actualiza" Style="float: right; margin-left: 5px" CssClass="btn btn-success" ToolTip="Elimina un flujo de trabajo" />
                                </ContentTemplate>
                            </asp:UpdatePanel>

                        </td>
                    </tr>
                </table>
            </div>
            <div id="tab_reasigna_sii" class="tabcontent d-none border-0">
                <div id="conten_consulta_sii"  >
                    <div id="div_contenido_controles_consulta" style="width: 100%">
                        <div id="title_reasigna_sii" class="navbar navbar-expand-lg navbar-light bg-light_ pl-0">
                            <div class="row w-100">
                                <div class="col-6">
                                    <span class="navbar-text h6" style="color: #678098">REASIGNAR TAREA WORKFLOW
                                    </span>
                                </div>
                                <div class="col-6 justify-content-end">
                                </div>
                            </div>
                        </div>
                         <div id="error_div_reasigna_sii" style="position: relative; width: 100%"></div>
                        <div class="row pt-4 pb-2">
                            <div class="col-4">
                                <span>Código de barras</span>
                            </div>
                            <div class="col-8 input-group">
                                 <input id="Text_codigo_barra_sii" class="form-controls form-control" type="text" />
                                <div class="input-group-append">
                                    <button class="btn btn-success" id="Button_consultar_radicado" name_event="C-RAD-SII" title="Consultar código SII" type="button">
                                    <i class="far fa-search"></i>
                                </button>
                                </div>
                            </div>        
                        </div>

                    </div>        
                    <div id="div_content_tabla">
                        <table
                            id="table"
                            data-height="200"
                            data-pagination="false"
                            data-page-list="[10, 25, 50, 100, all]"
                            data-show-export="false"
                            data-toggle="table"
                            data-unique-id="ID_TAREA"
                            data-search="false"
                            data-locale="es-SP">
                            <thead class="GridviewScrollHeader_line_blue_wite">
                                <tr >
                                    <th data-field="operate" data-formatter="operateFormatter_reasing" data-events="operateEvents">OPCION</th>
                                    <th data-field="ID_TAREA" data-visible="false">ID_TAREA</th>
                                    <th data-field="ACTIVIDAD">ACTIVIDAD</th>
                                    <th data-field="USUARIO">USUARIO</th>
                                    <th data-field="CARGO">CARGO</th>
                                </tr>
                            </thead>
                            <tbody style="font-weight:300">

                           </tbody>
                        </table>
                    </div>
                    <div id="conte_foter_consulta" class="modal-footer d-none">
                        <input id="Button_activa_reasignar" type="button" value="Reasignar" name_event="A-REA-SII" value_event="na" class="btn btn-success bt_sys_event_element" />
                    </div>
                </div>
               
            </div>
            <div id="tab_gestion_balance" class="tabcontent d-none border-0">
                <div id="conten_consulta_balance" class=" pt-1" style="">   
                    <div id="div_contenido_controles_consulta_balance" style="width: 100%">   
                        <div id="title_gestion_balance" class="navbar navbar-expand-lg navbar-light bg-light_  pl-0">
                            <div class="row w-100">
                                <div class="col-6">
                                    <span class="navbar-text h6" style="color: #678098"> GESTION DE USUARIOS
                                    </span>
                                </div>
                                <div class="col-6 justify-content-end">
                                </div>
                            </div>
                        </div>
                        <div id="error_div_gestion_balance" style="position: relative; width: 100%"></div>
                        <div class="row pt-4 pb-2">
                            <div class="col-4">
                                <span>Usuario balanceo</span>
                            </div>
                            <div class="col-8 input-group">
                                 <input id="Text_usuario_workflow" class="form-controls form-control" type="text" />
                                <div class="input-group-append">
                                    <button class="btn btn-success" id="Button_consultar_usuario" name_event="C-CONS-USER" title="Consultar" type="button">
                                    <i class="far fa-search"></i>
                                </button>
                                </div>
                            </div>    
                        </div>
                    </div>
                    <div id="div_content_tabla_balance">
                        <table
                            id="table_balance"
                            data-height="200"
                            data-pagination="true"
                            data-page-list="[10, 25, 50, 100, all]"
                            data-show-export="false"
                            data-toggle="table"
                            data-id-field="idU_suario"
                            data-search="false"
                            data-locale="es-SP">
                             <thead class="GridviewScrollHeader_line_blue_wite">
                                <tr>
                                    <th data-field="idU_suario" data-visible="false" style="display: none">ID_UUSARIO_WORKFLOW</th>
                                    <th data-field="estado_balanceo_grupo" data-visible="false" style="display: none">ESTADO</th>
                                    <th data-field="UTIL_ASIGNA_TAREA" data-visible="false" style="display: none">ESTADO BALANCE</th>
                                    <th data-field="operate" data-sortable="true" data-sort-name="estado_balanceo_grupo" data-formatter="operateFormatter_balance" data-events="operateEvents">BALANCE</th>
                                    <th data-field="operate_" data-sortable="true" data-sort-name="estado_asigna_tarea" data-formatter="operateFormatter_asing" data-events="operateEvents">ASIGNACION</th>
                                    <th data-field="login_Usuario">LOGIN</th>
                                    <th data-field="Nombre_usuario">USUARIO</th>
                                    <th data-field="Cargo_Usuario">CARGO</th>
                                    <th data-field="Nombre_Grupo">GRUPO</th>
                                </tr>
                            </thead>
                            <tbody style="font-weight:300">

                           </tbody>
                        </table>
                    </div>
                    <div id="conte_foter_balanceo" class="modal-footer d-none">
                    </div>
                </div>
                
            </div>
            <div id="tab_gestion_rue" class="tabcontent d-none border-0" style="border-radius: 0px 0px 0px 0px">
                <div id="title_gestion_rue" class="navbar navbar-expand-lg navbar-light bg-light_ pl-0">
                    <div class="row w-100">
                        <div class="col-6">
                            <span class="navbar-text h6" style="color: #678098">GESTIÓN DE TRAMITES RUES
                            </span>   
                        </div>
                        <div class="col-md-3 justify-content-end" >
                            <div class="navbar-nav">
                                <li>
                                    <a class="nav-link  dropdown-toggle " style="color: black" href="#" id="A100" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: black; display: none" class="fad fa-th-list"></i> Opciones</a>
                                    <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                                        <a id="Button_load_file_rue" style="color: black" href="#" class="dropdown-item font-weight-light "><i class="far fa-upload"></i> Cargar registros rue </a>
                                        <a style="color: black" href="#" class="dropdown-item font-weight-light"><i class="far fa-sign-out"></i> Salir del menú</a>
                                    </div>
                                </li>
                            </div>  
                        </div>     
                    </div>
                </div>
                <div id="conten_gestion_rue">
                    <div id="error_div_gestion_rue" class="pl-0 pr-0 ml-0 mr-0" style="position: relative; width: 100%"></div>
                    <div id="div_content_tabla_gestion_rue" style="height: 400px">
                        <table
                            id="table_gestion_rue"
                            data-height="200"
                            data-pagination="true"
                            data-page-list="[10, 25, 50, 100, all]"
                            data-show-export="true"
                            data-toggle="table"
                            data-unique-id="RECIBO"
                            data-search="true"
                            data-locale="es-SP">
                            <thead class="GridviewScrollHeader_line_blue_wite">
                              
                                <tr >
                                </tr>
                            </thead>  
                            <tbody style="font-weight:300">

                           </tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer align-content-end d-none" id="modal_foter_gestion_rue">
                    
                </div>
            </div>
            <div id="tab_gestion_virtual" class="tabcontent d-none border-0" style="border-radius: 0px 0px 0px 0px">  
                <div id="title_gestion_virtual" class="navbar navbar-expand-lg navbar-light bg-light_ pl-0">
                    <div class="row w-100">
                        <div class="col-6">
                            <span class="navbar-text h6" style="color: #678098">GESTIÓN DE TRAMITES VIRTUALES
                            </span>   
                        </div>
                        <div class="col-md-3 justify-content-end" >
                            <div class="navbar-nav">
                                <li>
                                    <a class="nav-link  dropdown-toggle " style="color: black" href="#" id="A5" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i style="color: black; display: none" class="fad fa-th-list"></i> Opciones</a>
                                    <div class="dropdown-menu" aria-labelledby="navbarDropdownMenuLink_">
                                        <a id="Button_load_file_virtual" style="color: black" href="#" class="dropdown-item font-weight-light "><i class="far fa-upload"></i> Cargar registros virtuales </a>
                                        <a style="color: black" href="#" class="dropdown-item font-weight-light"><i class="far fa-sign-out"></i> Salir del menú</a>
                                    </div>
                                </li>
                            </div>  
                        </div>  
                        
                    </div>
                </div>
                <div id="conten_gestion_virtual">
                    <div id="error_div_gestion_virtual" class="pl-0 pr-0 ml-0 mr-0" style="position: relative; width: 100%"></div>
                    <div id="div_content_tabla_gestion_virtual" style="height: 400px">
                        <table
                            id="table_gestion_virtual"
                            data-height="200"
                            data-pagination="true"
                            data-page-list="[10, 25, 50, 100, all]"
                            data-show-export="true"
                            data-toggle="table"
                            data-unique-id="CODIGOBARRAS"
                            data-search="true"
                            data-locale="es-SP">
                            <thead class="GridviewScrollHeader_line_blue_wite">
                                <tr>
                                </tr>
                            </thead>
                            <tbody style="font-weight:300">

                           </tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer align-content-end d-none" id="modal_foter_gestion_virtual">
                  
                </div>
            </div>
        </div>
         
   
         <asp:Panel ID="Panel_reasigna_tarea_workflow_sii" runat="server" Style="display:none; width: 40%; height: auto" CssClass="modal_content_general_">
            <asp:ModalPopupExtender ID="ModalPopupExtender_edition_reasigna_tarea_workflow_sii" runat="server"
                TargetControlID="ButtonSalir_reasigna_tarea_workflow_sii" BackgroundCssClass="FondoAplicacion"
                CancelControlID="Button_cerrar_reasigna_tarea_workflow_sii" PopupControlID="Panel_reasigna_tarea_workflow_sii">
            </asp:ModalPopupExtender>     
             <div id="modal_content_reasigna_tarea_workflow_sii" class="modal-content">
                 <div id="diver_cabcera_reasigna_tarea_workflow_sii" class="modal_title_superior_ modal-header">
                     <h6 class="modal-title d-inline ">Reasignar</h6>
                     <button type="button" value="Button_cerrar_reasigna_tarea_workflow_sii" class="close da_event_captive ">&times;</button>
                 </div>
                 <div id="contenido_procesa_reasigna_tarea_workflow_sii" style="background-color: white; width: 100%; height: 100%; border-top: none" class="modal_content_back modal-body">
                     <div id="content_data_grid_reasigna_tarea_workflow_sii" class="conten_gred_border_" style="width: 100%">
                         <div class="row ">
                            <div class="form-check col-12">
                            <input class="" type="checkbox" value="" checked="checked" id="cheked_cambia_estado_sii" />
                            <label class="" for="flexCheckChecked">
                                Cambia estado usuario SII
                             </label>
                          </div>   
                        </div>
                         <div class="row ">
                             <div class="col-4">
                                 <span> Actividades</span>
                             </div>
                             <div class="col-8">
                                 <asp:DropDownList ID="DropDownList_list_actividad_workflow_sii" name_event="L-USER-WF_" value_event="na" Style="width: 100%" CssClass="custom-select mr-sm-2" runat="server"></asp:DropDownList>
                             </div>
                         </div>
                         <div class="row pt-2">
                             <div class="col-4">
                                 <span>Usuario</span>
                             </div>
                             <div class="col-8">
                                 <asp:DropDownList ID="DropDownList_list_usuario_workflow_sii" Style="width: 100%" CssClass="custom-select mr-sm-2" runat="server"></asp:DropDownList>
                             </div>
                         </div>
                         
                     </div>
                     <div style="display: none; height: 1px">
                         <asp:Button ID="ButtonSalir_reasigna_tarea_workflow_sii" CssClass="invisible" runat="server" Text="" Height="1px" Width="1px" />
                         <asp:Button ID="Button_cerrar_reasigna_tarea_workflow_sii" runat="Server" Text="X" CssClass="invisible" />
                     </div>
                 </div>
                 <div id="error_div_reasigna_sii_popup" style="position: relative; width: 100%"></div>
                 <div class="modal-footer justify-content-end" id="modal-footer_list_inscripciones_sii">
                     <input id="Button_reasigna_sii" type="button" value="Aceptar"  value_event="na" class="btn btn-success" />
                 </div>

             </div>
        </asp:Panel>
        <!--Popup adjunta documento para rue SII -->
        <div class="modal fade modal_opacity" id="modal_adjunta_archivo_rue_sii" data-backdrop="false" role="dialog">
            <div class="modal-dialog  modal-mediunscreen-sm-down ">
                <div class="modal-content-fullscreen">
                    <div class="modal-header">
                        <h4 style="color: black" class="modal-title">Adjunta archivo RUE SII </h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body-fullscreen modal-body">
                        <div class="row row-body-fullscreen">
                            <div class="p-4 w-100">
                                <div class="row p-2" id="content_boton_adjunta_archivo_rue_sii">
                                    <div class="col-12 p-0 pl-1">
                                        <div class="file-select " id="src-file">
                                            <input id="file_element_adjunta_archivo_rue_sii" type="file" multiple="multiple" accept="" style="width: 100px; height: 40px" name="src-file" class="p-1" contente_file="ModalPopupExtender_sube_documento_adjunto" aria-label="Archivo" />
                                        </div>
                                        <a id="save_file_element_adjunta_archivo_rue_sii" title="Guardar todos los archivos" class="btn  btn-success ml-1" style="opacity: 0; color: white"><i style="color: white" class="fas fa-save "></i> Guardar </a>
                                        <a id="delete_file_element_adjunta_archivo_rue_sii" title="Elminar todos los archivos cargados" class="btn  btn-danger " style="opacity: 0; color: white"><i style="color: white" class="fal fa-trash-alt "></i> Eliminar </a>
                                        <a id="cancel_file_element_adjunta_archivo_rue_sii" title="Cancelar guardar archivos" class="btn  btn-warning" style="opacity: 0; color: white"><i style="color: white" class="fas fa-window-close "></i> Cancelar </a>
                                    </div>
                                </div>
                                <div class="paren_element background_upload" id="conten_file_element_adjunta_archivo_rue_sii" style="overflow: auto; height: 80%">
                                    <div id="content_drop_element_adjunta_archivo_rue_sii" claas="">
                                    </div>
                                    <table id="table_file_element_adjunta_archivo_rue_sii" class="table table-striped">
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="error_content_adjunta_archivo_rue_sii" style="position: relative; width: 100%"></div>
                    <div class=" modal-footer_">
                        <div class="row border_ pt-2 w-100" id="content_pie_title_adjunta_archivo_rue_sii">
                            <div class="col-8 justify-content-start">
                                <div class="row p-2">
                                    <div class="col-4 p-0">
                                        <div>
                                            <asp:Label ID="Label_progres_bar_file_element_adjunta_archivo_rue_sii" runat="server" Text="" Style="font-family: Arial; text-align: center; font-size: 20px"></asp:Label>
                                        </div>
                                        <div id="pogres_file_element_contador_adjunta_archivo_rue_sii" style="text-align: center; font-family: Arial; font-size: 14px">
                                        </div>
                                        <div id="pogres_file_element_porcent_adjunta_archivo_rue_sii" style="text-align: center; font-family: Arial; font-size: 14px">
                                        </div>
                                    </div>
                                    <div class="col-5 p-0">
                                        <div>
                                            <div id="myProgress_file_element_adjunta_archivo_rue_sii">
                                                <div id="myBar_file_element_adjunta_archivo_rue_sii" class="file-select-bar"></div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-3 p-0 pl-3">
                                        <p id="count_byte_file_element_adjunta_archivo_rue_sii"></p>
                                    </div>
                                </div>

                            </div>
                            <div class="col-4 justify-content-end pt-2">
                                <p id="count_file_element_adjunta_archivo_rue_sii" class="font-weight-light" style="float: right">Estado </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!--Trmina Popup adjunta documento para rue SII-->
        <!--Popup adjunta documento virtual rue SII -->
        <div class="modal fade modal_opacity" id="modal_adjunta_archivo_virtual_sii" data-backdrop="false" role="dialog">
            <div class="modal-dialog  modal-mediunscreen-sm-down ">
                <div class="modal-content-fullscreen">
                    <div class="modal-header">
                        <h4 style="color: black" class="modal-title">Adjunta archivo virtual SII </h4>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                    </div>
                    <div class="modal-body-fullscreen modal-body">
                        <div class="row row-body-fullscreen">
                            <div class="p-4 w-100">
                                <div class="row p-2" id="content_boton_adjunta_archivo_virtual_sii">
                                    <div class="col-12 p-0 pl-1">
                                        <div class="file-select " id="src-file-">
                                            <input id="file_element_adjunta_archivo_virtual_sii" type="file" multiple="multiple" accept="" style="width: 100px; height: 40px" name="src-file" class="p-1" contente_file="ModalPopupExtender_sube_documento_adjunto" aria-label="Archivo" />
                                        </div>
                                        <a id="save_file_element_adjunta_archivo_virtual_sii" title="Guardar todos los archivos" class="btn  btn-success ml-1" style="opacity: 0; color: white"><i style="color: white" class="fas fa-save "></i> Guardar </a>
                                        <a id="delete_file_element_adjunta_archivo_virtual_sii" title="Elminar todos los archivos cargados" class="btn  btn-danger " style="opacity: 0; color: white"><i style="color: white" class="fal fa-trash-alt "></i> Eliminar </a>
                                        <a id="cancel_file_element_adjunta_archivo_virtual_sii" title="Cancelar guardar archivos" class="btn  btn-warning" style="opacity: 0; color: white"><i style="color: white" class="fas fa-window-close "></i> Cancelar </a>
                                    </div>
                                </div>
                                <div class="paren_element background_upload" id="conten_file_element_adjunta_archivo_virtual_sii" style="overflow: auto; height: 80%">
                                    <div id="content_drop_element_adjunta_archivo_virtual_sii" claas="">
                                    </div>
                                    <table id="table_file_element_adjunta_archivo_virtual_sii" class="table table-striped">
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="error_content_adjunta_archivo_virtual_sii" style="position: relative; width: 100%"></div>
                    <div class=" modal-footer_">
                        <div class="row border_ pt-2 w-100" id="content_pie_title_adjunta_archivo_virtual_sii">
                            <div class="col-8 justify-content-start">
                                <div class="row p-2">
                                    <div class="col-4 p-0">
                                        <div>
                                            <asp:Label ID="Label_progres_bar_file_element_adjunta_archivo_virtual_sii" runat="server" Text="" Style="font-family: Arial; text-align: center; font-size: 20px"></asp:Label>
                                        </div>
                                        <div id="pogres_file_element_contador_adjunta_archivo_virtual_sii" style="text-align: center; font-family: Arial; font-size: 14px">
                                        </div>
                                        <div id="pogres_file_element_porcent_adjunta_archivo_virtual_sii" style="text-align: center; font-family: Arial; font-size: 14px">
                                        </div>
                                    </div>
                                    <div class="col-5 p-0">
                                        <div>
                                            <div id="myProgress_file_element_adjunta_archivo_virtual_sii">
                                                <div id="myBar_file_element_adjunta_archivo_virtual_sii" class="file-select-bar"></div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-3 p-0 pl-3">
                                        <p id="count_byte_file_element_adjunta_archivo_virtual_sii"></p>
                                    </div>
                                </div>

                            </div>
                            <div class="col-4 justify-content-end pt-2">
                                <p id="count_file_element_adjunta_archivo_virtual_sii" class="font-weight-light" style="float: right">Estado </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!--Trmina Popup adjunta documento virtual SII-->
        <!--modal lista documentos SII--->
        <div class="modal fade modal_opacity" id="modal_sii_rues_file" role="dialog" data-backdrop="false">      
            <div class="modal-dialog modal-fullscreen-sm-down">
                <div class="modal-content-fullscreen">
                    <div id="header_modal_sii_rues_file" style="width: 100%">
                        <div class="modal-header" style="max-height:73px">
                            <h6 class="modal-title" style="color: black" >Documentos SII</h6>
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                        </div>
                    </div>      
                    <div class="modal-body-fullscreen" >
                         <div id="tool_bar_sii_rues_file" class="navbar navbar-expand-sm  modal_content_no_back_inferior row">
                            
                       </div>
                        <div class="row row-body-fullscreen">
                            <div class="col-4  modal-body-fullscreen_ pr-0">
                                <div class="conten_gred_border modal_bot_traf_table_" id="content_tabl_lista_sii_rues_file" style="height:100%" >
                                    <table  style="background-color: white"
                                        id="tabl_lista_sii_rues_file"
                                        data-unique-id=""       
                                        data-locale="es-SP">
                                        <thead class="GridviewScrollHeader_line_boot_black">
                                           
                                        </thead>
                                    </table>
                                </div>
                            </div>
                            <div class="col-8 modal-body-fullscreen pl-0">
                                <div class="conten_gred_border" id="content_view_sii_rues_file" style="height: 100%">
                                    <div id="down_lad_file_sii_rues" style="position: relative; width:100%"></div>
                                    <iframe id="Iframe_view_sii_rues_file" runat="server" loading="lazy" frameborder="0" width="100%" scrolling="no" height="100%"></iframe>
                                </div>
                            </div>
                        </div>
                    </div>
                   <div id="error_content_sii_rues_file" style="position: relative; width:100%"></div>
                    <div id="footer_modal_sii_rues_file" >
                        <div class=" modal-footer" style="max-height:73px">
                            <div class="row row-body-fullscreen">
                                <div class="col-6 justify-content-start">
                                    <h6 style="color: black" id="h_title_radicado_sii"></h6>
                                </div>
                                <div class="col-6 justify-content-end">
                                   
                                </div>
                            </div>
                        </div>
                    </div>
                    
                </div>
                
            </div>
        </div>
        <!--Termina popout lista documentos SII-->
        <!--modal registro flujo rue virtual sii--->
        <div class="modal fade modal_opacity" id="modal_rue_registro_vitual_sii" role="dialog" data-backdrop="false">
            <div class="modal-dialog modal-fullscreen-sm-down">
                <div class="modal-content-fullscreen">
                    <div id="header_modal_rue_registro_vitual_sii" style="width: 100%">
                        <div class="modal-header" style="max-height: 73px">
                            <h6 class="modal-title" id="title_registro_vitual_sii_" style="color: black">Registra tarea flujo</h6>
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                        </div>
                    </div>
                    <div class="modal-body-fullscreen">
                        <div class="conten_gred_border modal_bot_traf_table_ pl-2 pt-2" id="content_tabl_rue_registro_vitual_sii" style="height: 100%">
                             <div class="row pt-2 pb-2 ml-0 mr-0">
                                 <div class="col-12 pl-0 pr-0  justify-content-end">
                                      <h6 class="modal-title" id="title_registro_vitual_sii" style="color: black"></h6>
                                </div>
                             </div>
                            <div class="row pt-2 pb-2 ml-0 mr-0">
                                <div class="col-4 pl-0 pr-0 btn-group_">
                                    <span class="h6 font-weight-light control-label">Recibo SII *</span><a data-bs-toggle="tooltip" data-bs-placement="top" title="" data-original-title=""><i class="fal fa-info-circle ml-1"></i></a>
                                </div>
                                <div class="col-8">
                                    <input class="form-control  conten_registro_flujo_tarea_sii form-controls" id="TextBox_recibo_flujo_tarea_sii" disabled="disabled" type="text" maxlength="30" atrib_aleas_c="recibo" atrib_campo_o="1"
                                        atrib_campo_n="recibo" atrib_campo_v="1" atrib_campo_tip="1" atrib_campo_nl="0" atrib_campo_id="0" atrib_name_campo_id="null"
                                        atrib_campo_t="VARCHAR" atrib_campo_tbl="" atrib_campo_drow_destino="null" atrib_name_espace_control="conten_registro_flujo_tarea_sii"
                                        atrib_control_tip_correo="0" atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null" />
                                </div>
                            </div>
                            <div class="row pb-2 ml-0 mr-0">
                                <div class="col-4 pl-0 pr-0">
                                    <span class="h6 font-weight-light">Códogo de Barras / Radicado *</span>
                                </div>
                                <div class="col-8">
                                    <input id="TextBox_codigo_barras_flujo_tarea_sii" type="text" disabled="disabled" class="form-control w-100 conten_registro_flujo_tarea_sii form-controls" maxlength="30" atrib_aleas_c="codigo barras" atrib_campo_o="1"
                                        atrib_campo_n="codigo_barras" atrib_campo_v="1" atrib_campo_tip="1" atrib_campo_nl="0" atrib_campo_id="0" atrib_name_campo_id="null"
                                        atrib_campo_t="VARCHAR" atrib_campo_tbl="" atrib_campo_drow_destino="null" atrib_name_espace_control="conten_registro_flujo_tarea_sii"
                                        atrib_control_tip_correo="0" atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null" />
                                </div>
                            </div>
                            <div class="row pb-2 ml-0 mr-0">
                                <div class="col-4 pl-0 pr-0">
                                    <span class="h6 font-weight-light">Matricula *</span>
                                </div>
                                <div class="col-8">
                                    <input id="TextBox_matricula_flujo_tarea_sii" type="text" class="form-control w-100 conten_registro_flujo_tarea_sii form-controls" maxlength="30" atrib_aleas_c="matricula" atrib_campo_o="1"
                                        atrib_campo_n="matricula" atrib_campo_v="1" atrib_campo_tip="1" atrib_campo_nl="0" atrib_campo_id="0" atrib_name_campo_id="null"
                                        atrib_campo_t="VARCHAR" atrib_campo_tbl="" atrib_campo_drow_destino="null" atrib_name_espace_control="conten_registro_flujo_tarea_sii"
                                        atrib_control_tip_correo="0" atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null" />
                                </div>
                            </div>
                            <div class="row pb-2 ml-0 mr-0">
                                <div class="col-4 pl-0 pr-0">
                                    <span class="h6 font-weight-light">Razón social / Nombre  *</span>
                                </div>
                                <div class="col-8">
                                    <input id="TextBox_razon_social_flujo_tarea_sii" type="text" class="form-control w-100 conten_registro_flujo_tarea_sii form-controls" maxlength="120" atrib_aleas_c="Razón Social" atrib_campo_o="1"
                                        atrib_campo_n="rscocial" atrib_campo_v="1" atrib_campo_tip="1" atrib_campo_nl="0" atrib_campo_id="0" atrib_name_campo_id="null"
                                        atrib_campo_t="VARCHAR" atrib_campo_tbl="" atrib_campo_drow_destino="null" atrib_name_espace_control="conten_registro_flujo_tarea_sii"
                                        atrib_control_tip_correo="0" atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null" />
                                </div>
                            </div>
                            <div class="row pb-2 ml-0 mr-0">
                                <div class="col-4 pl-0 pr-0">
                                    <span class="h6 font-weight-light">Trámites disponibles * </span>
                                </div>
                                <div class="col-8">
                                    <select id="DropDownList_tramites_flujo_tarea_sii"  class="form-select form-select-lg mb-3 w-100 conten_registro_flujo_tarea_sii form-control-drow" atrib_aleas_c="Tramite " atrib_campo_o="1"
                                        atrib_campo_n="id_tramite" atrib_campo_v="1"
                                        atrib_campo_tip="0" atrib_campo_nl="1" atrib_campo_id="0" atrib_name_campo_id="null" atrib_campo_t="VARCHAR" atrib_campo_tbl=""
                                        atrib_campo_drow_destino="" atrib_name_espace_control="conten_registro_flujo_tarea_sii" atrib_control_tip_correo="0"
                                        atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null">
                                    </select>
                                </div>
                            </div>
                            <div class="row pb-2 ml-0 mr-0 d-none" id="row_flujos_tarea_sii">
                                <div class="col-4 pl-0 pr-0">
                                    <span class="h6 font-weight-light">Flujos disponibles  *</span>
                                </div>
                                <div class="col-8">
                                    <select id="DropDownList_flujos_tarea_sii" class="form-select form-select-lg mb-3 w-100 conten_registro_flujo_tarea_sii form-control-drow" atrib_aleas_c="Flujo trabajo" atrib_campo_o="1"
                                        atrib_campo_n="id_flujo" atrib_campo_v="1"
                                        atrib_campo_tip="0" atrib_campo_nl="1" atrib_campo_id="0" atrib_name_campo_id="null" atrib_campo_t="VARCHAR" atrib_campo_tbl=""
                                        atrib_campo_drow_destino="" atrib_name_espace_control="conten_registro_flujo_tarea_sii" atrib_control_tip_correo="0"
                                        atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null">
                                    </select>
                                </div>
                            </div>
                            <div class="row pb-2 ml-0 mr-0 d-none" id="row_actividades_flujo_sii">
                                <div class="col-4 pl-0 pr-0">
                                    <span class="h6 font-weight-light">Actividades disponibles * </span>
                                </div>
                                <div class="col-8">
                                    <select id="DropDownList_actividades_flujo_sii" class="form-select form-select-lg mb-3 w-100 conten_registro_flujo_tarea_sii form-control-drow" atrib_aleas_c="Actividad flujo" atrib_campo_o="1"
                                        atrib_campo_n="id_actividad_fjujo" atrib_campo_v="1"
                                        atrib_campo_tip="0" atrib_campo_nl="1" atrib_campo_id="0" atrib_name_campo_id="null" atrib_campo_t="VARCHAR" atrib_campo_tbl=""
                                        atrib_campo_drow_destino="" atrib_name_espace_control="conten_registro_flujo_tarea_sii" atrib_control_tip_correo="0"
                                        atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null">
                                    </select>
                                </div>
                            </div>
                            <div class="row pb-2  ml-0 mr-0 d-none" id="row_actividades_ruta_sii">
                                <div class="col-4 pl-0 pr-0">
                                    <span class="h6 font-weight-light">Actividades rutas  *</span>
                                </div>
                                <div class="col-8">
                                    <select id="DropDownList_actividades_ruta_sii" class="form-select form-select-lg mb-3 w-100 conten_registro_flujo_tarea_sii form-control-drow" atrib_aleas_c="Usuario de la  tarea" atrib_campo_o="1"
                                        atrib_campo_n="id_actividad" atrib_campo_v="1"
                                        atrib_campo_tip="0" atrib_campo_nl="1" atrib_campo_id="0" atrib_name_campo_id="null" atrib_campo_t="VARCHAR" atrib_campo_tbl=""
                                        atrib_campo_drow_destino="" atrib_name_espace_control="conten_registro_flujo_tarea_sii" atrib_control_tip_correo="0"
                                        atrib_value_campo_old="null" atrib_drow_name_control_id="null" atrib_tom_alow="null">
                                    </select>
                                </div>
                            </div>
                        </div>

                    </div>
                    <div id="error_content_rue_registro_vitual_sii" style="position: relative; width: 100%"></div>
                    <div id="footer_modal_rue_registro_vitual_sii" class="modal-footer " style="max-height: 73px">
                        <div class="justify-content-end">
                            <button type="button" id="Button_registro_actividad_flujo_tarea_sii" class="btn  btn-success d-none" title="Registro actividad rue ">Aceptar</button>
                            <button type="button" id="Button_registro_actividad_flujo_tarea_virtual_sii" class="btn  btn-success d-none" title="Registro actividad virtual ">Aceptar</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
                
          
        <!--Termina popout lista documentos SII-->
        <!--mensaje_progreso evento-->
        <div id="progres_bar" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
            <img src="../workflow/loading.gif" style="vertical-align: middle" alt="Processing" />
            Processing ...
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

    </form>
  
</body>
</html>
