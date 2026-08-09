
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


    $(document).ready(bodyResize);
    $(window).resize(bodyResize);
    function bodyResize() {

        document.getElementById("vista").style.display = "block";
        //CKEDITOR.instances['htmlEditor'].resize(with_frame - 10, ((espacio_iframe - 200) - document.getElementById("vista").offsetHeight));
        
    }
}
function cargar_win() {
    var obj = {};
    obj.m_ckeditor =  CKEDITOR.instances['htmlEditor'].getData();
    obj.m_id_respuesta = document.getElementById("Hidden_id_respuesta").value;
    //http://codepedia.info/generic-handler-ashx-file-post-send-json-data-parameters-in-asp-net-c-jquery/
    var jsonData = JSON.stringify(obj);
    if (document.getElementById("Hidden_id_respuesta").value == "-1") {
        alert("El sistema no detecto la identificación de la respuesta");
        return false;
    }
    var mensaje = confirm("¿Desea guardar los cambios?");
    //Detectamos si el usuario acepto el mensaje
    if (mensaje) {
  
    }
        //Detectamos si el usuario denegó el mensaje
    else {
      
        return false;
    }
    $.ajax({
        url: 'Handler_file_guardar_chkd.ashx',
        type: 'POST',
        data: jsonData,
        success: function (data) {
            //console.log(data);
            //alert("Success :" + data);
            if (data != "YES" && data != "Guarda_documento_nuevo_respuesta") {
                alert("Success :" + data);
            } else {

                if (data == "Guarda_documento_nuevo_respuesta") {
                    window.parent.document.getElementById("Button_antualiza_semaforo_chkeditor").click();
                    var i = 1;
                }
            }
        },
        error: function (errorText) {
            alert("Error general funcion axion_script !" + errorText);
        }
    });
}