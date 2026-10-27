'use strict';
const {test}=require('node:test');
const assert=require('node:assert/strict');
const {sessionIdleMinutes,verifySessionExpiry}=require('./support/doc110-session-expiry.cjs');
function fixture(fault){
  let expired=false,closed=false,waits=0;const writes=[];
  const profile={PerfilId:'1',Version:{Estado:'PUBLICADA'}},history=[{Numero:1}];
  const ok=datos=>({status:200,result:{ok:true,datos}});
  const denied=(status,codigo)=>({status,result:{ok:false,codigo,datos:null}});
  const control=async(method,data)=>{
    if(method==='GuardarBorrador')throw new Error('No valid control write allowed');
    return ok(method==='ObtenerPerfil'?{...profile,...(expired&&fault==='changed-profile'?{Revision:2}:{})}:
      method==='ConsultarHistorial'?history:{Elementos:[],Total:0});
  };
  const expiring=async(method,data)=>{
    if(method==='GuardarBorrador')writes.push(data);
    return expired&&fault!=='still-authenticated'?denied(401,'SESION_REQUERIDA'):ok({Elementos:[],Total:0});
  };
  return {options:{control,expiring,profileId:'1',
    foreignControl:async(method,data)=>{if(method==='GuardarBorrador')writes.push(data);return fault==='wrong-token-error'?denied(403,'ACCESO_DENEGADO'):denied(403,'SOLICITUD_NO_VERIFICADA');},
    beforeIdle:async()=>{closed=true;},waitIdle:async keepAlive=>{assert.equal(closed,true);waits++;await keepAlive();expired=true;}
  },writes,waits:()=>waits};
}
test('sesion previamente valida expira; token ajeno y escritura invalida no alteran perfil',async()=>{
  const f=fixture();assert.equal((await verifySessionExpiry(f.options)).checks,4);
  assert.equal(f.waits(),1);assert.equal(f.writes.length,2);
  assert.ok(f.writes.every(data=>data.versionId==='0'&&data.configuracion===null));
});
test('no acredita sesion aun vigente, perfil cambiado ni error de permiso como CSRF',async()=>{
  for(const fault of ['still-authenticated','changed-profile','wrong-token-error']){
    const f=fixture(fault);await assert.rejects(verifySessionExpiry(f.options));
  }
});
test('timeout procede de sessionState explicito y acotado, sin inferir configuracion ausente',()=>{
  assert.equal(sessionIdleMinutes('<sessionState mode="InProc" timeout="10" />'),10);
  for(const config of ['', '<sessionState />','<other timeout="10" />','<sessionState timeout="0" />','<sessionState timeout="61" />'])
    assert.throws(()=>sessionIdleMinutes(config),/TIMEOUT_CONFIGURATION_REQUIRED/);
});
