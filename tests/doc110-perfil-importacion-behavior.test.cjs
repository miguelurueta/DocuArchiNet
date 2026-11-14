const assert = require('node:assert/strict');
const { execFileSync } = require('node:child_process');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const test = require('node:test');
const root = path.resolve(__dirname, '..');
function run(command, args, cwd = root) {
  try { return execFileSync(command, args, { cwd, encoding: 'utf8', stdio: ['ignore','pipe','pipe'] }); }
  catch (error) { throw new Error(`${error.stdout || ''}\n${error.stderr || ''}`); }
}
function compiler(name) {
  return run('powershell.exe', ['-NoProfile','-Command',`(Get-Command ${name} -ErrorAction Stop).Source`]).trim();
}
function compileBehaviorHarness(directory) {
    const dll = path.join(directory, 'PerfilImportacion.dll');
    const exe = path.join(directory, 'PerfilImportacionTests.exe');
    const json = path.join(root, 'packages/Newtonsoft.Json.13.0.2/lib/net45/Newtonsoft.Json.dll');
    const source = [
      'Modelo/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionModels.vb',
      'Modelo/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasModels.vb',
      'Modelo/ProduccionDocumental/ImportacionCarpetas/DocumentosAlmacenamientoV2Importacion.vb',
      'Modelo/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasStagingV2Interfaces.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/AlmacenStagingImportacion.Recuperacion.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/PersistenciaDocumentoV2Importacion.vb',
      'Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.DocumentoV2.vb',
      'Modelo/ProduccionDocumental/ImportacionCarpetas/RecuperacionDocumentoV2Importacion.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.RecuperacionV2.vb',
      'Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.RecuperacionV2.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionCatalogAdapter.RecuperacionV2.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.ProtocoloV2.vb',
      'Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.vb',
      'Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Control.vb',
      'Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Consulta.vb',
      'Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Verificacion.vb',
      'Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.ResultadoCreacion.vb',
      'Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Barreras.vb',
      'Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Valores.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Valores.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComposicionPerfilImportacionOperativa.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Barreras.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Staging.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Creacion.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Documento.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Consolidacion.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Reconciliacion.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Operaciones.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/ServicioConsultaImportacion.Limpieza.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/AlmacenStagingImportacion.Limpieza.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComposicionLimpiezaOriginalImportacion.vb',
      'Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Limpieza.vb',
      'tests/Fixtures/ProduccionDocumental/Doc118ComposicionEscritoresStub.vb',
      'tests/Fixtures/ProduccionDocumental/Doc118PerdidaAcuseV2.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComposicionDocumentoImportacion.vb',
      'webservice/WebServiceImportacionCarpetas.Composicion.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/VerificadorReconciliacionImportacion.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComposicionReconciliacionImportacion.vb',
      'Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Reconciliacion.vb',
      'Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Consolidacion.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasContextAdapter.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasRequestGuard.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/AlmacenStagingImportacion.vb',
      'generic_control/ImportacionCarpetasUpload.ashx.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorPerfilImportacionOperativa.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComandoCreacionExpedienteImportacion.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComandoAlmacenamientoCarpetas.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionCatalogAdapter.Documento.vb',
      'Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Documento.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionCatalogAdapter.Creacion.vb',
      'Modelo/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasInterfaces.vb',
      'DTOs/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasDtos.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/LimitesImportacionCarpetas.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/ServicioPreparacionImportacion.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/ServicioRecepcionImportacion.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/ServicioConsultaImportacion.vb',
      'webservice/WebServiceImportacionCarpetas.asmx.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/CanonizadorManifiestoImportacion.vb',
      'Modelo/ProduccionDocumental/ImportacionCarpetas/ProveedorIndicesModels.vb',
      'Modelo/ProduccionDocumental/ImportacionCarpetas/AdministracionProveedorModels.vb',
      'Modelo/ProduccionDocumental/ImportacionCarpetas/FuenteImportacionModels.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/ReglasFuenteImportacion.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/ServicioFuenteImportacion.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/MetadatosFuenteImportacion.vb',
      'Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlFuenteImportacionRepository.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/ServicioAdministracionProveedor.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/ValidadorAdministracionProveedor.vb',
      'Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlAdministracionProveedorRepository.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/MySqlProveedorDatosIndices.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/PoliticaFuentesProveedor.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/RestriccionTablasFuente.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/MetadatosProveedorIndices.vb',
      'Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlProveedorIndicesRepository.vb',
      'Modelo/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionInterfaces.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/ResolvedorReglasPerfilImportacion.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/ReglasProveedorIndices.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorProveedorIndices.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/ValidadorPerfilImportacion.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionJson.vb',
      'Services/ProduccionDocumental/ImportacionCarpetas/ServicioPerfilImportacion.vb',
      'Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlPerfilImportacionRepository.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionContextAdapter.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionRequestGuard.vb',
      'Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionCatalogAdapter.vb',
      'DTOs/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionDtos.vb',
      'Infrastructure/Shared/Data/ModuleSessionConnectionStringResolver.vb'
    ];
    run(compiler('vbc.exe'), ['/nologo','/target:library','/optioncompare:Text',`/out:${dll}`,
      '/reference:System.dll','/reference:System.Core.dll','/reference:System.Data.dll','/reference:System.Data.DataSetExtensions.dll','/reference:System.Web.dll','/reference:System.Web.Services.dll','/reference:System.Web.Extensions.dll','/reference:System.Configuration.dll',
      `/reference:${path.join(root,'bin/MySql.Data.dll')}`,`/reference:${json}`,`/reference:${path.join(root,'itextshare/itextsharp.dll')}`, ...source]);
    fs.copyFileSync(json, path.join(directory, 'Newtonsoft.Json.dll'));
    fs.copyFileSync(path.join(root,'bin/MySql.Data.dll'), path.join(directory,'MySql.Data.dll'));
    fs.copyFileSync(path.join(root,'itextshare/itextsharp.dll'), path.join(directory,'itextsharp.dll'));
    run(compiler('csc.exe'), ['/nologo','/target:exe',`/out:${exe}`,`/reference:${dll}`,`/reference:${json}`,`/reference:${path.join(root,'bin/MySql.Data.dll')}`,`/reference:${path.join(root,'itextshare/itextsharp.dll')}`,
      '/reference:System.dll','/reference:System.Core.dll','/reference:System.Web.dll','/reference:System.Web.Services.dll','/reference:System.Data.dll',
      'tests/PerfilImportacionReglasBehaviorTests.cs','tests/PerfilImportacionServiceBehaviorTests.cs',
      'tests/PerfilImportacionRequestGuardBehaviorTests.cs','tests/PerfilImportacionCatalogBehaviorTests.cs',
      'tests/PerfilImportacionMigrationBehaviorTests.cs','tests/ProveedorIndicesBehaviorTests.cs','tests/ProveedorIndicesReglasBehaviorTests.cs','tests/PoliticaFuentesProveedorBehaviorTests.cs',
      'Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Sql/Doc110MigrationJson.cs',
      'tests/FuenteImportacionBehaviorTests.cs','tests/AdministracionProveedorBehaviorTests.cs','tests/ImportacionCarpetasBehaviorTests.cs','tests/ImportacionCarpetasDocumentosV2BehaviorTests.cs','tests/ImportacionCarpetasStagingV2BehaviorTests.cs','tests/ImportacionCarpetasAdaptadorV2BehaviorTests.cs','tests/ImportacionCarpetasPersistenciaV2BehaviorTests.cs','tests/ImportacionCarpetasRecuperacionV2BehaviorTests.cs','tests/ImportacionCarpetasRecuperacionHttpV2BehaviorTests.cs','tests/ImportacionCarpetasMatrizV2BehaviorTests.cs','tests/ImportacionCarpetasEnlaceV2BehaviorTests.cs','tests/ImportacionCarpetasPerdidaAcuseV2BehaviorTests.cs','tests/ImportacionCarpetasRepositoryIntegrationTests.cs','tests/PerfilImportacionBehaviorTestRunner.cs']);
    fs.copyFileSync(path.join(root,'tests/Fixtures/ProduccionDocumental/doc110-perfil-antes-proveedor.json'),path.join(directory,'doc110-perfil-antes-proveedor.json'));
    fs.copyFileSync(path.join(root,'tests/Fixtures/ProduccionDocumental/doc111-esquema-instalado-mysql51.json'),path.join(directory,'doc111-esquema-instalado-mysql51.json'));
    return { exe };
}
module.exports = { compileBehaviorHarness };

