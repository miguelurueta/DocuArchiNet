'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),crypto=require('node:crypto'),fs=require('node:fs');
const {createAckLossConductor}=require('../tools/e2e/tests/support/doc118-v2-fault-conductor.cjs');
const {createDriver}=require('../tools/e2e/tests/support/doc111-importacion-carpetas.cjs');
const scope=caseId=>({runId:'DOC118-V2-fixture',caseId,loteId:'20',archivoId:'30',revisionArchivoEsperada:3,claveOperacion:'PRIVATE_OPERATION_KEY',contextHash:'a'.repeat(64),snapshotHash:'b'.repeat(64),inventoryHash:'c'.repeat(64),originalHash:'d'.repeat(64)});
async function fixture(caseId='Q1',options={}){
  const target=scope(caseId),calls=[],order=[],budget=[];let reads=0,arms=0,closes=0,recoveries=0;
  const state={loteId:'20',archivoId:'30',intentoId:null,revisionLote:4,revisionArchivo:3,contextHash:target.contextHash,snapshotHash:target.snapshotHash,inventoryHash:target.inventoryHash,originalHash:target.originalHash,estado:'NO_INICIADO',fase:'NONE',documentoId:null,inventarioId:null,escritores:0,eventosRecuperacion:0,huella:'e'.repeat(64),autorizado:true,reclamacionActiva:false,physicalConcordante:true,acuse:null};
  const seal=()=>state.huella=crypto.createHash('sha256').update(JSON.stringify({...state,huella:null})).digest('hex');seal();
  const reply=repetition=>({loteId:'20',intentoId:'50',fase:'ALMACENAR',estado:caseId==='Q3'?'CONFIRMADO':'RECHAZADO',codigo:caseId==='Q3'?'DOCUMENTO_CONFIRMADO':'EJECUCION_CERRADA_SIN_INVOCACION',resuelta:true,repeticion:repetition,cierreSinInvocacion:caseId!=='Q3',documentoConfirmado:caseId==='Q3',expedienteId:'60',relacionNivelId:'70',gabineteId:'7',documentoId:caseId==='Q3'?'90':null,inventarioId:caseId==='Q3'?'100':null});
  const auth={page:{locator:()=>({getAttribute:async()=>'synthetic-token'})},context:{request:{post:async(url,request)=>{
    const operation=url.split('/').at(-1);calls.push({operation,request});order.push(operation);if(options.onRequest)options.onRequest(operation,calls.length,state);
    if(operation==='AvanzarLote'){
      state.intentoId='50';state.estado='EN_CURSO';state.fase={Q1:'PREPARADO',Q2:'AUTORIZADO',Q3:'RETORNO_OBSERVADO'}[caseId];state.revisionLote++;state.revisionArchivo++;state.reclamacionActiva=true;
      if(caseId==='Q3'){state.documentoId='90';state.inventarioId='100';state.escritores=1;}
      const callbacks={Q1:1,Q2:2,Q3:3}[caseId];state.acuse={fase:{Q1:'INICIO',Q2:'AUTORIZAR',Q3:'OBSERVAR'}[caseId],callbacks,acusesValidos:callbacks,perdidas:1,cerrado:true,ocupado:false};seal();
      return {status:()=>409,json:async()=>({ok:false,codigo:'RESULTADO_INCIERTO'})};
    }
    recoveries++;if(options.recoveryTransport)throw Error('PRIVATE_COOKIE_SQL_SENTINEL');
    if(caseId==='Q2'){if(options.mutateBlocked){state.eventosRecuperacion++;seal();}return {status:()=>409,json:async()=>({ok:false,codigo:options.wrongBlock?'OPERACION_EN_CURSO':'CONFLICTO_REVISION'})};}
    const result=reply(recoveries>1);if(recoveries===1){state.estado=result.estado;state.fase='RESULTADO';state.reclamacionActiva=false;state.eventosRecuperacion++;state.revisionLote++;state.revisionArchivo++;seal();}
    if(recoveries>1&&options.mutateHistory){state.eventosRecuperacion++;seal();}
    if(options.invalidReply)result.documentoConfirmado=!result.documentoConfirmado;
    return {status:()=>200,json:async()=>({ok:true,codigo:result.codigo,datos:result})};
  }}}};
  const driver=createDriver(auth,{baseUrl:'https://test.invalid/app/'},{environment:true,account:true,effects:true,discardable:true,select:true});await driver.refreshToken();
  const ports={driver,assertActive(){if(options.revoked)throw Error('PRIVATE_AUTHORITY_SENTINEL');},beforeDispatch(entry){budget.push(entry.kind);if(options.denyHttp&&entry.kind==='HTTP')throw Error('PRIVATE_BUDGET_SENTINEL');},
    async readEvidence(){reads++;const value=structuredClone(state);options.changeRead?.(reads,value);return value;},
    async arm(value){arms++;order.push('ARM');assert.equal(value.phase,{Q1:'INICIO',Q2:'AUTORIZAR',Q3:'OBSERVAR'}[caseId]);if(options.armFailure)throw Error('PRIVATE_ARM_SENTINEL');return {armed:true,scopeMatched:true};},
    async disarm(){closes++;order.push('DISARM');if(options.closeFailure)throw Error('PRIVATE_CLOSE_SENTINEL');return {closed:true};}};
  const conductor=createAckLossConductor(target,ports);return {conductor,ports,target,state,calls,order,budget,counts:()=>({reads,arms,closes,recoveries})};
}
test('DOC118 V2 fault conductor: Q1 cierra control y repite solo historial',async()=>{
  const f=await fixture();const r=await f.conductor.run();assert.equal(r.localProtocolVerified,true);assert.equal(r.integrationPass,false);assert.equal(r.businessEvidenceVerified,false);assert.deepEqual(r.counts,{storage:1,recovery:2,reads:4});assert.equal(f.state.escritores,0);assert.equal(f.state.estado,'RECHAZADO');assert.equal(f.state.eventosRecuperacion,1);assert.equal(f.counts().closes,1);assert.ok(!JSON.stringify(r).includes('PRIVATE'));assert.equal(f.calls[0].request.maxRetries,0);
});
test('DOC118 V2 fault conductor: Q2 conserva bloqueo sin replay',async()=>{const f=await fixture('Q2');const r=await f.conductor.run();assert.deepEqual(r.counts,{storage:1,recovery:1,reads:3});assert.equal(f.state.estado,'EN_CURSO');assert.equal(f.state.reclamacionActiva,true);assert.equal(f.state.eventosRecuperacion,0);assert.equal(f.state.escritores,0);assert.equal(f.counts().closes,1);});
test('DOC118 V2 fault conductor: Q3 confirma IDs observados con un escritor',async()=>{const f=await fixture('Q3');const r=await f.conductor.run();assert.equal(r.localProtocolVerified,true);assert.equal(f.state.documentoId,'90');assert.equal(f.state.inventarioId,'100');assert.equal(f.state.escritores,1);assert.equal(f.state.eventosRecuperacion,1);assert.equal(f.calls.filter(c=>c.operation==='AvanzarLote').length,1);});
test('DOC118 V2 fault conductor: alcance cerrado y decorador excluido del producto',async()=>{
  const f=await fixture();for(const change of [s=>s.caseId='Q4',s=>s.loteId='24',s=>s.loteId='18',s=>s.loteId='21',s=>s.archivoId=30,s=>s.revisionArchivoEsperada=0,s=>s.runId='http-value',s=>s.evidencia={},s=>s.snapshotHash='invalid']){const s=scope('Q1');change(s);assert.throws(()=>createAckLossConductor(s,f.ports),/DOC118_V2_CONDUCTOR/);}assert.ok(!fs.readFileSync('GestionDocumental-Docuarchi.net.vbproj','utf8').includes('Doc118PerdidaAcuseV2.vb'));assert.equal(f.calls.length,0);
});
test('DOC118 V2 fault conductor: autoridad o presupuesto bloquean antes de HTTP',async()=>{for(const options of [{revoked:true},{denyHttp:true}]){const f=await fixture('Q1',options);await assert.rejects(f.conductor.run(),e=>!e.message.includes('PRIVATE')&&e.report.integrationPass===false);assert.equal(f.calls.length,0);assert.equal(f.counts().closes,options.revoked?0:1);}});
test('DOC118 V2 fault conductor: contexto o inventario distintos impiden recuperar',async()=>{for(const field of ['contextHash','snapshotHash','inventoryHash','originalHash','autorizado']){const f=await fixture('Q1',{changeRead(n,s){if(n===2)s[field]=field==='autorizado'?false:'f'.repeat(64);}});await assert.rejects(f.conductor.run(),/EVIDENCE_CHANGED/);assert.equal(f.calls.length,1);assert.equal(f.counts().closes,1);}});
test('DOC118 V2 fault conductor: acuse no demostrado o duplicado no se acepta',async()=>{for(const mutate of [s=>s.acuse=null,s=>s.acuse.fase='AUTORIZAR',s=>s.acuse.perdidas=2,s=>s.acuse.acusesValidos=0,s=>s.acuse.ocupado=true,s=>s.acuse.cerrado=false]){const f=await fixture('Q1',{changeRead(n,s){if(n===2)mutate(s);}});await assert.rejects(f.conductor.run(),/DOC118_V2_CONDUCTOR/);assert.equal(f.calls.length,1);assert.equal(f.counts().closes,1);}});
test('DOC118 V2 fault conductor: IDs parciales o evidencia fisica discrepante conservan bloqueo',async()=>{
  for(const mutate of [s=>s.documentoId=null,s=>s.inventarioId=null,s=>s.physicalConcordante=false,s=>s.intentoId='69',s=>s.fase='AUTORIZADO']){const f=await fixture('Q3',{changeRead(n,s){if(n===2)mutate(s);}});await assert.rejects(f.conductor.run(),/DOC118_V2_CONDUCTOR/);assert.equal(f.calls.length,1);assert.equal(f.counts().closes,1);}
  for(const step of [2,3]){const f=await fixture('Q1',{changeRead(n,s){if(n===step)s.physicalConcordante=false;}});await assert.rejects(f.conductor.run(),/PHYSICAL_NOT_CONCORDANT|RECOVERY_CHECKPOINT_INVALID/);assert.equal(f.calls.length,step-1);assert.equal(f.counts().closes,1);}
});
test('DOC118 V2 fault conductor: respuesta o transporte inciertos no repiten recuperacion',async()=>{for(const options of [{recoveryTransport:true},{invalidReply:true}]){const f=await fixture('Q3',options);await assert.rejects(f.conductor.run(),e=>!JSON.stringify(e.report).includes('PRIVATE')&&!e.message.includes('PRIVATE'));assert.equal(f.calls.length,2);assert.equal(f.counts().recoveries,1);assert.equal(f.counts().closes,1);}});
test('DOC118 V2 fault conductor: otro rechazo o cambio bajo bloqueo no acreditan Q2',async()=>{for(const options of [{wrongBlock:true},{mutateBlocked:true}]){const f=await fixture('Q2',options);await assert.rejects(f.conductor.run(),/GATE_NOT_BLOCKED|BLOCK_CHANGED_EVIDENCE/);assert.equal(f.calls.length,2);assert.equal(f.counts().closes,1);}});
test('DOC118 V2 fault conductor: historial que escribe no se presenta estable',async()=>{const f=await fixture('Q1',{mutateHistory:true});await assert.rejects(f.conductor.run(),/HISTORY_WRITES/);assert.equal(f.calls.length,3);assert.equal(f.counts().closes,1);});
test('DOC118 V2 fault conductor: capacidad unica y cierre siempre intentado',async()=>{
  const f=await fixture();await f.conductor.run();await assert.rejects(f.conductor.run(),/ALREADY_CONSUMED/);assert.equal(f.calls.length,3);
  for(const options of [{armFailure:true},{closeFailure:true}]){const g=await fixture('Q1',options);await assert.rejects(g.conductor.run(),e=>!e.message.includes('PRIVATE')&&e.report.releaseUnconfirmed===!!options.closeFailure);assert.equal(g.counts().closes,1);}
});
