$(document).ready(function () {
    $.fn.inicio = function () {
        
        $('.close').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });    
        auto_zise_iframe();
        auto_zise_popup_detalle_respuesta();
        auto_zise_popup_respuesta();   
        auto_zise_popup_edicion_word_html();
        auto_zise_popup_gestion_externo();
        auto_zise_popup_solicitud_aprobacion();
        auto_zise_popup_registro_colaboracion();
        auto_zise_popup_compartir_documento();
        auto_zise_popup_detalle_transacciones();       
    }
    
    
   
})

$(window).on("load", function () {
    var elment = document.getElementsByClassName("da_event_captive");
    if (elment) {
        for (var i = 0; i < elment.length; i++) {
            elment[i].addEventListener("click", event_click, false);
        }
    }
    window.addEventListener("resize", rezize_event);
    ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100001);
    ShowModalPopup("ModalPopupExtender_solicitud_aprobacion_backgroundElement", "Panel_solicitud_aprobacion", 100001);
    //inicializa el control token
    toke_ini('tokenize-callable-demo_respuesta');
    //asigna el correo eletronico del peticionarion al tokenize
    asig_correo_token('tokenize-callable-demo_respuesta');
    //agrega evento para agregar nuevos token personalizados
    set_tokenize_add_event_valid('tokenize-callable-demo_respuesta');
    //agrega evento para eliminar token
    set_tokenize_delete_event('tokenize-callable-demo_respuesta');
    if (COORREO_INI !== "") {
        add_regitro_correo_service(COORREO_INI);
    }
    //inicializa el control token
    toke_ini('tokenize-callable-demo_respuesta_');
    //asigna el correo del usuario peticionario al token
    asig_correo_token('tokenize-callable-demo_respuesta_');
    //agrega evento para agregar nuevos token personalizados
    set_tokenize_add_event_valid('tokenize-callable-demo_respuesta_');
    //agrega evento para eliminar token
    set_tokenize_delete_event('tokenize-callable-demo_respuesta_');
    //$("Button_confirmar_envio_respuesta").on("click", asig_array_tokenize('tokenize-callable-demo_respuesta_'));
    //document.getElementsByClassName("btn btn-primary nnnnn")[0].addEventListener("click", function () { asig_array_tokenize('tokenize-callable-demo_respuesta_') }, true);
    //inicializa el control token
    toke_ini('tokenize-callable-demo_respuesta__');
    //asigna el correo del usuario peticionario al token
    asig_correo_token('tokenize-callable-demo_respuesta__');
    //agrega evento para agregar nuevos token personalizados
    set_tokenize_add_event_valid('tokenize-callable-demo_respuesta__');
    //agrega evento para eliminar token
    set_tokenize_delete_event('tokenize-callable-demo_respuesta__');
    //document.getElementById("Button_notificar_correo").addEventListener("click", function () { asig_array_tokenize('tokenize-callable-demo_respuesta__') }, false);
    toke_ini_solicitud('tokenize-callable-demo_respuesta___');
    $('.tokenize-callable-demo_respuesta___').on('tokenize:tokens:added', function (e, value, text) {
        ITEMS_DATOS_TOKENIZE_5.push({ text: text, value: value });
    });
    $('.tokenize-callable-demo_respuesta___').on('tokenize:tokens:remove', function (e, value) {
        delete_array_tokenize_solicitud(value);
    });
});
function rezize_event() {
    try {
        auto_zise_popup_respuesta();
        auto_zise_popup_gestion_externo();
        auto_zise_popup_detalle_transacciones();
        auto_zise_popup_detalle_respuesta();
        auto_zise_popup_solicitud_aprobacion();
        auto_zise_popup_registro_colaboracion();
        asigna_datos_heig_with();
    } catch (ex) {
        alert(ex.message + " Función rezize_event")
    }
}
function ShowModalPopup(modalPopupId_, name_panel, zIndex) {
    try {
        var modalPopupId = document.getElementById(modalPopupId_);
        var name_panel_ = document.getElementById(name_panel);
        var modalPopupBehavior = modalPopupId;
        zIndex = typeof (zIndex) != 'undefined' ? zIndex : null;
        if (zIndex != null) {
            modalPopupBehavior.style.zIndex = zIndex;
            name_panel_.style.zIndex = zIndex + 1;
        }
    }
    catch (ex) {
        alert('Exception in ShowModalPopup: ' + ex.message);
    }
}
function event_click(e) {
    try {
        var k = e.currentTarget.value;
        var d = document.getElementById(k);
        d.click();
        e.preventDefault();
    }
    catch (err) {
        alert(err.message + " Funcion event_click");
    }
}
var COORREO_INI="";
var ITEMS_DATOS_TOKENIZE_2 = new Array();  //GUARDA LOS ITEM SELECIONANDO EN TEX SELECTOR
var ITEMS_DATOS_TOKENIZE_3 = new Array();  //GUARDA LOS ITEM SELECIONANDO EN TEX SELECTOR
var ITEMS_DATOS_TOKENIZE_4 = new Array();  //GUARDA LOS ITEM SELECIONANDO EN TEX SELECTOR
var ITEMS_DATOS_TOKENIZE_5 = new Array();  //GUARDA LOS ITEM SELECIONANDO EN TEX SELECTOR
function set_tokenize_add_event_valid(name_token) {
    try {
        $('.' + name_token).on('tokenize:tokens:added', function (e, value, text) {
            var ident = validator_cuenta_correo(value);
            if (ident) {

                if (name_token == "tokenize-callable-demo_respuesta_simple") {
                    ITEMS_DATOS_TOKENIZE_6.push({ text: text, value: value });
                }
                if (name_token == "tokenize-callable-demo_respuesta_") {
                    ITEMS_DATOS_TOKENIZE_3.push({ text: text, value: value });
                }
                if (name_token == "tokenize-callable-demo_respuesta_k") {
                    ITEMS_DATOS_TOKENIZE_8.push({ text: text, value: value });
                }
                if (name_token == "tokenize-callable-demo_respuesta__") {
                    ITEMS_DATOS_TOKENIZE_4.push({ text: text, value: value });
                }
                var k = add_regitro_correo_service(text);
            } else {
                var token = document.getElementsByClassName("token")
                if (token) {
                    var confirma = confirm("El correo eletrónico informado nos es correcto, si desea agregarlo presione aceptar");
                    if (confirma == false) {
                        for (var i = 0; i < token.length; i++) {
                            if (token[i].innerText == value) {
                                token[i].parentNode.removeChild(token[i]);
                            }
                        }
                        return false;
                    } else {

                        if (name_token == "tokenize-callable-demo_respuesta_simple") {
                            ITEMS_DATOS_TOKENIZE_6.push({ text: text, value: value });
                        }
                        if (name_token == "tokenize-callable-demo_respuesta_") {
                            ITEMS_DATOS_TOKENIZE_3.push({ text: text, value: value });
                        }
                        if (name_token == "tokenize-callable-demo_respuesta__") {
                            ITEMS_DATOS_TOKENIZE_4.push({ text: text, value: value });
                        }
                        if (name_token == "tokenize-callable-demo_respuesta_k") {
                            ITEMS_DATOS_TOKENIZE_8.push({ text: text, value: value });
                        }
                        return false;
                    }

                }

            }

        });
    } catch (ex) { alert(ex + " funcion set_tokenize_add_event_valid") }
}

