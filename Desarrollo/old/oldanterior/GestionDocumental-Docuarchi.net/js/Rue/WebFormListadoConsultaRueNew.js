$(document).ready(function () {
    $.fn.inicio = function () {
          
        $(window).resize(bodyResize);
        function bodyResize() {
           
        }
    }
})
let ParramRue = "";
let CodigoCamara = "";
//-----------------------ZONA LOAD-------------------------------------------
$(window).on("load", function () {
    try {
        var elment = document.getElementsByClassName("da_event_captive");
        if (elment) {
            for (var i = 0; i < elment.length; i++) {
                elment[i].addEventListener("click", event_click, false);
            }
        }
        ini_event_page();
        IniciaConsultaRue();
        INTERVAL_SESION_ITEM_MANTENT_SSION_GESTOR = setInterval('Service_REST_validate_sesion_tab();', '6030');
      
    } catch (e) {
        alert(" funcion load " + e.message);
    }

});
const ini_event_page = () => {
    let array_element = new Array;
    array_element.push({ id: "Button_div_datos_ingreso" });
    for (let i = 0; i < array_element.length; i++) {
        let elment_a_document_production = document.getElementById(array_element[i].id);
        if (elment_a_document_production) {
            elment_a_document_production.addEventListener("click", handler_element_event, false);
        }
    }
}
const handler_element_event = (e) => {
    try {
        let name_ID = e.currentTarget.id;
        let result = "";
        delete_alert_boot();
        switch (name_ID) {
            case "help_sear_documente":
                
                break;
            
        }
    } catch (ex) {
        alert(ex.mensaje);
    }
}
const event_element_click_promise = async (e) => {
    let name_control = e.currentTarget.id;
    try {
        let result = "";
        delete_alert_boot(); 
    }
    catch (ex) {
        alert_bot(ex.message, 'warning', "error_div_container_general");
    } finally {
        progres_hiden('progres_bar');
        document.getElementById(name_control).disabled = false;
    }
}
const IniciaConsultaRue = async () => {
    try {
        delete_alert_boot();
        posicion_update_pogres('progres_bar');
        let Result = "";
        /*Depende del load del formulario webformlistadoconsutlarue.aspx.vb*/
        ParramRue = document.getElementById("HiddenParramRue").value;
        CodigoCamara = document.getElementById("HiddenCamaraRue").value;
        /*** Dependendencia del archivo JSRues */
        let _OPtionRues = ({
            NameService: "ServiceConsultaExpedienteRue",
            ParamRue: ParramRue, CodigoCamara: CodigoCamara, NameContendorVisor: "NameVisorRueDocumento",
            NameControlPadreVisor: "div_parent_general", NameControlError: "errorgeneralrue"
        });
        /*Inicia la consulta de docuentos rue*/
        Result = await JSRue(_OPtionRues);
        if (Result != "YES") {
            alert_bot(Result, 'warning', "errorgeneralrue");
        }
        return Result;
    } catch (ex) {
    } finally {
        progres_hiden('progres_bar');
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