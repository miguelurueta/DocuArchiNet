const test = require("node:test");
const assert = require("node:assert/strict");
const crypto = require("node:crypto");
const fs = require("node:fs");

const fixture = JSON.parse(fs.readFileSync("Tests/Fixtures/Workflow/ImportarServicioWeb/inscription-aggregate-v1/multiple-inscriptions.json", "utf8"));
const mapper = fs.readFileSync("Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportContractMapper.vb", "utf8");
const repository = fs.readFileSync("Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportIntentRepository.vb", "utf8");
const models = fs.readFileSync("Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb", "utf8");
const dtos = fs.readFileSync("DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb", "utf8");

const key = (book, registration) => crypto.createHash("sha256").update(`${book}\x1f${registration}`).digest("hex");

test("fixture conserva varias inscripciones y sus documentos", () => {
  const aggregate = fixture.provider.inscripciones.map((value, index) => ({
    key: key(value.libro, value.registro), ordinal: index + 1,
    clients: fixture.selected.filter(item => item.book === value.libro && item.registration === value.registro).map(item => item.clientItemId)
  }));
  assert.equal(aggregate.length, 2);
  assert.deepEqual(aggregate.map(x => x.clients), [["item-a1", "item-a2"], ["item-b1"]]);
  assert.notEqual(aggregate[0].key, aggregate[1].key);
});

test("mapper asigna clave estable sin ampliar DTO seleccionable", () => {
  assert.match(mapper, /Function MapInscriptions\(/);
  assert.match(mapper, /Function BuildInscriptionKey\(/);
  assert.match(mapper, /item\.ClaveInscripcion = key/);
  assert.match(mapper, /aggregate\.ClientItemIds\.Add\(item\.ClientItemId\)/);
  const externalDto = dtos.slice(dtos.indexOf("Public Class ExternalItemDto"), dtos.indexOf("Public Class QueryItemsResponseDto"));
  assert.doesNotMatch(externalDto, /Inscripcion|Libro|Registro|Clave/i);
});

test("crear persiste inscripciones antes de items asociados", () => {
  const inscriptionInsert = repository.indexOf("INSERT INTO workflow_import_inscription");
  const itemInsert = repository.indexOf("INSERT INTO workflow_import_intent_item");
  assert.ok(inscriptionInsert >= 0 && itemInsert > inscriptionInsert);
  assert.match(repository, /P\("@inscriptionKey", item\.ClaveInscripcion\)/);
});

test("obtener y recuperar rehidratan agregado y asociación", () => {
  assert.match(repository, /intent\.Inscripciones = _executor\.ExecuteReader/);
  assert.match(repository, /FROM workflow_import_inscription WHERE intent_id=@intentId ORDER BY inscription_ordinal/);
  assert.match(repository, /SELECT client_item_id,inscription_key,provider_id/);
  assert.match(repository, /\.ClaveInscripcion=Convert\.ToString\(reader\("inscription_key"\)\)/);
  assert.match(models, /Property Inscripciones As IList\(Of InscripcionImportacion\)/);
});
