const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path');
const root=path.resolve(__dirname,'..');
const preflight=fs.readFileSync(path.join(root,'Services/Workflow/ImportarServicioWeb/ServicioPreflightImportacion.vb'),'utf8');
const intent=fs.readFileSync(path.join(root,'Services/Workflow/ImportarServicioWeb/ServicioIntencionImportacion.vb'),'utf8');
const adapter=fs.readFileSync(path.join(root,'Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportEffectConfigurationRepository.vb'),'utf8');
const config=fs.readFileSync(path.join(root,'Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportExpedientConfigurationRepository.vb'),'utf8');
test('preflight solo lee configuración y no contiene puertos de mutación',()=>{
  assert.match(preflight,/_configurations\.Obtener\(contexto\)/);
  assert.doesNotMatch(preflight,/CrearOReutilizar|Persistir|Actualizar|RegistrarVerificado|SiiExternalImportProviderClient|SolicitarToken|ConsultarInformacionSello/);
  assert.match(adapter,/_source\.Obtener\(contexto\)/); assert.doesNotMatch(adapter,/INSERT|UPDATE|DELETE/);
  assert.match(config,/@procedureId/); assert.match(config,/New List\(Of IDataParameter\)/); assert.doesNotMatch(config,/INSERT|UPDATE|DELETE/);
});
test('intención revalida antes de adquirir lock',()=>{
  const revalidate=intent.indexOf('_preflight.Preflight'); const lock=intent.indexOf('_guard.Adquirir');
  assert.ok(revalidate>0 && lock>revalidate); assert.match(intent,/PREFLIGHT_STALE/);
});
