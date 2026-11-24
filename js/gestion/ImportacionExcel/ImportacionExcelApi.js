(function (root, factory) {
    "use strict";
    if (typeof module === "object" && module.exports) { module.exports = factory; }
    else { root.ImportacionExcelApi = factory(root.jQuery, root); }
}(typeof window !== "undefined" ? window : this, function ($, env) {
    "use strict";
    env = env || {};
    var operations = ["AbrirCargaExcel", "ListarPlantillas", "ConsultarPlantilla", "GuardarPlantilla", "IniciarRecepcionExcel", "Preparar", "Confirmar", "ConsultarResultado", "ConsultarCargaExcel"];
    var mutations = ["GuardarPlantilla", "IniciarRecepcionExcel", "Preparar", "Confirmar", "Upload"];
    function route(value) {
        return typeof value === "string" && /^\/(?!\/)[A-Za-z0-9_./-]+$/.test(value) && !/(^|\/)\.{1,2}(\/|$)/.test(value);
    }
    function envelope(raw) {
        var r = raw && Object.prototype.hasOwnProperty.call(raw, "d") ? raw.d : raw;
        return r && typeof r === "object" && !Array.isArray(r) && typeof r.ok === "boolean" &&
            /^[A-Z_]{1,80}$/.test(r.codigo || "") ? r : null;
    }
    function failure(code, uncertain, status, data) {
        return {ok:false, codigo:code, resultadoIncierto:!!uncertain, httpStatus:status || 0, datos:data || null};
    }
    function create(options) {
        if (!options || !route(options.url) || !route(options.upload) || !/^[a-f0-9]{32}$/.test(options.recuperacion || "")) { throw new Error("Contexto Excel inválido."); }
        var token = "", closed = false;
        function request(operation, data, multipart) {
            var mutation = mutations.indexOf(operation) !== -1;
            return new Promise(function (resolve, reject) {
                if ((operations.indexOf(operation) === -1 && operation !== "Upload") ||
                    (operation !== "AbrirCargaExcel" && (closed || !token))) {
                    reject(failure("SESION_REQUERIDA", false)); return;
                }
                var settings = {url: multipart ? options.upload : options.url + "/" + operation, type:"POST", dataType:"json", timeout:30000, cache:false,
                    headers: {"X-Importacion-Excel-Contexto":options.recuperacion}};
                if (operation !== "AbrirCargaExcel") { settings.headers["X-Importacion-Excel-CSRF"] = token; }
                try {
                    if (multipart) { settings.data = data; settings.processData = false; settings.contentType = false; }
                    else { settings.data = JSON.stringify(operation === "AbrirCargaExcel" ? {} : {solicitud:data}); settings.contentType = "application/json; charset=utf-8"; }
                    $.ajax(settings).done(function (raw) {
                        var r = envelope(raw);
                        if (!r) { reject(failure("RESPUESTA_INVALIDA", mutation, 200)); return; }
                        if (!r.ok) {
                            if (["SESION_REQUERIDA","ACCESO_DENEGADO","SOLICITUD_NO_VERIFICADA"].indexOf(r.codigo) !== -1) { closed = true; token = ""; }
                            reject(failure(r.codigo, r.codigo === "RESULTADO_INCIERTO", 200, r.datos)); return;
                        }
                        if (operation === "AbrirCargaExcel") {
                            if (!r.datos || typeof r.datos.CsrfToken !== "string" || !r.datos.CsrfToken || r.datos.CsrfToken.length > 256) {
                                reject(failure("RESPUESTA_INVALIDA", false, 200)); return;
                            }
                            token = r.datos.CsrfToken; closed = false;
                        }
                        resolve(r);
                    }).fail(function (xhr) {
                        var status = xhr && xhr.status || 0, r = envelope(xhr && xhr.responseJSON);
                        if (status === 401 || status === 403) { closed = true; token = ""; }
                        if (r && ["SESION_REQUERIDA","ACCESO_DENEGADO","SOLICITUD_NO_VERIFICADA"].indexOf(r.codigo) !== -1) { closed = true; token = ""; }
                        reject(failure(r ? r.codigo : status === 401 ? "SESION_REQUERIDA" : status === 403 ? "ACCESO_DENEGADO" : "ERROR_TRANSPORTE",
                            mutation && (status === 0 || status >= 500 || (r && r.codigo === "RESULTADO_INCIERTO")), status, r && r.datos));
                    });
                } catch (_) { reject(failure("ERROR_TRANSPORTE", mutation)); }
            });
        }
        return {
            ejecutar: function (operation, data) {
                if (operations.indexOf(operation) === -1 || (operation !== "AbrirCargaExcel" && (!data || typeof data !== "object" || Array.isArray(data)))) {
                    return Promise.reject(failure("SOLICITUD_INVALIDA", false));
                }
                return request(operation, data, false);
            },
            cargar: function (data, file) {
                if (!data || !file || typeof env.FormData !== "function") { return Promise.reject(failure("SOLICITUD_INVALIDA", false)); }
                var form = new env.FormData();
                ["idCarga","referenciaRecepcion","conexionId","plantillaId"].forEach(function (key) { form.append(key, String(data[key])); });
                form.append("archivo", file, file.name);
                return request("Upload", form, true);
            },
            cerrar: function () { token = ""; closed = true; }
        };
    }
    return {crear:create};
}));
