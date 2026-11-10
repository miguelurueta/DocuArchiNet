'use strict';
// No login, HTTP implementation, SQL, debugger or runtime installation here.
// The host supplies the existing driver, native test binding and counted reads.
const {performance}=require('node:perf_hooks');
const CASES=Object.freeze({Q1:{phase:'INICIO',document:false},Q2:{phase:'AUTORIZAR',document:false},Q3:{phase:'OBSERVAR',document:true}});
const check=(v,c)=>{if(!v)throw Object.assign(new Error('DOC118_V2_CONDUCTOR_'+c),{conductorCode:c});};
const id=v=>typeof v==='string'&&/^[1-9][0-9]{0,9}$/.test(v)&&BigInt(v)<=4294967295n;
const revision=v=>Number.isInteger(v)&&v>0&&v<=4294967295;
const hash=v=>typeof v==='string'&&/^[a-f0-9]{64}$/.test(v);
const closed=(v,fields)=>check(v&&typeof v==='object'&&!Array.isArray(v)&&Object.keys(v).sort().join()===fields.slice().sort().join(),'FIELDS_INVALID');
function createAckLossConductor(scope,ports){
  closed(scope,['runId','caseId','loteId','archivoId','revisionArchivoEsperada','claveOperacion','contextHash','snapshotHash','inventoryHash','originalHash']);
  check(/^DOC118-V2-[A-Za-z0-9_-]{1,48}$/.test(scope.runId)&&Object.hasOwn(CASES,scope.caseId),'CASE_INVALID');
  check(id(scope.loteId)&&!['18','21','24'].includes(scope.loteId)&&id(scope.archivoId)&&revision(scope.revisionArchivoEsperada)&&typeof scope.claveOperacion==='string'&&/^[A-Za-z0-9_-]{1,64}$/.test(scope.claveOperacion)&&['contextHash','snapshotHash','inventoryHash','originalHash'].every(k=>hash(scope[k])),'RESOURCE_INVALID');
  check(ports?.driver&&typeof ports.driver.request==='function'&&['readEvidence','arm','disarm','assertActive','beforeDispatch'].every(k=>typeof ports[k]==='function'),'PORTS_REQUIRED');
  const target=Object.freeze({...scope}),definition=CASES[target.caseId];let consumed=false;
  async function run(){
    check(!consumed,'ALREADY_CONSUMED');consumed=true;
    const counts={storage:0,recovery:0,reads:0},order=[];let armAttempted=false,errorCode=null,releaseUnconfirmed=false,verified=false;const started=performance.now();
    const active=()=>{check(performance.now()-started<=60000,'DURATION_EXCEEDED');ports.assertActive();};
    async function read(){
      active();check(counts.reads<4,'READ_LIMIT');ports.beforeDispatch({kind:'READ_EVIDENCE',caseId:target.caseId});counts.reads++;
      const value=await ports.readEvidence();active();
      closed(value,['loteId','archivoId','intentoId','revisionLote','revisionArchivo','contextHash','snapshotHash','inventoryHash','originalHash','estado','fase','documentoId','inventarioId','escritores','eventosRecuperacion','huella','autorizado','reclamacionActiva','physicalConcordante','acuse']);
      check(value.autorizado===true&&value.loteId===target.loteId&&value.archivoId===target.archivoId&&revision(value.revisionLote)&&revision(value.revisionArchivo)&&hash(value.huella)&&['contextHash','snapshotHash','inventoryHash','originalHash'].every(k=>value[k]===target[k]),'EVIDENCE_CHANGED');
      check(Number.isInteger(value.escritores)&&value.escritores>=0&&value.escritores<=1&&Number.isInteger(value.eventosRecuperacion)&&value.eventosRecuperacion>=0&&typeof value.reclamacionActiva==='boolean'&&typeof value.physicalConcordante==='boolean','EVIDENCE_INVALID');
      return value;
    }
    async function request(operation,payload){
      active();const key=operation==='AvanzarLote'?'storage':'recovery';check(counts[key]<(key==='storage'?1:2),'HTTP_LIMIT');
      ports.beforeDispatch({kind:'HTTP',operation,payload});counts[key]++;order.push(operation);
      return ports.driver.request(operation,payload);
    }
    try{
      const baseline=await read();check(baseline.intentoId===null&&baseline.estado==='NO_INICIADO'&&baseline.fase==='NONE'&&!baseline.reclamacionActiva&&baseline.documentoId===null&&baseline.inventarioId===null&&baseline.escritores===0&&baseline.eventosRecuperacion===0&&baseline.acuse===null&&baseline.revisionArchivo===target.revisionArchivoEsperada,'NOT_ELIGIBLE');
      active();armAttempted=true;order.push('ARM');const armed=await ports.arm({...target,phase:definition.phase});check(armed?.armed===true&&armed?.scopeMatched===true,'ARM_NOT_CONFIRMED');
      const advance={loteId:target.loteId,recursoId:target.archivoId,fase:'ALMACENAR',revisionEsperada:target.revisionArchivoEsperada,claveOperacion:target.claveOperacion};
      let uncertain=false;try{const r=await request('AvanzarLote',advance);uncertain=r?.estado==='INCIERTO';}catch(e){if(e?.serverCode==='RESULTADO_INCIERTO'&&e?.httpStatus===409)uncertain=true;else throw e;}
      check(uncertain,'FAULT_NOT_OBSERVED');const before=await read();
      check(id(before.intentoId)&&!['41','53','69'].includes(before.intentoId)&&before.estado==='EN_CURSO'&&before.reclamacionActiva&&before.eventosRecuperacion===0&&before.escritores===(definition.document?1:0),'ATTEMPT_INVALID');
      closed(before.acuse,['fase','callbacks','acusesValidos','perdidas','cerrado','ocupado']);const callbacks=['INICIO','AUTORIZAR','OBSERVAR'].indexOf(definition.phase)+1;
      check(before.acuse.fase===definition.phase&&before.acuse.callbacks===callbacks&&before.acuse.acusesValidos===callbacks&&before.acuse.perdidas===1&&before.acuse.cerrado===true&&before.acuse.ocupado===false,'ACK_LOSS_UNCONFIRMED');
      check(before.fase===({Q1:'PREPARADO',Q2:'AUTORIZADO',Q3:'RETORNO_OBSERVADO'})[target.caseId],'PHASE_INVALID');
      check(definition.document?id(before.documentoId)&&id(before.inventarioId)&&before.physicalConcordante:before.documentoId===null&&before.inventarioId===null,'OBSERVED_IDS_INVALID');
      if(target.caseId==='Q1')check(before.physicalConcordante,'PHYSICAL_NOT_CONCORDANT');
      const recovery={loteId:target.loteId,intentoId:before.intentoId,revisionLoteEsperada:before.revisionLote,revisionArchivoEsperada:before.revisionArchivo};
      if(target.caseId==='Q2'){
        let blocked=false;try{await request('RecuperarDocumentoV2',recovery);}catch(e){blocked=e?.httpStatus===409&&e?.serverCode==='CONFLICTO_REVISION';}
        check(blocked,'GATE_NOT_BLOCKED');const after=await read();check(after.huella===before.huella&&after.reclamacionActiva&&after.escritores===0&&after.eventosRecuperacion===0,'BLOCK_CHANGED_EVIDENCE');
      }else{
        const first=await request('RecuperarDocumentoV2',recovery);check(first?.loteId===target.loteId&&first?.intentoId===before.intentoId&&first.resuelta===true&&first.repeticion===false,'RECOVERY_INVALID');
        check(definition.document?first.estado==='CONFIRMADO'&&first.codigo==='DOCUMENTO_CONFIRMADO'&&first.documentoConfirmado===true&&first.cierreSinInvocacion===false&&first.documentoId===before.documentoId&&first.inventarioId===before.inventarioId:first.estado==='RECHAZADO'&&first.codigo==='EJECUCION_CERRADA_SIN_INVOCACION'&&first.documentoConfirmado===false&&first.cierreSinInvocacion===true&&first.documentoId===null&&first.inventarioId===null,'RECOVERY_IDENTITY_CHANGED');
        const after=await read();check(after.intentoId===before.intentoId&&!after.reclamacionActiva&&after.fase==='RESULTADO'&&after.physicalConcordante&&after.estado===(definition.document?'CONFIRMADO':'RECHAZADO')&&after.escritores===before.escritores&&after.eventosRecuperacion===1&&after.documentoId===before.documentoId&&after.inventarioId===before.inventarioId,'RECOVERY_CHECKPOINT_INVALID');
        const replay=await request('RecuperarDocumentoV2',recovery);check(replay?.repeticion===true&&JSON.stringify({...replay,repeticion:false})===JSON.stringify(first),'HISTORY_CHANGED');
        const last=await read();check(last.huella===after.huella&&last.escritores===after.escritores&&last.eventosRecuperacion===after.eventosRecuperacion,'HISTORY_WRITES');
      }
      verified=true;
    }catch(e){errorCode=e?.conductorCode||'OPERATION_NOT_CONFIRMED';}
    finally{if(armAttempted){order.push('DISARM');try{const r=await ports.disarm();releaseUnconfirmed=r?.closed!==true;}catch{releaseUnconfirmed=true;}}}
    if(releaseUnconfirmed)errorCode=errorCode||'ARM_CLOSE_UNCONFIRMED';
    const report={caseId:target.caseId,faultPhase:definition.phase,localProtocolVerified:verified&&!errorCode,counts,order,releaseUnconfirmed,businessEvidenceVerified:false,integrationPass:false};
    if(errorCode){const error=new Error('DOC118_V2_CONDUCTOR_'+errorCode);error.report=report;throw error;}return report;
  }
  return Object.freeze({run});
}
module.exports={createAckLossConductor};
