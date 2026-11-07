'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const {preparePartialProfile,validatePartialProposal}=require('./support/doc111-partial-profile.cjs');
const runner=require('../scripts/run-doc111-importacion-carpetas.cjs');
function fixture(failure){
  const profile={baseUrl:'https://example.invalid/app/',mode:'PARCIAL',nivelRef:'7',versionId:'38',rootNames:['PAR-01','PAR-02']};
  const config={ModoIntegridad:'PARCIAL_PERMITIDO',AmbitoIntegridad:'NO_APLICA',GabineteId:10,Extensiones:['pdf']};
  const proposal={authorized:true,sourceProfileId:'24',sourceVersionId:'38',levelId:'7',cabinetId:'10',rootNames:profile.rootNames,validPdfs:2,rejectedManifestFiles:1,newProfileCode:'DOC111-PARCIAL',newProfileName:'Prueba parcial',configuracion:config,reglas:[{ReglaId:'11',DestinoRef:'14',ValorConstante:'EEE'}]};
  const authorization={environment:true,account:true,effects:true,discardable:true,select:true};
  const calls=[],references=[];
  let p={PerfilId:'30',Revision:1,Activo:false,Version:{VersionId:'40',PerfilId:'30',Estado:'BORRADOR',Revision:1}};
  const auth={page:{locator:()=>({count:async()=>1,getAttribute:async()=>'PRIVATE_TOKEN'})},context:{request:{post:async(url,options)=>{
    const op=url.split('/').at(-1),r=options.data.solicitud;calls.push({op,r});
    assert.equal(options.headers['X-Importacion-Perfil-CSRF'],'PRIVATE_TOKEN');assert.equal(options.maxRetries,0);
    if(op===failure)return {status:()=>409,json:async()=>({d:{ok:false,private:'PRIVATE_BODY'}})};
    let data;
    if(op==='ObtenerPerfil')data={PerfilId:'24',Version:{VersionId:'38',Estado:'PUBLICADA',Configuracion:{...config,ModoIntegridad:'COMPLETO_OBLIGATORIO',AmbitoIntegridad:'LOTE'},Reglas:proposal.reglas}};
    else if(op==='CrearPerfil')data=p;
    else {assert.equal(r.perfilId,'30');
      if(op==='GuardarBorrador')p={...p,Version:{...p.Version,Revision:2,Configuracion:r.configuracion,Reglas:r.reglas.map(rule=>({...rule,ReglaId:'50'}))}};
      if(op==='PublicarVersion')p={...p,Revision:2,Version:{...p.Version,Estado:'PUBLICADA',Revision:3}};
      if(op==='CambiarDisponibilidad')p={...p,Activo:true,Revision:3};data=p;
    }
    return {status:()=>200,json:async()=>({d:{ok:true,datos:structuredClone(data)}})};
  }}}};
  return {profile,proposal,authorization,auth,calls,references,run:()=>preparePartialProfile(auth,profile,proposal,authorization,r=>references.push(r))};
}
test('PARCIAL: publica un perfil nuevo con CSRF existente y usa la versión devuelta, preservando origen',async()=>{
  const f=fixture(),original=JSON.stringify(f.proposal);assert.equal((await f.run()).versionId,'40');
  assert.deepEqual(f.calls.map(c=>c.op),['ObtenerPerfil','CrearPerfil','GuardarBorrador','PublicarVersion','CambiarDisponibilidad']);
  assert.deepEqual(f.references,[{perfilId:'30',versionId:'40'}]);assert.equal(JSON.stringify(f.proposal),original);assert.equal(f.profile.versionId,'38');
  assert.equal(f.calls[2].r.reglas[0].ReglaId,null);
});
test('PARCIAL: autorización concreta y perfil concordante antes de consultar o mutar',async()=>{
  for(const alter of [f=>f.proposal.authorized=false,f=>f.authorization.effects=false,f=>f.profile.mode='LOTE',f=>f.proposal.rootNames=['other'],f=>f.proposal.configuracion.AmbitoIntegridad='LOTE']){
    const f=fixture();alter(f);await assert.rejects(f.run(),/DOC111_/);assert.equal(f.calls.length,0);
  }
});
test('PARCIAL: origen cambiado y host administrativo ausente impiden crear',async()=>{
  const f=fixture();f.proposal.reglas=[{DestinoRef:'14',ValorConstante:'OTHER'}];
  // El servidor conserva el snapshot original independiente del objeto propuesto.
  const original=f.auth.context.request.post;
  f.auth.context.request.post=async(...args)=>{const r=await original(...args);const json=r.json;r.json=async()=>{const value=await json();value.d.datos.Version.Reglas=[{DestinoRef:'14',ValorConstante:'EEE'}];return value;};return r;};
  await assert.rejects(f.run(),/PARTIAL_SOURCE_CHANGED/);assert.deepEqual(f.calls.map(c=>c.op),['ObtenerPerfil']);
  const missing=fixture();missing.auth.page.locator=()=>({count:async()=>0});await assert.rejects(missing.run(),/PARTIAL_ADMIN_UNAVAILABLE/);assert.equal(missing.calls.length,0);
});
test('PARCIAL: fallo intermedio conserva referencia nueva sin retry ni filtrar respuesta',async()=>{
  for(const operation of ['CrearPerfil','GuardarBorrador','PublicarVersion','CambiarDisponibilidad']){
    const f=fixture(operation);await assert.rejects(f.run(),error=>error.message==='DOC111_PARTIAL_PROFILE_NOT_CONFIRMED');
    assert.equal(f.calls.filter(c=>c.op===operation).length,1);assert.equal(f.calls.at(-1).op,operation);assert.equal(f.references.length,operation==='CrearPerfil'?0:1);
  }
});
test('PARCIAL: selector excluye diagnóstico/recuperación y referencias no incluyen campos privados',()=>{
  const args=['--profile','x','--scenario','PARCIAL','--partial-profile','p'];assert.equal(runner.parseArgs(args)['partial-profile'],'p');
  for(const a of [[...args,'--diagnose'],args.map(x=>x==='PARCIAL'?'LOTE':x),[...args,'--verify-lot','2']])assert.throws(()=>runner.parseArgs(a),/DOC111_/);
  const ref='DOC111_PROFILE_REFERENCE='+JSON.stringify({mode:'PARCIAL',perfilId:'30',versionId:'40',secret:'PRIVATE'});
  assert.deepEqual(runner.safeProfileReference(ref,'PARCIAL'),{mode:'PARCIAL',perfilId:'30',versionId:'40'});assert.equal(runner.safeProfileReference(ref,'LOTE'),null);
  const f=fixture();assert.equal(validatePartialProposal(f.proposal,f.profile),f.proposal);
});
test('PARCIAL: token retirado del DOM se observa por navegación DOC110 y lectura oficial',async()=>{
  const f=fixture();let opens=0;
  f.auth.page.locator=()=>({count:async()=>1,getAttribute:async()=>null});
  const result=await preparePartialProfile(f.auth,f.profile,f.proposal,f.authorization,r=>f.references.push(r),{openAdministrator:async(page,access,method)=>{
    opens++;assert.equal(page,f.auth.page);assert.equal(access,'abrir');assert.equal(method,'ListarPerfiles');
    return {status:()=>200,request:()=>({headers:()=>({'x-importacion-perfil-csrf':'PRIVATE_TOKEN'})})};
  }});
  assert.equal(opens,1);assert.equal(result.versionId,'40');assert.equal(f.references.length,1);
});
test('PARCIAL: lectura administrativa denegada o sin token detiene sin crear ni reintentar',async()=>{
  for(const response of [{status:()=>403},{status:()=>200,request:()=>({headers:()=>({})})}]){
    const f=fixture();let opens=0;f.auth.page.locator=()=>({count:async()=>1,getAttribute:async()=>null});
    await assert.rejects(preparePartialProfile(f.auth,f.profile,f.proposal,f.authorization,()=>{}, {openAdministrator:async()=>{opens++;return response;}}),/PARTIAL_ADMIN_UNAVAILABLE/);
    assert.equal(opens,1);assert.equal(f.calls.length,0);
  }
});
test('PARCIAL: ASMX Proveedor null equivale a omisión Newtonsoft al leer, guardar y publicar',async()=>{
  const f=fixture(),post=f.auth.context.request.post;
  f.auth.context.request.post=async(...args)=>{
    const response=await post(...args),json=response.json;
    response.json=async()=>{const body=await json();const version=body.d.datos.Version;if(version?.Reglas)version.Reglas=version.Reglas.map(r=>({...r,Proveedor:null}));return body;};
    return response;
  };
  assert.equal((await f.run()).versionId,'40');assert.equal(f.calls.filter(c=>c.op==='CrearPerfil').length,1);
  assert.ok(!Object.hasOwn(f.proposal.reglas[0],'Proveedor'));
});
test('PARCIAL: proveedor configurado y diferencias null/ausente de otros campos se siguen rechazando',async()=>{
  for(const difference of [{Proveedor:{ProveedorId:'9',VersionId:'10'}},{PropiedadExpediente:null}]){
    const f=fixture(),post=f.auth.context.request.post;
    f.auth.context.request.post=async(...args)=>{
      const response=await post(...args),json=response.json;
      response.json=async()=>{const body=await json();body.d.datos.Version.Reglas=body.d.datos.Version.Reglas.map(r=>({...r,...difference}));return body;};return response;
    };
    await assert.rejects(f.run(),/PARTIAL_SOURCE_CHANGED/);assert.deepEqual(f.calls.map(c=>c.op),['ObtenerPerfil']);
  }
});
