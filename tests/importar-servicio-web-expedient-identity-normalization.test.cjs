const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const source = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ImportExpedientIdentityNormalizer.vb", "utf8");
const contracts = fs.readFileSync("Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb", "utf8");
const project = fs.readFileSync("GestionDocumental-Docuarchi.net.vbproj", "utf8");
const cases = JSON.parse(fs.readFileSync("Tests/Fixtures/Workflow/ImportarServicioWeb/expedient-identity-v1/cases.json", "utf8"));

function normalize(input) {
  const cabinet = input.cabinet.trim().toUpperCase();
  const raw = cabinet === "RUP" ? input.proponent : input.enrollment;
  const value = raw.replace(/[^0-9]/g, "").replace(/^0+/, "");
  if (["MERCANTIL", "ESAL", "RUP"].includes(cabinet)) return { query: value, persisted: value, valid: value.length > 0 };
  return { query: "", persisted: "", valid: false };
}

test("tabla de casos es simetrica para consulta y persistencia", () => {
  for (const value of cases) assert.deepEqual(normalize(value), { query: value.query, persisted: value.persisted, valid: value.valid }, value.case);
  assert.equal(cases.some((value) => value.case.includes("secondary")), true);
});

test("normalizador implementa las tres reglas sin sesion", () => {
  assert.match(source, /Case "MERCANTIL"/);
  assert.match(source, /Case "ESAL"[\s\S]*NormalizeEnrollment\(matricula\)/);
  assert.match(source, /Case "RUP"[\s\S]*NormalizeEnrollment\(proponente\)/);
  assert.match(source, /Regex\.Replace\(Clean\(value\), "\[\^0-9\]", String\.Empty\)/);
  assert.match(source, /TrimStart\("0"c\)/);
  assert.doesNotMatch(source, /HttpContext|Session|CIncripcionSII|ClassConsultaExpedienteSII/);
});

test("misma operacion sirve para principales secundarios y recuperacion", () => {
  assert.match(contracts, /Interface IImportExpedientIdentityNormalizer[\s\S]*Function Normalizar/);
  assert.equal((source.match(/Public Function Normalizar\(/g) || []).length, 1);
  assert.doesNotMatch(source, /Primario|Secundario|Crear|Recuperar/);
});

test("entradas invalidas fallan con codigos seguros", () => {
  assert.match(source, /EXPEDIENT_IDENTITY_INVALID/);
  assert.match(source, /EXPEDIENT_CABINET_NOT_SUPPORTED/);
  assert.match(source, /If String\.IsNullOrWhiteSpace\(queryValue\)/);
  assert.doesNotMatch(source, /Val\(|CLng\(|CInt\(/);
});

test("archivo se registra una sola vez en el proyecto", () => {
  const include = 'Compile Include="Services\\Workflow\\ImportarServicioWeb\\ImportExpedientIdentityNormalizer.vb"';
  assert.equal(project.split(include).length - 1, 1);
});
