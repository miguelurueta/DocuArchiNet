(function (window, document) {
    "use strict";

    var activeDialog = null;
    var openingSequence = 0;
    var api = {};

    function isObject(value) {
        return value !== null && typeof value === "object" && !Array.isArray(value);
    }

    function asText(value, fallback) {
        var text = value === undefined || value === null ? "" : String(value).replace(/^\s+|\s+$/g, "");
        return text || (fallback || "");
    }

    function asArray(value) {
        return Array.isArray(value) ? value : [];
    }

    function asCallback(value) {
        return typeof value === "function" ? value : function () {};
    }

    function createElement(tagName, className, text) {
        var element = document.createElement(tagName);
        if (className) {
            element.className = className;
        }
        if (text !== undefined) {
            element.textContent = text;
        }
        return element;
    }

    function empty(node) {
        while (node && node.firstChild) {
            node.removeChild(node.firstChild);
        }
    }

    function normalizeFields(fields) {
        return asArray(fields).map(function (field) {
            field = isObject(field) ? field : {};
            return {
                label: asText(field.label),
                value: asText(field.value)
            };
        }).filter(function (field) {
            return field.label && field.value;
        });
    }

    function normalizeItems(items) {
        return asArray(items).map(function (item) {
            if (isObject(item)) {
                return asText(item.descripcion || item.description || item.mensaje || item.message);
            }
            return asText(item);
        }).filter(function (item) {
            return !!item;
        });
    }

    function normalizeConfig(config) {
        config = isObject(config) ? config : {};
        if (typeof config.execute !== "function") {
            throw new Error("ConfirmationDialog requiere una función execute.");
        }
        if (typeof config.normalizeResult !== "function") {
            throw new Error("ConfirmationDialog requiere una función normalizeResult.");
        }

        return {
            title: asText(config.title, "Confirmar acción"),
            primaryLabel: asText(config.primaryLabel, "Confirmar"),
            cancelLabel: asText(config.cancelLabel, "Cancelar"),
            closeLabel: asText(config.labels && config.labels.close, "Cerrar"),
            sendingLabel: asText(config.labels && config.labels.sending, "Procesando…"),
            sendingLockedLabel: asText(config.labels && config.labels.sendingLocked, "La operación está en curso. Espere la respuesta antes de cerrar."),
            technicalErrorLabel: asText(config.labels && config.labels.technicalError, "No fue posible completar la operación. Intente nuevamente."),
            summaryFields: normalizeFields(config.summaryFields),
            requirements: normalizeItems(config.requirements),
            warnings: normalizeItems(config.warnings),
            confirmationNotice: asText(config.confirmationNotice),
            executionContext: config.executionContext,
            execute: config.execute,
            normalizeResult: config.normalizeResult,
            onSuccess: asCallback(config.onSuccess),
            onBlocked: asCallback(config.onBlocked),
            onTechnicalError: asCallback(config.onTechnicalError),
            onCancel: asCallback(config.onCancel)
        };
    }

    function addDefinition(list, label, value) {
        list.appendChild(createElement("dt", "confirmation-dialog__field-label", label));
        list.appendChild(createElement("dd", "confirmation-dialog__field-value", value));
    }

    function renderTextList(container, title, items, modifier) {
        var section;
        var list;

        empty(container);
        if (!items.length) {
            container.hidden = true;
            return;
        }

        section = createElement("section", "confirmation-dialog__list " + modifier);
        section.appendChild(createElement("h3", "confirmation-dialog__list-title", title));
        list = createElement("ul", "confirmation-dialog__list-items");
        items.forEach(function (item) {
            list.appendChild(createElement("li", "confirmation-dialog__list-item", item));
        });
        section.appendChild(list);
        container.hidden = false;
        container.appendChild(section);
    }

    function createControl() {
        var root = createElement("div", "confirmation-dialog");
        var backdrop = createElement("div", "confirmation-dialog__backdrop");
        var dialog = createElement("section", "confirmation-dialog__surface");
        var header = createElement("header", "confirmation-dialog__header");
        var title = createElement("h2", "confirmation-dialog__title");
        var close = createElement("button", "confirmation-dialog__close", "×");
        var body = createElement("div", "confirmation-dialog__body");
        var status = createElement("div", "confirmation-dialog__status");
        var summary = createElement("dl", "confirmation-dialog__summary");
        var requirements = createElement("div", "confirmation-dialog__requirements");
        var warnings = createElement("div", "confirmation-dialog__warnings");
        var notice = createElement("p", "confirmation-dialog__notice");
        var footer = createElement("footer", "confirmation-dialog__footer");
        var cancel = createElement("button", "confirmation-dialog__cancel");
        var primary = createElement("button", "confirmation-dialog__primary");
        var control;

        root.hidden = true;
        root.setAttribute("aria-hidden", "true");
        root.setAttribute("data-confirmation-dialog-state", "cerrado");
        backdrop.setAttribute("data-confirmation-dialog-close", "true");
        dialog.setAttribute("role", "dialog");
        dialog.setAttribute("aria-modal", "true");
        dialog.setAttribute("tabindex", "-1");
        title.id = "confirmation-dialog-title";
        dialog.setAttribute("aria-labelledby", title.id);
        close.type = "button";
        cancel.type = "button";
        primary.type = "button";
        status.setAttribute("role", "status");
        status.setAttribute("aria-live", "polite");

        header.appendChild(title);
        header.appendChild(close);
        body.appendChild(status);
        body.appendChild(summary);
        body.appendChild(requirements);
        body.appendChild(warnings);
        body.appendChild(notice);
        footer.appendChild(cancel);
        footer.appendChild(primary);
        dialog.appendChild(header);
        dialog.appendChild(body);
        dialog.appendChild(footer);
        root.appendChild(backdrop);
        root.appendChild(dialog);
        document.body.appendChild(root);

        control = {
            root: root,
            dialog: dialog,
            title: title,
            close: close,
            status: status,
            summary: summary,
            requirements: requirements,
            warnings: warnings,
            notice: notice,
            cancel: cancel,
            primary: primary
        };

        close.addEventListener("click", function () { closeActive(true); });
        cancel.addEventListener("click", function () { closeActive(true); });
        primary.addEventListener("click", submitActive);
        root.addEventListener("click", function (event) {
            if (event.target && event.target.getAttribute("data-confirmation-dialog-close") === "true") {
                closeActive(true);
            }
        });
        dialog.addEventListener("keydown", function (event) { handleKeyboard(control, event); });
        return control;
    }

    function setState(context, state, message, kind) {
        context.control.root.setAttribute("data-confirmation-dialog-state", state);
        context.control.status.setAttribute("data-confirmation-dialog-status", kind || "informacion");
        context.control.status.textContent = asText(message);
    }

    function setActions(context, primaryEnabled, closeEnabled) {
        var canClose = closeEnabled !== false;

        context.control.primary.disabled = !primaryEnabled;
        context.control.cancel.disabled = !canClose;
        context.control.close.disabled = !canClose;
    }

    function renderContext(context) {
        var control = context.control;
        var config = context.config;

        control.title.textContent = config.title;
        control.close.setAttribute("aria-label", config.closeLabel);
        control.cancel.textContent = config.cancelLabel;
        control.primary.textContent = config.primaryLabel;
        empty(control.summary);
        config.summaryFields.forEach(function (field) {
            addDefinition(control.summary, field.label, field.value);
        });
        control.summary.hidden = config.summaryFields.length === 0;
        renderTextList(control.requirements, "Requisitos", config.requirements, "confirmation-dialog__list--requirements");
        renderTextList(control.warnings, "Advertencias", config.warnings, "confirmation-dialog__list--warnings");
        control.notice.textContent = config.confirmationNotice;
        control.notice.hidden = !config.confirmationNotice;
        setActions(context, true);
        setState(context, "confirmando", "", "informacion");
    }

    function focusableElements(control) {
        var selector = "button:not([disabled]), [href], input:not([disabled]), select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex='-1'])";
        var candidates = control.dialog.querySelectorAll ? control.dialog.querySelectorAll(selector) : [control.close, control.cancel, control.primary];
        return Array.prototype.filter.call(candidates, function (element) {
            return element && !element.hidden && element.getAttribute("aria-hidden") !== "true";
        });
    }

    function handleKeyboard(control, event) {
        var elements;
        var first;
        var last;

        if (event.key !== "Tab" && event.keyCode !== 9) {
            return;
        }
        elements = focusableElements(control);
        if (!elements.length) {
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

    function handleGlobalKeyboard(event) {
        if (!activeDialog || !event || (event.key !== "Escape" && event.keyCode !== 27)) {
            return;
        }
        if (typeof event.preventDefault === "function") {
            event.preventDefault();
        }
        if (typeof event.stopPropagation === "function") {
            event.stopPropagation();
        }
        closeActive(true);
    }

    function safeCallback(callback, result) {
        try {
            callback(result);
        } catch (ignored) {
            //Un callback consumidor no debe romper el estado accesible del diálogo.
        }
    }

    function closeContext(context, notifyCancel) {
        if (!context) {
            return;
        }
        if (activeDialog === context) {
            activeDialog = null;
        }
        context.control.root.hidden = true;
        context.control.root.setAttribute("aria-hidden", "true");
        context.control.root.setAttribute("data-confirmation-dialog-state", "cerrado");
        if (document.body && document.body.classList) {
            document.body.classList.remove("confirmation-dialog-open");
        }
        if (context.returnFocus && typeof context.returnFocus.focus === "function") {
            context.returnFocus.focus();
        }
        if (notifyCancel) {
            safeCallback(context.config.onCancel);
        }
    }

    function closeActive(notifyCancel) {
        if (activeDialog && activeDialog.sending) {
            setState(activeDialog, "enviando", activeDialog.config.sendingLockedLabel, "informacion");
            return false;
        }
        closeContext(activeDialog, notifyCancel === true);
        return true;
    }

    function normalizedTechnicalError(context) {
        return {
            status: "technical-error",
            //Los detalles de transporte del navegador no son mensajes funcionales seguros.
            message: context.config.technicalErrorLabel,
            warnings: [],
            canRetry: true,
            reference: ""
        };
    }

    function normalizeVisualResult(context, rawResult) {
        var result = context.config.normalizeResult(rawResult);
        result = isObject(result) ? result : {};
        return {
            status: asText(result.status),
            message: asText(result.message, context.config.technicalErrorLabel),
            warnings: normalizeItems(result.warnings),
            canRetry: result.canRetry === true,
            reference: asText(result.reference),
            raw: result.raw
        };
    }

    function applyVisualResult(context, result) {
        if (activeDialog !== context || result.status === "ignored") {
            return;
        }

        context.sending = false;
        if (result.warnings.length) {
            renderTextList(context.control.warnings, "Advertencias", result.warnings, "confirmation-dialog__list--warnings");
        }
        if (result.status === "success") {
            setActions(context, false);
            setState(context, "exito", result.message, "exito");
            safeCallback(context.config.onSuccess, result);
            return;
        }
        if (result.status === "blocked") {
            setActions(context, result.canRetry);
            setState(context, "bloqueo-funcional", result.message, "bloqueo");
            safeCallback(context.config.onBlocked, result);
            return;
        }

        setActions(context, result.canRetry);
        setState(context, "error-tecnico-controlado", result.message, "error");
        safeCallback(context.config.onTechnicalError, result);
    }

    function submitActive() {
        var context = activeDialog;
        var operation;

        if (!context || context.sending) {
            return;
        }
        context.sending = true;
        operation = context.operation + 1;
        context.operation = operation;
        setActions(context, false, false);
        setState(context, "enviando", context.config.sendingLabel, "informacion");

        Promise.resolve().then(function () {
            return context.config.execute(context.config.executionContext);
        }).then(function (rawResult) {
            var result;
            if (activeDialog !== context || context.operation !== operation) {
                return;
            }
            try {
                result = normalizeVisualResult(context, rawResult);
            } catch (error) {
                result = normalizedTechnicalError(context, error);
            }
            applyVisualResult(context, result);
        }).catch(function (error) {
            if (activeDialog !== context || context.operation !== operation || (error && error.confirmationDialogIgnored)) {
                return;
            }
            applyVisualResult(context, normalizedTechnicalError(context, error));
        });
    }

    function open(config) {
        var context;
        var control;

        if (activeDialog && activeDialog.sending) {
            setState(activeDialog, "enviando", activeDialog.config.sendingLockedLabel, "informacion");
            return { id: activeDialog.id, pending: true };
        }
        closeActive(false);
        control = createControl();
        context = {
            id: openingSequence + 1,
            operation: 0,
            sending: false,
            control: control,
            config: normalizeConfig(config),
            returnFocus: document.activeElement
        };
        openingSequence = context.id;
        activeDialog = context;
        renderContext(context);
        control.root.hidden = false;
        control.root.removeAttribute("hidden");
        control.root.setAttribute("aria-hidden", "false");
        if (document.body && document.body.classList) {
            document.body.classList.add("confirmation-dialog-open");
        }
        window.setTimeout(function () {
            if (activeDialog === context && control.close && typeof control.close.focus === "function") {
                control.close.focus();
            }
        }, 0);
        return { id: context.id };
    }

    api.open = open;
    api.close = function () { return closeActive(false); };
    if (typeof window.addEventListener === "function") {
        //Captura Escape antes de que los modales legacy de la página lo consuman.
        window.addEventListener("keydown", handleGlobalKeyboard, true);
        window.addEventListener("beforeunload", function (event) {
            if (!activeDialog || !activeDialog.sending) {
                return undefined;
            }
            if (event && typeof event.preventDefault === "function") {
                event.preventDefault();
            }
            if (event) {
                event.returnValue = "";
            }
            return "";
        });
    }
    window.ConfirmationDialog = api;
}(window, document));