if (require.main === module) test('DOC-110: reglas y configuración ejecutadas en .NET Framework sin conexión ni HTTP', () => {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'doc110-behavior-'));
  try {
    const { exe } = compileBehaviorHarness(directory);
    const output=run(exe, [], directory);
    assert.match(output, /DOC-110 behavior tests: passed/);
    assert.match(output, /DOC-120 canonical resolved dates preserve input length and immutable rules: passed/);
    assert.match(output, /DOC-120 creation accepts canonical dates from compact input without mutating confirmed resources: passed/);
    assert.match(output, /DOC-118 V2 internal document behavior cases: 19 passed; no execution capability/);
    assert.match(output, /DOC-118 V2 real staging behavior cases: 21 passed; synthetic child processes: 3 closed; no recovery activated/);
    assert.match(output, /DOC-118 V2 adapter behavior cases: 20 passed; persistence and writer doubles; no public activation/);
    assert.match(output, /DOC-118 V2 repository SQL behavior cases: 20 passed; transactional double; adapter integrated; no database connection/);
    assert.match(output, /DOC-118 V2 recovery behavior cases: 22 passed; real guard; SELECT and transaction doubles; zero writer/);
    assert.match(output, /DOC-118 V2 recovery HTTP behavior cases: 19 passed; synthetic HTTP; real guard; zero writer; no database connection/);
    assert.match(output, /DOC-118 V2 interleaving matrix cases: 12 passed; synthetic child processes: 2 closed; real guards; SQL doubles; no real runtime/);
    assert.match(output, /DOC-118 V2 storage binding cases: 12 passed; production composition compiled; server revision; SQL and writer doubles/);
    assert.match(output, /DOC-118 V2 acknowledgment loss cases: 12 passed; test-only decorator; native core and real guard; SQL and writer doubles/);
    assert.match(output, /DOC-111 contracts and limits: passed/);
    assert.match(output, /DOC-111 private streaming staging and retained copy: passed/);
    assert.match(output, /DOC-111 receipt ownership, replay, recovery and concurrency: passed/);
    assert.match(output, /DOC-111 multipart handler guards and private JSON: passed/);
    assert.match(output, /DOC-111 durable expected file record guards: passed/);
    assert.match(output, /DOC-111 exhaustive state dimensions and forbidden replay: passed/);
    assert.match(output, /DOC-111 documentary claim guards and stop signal \(local persistence double\): passed/);
    assert.match(output, /DOC-111 authorized read-only pages, private projection and CSV: passed/);
    assert.match(output, /DOC-120 creation readiness: full scope, partial policy, SELECT only and no effects: passed/);
    assert.match(output, /DOC-111 retention diagnosis preserves uncertain evidence \(read-only double\): passed/);
    assert.match(output, /DOC-111 known expedition relation rejects wrong scope \(synthetic SELECT records\): passed/);
    assert.match(output, /DOC-111 known expedition classification and optional subseries \(synthetic SELECT records\): passed/);
    assert.match(output, /DOC-111 immutable creation command from confirmed root values \(no effects\): passed/);
    assert.match(output, /DOC-111 shared creation catalogs verify exact IDs and reverse names \(synthetic SELECT records\): passed/);
    assert.match(output, /DOC-111 legacy creation compatibility rejects unsafe text and Integer overflow \(no effects\): passed/);
    assert.match(output, /DOC-111 legacy creation authority and effective RA connection \(synthetic session\/SELECT only\): passed/);
    assert.match(output, /DOC-111 approved production archive cycle requires exact unambiguous catalog row \(synthetic SELECT\): passed/);
    assert.match(output, /DOC-111 creation adapter starts once and preserves uncertain known IDs \(explicit doubles\): passed/);
    assert.match(output, /DOC-111 creation result checks destination classification cycle and effective values \(synthetic SELECT\): passed/);
    assert.match(output, /DOC-111 creation checkpoint idempotence scope and uncertainty \(synthetic SQL executor\): passed/);
    assert.match(output, /DOC-111 creation receipt barriers preserve inventory for EXPEDIENTE LOTE and partial \(synthetic metadata\): passed/);
    assert.match(output, /DOC-111 physical retained staging checked before creation reservation \(local PDF and reservation double\): passed/);
    assert.match(output, /DOC-111 coordinated single creation phase retained staging and durable replay \(local PDF and persistence\/writer doubles\): passed/);
    assert.match(output, /DOC-111 deferred values fixed once with provenance and preserved expedition \(provider and SQL doubles\): passed/);
    assert.match(output, /DOC-111 documentary adapter retains original consumes named copy and checkpoints once \(local PDF and writer SQL doubles\): passed/);
    assert.match(output, /DOC-111 documentary verification checks cabinet inventory incorporation names and fixed indices \(synthetic SELECT\): passed/);
    assert.match(output, /DOC-111 documentary catalogs map physical indices and reject automatic field conflicts \(synthetic SELECT\): passed/);
    assert.match(output, /DOC-111 coordinated document phase barriers retained unit durable replay and stop \(local PDF and persistence writer doubles\): passed/);
    assert.match(output, /DOC-111 shared receipt barrier covers storage across independent roots without changing inventory \(synthetic metadata\): passed/);
    assert.match(output, /DOC-111 consolidation requires full original inventory positive checkpoints and preserves uncertainty stop \(synthetic SQL\): passed/);
    assert.match(output, /DOC-111 reconciliation confirms only known uncertain evidence without replaying effects \(synthetic SELECT and SQL\): passed/);
    assert.match(output, /DOC-111 ASMX bounded phases stop and reconciliation reuse guard and safe projection \(synthetic HTTP and effect doubles\): passed/);
    assert.match(output, /DOC-111 cleanup retains recovery evidence and journals exact original removal \(local PDF and SQL doubles\): passed/);
    assert.match(output, /DOC-111 scoped SELECT page queries and closed filters \(synthetic records\): passed/);
    assert.match(output, /DOC-111 ASMX read\/export guard and typed responses \(synthetic HTTP\): passed/);
    assert.match(output, /DOC-111 manifest canonicalization and quotas: passed/);
    assert.match(output, /DOC-111 schema guards: passed/);
    assert.match(output, /DOC-111 plan storage guards: passed/);
    assert.match(output, /DOC-111 parametrized coincidence reader: passed/);
    assert.match(output, /DOC-111 confirmation revalidation and ASMX replay: passed/);
    assert.match(output, /DOC-115 altered retention and concurrent finite quota \(real disk, persistence double\): passed/);
    assert.match(output, /DOC-115 repository integration authorization guard \(no database\): passed/);
    assert.match(output, /DOC-111 operational authority: passed/);
    assert.match(output, /DOC-111 destination and cabinet authority: passed/);
    assert.match(output, /DOC-111 operational request guard: passed/);
    assert.match(output, /DOC-111 import\/admin capability matrix: passed/);
    assert.match(output, /DOC-111 published operational snapshots: passed/);
    assert.match(output, /DOC-111 operative shared catalogs: passed/);
    assert.match(output, /DOC-111 operative profile selection: passed/);
    assert.match(output, /DOC-111 operative provider authority: passed/);
    assert.match(output, /DOC-111 read-only opening: passed/);
    assert.match(output, /DOC-111 operational ASMX dispatcher: passed/);
    assert.match(output, /DOC-111 existing transport\/session capacity without appSettings: passed/);
    assert.match(output, /DOC-111 metadata preflight and typed deferred sources: passed/);
    assert.match(output, /DOC-111 service plan persistence and ASMX preparation: passed/);
  } finally {
    // Solo el directorio temporal creado por esta prueba, comprobado antes del borrado.
    assert.equal(path.dirname(directory), path.resolve(os.tmpdir()));
    assert.ok(path.basename(directory).startsWith('doc110-behavior-'));
    fs.rmSync(directory, {recursive:true, force:true});
  }
});

