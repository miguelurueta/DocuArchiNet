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
        function button(text, name, value, disabled) { var b = node("button", text); b.type = "button"; b.setAttribute("data-doc111-action", name); b.doc111Value = value; b.disabled = !!disabled; return b; }
        var overlay = node("div", undefined, "doc111-overlay"), dialog = node("section", undefined, "doc111-dialog");
        dialog.setAttribute("role", "dialog"); dialog.setAttribute("aria-modal", "true"); dialog.setAttribute("aria-labelledby", "doc111-title"); dialog.tabIndex = -1;
        var head = node("header", undefined, "doc111-head"), title = node("h2", "Importar carpetas"); title.id = "doc111-title";
        head.appendChild(title); head.appendChild(button("Cerrar", "cerrar")); dialog.appendChild(head);
        var destination = node("p", "", "doc111-destino"); dialog.appendChild(destination);
        var steps = node("ol", undefined, "doc111-steps"); steps.setAttribute("aria-label", "Etapas de importación");
        ["Perfil", "Carpetas", "Revisión", "Resultados"].forEach(function (text) { steps.appendChild(node("li", text)); }); dialog.appendChild(steps);
        var notice = node("p", "", "doc111-notice"); notice.setAttribute("role", "status"); notice.setAttribute("aria-live", "polite"); dialog.appendChild(notice);
        var layout = node("div", undefined, "doc111-layout"), content = node("div", undefined, "doc111-content"), summary = node("aside", undefined, "doc111-summary");
        summary.setAttribute("aria-label", "Configuración e inventario"); layout.appendChild(content); layout.appendChild(summary); dialog.appendChild(layout);
        var picker = node("input"); picker.type = "file"; picker.hidden = true; picker.setAttribute("aria-label", "Agregar carpeta"); dialog.appendChild(picker);
        var reselector = node("input"); reselector.type = "file"; reselector.hidden = true; reselector.setAttribute("aria-label", "Reseleccionar archivo pendiente"); dialog.appendChild(reselector);
        overlay.appendChild(dialog); host.appendChild(overlay);
        function click(event) { var target = event.target.closest("[data-doc111-action]"); if (target && dialog.contains(target) && !target.disabled) { action(target.getAttribute("data-doc111-action"), target.doc111Value); } }
        function change(event) { if (event.target.getAttribute("data-doc111-profile") === "1") { action("perfil", event.target.value); } }
        function key(event) {
            if (event.key === "Escape") { event.preventDefault(); action("cerrar"); }
            if (event.key === "Tab") {
                var controls = Array.prototype.filter.call(dialog.querySelectorAll("button,input,select,[tabindex]"), function (n) { return !n.disabled && !n.hidden && n.getClientRects().length > 0; });
                var first = controls[0], last = controls[controls.length - 1];
                if (event.shiftKey && (doc.activeElement === first || doc.activeElement === dialog)) { event.preventDefault(); if (last) { last.focus(); } }
                else if (!event.shiftKey && doc.activeElement === last) { event.preventDefault(); if (first) { first.focus(); } }
            }
        }
        dialog.addEventListener("click", click); dialog.addEventListener("change", change); dialog.addEventListener("keydown", key); dialog.focus();
        function table(labels, rows, cards) {
            var scroll = node("div", undefined, "doc111-table"), t = node("table"), tr = node("tr");
            if (cards) { scroll.className += " doc111-resultados"; }
            labels.forEach(function (label) { var th = node("th", label); th.scope = "col"; tr.appendChild(th); }); var thead = node("thead"); thead.appendChild(tr); t.appendChild(thead);
            var body = node("tbody"); rows.forEach(function (values) { var row = node("tr"); values.forEach(function (value, index) { var cell = node("td"); if (value && value.nodeType) { cell.appendChild(value); } else { cell.textContent = value == null ? "" : String(value); } if (cards) { cell.insertBefore(node("span", labels[index], "doc111-cell-label"), cell.firstChild); } row.appendChild(cell); }); body.appendChild(row); }); t.appendChild(body); scroll.appendChild(t); return scroll;
        }
        function pager(parent, page, total, size, name, busy) {
            var bar = node("div", undefined, "doc111-pager"); bar.appendChild(button("Anterior", name, page - 1, busy || page <= 1));
            bar.appendChild(node("span", total ? "Página " + page + " · " + total + (total === 1 ? " elemento" : " elementos") : "Sin elementos"));
            bar.appendChild(button("Siguiente", name, page + 1, busy || page * size >= total)); parent.appendChild(bar);
        }
        function render(model) {
            var focused = doc.activeElement, restore = content.contains(focused), focusAction = focused && focused.getAttribute("data-doc111-action"), focusValue = focused && focused.doc111Value;
            destination.textContent = "Destino: " + (model.contexto ? model.contexto.rutaCanonica : "Validando nivel seleccionado…");
            notice.textContent = model.mensaje || ""; notice.hidden = !model.mensaje; dialog.setAttribute("aria-busy", model.busy ? "true" : "false");
            Array.prototype.forEach.call(steps.children, function (step, index) { if (index === model.etapa) { step.setAttribute("aria-current", "step"); } else { step.removeAttribute("aria-current"); } });
            content.textContent = ""; summary.textContent = "";
            summary.appendChild(node("h3", "Configuración"));
            var profile = model.perfil, config = profile && profile.Configuracion;
            summary.appendChild(node("p", profile ? "Perfil " + profile.PerfilId + " · versión " + profile.NumeroVersion : "Selecciona una versión publicada."));
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
            summary.appendChild(node("p", inventory.raices.length + " carpetas · " + inventory.archivos.length + " archivos seleccionados"));
            if (model.loteId) { summary.appendChild(node("p", "Lote: " + model.loteId)); }
            summary.appendChild(node("p", "Cerrar conserva la referencia del lote confirmado. Los archivos locales pendientes pueden requerir una nueva selección."));
            if (model.etapa === 0) {
                content.appendChild(node("h3", "1. Perfil publicado")); var select = node("select"); select.setAttribute("data-doc111-profile", "1"); select.setAttribute("aria-label", "Perfil publicado"); select.disabled = model.busy;
                var empty = node("option", "Selecciona un perfil"); empty.value = ""; select.appendChild(empty);
                model.perfiles.forEach(function (p) { var option = node("option", (p.Configuracion.NombreVisible || "Perfil " + p.PerfilId) + " · versión " + p.NumeroVersion); option.value = p.VersionId; option.selected = profile && profile.VersionId === p.VersionId; select.appendChild(option); }); content.appendChild(select);
                if (model.masPerfiles) { content.appendChild(button("Más perfiles", "mas-perfiles", null, model.busy)); }
                content.appendChild(node("p", "El destino permanece fijado al nivel desde el que abriste esta ventana."));
                content.appendChild(button("Agregar carpetas", "etapa", 1, model.busy || !profile));
            } else if (model.etapa === 1) {
                content.appendChild(node("h3", "2. Carpetas y archivos")); content.appendChild(button("Agregar carpeta", "agregar", null, model.busy || !model.selectorDisponible));
                content.appendChild(node("p", "Agrega una carpeta cada vez. Las carpetas vacías no están representadas por el selector."));
                content.appendChild(table(["Carpeta", "Identidad", "Acción"], inventory.raices.slice((model.paginaRaices - 1) * 10, model.paginaRaices * 10).map(function (r) { return [r.nombre, r.idCliente, button("Quitar", "quitar-raiz", r.idCliente, model.busy)]; })));
                pager(content, model.paginaRaices, inventory.raices.length, 10, "pagina-raices", model.busy);
                content.appendChild(table(["Archivo", "Ruta relativa", "Bytes", "Acción"], inventory.archivos.slice((model.paginaArchivos - 1) * 20, model.paginaArchivos * 20).map(function (f) { return [f.nombre, f.rutaRelativa, f.bytes, button("Quitar", "quitar-archivo", f.idCliente, model.busy)]; })));
                pager(content, model.paginaArchivos, inventory.archivos.length, 20, "pagina-archivos", model.busy);
                content.appendChild(button("Perfil", "etapa", 0, model.busy)); content.appendChild(button("Preparar revisión", "preparar", null, model.busy || !inventory.archivos.length));
            } else if (model.etapa === 2) {
                content.appendChild(node("h3", "3. Revisión del servidor")); var result = model.plan && model.plan.resultado;
                if (result) {
                    content.appendChild(node("p", "Referencia del plan: " + model.plan.planId));
                    content.appendChild(node("p", "Unidades elegibles: " + result.UnidadesElegibles.length + " de " + inventory.raices.length + ". Total original: " + inventory.archivos.length + " archivos."));
                    content.appendChild(node("p", result.PuedeConfirmar ? "El servidor permite confirmar este plan." : "El plan contiene incidencias que impiden confirmarlo."));
                    var incidents = result.Incidencias || [];
                    content.appendChild(table(["Raíz / archivo", "Campo", "Incidencia"], incidents.slice((model.paginaIncidencias - 1) * 20, model.paginaIncidencias * 20).map(function (i) { return [identity(i.RaizIdCliente, i.ArchivoIdCliente), i.Campo, i.Codigo + ": " + i.Mensaje]; })));
                    pager(content, model.paginaIncidencias, incidents.length, 20, "pagina-incidencias", model.busy);
                    var values = result.Elementos || [];
                    content.appendChild(table(["Raíz / archivo", "Valores resueltos o diferidos"], values.slice((model.paginaValores - 1) * 10, model.paginaValores * 10).map(function (item) { return [identity(item.RaizIdCliente, item.ArchivoIdCliente), item.Valores.map(function (v) { return v.DestinoRef + ": " + (v.Diferido ? "Pendiente: " + v.PropiedadPendiente : v.Valor); }).join(" · ")]; })));
                    pager(content, model.paginaValores, values.length, 10, "pagina-valores", model.busy);
                }
                content.appendChild(button("Cambiar selección", "etapa", 1, model.busy || model.confirmacionIncierta));
                content.appendChild(button("Confirmar plan", "confirmar", null, model.busy || !model.confirmable || model.confirmacionIncierta));
            } else {
                content.appendChild(node("h3", "4. Resultados persistidos"));
                content.appendChild(node("p", "Lote " + model.loteId + (model.estado ? " · " + model.estado.estado : "")));
                content.appendChild(node("p", "La recepción de archivos es independiente del almacenamiento y de la incorporación documental. Cada acción avanza una fase y consulta su resultado."));
                if (model.estado) { content.appendChild(node("p", "Inventario original: " + model.estado.totalRaices + " carpetas · " + model.estado.totalArchivos + " archivos · " + model.estado.totalBytes + " bytes.")); }
                content.appendChild(button("Consultar estado", "consultar", null, model.busy));
                if (model.estado && model.estado.accionesPermitidas.indexOf("SOLICITAR_DETENCION") !== -1) { content.appendChild(button("Solicitar detención", "detener", null, model.busy || model.mutacionIncierta)); }
                function resourceActions(resource, raiz) {
                    var group = node("span"), labels = {CREAR_EXPEDIENTE:"Crear expediente", MATERIALIZAR_VALORES:"Preparar índices", ALMACENAR:"Almacenar archivo", RECONCILIAR:"Verificar intento"};
                    (model.accionesRecurso ? model.accionesRecurso(resource, raiz) : []).forEach(function (phase) {
                        group.appendChild(button(labels[phase] + " " + resource.recursoId, phase === "RECONCILIAR" ? "reconciliar" : "fase", {tipo:raiz ? "RAICES" : "ARCHIVOS", id:resource.recursoId, fase:phase}, model.busy));
                    }); return group;
                }
                if (model.raices) {
                    content.appendChild(node("h4", "Carpetas del lote"));
                    content.appendChild(table(["Carpeta", "Expediente", "Estado", "Acción"], model.raices.elementos.map(function (r) {
                        return [r.nombre + " · raíz " + r.recursoId, r.nombreExpediente || r.expedienteId || "Pendiente", r.estado, resourceActions(r, true)];
                    }), true));
                    pager(content, model.raices.pagina, model.raices.totalFiltrado, model.raices.tamanoPagina, "pagina-raices-resultado", model.busy);
                }
                if (model.resultados) {
                    var page = model.resultados;
                    content.appendChild(table(["Archivo y raíz", "Recepción", "Efecto documental", "Reselección", "Acción"], page.elementos.map(function (f) {
                        var pending = f.estadoRecepcion === "PENDIENTE" && f.estado === "NO_INICIADO" && !f.tieneIntentoActivo;
                        return [(f.rutaRelativa || f.nombre) + " · raíz " + f.raizId + " · archivo " + f.recursoId, f.estadoRecepcion, f.estado + (f.documentoId ? " · documento " + f.documentoId : ""), pending ? button("Reseleccionar archivo " + f.recursoId, "reseleccionar", f.recursoId, model.busy) : "No requiere reselección", resourceActions(f, false)];
                    }), true));
                    content.appendChild(node("p", "Elige explícitamente el archivo pendiente. Se comprueban nombre, tamaño y fecha; los ya recibidos o inciertos no se sustituyen."));
                    pager(content, page.pagina, page.totalFiltrado, page.tamanoPagina, "pagina-resultados", model.busy);
                    content.appendChild(button("Enviar pendientes de esta página", "enviar", null, model.busy || !model.puedeEnviar));
                    if (model.estado && model.estado.accionesPermitidas.indexOf("EXPORTAR_PAGINA") !== -1) { content.appendChild(button("Exportar esta página", "exportar", null, model.busy)); }
                }
            }
            if (restore) {
                var replacement = focusAction ? Array.prototype.filter.call(content.querySelectorAll("[data-doc111-action]"), function (b) { return b.getAttribute("data-doc111-action") === focusAction && b.doc111Value === focusValue && !b.disabled; })[0] : content.querySelector("[data-doc111-profile]");
                (replacement || dialog).focus();
            }
        }
        return {render:render, picker:picker, reselector:reselector, destruir:function () { dialog.removeEventListener("click", click); dialog.removeEventListener("change", change); dialog.removeEventListener("keydown", key); overlay.remove(); doc.body.style.overflow = previousOverflow; if (previousFocus && previousFocus.isConnected) { previousFocus.focus(); } }};
    }
    return {crear:crear};
}));
