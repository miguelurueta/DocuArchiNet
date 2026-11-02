(function (root, factory) {
    "use strict";
    if (typeof module === "object" && module.exports) { module.exports = factory(); }
    else { root.InventarioImportacion = factory(); }
}(typeof window !== "undefined" ? window : this, function () {
    "use strict";
    function error(code) { var e = new Error(code); e.codigo = code; return e; }
    function crear() {
        var roots = [], files = [], listeners = [], serial = 0, revision = 0;
        function id(prefix) { serial += 1; return prefix + serial; }
        function notify() { revision += 1; listeners.slice().forEach(function (fn) { fn(revision); }); }
        function manifiesto() {
            return {
                raices: roots.map(function (r) { return {seleccionId:r.seleccionId, idCliente:r.idCliente, nombre:r.nombre}; }),
                archivos: files.map(function (f) { return {idCliente:f.idCliente, raizIdCliente:f.raizIdCliente,
                    nombre:f.nombre, rutaRelativa:f.rutaRelativa, bytes:f.bytes, ultimaModificacionLocal:f.ultimaModificacionLocal}; })
            };
        }
        function agregarSeleccion(selection) {
            var selected = Array.prototype.slice.call(selection || []), name, paths = Object.create(null);
            // Cancelar no equivale a seleccionar una carpeta vacía. No se infieren directorios.
            if (!selected.length) { return null; }
            selected.forEach(function (file) {
                var path = file.webkitRelativePath, parts;
                if (typeof path !== "string" || path.indexOf("\\") !== -1 || path.indexOf(":") !== -1 || /[\x00-\x1f\x7f]/.test(path)) {
                    throw error("RUTA_RELATIVA_INVALIDA");
                }
                parts = path.split("/");
                if (parts.length < 2 || parts.some(function (p) { return !p || p === "." || p === ".."; }) || parts[parts.length - 1] !== file.name) {
                    throw error("RUTA_RELATIVA_INVALIDA");
                }
                if (name !== undefined && name !== parts[0]) { throw error("SELECCION_MULTIPLES_RAICES"); }
                name = parts[0];
                var canonical = typeof path.normalize === "function" ? path.normalize("NFC") : path;
                if (paths[canonical]) { throw error("RUTA_DUPLICADA"); }
                paths[canonical] = true;
                if (typeof file.size !== "number" || !isFinite(file.size) || Math.floor(file.size) !== file.size || file.size < 0 || file.size > 9007199254740991 ||
                    typeof file.lastModified !== "number" || !isFinite(file.lastModified) || Math.floor(file.lastModified) !== file.lastModified || file.lastModified < 0 || file.lastModified > 9007199254740991) {
                    throw error("METADATOS_ARCHIVO_INVALIDOS");
                }
            });
            var root = {seleccionId:id("s"), idCliente:id("r"), nombre:name};
            var additions = selected.map(function (file) { return {idCliente:id("a"), raizIdCliente:root.idCliente,
                nombre:file.name, rutaRelativa:file.webkitRelativePath, bytes:String(file.size),
                ultimaModificacionLocal:String(file.lastModified), file:file}; });
            roots.push(root); files = files.concat(additions); notify();
            return root.idCliente;
        }
        function quitarRaiz(rootId) {
            if (!roots.some(function (r) { return r.idCliente === rootId; })) { return false; }
            roots = roots.filter(function (r) { return r.idCliente !== rootId; });
            files = files.filter(function (f) { return f.raizIdCliente !== rootId; }); notify(); return true;
        }
        function quitarArchivo(fileId) {
            var found = files.filter(function (f) { return f.idCliente === fileId; })[0];
            if (!found) { return false; }
            files = files.filter(function (f) { return f.idCliente !== fileId; });
            // Una raíz sin archivos no representa una carpeta vacía seleccionada.
            if (!files.some(function (f) { return f.raizIdCliente === found.raizIdCliente; })) {
                roots = roots.filter(function (r) { return r.idCliente !== found.raizIdCliente; });
            }
            notify(); return true;
        }
        return {agregarSeleccion:agregarSeleccion, quitarRaiz:quitarRaiz, quitarArchivo:quitarArchivo,
            manifiesto:manifiesto, revision:function () { return revision; },
            obtenerFile:function (fileId) { var f = files.filter(function (item) { return item.idCliente === fileId; })[0]; return f ? f.file : null; },
            alCambiar:function (fn) { listeners.push(fn); return function () { listeners = listeners.filter(function (item) { return item !== fn; }); }; }};
    }
    function soportaDirectorios(input) { return !!input && "webkitdirectory" in input; }
    function vincularSelector(input, inventory, onError) {
        if (!soportaDirectorios(input)) { throw error("SELECCION_DIRECTORIOS_NO_DISPONIBLE"); }
        input.setAttribute("webkitdirectory", ""); input.setAttribute("multiple", "");
        function change() {
            try { inventory.agregarSeleccion(input.files); }
            catch (e) { if (onError) { onError(e); } else { throw e; } }
            finally { input.value = ""; }
        }
        input.addEventListener("change", change);
        return {abrir:function () { input.value = ""; input.click(); },
            destruir:function () { input.removeEventListener("change", change); }};
    }
    return {crear:crear, soportaDirectorios:soportaDirectorios, vincularSelector:vincularSelector};
}));
