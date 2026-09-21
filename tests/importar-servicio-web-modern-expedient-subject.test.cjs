const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const client = fs.readFileSync("Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiExternalImportProviderClient.vb", "utf8");
const resolver = fs.readFileSync("Infrastructure/Workflow/ImportarServicioWeb/Expedients/ModernSiiExpedientSubjectResolver.vb", "utf8");
const legacy = fs.readFileSync("Integracionccv/ClassConsultaExpedienteSII.vb", "utf8");
const composition = fs.readFileSync("webservice/WebServiceImportarServicioWebModern.asmx.vb", "utf8");

test("consulta moderna de sujeto usa transporte tipado para MERCANTIL ESAL y RUP", () => {
  assert.match(client, /ResolveExpedientSubjectAsync/);
  assert.match(client, /Case "MERCANTIL"[\s\S]*consultarExpedienteMercantil/);
  assert.match(client, /Case "ESAL"[\s\S]*StartsWith\("9000"[\s\S]*"S0" & lookup/);
  assert.match(client, /Case "RUP"[\s\S]*consultarExpedienteProponente/);
  assert.match(client, /SII_SUBJECT_NOT_FOUND/);
  assert.match(client, /\.Identificacion = identification\.Trim\(\), \.RazonSocial = name\.Trim\(\)/);
  assert.doesNotMatch(client, /String\.IsNullOrWhiteSpace\(identification\)[\s\S]{0,100}SII_SUBJECT_INCOMPLETE/);
  assert.match(client, /Encoding\.UTF8\.GetBytes/);
  assert.doesNotMatch(client, /ServerCertificateValidationCallback|validarCertificado|HttpContext|Session/);
});

test("resolvedor moderno valida sujeto y conserva fallback legacy configurable", () => {
  assert.match(resolver, /Implements ISiiExpedientSubjectResolver/);
  assert.match(resolver, /ResolveExpedientSubjectAsync/);
  assert.match(resolver, /MaterializeIdentityFields/);
  assert.match(resolver, /ModoExpedienteImportacion\.SinExpediente Then Return Confirmed\(\)/);
  assert.match(resolver, /_fallbackEnabled AndAlso _legacyFallback IsNot Nothing/);
  assert.match(composition, /New ModernSiiExpedientSubjectResolver/);
  assert.match(composition, /ImportarServicioWebSiiSubjectLegacyFallback/);
});

test("función original queda conservada sin redirección al moderno", () => {
  assert.match(legacy, /Function ConsultaExpedienteMercantilEsal/);
  assert.doesNotMatch(legacy, /ModernSiiExpedientSubjectResolver|SiiExternalImportProviderClient/);
});
