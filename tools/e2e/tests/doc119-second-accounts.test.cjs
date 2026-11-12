'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const {BASE_URL}=require('./support/doc119-seguimiento.cjs');
const {sourceLot,findSecondAccounts,selectSecondAccount}=require('./support/doc119-second-accounts.cjs');
const {parseArgs}=require('../scripts/inspect-doc119-second-accounts.cjs');
const runId='a'.repeat(32);
test('DOC119 inventario vincula lote a la corrida sin aceptar rutas o destino ajenos',()=>{
  assert.deepEqual(parseArgs(['--run',runId,'--source-run','b'.repeat(32)]),{runId,sourceRunId:'b'.repeat(32)});
  assert.throws(()=>parseArgs(['--run','../private','--source-run',runId]),/ARGUMENT_INVALID/);
  const source={runId,baseUrl:BASE_URL,authorizedPlan:true,authorizationRevision:'20261112-http600',resources:{lotId:'5'}};
  assert.equal(sourceLot(source,runId),'5');
  for(const patch of [{authorizedPlan:false},{baseUrl:'https://elsewhere.invalid/'},{resources:{lotId:'0'}}])assert.throws(()=>sourceLot({...source,...patch},runId),/SOURCE_INVALID/);
});
test('DOC119 inventario limita SELECT a empresa del lote, excluye propietario y no proyecta secretos',async()=>{
  const calls=[];
  const result=await findSecondAccounts(async(sql,args)=>{calls.push({sql,args});return calls.length===1?[{owner_ref:'141',company_ref:'2'}]:[{user_ref:'142',login:'OTRA.PRUEBA',company_ref:'2',active:1,can_import:1,password:'synthetic-never-projected'}];},'5');
  assert.equal(calls.length,2);assert.deepEqual(calls[0].args,['5']);assert.deepEqual(calls[1].args,['2','141']);
  assert.match(calls[1].sql,/u\.Estado_Usuario=1 AND p\.IMPORTAR_CARPETAS=1/);assert.match(calls[1].sql,/LIMIT 30$/);
  for(const call of calls){assert.match(call.sql,/^SELECT /);assert.doesNotMatch(call.sql,/\b(?:UPDATE|INSERT|DELETE|CALL|SET|DROP)\b/i);assert.doesNotMatch(call.sql,/\b(?:PASW|password|PASW_DBMS_MODULO)\b/i);}
  assert.deepEqual(result,{lotId:'5',ownerRef:'141',companyRef:'2',candidates:[{userRef:'142',login:'OTRA.PRUEBA'}]});
});
test('DOC119 inventario informa cero candidatas y rechaza identidad/permiso discordantes',async()=>{
  const scope=[{owner_ref:'141',company_ref:'2'}];
  assert.deepEqual((await findSecondAccounts(async sql=>sql.includes('FROM ra_integracion_lote')?scope:[],'5')).candidates,[]);
  for(const patch of [{user_ref:'141'},{company_ref:'3'},{can_import:0},{active:0},{login:'CONSULTAPUBLICO'},{login:'bad\u001blogin'}]){
    const row={user_ref:'142',login:'OTRA.PRUEBA',company_ref:'2',active:1,can_import:1,...patch};
    await assert.rejects(()=>findSecondAccounts(async sql=>sql.includes('FROM ra_integracion_lote')?scope:[row],'5'),/RESULT_INVALID/);
  }
  let calls=0;await assert.rejects(()=>findSecondAccounts(async()=>{calls++;return [];},'5'),/SCOPE_INVALID/);assert.equal(calls,1);
});

test('DOC119 login usa nombre canonico de la unica candidata sin volver a solicitar usuario',async()=>{
  let calls=0;
  const candidate=await selectSecondAccount(async()=>++calls===1?[{owner_ref:'141',company_ref:'2'}]:[{user_ref:'136',login:' CAROLINA.CRUZ ',company_ref:'2',active:1,can_import:1}],'5',async()=>assert.fail('single candidate must not ask for an account name'));
  assert.deepEqual(candidate,{userRef:'136',login:'CAROLINA.CRUZ',companyRef:'2'});assert.equal(calls,2);
});

test('DOC119 seleccion multiple solo acepta una cuenta del inventario vigente y cero candidatas no inicia login',async()=>{
  const users=[{user_ref:'136',login:'CAROLINA.CRUZ',company_ref:'2',active:1,can_import:1},{user_ref:'142',login:'OTRA.PRUEBA',company_ref:'2',active:1,can_import:1}];
  const query=async sql=>sql.includes('FROM ra_integracion_lote')?[{owner_ref:'141',company_ref:'2'}]:users;
  assert.equal((await selectSecondAccount(query,'5',async items=>items[1])).login,'OTRA.PRUEBA');
  await assert.rejects(()=>selectSecondAccount(query,'5',async()=>({userRef:'999',login:'FUERA'})),/SELECTION_INVALID/);
  await assert.rejects(()=>selectSecondAccount(async sql=>sql.includes('FROM ra_integracion_lote')?[{owner_ref:'141',company_ref:'2'}]:[],'5',async()=>assert.fail('no candidates must stop')),/SECOND_ACCOUNT_UNAVAILABLE/);
});
