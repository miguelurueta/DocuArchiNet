'use strict';
const fs=require('node:fs'),path=require('node:path');
const {BASE_URL,check,hash,id}=require('./doc119-seguimiento.cjs');
const FIXTURE_DIRECTORY='.tmp/doc119-next-preflight/eb09fc5981be453b936b2bf1714c43a3';
function readPrepared(root){
  const directory=fs.realpathSync(path.join(root,FIXTURE_DIRECTORY));
  const fixture=JSON.parse(fs.readFileSync(path.join(directory,'fixture-manifest.json'),'utf8'));
  check(fixture.status==='LOCAL_ONLY_NOT_AUTHORIZED_FOR_EFFECTS'&&fixture.baseUrl===BASE_URL&&fixture.ownerRef==='141'&&fixture.companyRef==='2'&&fixture.proposedProfileRef==='26','FIXTURE_SCOPE_INVALID');
  const request=fixture.request;
  check(request.nivelRef==='7'&&request.versionId==='40'&&request.raices.length===2&&request.archivos.length===27&&fixture.originals.length===27,'FIXTURE_SCOPE_INVALID');
  check(request.raices.every((r,i)=>r.idCliente==='r'+(i+1)&&r.seleccionId==='s'+(i+1)&&r.nombre==='DOC119-20261112-SEG-eb09fc59'),'FIXTURE_SCOPE_INVALID');
  const files=new Map();
  for(let i=0;i<27;i++){
    const file=request.archivos[i],original=fixture.originals[i];
    check(file.idCliente==='a'+(i+1)&&original.idCliente===file.idCliente&&file.raizIdCliente===(i===0?'r1':'r2')&&file.nombre==='DOC119-'+String(i+1).padStart(3,'0')+'.pdf'&&file.rutaRelativa===request.raices[0].nombre+'/'+file.nombre&&file.bytes==='677','FIXTURE_SCOPE_INVALID');
    const target=fs.realpathSync(path.resolve(directory,original.path));
    check(target.startsWith(path.join(directory,'fixture')+path.sep),'FIXTURE_SCOPE_INVALID');
    const bytes=fs.readFileSync(target),stat=fs.statSync(target);
    check(stat.size===677&&path.basename(target)===file.nombre&&String(Math.floor(stat.mtimeMs))===file.ultimaModificacionLocal&&hash(bytes)===original.sha256&&original.sha256==='a5f1924afbae4bd631de893c49a56d9f09219075113e6da2ec983602cff84c6c','FIXTURE_METADATA_CHANGED');
    files.set(file.idCliente,{path:target,bytes});
  }
  return {fixture,files};
}
function failureCategory(message){
  const known={
    'La forma de la solicitud no es válida.':'REQUEST_ENVELOPE',
    'El manifiesto esta incompleto o contiene rutas o correlaciones invalidas.':'MANIFEST',
    'La preparación o solicitud no es válida.':'PLAN_REPRESENTATION',
    'Solicitud de catálogo inválida.':'CATALOG',
    'El inventario y sus referencias son obligatorios.':'INVENTORY',
    'La configuración excede el límite permitido.':'JSON_SIZE'
  };
  return typeof message==='string'&&Object.hasOwn(known,message)?known[message]:'UNCLASSIFIED';
}
function setupTransport(request,budget,manifest,refs,record=()=>{},observe=()=>{}){
  const used=new Set();
  const once=key=>{check(!used.has(key),'SETUP_REPLAY_DENIED');used.add(key);};
  return {async post(url,options){
    const target=new URL(url),base=new URL(BASE_URL);
    check(target.origin===base.origin&&!target.username&&!target.password&&!target.search&&!target.hash,'SETUP_SCOPE_DENIED');
    const relative=target.pathname.slice(base.pathname.length);
    check(target.pathname.startsWith(base.pathname)&&options.maxRedirects===0&&options.maxRetries===0,'SETUP_SCOPE_DENIED');
    let operation;
    if(relative==='generic_control/ImportacionCarpetasUpload.ashx'){
      operation='RecibirArchivo';const data=options.multipart;
      check(id(refs.lotId)&&id(refs.receiptFileId)&&data?.loteId===refs.lotId&&data.archivoId===refs.receiptFileId&&Object.keys(data).sort().join(',')==='archivoId,claveIntento,file,loteId'&&/^[a-f0-9]{32}$/.test(data.claveIntento)&&data.file?.name===manifest.archivos[0].nombre&&data.file.mimeType==='application/pdf'&&Buffer.isBuffer(data.file.buffer)&&data.file.buffer.length===677&&hash(data.file.buffer)==='a5f1924afbae4bd631de893c49a56d9f09219075113e6da2ec983602cff84c6c','SETUP_SCOPE_DENIED');
      once(operation);
    }else{
      check(relative.startsWith('webservice/WebServiceImportacionCarpetas.asmx/'),'SETUP_SCOPE_DENIED');
      operation=relative.split('/').pop();const payload=options.data?.solicitud;
      check(options.data&&Object.keys(options.data).join(',')==='solicitud','SETUP_SCOPE_DENIED');
      if(operation==='AbrirImportacion'){check(hash(payload)===hash({nivelRef:'7'}),'SETUP_SCOPE_DENIED');once(operation);}
      else if(operation==='PrepararPlan'){check(hash(payload)===hash(manifest),'SETUP_SCOPE_DENIED');once(operation);}
      else if(operation==='ConfirmarPlan'){check(refs.confirmation&&hash(payload)===hash(refs.confirmation),'SETUP_SCOPE_DENIED');once(operation);}
      else if(operation==='AvanzarLote'){check(refs.creation&&payload?.fase==='CREAR_EXPEDIENTE'&&hash(payload)===hash(refs.creation),'SETUP_SCOPE_DENIED');once(operation);}
      else if(operation==='ConsultarLote')check(id(refs.lotId)&&hash(payload)===hash({loteId:refs.lotId}),'SETUP_SCOPE_DENIED');
      else if(operation==='ConsultarRecursosLote')check(id(refs.lotId)&&payload?.loteId===refs.lotId&&['RAICES','ARCHIVOS'].includes(payload.tipo)&&[1,2].includes(payload.pagina)&&payload.tamanoPagina===25&&Object.keys(payload).sort().join(',')==='loteId,pagina,tamanoPagina,tipo','SETUP_SCOPE_DENIED');
      else check(false,'SETUP_SCOPE_DENIED');
    }
    budget.take('http');record(operation);
    const response=await request.post(url,options);
    if(typeof response?.status==='function'&&response.status()>=400){
      let category='UNCLASSIFIED';try{const raw=await response.json();category=failureCategory((raw?.d??raw)?.mensaje);}catch{}
      observe({operation,httpStatus:response.status(),category});
    }
    return response;
  }};
}
module.exports={FIXTURE_DIRECTORY,readPrepared,setupTransport,failureCategory};
