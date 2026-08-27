const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const source = fs.readFileSync(path.resolve(__dirname, "..", "js", "java_general", "general_code_java.js"), "utf8");

test("general_code_java usa attachEvent cuando jQuery no admite .on", () => {
    const attached = [];
    const document = {
        attachEvent(name, handler) { attached.push({ name, handler }); }
    };

    vm.runInNewContext(source, {
        window: { jQuery: function jQueryIncompatible() {} },
        document
    });

    assert.deepEqual(attached.map((event) => event.name), ["onkeydown", "onclick"]);
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
