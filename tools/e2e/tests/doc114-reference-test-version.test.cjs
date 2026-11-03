'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const {publishReferenceVersion}=require('./support/doc114-reference-test-version.cjs');
const sourceProposal=require('../../../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc114-perfiles-propuestos-20261103.json');
const proposalBase=require('../../../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc114-format-b-reference-proposal-20261103.json');
function fixture(failAt){
 const proposal=structuredClone(proposalBase);proposal.authorized=true;
 const profile=structuredClone(require('../profiles/doc111-importacion-carpetas.profile.example.json'));profile.baseUrl='https://localhost/GestionDocumental-Docuarchi.net/';profile.nivelRef='7';
 const authorization={environment:true,account:true,publication:true,discardable:true,tls:true};
 const config={NombreVisible:'source',GabineteId:10,ModoIntegridad:'COMPLETO_OBLIGATORIO',AmbitoIntegridad:'LOTE'};
 const old=new Map(proposal.preserve.map(ref=>[ref.versionId,{PerfilId:ref.perfilId,AmbitoRef:'2',Activo:true,Codigo:ref.perfilId==='27'?proposal.source.codigo:'other-'+ref.perfilId,Revision:1,Version:{PerfilId:ref.perfilId,VersionId:ref.versionId,Revision:1,Estado:'PUBLICADA',Configuracion:ref.versionId==='41'?{...config,...sourceProposal.profiles[1].configurationPatch}:config,Reglas:ref.versionId==='41'?sourceProposal.profiles[1].reglas:sourceProposal.profiles[0].reglas}}]));
 let current;const calls=[],refs=[],attempts=[];
 const runtime={administrator:{call:async(op,payload)=>{
  calls.push(op);if(op===failAt)throw Error('private response must not leak');
  if(op==='ObtenerPerfil')return structuredClone(payload.versionId==='42'?current:old.get(payload.versionId));
  if(op==='CrearVersion'){current=structuredClone(old.get('41'));current.Revision++;current.Version={...current.Version,VersionId:'42',Estado:'BORRADOR'};}
  else if(op==='GuardarBorrador'){current.Version.Configuracion=payload.configuracion;current.Version.Reglas=payload.reglas.map((r,i)=>({...r,ReglaId:String(100+i),Proveedor:null}));current.Version.Revision++;}
  else if(op==='PublicarVersion'){current.Version.Estado='PUBLICADA';current.Revision++;current.Version.Revision++;}
  else throw Error('unexpected operation');
  return structuredClone(current);
 }},reader:{refreshToken:async()=>{},request:async()=>({perfiles:[{PerfilId:'27',VersionId:'42',Configuracion:current.Version.Configuracion,Reglas:current.Version.Reglas}]})},onAttempt:a=>attempts.push(a)};
 return {proposal,profile,authorization,runtime,old,calls,refs,attempts,run:()=>publishReferenceVersion({},profile,proposal,authorization,r=>refs.push(r),runtime)};
}
test('nueva version cambia solo tipo de valor y preserva las cuatro publicaciones',async()=>{
 const f=fixture(),before=structuredClone([...f.old]);const result=await f.run();
 assert.equal(result.status,'PASS');assert.equal(result.versionId,'42');assert.equal(result.plans,0);
 assert.deepEqual([...f.old],before);assert.deepEqual(f.refs,[{perfilId:'27',versionId:'42'}]);
 assert.deepEqual(f.attempts.map(a=>a.operation),['CrearVersion','GuardarBorrador','PublicarVersion']);
});
test('autorizacion ausente o delta adicional bloquean antes de consultar',async()=>{
 for(const alter of [f=>f.proposal.authorized=false,f=>f.proposal.change.rules[0].LongitudMaxima=1,f=>f.proposal.effects.plans=1]){
  const f=fixture();alter(f);await assert.rejects(f.run(),/^Error: DOC114_/);assert.equal(f.calls.length,0);
 }
});
test('origen cambiado impide crear version',async()=>{
 const f=fixture();f.old.get('41').Version.Reglas=structuredClone(f.old.get('41').Version.Reglas);f.old.get('41').Version.Reglas[0].Separador='X';
 await assert.rejects(f.run(),/SOURCE_CHANGED/);assert.equal(f.attempts.length,0);
});
test('fallo en cada mutacion conserva referencia disponible y nunca reintenta',async()=>{
 for(const op of ['CrearVersion','GuardarBorrador','PublicarVersion']){
  const f=fixture(op);await assert.rejects(f.run(),/^Error: DOC114_B_VERSION_OPERATION_NOT_CONFIRMED$/);
  assert.equal(f.calls.filter(x=>x===op).length,1);assert.equal(f.refs.length,op==='CrearVersion'?0:1);
 }
});
test('alteracion del historial o catalogo operativo impide PASS despues de publicar',async()=>{
 const f=fixture(),call=f.runtime.administrator.call;let reads=0;
 f.runtime.administrator.call=async(op,p)=>{const result=await call(op,p);if(op==='ObtenerPerfil'&&++reads>5&&p.versionId==='38')result.Version.Revision++;return result;};
 await assert.rejects(f.run(),/HISTORY_CHANGED/);assert.equal(f.refs.length,1);
 const g=fixture();g.runtime.reader.request=async()=>({perfiles:[]});await assert.rejects(g.run(),/OPERATIVE_UNAVAILABLE/);assert.equal(g.refs.length,1);
});
