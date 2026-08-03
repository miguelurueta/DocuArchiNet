$(document).ready(function () {
    $.fn.inicio = function () { 
        inicia_variables();
        //redimenciona_popup_gestion_externo();
    }


})
function inicia_variables() {
    try {
        
        if (window.document !== undefined) {
            if (document.getElementById("Hidden_estado_update").value == "-1"){
                document.getElementById("Hidden_radicado").value = window.parent.document.getElementById("Hidden_radicado").value;
            document.getElementById("Hidden_id_respuesta").value = window.parent.document.getElementById("Hidden_id_respuesta").value;
            document.getElementById("Button_hident").click();
        }
              
        }
    } catch (err) {
        alert(err.message + " funcion inicia_variables " + err.message);
    }
}
function redimenciona_popup_gestion_externo() {
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
        document.getElementById("Panel_valiacion_plantilla").style.height = espacio_iframe + "px";
        document.getElementById("Contenido_validacion_plantilla").style.height = espacio_iframe + "px";
        document.getElementById("Iframe_validacion_plantilla_").style.height = espacio_iframe + "px";
    }
    catch (err) {
        alert(err.message + " funcion redimenciona_popup_gestion_externo " + err.message);
    }
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
        document.getElementById("Hidden_height").value = espacio_iframe - 20;
        document.getElementById("Hidden_width").value = with_frame - 20;
       
}
 catch (err) {
     alert(err.message + " funcion asigna_datos_heig_with " + err.message);
}
}
function cargar() {

    // CKEDITOR(document.getElementById("htmlEditor"), "c:\salida\TemplateDocument.html", "TemplateDocument.html");
    var file = "c:\salida\TemplateDocument.html";
    //var loa_der = CKEDITOR.editor.uploadRepository.create(file);
    var t = CKEDITOR.fileTools.uploadRepository
    CKEDITOR.fileTools.fileLoader(document.getElementById("htmlEditor"), "c:\salida\TemplateDocument.html", "TemplateDocument.html");

}
function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;
    htmlEditor
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
  var ele=  document.getElementsByClassName("ajax__fileupload_fileItemInfo");
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
        alert(err.message + " funcion eliminar_ajaxtolkit " + err.message);
    }
}