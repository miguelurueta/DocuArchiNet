// IDs de MySQL: cadenas decimales canónicas; preservar null y no aplicar parseInt.
(function (root, factory) {
    "use strict";
    if (typeof module === "object" && module.exports) {
        module.exports = factory;
    } else {
        root.PerfilImportacionApi = factory(root.jQuery);
    }
}(typeof window !== "undefined" ? window : this, function ($) {
    "use strict";
    var operations = [
        "ListarFuentes", "ObtenerFuente", "HistorialFuente", "ConsultarMetadatosFuente", "CrearFuente", "GuardarFuente", "ActivarFuente", "RevocarFuente",
        "ListarAdministracionProveedores", "ObtenerAdministracionProveedor", "CrearProveedor", "GuardarProveedor", "ValidarProveedor", "EnsayarProveedor", "PublicarProveedor", "CrearVersionProveedor", "CambiarDisponibilidadProveedor",
        "ListarPerfiles", "ObtenerPerfil", "ConsultarHistorial", "ConsultarCatalogo", "ConsultarProveedores", "ConsultarMetadatosProveedor",
        "CrearPerfil", "GuardarBorrador", "ActualizarCabecera", "EnsayarReglas",
        "PublicarVersion", "CrearVersion", "CambiarDisponibilidad", "ValidarSeleccionPerfil"
    ];
    var mutations = [
        "CrearFuente", "GuardarFuente", "ActivarFuente", "RevocarFuente",
        "CrearProveedor", "GuardarProveedor", "PublicarProveedor", "CrearVersionProveedor", "CambiarDisponibilidadProveedor",
        "CrearPerfil", "GuardarBorrador", "ActualizarCabecera", "PublicarVersion",
        "CrearVersion", "CambiarDisponibilidad"
    ];
    function error(code, message, status, uncertain, incidents) {
        return {
            ok: false, codigo: code, mensaje: message, httpStatus: status || 0,
            resultadoIncierto: !!uncertain, incidencias: incidents || []
        };
    }
    function envelope(raw) {
        var value = raw && Object.prototype.hasOwnProperty.call(raw, "d") ? raw.d : raw;
        if (!value || typeof value !== "object" || Array.isArray(value) ||
            typeof value.ok !== "boolean" || typeof value.codigo !== "string" ||
            typeof value.mensaje !== "string" || !Array.isArray(value.incidencias)) {
            return null;
        }
        return value;
    }
    function create(options) {
        if (!options || typeof options.url !== "string" || typeof options.token !== "string" ||
            !/^\/(?!\/)/.test(options.url) || /[?#\\]/.test(options.url)) {
            throw new Error("Contexto del perfil inválido.");
        }
        // La página proporciona una ruta de aplicación del mismo origen y el token autorizado.
        // No hay persistencia local, renovación de token ni reintentos automáticos.
        var url = options.url.replace(/\/$/, "");
        var token = options.token;
        var closed = false;
        function invoke(operation, request) {
            var deferred = $.Deferred();
            var mutation = mutations.indexOf(operation) !== -1;
            if (closed) {
                deferred.reject(error("SESION_REQUERIDA", "Recargue el contexto autenticado.", 401, false));
                return deferred.promise();
            }
            if (operations.indexOf(operation) === -1 || !request || typeof request !== "object" || Array.isArray(request)) {
                deferred.reject(error("SOLICITUD_INVALIDA", "Operación o solicitud inválida.", 400, false));
                return deferred.promise();
            }
            var body;
            try { body = JSON.stringify({ solicitud: request }); }
            catch (_) {
                deferred.reject(error("SOLICITUD_INVALIDA", "La solicitud no es serializable.", 400, false));
                return deferred.promise();
            }
            try {
                $.ajax({
                    url: url + "/" + operation, type: "POST",
                    contentType: "application/json; charset=utf-8", dataType: "json",
                    headers: { "X-Importacion-Perfil-CSRF": token },
                    data: body, timeout: 30000, cache: false
                }).done(function (raw) {
                    var response = envelope(raw);
                    if (!response) {
                        deferred.reject(error("RESPUESTA_INVALIDA", "No se pudo interpretar la respuesta. Consulte el estado antes de repetir.", 200, mutation));
                    } else if (!response.ok) {
                        if (response.codigo === "SESION_REQUERIDA" || response.codigo === "SOLICITUD_NO_VERIFICADA" ||
                            response.codigo === "ACCESO_DENEGADO") { closed = true; token = ""; }
                        deferred.reject(response);
                    } else {
                        deferred.resolve(response);
                    }
                }).fail(function (xhr, status) {
                    var httpStatus = xhr && xhr.status || 0;
                    var response = envelope(xhr && xhr.responseJSON);
                    if (httpStatus === 401 || httpStatus === 403) { closed = true; token = ""; }
                    if (response && !response.ok) {
                        response.httpStatus = httpStatus;
                        response.resultadoIncierto = mutation && (httpStatus === 0 || httpStatus >= 500);
                        // Este codigo se emite solo en el preflight, antes de escribir la fuente.
                        if(httpStatus===503 && response.codigo==='ESQUEMA_FUENTES_INCOMPLETO' &&
                            ['CrearFuente','GuardarFuente','ActivarFuente','RevocarFuente'].indexOf(operation)!==-1){response.resultadoIncierto=false;}
                        deferred.reject(response);
                        return;
                    }
                    var codes = { 400: "SOLICITUD_INVALIDA", 401: "SESION_REQUERIDA",
                        403: "ACCESO_DENEGADO", 409: "CONFLICTO_REVISION", 503: "CAPACIDAD_NO_DISPONIBLE" };
                    deferred.reject(error(codes[httpStatus] || (status === "timeout" ? "TIEMPO_AGOTADO" : "ERROR_TRANSPORTE"),
                        "No se confirmó la operación. Consulte el estado antes de repetir.",
                        httpStatus, mutation && (httpStatus === 0 || httpStatus >= 500 || status === "parsererror")));
                });
            } catch (_) {
                deferred.reject(error("ERROR_TRANSPORTE", "No se confirmó la operación.", 0, mutation));
            }
            return deferred.promise();
        }
        return {
            ejecutar: invoke,
            cerrar: function () { closed = true; token = ""; }
        };
    }
    return { crear: create };
}));
