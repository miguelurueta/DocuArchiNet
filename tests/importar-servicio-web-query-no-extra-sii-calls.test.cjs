const test=require('node:test');
const assert=require('node:assert/strict');
const fs=require('node:fs');
const path=require('node:path');
const root=path.resolve(__dirname,'..');
const read=(...p)=>fs.readFileSync(path.join(root,...p),'utf8');

test('QueryItems invoca proveedor una vez y enriquece localmente',()=>{
  const service=read('webservice','WebServiceImportarServicioWebModern.asmx.vb');
  const block=service.slice(service.indexOf('Public Function QueryItems'),service.indexOf('Public Function GetPreview'));
  assert.equal((block.match(/\.QueryItemsAsync\(/g)||[]).length,1);
  assert.match(block,/presentation\.EnrichItems/); assert.match(block,/presentation\.ApplyPagination/);
  assert.doesNotMatch(block,/GetPreviewAsync|DownloadAsync|QuerySealAsync/);
});

test('estado se resuelve localmente por tarea proveedor y clave',()=>{
  const repo=read('Infrastructure','Repositories','Workflow','ImportarServicioWeb','MySqlImportItemStatusRepository.vb');
  for(const p of ['@taskId','@providerId','@externalKey']) assert.match(repo,new RegExp(p));
  assert.match(repo,/external_key IN \(/);
  assert.match(repo,/workflow_import_intent_item/);
  assert.doesNotMatch(repo,/HttpClient|consultarInformacionSello|SiiExternal/);
  const presentation=read('Services','Workflow','ImportarServicioWeb','ImportItemPresentationService.vb');
  assert.equal((presentation.match(/ObtenerLote\(/g)||[]).length,1);
  for(const status of ['Disponible','Importado','ConNovedad']) assert.match(presentation,new RegExp(`"${status}"`));
});
