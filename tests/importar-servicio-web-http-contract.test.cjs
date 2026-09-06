const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const http = require("node:http");

const root = path.resolve(__dirname, "..");
const read = (...parts) => fs.readFileSync(path.join(root, ...parts), "utf8");
const transport = read("Infrastructure", "Workflow", "ImportarServicioWeb", "Http", "ExternalImportHttpTransport.vb");
const interfaces = read("Modelo", "Workflow", "ImportarServicioWeb", "ImportarServicioWebInterfaces.vb");

test("publica cuatro operaciones asincronas cancelables sin duplicar el puerto", () => {
  assert.equal((interfaces.match(/Interface IExternalImportProviderClient/g) || []).length, 1);
  for (const operation of ["ResolveCapabilitiesAsync", "QueryItemsAsync", "GetPreviewAsync", "DownloadAsync"]) {
    assert.match(interfaces, new RegExp(`Function ${operation}[\\s\\S]*CancellationToken[\\s\\S]*Task\\(Of`));
  }
});

test("preserva bytes, media type y encabezados de cada solicitud", () => {
  assert.match(transport, /New ByteArrayContent\(body\)/);
  assert.match(transport, /MediaTypeHeaderValue\.Parse\(prepared\.ContentType\)/);
  assert.match(transport, /TryAddWithoutValidation\(header\.Key, header\.Value\)/);
  assert.doesNotMatch(transport, /FormUrlEncodedContent|JsonConvert\.SerializeObject/);
});

test("registra exactamente los cuatro archivos canonicos", () => {
  const project = read("GestionDocumental-Docuarchi.net.vbproj");
  for (const file of ["ExternalImportHttpClientFactory.vb", "ExternalImportHttpErrorMapper.vb", "ExternalImportHttpResponseValidator.vb", "ExternalImportHttpTransport.vb"]) {
    assert.equal(project.split(file).length - 1, 1, file);
  }
});

test("los fixtures son locales y saneados", () => {
  const dir = path.join(root, "Tests", "Fixtures", "Workflow", "ImportarServicioWeb", "http-v1");
  const files = fs.readdirSync(dir);
  assert.deepEqual(files.sort(), ["invalid-response.json", "oversized-response.json", "preview-success.json", "query-success.json", "token-success.json"]);
  for (const file of files) assert.doesNotMatch(fs.readFileSync(path.join(dir, file), "utf8"), /https?:\/\/|password|secret|INTEGRACIONSII/i);
});

test("servidor loopback conserva cuerpo y content-type", async (t) => {
  const received = [];
  const server = http.createServer((request, response) => {
    const chunks = [];
    request.on("data", (chunk) => chunks.push(chunk));
    request.on("end", () => {
      received.push({ type: request.headers["content-type"], body: Buffer.concat(chunks).toString("utf8") });
      response.writeHead(200, { "content-type": "application/json" });
      response.end('{"ok":true}');
    });
  });
  await new Promise((resolve) => server.listen(0, "127.0.0.1", resolve));
  t.after(() => new Promise((resolve) => server.close(resolve)));
  const port = server.address().port;
  const send = (type, body) => new Promise((resolve, reject) => {
    const request = http.request({ hostname: "127.0.0.1", port, method: "POST", headers: { "content-type": type } }, (response) => {
      response.resume(); response.on("end", resolve);
    });
    request.on("error", reject); request.end(body);
  });
  await send("application/json", '{"externalKey":"fixture-001"}');
  await send("application/x-www-form-urlencoded", "externalKey=fixture-001");
  assert.deepEqual(received, [
    { type: "application/json", body: '{"externalKey":"fixture-001"}' },
    { type: "application/x-www-form-urlencoded", body: "externalKey=fixture-001" },
  ]);
});
