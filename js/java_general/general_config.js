function ini_config_control() {
    add_event_element_booton("bt_sys_event_element");
    add_event_element_option("bt_sys_event_element_option");
}
//-----------------------------------------------
//zona eventos  onchange
//-----------------------------------------------
function add_event_element_option(class_element) {
    //---Registra evento onchange de option
    try {
        var elment = document.getElementsByClassName(class_element);
        if (elment) {
            for (var i = 0; i < elment.length; i++) {
                elment[i].addEventListener("change", event_onchange, false);
            }
        }
    } catch (ex) {
        alert("incosistencia funciton add_event_element_option " + ex.mensaje);
    }
}
function event_onchange(e) {
    //----evento changue
    try {
        let name_elemnt_event = e.currentTarget.id;
        let element_event = document.getElementById(name_elemnt_event);
        if (element_event == null) {
            alert("Imposible encontar el control (" + name_elemnt_event + ")");
            return true;
        }
        let name_evento = element_event.getAttribute("name_event");
        let value_evento = element_event.getAttribute("value_event");
        event_element_menu(name_evento, value_evento);
    } catch (ex) {
        alert("Inconsistencia event_onchange " + ex.mensaje);
    }
}
//-----------------------------------------------
//zona eventos  botoon
//-----------------------------------------------
function add_event_element_booton(class_element) {
    //---Registra evento boton
    try {
        var elment = document.getElementsByClassName(class_element);
        if (elment) {
            for (var i = 0; i < elment.length; i++) {
                elment[i].addEventListener("click", event_botom_click, false);
            }
        }
    } catch (ex) {
        alert("incosistencia funciton add_event_element_booton " + ex.mensaje);
    }
}
function event_botom_click(e) {
    //---Evento boton 
    try {
        let name_elemnt_event = e.currentTarget.id;
        let element_event = document.getElementById(name_elemnt_event);
        if (element_event == null) {
            alert("Imposible encontar el control (" + name_elemnt_event + ")");
            return true;
        }
        let name_evento = element_event.getAttribute("name_event");
        let value_evento = element_event.getAttribute("value_event");
        event_element_menu(name_evento, value_evento);
    } catch (ex) {
        alert('event_element_menu  ' + ex.message);
    }
}
let FiltroJosonString = new Array();
const FiltroJonsonStrinstringify = (ContentJonson) => {
    try {
        FiltroJosonString = new Array();
        FiltroJosonString.push({ Error: "YES", ContentJonson: "" });
        if (ContentJonson == "") {
            FiltroJosonString[0].ContentJonson = ContentJonson;
            return FiltroJosonString;
        }
        FiltroJosonString[0].ContentJonson = ContentJonson;
        FiltroJosonString[0].ContentJonson = FiltroJosonString[0].ContentJonson.replace(/"/g, "");
        FiltroJosonString[0].ContentJonson = FiltroJosonString[0].ContentJonson.replace(/'/g, "");
        FiltroJosonString[0].ContentJonson = FiltroJosonString[0].ContentJonson.replace(/;/g, "");
        return FiltroJosonString;
        } catch (ex) {
        FiltroJosonString[0].Error = "Inconsistencia general funcion FiltroJonsonStrinstringify";
        return FiltroJosonString
        }
}
const MostrarLoadingSpiner = () => {
    try {
        const loading = document.createElement("div");
        loading.id = "loadingSpinner";
        loading.style.position = "fixed";
        loading.style.top = "0";
        loading.style.left = "0";
        loading.style.width = "100%";
        loading.style.height = "100%";
        loading.style.backgroundColor = "rgba(255, 255, 255, 0.8)";
        loading.style.display = "flex";
        loading.style.justifyContent = "center";
        loading.style.alignItems = "center";
        loading.style.zIndex = "110061";
        const spinner = document.createElement("div");
        spinner.className = "spinner-file";
        loading.appendChild(spinner);
        document.body.appendChild(loading);
        return "YES";
    } catch (ex) {
        return "Inconsistencia general funcion _MostrarLoading " + ex.mensaje;
    }
}

const OcultarLoadingSpiner = () => {
    const loading = document.getElementById("loadingSpinner");
    if (loading) loading.remove();
}
class SpinnerManager {
    constructor() {
        this.localSpinners = new Map();
        this.globalSpinner = null;
        this.blockedElements = new Set();
    }

    showOnButton(elementId, type = "circle") {
        const el = document.getElementById(elementId);
        if (!el || this.localSpinners.has(elementId)) return;

        // Crear contenedor relativo externo si no existe
        let wrapper = el.parentElement;
        if (!wrapper || getComputedStyle(wrapper).position === "static") {
            wrapper = document.createElement("div");
            wrapper.style.position = "relative";
            wrapper.style.display = "inline-block";
            el.parentNode.insertBefore(wrapper, el);
            wrapper.appendChild(el);
        }

        // Crear overlay absoluto que cubre todo el botón
        const overlay = document.createElement("div");
        overlay.style.position = "absolute";
        overlay.style.top = "0";
        overlay.style.left = "0";
        overlay.style.width = "100%";
        overlay.style.height = "100%";
        overlay.style.backgroundColor = "rgba(255,255,255,0.5)";
        overlay.style.display = "flex";
        overlay.style.justifyContent = "center";
        overlay.style.alignItems = "center";
        overlay.style.zIndex = "1000";

        // Crear spinner
        const spinner = document.createElement("div");
        spinner.className = type === "file" ? "spinner-file" : "spinner-circle small";
        overlay.appendChild(spinner);

        wrapper.appendChild(overlay);
        this.localSpinners.set(elementId, { overlay, wrapper });

        // Bloquear el botón
        el.disabled = true;
        this.blockedElements.add(el);
    }

    hideOnButton(elementId) {
        const spinnerObj = this.localSpinners.get(elementId);
        if (!spinnerObj) return;
        const { overlay, wrapper } = spinnerObj;
        overlay.remove();

        const el = document.getElementById(elementId);
        if (el && this.blockedElements.has(el)) {
            el.disabled = false;
            this.blockedElements.delete(el);
        }

        // Si envolvimos el botón, no rompemos la estructura
        if (wrapper && wrapper.childNodes.length === 1 && wrapper.firstChild === el) {
            wrapper.replaceWith(el);
        }

        this.localSpinners.delete(elementId);
    }

    hideAllButtons() {
        for (const id of Array.from(this.localSpinners.keys())) {
            this.hideOnButton(id);
        }
    }

    showGlobal(type = "file") {
        if (this.globalSpinner) return;
        const loading = document.createElement("div");
        loading.id = "loadingSpinner";
        loading.style.position = "fixed";
        loading.style.top = "0";
        loading.style.left = "0";
        loading.style.width = "100%";
        loading.style.height = "100%";
        loading.style.backgroundColor = "rgba(255,255,255,0.8)";
        loading.style.display = "flex";
        loading.style.justifyContent = "center";
        loading.style.alignItems = "center";
        loading.style.zIndex = "110061";

        const spinner = document.createElement("div");
        spinner.className = type === "file" ? "spinner-file" : "spinner-circle";
        loading.appendChild(spinner);
        document.body.appendChild(loading);

        this.globalSpinner = loading;
    }

    hideGlobal() {
        if (this.globalSpinner) {
            this.globalSpinner.remove();
            this.globalSpinner = null;
        }
    }
    // Función para crear y mostrar el spinner global
 
    // Función para crear y mostrar el spinner global
 mostrarProgresBar() {
    // Crear el contenedor del progreso solo si no existe
    let progresBar = document.getElementById("progres_bar_general");
    if (!progresBar) {
        // Crear el div que contendrá el spinner y el mensaje de "Processing ..."
        progresBar = document.createElement("div");
        progresBar.id = "progres_bar_general";
        progresBar.style.position = "fixed";
        progresBar.style.textAlign = "center";
        progresBar.style.display = "none";  // Inicialmente oculto
        progresBar.style.width = "200px";  // Ajustar el tamaño del spinner
        progresBar.style.zIndex = "1000009";  // Asegurar que el spinner esté por encima de otros elementos

        // Crear la imagen del spinner
        /*const spinnerImg = document.createElement("img");
        spinnerImg.src = "../workflow/loading.gif";  // Ruta del spinner
        spinnerImg.style.verticalAlign = "middle";
        spinnerImg.alt = "Processing";*/
        const spinnerContainer = document.createElement("div");
        spinnerContainer.className = "spinner-container"; // Spinner hecho en CSS

        // Crear el spinner
        const spinner = document.createElement("div");
        spinner.className = "spinner-circle-general";

        // Crear el texto
        const spinnerText = document.createElement("div");
        spinnerText.className = "spinner-text";
        spinnerText.innerText = "Procesando..."; // El texto fijo

        // Añadir el spinner y el texto al contenedor
        spinnerContainer.appendChild(spinner);
        spinnerContainer.appendChild(spinnerText);
        //loading.appendChild(spinner);
        // Agregar la imagen del spinner al contenedor
        progresBar.appendChild(spinnerContainer);

        // Crear el texto de "Processing ..."
        const processingText = document.createElement("div");
        //processingText.innerText = "Processing ...";
        progresBar.appendChild(processingText);

        // Agregar el contenedor del progreso al body del documento
        document.body.appendChild(progresBar);
    }

    // Ahora mostrar el spinner
    progresBar.style.display = "block";  // Mostrar el spinner

    // Llamar a la función para centrar el spinner en la pantalla
    this.posicionUpdateProgres();
}

// Función para ocultar el progreso (spinner)
 ocultarProgresBar() {
     const progresBar = document.getElementById("progres_bar_general");
    if (progresBar) {
        progresBar.style.display = "none";  // Ocultar el spinner
    }
}

// Función para actualizar la posición del spinner en el centro de la pantalla
 posicionUpdateProgres() {
     const progresBar = document.getElementById("progres_bar_general");
    if (!progresBar) return;  // Si no existe el spinner, salir

    // Calcular las dimensiones de la ventana
    let espacioIframe = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
    let withFrame = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;

    // Calcular la posición central
    const widtop = (espacioIframe - progresBar.offsetHeight) / 2;
    const heitop = (withFrame - progresBar.offsetWidth) / 2;

    // Establecer la posición del spinner
    progresBar.style.top = `${widtop}px`;
    progresBar.style.left = `${heitop}px`;
    progresBar.style.position = "fixed";
    progresBar.style.zIndex = "1000009";  // Asegurar que el spinner esté encima de otros elementos
}

}

// Función para abrir y posicionar el modal
const openAndPositionModal = async (modalId, referenceElementId) => {
    // Obtener el modal y el elemento de referencia
    var modal = document.getElementById(modalId);
    var referenceElement = document.getElementById(referenceElementId);

    // Verificar si ambos elementos existen
    if (modal && referenceElement) {
        // Posicionar el modal según la posición del elemento de referencia
        var rect = referenceElement.getBoundingClientRect();
        var topPosition = rect.bottom + window.scrollY; // Sumar el desplazamiento de la página

        // Posicionar el modal en el top deseado
        var modalDialog = modal.querySelector('.modal-dialog');
        //modalDialog.style.position = 'absolute';
        modalDialog.style.top = topPosition + 'px';
        modalDialog.style.left = '';  // Asegura que no se modifique el valor de 'left'
        // Abrir el modal
        var bootstrapModal = new bootstrap.Modal(modal);
        bootstrapModal.show();
        return "YES";
    } else {
        console.error('El modal o el elemento de referencia no se encuentran.');
        return "El modal o el elemento de referencia no se encuentran.";
    }
}









