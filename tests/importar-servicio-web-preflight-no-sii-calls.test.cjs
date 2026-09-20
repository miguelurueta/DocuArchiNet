const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path');
const root=path.resolve(__dirname,'..');
const files=['Services/Workflow/ImportarServicioWeb/ServicioPreflightImportacion.vb','Services/Workflow/ImportarServicioWeb/ImportEffectPlanBuilder.vb','Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportEffectConfigurationRepository.vb'];
test('frontera DOC-70 no depende de token sello preview o descarga SII',()=>{
  const source=files.map(f=>fs.readFileSync(path.join(root,f),'utf8')).join('\n');
  for(const forbidden of ['SiiExternalImportProviderClient','solicitarToken','consultarInformacionSello','GetPreview','DownloadAsync','DESCARGAR_ANEXO']) assert.doesNotMatch(source,new RegExp(forbidden,'i'));
});
test('fingerprint incluye contexto configuración y orden canónico',()=>{
  const source=fs.readFileSync(path.join(root,files[0]),'utf8');
  for(const symbol of ['IdUsuario','IdTarea','IdRuta','IdTramite','ProviderId','configuration.CanonicalValue']) assert.match(source,new RegExp(symbol.replace('.','\\.')));
  assert.match(source,/values\.Sort\(StringComparer\.Ordinal\)/);
});
