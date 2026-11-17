'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),crypto=require('node:crypto');
const segmented=require('../js/gestion/ImportacionCarpetas/PreparacionSegmentadaImportacion.js');
const core=require('../js/gestion/ImportacionCarpetas/ImportacionCarpetas.js');
test('DOC120-PREPREF01: solo persiste el ID del borrador, aislado por contexto y sin consentimiento',()=>{
 const values=new Map(),storage={getItem:k=>values.get(k)||null,setItem:(k,v)=>values.set(k,v),removeItem:k=>values.delete(k)};
 const own=segmented.referencias(storage,'a'.repeat(32)),other=segmented.referencias(storage,'b'.repeat(32));
 assert.equal(own.guardar('77'),true);assert.equal(own.leer(),'77');assert.equal(other.leer(),null);
 assert.deepEqual(JSON.parse([...values.values()][0]),{v:1,preparacionId:'77'});
 values.set('DOC120_PREP_'+'a'.repeat(32),JSON.stringify({v:1,preparacionId:'77',clave:'secret'}));assert.equal(own.leer(),null);assert.equal(values.size,0);
 own.guardar('77');own.guardar(null);assert.equal(own.leer(),null);
 assert.equal(segmented.referencias(storage,'untrusted').guardar('77'),false);
 const denied=segmented.referencias({getItem:()=>{throw Error('blocked');},setItem:()=>{throw Error('blocked');},removeItem:()=>{}},'c'.repeat(32));
 assert.equal(denied.leer(),null);assert.equal(denied.guardar('77'),false);
});
function request(count=1000,roots=1){return {nivelRef:'7',versionId:'43',raices:Array.from({length:roots},(_,i)=>({seleccionId:'s'+i,idCliente:'r'+i,nombre:'Egresos'+i})),archivos:Array.from({length:count},(_,i)=>({idCliente:'a'+i,raizIdCliente:'r'+i%roots,nombre:i+'.pdf',rutaRelativa:'Egresos'+i%roots+'/'+i+'.pdf',bytes:'677',ultimaModificacionLocal:'1234'}))};}
function fixture(body=request(),extra={}){
 const calls=[],parts=new Map();let rev=1,started=false,sealed=false,lost=false;const completed=new Set();
 const state=()=>({existe:true,preparacionId:'77',revision:rev,estado:sealed?'SELLADA':'BORRADOR',planId:sealed?'11':null,totalRaices:body.raices.length,totalArchivos:body.archivos.length,partesRecibidas:parts.size,partesValidadas:completed.size});
 function summary(type='RESUMEN',page=1){return{planId:'11',revision:1,planHash:'a'.repeat(64),nivelRef:body.nivelRef,puedeConfirmar:true,unidadesElegibles:body.raices.length,totalArchivos:body.archivos.length,totalIncidencias:0,tipo:type,pagina:page,puedeHaberMas:false,valores:[],incidencias:[],unidadesElegiblesPagina:[]};}
 const api={invocar:async(op,input)=>{
  calls.push({op,input:structuredClone(input)});let output;
  if(op==='IniciarPreparacionPlan'){started=true;output=state();}
  else if(op==='ConsultarPreparacionPlan'){output=started?state():{existe:false,estado:'NO_ENCONTRADA'};}
  else if(op==='RegistrarPartePreparacion'){
   assert.ok(started);const key=input.tipo+':'+input.numero,old=parts.get(key),text=JSON.stringify([input.raices,input.archivos]);
   if(old){assert.equal(old,text);}else{assert.equal(input.revisionEsperada,rev);parts.set(key,text);rev++;}output=state();
  }else if(op==='ValidarPartePreparacion'){
   assert.ok(parts.has(input.tipo+':'+input.numero));const key=input.tipo+':'+input.numero;
   if(!completed.has(key)){assert.equal(input.revisionEsperada,rev);completed.add(key);rev++;}output=state();
  }else if(op==='SellarPreparacionPlan'){assert.equal(parts.size,completed.size);if(!sealed){sealed=true;rev++;}output=summary();}
  else if(op==='ConsultarRevisionPlan'){
   assert.ok(sealed);output=summary(input.tipo,input.pagina);
   if(input.tipo==='VALORES'){
    const all=body.raices.map(r=>({Ambito:'RAIZ',RaizIdCliente:r.idCliente,ArchivoIdCliente:null,Valores:[]})).concat(body.archivos.map(f=>({Ambito:'ARCHIVO',RaizIdCliente:f.raizIdCliente,ArchivoIdCliente:f.idCliente,Valores:[]})));
    output.valores=all.slice((input.pagina-1)*25,input.pagina*25);output.puedeHaberMas=input.pagina*25<all.length;
    output.unidadesElegiblesPagina=output.valores.filter(v=>v.Ambito==='RAIZ').map(v=>v.RaizIdCliente);
   }
  }else throw Error('Unexpected operation '+op);
  if(extra.loss===op&&!lost){lost=true;throw Object.assign(Error('ERROR_RED'),{codigo:'ERROR_RED',resultadoIncierto:true});}
  if(extra.alter)output=extra.alter(op,output);
  return structuredClone(output);
 }};
 const f={calls,parts,completed,api};f.engine=segmented.crear({api,solicitud:body,clave:'a'.repeat(32),maximoJson:102400,cambiar:value=>{if(extra.progress)extra.progress(value,f);},vigente:()=>f.live!==false});return f;
}
test('DOC120-SEGCLIENT01: mil metadatos, una preparación/revisión, sin recepción ni confirmación',async()=>{
 const f=fixture(),result=await f.engine.iniciar();assert.equal(result.resultado.Elementos.length,1001);assert.equal(result.resultado.UnidadesElegibles.length,1);
 assert.equal(f.calls.filter(c=>c.op==='IniciarPreparacionPlan').length,1);assert.equal(f.calls.filter(c=>c.op==='SellarPreparacionPlan').length,1);
 assert.equal(f.parts.size,11);assert.equal(f.completed.size,11);assert.ok(f.calls.every(c=>!['ConfirmarPlan','RecibirArchivo','AvanzarLote'].includes(c.op)));
 const before=f.calls.length;assert.equal(await f.engine.iniciar(),result);assert.equal(f.calls.length,before);
});
test('DOC120-SEGCLIENT02: corte por JSON real y persistencia Unicode, no solo por cantidad',()=>{
 const body=request(300);body.archivos.forEach(f=>{f.nombre='ñ😀'.repeat(80)+f.nombre;f.rutaRelativa='Egresos0/'+f.nombre;});
 const parts=segmented.particionar(body,4096);assert.ok(parts.length>4);
 for(const p of parts){const text=JSON.stringify({solicitud:{preparacionId:'4294967295',revisionEsperada:4294967294,tipo:p.tipo,numero:4294967295,raices:p.raices,archivos:p.archivos}});assert.ok(text.length<=4096);assert.ok(text.replace(/[\u007f-\uffff]/g,c=>'\\u'+c.charCodeAt(0).toString(16).padStart(4,'0')).length<=262144);assert.ok(p.raices.length+p.archivos.length<=100);}
 body.archivos[0].nombre='x'.repeat(70000);assert.throws(()=>segmented.particionar(body,65536),/REVISION_TAMANO_EXCEDIDO/);
 assert.equal(segmented.sumarDecimales('9007199254740991','9007199254740991'),'18014398509481982');
});
test('DOC120-SEGCLIENT03: pausa entre partes y continuación conservan identidad sin reenvío automático',async()=>{
 let paused=false;const f=fixture(request(),{progress:(value,fixture)=>{if(!paused&&value.fase==='REGISTRAR'&&value.archivosRegistrados>=100){paused=true;fixture.engine.pausar();}}});
 await assert.rejects(f.engine.iniciar(),/PREPARACION_PAUSADA/);const before=f.calls.length;await new Promise(setImmediate);assert.equal(f.calls.length,before);
 assert.ok((await f.engine.iniciar()).resultado.PuedeConfirmar);assert.equal(f.parts.size,11);
});
for(const operation of ['IniciarPreparacionPlan','RegistrarPartePreparacion','ValidarPartePreparacion','SellarPreparacionPlan'])test('DOC120-SEGCLIENT04: acuse perdido '+operation+' consulta una vez y espera consentimiento',async()=>{
 const f=fixture(request(4),{loss:operation});await assert.rejects(f.engine.iniciar(),/ERROR_RED/);
 assert.equal(f.calls.at(-1).op,'ConsultarPreparacionPlan');const before=f.calls.length;await new Promise(setImmediate);assert.equal(f.calls.length,before);
 const result=await f.engine.iniciar();assert.equal(result.resultado.Elementos.length,5);assert.equal(f.parts.size,2);assert.equal(f.completed.size,2);
 assert.equal(f.calls.filter(c=>c.op==='IniciarPreparacionPlan').length,1);assert.equal(f.calls.filter(c=>c.op==='SellarPreparacionPlan').length,1);
});
test('DOC120-SEGCLIENT05: cierre durante la recepción de metadatos no solicita otro paso',async()=>{
 const f=fixture(request(4),{progress:value=>{if(value.preparacionId)f.engine.destruir();}});await assert.rejects(f.engine.iniciar(),/SOLICITUD_CANCELADA/);assert.equal(f.calls.length,1);
});
test('DOC120-SEGCLIENT06: más de cien raíces se registran antes de validar las partes',async()=>{
 const f=fixture(request(150,150));await f.engine.iniciar();const firstValidation=f.calls.findIndex(c=>c.op==='ValidarPartePreparacion'),registrations=f.calls.filter(c=>c.op==='RegistrarPartePreparacion');assert.equal(registrations.length,4);assert.equal(f.calls.slice(0,firstValidation).filter(c=>c.op==='RegistrarPartePreparacion').length,4);
});
test('DOC120-SEGCLIENT07: datos de otro contexto, filas repetidas o páginas incompletas impiden revisar',async()=>{
 for(const mode of ['context','duplicate','missing']){const f=fixture(request(4),{alter:(op,value)=>{if(op==='ConsultarRevisionPlan'&&value.tipo==='VALORES'){if(mode==='context')value.nivelRef='99';if(mode==='duplicate')value.valores[1]=value.valores[0];if(mode==='missing')value.valores.pop();}return value;}});await assert.rejects(f.engine.iniciar(),/RESPUESTA_INVALIDA/);}
});
test('DOC120-SEGCLIENT08: continuar despues de fallar la lectura conserva el plan sellado sin repetir metadatos',async()=>{
 let failed=false;const f=fixture(request(),{alter:(op,value)=>{
  if(op==='ConsultarRevisionPlan'&&value.tipo==='VALORES'&&!failed){failed=true;throw Object.assign(Error('CAPACIDAD_NO_DISPONIBLE'),{codigo:'CAPACIDAD_NO_DISPONIBLE'});}return value;
 }});
 await assert.rejects(f.engine.iniciar(),/CAPACIDAD_NO_DISPONIBLE/);const before=f.calls.length;
 const result=await f.engine.iniciar();assert.equal(result.planId,'11');assert.equal(result.resultado.Elementos.length,1001);
 assert.ok(f.calls.slice(before).every(c=>['ConsultarPreparacionPlan','ConsultarRevisionPlan'].includes(c.op)));
 assert.equal(f.calls.filter(c=>c.op==='IniciarPreparacionPlan').length,1);assert.equal(f.calls.filter(c=>c.op==='SellarPreparacionPlan').length,1);
});
test('DOC120-ORIG01: carpeta completa vincula solo pendientes y separa modificados/recibidos/homónimos',()=>{
 const root={recursoId:'52'},rows=[{recursoId:'78',raizId:'52',nombre:'a.pdf',rutaRelativa:'Original/sub/a.pdf',bytesPrevistos:'677',ultimaModificacionLocal:'1234',estadoRecepcion:'PENDIENTE',estado:'NO_INICIADO'},
 {recursoId:'79',raizId:'52',nombre:'b.pdf',rutaRelativa:'Original/b.pdf',bytesPrevistos:'677',ultimaModificacionLocal:'1234',estadoRecepcion:'PENDIENTE',estado:'NO_INICIADO'},
 {recursoId:'80',raizId:'52',nombre:'c.pdf',rutaRelativa:'Original/c.pdf',estadoRecepcion:'RECIBIDO'},
 {recursoId:'81',raizId:'53',nombre:'a.pdf',rutaRelativa:'Original/sub/a.pdf',bytesPrevistos:'677',ultimaModificacionLocal:'1234',estadoRecepcion:'PENDIENTE',estado:'NO_INICIADO'}];
 const a={name:'a.pdf',webkitRelativePath:'Reseleccion/sub/a.pdf',size:677,lastModified:1234};
 let report=segmented.vincularOriginales(root,rows,[a,{name:'b.pdf',webkitRelativePath:'Reseleccion/b.pdf',size:678,lastModified:1234},{name:'c.pdf',webkitRelativePath:'Reseleccion/c.pdf',size:0,lastModified:0}]);
 assert.deepEqual(Object.keys(report.vinculados),['78']);assert.deepEqual(report.modificados,['79']);assert.deepEqual(report.faltantes,['79']);assert.equal(report.yaRecibidos,1);assert.equal(report.vinculados['81'],undefined);
 report=segmented.vincularOriginales(root,rows,[a,a]);assert.deepEqual(Object.keys(report.vinculados),[]);assert.deepEqual(report.ambiguos,['78']);
 assert.throws(()=>segmented.vincularOriginales(root,rows,[{...a,webkitRelativePath:'Reseleccion/../a.pdf'}]),/RUTA_RELATIVA_INVALIDA/);
});
test('DOC120-HASH01: huella de versiones usa dominio/lote/parentesco y detecta otro recurso',async()=>{
 const snapshot={estado:{loteId:'34'},raices:[{recursoId:'52',idCliente:'r1',revision:1}],archivos:[{recursoId:'78',raizId:'52',idCliente:'a1',revision:2}]};
 const expected=crypto.createHash('sha256').update('DOC120_RECURSOS_1\n34\nA:78:52:a1:2\nR:52:r1:1\n').digest('hex');
 assert.equal(await segmented.huellaRecursos(snapshot,crypto.webcrypto),expected);snapshot.archivos[0].revision++;assert.notEqual(await segmented.huellaRecursos(snapshot,crypto.webcrypto),expected);
});
async function focalRun(count,rootCount=1,foreign=false){
 const state={loteId:'34',nivelRef:'7',perfilId:'28',versionId:'43',rutaCanonica:'SYNTHETIC',estado:'RECIBIENDO',revision:1,totalRaices:rootCount,totalArchivos:count,detencionSolicitada:false,hayIntentosActivos:false,hayIncertidumbre:false,accionesPermitidas:['CONSULTAR','CONSULTAR_RAICES','SOLICITAR_AVANCE']};
 const roots=Array.from({length:rootCount},(_,i)=>({recursoId:String(i+1),idCliente:'r'+i,nombre:'Root',estado:'PENDIENTE',revision:1,recepcionListaParaCrear:false,tieneIntentoActivo:false}));
 const files=Array.from({length:count},(_,i)=>({recursoId:String(100+i),raizId:roots[i%rootCount].recursoId,idCliente:'a'+i,nombre:i+'.pdf',estadoRecepcion:'PENDIENTE',estado:'NO_INICIADO',revision:1,valoresEfectivosFijados:false,tieneIntentoActivo:false}));
 let reads=0,effects=0,active=0,maxActive=0,failure=null,heap=process.memoryUsage().heapUsed,maxHeap=heap,start=performance.now();
 const snapshot=()=>({estado:state,raices:roots,archivos:files});
 const engine=core.crearCoordinacion({vigente:()=>true,original:()=>true,fallar:e=>{failure=e.codigo;},
  leer:async(consume,limits,op,previous)=>{
   if(!op){for(let i=0;i<2+Math.ceil(rootCount/25)+Math.ceil(count/25);i++){consume();reads++;}return structuredClone(snapshot());}
   consume();reads++;const next={estado:structuredClone(state),raices:previous.raices.slice(),archivos:previous.archivos.slice()};
   if(op.tipo==='RAICES'){const i=roots.findIndex(r=>r.recursoId===op.row.recursoId);next.raices[i]=structuredClone(roots[i]);}
   else{const i=files.findIndex(f=>f.recursoId===op.row.recursoId),p=roots.findIndex(r=>r.recursoId===files[i].raizId);next.archivos[i]=structuredClone(files[i]);next.raices[p]=structuredClone(roots[p]);}
   if(foreign&&effects===1)files.at(-1).revision++;
   if(await segmented.huellaRecursos(next,crypto.webcrypto)!==await segmented.huellaRecursos(snapshot(),crypto.webcrypto))throw Object.assign(Error('CONFLICTO_REVISION'),{codigo:'CONFLICTO_REVISION'});
   return next;
  },
  ejecutar:async op=>{
   effects++;active++;maxActive=Math.max(maxActive,active);const row=(op.tipo==='RAICES'?roots:files).find(r=>r.recursoId===op.row.recursoId),result={loteId:'34',fase:op.fase,estado:'CONFIRMADO'};
   if(op.tipo==='RAICES'){Object.assign(row,{estado:'EXPEDIENTE_CONFIRMADO',expedienteId:String(2000+roots.indexOf(row)),relacionNivelId:String(3000+roots.indexOf(row))});Object.assign(result,{raizId:row.recursoId,expedienteId:row.expedienteId,relacionNivelId:row.relacionNivelId});}
   else{result.archivoId=row.recursoId;if(op.fase==='RECEPCION'){row.estadoRecepcion='RECIBIDO';result.estadoRecepcion='RECIBIDO';roots.find(r=>r.recursoId===row.raizId).recepcionListaParaCrear=files.filter(f=>f.raizId===row.raizId).every(f=>f.estadoRecepcion==='RECIBIDO');}
    if(op.fase==='MATERIALIZAR_VALORES')row.valoresEfectivosFijados=true;
    if(op.fase==='ALMACENAR'){Object.assign(row,{estado:'COMPLETADO',documentoId:String(4000+files.indexOf(row)),gabineteId:'10'});Object.assign(result,{documentoId:row.documentoId,gabineteId:'10'});}}
   row.revision++;state.revision++;state.estado=files.every(f=>f.estado==='COMPLETADO')?'COMPLETADO':'EN_CURSO';active--;maxHeap=Math.max(maxHeap,process.memoryUsage().heapUsed);return result;
  }});
 await engine.iniciar();return{engine,reads,effects,maxActive,failure,elapsedMs:performance.now()-start,sampledHeapIncrease:maxHeap-heap};
}
test('DOC120-FOCAL01: mil archivos completan todas las fases con 3044 lecturas y una mutación activa',async t=>{
 const result=await focalRun(1000);assert.equal(result.engine.estado().completo,true);assert.equal(result.effects,3001);assert.equal(result.reads,3044);assert.equal(result.maxActive,1);assert.ok(result.elapsedMs<=10000);assert.ok(result.sampledHeapIncrease<=128*1024*1024);t.diagnostic(JSON.stringify({files:1000,reads:result.reads,effects:result.effects,elapsedMs:Math.round(result.elapsedMs),sampledHeapIncrease:result.sampledHeapIncrease}));
});
test('DOC120-FOCAL02: múltiples raíces completan; cambio ajeno pausa después del acuse',async()=>{
 const normal=await focalRun(100,2);assert.equal(normal.engine.estado().completo,true);assert.equal(normal.effects,302);assert.equal(normal.maxActive,1);
 const changed=await focalRun(3,1,true);assert.equal(changed.effects,1);assert.equal(changed.engine.estado().estado,'blocked');assert.equal(changed.failure,'CONFLICTO_REVISION');
});
