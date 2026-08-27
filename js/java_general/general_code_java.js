function event_elemento(event, nue_event, e) {
    document.getElementById("Hidden_lik_service_boton").value = nue_event;
    document.getElementById("Button_lik_service_boton").click();
    document.getElementById("myDropdown").classList.remove("show_filter");
    var matri_clas = document.getElementsByClassName("e_list_marc");
    for (i = 0; i < matri_clas.length; i++) {
        var text = matri_clas[i].innerHTML;
        var index = text.indexOf("✓");
        if (index !== -1) {
            var temp_iner = matri_clas[i].innerHTML;
            matri_clas[i].innerHTML = temp_iner.replace("✓", '');
        }
    }
    document.getElementById("Label_anunciado_filtro").innerHTML = "&#10003 " + e.innerHTML;
    e.innerHTML = "&#10003" + e.innerHTML;
    event.preventDefault();
}
function myFunction(event, thiss) {
    document.getElementById("myDropdown").classList.toggle("show_filter");
    event.preventDefault();
}

function registrar_evento_documento(nombre_evento, manejador) {
    var receptor_jquery;
    if (typeof window.jQuery === "function") {
        try {
            receptor_jquery = window.jQuery(document);
            if (receptor_jquery && typeof receptor_jquery.on === "function") {
                receptor_jquery.on(nombre_evento, manejador);
                return;
            }
        } catch (error_jquery) {
            receptor_jquery = null;
        }
    }
    if (document.addEventListener) {
        document.addEventListener(nombre_evento, manejador, false);
    } else if (document.attachEvent) {
        document.attachEvent("on" + nombre_evento, manejador);
    }
}

function obtener_evento_documento(evento) {
    return evento || window.event || null;
}

registrar_evento_documento("keydown", function (tecla) {
    tecla = obtener_evento_documento(tecla);
    if (tecla && tecla.keyCode == 27) {
        if (document.getElementById("myDropdown")) {
            document.getElementById("myDropdown").classList.remove("show_filter");
        }
        
    }
});
registrar_evento_documento("click", function (e) {
    e = obtener_evento_documento(e);
    var elemento_objetivo = e && (e.target || e.srcElement);
    if (!elemento_objetivo) return;
    if (elemento_objetivo.id !== "myDropdown" && elemento_objetivo.id !== "div_filtro__fil" && elemento_objetivo.id !== "boton__filtro_ver" && elemento_objetivo.id !== "myInput") {
        if (document.getElementById("myDropdown")) {
            document.getElementById("myDropdown").classList.remove("show_filter");
        }
       
    }
});

function filterFunction() {
    var input, filter, ul, li, a, i;
    input = document.getElementById("myInput");
    filter = input.value.toUpperCase();
    div = document.getElementById("myDropdown");
    a = div.getElementsByTagName("a");
    for (i = 0; i < a.length; i++) {
        if (a[i].innerHTML.toUpperCase().indexOf(filter) > -1) {
            a[i].style.display = "";
        } else {
            a[i].style.display = "none";
        }
    }
}

function activa_menu_general_diference(event,e,event_name) {
    try
    {
        if (event_name !== "") {
            document.getElementById("Hidden_menu_var_event_dive").value = event_name;
            document.getElementById("Button_me_active_men_dive").click();
        }
        
        event.preventDefault();
    }
    catch (ex) {
        alert("Inconsistencia general function activa_menu_general_diference " + ex.message)
    }
}
function activa_menu_general_diference_event(event, e, event_name,tipo_event) {
    try {
        if (event_name !== "") {
            if (tipo_event == "DELETE") {
                var r = confirm("Desea eliminar el elemento");
                if (r == true) {
                    document.getElementById("Hidden_menu_var_event_dive").value = event_name;
                    document.getElementById("Hidden_edita_red_event").value = tipo_event;
                    document.getElementById("Button_me_active_men_dive").click();
                }
            } else {
                document.getElementById("Hidden_menu_var_event_dive").value = event_name;
                document.getElementById("Hidden_edita_red_event").value = tipo_event;
                document.getElementById("Button_me_active_men_dive").click();
            }
           
        }

        event.preventDefault();
    }
    catch (ex) {
        alert("Inconsistencia general function activa_menu_general_diference " + ex.message)
    }
}
function activa_boton_client_server(nombre_boton) {
    try {
        if (nombre_boton !== "" && document.getElementById(nombre_boton)) {
            document.getElementById(nombre_boton).click();
        }

        event.preventDefault();
    }
    catch (ex) {
        alert("Inconsistencia general function activa_boton_client_server " + ex.message)
    }
}
function dowload_file(filename, name_file) {
    try {
        var element = document.createElement('a');
        element.setAttribute('href', filename);
        element.setAttribute('download', name_file);
        element.style.display = 'none';
        document.body.appendChild(element);
        element.click();
        document.body.removeChild(element);
    } catch (err) {
        alert(err.mensaje + " funcion dowload_file");
    }
}
var IFRAME_CONTAINER = "";
function load_iframe() {
    document.getElementById("container_loading_iframe").style.display = 'none';
    document.getElementById(IFRAME_CONTAINER).style.opacity = 1;
}
function loading_iframe(name_iframe, url_src) {
    IFRAME_CONTAINER = name_iframe;
    document.getElementById(IFRAME_CONTAINER).addEventListener("load", load_iframe);
    document.getElementById("container_loading_iframe").style.display = 'flex';
    document.getElementById(IFRAME_CONTAINER).src = url_src;
    document.getElementById(IFRAME_CONTAINER).style.opacity = 0;
}

