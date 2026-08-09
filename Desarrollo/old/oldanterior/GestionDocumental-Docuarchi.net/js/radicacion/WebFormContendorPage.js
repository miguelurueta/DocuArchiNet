$(document).ready(function () {
    $.fn.inicio = function () {

        determina_tipo_notificacion();
        auto_zise_iframe();
        
    }


})
function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;

}
function actualiza_documento_relacionado() {
    try {
        window.opener.document.getElementById("Button_actualiza_trevie_seleccion").click();
        document.getElementById("ContentPlacenter_Hidden_estado_actualizacion").value = "YES";
        //alert("ojo");
    }
    catch (err) {
        
        alert(err.message + " funcion actualiza_documento_relacionado " + err.message);
   
}
}
function determina_cambio_tarea_seleccionada() {
    try {
        if (window.opener !== undefined) {
            var Hidden_id_tarea_selecionada = window.opener.document.getElementById("Hidden_id_tarea_selecionada");
            if (Hidden_id_tarea_selecionada == undefined) {
                document.getElementById("ContentPlacenter_ifrm_ds_").style.display = "none";
                return false;
            } else {
                if (document.getElementById("ContentPlacenter_Hidden_id_tarea_selecionada").value !== Hidden_id_tarea_selecionada.value) {
                    document.getElementById("ContentPlacenter_Hidden_id_tarea_selecionada").value = Hidden_id_tarea_selecionada.value;
                    if (Hidden_id_tarea_selecionada.value == "0") {
                        document.title = "Caduco el formulario de respuesta ";
                        //document.getElementById("ContentPlacenter_ifrm_ds_").style.display = "none";
                        document.getElementById("ContentPlacenter_ifrm_ds_").src = "../Gestion/WebFormContenidoCaducado.aspx";

                    } else {
                        //document.getElementById("ContentPlacenter_ifrm_ds_").src = "../radicador/WebFormRespuestaRadicado.aspx";
                    }
                   
                    
                }
                
            }
        }
    }
    catch (err) {
        
        //alert(err.message + " funcion determina_cambio_tarea_seleccionada " + err.message);
        window.close();
    }
}
function determina_tipo_notificacion() {
    try {
        if (window.opener !== undefined) {
            var winpar = window.opener.document;
            var hiden_parent_correo = window.opener.document.getElementById("Hidden_tipo_contenido");
            document.getElementById("ContentPlacenter_Hidden_tipo_conten").value = "contenedor";
            if (hiden_parent_correo == undefined) {
                document.getElementById("ContentPlacenter_ifrm_ds_").style.display = "none";
                return false;
            } else {
                if (hiden_parent_correo.value == "RESPUESTA") {
                    var Hidden_id_tarea_selecionada = window.opener.document.getElementById("Hidden_id_tarea_selecionada");
                    if (Hidden_id_tarea_selecionada == undefined) {
                        document.getElementById("ContentPlacenter_ifrm_ds_").style.display = "none";
                        return false;
                    } else {
                        if (document.getElementById("ContentPlacenter_Hidden_estado_actualizacion").value !== "YES") {
                            document.getElementById("ContentPlacenter_Hidden_id_tarea_selecionada").value = Hidden_id_tarea_selecionada.value;
                            document.getElementById("ContentPlacenter_ifrm_ds_").src = "../radicador/WebFormRespuestaRadicado.aspx";
                            document.title = window.opener.document.getElementById("Hidden_radic_select").value;
                        } else {
                            //document.getElementById("ContentPlacenter_Hidden_estado_actualizacion").value = "";
                        }
                        
                    }
                }
            }

        }

        
    }
    catch (err) {
        alert(err.message + " funcion determina tipo notificacion " + err.message);
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
        $('#ContentPlacenter_ifrm_ds_').css("height", (espacio_iframe - 100) + "px");
        $('#ContentPlacenter_ifrm_ds_').css("width", (with_frame - 20) + "px");
    }


}