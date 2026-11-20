'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path'),vm=require('node:vm');
const {MAX,selectTargets,loadOriginals,StorageGate,allowedRead,runJourney,protectedFingerprint}=require('../tools/e2e/tests/support/doc120-storage-ui.cjs');
const {parseArgs}=require('../tools/e2e/scripts/run-doc120-storage-ui.cjs');
function inventory(){return {headers:[{lote_id:'52',nivel_ref:'223',version_id:'45',total_archivos:1000,total_raices:1,unresolved:0,detencion_solicitada:0,estado:'EN_CURSO',config_doc:'{}'}],roots:[{raiz_id:'71',estado:'EXPEDIENTE_CONFIRMADO',identidad_expediente_doc:JSON.stringify({ExpedienteId:'693',RelacionNivelId:'1117'}),intento_activo_id:null}],files:Array.from({length:1000},(_,i)=>({archivo_id:String(i+1),raiz_id:'71',revision:2,estado_recepcion:'RECIBIDO',estado_efecto:i<10?'COMPLETADO':'NO_INICIADO',fixed_values:1,has_staging:1,bytes_previstos:'6',bytes_recibidos:'6',active_marker:0,resultado_doc:i<10?'existing':null}))};}
const payload=row=>({loteId:'52',recursoId:String(row.archivo_id),revisionEsperada:Number(row.revision),fase:'ALMACENAR',claveOperacion:'web_'+'a'.repeat(32)});
test('STORAGE-01: selecciona proximos cuatro sin saltar faltantes ni repetir documentos previos',()=>{
 const data=inventory();assert.deepEqual(selectTargets(data).selected.map(f=>f.archivo_id),['11','12','13','14']);data.files.reverse();assert.deepEqual(selectTargets(data).selected.map(f=>f.archivo_id),['11','12','13','14']);
 const first=data.files.find(f=>f.archivo_id==='11');Object.assign(first,{estado_recepcion:'PENDIENTE',bytes_recibidos:'0',has_staging:0,fixed_values:0});assert.throws(()=>selectTargets(data),/FOUR_NEXT_FILES_NOT_ELIGIBLE/);assert.equal(selectTargets(data,false).selected[0],first);
 for(const change of [d=>d.headers[0].detencion_solicitada=1,d=>d.headers[0].unresolved=1,d=>d.files[30].active_marker=1,d=>d.roots[0].identidad_expediente_doc=null,d=>d.files[10].bytes_previstos='-1']){const d=inventory();change(d);assert.throws(()=>selectTargets(d));}
});
test('STORAGE-02: otro lote, fase, ID, revision y escritor simultaneo se rechazan antes de red',()=>{
 const selected=selectTargets(inventory()).selected,g=new StorageGate(selected),p=payload(selected[0]);
 for(const change of [{loteId:'50'},{fase:'CREAR_EXPEDIENTE'},{recursoId:'15'},{revisionEsperada:1},{extra:true}])assert.throws(()=>g.dispatch('AvanzarLote',{...p,...change}));assert.throws(()=>g.dispatch('RecibirArchivo',p));assert.equal(g.calls.length,0);
 const a=g.dispatch('AvanzarLote',p);assert.throws(()=>g.dispatch('AvanzarLote',payload(selected[1])),/OVERLAPPING_STORAGE_DENIED/);a.delivered=true;a.notifyFinished();
 assert.throws(()=>g.dispatch('AvanzarLote',p),/REPLAY_OR_RESOURCE_DENIED/);
 for(const row of selected.slice(1)){const e=g.dispatch('AvanzarLote',payload(row));e.delivered=true;e.notifyFinished();}
 assert.throws(()=>g.dispatch('AvanzarLote',payload(selected[3])),/REPLAY_OR_RESOURCE_DENIED/);assert.equal(g.calls.length,MAX.storage);
});
test('STORAGE-03: lecturas focales solo de muestra y paginas acotadas, resto de escritores denegados',()=>{
 const s={level:'223',ids:['11','12','13','14']};assert.equal(allowedRead('ConsultarLote',{loteId:'52'},s),true);
 assert.equal(allowedRead('ConsultarRecursoLote',{loteId:'52',tipo:'ARCHIVOS',recursoId:'11',versionLectura:2},s),true);
 for(const [op,p]of [['ConsultarLote',{loteId:'50'}],['ConfirmarPlan',{loteId:'52'}],['ConsultarLote',{loteId:'52',extra:1}],['ConsultarRecursoLote',{loteId:'52',tipo:'ARCHIVOS',recursoId:'15',versionLectura:2}],['ConsultarRecursosLote',{loteId:'52',tipo:'ARCHIVOS',pagina:401,tamanoPagina:25}]])assert.equal(allowedRead(op,p,s),false);
});
test('STORAGE-04: originales exigen ruta interna, nombre, bytes, fecha y PDF antes de recibir',()=>{
 const row={recursoId:'11',estadoRecepcion:'PENDIENTE',rutaRelativa:'Raiz/sub/a.pdf',nombre:'a.pdf',bytesPrevistos:'6',ultimaModificacionLocal:'1000'},win=path.win32;
 const io={realpathSync:p=>p,statSync:()=>({isFile:()=>true,size:6,mtimeMs:1000}),readFileSync:()=>Buffer.from('%PDF-x')};assert.equal(loadOriginals([row],'D:\\allowed',io,win).size,1);
 assert.equal(loadOriginals([{...row,estadoRecepcion:'RECIBIDO'}],undefined,io,win).size,0);
 for(const patch of [{rutaRelativa:'Raiz/../a.pdf'},{nombre:'wrong.pdf'},{bytesPrevistos:'7'},{ultimaModificacionLocal:'999'}])assert.throws(()=>loadOriginals([{...row,...patch}],'D:\\allowed',io,win));
 assert.throws(()=>loadOriginals([row],'D:\\allowed',{...io,realpathSync:p=>p.endsWith('a.pdf')?'D:\\outside\\a.pdf':p},win),/LOCAL_PATH_OUTSIDE_ROOT/);
});
test('STORAGE-05: huella de otros996 cambia ante cualquier alteracion fuera de muestra',()=>{
 const d=inventory(),selected=selectTargets(d).selected,before=protectedFingerprint(d.files,selected);d.files[10].revision++;assert.equal(protectedFingerprint(d.files,selected),before);d.files[14].revision++;assert.notEqual(protectedFingerprint(d.files,selected),before);
});
test('STORAGE-06: argumentos no permiten elegir otro lote, cantidad ni ejecutar sin etiqueta',()=>{
 assert.deepEqual(parseArgs(['--check']),{check:true});assert.deepEqual(parseArgs(['--run','a'.repeat(32),'--authorized-storage-ui']),{runId:'a'.repeat(32)});
 for(const args of [[],['--run','a'.repeat(32)],['--run','a'.repeat(32),'--authorized-storage-ui','--lot','50']])assert.throws(()=>parseArgs(args));
});
test('STORAGE-08: cerrar admision libera entrega pero espera que termine la solicitud iniciada',async()=>{
 const gate=new StorageGate(selectTargets(inventory()).selected),entry=gate.dispatch('AvanzarLote',payload(selectTargets(inventory()).selected[0]));let settled=false;
 const cleanup=gate.settle().then(()=>settled=true);await Promise.resolve();assert.equal(settled,false);entry.confirmed=true;entry.delivered=true;entry.notifyFinished();await cleanup;assert.equal(settled,true);
});
for(const authorized of [false,true])test('STORAGE-07 '+authorized+': sin consentimiento o con otra identidad no abre navegador ni despacha efectos',async()=>{
 const file=path.resolve(__dirname,'../tools/e2e/scripts/run-doc120-storage-ui.cjs'),states=[],calls=[];const runtime={module:{exports:{}},exports:{},__dirname:path.dirname(file),process:{argv:[],env:{},pid:1},console:{log(){},error(){}},Buffer,Set,Map,Date,URL,setTimeout,clearTimeout};
 runtime.require=name=>{
  if(name==='node:fs')return {mkdirSync(){},existsSync:()=>false,writeFileSync(_p,value){states.push(JSON.parse(value));},readFileSync:()=>Buffer.from('source')};
  if(name==='./support/interactive-e2e-console.cjs')return{requireInteractiveConsole(){},async collectValue(target,key,_label,options={}){target[key]=key==='authorization'?(authorized?'SI':'NO'):key==='tls'?'SI':options.defaultValue||'synthetic';}};
  if(name==='./support/doc32-e2e-odbc.cjs')return{queryRows:async sql=>{calls.push(sql);return [{usuario:'999',empresa:'2'}];}};
  if(name==='../node_modules/playwright')throw Error('BROWSER_NOT_ALLOWED');return require(require.resolve(name,{paths:[path.dirname(file)]}));
 };vm.runInNewContext(fs.readFileSync(file,'utf8'),runtime,{filename:file});await runtime.module.exports.main(['--run','a'.repeat(32),'--authorized-storage-ui']);
 assert.equal(calls.length,authorized?1:0);assert.equal(states.at(-1).closed,true);assert.equal(states.at(-1).credentialsCleared,true);assert.equal(states.at(-1).operations.length,0);assert.equal(states.at(-1).sessions,0);
});

