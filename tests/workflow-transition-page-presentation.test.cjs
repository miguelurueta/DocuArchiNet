const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const sourcePath = path.resolve(__dirname, "../js/workflow/workflow-transition-page-presentation.js");
const source = fs.readFileSync(sourcePath, "utf8");

function element(attributes = {}) {
    const values = new Map(Object.entries(attributes));
    return {
        textContent: "",
        hidden: false,
        style: { display: "" },
        parentNode: null,
        removedAttributes: [],
        setAttribute(name, value) { values.set(name, String(value)); },
        getAttribute(name) { return values.has(name) ? values.get(name) : null; },
        removeAttribute(name) { this.removedAttributes.push(name); values.delete(name); }
    };
}

function loadPagePresentation() {
    const row = element({ "data-workflow-task-id": "41" });
    const parent = {
        children: [row],
        removeChild(child) { this.children.splice(this.children.indexOf(child), 1); child.parentNode = null; }
    };
    row.parentNode = parent;
    const counter = element({ "data-workflow-task-count": "true" });
    counter.textContent = "Tareas de Grupo=(3)";
    const context = element({ "data-workflow-task-context": "true" });
    const viewer = element({ "data-workflow-task-viewer": "true", src: "visor.aspx" });
    const list = element({ "data-workflow-task-list": "true" });
    list.hidden = true;
    list.style.display = "none";
    const scroller = element({ "data-workflow-task-scroll": "true" });
    scroller.scrollLeft = 236;
    const actions = [element({ "data-workflow-task-action": "true" }), element({ "data-workflow-task-action": "true" })];
    const toggles = [element({ "data-workflow-task-toggle": "true" })];
    const success = element({ "data-workflow-transition-success": "true", hidden: "hidden" });
    success.hidden = true;
    const userSuccess = element({ "data-workflow-user-send-success": "true", hidden: "hidden" });
    userSuccess.hidden = true;
    const document = {
        querySelectorAll(selector) {
            if (selector === "[data-workflow-task-id]") return parent.children;
            if (selector === "[data-workflow-task-action]") return actions;
            if (selector === "[data-workflow-task-toggle]") return toggles;
            return [];
        },
        querySelector(selector) {
            return {
                "[data-workflow-task-count]": counter,
                "[data-workflow-task-context]": context,
                "[data-workflow-task-viewer]": viewer,
                "[data-workflow-task-list]": list,
                "[data-workflow-task-scroll]": scroller,
                "[data-workflow-transition-success]": success
            }[selector] || null;
        },
        getElementById(id) { return id === "workflow-user-send-success-message" ? userSuccess : null; }
    };
    const timers = [];
    const window = {
        setTimeout(callback, delay) {
            timers.push({ callback, delay, cleared: false });
            return timers.length;
        },
        clearTimeout(identifier) {
            if (timers[identifier - 1]) timers[identifier - 1].cleared = true;
        },
        auto_zise_popup_workflow(argument) { window.layoutRefreshes.push(argument); },
        layoutRefreshes: []
    };
    vm.runInNewContext(source, { window, document, Number, String, Math, isFinite });
    return { api: window.WorkflowTransitionPagePresentation, parent, counter, context, viewer, list, scroller, actions, toggles, success, userSuccess, timers, layoutRefreshes: window.layoutRefreshes };
}

test("actualiza solo la representación de la tarea confirmada mediante atributos data", () => {
    const { api, parent, counter, context, viewer, list, scroller, actions, toggles, success, timers, layoutRefreshes } = loadPagePresentation();
    assert.equal(api.applySuccess({ idTarea: 41, tokenVersion: "v-41" }), true);
    assert.equal(parent.children.length, 0);
    assert.equal(counter.textContent, "Tareas de Grupo=(2)");
    assert.equal(context.hidden, true);
    assert.equal(context.getAttribute("aria-hidden"), "true");
    assert.ok(viewer.removedAttributes.includes("src"));
    assert.equal(list.hidden, false);
    assert.equal(list.style.display, "block");
    assert.equal(scroller.scrollLeft, 0);
    assert.deepEqual(layoutRefreshes, ["1"]);
    assert.ok(actions.every((action) => action.hidden && action.style.display === "none"));
    assert.ok(toggles.every((toggle) => toggle.hidden && toggle.style.display === "none"));
    assert.equal(success.hidden, false);
    assert.match(success.textContent, /enviada correctamente/);
    assert.equal(timers.length, 1);
    assert.equal(timers[0].delay, 6000);
    timers[0].callback();
    assert.equal(success.hidden, true);
    assert.equal(success.getAttribute("hidden"), "hidden");
    assert.equal(success.textContent, "");
});

test("limpia la selección correlacionada aunque la fila ya no esté representada", () => {
    const { api, parent, counter, context, list } = loadPagePresentation();
    assert.equal(api.applySuccess({ idTarea: 999 }), true);
    assert.equal(parent.children.length, 1);
    assert.equal(counter.textContent, "Tareas de Grupo=(2)");
    assert.equal(context.hidden, true);
    assert.equal(list.hidden, false);
});

test("no cambia la página cuando no hay una tarea válida", () => {
    const { api, parent, counter, context } = loadPagePresentation();
    assert.equal(api.applySuccess({ idTarea: 0 }), false);
    assert.equal(parent.children.length, 1);
    assert.equal(counter.textContent, "Tareas de Grupo=(3)");
    assert.equal(context.hidden, false);
    assert.doesNotMatch(source, /Hidden_id_tarea|eliminar_fila_data_gred_lista|Terminar_Tarea_Workflow|Cambia_Estado/);
});

test("el éxito de usuario usa un mensaje y temporizador propios", () => {
    const { api, success, userSuccess, timers } = loadPagePresentation();
    assert.equal(api.applySuccess({ idTarea: 41 }, {
        successElementId: "workflow-user-send-success-message",
        successMessage: "Tarea enviada al usuario."
    }), true);
    assert.equal(success.hidden, true);
    assert.equal(userSuccess.hidden, false);
    assert.equal(userSuccess.textContent, "Tarea enviada al usuario.");
    assert.equal(timers.length, 1);
});
