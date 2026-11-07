'use strict';
const fs=require('node:fs'),path=require('node:path');
const {ROOT,sha}=require('../../scripts/support/doc117-alm03-guard-diagnostic.cjs');
const {createControls}=require('./doc117-alm03-controls.cjs');
const {assertCheckpoint}=require('./doc117-alm03-coexistence.cjs');
const check=(v,c)=>{if(!v)throw Error('DOC117_ALM03_PROBE_'+c);};
function compose(query,scope,historicalScope){
 const names=['DOC117-ALM03-A-20261106','DOC117-ALM03-B-20261106'],profile={nivelRef:'7',versionId:'38',mode:'LOTE',expectations:names.map(n=>({codigo:n,nombre:n,tipologiaId:'79',indices:[{referencia:'14',tipo:'TEXTO',valor:'EEE'}]}))},controls=createControls(query,historicalScope,profile),ref={intentoId:'58',documentoId:'16224',inventarioId:'23322'};
 async function baseline(){
  const view=await controls.snapshot('22');assertCheckpoint(view,historicalScope,0,ref,true);
  const b=view.files.filter(f=>String(f.archivo_id)==='37');check(view.lot.estado==='EN_CURSO'&&view.attempts.length===5&&view.attempts.filter(a=>a.fase==='ALMACENAR').length===1&&b.length===1&&b[0].estado_efecto==='NO_INICIADO'&&!b[0].intento_activo_id&&!b[0].resultado_doc,'HISTORICAL_CASE_CHANGED');
  const physical=await controls.physical(view,0,ref),originals=[await controls.original(0),await controls.original(1)],individual=await controls.individualBefore();
  const absent=await query('RA','SELECT ID_REGISTRO_PRODUCION_DOCUMENTAL FROM registro_producion_documental WHERE ID_EMPRESA_DOCUMENTO=? AND EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=? AND SEGUNDO_NOMBRE_DOCUMENTO=? LIMIT 2',['2','467',scope.fileName]);check(absent.length===0,'DIAGNOSTIC_DOCUMENT_EXISTS');
  const inventory=await controls.inventoryCount(),preserved=await controls.preservation({maxPlan:'38',maxLot:'22'});
  const previous=fs.readFileSync(path.join(ROOT,'Temp_Image/upload_file/141',historicalScope.individual.fileName));check(previous.length===677&&sha(previous)===scope.contentSha256,'HISTORICAL_TEMP_CHANGED');
  return{lotSha256:sha(JSON.stringify(view)),aPhysicalSha256:physical.fingerprint,originals,individualSha256:sha(JSON.stringify(individual)),inventory,preserved,historicalIndividualTempSha256:sha(previous)};
 }
 function temporary(){const file=path.join(ROOT,'Temp_Image/upload_file/141',scope.fileName);if(!fs.existsSync(file))return{present:false};const bytes=fs.readFileSync(file);check(bytes.length===677&&sha(bytes)===scope.contentSha256,'DIAGNOSTIC_TEMP_CHANGED');return{present:true,bytes:bytes.length,sha256:sha(bytes)};}
 return{baseline,temporary};
}
module.exports={compose};
