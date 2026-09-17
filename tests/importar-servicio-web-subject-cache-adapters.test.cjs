const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const subject = fs.readFileSync("Infrastructure/Workflow/ImportarServicioWeb/Expedients/LegacySiiExpedientSubjectResolver.vb", "utf8");
const cache = fs.readFileSync("Infrastructure/Repositories/Workflow/ImportarServicioWeb/LegacyImportExpedientCacheRepository.vb", "utf8");

test("el sujeto reutiliza la función legacy conservada y falla cerrado", () => {
  assert.match(subject, /SolicitaEstructuraExpedienteSII/);
  assert.match(subject, /SII_SUBJECT_INCOMPLETE/);
});

test("la caché legacy se verifica por lectura y detecta conflictos", () => {
  assert.ok((cache.match(/SolicitaCacheCreacionExpedienteSII/g) || []).length >= 2);
  assert.match(cache, /RegistraCacheCreacionExpedienteSII/);
  assert.match(cache, /EXPEDIENT_CACHE_CONFLICT/);
  assert.match(cache, /clave hash moderna no tiene equivalencia segura/);
});
