'use strict';
const {test}=require('node:test');
const assert=require('node:assert/strict');
const {verifyMissingSchema}=require('./support/doc110-missing-schema.cjs');
function fixture(fault){
  let missing=false,count=3;const operations=[];let uploads=0;
  const bridge={async send(operation){
    operations.push(operation);
    if(operation==='LEGACY_COUNT')return {ok:true,stage:operation,count};
    if(operation==='HIDE'){missing=true;if(fault==='uncertain-hide')throw new Error('synthetic uncertain result');return {ok:true,stage:'HIDDEN',missingOriginalTables:3};}
    missing=false;return {ok:true,stage:'RESTORED',profileDataAndSchemaUnchanged:fault!=='changed-profile'};
  }};
  return {options:{bridge,listProfiles:async()=>missing?{status:503,result:{ok:false,codigo:'CAPACIDAD_NO_DISPONIBLE',datos:null}}:{status:200,result:{ok:true,datos:{}}},
    checkUnavailableUI:async()=>{if(fault==='ui')throw new Error('synthetic UI failure');},
    upload:async()=>{uploads++;if(fault==='upload')throw new Error('synthetic upload failure');count+=fault==='double-document'?2:1;return {count:1,codes:{storage:'CONFIRMED',projection:'CONFIRMED'}};}
  },operations,uploads:()=>uploads,restored:()=>!missing};
}
test('carga una vez sin esquema, confirma SELECT y restaura antes de recuperar API',async()=>{
  const f=fixture();assert.equal((await verifyMissingSchema(f.options)).documentDelta,1);
  assert.equal(f.uploads(),1);assert.equal(f.restored(),true);assert.equal(f.operations.at(-1),'RESTORE');
});
test('restaura ante fallo UI, carga, ocultacion incierta o doble registro sin reintentar',async()=>{
  for(const fault of ['ui','upload','uncertain-hide','double-document']){
    const f=fixture(fault);await assert.rejects(verifyMissingSchema(f.options));
    assert.equal(f.restored(),true);assert.equal(f.operations.at(-1),'RESTORE');assert.ok(f.uploads()<=1);
  }
});
test('no acredita restauracion si cambia contenido o esquema de perfiles',async()=>{
  const f=fixture('changed-profile');await assert.rejects(verifyMissingSchema(f.options));assert.equal(f.restored(),true);
});
