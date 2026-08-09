$(document).ready(function () {

    $.fn.inicio = function () {   
        ini_tokennize_2('tokenize-callable-demo');
        determina_tipo_notificacion();
        $('.tokenize-callable-demo').on('tokenize:tokens:remove', function (e, value) {
            delete_array_tokenize(value);
        });
        $('.tokenize-callable-demo').on('tokenize:tokens:added', function (e, value, text) {
            var ident = validator_cuenta_correo(value);
            if (ident) {
                ITEMS_DATOS_TOKENIZE_2.push({ text: text, value: value });
                var k = add_regitro_correo_service(text);
            } else {
                var token = document.getElementsByClassName("token")
                if (token) {
                    var confirma = confirm("El correo eletrónico informado nos es correcto, desea agregarlo presione aceptar");
                    if (confirma == false) {
                        for (var i = 0; i < token.length; i++) {
                            if (token[i].innerText == value) {
                                token[i].parentNode.removeChild(token[i]);
                            }
                        }
                    }

                }

            }

        });    
    }
    
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
        ini_tokennize_2('tokenize-callable-demo');
        determina_tipo_notificacion();
        
    } catch (e) {
        alert(" funcion load " + e.message);
    }

});
$('.tokenize-callable-demo').on('tokenize:tokens:add', function (e, value, text, force) {

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
function ini_tokennize_2(name_tokenize) {
    try {
        $("." + name_tokenize).tokenize2({
            placeholder: "digita correos electrónicos",
            tokensAllowCustom: true,
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
                        object.trigger('tokenize:dropdown:fill', [$items]);

                    }
                });
            }

        });
    } catch (e) { alert("Funcion ini_tokennize_2" + e.message) }
}
var ITEMS_DATOS_TOKENIZE_2 = new Array();  //GUARDA LOS ITEM SELECIONANDO EN TEX SELECTOR
function delete_array_tokenize(value_id) {
    try {
        for (var i = 0; i < ITEMS_DATOS_TOKENIZE_2.length; i++) {
            if (ITEMS_DATOS_TOKENIZE_2[i].value == value_id) {
                ITEMS_DATOS_TOKENIZE_2.splice(i, 1);
                i = ITEMS_DATOS_TOKENIZE_2.length;
            }
        }
    } catch (err) {
        alert(err.message + " Funcion delete_array_tokenize");
    }
}
function asig_array_tokenize() {
    try {
        document.getElementById("Hidden_text_user_correo").value = "";
        for (var i = 0; i < ITEMS_DATOS_TOKENIZE_2.length; i++) {
            if (i == 0) {
                document.getElementById("Hidden_text_user_correo").value = ITEMS_DATOS_TOKENIZE_2[i].text;
            } else {
                document.getElementById("Hidden_text_user_correo").value = document.getElementById("Hidden_text_user_correo").value + ',' + ITEMS_DATOS_TOKENIZE_2[i].text;
            }
        }
    } catch (err) {
        alert(err.message + " Funcion asig_array_tokenize");
    }
}
function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;

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
function hiden_cierra_ventana_principal() {
    try {
        if (window.parent.document.getElementById("Button_cerrar_notifica_gestion")) {
            window.parent.document.getElementById("Button_cerrar_notifica_gestion").click();
        }
    }
    catch (err) {
        alert(err.message + " funcion determina tipo notificacion " + err.message);
    }
}
function determina_tipo_notificacion() {
    try {
    var hiden_parent_correo = $('#Hidden_correo_envio_default', window.parent.document);
    if (hiden_parent_correo != undefined) {
        
        $('.tokenize-callable-demo').tokenize2().trigger('tokenize:tokens:add', [hiden_parent_correo[0].value, hiden_parent_correo[0].value, true]);
        
       
    }

    var Hidden_cuenta_correo_envio = $('#Hidden_cuenta_correo_envio', window.parent.document);
    if (Hidden_cuenta_correo_envio != undefined) {
        if (document.getElementById("Hidden_cuenta_correo_envio").value == "") {
            document.getElementById("Hidden_cuenta_correo_envio").value = Hidden_cuenta_correo_envio[0].value;
            document.getElementById("Label_notificar").innerHTML = "Notifica el correo " + Hidden_cuenta_correo_envio[0].value;
        }

    }
    var Hidden_tipo_notificacion = $("#Hidden_tipo_notificacion", window.parent.document);
    if (Hidden_tipo_notificacion != undefined) {
        if (document.getElementById("Hidden_tipo_notificacion").value == "") {
            document.getElementById("Hidden_tipo_notificacion").value = Hidden_tipo_notificacion[0].value;
           
        }

    }
    var hdnEmailID_VAL = $("#hdnEmailID_VAL", window.parent.document);
    if (hdnEmailID_VAL != undefined) {
        if (document.getElementById("hdnEmailID_VAL").value == "-1") {
            document.getElementById("hdnEmailID_VAL").value = hdnEmailID_VAL[0].value;

        }

    }
   /* var Hidden_ruta_tempo_ref = $("#Hidden_ruta_tempo", window.parent.document);
    if (Hidden_ruta_tempo_ref !== undefined) {
        if (document.getElementById("Hidden_ruta_tempo").value == "") {
            document.getElementById("Hidden_ruta_tempo").value = Hidden_ruta_tempo_ref[0].value;

        }

    }*/

    var Hidden_id_plantilla_radicado = $("#Hidden_id_plantilla_radicado", window.parent.document);
    if (Hidden_id_plantilla_radicado !== undefined) {
        if (document.getElementById("Hidden_id_plantilla_radicado").value == "") {
            document.getElementById("Hidden_id_plantilla_radicado").value = Hidden_id_plantilla_radicado[0].value;

        }

    }
    }
    catch (err) {
        alert(err.message + " funcion determina tipo notificacion " + err.message);
    }
}

function progres_hiden(progres) {
    $("#progres_bar").css("display", "none");
}
function posicion_update_pogres(progres) {
    try{
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
    catch (err) {
        alert(err.message + " funcion posicion_update_pogres " + err.message);
    }

}