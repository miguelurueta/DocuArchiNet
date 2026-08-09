$(document).ready(function () {
    $.fn.inicio = function () {

        determina_tipo_notificacion();
        auto_zise_iframe();
        $(window).resize(bodyResize);
        function bodyResize() {
            auto_zise_iframe();

        }

    }
    

})
$(window).on("load", function () {
    try {
        INTERVAL_SESION_ITEM_MANTENT_SSION_GESTOR = setInterval('Service_REST_validate_sesion_tab();', '6030');
    } catch (e) {
        alert(" funcion load " + e.message);
    }

});
function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;

}

function determina_tipo_notificacion() {
    try {
        if (window.opener !== undefined) {
            var winpar = window.opener.document;
            var hiden_parent_correo = window.opener.document.getElementById("Hidden_tipo_contenido_content");
            let option_paren_span = document.getElementById("option_paren_span");
            if (hiden_parent_correo == undefined) {
                document.getElementById("ContentPlacenter_ifrm_ds_").style.display = "none";
                return false;
            } else {
                if (hiden_parent_correo.value == "CONSULTA DOCUMENTOS") {
                    document.getElementById("ContentPlacenter_ifrm_ds_").src = "../Docuarchi/WebFormDaPrincipal.aspx";
                    document.title = "CONSULTA DOCUMENTOS";
                    option_paren_span.innerHTML = "Consulta de documentos";
                }
                if (hiden_parent_correo.value == "REPORTES GESTION") {
                    document.getElementById("ContentPlacenter_ifrm_ds_").src = "../radicador/WebFormRadicadoExterno.aspx";
                    document.title = "REPORTES GESTION";
                    option_paren_span.innerHTML = "Reportes gestión";
                }
                if (hiden_parent_correo.value == "REPORTES WORKFLOW") {
                    document.getElementById("ContentPlacenter_ifrm_ds_").src = "../workflow/WebFormReportesWorkflow.aspx";
                    document.title = "REPORTES WORKFLOW";
                    option_paren_span.innerHTML = "Reportes workflow";
                }
                if (hiden_parent_correo.value == "GESTION FLUJOS") {
                    document.getElementById("ContentPlacenter_ifrm_ds_").src = "../workflow/WebFormGestionFlujoTrabajoCamaras.aspx";
                    document.title = "GESTIÓN TAREAS Y FLUJOS DE TRABAJO";
                    option_paren_span.innerHTML = "Gestión de tareas y flujos de trabajo";
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

    $('#ContentPlacenter_ifrm_ds_').css("height", (espacio_iframe - document.getElementById("id_scoop_header").clientHeight) + "px");
}