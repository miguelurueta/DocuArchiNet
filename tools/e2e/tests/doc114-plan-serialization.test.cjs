'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const {comparePreparationDocuments,comparePublishedSnapshot}=require('./support/doc114-plan-importacion.cjs');
function fixture(){return {snapshot:{VersionId:'40',Reglas:[{ReglaId:'1',FuenteTipo:'ARCHIVO',ValorConstante:null}]},result:{PuedeConfirmar:true,Elementos:[{Valores:[{DestinoRef:'14',Valor:null,Diferido:true,PropiedadPendiente:'ID'}]}]}};}
test('sólo Proveedor y Procedencia nulos equivalen a omisión Newtonsoft',()=>{
 const f=fixture(),http=structuredClone(f);http.snapshot.Reglas[0].Proveedor=null;http.result.Elementos[0].Valores[0].Procedencia=null;
 assert.deepEqual(comparePreparationDocuments(f.snapshot,http.snapshot,f.result,http.result),{snapshotEqual:true,resultEqual:true});
 assert.deepEqual(comparePreparationDocuments(http.snapshot,f.snapshot,http.result,f.result),{snapshotEqual:true,resultEqual:true});
});
test('publicación por Orden y snapshot persistido por destino conservan el mismo contenido',()=>{
 const publication={PerfilId:'26',VersionId:'40',Huella:'a'.repeat(64),Reglas:[{ReglaId:'1',DestinoTipo:'TIPOLOGIA',DestinoRef:'TIPO_DOCUMENTAL',Orden:0,Proveedor:null},{ReglaId:'2',DestinoTipo:'CAMPO_EXPEDIENTE',DestinoRef:'CODIGO',Orden:1,Proveedor:null}]};
 const stored=structuredClone(publication);stored.Reglas.reverse();stored.Reglas.forEach(r=>delete r.Proveedor);
 assert.equal(comparePublishedSnapshot(stored,publication),true);
 assert.equal(comparePreparationDocuments(stored,publication,null,null).snapshotEqual,false);
 for(const change of [p=>p.Reglas[0].Orden=3,p=>p.Reglas[0].ReglaId='99',p=>p.Huella='b'.repeat(64),p=>p.Reglas.push(structuredClone(p.Reglas[0]))]){
  const bad=structuredClone(publication);change(bad);assert.equal(comparePublishedSnapshot(stored,bad),false);
 }
});
test('proveedor configurado, procedencia, ID y otros nulos siguen verificándose',()=>{
 for(const change of [f=>f.snapshot.Reglas[0].Proveedor={ProveedorId:'1'},f=>f.snapshot.Reglas[0].ReglaId='2',f=>delete f.snapshot.Reglas[0].ValorConstante]){
  const a=fixture(),b=structuredClone(a);change(b);assert.equal(comparePreparationDocuments(a.snapshot,b.snapshot,a.result,b.result).snapshotEqual,false);
 }
 for(const change of [v=>v.Procedencia='1:2:prop',v=>v.Valor='99',v=>delete v.Valor,v=>v.PropiedadPendiente='CODIGO']){
  const a=fixture(),b=structuredClone(a);change(b.result.Elementos[0].Valores[0]);assert.equal(comparePreparationDocuments(a.snapshot,b.snapshot,a.result,b.result).resultEqual,false);
 }
});
