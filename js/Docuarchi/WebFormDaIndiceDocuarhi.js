$(document).ready(function () {
    visualiza_boton_marco_padre();
    $.fn.inicio = function () {
        
        auto_zise_seleccion_tipo();
        redimenciona_marco_padre_visor_externo();
        $(window.parent).resize(bodyResize);
        $(window).resize(bodyResize);
        function bodyResize() {
            //windows_resize_redimenciona_marco_indice();
            auto_zise_seleccion_tipo();
            redimenciona_marco_padre_visor_externo();
            

        }
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
function seter_size_hiden() {
}

function actualiza_documento_seleccion_workflow() {
    try {
        if (window.parent.document.getElementById("Button_actualizar_seleccion_digitalizacion")) {
            window.parent.document.getElementById("Button_actualizar_seleccion_digitalizacion").click();
        }
    }
    catch (err) {
        alert(err.message + " Funcion actaliza_documento_seleccion_workflow");
    }
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
        alert(err.message + " Funcion tamano_ventana_nuevo_volumen_expediente");
    }
}
function actualiza_indice_padre() {
    try {
    var boton_indice = window.parent.document.getElementById("Button_actualiza_indice")
    if (boton_indice != undefined) {
        
        if (document.getElementById("Hidden_resultado").value == "YES") {
            boton_indice.click();
            document.getElementById("Hidden_resultado").value = "";
        }
        
    }
    document.getElementById("Hidden_resultado").value = "";
    }
    catch (err) {
        alert(err.message + " Funcion actualiza_indice_padre");
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
function setear_expdiente() {
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
    sender._popupBehavior._element.style.zIndex = 1000001;
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
function auto_zise_gestion_expeidentes() {
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
        $('#Contenido_superior').css("height", heigconetedor + "px");

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
        
        var heigconetedor = (espacio_iframe - 40) - (((espacio_iframe - 40) * 95) / 100);
        $('#Contenido_superior').css("height", heigconetedor + "px");
        heigconetedor = (espacio_iframe - 40) - (((espacio_iframe - 40) * 25) / 100);
        $('#contenido_gred').css("height", heigconetedor + "px");
        heigconetedor = (espacio_iframe - 40) - (((espacio_iframe - 40) * 80) / 100);
        $('#contenido_inferior').css("height", heigconetedor + "px");
        var gridwith = (with_frame - 5);
        $('#contenido_inferior').css("width", gridwith + "px");
        $('#contenido_gred').css("width", gridwith + "px");
        $('#Contenido_superior').css("width", gridwith + "px");
        var heigconetedor = (espacio_iframe - 40) - (((espacio_iframe - 40) * 25) / 100);
        var gridwith = (with_frame - 5);
        //TextBoxinfotipo
        $('#TextBoxinfotipo').css("width", (gridwith - 10) + "px");
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
function redimenciona_marco_padre_visor_externo() {
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
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val(); Ocultaindice
            }
        }
        if (window.parent.document.getElementById("Label_indice_visor_externo")) {
            var heig = window.parent.document.getElementById("Cotenedor_indice").clientHeight - document.getElementById("div_buton").clientHeight;
            $('#indice_imagen_div').css("height", (heig - 60) + "px");
            $('#Panel1').css("height", (heig - 60) + "px");
        }
        //controla indice padre visor clasificación  
        if (window.parent.document.getElementById("ifrm_indice_")) {
            var heig = (window.parent.document.getElementById("ifrm_indice_").clientHeight - document.getElementById("div_buton").clientHeight);
            $('#indice_imagen_div').css("height", (heig - 60) + "px");
            $('#Panel1').css("height", (heig - 60) + "px");
        }
        //controla indice padre visor docuarchi webformdavisorDocuarchi.aspx   
        if (window.parent.document.getElementById("ifrm_indice_visor_docuarchi_")) {
            var heig = (window.parent.document.getElementById("ifrm_indice_visor_docuarchi_").clientHeight - document.getElementById("div_buton").clientHeight);
            $('#indice_imagen_div').css("height", (heig - 10) + "px");
            $('#Panel1').css("height", (heig - 10) + "px");
        }
        if (window.parent.document.getElementById("ifrm_indice_visor_externo_")) {
            var heig = (window.parent.document.getElementById("ifrm_indice_visor_externo_").clientHeight - document.getElementById("div_buton").clientHeight);
            $('#indice_imagen_div').css("height", (heig - 10) + "px");
            $('#Panel1').css("height", (heig - 10) + "px");
        }
    }
    catch (ex) {
        //alert(ex.message + " Funcion redimenciona_marco_padre_visor_externo");
    }
}
function redimenciona_marco_indice() {
    try {
       // DOCUARCHI_NET
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
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val(); Ocultaindice
            }
        }
        //ifrm_indice DOCUARCHI.VISOR
        if (document.getElementById("Hiddennameasigna").value == "DOCUARCHI_NET") {
           
            if (window.parent.document.getElementById("cuerpoindice")) {
                
                if (window.parent.document.getElementById("cuerpoindice")) {
                    window.parent.document.getElementById("cuerpoindice").style.width = with_frame + "px";
                }
               
                if (window.parent.document.getElementById("content")) {
                    window.parent.document.getElementById("content").style.display = "none";
                }
              
                if (window.parent.document.getElementById("tollimage")) {
                    window.parent.document.getElementById("tollimage").style.display = "none";
                }
                if (window.parent.document.getElementById("Ocultaindice")) {
                    window.parent.document.getElementById("Ocultaindice").style.display = "none";
                }
                             
            }
            
        }
        if (document.getElementById("Hiddennameasigna").value == "DOCUARCHI.VISOR") {
           
            if (window.parent.document.getElementById("cuerpoindice")) {

                if (window.parent.document.getElementById("cuerpoindice")) {
                    window.parent.document.getElementById("cuerpoindice").style.width = with_frame + "px";
                }
                
                if (window.parent.document.getElementById("content")) {
                    window.parent.document.getElementById("content").style.display = "none";
                }

                if (window.parent.document.getElementById("tollimage")) {
                    window.parent.document.getElementById("tollimage").style.display = "none";
                }
                if (window.parent.document.getElementById("Ocultaindice")) {
                    window.parent.document.getElementById("Ocultaindice").style.display = "none";
                }

            }

        }
    }
    catch (err) {
        alert(err.message + " Funcion redimenciona_marco_indice");
    }
}
function windows_resize_redimenciona_marco_indice() {
    try {
        // DOCUARCHI_NET
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
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val(); Ocultaindice
            }
        }
        
        if (document.getElementById("Hiddennameasigna").value == "DOCUARCHI_NET") {           
            if (window.parent.document.getElementById("cuerpoindice")) {
                window.parent.document.getElementById("cuerpoindice").style.width = with_frame + "px";              
            }
        }
        if (document.getElementById("Hiddennameasigna").value == "DOCUARCHI.VISOR") {
            if (window.parent.document.getElementById("cuerpoindice")) {
                window.parent.document.getElementById("cuerpoindice").style.width = with_frame + "px";
            }

        }
    }
    catch (err) {
        alert(err.message + " Funcion redimenciona_marco_indice");
    }
}
function redimenciona_padre_marco() {
    try {
        //DOCUARCHI.VISOR
        if (document.getElementById("Hiddennameasigna").value == "DOCUARCHI_NET") {
            
            if (window.parent.document.getElementById("cuerpoindice")) {
                window.parent.document.getElementById("cuerpoindice").style.width = "30%";
                if (window.parent.document.getElementById("content")) { window.parent.document.getElementById("content").style.display = "block"; }
                if (window.parent.document.getElementById("tollimage")) {
                    window.parent.document.getElementById("tollimage").style.display = "block";
                }
                if (window.parent.document.getElementById("Ocultaindice")) {
                    window.parent.document.getElementById("Ocultaindice").style.display = "block";
                }
            }
        }
        if (document.getElementById("Hiddennameasigna").value == "DOCUARCHI.VISOR") { 
            if (window.parent.document.getElementById("cuerpoindice")) {
                window.parent.document.getElementById("cuerpoindice").style.width = "320px";
                if (window.parent.document.getElementById("tollimage")) {
                    window.parent.document.getElementById("tollimage").style.display = "block";
                }
                if (window.parent.document.getElementById("Ocultaindice")) {
                    window.parent.document.getElementById("Ocultaindice").style.display = "block";
                }
                if (window.parent.document.getElementById("content")) {
                    window.parent.document.getElementById("content").style.display = "block";
                }
            }
        }
      
    }
    catch (err) {
        alert(err.message + " Funcion redimenciona_padre_marco");
    }
}
function visualiza_boton_marco_padre() {
    try {
        if (window.parent.document.getElementById("Buttoncerrarimpre_indice_enlace")) {document.getElementById("Buttoncerrarimpre_indice_enlace").style.display = "block"; }
    }
    catch (err) {
        alert(err.message + " Funcion cierra_marco_padre");
    }
}
function cierra_marco_padre() {
    try {
        if (window.parent.document.getElementById("Buttoncerrarimpre_indice_enlace")) { window.parent.document.getElementById("Buttoncerrarimpre_indice_enlace").click(); }
    }
    catch (err) {
        alert(err.message + " Funcion cierra_marco_padre");
    }
}
function mueve_scroll_data_gred(buton, panel) {
    try {
       // $("#" + panel).scrollTop(70);
        $("#" + panel).scrollTop(($("#" + buton).offset().top));
    }
    catch (err) {
        alert(err.message + " Funcion mueve_scroll_data_gred");
    }
}
function mueve_scroll_value(valor, panel) {
    try {
       // $("#" + panel).scrollTop(70);
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
        if (id_element == "date_indice" && e.srcElement.value != "") {
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
                salidadato = Año_F + "/" + Mes_f + "/" + Dia_f;
                e.srcElement.value = salidadato;
            }

            if (numerocaracter == 10) {
                salidadato = Año_F + "/" + Mes_f + "/" + Dia_f;
                e.srcElement.value = salidadato;
            }

        }
    }
});