const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");

const source = fs.readFileSync("Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportExpedientCacheRepository.vb", "utf8");
const composition = fs.readFileSync("webservice/WebServiceImportarServicioWebModern.asmx.vb", "utf8");

test("la lectura moderna es parametrizada y distingue duplicados", () => {
  assert.match(source, /WHERE Matricula=@identity AND NombreGabinete=@cabinet/);
  assert.match(source, /ORDER BY id_ra_sii_cache_exepediente LIMIT 2/);
  assert.match(source, /If rows\.Count = 0 Then Return Nothing/);
  assert.match(source, /If rows\.Count > 1 Then Return FailureCopy\(expected, EstadoEfectoExpedienteImportacion\.Conflicto\)/);
  assert.doesNotMatch(source, /SolicitaCacheCreacionExpedienteSII/);
});

test("la escritura serializa por identidad, usa transacción y relee", () => {
  assert.match(source, /SELECT GET_LOCK\(@lockName,5\)/);
  assert.match(source, /_transactions\.BeginTransaction\(connection\)/);
  assert.match(source, /INSERT INTO ra_sii_cache_exepediente/);
  assert.match(source, /WHERE NOT EXISTS/);
  assert.match(source, /transaction\.Commit\(\)/);
  assert.match(source, /Dim confirmed = ReadOne\(connection, Nothing, inscripcion\)/);
  assert.match(source, /SELECT RELEASE_LOCK\(@lockName\)/);
  assert.doesNotMatch(source, /RegistraCacheCreacionExpedienteSII/);
});

test("un radicado nuevo reutiliza el mismo expediente y registra su relación sin reescribir la caché", () => {
  const sameDestination = source.slice(source.indexOf("Private Shared Function SameDestination"), source.indexOf("Private Shared Function Valid"));
  assert.match(sameDestination, /current\.IdExpediente\.Value = expected\.IdExpediente\.Value/);
  assert.doesNotMatch(sameDestination, /RadicadoSii/);
  assert.match(source, /If Not SameDestination\(current, inscripcion\) Then[\s\S]*EnsureRadicadoRelation\(connection, transaction, inscripcion\)/);
  assert.match(source, /WHERE expediente_archivo_ID_EXPEDIENTE=@expedientId AND RadicadoExterno=@radicado/);
  assert.doesNotMatch(source, /UPDATE ra_sii_cache_exepediente/);
});

test("la composición productiva usa la caché moderna", () => {
  assert.match(composition, /New MySqlImportExpedientCacheRepository\(docuarchiConnections, executor, transactions\)/);
  assert.doesNotMatch(composition, /New LegacyImportExpedientCacheRepository\(\)/);
});

test("el puerto recibe la inscripción y no una clave insuficiente", () => {
  const contract = fs.readFileSync("Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb", "utf8");
  const coordinator = fs.readFileSync("Services/Workflow/ImportarServicioWeb/ImportExpedientCoordinator.vb", "utf8");
  assert.match(contract, /Function Obtener\(ByVal contexto As ContextoImportacionServicio,[\s\S]*ByVal inscripcion As InscripcionImportacion\)/);
  assert.match(coordinator, /_cache\.Obtener\(contexto, inscription\)/);
  assert.match(coordinator, /cached\.EstadoCache = EstadoEfectoExpedienteImportacion\.Conflicto/);
});
