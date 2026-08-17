(function (window, document) {
    "use strict";

    var previewUrl = "../webservice/WebServiceWorkflowModern.asmx/PreviewEnviarTarea";
    var api = {};
    var requestSequence = 0;
    var activeControl = null;

    function isObject(value) {
        return value !== null && typeof value === "object" && !Array.isArray(value);
    }

    function asText(value, fallback) {
        var text = value === undefined || value === null ? "" : String(value).replace(/^\s+|\s+$/g, "");
        return text || (fallback || "");
    }

    function asPositiveInteger(value) {
        var parsed = Number(value);
        return isFinite(parsed) && parsed > 0 && Math.floor(parsed) === parsed ? parsed : 0;
    }

    function asArray(value) {
        return Array.isArray(value) ? value : [];
    }

    function normalizarDestino(raw, index) {
        raw = isObject(raw) ? raw : {};
        return {
            id: asPositiveInteger(raw.Id),
            nombre: asText(raw.Nombre, "Destino disponible"),
            destinatario: asText(raw.Destinatario),
            grupo: asText(raw.Grupo),
            tipo: asText(raw.Tipo, "No especificado"),
            orden: asPositiveInteger(raw.Orden) || index + 1
        };
    }

    function normalizarPrevisualizacion(raw) {
        var contexto;
        var error;

        if (!isObject(raw)) {
            throw new Error("La respuesta de previsualización no tiene el formato esperado.");
        }

        contexto = isObject(raw.Contexto) ? raw.Contexto : {};
        error = isObject(raw.Error) ? raw.Error : null;

        return {
            idTarea: asPositiveInteger(raw.IdTarea),
            tipoDecision: asText(raw.TipoDecision, "No especificado"),
            contexto: {
                radicado: asText(contexto.Radicado, "No disponible"),
                grupoActual: asText(contexto.GrupoActual, "No disponible")
            },
            destinos: asArray(raw.Destinos).map(normalizarDestino).sort(function (left, right) {
                return left.orden - right.orden;
            }),
            tokenVersion: asText(raw.TokenVersion),
            error: error ? {
                codigo: asText(error.Codigo),
                mensajeVisible: asText(error.MensajeVisible, "No fue posible cargar los destinos.")
            } : null
        };
    }

    function desempaquetarRespuestaAsmx(raw) {
        var contenido;

        if (!isObject(raw) || !Object.prototype.hasOwnProperty.call(raw, "d")) {
            throw new Error("La respuesta del servicio no contiene el envoltorio ASMX esperado.");
        }

        contenido = raw.d;
        if (typeof contenido === "string") {
            try {
                contenido = JSON.parse(contenido);
            } catch (error) {
                throw new Error("La respuesta del servicio no contiene JSON válido.");
            }
        }

        return normalizarPrevisualizacion(contenido);
    }

    async function solicitarPrevisualizacion(idTarea, fetchImplementation) {
        var respuesta;
        var contenido;

        fetchImplementation = fetchImplementation || window.fetch;
        if (typeof fetchImplementation !== "function") {
            throw new Error("Este navegador no permite cargar destinos de forma segura.");
        }

        respuesta = await fetchImplementation(previewUrl, {
            method: "POST",
            credentials: "same-origin",
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify({ idTarea: idTarea })
        });

        if (!respuesta) {
            throw new Error("No fue posible consultar los destinos.");
        }
        if (respuesta.ok === false) {
            throw new Error("No fue posible consultar los destinos (HTTP " + asText(respuesta.status, "no disponible") + ").");
        }

        try {
            contenido = await respuesta.json();
        } catch (error) {
            throw new Error("El servicio devolvió una respuesta no válida.");
        }
        return desempaquetarRespuestaAsmx(contenido);
    }

    function vaciar(node) {
        while (node && node.firstChild) {
            node.removeChild(node.firstChild);
        }
    }

    function crearElemento(tagName, className, text) {
        var element = document.createElement(tagName);
        if (className) {
            element.className = className;
        }
        if (text !== undefined) {
            element.textContent = text;
        }
        return element;
    }

    function crearDefinicion(contexto, etiqueta, valor) {
        contexto.appendChild(crearElemento("dt", "workflow-transition-modal__context-label", etiqueta));
        contexto.appendChild(crearElemento("dd", "workflow-transition-modal__context-value", valor));
    }

    function actualizarEstado(control, state, message, kind) {
        control.modal.setAttribute("data-workflow-transition-state", state);
        control.status.setAttribute("data-workflow-transition-status", kind || "informacion");
        control.status.textContent = message || "";
    }

    function limpiarDestinos(control) {
        vaciar(control.tableBody);
        vaciar(control.cards);
    }

    function renderizarContexto(control, preview) {
        vaciar(control.contexto);
        crearDefinicion(control.contexto, "Radicado", preview.contexto.radicado);
        crearDefinicion(control.contexto, "Tipo", preview.tipoDecision);
        crearDefinicion(control.contexto, "Grupo actual", preview.contexto.grupoActual);
    }

    function descripcionDestinatario(destino) {
        return destino.destinatario || destino.grupo || "No especificado";
    }

    function crearBotonSeleccion(control, destino) {
        var button = crearElemento("button", "workflow-transition-modal__select", "Seleccionar");
        button.type = "button";
        button.disabled = destino.id <= 0;
        button.addEventListener("click", function () {
            seleccionarDestino(control, destino);
        });
        return button;
    }

    function crearFilaDestino(control, destino) {
        var row = document.createElement("tr");
        var cell;

        cell = crearElemento("td", "workflow-transition-modal__destination", destino.nombre);
        row.appendChild(cell);
        row.appendChild(crearElemento("td", "workflow-transition-modal__recipient", descripcionDestinatario(destino)));
        row.appendChild(crearElemento("td", "workflow-transition-modal__type", destino.tipo));
        cell = document.createElement("td");
        cell.className = "workflow-transition-modal__action";
        cell.appendChild(crearBotonSeleccion(control, destino));
        row.appendChild(cell);
        return row;
    }

    function crearTarjetaDestino(control, destino) {
        var card = crearElemento("article", "workflow-transition-modal__card");
        var metadata = crearElemento("dl", "workflow-transition-modal__card-metadata");

        card.appendChild(crearElemento("h3", "workflow-transition-modal__card-title", destino.nombre));
        crearDefinicion(metadata, "Destinatario o grupo", descripcionDestinatario(destino));
        crearDefinicion(metadata, "Tipo", destino.tipo);
        card.appendChild(metadata);
        card.appendChild(crearBotonSeleccion(control, destino));
        return card;
    }

    function renderizarDestinos(control, preview) {
        var index;

        limpiarDestinos(control);
        renderizarContexto(control, preview);
        for (index = 0; index < preview.destinos.length; index += 1) {
            control.tableBody.appendChild(crearFilaDestino(control, preview.destinos[index]));
            control.cards.appendChild(crearTarjetaDestino(control, preview.destinos[index]));
        }
        actualizarEstado(control, "lista-disponible", "Seleccione un destino para continuar.", "informacion");
    }

    function renderizarSinDestinos(control, preview) {
        limpiarDestinos(control);
        renderizarContexto(control, preview);
        actualizarEstado(control, "sin-destinos", "No hay destinos disponibles para esta tarea.", "informacion");
    }

    function renderizarError(control, preview, retry, message) {
        var retryButton;

        limpiarDestinos(control);
        if (preview) {
            renderizarContexto(control, preview);
        } else {
            vaciar(control.contexto);
        }
        message = asText(message, preview && preview.error ? preview.error.mensajeVisible : "No fue posible cargar los destinos. Intente nuevamente.");
        actualizarEstado(control, "error-controlado", message, "error");
        retryButton = crearElemento("button", "workflow-transition-modal__retry", "Reintentar");
        retryButton.type = "button";
        retryButton.addEventListener("click", retry);
        control.status.appendChild(retryButton);
    }

    function abrirModal(control) {
        control.modal.hidden = false;
        control.modal.removeAttribute("hidden");
        control.modal.setAttribute("aria-hidden", "false");
        document.body.classList.add("workflow-transition-modal-open");
        window.setTimeout(function () {
            control.close.focus();
        }, 0);
    }

    function cerrarModal(control) {
        control.modal.hidden = true;
        control.modal.setAttribute("hidden", "hidden");
        control.modal.setAttribute("aria-hidden", "true");
        control.modal.setAttribute("data-workflow-transition-state", "cerrado");
        document.body.classList.remove("workflow-transition-modal-open");
        if (control.trigger && typeof control.trigger.focus === "function") {
            control.trigger.focus();
        }
    }

    function focoNavegable(dialog) {
        var candidates = dialog.querySelectorAll("button:not([disabled]), [href], input:not([disabled]), select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex='-1'])");
        return Array.prototype.filter.call(candidates, function (element) {
            return element.getAttribute("aria-hidden") !== "true" &&
                !element.hidden &&
                (!element.getClientRects || element.getClientRects().length > 0);
        });
    }

    function controlarTeclado(control, event) {
        var elements;
        var first;
        var last;

        if (event.key === "Escape" || event.keyCode === 27) {
            event.preventDefault();
            cerrarModal(control);
            return;
        }

        if (event.key !== "Tab" && event.keyCode !== 9) {
            return;
        }

        elements = focoNavegable(control.dialog);
        if (elements.length === 0) {
            event.preventDefault();
            control.dialog.focus();
            return;
        }

        first = elements[0];
        last = elements[elements.length - 1];
        if (event.shiftKey && document.activeElement === first) {
            event.preventDefault();
            last.focus();
        } else if (!event.shiftKey && document.activeElement === last) {
            event.preventDefault();
            first.focus();
        }
    }

    function crearDetalleSeleccion(preview, destino) {
        return {
            idTarea: preview.idTarea,
            idConector: destino.id,
            tokenVersion: preview.tokenVersion,
            tipoDecision: preview.tipoDecision,
            contexto: {
                radicado: preview.contexto.radicado,
                grupoActual: preview.contexto.grupoActual
            },
            destino: {
                nombre: destino.nombre,
                destinatario: destino.destinatario,
                grupo: destino.grupo,
                tipo: destino.tipo
            }
        };
    }

    function emitirSeleccion(control, destino) {
        var detail = crearDetalleSeleccion(control.preview, destino);
        var event;

        if (typeof api.onDestinationSelected === "function") {
            try {
                api.onDestinationSelected(detail);
            } catch (ignored) {
                //El adaptador de confirmación no puede bloquear ni ejecutar una transición desde aquí.
            }
        }

        if (typeof window.CustomEvent === "function") {
            event = new window.CustomEvent("workflow:destination-selected", { detail: detail });
        } else {
            event = document.createEvent("CustomEvent");
            event.initCustomEvent("workflow:destination-selected", false, false, detail);
        }
        window.dispatchEvent(event);
    }

    function seleccionarDestino(control, destino) {
        if (!control.preview || destino.id <= 0) {
            return;
        }

        emitirSeleccion(control, destino);
        actualizarEstado(control, "destino-seleccionado", "Destino seleccionado: " + destino.nombre + ".", "exito");
    }

    function aplicarTransicionExitosa(detail) {
        var preview = activeControl && activeControl.preview;

        if (!preview || !detail || preview.idTarea !== detail.idTarea || preview.tokenVersion !== detail.tokenVersion) {
            return false;
        }

        requestSequence += 1;
        activeControl.preview = null;
        limpiarDestinos(activeControl);
        vaciar(activeControl.contexto);
        cerrarModal(activeControl);
        return true;
    }

    function idTareaActual(control) {
        var inputIds = [
            control.trigger.getAttribute("data-workflow-current-task-input-id"),
            control.trigger.getAttribute("data-workflow-task-input-id")
        ];
        var index;
        var input;
        var idTarea;

        for (index = 0; index < inputIds.length; index += 1) {
            input = inputIds[index] ? document.getElementById(inputIds[index]) : null;
            idTarea = input ? asPositiveInteger(input.value) : 0;
            if (idTarea > 0) {
                return idTarea;
            }
        }
        return 0;
    }

    async function cargarDestinos(control) {
        var idTarea = idTareaActual(control);
        var sequence;
        var preview;

        if (idTarea <= 0) {
            renderizarError(control, null, function () { cargarDestinos(control); }, "Seleccione una tarea activa antes de continuar.");
            return;
        }

        sequence = requestSequence + 1;
        requestSequence = sequence;
        control.preview = null;
        limpiarDestinos(control);
        vaciar(control.contexto);
        actualizarEstado(control, "cargando", "Cargando destinos disponibles…", "informacion");

        try {
            preview = await solicitarPrevisualizacion(idTarea);
            if (sequence !== requestSequence) {
                return;
            }

            control.preview = preview;
            if (preview.error && preview.error.codigo === "WORKFLOW_NO_DESTINATIONS") {
                renderizarSinDestinos(control, preview);
            } else if (preview.error) {
                renderizarError(control, preview, function () { cargarDestinos(control); }, preview.error.mensajeVisible);
            } else if (preview.destinos.length === 0) {
                renderizarSinDestinos(control, preview);
            } else {
                renderizarDestinos(control, preview);
            }
        } catch (error) {
            if (sequence === requestSequence) {
                renderizarError(control, null, function () { cargarDestinos(control); }, error && error.message);
            }
        }
    }

    function interceptarContinuar(control, event) {
        if (event) {
            event.preventDefault();
            event.stopPropagation();
        }
        abrirModal(control);
        cargarDestinos(control);
        return false;
    }

    function inicializar() {
        var trigger = document.getElementById("workflow-transition-trigger");
        var control;

        if (!trigger || trigger.getAttribute("data-workflow-modern-active") !== "true") {
            return;
        }
        if (trigger.getAttribute("data-workflow-modern-bound") === "true") {
            return;
        }

        control = {
            trigger: trigger,
            modal: document.getElementById("workflow-transition-modern-modal"),
            dialog: document.getElementById("workflow-transition-modern-dialog"),
            close: document.getElementById("workflow-transition-modern-close"),
            status: document.getElementById("workflow-transition-modern-status"),
            contexto: document.getElementById("workflow-transition-modern-context"),
            tableBody: document.getElementById("workflow-transition-modern-table-body"),
            cards: document.getElementById("workflow-transition-modern-cards"),
            preview: null
        };

        if (!control.modal || !control.dialog || !control.close || !control.status || !control.contexto || !control.tableBody || !control.cards) {
            return;
        }

        activeControl = control;

        trigger.setAttribute("data-workflow-modern-bound", "true");
        trigger.onclick = function (event) {
            return interceptarContinuar(control, event || window.event);
        };
        control.close.addEventListener("click", function () { cerrarModal(control); });
        control.modal.addEventListener("click", function (event) {
            if (event.target && event.target.getAttribute("data-workflow-transition-close") === "true") {
                cerrarModal(control);
            }
        });
        control.dialog.addEventListener("keydown", function (event) { controlarTeclado(control, event); });
    }

    api.normalizarPrevisualizacion = normalizarPrevisualizacion;
    api.desempaquetarRespuestaAsmx = desempaquetarRespuestaAsmx;
    api.solicitarPrevisualizacion = solicitarPrevisualizacion;
    api.crearDetalleSeleccion = crearDetalleSeleccion;
    api.aplicarTransicionExitosa = aplicarTransicionExitosa;
    api.inicializar = inicializar;
    api.onDestinationSelected = null;
    window.WorkflowTransitionUi = api;

    if (window.Sys && window.Sys.Application && typeof window.Sys.Application.add_load === "function") {
        window.Sys.Application.add_load(inicializar);
    }

    if (document && typeof document.getElementById === "function") {
        if (document.readyState === "loading") {
            document.addEventListener("DOMContentLoaded", inicializar);
        } else {
            inicializar();
        }
    }
}(window, document));
