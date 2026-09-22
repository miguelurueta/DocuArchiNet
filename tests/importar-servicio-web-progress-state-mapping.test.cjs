const test = require('node:test');
const assert = require('node:assert/strict');
const progress = require('../js/workflow/importar-servicio-web/importar-servicio-web-progress-adapter.js');

test('mapea exactamente fases backend a estados visibles seguros', () => {
  const cases=[['Creada','Disponible'],['Validada','Preparando'],['RecursoObtenido','Procesando'],['ExpedientePreparado','Procesando'],['DocumentoAlmacenado','Procesando'],['ÍndicesActualizados','Procesando'],['CachéActualizado','Procesando'],['ResultadoIncierto','Verificando'],['Reconciliada','Importada'],['Completada','Importada'],['RequiereDecision','Requiere decisión'],['FallidaAntesDePersistir','Fallida'],['Parcial','Parcial'],['Detenida','No procesada'],['Omitida','Omitida'],['Futura','No procesada']];
  cases.forEach(([phase,visible])=>assert.equal(progress.visibleState({Status:phase,DocumentId:phase==='Reconciliada'||phase==='Completada'?10:null}),visible,phase));
});

test('resume guardadas, omitidas, fallidas y no procesadas sin falso éxito', () => {
  const snapshot=progress.adapt({IntentId:'i',Status:'Parcial',Items:[{ExternalKey:'a',Status:'Completada',DocumentId:1},{ExternalKey:'b',Status:'Omitida'},{ExternalKey:'c',Status:'FallidaAntesDePersistir'},{ExternalKey:'d',Status:'Detenida'}]});
  assert.deepEqual({saved:snapshot.summary.saved,skipped:snapshot.summary.skipped,failed:snapshot.summary.failed,notProcessed:snapshot.summary.notProcessed},{saved:1,skipped:1,failed:1,notProcessed:1});
  assert.equal(snapshot.isTotalSuccess,false);
  assert.equal(progress.adapt({Items:[{Status:'Completada',DocumentId:9}]}).isTotalSuccess,true);
  assert.equal(progress.adapt({Items:[{Status:'Completada'}]}).isTotalSuccess,false);
});
