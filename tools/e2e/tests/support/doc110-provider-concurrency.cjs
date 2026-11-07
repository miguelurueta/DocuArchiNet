'use strict';
const {expect}=require('@playwright/test');
const {safePost}=require('./doc110-safe-request.cjs');
const {createWorkflowLoginPageSession}=require('./authenticated-workflow-session.cjs');
const refs=d=>({ProveedorId:d.ProveedorId,VersionId:d.VersionId,Revision:d.Revision,RevisionProveedor:d.RevisionProveedor});
const same=(a,b)=>expect(JSON.stringify(a)===JSON.stringify(b)).toBe(true);

async function runProviderRaces(peers,current,availability=false){
  async function race(method,payloads){
    const results=await Promise.all(peers.map((call,i)=>call(method,payloads[i])));
    expect(results.map(r=>r.status).sort()).toEqual([200,409]);
    const loser=results.find(r=>r.status===409).result;
    expect(loser.ok).toBe(false);expect(loser.codigo).toBe('CONFLICTO_REVISION');expect(loser.datos==null).toBe(true);
    const winnerIndex=results.findIndex(r=>r.status===200);
    expect(results[winnerIndex].result.ok).toBe(true);
    return {document:results[winnerIndex].result.datos,winnerIndex};
  }
  async function shared(document){
    for(const peer of peers){
      const read=await peer('ObtenerAdministracionProveedor',refs(document));
      expect(read.status).toBe(200);expect(read.result.ok).toBe(true);same(read.result.datos,document);
    }
    expect(document.Activo).toBe(false);
  }
  const contracts=['concurrent_a','concurrent_b'].map(name=>{
    const contract=JSON.parse(JSON.stringify(current.Contrato));contract.Propiedades[0].Nombre=name;return contract;
  });
  const saved=await race('GuardarProveedor',contracts.map(Contrato=>({...refs(current),Contrato})));
  expect(saved.document.Revision).toBe(current.Revision+1);
  expect(saved.document.RevisionProveedor).toBe(current.RevisionProveedor+1);
  expect(saved.document.VersionId).toBe(current.VersionId);
  same(saved.document.Contrato,contracts[saved.winnerIndex]);await shared(saved.document);
  console.log('DOC110_PASS: PROVIDER_CONCURRENT_SAVE_ONE_WINNER_SHARED_RESULT');
  current=saved.document;
  const published=(await race('PublicarProveedor',[refs(current),refs(current)])).document;
  expect(published.Estado).toBe('PUBLICADA');expect(published.Revision).toBe(current.Revision+1);
  expect(published.RevisionProveedor).toBe(current.RevisionProveedor+1);
  same(published.Contrato,current.Contrato);await shared(published);
  console.log('DOC110_PASS: PROVIDER_CONCURRENT_PUBLICATION_SINGLE_INCREMENT');
  const cloned=(await race('CrearVersionProveedor',[refs(published),refs(published)])).document;
  expect(cloned.Estado).toBe('BORRADOR');expect(cloned.Revision).toBe(1);
  expect(cloned.Numero).toBe(published.Numero+1);expect(cloned.VersionId!==published.VersionId).toBe(true);
  expect(cloned.RevisionProveedor).toBe(published.RevisionProveedor+1);
  same(cloned.Contrato,published.Contrato);await shared(cloned);
  const history=await peers[0]('ListarAdministracionProveedores',{Pagina:1,ProveedorId:cloned.ProveedorId});
  expect(history.status).toBe(200);expect(history.result.ok).toBe(true);
  expect(history.result.datos.length).toBe(2);
  expect(new Set(history.result.datos.map(d=>d.Numero)).size).toBe(2);
  const old=await peers[0]('ObtenerAdministracionProveedor',refs(published));
  expect(old.status).toBe(200);expect(old.result.ok).toBe(true);
  same(old.result.datos,{...published,RevisionProveedor:cloned.RevisionProveedor});
  expect(history.result.datos.every(d=>!d.Activo)).toBe(true);
  console.log('DOC110_PASS: PROVIDER_CONCURRENT_VERSION_NO_DUPLICATE_FIRST_IMMUTABLE_INACTIVE');
  if(availability)await runProviderAvailability(peers,cloned,history.result.datos);
}

