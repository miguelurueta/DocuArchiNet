'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),os=require('node:os'),path=require('node:path');
const {queries,inspect,compile,validateCompiled,MAX_SELECT}=require('../tools/e2e/scripts/inspect-doc120-independent-preflight.cjs');
function data(){return{lot:[{lote_id:'50',solicitante_ref:'141',ambito_ref:'2',nivel_ref:'223',detencion_solicitada:'0',total_archivos:'1000',total_raices:'1',revision:'3057'}],plan:[{}],fragments:[],roots:[{}],files:Array.from({length:1000},()=>({})),attempts:[]};}
const result={ok:true,stage:'PURE_PREFLIGHT_PASSED',readOnly:true,writerInvoked:false};
test('PURE01: siete SELECT cerrados al mismo lote, sin bloqueos, efectos ni HTTP',()=>{
 assert.equal(queries().length,MAX_SELECT);assert.equal(MAX_SELECT,7);
 for(const q of queries()){assert.match(q.sql,/^SELECT /);assert.match(q.sql,/l\.lote_id=\? AND l\.solicitante_ref=\? AND l\.ambito_ref=\?/);assert.equal((q.sql.match(/\?/g)||[]).length,3);assert.doesNotMatch(q.sql,/;|\b(?:UPDATE|INSERT|DELETE|ALTER|CREATE|REPLACE|CALL|LOCK|OUTFILE)\b/i);assert.match(q.sql,/LIMIT [0-9]+$/);}
});
test('PURE02: cambio de revision, propietario, inventario o actividad impide invocar el host',async()=>{
 for(const change of [d=>d.lot[0].solicitante_ref='142',d=>d.lot[0].detencion_solicitada='1',d=>d.files.pop(),d=>d.attempts.push({estado:'INCIERTO'})]){
  const d=data();change(d);let invoked=0;await assert.rejects(inspect(async q=>d[q.name==='after'?'lot':q.name],async()=>{invoked++;return result;}));assert.equal(invoked,0);
 }
 const d=data();await assert.rejects(inspect(async q=>q.name==='after'?[{...d.lot[0],revision:'3058'}]:d[q.name],async()=>result),/READ_CHANGED/);
});
test('PURE03: conserva solo diagnostico cerrado; nunca acepta mensaje privado o un escritor invocado',async()=>{
 const d=data(),read=async q=>d[q.name==='after'?'lot':q.name];let called=0;
 assert.deepEqual(await inspect(read,async packet=>{called++;assert.equal(packet.files.length,1000);assert.equal(packet.after,undefined);return result;}),result);assert.equal(called,1);
 for(const invalid of [{...result,message:'private-sentinel'},{...result,writerInvoked:true},{...result,methods:['a secret']},{...result,stage:'private-sentinel'}])await assert.rejects(inspect(read,async()=>invalid),/HOST_RESULT_INVALID/);
});
test('PURE04: host compilado sobre DLL real no publica mensajes ni contenido privado al fallar un lector',()=>{
 const folder=fs.mkdtempSync(path.join(os.tmpdir(),'doc120-pure-host-'));
 try{
  const exe=compile(folder),binary=fs.readFileSync(exe);assert.equal(binary.readUInt16LE(binary.readUInt32LE(0x3c)+24),0x20b,'Diagnostic must use 64-bit DSN view');const packet={lot:[{lote_id:'50',solicitante_ref:'141',ambito_ref:'2',nivel_ref:'223',plan_id:'90',contexto_doc:'{}',config_doc:'{}',total_raices:'1',total_archivos:'1000',total_bytes:'0',estado:'EN_CURSO',revision:'3057',detencion_solicitada:'0'}],plan:[{private:'private-sentinel'}]};
  const r=validateCompiled(exe,packet);assert.equal(r.ok,false);assert.equal(r.stage,'READ_PLAN');assert.equal(r.readOnly,true);assert.equal(r.writerInvoked,false);assert.match(r.exceptionType,/Exception$/);assert.doesNotMatch(JSON.stringify(r),/private-sentinel/);
  const native=validateCompiled(exe,{dsn:'unapproved',user:'private-sentinel',password:'private-sentinel'},true);assert.equal(native.ok,false);assert.equal(native.stage,'NATIVE_INPUT');assert.equal(native.selectReserved,0);assert.equal(native.writerInvoked,false);assert.doesNotMatch(JSON.stringify(native),/private-sentinel/);
 }finally{assert.equal(path.dirname(folder),os.tmpdir());assert.ok(path.basename(folder).startsWith('doc120-pure-host-'));fs.rmSync(folder,{recursive:true,force:true});}
});
