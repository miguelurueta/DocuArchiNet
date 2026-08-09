$(document).ready(function () {
    $.fn.inicio = function () {
        
        auto_zise_page();
        resize_table_boot();
        $('#table').on('page-change.bs.table', function (e, arg1, arg2) {
            resize_table_boot();
        })
        $('#table').on('all.bs.table', function (e, arg1, arg2) {
            resize_table_boot();
        })
    }
})
$(window).on("load", function () {
    try {
        var elment = document.getElementsByClassName("da_event_captive");
        if (elment) {
            for (var i = 0; i < elment.length; i++) {
                elment[i].addEventListener("click", event_click, false);
            }
        }
       
        window.addEventListener("resize", rezize_event);
        ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100001);


    } catch (e) {
        alert(" funcion load " + e.message);
    }

});
function rezize_event() {
    try {
        
        auto_zise_page();
       
    } catch (ex) {
        alert(ex.message + " Función rezize_event")
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

function progres_hiden(progres) {
    $("#progres_bar").css("display", "none");
}
function ConfirmMensajeGeneral(mensaje, name_hiden) {
    try {
        var element_hiden = document.getElementById(name_hiden)
        if (element_hiden === null) {
            alert("Imposible encontrar el control " + name_hiden);
            return false;
        }
        var x = "";
        var r = confirm(mensaje);
        if (r == true) {
            x = "1";
        }
        else {
            x = "0";
        }
        document.getElementById(name_hiden).value = x;
    }
    catch (err) {
        alert(err.message + " ConfirmMensajeGeneral");
    }
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
var ITEMS_DATOS_TOKENIZE_2 = new Array();
var ESTADO_RESULTADO = "";
var ESTADO_RESULTADO_ = "";
var REGISTRO;
var INTERVAL;
var elem = document.getElementById("myBar");
var elment_progres = document.getElementById("myProgress_porcent");
var elment_conta = document.getElementById("myProgress_contador");
function migra_trammite_sii() {
    try {
        if (document.getElementById("content_rango_fecha").checked == true) {
            migra_trammite_sii_fecha();
        } else {
            migra_tramite_radicado();
        }
    } catch (ex) {
        alert("Funcion migra_trammite_sii " + ex.message);

    }
}
function migra_tramite_radicado() {
    try {
        var radicado = "";
        radicado = document.getElementById("text_codigo_barras");
        if (radicado.value == "") {
            alert("Por favor digite el numero del radicado");
            return true;
        }
        ESTADO_RESULTADO = "";
        INTERVAL = setInterval(call_estado, 400);
        function call_estado() {
            if (ESTADO_RESULTADO == "") {
                document.getElementById("Button_migrar").disabled = true;
                document.getElementById("Button_migrar").value = "Esperar....";
                ESTADO_RESULTADO = "rrr";
                Service_migra_registro_radicado_sii(radicado.value);
            }
        }
    } catch (ex) {
        alert("Funcion migra_tramite_radicado " + ex.message);
    }
}
function lista_registro_sii_migrados() {
    try {
        var fecha_ini = "";
        var fecha_fin = "";
        var codigo_sii = "";
        fecha_ini = document.getElementById("TextBox_FINAL_INICIAL");
        fecha_fin = document.getElementById("TextBox_FINAL_FINAL");
        codigo_sii = document.getElementById("text_codigo_consulta");
        ESTADO_RESULTADO = "";
        INTERVAL = setInterval(call_estado, 400);
        function call_estado() {
            if (ESTADO_RESULTADO == "") {
                document.getElementById("Button_consultar").disabled = true;
                document.getElementById("Button_consultar").value = "Esperar....";
                ESTADO_RESULTADO = "rrr";
                Service_solicita_lista_registro_sii_migrados(fecha_ini.value, fecha_fin.value, codigo_sii.value);
            }
        }
    } catch (ex) {
        alert("Funcion lista_registro_sii_migrados " + ex.message);
    }
}
var width_ = 0;
function migra_trammite_sii_fecha() {
    try {
        
        var fecha_ini = "";
        var fecha_fin = "";
        fecha_ini = document.getElementById("TextBoxFECHA_EXTREMA_FINAL_INICIAL");
        fecha_fin = document.getElementById("TextBoxFECHA_EXTREMA_FINAL_FINAL");
        if (fecha_ini.value == "") {
            alert("Por favor seleccione la fecha inicial");
            return true;
        }
        if (fecha_fin.value == "") {
            alert("Por favor seleccione la fecha final");
            return true;
        }
        ESTADO_RESULTADO = "";
        web_service_solicita_rago_fecha_migracion_tramite_sii(fecha_ini.value, fecha_fin.value);
        INTERVAL = setInterval(call_estado, 400);
        function call_estado() {
            if (ESTADO_RESULTADO !== "") {
                if (ESTADO_RESULTADO !== "YES") {
                    clearInterval(INTERVAL);
                    return true;
                } else {
                    clearInterval(INTERVAL);
                    if (ITEMS_DATOS_TOKENIZE_2.length > 0) {
                        document.getElementById("myProgress_porcent").innerHTML = "0%";
                        document.getElementById("myProgress_contador").innerHTML = "0";
                        document.getElementById("myBar").style.width = 0 + '%';
                        elem = document.getElementById("myBar");
                        elment_progres = document.getElementById("myProgress_porcent");
                        elment_conta = document.getElementById("myProgress_contador");
                        document.getElementById("Button_pogres_show").click();           
                        var numero_fin = ITEMS_DATOS_TOKENIZE_2.length;
                        ESTADO_RESULTADO = "YES";
                        ESTADO_RESULTADO_ == ""
                        INTERVAL = setInterval(migra, 400);
                        function migra() {    
                                if (width_ >= numero_fin) {
                                    clearInterval(INTERVAL);
                                    ITEMS_DATOS_TOKENIZE_2 = [];
                                    document.getElementById('Button_cerrar_pro_gres_bar').click();
                                } else {
                                    if (ESTADO_RESULTADO_ == "") {
                                        ESTADO_RESULTADO_ = "INI";
                                        Service_migra_sii_registro(ITEMS_DATOS_TOKENIZE_2[width_]);
                                        //alert("solictud copiando");
                                    }
                                    
                                    if (ESTADO_RESULTADO_ == "YES") {
                                        ESTADO_RESULTADO_ = "";
                                        width_++;
                                        var porcent = (100 * width_) / numero_fin;
                                        porcent = Math.round(porcent)
                                        elem.style.width = porcent + '%';
                                        elment_progres.innerHTML = porcent + '% ';
                                        elment_conta.innerHTML = "(" + width_ + ' de ' + numero_fin + ")";
                                    }                   
                                }   
                        }
                    }
                }    
            }       
        }
       
               
    } catch (ex) {
        alert("Funcion migra_trammite_sii_fecha " + ex.message);
    }
}
function myStopFunction(event) {
    try {
        var con = confirm("Desea cancelar el proceso?");
        if (con == true) {
            elem.style.width = 0 + '%';
            elment_progres.innerHTML = 0 + '%';
            //Restaura_array();
            clearInterval(INTERVAL);
            document.getElementById('Button_cerrar_pro_gres_bar').click();
        }
        event.preventDefault();
    } catch (err) {
        document.getElementById('Button_cerrar_pro_gres_bar').click();
        alert(err.message + " Funcion myStopFunction");
        event.preventDefault();
    }
}
function myStopFunction_Event(error) {
    try {
            alert(error);
            elem.style.width = 0 + '%';
            elment_progres.innerHTML = 0 + '%';
            document.getElementById('Button_cerrar_pro_gres_bar').click();
            clearInterval(INTERVAL);
        

    } catch (err) {
        document.getElementById('Button_cerrar_pro_gres_bar').click();
        alert(err.message + " Funcion myStopFunction_Event");

    }
}
function myStopFunction_Event_radicado(error) {
    try {
        document.getElementById("Button_migrar").disabled = false;
        document.getElementById("Button_migrar").value = "Aceptar";
       
        alert(error);
        clearInterval(INTERVAL);
    } catch (err) {
       
        alert(err.message + " Funcion myStopFunction_Event_radicado");

    }
}
function mystopfunction_event_lista_radicado(error) {
    try {
        document.getElementById("Button_consultar").disabled = false;
        document.getElementById("Button_consultar").value = "Aceptar";
        var $table = $('#table')       
        if (error !== "") {
            $table.bootstrapTable('removeAll');
            //resize_table_boot();
        } else {
            ESTADO_RESULTADO = "";
            $table.bootstrapTable('destroy').bootstrapTable({ data: REGISTRO });
            //resize_table_boot();
        }
        clearInterval(INTERVAL);
    } catch (err) {
        alert(err.message + " Funcion mystopfunction_event_lista_radicado");
    }
}
function web_service_solicita_rago_fecha_migracion_tramite_sii(fecha_ini, fecha_fin) {
    try {
        var search = "";
        $.ajax('../webservice/WebService_integracion_sii.asmx/web_service_Solicita_rago_fecha_migracion_tramite_sii', {
            data: "{'DName':'" + fecha_ini + "'," + "'DName_':'" + fecha_fin + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_funcion !== "YES") {
                    ESTADO_RESULTADO = data.d[0].error_funcion;
                    alert("Error funcion  web_service_Solicita_rago_fecha_migracion_tramite_sii " + data.d[0].error_funcion)
                } else {
                    ITEMS_DATOS_TOKENIZE_2 = new Array();
                    $.each(data.d, function (k, v) {
                        ITEMS_DATOS_TOKENIZE_2.push(v);
                    });
                    ESTADO_RESULTADO = "YES";
                }
            },
            error: function (errorText) {
                //ESTADO_RESULTADO = data.d[0].error_funcion;
                ESTADO_RESULTADO = errorText.responseText;
                alert("Error web_service_Solicita_rago_fecha_migracion_tramite_sii : " + errorText.responseText);
            }
        });

    } catch (ex) {
        alert("Funcion web_service_Solicita_rago_fecha_migracion_tramite_sii " + ex.message);

    }
}
function Service_migra_sii_registro(para_meter_ca) {
    try {      
        var serialice = JSON.stringify(para_meter_ca);
        serialice = "[" + serialice + "]";
        $.ajax('../webservice/WebService_integracion_sii.asmx/Service_migra_sii_registro', {
            data: "{" + "'parameter':'" + serialice + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d !== "YES") {
                    myStopFunction_Event(data.d)
                } else {
                    //alert("copiado");
                    ESTADO_RESULTADO_ = "YES";
                }
            },
            error: function (result) {
                myStopFunction_Event(data.d)
            }, compelete: function () {
                ESTADO_RESULTADO_ = "YES";
            }
        });
    } catch (ex) {
        alert(ex.message + " funcion Service_migra_sii_registro");
    }
}
function Service_migra_registro_radicado_sii(para_meter_ca) {
    try {
       
        $.ajax('../webservice/WebService_integracion_sii.asmx/Service_migra_registro_radicado_sii', {
            data: "{" + "'dname':'" + para_meter_ca + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d !== "YES") {
                    myStopFunction_Event_radicado(data.d);
                } else {
                    myStopFunction_Event_radicado("Radicado migrado");
                }
            },
            error: function (result) {
                myStopFunction_Event_radicado(result.innerHTML);
            }, compelete: function () {
                myStopFunction_Event_radicado("Radicado migrado");
            }
        });
    } catch (ex) {
        alert(ex.message + " funcion Service_migra_registro_radicado_sii");
    }
}
function Service_solicita_lista_registro_sii_migrados(fecha_ini, fecha_fin, codigo_sii) {
    try {
        $.ajax('../webservice/WebService_integracion_sii.asmx/solicita_lista_registro_sii_migrados', {
            data: "{'fecha_ini':'" + fecha_ini + "'," + "'fecha_fin':'" + fecha_fin + "'," + "'codigo_sii':'" + codigo_sii + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
             var tipe =   typeof data.d;
             if (tipe === "string") {
                    mystopfunction_event_lista_radicado(data.d);
                } else {
                 //REGISTRO = JSON.stringify(data.d);
                 REGISTRO = data.d;
                 mystopfunction_event_lista_radicado("");
                }
            },
            error: function (result) {
                mystopfunction_event_lista_radicado(result.innerHTML);
            }, compelete: function () {
                mystopfunction_event_lista_radicado("");
            }
        });
    } catch (ex) {
        alert(ex.message + " funcion Service_solicita_lista_registro_sii_migrados");
    }
}
function auto_zise_page() {
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
    $('#content_general').css("height", (espacio_iframe - 1) + "px");
    $('#div_tab_migra_fecha').css("height", (document.getElementById("content_general").clientHeight - (document.getElementById("div_tab_content_resp_general").clientHeight + document.getElementById("div_title").clientHeight)) + "px");
    $('#div_tab_consulta_migracion').css("height", (document.getElementById("content_general").clientHeight - (document.getElementById("div_tab_content_resp_general").clientHeight + document.getElementById("div_title").clientHeight)) + "px");
    $('#conten_fechas').css("height", (document.getElementById("div_tab_migra_fecha").clientHeight - document.getElementById("conte_foter").clientHeight) + "px");
    $('#div_content_consulta').css("height", (document.getElementById("div_tab_consulta_migracion").clientHeight - document.getElementById("conte_foter_consulta").clientHeight) + "px");
    $('#div_content_tabla').css("height", (document.getElementById("div_content_consulta").clientHeight - (document.getElementById("div_contenido_controles_consulta").clientHeight )) + "px");
    $('#table').css("width", "100%");
    
    }
    catch (err) {
        alert(err.message + " auto_zise_page");
    }
}
function resize_table_boot() {
    try {
        
        var element_table_body = document.getElementById('div_content_tabla').getElementsByClassName('fixed-table-body');
        var element_table = document.getElementById('div_content_tabla').getElementsByClassName('fixed-table-container');
        var element_toolbar = document.getElementById('div_content_tabla').getElementsByClassName('fixed-table-toolbar');
        var element_pagination = document.getElementById('div_content_tabla').getElementsByClassName('fixed-table-pagination');
        $(element_table[0]).css("height", (document.getElementById("div_content_consulta").clientHeight - (element_toolbar[0].clientHeight + element_pagination[0].clientHeight + 120)) + "px");
        $(element_table_body[0]).css("height", (document.getElementById("div_content_consulta").clientHeight - (element_toolbar[0].clientHeight + element_pagination[0].clientHeight + 160)) + "px");
        
    } 
    catch (err) {
        alert(err.message + " resize_table_boot");
    }

}
