'use strict';
const {test}=require('node:test');
const assert=require('node:assert/strict');
const {verifyPermissionRevocation}=require('./support/doc110-permission-revocation.cjs');
function fixture(fault){
  let granted=true,changed=false;const operations=[],writes=[],ui=[];
  const bridge={companyRef:'2',async send(operation){
    operations.push(operation);granted=operation==='RESTORE';changed=true;
    if(operation==='REVOKE'&&fault==='revoke-transport')throw new Error('synthetic uncertain result');
    return {ok:true,stage:operation==='RESTORE'?'RESTORED':'REVOKED',otherFieldsUnchanged:!(fault==='other-fields'&&operation==='REVOKE')};
  }};
  const call=async(method,data)=>{
    if(method==='GuardarBorrador')writes.push(data);
    if(!granted&&fault!=='cached-permission')return {status:fault==='session-lost'?401:403,
      result:{ok:false,codigo:fault==='session-lost'?'SESION_REQUERIDA':'ACCESO_DENEGADO',datos:null}};
    const profile={PerfilId:'1',AmbitoRef:'2',Revision:changed&&fault==='changed-profile'?2:1};
    return {status:200,result:{ok:true,datos:method==='ObtenerPerfil'?profile:method==='ConsultarHistorial'?[{Numero:1}]:{Elementos:[],Total:0}}};
  };
  return {options:{bridge,call,profileId:'1',checkUI:async allowed=>{ui.push(allowed);if(!allowed&&fault==='ui-failure')throw new Error('synthetic UI failure');}},
    operations,writes,ui,granted:()=>granted};
}
test('revocacion en sesion vigente, restauracion y documentos intactos sin guardado valido',async()=>{
  const f=fixture();assert.equal((await verifyPermissionRevocation(f.options)).checks,5);
  assert.deepEqual(f.operations,['REVOKE','RESTORE']);assert.equal(f.granted(),true);
  assert.deepEqual(f.ui,[false,true]);assert.equal(f.writes.length,1);assert.equal(f.writes[0].versionId,'0');
});
test('restaura incluso ante cache indebida, sesion perdida, fallo UI o resultado incierto',async()=>{
  for(const fault of ['cached-permission','session-lost','ui-failure','revoke-transport','other-fields']){
    const f=fixture(fault);await assert.rejects(verifyPermissionRevocation(f.options));
    assert.deepEqual(f.operations,['REVOKE','RESTORE']);assert.equal(f.granted(),true);
  }
});
test('no acredita PASS cuando se altera el perfil aunque el permiso se restaure',async()=>{
  const f=fixture('changed-profile');await assert.rejects(verifyPermissionRevocation(f.options));assert.equal(f.granted(),true);
});
test('contexto empresarial distinto detiene antes de retirar permiso',async()=>{
  const f=fixture();f.options.bridge.companyRef='3';
  await assert.rejects(verifyPermissionRevocation(f.options));assert.deepEqual(f.operations,[]);
});