test('STORAGE-UI01: cliente guarda cuatro, pausa/cancela/cierra/recarga y conserva los restantes',async()=>{
 const {fixture,click,openFixture}=require('./support/doc120-coordinacion-browser.cjs');
 await fixture(async page=>{
  const selected=Array.from({length:4},(_,i)=>({archivo_id:String(78+i),revision:1})),gate=new StorageGate(selected),entries=new Map(),events=[];
  await page.exposeFunction('storageUiDispatch',p=>{const entry=gate.dispatch('AvanzarLote',p);entries.set(entry.fileId,entry);});
  await page.exposeFunction('storageUiResponse',(id,body)=>{const e=entries.get(id);e.responseAt=Date.now();e.confirmed=body.ok&&body.datos.estado==='CONFIRMADO';e.notifyReady();return e.releasePromise;});
  await page.exposeFunction('storageUiDelivered',id=>{const e=entries.get(id);e.delivered=true;e.notifyFinished();});
  async function hook(){await page.evaluate(()=>{const send=XMLHttpRequest.prototype.send;XMLHttpRequest.prototype.send=function(body){if(this.operation!=='AvanzarLote')return send.call(this,body);const p=JSON.parse(body).solicitud;if(p.fase!=='ALMACENAR')throw Error('UNEXPECTED_WRITE');const load=this.onload;this.onload=async()=>{await storageUiResponse(p.recursoId,JSON.parse(this.responseText));load();await storageUiDelivered(p.recursoId);};storageUiDispatch(p).then(()=>send.call(this,body));};});}
  await page.evaluate(()=>{roots=[{recursoId:'52',idCliente:'r1',nombre:'Root',estado:'EXPEDIENTE_CONFIRMADO',revision:1,tieneIntentoActivo:false,recepcionListaParaCrear:true,expedienteId:'100',relacionNivelId:'200',codigoExpediente:'COD'}];files=Array.from({length:6},(_,i)=>({recursoId:String(78+i),raizId:'52',idCliente:'a'+i,nombre:i+'.pdf',rutaRelativa:'Root/'+i+'.pdf',bytesPrevistos:'6',bytesRecibidos:'6',ultimaModificacionLocal:'0',estadoRecepcion:'RECIBIDO',estado:'NO_INICIADO',revision:1,tieneIntentoActivo:false,valoresEfectivosFijados:true}));});await hook();
  async function query(){await click(page,'abrir-consulta');await page.getByLabel('Referencia del lote',{exact:true}).filter({visible:true}).fill('52');await page.locator('[data-doc111-consultar]').filter({visible:true}).click();await page.getByRole('button',{name:'Continuar importación',exact:true}).waitFor();}
  await query();const reload=async()=>{const seed=await page.evaluate(()=>({roots,files,manifest,rev}));await page.reload();await openFixture(page,{seed});await hook();await query();};
  const results=await runJourney({page,gate,reload,record:(event,data)=>events.push({event,...data}),verify:async e=>{const f=await page.evaluate(id=>files.find(f=>f.recursoId===id),e.fileId);assert.equal(f.estado,'COMPLETADO');return {fileId:e.fileId,documentId:f.documentoId};}});
  assert.equal(results.noReplay,true);assert.equal(new Set(results.documents.map(d=>d.documentId)).size,4);assert.deepEqual(gate.calls,['78','79','80','81']);assert.equal(await page.evaluate(()=>files.filter(f=>f.documentoId).length),4);assert.equal(await page.evaluate(()=>files.slice(4).every(f=>f.estado==='NO_INICIADO')),true);assert.equal(events.some(e=>e.event==='closeWaiting'),true);
 });
});
