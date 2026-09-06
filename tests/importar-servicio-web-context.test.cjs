const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const read = (relative) => fs.readFileSync(path.join(root, relative), "utf8");
const models = read("Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb");
const validator = read("Services/Workflow/ImportarServicioWeb/ValidadorContextoImportacion.vb");
const service = read("Services/Workflow/ImportarServicioWeb/ServicioImportarServicioWeb.vb");

test("contexto fija valores por constructor y solo lectura", () => {
  const contextMatch = models.match(/Public Class ContextoImportacionServicio[\s\S]*?End Class/);
  assert.ok(contextMatch, "ContextoImportacionServicio debe existir");
  const context = contextMatch[0];
  for (const property of ["IdUsuario", "IdGrupo", "LoginUsuario", "IdTarea", "IdRuta", "IdTramite", "ProviderId", "PermiteImportar"]) {
    assert.match(context, new RegExp(`Public ReadOnly Property ${property} `));
  }
  assert.doesNotMatch(context, /Public Property (?:IdUsuario|IdTarea|IdRuta|IdTramite|ProviderId|PermiteImportar) /);
  assert.doesNotMatch(models, /HttpContext|Session|HttpClient|WebRequest/);
});

test("valida identidad, autorizacion, tarea, ruta, tramite y proveedor en orden", () => {
  assert.match(validator, /contexto\.IdRuta <= 0 OrElse contexto\.IdTramite <= 0/);
  const codes = ["INVALID_CONTEXT", "FORBIDDEN", "TASK_NOT_OPERABLE", "ROUTE_MISMATCH", "PROCEDURE_MISMATCH", "PROVIDER_NOT_SUPPORTED"];
  let previous = -1;
  for (const code of codes) {
    const current = validator.indexOf(`"${code}"`);
    assert.ok(current > previous, code);
    previous = current;
  }
});

test("fachada valida, resuelve y solo delega capacidades o consulta", () => {
  const validateAt = service.indexOf("_validador.Validar(contexto)");
  const resolveAt = service.indexOf("_registro.Resolver(contexto.ProviderId)");
  const delegateAt = service.indexOf("resolucion.Proveedor.ResolverCapacidades(contexto)");
  assert.ok(validateAt >= 0 && resolveAt > validateAt && delegateAt > resolveAt);
  assert.match(service, /Function ConsultarElementos/);
  assert.doesNotMatch(service, /HttpContext|Session|HttpClient|WebRequest|AlmacenaDocumentoTareaWorkflow|IImportIntentRepository/);
});
