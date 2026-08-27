const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const source = fs.readFileSync(path.resolve(__dirname, "..", "js", "java_general", "general_code_java.js"), "utf8");
const workflowDirectory = path.resolve(__dirname, "..", "workflow");

test("general_code_java usa attachEvent cuando jQuery no admite .on", () => {
    const attached = [];
    const document = {
        attachEvent(name, handler) { attached.push({ name, handler }); },
        getElementById() { return null; }
    };
    const window = {
        event: { keyCode: 0, srcElement: { id: "otro-control" } },
        jQuery: function jQueryIncompatible() {}
    };

    vm.runInNewContext(source, {
        window,
        document
    });

    assert.deepEqual(attached.map((event) => event.name), ["onkeydown", "onclick"]);
    assert.doesNotThrow(() => attached[0].handler());
    assert.doesNotThrow(() => attached[1].handler());
});

test("general_code_java usa jQuery solo cuando expone .on", () => {
    const registered = [];
    const document = {
        addEventListener() { throw new Error("No debe usar addEventListener cuando jQuery es compatible"); },
        attachEvent() { throw new Error("No debe usar attachEvent cuando jQuery es compatible"); }
    };
    function jQuery(target) {
        assert.equal(target, document);
        return { on(name, handler) { registered.push({ name, handler }); } };
    }
    jQuery.fn = { on() {} };

    vm.runInNewContext(source, { window: { jQuery }, document });

    assert.deepEqual(registered.map((event) => event.name), ["keydown", "click"]);
});

test("Workflow solicita una versión nueva del script global en todas sus páginas", () => {
    const pages = fs.readdirSync(workflowDirectory)
        .filter((name) => name.endsWith(".aspx"))
        .map((name) => ({ name, content: fs.readFileSync(path.join(workflowDirectory, name), "utf8") }))
        .filter((page) => page.content.includes("general_code_java.js"));

    assert.ok(pages.length > 0);
    for (const page of pages) {
        const references = page.content.match(/general_code_java\.js[^"']*/g) || [];
        assert.ok(references.length > 0, `${page.name} debe referenciar el script global`);
        for (const reference of references) {
            assert.match(reference, /\?v=20260827-compatible-events5$/, `${page.name} no puede dejar el script global sin versión`);
        }
    }
});
