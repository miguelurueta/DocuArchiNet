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
    // DOC120_DIAGNOSTICOS: las frases de este catálogo se contrastan con VB.
    var diagnosticos = {
    "SEGMENTO_INVALIDO": "La posición o el separador de extracción no cumple la regla configurada.",
    "LONGITUD_INVALIDA": "El límite configurado no está dentro del intervalo admitido.",
    "LIMITE_INVALIDO": "El límite configurado no está dentro del intervalo admitido.",
    "LONGITUD_CODIGO_EXCEDIDA": "El código del expediente excede la longitud o representación admitida por el destino.",
    "CODIGO_NO_REPRESENTABLE": "El código del expediente excede la longitud o representación admitida por el destino.",
    "LONGITUD_NOMBRE_EXCEDIDA": "El nombre del expediente supera la longitud admitida por el destino.",
    "EJEMPLO_INVALIDO": "La entrada de comprobación no contiene los metadatos necesarios o excede su límite.",
    "FUENTE_REQUERIDA": "Falta seleccionar una fuente para resolver el campo.",
    "FUENTE_CIRCULAR_O_AMBIGUA": "La regla del expediente depende de un archivo o del propio expediente todavía no creado.",
    "TIPO_DEPENDENCIA_INVALIDO": "El tipo del campo no es compatible con la propiedad del expediente de la que depende.",
    "ETAPA_INVALIDA": "La etapa de resolución no corresponde a la fuente de la regla.",
    "EXTRACCION_INVALIDA": "La forma de extraer el valor no está admitida por la regla.",
    "EXTENSION_INVALIDA": "Quitar la extensión solo está permitido sobre el nombre de un archivo.",
    "PROVEEDOR_INVALIDO": "El proveedor no es compatible con la regla y versión seleccionadas.",
    "PROVEEDOR_INCOMPATIBLE": "El proveedor no es compatible con la regla y versión seleccionadas.",
    "PROVEEDOR_NO_APLICA": "La regla incluye un proveedor aunque utiliza otra fuente de datos.",
    "REFERENCIA_INVALIDA": "Falta una referencia válida requerida por la configuración.",
    "REFERENCIA_REQUERIDA": "Falta una referencia válida requerida por la configuración.",
    "REFERENCIA_NO_AUTORIZADA": "Una referencia configurada no tiene autorización vigente para esta operación.",
    "RESPONSABLE_NO_AUTORIZADO": "Una referencia configurada no tiene autorización vigente para esta operación.",
    "GABINETE_NO_AUTORIZADO": "Una referencia configurada no tiene autorización vigente para esta operación.",
    "TIPOLOGIA_NO_UNIVOCA": "La tipología documental no tiene una coincidencia única en la clasificación autorizada.",
    "ORIGEN_INVALIDO": "El origen configurado no pudo verificarse o ya no coincide con el confirmado.",
    "ORIGEN_CAMBIADO": "El origen configurado no pudo verificarse o ya no coincide con el confirmado.",
    "ORIGEN_NO_DISPONIBLE": "El origen configurado no pudo verificarse o ya no coincide con el confirmado.",
    "GABINETE_REQUERIDO": "Seleccione el gabinete de destino requerido por el perfil.",
    "INDICE_REQUERIDO": "Falta una regla para un índice obligatorio del gabinete.",
    "INDICE_INCOMPATIBLE": "El índice o su valor no cumple el tipo y formato del gabinete confirmado.",
    "VALOR_INDICE_INVALIDO": "El índice o su valor no cumple el tipo y formato del gabinete confirmado.",
    "ESQUEMA_INVALIDO": "El esquema requerido por la importación no está disponible con la estructura esperada.",
    "ESQUEMA_FUENTES_INCOMPLETO": "El esquema requerido por la importación no está disponible con la estructura esperada.",
    "NOMBRE_INVALIDO": "El nombre indicado no cumple las restricciones de la configuración.",
    "POLITICA_INVALIDA": "La política, modo o ámbito de integridad no forman una combinación admitida.",
    "AMBITO_INVALIDO": "La política, modo o ámbito de integridad no forman una combinación admitida.",
    "AMBITO_REQUERIDO": "La política, modo o ámbito de integridad no forman una combinación admitida.",
    "MODO_INVALIDO": "La política, modo o ámbito de integridad no forman una combinación admitida.",
    "EXTENSIONES_INVALIDAS": "La extensión no está incluida en los formatos permitidos por el perfil.",
    "EXTENSION_NO_PERMITIDA": "La extensión no está incluida en los formatos permitidos por el perfil.",
    "OPCION_REQUERIDA": "Falta una opción requerida o su valor no está permitido por la configuración.",
    "OPCION_INVALIDA": "Falta una opción requerida o su valor no está permitido por la configuración.",
    "CONFIGURACION_REQUERIDA": "Falta una opción requerida o su valor no está permitido por la configuración.",
    "CODIGO_REPETIDO_INVENTARIO": "La revisión contiene más de una carpeta que resuelve al mismo código de expediente.",
    "COINCIDENCIA_AMBIGUA": "La consulta devolvió varias coincidencias donde se exige una sola.",
    "AMBIGUO": "La consulta devolvió varias coincidencias donde se exige una sola.",
    "CODIGO_EXPEDIENTE_EXISTENTE": "La comprobación del destino encontró un expediente con el código resuelto para esta carpeta.",
    "REUTILIZACION_NO_DISPONIBLE": "No se pudo comprobar una reutilización compatible con la política del perfil.",
    "IDENTIDAD_INMUTABLE": "La identidad confirmada no se puede sustituir mediante esta operación.",
    "SIN_COINCIDENCIA": "La consulta de datos no encontró una coincidencia para la clave de esta regla.",
    "CLAVE_INVALIDA": "La clave de consulta no cumple el tipo o formato exigido por el proveedor.",
    "PROPIEDAD_AUSENTE": "La fuente no devolvió la propiedad requerida por el campo.",
    "SUBCARPETA_NO_PERMITIDA": "El perfil no admite subcarpetas dentro de la raíz seleccionada.",
    "IMPORTACION_CERRADA": "Esta vista de importación está cerrada. Abra el módulo y consulte su referencia.",
    "PREPARACION_REQUERIDA": "Prepare y revise el plan antes de confirmar la importación.",
    "RESPUESTA_NO_CONFIRMADA": "No hay un acuse válido para confirmar el resultado; consulte la misma referencia.",
    "RECEPCION_NO_CONFIRMADA": "No hay un acuse válido para confirmar el resultado; consulte la misma referencia.",
    "CONTEXTO_INVALIDO": "No se pudo validar el contexto de acceso al módulo. Recargue la página.",
    "RUTA_RELATIVA_INVALIDA": "El archivo no tiene una ruta relativa, tamaño o fecha local válidos para la selección.",
    "METADATOS_ARCHIVO_INVALIDOS": "El archivo no tiene una ruta relativa, tamaño o fecha local válidos para la selección.",
    "SELECCION_MULTIPLES_RAICES": "La selección contiene varias raíces. Agregue cada carpeta mediante una selección independiente.",
    "RUTA_DUPLICADA": "La misma raíz contiene más de un archivo con la misma ruta relativa.",
    "CODIGO_PERFIL_EXISTENTE": "Ya existe un perfil con ese código en el ámbito autorizado.",
    "EVIDENCIA_DOCUMENTAL_V2_INVALIDA": "La evidencia retenida no permite confirmar este resultado documental.",
    "EVIDENCIA_NO_VERIFICABLE": "La evidencia retenida no permite confirmar este resultado documental.",
    "NO_SOPORTADO": "La capacidad solicitada no está habilitada en este recorrido.",
    "DEPENDENCIA_NO_RESUELTA": "Todavía falta resolver una propiedad del expediente o de su clasificación.",
    "ORIGINAL_NO_VERIFICABLE": "El original retenido no pudo comprobarse para esta operación.",
    "LIMPIEZA_NO_CONFIRMADA": "La limpieza no tiene un resultado confirmado; se conserva la evidencia retenida.",
    "RETENCION_NO_CONFIGURADA": "No hay una política de retención configurada para esa capacidad.",
    "FECHA_NO_VERIFICABLE": "No se pudo comprobar la fecha requerida por la operación.",
    "RESULTADO_REGISTRADO": "El intento ya tiene un resultado registrado; consulte su evidencia sin repetir el efecto.",
    "LIMITE_PERFIL_EXCEDIDO": "El archivo supera el límite configurado en el perfil confirmado.",
    "RESELECCION_NO_COINCIDE": "El archivo no coincide con el nombre, tamaño y fecha del pendiente elegido. Seleccione el original correspondiente.",
    "NAVEGADOR_NO_COMPATIBLE": "El navegador no dispone de las capacidades necesarias para este selector de carpetas.",
    "VALOR_REQUERIDO": "Falta un valor obligatorio. Revise la regla del campo.",
    "VALOR_VACIO": "La fuente produjo un valor vacío. Revise la selección y la regla.",
    "SEGMENTO_AUSENTE": "El nombre no contiene el segmento que exige la regla. Revise la estructura del nombre.",
    "LONGITUD_EXCEDIDA": "El texto supera la longitud permitida para el campo.",
    "ENTRADA_EXCEDIDA": "El texto supera la longitud permitida para el campo.",
    "CODIGO_EXCEDE_45": "El texto supera la longitud permitida para el campo.",
    "FECHA_INVALIDA": "La fecha no coincide con el formato de la regla o no es válida.",
    "FECHA_CONSTANTE_NO_CANONICA": "La fecha no coincide con el formato de la regla o no es válida.",
    "RANGO_FECHAS_INVALIDO": "La fecha final es anterior a la fecha inicial. Corrija la regla o el origen.",
    "ENTERO_INVALIDO": "El campo requiere un número entero representable, sin separadores.",
    "DECIMAL_INVALIDO": "El campo requiere un número decimal representable con punto decimal.",
    "CARACTER_NO_REPRESENTABLE": "El texto contiene caracteres que el destino no admite. No se modificó ni truncó el valor.",
    "TEXTO_LEGACY_INVALIDO": "El texto contiene caracteres que el destino no admite. No se modificó ni truncó el valor.",
    "VALOR_NO_CANONICO": "El valor guardado no coincide con la representación canónica exigida.",
    "FUENTE_NO_RESUELTA": "No se pudo resolver la fuente del campo. Revise su disponibilidad y configuración.",
    "FUENTE_NO_DISPONIBLE": "No se pudo resolver la fuente del campo. Revise su disponibilidad y configuración.",
    "REGLA_INVALIDA": "La regla del campo no es compatible con el destino o tipo configurado.",
    "DESTINO_INVALIDO": "La regla del campo no es compatible con el destino o tipo configurado.",
    "CAMPO_INVALIDO": "La regla del campo no es compatible con el destino o tipo configurado.",
    "TIPO_INVALIDO": "La regla del campo no es compatible con el destino o tipo configurado.",
    "FORMATO_FECHA_INVALIDO": "La regla del campo no es compatible con el destino o tipo configurado.",
    "DEPENDENCIA_INVALIDA": "La regla del campo no es compatible con el destino o tipo configurado.",
    "FUENTE_INVALIDA": "La regla del campo no es compatible con el destino o tipo configurado.",
    "DESTINO_DUPLICADO": "Hay más de una regla para el mismo campo de destino.",
    "VALORES_NO_COINCIDEN": "Los valores no coinciden con las reglas del perfil confirmado.",
    "SNAPSHOT_NO_COINCIDE": "La configuración no coincide con el perfil confirmado. Consulte la importación sin sustituir su plan.",
    "REFERENCIA_LEGACY_INVALIDA": "Una referencia no cabe en el identificador admitido por el destino.",
    "RECEPCION_INCOMPLETA": "Todavía falta recepción verificada en el ámbito exigido por el perfil.",
    "BARRERA_CREACION_NO_DISPONIBLE": "La reserva, estado o recepción del ámbito exigido ya no permite crear el expediente. Consulte las acciones actuales.",
    "CONTEXTO_DOCUMENTAL_NO_COINCIDE": "El contexto, archivo o intento no coincide con los datos confirmados exigidos para almacenar.",
    "CONTEXTO_EXPEDIENTE_NO_DISPONIBLE": "El expediente de destino no tiene identidad y contexto disponibles o mantiene un intento activo.",
    "REFERENCIAS_EVIDENCIA_INVALIDAS": "Las referencias de la reserva o las huellas de recepción no cumplen el contrato de evidencia exigido.",
    "LIMITES_PERFIL_INCOMPATIBLES": "La extensión o el tamaño del archivo no cumple las restricciones del perfil confirmado.",
    "RESERVA_NO_DISPONIBLE": "La reserva o sus revisiones ya no permiten esta operación. Consulte el estado actual.",
    "PLAN_NO_REVISABLE": "El plan ya no está revisable, cambió de revisión o conserva incidencias que impiden confirmarlo.",
    "PLAN_EXPIRADO": "La vigencia del plan no permite confirmarlo. Prepare una revisión nueva tras verificar que no exista confirmación pendiente.",
    "PERFIL_CAMBIADO": "La publicación del perfil cambió desde la revisión. Prepare un plan con la versión vigente.",
    "COINCIDENCIA_DETECTADA": "La comprobación del destino detectó una coincidencia que bloquea esta unidad. Revise el detalle del plan.",
    "DESTINO_CAMBIADO": "La ruta o el sello del destino ya no coincide con la referencia consultada.",
    "DETENCION_SOLICITADA": "La importación tiene una detención solicitada. Consulte las acciones permitidas antes de continuar.",
    "CATALOGO_NO_DISPONIBLE": "No se pudo comprobar el catálogo requerido por esta fase.",
    "INDICES_PENDIENTES": "Primero debe preparar los índices del archivo en el expediente confirmado.",
    "EXPEDIENTE_NO_CONFIRMADO": "No hay una identidad de expediente confirmada disponible para almacenar este archivo.",
    "GABINETE_INCOMPATIBLE": "El gabinete confirmado no es compatible con el destino de almacenamiento.",
    "TIPOLOGIA_INCOMPATIBLE": "No hay una única tipología documental válida para el archivo.",
    "ORIGINAL_NO_COMPROBADO": "El original recibido no coincide con la evidencia de recepción exigida.",
    "NOMBRE_ARCHIVO_INVALIDO": "El nombre del archivo no es compatible con el destino. Revise su nombre y extensión.",
    "PDF_INVALIDO": "El contenido no cumple la validación del PDF admitido.",
    "PDF_FIRMA_INVALIDA": "El contenido recibido no tiene la cabecera PDF admitida.",
    "PDF_CIERRE_INVALIDO": "El contenido recibido no tiene un cierre PDF completo.",
    "PDF_ESTRUCTURA_INVALIDA": "El PDF está cifrado, requiere reconstrucción o supera los límites de páginas u objetos admitidos.",
    "BYTES_NO_COINCIDEN": "Los bytes recibidos no coinciden con el tamaño previsto para el archivo.",
    "RESPUESTA_INVALIDA": "La respuesta del servidor no es válida para esta fase.",
    "ERROR_RED": "Se interrumpió la comunicación con el servidor.",
    "ERROR_TRANSPORTE": "Se interrumpió la comunicación con el servidor.",
    "TIEMPO_AGOTADO": "El servidor no devolvió una respuesta dentro del tiempo de espera.",
    "SOLICITUD_CANCELADA": "Se canceló la espera de la solicitud; esto no revierte efectos ya iniciados.",
    "CONTENIDO_DISTINTO": "El archivo seleccionado no coincide con el contenido retenido; no se sustituyó el original.",
    "PERFIL_REQUERIDO": "Seleccione una versión publicada del perfil antes de preparar la revisión.",
    "VERSION_INVALIDA": "Seleccione una versión publicada del perfil antes de preparar la revisión.",
    "SELECCION_DIRECTORIOS_NO_DISPONIBLE": "Este navegador no permite seleccionar carpetas. Use un navegador compatible.",
    "FORMATO_NO_SOPORTADO": "Esta importación admite PDF; TIFF y otros formatos no están habilitados.",
    "LIMITE_EXCEDIDO": "La selección o el archivo supera un límite configurado para esta importación.",
    "RECEPCION_OCUPADA": "La recepción está ocupada por otro intento. Consulte el archivo antes de volver a enviarlo.",
    "CONFLICTO_REVISION": "La revisión o el estado consultado cambió. Actualice la misma referencia antes de continuar.",
    "ACCESO_DENEGADO": "No tiene autorización vigente para esta operación.",
    "SESION_REQUERIDA": "La sesión no está vigente. Inicie sesión y consulte la referencia conservada.",
    "SOLICITUD_NO_VERIFICADA": "El contexto de seguridad no pudo verificarse. Recargue la página.",
    "SOLICITUD_INVALIDA": "La solicitud no cumple el contrato de esta operación. Revise los datos y la selección.",
    "CONFIGURACION_INVALIDA": "La configuración no cumple una comprobación de esta fase. Revise las incidencias disponibles.",
    "CAPACIDAD_NO_DISPONIBLE": "No se pudo comprobar una dependencia necesaria para esta fase.",
    "OPERACION_EN_CURSO": "Hay un intento sin resultado final confirmado. Consulte su estado; no repita el efecto.",
    "RESULTADO_INCIERTO": "No se recibió un resultado comprobable de esta operación. Conserve su referencia.",
    "ERROR_INTERNO": "No se recibió un resultado comprobable de esta operación. Conserve su referencia.",
    "ARCHIVO_RECHAZADO": "El archivo fue rechazado por una comprobación de la fase. Revise sus incidencias antes de continuar."
};
    function detalleIncidencia(item) {
        if (!item || !Object.prototype.hasOwnProperty.call(diagnosticos, item.Codigo)) { return "La comprobación no pudo completarse con los datos disponibles."; }
        var text = diagnosticos[item.Codigo], supplied = typeof item.Mensaje === "string" ? item.Mensaje : "", tail = supplied.indexOf(text) === 0 ? supplied.slice(text.length) : "";
        if (/^(?: Longitud observada: [0-9]{1,10}\. Máximo permitido: [0-9]{1,10}\.)?(?: Segmento requerido: [0-9]{1,3} \(desde 1\)\.)?(?: Formato requerido: (?:yyyy-MM-dd|yyyyMMdd|dd-MM-yyyy|dd_MM_yyyy|dd\/MM\/yyyy H:mm:ss)\.)?$/.test(tail)) { text += tail; }
        var field = typeof item.Campo === "string" && /^(?:(?:CAMPO_EXPEDIENTE:)?(?:CODIGO|NOMBRE|FECHA_INICIAL|FECHA_FINAL|RESPONSABLE)|INDICE_GABINETE:[A-Za-z0-9_]{1,64}|TIPOLOGIA:TIPO_DOCUMENTAL)$/.test(item.Campo) ? item.Campo : "";
        return (field ? field + ": " : "") + text;
    }
    function describirError(error, context) {
        error = error || {}; context = context || {};
        var code = error.codigo || error.message, phase = error.fase || context.fase || error.operacion,
            labels = {PrepararPlan:"Preparación de la revisión",ConfirmarPlan:"Confirmación del plan",RECEPCION:"Recepción de archivos",CREAR_EXPEDIENTE:"Creación del expediente",RETOMAR_CREACION:"Reanudación de la creación",MATERIALIZAR_VALORES:"Preparación de índices",ALMACENAR:"Almacenamiento documental"},
            text = Object.prototype.hasOwnProperty.call(diagnosticos, code) ? diagnosticos[code] : "No se pudo completar la solicitud de esta fase.",
            id = function (value) { return typeof value === "string" && /^[1-9][0-9]{0,9}$/.test(value) && Number(value) <= 4294967295; };
        if (["ACCESO_DENEGADO","SESION_REQUERIDA","SOLICITUD_NO_VERIFICADA"].indexOf(code) !== -1) { return text; }
        if (error.lecturaPosterior) { text = "La operación quedó confirmada, pero no se pudo actualizar su vista. Consulta el mismo lote; no repitas la operación."; if (phase === "RECEPCION" && Number.isSafeInteger(error.totalRecibidos) && error.totalRecibidos >= 0) { text += " Recepciones confirmadas de esta cola: " + error.totalRecibidos + "."; } }
        else if (phase === "PrepararPlan" && error.resultadoIncierto) { text += " No se recibió el resultado de la revisión; puede haber quedado guardada. Se conserva tu selección y perfil. Todavía no hay un lote confirmado con esta respuesta. Puedes preparar otra revisión manualmente cuando no haya una confirmación pendiente."; }
        else if (error.planPendiente && id(error.planPendiente)) { text = "No se pudo confirmar el resultado del plan " + error.planPendiente + ". Usa Verificar confirmación pendiente antes de confirmar otra vez."; }
        else if (error.resultadoIncierto) { text = ["CREAR_EXPEDIENTE","RETOMAR_CREACION"].indexOf(phase) !== -1 ? "No se confirmó la creación del expediente. Consulta el estado del lote; no repitas la creación." : "No se confirmó el resultado de esta fase. Consulta el estado del lote antes de repetir cualquier efecto."; }
        else if (phase === "PrepararPlan") { text += " Corrige la selección o la regla indicada y prepara una nueva revisión; esta acción no envía ni crea expedientes."; }
        else if (phase === "ConfirmarPlan") { text += " Conserva el plan y verifica su confirmación antes de decidir cómo continuar."; }
        else { text += " Actualiza el estado y utiliza únicamente las acciones disponibles para el recurso."; }
        if (Array.isArray(error.incidencias)) { text += error.incidencias.slice(0, 5).map(function (item) { return " " + detalleIncidencia(item); }).join(""); if (error.incidencias.length > 5) { text += " Hay más incidencias en el detalle de revisión."; } }
        if (id(context.loteId)) { text += " Lote " + context.loteId + "."; }
        if (typeof error.operacionId === "string" && /^[a-f0-9]{32}$/i.test(error.operacionId)) { text += " Referencia de diagnóstico: " + error.operacionId.toLowerCase() + "."; }
        return (labels[phase] ? labels[phase] + ". " : "") + text;
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
                            error.incidencias = ["ACCESO_DENEGADO","SESION_REQUERIDA"].indexOf(response.codigo) === -1 ? response.incidencias.slice(0, 128).map(function (item) { return {Campo:item && item.Campo, Codigo:item && item.Codigo, Mensaje:detalleIncidencia({Codigo:item && item.Codigo,Mensaje:item && item.Mensaje})}; }) : [];
                            if (typeof response.operacionId === "string" && /^[a-f0-9]{32}$/i.test(response.operacionId)) { error.operacionId = response.operacionId.toLowerCase(); }
                            if (["ACCESO_DENEGADO","SESION_REQUERIDA"].indexOf(response.codigo) === -1 && response.datos && typeof response.datos.intentoId === "string" && /^[1-9][0-9]{0,9}$/.test(response.datos.intentoId) && Number(response.datos.intentoId) <= 4294967295) { error.intentoId = response.datos.intentoId; }
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
            var transport = send(options.url.replace(/\/$/, "") + "/" + operation, body, false, ["ConfirmarPlan", "PrepararPlan", "AvanzarLote", "SolicitarDetencion", "ReconciliarRecurso"].indexOf(operation) !== -1);
            var result = transport.catch(function (error) { error.operacion = operation; error.fase = request.fase || operation; throw error; });
            result.cancelar = transport.cancelar; return result;
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
            var promise = Promise.resolve().then(next).catch(function (error) {
                error.fase = "RECEPCION";
                error.recepcionParcial = {total:items.length, confirmados:received.slice(), archivoActual:index > received.length ? items[index - 1].solicitud.archivoId : null, noIntentados:items.length - index, resultadoActual:error.resultadoIncierto ? "SIN_CONFIRMAR" : "RECHAZADO"};
                throw error;
            });
            promise.cancelar = function () { cancelled = true; if (current) { current.cancelar(); } }; return promise;
        }
        return {invocar:invocar, recibir:recibir, enviarCola:enviarCola, detalleIncidencia:detalleIncidencia, describirError:describirError, cerrar:function () { closed = true; token = ""; pending.slice().forEach(function (cancel) { cancel(); }); }};
    }
    return {crear:crear, detalleIncidencia:detalleIncidencia, describirError:describirError, diagnosticos:diagnosticos};
}));
