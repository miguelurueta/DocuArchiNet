'use strict';
const test=require('node:test'),assert=require('node:assert/strict'),fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const {publishTestProfiles}=require('./support/doc114-published-test-profiles.cjs');
const proposalFile=path.resolve(__dirname,'../../../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc114-perfiles-propuestos-20261103.json');
const digest=v=>crypto.createHash('sha256').update(JSON.stringify(v)).digest('hex');
function fixture(failure){
 const proposal=JSON.parse(fs.readFileSync(proposalFile,'utf8'));proposal.authorized=true;
 const profile=JSON.parse(fs.readFileSync(path.resolve(__dirname,'../profiles/doc111-importacion-carpetas.profile.example.json'),'utf8'));profile.versionId='38';
 const configuration={Esquema:1,GabineteId:10,NombreVisible:'Source',ModoIntegridad:'COMPLETO_OBLIGATORIO',AmbitoIntegridad:'LOTE',AreaRef:'9'};
 const source={PerfilId:'24',VersionId:'38',NumeroVersion:2,Configuracion:configuration,Reglas:proposal.profiles[0].reglas.map((r,i)=>({...r,ReglaId:String(10+i)}))};
 const partial={...structuredClone(source),PerfilId:'25',VersionId:'39',Configuracion:{...configuration,ModoIntegridad:'PARCIAL_PERMITIDO',AmbitoIntegridad:'NO_APLICA'}};
 proposal.source.snapshotSha256=digest(source);proposal.preservePublications=[source,partial].map(s=>({perfilId:s.PerfilId,versionId:s.VersionId,snapshotSha256:digest(s)}));
 const calls=[],refs=[],attempts=[],created=new Map();let reads=0,duplicate=false,changeSource=false;
 const administrator={call:async(op,r)=>{
   calls.push({op,r:structuredClone(r)});if(op===failure)throw Error('PRIVATE_RESPONSE');
   if(op==='ListarPerfiles')return {Total:duplicate?1:0,Elementos:duplicate?[{Codigo:r.codigo}]:[]};
   if(op==='ObtenerPerfil'&&r.perfilId==='24')return {PerfilId:'24',AmbitoRef:'2',Version:{VersionId:'38',Estado:'PUBLICADA',Configuracion:structuredClone(configuration),Reglas:structuredClone(source.Reglas)}};
   if(op==='CrearPerfil'){
     const n=created.size,p={PerfilId:String(26+n),AmbitoRef:'2',Codigo:r.codigo,Activo:false,Revision:1,Version:{VersionId:String(40+n),PerfilId:String(26+n),Estado:'BORRADOR',Revision:1}};created.set(p.PerfilId,p);return structuredClone(p);
   }
   const p=created.get(r.perfilId);assert.ok(p);
   if(op==='GuardarBorrador'){assert.ok(r.reglas.every(x=>x.ReglaId===null));p.Version.Configuracion=structuredClone(r.configuracion);p.Version.Reglas=r.reglas.map((x,i)=>({...x,ReglaId:String(100+i),Proveedor:null}));p.Version.Revision++;}
   if(op==='PublicarVersion'){p.Version.Estado='PUBLICADA';p.VersionPublicadaId=p.Version.VersionId;p.Revision++;p.Version.Revision++;}
   if(op==='CambiarDisponibilidad'){p.Activo=r.activo;p.Revision++;}
   return structuredClone(p);
 }};
 const reader={refreshToken:async()=>{},request:async(op)=>{
   assert.equal(op,'AbrirImportacion');reads++;
   if(changeSource&&reads>1)partial.Configuracion.NombreVisible='External change';
   return {perfiles:[structuredClone(source),structuredClone(partial),...[...created.values()].filter(p=>p.Activo&&p.Version.Estado==='PUBLICADA').map(p=>({PerfilId:p.PerfilId,VersionId:p.Version.VersionId,Configuracion:structuredClone(p.Version.Configuracion),Reglas:structuredClone(p.Version.Reglas)}))],paginaPerfiles:{puedeHaberMas:false}};
 }};
 const authorization={environment:true,account:true,publication:true,discardable:true,tls:true};
 return {proposal,profile,authorization,calls,refs,attempts,source,duplicate(){duplicate=true;},changeSource(){changeSource=true;},run:()=>publishTestProfiles({},profile,proposal,authorization,r=>refs.push(r),{reader,administrator,onAttempt:r=>attempts.push(r)})};
}
test('DOC114 perfiles: crea/publica solo dos copias y conserva configuracion original',async()=>{
 const f=fixture(),result=await f.run();assert.equal(result.status,'PASS');assert.equal(result.sourcePreserved,true);assert.equal(result.plans,0);assert.equal(result.permissionChanges,0);
 assert.deepEqual(f.refs,[{key:'format-a',perfilId:'26',versionId:'40'},{key:'format-b',perfilId:'27',versionId:'41'}]);
 assert.equal(f.calls.filter(c=>c.op==='CrearPerfil').length,2);assert.equal(f.calls.filter(c=>c.op==='PublicarVersion').length,2);
 for(const c of f.calls.filter(c=>['GuardarBorrador','PublicarVersion','CambiarDisponibilidad'].includes(c.op)))assert.ok(['26','27'].includes(c.r.perfilId));
 for(const c of f.calls.filter(c=>c.op==='GuardarBorrador'))assert.equal(c.r.configuracion.AreaRef,'9');
 assert.equal(f.source.Configuracion.AmbitoIntegridad,'LOTE');assert.equal(f.attempts.length,8);
});
test('DOC114 perfiles: falta de aprobacion, reglas fuera de alcance o efecto extra bloquean antes de leer',async()=>{
 for(const change of [f=>f.proposal.authorized=false,f=>f.authorization.publication=false,f=>f.proposal.maximumEffects.newPlans=1,f=>f.proposal.profiles[0].configurationPatch.GabineteId=11,f=>f.proposal.profiles[0].reglas[4].PropiedadExpediente='CONSECUTIVO']){
   const f=fixture();change(f);await assert.rejects(f.run(),/DOC114_/);assert.equal(f.calls.length,0);assert.equal(f.refs.length,0);
 }
});
test('DOC114 perfiles: fuente cambiada o codigo existente impiden crear',async()=>{
 const changed=fixture();changed.proposal.source.snapshotSha256='f'.repeat(64);changed.proposal.preservePublications[0].snapshotSha256='f'.repeat(64);await assert.rejects(changed.run(),/SOURCE_CHANGED/);assert.equal(changed.calls.length,0);
 const duplicate=fixture();duplicate.duplicate();await assert.rejects(duplicate.run(),/NAME_ALREADY_PRESENT/);assert.equal(duplicate.calls.some(c=>c.op==='CrearPerfil'),false);
});
test('DOC114 perfiles: fallo incierto conserva referencias y no repite mutaciones',async()=>{
 for(const op of ['CrearPerfil','GuardarBorrador','PublicarVersion','CambiarDisponibilidad']){
   const f=fixture(op);await assert.rejects(f.run(),e=>e.message==='DOC114_PROFILE_OPERATION_NOT_CONFIRMED');assert.equal(f.calls.filter(c=>c.op===op).length,1);assert.equal(f.calls.at(-1).op,op);assert.equal(f.refs.length,op==='CrearPerfil'?0:1);assert.equal(f.attempts.at(-1).operation,op);
 }
});
test('DOC114 perfiles: cambio concurrente de fuente impide PASS incluso tras publicar copias',async()=>{
 const f=fixture();f.changeSource();await assert.rejects(f.run(),/SOURCE_CHANGED/);assert.equal(f.refs.length,2);
});
