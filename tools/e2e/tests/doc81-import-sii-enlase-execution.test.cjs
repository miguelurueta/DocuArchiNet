'use strict';
const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const { IMPORTAR_SERVICIO_WEB_E2E_ADAPTER } = require('../scripts/adapters/importar-servicio-web-e2e-adapter.cjs');
const { resolveScenario } = require('../scripts/support/workflow-e2e-platform-registry.cjs');
const { validateProfile } = require('../scripts/support/workflow-e2e-platform-profile.cjs');
const { createSafeEvidence, preflightPlatform, requiredAuthorizationsFor } = require('../scripts/support/workflow-e2e-platform.cjs');

test('DOC-81 registra ejecución ENLASE mutadora con recurso descartable y sin efectos de expediente', () => {
  const scenario = resolveScenario('import-sii-enlase-execution');
  const profile = validateProfile(JSON.parse(fs.readFileSync('tools/e2e/profiles/doc81-import-sii-enlase-execution.profile.example.json','utf8')));
  assert.equal(scenario.doc,'doc81'); assert.equal(scenario.stage,'execution'); assert.equal(scenario.resource.mutating,true);
  assert.deepEqual(requiredAuthorizationsFor(scenario,profile),['environment','gate','execution','discardable-resource']);
  for(const id of ['import-expedient-state','import-document-relation-state','import-document-link-cache-state','import-document-index-state']) assert.equal(scenario.controlExpectations[id],'unchanged');
});

test('DOC-81 ejecuta capacidad ENLASE, reconcilia evidencia y no acepta efectos de expediente', async () => {
  const calls=[];
  const stored={ExternalKey:'annex-1',DocumentId:9001,Status:'Disponible',PersistenceKnown:true,ReachedPhase:'Completada'};
  const invoke=async(operation,payload)=>{
    calls.push({operation,payload:payload.request});
    const dto={
      ResolveCapabilities:{Capabilities:[{Codigo:'ANEXOS_RADICADO_ENLASE',Habilitada:true}]},
      QueryItems:{Items:[{ExternalKey:'annex-1',DisplayName:'anexo.pdf',ContentType:'application/pdf',ImportStatus:'Disponible',AllowedActions:['Preview','Import']}]},
      PreflightImport:{IsValid:true,Executable:true,ContextFingerprint:'a'.repeat(64),Requirements:[]},
      CreateImportIntent:{IntentId:'0123456789abcdef0123456789abcdef',VersionToken:'v1'},
      ExecuteImportIntent:{Items:[stored],VersionToken:'v2'},
      GetImportIntent:{Items:[stored],VersionToken:'v2'},
      ReconcileImportIntent:{Items:[stored],Status:'Completado',ExpedientEffects:[]}
    }[operation];
    return {elapsedMs:2,dto};
  };
  const result=await IMPORTAR_SERVICIO_WEB_E2E_ADAPTER.executeExecution({invoke,taskId:1,budgetMs:1000,profile:{scenarioId:'import-sii-enlase-execution',codigoBarras:'CODIGO',radicado:'RADICADO',documentTypeId:154,documentTypeName:'Constancia De Inscripción',sampleSize:1}});
  assert.equal(result.codes.physicalEvidence,'CONFIRMED'); assert.equal(result.codes.noExpedientEffects,'CONFIRMED');
  for(const call of calls) assert.equal(call.payload.Capability,'ANEXOS_RADICADO_ENLASE',call.operation);
  assert.equal(calls.filter(x=>x.operation==='CreateImportIntent').length,2);
  assert.deepEqual(calls.filter(x=>x.operation==='CreateImportIntent')[0].payload,calls.filter(x=>x.operation==='CreateImportIntent')[1].payload);
  assert.deepEqual(result.assertions.map(({id})=>id), ['DOC81-E2E-01','DOC81-E2E-02','DOC81-E2E-03','DOC81-E2E-04','DOC81-E2E-05','DOC81-E2E-06']);
  assert.ok(result.assertions.every(({code})=>/^ASSERTION_[A-Z_]+$/.test(code)));
  const evidenceProfile=validateProfile(JSON.parse(fs.readFileSync('tools/e2e/profiles/doc81-import-sii-enlase-execution.profile.example.json','utf8')));
  const evidencePlan=preflightPlatform({profile:evidenceProfile,authorizations:['environment','gate','execution','discardable-resource']});
  const evidence=createSafeEvidence({plan:evidencePlan,result,before:{},after:{},failureCode:null,resourceEvents:[]});
  assert.equal(evidence.success,true); assert.equal(evidence.result.assertions.length,6);
});

