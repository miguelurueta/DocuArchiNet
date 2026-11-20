'use strict';
const {id,hash}=require('./doc120-qa-window.cjs');
const check=(ok,code)=>{if(!ok)throw Error('DOC120_STORAGE_UI_'+code);};
const MAX=Object.freeze({http:1200,select:80,sessions:1,storage:4,receipts:4,metadata:4,sourceBytes:96*1024*1024,milliseconds:45*60*1000});
function selectTargets(data,requirePrepared=true){
 check(data.headers.length===1&&data.roots.length===1,'INVENTORY_CHANGED');const h=data.headers[0],root=data.roots[0];
 check(String(h.lote_id)==='52'&&Number(h.total_archivos)===1000&&Number(h.total_raices)===1&&Number(h.unresolved)===0&&!Number(h.detencion_solicitada)&&['EN_CURSO','PARCIAL','LISTO'].includes(h.estado),'LOT_NOT_READY');
 check(data.files.length===1000&&!root.intento_activo_id&&['EXPEDIENTE_CONFIRMADO','PARCIAL'].includes(root.estado)&&data.files.every(f=>!Number(f.active_marker)),'LOT_NOT_QUIESCENT');
 const identity=JSON.parse(root.identidad_expediente_doc||'null');check(identity&&id(identity.ExpedienteId)&&id(identity.RelacionNivelId),'EXPEDITION_REQUIRED');
 const pending=data.files.slice().sort((a,b)=>Number(a.archivo_id)-Number(b.archivo_id)).filter(f=>f.estado_efecto!=='COMPLETADO'&&f.estado_efecto!=='ALMACENADO').slice(0,4);
 check(pending.length===4&&pending.every(f=>['RECIBIDO','PENDIENTE'].includes(f.estado_recepcion)&&f.estado_efecto==='NO_INICIADO'&&(!requirePrepared||f.estado_recepcion==='RECIBIDO'&&Number(f.fixed_values)===1)&&!f.resultado_doc&&Number.isSafeInteger(Number(f.bytes_previstos))&&Number(f.bytes_previstos)>0&&(f.estado_recepcion==='RECIBIDO'?Number(f.has_staging)===1&&String(f.bytes_previstos)===String(f.bytes_recibidos):!Number(f.has_staging)&&Number(f.bytes_recibidos)===0)&&Number.isInteger(Number(f.revision))&&Number(f.revision)>0&&Number(f.revision)<=4294967295&&String(f.raiz_id)===String(root.raiz_id)&&id(String(f.archivo_id))),'FOUR_NEXT_FILES_NOT_ELIGIBLE');
 check(pending.reduce((sum,f)=>sum+Number(f.bytes_previstos),0)<=MAX.sourceBytes,'SOURCE_BYTES_EXCEEDED');
 return {selected:pending,identity,level:String(h.nivel_ref),config:JSON.parse(h.config_doc),header:h};
}
function deferred(){let resolve;const promise=new Promise(r=>resolve=r);return {promise,resolve};}
function loadOriginals(rows,directory,fs=require('node:fs'),path=require('node:path')){
 const originals=new Map();for(const row of rows){if(row.estadoRecepcion!=='PENDIENTE'){check(row.estadoRecepcion==='RECIBIDO','RECEIPT_STATE_CHANGED');continue;}
  try{
  check(typeof directory==='string'&&directory.length>0,'LOCAL_DIRECTORY_REQUIRED');
  const read=action=>{try{return action();}catch{check(false,'LOCAL_ORIGINAL_UNAVAILABLE');}};
  const parts=String(row.rutaRelativa||'').split('/');check(parts.length>=2&&parts.every(p=>p&&p!=='.'&&p!=='..'&&!/[\\:\x00-\x1f]/.test(p)),'LOCAL_PATH_INVALID');
  const root=read(()=>fs.realpathSync(directory)),candidate=read(()=>fs.realpathSync(path.join(root,...parts.slice(1))));check(!root.startsWith('\\\\')&&candidate.toLowerCase().startsWith((root+path.sep).toLowerCase()),'LOCAL_PATH_OUTSIDE_ROOT');
  const stat=read(()=>fs.statSync(candidate));check(stat.isFile()&&path.basename(candidate)===row.nombre&&String(stat.size)===row.bytesPrevistos&&String(Math.trunc(stat.mtimeMs))===row.ultimaModificacionLocal,'ORIGINAL_METADATA_MISMATCH');const bytes=read(()=>fs.readFileSync(candidate));check(bytes.length===stat.size&&bytes.subarray(0,5).toString('ascii')==='%PDF-','LOCAL_PDF_INVALID');originals.set(row.recursoId,{name:row.nombre,bytes});
  }catch(error){error.fileId=row.recursoId;throw error;}
 }
 check([...originals.values()].reduce((n,f)=>n+f.bytes.length,0)<=MAX.sourceBytes,'SOURCE_BYTES_EXCEEDED');return originals;
}
class StorageGate{
 constructor(selected){check(selected.length===4&&new Set(selected.map(f=>String(f.archivo_id))).size===4,'TARGETS_INVALID');this.selected=selected;this.calls=[];this.pending=[];this.listeners=[];this.failure=null;}
 dispatch(operation,p){
  check(!this.failure,'PREVIOUS_FAILURE');check(operation==='AvanzarLote'&&p&&Object.keys(p).sort().join(',')==='claveOperacion,fase,loteId,recursoId,revisionEsperada'&&p.loteId==='52'&&p.fase==='ALMACENAR','WRITE_SCOPE_DENIED');
  check(!this.pending.some(e=>!e.delivered),'OVERLAPPING_STORAGE_DENIED');const row=this.selected[this.calls.length];
  check(row&&p.recursoId===String(row.archivo_id)&&p.revisionEsperada===Number(row.revision)&&typeof p.claveOperacion==='string'&&/^[A-Za-z0-9_-]{16,128}$/.test(p.claveOperacion),'REPLAY_OR_RESOURCE_DENIED');
  const ready=deferred(),release=deferred(),finished=deferred(),entry={fileId:p.recursoId,phase:p.fase,began:Date.now(),ready:ready.promise,finish:finished.promise,release:()=>release.resolve(),releasePromise:release.promise,notifyReady:ready.resolve,notifyFinished:finished.resolve,delivered:false};
  this.calls.push(p.recursoId);this.pending.push(entry);const listener=this.listeners.shift();if(listener)listener.resolve(entry);return entry;
 }
 next(action){const d=deferred();this.listeners.push(d);let timer;const admission=new Promise((_,reject)=>{timer=setTimeout(()=>reject(Error('DOC120_STORAGE_UI_NO_STORAGE_STARTED')),30000);});return Promise.race([Promise.resolve().then(action).then(()=>d.promise),admission]).finally(()=>clearTimeout(timer));}
 async releaseAndWait(entry){entry.release();await entry.finish;check(entry.confirmed===true,'STORAGE_UNCONFIRMED');}
 async settle(){for(const entry of this.pending)entry.release();await Promise.all(this.pending.map(e=>e.finish));}
}
function allowedRead(operation,p,scope){
 if(!p||typeof p!=='object'||Array.isArray(p))return false;
 const fields={ConsultarLote:['loteId'],ConsultarRecursosLote:['loteId','tipo','pagina','tamanoPagina'],ConsultarRecursoLote:['loteId','tipo','recursoId','versionLectura'],AbrirImportacion:['nivelRef'],ListarPerfilesImportacion:['nivelRef','pagina','tamanoPagina']}[operation];
 if(!fields||Object.keys(p).some(k=>!fields.includes(k)))return false;
 if(operation==='AbrirImportacion'||operation==='ListarPerfilesImportacion')return p.nivelRef===scope.level;
 if(p.loteId!=='52')return false;
 if(operation==='ConsultarLote')return true;
 if(!['RAICES','ARCHIVOS'].includes(p.tipo))return false;
 if(operation==='ConsultarRecursoLote')return p.tipo==='ARCHIVOS'&&scope.ids.includes(p.recursoId)&&[1,2].includes(p.versionLectura);
 return Number.isInteger(p.pagina)&&p.pagina>=1&&p.pagina<=400&&Number.isInteger(p.tamanoPagina)&&p.tamanoPagina>=1&&p.tamanoPagina<=25;
}
const click=(page,action)=>page.locator('[data-doc111-action="'+action+'"]').filter({visible:true}).first().click();
async function paused(page){await page.getByRole('button',{name:'Continuar importación',exact:true}).waitFor();await page.waitForFunction(()=>!!document.querySelector('[data-doc111-action="continuar-importacion"]:not(:disabled)')&&!document.querySelector('[data-doc120-unavailable][open]'));}
async function runJourney({page,gate,reload,verify,record=()=>{},assertLive=()=>{}}){
 async function start(){assertLive();return gate.next(()=>click(page,'continuar-importacion'));}
 async function pause(entry){await click(page,'pausar-importacion');await page.getByRole('button',{name:'Aplicando pausa…',exact:true}).waitFor();record('pauseRequested',{fileId:entry.fileId,serverResponseAlreadyAvailable:entry.responseAt!=null});await gate.releaseAndWait(entry);await paused(page);return verify(entry);}
 const a=await start();record('A_WAITING',{fileId:a.fileId});const A=await pause(a);check(gate.calls.length===1,'PAUSE_ADMITTED_NEXT');record('pauseAndStore',{...A});
 const b=await start();await page.locator('.doc111-head [data-doc111-action="cerrar"]').click();await page.locator('.doc120-close-modal[open] [data-doc111-action="cancelar-cierre"]').click();check(await page.getByRole('button',{name:'Pausar importación',exact:true}).isEnabled(),'CANCEL_PAUSED');record('cancelCloseKeptRunning',{fileId:b.fileId});
 // Cancellation must actually allow the next storage, without another Continue click.
 const c=await gate.next(()=>gate.releaseAndWait(b));check(gate.calls.length===3,'CANCEL_DID_NOT_CONTINUE');
 await page.locator('.doc111-head [data-doc111-action="cerrar"]').click();await page.locator('.doc120-close-modal[open] [data-doc111-action="confirmar-cierre"]').click();check(await page.locator('.doc111-overlay').count()===1,'CLOSED_BEFORE_RESPONSE');record('closeWaiting',{fileId:c.fileId,serverResponseAlreadyAvailable:c.responseAt!=null});await gate.releaseAndWait(c);await page.locator('.doc111-overlay').waitFor({state:'hidden'});const B=await verify(b),C=await verify(c);check(gate.calls.length===3,'CLOSE_ADMITTED_NEXT');
 await reload();check(gate.calls.length===3,'RELOAD_REPLAYED');const d=await start(),D=await pause(d);check(gate.calls.length===4,'RESUME_REPLAYED');await reload();check(gate.calls.length===4,'FINAL_RELOAD_REPLAYED');
 return {pauseWaited:true,cancelCloseKeptRunning:true,acceptedCloseWaited:true,resumedAfterReload:true,noReplay:true,documents:[A,B,C,D]};
}
function protectedFingerprint(files,selected){const ids=new Set(selected.map(f=>String(f.archivo_id)));return hash(files.filter(f=>!ids.has(String(f.archivo_id))));}
module.exports={MAX,check,selectTargets,loadOriginals,StorageGate,allowedRead,runJourney,protectedFingerprint};
