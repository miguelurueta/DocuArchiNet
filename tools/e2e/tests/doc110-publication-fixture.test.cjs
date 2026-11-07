'use strict';
const {test}=require('node:test');
const assert=require('node:assert/strict');
const {discoverFixture}=require('./support/doc110-publication-fixture.cjs');
function mock(options={}){
  return async(method,request)=>{
    assert.equal(method,'ConsultarCatalogo');
    const c=request.configuracion;
    const ids={FONDO:'11',TIPO_EXPEDIENTE:'12',UNIDAD_DOCUMENTAL:'13',INSTRUMENTO:'14',AREA:'15',SERIE:'16',SUBSERIE:'17',GABINETE:'18'};
    if(request.catalogo==='AREA')assert.equal(c.InstrumentoRef,'14');
    if(request.catalogo==='SERIE')assert.equal(c.AreaRef,'15');
    if(request.catalogo==='TIPOLOGIA')return {datos:options.noTypology || c.SubserieRef!=='17'?[]:[{value:'19'}]};
    if(request.catalogo==='INDICE'){
      assert.equal(c.GabineteId,18);assert.equal(c.OrigenGabinete,'DA');
      return {datos:[{value:'20',Tipo:options.unsupported?'NO_SOPORTADO':'TEXTO',Longitud:2,Requerida:true}]};
    }
    assert.ok(ids[request.catalogo]);return {datos:[{value:ids[request.catalogo]}]};
  };
}
test('DOC110: obtiene referencias autorizadas dependientes y regla requerida del gabinete DA',async()=>{
  const f=await discoverFixture(mock());
  assert.equal(f.configuracion.SubserieRef,'17');assert.equal(f.configuracion.GabineteId,18);
  assert.equal(f.reglas.find(r=>r.DestinoTipo==='TIPOLOGIA').ValorConstante,'19');
  const index=f.reglas.find(r=>r.DestinoTipo==='INDICE_GABINETE');
  assert.equal(index.DestinoRef,'20');assert.equal(index.ValorConstante,'EE');assert.equal(index.Obligatoria,true);
});
test('DOC110: no inventa tipología ni ignora índice requerido no soportado',async()=>{
  await assert.rejects(()=>discoverFixture(mock({noTypology:true})),/NO_CLASSIFICATION_WITH_TYPOLOGY/);
  await assert.rejects(()=>discoverFixture(mock({unsupported:true})),/NO_COMPATIBLE_DA_CABINET/);
});
test('DOC110: no oculta errores del catálogo autorizado',async()=>{
  await assert.rejects(()=>discoverFixture(async()=>{throw new Error('CATALOG_UNAVAILABLE');}),/CATALOG_UNAVAILABLE/);
});
