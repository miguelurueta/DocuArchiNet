$(document).ready(function () {
    $.fn.inicio = function () {
        //cli_node_tre();
        //precarga_datos_busqueda();
        auto_zise_publico();
        auto_size_principal();
        
        $("#ocultaleft").click(function (e) {
            /*alert("X: " + e.pageX + " - Y: " + e.pageY)*/
            $("#cuerpoleft").css("display", "none")
            $("#cuerpoleft").css("width", "0.2%")
            $("#pie_cuerpo_left").css("display", "none")
            $("#pie_cuerpo_left").css("width", "0.2%")
            $("#tre").css("overflow", "hidden")
            $("#cuerporigth").css("width", "100%")
            $("#Ocultarigth").css("display", "block")
            $("#ocultaleft").css("display", "none")
            $("#TreeView1").css("display", "none")


        });

        $("#Ocultarigth").click(function (e) {
            /*alert("X: " + e.pageX + " - Y: " + e.pageY)*/
            $("#cuerpoleft").css("display", "block")
            $("#cuerpoleft").css("width", "16.5%")
            $("#pie_cuerpo_left").css("display", "block")
            $("#pie_cuerpo_left").css("width", "16.5%")
            $("#tre").css("overflow", "auto")
            $("#cuerporigth").css("width", "83.2%")
            $("#Ocultarigth").css("display", "none")
            $("#ocultaleft").css("display", "block")
            $("#TreeView1").css("display", "block")
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
        inicializa_menu_scoope();
        window.addEventListener("resize", rezize_event);
        //ShowModalPopup("ModalPopupExtender_mensaje_personalizado_backgroundElement", "Panel_mensaje_personalizado", 100001);


    } catch (e) {
        alert(" funcion load " + e.message);
    }

});
var ITEMS_DATOS_TOKENIZE_2 = new Array();

var stat_item = 1;

function inicializa_menu_scoope() {
    web_service_lista_item_menu();
    if (stat_item == 0) {
        stat_item = 1;
        ITEMS_DATOS_TOKENIZE_2.push({ Text_node: "Workflow ", value_node: "WF-WF-01", Toltip_node: "", url_node: "", visible_node: "1", nodo_plantilla_radicado: "", tipo_plantilla: "1", id_plantilla: "0" });
        ITEMS_DATOS_TOKENIZE_2.push({ Text_node: "Flujos y tareas ", value_node: "WF-CL-01", Toltip_node: "Despliega los flujos y las tareas", url_node: "../workflow/Webworkflow.aspx", visible_node: "1", nodo_plantilla_radicado: "", tipo_plantilla: "1", id_plantilla: "0" });
        ITEMS_DATOS_TOKENIZE_2.push({ Text_node: "Rerportes de flujos y tareas ", value_node: "WF-RW-03", Toltip_node: "Lista los reportes de flujos y tareas", url_node: "../Workflow/WebFormContenedorPageWF.aspx", visible_node: "1", nodo_plantilla_radicado: "", tipo_plantilla: "1", id_plantilla: "0" });
        ITEMS_DATOS_TOKENIZE_2.push({ Text_node: "Gestión de flujos y tareas ", value_node: "WF-GF-04", Toltip_node: "Gestiona los flujos y tareas", url_node: "../Workflow/WebFormContenedorPageWF.aspx", visible_node: "1", nodo_plantilla_radicado: "", tipo_plantilla: "1", id_plantilla: "0" });
        ITEMS_DATOS_TOKENIZE_2.push({ Text_node: "Consulta de documentos ", value_node: "WF-CD-05", Toltip_node: "Gestiona los flujos y tareas", url_node: "../Workflow/WebFormContenedorPageWF.aspx", visible_node: "0", nodo_plantilla_radicado: "", tipo_plantilla: "1", id_plantilla: "0" });
        ITEMS_DATOS_TOKENIZE_2.push({ Text_node: "Administración de rutas ", value_node: "WF-DR-06", Toltip_node: "Administración de rutas y tareas", url_node: "../workflow/WebWorkflowDigramaRuta.aspx", visible_node: "1", nodo_plantilla_radicado: "", tipo_plantilla: "1", id_plantilla: "0" });
        ITEMS_DATOS_TOKENIZE_2.push({ Text_node: "Administración de flujos ", value_node: "WF-DF-07", Toltip_node: "Administración flujos", url_node: "../workflow/WebFormDiagramadorFlujoTrabajo.aspx", visible_node: "1", nodo_plantilla_radicado: "", tipo_plantilla: "1", id_plantilla: "0" });
        ITEMS_DATOS_TOKENIZE_2.push({ Text_node: "Autenticación ", value_node: "WF-PC-09", Toltip_node: "Administración flujos", url_node: "../workflow/WebFormDiagramadorFlujoTrabajo.aspx", visible_node: "0", nodo_plantilla_radicado: "", tipo_plantilla: "1", id_plantilla: "0" });
        ITEMS_DATOS_TOKENIZE_2.push({ Text_node: "RADGESTION ", value_node: "-----", Toltip_node: "Administración flujos", url_node: "../workflow/WebFormDiagramadorFlujoTrabajo.aspx", visible_node: "1", nodo_plantilla_radicado: "yes", tipo_plantilla: "1", id_plantilla: "10" });
        for (i = 0; i < ITEMS_DATOS_TOKENIZE_2.length; i++) {
            if (ITEMS_DATOS_TOKENIZE_2[i].visible_node == 0) {
                if (document.getElementById(ITEMS_DATOS_TOKENIZE_2[i].value_node)) {
                    var ob_remo = document.getElementById(ITEMS_DATOS_TOKENIZE_2[i].value_node);
                    ob_remo.remove();

                }
            }

        }
        //Agrega los item de plantilla de radicacion dinamica
        if (document.getElementById("CR-PR-11")) {
            for (i = 0; i < ITEMS_DATOS_TOKENIZE_2.length; i++) {
                if (ITEMS_DATOS_TOKENIZE_2[i].nodo_plantilla_radicado == "yes") {
                    //crea el elemento ul del menu
                    var element_ul = document.createElement("ul");
                    element_ul.classList.add("scoop-submenu");

                    //crea elemento li
                    var element_li = document.createElement("li");
                    var na = "scoop-trigger";
                    element_li.classList.add(na);

                    //Agrega elemento li
                    element_ul.appendChild(element_li);

                    //Agrega elemento a
                    var element_a = document.createElement("A");
                    //element_a.href="javascript:void(0)";
                    element_a.href = "#";

                    //Agrega elemento span contenedor del elemento i del icono
                    var element_span = document.createElement("span");
                    element_span.classList.add("scoop-micon");

                    //Agrega elemento de i del icono
                    var element_i = document.createElement("I");
                    element_i.classList.add("icon-chart");
                    element_span.appendChild(element_i);
                    element_a.appendChild(element_span);

                    //Agrega elemento span con el nombre del menu
                    element_span = document.createElement("span");
                    element_span.classList.add("scoop-mtext");
                    element_span.textContent = ITEMS_DATOS_TOKENIZE_2[i].Text_node;
                    element_a.appendChild(element_span);

                    //Agrega el elemento span del marcado
                    element_span = document.createElement("span");
                    element_span.classList.add("scoop-mcaret");
                    element_a.appendChild(element_span);

                    element_li.appendChild(element_a);

                    var element_ = document.getElementById("CR-PR-11");

                    element_.appendChild(element_ul);


                }
            }

        }
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
function rezize_event() {
    try {
        auto_zise_publico();
        auto_size_principal();
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
function web_service_lista_item_menu() {
    try {
        var search = "";
        $.ajax('../webservice/WebServiceInicioGestor.asmx/web_service_lista_item_menu', {
            data: "{'DName':'" + search + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d[0].error_funcion !== "YES") {
                    alert("Error funcion  web_service_lista_item_menu " + data.d[0].error_funcion)
                    return false;
                } else {
                    $.each(data.d, function (k, v) {
                        ITEMS_DATOS_TOKENIZE_2.push(v);
                    });
                }
            },
            error: function (errorText) {
                alert("Error web_service_lista_item_menu : " + errorText.responseText);
            }
        });

    } catch (ex) { alert("Funcion web_service_lista_item_menu " + ex.message); }
}
function cli_node_tre() {
    try {
        $("#tre .LeafNodeStyle a").click(function () {

            var url = $(this).text();
            if (url == "Reportes de flujos y tareas") {
                document.getElementById("ContentPlacenter_Hidden_selecion_url").value = "../workflow/WebFormReportesWorkflow.aspx";
                document.getElementById("ContentPlacenter_Hidden_tipo_contenido_content").value = "REPORTES WORKFLOW";
                var w = window.open('../Workflow/WebFormContenedorPageWF.aspx', '_blank');
                //var w = window.open('../workflow/WebFormWorkflowExterno.aspx', '_blank');
                //w.document.title = "Reportes Workflow"
                w.focus();
                return false;

            }
            if (url == "Reportes de gestión documental") {
                document.getElementById("ContentPlacenter_Hidden_selecion_url").value = "../radicador/WebFormReportesRadicacion.aspx";
                document.getElementById("ContentPlacenter_Hidden_tipo_contenido_content").value = "REPORTES GESTION";
                //var w = window.open('../Workflow/WebFormContenedorPageWF.aspx', '_blank');
                var w = window.open('../radicador/WebFormRadicadoExterno.aspx', '_blank');
                //w.document.title = "Reportes gestión"
                w.focus();
                return false;
            }
            if (url == "Reportes gestión") {
                document.getElementById("ContentPlacenter_Hidden_selecion_url").value = "../radicador/WebFormReportesRadicacion.aspx";
                document.getElementById("ContentPlacenter_Hidden_tipo_contenido_content").value = "REPORTES GESTION";
                //var w = window.open('../Workflow/WebFormContenedorPageWF.aspx', '_blank');
                var w = window.open('../radicador/WebFormRadicadoExterno.aspx', '_blank');
                //w.document.title = "Reportes gestión"
                w.focus();
                return false;
            }
            if (url == "Consultar documentos") {
                document.getElementById("ContentPlacenter_Hidden_selecion_url").value = "../Docuarchi/WebFormDaPrincipal.aspx";
                document.getElementById("ContentPlacenter_Hidden_tipo_contenido_content").value = "CONSULTA DOCUMENTOS";
                var w = window.open('../Workflow/WebFormContenedorPageWF.aspx', '_blank');
                //w.document.title = "Consulta Docuarchi web"
                w.focus();
                return false;
            }
            //Gestión de tareas y flujos de trabajo
            if (url == "Gestión de tareas y flujos de trabajo") {
                document.getElementById("ContentPlacenter_Hidden_selecion_url").value = "../workflow/WebFormGestionFlujoTrabajoCamaras.aspx";
                document.getElementById("ContentPlacenter_Hidden_tipo_contenido_content").value = "GESTION FLUJOS";
                var w = window.open('../Workflow/WebFormContenedorPageWF.aspx', '_blank');
                //w.document.title = "Consulta Docuarchi web"
                w.focus();
                return false;
            }

        });
    }
    catch (err) {
        alert(err.message + " Funcion cli_node_tre");
    }
};
function precarga_datos_busqueda() {
    var data = CargarDatos_ini();
    $("#ContentPlacenter_TextBox_busqueda").autocomplete({

        source: data,
        select: function (event, ui) {
            Seleccionar(ui.item.value);
            return ui.item.value;
         }
      
    })

}
function CargarDatos_ini() {
    var treeN = $(".TreeN_");
    //var items = new Array();
    var items = new Array();
    if (treeN.length) {

        var nodo = $("#" + treeN[0].id + " td: a");

        for (var index = 0; index < nodo.length; index++) {

            //items[index] = { text: nodo[index].innerText, id: nodo[index].id };
            items[index] = nodo[index].innerText;

        }

    }

    return items;

}
function Seleccionar(treeID) {

    var treeN = $(".TreeN_");

    //var items = new Array();

    if (treeN.length) {

        var nodo = $("#" + treeN[0].id + " td: a");

        for (var index = 0; index < nodo.length; index++) {
            var nodval = nodo[index].innerText;
            if (nodval == treeID) {
                //nodo[index].innerText = "ojo";
                document.getElementById("ContentPlacenter_Hidden_texto_buequeda").value = nodo[index].innerText;
                document.getElementById("ContentPlacenter_Button_activa_busqueda_treview").click();
                return true;
            }
            /*if (nodval.search(datos) != -1) {
                nodo[index].innerText = datos + " (" + numero_solicitudes + ")";
                nodo[index].innerHTML = datos + " (" + numero_solicitudes + ")";
            }
            items[index] = { text: nodo[index].innerText, id: nodo[index].id, value: nodo[index].nodeValue };*/

        }

    }
    //Limpiamos los estilos seleccionados anteriormente
    /*
        var es = $('.SelectedNodeTree');
    
        for (var index = 0; index < es.length; index++) {
    
            es[index].className = "ItemNodeTree";
    
        }
    
    
    
        //Definimos el estilo seleccionado al item correcto
    
        $("#" + treeID)[0].className = "SelectedNodeTree";*/

}
function CargarDatos() {


    var treeN = $(".TreeN_");

    var items = new Array();

    if (treeN.length) {

        var nodo = $("#" + treeN[0].id + " td: a");

        for (var index = 0; index < nodo.length; index++) {
            var nod = nodo[index];
            items[index] = { text: nodo[index].innerText, id: nodo[index].id, value: nodo[index].nodeValue };

        }

    }

    return items;

}
function auto_size_principal() {
    try {
    var espacio_iframe;
    if (window.innerHeight) {
        //navegadores basados en mozilla 
        espacio_iframe = window.innerHeight
    } else {
        if (document.hidden == true) {
            if (document.body.clientHeight != undefined) {
                //Navegadores basados en IExplorer, es que no tengo innerheight 
                espacio_iframe = document.body.clientHeight
            } else {
                //otros navegadores 
                espacio_iframe = 478
            }
        }
    }
    

    var altoCab = $("#cabecera").height();
    //var altoPie = $("#Piepagina").height();
    //var altoPag = $("body").height();
    //alert(altoPag);
    $("#body").css("height", (espacio_iframe - altoCab ) + "px");
    $("#form1").css("height", (espacio_iframe - altoCab - 2 ) + "px");
    }
    catch (err) {
        alert(err.message + " Funcion auto_size_principal");
    }
}


function oculta_lef() {
    try {
        
        if (document.getElementById("ContentPlacenter_Hiddenseleccion").value == "PUBLICO") {
            $('#ContentPlacenter_cuerpoleft').css("display", "none");
            //$("#cuerpoleft", window.parent.document).css("width", "0.2%")
            $("#ContentPlacenter_pie_cuerpo_left").css("display", "none")
            //$("#pie_cuerpo_left", window.parent.document).css("width", "0.2%")
            //$("#tre", window.parent.document).css("overflow", "hidden")
            $("#ContentPlacenter_cuerporigth").css("width", "100%")
            $("#ContentPlacenter_Ocultarigth").css("display", "none")
            $("#ContentPlacenter_ocultaleft").css("display", "none")
            //$("#TreeView1"), window.parent.document.css("display", "none")
        }
        
    }
    catch (err) {
        alert(err.message + " funcion oculta_lef " + err.message);
    }
}
function auto_zise_publico() {
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
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val(); Contenido_consulta_documento tol_pie

            }
        }
        var heit_conten = 80;
        if (document.getElementById("cabecera")) {
            heit_conten = document.getElementById("cabecera").clientHeight + 20;
        }
        $('#ContentPlacenter_ifrm_ds_').css("height", (espacio_iframe - heit_conten) + "px");
    }
    catch (err) {
        alert(err.message + " funcion auto_zise_publico " + err.message);
    }
}

function web_service_solicitudes_usuario(datas,hiden) {
    try {       
        var obj = {};   
        var jsonData = JSON.stringify(obj);
        $.ajax({
            url: '../radicador/' + datas,
            type: 'POST',
            data: jsonData,
            success: function (data) {
                //alert(data);
                document.getElementById(hiden).value = data;
               
            },
            error: function (errorText) {
                //alert("Error general funcion axion_script !" + errorText);
            }
        });
    }
    catch (err) {
        //alert(err.message + " Funcion web_service_solicitudes_usuario");
    }
}
function web_service_test_db(datas) {
    try {
        var obj = {};
        var jsonData = JSON.stringify(obj);
        $.ajax({
            url: datas,
            type: 'POST',
            data: jsonData,
            success: function (data) {
                //alert(data);
                //document.getElementById(hiden).value = data;

            },
            error: function (errorText) {
                //alert("Error general funcion axion_script !" + errorText);
            }
        });
    }
    catch (err) {
        //alert(err.message + " Funcion web_service_test_db");
    }
}


function web_service_solicitudes_db(datas, hiden) {
    try {
        var obj = {};
        var jsonData = JSON.stringify(obj);
        $.ajax({
            url: '../radicador/' + datas,
            type: 'POST',
            data: jsonData,
            success: function (data) {
                //alert(data);
                document.getElementById(hiden).value = data;

            },
            error: function (errorText) {
                //alert("Error general funcion axion_script !" + errorText);
            }
        });
    }
    catch (err) {
        //alert(err.message + " Funcion web_service_solicitudes_usuario");
    }
}
function set_actualiza_log_sesion_usuario_gestion_documental() {
    try {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "../webservice/WebServiceGestorDocumental.asmx/Service_actualiza_log_sesion_usuario_gestion_documental",
                data: "{'name':'" + name + "'}",
                dataType: "json",
                success: function (data) {
                    //response(data.d);
                    if (data.d !== "YES") {
                        //alert(data.d);
                    }
                },
                error: function (result) {
                    //alert("Error......" + result);
                    
                }
            });
    } catch (ex) {
    }
}
function remplaza_datos_solicitudes_usuario(clave_busqueda,url_service,noticia_popou,hiden) {
    try {
    //document.getElementById("ContentPlacenter_Hidden_resultado_web_service").value = "";
        web_service_solicitudes_usuario(url_service, hiden);
        if (document.getElementById(hiden).value != "") {
            RemplazarDatos_solicitudes_usuario(clave_busqueda, document.getElementById(hiden).value);
            if (document.getElementById(hiden).value > 0 && noticia_popou != "") {
                create_element_popup(noticia_popou + document.getElementById(hiden).value);
        }
    }
    
}
    catch (err) {
    //alert(err.message + " Funcion web_service_solicitudes_usuario");
}
}
function remplaza_datos_documentos_compartidos(clave_busqueda, url_service, noticia_popou, hiden) {
    try {
        //document.getElementById("ContentPlacenter_Hidden_resultado_web_service").value = "";
        web_service_solicitudes_usuario(url_service, hiden);
        if (document.getElementById(hiden).value != "") {
            RemplazarDatos_solicitudes_usuario(clave_busqueda, document.getElementById(hiden).value);
            if (document.getElementById(hiden).value > 0 && noticia_popou != "") {
                create_element_popup(noticia_popou + document.getElementById(hiden).value);
            }
        }

    }
    catch (err) {
        //alert(err.message + " Funcion web_service_solicitudes_usuario");
    }
}
function listas_documentos_compartidos_por_revision(clave_busqueda, url_service, noticia_popou, hiden) {
    try {
        web_service_solicitudes_db(url_service, hiden);
        if (document.getElementById(hiden).value != "") {
            //Crea el elemento de alarma
            if (document.getElementById(hiden).value > 0 && noticia_popou != "") {
                create_element_popup(noticia_popou);
            }
        }
    }
    catch (err) {
        //alert(err.message + " Funcion listas_solicitudes_pendientes_por_aprobacion");
    }
}
function listas_solicitudes_pendientes_por_aprobacion(clave_busqueda, url_service, noticia_popou, hiden) {
    try {
        web_service_solicitudes_db(url_service, hiden);
        if (document.getElementById(hiden).value != "") {
            //Crea el elemento de alarma
            if (document.getElementById(hiden).value > 0 && noticia_popou != "") {
                create_element_popup(noticia_popou);
        }
    }
}
    catch (err) {
        //alert(err.message + " Funcion listas_solicitudes_pendientes_por_aprobacion");
}
}
function RemplazarDatos_solicitudes_usuario(datos, numero_solicitudes) {
    //Remplaza los nuevos datos en el treview
    try {
    var treeN = $(".TreeN_");

    var items = new Array();

    if (treeN.length) {

        var nodo = $("#" + treeN[0].id + " td: a");

        for (var index = 0; index < nodo.length; index++) {
            var nodval = nodo[index].innerText;
            if (nodval.search(datos) != -1) {
                nodo[index].innerText = datos + " (" + numero_solicitudes + ")";
                nodo[index].innerHTML = datos + " (" + numero_solicitudes + ")";
            }
            items[index] = { text: nodo[index].innerText, id: nodo[index].id, value: nodo[index].nodeValue };

        }

    }

    return items;
    }
    catch (err) {
        //alert(err.message + " Funcion RemplazarDatos_solicitudes_usuario");
    }
}
function create_element_popup(texto_popup) {
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
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val(); Contenido_consulta_documento tol_pie

            }
        }
       
        var document_parent = document.getElementById("pie_cuerpo_left");
        var documento = document.getElementById("myModal");
        $('#myModal').css("width", "400px");
        $('#myModal').css("height", "100px");
        var i = espacio_iframe - (document_parent.clientHeight + 100);
        document.getElementById("tex_modal").innerHTML = texto_popup;
        documento.style.top = i + "px";
        documento.style.display = "block";
        $('#myModal').show(1000);
        setTimeout('hide("#myModal")', 60000);
      
    }
    catch (err) {
        alert(err.message + " Función create_element_popup");
    }
}
function hide(content) {
    $(content).hide(3000);
}
function hide_autonomo() {
    document.getElementById("myModal").style.display = "none";
}
function posicion_update_pogres(progres) {
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
        alert(err.message + " Funcion posicion_update_pogres");
    }

}
function progres_hiden(progres) {
    try {
        $("#progres_bar").css("display", "none");
    }
    catch (err) {
        alert(err.message + " Funcion progres_hiden");
    }
}