async function runProviderAvailability(peers,current,beforeHistory){
  async function read(peer){
    const response=await peer('ObtenerAdministracionProveedor',refs(current));
    expect(response.status).toBe(200);expect(response.result.ok).toBe(true);return response.result.datos;
  }
  try{
    // Esperar ambas solicitudes incluso ante fallo de transporte antes de reconciliar disponibilidad.
    const settled=await Promise.allSettled(peers.map(peer=>peer('CambiarDisponibilidadProveedor',{...refs(current),Activo:true})));
    if(settled.some(r=>r.status==='rejected'))throw new Error('DOC110_PROVIDER_AVAILABILITY_TRANSPORT_UNCERTAIN');
    const results=settled.map(r=>r.value);
    expect(results.map(r=>r.status).sort()).toEqual([200,409]);
    const rejected=results.find(r=>r.status===409).result;
    expect(rejected.ok).toBe(false);expect(rejected.codigo).toBe('CONFLICTO_REVISION');expect(rejected.datos==null).toBe(true);
    const winner=results.find(r=>r.status===200).result;
    expect(winner.ok).toBe(true);
    same(winner.datos,{...current,Activo:true,RevisionProveedor:current.RevisionProveedor+1});
    for(const peer of peers)same(await read(peer),winner.datos);
    console.log('DOC110_PASS: PROVIDER_CONCURRENT_ACTIVATION_ONE_WINNER_SHARED_RESULT');
  }finally{
    const latest=await read(peers[0]);
    if(latest.Activo){
      const disabled=await peers[0]('CambiarDisponibilidadProveedor',{...refs(latest),Activo:false});
      expect(disabled.status).toBe(200);expect(disabled.result.ok).toBe(true);
      same(disabled.result.datos,{...latest,Activo:false,RevisionProveedor:latest.RevisionProveedor+1});
    }
    for(const peer of peers)expect((await read(peer)).Activo).toBe(false);
    console.log('DOC110_PASS: PROVIDER_AVAILABILITY_FINALLY_DISABLED_BOTH_SESSIONS');
  }
  const final=await read(peers[0]);
  expect(final.RevisionProveedor).toBe(current.RevisionProveedor+2);
  for(const peer of peers){
    const history=await peer('ListarAdministracionProveedores',{Pagina:1,ProveedorId:current.ProveedorId});
    expect(history.status).toBe(200);expect(history.result.ok).toBe(true);
    const ordered=items=>items.slice().sort((a,b)=>a.Numero-b.Numero);
    same(ordered(history.result.datos),ordered(beforeHistory.map(d=>({...d,Activo:false,RevisionProveedor:final.RevisionProveedor}))));
  }
  console.log('DOC110_PASS: PROVIDER_AVAILABILITY_HISTORY_AND_VERSION_REFERENCES_UNCHANGED');
}

async function exerciseProviderConcurrency(auth,baseUrl,token,current){
  const contender=await createWorkflowLoginPageSession(auth.context.browser(),{
    baseUrl,moduleEnvironmentVariable:'DOC110_E2E_MODULE',userEnvironmentVariable:'DOC110_E2E_USER',
    passwordEnvironmentVariable:'DOC110_E2E_PASSWORD',ignoreHTTPSErrors:process.env.DOC10_E2E_IGNORE_HTTPS_ERRORS==='true'
  });
  try{
    await contender.page.goto(new URL('Gestion/WebFormProducionDocumental.aspx',baseUrl).href);
    await contender.page.locator('#doc110-navigation-toggle').click();
    const pending=contender.page.waitForResponse(r=>r.url().endsWith('/ListarPerfiles') && r.request().method()==='POST');
    await contender.page.locator('[data-doc110=abrir]').click();
    const response=await pending;expect(response.status()).toBe(200);
    expect((await response.json()).d?.ok).toBe(true);
    const otherToken=response.request().headers()['x-importacion-perfil-csrf'];
    if(!otherToken)throw new Error('DOC110_CSRF_NOT_OBSERVED');
    const session=async c=>(await c.cookies(baseUrl)).find(x=>x.name==='ASP.NET_SessionId')?.value;
    const first=await session(auth.context),second=await session(contender.context);
    expect(Boolean(first && second && first!==second)).toBe(true);
    console.log('DOC110_PASS: PROVIDER_INDEPENDENT_ASPNET_SESSIONS');
    const peers=[[auth.context,token],[contender.context,otherToken]].map(([context,csrf])=>async(method,payload)=>{
      const res=await safePost(context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/'+method,baseUrl).href,{
        headers:{'X-Importacion-Perfil-CSRF':csrf},data:{solicitud:payload}
      },method);
      let result;try{result=(await res.json()).d;}catch{throw new Error('DOC110_PROVIDER_CONCURRENCY_NON_JSON');}
      return {status:res.status(),result};
    });
    await runProviderRaces(peers,current,process.env.DOC110_E2E_PROVIDER_AVAILABILITY==='YES');
  }finally{await contender.context.close();}
}
module.exports={exerciseProviderConcurrency,runProviderRaces,runProviderAvailability};
