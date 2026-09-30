const test = require('node:test');
const assert = require('node:assert/strict');
const workflowList = require('../js/workflow/importar-servicio-web/importar-servicio-web-workflow-document-list-adapter.js');

function fixture(taskId=220586) {
  const rows=[];
  const calls=[];
  const grid={getElementsByTagName:name=>name==='tr'?rows:[]};
  const append=workflowList.create({document:{getElementById:id=>id==='GridView_list_documento_relacion_wf'?grid:null},currentTaskId:()=>taskId,insertRow:(data,destination,versioned)=>{
    calls.push({data,destination,versioned});
    const fields=data.split('|');
    rows.push({getAttribute:name=>name==='id_wf'?fields[1]:null});
  }});
  return {append,calls};
}

test('inserta en Workflow los ocho campos autoritativos y conserva tipología e icono', () => {
  const {append,calls}=fixture();
  const projection={cabinetName:'MERCANTIL',documentId:93,radicado:'S002469800',storageType:'PDF',documentTypeName:'Certificado <Tradición>|SII',taskId:220586,signatureStatus:0,iconClass:'fa-file-pdf'};
  assert.equal(append({documentId:93,taskId:220586,workflowProjection:projection}),true);
  assert.deepEqual(calls,[{data:'MERCANTIL|93|S002469800|PDF|Certificado &lt;Tradición&gt;SII|220586|0|fa-file-pdf',destination:'wf',versioned:1}]);
});

test('rechaza proyección incompleta, tarea distinta y evita duplicados', () => {
  const {append,calls}=fixture();
  assert.equal(append({documentId:93,taskId:220586,workflowProjection:{documentId:93,taskId:220586}}),false);
  assert.equal(append({documentId:93,taskId:999,workflowProjection:{cabinetName:'M',documentId:93,radicado:'S',storageType:'PDF',documentTypeName:'Tipo',taskId:999,iconClass:'fa-file'}}),false);
  const projection={cabinetName:'M',documentId:93,radicado:'S',storageType:'PDF',documentTypeName:'Tipo',taskId:220586,iconClass:'fa-file'};
  assert.equal(append({documentId:93,taskId:220586,workflowProjection:projection}),true);
  assert.equal(append({documentId:93,taskId:220586,workflowProjection:projection}),true);
  assert.equal(calls.length,1);
});
