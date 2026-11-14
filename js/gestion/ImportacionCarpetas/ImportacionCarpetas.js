(function (root, factory) {
    "use strict";
    if (typeof module === "object" && module.exports) { module.exports = factory({}); }
    else { root.ImportacionCarpetas = factory(root); }
}(typeof window !== "undefined" ? window : this, function (env) {
    "use strict";
    // Estado local de revisión. El servidor vuelve a autorizar y validar el plan.
    function crear(inventory) {
        var version = null, epoch = 0, pending = null, prepared = null, disposed = false;
        function invalidar() { epoch += 1; pending = null; prepared = null; }
        var unsubscribe = inventory.alCambiar(invalidar);
        function vigente(ticket) { return !disposed && ticket === pending && ticket.epoch === epoch && ticket.revision === inventory.revision() && ticket.versionId === version; }
        return {
            seleccionarPerfil:function (versionId) {
                if (disposed) { throw new Error("IMPORTACION_CERRADA"); }
                if (typeof versionId !== "string" || !/^[1-9][0-9]*$/.test(versionId)) { throw new Error("VERSION_INVALIDA"); }
                if (version !== versionId) { version = versionId; invalidar(); }
            },
            limpiarPerfil:function () { version = null; invalidar(); },
            iniciarPreparacion:function () {
                if (disposed || !version) { throw new Error("PERFIL_REQUERIDO"); }
                invalidar();
                pending = {epoch:epoch, revision:inventory.revision(), versionId:version, manifiesto:inventory.manifiesto()};
                return pending;
            },
            registrarPreparacion:function (ticket, result) {
                if (!vigente(ticket)) { return false; }
                if (!result || typeof result.planId !== "string" || !/^[1-9][0-9]*$/.test(result.planId) ||
                    typeof result.revision !== "number" || result.revision < 1 || result.revision > 4294967295 || Math.floor(result.revision) !== result.revision ||
                    typeof result.planHash !== "string" || !/^[a-f0-9]{64}$/.test(result.planHash) || !result.resultado || result.resultado.PuedeConfirmar !== true) {
                    prepared = null; return false;
                }
                prepared = {planId:result.planId, revisionEsperada:result.revision, planHashEsperado:result.planHash}; return true;
            },
            referenciaConfirmacion:function () {
                if (!prepared || !vigente(pending)) { throw new Error("PREPARACION_REQUERIDA"); }
                return {planId:prepared.planId, revisionEsperada:prepared.revisionEsperada, planHashEsperado:prepared.planHashEsperado};
            },
            vigente:vigente,
            destruir:function () { disposed = true; invalidar(); unsubscribe(); }
        };
    }
    // Solo pistas de consulta: IDs canonicos. Nunca token, File, nombre/ruta,
    // snapshot, hash o clave de mutacion. El servidor siempre reautoriza.
    function crearReferencias(storage, scope) {
        var name = typeof scope === "string" && /^[a-f0-9]{32}$/.test(scope) ? "DOC111_REF_" + scope : null, available = !!storage && !!name;
        function id(value) { return typeof value === "string" && /^[1-9][0-9]{0,9}$/.test(value) && Number(value) <= 4294967295; }
        function read() {
            if (!available) { return {}; }
            try {
                var raw = storage.getItem(name); if (!raw) { return {}; }
                if (raw.length > 256) { throw new Error(); }
                var value = JSON.parse(raw);
                if (!value || value.v !== 1 || Object.keys(value).some(function (key) { return ["v", "loteId", "planId"].indexOf(key) === -1; }) || (value.loteId !== undefined && !id(value.loteId)) || (value.planId !== undefined && !id(value.planId))) { throw new Error(); }
                return value;
            } catch (_) { try { storage.removeItem(name); } catch (ignore) { available = false; } return {}; }
        }
        function write(value) {
            if (!available) { return false; }
            var copy = {v:1}; if (id(value.loteId)) { copy.loteId = value.loteId; } if (id(value.planId)) { copy.planId = value.planId; }
            try { storage.setItem(name, JSON.stringify(copy)); return true; } catch (_) { available = false; return false; }
        }
        return {leer:read, guardar:write, disponible:function () { return available; }};
    }
    function validarReseleccion(expected, file) {
        if (!expected || !file || expected.estadoRecepcion !== "PENDIENTE" || expected.estado !== "NO_INICIADO" || expected.tieneIntentoActivo ||
            typeof expected.recursoId !== "string" || !/^[1-9][0-9]*$/.test(expected.recursoId) || typeof expected.raizId !== "string" || !/^[1-9][0-9]*$/.test(expected.raizId) ||
            typeof file.name !== "string" || file.name !== expected.nombre || !Number.isSafeInteger(file.size) || file.size < 0 || String(file.size) !== expected.bytesPrevistos || !Number.isSafeInteger(file.lastModified) || file.lastModified < 0 || String(file.lastModified) !== expected.ultimaModificacionLocal) {
            throw new Error("RESELECCION_NO_COINCIDE");
        }
        return file;
    }
    function accionesRecurso(model, resource, raiz) {
        if (!model || !model.estado || !resource || model.busy || model.mutacionIncierta) { return []; }
        var allowed = model.estado.accionesPermitidas || [], actions = [];
        if (allowed.indexOf("SOLICITAR_VERIFICACION") !== -1 && resource.estado === "INCIERTO" && /^[1-9][0-9]*$/.test(resource.intentoActivoId || "")) { actions.push("RECONCILIAR"); }
        if (allowed.indexOf("SOLICITAR_AVANCE") === -1 || resource.tieneIntentoActivo) { return actions; }
        if (raiz) {
            if (resource.estado === "PENDIENTE" && !resource.expedienteId && resource.recepcionListaParaCrear === true) { actions.push("CREAR_EXPEDIENTE"); }
        } else {
            var parent = model.raices && model.raices.elementos.filter(function (r) { return r.recursoId === resource.raizId; })[0];
            if (parent && parent.estado === "EXPEDIENTE_CONFIRMADO" && !parent.tieneIntentoActivo && resource.estadoRecepcion === "RECIBIDO" && resource.estado === "NO_INICIADO") {
                actions.push(resource.valoresEfectivosFijados === true ? "ALMACENAR" : "MATERIALIZAR_VALORES");
            }
        }
        return actions;
    }
    function rotularAccesoMenu(button, text, icon) {
        var label = button.querySelector("[data-doc120-menu-label]");
        if (!label) {
            var glyph = button.ownerDocument.createElement("i");
            glyph.className = "fas " + icon + " doc120-menu-icon"; glyph.setAttribute("aria-hidden", "true");
            label = button.ownerDocument.createElement("span"); label.setAttribute("data-doc120-menu-label", "");
            button.textContent = ""; button.appendChild(glyph); button.appendChild(label);
        }
        label.textContent = text;
    }
    function asegurarMenu() {
        var doc = env.document, bar = doc && doc.querySelector("#menu_var .navbar-nav");
        if (!bar) { return null; }
        var existing = doc.getElementById("doc110-navigation");
        if (existing) { return existing.querySelector(".dropdown-menu"); }
        var item = doc.createElement("li"), toggle = doc.createElement("a"), options = doc.createElement("div");
        item.id = "doc110-navigation"; item.className = "nav-item dropdown active ml-2 mr-0 active_";
        toggle.id = "doc110-navigation-toggle"; toggle.href = "#"; toggle.className = "nav-link dropdown-toggle";
        rotularAccesoMenu(toggle, "Importación", "fa-file-import"); toggle.setAttribute("data-toggle", "dropdown"); toggle.setAttribute("aria-haspopup", "true"); toggle.setAttribute("aria-expanded", "false");
        options.className = "dropdown-menu"; options.setAttribute("aria-labelledby", toggle.id);
        item.appendChild(toggle); item.appendChild(options); bar.appendChild(item);
        if (!(env.jQuery && env.jQuery.fn && env.jQuery.fn.dropdown)) { toggle.addEventListener("click", function (event) { event.preventDefault(); var expanded = item.classList.toggle("show"); options.classList.toggle("show", expanded); toggle.setAttribute("aria-expanded", String(expanded)); }); }
        options.addEventListener("click", function (event) { if (event.target.closest("button")) { item.classList.remove("show"); options.classList.remove("show"); toggle.setAttribute("aria-expanded", "false"); toggle.focus(); } }, true);
        return options;
    }
    function montar(host, dependencies) {
        var incomingScope = host.getAttribute("data-recuperacion");
        if (host.doc111Instancia && host.doc111Scope === incomingScope) { host.doc111Instancia.sincronizar(); return host.doc111Instancia; }
        if (host.doc111Instancia) { host.doc111Instancia.destruir(); }
        if (host.doc111Scope && host.doc111Scope !== incomingScope) { delete host.doc111UltimoLote; delete host.doc111ConfirmacionPendiente; }
        host.doc111Scope = incomingScope;
        var deps = dependencies || env, active = null, opening = false, errorApertura = "", destroyed = false, verificationApi = null, storage = null;
        var openButton = host.querySelector("[data-doc111-open]"), resumeButton = host.querySelector("[data-doc111-resume]"), status = host.querySelector("[data-doc111-status]");
        var verifyButton = host.querySelector("[data-doc111-verificar]"), manualButton = host.querySelector("[data-doc111-consultar]"), manualInput = host.querySelector("[data-doc111-lote]");
        var modern = host.getAttribute("data-navegacion") === "menu", queryOpen = null, navigation = null;
        if (modern) {
            navigation = asegurarMenu();
            if (navigation) {
                var section = host.ownerDocument.createElement("div"); section.setAttribute("data-doc119-operacion", "1");
                section.className = "doc119-operation-menu";
                [openButton, resumeButton, verifyButton].forEach(function (button) { if (button) { button.className = "dropdown-item"; section.appendChild(button); } });
                openButton.hidden = false; rotularAccesoMenu(openButton, "Importar carpetas…", "fa-folder-open");
                rotularAccesoMenu(resumeButton, resumeButton.textContent, "fa-history");
                if (verifyButton) { rotularAccesoMenu(verifyButton, verifyButton.textContent, "fa-check-circle"); }
                queryOpen = host.ownerDocument.createElement("button"); queryOpen.type = "button"; queryOpen.className = "dropdown-item"; rotularAccesoMenu(queryOpen, "Consultar importación…", "fa-search"); queryOpen.setAttribute("data-doc111-query-open", "");
                section.insertBefore(queryOpen, resumeButton); navigation.insertBefore(section, navigation.firstChild);
            }
        }
        try { storage = deps.sessionStorage; } catch (_) {}
        var references = crearReferencias(storage, host.getAttribute("data-recuperacion")), saved = references.leer();
        if (saved.loteId) { host.doc111UltimoLote = {loteId:saved.loteId}; }
        if (saved.planId) { host.doc111ConfirmacionPendiente = {planId:saved.planId}; }
        function save() { return references.guardar({loteId:host.doc111UltimoLote && host.doc111UltimoLote.loteId, planId:host.doc111ConfirmacionPendiente && host.doc111ConfirmacionPendiente.planId}); }
        function validId(id) { return typeof id === "string" && /^[1-9][0-9]*$/.test(id); }
        // Solo forma de los datos que consume la vista; reglas y elegibilidad son del servidor.
        function object(value) { return !!value && typeof value === "object" && !Array.isArray(value); }
        function optionalText(value) { return value == null || typeof value === "string"; }
        function revision(value) { return typeof value === "number" && Math.floor(value) === value && value > 0 && value <= 4294967295; }
        function profile(value) {
            if (!object(value) || !validId(value.PerfilId) || !validId(value.VersionId) || !object(value.Configuracion)) { return false; }
            var config = value.Configuracion;
            return (value.NumeroVersion == null || revision(value.NumeroVersion)) &&
                (config.GabineteId == null || revision(config.GabineteId)) &&
                ["NombreVisible", "ModoIntegridad", "AmbitoIntegridad", "FondoRef", "InstrumentoRef", "AreaRef", "SerieRef", "SubserieRef"].every(function (field) { return optionalText(config[field]); });
        }
        function profilePage(value) { return object(value) && revision(value.pagina) && typeof value.puedeHaberMas === "boolean" && Array.isArray(value.perfiles) && value.perfiles.every(profile); }
        function valueForView(value) {
            return object(value) && typeof value.DestinoRef === "string" && typeof value.Diferido === "boolean" && optionalText(value.Valor) &&
                (value.Diferido ? typeof value.PropiedadPendiente === "string" : optionalText(value.PropiedadPendiente));
        }
        function preparation(value, level) {
            if (!object(value) || !validId(value.planId) || !revision(value.revision) || typeof value.planHash !== "string" || !/^[a-f0-9]{64}$/.test(value.planHash) || value.nivelRef !== level || !object(value.resultado)) { return false; }
            var result = value.resultado;
            return typeof result.PuedeConfirmar === "boolean" && Array.isArray(result.UnidadesElegibles) && result.UnidadesElegibles.every(function (id) { return typeof id === "string"; }) &&
                Array.isArray(result.Incidencias) && result.Incidencias.every(function (item) { return object(item) && optionalText(item.RaizIdCliente) && optionalText(item.ArchivoIdCliente) && optionalText(item.Campo) && typeof item.Codigo === "string" && typeof item.Mensaje === "string"; }) &&
                Array.isArray(result.Elementos) && result.Elementos.every(function (item) { return object(item) && optionalText(item.RaizIdCliente) && optionalText(item.ArchivoIdCliente) && Array.isArray(item.Valores) && item.Valores.every(valueForView); });
        }
        function key() {
            var bytes = new Uint8Array(16); if (!deps.crypto || !deps.crypto.getRandomValues) { throw new Error("NAVEGADOR_NO_COMPATIBLE"); }
            deps.crypto.getRandomValues(bytes); return "web_" + Array.prototype.map.call(bytes, function (b) { return ("0" + b.toString(16)).slice(-2); }).join("");
        }
        function sync() {
            if (destroyed) { return; }
            if (modern && queryOpen) { var currentMenu = asegurarMenu(); if (currentMenu && !currentMenu.contains(section)) { currentMenu.insertBefore(section, currentMenu.firstChild); navigation = currentMenu; } }
            openButton.disabled = opening || !!host.doc111ConfirmacionPendiente || !validId(host.getAttribute("data-nivel"));
            resumeButton.hidden = !host.doc111UltimoLote;
            if (host.doc111UltimoLote) { if (modern) { rotularAccesoMenu(resumeButton, "Consultar lote " + host.doc111UltimoLote.loteId, "fa-history"); } else { resumeButton.textContent = "Consultar lote " + host.doc111UltimoLote.loteId; } }
            if (verifyButton) { verifyButton.hidden = !host.doc111ConfirmacionPendiente; verifyButton.disabled = opening; }
            status.textContent = host.doc111ConfirmacionPendiente ? "La confirmación del plan " + host.doc111ConfirmacionPendiente.planId + " requiere verificar su resultado antes de iniciar otra preparación. " + errorApertura : (opening ? "Validando el destino de la importación…" : errorApertura);
            var storageWarning = !references.disponible() && (host.doc111UltimoLote || host.doc111ConfirmacionPendiente);
            if (storageWarning) { status.textContent += " El navegador no permite guardar esta referencia; anota el lote o plan antes de recargar."; }
            if (modern) {
                status.setAttribute("data-doc111-state", opening && !host.doc111ConfirmacionPendiente && !errorApertura && !storageWarning ? "loading" : "notice");
                status.hidden = !status.textContent;
                var menu = host.ownerDocument.getElementById("doc110-navigation");
                if (menu) { menu.setAttribute("aria-busy", opening ? "true" : "false"); }
                if (queryOpen) { queryOpen.disabled = opening; rotularAccesoMenu(queryOpen, host.doc111ConfirmacionPendiente ? "Consultar importación… · verificación pendiente" : "Consultar importación…", "fa-search"); }
            }
        }
        function message(error) {
            if (error && error.planPendiente) { return "No se pudo confirmar el resultado del plan " + error.planPendiente + ". Conserva esta referencia para su verificación. No se repetirá la confirmación automáticamente."; }
            if (error && error.resultadoIncierto) { return "No se pudo confirmar el resultado. Consulta el estado antes de volver a enviar."; }
            var texts = {SESION_REQUERIDA:"La sesión expiró. Recarga la página.", ACCESO_DENEGADO:"No tienes autorización vigente para esta operación.", CAPACIDAD_NO_DISPONIBLE:"La importación no está disponible con la configuración actual.", SOLICITUD_NO_VERIFICADA:"Recarga la página para renovar el contexto de la operación.", SELECCION_DIRECTORIOS_NO_DISPONIBLE:"Este navegador no permite seleccionar carpetas.", CONFLICTO_REVISION:"Los datos cambiaron. Consulta el estado o prepara una nueva revisión.", RESPUESTA_INVALIDA:"La respuesta del servidor no es válida. Revisa la selección o vuelve a preparar."};
            if (error && error.message === "RESELECCION_NO_COINCIDE") { return "El archivo no coincide con el nombre, tamaño y fecha del archivo pendiente seleccionado. El lote permanece sin cambios."; }
            return texts[error && (error.codigo || error.message)] || "No se pudo completar la solicitud. Revisa la selección y consulta el estado antes de repetir.";
        }
        function refresh(session) {
            if (active !== session) { return; }
            var model = session.model; model.manifiesto = model.consulta ? {raices:[], archivos:[]} : session.inventory.manifiesto();
            model.ultimoLoteId = host.doc111UltimoLote && host.doc111UltimoLote.loteId; model.planPendienteId = host.doc111ConfirmacionPendiente && host.doc111ConfirmacionPendiente.planId;
            model.paginaRaices = Math.min(model.paginaRaices, Math.max(1, Math.ceil(model.manifiesto.raices.length / 10)));
            model.paginaArchivos = Math.min(model.paginaArchivos, Math.max(1, Math.ceil(model.manifiesto.archivos.length / 20)));
            var files = model.resultados ? model.resultados.elementos : [];
            model.accionesRecurso = function (resource, raiz) { return accionesRecurso(model, resource, raiz); };
            model.archivosParaEnviar = model.mutacionIncierta ? 0 : files.filter(function (f) { return f.estadoRecepcion === "PENDIENTE" && f.estado === "NO_INICIADO" && !f.tieneIntentoActivo && (session.reselected[f.recursoId] || (!model.consulta && session.inventory.obtenerFile(f.idCliente))); }).length;
            model.puedeEnviar = model.archivosParaEnviar > 0;
            session.view.render(model);
        }
        function run(session, work) {
            if (active !== session || session.model.busy) { return Promise.resolve(); }
            session.model.busy = true; session.model.mensaje = ""; refresh(session);
            return Promise.resolve().then(work).catch(function (error) {
                if (active === session) { session.model.mensaje = message(error); }
            }).then(function () { if (active === session) { session.model.busy = false; refresh(session); } });
        }
        function solicitudPagina(session, type, page) {
            var model = session.model, filters = (type === "RAICES" ? model.filtrosRaices : model.filtrosArchivos) || {};
            var request = {loteId:model.loteId, tipo:type, pagina:page || 1, tamanoPagina:model.tamanoResultados};
            (type === "RAICES" ? ["estado"] : ["raizId", "estado", "estadoRecepcion"]).forEach(function (key) {
                if (typeof filters[key] === "string" && filters[key].length) { request[key] = filters[key]; }
            });
            return request;
        }
        function consultar(session, page) {
            if (active !== session) { return Promise.resolve(); }
            session.model.ultimaConsulta = null;
            function consultaCompleta() { session.model.ultimaConsulta = Date.now(); session.model.consultasCompletadas = (session.model.consultasCompletadas || 0) + 1; }
            session.consultaArchivos = null; session.model.resultados = null; session.model.raices = null;
            return session.api.invocar("ConsultarLote", {loteId:session.model.loteId}).then(function (state) {
                if (active !== session) { return; }
                if (!state || state.loteId !== session.model.loteId || !validId(state.nivelRef) || !Array.isArray(state.accionesPermitidas) || (session.model.contexto && state.nivelRef !== session.model.contexto.nivelRef)) { throw new Error("RESPUESTA_INVALIDA"); }
                session.model.estado = state; session.model.contexto = {nivelRef:state.nivelRef, rutaCanonica:state.rutaCanonica};
                host.doc111UltimoLote = {loteId:state.loteId}; save();
                session.model.tamanoResultados = Math.min(25, state.maximoTamanoPagina || 1);
                var request = solicitudPagina(session, "ARCHIVOS", page);
                return session.api.invocar("ConsultarRecursosLote", request).then(function (result) {
                    if (active === session && result && result.loteId === session.model.loteId && Array.isArray(result.elementos)) { session.consultaArchivos = request; }
                    return result;
                });
            }).then(function (result) {
                if (active !== session || !result) { return; }
                if (result.loteId !== session.model.loteId || !Array.isArray(result.elementos)) { throw new Error("RESPUESTA_INVALIDA"); }
                session.model.resultados = result; session.model.raices = null;
                if (session.model.estado.accionesPermitidas.indexOf("CONSULTAR_RAICES") === -1) { session.model.mutacionIncierta = false; consultaCompleta(); return; }
                return session.api.invocar("ConsultarRecursosLote", solicitudPagina(session, "RAICES", session.model.paginaRaicesResultado)).then(function (roots) {
                    if (active !== session) { return; }
                    if (!roots || roots.loteId !== session.model.loteId || roots.tipo !== "RAICES" || !Array.isArray(roots.elementos)) { throw new Error("RESPUESTA_INVALIDA"); }
                    session.model.raices = roots; session.model.mutacionIncierta = false; consultaCompleta();
                    if (typeof deps.Doc111ActualizarRama === "function") {
                        try { deps.Doc111ActualizarRama({nivelRef:session.model.contexto.nivelRef, referenciaContexto:session.scope}, roots.elementos); } catch (_) { /* La vista del arbol no invalida resultados persistidos. */ }
                    }
                });
            });
        }
        function cerrar(force) {
            var session = active; if (!session) { return; }
            var preparation = session.importacionGuardada ? session.importacionGuardada.model : session.model;
            if (!force && (session.confirming || preparation.confirmacionIncierta) && !deps.confirm("La confirmación puede seguir en el servidor. Cerrar liberará los archivos locales y conservará la referencia del plan para verificarla. ¿Cerrar?")) { return; }
            if (!force && !preparation.loteId && !session.confirming && !preparation.confirmacionIncierta && session.inventory.manifiesto().archivos.length && !deps.confirm("Se descartará esta preparación sin confirmar. ¿Cerrar?")) { return; }
            if (session.confirming) { host.doc111ConfirmacionPendiente = session.confirming; }
            save(); active = null; opening = false; session.api.cerrar(); session.picker.destruir(); session.unsubscribe(); session.operation.destruir(); session.view.destruir();
            session.urls.forEach(function (url) { deps.URL.revokeObjectURL(url); }); session.timers.forEach(function (id) { deps.clearTimeout(id); }); sync();
            if (!force && session.returnFocus && session.returnFocus.isConnected && typeof session.returnFocus.focus === "function") { session.returnFocus.focus(); }
        }
        function action(session, name, value) {
            var model = session.model;
            if (name === "cerrar") { cerrar(); return; }
            if (active !== session || model.busy) { return; }
            if (name === "volver-importacion") {
                if (!session.importacionGuardada) { return; }
                var saved = session.importacionGuardada; session.importacionGuardada = null; session.model = saved.model; session.reselected = saved.reselected; session.consultaArchivos = saved.consultaArchivos; session.expected = null;
                if (session.model.loteId) { run(session, function () { return consultar(session, session.model.resultados ? session.model.resultados.pagina : 1); }); }
                else { refresh(session); }
                return;
            }
            if (name === "consultar-referencia" || name === "consultar-ultimo") {
                var reference = name === "consultar-ultimo" ? model.ultimoLoteId : String(value || "").trim();
                if (!validId(reference)) { model.mensaje = "Escribe una referencia de lote válida."; refresh(session); return; }
                if (!model.consulta) {
                    session.importacionGuardada = {model:model, reselected:session.reselected, consultaArchivos:session.consultaArchivos};
                    session.model = Object.assign({}, model, {consulta:true, perfil:null, plan:null, confirmable:false, puedeVolverImportacion:true}); model = session.model;
                }
                model.referencia = reference; model.loteId = reference; model.contexto = null; model.estado = null; model.resultados = null; model.raices = null; model.filtrosArchivos = {}; model.filtrosRaices = {}; model.etapa = 3; model.mutacionIncierta = false; session.reselected = Object.create(null);
                run(session, function () { return consultar(session, 1); }); return;
            }
            if (name === "verificar-referencia") { cerrar(true); verify(); return; }
            if (name === "etapa" && !model.confirmacionIncierta) { model.etapa = value; refresh(session); return; }
            if (name === "perfil") {
                if (!validId(value)) { session.operation.limpiarPerfil(); model.perfil = null; }
                else { session.operation.seleccionarPerfil(value); model.perfil = model.perfiles.filter(function (p) { return p.VersionId === value; })[0]; }
                model.plan = null; model.confirmable = false; refresh(session); return;
            }
            if (name === "agregar") { session.picker.abrir(); return; }
            if (name === "reseleccionar") {
                session.expected = model.resultados.elementos.filter(function (f) { return f.recursoId === value; })[0];
                if (!session.expected || session.expected.estadoRecepcion !== "PENDIENTE" || session.expected.estado !== "NO_INICIADO" || session.expected.tieneIntentoActivo) { return; }
                session.view.reselector.value = ""; session.view.reselector.click(); return;
            }
            if (name === "quitar-raiz" || name === "quitar-archivo") { session.inventory[name === "quitar-raiz" ? "quitarRaiz" : "quitarArchivo"](value); return; }
            var pagers = {"pagina-raices":"paginaRaices", "pagina-archivos":"paginaArchivos", "pagina-incidencias":"paginaIncidencias", "pagina-valores":"paginaValores"};
            if (pagers[name]) { model[pagers[name]] = value; refresh(session); return; }
            if (name === "mas-perfiles") {
                run(session, function () { return session.api.invocar("ListarPerfilesImportacion", {nivelRef:model.contexto.nivelRef, pagina:model.paginaPerfiles + 1, tamanoPagina:50}).then(function (page) {
                    if (active !== session) { return; }
                    if (!profilePage(page) || page.pagina !== model.paginaPerfiles + 1) { throw new Error("RESPUESTA_INVALIDA"); }
                    model.paginaPerfiles = page.pagina; model.masPerfiles = page.puedeHaberMas;
                    page.perfiles.forEach(function (p) { if (!model.perfiles.some(function (old) { return old.VersionId === p.VersionId; })) { model.perfiles.push(p); } });
                }); }); return;
            }
            if (name === "preparar") {
                run(session, function () {
                    model.plan = null; model.confirmable = false;
                    var ticket = session.operation.iniciarPreparacion(), request = ticket.manifiesto;
                    request.nivelRef = model.contexto.nivelRef; request.versionId = ticket.versionId;
                    return session.api.invocar("PrepararPlan", request).then(function (plan) {
                        if (active !== session || !session.operation.vigente(ticket)) { return; }
                        if (!preparation(plan, model.contexto.nivelRef)) { throw new Error("RESPUESTA_INVALIDA"); }
                        model.confirmable = session.operation.registrarPreparacion(ticket, plan); model.plan = plan; model.etapa = 2; model.paginaIncidencias = model.paginaValores = 1;
                    });
                }); return;
            }
            if (name === "confirmar") {
                if (!model.confirmable || model.confirmacionIncierta) { return; }
                run(session, function () {
                    var request = session.operation.referenciaConfirmacion(); request.claveConfirmacion = key();
                    var expectedPlan = request.planId, expectedLevel = model.contexto.nivelRef; session.confirming = request;
                    host.doc111ConfirmacionPendiente = {planId:expectedPlan}; save();
                    return session.api.invocar("ConfirmarPlan", request).then(function (result) {
                        if (!result || !validId(result.loteId) || result.planId !== expectedPlan || result.nivelRef !== expectedLevel) { var bad = new Error("RESPUESTA_INVALIDA"); bad.resultadoIncierto = true; throw bad; }
                        host.doc111UltimoLote = {loteId:result.loteId, nivelRef:result.nivelRef}; host.doc111ConfirmacionPendiente = null; session.confirming = null; save(); sync();
                        if (active !== session) { return; } model.loteId = result.loteId; model.etapa = 3; return consultar(session, 1);
                    }).catch(function (error) {
                        if (session.confirming && error.resultadoIncierto) { host.doc111ConfirmacionPendiente = session.confirming; model.confirmacionIncierta = true; error.planPendiente = session.confirming.planId; }
                        else if (session.confirming) { host.doc111ConfirmacionPendiente = null; }
                        save(); sync();
                        session.confirming = null; throw error;
                    });
                }); return;
            }
            if (name === "consultar" || name === "pagina-resultados") { run(session, function () { return consultar(session, name === "consultar" ? (model.resultados ? model.resultados.pagina : 1) : value); }); return; }
            if (["filtrar-resultados", "limpiar-resultados", "filtrar-raices", "limpiar-raices"].indexOf(name) !== -1) {
                var rootsFilter = name === "filtrar-raices" || name === "limpiar-raices", filters = {}, currentPage = model.resultados ? model.resultados.pagina : 1;
                if (name.indexOf("filtrar-") === 0 && value && typeof value === "object") {
                    (rootsFilter ? ["estado"] : ["raizId", "estado", "estadoRecepcion"]).forEach(function (key) {
                        if (typeof value[key] === "string" && value[key].trim().length) { filters[key] = value[key].trim(); }
                    });
                }
                model[rootsFilter ? "filtrosRaices" : "filtrosArchivos"] = filters;
                if (rootsFilter) { model.paginaRaicesResultado = 1; }
                run(session, function () { return consultar(session, rootsFilter ? currentPage : 1); }); return;
            }
            if (name === "pagina-raices-resultado") { model.paginaRaicesResultado = value; run(session, function () { return consultar(session, model.resultados.pagina); }); return; }
            if (name === "fase" || name === "reconciliar" || name === "detener") {
                var row, phase, kind;
                if (name === "detener") {
                    if (!model.estado || model.estado.accionesPermitidas.indexOf("SOLICITAR_DETENCION") === -1 || model.mutacionIncierta) { return; }
                } else {
                    if (!value || !["RAICES", "ARCHIVOS"].some(function (type) { return type === value.tipo; })) { return; }
                    kind = value.tipo === "RAICES"; phase = name === "reconciliar" ? "RECONCILIAR" : value.fase;
                    var resourcePage = kind ? model.raices : model.resultados;
                    row = resourcePage && resourcePage.elementos.filter(function (r) { return r.recursoId === value.id; })[0];
                    if (accionesRecurso(model, row, kind).indexOf(phase) === -1) { return; }
                }
                run(session, function () {
                    var operation = name === "detener" ? "SolicitarDetencion" : name === "reconciliar" ? "ReconciliarRecurso" : "AvanzarLote";
                    var request = {loteId:model.loteId, revisionEsperada:name === "fase" ? row.revision : model.estado.revision};
                    if (name === "fase") { request.recursoId = row.recursoId; request.fase = phase; request.claveOperacion = key(); }
                    if (name === "reconciliar") { request.intentoId = row.intentoActivoId; }
                    return session.api.invocar(operation, request).then(function (result) {
                        if (active !== session) { return; }
                        if (!result || result.loteId !== model.loteId || (name === "fase" && (result.fase !== phase || result.estado !== "CONFIRMADO" || (kind ? result.raizId : result.archivoId) !== row.recursoId)) || (name === "reconciliar" && (result.intentoId !== row.intentoActivoId || result.resuelta !== true || result.estado !== "CONFIRMADO")) || (name === "detener" && result.detencionSolicitada !== true && ["COMPLETADO", "FALLIDO"].indexOf(result.estado) === -1)) {
                            var invalid = new Error("RESPUESTA_INVALIDA"); invalid.resultadoIncierto = true; throw invalid;
                        }
                        return consultar(session, model.resultados.pagina);
                    }).catch(function (error) { model.mutacionIncierta = true; throw error; });
                }); return;
            }
            if (name === "enviar") {
                if (!model.puedeEnviar) { return; }
                run(session, function () {
                    var items = model.resultados.elementos.filter(function (f) { return f.estadoRecepcion === "PENDIENTE" && f.estado === "NO_INICIADO" && !f.tieneIntentoActivo && (session.reselected[f.recursoId] || (!model.consulta && session.inventory.obtenerFile(f.idCliente))); }).map(function (f) {
                        var file = session.reselected[f.recursoId] || (!model.consulta && session.inventory.obtenerFile(f.idCliente));
                        if (session.reselected[f.recursoId]) { validarReseleccion(f, file); }
                        if (file.name !== f.nombre || String(file.size) !== f.bytesPrevistos) { throw new Error("CONTENIDO_DISTINTO"); }
                        return {file:file, solicitud:{loteId:model.loteId, archivoId:f.recursoId, claveIntento:key()}};
                    });
                    return session.api.enviarCola(items, function (item, progress) { if (active === session) { model.mensaje = "Enviando archivo " + (items.indexOf(item) + 1) + " de " + items.length + ": " + item.file.name + ". Transferencia: " + progress.enviados + " de " + progress.total + " bytes de solicitud enviados. La recepción no implica almacenamiento documental."; refresh(session); } }).then(function () { return consultar(session, model.resultados.pagina); }).then(function () { if (active === session) { model.mensaje = "Recepción confirmada para " + items.length + " archivos. Continúa con las acciones disponibles; todavía no implica almacenamiento documental."; } }).catch(function (error) { model.mutacionIncierta = true; throw error; });
                }); return;
            }
            if (name === "exportar") {
                if (!model.resultados || !session.consultaArchivos) { return; }
                run(session, function () { return session.api.invocar("ExportarPaginaLote", Object.assign({}, session.consultaArchivos)).then(function (result) {
                    if (active !== session || !result || typeof result.contenido !== "string") { return; }
                    var url = deps.URL.createObjectURL(new deps.Blob([result.contenido], {type:"text/csv;charset=utf-8"})); session.urls.push(url);
                    var anchor = host.ownerDocument.createElement("a"); anchor.href = url; anchor.download = "importacion-" + model.loteId + "-pagina-" + model.resultados.pagina + ".csv"; host.appendChild(anchor); anchor.click(); anchor.remove();
                    session.timers.push(deps.setTimeout(function () { deps.URL.revokeObjectURL(url); session.urls = session.urls.filter(function (item) { return item !== url; }); }, 1000));
                }); });
            }
        }
        function abrir(resume, explicitLevel) {
            if (active || opening) { if (active && active.view.enfocar) { active.view.enfocar(); } return; }
            var requestedLevel = explicitLevel === undefined ? host.getAttribute("data-nivel") : explicitLevel;
            if (!resume && (host.doc111ConfirmacionPendiente || !validId(requestedLevel))) { errorApertura = host.doc111ConfirmacionPendiente ? "Verifica la confirmación pendiente antes de importar." : "Selecciona un nivel para importar carpetas."; sync(); return; }
            var returnFocus = host.ownerDocument && host.ownerDocument.activeElement; errorApertura = ""; opening = true; sync();
            var session = {inventory:deps.InventarioImportacion.crear(), reselected:Object.create(null), urls:[], timers:[], confirming:null, scope:host.getAttribute("data-recuperacion"), returnFocus:returnFocus};
            try { session.api = deps.ImportacionCarpetasApi.crear({url:host.getAttribute("data-api"), uploadUrl:host.getAttribute("data-upload"), token:host.getAttribute("data-token")}); }
            catch (error) { opening = false; errorApertura = message(error); sync(); return; }
            session.operation = crear(session.inventory);
            session.model = {etapa:resume ? 3 : 0, busy:false, contexto:null, perfiles:[], perfil:null, paginaPerfiles:1, masPerfiles:false, paginaRaices:1, paginaArchivos:1, paginaIncidencias:1, paginaValores:1, confirmable:false, loteId:resume ? resume.loteId : null, manifiesto:{raices:[], archivos:[]}};
            session.model.consulta = !!(modern && resume); session.model.referencia = resume && resume.loteId || "";
            if (resume && resume.consulta) { session.model.etapa = -1; session.model.consulta = true; }
            // La ventana solo se monta tras resolver el destino autorizado.
            active = session; session.view = {render:function () {}, destruir:function () {}}; session.picker = {destruir:function () {}}; session.unsubscribe = function () {};
            run(session, function () {
                if (resume) { return resume.consulta ? Promise.resolve() : consultar(session, 1); }
                return session.api.invocar("AbrirImportacion", {nivelRef:requestedLevel}).then(function (result) {
                    if (active !== session) { return; }
                    if (!object(result) || result.nivelRef !== requestedLevel || typeof result.rutaCanonica !== "string" || !Array.isArray(result.perfiles) || !result.perfiles.every(profile) || (result.paginaPerfiles != null && (!object(result.paginaPerfiles) || typeof result.paginaPerfiles.puedeHaberMas !== "boolean"))) { throw new Error("RESPUESTA_INVALIDA"); }
                    session.model.contexto = result; session.model.perfiles = result.perfiles; session.model.masPerfiles = result.paginaPerfiles && result.paginaPerfiles.puedeHaberMas;
                });
            }).then(function () {
                if (active !== session) { return; }
                opening = false;
                if (active === session) {
                    if (!session.model.contexto && !session.model.consulta) { errorApertura = session.model.mensaje; cerrar(true); return; }
                    session.view = deps.ImportacionCarpetasVista.crear(host, function (name, value) { action(session, name, value); });
                    try { session.picker = deps.InventarioImportacion.vincularSelector(session.view.picker, session.inventory, function (error) { session.model.mensaje = message(error); refresh(session); }); session.model.selectorDisponible = true; }
                    catch (_) { session.model.selectorDisponible = false; session.model.mensaje = "Este navegador no permite seleccionar carpetas."; }
                    session.unsubscribe = session.inventory.alCambiar(function () { session.model.plan = null; session.model.confirmable = false; refresh(session); }); refresh(session);
                    function reselect() {
                        try {
                            if (active !== session || session.model.busy || !session.view.reselector.files.length) { return; }
                            var current = session.model.resultados.elementos.filter(function (f) { return session.expected && f.recursoId === session.expected.recursoId; })[0];
                            var file = validarReseleccion(current, session.view.reselector.files[0]);
                            session.reselected[current.recursoId] = file; session.model.mensaje = "Archivo vinculado al pendiente " + current.recursoId + ". Puedes enviar los pendientes de esta página."; refresh(session);
                        } catch (error) { session.model.mensaje = message(error); refresh(session); }
                        finally { session.view.reselector.value = ""; session.expected = null; }
                    }
                    session.view.reselector.addEventListener("change", reselect);
                    var unsubscribe = session.unsubscribe;
                    session.unsubscribe = function () { unsubscribe(); session.view.reselector.removeEventListener("change", reselect); session.reselected = Object.create(null); };
                }
                sync();
            });
        }
        function open() { abrir(null); } function resume() { if (host.doc111UltimoLote) { abrir(host.doc111UltimoLote); } }
        function consulta() { abrir({consulta:true}); }
        function manual() {
            if (!validId(manualInput.value)) { errorApertura = "Escribe una referencia de lote válida."; sync(); return; }
            abrir({loteId:manualInput.value});
        }
        function verify() {
            if (active || opening || !host.doc111ConfirmacionPendiente) { return; }
            var planId = host.doc111ConfirmacionPendiente.planId, found = false; opening = true; errorApertura = ""; sync();
            try { verificationApi = deps.ImportacionCarpetasApi.crear({url:host.getAttribute("data-api"), uploadUrl:host.getAttribute("data-upload"), token:host.getAttribute("data-token")}); }
            catch (error) { opening = false; errorApertura = message(error); sync(); return; }
            verificationApi.invocar("ConsultarConfirmacionLote", {planId:planId}).then(function (result) {
                if (destroyed) { return; }
                if (!result || result.planId !== planId || typeof result.confirmada !== "boolean") { throw new Error("RESPUESTA_INVALIDA"); }
                if (result.confirmada) {
                    if (!validId(result.loteId)) { throw new Error("RESPUESTA_INVALIDA"); }
                    host.doc111UltimoLote = {loteId:result.loteId}; host.doc111ConfirmacionPendiente = null; save(); found = true;
                } else { errorApertura = "Todavía no hay una confirmación verificable. Puedes volver a consultar; la confirmación no se repetirá."; }
            }).catch(function (error) { errorApertura = message(error); }).then(function () {
                if (verificationApi) { verificationApi.cerrar(); verificationApi = null; } opening = false; sync(); if (found && !destroyed) { resume(); }
            });
        }
        openButton.addEventListener("click", open); resumeButton.addEventListener("click", resume);
        if (verifyButton) { verifyButton.addEventListener("click", verify); } if (manualButton && manualInput) { manualButton.addEventListener("click", manual); }
        if (queryOpen) { queryOpen.addEventListener("click", consulta); }
        var instance = {abrir:function (level) { abrir(null, level); }, consultar:consulta, sincronizar:sync, cerrar:cerrar, destruir:function () { cerrar(true); destroyed = true; if (verificationApi) { verificationApi.cerrar(); } openButton.removeEventListener("click", open); resumeButton.removeEventListener("click", resume); if (verifyButton) { verifyButton.removeEventListener("click", verify); } if (manualButton) { manualButton.removeEventListener("click", manual); } if (queryOpen) { queryOpen.removeEventListener("click", consulta); [openButton,resumeButton,verifyButton].forEach(function (button) { if (button) { button.hidden = true; host.appendChild(button); } }); queryOpen.parentNode.remove(); } delete host.doc111Instancia; }};
        host.doc111Instancia = instance; sync(); return instance;
    }
    var mountedHost = null;
    function inicializar() {
        var host = env.document && env.document.getElementById("doc111-importacion");
        if (mountedHost && mountedHost !== host && mountedHost.doc111Instancia) { mountedHost.doc111Instancia.destruir(); }
        mountedHost = host; if (host) { montar(host); }
    }
    if (env.document) {
        var installed = false;
        function install() { inicializar(); if (!installed && env.Sys && env.Sys.Application) { installed = true; env.Sys.Application.add_load(inicializar); } }
        if (env.document.readyState === "loading") { env.document.addEventListener("DOMContentLoaded", install, {once:true}); } else { install(); }
    }
    return {crear:crear, crearReferencias:crearReferencias, validarReseleccion:validarReseleccion, accionesRecurso:accionesRecurso, asegurarMenu:asegurarMenu, rotularAccesoMenu:rotularAccesoMenu, montar:montar, inicializar:inicializar};
}));
