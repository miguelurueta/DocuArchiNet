const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const sourcePath = path.resolve(__dirname, "../js/java_general/ConfirmationDialog.js");
const source = fs.readFileSync(sourcePath, "utf8");

function createEnvironment() {
    const listeners = new Map();
    const document = { activeElement: null };

    function createElement(tagName) {
        const attributes = new Map();
        const elementListeners = new Map();
        const element = {
            tagName: String(tagName).toUpperCase(),
            children: [],
            className: "",
            disabled: false,
            hidden: false,
            parentNode: null,
            _text: "",
            appendChild(child) {
                child.parentNode = this;
                this.children.push(child);
                return child;
            },
            removeChild(child) {
                this.children.splice(this.children.indexOf(child), 1);
                child.parentNode = null;
                return child;
            },
            setAttribute(name, value) { attributes.set(name, String(value)); },
            getAttribute(name) { return attributes.has(name) ? attributes.get(name) : null; },
            removeAttribute(name) { attributes.delete(name); },
            addEventListener(name, callback) { elementListeners.set(name, callback); },
            emit(name, event = {}) {
                const callback = elementListeners.get(name);
                if (callback) {
                    callback(Object.assign({ target: this, preventDefault() {} }, event));
                }
            },
            focus() { document.activeElement = this; },
            querySelectorAll() {
                const result = [];
                function walk(node) {
                    node.children.forEach((child) => {
                        if (child.tagName === "BUTTON" && !child.disabled) {
                            result.push(child);
                        }
                        walk(child);
                    });
                }
                walk(this);
                return result;
            }
        };
        Object.defineProperty(element, "firstChild", {
            get() { return this.children[0] || null; }
        });
        Object.defineProperty(element, "textContent", {
            get() { return this._text + this.children.map((child) => child.textContent).join(""); },
            set(value) { this._text = String(value); }
        });
        return element;
    }

    document.createElement = createElement;
    document.body = createElement("body");
    document.body.classList = {
        values: new Set(),
        add(value) { this.values.add(value); },
        remove(value) { this.values.delete(value); }
    };

    const window = {
        document,
        setTimeout(callback) { callback(); },
        addEventListener(name, callback) { listeners.set(name, callback); },
        dispatchEvent() {}
    };
    return { window, document, listeners };
}

function loadDialog() {
    const environment = createEnvironment();
    vm.runInNewContext(source, {
        window: environment.window,
        document: environment.document,
        Promise,
        JSON,
        Array,
        Object,
        Number,
        String,
        Error
    });
    return { ...environment, api: environment.window.ConfirmationDialog };
}

function findByClass(root, className) {
    if (root.className === className) {
        return root;
    }
    for (const child of root.children) {
        const found = findByClass(child, className);
        if (found) {
            return found;
        }
    }
    return null;
}

function config(overrides = {}) {
    return Object.assign({
        title: "Confirmación externa",
        primaryLabel: "Aceptar",
        cancelLabel: "Volver",
        summaryFields: [{ label: "Referencia", value: "EXT-41" }],
        executionContext: { external: true },
        execute: async () => ({ ok: true }),
        normalizeResult: () => ({ status: "success", message: "Completado", warnings: [], canRetry: false })
    }, overrides);
}

test("abre y cierra con un consumidor que no conoce Workflow", () => {
    const { api, document } = loadDialog();
    const trigger = document.createElement("button");
    document.activeElement = trigger;

    api.open(config());
    const root = document.body.children[0];
    assert.equal(root.hidden, false);
    assert.match(root.textContent, /Confirmación externa/);
    assert.match(root.textContent, /EXT-41/);
    assert.doesNotMatch(source, /Webworkflow\.aspx|GridView_envia_flujo|Terminar_Tarea_Workflow/);

    api.close();
    assert.equal(root.hidden, true);
    assert.equal(document.activeElement, trigger);
});

test("omite secciones vacías y representa contenido como texto", () => {
    const { api, document } = loadDialog();
    api.open(config({
        summaryFields: [{ label: "Referencia", value: "<b>EXT-41</b>" }, { label: "", value: "No mostrar" }],
        requirements: [],
        warnings: []
    }));

    const root = document.body.children[0];
    assert.match(root.textContent, /<b>EXT-41<\/b>/);
    assert.equal(findByClass(root, "confirmation-dialog__requirements").hidden, true);
    assert.equal(findByClass(root, "confirmation-dialog__warnings").hidden, true);
    assert.doesNotMatch(source, /innerHTML/);
});

