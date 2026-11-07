'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const {createPlanControls,runPlanSession}=require('./support/doc114-plan-importacion.cjs');
const profile=require('../profiles/doc111-importacion-carpetas.profile.example.json');
test('control documental aplica empresa del inventario y vincula documento, inventario, expediente y gabinete',async()=>{
 const controls=createPlanControls(async(origin,sql,args)=>{
  assert.match(sql,/^SELECT /);assert.doesNotMatch(sql,/;|FOR UPDATE/);
  if(sql.includes('SELECT gabinete_id'))return [{gabinete_id:'10'}];
  if(sql.includes('FROM system1'))return [{NOMBRE:'fixture_cabinet'}];
  if(origin==='DA'){
   assert.match(sql,/r\.ID_EMPRESA_DOCUMENTO=\?/);assert.doesNotMatch(sql,/g\.ID_EMPRESA_DOCUMENTO/);
   assert.match(sql,/r\.ID_REGISTRO_PRODUCION_DOCUMENTAL=g\.ID_INVENTARIO_DOCUMENTAL/);
   assert.match(sql,/r\.ID_DOCUMENTO_DOCUARCHI_ALMACEN=g\.ID/);
   assert.match(sql,/r\.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=g\.ID_EXPEDIENTE/);
   assert.deepEqual(args,['1','2','fixture_cabinet']);
  }
  return [{total:'0'}];
 },profile,{userRef:'1',ambitoRef:'2'});
 const s=await controls.snapshot();assert.equal(s.documents,'0');assert.equal(controls.unchanged(s,{...s,plans:'1'},1),true);
 await assert.rejects(async()=>controls.unchanged(s,{...s,documents:'1'},0),/DOCUMENT_EFFECT_FOUND/);
});
test('fallo de control inicial registra fase anterior al HTTP sin preparar',async()=>{
 const session=structuredClone(require('../profiles/doc114-plan-session.profile.example.json')),progress=[];
 let posts=0;const auth={context:{request:{post:async()=>posts++}}};
 await assert.rejects(runPlanSession(auth,session,{},async()=>{throw Error('DOC111_SELECT_COLUMN_UNAVAILABLE');},()=>{throw Error('unexpected reference');},{onProgress:p=>progress.push(p)}),/^Error: DOC114_CONTROL_VERSION_COLUMN_UNAVAILABLE$/);
 assert.equal(posts,0);assert.deepEqual(progress,[{caseKey:session.cases[0].key,versionId:session.profiles[0].versionId,phase:'CONTROL_BEFORE'}]);
});
test('error SELECT conserva solo etiqueta y codigo seguro',async()=>{
 const controls=createPlanControls(async(origin,sql)=>{
  if(sql.includes('SELECT gabinete_id'))return [{gabinete_id:'10'}];
  if(sql.includes('FROM system1'))return [{NOMBRE:'fixture_cabinet'}];
  if(origin==='DA')throw Error('DOC111_SELECT_COLUMN_UNAVAILABLE');
  return [{total:'0'}];
 },profile,{userRef:'1',ambitoRef:'2'});
 await assert.rejects(controls.snapshot(),/^Error: DOC114_CONTROL_DOCUMENTS_COLUMN_UNAVAILABLE$/);
 const privateError=createPlanControls(async()=>{throw Error('private credentials and response');},profile,{userRef:'1',ambitoRef:'2'});
 await assert.rejects(privateError.snapshot(),/^Error: DOC114_CONTROL_VERSION_FAILED$/);
});