function set_tokenize_delete_event(name_token) {
    try {
        $('.' + name_token).on('tokenize:tokens:remove', function (e, value) {
            delete_array_tokenize(value, name_token);
        });
    } catch (ex) { alert(ex + " funcion set_nize_delete_event") }
}
function asig_correo_token(name_token) {
    try {
        if (COORREO_INI == "") {
            var id_respuest = document.getElementById("Hidden_id_respuesta").value;
            solicita_correorespuesta_documento_tokenize(id_respuest);
            if (COORREO_INI !== "") {
                $('.' + name_token).tokenize2().trigger('tokenize:tokens:add', [COORREO_INI, COORREO_INI, true]);
                if (name_token == "tokenize-callable-demo_respuesta") { 
                    ITEMS_DATOS_TOKENIZE_2.push({ text: COORREO_INI, value: COORREO_INI });
                }
                if (name_token == "tokenize-callable-demo_respuesta_") {
                    ITEMS_DATOS_TOKENIZE_3.push({ text: COORREO_INI, value: COORREO_INI });
                }
                if (name_token == "tokenize-callable-demo_respuesta__") {
                    ITEMS_DATOS_TOKENIZE_4.push({ text: COORREO_INI, value: COORREO_INI });
                }
            }
        } else {
            $('.' + name_token).tokenize2().trigger('tokenize:tokens:add', [COORREO_INI, COORREO_INI, true]);
            var exit = "";
            if (name_token == "tokenize-callable-demo_respuesta") {              
                exit = verifi_token_array(ITEMS_DATOS_TOKENIZE_2, COORREO_INI);
                if (exit !== "YES") {
                    ITEMS_DATOS_TOKENIZE_2.push({ text: COORREO_INI, value: COORREO_INI });
                }
                
            }
            if (name_token == "tokenize-callable-demo_respuesta_") {
                exit = verifi_token_array(ITEMS_DATOS_TOKENIZE_3, COORREO_INI);
                if (exit !== "YES") {
                    ITEMS_DATOS_TOKENIZE_3.push({ text: COORREO_INI, value: COORREO_INI });
                }
            }
            if (name_token == "tokenize-callable-demo_respuesta__") {
                exit = verifi_token_array(ITEMS_DATOS_TOKENIZE_4, COORREO_INI);
                if (exit !== "YES") {
                    ITEMS_DATOS_TOKENIZE_4.push({ text: COORREO_INI, value: COORREO_INI });
                }
            }
        }
        
    } catch (ex) { alert(ex + " funcion asig_correo_token") }
}
function verifi_token_array(token, value) {
    try {
        for (var i = 0; i < token.length; i++) {
            if (token[i].text == value) {
                return "YES";
            } 
        }
        return "";
    } catch (ex) { alert(ex.message);}
}
function toke_ini_solicitud(name_token) {
    try {
        $('.' + name_token).tokenize2({
            placeholder: "Para relacionar los usuarios puede digitar el nombre del usuario o el cargo del usuario...",
            dataSource: function (search, object) {
                $.ajax('../webservice/WebServiceWorkflow.asmx/GetLista_usuarios_workflow_z2', {
                    data: "{'DName':'" + search + "'}",
                    dataType: 'json',
                    type: "POST",
                    traditional: true,
                    processData: false,
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        var $items = [];
                        $.each(data.d, function (k, v) {
                            $items.push(v);
                        });
                        object.trigger('tokenize:dropdown:fill', [$items]);

                    }
                });
            }

        });
    } catch (ex) { alert(ex.message + " funcion toke_ini_solicitud ") }
}
function toke_ini(name_token) {
    try {
        $('.' + name_token).tokenize2({
            placeholder: "digita correos electrónicos",
            tokensAllowCustom: true,
            zIndexMargin: 10001,
            dataSource: function (search, object) {
                $.ajax('../webservice/WebServiceRadicacion.asmx/GetLista_correos_usuarios_gestion_tokenize', {
                    data: "{'DName':'" + search + "'}",
                    dataType: 'json',
                    type: "POST",
                    traditional: true,
                    processData: false,
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        var $items = [];
                        $.each(data.d, function (k, v) {
                            $items.push(v);
                        });
                        try {
                            object.trigger('tokenize:dropdown:fill', [$items]);

                        }
                        catch (ex) { alert(ex + " Funcion toke_ini"); }
                    }
                });

            }

        });
    } catch (ex) {
        alert(ex.message);
    }
}
function delete_array_tokenize_solicitud(value_id) {
    try {
        for (var i = 0; i < ITEMS_DATOS_TOKENIZE_5.length; i++) {
            if (ITEMS_DATOS_TOKENIZE_5[i].value == value_id) {
                ITEMS_DATOS_TOKENIZE_5.splice(i, 1);
                i = ITEMS_DATOS_TOKENIZE_5.length;
            }
        }
    } catch (err) {
        alert(err.message + " Funcion delete_array_tokenize_solicitud");
    }
}
function delete_array_tokenize(value_id, name_token) {
    try {
        if (name_token == "tokenize-callable-demo_respuesta") {
            for (var i = 0; i < ITEMS_DATOS_TOKENIZE_2.length; i++) {
                if (ITEMS_DATOS_TOKENIZE_2[i].value == value_id) {
                    ITEMS_DATOS_TOKENIZE_2.splice(i, 1);
                    
                }
            }
        }
        if (name_token == "tokenize-callable-demo_respuesta_") {
            for (var i = 0; i < ITEMS_DATOS_TOKENIZE_3.length; i++) {
                if (ITEMS_DATOS_TOKENIZE_3[i].value == value_id) {
                    ITEMS_DATOS_TOKENIZE_3.splice(i, 1);
                    i = ITEMS_DATOS_TOKENIZE_3.length;
                }
            }
        }
        if (name_token == "tokenize-callable-demo_respuesta__") {
            for (var i = 0; i < ITEMS_DATOS_TOKENIZE_4.length; i++) {
                if (ITEMS_DATOS_TOKENIZE_4[i].value == value_id) {
                    ITEMS_DATOS_TOKENIZE_4.splice(i, 1);
                    
                }
            }
        }
       
    } catch (err) {
        alert(err.message + " Funcion delete_array_tokenize");
    }
}
function asig_array_tokenize_solicitud() {
    try {
        document.getElementsByName("Hidden_text_user").value = "";
        for (var i = 0; i < ITEMS_DATOS_TOKENIZE_4.length; i++) {
            if (i == 0) {
                document.getElementsByName("Hidden_text_user").value = '|' + ITEMS_DATOS_TOKENIZE_4[i].value + '|| ' + ITEMS_DATOS_TOKENIZE_4[i].text + ',';
            } else {
                document.getElementsByName("Hidden_text_user").value = document.getElementsByName("Hidden_text_user").value + '|' + ITEMS_DATOS_TOKENIZE_4[i].value + '|| ' + ITEMS_DATOS_TOKENIZE_4[i].text + ',';
            }
        }
    } catch (err) {
        alert(err.message + " Funcion asig_array_tokenize");
    }
}
function asig_array_tokenize(name_token) {
    try {
        if (name_token == "tokenize-callable-demo_respuesta") {
            document.getElementById("Hidden_text_user_correo").value = "";
            for (var i = 0; i < ITEMS_DATOS_TOKENIZE_2.length; i++) {
                if (i == 0) {
                    document.getElementById("Hidden_text_user_correo").value = ITEMS_DATOS_TOKENIZE_2[i].text;
                } else {
                    document.getElementById("Hidden_text_user_correo").value = document.getElementById("Hidden_text_user_correo").value + ',' + ITEMS_DATOS_TOKENIZE_2[i].text;
                }
            }
           
        }
        if (name_token == "tokenize-callable-demo_respuesta_") {
            document.getElementById("Hidden_text_user_correo").value = "";
            for (var i = 0; i < ITEMS_DATOS_TOKENIZE_3.length; i++) {
                if (i == 0) {
                    document.getElementById("Hidden_text_user_correo").value = ITEMS_DATOS_TOKENIZE_3[i].text;
                } else {
                    document.getElementById("Hidden_text_user_correo").value = document.getElementById("Hidden_text_user_correo").value + ',' + ITEMS_DATOS_TOKENIZE_3[i].text;
                }
            }
            
        }
        if (name_token == "tokenize-callable-demo_respuesta__") {
            document.getElementById("Hidden_text_user_correo").value = "";
            for (var i = 0; i < ITEMS_DATOS_TOKENIZE_4.length; i++) {
                if (i == 0) {
                    document.getElementById("Hidden_text_user_correo").value = ITEMS_DATOS_TOKENIZE_4[i].text;
                } else {
                    document.getElementById("Hidden_text_user_correo").value = document.getElementById("Hidden_text_user_correo").value + ',' + ITEMS_DATOS_TOKENIZE_4[i].text;
                }
            }
            
        }
       
    } catch (err) {
        alert(err.message + " Funcion asig_array_tokenize");
    }
}
function validator_cuenta_correo(value_correo) {
    try {
        var mailformat = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
        if (value_correo.match(mailformat)) {
            return true;
        }
        else {

            return false;
        }
    } catch (ex) {
        alert(ex.message + " funcion validator_cuenta_correo")
    }
}
function add_regitro_correo_service(correo) {
    $.ajax({
        async: false,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "../webservice/WebServiceRadicacion.asmx/Add_correos_cache_tokenize",
        data: "{'DName':'" + correo + "'}",
        dataType: "json",
        success: function (data) {
            if (data.d) {
                //add_regitro_correo_service = data.d;
            } else {
                //add_regitro_correo_service = "YES";
            }
        },
        error: function (result) {
            alert("Error......" + result);

        }
    });
}
function solicita_correorespuesta_documento_tokenize(id_respuesta) {
    $.ajax({
        async: false,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "../webservice/WebServiceRadicacion.asmx/GetLista_correos_respuesta_documento_tokenize",
        data: "{'DName':'" + id_respuesta + "'}",
        dataType: "json",
        success: function (data) {
            if (data.d) {
                COORREO_INI = data.d;
                return data.d;
                //add_regitro_correo_service = data.d;
            } else {
                return "";
                COORREO_INI = "";
                //add_regitro_correo_service = "YES";
            }
        },
        error: function (result) {
            //alert("Error......" + result);
            return "";
        }
    });
}
function Solicitud_aprobacion_tokenize() {
    try {
        if (ITEMS_DATOS_TOKENIZE_5.length == 0) {
            alert("Debe selecionar los usuarios a compartir");
            return false;
        }
        var valParam = JSON.stringify(ITEMS_DATOS_TOKENIZE_5);
        var para_meter_ca = new Array();
        var asunto_ = "";
        var nota_ = document.getElementsByName('TextBox_nota_aprobacion');
        var nivel_urgencia_solicitud_ = document.getElementsByName('DropDownList_prioridad_solicitud');
        var tipo_solicitud_ = "";
        var fecha_limite_ = document.getElementsByName("TextBox_fecha_limite_solicitud");
        var radicado_relacionado_ = "";
        var id_usuario_propietario_ = document.getElementById("Hidden_id_respuesta").value;
        var matri_documentos_ = "";
        para_meter_ca.push({ asunto_: asunto_, nota_: nota_[0].value, nivel_urgencia_solicitud_: nivel_urgencia_solicitud_[0].value, tipo_solicitud_: tipo_solicitud_, radicado_relacionado_: radicado_relacionado_, id_usuario_propietario_: id_usuario_propietario_, matri_documentos_: matri_documentos_, fecha_limite_: fecha_limite_[0].value });
        var serialice = JSON.stringify(para_meter_ca);
        $.ajax('../webservice/WebServiceWorkflow.asmx/Set_Registra_solicitud_aprobacion', {
            data: "{'item_user':'" + valParam + "'," + "'parameter':'" + serialice + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d) {
                    var split = data.d.split("|");
                    if (split[0] !== "YES") {
                        alert(data.d);
                    } else {
                        
                        document.getElementById("Button_cancelar_registro").click();
                    }
                } 
            }
        });
    } catch (ex) { alert(ex.message + " funcion Solicitud_aprobacion_tokenize"); }
}
function dat_togle_bot(id, mensaje) {
    try {

    } catch (ex) { alert(ex.message + " funcion dat_togle") }
}
function prevent_hident(event) {
    try {
        event.preventDefault();
      
    }
    catch (err) {
        alert(err.message + " Funcion prevent_hident");
    }
}
function display_default() {
    document.getElementById('conten_general_respuesta_formal').style.display ="block";
}
function openCity_post(default_tab) {
    var i, tabcontent, tablinks;
    var cityName="";
    tabcontent = document.getElementsByClassName("tabcontent");
    /*for (i = 0; i < tabcontent.length; i++) {
        var p = tabcontent[i];
        if (tabcontent[i].hidden == "true") {
            cityName = tabcontent[i].id;
        }
    }*/
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].hidden = "false";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    if (document.getElementById("Hidden_select_tab").value == "") {
        document.getElementById("Hidden_select_tab").value = default_tab;
    }
    cityName = document.getElementById("Hidden_select_tab").value;
    document.getElementById(cityName).style.display = "block";
    
    
}
function openCity_(evt, cityName) {
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent_");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks_");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active_", "");
    }
    document.getElementById(cityName).style.display = "block";
    document.getElementById("Hidden_select_tab_").value = cityName;
    evt.currentTarget.className += " active_";

}
function openCity__(evt, cityName) {
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent__");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks__");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active_vis_", "");
    }
    document.getElementById(cityName).style.display = "block";

    evt.currentTarget.className += " active_vis_";

}
function service_usuarios_gestion_text(name_texbox) {
    function split(val) {
        return val.split(/,\s*/);
    }
    function extractLast(term) {
        return split(term).pop();
    }
    $("#" + name_texbox)
        .on("keydown", function (event) {
            if (event.keyCode === $.ui.keyCode.TAB &&
                $(this).autocomplete("instance").menu.active) {
                event.preventDefault();
            }
        })
        .autocomplete({
            source: function (request, response) {
                var param = { keyword: $('#' + name_texbox).val() };
                $.ajax({
                    url: "../webservice/WebServiceRadicacion.asmx/GetLista_usuarios_gestion",
                    data: "{'DName':'" + document.getElementById(name_texbox).value + "'}",
                    dataType: "json",
                    type: "POST",
                    traditional: true,
                    processData: false,
                    contentType: "application/json; charset=utf-8",
                    //dataFilter: function (data) { return data; },
                    success: function (data) {
                        term: extractLast(request.term)
                        response($.ui.autocomplete.filter(
               data.d, extractLast(request.term)));

                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus);
                    }
                });
            },


            focus: function () {
                // prevent value inserted on focus
                return false;
            },
            select: function (event, ui) {
                var terms = split(this.value);
                // remove the current input
                terms.pop();
                // add the selected item
                terms.push(ui.item.value);
                // add placeholder to get the comma-and-space at the end
                terms.push("");
                this.value = terms.join(", ");
                return false;
            }

            , minLength: 3, max: 10, scroll: true
        });
}
function service_usuarios_gestion(name_texbox) {
    function split(val) {
        return val.split(/,\s*/);
    }
    function extractLast(term) {
        return split(term).pop();
    }
    $("#" + name_texbox)
        .on("keydown", function (event) {
            if (event.keyCode === $.ui.keyCode.TAB &&
                $(this).autocomplete("instance").menu.active) {
                event.preventDefault();
            }
        })
        .autocomplete({
            source: function (request, response) {
                var param = { keyword: $('#' + name_texbox).val() };
                $.ajax({
                    url: "../webservice/WebServiceRadicacion.asmx/GetLista_correos_usuarios_gestion",
                    data: "{'DName':'" + document.getElementById(name_texbox).value + "'}",
                    dataType: "json",
                    type: "POST",
                    traditional: true,
                    processData: false,
                    contentType: "application/json; charset=utf-8",
                    //dataFilter: function (data) { return data; },
                    success: function (data) {
                        term: extractLast(request.term)
                        response($.ui.autocomplete.filter(
               data.d, extractLast(request.term)));

                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus);
                    }
                });
            },


            focus: function () {
                // prevent value inserted on focus
                return false;
            },
            select: function (event, ui) {
                var terms = split(this.value);
                // remove the current input
                terms.pop();
                // add the selected item
                terms.push(ui.item.value);
                // add placeholder to get the comma-and-space at the end
                terms.push("");
                this.value = terms.join(", ");
                return false;
            }

            , minLength: 3, max: 10, scroll: true
        });
}

