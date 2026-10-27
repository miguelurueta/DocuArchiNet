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
test('DOC-110: reglas y configuración ejecutadas en .NET Framework sin conexión ni HTTP', () => {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'doc110-behavior-'));
  try {
    const dll = path.join(directory, 'PerfilImportacion.dll');
    const exe = path.join(directory, 'PerfilImportacionTests.exe');
    const json = path.join(root, 'packages/Newtonsoft.Json.13.0.2/lib/net45/Newtonsoft.Json.dll');
    const source = [
      'Modelo/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionModels.vb',
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
    run(compiler('vbc.exe'), ['/nologo','/target:library',`/out:${dll}`,
      '/reference:System.dll','/reference:System.Core.dll','/reference:System.Data.dll','/reference:System.Data.DataSetExtensions.dll','/reference:System.Web.dll',
      `/reference:${path.join(root,'bin/MySql.Data.dll')}`,`/reference:${json}`, ...source]);
    fs.copyFileSync(json, path.join(directory, 'Newtonsoft.Json.dll'));
    fs.copyFileSync(path.join(root,'bin/MySql.Data.dll'), path.join(directory,'MySql.Data.dll'));
    run(compiler('csc.exe'), ['/nologo','/target:exe',`/out:${exe}`,`/reference:${dll}`,`/reference:${json}`,
      '/reference:System.dll','/reference:System.Core.dll','/reference:System.Web.dll','/reference:System.Data.dll',
      'tests/PerfilImportacionReglasBehaviorTests.cs','tests/PerfilImportacionServiceBehaviorTests.cs',
      'tests/PerfilImportacionRequestGuardBehaviorTests.cs','tests/PerfilImportacionCatalogBehaviorTests.cs',
      'tests/PerfilImportacionMigrationBehaviorTests.cs','tests/ProveedorIndicesBehaviorTests.cs','tests/ProveedorIndicesReglasBehaviorTests.cs','tests/PoliticaFuentesProveedorBehaviorTests.cs',
      'Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Sql/Doc110MigrationJson.cs',
      'tests/FuenteImportacionBehaviorTests.cs','tests/AdministracionProveedorBehaviorTests.cs','tests/PerfilImportacionBehaviorTestRunner.cs']);
    fs.copyFileSync(path.join(root,'tests/Fixtures/ProduccionDocumental/doc110-perfil-antes-proveedor.json'),path.join(directory,'doc110-perfil-antes-proveedor.json'));
    assert.match(run(exe, [], directory), /DOC-110 behavior tests: passed/);
  } finally {
    // Solo el directorio temporal creado por esta prueba, comprobado antes del borrado.
    assert.equal(path.dirname(directory), path.resolve(os.tmpdir()));
    assert.ok(path.basename(directory).startsWith('doc110-behavior-'));
    fs.rmSync(directory, {recursive:true, force:true});
  }
});