if (require.main === module) test('DOC-111: columnas UPDATE de intentos corresponden al DDL operativo', () => {
  const ddl = fs.readFileSync(path.join(root, 'Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Sql/02-carga-carpetas-mysql51.sql'), 'utf8');
  const table = ddl.match(/CREATE TABLE ra_integracion_intento\s*\(([\s\S]*?)\)\s*ENGINE=/i);
  assert.ok(table, 'Falta el contrato DDL de intentos');
  const columns = new Set([...table[1].matchAll(/^\s*(\w+)\s+(?:INT|VARCHAR|CHAR|BINARY|DATETIME|LONGTEXT)\b/gim)].map(m => m[1].toLowerCase()));
  let checked = 0;
  for (const name of ['MySqlImportacionCarpetasRepository.Control.vb', 'MySqlImportacionCarpetasRepository.ResultadoCreacion.vb', 'MySqlImportacionCarpetasRepository.Documento.vb', 'MySqlImportacionCarpetasRepository.DocumentoV2.vb', 'MySqlImportacionCarpetasRepository.RecuperacionV2.vb', 'MySqlImportacionCarpetasRepository.Reconciliacion.vb']) {
    const source = fs.readFileSync(path.join(root, 'Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas', name), 'utf8');
    for (const update of source.matchAll(/UPDATE ra_integracion_intento SET (.*?) WHERE /g)) {
      for (const assignment of update[1].split(',')) {
        const column = assignment.trim().split('=')[0].toLowerCase();
        assert.ok(columns.has(column), `Columna de intento fuera del DDL: ${column}`);
      }
      checked++;
    }
  }
  assert.ok(checked >= 2, 'No se contrastaron inicio y resultado de creación');
});
