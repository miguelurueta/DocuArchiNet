'use strict';
const {test}=require('node:test'),assert=require('node:assert/strict');
const {verifySourceGuards}=require('./support/doc110-source-guards.cjs');
function fixture(fault){
  const current={FuenteId:'1',Revision:6,Activo:false,Columnas:['CONSECUTIVO']};
  const snapshot={source:current,history:[{Revision:6,Accion:'REVOCAR'}]};
  const calls=[];let reads=0;
  const read=async()=>{reads++;return fault==='mutation'&&reads>1?{...snapshot,source:{...current,Revision:7}}:
    fault==='audit'&&reads>1?{...snapshot,history:[]}:structuredClone(snapshot);};
  const post=async(mode,method,payload)=>{
    calls.push({mode,method});assert.equal(payload.Cambio.Documento.Revision,5);
    assert.equal(payload.Cambio.Documento.FuenteId,'1');
    let status=mode==='ANONYMOUS'?401:mode!=='TRUSTED'?403:method==='CrearFuente'||payload.UsuarioRef?400:409;
    let code=mode==='ANONYMOUS'?'SESION_REQUERIDA':mode!=='TRUSTED'?'SOLICITUD_NO_VERIFICADA':payload.UsuarioRef?'CONFIGURACION_INVALIDA':method==='CrearFuente'?'SOLICITUD_INVALIDA':'CONFLICTO_REVISION';
    if(fault==='wrong-code')code='ACCESO_DENEGADO';if(fault==='success')status=200;
    return {status,result:{ok:status===200,codigo:code,datos:fault==='leak'?{}:null}};
  };
  return {current,read,post,calls,reads:()=>reads};
}
test('fuentes: diecisiete rechazos y lecturas posteriores de documento e historial sin cambiar revision',async()=>{
  const f=fixture(),result=await verifySourceGuards(f.post,f.read,f.current);
  assert.equal(result.checks,17);assert.equal(f.calls.length,17);assert.equal(f.reads(),18);
  assert.equal(f.current.Revision,6);
});
test('fuentes: rechazo por motivo incorrecto, efecto persistido, auditoria alterada o datos filtrados no acreditan guard',async()=>{
  for(const fault of ['wrong-code','mutation','audit','leak','success']){
    const f=fixture(fault);await assert.rejects(verifySourceGuards(f.post,f.read,f.current));
  }
});