test('DOC-81 selecciona la tarea por el contexto oficial ENLASE',()=>{
  const runner=fs.readFileSync('tools/e2e/scripts/run-workflow-e2e-platform.cjs','utf8');
  assert.match(runner,/\['import-sii-enlase-read', 'import-sii-enlase-execution'\]\.includes\(plan\.scenario\.id\)/);
  assert.match(runner,/#HiddenIdFlujo/); assert.match(runner,/parts\[3\]\.toUpperCase\(\) === 'ENLASE'/);
  assert.ok(runner.includes('page.locator(`[tip_event="seleccion_tarea_wf"][idd="${taskId}"]`).first()'));
  assert.match(runner,/selectCommand\.waitFor\(\{ state: 'attached'/);
});
test('DOC-81 conserva una intención y una ejecución para tres anexos', async () => {
  const calls=[];
  const annexes=[{ExternalKey:'annex-imported',DisplayName:'Importado',ContentType:'application/pdf',ImportStatus:'Importado',AllowedActions:['View']}, ...[1,2,3].map((value)=>({ExternalKey:'annex-'+value,DisplayName:'Anexo '+value,ContentType:'application/pdf',ImportStatus:'Disponible',AllowedActions:['Preview','Import']}))];
  const stored=annexes.slice(1).map((item,index)=>({ExternalKey:item.ExternalKey,DocumentId:9100+index,Status:'Disponible',PersistenceKnown:true,ReachedPhase:'Completada'}));
  const invoke=async(operation,payload)=>{
    calls.push({operation,payload:payload.request});
    const dto={
      ResolveCapabilities:{Capabilities:[{Codigo:'ANEXOS_RADICADO_ENLASE',Habilitada:true}]},
      QueryItems:{Items:annexes,ImageCount:annexes.length},
      PreflightImport:{IsValid:true,Executable:true,ContextFingerprint:'b'.repeat(64),Requirements:[]},
      CreateImportIntent:{IntentId:'1123456789abcdef0123456789abcdef',VersionToken:'v1'},
      ExecuteImportIntent:{Items:stored,VersionToken:'v2'},
      GetImportIntent:{Items:stored,VersionToken:'v2'},
      ReconcileImportIntent:{Items:stored,Status:'Completado',ExpedientEffects:[]}
    }[operation];
    return {elapsedMs:2,dto};
  };
  const result=await IMPORTAR_SERVICIO_WEB_E2E_ADAPTER.executeExecution({invoke,taskId:2,budgetMs:1000,profile:{scenarioId:'import-sii-enlase-execution',codigoBarras:'CODIGO',radicado:'RADICADO',documentTypeId:154,documentTypeName:'Constancia De Inscripción',sampleSize:3}});
  const creates=calls.filter(({operation})=>operation==='CreateImportIntent');
  assert.equal(creates.length,2);
  assert.equal(creates[0].payload.Items.length,3);
  assert.ok(creates[0].payload.Items.every(({ExternalKey})=>ExternalKey!=='annex-imported'));
  assert.equal(calls.find(({operation})=>operation==='QueryItems').payload.PageSize,100);
  assert.deepEqual(creates[0].payload,creates[1].payload);
  assert.equal(calls.filter(({operation})=>operation==='ExecuteImportIntent').length,1);
  assert.equal(result.count,3);
  const physical=result.assertions.find(({code})=>code==='ASSERTION_PHYSICAL_EVIDENCE');
  assert.deepEqual(physical,{id:'DOC81-E2E-03',scenario:'import-sii-enlase-execution',status:'passed',expectedCount:3,observedCount:3,code:'ASSERTION_PHYSICAL_EVIDENCE'});
});
