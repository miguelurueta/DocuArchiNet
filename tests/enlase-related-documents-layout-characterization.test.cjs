const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const serverSource = fs.readFileSync(path.join(root, "Docuarchi/ClassDaGabinete.vb"), "utf8");
const javascriptSource = fs.readFileSync(path.join(root, "js/java_general/GredviewControl.js"), "utf8");

function extract(source, startMarker, endMarker) {
  const start = source.indexOf(startMarker);
  const end = source.indexOf(endMarker, start);

  assert.notEqual(start, -1, `No se encontro ${startMarker}`);
  assert.notEqual(end, -1, `No se encontro ${endMarker}`);
  return source.slice(start, end);
}

const serverRenderer = extract(
  serverSource,
  "Function Lista_documentos_relacionados_a_radicado_enlace",
  "End Function"
);
const javascriptRenderer = javascriptSource;

test("ENLASE mantiene el menu en la misma fila tanto en servidor como al importar desde SII", () => {
  assert.match(serverRenderer, /class", "w-100 col-10 pl-2 row"/);
  assert.match(serverRenderer, /Style\.Add\("margin-right", "0px"\)/);
  assert.doesNotMatch(serverRenderer, /Style\.Add\("margin-right", "1px"\)/);

  assert.match(javascriptRenderer, /class="w-100 col-10 pl-2 row" style="margin-right:0px;"/);
  assert.match(serverRenderer, /class", "col-2 p-0 nav-item dropdown active"/);
  assert.match(javascriptRenderer, /class="col-2 p-0 nav-item dropdown active"/);
});
