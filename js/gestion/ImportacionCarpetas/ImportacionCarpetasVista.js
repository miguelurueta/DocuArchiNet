(function (root, factory) {
    "use strict";
    if (typeof module === "object" && module.exports) { module.exports = factory(); }
    else { root.ImportacionCarpetasVista = factory(); }
}(typeof window !== "undefined" ? window : this, function () {
    "use strict";
    function crear(host, action) {
        var doc = host.ownerDocument, previousFocus = doc.activeElement, previousOverflow = doc.body.style.overflow;
        doc.body.style.overflow = "hidden";
        function node(tag, text, style) { var n = doc.createElement(tag); if (text !== undefined) { n.textContent = String(text); } if (style) { n.className = style; } return n; }
        function button(text, name, value, disabled) { var b = node("button"); var glyph = {agregar:"folder",enviar:"upload",exportar:"download",consultar:"refresh",cerrar:"close",ayuda:"info",preparar:"arrow",confirmar:"arrow",reseleccionar:"document",reconciliar:"refresh"}[name] || (name === "fase" ? ["CREAR_EXPEDIENTE","RETOMAR_CREACION"].indexOf(value.fase) !== -1 ? "folder" : "document" : null); if (glyph) { b.appendChild(icon(glyph)); } var visibleText = text; if (["fase","reconciliar","reseleccionar"].indexOf(name) !== -1) { visibleText = text.replace(/ [0-9]+$/,""); b.setAttribute("aria-label",text); } var label = node("span",visibleText); if (name === "ayuda") { label.className = "doc111-help-text"; } b.appendChild(label); b.type = "button"; b.setAttribute("data-doc111-action", name); b.doc111Value = value; b.disabled = !!disabled; return b; }
        function add(parent) { Array.prototype.slice.call(arguments, 1).forEach(function (n) { if (n) { parent.appendChild(n); } }); return parent; }
        function icon(name) {
            var paths = {folder:["M3 7a2 2 0 0 1 2-2h5l2 2h7a2 2 0 0 1 2 2v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2Z"],upload:["M12 16V3m-5 5 5-5 5 5M4 16v4h16v-4"],check:["m5 12 4 4L19 6"],info:["M12 3a9 9 0 1 0 0 18 9 9 0 0 0 0-18","M12 11v6m0-10v1"],close:["m6 6 12 12M18 6 6 18"],lock:["M5 10h14v11H5Z","M8 10V7a4 4 0 0 1 8 0v3"],download:["M12 3v13m-5-5 5 5 5-5M4 17v4h16v-4"],document:["M14 3H6a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9Z","M14 3v6h6M8 13h8m-8 4h6"],arrow:["M4 12h16m-6-6 6 6-6 6"],refresh:["M20 7v5h-5M4 17v-5h5","M6 7a7 7 0 0 1 12-1l2 3M4 15l2 3a7 7 0 0 0 12-1"]},svg = doc.createElementNS("http://www.w3.org/2000/svg","svg");
            svg.setAttribute("class","icon"); svg.setAttribute("viewBox","0 0 24 24"); svg.setAttribute("fill","none"); svg.setAttribute("stroke","currentColor"); svg.setAttribute("stroke-width","1.7"); svg.setAttribute("stroke-linecap","round"); svg.setAttribute("stroke-linejoin","round"); svg.setAttribute("aria-hidden","true"); (paths[name] || paths.info).forEach(function (path) { var p = doc.createElementNS(svg.namespaceURI,"path"); p.setAttribute("d",path); svg.appendChild(p); }); return svg;
        }
        function badge(text,kind) { return node("span",text,"tag" + (kind ? " " + kind : "")); }
        function bytes(value) { var n = Number(value); return Number.isFinite(n) && n >= 0 ? n < 1024 ? n + " bytes" : (n / (n < 1048576 ? 1024 : 1048576)).toLocaleString("es-CO",{maximumFractionDigits:1}) + (n < 1048576 ? " KB" : " MB") : "Tamaño no disponible"; }
        function stats(parent,items) { var group = node("div",undefined,"stats"); items.forEach(function (v) { group.appendChild(add(node("div",undefined,"stat"),node("strong",v[0]),node("span",v[1]))); }); parent.appendChild(group); }
        function sectionHead(label,copy,step) { var h = node("div",undefined,"section-head"); add(h,add(node("div"),node("h3",label),node("p",copy)),step ? badge("Paso " + step + " de 3","blue") : null); content.appendChild(h); }
        function disclosure(parent,keyName,label,initial) { var d = node("details",undefined,"disclosure"), body = node("div",undefined,"detail-body"); d.setAttribute("data-doc111-panel",keyName); d.open = Object.prototype.hasOwnProperty.call(panelState,keyName) ? panelState[keyName] : !!initial; add(d,node("summary",label),body); parent.appendChild(d); return body; }
        function queryForm(parent,model) { var form = node("div",undefined,"doc111-query"), label = node("label","Referencia del lote"), input = node("input"); form.setAttribute("data-doc111-query-form",""); input.type = "text"; input.inputMode = "numeric"; input.maxLength = 10; input.autocomplete = "off"; input.setAttribute("aria-label","Referencia del lote"); input.setAttribute("data-doc111-reference",""); input.setAttribute("data-doc111-lote",""); input.value = model.referencia || ""; input.disabled = model.busy; label.appendChild(input); var submit = button("Consultar referencia","consultar-referencia",null,model.busy); submit.className = "primary"; submit.setAttribute("data-doc111-consultar",""); add(form,label,node("p","La consulta conserva el destino original y comprueba la autorización vigente.","small muted"),submit); parent.appendChild(form); return input; }
        function openQuery() { var inline = content.querySelector("[data-doc111-reference]"); if (inline) { inline.focus(); return; } queryReturnFocus = doc.activeElement; queryTitle.textContent = "Consultar importación"; queryBody.textContent = ""; var input = queryForm(queryBody,currentModel); queryModal.showModal(); input.focus(); }
        function closeQuery() { if (queryModal.open) { queryModal.close(); } var returnTo = queryReturnFocus && queryReturnFocus.isConnected ? queryReturnFocus : shortcuts.querySelector('[data-doc111-action="abrir-consulta"]'); if (returnTo && !returnTo.disabled) { returnTo.focus(); } }
        function shortcutsRender(model) {
            shortcuts.textContent = ""; var intro = add(node("div"),node("strong","Importación"),node("span","Consultas disponibles sin preparar otra importación","small muted")), access = node("div"); add(access,button("Consultar importación","abrir-consulta",null,model.busy && !(model.coordinacion&&model.coordinacion.activa)),button("Consultar último lote" + (model.ultimoLoteId ? " " + model.ultimoLoteId : ""),"consultar-ultimo",null,(model.busy && !(model.coordinacion&&model.coordinacion.activa)) || !model.ultimoLoteId)); access.lastChild.className = "subtle"; if (!model.ultimoLoteId) { access.lastChild.title = "Todavía no hay una referencia de lote guardada en este contexto."; } if(model.preparacionPendienteId){access.appendChild(button("Consultar revisión pendiente","consultar-preparacion",null,model.busy));} if (model.planPendienteId) { access.appendChild(button("Verificar confirmación pendiente","verificar-referencia",null,model.busy)); } if (model.puedeVolverImportacion) { access.appendChild(button("Volver a mi importación","volver-importacion",null,model.busy)); } add(shortcuts,intro,access);
        }
        function organizeFooter() { var left = node("div",undefined,"footer-left"), right = node("div",undefined,"footer-right"); Array.prototype.slice.call(footer.children).forEach(function (n) { if (n.tagName === "P") { n.className = "footer-hint"; left.appendChild(n); } else if (n.getAttribute("data-doc111-action") === "exportar") { left.insertBefore(n,left.firstChild); } else { if (n.classList.contains("primary")) { var glyph = n.querySelector("svg"); if (glyph) { glyph.remove(); } n.appendChild(icon("arrow")); } right.appendChild(n); } }); add(footer,left,right); }
        function phaseList(parent,resource,isRoot,model) { var root = isRoot ? resource : model.raices && model.raices.elementos.filter(function (r) { return r.recursoId === resource.raizId; })[0], created = !!root && (root.estado === "EXPEDIENTE_CONFIRMADO" || !!root.expedienteId && ["COMPLETADA","PARCIAL"].indexOf(root.estado) !== -1), flags = [isRoot ? resource.recepcionListaParaCrear === true || created || ["CREANDO","INCIERTO"].indexOf(resource.estado) !== -1 : resource.estadoRecepcion === "RECIBIDO",created,!isRoot && resource.valoresEfectivosFijados === true,!isRoot && !!resource.documentoId && ["ALMACENADO","COMPLETADO"].indexOf(resource.estado) !== -1], first = isRoot && created ? -1 : flags.indexOf(false), list = node("ol",undefined,"phase-list"); list.setAttribute("aria-label","Avance del recurso; datos y guardado corresponden al archivo"); [isRoot ? "Recepción requerida" : "Envío","Expediente","Datos","Guardado"].forEach(function (name,i) { list.appendChild(add(node("li",undefined,flags[i] ? "phase-done" : i === first ? "phase-current" : ""),node("span",flags[i] ? "✓" : i+1,"phase-dot"),node("span",name))); }); parent.appendChild(list); }

        var overlay = node("div", undefined, "doc111-overlay"), dialog = node("section", undefined, "doc111-dialog");
        dialog.setAttribute("role", "dialog"); dialog.setAttribute("aria-modal", "true"); dialog.setAttribute("aria-labelledby", "doc111-title"); dialog.tabIndex = -1;
        var shortcuts = node("section",undefined,"query-shortcuts doc111-shortcuts"), sheet = node("div",undefined,"sheet doc111-sheet"), head = node("header",undefined,"sheet-head doc111-head"), heading = node("div",undefined,"heading"), title = node("h2","Importar carpetas","doc111-title"); title.id = "doc111-title"; shortcuts.setAttribute("aria-label","Consultas de importación");
        var tools = node("div",undefined,"heading-tools"), close = button("Cerrar","cerrar"); close.className = "subtle"; close.querySelector("span").className = "sr-only"; close.setAttribute("aria-label","Cerrar"); var help = button("Cómo funciona","ayuda"); help.className = "subtle"; add(tools,help,close); add(heading,add(node("div"),title,node("p","Convierte tus carpetas en expedientes, paso a paso.")),tools); head.appendChild(heading); add(dialog,shortcuts,sheet); sheet.appendChild(head);
        var destination = node("div", "", "contextbar doc111-destino"); head.appendChild(destination);
        var steps = node("ol", undefined, "steps doc111-steps"); steps.setAttribute("aria-label", "Etapas de importación");
        ["Seleccionar", "Revisar", "Importar"].forEach(function (text,index) { steps.appendChild(add(node("li"),add(node("span",undefined,"step"),node("span",index+1,"step-num"),node("span",text)))); }); head.appendChild(steps);
        var notice = node("p", "", "doc111-notice"); notice.setAttribute("role", "status"); notice.setAttribute("aria-live", "polite");
        var panelState = {}, filterDrafts = {}, lastLot = null, pendingFocus = null, selectedResource = null, currentModel, queryReturnFocus, viewStates = Object.create(null), viewKey = null;
        var layout = node("div", undefined, "doc111-layout"), content = node("div", undefined, "doc111-content"), summary = node("aside", undefined, "doc111-summary");
        var configuration = node("details", undefined, "disclosure doc111-configuration"); configuration.setAttribute("data-doc111-panel", "configuracion"); configuration.appendChild(node("summary", "Detalles de configuración")); summary.className += " detail-body"; configuration.appendChild(summary); summary.setAttribute("aria-label", "Configuración e inventario"); layout.appendChild(content); layout.appendChild(configuration); var scroll = node("div",undefined,"doc111-scroll"); add(scroll,notice,layout); sheet.appendChild(scroll);
        sheet.insertBefore(notice,scroll);
        var footer = node("footer", undefined, "actionbar doc111-actionbar"), feedback = node("p", "", "query-feedback doc111-feedback"); feedback.setAttribute("role", "status"); feedback.setAttribute("aria-live", "polite"); sheet.appendChild(footer);
        var queryModal = node("dialog",undefined,"doc111-query-modal"), queryTitle = node("h2","Consultar importación"), queryBody = node("div",undefined,"modal-body"), modalHead = node("div",undefined,"modal-head"); queryTitle.id = "doc111-query-title"; queryModal.setAttribute("aria-labelledby",queryTitle.id); add(modalHead,queryTitle,button("Cerrar consulta","cerrar-consulta")); add(queryModal,modalHead,queryBody); dialog.appendChild(queryModal);
        var picker = node("input"); picker.type = "file"; picker.hidden = true; picker.setAttribute("aria-label", "Agregar carpeta"); dialog.appendChild(picker);
        var reselector = node("input"); reselector.type = "file"; reselector.hidden = true; reselector.setAttribute("aria-label", "Reseleccionar archivo pendiente"); dialog.appendChild(reselector);
        var reselectorCarpeta = node("input"); reselectorCarpeta.type = "file"; reselectorCarpeta.hidden = true; reselectorCarpeta.setAttribute("data-doc120-folder-reselect", ""); reselectorCarpeta.setAttribute("aria-label", "Vincular carpeta de originales pendientes"); dialog.appendChild(reselectorCarpeta);
        overlay.appendChild(dialog); host.appendChild(overlay);
        function click(event) {
            var target = event.target.closest("[data-doc111-action]");
            if (!target || !dialog.contains(target) || target.disabled) { return; }
            var name = target.getAttribute("data-doc111-action"), value = target.doc111Value;
            if (name === "abrir-consulta") { var paused=action("pausar-importacion");if(paused&&typeof paused.then==='function'){paused.then(function(){if(overlay.isConnected){openQuery();}});}else{openQuery();}return; }
            if (name === "ver-expedientes" && value == null) { mostrarExpedientes(1);return; }
            if (name === "expedientes-anterior" || name === "expedientes-siguiente") { mostrarExpedientes(resultPage+(name=== "expedientes-anterior"?-1:1));return; }
            if (name === "cerrar-consulta") { closeQuery(); return; }
            if (name === "ayuda") { queryReturnFocus = target; queryTitle.textContent = "Cómo continuar la importación"; queryBody.textContent = "Selecciona perfil y carpetas, prepara la revisión y confirma el plan. Confirmar e importar coordina envío, creación, datos y guardado. Puedes pausar antes de la siguiente operación; consultar o verificar nunca reanuda automáticamente. Para retomar, consulta la referencia y reselecciona los originales pendientes. Las operaciones inciertas requieren consultar o verificar su intento."; queryModal.showModal(); return; }
            if (name === "ver-accion") { selectedResource = value; pendingFocus = null; render(currentModel); scroll.scrollTop = 0; var primary = footer.querySelector(".doc111-primary:not(:disabled)"); if (primary) { primary.focus({preventScroll:true}); } return; }
            if (name === "ver-importacion") { selectedResource = null; render(currentModel); scroll.scrollTop = 0; var resume=footer.querySelector('.doc111-primary:not(:disabled)');if(resume){resume.focus({preventScroll:true});}return; }
            if (name === "ver-bloqueo") { scroll.scrollTop = 0; var blockedHeading = content.querySelector(".doc111-next h4"); if (blockedHeading) { blockedHeading.tabIndex = -1; blockedHeading.focus({preventScroll:true}); } return; }
            if (["fase","reconciliar","enviar"].indexOf(name) !== -1) { scroll.scrollTop = 0; }
            if (/^(filtrar|limpiar)-/.test(name)) { delete filterDrafts[name.indexOf("raices") !== -1 ? "RAICES" : "ARCHIVOS"]; }
            if (name === "consultar-referencia") { value = target.closest("[data-doc111-query-form]").querySelector("[data-doc111-reference]").value; }
            if (name === "filtrar-resultados" || name === "filtrar-raices") {
                value = {};
                Array.prototype.forEach.call(target.closest("[data-doc111-filtros]").querySelectorAll("[data-doc111-filter]"), function (field) {
                    value[field.getAttribute("data-doc111-filter")] = field.value.trim();
                });
            }
            action(name, value);
        }
        function change(event) { if (event.type === "change" && event.target.getAttribute("data-doc111-profile") === "1") { action("perfil", event.target.value); }
            if (event.target.hasAttribute("data-doc111-filter")) { var group = event.target.closest("[data-doc111-filtros]"), key = group.getAttribute("data-doc111-filtros"); filterDrafts[key] = filterDrafts[key] || {}; Array.prototype.forEach.call(group.querySelectorAll("[data-doc111-filter]"), function (field) { filterDrafts[key][field.getAttribute("data-doc111-filter")] = field.value; }); }
        }
        function key(event) {
            if (event.key === "Enter" && !event.isComposing && event.target.hasAttribute("data-doc111-reference")) { event.preventDefault(); event.stopPropagation(); event.target.closest("[data-doc111-query-form]").querySelector("[data-doc111-consultar]").click(); return; }
            if (event.key === "Enter" && !event.isComposing && event.target.getAttribute("data-doc111-filter") === "raizId") {
                event.preventDefault(); event.stopPropagation();
                var apply = event.target.closest("[data-doc111-filtros]").querySelector('[data-doc111-action="filtrar-resultados"]');
                if (apply && !apply.disabled) { apply.click(); }
            }
            if (event.key === "Escape" && queryModal.open) { event.preventDefault(); event.stopPropagation(); closeQuery(); return; }
            if (event.key === "Escape") { event.preventDefault(); action("cerrar"); }
            if (event.key === "Tab") {
                var controls = Array.prototype.filter.call((queryModal.open ? queryModal : dialog).querySelectorAll("button,input,select,summary,[tabindex]"), function (n) { return !n.disabled && !n.hidden && n.getClientRects().length > 0; });
                var first = controls[0], last = controls[controls.length - 1];
                if (event.shiftKey && (doc.activeElement === first || doc.activeElement === dialog)) { event.preventDefault(); if (last) { last.focus(); } }
                else if (!event.shiftKey && doc.activeElement === last) { event.preventDefault(); if (first) { first.focus(); } }
            }
        }
        queryModal.addEventListener("cancel",function (event) { event.preventDefault(); closeQuery(); });
        dialog.addEventListener("click", click); dialog.addEventListener("change", change); dialog.addEventListener("input", change); dialog.addEventListener("keydown", key); dialog.focus();
        function table(labels, rows, cards) {
            var scroll = node("div", undefined, "table-wrap doc111-table"), t = node("table"), tr = node("tr");
            if (cards) { scroll.className += " doc111-resultados"; }
            labels.forEach(function (label,index) { var th = node("th", label); th.scope = "col"; if (index === 0 && labels.length >= 3) { th.style.width = label === "Carpeta" ? labels.length === 3 ? "55%" : "43%" : label === "Archivo y raíz" ? "38%" : ""; } tr.appendChild(th); }); var thead = node("thead"); thead.appendChild(tr); t.appendChild(thead);
            var body = node("tbody"); rows.forEach(function (values) { var row = node("tr"); values.forEach(function (value, index) { var cell = node("td"); cell.setAttribute("data-label",labels[index]); if (value && value.nodeType) { cell.appendChild(value); } else { cell.textContent = value == null ? "" : String(value); } if (cards) { cell.insertBefore(node("span", labels[index], "doc111-cell-label"), cell.firstChild); } row.appendChild(cell); }); body.appendChild(row); }); if (!rows.length) { var row = node("tr"), empty = node("td","No hay elementos en esta página con los filtros aplicados.","empty-table"); empty.colSpan = labels.length; row.appendChild(empty); body.appendChild(row); } t.appendChild(body); scroll.appendChild(t); return scroll;
        }
        function pager(parent, page, total, size, name, busy) {
            if (page === 1 && total <= size) { return; }
            var bar = node("div", undefined, "mock-pager doc111-pager"); bar.appendChild(button("Anterior", name, page - 1, busy || page <= 1));
            bar.appendChild(node("span", total ? "Página " + page + " · " + total + (total === 1 ? " elemento" : " elementos") : "Sin elementos"));
            bar.appendChild(button("Siguiente", name, page + 1, busy || page * size >= total)); parent.appendChild(bar);
        }
        function filters(parent, roots, values, busy) {
            values = values || {};
            var panelKey = roots ? "RAICES" : "ARCHIVOS", panel = node("details", undefined, "disclosure filter-disclosure doc111-filter-panel"), activeCount = Object.keys(values).filter(function (key) { return !!values[key]; }).length;
            panel.setAttribute("data-doc111-panel", panelKey); panel.open = !!panelState[panelKey];
            panel.appendChild(node("summary", "Filtrar " + (roots ? "carpetas" : "archivos"))); panel.doc111FilterBadge = add(node("div",undefined,"filters-title"),badge(activeCount ? activeCount + (activeCount === 1 ? " filtro activo" : " filtros activos") : "Sin filtros",activeCount ? "blue" : ""));
            values = filterDrafts[panelKey] || values;
            var group = node("fieldset", undefined, "doc111-filters"), fields = node("div",undefined,"filter-fields"), filterActions = node("div",undefined,"filter-actions"); group.setAttribute("data-doc111-filtros", panelKey);
            group.appendChild(node("legend", roots ? "Filtrar carpetas" : "Filtrar archivos"));
            function field(label, name, options) {
                var wrapper = node("label", label), control = node(options ? "select" : "input");
                control.setAttribute("data-doc111-filter", name); control.setAttribute("aria-label", label); control.disabled = !!busy;
                if (options) {
                    options.forEach(function (item) { var option = node("option", item[1]); option.value = item[0]; control.appendChild(option); });
                } else { control.type = "text"; control.inputMode = "numeric"; control.maxLength = 10; control.autocomplete = "off"; }
                control.value = values[name] || ""; wrapper.appendChild(control); fields.appendChild(wrapper);
            }
            if (roots) {
                field("Estado de carpeta", "estado", [["", "Todos"], ["PENDIENTE", "Sin iniciar"], ["CREANDO", "Creando"], ["EXPEDIENTE_CONFIRMADO", "Expediente confirmado"], ["INCIERTO", "Por verificar"], ["BLOQUEADO", "Bloqueado"], ["COMPLETADA", "Completada"], ["PARCIAL", "Parcial"], ["CANCELADA", "Cancelada"]]);
            } else {
                field("Referencia de carpeta", "raizId");
                field("Efecto documental", "estado", [["", "Todos"], ["NO_INICIADO", "Sin iniciar"], ["EN_CURSO", "En curso"], ["ALMACENADO", "Almacenado"], ["COMPLETADO", "Completado"], ["RECHAZADO", "Rechazado"], ["INCIERTO", "Por verificar"], ["CANCELADO", "Cancelado"]]);
                field("Recepción", "estadoRecepcion", [["", "Todas"], ["PENDIENTE", "Sin recibir"], ["RECIBIENDO", "Recibiendo"], ["RECIBIDO", "Recibido"], ["RECHAZADO", "Rechazado"], ["INCIERTO", "Por verificar"], ["CANCELADO", "Cancelado"]]);
            }
            filterActions.appendChild(button("Aplicar filtros de " + (roots ? "carpetas" : "archivos"), roots ? "filtrar-raices" : "filtrar-resultados", null, busy));
            filterActions.appendChild(button("Limpiar filtros de " + (roots ? "carpetas" : "archivos"), roots ? "limpiar-raices" : "limpiar-resultados", null, busy)); add(group,fields,filterActions); panel.appendChild(add(node("div",undefined,"detail-body"),group)); parent.appendChild(panel);
        }
        function coordinacionVista(model,guide) {
            var flow=model.coordinacion, state=model.estado, done=state&&state.estado==='COMPLETADO', running=flow&&flow.activa;
            if (!state || model.bloqueoFlujo && !model.bloqueoFlujo.requiereConsulta && !running && !(flow&&flow.estado==='blocked'&&model.detalleErrorOperacion)) { return; }
            if(selectedResource&&!running&&!done&&!(flow&&model.mutacionIncierta)){footer.insertBefore(button('Ver avance del lote','ver-importacion',null,model.busy),footer.firstChild);return;}
            var titleText,copy,primary,reference=flow&&flow.actual,detail=!running&&flow&&flow.estado==='blocked'?model.detalleErrorOperacion:null,known=model.expedientesConfirmados||model.raices&&model.raices.elementos.filter(function(r){return r.expedienteId&&r.relacionNivelId;})||[];
            if(done){titleText='Importación completada';copy='El servidor confirmó el resultado del lote. Revisa sus expedientes y documentos o cierra el asistente.';primary=button(known.length===1?'Ver expediente':'Ver expedientes creados','ver-expedientes',known.length===1?known[0].recursoId:null,false);}
            else if(running){titleText=reference?{RECEPCION:'Enviando PDF',CREAR_EXPEDIENTE:'Creando expediente',MATERIALIZAR_VALORES:'Preparando datos',ALMACENAR:'Guardando documento'}[reference.fase]:'Consultando las condiciones del lote';if(reference&&reference.fase==='RECEPCION'&&model.transferencia&&model.transferencia.total>0&&model.transferencia.enviados>=model.transferencia.total){titleText='Esperando confirmación del servidor';}copy=flow.estado==='pausing'?'Se espera la solicitud iniciada antes de aplicar la pausa. No se enviará la siguiente.':'Se espera la respuesta del servidor. El siguiente archivo o fase solo comienza después de confirmar esta operación y consultar su resultado.';primary=button(flow.estado==='pausing'?'Aplicando pausa…':'Pausar importación','pausar-importacion',null,flow.estado==='pausing');}
            else if(!model.mutacionIncierta && (state.estado==='DETENIDO'||state.detencionSolicitada===true)){
                var resumable=state.estado==='DETENIDO'&&state.detencionSolicitada===true&&state.hayIntentosActivos===false&&state.hayIncertidumbre===false&&state.accionesPermitidas.indexOf('REANUDAR_LOTE')!==-1;
                titleText=resumable?'Importación detenida; puedes reanudarla':'Importación detenida';
                copy=resumable?'Se conservan los archivos recibidos, los expedientes y los documentos confirmados. Reanudar levantará la detención comprobada y continuará solo los pendientes del mismo lote.':state.hayIntentosActivos||state.hayIncertidumbre?'Hay un intento activo o incierto que debe comprobarse antes de reanudar. Consulta el resultado y utiliza la recuperación disponible.':'El servidor no ofrece una reanudación para el estado consultado. Actualizar solo consulta; conserva esta referencia para revisar el bloqueo.';
                primary=button(resumable?'Reanudar importación':'Actualizar estado',resumable?'reanudar-importacion':'consultar',null,model.busy);
            }
            else if(flow&&flow.estado==='needs-original'){titleText='Vincula el PDF pendiente para continuar';copy='Los resultados confirmados se conservan. Selecciona el original faltante; la selección no lo envía.';primary=reference?button('Vincular PDF pendiente','reseleccionar',reference.id,false):button('Actualizar estado','consultar',null,false);}
            else if(model.mutacionIncierta||flow&&flow.estado==='blocked'){titleText=detail?detail.titulo:'Revisa el resultado antes de continuar';copy=detail?detail.causa:flow&&flow.codigo==='COORDINACION_LIMITE'?'La coordinación alcanzó su tope de consultas u operaciones. Consulta el mismo lote y revisa sus resultados antes de continuar.':'La cola se detuvo. Consulta el mismo lote y utiliza únicamente la recuperación que permita el servidor; no repitas un efecto sin confirmación.';primary=button(flow&&flow.consultandoResultado?'Consultando resultado…':'Consultar resultado','consultar',null,flow&&flow.consultandoResultado);}
            else if(state.accionesPermitidas.indexOf('SOLICITAR_AVANCE')===-1){titleText='La importación requiere revisar su estado';copy='El servidor no permite iniciar otro archivo en este estado. Consulta el resultado y utiliza únicamente las acciones disponibles para los pendientes.';primary=button('Consultar resultado','consultar',null,model.busy);}
            else{titleText='Puedes continuar esta importación';copy='Se comprobarán todos los pendientes del mismo lote, aunque estén en otra página o fuera de los filtros. Los resultados confirmados no se repetirán.';primary=button('Continuar importación','continuar-importacion',null,model.busy);}
            guide.textContent='';guide.className='action-panel doc111-next'+(done?' complete':flow&&['blocked','needs-original'].indexOf(flow.estado)!==-1?' warn':'');guide.setAttribute('aria-label',done?'Resultado confirmado':'Estado de la importación');
            var execution=add(node('div',undefined,'execution-message'),node('div',done?'Resultado confirmado':running?'Operación en curso':'Cómo continuar','eyebrow'),node('h4',titleText),node('p',copy));
            if(detail){
                var fields=node('dl',undefined,'doc120-operation-detail');
                [['Fase afectada',detail.fase],[detail.tipo==='RAICES'?'Carpeta':'Archivo',detail.nombre],['Posición en el lote',detail.posicion?detail.posicion+' de '+detail.total:null],['Ruta dentro de la selección',detail.rutaRelativa],['Estado consultado',detail.estadoConsultado]].forEach(function(item){if(item[1]){add(fields,node('dt',item[0]),node('dd',item[1]));}});
                add(execution,fields,node('p',detail.certeza),node('p',flow.consultandoResultado?'Se está consultando el resultado del lote. No se repetirá esta operación ni se iniciará el siguiente archivo.':detail.siguiente,'doc120-operation-next'));
                if(detail.consultaPosterior){execution.appendChild(node('p',detail.consultaPosterior));}
                var support=node('details',undefined,'doc120-operation-support');support.appendChild(node('summary','Datos para soporte'));
                support.appendChild(node('p','Código: '+detail.codigo+' · lote '+model.loteId+' · '+(detail.tipo==='RAICES'?'carpeta ':'archivo ')+detail.id+(detail.intentoId?' · intento '+detail.intentoId:'')+(detail.operacionId?' · diagnóstico '+detail.operacionId:''),'small muted'));execution.appendChild(support);
            }
            else if(reference){execution.appendChild(node('p',(reference.tipo==='RAICES'?'Carpeta':'Archivo')+(reference.posicion?' '+reference.posicion+' de '+reference.total:'')+': '+reference.nombre+' · referencia '+reference.id,'current-folder'));}
            var transport=reference&&reference.fase==='RECEPCION'&&model.transferencia&&Number.isSafeInteger(model.transferencia.enviados)&&Number.isSafeInteger(model.transferencia.total)?model.transferencia.enviados+' de '+model.transferencia.total+' bytes de la solicitud enviados':'';
            if(transport){execution.appendChild(node('p',transport+'. El envío no equivale a documento guardado.','small muted'));}
            if(flow&&flow.enumeracionCompleta){var total=flow.totalArchivos*3+flow.totalRaices,value=flow.recibidos+flow.expedientes+flow.datos+flow.guardados,bar=node('progress');bar.max=total||1;bar.value=Math.min(value,total);bar.setAttribute('aria-label','Avance por fases confirmadas');add(execution,bar,node('span',(detail?'Último avance confirmado: ':'')+flow.guardados+' de '+flow.totalArchivos+' documentos guardados · '+flow.expedientes+' de '+flow.totalRaices+' expedientes creados','small muted'));}
            add(guide,add(node('div',undefined,'action-glyph'),icon(done?'check':reference&&reference.fase==='CREAR_EXPEDIENTE'?'folder':'document')),execution);
            var existingStats=content.querySelector('.stats');if(existingStats&&flow&&flow.enumeracionCompleta){var counts=node('div');stats(counts,[[flow.recibidos+' / '+flow.totalArchivos,'PDF recibidos'],[flow.expedientes+' / '+flow.totalRaices,'expedientes creados'],[flow.guardados+' / '+flow.totalArchivos,'documentos guardados']]);existingStats.replaceWith(counts.firstChild);}
            var stateHeading=content.querySelector('.section-head h3');if(stateHeading){stateHeading.textContent=done?'Resultado de la importación':'Importación del lote revisado';}
            var queryControl=content.querySelector('.query-control'),contextLabel=queryControl&&queryControl.previousElementSibling;if(contextLabel){contextLabel.querySelector('span').textContent='Lote '+model.loteId;contextLabel.querySelector('h4').textContent='Resultados de la importación';}
            if(done){Array.prototype.forEach.call(steps.children,function(step){step.removeAttribute('aria-current');step.firstChild.className='step done';step.firstChild.removeAttribute('aria-current');step.firstChild.firstChild.textContent='✓';});}
            var exportButton=footer.querySelector('[data-doc111-action="exportar"]');footer.textContent='';var footnote=reference?titleText+' · '+reference.nombre:done?'Resultado confirmado. Puedes cerrar sin repetir operaciones.':'La continuación conserva el mismo lote y sus resultados.';if(flow&&flow.enumeracionCompleta){footnote+=' · PDF '+flow.recibidos+'/'+flow.totalArchivos+' · expedientes '+flow.expedientes+'/'+flow.totalRaices+' · guardados '+flow.guardados+'/'+flow.totalArchivos;}if(transport){footnote+=' · '+transport;}add(footer,node('p',footnote));if(exportButton){footer.appendChild(exportButton);}if(done){footer.appendChild(button('Cerrar','cerrar',null,false));}primary.className='primary doc111-primary';footer.appendChild(primary);
        }
        var resultPage=1;
        function mostrarExpedientes(page){
            resultPage=page;var model=currentModel,rows=model.expedientesConfirmados||model.raices&&model.raices.elementos.filter(function(r){return r.expedienteId&&r.relacionNivelId;})||[],size=25;
            queryReturnFocus=doc.activeElement;queryTitle.textContent='Expedientes creados';queryBody.textContent='';queryBody.appendChild(node('p','El árbol muestra el código. Cada expediente conserva su ID, aunque el código se repita.'));
            queryBody.appendChild(table(['Código','ID del expediente','Acceso'],rows.slice((page-1)*size,page*size).map(function(r){return[r.codigoExpediente||'Código no disponible',r.expedienteId,button('Ver en la estructura','ver-expedientes',r.recursoId,false)];}),true));
            if(rows.length>size){add(queryBody,button('Anterior','expedientes-anterior',null,page<=1),node('span',' Página '+page+' '),button('Siguiente','expedientes-siguiente',null,page*size>=rows.length));}
            if(!queryModal.open){queryModal.showModal();}
        }

        function render(model) {
            currentModel = model; var oldScroll = scroll.scrollTop;
            var focused = doc.activeElement, restore = focused !== dialog && dialog.contains(focused) && !queryModal.contains(focused), referenceFocused = focused && focused.hasAttribute("data-doc111-reference"), focusAction = focused && focused.getAttribute("data-doc111-action"), focusValue = focused && JSON.stringify(focused.doc111Value), focusFilter = focused && focused.getAttribute("data-doc111-filter"), focusPanel = focused && focused.closest("[data-doc111-filtros]"), focusPanelKey = focusPanel && focusPanel.getAttribute("data-doc111-filtros");
            Array.prototype.forEach.call(content.querySelectorAll("details[data-doc111-panel]"), function (panel) { panelState[panel.getAttribute("data-doc111-panel")] = panel.open; });
            var nextViewKey = (model.consulta ? "consulta:" : "importacion:") + (model.loteId || "seleccion");
            if (viewKey !== nextViewKey) { if (viewKey) { viewStates[viewKey] = {drafts:filterDrafts,selected:selectedResource,panels:panelState,scroll:oldScroll,configurationOpen:configuration.open}; } var savedView = viewStates[nextViewKey]; filterDrafts = savedView ? savedView.drafts : {}; selectedResource = savedView ? savedView.selected : null; panelState = savedView ? savedView.panels : {}; oldScroll = savedView ? savedView.scroll : 0; configuration.open = !!savedView && savedView.configurationOpen; viewKey = nextViewKey; lastLot = model.loteId; }
            var closingQuery = model.busy && queryModal.open; if (closingQuery) { closeQuery(); }
            shortcutsRender(model);
            if (restore) { pendingFocus = {reference:referenceFocused, action:focusAction, value:focusValue, filter:focusFilter, panel:focusPanelKey}; }
            else if (focused !== dialog) { pendingFocus = null; }
            if (pendingFocus) { restore = true; referenceFocused = pendingFocus.reference; focusAction = pendingFocus.action; focusValue = pendingFocus.value; focusFilter = pendingFocus.filter; focusPanelKey = pendingFocus.panel; }
            if (closingQuery) { restore = true; referenceFocused = false; focusAction = "abrir-consulta"; focusValue = JSON.stringify(null); focusFilter = null; focusPanelKey = null; pendingFocus = {action:focusAction,value:focusValue}; }
            title.textContent = model.consulta ? "Consultar importación" : "Importar carpetas";
            steps.hidden = !!model.consulta;
            destination.textContent = ""; add(destination,add(node("span",undefined,"context-item"),icon("lock"),node("span",model.consulta ? "Destino original: " : "Destino: "),node("b",model.contexto ? model.contexto.rutaCanonica : "Consulta una referencia autorizada")));
            if (model.perfil) { destination.appendChild(add(node("span",undefined,"context-item"),node("span","Perfil: "),node("b",(model.perfil.Configuracion.NombreVisible || "Perfil " + model.perfil.PerfilId) + " · v" + model.perfil.NumeroVersion))); }
            if (model.loteId) { destination.appendChild(add(node("span",undefined,"context-item"),node("span","Lote: "),node("b",model.loteId + (model.estado && model.estado.estado ? " · " + model.estado.estado : "")))); }
            notice.textContent = model.mensaje || ""; notice.hidden = !model.mensaje; dialog.setAttribute("aria-busy", model.busy ? "true" : "false");
            if (model.bloqueoFlujo && !model.busy && !model.mensaje) { var blocked = model.bloqueoFlujo, reference = "lote " + model.loteId + (blocked.recurso ? " · " + (blocked.tipo === "RAICES" ? "carpeta " : "archivo ") + blocked.recurso.recursoId + (blocked.recurso.intentoActivoId ? " · intento " + blocked.recurso.intentoActivoId : "") : ""); notice.textContent = blocked.retomable ? "Creación pendiente · " + reference + ". Puedes solicitar Retomar creación; se comprobarán nuevamente sus condiciones." : blocked.verificable ? "Resultado pendiente · " + reference + ". El servidor permite verificar este intento sin repetir efectos." : (blocked.fase === "CREAR_EXPEDIENTE" ? "Creación sin confirmar" : "Operación sin confirmar") + " · " + reference + ". Actualizar solo consulta. Si continúa igual, requiere diagnóstico del intento; no repitas la operación."; notice.hidden = false; }
            notice.className = "notice doc111-notice" + ((model.bloqueoFlujo || ["ERROR","AVISO"].indexOf(model.tipoMensaje) !== -1) && (!model.busy || model.coordinacion && model.coordinacion.estado === 'blocked') ? " warn" : "");
            notice.setAttribute("role",(model.mutacionIncierta || model.tipoMensaje === "ERROR") && model.mensaje ? "alert" : "status");
            notice.setAttribute("aria-label","Detalle de la última operación");
            if (model.avisoArbol && !model.busy) { notice.textContent += (notice.textContent ? " " : "") + model.avisoArbol; notice.hidden = false; notice.className += " warn"; notice.setAttribute("role","alert"); }
            notice.tabIndex = notice.hidden ? -1 : 0;
            Array.prototype.forEach.call(steps.children, function (step, index) { var current = model.etapa < 2 ? 0 : model.etapa - 1, inner = step.firstChild; inner.className = "step" + (index < current ? " done" : ""); inner.firstChild.textContent = index < current ? "✓" : index + 1; if (index === current) { step.setAttribute("aria-current", "step"); inner.setAttribute("aria-current","step"); } else { step.removeAttribute("aria-current"); inner.removeAttribute("aria-current"); } });
            content.textContent = ""; summary.textContent = ""; footer.textContent = ""; feedback.textContent = model.ultimaConsulta ? "Estado actualizado " + new Date(model.ultimaConsulta).toLocaleTimeString() + " · consulta " + model.consultasCompletadas : "";
            feedback.hidden = !model.ultimaConsulta; shortcuts.appendChild(feedback);
            if (model.consulta && !model.estado) { sectionHead("Consulta una importación","Escribe la referencia o usa el último lote guardado."); queryForm(content,model); summary.textContent = "Consulta y recuperación de referencias. La autorización se comprueba en el servidor."; configuration.hidden = true; organizeFooter(); if (restore && !queryModal.open) { var input = content.querySelector("[data-doc111-reference]"); (input && !input.disabled ? input : dialog).focus(); } return; }
            configuration.hidden = false;
            summary.appendChild(node("h3", model.consulta ? "Seguimiento" : "Configuración"));
            var profile = model.perfil, config = profile && profile.Configuracion;
            if (!model.consulta) { summary.appendChild(node("p", profile ? "Perfil " + profile.PerfilId + " · versión " + profile.NumeroVersion : "Selecciona una versión publicada.")); }
            if (config) {
                summary.appendChild(node("p", "Gabinete: " + config.GabineteId));
                summary.appendChild(node("p", "Integridad: " + (config.ModoIntegridad === "PARCIAL_PERMITIDO" ? "Se permiten resultados parciales" : (config.AmbitoIntegridad === "LOTE" ? "Completa por lote" : "Completa por expediente"))));
                summary.appendChild(node("p", "Clasificación: " + [config.FondoRef, config.InstrumentoRef, config.AreaRef, config.SerieRef, config.SubserieRef].filter(Boolean).join(" / ")));
            }
            var inventory = model.manifiesto;
            function identity(rootId, fileId) {
                var r = inventory.raices.filter(function (item) { return item.idCliente === rootId; })[0], f = inventory.archivos.filter(function (item) { return item.idCliente === fileId; })[0];
                return (r ? r.nombre + " (" + rootId + ")" : rootId || "Global") + (fileId ? " / " + (f ? f.nombre + " (" + fileId + ")" : fileId) : "");
            }
            function revisionIncident(incident, result) {
                if (incident.Codigo !== "CODIGO_EXPEDIENTE_EXISTENTE") { return model.describirIncidencia ? model.describirIncidencia(incident) : "Revisa la regla del campo indicado."; }
                var detail = node("div"), roots = (result.Elementos || []).filter(function (item) { return incident.Ambito === "RAIZ" && incident.RaizIdCliente && !incident.ArchivoIdCliente && item.Ambito === "RAIZ" && item.RaizIdCliente === incident.RaizIdCliente && !item.ArchivoIdCliente; }), codes = roots.length === 1 ? roots[0].Valores.filter(function (value) { return value.DestinoTipo === "CAMPO_EXPEDIENTE" && value.DestinoRef === "CODIGO" && value.Diferido !== true; }) : [];
                if (codes.length === 1 && typeof codes[0].Valor === "string" && codes[0].Valor.trim() && codes[0].Valor.length <= 45 && !/[\x00-\x1f\x7f-\x9f\uD800-\uDFFF]/.test(codes[0].Valor)) { detail.appendChild(node("p", "Código del expediente: «" + codes[0].Valor + "».")); }
                var copy = Object.assign({}, incident, {Campo:""});
                detail.appendChild(node("p", model.describirIncidencia ? model.describirIncidencia(copy) : "Ya existe un expediente con este código en la misma empresa y área. Revisa los datos previstos y la regla del código antes de preparar una nueva revisión."));
                return detail;
            }
            if (!model.consulta) { summary.appendChild(node("p", inventory.raices.length + " carpetas · " + inventory.archivos.length + " archivos seleccionados")); }
            else if (model.estado) { summary.appendChild(node("p", model.estado.totalRaices + " carpetas · " + model.estado.totalArchivos + " archivos del inventario original")); }
            if (model.loteId) { summary.appendChild(node("p", "Lote: " + model.loteId)); }
            summary.appendChild(node("p", "Cerrar conserva la referencia del lote confirmado. Los archivos locales pendientes pueden requerir una nueva selección."));
            if (model.progresoPreparacion) {
                var preparationProgress = model.progresoPreparacion, preparationPanel = node("section",undefined,"notice doc120-preparation-progress"), progressBar = node("progress");
                preparationPanel.setAttribute("role","status"); preparationPanel.setAttribute("aria-live","polite");
                preparationPanel.appendChild(node("strong",preparationProgress.fase === "REGISTRAR" ? "Registrando el inventario de la carpeta" : preparationProgress.fase === "REVISION" ? "Preparando la vista de revisión" : "Revisando los datos de los archivos"));
                preparationPanel.appendChild(node("p", (preparationProgress.archivosRevisados || 0) + " de " + preparationProgress.totalArchivos + " archivos revisados. Todavía no se envían PDF ni se crean expedientes."));
                progressBar.max = preparationProgress.totalArchivos; progressBar.value = preparationProgress.archivosRevisados || 0; progressBar.setAttribute("aria-label","Archivos revisados"); preparationPanel.appendChild(progressBar);
                preparationPanel.appendChild(button(model.busy ? preparationProgress.pausada ? "Pausando revisión…" : "Pausar revisión" : "Continuar revisión", model.busy ? "pausar-preparacion" : "continuar-preparacion",null,!!(model.busy && preparationProgress.pausada)));
                content.appendChild(preparationPanel);
            }
            if (model.etapa === 0 || model.etapa === 1) {
                sectionHead("Selecciona qué vas a importar","Elige el perfil y agrega una carpeta por cada expediente.",1);
                var form = node("div",undefined,"form-row"), profileField = node("label","Perfil de importación","field"), select = node("select"); select.setAttribute("data-doc111-profile","1"); select.setAttribute("aria-label","Perfil publicado"); select.disabled = model.busy;
                var empty = node("option","Selecciona un perfil"); empty.value = ""; select.appendChild(empty);
                model.perfiles.forEach(function (p) { var option = node("option",(p.Configuracion.NombreVisible || "Perfil " + p.PerfilId) + " · versión " + p.NumeroVersion); option.value = p.VersionId; option.selected = profile && profile.VersionId === p.VersionId; select.appendChild(option); }); add(profileField,select,node("small","Define cómo se interpretan los nombres y los datos del documento."));
                if (model.masPerfiles) { var more = button("Más perfiles","mas-perfiles",null,model.busy); more.className = "subtle doc111-profile-more"; profileField.appendChild(more); }
                var destField = node("div","Destino de los expedientes","field"); add(destField,add(node("div",undefined,"read-value"),icon("lock"),node("span",model.contexto.rutaCanonica)),node("small","Corresponde al nivel que elegiste antes de abrir la importación.")); add(form,profileField,destField); content.appendChild(form);
                var selection = node("section",undefined,"selection"), selectionTitle = node("div",undefined,"selection-title"), folderButton=button("Agregar carpeta","agregar",null,model.busy || !profile || !model.selectorDisponible); selection.setAttribute("aria-label","Carpetas seleccionadas");folderButton.setAttribute('aria-describedby','doc120-permiso-carpeta');add(selectionTitle,add(node("h4"),node("span","Carpetas seleccionadas "),badge(inventory.raices.length)),folderButton);selection.appendChild(selectionTitle);
                var consentHelp=node('p','El navegador puede pedir permiso con el botón «Subir». Ese permiso permite leer la carpeta seleccionada. El contenido de los PDF se enviará al pulsar «Confirmar e importar».','doc120-folder-permission');consentHelp.id='doc120-permiso-carpeta';selection.appendChild(consentHelp);
                if (!inventory.raices.length) { selection.appendChild(add(node("div",undefined,"empty"),icon("folder"),node("strong","Agrega tu primera carpeta"),node("p","Selecciona una carpeta en este equipo. Puedes agregar varias, una por selección."))); }
                inventory.raices.slice((model.paginaRaices-1)*10,model.paginaRaices*10).forEach(function (r) { var files = inventory.archivos.filter(function (f) { return f.raizIdCliente === r.idCliente; }), info = add(node("div",undefined,"folder-info"),node("span",r.nombre,"folder-name"),node("span",files.length + " archivos · " + bytes(files.reduce(function (n,f) { return n + Number(f.bytes); },0)),"folder-sub")); var remove = button("Quitar","quitar-raiz",r.idCliente,model.busy); remove.className = "subtle"; selection.appendChild(add(node("div",undefined,"folder-row"),add(node("div",undefined,"folder-glyph"),icon("folder")),info,remove)); }); pager(selection,model.paginaRaices,inventory.raices.length,10,"pagina-raices",model.busy); content.appendChild(selection);
                if (inventory.archivos.length) { var selectedFiles = disclosure(content,"inventario-archivos","Ver archivos seleccionados (" + inventory.archivos.length + ")",false); selectedFiles.appendChild(table(["Archivo","Ruta relativa","Tamaño","Acción"],inventory.archivos.slice((model.paginaArchivos-1)*20,model.paginaArchivos*20).map(function (f) { return [f.nombre,f.rutaRelativa,bytes(f.bytes),button("Quitar","quitar-archivo",f.idCliente,model.busy)]; }),true)); pager(selectedFiles,model.paginaArchivos,inventory.archivos.length,20,"pagina-archivos",model.busy); }
                footer.appendChild(node("p","Revisa los datos antes de confirmar el plan.")); footer.appendChild(button("Preparar revisión","preparar",null,model.busy || !profile || !inventory.archivos.length));
            } else if (model.etapa === 2) {
                sectionHead("Revisa antes de importar","Comprueba cómo quedarán los expedientes en el destino seleccionado.",2); var result = model.plan && model.plan.resultado;
                if (result) {
                    stats(content,[[result.UnidadesElegibles.length+' / '+inventory.raices.length,"carpetas listas"],[inventory.archivos.length,"PDF seleccionados"],[(result.Incidencias || []).length,"incidencias de revisión"]]);
                    content.appendChild(add(node("div",undefined,"table-head"),node("h4","Selección revisada"),badge(profile ? profile.Configuracion.NombreVisible || "Perfil " + profile.PerfilId : "Perfil confirmado")));
                    var reviewed=node('ul',undefined,'doc120-review-list');reviewed.setAttribute('aria-label','Carpetas revisadas');inventory.raices.slice(((model.paginaRaicesRevision || 1)-1)*10,(model.paginaRaicesRevision || 1)*10).forEach(function (r) {
                        var files = inventory.archivos.filter(function (f) { return f.raizIdCliente === r.idCliente; });
                        var entries=(result.Elementos||[]).filter(function(e){return e.Ambito==='RAIZ'&&e.RaizIdCliente===r.idCliente&&!e.ArchivoIdCliente;});
                        function planned(field){var values=entries.length===1?entries[0].Valores.filter(function(v){return v.DestinoTipo==='CAMPO_EXPEDIENTE'&&v.DestinoRef===field;}):[];return values.length===1&&!values[0].Diferido?values[0].Valor:'Consulta los datos previstos';}
                        var ready=result.UnidadesElegibles.indexOf(r.idCliente)!==-1,card=node('li',undefined,'doc120-review-card'),heading=add(node('div',undefined,'doc120-review-heading'),add(node('div'),node('span','Carpeta seleccionada','small muted'),node('strong',r.nombre)),badge(ready?'Lista para importar':'Consulta las incidencias',ready?'green':'amber'));
                        var fields=node('dl',undefined,'doc120-review-values');['CODIGO','NOMBRE'].forEach(function(field){fields.appendChild(add(node('div'),node('dt',field==='CODIGO'?'Código del expediente':'Nombre informativo'),node('dd',planned(field))));});
                        reviewed.appendChild(add(card,heading,fields,node('p',files.length+' '+(files.length===1?'PDF seleccionado':'PDF seleccionados'),'small muted')));
                    });content.appendChild(reviewed);if(inventory.raices.length>10){pager(content,model.paginaRaicesRevision||1,inventory.raices.length,10,"pagina-raices-revision",model.busy);}
                    var incidents = result.Incidencias || [];
                    if (incidents.length) { var issues = disclosure(content,"incidencias","Incidencias de revisión (" + incidents.length + ")",true); issues.appendChild(table(["Raíz / archivo", "Campo", "Incidencia"], incidents.slice((model.paginaIncidencias - 1) * 20, model.paginaIncidencias * 20).map(function (i) { return [identity(i.RaizIdCliente, i.ArchivoIdCliente), i.Codigo === "CODIGO_EXPEDIENTE_EXISTENTE" ? "Código del expediente" : /^[A-Za-z0-9_:]{1,96}$/.test(i.Campo || "") ? i.Campo : "selección", revisionIncident(i,result)]; }),true)); pager(issues, model.paginaIncidencias, incidents.length, 20, "pagina-incidencias", model.busy); }
                    var values = result.Elementos || [], anticipated = disclosure(content,"valores","Ver datos previstos de los expedientes",false);
                    anticipated.appendChild(table(["Raíz / archivo", "Valores resueltos o diferidos"], values.slice((model.paginaValores - 1) * 10, model.paginaValores * 10).map(function (item) { return [identity(item.RaizIdCliente, item.ArchivoIdCliente), item.Valores.map(function (v) { return v.DestinoRef + ": " + (v.Diferido ? "Pendiente: " + v.PropiedadPendiente : v.Valor); }).join(" · ")]; })));
                    pager(anticipated, model.paginaValores, values.length, 10, "pagina-valores", model.busy);
                }
                footer.appendChild(node("p", "Al confirmar autorizas enviar los PDF, crear sus expedientes, preparar los datos y guardar los documentos. Puedes pausar entre operaciones.")); footer.appendChild(button("Cambiar selección", "etapa", 1, model.busy || model.confirmacionIncierta));
                footer.appendChild(button("Confirmar e importar", "confirmar-importar", null, model.busy || !model.confirmable || model.confirmacionIncierta));
            } else {
                sectionHead("Seguimiento de la importación","El lote conserva su destino original. Consultar y exportar no ejecuta efectos.",model.consulta ? null : 3);
                if (model.estado && model.estado.estado) { var stateHead = content.lastChild, stateTag = stateHead.querySelector(".tag"); if (stateTag) { stateTag.textContent = model.estado.estado; } else { stateHead.appendChild(badge(model.estado.estado,"blue")); } }



                var statusButton = button("Actualizar estado", "consultar", null, model.busy); content.appendChild(statusButton);
                if (model.resultados) {

                    var send = button(model.archivosParaEnviar === 1 ? "Enviar archivo" : "Enviar " + (model.archivosParaEnviar || 0) + " archivos", "enviar", null, model.busy || !model.puedeEnviar); send.setAttribute("data-doc111-send-count",model.archivosParaEnviar || 0); send.title = "Solo archivos pendientes de la página filtrada seleccionados en este equipo"; content.appendChild(send);

                }
                if (model.estado) {
                    if (model.estado.accionesPermitidas.indexOf("CONSULTAR_RAICES") !== -1) { filters(content, true, model.filtrosRaices, model.busy); }
                    filters(content, false, model.filtrosArchivos, model.busy);
                }
                if (model.estado && model.estado.accionesPermitidas.indexOf("SOLICITAR_DETENCION") !== -1) { content.appendChild(button("Solicitar detención", "detener", null, model.busy || model.mutacionIncierta)); }
                var candidate = null, candidates = [];
                function resourceActions(resource, raiz) {
                    var group = node("span",undefined,"doc111-resource-actions"), labels = {CREAR_EXPEDIENTE:"Crear expediente", RETOMAR_CREACION:"Retomar creación", MATERIALIZAR_VALORES:"Preparar índices", ALMACENAR:"Almacenar archivo", RECONCILIAR:"Verificar intento"};
                    (model.accionesRecurso ? model.accionesRecurso(resource, raiz) : []).forEach(function (phase) {
                        var operationButton = button(labels[phase] + " " + resource.recursoId, phase === "RECONCILIAR" ? "reconciliar" : "fase", {tipo:raiz ? "RAICES" : "ARCHIVOS", id:resource.recursoId, fase:phase}, model.busy);
                        candidates.push({button:operationButton,group:group,resource:resource,root:raiz,title:labels[phase]});
                    });
                    var choose = button("Ver acciones","ver-accion",{tipo:raiz ? "RAICES" : "ARCHIVOS",id:resource.recursoId},model.busy); choose.setAttribute("aria-label","Ver acciones de " + (raiz ? "carpeta " : "archivo ") + resource.recursoId); group.appendChild(choose);
                    if(raiz && model.reseleccionCarpetaDisponible && (model.consulta || model.coordinacion && model.coordinacion.estado === "needs-original") && ["COMPLETADA","BLOQUEADO","CANCELADA"].indexOf(resource.estado) === -1){group.appendChild(button("Vincular carpeta de originales","vincular-carpeta",resource.recursoId,model.busy || model.mutacionIncierta));}
                    if (raiz && resource.estado === "PENDIENTE" && !resource.expedienteId && !resource.tieneIntentoActivo && resource.recepcionListaParaCrear !== true) {
                        var scope = model.estado && model.estado.ambitoIntegridad === "LOTE" ? "del lote" : "de esta carpeta";
                        var partial = model.estado && model.estado.modoIntegridad === "PARCIAL_PERMITIDO";
                        group.appendChild(node("span", partial ? "Recibe al menos un archivo elegible de esta carpeta y consulta el estado para crear el expediente." : "Completa la recepción de los archivos " + scope + " y consulta el estado para crear el expediente."));
                    }
                    return group;
                }
                if (model.raices) {
                    content.appendChild(node("h4", "Carpetas del lote"));
                    content.appendChild(table(["Carpeta", "Expediente", "Estado", "Acciones"], model.raices.elementos.map(function (r) {
                        return [add(node("div"),node("strong",r.nombre),node("span","Referencia de carpeta: " + r.recursoId,"small muted")), r.codigoExpediente || r.nombreExpediente || r.expedienteId || "Pendiente", badge(r.estado,r.estado === "EXPEDIENTE_CONFIRMADO" ? "green" : ""), resourceActions(r, true)];
                    }), true));
                    pager(content, model.raices.pagina, model.raices.totalFiltrado, model.raices.tamanoPagina, "pagina-raices-resultado", model.busy);
                }
                if (model.resultados) {
                    var page = model.resultados;
                    content.appendChild(table(["Archivo y raíz", "Recepción", "Efecto documental", "Acciones"], page.elementos.map(function (f) {
                        var pending = f.estadoRecepcion === "PENDIENTE" && f.estado === "NO_INICIADO" && !f.tieneIntentoActivo;
                        var controls = resourceActions(f,false); if (pending && !(model.originalDisponible && model.originalDisponible(f))) { controls.appendChild(button("Reseleccionar archivo " + f.recursoId,"reseleccionar",f.recursoId,model.busy)); }
                        return [add(node("div",undefined,"file-name"),node("span","PDF","pdf"),add(node("div"),node("strong",f.nombre || f.rutaRelativa),node("span",(f.rutaRelativa || f.nombre) + " · carpeta " + f.raizId + " · archivo " + f.recursoId,"small muted"))),badge(f.estadoRecepcion,f.estadoRecepcion === "RECIBIDO" ? "green" : ""),add(node("div"),badge(f.estado),f.documentoId ? node("span","Documento " + f.documentoId,"small muted") : null),controls];
                    }), true));
                    var manualNote=node("p", "Envío manual: el botón Enviar archivo(s) solo recibe pendientes seleccionados en este equipo de la página filtrada. Continuar importación revisa todo el lote. Recibir no equivale a almacenar.");
                    pager(content, page.pagina, page.totalFiltrado, page.tamanoPagina, "pagina-resultados", model.busy);
                    if (model.estado && model.estado.accionesPermitidas.indexOf("EXPORTAR_PAGINA") !== -1) { footer.appendChild(button("Exportar esta página", "exportar", null, model.busy)); }
                }
            }
            if (model.etapa === 3 && model.loteId) {
                var blocking = model.bloqueoFlujo;
                candidate = candidates.filter(function (c) { return selectedResource && c.resource.recursoId === selectedResource.id && (c.root ? "RAICES" : "ARCHIVOS") === selectedResource.tipo; })[0];
                if (!candidate && selectedResource && selectedResource.tipo === "RAICES") { candidate = candidates.filter(function (c) { return !c.root && c.resource.raizId === selectedResource.id; })[0]; }
                if (!selectedResource) { candidate = candidates[0]; }
                if (blocking) { candidate = blocking.recurso && candidates.filter(function (c) { return c.resource.recursoId === blocking.recurso.recursoId && (c.root ? "RAICES" : "ARCHIVOS") === blocking.tipo; })[0]; }
                var chosenRoot = selectedResource && selectedResource.tipo === "RAICES", chosen = candidate && candidate.resource, chosenIsRoot = candidate && candidate.root;
                if (blocking && blocking.recurso) { chosen = blocking.recurso; chosenIsRoot = blocking.tipo === "RAICES"; }
                if (!chosen && selectedResource) { var selectedPage = chosenRoot ? model.raices : model.resultados; chosen = selectedPage && selectedPage.elementos.filter(function (r) { return r.recursoId === selectedResource.id; })[0]; chosenIsRoot = chosenRoot; }
                if (!chosen) { chosen = model.resultados && model.resultados.elementos[0] || model.raices && model.raices.elementos[0]; chosenIsRoot = !(model.resultados && model.resultados.elementos.length); }
                if (blocking && blocking.fueraDePagina) { chosen = null; chosenIsRoot = false; }
                var promote = candidate && candidate.button, guideTitle = candidate && {CREAR_EXPEDIENTE:"La recepción de esta carpeta está completa",MATERIALIZAR_VALORES:"El expediente está creado",ALMACENAR:"Los índices están preparados",RECONCILIAR:"Hay un intento por verificar"}[candidate.button.doc111Value.fase], guideCopy = candidate && {CREAR_EXPEDIENTE:"Puedes solicitar la creación de su expediente en el destino original. Los documentos todavía no están almacenados.",MATERIALIZAR_VALORES:"Prepara los índices del archivo con el perfil confirmado. Esta acción es independiente del almacenamiento.",ALMACENAR:"Solicita el almacenamiento de este archivo en el expediente confirmado. Se comprobarán nuevamente sus condiciones.",RECONCILIAR:"Verifica el resultado del intento registrado sobre este recurso. No se inicia una operación nueva."}[candidate.button.doc111Value.fase], glyph = candidate && candidate.root ? "folder" : "document";
                if (!blocking && model.puedeEnviar && !model.busy && (!selectedResource || chosen && !chosenIsRoot && chosen.estadoRecepcion === "PENDIENTE")) { promote = send; guideTitle = model.archivosParaEnviar === 1 ? "Archivo listo para enviar" : "Archivos listos para enviar"; guideCopy = "Se enviarán " + model.archivosParaEnviar + " archivos pendientes del listado que están seleccionados en tu equipo. Las demás fases requieren su acción explícita."; glyph = "upload"; }
                if (!blocking && !promote && chosen && !chosenIsRoot && chosen.estadoRecepcion === "PENDIENTE" && chosen.estado === "NO_INICIADO" && !chosen.tieneIntentoActivo && !model.mutacionIncierta) { promote = Array.prototype.filter.call(content.querySelectorAll('[data-doc111-action="reseleccionar"]'),function (b) { return b.doc111Value === chosen.recursoId; })[0]; guideTitle = "Vincula el archivo pendiente"; guideCopy = "Reselecciona el original. Se comprueban nombre, tamaño y fecha. La selección no lo envía y los ya recibidos se conservan."; }
                if (blocking) {
                    if (blocking.requiereConsulta) { promote = null; guideTitle = "Consulta el resultado de la operación solicitada"; guideCopy = "La solicitud no devolvió confirmación. Todavía no se ha consultado su estado posterior. Consulta el lote antes de decidir cómo continuar; la operación no se repetirá automáticamente."; glyph = "refresh"; }
                    else if (blocking.verificable) { guideTitle = "Verifica el resultado del intento pendiente"; guideCopy = "El servidor permite comprobar este intento. Verificar no repite la creación ni el almacenamiento; puede conservar el bloqueo si la evidencia no alcanza."; glyph = "refresh"; }
                    else if (blocking.retomable) { guideTitle = "Puedes retomar la creación pendiente"; guideCopy = "Se conservarán esta importación y los archivos recibidos. Al confirmar se volverán a comprobar las condiciones para crear el expediente." + (model.estado.detencionSolicitada === true ? " Se levantará la detención consultada." : ""); glyph = "folder"; }
                    else { promote = null; guideTitle = blocking.fase === "CREAR_EXPEDIENTE" ? "La creación del expediente sigue sin confirmación" : "Hay una operación sin resultado confirmado"; guideCopy = blocking.fueraDePagina ? "El servidor informa un intento activo o incierto fuera de esta página o sus filtros. Revisa las carpetas y archivos para localizarlo. Actualizar solo consulta, no reanuda el proceso." : "El servidor conserva un intento activo para este recurso. Todavía no permite verificarlo ni repetirlo. Actualizar estado solo consulta: si permanece igual, este intento requiere diagnóstico antes de continuar con los índices y el almacenamiento."; glyph = "info"; }
                    if (!promote && !blocking.requiereConsulta && !blocking.solicitando) { promote = button("Revisar bloqueo","ver-bloqueo",null,model.busy); }
                } else if (model.estado && model.estado.estado === "COMPLETADO") { guideTitle = "Importación completada"; guideCopy = "El servidor confirmó la finalización del lote. Puedes consultar el resultado de los archivos y descargar su seguimiento; no necesitas volver a enviarlos."; }
                else if (!promote && chosen && chosen.documentoId && ["ALMACENADO","COMPLETADO"].indexOf(chosen.estado) !== -1) { guideTitle = "El documento está guardado"; guideCopy = "Documento confirmado: " + chosen.documentoId + ". Consulta su expediente y el estado del lote. Los demás archivos conservan sus propias acciones pendientes."; }
                var guide = node("section",undefined,"action-panel doc111-next"), execution = node("div",undefined,"execution-message"); guide.setAttribute("aria-label","Siguiente acción disponible");
                add(execution,node("div",blocking && !blocking.retomable && !model.busy ? "Estado que impide continuar" : "Siguiente acción disponible","eyebrow"),node("h4",model.busy ? blocking && !blocking.solicitando ? "Consultando el estado registrado" : "Operación en curso" : guideTitle || (model.mutacionIncierta ? "Consulta el estado antes de repetir" : chosen && chosen.documentoId ? "El documento está guardado" : "Consulta los resultados y los requisitos pendientes")),node("p",model.busy ? model.mensaje || "Espera la respuesta del servidor. No repitas la operación." : guideCopy || "La consulta no confirma planes, recibe archivos, crea expedientes ni almacena documentos. Los efectos inciertos requieren verificar su resultado."));
                if (chosen) { phaseList(execution,chosen,chosenIsRoot,model); } add(guide,add(node("div",undefined,"action-glyph"),icon(glyph || "info")),execution);
                if (blocking) { guide.className += " warn"; var diagnostic = add(node("p",undefined,"current-folder"),node("strong","Referencia para revisar: "),node("span","lote " + model.loteId + (chosen ? " · " + (chosenIsRoot ? "carpeta " : "archivo ") + chosen.recursoId : "") + (chosen && chosen.intentoActivoId ? " · intento " + chosen.intentoActivoId : ""))); execution.appendChild(diagnostic); var lastOperation = model.ultimaOperacion; if (lastOperation && lastOperation.estado === "SIN_CONFIRMAR") { execution.appendChild(node("p",(lastOperation.codigo ? "Código: " + lastOperation.codigo : "Respuesta sin confirmar") + (lastOperation.operacionId ? " · operación " + lastOperation.operacionId : ""),"small muted")); } }
                else if (model.estado && model.estado.estado === "COMPLETADO") { guide.className += " complete"; }
                var selectedHeader = node("div",undefined,"table-head"), selectedCopy = add(node("div"),node("span",chosen ? (chosenIsRoot ? "Carpeta seleccionada" : "Archivo seleccionado") + " · referencia " + chosen.recursoId : "Lote " + model.loteId,"small muted"),node("h4",chosen ? chosen.nombre || chosen.rutaRelativa : "Resultados de la importación")), refreshControl = node("div",undefined,"query-control");
                add(refreshControl,statusButton,model.ultimaConsulta ? feedback : null); add(selectedHeader,selectedCopy,refreshControl); content.insertBefore(selectedHeader,content.querySelector(".doc111-filter-panel")); content.insertBefore(guide,content.querySelector(".doc111-filter-panel"));
                if (model.estado) { var statHost = node("div"); stats(statHost,[[model.estado.totalRaices,"carpetas originales"],[model.estado.totalArchivos,"archivos originales"],[bytes(model.estado.totalBytes),"tamaño original"]]); content.insertBefore(statHost.firstChild,guide.nextSibling); }
                if (promote) { promote.className = "primary doc111-primary"; footer.appendChild(promote); }
                else { statusButton.className = "primary doc111-primary"; footer.appendChild(statusButton); }
                if (send && promote !== send) { var sendOptions = disclosure(content,"envio","Opciones de envío manual de esta página",false);add(sendOptions,manualNote,send); }
                else if(manualNote&&selectedResource){content.appendChild(manualNote);}
                footer.insertBefore(node("p",blocking && !blocking.retomable && !blocking.verificable && !blocking.requiereConsulta ? "Este intento impide continuar. Revisar el bloqueo no repite la operación." : "Cada botón solicita una operación. No se encadenan fases."),footer.firstChild);
                var rootPanel = content.querySelector('[data-doc111-panel="RAICES"]'), filePanel = content.querySelector('[data-doc111-panel="ARCHIVOS"]'), rootTitle = Array.prototype.filter.call(content.querySelectorAll("h4"),function (h) { return h.textContent === "Carpetas del lote"; })[0], fileTable = Array.prototype.slice.call(content.querySelectorAll(".doc111-table")).filter(function (t) { return t.querySelector("th") && t.querySelector("th").textContent === "Archivo y raíz"; })[0];
                if (rootTitle) { var rootSection = node("section",undefined,"resource-section"), rootTable = rootTitle.nextElementSibling, rootPager = rootTable && rootTable.nextElementSibling; content.insertBefore(rootSection,rootTitle); add(rootSection,add(node("div",undefined,"table-head"),rootTitle),rootPanel && rootPanel.doc111FilterBadge,rootPanel,rootTable); if (rootPager && rootPager.classList.contains("doc111-pager")) { rootSection.appendChild(rootPager); } }
                if (fileTable) { var fileSection = node("section",undefined,"resource-section"), filePager = fileTable.nextElementSibling; content.insertBefore(fileSection,fileTable); add(fileSection,add(node("div",undefined,"table-head"),node("h4","Archivos del lote")),filePanel && filePanel.doc111FilterBadge,filePanel,fileTable); if (filePager && filePager.classList.contains("doc111-pager")) { fileSection.appendChild(filePager); } }
                coordinacionVista(model,guide);
            }
            Array.prototype.forEach.call(footer.querySelectorAll("button"), function (b) { if (["preparar", "confirmar", "confirmar-importar"].indexOf(b.getAttribute("data-doc111-action")) !== -1) { b.className = "primary doc111-primary"; } });
            summary.textContent = "";
            var configValues = [["Destino",model.contexto ? model.contexto.rutaCanonica : "Consulta una referencia autorizada"],["Perfil",profile ? (config.NombreVisible || "Perfil " + profile.PerfilId) + " · versión " + profile.NumeroVersion : "Configuración del lote confirmado"]];
            if (config) { configValues.push(["Gabinete",config.GabineteId],["Integridad",config.ModoIntegridad === "PARCIAL_PERMITIDO" ? "Se permiten resultados parciales" : config.AmbitoIntegridad === "LOTE" ? "Completa por lote" : "Completa por expediente"],["Clasificación",[config.FondoRef,config.InstrumentoRef,config.AreaRef,config.SerieRef,config.SubserieRef].filter(Boolean).join(" / ")]); }
            else if (model.estado) { configValues.push(["Integridad",[model.estado.modoIntegridad,model.estado.ambitoIntegridad].filter(Boolean).join(" / ") || "Definida por el perfil confirmado"]); }
            if(model.plan){configValues.push(['Referencia de revisión','plan '+model.plan.planId]);}
            var pairs = node("dl",undefined,"kv"); configValues.forEach(function (v) { add(pairs,node("dt",v[0]),node("dd",v[1] == null ? "No disponible" : v[1])); }); add(summary,pairs,node("p","Cerrar conserva la referencia del lote confirmado. Los archivos locales pendientes pueden requerir una nueva selección.","small muted"));
            organizeFooter(); scroll.scrollTop = oldScroll;
            if (queryModal.open && model.mensaje) { var error = queryBody.querySelector("[role=alert]"); if (!error) { error = node("p",undefined,"notice warn"); error.setAttribute("role","alert"); queryBody.appendChild(error); } error.textContent = model.mensaje; }
            if (restore && !queryModal.open) {
                var replacement = referenceFocused ? content.querySelector("[data-doc111-reference]:not(:disabled)") : focusFilter ? content.querySelector('[data-doc111-filtros="' + focusPanelKey + '"] [data-doc111-filter="' + focusFilter + '"]') : focusAction ? Array.prototype.filter.call(dialog.querySelectorAll("[data-doc111-action]"), function (b) { return b.getAttribute("data-doc111-action") === focusAction && JSON.stringify(b.doc111Value) === focusValue && !b.disabled; })[0] : content.querySelector("[data-doc111-profile]");
                (replacement && !replacement.disabled ? replacement : dialog).focus();
                if (!model.busy) { pendingFocus = null; }
            }
        }
        return {render:render, picker:picker, reselector:reselector, reselectorCarpeta:reselectorCarpeta, enfocar:function () { dialog.focus(); }, destruir:function () { dialog.removeEventListener("click", click); dialog.removeEventListener("change", change); dialog.removeEventListener("input", change); dialog.removeEventListener("keydown", key); if (queryModal.open) { queryModal.close(); } overlay.remove(); doc.body.style.overflow = previousOverflow; if (previousFocus && previousFocus.isConnected) { previousFocus.focus(); } }};
    }
    return {crear:crear};
}));