test("atrapa el foco y cancela con Escape", () => {
    const { api, document, listeners } = loadDialog();
    const trigger = document.createElement("button");
    let cancelled = 0;
    document.activeElement = trigger;
    api.open(config({ onCancel() { cancelled += 1; } }));

    const root = document.body.children[0];
    const dialog = findByClass(root, "confirmation-dialog__surface");
    const close = findByClass(root, "confirmation-dialog__close");
    const primary = findByClass(root, "confirmation-dialog__primary");
    document.activeElement = close;
    dialog.emit("keydown", { key: "Tab", shiftKey: true });
    assert.equal(document.activeElement, primary);

    const escapeEvent = {
        key: "Escape",
        prevented: false,
        propagationStopped: false,
        preventDefault() { this.prevented = true; },
        stopPropagation() { this.propagationStopped = true; }
    };
    listeners.get("keydown")(escapeEvent);
    assert.equal(root.hidden, true);
    assert.equal(cancelled, 1);
    assert.equal(document.activeElement, trigger);
    assert.equal(escapeEvent.prevented, true);
    assert.equal(escapeEvent.propagationStopped, true);
});

test("bloquea doble envío y no permite cerrar, reemplazar o recargar durante el envío", async () => {
    const { api, document, listeners } = loadDialog();
    let executeCalls = 0;
    let resolveFirst;
    let successCalls = 0;
    let cancelled = 0;
    const opening = api.open(config({
        execute() {
            executeCalls += 1;
            return new Promise((resolve) => { resolveFirst = resolve; });
        },
        onSuccess() { successCalls += 1; },
        onCancel() { cancelled += 1; }
    }));

    const root = document.body.children[0];
    const primary = findByClass(root, "confirmation-dialog__primary");
    const close = findByClass(root, "confirmation-dialog__close");
    const cancel = findByClass(root, "confirmation-dialog__cancel");
    const backdrop = findByClass(root, "confirmation-dialog__backdrop");
    const dialog = findByClass(root, "confirmation-dialog__surface");
    primary.emit("click");
    primary.emit("click");
    assert.equal(executeCalls, 0);
    await Promise.resolve();
    assert.equal(executeCalls, 1);
    assert.equal(primary.disabled, true);
    assert.equal(close.disabled, true);
    assert.equal(cancel.disabled, true);

    close.emit("click");
    cancel.emit("click");
    root.emit("click", { target: backdrop });
    listeners.get("keydown")({ key: "Escape", preventDefault() {}, stopPropagation() {} });
    assert.equal(api.close(), false);
    const replacement = api.open(config());
    assert.equal(replacement.id, opening.id);
    assert.equal(replacement.pending, true);
    assert.equal(root.hidden, false);
    assert.equal(cancelled, 0);
    assert.match(findByClass(root, "confirmation-dialog__status").textContent, /Espere la respuesta antes de cerrar/);

    const unloadEvent = { prevented: false, returnValue: null, preventDefault() { this.prevented = true; } };
    assert.equal(listeners.get("beforeunload")(unloadEvent), "");
    assert.equal(unloadEvent.prevented, true);
    assert.equal(unloadEvent.returnValue, "");

    resolveFirst({ ok: true });
    await new Promise((resolve) => setTimeout(resolve, 0));
    assert.equal(successCalls, 1);
    assert.equal(root.getAttribute("data-confirmation-dialog-state"), "exito");
    assert.equal(api.close(), true);
    assert.equal(root.hidden, true);
});

test("reemplaza el detalle técnico de red por el mensaje seguro configurado", async () => {
    const { api, document } = loadDialog();
    api.open(config({
        labels: { technicalError: "No fue posible enviar la tarea. Intente nuevamente." },
        execute() { return Promise.reject(new Error("Failed to fetch")); }
    }));

    const root = document.body.children[0];
    const primary = findByClass(root, "confirmation-dialog__primary");
    primary.emit("click");
    await new Promise((resolve) => setTimeout(resolve, 0));

    const status = findByClass(root, "confirmation-dialog__status");
    assert.equal(status.textContent, "No fue posible enviar la tarea. Intente nuevamente.");
    assert.doesNotMatch(status.textContent, /Failed to fetch/);
    assert.equal(primary.disabled, false);
});
