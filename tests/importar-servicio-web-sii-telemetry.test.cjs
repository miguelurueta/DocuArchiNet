const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");

const root = path.resolve(__dirname, "..");
const read = (relative) => fs.readFileSync(path.join(root, relative), "utf8");
const migration = read("Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-56-pruebas-backend-evidencia/Sql/003-create-ra-ser-intento-serviciointegracion.sql");
const businessContextMigration = read("Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-56-pruebas-backend-evidencia/Sql/004-add-external-service-business-context.sql");
const repository = read("Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlExternalServiceTelemetryRepository.vb");
const telemetry = read("Infrastructure/Workflow/ImportarServicioWeb/Http/ExternalServiceAttemptTelemetry.vb");
const client = read("Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiExternalImportProviderClient.vb");
const composition = read("webservice/WebServiceImportarServicioWebModern.asmx.vb");
const executionSteps = read("Services/Workflow/ImportarServicioWeb/ImportExecutionSteps.vb");

test("la bitacora pertenece a DocuArchi y solo tiene FK local al catalogo", () => {
  assert.match(migration, /CREATE TABLE ra_ser_intento_serviciointegracion/);
  assert.match(migration, /REFERENCES ra_ser_serviciointegracion \(Id_ser_servicioIntegracion\)/);
  assert.match(migration, /ON DELETE RESTRICT/);
  assert.doesNotMatch(migration, /REFERENCES workflow_import_intent/i);
  assert.match(migration, /IntentId VARCHAR\(32\) NULL/);
  assert.match(migration, /ix_ra_ser_intento_disponibilidad/);
});

test("el repositorio resuelve el proveedor activo y usa exclusivamente SQL parametrizado", () => {
  assert.match(repository, /INSERT INTO ra_ser_intento_serviciointegracion/);
  assert.match(repository, /FROM ra_ser_serviciointegracion WHERE NombreServicio=@providerId AND EstadoServicio=1/);
  assert.match(repository, /MySqlParameter\(name/);
  assert.doesNotMatch(repository, /["']\s*&\s*(intento|providerId|operation)/i);
  assert.match(repository, /INTEGRATION_PROVIDER_NOT_REGISTERED_OR_DISABLED/);
});

test("el contexto de negocio es opcional y consultable para cualquier proveedor", () => {
  for (const definition of [
    /TaskId BIGINT NULL/,
    /Radicado VARCHAR\(40\) NULL/,
    /CodigoBarras VARCHAR\(20\) NULL/,
    /ReferenciaProveedor VARCHAR\(120\) NULL/,
  ]) assert.match(businessContextMigration, definition);
  for (const index of ["ix_ra_ser_intento_tarea", "ix_ra_ser_intento_radicado", "ix_ra_ser_intento_codigo_barras", "ix_ra_ser_intento_referencia_proveedor"]) {
    assert.match(businessContextMigration, new RegExp(index));
  }
  assert.match(repository, /@taskId,@radicado,@codigoBarras,@referenciaProveedor/);
  assert.match(repository, /NullIfEmpty\(intento\.CodigoBarras\)/);
});

test("normaliza categorias estables sin persistir mensajes crudos", () => {
  for (const category of ["TIMEOUT", "NETWORK", "AUTHENTICATION", "CANCELLED", "BUSINESS_REJECTION", "INVALID_RESPONSE"]) {
    assert.match(telemetry, new RegExp(`\"${category}\"`));
  }
  assert.match(telemetry, /Fallo normalizado del proveedor externo\./);
  assert.doesNotMatch(telemetry, /failure\.StackTrace|failure\.ToString|InnerException\.Message/);
});

test("registra una observacion por llamada SII y la falla de telemetria no altera el negocio", () => {
  for (const operation of ["SOLICITAR_TOKEN", "CONSULTAR_SELLO", "DESCARGAR_ANEXO"]) {
    assert.match(client, new RegExp(`ObserveAsync\\(\"${operation}\"`));
  }
  assert.match(client, /_attempts\.Registrar\(ExternalServiceAttemptNormalizer\.Create/);
  assert.match(client, /Catch[\s\S]*La telemetria nunca cambia ni duplica el resultado funcional de SII/);
  assert.match(composition, /New MySqlExternalServiceTelemetryRepository\(docuarchiConnections/);
});

test("propaga la correlacion funcional de la intencion hasta cada llamada SII", () => {
  assert.match(telemetry, /\.IntentId=Limit\(intentId,32\)/);
  assert.match(telemetry, /\.ClientItemId=Limit\(clientItemId,128\)/);
  assert.match(telemetry, /\.OperationId=Limit\(operationId,128\)/);
  assert.match(executionSteps, /CancellationToken\.None, intencion\.Id, item\.ClientItemId/);
  assert.match(client, /failure, intentId, clientItemId, operationId/);
  assert.match(client, /taskId, radicado, codigoBarras, referenciaProveedor/);
  assert.match(client, /request\.TaskId, Nothing, request\.CodigoBarras\.Trim\(\), request\.CodigoBarras\.Trim\(\)/);
  assert.match(executionSteps, /contexto\.IdTarea[\s\S]*ContextoOriginal\.Radicado[\s\S]*IdentidadExterna\.ExternalKey/);
});

test("reutiliza los metadatos resueltos durante la descarga sin consultar dos veces el sello", () => {
  assert.match(client, /_resolvedMetadata\(CacheKey\(externalKey, correlationId\)\) = selected\.Metadata/);
  assert.match(client, /_resolvedMetadata\.TryRemove\(CacheKey\(externalKey, correlationId\), cached\) Then Return cached/);
});

test("la consulta calcula disponibilidad y latencia por proveedor operacion y periodo", () => {
  assert.match(repository, /COUNT\(\*\) total/);
  assert.match(repository, /SUM\(CASE WHEN a\.Exitoso=1 THEN 1 ELSE 0 END\) exitosos/);
  assert.match(repository, /AVG\(a\.DuracionMs\) latencia/);
  assert.match(repository, /a\.FechaInicioUtc>=@fromUtc AND a\.FechaInicioUtc<@toUtc/);
  assert.match(repository, /CDec\(result\.Exitosos\) \* 100D \/ CDec\(result\.Total\)/);
});
