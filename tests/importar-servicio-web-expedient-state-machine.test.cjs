const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const machine = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ImportIntentStateMachine.vb", "utf8");
const steps = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ImportExecutionSteps.vb", "utf8");
const models = fs.readFileSync("Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb", "utf8");

test("enum contiene todas las fases agregadas", () => {
  for (const phase of ["ExpedientesPlanificados", "ExpedientesResueltos", "ItemsSiiAlmacenados", "UniversoDocumentalConsultado", "VinculacionesProcesadas", "IndicesYXmlActualizados"]) {
    assert.match(models, new RegExp(`\\n    ${phase}\\r?\\n`));
  }
});

test("cada frontera agregada admite parcial incierto o detencion segun corresponda", () => {
  for (const phase of ["ExpedientesPlanificados", "ExpedientesResueltos", "ItemsSiiAlmacenados", "UniversoDocumentalConsultado", "VinculacionesProcesadas"]) {
    const line = machine.split(/\r?\n/).find((value) => value.includes(`Agregar(t, FaseImportacionServicio.${phase},`));
    assert.match(line, /Parcial/);
    assert.match(line, /ResultadoIncierto/);
    assert.match(line, /Detenida/);
  }
  const indices = machine.split(/\r?\n/).find((value) => value.includes("Agregar(t, FaseImportacionServicio.IndicesYXmlActualizados,"));
  assert.match(indices, /Parcial/);
  assert.match(indices, /ResultadoIncierto/);
});

test("paso final falla cerrado si la intencion no fue reconciliada", () => {
  assert.match(steps, /_phase = FaseImportacionServicio\.Completada[\s\S]*ImportIntentStateMachine\.PuedeCompletar\(intencion\.Fase\)/);
  assert.match(steps, /RECONCILIATION_REQUIRED/);
  assert.match(steps, /PersistenciaConocida = True/);
  assert.match(steps, /Reintentable = False/);
});

test("no existe ruta directa desde cache o indices a completada", () => {
  assert.doesNotMatch(machine, /Agregar\(t, FaseImportacionServicio\.(?:CacheActualizado|IndicesActualizados|IndicesYXmlActualizados),[^\r\n]*FaseImportacionServicio\.Completada/);
});

