(function (root, factory) {
    "use strict";
    if (typeof module === "object" && module.exports) { module.exports = factory; }
    else { root.ImportacionCarpetasApi = factory(root); }
}(typeof window !== "undefined" ? window : this, function (env) {
    "use strict";
    var operations = ["AbrirImportacion", "ListarPerfilesImportacion", "PrepararPlan", "ConfirmarPlan", "ConsultarConfirmacionLote", "ConsultarLote", "ConsultarRecursosLote", "ExportarPaginaLote", "AvanzarLote", "SolicitarDetencion", "ReconciliarRecurso"];
    function fallo(code, status, uncertain) {
        var e = new Error(code); e.codigo = code; e.httpStatus = status || 0; e.resultadoIncierto = !!uncertain; return e;
    }
    function sobre(raw) {
        var value = raw && Object.prototype.hasOwnProperty.call(raw, "d") ? raw.d : raw;
        return value && typeof value === "object" && !Array.isArray(value) && typeof value.ok === "boolean" &&
            typeof value.codigo === "string" && typeof value.mensaje === "string" && Array.isArray(value.incidencias) ? value : null;
    }
    function ruta(value) { return typeof value === "string" && /^\/(?!\/)/.test(value) && !/[?#\\\x00-\x20]/.test(value); }
    function crear(options) {
        if (!options || !ruta(options.url) || !ruta(options.uploadUrl) || typeof options.token !== "string" || !options.token) { throw fallo("CONTEXTO_INVALIDO"); }
        var closed = false, pending = [], token = options.token;
        function send(url, body, multipart, mutation, progress) {
            var xhr, finished = false, cancel;
            var promise = new Promise(function (resolve, reject) {
                function finish(error, value) {
                    if (finished) { return; } finished = true;
                    pending = pending.filter(function (item) { return item !== cancel; });
                    if (xhr) { xhr.onload = xhr.onerror = xhr.ontimeout = xhr.onabort = null; if (xhr.upload) { xhr.upload.onprogress = null; } }
                    if (error) { reject(error); } else { resolve(value); }
                }
                cancel = function () { if (xhr && !finished) { try { xhr.abort(); } catch (_) {} } finish(fallo("SOLICITUD_CANCELADA", 0, mutation)); };
                if (closed) { finish(fallo("SESION_REQUERIDA", 401, false)); return; }
                pending.push(cancel);
                try {
                    xhr = options.xhrFactory ? options.xhrFactory() : new env.XMLHttpRequest();
                    xhr.open("POST", url, true); xhr.timeout = options.timeout || 30000;
                    xhr.setRequestHeader("X-Importacion-Carpetas-CSRF", token);
                    if (!multipart) { xhr.setRequestHeader("Content-Type", "application/json; charset=utf-8"); }
                    xhr.onload = function () {
                        var response = null;
                        try { response = sobre(JSON.parse(xhr.responseText)); } catch (_) {}
                        if (xhr.status === 401 || xhr.status === 403) { closed = true; token = ""; }
                        if (!response) { finish(fallo("RESPUESTA_INVALIDA", xhr.status, mutation)); return; }
                        if (!response.ok || xhr.status < 200 || xhr.status >= 300) {
                            var error = fallo(response.codigo, xhr.status, mutation && (xhr.status >= 500 || response.codigo === "RESULTADO_INCIERTO" || response.codigo === "OPERACION_EN_CURSO"));
                            error.incidencias = response.incidencias;
                            finish(error); return;
                        }
                        finish(null, response.datos);
                    };
                    xhr.onerror = function () { finish(fallo("ERROR_RED", xhr.status, mutation)); };
                    xhr.ontimeout = function () { finish(fallo("TIEMPO_AGOTADO", xhr.status, mutation)); };
                    xhr.onabort = function () { finish(fallo("SOLICITUD_CANCELADA", xhr.status, mutation)); };
                    if (xhr.upload && progress) {
                        xhr.upload.onprogress = function (event) {
                            // Bytes de la peticion multipart; no equivalen a recepcion o incorporacion.
                            if (!finished && event.lengthComputable) { try { progress({enviados:event.loaded, total:event.total, fase:"TRANSFERENCIA"}); } catch (_) {} }
                        };
                    }
                    xhr.send(body);
                } catch (_) { finish(fallo("ERROR_TRANSPORTE", 0, mutation)); }
            });
            promise.cancelar = function () { if (cancel) { cancel(); } }; return promise;
        }
        function invocar(operation, request) {
            if (operations.indexOf(operation) === -1 || !request || typeof request !== "object" || Array.isArray(request)) { return Promise.reject(fallo("SOLICITUD_INVALIDA")); }
            var body;
            try { body = JSON.stringify({solicitud:request}); } catch (_) { return Promise.reject(fallo("SOLICITUD_INVALIDA")); }
            return send(options.url.replace(/\/$/, "") + "/" + operation, body, false, ["ConfirmarPlan", "PrepararPlan", "AvanzarLote", "SolicitarDetencion", "ReconciliarRecurso"].indexOf(operation) !== -1);
        }
        function recibir(request, file, progress) {
            if (!request || !/^[1-9][0-9]*$/.test(request.loteId) || !/^[1-9][0-9]*$/.test(request.archivoId) || !/^[A-Za-z0-9_-]{1,64}$/.test(request.claveIntento) || !file) { return Promise.reject(fallo("SOLICITUD_INVALIDA")); }
            var form;
            try {
                form = options.formDataFactory ? options.formDataFactory() : new env.FormData();
                form.append("loteId", request.loteId); form.append("archivoId", request.archivoId); form.append("claveIntento", request.claveIntento); form.append("file", file);
            } catch (_) { return Promise.reject(fallo("SOLICITUD_INVALIDA")); }
            return send(options.uploadUrl, form, true, true, progress);
        }
        function enviarCola(items, progress) {
            if (!Array.isArray(items)) { return Promise.reject(fallo("SOLICITUD_INVALIDA")); }
            // Una transferencia en vuelo. Un fallo detiene la cola, sin replay.
            var index = 0, cancelled = false, current = null, received = [];
            function next() {
                if (cancelled) { throw fallo("SOLICITUD_CANCELADA", 0, false); }
                if (index === items.length) { return received; }
                var item = items[index++]; current = recibir(item.solicitud, item.file, function (event) { if (progress) { progress(item, event); } });
                return current.then(function (result) {
                    if (!result || result.archivoId !== item.solicitud.archivoId || result.loteId !== item.solicitud.loteId || result.estadoRecepcion !== "RECIBIDO") { throw fallo("RESPUESTA_INVALIDA", 200, true); }
                    received.push(result); return next();
                });
            }
            var promise = Promise.resolve().then(next);
            promise.cancelar = function () { cancelled = true; if (current) { current.cancelar(); } }; return promise;
        }
        return {invocar:invocar, recibir:recibir, enviarCola:enviarCola, cerrar:function () { closed = true; token = ""; pending.slice().forEach(function (cancel) { cancel(); }); }};
    }
    return {crear:crear};
}));
