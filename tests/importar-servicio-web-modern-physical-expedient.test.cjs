const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const lookup = fs.readFileSync("Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlPhysicalExpedientIdentityLookup.vb", "utf8");
const gateway = fs.readFileSync("Infrastructure/Workflow/ImportarServicioWeb/Expedients/LegacyPhysicalExpedientGateway.vb", "utf8");
const legacy = fs.readFileSync("Gestion/ClassGaExpediente.vb", "utf8");
const mutationBridge = fs.readFileSync("Infrastructure/Workflow/ImportarServicioWeb/Expedients/ModernExpedientMutationBridge.vb", "utf8");
const composition = fs.readFileSync("webservice/WebServiceImportarServicioWebModern.asmx.vb", "utf8");

test("precheck usa todos los campos únicos con valores parametrizados", () => {
  assert.match(lookup, /For index As Integer = 0 To configuracion\.CamposIdentidad\.Count - 1/);
  assert.match(lookup, /SafeIdentifier\.IsMatch/);
  assert.match(lookup, /`=@identity/);
  assert.match(lookup, /New MySqlParameter/);
  assert.match(lookup, /LIMIT 2/);
  assert.doesNotMatch(lookup, /field\.Valor[^\n]*Append/);
});

test("gateway no crea si ya existe y omite únicamente el precheck legacy", () => {
  assert.match(gateway, /_identityLookup\.Buscar\(contexto, configuracion\)/);
  assert.match(gateway, /existing\.Count = 1/);
  assert.match(gateway, /contexto\.IdEmpresaGestion, id, name/);
  assert.doesNotMatch(gateway, /HttpContext|\.Session/);
  assert.doesNotMatch(mutationBridge, /Valida_existencia_expediente_auto_registro/);
  assert.match(legacy, /Valida_existencia_expediente_auto_registro/);
  assert.match(gateway, /ConfirmXmlIndex\(records\(0\), idExpediente\)/);
  assert.match(gateway, /GetElementsByTagName\("identicacionexpediente"\)/);
});

test("creación legacy encapsulada publica XML antes del commit y revierte si falla", () => {
  const core = legacy.slice(legacy.indexOf("Function Registrar_Expediente_Conservacion"), legacy.indexOf("Public Structure stru_values_cambio_indice"));
  assert.ok(core.indexOf("Crea_archivo_indice_xml_expediente") < core.indexOf("myTrans.Commit()"));
  assert.match(core, /If Result <> "YES" Then[\s\S]*?myTrans\.Rollback\(\)/);
});

test("composición productiva inyecta el lookup moderno", () => {
  assert.match(composition, /New ModernPhysicalExpedientGateway\(New MySqlPhysicalExpedientIdentityLookup/);
});
