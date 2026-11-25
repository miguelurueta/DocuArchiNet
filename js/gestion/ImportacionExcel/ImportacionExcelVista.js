(function (root, factory) {
    "use strict";
    if (typeof module === "object" && module.exports) { module.exports = factory; }
    else { root.ImportacionExcelVista = factory(root); root.ImportacionExcelVista.inicializar(); }
}(typeof window !== "undefined" ? window : this, function (env) {
    "use strict";
    var current = null, registeredManager = null;
    var uuid = /^[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}$/;
    var hash = /^[a-f0-9]{64}$/;
    var messages = {
        SESION_REQUERIDA:"La sesión terminó. Vuelve a acceder para consultar tu carga.",
        ACCESO_DENEGADO:"No tienes permiso vigente para esta operación.",
        SOLICITUD_NO_VERIFICADA:"Recarga la página para renovar el acceso a esta herramienta.",
        CONFLICTO_REVISION:"La plantilla cambió. Consulta su versión actual antes de continuar.",
        CONFLICTO_PREPARACION:"La validación cambió. Consulta la carga antes de prepararla de nuevo.",
        CONFLICTO_CONFIRMACION:"La carga no corresponde a la validación seleccionada. Consulta su resultado.",
        CONFLICTO_CLAVE:"La carga encontró valores duplicados. No se confirmó ninguna de sus filas.",
        CLAVES_INVALIDAS:"Hay claves duplicadas o incompatibles. Revisa las incidencias.",
        LECTURA_INVALIDA:"El libro tiene incidencias. Revisa los datos antes de continuar.",
        LIMITE_ARCHIVO_EXCEDIDO:"El archivo supera el tamaño permitido.",
        ARCHIVO_INVALIDO:"El archivo no es un libro válido para esta plantilla.",
        HOJA_NO_ENCONTRADA:"La hoja seleccionada no está disponible en el libro.",
        CAPACIDAD_NO_DISPONIBLE:"La carga no está disponible. Puedes intentar consultar una referencia conocida.",
        OPERACION_EN_CURSO:"La carga está siendo procesada. Consulta su resultado antes de repetir.",
        RESULTADO_INCIERTO:"No se conoce el resultado. Consulta el mismo ID antes de repetir.",
        DESTINO_YA_EXISTE:"La tabla ya existe. Inicia una nueva carga para actualizar esta plantilla o cambia su operación a agregar o reemplazar.",
        CONFIGURACION_INVALIDA:"Revisa la configuración de la plantilla y sus campos.",
        DESTINO_INVALIDO:"El destino no es compatible con esta plantilla. Revisa su configuración.",
        SOLICITUD_INVALIDA:"Revisa los campos requeridos antes de continuar."
    };
    Object.assign(messages, {ENCABEZADOS_INVALIDOS:"La fila 1 debe contener encabezados no vacíos, de hasta 120 caracteres y como máximo 256 columnas. Revisa también las columnas con datos sin encabezado.",ENCABEZADO_AMBIGUO:"Hay encabezados repetidos. Corrígelos en el Excel antes de generar los campos.",LIBRO_SIN_DATOS:"La hoja no contiene filas de datos.",LIMITE_FILAS_EXCEDIDO:"La hoja supera el límite de filas permitido.",LIBRO_INVALIDO:"El archivo no es un libro Excel válido.",FORMULA_SIN_RESULTADO:"Hay fórmulas sin resultado guardado. Calcula y guarda el libro desde Excel.",ERROR_CELDA_EXCEL:"El libro contiene errores de celda. Corrígelos antes de continuar.",LONGITUD_NO_ADMITIDA:"Una columna requiere un tipo de texto mayor que el admitido para crear tablas. Revisa sus valores.",ANALISIS_NO_DISPONIBLE:"No se pudo analizar el libro. Puedes corregirlo o configurar la plantilla manualmente.",ARCHIVO_ALTERADO:"El archivo recibido es distinto del analizado. Inicia una nueva carga y revisa el libro."});
    var modeNames = {CREAR:"Crear tabla", AGREGAR:"Agregar filas", REEMPLAZAR:"Reemplazar contenido autorizado"};
    messages.FECHA_CON_HORA_NO_ADMITIDA = "Hay fechas con hora. Revisa cómo quieres conservarlas antes de configurar la plantilla manualmente; el asistente propone sólo fechas sin hora.";
    function integer(v) { return typeof v === "number" && isFinite(v) && Math.floor(v) === v && v > 0 && v <= 4294967295; }
    function text(v) { return v === null || v === undefined ? "" : String(v); }
    function copy(v) { return JSON.parse(JSON.stringify(v)); }
    function value(v) {
        if (v.EsNulo) { return "NULL"; }
        if (v.Tipo === "DECIMAL" || v.Tipo === "ENTERO") { return text(v.NumeroCanonico); }
        if (v.Tipo === "DATE") {
            var source = text(v.Fecha), match = /^\/Date\((-?\d+)(?:[+-]\d{4})?\)\/$/.exec(source), date;
            if (match) { date = new Date(Number(match[1])); return isNaN(date.getTime()) ? "Fecha no disponible" : date.toISOString().slice(0,10); }
            return /^\d{4}-\d{2}-\d{2}(?:T.*)?$/.test(source) ? source.slice(0,10) : "Fecha no disponible";
        }
        return text(v.Texto !== undefined ? v.Texto : v.NumeroCanonico);
    }
    function mount(host) {
        if (host.doc126Instancia) { return host.doc126Instancia; }
        var doc = host.ownerDocument, scope = host.getAttribute("data-recuperacion");
        if (!/^[a-f0-9]{32}$/.test(scope || "") || !env.ImportacionExcelApi || !env.ImportacionCarpetas) { return null; }
        var api = env.ImportacionExcelApi.crear({url:host.getAttribute("data-api"), upload:host.getAttribute("data-upload"),analisis:host.getAttribute("data-analisis"),recuperacion:scope});
        var epoch = 0, busy = false, writeBusy = false, destroyed = false, closed = true, opener = null;
        var capability = null, template = null, received = null, prepared = null, frozen = null, preparationHash = "", uncertain = false, blocked = false;
        var pageNumber = 1, listTotal = 0, editorOriginal = null, editorDirty = false, saveUncertain = false, refs = [], storage = null, storageOk = true;
        var namespace = "doc126-excel:" + scope, state = "VACIO", controls = [];
        var analysis = null, analyzedFile = null, pendingFile = null, pendingHash = "", pendingTemplate = 0, suggestedTable = "", flowOutcome = null, guideAction = null;
        function node(tag, content, attrs) {
            var e = doc.createElement(tag);
            if (content !== null && content !== undefined) { e.textContent = content; }
            Object.keys(attrs || {}).forEach(function (k) { e.setAttribute(k,attrs[k]); });
            return e;
        }
        function button(label, key, action, primary) {
            var b = node("button",label,{type:"button", "data-doc126":key, "class":"doc126-button" + (primary ? " doc126-primary" : "")});
            b.addEventListener("click",action); controls.push(b); return b;
        }
        function field(parent, label, tag, key, attrs) {
            var box = node("div",null,{"class":"doc126-field"}), id = "doc126-" + key;
            var input = node(tag || "input",null,Object.assign({id:id,"data-doc126":key},attrs || {}));
            box.appendChild(node("label",label,{"for":id})); box.appendChild(input); parent.appendChild(box); controls.push(input); return input;
        }
        function option(select, label, id) { select.appendChild(node("option",label,{value:String(id)})); }
        function clear(element) { while (element.firstChild) { element.removeChild(element.firstChild); } }
        function notice(message) { status.textContent = message || ""; }
        function invalid(ambiguous) { return {codigo:"RESPUESTA_INVALIDA",resultadoIncierto:!!ambiguous}; }
        function live() { return !destroyed && host.isConnected !== false; }
        function storeRefs() {
            try { if (!storage) { throw new Error(); } storage.setItem(namespace,JSON.stringify(refs)); }
            catch (_) { storageOk = false; }
            storageNotice.hidden = storageOk;
        }
        try {
            storage = env.sessionStorage;
            var saved = JSON.parse(storage.getItem(namespace) || "[]");
            if (Array.isArray(saved)) { refs = saved.filter(function (r) { return r && uuid.test(r.IdCarga) && integer(r.ConexionId) && Object.keys(r).length === 2; }).slice(-20); }
        } catch (_) { storageOk = false; }
        function remember(id, connection) {
            if (!uuid.test(id || "") || !integer(connection)) { throw invalid(true); }
            refs = refs.filter(function (r) { return r.IdCarga !== id; }); refs.push({IdCarga:id,ConexionId:connection}); refs = refs.slice(-20);
            storeRefs(); history(); queryId.value = id; queryConnection.value = String(connection); idNotice.textContent = "ID de carga: " + id;
        }
        var access = node("button","Cargar datos desde Excel…",{type:"button","class":"dropdown-item","data-doc126":"abrir"});
        var entryNotice = node("p","",{role:"status","class":"doc126-entry-notice"});
        var section = node("div",null,{"data-doc126-menu":"1"}); section.appendChild(access);
        var overlay = node("div",null,{"class":"doc126-overlay",hidden:"hidden"});
        var dialog = node("section",null,{role:"dialog","aria-modal":"true","aria-labelledby":"doc126-title","aria-describedby":"doc126-intro","class":"doc126-dialog",tabindex:"-1"});
        var header = node("header",null,{"class":"doc126-header"}), heading = node("div");
        heading.appendChild(node("span","PRODUCCIÓN DOCUMENTAL",{"class":"doc126-eyebrow"}));
        heading.appendChild(node("h2","Cargar datos desde Excel",{id:"doc126-title"}));
        heading.appendChild(node("p","Selecciona tu plantilla y revisa el archivo antes de confirmar.",{id:"doc126-intro"})); header.appendChild(heading);
        var closeButton = button("Cerrar","cerrar",close); header.appendChild(closeButton); dialog.appendChild(header);
        var status = node("p","",{id:"doc126-status-message",role:"status","aria-live":"polite",tabindex:"0","class":"doc126-status","data-doc126":"estado"}); dialog.appendChild(status);
        var body = node("div",null,{"class":"doc126-body"}), tabs = node("nav",null,{"aria-label":"Opciones de carga","class":"doc126-tabs"});
        var loadPanel = node("section",null,{"data-doc126":"carga"}), editPanel = node("section",null,{"data-doc126":"editor",hidden:"hidden"}), queryPanel = node("section",null,{"data-doc126":"consulta",hidden:"hidden"});
        var panels = [loadPanel,editPanel,queryPanel];
        tabs.appendChild(button("Cargar archivo","tab-carga",function () { showPanel(loadPanel); }));
        tabs.appendChild(button("Plantillas","tab-plantillas",function () { if (flowOutcome && flowOutcome.Estado === "CONFIRMADA" && template) { changeTemplateOperation(); } else { editTemplate(false); } }));
        tabs.appendChild(button("Consultar resultado","tab-consulta",function () { showPanel(queryPanel); })); body.appendChild(tabs);
        var guide = node("section",null,{"class":"doc126-guide","aria-label":"Guía de importación","data-doc126":"guia"});
        guide.appendChild(node("h3","Tu importación, paso a paso"));
        guide.appendChild(node("p","Verde: completado · Amarillo: siguiente paso · Rojo: requiere atención · Gris: pendiente.",{"class":"doc126-limits"}));
        var steps = node("ol",null,{"class":"doc126-steps"}), stepLabels = ["Plantilla","Recibir archivo","Validar archivo","Confirmar importación"];
        var stepNodes = stepLabels.map(function (label,i) {
            var item = node("li",null,{"data-doc126-step":String(i + 1)});
            item.appendChild(node("strong",String(i + 1) + ". " + label));
            var detail = node("span","Pendiente"); item.appendChild(detail); steps.appendChild(item); return {item:item,detail:detail};
        }); guide.appendChild(steps);
        var guideMessage = node("p","",{"data-doc126":"guia-mensaje"}); guide.appendChild(guideMessage);
        var guideButton = button("Seleccionar plantilla","siguiente-paso",function () { if (!busy && guideAction) { guideAction(); } },true); guide.appendChild(guideButton); body.appendChild(guide);
        var diagnostics = node("section",null,{"data-doc126":"diagnostico-configuracion","class":"doc126-diagnostics",hidden:"hidden",tabindex:"-1","aria-label":"Detalle del error de configuración"}); body.appendChild(diagnostics);
        var selection = node("div",null,{"class":"doc126-grid"});
        var connection = field(selection,"Conexión autorizada","select","conexion");
        var search = field(selection,"Buscar plantilla","input","buscar",{type:"search",maxlength:"120"});
        var selectTemplate = field(selection,"Plantilla","select","plantilla");
        var searchButton = button("Buscar","buscar-plantillas",function () { pageNumber = 1; fetchList(); }); selection.appendChild(searchButton);
        var listActions = node("div",null,{"class":"doc126-actions"});
        var previous = button("Anterior","anterior",function () { if (pageNumber > 1) { pageNumber--; fetchList(); } });
        var next = button("Siguiente","siguiente",function () { pageNumber++; fetchList(); });
        var listPage = node("span","",{"data-doc126":"pagina"}); listActions.appendChild(previous); listActions.appendChild(listPage); listActions.appendChild(next);
        loadPanel.appendChild(selection); loadPanel.appendChild(listActions);
        var summary = node("p","Selecciona una plantilla para conocer el destino y la clave.",{"class":"doc126-summary","data-doc126":"destino"}); loadPanel.appendChild(summary);
        var operationHelp = node("p","",{"data-doc126":"operacion-ayuda"}); loadPanel.appendChild(operationHelp);
        var changeOperation = button("Cambiar operación de esta plantilla","cambiar-operacion",changeTemplateOperation); loadPanel.appendChild(changeOperation);
        var limits = node("p","",{"class":"doc126-limits","data-doc126":"limites"}); loadPanel.appendChild(limits);
        var fileGrid = node("div",null,{"class":"doc126-grid"});
        var file = field(fileGrid,"Libro Excel","input","archivo",{type:"file",accept:".xls,.xlsx"});
        var sheet = field(fileGrid,"Hoja del libro","select","hoja");
        var mode = field(fileGrid,"Modo de la plantilla","select","modo");
        loadPanel.appendChild(fileGrid);
        var loadActions = node("div",null,{"class":"doc126-actions"});
        var createFromExcel = button("Crear plantilla desde Excel","crear-desde-excel",function () { editTemplate(true); if (!editPanel.hidden) { analysisFile.focus(); } });
        createFromExcel.hidden = !host.getAttribute("data-analisis"); loadActions.appendChild(createFromExcel);
        var uploadButton = button("Recibir archivo","recibir",upload), prepareButton = button("Validar archivo","preparar",prepare);
        loadActions.appendChild(uploadButton); loadActions.appendChild(prepareButton); loadActions.appendChild(button("Nueva carga","nueva-carga",newLoad)); loadPanel.appendChild(loadActions);
        var preview = node("div",null,{"data-doc126":"preview"}), incidents = node("div",null,{"data-doc126":"incidencias"}); loadPanel.appendChild(preview); loadPanel.appendChild(incidents);
        var consentBox = node("div",null,{"class":"doc126-consent",hidden:"hidden"});
        var consent = field(consentBox,"Acepto reemplazar el contenido autorizado del destino mostrado.","input","consentimiento",{type:"checkbox"});
        var consentText = node("p","",{"data-doc126":"reemplazo-resumen"}); consentBox.appendChild(consentText); loadPanel.appendChild(consentBox);
        var confirmButton = button("Confirmar carga","confirmar",confirmLoad,true); loadPanel.appendChild(confirmButton);
        var editorGrid = node("div",null,{"class":"doc126-grid"});
        var editor = {};
        [["NOMBRE_PLANTILLA","Tabla de destino","tabla",64],["CAMPO_UNICO","Campo único","clave",64],["LIBRO_EXCEL","Hoja predeterminada","libro",120]].forEach(function (r) { editor[r[0]] = field(editorGrid,r[1],"input",r[2],{type:"text",maxlength:String(r[3]),required:"required"}); });
        editor.Formato = field(editorGrid,"Formato","select","formato"); ["XLSX","XLS"].forEach(function (v) { option(editor.Formato,v,v); });
        editor.Modo = field(editorGrid,"Modo","select","editor-modo"); Object.keys(modeNames).forEach(function (v) { option(editor.Modo,modeNames[v],v); });
        editor.ESTADO_PLANTILLA = field(editorGrid,"Plantilla activa","input","activa",{type:"checkbox"});
        editPanel.appendChild(node("h3","Configuración de plantilla")); editPanel.appendChild(editorGrid);
        editPanel.appendChild(node("p","Puedes cambiar la operación y guardar esta misma plantilla: Crear tabla para un destino nuevo; Agregar filas para conservar las existentes; Reemplazar contenido para sustituirlo con confirmación expresa. Reutiliza la plantilla cuando el destino y los campos sean los mismos. Una tabla o estructura diferente requiere otra configuración."));
        var assistant = node("section",null,{"data-doc126":"asistente","class":"doc126-assistant",hidden:"hidden"});
        assistant.appendChild(node("h3","Crear campos desde Excel"));
        assistant.appendChild(node("p","Encabezados en la fila 1. Los campos y tipos son propuestas editables; elige la clave y revisa la configuración antes de guardar."));
        var analysisGrid = node("div",null,{"class":"doc126-grid"});
        var analysisFile = field(analysisGrid,"Excel para generar los campos","input","analisis-archivo",{type:"file",accept:".xls,.xlsx"});
        var analysisSheet = field(analysisGrid,"Hoja para generar los campos","select","analisis-hoja");
        var analysisKey = field(analysisGrid,"Elegir campo único de la propuesta","select","analisis-clave");
        assistant.appendChild(analysisGrid);
        var analyzeButton = button("Analizar Excel y proponer campos","analizar",analyze,true); assistant.appendChild(analyzeButton);
        var analysisPreview = node("div",null,{"data-doc126":"analisis-preview"}); assistant.appendChild(analysisPreview); editPanel.appendChild(assistant);
        var pendingNotice = node("p","",{"data-doc126":"archivo-conservado","class":"doc126-limits"}); loadPanel.appendChild(pendingNotice);
        var fieldsWrap = node("div",null,{"class":"doc126-table-scroll"}), fieldsTable = node("table",null,{"class":"doc126-table"});
        fieldsTable.appendChild(node("caption","Campos y transformaciones de la plantilla"));
        var fieldsHeader = node("tr"); ["Origen","Destino","Tipo","Cambiar coma","Cambiar punto","Separar","Separador","Posición","Acción"].forEach(function (v) { fieldsHeader.appendChild(node("th",v,{scope:"col"})); });
        var thead = node("thead"); thead.appendChild(fieldsHeader); fieldsTable.appendChild(thead);
        var fieldsBody = node("tbody",null,{"data-doc126":"campos"}); fieldsTable.appendChild(fieldsBody); fieldsWrap.appendChild(fieldsTable); editPanel.appendChild(fieldsWrap);
        var editActions = node("div",null,{"class":"doc126-actions"});
        editActions.appendChild(button("Añadir campo","agregar-campo",function () { if (fieldsBody.children.length < 256) { addField({}); editorDirty = true; invalidate(false); } }));
        var saveButton = button("Guardar plantilla","guardar",saveTemplate,true); editActions.appendChild(saveButton);
        editActions.appendChild(button("Nueva plantilla","nueva-plantilla",function () { editTemplate(true); })); editPanel.appendChild(editActions);
        editPanel.appendChild(node("p","Guardar una plantilla no carga datos ni publica una fuente de índices.",{"class":"doc126-limits"}));
        queryPanel.appendChild(node("h3","Recuperar el resultado de una carga"));
        var queryGrid = node("div",null,{"class":"doc126-grid"});
        var recent = field(queryGrid,"Cargas recientes de esta sesión","select","recientes");
        var queryConnection = field(queryGrid,"Conexión de la carga","select","consulta-conexion");
        var queryId = field(queryGrid,"ID de carga","input","id-carga",{type:"text",maxlength:"36",autocomplete:"off",spellcheck:"false"});
        queryPanel.appendChild(queryGrid); var queryButton = button("Consultar resultado","consultar",queryResult,true); queryPanel.appendChild(queryButton);
        var result = node("div",null,{"data-doc126":"resultado"}); queryPanel.appendChild(result);
        panels.forEach(function (p) { body.appendChild(p); }); dialog.appendChild(body);
        var footer = node("footer",null,{"class":"doc126-footer",tabindex:"0"}), idNotice = node("p","Sin carga iniciada.",{"data-doc126":"referencia"});
        var storageNotice = node("p","El navegador no puede conservar referencias. Anota el ID antes de recargar.",{role:"status","data-doc126":"storage-aviso"});
        storageNotice.hidden = storageOk; footer.appendChild(idNotice); footer.appendChild(storageNotice); dialog.appendChild(footer); overlay.appendChild(dialog); host.appendChild(entryNotice); host.appendChild(overlay);
        function fitViewport() {
            var zoom = Number(env.getComputedStyle(doc.documentElement).zoom) || 1, viewport = env.visualViewport;
            var width = (viewport ? viewport.width : env.innerWidth) / zoom, height = (viewport ? viewport.height : env.innerHeight) / zoom;
            overlay.style.width = width + "px"; overlay.style.height = height + "px";
            dialog.style.maxHeight = Math.max(80,height - 24) + "px";
            dialog.setAttribute("data-compact",String(width < 420 || height < 400));
            status.style.maxHeight = Math.max(28,height * 0.2) + "px"; footer.style.maxHeight = Math.max(28,height * 0.2) + "px";
        }
        env.addEventListener("resize",fitViewport);
        if (env.visualViewport) { env.visualViewport.addEventListener("resize",fitViewport); }
        var viewportObserver = env.ResizeObserver ? new env.ResizeObserver(fitViewport) : null;
        if (viewportObserver) { viewportObserver.observe(doc.documentElement); }
        var zoomObserver = env.MutationObserver ? new env.MutationObserver(fitViewport) : null;
        if (zoomObserver) { zoomObserver.observe(doc.documentElement,{attributes:true,attributeFilter:["style"]}); }
        function attachMenu() {
            var menu = env.ImportacionCarpetas.asegurarMenu();
            if (menu && !menu.contains(section)) { menu.appendChild(section); }
            env.ImportacionCarpetas.rotularAccesoMenu(access,"Cargar datos desde Excel…","fa-file-excel");
        }
        function history() {
            clear(recent); option(recent,"Selecciona una referencia","");
            refs.slice().reverse().forEach(function (r) { option(recent,r.IdCarga,r.IdCarga); });
        }
        function allowed() { return capability && capability.PermiteNuevaCarga !== false && !blocked; }
        function loadAllowed() { return allowed() && template && template.ESTADO_PLANTILLA === 1 && /^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(template.CAMPO_UNICO || ""); }
        function confirmable() { return prepared && frozen && !uncertain && state === "PREPARADO" && (!consentBox.hidden ? consent.checked : true); }
        function syncGuide() {
            guide.hidden = !editPanel.hidden;
            var complete = !!(received && flowOutcome && flowOutcome.IdCarga === received.IdCarga && flowOutcome.Estado === "CONFIRMADA");
            var validated = !!(prepared && frozen), ready = !!loadAllowed(), active = !ready ? 0 : !received ? 1 : !validated && !complete ? 2 : 3;
            var done = [ready,!!received,validated || complete,complete];
            var attention = blocked || (capability && !allowed()) || uncertain || saveUncertain || state === "INCIDENCIAS" || (flowOutcome && flowOutcome.Estado === "FALLIDA") || (template && !ready);
            stepNodes.forEach(function (step,i) {
                var tone = done[i] ? "green" : i === active ? "amber" : "gray";
                if (attention && i === active) { tone = "red"; }
                step.item.setAttribute("data-tone",tone);
                step.detail.textContent = tone === "green" ? "Completado" : tone === "red" ? "Requiere atención" : tone === "amber" ? (busy ? "En curso" : "Siguiente paso") : "Pendiente";
                if (!complete && i === active) { step.item.setAttribute("aria-current","step"); } else { step.item.removeAttribute("aria-current"); }
            });
            var label, message, enabled = true;
            if (complete) {
                label = "Nueva importación con esta plantilla"; message = "Importación confirmada por el servidor. Puedes reutilizar esta plantilla; consultaremos su operación vigente antes de recibir otro archivo."; guideAction = newLoad;
            } else if (!ready) {
                label = template ? "Revisar plantilla" : "Seleccionar plantilla"; message = "Selecciona una plantilla existente o usa Crear plantilla desde Excel si el destino y los campos son nuevos.";
                guideAction = function () { if (template) { editTemplate(false); } else if (showPanel(loadPanel)) { selectTemplate.focus(); } };
            } else if (!received) {
                var hasFile = !!pendingFile || !!(file.files && file.files.length === 1);
                label = hasFile ? "Recibir archivo" : "Seleccionar archivo Excel";
                message = "Operación: " + modeNames[template.Modo] + ". Recibir el archivo permite elegir su hoja; todavía no importa datos.";
                guideAction = hasFile ? upload : function () { if (showPanel(loadPanel)) { file.focus(); file.click(); } };
                enabled = hasFile ? !uploadButton.disabled : !file.disabled;
            } else if (!validated) {
                label = "Validar archivo"; message = "Selecciona la hoja. La validación revisa todas las filas; confirma sólo después de revisar la muestra y las incidencias.";
                guideAction = function () { if (showPanel(loadPanel)) { prepare(); } }; enabled = !prepareButton.disabled;
            } else {
                label = !consentBox.hidden && !consent.checked ? "Revisar y aceptar reemplazo" : "Revisar y confirmar importación";
                message = "Archivo validado. Revisa la muestra y el destino antes de pulsar Confirmar carga. Esta acción escribe los datos.";
                guideAction = function () { if (showPanel(loadPanel)) { var target = !consentBox.hidden && !consent.checked ? consent : confirmButton; target.scrollIntoView({block:"center"}); target.focus(); } };
            }
            if ((state === "INCIDENCIAS" || (flowOutcome && flowOutcome.Estado === "FALLIDA")) && !uncertain && !saveUncertain) {
                message = "La operación requiere atención. Revisa el mensaje y las incidencias antes de continuar; no se acredita una importación confirmada.";
                label = "Revisar incidencias"; enabled = true;
                guideAction = function () {
                    if (incidents.childNodes.length && showPanel(loadPanel)) { incidents.setAttribute("tabindex","-1"); incidents.scrollIntoView({block:"center"}); incidents.focus(); }
                    else { status.focus(); }
                };
            }
            if (uncertain || saveUncertain) {
                label = saveUncertain ? "Consultar plantillas" : "Consultar resultado pendiente";
                message = saveUncertain ? "El guardado no está acreditado. Busca y consulta la plantilla antes de repetirlo." : "No se conoce el resultado. Consulta el mismo ID antes de iniciar otra importación.";
                guideAction = function () { if (showPanel(saveUncertain ? loadPanel : queryPanel)) { (saveUncertain ? searchButton : queryButton).focus(); } }; enabled = !blocked;
            }
            if (!allowed()) { message = blocked ? "El acceso no está vigente. Renueva la sesión para continuar." : "Las nuevas importaciones no están disponibles. Puedes consultar resultados anteriores."; enabled = false; }
            guideButton.textContent = label; guideMessage.textContent = busy ? "Operación en curso. Espera su resultado; cerrar la ventana no la cancela." : message;
            guideButton.disabled = busy || !enabled;
            [[uploadButton,1],[prepareButton,2],[confirmButton,3]].forEach(function (pair) {
                var tone = done[pair[1]] ? "green" : pair[1] === active ? (attention ? "red" : "amber") : "gray";
                pair[0].setAttribute("data-tone",tone);
            });
            operationHelp.textContent = template ? (template.Modo === "CREAR" ? "Crear tabla: la primera importación creará el destino. Después podrás agregar filas con esta misma plantilla." : template.Modo === "AGREGAR" ? "Agregar filas: conserva las filas existentes e incorpora las nuevas. Puedes cambiar a Reemplazar usando esta misma plantilla." : "Reemplazar contenido: sustituye las filas autorizadas del destino; requiere aceptar el reemplazo antes de confirmar.") : "La plantilla define el destino, los campos y la operación; se puede reutilizar para archivos con la misma estructura.";
        }
        function sync() {
            dialog.setAttribute("aria-busy",busy ? "true" : "false"); status.setAttribute("data-state",state);
            controls = controls.filter(function (c) { return c.isConnected !== false; });
            controls.forEach(function (c) { c.disabled = busy && c !== closeButton; }); closeButton.disabled = false;
            if (busy) { syncGuide(); return; }
            mode.disabled = true;
            uploadButton.disabled = !loadAllowed() || uncertain || saveUncertain || !!received || (!pendingFile && (!file.files || file.files.length !== 1));
            prepareButton.disabled = !loadAllowed() || !received || uncertain || !sheet.value;
            confirmButton.disabled = !loadAllowed() || !confirmable();
            previous.disabled = !allowed() || pageNumber <= 1; next.disabled = !allowed() || pageNumber * 25 >= listTotal;
            searchButton.disabled = !allowed(); selectTemplate.disabled = !allowed(); connection.disabled = !allowed(); file.disabled = !allowed() || uncertain;
            saveButton.disabled = !allowed() || saveUncertain || !editorOriginal;
            createFromExcel.disabled = !allowed() || saveUncertain || uncertain;
            changeOperation.disabled = !allowed() || !template || uncertain || saveUncertain;
            analyzeButton.disabled = !allowed() || saveUncertain || !analysisFile.files || analysisFile.files.length !== 1;
            analysisKey.disabled = !analysis;
            queryButton.disabled = !capability || blocked;
            if (flowOutcome && flowOutcome.Estado === "CONFIRMADA") { file.disabled = true; sheet.disabled = true; prepareButton.disabled = true; }
            syncGuide();
        }
        function invalidate(resetReceipt) {
            prepared = null; frozen = null; flowOutcome = null; consent.checked = false; consentBox.hidden = true; clear(preview); clear(incidents);
            if (resetReceipt) { received = null; preparationHash = ""; clear(sheet); }
            if (!uncertain) { state = "VACIO"; }
            sync();
        }
        function fail(error) {
            error = error || {};
            if (error.datos) { showPreparation(error.datos,false); }
            if (["SESION_REQUERIDA","ACCESO_DENEGADO","SOLICITUD_NO_VERIFICADA"].indexOf(error.codigo) !== -1) { blocked = true; uncertain = true; state = "CAPACIDAD_CERRADA"; }
            else if (error.resultadoIncierto || error.codigo === "RESULTADO_INCIERTO" || error.codigo === "OPERACION_EN_CURSO") {
                state = "RESULTADO_INCIERTO";
                if (saveUncertain) { notice("No se conoce si la plantilla se guardó. Consulta la lista y selecciona su detalle antes de decidir otra edición; no repitas el alta."); return; }
                uncertain = true;
            }
            else { state = "INCIDENCIAS"; }
            if (!editPanel.hidden && writeBusy && state === "INCIDENCIAS" && showConfigurationErrors(error)) { return; }
            notice((messages[error.codigo] || "No se pudo completar la operación. Revisa la selección o consulta su resultado.") + (uncertain ? " Conserva el ID de carga y consulta antes de repetir." : ""));
        }
        function clearConfigurationErrors() {
            clear(diagnostics); diagnostics.hidden = true;
            Array.prototype.forEach.call(editPanel.querySelectorAll('[aria-invalid="true"]'),function (input) { input.removeAttribute("aria-invalid"); input.removeAttribute("aria-describedby"); });
        }
        function configurationExplanation(d) {
            var toggles = ["REMPLAZA_COMA","REMPLAZA_PUNTO","OPCION_SPLITER"];
            if (d.Regla === "REQUERIDO" && toggles.indexOf(d.Campo) >= 0) { var effect = {REMPLAZA_COMA:"elimina las comas de cada valor",REMPLAZA_PUNTO:"elimina los puntos de cada valor",OPCION_SPLITER:"divide cada valor usando el separador y conserva el segmento indicado"}; return ["Se envió «Sin configurar» (sin valor), pero esta instalación exige una elección explícita.","Selecciona «No» para conservar los valores. «Sí» " + effect[d.Campo] + ". Revisa esta misma opción en los demás campos."]; }
            if (d.Regla === "REQUERIDO" && d.Campo === "NUMERO_MATRIZ_EXPLITER") { return ["La posición quedó sin valor. Esta instalación exige un número incluso cuando no se separan valores.","Escribe 0 si «Separar valor» está en «No». Si está en «Sí», indica el segmento: 0 es el primero, 1 el segundo, y así sucesivamente."]; }
            var rules = {
                REQUERIDO:["Falta un valor obligatorio en esta configuración.","Completa el campo indicado antes de volver a guardar."],
                LONGITUD_EXCEDIDA:["La configuración admite un máximo de " + (d.Limite === null ? "caracteres definido por la instalación" : d.Limite + " caracteres") + (d.LongitudActual === null ? "." : "; se enviaron " + d.LongitudActual + "."),"Revisa y ajusta el nombre o la configuración. Si necesitas conservar ese valor completo, solicita al administrador revisar el límite; no recortes datos del Excel automáticamente."],
                CARACTERES_INCOMPATIBLES:["Uno o más caracteres no pueden guardarse con la codificación de esta instalación.","Revisa el nombre o texto de configuración. Si debes conservar esos caracteres, solicita al administrador revisar la codificación."],
                CARACTERES_CONTROL:["El texto contiene un salto de línea u otro carácter de control no permitido.","Corrige el nombre o encabezado indicado y vuelve a revisar la plantilla."],
                NOMBRE_TABLA_INVALIDO:["El nombre de tabla está incompleto o no tiene el formato permitido.","Usa imp_02_ seguido de letras, números o guion bajo, hasta 64 caracteres. Ejemplo: imp_02_ordenes_repara."],
                IDENTIFICADOR_INVALIDO:["El nombre de columna de destino no es válido.","Empieza con una letra o guion bajo y usa sólo letras, números o guion bajo, hasta 64 caracteres. Mantén el encabezado original en Origen."],
                DESTINO_DUPLICADO:["Dos campos usan la misma columna de destino.","Cambia uno de los destinos o retira el campo duplicado. Cada destino debe aparecer una sola vez."],
                CLAVE_NO_MAPEADA:["El campo único no coincide con ninguna columna de destino de la plantilla.","Selecciona una columna existente en «Elegir campo único». Si cambiaste su nombre, vuelve a elegirla."],
                OPCION_INVALIDA:["La opción tiene un valor no admitido.","Selecciona una opción de la lista. Las transformaciones admiten «Sí» o «No»; la instalación determina si permite «Sin configurar»."],
                SEPARADOR_REQUERIDO:["Activaste «Separar valor» sin indicar un separador.","Escribe el separador que contiene el Excel, por ejemplo / o -, o selecciona «No» si no necesitas dividir el valor."],
                POSICION_REQUERIDA:["Activaste «Separar valor» sin elegir el segmento.","Indica un entero no negativo: 0 para el primer segmento, 1 para el segundo. Revisa también el separador."],
                TIPO_NO_ADMITIDO:["El tipo indicado no se admite para crear esta tabla.","Usa un tipo permitido, por ejemplo VARCHAR(120), CHAR(20), INT, BIGINT, DECIMAL(18,2) o DATE. VARCHAR admite hasta 4096 caracteres; DECIMAL hasta 65 dígitos totales y 30 decimales. No cambies un código con ceros iniciales a número sin revisarlo."],
                TIPO_DESTINO_INCOMPATIBLE:["El tipo configurado no coincide con el de la columna existente" + (d.TipoEsperado ? ": se espera " + d.TipoEsperado + "." : "."),"Ajusta Tipo al de la columna existente, o elige otra columna compatible. Este guardado no cambia la estructura de la tabla."],
                COLUMNA_NO_EXISTE:["La columna indicada no existe en la tabla elegida.","Corrige Destino para que coincida con una columna real, o selecciona una tabla compatible."],
                COLUMNA_AUTOMATICA:["La columna elegida se genera automáticamente y no admite este mapeo.","Usa otra columna de destino. No intentes escribir sobre una columna automática."],
                COLUMNA_OBLIGATORIA_NO_MAPEADA:["La tabla requiere una columna que la plantilla no está llenando.","Añade un campo que asigne un origen a esa columna o solicita al administrador revisar su configuración."],
                TABLA_NO_EXISTE:["La tabla elegida todavía no existe y el modo es agregar o reemplazar.","Selecciona una tabla existente. Si necesitas una tabla nueva, configura explícitamente el modo Crear tabla."],
                TABLA_YA_EXISTE:["El nombre elegido ya corresponde a una tabla y el modo es Crear tabla.","Elige otro nombre para crear una tabla nueva, o revisa la compatibilidad y selecciona Agregar filas o Reemplazar contenido. Reemplazar exige consentimiento al confirmar."],
                MOTOR_NO_ADMITIDO:["La tabla elegida no permite la transacción requerida por el cargador.","Selecciona una tabla InnoDB compatible o solicita al administrador revisar el destino."],
                CANTIDAD_CAMPOS:["La plantilla no contiene campos o supera el máximo de 256.","Añade al menos un campo y conserva como máximo 256 mapeos."]
            };
            return rules[d.Regla] || null;
        }
        function showConfigurationErrors(error) {
            if (["CONFIGURACION_INVALIDA","DESTINO_INVALIDO","DESTINO_YA_EXISTE"].indexOf(error.codigo) < 0) { return false; }
            var labels = {NOMBRE_PLANTILLA:"Tabla de destino",CAMPO_UNICO:"Campo único",LIBRO_EXCEL:"Hoja predeterminada",Formato:"Formato",Modo:"Modo",ESTADO_PLANTILLA:"Plantilla activa",Campos:"Campos de la plantilla",CAMPO_FUENTE:"Origen",CAMPO_DESTINO:"Destino",TIPO_CAMPO:"Tipo de dato",REMPLAZA_COMA:"Cambiar coma",REMPLAZA_PUNTO:"Cambiar punto",OPCION_SPLITER:"Separar valor",CARACTER_SPLITER:"Separador",NUMERO_MATRIZ_EXPLITER:"Posición",RUTA_ARCHIVO_FUENTE:"Referencia generada de la plantilla"};
            var details = (error.validacion || []).filter(function (d) { return Object.prototype.hasOwnProperty.call(labels,d.Campo) && configurationExplanation(d); });
            if (!details.length && error.codigo === "DESTINO_YA_EXISTE") { details = [{Campo:"NOMBRE_PLANTILLA",Regla:"TABLA_YA_EXISTE"}]; }
            if (!details.length && ["CONFIGURACION_INVALIDA","DESTINO_INVALIDO","DESTINO_YA_EXISTE"].indexOf(error.codigo) < 0) { return false; }
            clearConfigurationErrors(); diagnostics.hidden = false; diagnostics.appendChild(node("h3",details.length ? "No se pudo guardar la plantilla" : "El servidor no detalló el error de configuración"));
            if (details.length) { diagnostics.appendChild(node("p","No se guardaron los cambios de esta solicitud ni se importaron datos. Corrige la configuración y vuelve a pulsar Guardar plantilla.")); }
            if (!details.length) {
                diagnostics.appendChild(node("p","El servidor rechazó la configuración, pero su respuesta no identifica el campo ni la regla. No es posible afirmar una causa concreta con esa respuesta."));
                diagnostics.appendChild(node("p","El estado final del guardado no queda acreditado por esta respuesta. Consulta la lista y abre el detalle de la plantilla antes de reenviar para evitar un alta duplicada. Conservamos tu formulario."));
                diagnostics.appendChild(node("p","Revisa las opciones sin configurar, los nombres de origen y destino, los tipos y la clave. Si todo parece correcto, informa al administrador el código de validación para revisar la instalación; no cambies datos del Excel por suposición."));
                saveUncertain = true; state = "RESULTADO_INCIERTO";
            }
            details.forEach(function (d,index) {
                var explanation = configurationExplanation(d), title = labels[d.Campo], control = editor[d.Campo];
                if (d.FilaCampo) { var row = fieldsBody.children[d.FilaCampo - 1]; control = row && row.querySelector('[data-field="' + d.Campo + '"]'); title = "Campo " + d.FilaCampo + (d.CampoDestino ? " · " + d.CampoDestino : "") + " — " + title; }
                else if (d.CampoDestino) { title += " · " + d.CampoDestino; }
                var item = node("article",null,{"class":"doc126-diagnostic-item",id:"doc126-error-" + index}); item.appendChild(node("h4",title)); item.appendChild(node("p","Qué ocurre: " + explanation[0])); item.appendChild(node("p","Cómo corregirlo: " + explanation[1]));
                if (control) { control.setAttribute("aria-invalid","true"); control.setAttribute("aria-describedby",item.id); item.appendChild(button("Corregir " + labels[d.Campo],"corregir-configuracion",function () { control.scrollIntoView({block:"center",inline:"nearest"}); control.focus(); })); }
                diagnostics.appendChild(item);
            });
            if (error.totalValidacion > details.length) { diagnostics.appendChild(node("p","Se muestran " + details.length + " de " + error.totalValidacion + " observaciones. Revisa también las mismas opciones en los demás campos y vuelve a guardar para comprobarlas.")); }
            diagnostics.appendChild(node("p","Código de validación: " + (/^[A-Z_]{1,80}$/.test(error.codigo || "") ? error.codigo : "CONFIGURACION_INVALIDA")));
            notice(details.length ? "La plantilla no se guardó. " + details.length + " observaciones detalladas; revisa Qué ocurre y Cómo corregirlo." : "El guardado no está acreditado y el servidor no identificó el campo. Consulta la lista y el detalle antes de reenviar.");
            diagnostics.scrollIntoView({block:"start",inline:"nearest"}); diagnostics.focus(); return true;
        }
        function run(work, success, isWrite, phase, message) {
            if (busy || destroyed) { return Promise.resolve(); }
            var ticket = epoch, previousFocus = doc.activeElement;
            if (overlay.contains(previousFocus) && previousFocus !== closeButton) { dialog.focus(); }
            busy = true; writeBusy = !!isWrite; state = phase || "CARGANDO";
            notice(message || (phase === "CONFIRMANDO" ? "Confirmando la carga. Conserva el ID; cerrar la ventana no cancela la operación." : "Cargando información…")); sync();
            return Promise.resolve().then(work).then(function (r) {
                if (ticket === epoch && live() && !closed) { return success(r); }
            }).catch(function (error) { if (ticket === epoch && live() && !closed) { fail(error); } }).then(function () {
                if (ticket === epoch && !destroyed) {
                    busy = false; writeBusy = false; sync();
                    if (!closed && (doc.activeElement === dialog || doc.activeElement === doc.body)) {
                        var target = overlay.contains(previousFocus) && !previousFocus.disabled && previousFocus.getClientRects().length ? previousFocus : closeButton;
                        target.focus();
                    }
                }
            });
        }
        function showPanel(panel) {
            if (busy) { return false; }
            if (!editPanel.hidden && panel !== editPanel && editorDirty && !env.confirm("Hay cambios de plantilla sin guardar. ¿Quieres descartarlos?")) { return false; }
            if (panel !== editPanel) { editorDirty = false; }
            panels.forEach(function (p) { p.hidden = p !== panel; }); sync(); return true;
        }
        function open() {
            if (!closed) { dialog.focus(); return; }
            var other = Array.prototype.some.call(doc.querySelectorAll('[role="dialog"]'),function (d) { return d !== dialog && d.getClientRects().length && !d.hidden; });
            if (other) { entryNotice.textContent = "Cierra la ventana actual antes de abrir la carga Excel."; return; }
            opener = doc.activeElement; closed = false; overlay.hidden = false; entryNotice.textContent = ""; epoch++; busy = false;
            fitViewport(); closeButton.focus();
            run(function () { return api.ejecutar("AbrirCargaExcel"); },function (r) {
                var d = r.datos;
                if (!d || !Array.isArray(d.Conexiones) || !d.Conexiones.every(function (c) { return integer(c.ConexionId) && typeof c.Nombre === "string"; })) { throw invalid(false); }
                capability = d; blocked = false;
                var previousConnection = connection.value, previousQuery = queryConnection.value;
                clear(connection); clear(queryConnection);
                d.Conexiones.forEach(function (c) { option(connection,c.Nombre,c.ConexionId); option(queryConnection,c.Nombre,c.ConexionId); });
                if (d.Conexiones.some(function (c) { return String(c.ConexionId) === previousConnection; })) { connection.value = previousConnection; }
                if (d.Conexiones.some(function (c) { return String(c.ConexionId) === previousQuery; })) { queryConnection.value = previousQuery; }
                limits.textContent = allowed() ? "Límite: " + d.MaxFilasDatos + " filas y " + d.MaxArchivoMB + " MB por archivo (1 MB = 1.000.000 bytes)." : "Las nuevas cargas no están disponibles; puedes consultar resultados anteriores.";
                history(); state = uncertain ? "RESULTADO_INCIERTO" : prepared ? "PREPARADO" : "VACIO";
                notice(uncertain ? messages.RESULTADO_INCIERTO : "Selecciona una plantilla o consulta una referencia conocida.");
                if (!allowed()) { panels.forEach(function (p) { p.hidden = p !== queryPanel; }); }
                // Listar se solicita por una acción explícita o al primer montaje, fuera del request de apertura.
            },false).then(function () { if (!closed && allowed() && !template && !uncertain) { return fetchList(); } });
        }
        function close() {
            if (busy && writeBusy) { state = "RESULTADO_INCIERTO"; if (!editPanel.hidden) { saveUncertain = true; } else { uncertain = true; } }
            epoch++; busy = false; writeBusy = false; closed = true; overlay.hidden = true;
            var target = opener && opener.isConnected ? opener : doc.getElementById("doc110-navigation-toggle"); if (target) { target.focus(); }
        }
        function fetchList() {
            if (!allowed() || !integer(Number(connection.value))) { return Promise.resolve(); }
            var c = Number(connection.value), p = pageNumber, term = search.value;
            return run(function () { return api.ejecutar("ListarPlantillas",{ConexionId:c,Termino:term,Pagina:p,TamanoPagina:25}); },function (r) {
                var d = r.datos;
                if (!d || !Array.isArray(d.Plantillas) || !d.Plantillas.every(function (v) { return integer(v.PlantillaId); }) || typeof d.Total !== "number") { throw invalid(false); }
                clear(selectTemplate); option(selectTemplate,"Selecciona una plantilla","");
                d.Plantillas.forEach(function (t) { option(selectTemplate,t.Nombre + (t.Activa ? "" : " (inactiva)"),t.PlantillaId); });
                listTotal = d.Total; listPage.textContent = "Página " + p + " · " + d.Total + " plantillas"; state = "VACIO"; notice("Selecciona una plantilla de la lista.");
            },false);
        }
        function selected(p) {
            if (!p || !integer(p.ID_PLANTILLAIMP) || p.IMP01_XMLCONEXION_ID_XMLCONEXION !== Number(connection.value) || !integer(p.REVISION_CARGA) || !hash.test(p.Huella || "") || !Array.isArray(p.Campos) || !modeNames[p.Modo]) { throw invalid(false); }
            template = copy(p); received = null; invalidate(true); file.value = ""; saveUncertain = false;
            summary.textContent = "Destino: " + p.NOMBRE_PLANTILLA + " · Clave única: " + p.CAMPO_UNICO + " · Formato: " + p.Formato;
            clear(mode); option(mode,modeNames[p.Modo],p.Modo); state = "VACIO"; notice(loadAllowed() ? "Plantilla seleccionada. El modo pertenece a su configuración." : "La plantilla necesita una clave única y estar activa antes de recibir un archivo. Revisa su configuración.");
        }
        connection.addEventListener("change",function () { if (writeBusy) { return; } clearAssistant(); epoch++; busy = false; template = null; invalidate(true); pageNumber = 1; queryConnection.value = connection.value; fetchList(); });
        selectTemplate.addEventListener("change",function () {
            if (writeBusy) { return; } clearAssistant(); epoch++; busy = false; invalidate(true); template = null;
            var id = Number(selectTemplate.value); if (!integer(id)) { return; }
            run(function () { return api.ejecutar("ConsultarPlantilla",{ConexionId:Number(connection.value),PlantillaId:id}); },function (r) { selected(r.datos); },false);
        });
        file.addEventListener("change",function () { if (!busy) { clearPendingFile(); invalidate(true); uncertain = false; sync(); } });
        sheet.addEventListener("change",function () { if (!busy) { invalidate(false); } }); consent.addEventListener("change",sync);
        function newLoad() {
            if (busy) { return; }
            if (uncertain) { notice("Consulta primero el resultado de la operación pendiente antes de iniciar otra carga."); showPanel(queryPanel); return; }
            if (saveUncertain) { notice("Consulta primero la plantilla antes de iniciar otra carga."); return; }
            if (!showPanel(loadPanel)) { return; }
            var previousTemplate = template;
            clearAssistant(); template = null; received = null; invalidate(true); file.value = "";
            summary.textContent = "Actualizando la plantilla antes de una nueva importación…";
            if (previousTemplate) {
                run(function () { return api.ejecutar("ConsultarPlantilla",{ConexionId:Number(connection.value),PlantillaId:previousTemplate.ID_PLANTILLAIMP}); },function (r) {
                    if (!r.datos || r.datos.ID_PLANTILLAIMP !== previousTemplate.ID_PLANTILLAIMP) { throw invalid(false); }
                    selected(r.datos); notice("Plantilla actualizada. Operación: " + modeNames[template.Modo] + ". Selecciona el siguiente archivo; las referencias anteriores se conservan.");
                },false);
            } else { notice("Selecciona una plantilla y un libro para una nueva carga."); sync(); }
        }
        function changeTemplateOperation() {
            if (busy || !allowed() || !template || uncertain || saveUncertain) { return; }
            var id = template.ID_PLANTILLAIMP, ticket = epoch, refreshed = false;
            run(function () { return api.ejecutar("ConsultarPlantilla",{ConexionId:Number(connection.value),PlantillaId:id}); },function (r) {
                if (!r.datos || r.datos.ID_PLANTILLAIMP !== id) { throw invalid(false); }
                clearAssistant(); selected(r.datos); refreshed = true;
            },false).then(function () {
                if (refreshed && ticket === epoch && live() && !closed && template && template.ID_PLANTILLAIMP === id) {
                    editTemplate(false); if (!editPanel.hidden) { editor.Modo.focus(); }
                }
            });
        }
        function upload() {
            if (!loadAllowed() || uncertain || received || (!pendingFile && (!file.files || file.files.length !== 1))) { return; }
            var f = pendingFile || file.files[0], expectedHash = pendingFile ? pendingHash : "", p = copy(template), c = Number(connection.value), ticket = epoch;
            if (pendingFile && pendingTemplate !== p.ID_PLANTILLAIMP) { clearPendingFile(); sync(); return; }
            if (!f.size || f.size > capability.MaxArchivoMB * 1000000 || !new RegExp("\\." + p.Formato + "$","i").test(f.name)) { fail({codigo:"ARCHIVO_INVALIDO"}); sync(); return; }
            invalidate(true);
            run(function () {
                return api.ejecutar("IniciarRecepcionExcel",{ConexionId:c,PlantillaId:p.ID_PLANTILLAIMP,RevisionEsperada:p.REVISION_CARGA,HuellaEsperada:p.Huella}).then(function (r) {
                    if (ticket !== epoch || closed || !live()) { return null; }
                    var i = r.datos; if (!i || !uuid.test(i.IdCarga || "") || typeof i.ReferenciaRecepcion !== "string") { throw invalid(true); }
                    remember(i.IdCarga,c); notice("Recibiendo el archivo. Conserva el ID de carga.");
                    return api.cargar({idCarga:i.IdCarga,referenciaRecepcion:i.ReferenciaRecepcion,conexionId:c,plantillaId:p.ID_PLANTILLAIMP},f);
                });
            },function (r) {
                var d = r && r.datos;
                if (!d || !uuid.test(d.IdCarga || "") || d.IdCarga !== queryId.value || d.Estado !== "RECIBIDA" || !Array.isArray(d.Hojas) || !d.Hojas.length) { throw invalid(true); }
                received = copy(d); clear(sheet); d.Hojas.forEach(function (name) { option(sheet,name,name); });
                if (expectedHash && d.ArchivoHash !== expectedHash) { received = null; clearPendingFile(); invalidate(true); throw {codigo:"ARCHIVO_ALTERADO",resultadoIncierto:false}; }
                clearPendingFile();
                if (d.Hojas.indexOf(p.LIBRO_EXCEL) !== -1) { sheet.value = p.LIBRO_EXCEL; }
                state = "VACIO"; notice("Archivo recibido. Selecciona su hoja y valida el contenido completo.");
            },true);
        }
        function showPreparation(d, valid) {
            if (!d || !d.Carga || !Array.isArray(d.Muestra) || !Array.isArray(d.Incidencias)) { return; }
            clear(preview); clear(incidents);
            preview.appendChild(node("h3","Vista previa"));
            preview.appendChild(node("p",text(d.FilasObservadas) + " filas observadas · " + Math.min(d.Muestra.length,25) + " en la muestra. La validación corresponde al archivo completo."));
            var rows = d.Muestra.slice(0,25), table = node("table",null,{"class":"doc126-table"}), wrap = node("div",null,{"class":"doc126-table-scroll"});
            table.appendChild(node("caption","Muestra de valores normalizados"));
            var columns = rows.length && Array.isArray(rows[0].Valores) ? rows[0].Valores.map(function (v) { return v.CampoDestino; }).slice(0,256) : [], h = node("tr");
            h.appendChild(node("th","Fila",{scope:"col"})); columns.forEach(function (c) { h.appendChild(node("th",c,{scope:"col"})); });
            var th = node("thead"); th.appendChild(h); table.appendChild(th); var tb = node("tbody");
            rows.forEach(function (r) { var tr = node("tr"); tr.appendChild(node("th",text(r.Fila),{scope:"row"})); columns.forEach(function (c) { var v = (r.Valores || []).filter(function (x) { return x.CampoDestino === c; })[0]; tr.appendChild(node("td",v ? value(v) : "")); }); tb.appendChild(tr); });
            table.appendChild(tb); wrap.appendChild(table); preview.appendChild(wrap);
            incidents.appendChild(node("h3","Incidencias")); incidents.appendChild(node("p",text(d.TotalIncidencias) + " en total · se muestran hasta 25."));
            var ol = node("ol"); d.Incidencias.slice(0,25).forEach(function (i) {
                var where = []; if (i.Hoja) { where.push("Hoja " + i.Hoja); } if (i.Fila !== null && i.Fila !== undefined) { where.push("fila " + i.Fila); }
                if (i.Columna !== null && i.Columna !== undefined) { where.push("columna " + i.Columna); } if (i.CampoFuente) { where.push("campo " + i.CampoFuente); }
                ol.appendChild(node("li",where.join(" · ") + (where.length ? ": " : "") + (messages[i.Codigo] || (/DUPLICAD/.test(i.Codigo || "") ? "El valor está duplicado." : "Revisa el valor indicado."))));
            }); incidents.appendChild(ol);
            if (d.IncidenciasTruncadas) { incidents.appendChild(node("p","Hay más incidencias que las mostradas. Corrige el libro y vuelve a validarlo.")); }
            var c = d.Carga;
            if (valid && received && c.IdCarga === received.IdCarga && c.Estado === "PREPARADA" && d.LecturaCompleta === true && d.ValidacionCompleta === true && d.DuplicadosEvaluados === true && d.TotalIncidencias === 0 && hash.test(c.ArchivoHash || "") && hash.test(c.ConfiguracionHash || "") && hash.test(c.EsquemaHash || "") && c.Modo === template.Modo) {
                prepared = copy(d); frozen = Object.freeze({ConexionId:Number(connection.value),IdCarga:c.IdCarga,ArchivoHash:c.ArchivoHash,ConfiguracionHash:c.ConfiguracionHash,EsquemaHash:c.EsquemaHash});
                preparationHash = c.ConfiguracionHash;
                consent.checked = false; consentBox.hidden = c.Modo !== "REEMPLAZAR";
                consentText.textContent = "Se sustituirá el contenido autorizado de " + c.Tabla + " por " + c.FilasPrevistas + " filas validadas.";
                state = "PREPARADO"; notice("Validación completa. Revisa la muestra antes de confirmar.");
            } else { prepared = null; frozen = null; state = "INCIDENCIAS"; }
        }
        function prepare() {
            if (!loadAllowed() || !received || uncertain || !sheet.value) { return; }
            var request = {ConexionId:Number(connection.value),PlantillaId:template.ID_PLANTILLAIMP,IdCarga:received.IdCarga,RevisionEsperada:template.REVISION_CARGA,HuellaEsperada:template.Huella,Hoja:sheet.value,Modo:template.Modo};
            if (preparationHash) { request.PreparacionHashEsperada = preparationHash; }
            prepared = null; frozen = null; consent.checked = false;
            run(function () { return api.ejecutar("Preparar",request); },function (r) { showPreparation(r.datos,true); },true,"CARGANDO","Validando el archivo completo…");
        }
        function showResult(d) {
            if (!d || !uuid.test(d.IdCarga || "") || d.IdCarga !== queryId.value || ["RECIBIDA","PREPARADA","CONFIRMADA","FALLIDA"].indexOf(d.Estado) === -1) { throw invalid(false); }
            if (received && received.IdCarga === d.IdCarga && Number(queryConnection.value) === Number(connection.value)) { flowOutcome = {IdCarga:d.IdCarga,Estado:d.Estado}; }
            clear(result); result.appendChild(node("h3",d.Estado === "CONFIRMADA" ? "Carga confirmada" : d.Estado === "FALLIDA" ? "Carga revertida" : "Carga " + d.Estado.toLowerCase()));
            result.appendChild(node("p","ID: " + d.IdCarga)); result.appendChild(node("p","Destino: " + text(d.Tabla) + " · " + (modeNames[d.Modo] || "Sin modo confirmado")));
            if (d.Estado === "CONFIRMADA") { result.appendChild(node("p",d.FilasConfirmadas + " filas confirmadas · fecha de confirmación: " + dateTime(d.ConfirmadoUtc))); state = "CONFIRMADO"; uncertain = false; prepared = null; frozen = null; }
            else if (d.Estado === "FALLIDA") {
                result.appendChild(node("p",messages[d.CodigoIncidencia] || "El servidor acreditó que la carga no se confirmó.")); uncertain = false; state = "INCIDENCIAS";
                if (prepared && frozen && frozen.IdCarga === d.IdCarga && frozen.ConfiguracionHash === d.ConfiguracionHash && frozen.EsquemaHash === d.EsquemaHash && frozen.ArchivoHash === d.ArchivoHash) { state = "PREPARADO"; consent.checked = false; }
            } else { result.appendChild(node("p","Este estado no demuestra que una operación haya terminado. Consulta de nuevo antes de decidir una repetición.")); state = uncertain ? "RESULTADO_INCIERTO" : "VACIO"; }
            notice(d.Estado === "CONFIRMADA" ? "Resultado recuperado. No se ha vuelto a importar el archivo." : "Consulta completada. Se conserva la referencia original.");
        }
        function dateTime(s) { var m = /^\/Date\((-?\d+)(?:[+-]\d{4})?\)\/$/.exec(text(s)), d = m ? new Date(Number(m[1])) : new Date(s); return isNaN(d.getTime()) ? "no disponible" : d.toISOString().replace("T"," "); }
        function confirmLoad() {
            if (!loadAllowed() || !confirmable() || busy) { return; }
            var request = Object.assign({},frozen,{ConfirmacionReemplazo:template.Modo === "REEMPLAZAR" && consent.checked});
            remember(request.IdCarga,request.ConexionId);
            run(function () { return api.ejecutar("Confirmar",request); },function (r) { showResult(r.datos); panels.forEach(function (p) { p.hidden = p !== queryPanel; }); },true,"CONFIRMANDO");
        }
        function queryResult() {
            var id = queryId.value.trim().toLowerCase(), c = Number(queryConnection.value);
            if (!uuid.test(id) || !integer(c)) { notice("Introduce un ID de carga válido y su conexión autorizada."); return; }
            queryId.value = id; remember(id,c);
            run(function () { return api.ejecutar("ConsultarResultado",{ConexionId:c,IdCarga:id}); },function (r) { showResult(r.datos); },false);
        }
        recent.addEventListener("change",function () { var r = refs.filter(function (v) { return v.IdCarga === recent.value; })[0]; if (r) { queryId.value = r.IdCarga; queryConnection.value = String(r.ConexionId); } });
        function editTemplate(isNew) {
            if (busy || !allowed() || !showPanel(editPanel)) { return; }
            if (editorDirty && !env.confirm("Hay cambios sin guardar. ¿Quieres descartarlos?")) { return; }
            clearAssistant(); assistant.hidden = (!isNew && !!template) || !host.getAttribute("data-analisis");
            clearConfigurationErrors();
            [editor.NOMBRE_PLANTILLA,editor.CAMPO_UNICO].forEach(function (control) { control.removeAttribute("aria-invalid"); control.removeAttribute("aria-describedby"); });
            editorOriginal = !isNew && template ? copy(template) : {ID_PLANTILLAIMP:0,REVISION_CARGA:0,Huella:"",Formato:"XLSX",Modo:"CREAR",NOMBRE_PLANTILLA:"imp_02_",CAMPO_UNICO:"",LIBRO_EXCEL:"Datos",ESTADO_PLANTILLA:1,Campos:[]};
            Object.keys(editor).forEach(function (k) { if (k === "ESTADO_PLANTILLA") { editor[k].checked = editorOriginal[k] === 1; } else { editor[k].value = editorOriginal[k] || ""; } });
            clear(fieldsBody); editorOriginal.Campos.forEach(addField); if (!fieldsBody.children.length) { addField({}); }
            editorDirty = false; sync(); editor.NOMBRE_PLANTILLA.focus();
        }
        function clearPendingFile() {
            pendingFile = null; pendingHash = ""; pendingTemplate = 0; pendingNotice.textContent = "";
        }
        function clearAssistant() {
            analysis = null; analyzedFile = null; suggestedTable = ""; clearPendingFile();
            clearConfigurationErrors();
            analysisFile.value = ""; clear(analysisSheet); clear(analysisKey); clear(analysisPreview);
        }
        analysisFile.addEventListener("change",function () {
            if (busy) { return; } analysis = null; analyzedFile = null; clearPendingFile(); clear(analysisSheet); clear(analysisKey); clear(analysisPreview); invalidate(true);
        });
        analysisKey.addEventListener("change",function () { editor.CAMPO_UNICO.value = analysisKey.value; editor.CAMPO_UNICO.removeAttribute("aria-invalid"); editor.CAMPO_UNICO.removeAttribute("aria-describedby"); editorDirty = true; invalidate(true); });
        analysisSheet.addEventListener("change",function () { if (!busy) { analyze(); } });
        function suggestDestination(filename) {
            var currentName = editor.NOMBRE_PLANTILLA.value;
            if (editor.Modo.value !== "CREAR" || (currentName.trim() && currentName.trim() !== "imp_02_" && currentName !== suggestedTable)) { return; }
            var stem = filename.replace(/\.(?:xls|xlsx)$/i, "");
            if (typeof stem.normalize === "function") { stem = stem.normalize("NFD").replace(/[\u0300-\u036f]/g, ""); }
            stem = stem.toLowerCase().replace(/[^a-z0-9_]+/g,"_").replace(/^_+|_+$/g,"");
            suggestedTable = "imp_02_" + (stem || "datos_excel").slice(0,57); editor.NOMBRE_PLANTILLA.value = suggestedTable;
        }
        function analyze() {
            if (busy || !allowed() || saveUncertain || !editorOriginal || editorOriginal.ID_PLANTILLAIMP !== 0 || !analysisFile.files || analysisFile.files.length !== 1) { return; }
            var f = analysisFile.files[0], selectedSheet = analysisSheet.value;
            if (!f.size || f.size > capability.MaxArchivoMB * 1000000 || !/\.(?:xls|xlsx)$/i.test(f.name)) { notice(messages.ARCHIVO_INVALIDO); return; }
            if (editorDirty && Array.prototype.some.call(fieldsBody.querySelectorAll('[data-field="CAMPO_FUENTE"]'),function (c) { return !!c.value; }) && !env.confirm("Se reemplazarán los campos propuestos. ¿Quieres analizar el archivo?")) { analysisSheet.value = analysis ? analysis.Hoja : ""; return; }
            clearConfigurationErrors();
            analysis = null; analyzedFile = null; clearPendingFile(); clear(analysisPreview); clear(analysisKey); invalidate(true);
            run(function () { return api.analizar({conexionId:Number(connection.value),hoja:selectedSheet,tabla:editor.NOMBRE_PLANTILLA.value,modo:editor.Modo.value},f); },function (r) {
                var d = r.datos;
                if (!d || !hash.test(d.ArchivoHash || "") || !Array.isArray(d.Campos) || !d.Campos.length || d.Campos.length > 256 || d.Campos.some(function (c) { return !c || typeof c.CAMPO_FUENTE !== "string" || typeof c.CAMPO_DESTINO !== "string" || typeof c.TIPO_CAMPO !== "string"; }) || !Array.isArray(d.Hojas) || d.Hojas.indexOf(d.Hoja) < 0 || !Array.isArray(d.Muestra) || d.Muestra.some(function (row) { return !Array.isArray(row); }) || !integer(d.FilasObservadas) || d.FilasObservadas > capability.MaxFilasDatos || ["XLS","XLSX"].indexOf(d.Formato) < 0) { throw invalid(false); }
                analysis = copy(d); analyzedFile = f; editor.Formato.value = d.Formato; editor.LIBRO_EXCEL.value = d.Hoja;
                suggestDestination(f.name);
                clear(analysisSheet); d.Hojas.forEach(function (h) { option(analysisSheet,h,h); }); analysisSheet.value = d.Hoja;
                clear(fieldsBody); d.Campos.forEach(addField); clear(analysisKey); option(analysisKey,"Selecciona el campo único","");
                d.Campos.forEach(function (c) { option(analysisKey,c.CAMPO_DESTINO,c.CAMPO_DESTINO); }); editor.CAMPO_UNICO.value = "";
                analysisPreview.appendChild(node("p",d.FilasObservadas + " filas analizadas · muestra de " + Math.min(25,d.Muestra.length) + " filas (valores abreviados a 120 caracteres). Revisa tipos, longitudes y campo único. No se ha importado el archivo."));
                if ((d.Avisos || []).indexOf("REVISAR_MAPEO_DESTINO") !== -1) { analysisPreview.appendChild(node("p","Hay columnas sin correspondencia con la tabla existente. Ajusta el destino antes de guardar.")); }
                var wrap = node("div",null,{"class":"doc126-table-scroll"}), table = node("table",null,{"class":"doc126-table"}), head = node("tr"), tbody = node("tbody"); table.appendChild(node("caption","Muestra del libro analizado"));
                d.Campos.forEach(function (c) { head.appendChild(node("th",c.CAMPO_FUENTE)); }); var thead = node("thead"); thead.appendChild(head); table.appendChild(thead);
                d.Muestra.slice(0,25).forEach(function (row) { var tr = node("tr"); row.slice(0,d.Campos.length).forEach(function (v) { tr.appendChild(node("td",v === null ? "NULL" : text(v))); }); tbody.appendChild(tr); });
                table.appendChild(tbody); wrap.appendChild(table); analysisPreview.appendChild(wrap); editorDirty = true;
                notice("Campos generados. Revisa el nombre de tabla, elige el campo único y guarda la plantilla. La importación se validará después."); analysisKey.focus();
            },false,"CARGANDO","Analizando el libro para proponer los campos…");
        }
        function addField(f) {
            f = copy(f);
            if (!f.ID_CAMPOS_PLANTILLA) { ["REMPLAZA_COMA","REMPLAZA_PUNTO","OPCION_SPLITER","NUMERO_MATRIZ_EXPLITER"].forEach(function (key) { if (f[key] === null || f[key] === undefined) { f[key] = 0; } }); }
            var tr = node("tr"); tr.doc126Field = copy(f);
            ["CAMPO_FUENTE","CAMPO_DESTINO","TIPO_CAMPO","REMPLAZA_COMA","REMPLAZA_PUNTO","OPCION_SPLITER","CARACTER_SPLITER","NUMERO_MATRIZ_EXPLITER"].forEach(function (k) {
                var td = node("td"), select = ["REMPLAZA_COMA","REMPLAZA_PUNTO","OPCION_SPLITER"].indexOf(k) !== -1;
                var labels = {CAMPO_FUENTE:"Columna de origen",CAMPO_DESTINO:"Columna de destino",TIPO_CAMPO:"Tipo de dato",REMPLAZA_COMA:"Cambiar coma",REMPLAZA_PUNTO:"Cambiar punto",OPCION_SPLITER:"Separar valor",CARACTER_SPLITER:"Separador",NUMERO_MATRIZ_EXPLITER:"Posición del segmento"};
                var input = node(select ? "select" : "input",null,{"data-field":k,"aria-label":labels[k] + " · campo " + (fieldsBody.children.length + 1)});
                if (select) { option(input,"Sin configurar",""); option(input,"No",0); option(input,"Sí",1); }
                else { input.type = k === "NUMERO_MATRIZ_EXPLITER" ? "number" : "text"; input.maxLength = k === "TIPO_CAMPO" ? 64 : k === "CAMPO_DESTINO" ? 64 : 120; if (input.type === "number") { input.min = "0"; input.max = "4294967295"; input.step = "1"; } }
                input.value = f[k] === null || f[k] === undefined ? "" : String(f[k]); controls.push(input); td.appendChild(input); tr.appendChild(td);
            });
            var td = node("td"); td.appendChild(button("Quitar","quitar-campo",function () { clearConfigurationErrors(); tr.remove(); editorDirty = true; refreshAnalysisKey(); invalidate(false); })); tr.appendChild(td); fieldsBody.appendChild(tr);
        }
        function refreshAnalysisKey() {
            if (!analysis) { return; }
            var chosen = editor.CAMPO_UNICO.value, names = [];
            Array.prototype.forEach.call(fieldsBody.querySelectorAll('[data-field="CAMPO_DESTINO"]'),function (input) { if (input.value && names.indexOf(input.value) === -1) { names.push(input.value); } });
            clear(analysisKey); option(analysisKey,"Selecciona el campo único",""); names.forEach(function (name) { option(analysisKey,name,name); });
            if (names.indexOf(chosen) !== -1) { analysisKey.value = chosen; } else { editor.CAMPO_UNICO.value = ""; }
        }
        editPanel.addEventListener("input",function (event) { event.target.removeAttribute("aria-invalid"); event.target.removeAttribute("aria-describedby"); if (event.target === editor.NOMBRE_PLANTILLA) { suggestedTable = ""; } if (!assistant.contains(event.target)) { editorDirty = true; if (event.target.getAttribute("data-field") === "CAMPO_DESTINO") { refreshAnalysisKey(); } invalidate(false); } });
        editPanel.addEventListener("change",function (event) { if (!assistant.contains(event.target)) { editorDirty = true; invalidate(false); } });
        function saveTemplate() {
            if (!allowed() || busy || saveUncertain || !editorOriginal) { return; }
            clearConfigurationErrors();
            if (!assistant.hidden && analysisFile.files.length && !analysis) { notice("Analiza el archivo seleccionado antes de guardar los campos propuestos."); return; }
            var p = {ID_PLANTILLAIMP:editorOriginal.ID_PLANTILLAIMP,REVISION_CARGA:editorOriginal.REVISION_CARGA,IMP01_XMLCONEXION_ID_XMLCONEXION:Number(connection.value)};
            Object.keys(editor).forEach(function (k) { p[k] = k === "ESTADO_PLANTILLA" ? (editor[k].checked ? 1 : 0) : editor[k].value; });
            p.Campos = Array.prototype.map.call(fieldsBody.children,function (tr) {
                var f = {ID_CAMPOS_PLANTILLA:tr.doc126Field.ID_CAMPOS_PLANTILLA || 0};
                Array.prototype.forEach.call(tr.querySelectorAll("[data-field]"),function (input) { var k = input.getAttribute("data-field"); f[k] = ["REMPLAZA_COMA","REMPLAZA_PUNTO","OPCION_SPLITER","NUMERO_MATRIZ_EXPLITER"].indexOf(k) !== -1 ? input.value === "" ? null : Number(input.value) : input.value; }); return f;
            });
            var invalidNumber = p.Campos.some(function (f) { return ["REMPLAZA_COMA","REMPLAZA_PUNTO","OPCION_SPLITER","NUMERO_MATRIZ_EXPLITER"].some(function (key) { var n = f[key]; return n !== null && (!isFinite(n) || Math.floor(n) !== n || n < 0 || n > (key === "NUMERO_MATRIZ_EXPLITER" ? 4294967295 : 1)); }); });
            if (invalidNumber) { notice("La posición debe ser un entero no negativo. Revisa las opciones de transformación."); return; }
            [editor.NOMBRE_PLANTILLA,editor.CAMPO_UNICO].forEach(function (control) { control.removeAttribute("aria-invalid"); control.removeAttribute("aria-describedby"); });
            if (!/^imp_02_[A-Za-z0-9_]+$/.test(p.NOMBRE_PLANTILLA) || p.NOMBRE_PLANTILLA.length > 64) { editorProblem(editor.NOMBRE_PLANTILLA,"Completa Tabla de destino: escribe un nombre después de imp_02_, usando letras, números o guion bajo, hasta 64 caracteres. Por ejemplo: imp_02_relacion_ordenes_2025."); return; }
            if (!/^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(p.CAMPO_UNICO) || !p.Campos.some(function (f) { return f.CAMPO_DESTINO === p.CAMPO_UNICO; })) { editorProblem(editor.CAMPO_UNICO,"Elige un campo único que exista entre las columnas de destino de la plantilla."); return; }
            if (!p.LIBRO_EXCEL || !p.Campos.length || p.Campos.length > 256 || p.Campos.some(function (f) { return !f.CAMPO_FUENTE || !f.CAMPO_DESTINO || !f.TIPO_CAMPO; })) { notice(messages.CONFIGURACION_INVALIDA); return; }
            var request = {ConexionId:p.IMP01_XMLCONEXION_ID_XMLCONEXION,RevisionEsperada:editorOriginal.REVISION_CARGA,HuellaEsperada:editorOriginal.Huella,Plantilla:p};
            invalidate(true);
            var keepFile = analyzedFile, keepHash = analysis && analysis.ArchivoHash;
            run(function () { return api.ejecutar("GuardarPlantilla",request).catch(function (e) { if (e.resultadoIncierto) { saveUncertain = true; } throw e; }); },function (r) {
                editorDirty = false; selected(r.datos); option(selectTemplate,r.datos.NOMBRE_PLANTILLA,r.datos.ID_PLANTILLAIMP); selectTemplate.value = String(r.datos.ID_PLANTILLAIMP); panels.forEach(function (panel) { panel.hidden = panel !== loadPanel; }); notice("Plantilla guardada. Selecciona un archivo para iniciar una carga.");
                if (keepFile && keepHash) { pendingFile = keepFile; pendingHash = keepHash; pendingTemplate = r.datos.ID_PLANTILLAIMP; pendingNotice.textContent = "Archivo listo: " + keepFile.name + ". Pulsa Recibir archivo para continuar. Al recargar tendrás que seleccionarlo otra vez."; notice("Plantilla guardada. El archivo analizado está listo para recibir y validar; aún no se ha importado."); }
            },true);
        }
        function editorProblem(control,message) {
            control.setAttribute("aria-invalid","true"); control.setAttribute("aria-describedby",status.id); state = "INCIDENCIAS"; notice(message); sync();
            control.scrollIntoView({block:"center",inline:"nearest"}); control.focus();
        }
        function keydown(event) {
            if (closed) { return; }
            if (event.key === "Escape") { event.preventDefault(); event.stopPropagation(); close(); }
            if (event.key === "Tab") {
                var focusable = Array.prototype.filter.call(dialog.querySelectorAll('button,input,select,[tabindex="0"]'),function (e) { return !e.disabled && e.getClientRects().length > 0; });
                if (!focusable.length) { event.preventDefault(); dialog.focus(); return; }
                var first = focusable[0], last = focusable[focusable.length-1];
                if (event.shiftKey && (doc.activeElement === first || !dialog.contains(doc.activeElement))) { event.preventDefault(); last.focus(); }
                else if (!event.shiftKey && (doc.activeElement === last || !dialog.contains(doc.activeElement))) { event.preventDefault(); first.focus(); }
            }
        }
        access.addEventListener("click",open); overlay.addEventListener("keydown",keydown); overlay.addEventListener("keypress",function (event) { event.stopPropagation(); if (event.key === "Enter" && event.target.tagName === "INPUT" && event.target.type !== "file") { event.preventDefault(); } });
        attachMenu(); history(); sync();
        var instance = {sincronizar:attachMenu, destruir:function () {
            epoch++; destroyed = true; clearAssistant(); api.cerrar(); env.removeEventListener("resize",fitViewport);
            if (env.visualViewport) { env.visualViewport.removeEventListener("resize",fitViewport); } if (viewportObserver) { viewportObserver.disconnect(); }
            if (zoomObserver) { zoomObserver.disconnect(); }
            section.remove(); overlay.remove(); entryNotice.remove(); delete host.doc126Instancia;
        }};
        host.doc126Instancia = instance; return instance;
    }
    function initialize() {
        var doc = env.document; if (!doc) { return; }
        if (doc.readyState === "loading") { if (!initialize.waiting) { initialize.waiting = true; doc.addEventListener("DOMContentLoaded",initialize,{once:true}); } return; }
        var host = doc.getElementById("doc126-excel");
        if (current && (!host || current.host !== host || current.scope !== host.getAttribute("data-recuperacion"))) { current.instance.destruir(); current = null; }
        if (host) { var instance = mount(host); if (instance) { current = {host:host,scope:host.getAttribute("data-recuperacion"),instance:instance}; instance.sincronizar(); } }
        var manager = env.Sys && env.Sys.WebForms && env.Sys.WebForms.PageRequestManager.getInstance();
        if (manager && registeredManager !== manager) { if (registeredManager && registeredManager.remove_endRequest) { registeredManager.remove_endRequest(initialize); } manager.add_endRequest(initialize); registeredManager = manager; }
    }
    return {inicializar:initialize,montar:mount};
}));
