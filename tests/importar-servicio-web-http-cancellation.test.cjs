const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const read = (file) => fs.readFileSync(path.join(root, "Infrastructure", "Workflow", "ImportarServicioWeb", "Http", file), "utf8");

test("distingue token del llamador y timeout enlazado", () => {
  const source = read("ExternalImportHttpTransport.vb");
  assert.match(source, /CancellationTokenSource\(prepared\.Timeout\)/);
  assert.match(source, /CreateLinkedTokenSource\(cancellationToken, timeoutSource\.Token\)/);
  assert.match(source, /cancellationToken\.IsCancellationRequested/);
  assert.doesNotMatch(source, /\.Result\b|\.Wait\s*\(|GetAwaiter\(\)\.GetResult|Task\.Run/);
});

test("lee por bloques con cancelacion y limite real", () => {
  const source = read("ExternalImportHttpResponseValidator.vb");
  assert.match(source, /ContentLength/);
  assert.match(source, /ReadAsync\(buffer, 0, buffer\.Length, cancellationToken\)/);
  assert.match(source, /If total > maximumBytes Then Throw Invalid/);
  assert.match(source, /allowedMediaTypes\.Contains\(mediaType\)/);
});

test("el contenido invalido no es un fixture JSON valido", () => {
  const raw = fs.readFileSync(path.join(root, "Tests", "Fixtures", "Workflow", "ImportarServicioWeb", "http-v1", "invalid-response.json"), "utf8");
  assert.throws(() => JSON.parse(raw));
});
