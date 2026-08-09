<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebForm_migra_tramite_sii.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebForm_migra_tramite_sii" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Migracion</title>
     <script src="../js/ui/jquery-3.4.1.min.js"></script>  
    <script src="../js/ui/jquery-ui-1.8.22.custom.js" type="text/javascript"></script>
    <script src="../js/ui/jquery.ui.core.js" type="text/javascript"></script>
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <script src="../js/jquery.contextMenu.js" type="text/javascript"></script>
    <script src="../js/validate_campos.js"></script> 
    <link href="../js/ui/jquery-ui.css" rel="stylesheet" />
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <link href="../Styles/tumb.css" rel="stylesheet" />
    <link href="../ccs/themes/base/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/styleMenu.css" rel="stylesheet" type="text/css" /> 
    <link href="../Styles/Aplicaction.css" rel="stylesheet" />
    <link href="../js/jquery.contextMenu.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/Menu3.css" rel="stylesheet" />
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.11.0/umd/popper.min.js"></script>
    <link href="../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../bootstrap/js/bootstrap.min.js"></script>
    <link href="../Styles/bootra-person.css" rel="stylesheet" />
    <link href="../bootstrap/table/dist/bootstrap-table.min.css" rel="stylesheet" />
    <script src="../bootstrap/table/dist/bootstrap-table.min.js"></script>
    <script src="../bootstrap/table/dist/bootstrap-table-locale-all.js"></script>   
     <script src="../bootstrap/table/dist/extensions/export/bootstrap-table-export.min.js"></script>
    <script src="../bootstrap/table/dist/extensions/export/bootstrap-table-export.js"></script>     
    <script src="https://unpkg.com/tableexport.jquery.plugin/tableExport.min.js"></script>
    <script  src="../Awesome/js/all.js"></script>    
    <link href="../Awesome/css/fontawesome.css" rel="stylesheet"/>
    <link href="../Awesome/css/brands.css" rel="stylesheet"/>
    <link href="../Awesome/css/solid.css" rel="stylesheet"/>
    <script  src="../Awesome/js/brands.js"></script>
    <script  src="../Awesome/js/solid.js"></script>
    <script  src="../Awesome/js/fontawesome.js"></script>
    <script src="../js/Integracionccv/WebForm_migra_tramite_sii.js"></script>
