const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const read = (file) => fs.readFileSync(path.join(root, "Infrastructure", "Workflow", "ImportarServicioWeb", "Http", file), "utf8");
const all = ["ExternalImportHttpClientFactory.vb", "ExternalImportHttpErrorMapper.vb", "ExternalImportHttpResponseValidator.vb", "ExternalImportHttpTransport.vb"].map(read).join("\n");

test("no instala callbacks globales ni muta defaults por solicitud", () => {
  assert.doesNotMatch(all, /ServerCertificateValidationCallback|ServicePointManager|DefaultRequestHeaders|BaseAddress\s*=/);
  assert.match(read("ExternalImportHttpClientFactory.vb"), /Timeout\.InfiniteTimeSpan/);
});

test("errores publicos son categorias seguras con correlacion", () => {
  const mapper = read("ExternalImportHttpErrorMapper.vb");
  for (const code of ["EXTERNAL_ACCESS_DENIED", "EXTERNAL_TIMEOUT", "EXTERNAL_CANCELLED", "EXTERNAL_UNAVAILABLE", "EXTERNAL_INVALID_RESPONSE"]) assert.match(mapper, new RegExp(code));
  assert.match(mapper, /CorrelationId/);
  assert.doesNotMatch(mapper, /RequestUri|Authorization|response\.Content|innerException\.Message/);
});

test("no agrega autoridad externa al DTO publico de preview", () => {
  const dto = fs.readFileSync(path.join(root, "DTOs", "Workflow", "ImportarServicioWeb", "ImportarServicioWebDtos.vb"), "utf8");
  const preview = dto.slice(dto.indexOf("Class GetPreviewResponseDto"), dto.indexOf("Class PreflightImportRequestDto"));
  assert.doesNotMatch(preview, /ExternalUrl|Token|PhysicalPath|RawResponse|Payload/i);
});
