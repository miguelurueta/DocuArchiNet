const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const gateway = fs.readFileSync("Infrastructure/Workflow/ImportarServicioWeb/Expedients/LegacyPhysicalExpedientGateway.vb", "utf8");
const repository = fs.readFileSync("Infrastructure/Repositories/Workflow/ImportarServicioWeb/PhysicalImportExpedientRepository.vb", "utf8");

test("creación física recibe autoridad explícita y verifica el snapshot devuelto", () => {
  assert.match(gateway, /RegistrarExpedienteTramiteConContexto/);
  assert.doesNotMatch(gateway, /HttpContext|\.Session/);
  assert.match(gateway, /contexto\.IdUsuarioGestion/);
  assert.match(gateway, /contexto\.IdEmpresaGestion/);
  assert.match(gateway, /SolicitaDatosEstructuraExpediente/);
  assert.match(gateway, /SolicitaGabineteProducionExpediente/);
  assert.match(repository, /creado\.IdExpediente\.HasValue/);
  assert.match(repository, /VerifySnapshot\(snapshot, identidad, configuracion\)/);
});

test("respuesta perdida queda incierta y nunca crea en bucle", () => {
  assert.match(gateway, /RespuestaRecibida = False/);
  assert.match(repository, /EXPEDIENT_CREATE_RESULT_UNKNOWN/);
  assert.ok((repository.match(/_gateway\.Crear/g) || []).length === 1);
  assert.doesNotMatch(gateway, /EnsureSessionMatches/);
});