</head>
<body>
    <form id="form1" runat="server">
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
                    alert(" Funcion CheckStatus asincrona WebForm_migra_tramite_sii" + err.message);
                }
                finally {
                    progres_hiden('progres_bar');
                }
            }
            </script>
    <div id="content_general" style="height:98%; display:block;  background-color:white; border-top: inset 1px solid #ccc;">
        <div id="div_title" style="text-align: center">
            <asp:Label ID="label_title" runat="server" Text="Migración SI Imagenes" Style="font-size: 12px; font-family: Arial; color: black; font-weight: 700"></asp:Label>
        </div>
        <div id="div_tab_content_resp_general" style="height: auto; margin-left: 15px; margin-right: 15px">
            <ul class="tab" style="background-color: white; height: auto">
                <li><a style="font-family: Arial" href="javascript:void(0)" class="tablinks__ active_vis__boot_da" onclick="openCity__(event, 'div_tab_migra_fecha')" id="default_formal"><i class="fad fa-file"></i> Migracion </a></li>
                <li><a style="font-family: Arial" href="javascript:void(0)" class="tablinks__" onclick="openCity__(event, 'div_tab_consulta_migracion')" id="default_confirmar"><i class="fad fa-reply"></i> Consulta </a></li>
            </ul>
        </div>
        <div id="div_tab_migra_fecha" class="tabcontent___boot_da" style="margin-left:15px; margin-right:15px; border:none; height:100%; display:block">
            <div id="conten_fechas" class=" pt-3" style="width:50%; height:100%">
                <div class="row pt-2">
                    <div class="col-12 text-center">
                        <span>Migración por fechas</span>
                    </div>
                </div>
                <div class="row pt-2">
                <div class="col-2 p-0">
                    <input type="radio" id="content_rango_fecha" name="fav_language" checked="checked"  value="content_rango_fecha"/>
                </div>
                <div class="col-3 p-0">
                    
                    <asp:TextBox ID="TextBoxFECHA_EXTREMA_FINAL_INICIAL" runat="server" Style="width: 95%" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                    <asp:CalendarExtender ID="TextBoxFECHA_FINAL_INICIAL_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_FINAL_INICIAL_CalendarExtender" TargetControlID="TextBoxFECHA_EXTREMA_FINAL_INICIAL" Format='yyyy-MM-dd' PopupButtonID="ImageButton_FINAL_INICIAL" />
                </div>
                <div class="col-2 p-0">
                    <button class="ml-1 btn border-0" id="ImageButton_FINAL_INICIAL" type="button">
                        <i class="fad fa-calendar-alt fa-1x"></i>
                    </button>
                </div>
                <div class="col-3 p-0">
                    <asp:TextBox ID="TextBoxFECHA_EXTREMA_FINAL_FINAL" runat="server" Style="width: 95%" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                    <asp:CalendarExtender ID="TextBoxFECHA_EXTREMA_FINAL_FINAL_CalendarExtender" runat="server" BehaviorID="TextBoxFECHA_EXTREMA_FINAL_FINAL_CalendarExtender" TargetControlID="TextBoxFECHA_EXTREMA_FINAL_FINAL" Format='yyyy-MM-dd' PopupButtonID="ImageButtonfechaextremafinfin" />
                </div>
                <div class="col-2 p-0">
                    <button class="ml-1 btn border-0" id="ImageButtonfechaextremafinfin" type="button">
                        <i class="fad fa-calendar-alt fa-1x"></i>
                    </button>
                </div>
                </div>
                 <div class="row pt-2">
                    <div class="col-12 text-center">
                        <span> Migración por código de barras</span>
                    </div>
                </div>
                <div class="row pt-3">
                    <div class="col-2 p-0">
                        <input type="radio" id="content_radicado" name="fav_language" value="content_radicado"/>
                    </div>
                    <div class="col-10 p-0">
                        <input id="text_codigo_barras" type="text"  class="custom-control"/>
                    </div>
                </div>
            </div>
            <div id="conte_foter" class="modal-footer">
                <input id="Button_migrar" type="button" value="Aceptar"  class="btn btn-success"  onclick="migra_trammite_sii();" />
            </div>
        </div>
        <div id="div_tab_consulta_migracion" class="tabcontent___boot_da" style="height:auto; margin-left:15px; margin-right:15px; border:none">
             <div id="div_content_consulta" class=" pt-2" style="width:100%; height:100%">
                 <div id="div_contenido_controles_consulta" style="width:50%">
                     <div class="row pt-2">
                         <div class="col-2 p-0">
                             <span>Fechas extremas</span>
                         </div>
                         <div class="col-3 p-0 pl-2">

                             <asp:TextBox ID="TextBox_FINAL_INICIAL" runat="server" Style="width: 95%" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                             <asp:CalendarExtender ID="TextBoxFECHA_FINAL_INICIAL_CalendarExtender_" runat="server" BehaviorID="TextBoxFECHA_FINAL_INICIAL_CalendarExtender_" TargetControlID="TextBox_FINAL_INICIAL" Format='yyyy-MM-dd' PopupButtonID="ImageButton_FINAL_INICIAL_" />
                         </div>
                         <div class="col-2 p-0">
                             <button class="ml-1 btn border-0" id="ImageButton_FINAL_INICIAL_" type="button">
                                 <i class="fad fa-calendar-alt fa-1x"></i>
                             </button>
                         </div>
                         <div class="col-3 p-0">
                             <asp:TextBox ID="TextBox_FINAL_FINAL" runat="server" Style="width: 95%" onkeypress="return validate_fecha(event,this)" placeholder="0000 00 00"></asp:TextBox>
                             <asp:CalendarExtender ID="TextBox_FINAL_FINAL_CalendarExtender" runat="server" BehaviorID="TextBox_FINAL_FINAL_CalendarExtender" TargetControlID="TextBox_FINAL_FINAL" Format='yyyy-MM-dd' PopupButtonID="ImageButtonfechaextremafinfin_" />
                         </div>
                         <div class="col-2 p-0">
                             <button class="ml-1 btn border-0" id="ImageButtonfechaextremafinfin_" type="button">
                                 <i class="fad fa-calendar-alt fa-1x"></i>
                             </button>
                         </div>
                     </div>
                     <div class="row pt-2">
                         <div class="col-2 p-0">
                             <span>Código de barras</span>
                         </div>
                         <div class="col-10 p-0 pl-2">
                             <input id="text_codigo_consulta" type="text" class="custom-control" />
                         </div>
                     </div>
                 </div>
                 <div id="div_content_tabla" >
                     <table
                         id="table"
                         data-height="200"
                         data-pagination="true"
                         data-page-list="[10, 25, 50, 100, all]"
                         data-show-export="true"
                         data-toggle="table"
                         data-id-field="id_migra_registro"
                         data-search="true"
                         data-locale="es-SP">
                         <thead>
                             <tr>
                                 <th data-field="id_migra_registro" data-visible="false" style="display:none">id_migra_registro</th>
                                 <th data-field="codigo_sii" data-sortable="true" data-sort-name="codigo_sii" data-sort-order="desc">CODIGO_SII</th>
                                 <th data-field="fecha_migracion">FECHA_MIGRACION</th>
                                 <th data-field="usuario_migracion">USUARIO_MIGRACION</th>
                                 <th data-field="imagenes">NUMERO_IMAGENES</th>
                                 <th data-field="matricula">MATRICULA</th>
                                 <th data-field="nit_identificacion">NIT_IDENTIFICACION</th>
                                 <th data-field="recibo_sii">RECIBO_SII</th>
                             </tr>
                         </thead>
                     </table>
                 </div>
             </div>
             
              <div id="conte_foter_consulta" class="modal-footer">
                  <input id="Button_consultar" type="button" value="Aceptar"  class="btn btn-success" onclick="lista_registro_sii_migrados();" />  
 
              </div>
        </div>
    </div>
          <div id="progres_bar" class="load_ding" style="position: fixed; text-align: center; display: none; width: 150px; width: 200px">
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
        <!--PROGRES-->
        <div id="Divpro_gres_bar">
            <asp:Panel ID="Panel_pro_gres_bar" runat="server" Style="display:none; color: White; width:30%; height:auto" CssClass="border_superior_inferior_radius_blanco">
                <asp:ModalPopupExtender ID="ModalPopupExtender_edition_pro_gres_bar" runat="server" BehaviorID="Panel_pro_gres_bar_ModalPopupExtender" TargetControlID="ButtonSalir_pro_gres_bar" BackgroundCssClass="FondoAplicacion"
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
    </form>
    <script  accesskey="javascript" type="text/javascript">
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
            auto_zise_page();
            
        }
       
    </script>
</body>
</html>
