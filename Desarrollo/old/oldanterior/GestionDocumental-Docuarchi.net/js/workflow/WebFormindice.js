$(document).ready(function () {

    $.fn.inicio = function () {
        auto_zise_seleccion_tipo();
        auto_size_indice();
       
        $('#Panel1-1').contextMenu('context-menu-1', {
            'Set tabla de retención': {
                click: function (element) {
                    var conf = confirm("Desea limpiar datos tabla de retención");
                    if (conf == false) {
                        return false;
                    }
                    document.getElementById("Hidden_id_serie").value = "0";
                    document.getElementById("Hidden_id_sub_serie").value = "0";
                    document.getElementById("Hidden_id_documento").value = "0";
                    document.getElementById("Hidden_id_area").value = "0";
                    if (document.getElementById("NOMBRESERIE") != undefined) {
                        document.getElementById("NOMBRESERIE").value = "";
                    }
                    if (document.getElementById("NOMBRESUBSERIE") != undefined) {
                        document.getElementById("NOMBRESUBSERIE").value = "";
                    }

                    if (document.getElementById("TIPODOCUMENTO") != undefined) {
                        document.getElementById("TIPODOCUMENTO").value = "";
                    }
                }
            },

            'Set Expediente': {
                click: function (element) {
                    var conf = confirm("Desea limpiar datos del expediente");
                    if (conf == false) {
                        return false;
                    }
                    document.getElementById("Hidden_id_expediente").value = "0";
                    document.getElementById("Hidden_id_tipo_expediente").value = "0";
                    if (document.getElementById("EXPEDIENTE") != undefined) {
                        document.getElementById("EXPEDIENTE").value = "";
                    }
                    //document.getElementById("Button_actualiza_hiden_Expediente").click();
                    //mueve_scroll_data_gred('EXPEDIENTE', 'panel1');
                }
            },
            'Set clase de documento': {
                click: function (element) {
                    var conf = confirm("Desea limpiar la clase de documento");
                    if (conf == false) {
                        return false;
                    }
                    document.getElementById("Hidden_id_tipo").value = "0";
                    if (document.getElementById("CLASEDOCUMENTO") != undefined) {
                        document.getElementById("CLASEDOCUMENTO").value = "";
                    }
                }
            },
            'Set fecha elaboración': {
                click: function (element) {
                    var conf = confirm("Desea limpiar la fecha de elaboración");
                    if (conf == false) {
                        return false;
                    }

                    if (document.getElementById("FECHAELABORACION") != undefined) {
                        document.getElementById("FECHAELABORACION").value = "";
                    }
                }
            },
            'Set todas': {
                click: function (element) {
                    var conf = confirm("Desea limpiar toda la gestión");
                    if (conf == false) {
                        return false;
                    }
                    document.getElementById("Hidden_id_serie").value = "0";
                    document.getElementById("Hidden_id_sub_serie").value = "0";
                    document.getElementById("Hidden_id_documento").value = "0";
                    document.getElementById("Hidden_id_area").value = "0";
                    if (document.getElementById("NOMBRESERIE") != undefined) {
                        document.getElementById("NOMBRESERIE").value = "";
                    }
                    if (document.getElementById("NOMBRESUBSERIE") != undefined) {
                        document.getElementById("NOMBRESUBSERIE").value = "";
                    }

                    if (document.getElementById("TIPODOCUMENTO") != undefined) {
                        document.getElementById("TIPODOCUMENTO").value = "";
                    }
                    document.getElementById("Hidden_id_tipo_expediente").value = "0";
                    document.getElementById("Hidden_id_expediente").value = "0";
                    if (document.getElementById("EXPEDIENTE") != undefined) {
                        document.getElementById("EXPEDIENTE").value = "";
                    }
                    document.getElementById("Hidden_id_tipo").value = "0";
                    if (document.getElementById("CLASEDOCUMENTO") != undefined) {
                        document.getElementById("CLASEDOCUMENTO").value = "";
                    }
                   
                    if (document.getElementById("FECHAELABORACION") != undefined) {
                        document.getElementById("FECHAELABORACION").value = "";
                    }
                }
            }

        });
    };
   
   
});
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
function rezize_event() {
    try {
        auto_zise_seleccion_tipo();
        auto_size_indice();
        redimenciona_marco_indice();
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
function setear_fecha_elaboracion() {
    try {
    var conf = confirm("Desea limpiar la fecha de elaboración");
    if (conf == false) {
        return false;
    }

    if (document.getElementById("FECHAELABORACION") != undefined) {
        document.getElementById("FECHAELABORACION").value = "";
    }
}
    catch (err) {
        alert(err.message + " Funcion setear_fecha_elaboracion");
}
}
function setear_expdiente(){
    try {
    var conf = confirm("Desea limpiar datos del expediente");
    if (conf == false) {
        return false;
    }
    document.getElementById("Hidden_id_expediente").value = "0";
    document.getElementById("Hidden_id_tipo_expediente").value = "0";
    if (document.getElementById("EXPEDIENTE") != undefined) {
        document.getElementById("EXPEDIENTE").value = "";
    }

  }
    catch (err) {
        alert(err.message + " Funcion setear_expdiente");
  }
}
function setear_clase_documento() {
    try {
    var conf = confirm("Desea limpiar la clase de documento");
    if (conf == false) {
        return false;
    }
    document.getElementById("Hidden_id_tipo").value = "0";
    if (document.getElementById("CLASEDOCUMENTO") != undefined) {
        document.getElementById("CLASEDOCUMENTO").value = "";
    }
  }
    catch (err) {
        alert(err.message + " Funcion setear_clase_documento");
  }
}
function setear_trd_documento() {
    try {
    var conf = confirm("Desea limpiar datos tabla de retención");
    if (conf == false) {
        return false;
    }
    document.getElementById("Hidden_id_serie").value = "0";
    document.getElementById("Hidden_id_sub_serie").value = "0";
    document.getElementById("Hidden_id_documento").value = "0";
    document.getElementById("Hidden_id_area").value = "0";
    if (document.getElementById("NOMBRESERIE") != undefined) {
        document.getElementById("NOMBRESERIE").value = "";
    }
    if (document.getElementById("NOMBRESUBSERIE") != undefined) {
        document.getElementById("NOMBRESUBSERIE").value = "";
    }

    if (document.getElementById("TIPODOCUMENTO") != undefined) {
        document.getElementById("TIPODOCUMENTO").value = "";
    }
    }
    catch (err) {
        alert(err.message + " Funcion setear_trd_documento");
    }
}
function value_scrool() {
    document.getElementById("Hidden_scroll").value = $("#Panel1").scrollTop();
    //alert(document.getElementById("Hidden_scroll").value);
}

function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000003;
}

function asignar_clase_documento() {
    try {
    if (document.getElementById("Hidden_id_tipo").value == "0") {
        alert("Debe seleccionar la clase de documento");
        return false;
    }
    var text_clase_documento = document.getElementById("CLASEDOCUMENTO");
    if (text_clase_documento == undefined) {
        alert("Imposible encontrar el control CLASEDOCUMENTO");
        return false;
    }
    text_clase_documento.value = document.getElementById("Hidden_valor_seleccion").value
    document.getElementById("Buttoncerrar_tipo_popup").click();
   
}
 catch (err) {
     alert(err.message + " funcion asignar_clase_documento " + err.message);
}                       
              
   
}
function auto_zise_seleccion_tipo() {
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
       
        $('#Panel_tipo_popup').css("height", (espacio_iframe - 30) + "px");
        var heigconetedor;
        heigconetedor = (espacio_iframe - 29) - (30 + 60);
        $('#contenido_gred').css("height", heigconetedor + "px");
        $('#TextBoxinfotipo').css("height", (heigconetedor - 10) + "px");
       
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_seleccion_tipo " + err.message);
    }
}
function valor_tipo_documento() {
    try {
        
    if ($('#ComboBoxtipo')[0].value !== "SELECCIONE" && $('#ComboBoxtipo')[0].value !== "") {
        document.getElementById("Hidden_valor_seleccion").value = $('#ComboBoxtipo')[0].value;
        $('#Button_lista_ayuda_tipo').click();
    } else {
        document.getElementById("Hidden_valor_seleccion").value = "";
        document.getElementById("TextBoxinfotipo").value = "";
    }

}
 catch (err) {
     alert(err.message + " funcion valor_tipo_documento " + err.message);
    }
}
function seter_size_hiden() {
    try {
        document.getElementById("Hidden_esta_hiden").value = 0;
    }
    catch (ex) {
        alert("Errror generel funcion jva seter_size_hiden " & ex.message)
    }
}
function actualiza_documento_seleccion_workflow() {
    try {
        if (window.parent.document.getElementById("Button_Actualizar_seleccion_indice_wf")) {
            window.parent.document.getElementById("Button_Actualizar_seleccion_indice_wf").click();
        }
        }
            catch (err) {
                alert(err.message + " Funcion actualiza_documento_seleccion_workflow");
        }
}
function redimenciona_marco_indice() {
    try {
        var espacio_iframe = 420;
        var hidenpadre = 0;
        var with_frame = 420;
        if (window.parent.innerHeight) {
            //navegadores basados en mozilla 
            espacio_iframe = window.parent.innerHeight;
            with_frame = window.parent.innerWidth;
        } else {
            if (document.parentWindow.body.clientHeight) {
                //Navegadores basados en IExplorer, es que no tengo innerheight 
                espacio_iframe = document.body.clientHeight;
                with_frame = document.parentWindow.body.clientWidth;
            } else {
                //otros navegadores y iframe
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();
            }
        }
        //ifrm_indice_ 
        if (window.parent.document.getElementById("cuerpoindice") && document.getElementById("Hidden_esta_hiden").value != 1) {
            window.parent.document.getElementById("cuerpoindice").style.width = (window.parent.document.getElementById("div_content_general_wf").clientWidth) + "px";
            window.parent.document.getElementById("cuerpoindice").style.height = (window.parent.document.getElementById("div_content_general_wf").clientHeight - 60) + "px";
            if (document.getElementById("Iframe_trd_popup_")) {
                document.getElementById("Iframe_trd_popup_").style.height = window.parent.document.getElementById("cuerpoindice").clientHeight + (document.getElementById("divcabecer_trd_popup").clientHeight) + "px";
                document.getElementById("Iframe_trd_popup_").style.width = window.parent.document.getElementById("cuerpoindice").clientWidth - 5 + "px";
            }
            if (document.getElementById("Iframe_expdiente_popup_")) {
                document.getElementById("Iframe_expdiente_popup_").style.height = window.parent.document.getElementById("cuerpoindice").clientHeight + (document.getElementById("divcabecer_expdiente_popup").clientHeight) + "px";
                document.getElementById("Iframe_expdiente_popup_").style.width = window.parent.document.getElementById("cuerpoindice").clientWidth - 5 + "px";
            }
            if (window.parent.document.getElementById("ifrm_indice_")) {
                window.parent.document.getElementById("ifrm_indice_").style.height = (window.parent.document.getElementById("div_content_general_wf").clientHeight) + "px";
                window.parent.document.getElementById("ifrm_indice_").style.width = "100%";
            }
            window.parent.document.getElementById("cuerporigth").style.display = "none";
            window.parent.document.getElementById("cuerpoleft").style.display = "none";
            window.parent.document.getElementById("Ocultaindice").style.display = "none";
            window.parent.document.getElementById("Div_aplia_indice").style.display = "none"; 
            window.parent.document.getElementById("menucab").style.display = "none";
            window.parent.document.getElementById("Menutol").style.display = "none";
        }
    
    }
    catch (err) {
        alert(err.message + " Funcion redimenciona_marco_indice");
    }
}
    function auto_size_indice() {
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


        if (window.parent.document.getElementById("Menu1")) {
            var heit = (espacio_iframe + 29) - (window.parent.document.getElementById("Menu1").clientHeight + window.parent.document.getElementById("Menutol").clientHeight + document.getElementById("Button_actualiza_indice_imagen").clientHeight );
            $('#Panel1').css("height", (heit) + "px");
        }
   
    }
    function redimenciona_padre_marco() {
        try {
            document.getElementById("Hidden_esta_hiden").value = 1;
            window.parent.document.getElementById("menucab").style.display = "block";
            window.parent.document.getElementById("Menutol").style.display = "block";
            window.parent.document.getElementById("cuerpoleft").style.display = "block";
            window.parent.document.getElementById("cuerpoindice").style.display = "block";          
            window.parent.document.getElementById("cuerporigth").style.display = "block";
            window.parent.document.getElementById("cuerpoindice").style.width = "20%";
            window.parent.document.getElementById("cuerporigth").style.width = "59.6%";
            window.parent.document.getElementById("cuerpoleft").style.width = "20%";
            window.parent.document.getElementById("ocultaraigth").style.display = "block";
            window.parent.document.getElementById("verroght").style.display = "none";
            window.parent.document.getElementById("Ocultaindice").style.display = "block";
            window.parent.document.getElementById("ifrm_indice_").style.width = "98%";
            window.parent.document.getElementById("ocultaleft").style.display = "none";
            window.parent.document.getElementById("label").style.width = "96%";  
            if (parent.document.getElementById("Button_red_parent")) {
                parent.document.getElementById("Button_red_parent").click();
            }
           
        }
        catch (err) {
            alert(err.message + " Funcion redimenciona_padre_marco");
        }
    }
    function mueve_scroll_data_gred(buton, panel) {
        try {
            $("#" + panel).scrollTop(70);
            $("#" + panel).scrollTop(($("#" + buton).offset().top));
        }
        catch (err) {
            alert(err.message + " Funcion mueve_scroll_data_gred");
        }
    }
    function mueve_scroll_value(valor, panel) {
        try {
            $("#" + panel).scrollTop(70);
            $("#" + panel).scrollTop(document.getElementById("Hidden_scroll").value);
            //alert(document.getElementById("Hidden_scroll").value);
        }
        catch (err) {
            alert(err.message + " Funcion mueve_scroll_value");
        }
    }
    $(document).on('keydown', function (e) {
        if (e.which == 9) {
            var id_element = e.srcElement.className;

            var salidadato;
            if (id_element == "date_indice") {
                var dato = e.srcElement.value;


                if (dato == "") {

                    return false;
                }


                if (salidadato == "Formato fecha no cumple") {
                    alert(salidadato);
                    e.preventDefault();
                    return false;
                }
                var BisestA;
                var Año_F, Mes_f, Dia_f, tip;
                var numerocaracter = dato.length;
                if (numerocaracter == 10 || numerocaracter == 8) {

                }
                else {
                    alert("Formato fecha no cumple");
                    e.preventDefault();
                    return false;
                }
                if (numerocaracter == 10) {
                    Año_F = dato.substring(0, 4);
                    Mes_f = dato.substring(0, 7);
                    Mes_f = Mes_f.substring(7, 5);
                    Dia_f = dato.substring(8, 10);
                }
                else {
                    Año_F = dato.substring(0, 4);
                    Mes_f = dato.substring(0, 6);
                    Mes_f = Mes_f.substring(6, 4);
                    Dia_f = dato.substring(6, 8);
                }

                //Verifica el formato del dia
                if (Dia_f > 31 || Dia_f == 0) {

                    alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                    e.preventDefault();
                    return false;
                }

                //verifica el formato del mes
                if (Mes_f > 12 || Mes_f < 1) {
                    alert("EM_" + Año_F + "(" + Mes_f + ")" + Dia_f);
                    e.preventDefault();
                    return false;
                }

                switch (Mes_f) {
                    case "01":
                        if (Dia_f > 31) {
                            alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                            e.preventDefault();
                        }
                        break;

                    case "02":
                        if (Dia_f % 4 == 0) {

                            BisestA = 29;
                        }
                        else {
                            BisestA = 28;
                        }
                        if (Dia_f > BisestA) {
                            alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                            e.preventDefault();
                        }
                        break;
                    case "03":
                        if (Dia_f > 31) {
                            alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                            e.preventDefault();
                        }
                        break;

                    case "04":
                        if (Dia_f > 30) {
                            alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                            e.preventDefault();
                        }
                        break;

                    case "05":
                        if (Dia_f > 31) {
                            alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                            e.preventDefault();
                        }
                        break;

                    case "06":
                        if (Dia_f > 30) {
                            alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                            e.preventDefault();
                        }
                        break;

                    case "07":
                        if (Dia_f > 31) {
                            alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                            e.preventDefault();
                        }
                        break;

                    case "08":
                        if (Dia_f > 31) {
                            alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                            e.preventDefault();
                        }
                        break;

                    case "09":
                        if (Dia_f > 30) {
                            alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                            e.preventDefault();
                        }
                        break;

                    case "10":
                        if (Dia_f > 31) {
                            alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                            e.preventDefault();
                        }
                        break;

                    case "11":
                        if (Dia_f > 30) {
                            alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                            e.preventDefault();
                        }
                        break;

                    case "12":
                        if (Dia_f > 31) {
                            alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                            e.preventDefault();
                        }
                        break;
                }

                if (numerocaracter == 8) {
                    salidadato = Año_F + "-" + Mes_f + "-" + Dia_f;
                    e.srcElement.value = salidadato;
                }

                if (numerocaracter == 10) {
                    salidadato = Año_F + "-" + Mes_f + "-" + Dia_f;
                    e.srcElement.value = salidadato;
                }

            }
        }
    });