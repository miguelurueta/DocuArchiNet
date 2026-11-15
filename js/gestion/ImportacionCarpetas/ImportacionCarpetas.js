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
        if (raiz && allowed.indexOf("SOLICITAR_REANUDACION") !== -1 && model.estado.loteId === model.loteId && model.estado.estado === "EN_CURSO" && model.estado.hayIncertidumbre === false && model.estado.revision > 0 && model.raices && model.raices.loteId === model.loteId && model.raices.tipo === "RAICES" && model.raices.revisionLectura === model.estado.revision && resource.revision > 0 && resource.estado === "CREANDO" && resource.tieneIntentoActivo === true && resource.puedeRetomarCreacion === true && !resource.expedienteId && !resource.relacionNivelId && typeof resource.intentoActivoId === "string" && /^[1-9][0-9]{0,9}$/.test(resource.intentoActivoId) && Number(resource.intentoActivoId) <= 4294967295) { actions.push("RETOMAR_CREACION"); }
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
    function bloqueoFlujo(model) {
        if (!model || !model.estado) { return null; }
        var resources = (model.raices ? model.raices.elementos : []).map(function (r) { return {tipo:"RAICES", recurso:r, fase:"CREAR_EXPEDIENTE"}; }).concat((model.resultados ? model.resultados.elementos : []).map(function (f) { return {tipo:"ARCHIVOS", recurso:f, fase:["RECIBIENDO","INCIERTO"].indexOf(f.estadoRecepcion) !== -1 ? "RECEPCION" : "ALMACENAR"}; }));
        var active = resources.filter(function (item) { var r = item.recurso; return r.tieneIntentoActivo === true || ["CREANDO","EN_CURSO","INCIERTO"].indexOf(r.estado) !== -1 || ["RECIBIENDO","INCIERTO"].indexOf(r.estadoRecepcion) !== -1; });
        var last = model.ultimaOperacion && model.ultimaOperacion.loteId === model.loteId ? model.ultimaOperacion : null, needsRead = !!(last && model.mutacionIncierta && last.estado === "SIN_CONFIRMAR"), pending = last && (last.estado === "SOLICITANDO" || needsRead) && resources.filter(function (item) { return last.tipo === item.tipo && last.recursoId === item.recurso.recursoId; })[0], blocked = active.filter(function (item) { return last && last.tipo === item.tipo && last.recursoId === item.recurso.recursoId; })[0] || pending || active[0];
        if (!blocked) { return model.estado.hayIntentosActivos || model.estado.hayIncertidumbre || needsRead ? {tipo:"LOTE", fase:needsRead ? last.fase : null, fueraDePagina:true, verificable:false, requiereConsulta:needsRead} : null; }
        blocked.verificable = accionesRecurso(model, blocked.recurso, blocked.tipo === "RAICES").indexOf("RECONCILIAR") !== -1;
        blocked.retomable = accionesRecurso(model, blocked.recurso, blocked.tipo === "RAICES").indexOf("RETOMAR_CREACION") !== -1;
        blocked.solicitando = !!pending && last.estado === "SOLICITANDO";
        blocked.requiereConsulta = needsRead && active.indexOf(blocked) === -1;
        return blocked;
    }
    // Consentimiento efimero en esta vista; nunca se persiste una cola ejecutable.
    function crearCoordinacion(options) {
        if (!options || typeof options.leer !== "function" || typeof options.ejecutar !== "function" || typeof options.vigente !== "function") { throw new Error("SOLICITUD_INVALIDA"); }
        var running = null, stopped = true, disposed = false, generation = 0, reads = 0, effects = 0, members = null, scope = null;
        var completed = Object.create(null), confirmedRows = Object.create(null), failed = Object.create(null), current = null, status = "paused", code = null, latest = null, acknowledged = false;
        var limits = {resources:10000,pages:400,reads:10000,effects:10000};
        function id(value) { return typeof value === "string" && /^[1-9][0-9]{0,9}$/.test(value) && Number(value) <= 4294967295; }
        function integer(value) { return Number.isInteger(value) && value >= 0; }
        function fault(value) { var error = new Error(value); error.codigo = value; return error; }
        function live(token) { return !disposed && token === generation && options.vigente(); }
        function summary() {
            var roots = latest ? latest.raices : [], files = latest ? latest.archivos : [];
            return {activa:status === "running" || status === "pausing",estado:status,codigo:code,actual:current ? {tipo:current.tipo,id:current.row.recursoId,nombre:current.row.nombre,fase:current.fase} : null,
                totalRaices:roots.length,totalArchivos:files.length,recibidos:files.filter(function (f) { return f.estadoRecepcion === "RECIBIDO"; }).length,
                expedientes:roots.filter(function (r) { return id(r.expedienteId) && id(r.relacionNivelId); }).length,
                guardados:files.filter(function (f) { return id(f.documentoId) && ["ALMACENADO","COMPLETADO"].indexOf(f.estado) !== -1; }).length,
                datos:files.filter(function (f) { return f.valoresEfectivosFijados === true; }).length,
                completo:!!latest && latest.estado.estado === "COMPLETADO",lecturas:reads,efectos:effects,enumeracionCompleta:!!latest};
        }
        function publish() { if (!disposed && options.vigente() && options.cambiar) { options.cambiar(summary(),latest); } }
        function consumeRead() { if (++reads > limits.reads) { throw fault("COORDINACION_LIMITE"); } }
        function validate(snapshot) {
            if (!snapshot || !snapshot.estado || !Array.isArray(snapshot.raices) || !Array.isArray(snapshot.archivos)) { throw fault("RESPUESTA_INVALIDA"); }
            var state = snapshot.estado, roots = Object.create(null), files = Object.create(null), identity = [];
            if (!id(state.loteId) || !id(state.nivelRef) || !Number.isInteger(state.revision) || state.revision <= 0 || !Array.isArray(state.accionesPermitidas) || !integer(state.totalRaices) || !integer(state.totalArchivos) || state.totalRaices + state.totalArchivos > limits.resources || snapshot.raices.length !== state.totalRaices || snapshot.archivos.length !== state.totalArchivos) { throw fault("RESPUESTA_INVALIDA"); }
            if(!id(state.perfilId)||!id(state.versionId)||typeof state.rutaCanonica!=='string'||['detencionSolicitada','hayIntentosActivos','hayIncertidumbre'].some(function(field){return typeof state[field]!=='boolean';})){throw fault('RESPUESTA_INVALIDA');}
            var observedScope = JSON.stringify([state.loteId,state.nivelRef,state.perfilId,state.versionId,state.rutaCanonica]);
            if (scope && scope !== observedScope) { throw fault("CONFLICTO_REVISION"); }
            snapshot.raices.forEach(function (r) { if (!r || !id(r.recursoId) || roots[r.recursoId] || !Number.isInteger(r.revision) || r.revision <= 0) { throw fault("RESPUESTA_INVALIDA"); } roots[r.recursoId] = r; identity.push("R:" + r.recursoId + ":" + r.idCliente); });
            snapshot.archivos.forEach(function (f) { if (!f || !id(f.recursoId) || files[f.recursoId] || !roots[f.raizId] || !Number.isInteger(f.revision) || f.revision <= 0) { throw fault("RESPUESTA_INVALIDA"); } files[f.recursoId] = f; identity.push("A:" + f.recursoId + ":" + f.raizId + ":" + f.idCliente); });
            Object.keys(confirmedRows).forEach(function(key){var known=confirmedRows[key],row=(known.tipo==='RAICES'?roots:files)[known.id];if(!row||Object.keys(known.valores).some(function(field){return row[field]!==known.valores[field];})){throw fault('RESULTADO_PENDIENTE_LECTURA');}});
            var signature = identity.sort().join("|"); if (members && members !== signature) { throw fault("CONFLICTO_REVISION"); } scope = observedScope; members = signature;
            return snapshot;
        }
        function read(token) {
            if (!live(token)) { return Promise.resolve(null); }
            return options.leer(consumeRead,limits).then(function (snapshot) { if (!live(token)) { return null; } latest = validate(snapshot); publish(); return latest; });
        }
        function blocked(snapshot) {
            var state = snapshot.estado;
            return state.detencionSolicitada || state.hayIntentosActivos || state.hayIncertidumbre || ["DETENIDO","FALLIDO","PENDIENTE_VERIFICACION"].indexOf(state.estado) !== -1 || snapshot.raices.concat(snapshot.archivos).some(function (r) { return r.tieneIntentoActivo || ["CREANDO","EN_CURSO","INCIERTO"].indexOf(r.estado) !== -1 || ["RECIBIENDO","INCIERTO"].indexOf(r.estadoRecepcion) !== -1; });
        }
        function choose(snapshot) {
            var state = snapshot.estado, roots = snapshot.raices, files = snapshot.archivos, model = {loteId:state.loteId,estado:state,raices:{elementos:roots}}, rootMap = Object.create(null), result = null;
            roots.forEach(function (r) { rootMap[r.recursoId] = r; });
            function eligible(f) { return ["BLOQUEADO","CANCELADA"].indexOf(rootMap[f.raizId].estado) === -1 && ["CANCELADO","RECHAZADO"].indexOf(f.estado) === -1 && ["CANCELADO","RECHAZADO"].indexOf(f.estadoRecepcion) === -1; }
            function confirmedParent(f){return id(rootMap[f.raizId].expedienteId)&&id(rootMap[f.raizId].relacionNivelId);}
            if (state.accionesPermitidas.indexOf("SOLICITAR_AVANCE") === -1) { return null; }
            files.some(function (f) { if (eligible(f) && f.estadoRecepcion === "PENDIENTE" && f.estado === "NO_INICIADO" && !f.tieneIntentoActivo) { result = {tipo:"ARCHIVOS",fase:"RECEPCION",row:f}; return true; } return false; });
            if (!result) { roots.some(function (r) { if (accionesRecurso(model,r,true).indexOf("CREAR_EXPEDIENTE") !== -1) { result = {tipo:"RAICES",fase:"CREAR_EXPEDIENTE",row:r}; return true; } return false; }); }
            if (!result) { files.some(function (f) { if (!eligible(f)||!confirmedParent(f)) { return false; } var actions = accionesRecurso(model,f,false); if (actions.indexOf("MATERIALIZAR_VALORES") !== -1) { result = {tipo:"ARCHIVOS",fase:"MATERIALIZAR_VALORES",row:f}; return true; } return false; }); }
            if (!result) { files.some(function (f) { if (eligible(f)&&confirmedParent(f) && accionesRecurso(model,f,false).indexOf("ALMACENAR") !== -1) { result = {tipo:"ARCHIVOS",fase:"ALMACENAR",row:f}; return true; } return false; }); }
            if (result) { result.key = result.tipo + ":" + result.row.recursoId + ":" + result.fase; }
            return result;
        }
        function confirm(op,result,snapshot) {
            var receipt = op.fase === "RECEPCION";
            if (!result || result.loteId !== snapshot.estado.loteId || (receipt ? result.archivoId !== op.row.recursoId || result.estadoRecepcion !== "RECIBIDO" : result.fase !== op.fase || result.estado !== "CONFIRMADO" || (op.tipo === "RAICES" ? result.raizId : result.archivoId) !== op.row.recursoId)) { var error = fault("RESPUESTA_INVALIDA"); error.resultadoIncierto = true; throw error; }
            if (op.fase === "CREAR_EXPEDIENTE" && (!id(result.expedienteId) || !id(result.relacionNivelId)) || op.fase === "ALMACENAR" && (!id(result.documentoId) || !id(result.gabineteId))) { var incomplete = fault("RESPUESTA_INVALIDA"); incomplete.resultadoIncierto = true; throw incomplete; }
            var values={};if(receipt){values.estadoRecepcion='RECIBIDO';}else if(op.fase==='CREAR_EXPEDIENTE'){values.expedienteId=result.expedienteId;values.relacionNivelId=result.relacionNivelId;}else if(op.fase==='MATERIALIZAR_VALORES'){values.valoresEfectivosFijados=true;}else if(op.fase==='ALMACENAR'){values.documentoId=result.documentoId;values.gabineteId=result.gabineteId;}
            confirmedRows[op.key]={tipo:op.tipo,id:op.row.recursoId,valores:values};completed[op.key] = true; acknowledged = true; if (options.confirmar) { options.confirmar(op,result); }
        }
        function cycle(token,snapshot) {
            if (!live(token) || stopped) { return; }
            if (snapshot.estado.estado === "COMPLETADO") { status = "complete"; current = null; publish(); return; }
            if (blocked(snapshot)) { status = "blocked"; code = "OPERACION_EN_CURSO"; publish(); return; }
            current = choose(snapshot);
            if (!current) { status = "paused"; code = "SIN_AVANCE_DISPONIBLE"; publish(); return; }
            if (completed[current.key] || failed[current.key]) { status = "blocked"; code = "RESULTADO_PENDIENTE_LECTURA"; publish(); return; }
            if (effects >= limits.effects) { throw fault("COORDINACION_LIMITE"); }
            if (current.fase === "RECEPCION" && (!options.original || !options.original(current.row))) { status = "needs-original"; code = "ORIGINAL_REQUERIDO"; publish(); return; }
            acknowledged = false; publish(); var op = current;
            return Promise.resolve().then(function () {
                if (!live(token) || stopped) { return null; } effects++;
                return options.ejecutar(op,snapshot).then(function (result) {
                    if (!live(token)) { return null; } confirm(op,result,snapshot);
                    return read(token).catch(function (error) { error.lecturaPosterior = true; error.fase = op.fase; throw error; });
                });
            }).then(function (next) {
                if (!live(token)) { return; } if (stopped) { status = "paused"; current = null; publish(); return; }
                if (!next) { return; } current = null;
                return (options.ceder ? options.ceder() : Promise.resolve()).then(function () { return cycle(token,next); });
            });
        }
        function start() {
            if (disposed || !options.vigente() || running) { return running || Promise.resolve(); }
            stopped = false; code = null; status = "running"; current = null; reads = effects = 0; var token = ++generation; publish();
            running = Promise.resolve().then(function () { return read(token); }).then(function (snapshot) { if (snapshot) { return cycle(token,snapshot); } }).catch(function (error) {
                if (!live(token)) { return; }
                if (current && !acknowledged) { failed[current.key] = true; }
                error.fase = current ? current.fase : error.fase; status = "blocked"; code = /^[A-Z][A-Z0-9_]{0,63}$/.test(error.codigo || "") ? error.codigo : "RESPUESTA_INVALIDA";
                if (options.fallar) { options.fallar(error,current,acknowledged); }
                // Una sola consulta del resultado incierto; nunca replay o polling.
                if (error.resultadoIncierto && !error.lecturaPosterior) { return read(token).catch(function () {}); }
            }).then(function () { if (live(token)) { if (status === "running" || status === "pausing") { status = "paused"; } publish(); } running = null; });
            return running;
        }
        return {iniciar:start,pausar:function () { stopped = true; if (running) { status = "pausing"; publish(); } return running || Promise.resolve(); },estado:summary,
            destruir:function () { disposed = true; stopped = true; generation++; current = null; },limites:Object.freeze({resources:limits.resources,pages:limits.pages,reads:limits.reads,effects:limits.effects})};
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
            if (active && active.api.describirError && !(error && error.message === "RESELECCION_NO_COINCIDE")) { return active.api.describirError(error, {fase:active.model.faseSolicitud,loteId:active.model.loteId}); }
            if (error && error.planPendiente) { return "No se pudo confirmar el resultado del plan " + error.planPendiente + ". Conserva esta referencia para su verificación. No se repetirá la confirmación automáticamente."; }
            if (error && error.resultadoIncierto) { return ["CREAR_EXPEDIENTE","RETOMAR_CREACION"].indexOf(error.fase) !== -1 ? "No se confirmó la creación del expediente. Conserva la referencia del lote y consulta su estado; no repitas la creación." : "No se pudo confirmar el resultado. Consulta el estado antes de repetir la operación."; }
            var texts = {SESION_REQUERIDA:"La sesión expiró. Recarga la página.", ACCESO_DENEGADO:"No tienes autorización vigente para esta operación.", CAPACIDAD_NO_DISPONIBLE:"La importación no está disponible con la configuración actual.", SOLICITUD_NO_VERIFICADA:"Recarga la página para renovar el contexto de la operación.", SELECCION_DIRECTORIOS_NO_DISPONIBLE:"Este navegador no permite seleccionar carpetas.", CONFLICTO_REVISION:"Los datos cambiaron. Consulta el estado o prepara una nueva revisión.", RESPUESTA_INVALIDA:"La respuesta del servidor no es válida. Revisa la selección o vuelve a preparar."};
            if (error && error.message === "RESELECCION_NO_COINCIDE") { return "El archivo no coincide con el nombre, tamaño y fecha del archivo pendiente seleccionado. El lote permanece sin cambios."; }
            return texts[error && (error.codigo || error.message)] || "No se pudo completar la solicitud. Revisa la selección y consulta el estado antes de repetir.";
        }
        function refresh(session) {
            if (active !== session) { return; }
            var model = session.model; model.manifiesto = model.consulta ? {raices:[], archivos:[]} : session.inventory.manifiesto();
            model.describirIncidencia = session.api.detalleIncidencia;
            model.ultimoLoteId = host.doc111UltimoLote && host.doc111UltimoLote.loteId; model.planPendienteId = host.doc111ConfirmacionPendiente && host.doc111ConfirmacionPendiente.planId;
            model.paginaRaices = Math.min(model.paginaRaices, Math.max(1, Math.ceil(model.manifiesto.raices.length / 10)));
            model.paginaArchivos = Math.min(model.paginaArchivos, Math.max(1, Math.ceil(model.manifiesto.archivos.length / 20)));
            var files = model.resultados ? model.resultados.elementos : [];
            model.accionesRecurso = function (resource, raiz) { return accionesRecurso(model, resource, raiz); };
            model.bloqueoFlujo = bloqueoFlujo(model);
            model.archivosParaEnviar = model.mutacionIncierta ? 0 : files.filter(function (f) { return f.estadoRecepcion === "PENDIENTE" && f.estado === "NO_INICIADO" && !f.tieneIntentoActivo && (session.reselected[f.recursoId] || (!model.consulta && session.inventory.obtenerFile(f.idCliente))); }).length;
            model.puedeEnviar = model.archivosParaEnviar > 0;
            model.originalDisponible = function(row){return !!(session.reselected[row.recursoId] || (!model.consulta && session.loteOriginal===model.loteId && session.inventory.obtenerFile(row.idCliente)));};
            session.view.render(model);
        }
        function run(session, work) {
            if (active !== session || session.model.busy) { return Promise.resolve(); }
            session.model.busy = true; session.model.mensaje = ""; session.model.tipoMensaje = null; refresh(session);
            var succeeded = true;
            return Promise.resolve().then(work).catch(function (error) {
                succeeded = false;
                if (active === session) { error = error && typeof error === "object" ? error : new Error("RESPUESTA_INVALIDA"); session.model.tipoMensaje = "ERROR"; session.model.mensaje = message(error); if (error.recepcionParcial && ["ACCESO_DENEGADO","SESION_REQUERIDA"].indexOf(error.codigo) === -1) { var partial = error.recepcionParcial; session.model.mensaje += " Cola de esta página: " + partial.confirmados.length + " de " + partial.total + " recepciones confirmadas, " + partial.noIntentados + " archivos sin intentar." + (partial.archivoActual ? " Archivo " + partial.archivoActual + (partial.resultadoActual === "SIN_CONFIRMAR" ? " sin recepción confirmada; consulta antes de reenviar." : " interrumpió la cola; revisa la incidencia.") : ""); } }
            }).then(function () { if (active === session) { session.model.busy = false; refresh(session); } return succeeded; });
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
            session.model.avisoArbol = null;
            session.model.ultimaConsulta = null;
            function consultaCompleta() { invalidarCoordinacion(session); session.model.coordinacion = null; session.model.ultimaConsulta = Date.now(); session.model.consultasCompletadas = (session.model.consultasCompletadas || 0) + 1; }
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
                    session.model.avisoArbol = null;
                    if (typeof deps.Doc111ActualizarRama === "function") {
                        var confirmedRoots = roots.elementos.filter(function (root) { return root && !root.tieneIntentoActivo && ["EXPEDIENTE_CONFIRMADO", "COMPLETADA", "PARCIAL"].indexOf(root.estado) !== -1 && validId(root.expedienteId) && validId(root.relacionNivelId) && typeof root.codigoExpediente === "string" && root.codigoExpediente.trim(); });
                        if (confirmedRoots.length) {
                            try { if (deps.Doc111ActualizarRama({nivelRef:session.model.contexto.nivelRef, referenciaContexto:session.scope}, confirmedRoots) !== true) { session.model.avisoArbol = "El expediente está confirmado, pero no se pudo mostrar en el árbol. Localiza el destino original y pulsa Actualizar estado; no repitas la importación."; } }
                            catch (_) { session.model.avisoArbol = "El expediente está confirmado, pero no se pudo mostrar en el árbol. Localiza el destino original y pulsa Actualizar estado; no repitas la importación."; }
                        }
                    }
                });
            });
        }
        function originalCoordinado(session, model, file, roots) {
            var original = session.reselected[file.recursoId];
            if (original) { return validarReseleccion(file, original); }
            if (model.consulta || session.loteOriginal !== model.loteId) { return null; }
            original = session.inventory.obtenerFile(file.idCliente);
            if (!original) { return null; }
            var item = session.inventory.manifiesto().archivos.filter(function (entry) { return entry.idCliente === file.idCliente; })[0], parent = roots.filter(function (r) { return r.recursoId === file.raizId; })[0];
            if (!item || !parent || item.raizIdCliente !== parent.idCliente || item.rutaRelativa !== file.rutaRelativa) { throw new Error("CONTENIDO_DISTINTO"); }
            return validarReseleccion(file,original);
        }
        function proyectarCoordinacion(session, model, snapshot) {
            var state = snapshot.estado, size = Math.min(25,state.maximoTamanoPagina || 1);
            function project(type, rows, filters, page) {
                var selected = rows.filter(function (row) { return Object.keys(filters || {}).every(function (key) { return !filters[key] || row[key] === filters[key]; }); });
                return {loteId:state.loteId,tipo:type,pagina:page,tamanoPagina:size,revisionLectura:state.revision,totalOriginal:rows.length,totalFiltrado:selected.length,puedeHaberMas:page*size<selected.length,elementos:selected.slice((page-1)*size,page*size)};
            }
            model.estado = state; model.tamanoResultados = size; model.expedientesConfirmados = snapshot.raices.filter(function(r){return validId(r.expedienteId)&&validId(r.relacionNivelId);});
            model.resultados = project("ARCHIVOS",snapshot.archivos,model.filtrosArchivos,model.resultados ? model.resultados.pagina : 1);
            model.raices = project("RAICES",snapshot.raices,model.filtrosRaices,model.paginaRaicesResultado || 1);
            session.consultaArchivos = solicitudPagina(session,"ARCHIVOS",model.resultados.pagina);
            model.avisoArbol = null;
            if (typeof deps.Doc111ActualizarRama === "function") {
                var confirmed = snapshot.raices.filter(function (r) { return !r.tieneIntentoActivo && validId(r.expedienteId) && validId(r.relacionNivelId) && typeof r.codigoExpediente === "string" && ["EXPEDIENTE_CONFIRMADO","COMPLETADA","PARCIAL"].indexOf(r.estado) !== -1; });
                if (confirmed.length) { try { if (deps.Doc111ActualizarRama({nivelRef:model.contexto.nivelRef,referenciaContexto:session.scope},confirmed) !== true) { model.avisoArbol = "El expediente está confirmado, pero no se pudo mostrar en el árbol. Consulta el destino original sin repetir la importación."; } } catch (_) { model.avisoArbol = "El expediente está confirmado, pero no se pudo mostrar en el árbol. Consulta el destino original sin repetir la importación."; } }
            }
            model.ultimaConsulta = Date.now(); model.consultasCompletadas = (model.consultasCompletadas || 0) + 1;
        }
        function coordinacion(session) {
            if (session.coordinator) { return session.coordinator; }
            var model = session.model, lot = model.loteId, level = model.contexto && model.contexto.nivelRef;
            function live() { return active === session && session.model === model && model.loteId === lot && (!level || model.contexto && model.contexto.nivelRef === level) && !destroyed; }
            function fail(code) { var error = new Error(code); error.codigo = code; return error; }
            function readAll(consume,limits) {
                function read(operation,request) { if (!live()) { return Promise.reject(fail("SOLICITUD_CANCELADA")); } consume(); return session.api.invocar(operation,request); }
                var snapshot;
                return read("ConsultarLote",{loteId:lot}).then(function (state) {
                    if (!live() || !state || state.loteId !== lot || !validId(state.nivelRef) || level && state.nivelRef !== level || !revision(state.revision) || !Array.isArray(state.accionesPermitidas) || state.accionesPermitidas.indexOf("CONSULTAR_RAICES") === -1 || !Number.isInteger(state.maximoTamanoPagina) || state.maximoTamanoPagina<1 || !Number.isInteger(state.totalRaices) || !Number.isInteger(state.totalArchivos) || state.totalRaices+state.totalArchivos>limits.resources) { throw fail("RESPUESTA_INVALIDA"); }
                    snapshot = {estado:state,raices:[],archivos:[]};
                    function pages(type,expected,target,page) {
                        if (page>limits.pages) { throw fail("COORDINACION_LIMITE"); }
                        var size=Math.min(25,state.maximoTamanoPagina);
                        return read("ConsultarRecursosLote",{loteId:lot,tipo:type,pagina:page,tamanoPagina:size}).then(function (result) {
                            if (!live() || !result || result.loteId!==lot || result.tipo!==type || result.pagina!==page || result.tamanoPagina!==size || result.revisionLectura!==state.revision || result.totalOriginal!==expected || result.totalFiltrado!==expected || !Array.isArray(result.elementos) || result.elementos.length!==Math.min(size,Math.max(0,expected-(page-1)*size)) || result.puedeHaberMas!==((page-1)*size+result.elementos.length<expected)) { throw fail("CONFLICTO_REVISION"); }
                            Array.prototype.push.apply(target,result.elementos);
                            return result.puedeHaberMas ? pages(type,expected,target,page+1) : null;
                        });
                    }
                    return pages("RAICES",state.totalRaices,snapshot.raices,1).then(function () { return pages("ARCHIVOS",state.totalArchivos,snapshot.archivos,1); });
                }).then(function () { return read("ConsultarLote",{loteId:lot}); }).then(function (last) {
                    var first=snapshot.estado;
                    if (!live() || !last || last.loteId!==lot || last.nivelRef!==first.nivelRef || last.revision!==first.revision || last.versionId!==first.versionId || last.perfilId!==first.perfilId || last.estado!==first.estado || last.totalRaices!==first.totalRaices || last.totalArchivos!==first.totalArchivos || last.detencionSolicitada!==first.detencionSolicitada || last.hayIntentosActivos!==first.hayIntentosActivos || last.hayIncertidumbre!==first.hayIncertidumbre) { throw fail("CONFLICTO_REVISION"); }
                    return snapshot;
                });
            }
            session.coordinator = crearCoordinacion({vigente:live,leer:readAll,
                original:function (row) { return originalCoordinado(session,model,row,session.lecturaCoordinada.raices); },
                ceder:function () { return new Promise(function (resolve) { (deps.setTimeout || env.setTimeout)(resolve,0); }); },
                cambiar:function (value,snapshot) {
                    if (!live()) { return; } model.coordinacion=value; model.busy=value.activa;
                    if (snapshot) { session.lecturaCoordinada=snapshot; model.contexto={nivelRef:snapshot.estado.nivelRef,rutaCanonica:snapshot.estado.rutaCanonica}; host.doc111UltimoLote={loteId:lot}; save(); proyectarCoordinacion(session,model,snapshot); }
                    if (value.estado==='needs-original') { model.mensaje="Falta el original del archivo pendiente. Vincúlalo y pulsa Continuar importación; los resultados confirmados se conservan."; model.tipoMensaje="AVISO"; }
                    if (value.estado==='paused' && !model.mutacionIncierta) { model.mensaje="Importación en pausa. La siguiente operación requiere Continuar importación."; }
                    if (value.completo) { model.mensaje="Importación completada. Revisa los expedientes y documentos confirmados o cierra el asistente."; model.tipoMensaje="EXITO"; }
                    refresh(session);
                },
                ejecutar:function (op,snapshot) {
                    model.faseSolicitud=op.fase; model.tipoMensaje=null;model.transferencia=null;
                    model.ultimaOperacion={loteId:lot,tipo:op.tipo,recursoId:op.row.recursoId,fase:op.fase,estado:"SOLICITANDO"};
                    model.mensaje={RECEPCION:"Enviando PDF",CREAR_EXPEDIENTE:"Creando expediente",MATERIALIZAR_VALORES:"Preparando datos",ALMACENAR:"Guardando documento"}[op.fase]+": "+op.row.nombre+". Espera la confirmación de esta fase."; refresh(session);
                    if(op.fase==='RECEPCION'){
                        var original=originalCoordinado(session,model,op.row,snapshot.raices);
                        if(!original){return Promise.reject(fail('ORIGINAL_REQUERIDO'));}
                        return session.api.enviarCola([{file:original,solicitud:{loteId:lot,archivoId:op.row.recursoId,claveIntento:key()}}],function(item,progress){if(live()){model.transferencia={enviados:progress.enviados,total:progress.total};refresh(session);}}).then(function(receipts){return receipts[0];});
                    }
                    return session.api.invocar('AvanzarLote',{loteId:lot,recursoId:op.row.recursoId,fase:op.fase,revisionEsperada:op.row.revision,claveOperacion:key()});
                },
                confirmar:function(op,result){if(!live()){return;}model.mutacionIncierta=false;model.ultimaOperacion.estado='CONFIRMADA';model.ultimaOperacion.resultadoConfirmado={};['intentoId','expedienteId','relacionNivelId','documentoId','inventarioId','gabineteId'].forEach(function(field){if(validId(String(result[field]))){model.ultimaOperacion.resultadoConfirmado[field]=String(result[field]);}});},
                fallar:function(error,op,confirmed){
                    if(!live()){return;}model.mutacionIncierta=!!op;model.tipoMensaje='ERROR';
                    if(op&&model.ultimaOperacion){model.ultimaOperacion.estado=confirmed?'CONFIRMADA':'SIN_CONFIRMAR';model.ultimaOperacion.codigo=/^[A-Z][A-Z0-9_]{0,63}$/.test(error.codigo||'')?error.codigo:'RESPUESTA_NO_CONFIRMADA';model.ultimaOperacion.intentoId=error.intentoId||op.row.intentoActivoId||null;}
                    model.mensaje=message(error);refresh(session);
                }
            });
            return session.coordinator;
        }
        function invalidarCoordinacion(session) { if(session.coordinator){session.coordinator.destruir();session.coordinator=null;}session.lecturaCoordinada=null;session.model.busy=false;if(session.model.coordinacion){session.model.coordinacion.activa=false;session.model.coordinacion.estado='paused';session.model.coordinacion.actual=null;session.model.coordinacion.enumeracionCompleta=false;} }
        function archivoConocido(session, resourceId) {
            var model=session.model, snapshot=session.lecturaCoordinada;
            var rows=snapshot&&snapshot.estado.loteId===model.loteId?snapshot.archivos:model.resultados&&model.resultados.elementos||[];
            return rows.filter(function(row){return row.recursoId===resourceId;})[0];
        }
        function iniciarCoordinacion(session) { if(active!==session||!session.model.loteId||session.model.confirmacionIncierta){return Promise.resolve();}return coordinacion(session).iniciar(); }

        function cerrar(force) {
            var session = active; if (!session) { return; }
            var preparation = session.importacionGuardada ? session.importacionGuardada.model : session.model;
            if (!force && (session.confirming || preparation.confirmacionIncierta) && !deps.confirm("La confirmación puede seguir en el servidor. Cerrar liberará los archivos locales y conservará la referencia del plan para verificarla. ¿Cerrar?")) { return; }
            if (!force && !preparation.loteId && !session.confirming && !preparation.confirmacionIncierta && session.inventory.manifiesto().archivos.length && !deps.confirm("Se descartará esta preparación sin confirmar. ¿Cerrar?")) { return; }
            if (session.confirming) { host.doc111ConfirmacionPendiente = session.confirming; }
            invalidarCoordinacion(session); save(); active = null; opening = false; session.api.cerrar(); session.picker.destruir(); session.unsubscribe(); session.operation.destruir(); session.view.destruir();
            session.urls.forEach(function (url) { deps.URL.revokeObjectURL(url); }); session.timers.forEach(function (id) { deps.clearTimeout(id); }); sync();
            if (!force && session.returnFocus && session.returnFocus.isConnected && typeof session.returnFocus.focus === "function") { session.returnFocus.focus(); }
        }
        function action(session, name, value) {
            var model = session.model;
            model.faseSolicitud = "CONSULTAR";
            if (name === "cerrar") { cerrar(); return; }
            if (active !== session) { return; }
            if (name === "pausar-importacion") { if(session.coordinator){return session.coordinator.pausar();}return; }
            if (session.coordinator && session.coordinator.estado().activa) {
                if (["consultar","consultar-referencia","consultar-ultimo","volver-importacion","verificar-referencia","exportar","pagina-resultados","pagina-raices-resultado","filtrar-resultados","limpiar-resultados","filtrar-raices","limpiar-raices","ver-expedientes"].indexOf(name)!==-1) { session.coordinator.pausar().then(function(){if(active===session){action(session,name,value);}}); }
                return;
            }
            if (model.busy) { return; }
            if (name === "continuar-importacion") { iniciarCoordinacion(session);return; }
            if (name === "ver-expedientes") { if(typeof deps.Doc120MostrarExpediente==='function'&&value){var known=(model.expedientesConfirmados||model.raices&&model.raices.elementos||[]).filter(function(r){return r.recursoId===value;})[0];if(known&&validId(known.expedienteId)&&validId(known.relacionNivelId)){if(deps.Doc120MostrarExpediente(known,model.contexto)===true){cerrar();}else{model.mensaje="El expediente está confirmado, pero su nodo no está visible o es ambiguo. Abre el destino original en la estructura y consulta de nuevo; no repitas la importación.";model.tipoMensaje="AVISO";refresh(session);}}}return; }
            if (name === "volver-importacion") {
                if (!session.importacionGuardada) { return; }
                invalidarCoordinacion(session); var saved = session.importacionGuardada; session.importacionGuardada = null; session.model = saved.model; session.reselected = saved.reselected; session.consultaArchivos = saved.consultaArchivos; session.expected = null;
                if (session.model.loteId) { run(session, function () { return consultar(session, session.model.resultados ? session.model.resultados.pagina : 1); }); }
                else { refresh(session); }
                return;
            }
            if (name === "consultar-referencia" || name === "consultar-ultimo") {
                var reference = name === "consultar-ultimo" ? model.ultimoLoteId : String(value || "").trim();
                if (!validId(reference)) { model.mensaje = "Escribe una referencia de lote válida."; refresh(session); return; }
                invalidarCoordinacion(session);
                if (!model.consulta) {
                    session.importacionGuardada = {model:model, reselected:session.reselected, consultaArchivos:session.consultaArchivos};
                    session.model = Object.assign({}, model, {consulta:true, perfil:null, plan:null, confirmable:false, puedeVolverImportacion:true}); model = session.model;
                }
                model.referencia = reference; model.loteId = reference; model.contexto = null; model.estado = null; model.resultados = null; model.raices = null; model.expedientesConfirmados = null; model.coordinacion = null; model.filtrosArchivos = {}; model.filtrosRaices = {}; model.etapa = 3; model.mutacionIncierta = false; model.ultimaOperacion = null; session.reselected = Object.create(null);
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
                session.expected = archivoConocido(session,value);
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
                model.faseSolicitud = "PrepararPlan";
                run(session, function () {
                    model.plan = null; model.confirmable = false;
                    var ticket = session.operation.iniciarPreparacion(), request = ticket.manifiesto;
                    request.nivelRef = model.contexto.nivelRef; request.versionId = ticket.versionId;
                    return session.api.invocar("PrepararPlan", request).then(function (plan) {
                        if (active !== session || !session.operation.vigente(ticket)) { return; }
                        if (!preparation(plan, model.contexto.nivelRef)) { throw new Error("RESPUESTA_INVALIDA"); }
                        model.confirmable = session.operation.registrarPreparacion(ticket, plan); model.plan = plan; model.etapa = 2; model.paginaIncidencias = model.paginaValores = 1;
                        model.tipoMensaje = model.confirmable ? "EXITO" : "AVISO";
                        model.mensaje = "Revisión guardada como plan " + plan.planId + (model.confirmable ? ". Revisa los datos antes de confirmar." : ". Hay incidencias que impiden confirmar; revisa su detalle.") + " Todavía no se enviaron archivos ni se crearon expedientes.";
                    });
                }); return;
            }
            var coordinatedStart = name === "confirmar-importar";
            if (name === "confirmar" || coordinatedStart) {
                model.faseSolicitud = "ConfirmarPlan";
                if (!model.confirmable || model.confirmacionIncierta) { return; }
                run(session, function () {
                    var request = session.operation.referenciaConfirmacion(); request.claveConfirmacion = key();
                    var expectedPlan = request.planId, expectedLevel = model.contexto.nivelRef; session.confirming = request;
                    host.doc111ConfirmacionPendiente = {planId:expectedPlan}; save();
                    return session.api.invocar("ConfirmarPlan", request).then(function (result) {
                        if (!result || !validId(result.loteId) || result.planId !== expectedPlan || result.nivelRef !== expectedLevel) { var bad = new Error("RESPUESTA_INVALIDA"); bad.resultadoIncierto = true; throw bad; }
                        host.doc111UltimoLote = {loteId:result.loteId, nivelRef:result.nivelRef}; host.doc111ConfirmacionPendiente = null; session.confirming = null; save(); sync();
                        if (active !== session) { return; } model.loteId = result.loteId; session.loteOriginal = result.loteId; model.etapa = 3; model.mensaje = "Plan confirmado en el lote " + result.loteId + ". La importación puede continuar con sus operaciones verificadas."; return consultar(session, 1).catch(function (error) { error.lecturaPosterior = true; error.fase = "ConfirmarPlan"; throw error; });
                    }).catch(function (error) {
                        if (session.confirming && error.resultadoIncierto) { host.doc111ConfirmacionPendiente = session.confirming; model.confirmacionIncierta = true; error.planPendiente = session.confirming.planId; }
                        else if (session.confirming) { host.doc111ConfirmacionPendiente = null; }
                        save(); sync();
                        session.confirming = null; throw error;
                    });
                }).then(function(ok){if(ok&&coordinatedStart&&active===session&&model.loteId&&!model.confirmacionIncierta){iniciarCoordinacion(session);}}); return;
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
                    if (phase === "RETOMAR_CREACION" && !deps.confirm("Se retomará la creación del expediente en esta importación, conservando los archivos recibidos." + (model.estado.detencionSolicitada === true ? " También se levantará la detención que acabas de consultar." : "") + " ¿Retomar creación?")) { return; }
                }
                model.ultimaOperacion = {loteId:model.loteId,tipo:kind ? "RAICES" : "ARCHIVOS",recursoId:row && row.recursoId || null,fase:name === "detener" ? "DETENCION" : phase,estado:"SOLICITANDO"};
                model.faseSolicitud = phase || "DETENCION";
                run(session, function () {
                    var operation = name === "detener" ? "SolicitarDetencion" : name === "reconciliar" ? "ReconciliarRecurso" : "AvanzarLote";
                    var request = {loteId:model.loteId, revisionEsperada:name === "fase" ? row.revision : model.estado.revision};
                    if (name === "fase") { request.recursoId = row.recursoId; request.fase = phase; request.claveOperacion = key(); }
                    if (phase === "RETOMAR_CREACION") { request.intentoId = row.intentoActivoId; request.revisionLoteEsperada = model.estado.revision; request.levantarDetencion = model.estado.detencionSolicitada === true; }
                    if (name === "reconciliar") { request.intentoId = row.intentoActivoId; }
                    return session.api.invocar(operation, request).then(function (result) {
                        if (active !== session) { return; }
                        if (name === "reconciliar" && result && result.loteId === model.loteId && result.intentoId === row.intentoActivoId && result.resuelta === false && result.estado === "INCIERTO") { model.ultimaOperacion.estado = "SIN_RESOLVER"; model.mensaje = "La verificación no pudo confirmar el resultado del intento. Se conserva el bloqueo sin repetir efectos."; return consultar(session, model.resultados.pagina); }
                        if (!result || result.loteId !== model.loteId || (name === "fase" && (result.fase !== phase || result.estado !== "CONFIRMADO" || (kind ? result.raizId : result.archivoId) !== row.recursoId)) || (name === "reconciliar" && (result.intentoId !== row.intentoActivoId || result.resuelta !== true || result.estado !== "CONFIRMADO")) || (name === "detener" && result.detencionSolicitada !== true && ["COMPLETADO", "FALLIDO"].indexOf(result.estado) === -1)) {
                            var invalid = new Error("RESPUESTA_INVALIDA"); invalid.resultadoIncierto = true; throw invalid;
                        }
                        model.ultimaOperacion.estado = "CONFIRMADA";
                        model.ultimaOperacion.resultadoConfirmado = {};
                        ["intentoId","expedienteId","relacionNivelId","documentoId","inventarioId","gabineteId"].forEach(function (field) { if (validId(String(result[field]))) { model.ultimaOperacion.resultadoConfirmado[field] = String(result[field]); } });
                        model.mensaje = phase === "MATERIALIZAR_VALORES" ? "Índices preparados para el archivo " + row.recursoId + ". Todavía falta almacenar el documento; utiliza Almacenar archivo cuando esté disponible." : phase === "ALMACENAR" ? "Almacenamiento confirmado para el archivo " + row.recursoId + ". Consulta el estado de la carpeta y del lote para revisar sus pendientes." : ["CREAR_EXPEDIENTE","RETOMAR_CREACION"].indexOf(phase) !== -1 ? "Creación confirmada para la carpeta " + row.recursoId + ". Continúa con Preparar índices en los archivos disponibles; no se almacenan automáticamente." : "Operación confirmada. Consulta el estado actualizado.";
                        return consultar(session, model.resultados.pagina);
                    }).catch(function (error) { error.fase = phase; if (["CONFIRMADA","SIN_RESOLVER"].indexOf(model.ultimaOperacion.estado) !== -1) { error.lecturaPosterior = model.ultimaOperacion.estado === "CONFIRMADA"; model.mutacionIncierta = true; throw error; } model.mutacionIncierta = true; model.ultimaOperacion.estado = "SIN_CONFIRMAR"; model.ultimaOperacion.codigo = /^[A-Z][A-Z0-9_]{0,63}$/.test(error.codigo || "") ? error.codigo : "RESPUESTA_NO_CONFIRMADA"; model.ultimaOperacion.operacionId = error.operacionId || null; model.ultimaOperacion.intentoId = error.intentoId || row && row.intentoActivoId || null; throw error; });
                }); return;
            }
            if (name === "enviar") {
                model.faseSolicitud = "RECEPCION";
                if (!model.puedeEnviar) { return; }
                run(session, function () {
                    var items = model.resultados.elementos.filter(function (f) { return f.estadoRecepcion === "PENDIENTE" && f.estado === "NO_INICIADO" && !f.tieneIntentoActivo && (session.reselected[f.recursoId] || (!model.consulta && session.inventory.obtenerFile(f.idCliente))); }).map(function (f) {
                        var file = session.reselected[f.recursoId] || (!model.consulta && session.inventory.obtenerFile(f.idCliente));
                        if (session.reselected[f.recursoId]) { validarReseleccion(f, file); }
                        if (file.name !== f.nombre || String(file.size) !== f.bytesPrevistos) { throw new Error("CONTENIDO_DISTINTO"); }
                        return {file:file, solicitud:{loteId:model.loteId, archivoId:f.recursoId, claveIntento:key()}};
                    });
                    return session.api.enviarCola(items, function (item, progress) { if (active === session) { model.mensaje = "Enviando archivo " + (items.indexOf(item) + 1) + " de " + items.length + " de esta página filtrada: " + item.file.name + ". Transferencia: " + progress.enviados + " de " + progress.total + " bytes de solicitud enviados. La recepción no implica almacenamiento documental."; refresh(session); } }).then(function (receipts) { model.recepcionesConfirmadas = receipts; return consultar(session, model.resultados.pagina).catch(function (error) { error.lecturaPosterior = true; error.totalRecibidos = receipts ? receipts.length : items.length; error.fase = "RECEPCION"; throw error; }); }).then(function () { if (active === session) { model.mensaje = "Recepción confirmada para " + items.length + " archivos de esta página filtrada. Continúa con las acciones disponibles; todavía no implica almacenamiento documental."; } }).catch(function (error) { model.mutacionIncierta = true; throw error; });
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
                    session.view = deps.ImportacionCarpetasVista.crear(host, function (name, value) { return action(session, name, value); });
                    try { session.picker = deps.InventarioImportacion.vincularSelector(session.view.picker, session.inventory, function (error) { session.model.mensaje = message(error); refresh(session); }); session.model.selectorDisponible = true; }
                    catch (_) { session.model.selectorDisponible = false; session.model.mensaje = "Este navegador no permite seleccionar carpetas."; }
                    session.unsubscribe = session.inventory.alCambiar(function () { session.model.plan = null; session.model.confirmable = false; refresh(session); }); refresh(session);
                    function reselect() {
                        try {
                            if (active !== session || session.model.busy || !session.view.reselector.files.length) { return; }
                            var current = session.expected && archivoConocido(session,session.expected.recursoId);
                            var file = validarReseleccion(current, session.view.reselector.files[0]);
                            session.reselected[current.recursoId] = file;
                            if(session.model.coordinacion&&session.model.coordinacion.estado==='needs-original'){invalidarCoordinacion(session);session.model.coordinacion=null;}
                            session.model.mensaje = "Archivo vinculado al pendiente " + current.recursoId + ". La selección no lo envía. Puedes continuar la importación desde el avance del lote o usar el envío individual disponible."; refresh(session);
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
    return {crear:crear, crearReferencias:crearReferencias, validarReseleccion:validarReseleccion, crearCoordinacion:crearCoordinacion, accionesRecurso:accionesRecurso, bloqueoFlujo:bloqueoFlujo, asegurarMenu:asegurarMenu, rotularAccesoMenu:rotularAccesoMenu, montar:montar, inicializar:inicializar};
}));