function asigna_datos_heig_with() {
    try {
        var espacio_iframe = 420;
        var hidenpadre = 0;
        var with_frame = 420;
        if (window.innerHeight) {
            //navegadores basados en mozilla 
            espacio_iframe = window.innerHeight;
            with_frame = window.innerWidth;
        } else {
            if (document.body.clientHeight) {
                //Navegadores basados en IExplorer, es que no tengo innerheight 
                espacio_iframe = document.body.clientHeight;
                with_frame = document.body.clientWidth;
            } else {
                //otros navegadores y iframe
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

            }
        }
        document.getElementById("Hidden_height").value = espacio_iframe - 30;
        document.getElementById("Hidden_width").value = with_frame - 30;

    }
    catch (err) {
        alert(err.message + " funcion asigna_datos_heig_with " + err.message);
    }
}

function promp_respuesta(mensaje) {
    try {
        var mensaje = confirm(mensaje);
        document.getElementById("Hidden_resp").value = "0";
        //Detectamos si el usuario acepto el mensaje
        if (mensaje) {
            document.getElementById("Hidden_resp").value = "1";
        }
            //Detectamos si el usuario denegó el mensaje
        else {
            document.getElementById("Hidden_resp").value = "0";
        }
    } catch (err) {
        alert(err.message + " funcion promp_respuesta " + err.message);
    }
}
function prom_respuesta_personalizado(mensaje, name_hiden) {
    try {
        var mensaje = confirm(mensaje);
        document.getElementById(name_hiden).value = "0";
        //Detectamos si el usuario acepto el mensaje
        if (mensaje) {
            document.getElementById(name_hiden).value = "1";
        }
            //Detectamos si el usuario denegó el mensaje
        else {
            document.getElementById(name_hiden).value = "0";
        }
    } catch (e) { alert("Funcion prom_respuesta_personalizado " + e.message) }
}
function auto_zise_popup_respuesta() {
    var espacio_iframe = 420;
    var hidenpadre = 0;
    var with_frame = 420;
    if (window.innerHeight) {
        //navegadores basados en mozilla 
        espacio_iframe = window.innerHeight;
        with_frame = window.innerWidth;
    } else {
        if (document.body.clientHeight) {
            //Navegadores basados en IExplorer, es que no tengo innerheight 
            espacio_iframe = document.body.clientHeight;
            with_frame = document.body.clientWidth;
        } else {
            //otros navegadores y iframe
            //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

        }
    }

    $('#contenedor_general_respuesta').css("height", (espacio_iframe - 30) + "px");
}
function auto_zise_popup_edicion_word_html() {
    var espacio_iframe = 420;
    var hidenpadre = 0;
    var with_frame = 420;
    if (window.innerHeight) {
        //navegadores basados en mozilla 
        espacio_iframe = window.innerHeight;
        with_frame = window.innerWidth;
    } else {
        if (document.body.clientHeight) {
            //Navegadores basados en IExplorer, es que no tengo innerheight 
            espacio_iframe = document.body.clientHeight;
            with_frame = document.body.clientWidth;
        } else {
            //otros navegadores y iframe
            //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

        }
    }


    $('#contenido_procesa_edition_html').css("height", (espacio_iframe - 40) + "px");
    $('#ifm_html_editor').css("height", (espacio_iframe - 42) + "px");
   
}
function auto_zise_popup_gestion_externo() {
    var espacio_iframe = 420;
    var hidenpadre = 0;
    var with_frame = 420;
    if (window.innerHeight) {
        //navegadores basados en mozilla 
        espacio_iframe = window.innerHeight;
        with_frame = window.innerWidth;
    } else {
        if (document.body.clientHeight) {
            //Navegadores basados en IExplorer, es que no tengo innerheight 
            espacio_iframe = document.body.clientHeight;
            with_frame = document.body.clientWidth;
        } else {
            //otros navegadores y iframe
            //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

        }
    }


   
    $('#ModalPopupExtender_valiacion_plantilla').css("height", (espacio_iframe - 40) + "px");
    $('#Iframe_validacion_plantilla_').css("height", (espacio_iframe - 42) + "px");
}
function auto_zise_popup_detalle_transacciones() {
    try {
        var espacio_iframe = 420;
        var hidenpadre = 0;
        var with_frame = 420;
        if (window.innerHeight) {
            //navegadores basados en mozilla 
            espacio_iframe = window.innerHeight;
            with_frame = window.innerWidth;
        } else {
            if (document.body.clientHeight) {
                //Navegadores basados en IExplorer, es que no tengo innerheight 
                espacio_iframe = document.body.clientHeight;
                with_frame = document.body.clientWidth;
            } else {
                //otros navegadores y iframe
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

            }
        }
        $('#Panel_transacciones').css("height", (espacio_iframe - 40) + "px");
        $('#Cotenedorpendiente_transacciones').css("height", (espacio_iframe - 40) + "px");
        $('#Iframe_transacciones_').css("height", (espacio_iframe - 40) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_detalle_transacciones");
    }
}
function auto_zise_popup_detalle_respuesta() {
    var espacio_iframe = 420;
    var hidenpadre = 0;
    var with_frame = 420;
    if (window.innerHeight) {
        //navegadores basados en mozilla 
        espacio_iframe = window.innerHeight;
        with_frame = window.innerWidth;
    } else {
        if (document.body.clientHeight) {
            //Navegadores basados en IExplorer, es que no tengo innerheight 
            espacio_iframe = document.body.clientHeight;
            with_frame = document.body.clientWidth;
        } else {
            //otros navegadores y iframe
            //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

        }
    }


    $('#Panel_detalle_respuesta').css("height", (espacio_iframe - 5) + "px");
    $('#Cotenedorpendiente_detalle_respuesta').css("height", (espacio_iframe - 5) + "px");
    $('#Iframe_visor_externo_').css("height", (espacio_iframe - 5) + "px");


}

function auto_zise_popup_solicitud_aprobacion() {
    var espacio_iframe = 420;
    var hidenpadre = 0;
    var with_frame = 420;
    if (window.innerHeight) {
        //navegadores basados en mozilla 
        espacio_iframe = window.innerHeight;
        with_frame = window.innerWidth;
    } else {
        if (document.body.clientHeight) {
            //Navegadores basados en IExplorer, es que no tengo innerheight 
            espacio_iframe = document.body.clientHeight;
            with_frame = document.body.clientWidth;
        } else {
            //otros navegadores y iframe
            //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

        }
    }

    $('#Panel_solicitud_aprobacion').css("height", (espacio_iframe - 10) + "px");
    $('#contenido_procesa_solicitud_aprobacion').css("height", (espacio_iframe - 40) + "px");
    $('#Iframe_solicitud_aprobacion').css("height", (espacio_iframe - 45) + "px");

}
function auto_zise_popup_registro_colaboracion() {
    var espacio_iframe = 420;
    var hidenpadre = 0;
    var with_frame = 420;
    if (window.innerHeight) {
        //navegadores basados en mozilla 
        espacio_iframe = window.innerHeight;
        with_frame = window.innerWidth;
    } else {
        if (document.body.clientHeight) {
            //Navegadores basados en IExplorer, es que no tengo innerheight 
            espacio_iframe = document.body.clientHeight;
            with_frame = document.body.clientWidth;
        } else {
            //otros navegadores y iframe
            //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

        }
    }


    $('#Panel_registro_colaboracion').css("height", (espacio_iframe - 3) + "px");
    $('#contenido_procesa_registro_colaboracion').css("height", (espacio_iframe - 3) + "px");
    $('#Iframe_registro_colaboracion_').css("height", (espacio_iframe - 3) + "px");

}

function auto_zise_popup_compartir_documento() {
    var espacio_iframe = 420;
    var hidenpadre = 0;
    var with_frame = 420;
    if (window.innerHeight) {
        //navegadores basados en mozilla 
        espacio_iframe = window.innerHeight;
        with_frame = window.innerWidth;
    } else {
        if (document.body.clientHeight) {
            //Navegadores basados en IExplorer, es que no tengo innerheight 
            espacio_iframe = document.body.clientHeight;
            with_frame = document.body.clientWidth;
        } else {
            //otros navegadores y iframe
            //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

        }
    }


    $('#Panel_autoriza_compartir_documento').css("height", (espacio_iframe - 1) + "px");
    $('#contenido_procesa_autoriza_compartir_documento').css("height", (espacio_iframe - 1) + "px");
    $('#Iframe_compartir_documento_').css("height", (espacio_iframe - 1) + "px");
}
function size_colla_panel_formal() {
    var heithcollapext;
    var heigt_frame;
    if (window.innerHeight) {
        //navegadores basados en mozilla 
        heigt_frame = window.innerHeight;
       
    } else {
        if (document.body.clientHeight) {
            //Navegadores basados en IExplorer, es que no tengo innerheight 
            heigt_frame = document.body.clientHeight;
           
        } else {
            //otros navegadores y iframe
            //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val(); Iframe_2

        }
    }
    var heithcontrol;
    heithcontrol = document.getElementById("div_title").offsetHeight + document.getElementById("div_resp_formal").offsetHeight + document.getElementById("div_respuesta_confirmar").offsetHeight + document.getElementById("div_pie").offsetHeight;
    heithcollapext = (document.getElementById("Panel_seleccion").offsetHeight - heithcontrol) - 5;
    document.getElementById("Panel_respuesta_formal").parentElement.style.height = heithcollapext + "px";
    document.getElementById("Panel_respuesta_formal").style.height = heithcollapext + "px";
    //document.getElementById("Iframe_2").style.height = heithcollapext + "px";
}
function size_colla_panel_formal_radicado() {
    var heithcollapext;
    var heigt_frame;
    if (window.innerHeight) {
        //navegadores basados en mozilla 
        heigt_frame = window.innerHeight;

    } else {
        if (document.body.clientHeight) {
            //Navegadores basados en IExplorer, es que no tengo innerheight 
            heigt_frame = document.body.clientHeight;

        } else {
            //otros navegadores y iframe
            //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val(); Iframe_2

        }
    }
    var heithcontrol;
    //heithcontrol = document.getElementById("div_title").offsetHeight + document.getElementById("div_resp_formal").offsetHeight + document.getElementById("div_respuesta_confirmar").offsetHeight + document.getElementById("div_pie").offsetHeight;
    heithcontrol = document.getElementById("div_title").offsetHeight + document.getElementById("div_resp_formal").offsetHeight 
    heithcollapext = (document.getElementById("tabla_formal_respuesta").offsetHeight + heithcontrol);
    document.getElementById("Panel_respuesta_formal").parentElement.style.height = heithcollapext + "px";
    document.getElementById("Panel_respuesta_formal").style.height = document.getElementById("tabla_formal_respuesta").offsetHeight + "px";
    //document.getElementById("Iframe_2").style.height = heithcollapext + "px";
}
function size_colla_panel_confirma_radicado() {
    var heithcollapext;
    var heigt_frame;
    if (window.innerHeight) {
        //navegadores basados en mozilla 
        heigt_frame = window.innerHeight;

    } else {
        if (document.body.clientHeight) {
            //Navegadores basados en IExplorer, es que no tengo innerheight 
            heigt_frame = document.body.clientHeight;

        } else {
            //otros navegadores y iframe
            //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val(); Iframe_2

        }
    }
    var heithcontrol;
    heithcontrol = document.getElementById("div_title").offsetHeight + document.getElementById("div_resp_formal").offsetHeight + document.getElementById("div_respuesta_confirmar").offsetHeight;
    heithcollapext = (document.getElementById("table_solo_confirma").offsetHeight + heithcontrol) - 5;
    document.getElementById("panel_respuesta_confirmar").parentElement.style.height = heithcollapext + "px";
    document.getElementById("panel_respuesta_confirmar").style.height = document.getElementById("table_solo_confirma").offsetHeight + "px";
    //document.getElementById("Iframe_2").style.height = heithcollapext + "px";
}
function size_colla_panel_confirma() {
    var heithcollapext;
    var heigt_frame;
    if (window.innerHeight) {
        //navegadores basados en mozilla 
        heigt_frame = window.innerHeight;

    } else {
        if (document.body.clientHeight) {
            //Navegadores basados en IExplorer, es que no tengo innerheight 
            heigt_frame = document.body.clientHeight;

        } else {
            //otros navegadores y iframe
            //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val(); Iframe_2

        }
    }
    var heithcontrol;
    heithcontrol = document.getElementById("div_title").offsetHeight + document.getElementById("div_resp_formal").offsetHeight + document.getElementById("div_respuesta_confirmar").offsetHeight + document.getElementById("div_pie").offsetHeight;
    heithcollapext = (document.getElementById("Panel_seleccion").offsetHeight - heithcontrol) - 5;
    document.getElementById("panel_respuesta_confirmar").parentElement.style.height = heithcollapext + "px";
    document.getElementById("panel_respuesta_confirmar").style.height = heithcollapext + "px";
    //document.getElementById("Iframe_2").style.height = heithcollapext + "px";
}

function activa_boton() {
    try {
        if (document.getElementById("Check_respues_formal").checked == true) {
            document.getElementById("Button_respuesta_formal").click();
   
        }
        if (document.getElementById("CheckBox_respues_confirmar").checked == true) {
            document.getElementById("Button_respuesta_confirmar").click();
        }
    }

    catch (err) {
        alert(err.message + " funcion activa_boton " + err.message);
    }
}
function auto_zise_iframe() {
    var espacio_iframe = 420;
    var hidenpadre = 0;
    var with_frame = 420;
    if (window.innerHeight) {
        //navegadores basados en mozilla 
        espacio_iframe = window.innerHeight;
        with_frame = window.innerWidth;
    } else {
        if (document.body.clientHeight) {
            //Navegadores basados en IExplorer, es que no tengo innerheight 
            espacio_iframe = document.body.clientHeight;
            with_frame = document.body.clientWidth;
        } else {
            //otros navegadores y iframe
            //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

        }
    }


    $(document).ready(bodyResize);
    $(window).resize(bodyResize);
    function bodyResize() {
        $('#ifimpre').css("height", (espacio_iframe - 5) + "px");
        $('#ifimpre').css("width", (with_frame - 5) + "px");
    }


}
function progres_hiden(progres) {
    $("#progres_bar").css("display", "none");
}
function posicion_update_pogres(progres) {
    var espacio_iframe = 420;
    var hidenpadre = 0;
    var with_frame = 420;
    if (window.innerHeight) {
        //navegadores basados en mozilla 
        espacio_iframe = window.innerHeight;
        with_frame = window.innerWidth;
    } else {
        if (document.body.clientHeight) {
            //Navegadores basados en IExplorer, es que no tengo innerheight 
            espacio_iframe = document.body.clientHeight;
            with_frame = document.body.clientWidth;
        } else {
            //otros navegadores y iframe
            //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

        }
    }
    var prog = document.getElementById(progres);
    var widtop = (espacio_iframe / 2);
    var heitop = (with_frame / 2);
    prog.style.top = widtop + "px";
    prog.style.left = heitop + "px";
    prog.style.zIndex = "1000009";
    $("#progres_bar").css("display", "block");
    prog.style.position = "fixed";


}
function eliminar_ajaxtolkit() {
    try {
        var ele = document.getElementsByClassName("ajax__fileupload_fileItemInfo");
        for (var i = 0; i < ele.length; i++) {
            ele[i].parentNode.removeChild(ele[i]);
        }
    } catch (err) {
        alert(err.message + " funcion eliminar_ajaxtolkit " + err.message);
    }
}
function activa_boton_dowload() {
    try {

        document.getElementById("Button_sube_documento").click();
    }
    catch (err) {
        alert(err.message + " funcion activa_boton_dowload " + err.message);
    }
}
function activa_boton_dowload_adjunto() {
    try {

        document.getElementById("Button_sube_documento_adjunto_respuesta").click();
    }
    catch (err) {
        alert(err.message + " funcion activa_boton_dowload_adjunto " + err.message);
    }
}
function activa_boton_dowload_adjunto_simple() {
    try {

        document.getElementById("Button_sube_documento_adjunto_respuesta_simple").click();
    }
    catch (err) {
        alert(err.message + " funcion activa_boton_dowload_adjunto_simple " + err.message);
    }
}
//ZONA CODDIGO AYUDA
function create_element_popup(texto_popup, elemento_posicion) {
    try {
        var document_posicion = document.getElementById(elemento_posicion);
        var documento = document.getElementById("myModal");
        $('#myModal').css("width", "400px");
        $('#myModal').css("z-index", "10000000");
        $('#myModal').css("height", "150px");
        $('#mytexto_').css("width", "400px");
        $('#mytexto_').css("height", "150px");
        document.getElementById("tex_modal").innerHTML = texto_popup;
        documento.style.top = (document_posicion.offsetTop) + "px";
        documento.style.left = document_posicion.offsetLeft + "px";
        documento.style.display = "block";
        $('#myModal').show(1000);
    }
    catch (err) {
        alert(err.message + " Función create_element_popup");
    }
}
function hide_autonomo() {
    document.getElementById("myModal").style.display = "none";
}
function ayuda_general(ref_ayuda,nombre_element) {
    var texto;
    if (ref_ayuda == "DFP") {
        texto = "Para proyectar una respuesta formal al peticionario debe descargar el formato con protocolo de respuesta oficial de la entidad. <br />Nota : por favor sólo digite el contenido del documento, evite modificar el cuadro con la información del radicado de lo contrario el sistema no permitirá adjuntar el documento como respuesta.";
        create_element_popup(texto, nombre_element);
    }
    if (ref_ayuda == "ADR") {
        texto = "Para adjuntar el documento de respuesta debe ubicar el archivo con el formato de protocolo el cual debe estar diligenciado con el contenido de la respuesta. <br />Nota : para identificar el documento con el protocolo de respuesta tenga en cuenta el 'código interno' y el 'Radicado de respuesta' de tabla de radicación. Ejemplo para identificar el archivo 322-1700466703103.docx.";
        create_element_popup(texto, nombre_element);
    }
    if (ref_ayuda == "CFR") {
        texto = "Para confirmar la respuesta al peticionario es necesario que se haya adjuntando el documento de respuesta con el protocolo. <br />Nota : Los usuarios podrán confirmar al correo electrónico o enviar el documento al centro de correspondencia para el envío físico del documento.";
        create_element_popup(texto, nombre_element);
    }
    
}
function actualiza_hiden_formulario_padre() {
    try {

        var hiden = window.parent.document.getElementById("Hidden_estado_registro_solictud_aprobacion");
        if (hiden) {
            hiden.value = "YES";
        }
    }
    catch (err) {
        alert(err.message + " funcion actualiza_hiden_formulario_padre " + err.message);
    }
}
function AjaxFileUpload_change_text() {

    Sys.Extended.UI.Resources.AjaxFileUpload_SelectFile = "Adjuntar";
    Sys.Extended.UI.Resources.AjaxFileUpload_DropFiles = "Soltar y arrastrar archivos aquí";
    Sys.Extended.UI.Resources.AjaxFileUpload_Pending = "Pendiente";
    Sys.Extended.UI.Resources.AjaxFileUpload_Remove = "Eliminar";
    Sys.Extended.UI.Resources.AjaxFileUpload_Upload = "Guardar";
    Sys.Extended.UI.Resources.AjaxFileUpload_Uploaded = "Cargando";
    Sys.Extended.UI.Resources.AjaxFileUpload_UploadedPercentage = "Cargando {0} %";
    Sys.Extended.UI.Resources.AjaxFileUpload_Uploading = "Cargando";
    Sys.Extended.UI.Resources.AjaxFileUpload_FileInQueue = "{0} archivos(s) de .";
    Sys.Extended.UI.Resources.AjaxFileUpload_AllFilesUploaded = "All Files Uploaded.";
    Sys.Extended.UI.Resources.AjaxFileUpload_FileList = "Lista de archivos a cargar:";
    Sys.Extended.UI.Resources.AjaxFileUpload_SelectFileToUpload = "archivos(s) para cargar";
    Sys.Extended.UI.Resources.AjaxFileUpload_Cancelling = "Cancelando...";
    Sys.Extended.UI.Resources.AjaxFileUpload_UploadError = "Ocurrio un error cargando el archivo.";
    Sys.Extended.UI.Resources.AjaxFileUpload_CancellingUpload = "Cancelando carga...";
    Sys.Extended.UI.Resources.AjaxFileUpload_UploadingInputFile = "Cargando archivos: {0}.";
    Sys.Extended.UI.Resources.AjaxFileUpload_Cancel = "Cancelar";
    Sys.Extended.UI.Resources.AjaxFileUpload_Canceled = "cancelando";
    Sys.Extended.UI.Resources.AjaxFileUpload_UploadCanceled = "Carga de archivo cancelada";
    Sys.Extended.UI.Resources.AjaxFileUpload_DefaultError = "Error cargando archivo";
    Sys.Extended.UI.Resources.AjaxFileUpload_UploadingHtml5File = "Cargando archivo: {0} of size {1} bytes.";
    Sys.Extended.UI.Resources.AjaxFileUpload_error = "error";
}