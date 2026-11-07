'use strict';
const crypto=require('node:crypto');
const {validateProfile}=require('./doc111-importacion-carpetas.cjs');
const {safePost}=require('./doc110-safe-request.cjs');
const SCENARIOS=Object.freeze(['PLAN-01','PLAN-02','PLAN-03']);
const check=(ok,code)=>{if(!ok)throw Error('DOC114_'+code);};
const id=v=>typeof v==='string'&&/^[1-9][0-9]{0,9}$/.test(v)&&BigInt(v)<=4294967295n;
const object=v=>v!==null&&typeof v==='object'&&!Array.isArray(v);
function safePlanObservation(value){
  if(!object(value)||value.operation!=='PrepararPlan'||!Number.isInteger(value.httpStatus)||value.httpStatus<100||value.httpStatus>599||typeof value.domainEnvelope!=='boolean'||typeof value.hasData!=='boolean')return null;
  const codes=['OK','SOLICITUD_INVALIDA','ACCESO_DENEGADO','CONFIGURACION_INVALIDA','CONFLICTO_REVISION','LIMITE_EXCEDIDO','CAPACIDAD_NO_DISPONIBLE','SESION_REQUERIDA','SOLICITUD_NO_VERIFICADA','ERROR_INTERNO'];
  return {operation:'PrepararPlan',httpStatus:value.httpStatus,code:codes.includes(value.code)?value.code:null,domainEnvelope:value.domainEnvelope,hasData:value.hasData};
}
const stable=v=>JSON.stringify(canonical(v));
function canonical(v){
  if(Array.isArray(v))return v.map(canonical);
  if(!object(v))return v;
  return Object.fromEntries(Object.keys(v).sort().map(k=>[k,canonical(v[k])]));
}
function comparePreparationDocuments(storedSnapshot,responseSnapshot,storedResult,responseResult){
  const snapshot=value=>{
    const copy=structuredClone(value);
    if(Array.isArray(copy?.Reglas))for(const rule of copy.Reglas)if(rule?.Proveedor===null)delete rule.Proveedor;
    return copy;
  };
  const result=value=>{
    const copy=structuredClone(value);
    if(Array.isArray(copy?.Elementos))for(const element of copy.Elementos)if(Array.isArray(element?.Valores))for(const item of element.Valores)if(item?.Procedencia===null)delete item.Procedencia;
    return copy;
  };
  // Sólo estos dos atributos tienen NullValueHandling.Ignore en sus modelos.
  // No eliminar IDs ni otros campos nulos/ausentes, ni procedencias configuradas.
  return {snapshotEqual:stable(snapshot(storedSnapshot))===stable(snapshot(responseSnapshot)),resultEqual:stable(result(storedResult))===stable(result(responseResult))};
}
function comparePublishedSnapshot(stored,published){
  const normalized=value=>{
    if(!object(value)||!Array.isArray(value.Reglas)||value.Reglas.some(r=>!object(r)||typeof r.DestinoTipo!=='string'||typeof r.DestinoRef!=='string'))return null;
    const copy=structuredClone(value),keys=copy.Reglas.map(r=>r.DestinoTipo+':'+r.DestinoRef);
    if(new Set(keys).size!==keys.length)return null;
    const compare=(a,b)=>a<b?-1:a>b?1:0;
    // La publicación se lee por Orden; PrepararPersistencia fija orden ordinal
    // DestinoTipo/DestinoRef. Se conserva el campo Orden y cada identidad/valor.
    copy.Reglas.sort((a,b)=>compare(a.DestinoTipo,b.DestinoTipo)||compare(a.DestinoRef,b.DestinoRef));
    return copy;
  };
  const a=normalized(stored),b=normalized(published);
  return a!==null&&b!==null&&comparePreparationDocuments(a,b,null,null).snapshotEqual;
}
function authorize(profile,scope){
  validateProfile(profile);
  check(object(scope)&&[...SCENARIOS,'RECIB-01','RECIB-02','RECIB-03'].includes(scope.scenario),'SCENARIO_INVALID');
  check(['environment','account','select','effects','discardable'].every(k=>scope[k]===true),'AUTHORIZATION_REQUIRED');
  check(!profile.ignoreHttpsErrors||scope.tls===true,'TLS_AUTHORIZATION_REQUIRED');
  check(Number.isInteger(scope.maximumPreparations)&&scope.maximumPreparations>=1&&scope.maximumPreparations<=64,'PREPARATION_BUDGET_REQUIRED');
  if(scope.foreignLevel!==undefined)check(id(scope.foreignLevel)&&scope.foreignLevel!==profile.nivelRef,'FOREIGN_LEVEL_INVALID');
  if(scope.invalidVersion!==undefined)check(id(scope.invalidVersion)&&scope.invalidVersion!==profile.versionId,'INVALID_VERSION_INVALID');
}
function createPreparationClient(auth,profile,scope,onReference=()=>{},options={}){
  check(object(options)&&Object.keys(options).every(k=>k==='readOnly')&&(options.readOnly===undefined||typeof options.readOnly==='boolean'),'CLIENT_OPTIONS_INVALID');
  if(options.readOnly){validateProfile(profile);check(scope?.environment===true&&scope.account===true&&scope.effects!==true&&scope.discardable!==true&&(!profile.ignoreHttpsErrors||scope.tls===true),'READ_AUTHORIZATION_REQUIRED');}
  else authorize(profile,scope);
  let token,attempts=0,uncertain=false;
  async function refreshToken(){token=await auth.page.locator('#doc111-importacion').getAttribute('data-token');check(typeof token==='string'&&token.length>0,'TOKEN_UNAVAILABLE');}
  async function request(operation,payload,expectedCodes=[]){
    check(!uncertain,'UNCERTAIN_OPERATION_REQUIRES_REVIEW');
    check(['AbrirImportacion','ListarPerfilesImportacion','PrepararPlan'].includes(operation),'OPERATION_NOT_ALLOWED');
    if(options.readOnly)check(operation!=='PrepararPlan','READONLY_OPERATION_REQUIRED');
    check(object(payload)&&Array.isArray(expectedCodes)&&expectedCodes.every(c=>['SOLICITUD_INVALIDA','ACCESO_DENEGADO','CONFIGURACION_INVALIDA','CONFLICTO_REVISION','LIMITE_EXCEDIDO','CAPACIDAD_NO_DISPONIBLE'].includes(c)),'REQUEST_INVALID');
    check(payload.nivelRef===profile.nivelRef||expectedCodes.length>0&&payload.nivelRef===scope.foreignLevel,'LEVEL_NOT_AUTHORIZED');
    if(operation==='PrepararPlan'){
      check(attempts<scope.maximumPreparations,'PREPARATION_BUDGET_EXHAUSTED');
      check(payload.versionId===profile.versionId||expectedCodes.length>0&&payload.versionId===scope.invalidVersion,'VERSION_NOT_AUTHORIZED');
      attempts++;
    }
    check(typeof token==='string'&&token.length>0,'TOKEN_UNAVAILABLE');
    let response,body;
    try{
      response=await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/'+operation,profile.baseUrl).href,{maxRedirects:0,headers:{'X-Importacion-Carpetas-CSRF':token,Origin:new URL(profile.baseUrl).origin},data:{solicitud:payload}},operation);
      const raw=await response.json();body=raw?.d??raw;
    }catch{if(operation==='PrepararPlan')uncertain=true;throw Error('DOC114_RESULT_NOT_CONFIRMED');}
    // Conservar referencia antes de comprobar los demás datos de la respuesta.
    if(operation==='PrepararPlan'&&id(body?.datos?.planId))onReference({planId:body.datos.planId});
    if(expectedCodes.length){
      const rejected=[400,401,403,409,413,503].includes(response.status())&&body?.ok===false&&body.datos==null&&expectedCodes.includes(body.codigo);
      if(!rejected&&operation==='PrepararPlan')uncertain=true;
      if(!rejected){const error=Error('DOC114_EXPECTED_REJECTION_NOT_VERIFIED');error.observation=safePlanObservation({operation,httpStatus:response.status(),code:body?.codigo,domainEnvelope:typeof body?.ok==='boolean'&&typeof body?.codigo==='string',hasData:body?.datos!=null});throw error;}
      return {status:response.status(),code:body.codigo,rejected:true};
    }
    if(response.status()!==200||body?.ok!==true||!object(body.datos)){if(operation==='PrepararPlan')uncertain=true;throw Error('DOC114_RESULT_NOT_CONFIRMED');}
    return body.datos;
  }
  return {refreshToken,request,get attempts(){return attempts;},get uncertain(){return uncertain;}};
}
function assertPreparation(data,request,expected){
  check(object(data)&&id(data.planId)&&Number.isInteger(data.revision)&&data.revision>0&&/^[a-f0-9]{64}$/.test(data.planHash)&&data.nivelRef===request.nivelRef,'PLAN_INVALID');
  check(typeof data.expiraUtc==='string'&&Number.isFinite(Date.parse(data.expiraUtc)),'PLAN_EXPIRY_INVALID');
  check(object(data.snapshot)&&String(data.snapshot.VersionId)===request.versionId&&object(data.resultado),'PLAN_CONTEXT_INVALID');
  const result=data.resultado;
  check(typeof result.PuedeConfirmar==='boolean'&&Array.isArray(result.UnidadesElegibles)&&Array.isArray(result.Incidencias)&&Array.isArray(result.Elementos),'RESULT_INVALID');
  check(result.Incidencias.every(object),'INCIDENCE_INVALID');
  const roots=new Set(request.raices.map(r=>r.idCliente)),files=new Set(request.archivos.map(a=>a.raizIdCliente+':'+a.idCliente));
  check(result.UnidadesElegibles.every(r=>roots.has(r))&&new Set(result.UnidadesElegibles).size===result.UnidadesElegibles.length,'ELIGIBILITY_INVALID');
  check(result.Elementos.length===roots.size+files.size&&result.Elementos.every(e=>object(e)&&Array.isArray(e.Valores)&&e.Valores.every(object)&&(e.Ambito==='RAIZ'?roots.has(e.RaizIdCliente):e.Ambito==='ARCHIVO'&&files.has(e.RaizIdCliente+':'+e.ArchivoIdCliente))),'INVENTORY_RESULT_INVALID');
  check(new Set(result.Elementos.map(e=>e.Ambito+':'+e.RaizIdCliente+':'+(e.ArchivoIdCliente||''))).size===result.Elementos.length,'DUPLICATE_RESULT');
  if(expected.confirmable!==undefined)check(result.PuedeConfirmar===expected.confirmable,'POLICY_RESULT_INVALID');
  if(expected.eligible)check(stable([...result.UnidadesElegibles].sort())===stable([...expected.eligible].sort()),'POLICY_RESULT_INVALID');
  for(const issue of expected.incidences||[])check(result.Incidencias.some(i=>object(i)&&i.Codigo===issue.code&&(issue.root===undefined||i.RaizIdCliente===issue.root)&&(issue.file===undefined||i.ArchivoIdCliente===issue.file)),'INCIDENCE_NOT_FOUND');
  for(const value of expected.values||[]){
    const element=result.Elementos.find(e=>e.RaizIdCliente===value.root&&(value.file===undefined?e.Ambito==='RAIZ':e.ArchivoIdCliente===value.file));
    const found=element?.Valores.find(v=>v.DestinoTipo===value.type&&v.DestinoRef===value.ref);
    check(object(found)&&found.Valor===value.value&&found.Diferido===value.deferred,'VALUE_NOT_VERIFIED');
    if(value.deferred)check(found.Valor===null&&typeof found.PropiedadPendiente==='string'&&found.PropiedadPendiente.length>0,'DEFERRED_SOURCE_INVALID');
  }
  return crypto.createHash('sha256').update(stable({snapshot:data.snapshot,resultado:data.resultado})).digest('hex');
}
function createPlanControls(query,profile,identity){
  validateProfile(profile);check(object(identity)&&id(identity.userRef)&&id(identity.ambitoRef),'CONTROL_IDENTITY_REQUIRED');
  const parameters=[identity.userRef,identity.ambitoRef,profile.nivelRef,profile.versionId];
  const one=rows=>{check(Array.isArray(rows)&&rows.length===1,'CONTROL_ROW_NOT_UNIQUE');return rows[0];};
  async function read(label,origin,sql,args){
    try{return await query(origin,sql,args);}catch(error){
      const suffix=/^DOC111_SELECT_([A-Z_]{3,35})$/.exec(error?.message)?.[1];
      const allowed=['AUTHENTICATION_FAILED','DSN_UNAVAILABLE','DRIVER_UNAVAILABLE','DATABASE_ACCESS_DENIED','DATABASE_UNAVAILABLE','CONNECTION_UNAVAILABLE','OPEN_FAILED','QUERY_FAILED','COLUMN_UNAVAILABLE','TABLE_UNAVAILABLE','QUERY_UNSUPPORTED','RESULT_FAILED','INPUT_FAILED','FAILED'];
      throw Error('DOC114_CONTROL_'+label+'_'+(allowed.includes(suffix)?suffix:'FAILED'));
    }
  }
  async function count(label,origin,sql,args){const r=one(await read(label,origin,sql,args));check(/^\d+$/.test(String(r.total)),'CONTROL_COUNT_INVALID');return BigInt(r.total).toString();}
  async function snapshot(){
    const version=one(await read('VERSION','RA','SELECT gabinete_id FROM ra_integracion_perfil_version WHERE version_id=?',[profile.versionId]));
    check(id(String(version.gabinete_id)),'CONTROL_CABINET_INVALID');
    const cabinet=one(await read('CABINET','DA','SELECT NOMBRE FROM system1 WHERE id_gabinete=?',[String(version.gabinete_id)]));
    check(typeof cabinet.NOMBRE==='string'&&/^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(cabinet.NOMBRE),'CONTROL_IDENTIFIER_INVALID');
    return {
      plans:await count('PLANS','RA','SELECT COUNT(*) AS total FROM ra_integracion_plan WHERE usuario_ref=? AND ambito_ref=? AND nivel_ref=? AND version_id=?',parameters),
      lots:await count('LOTS','RA','SELECT COUNT(*) AS total FROM ra_integracion_lote WHERE solicitante_ref=? AND ambito_ref=? AND nivel_ref=? AND version_id=?',parameters),
      roots:await count('ROOTS','RA','SELECT COUNT(*) AS total FROM expediente_archivo e INNER JOIN ra_pro_niveles_has_expediente_archivo r ON r.expediente_archivo_ID_EXPEDIENTE=e.ID_EXPEDIENTE WHERE e.ID_USUARIO_GESTION=? AND e.ID_EMPRESA_EXPEDIENTE=? AND r.ra_pro_niveles_id_nivel=?',parameters.slice(0,3)),
      documents:await count('DOCUMENTS','DA','SELECT COUNT(*) AS total FROM `'+cabinet.NOMBRE+'` g INNER JOIN registro_producion_documental r ON r.ID_REGISTRO_PRODUCION_DOCUMENTAL=g.ID_INVENTARIO_DOCUMENTAL AND r.ID_DOCUMENTO_DOCUARCHI_ALMACEN=g.ID AND r.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=g.ID_EXPEDIENTE WHERE g.ID_USUARIO_GESTION=? AND r.ID_EMPRESA_DOCUMENTO=? AND r.NOMBRE_GABINETE=?',[...parameters.slice(0,2),cabinet.NOMBRE])
    };
  }
  async function verifyPlan(data,request){
    const row=one(await read('STORED_PLAN','RA',"SELECT estado,revision,HEX(plan_hash) AS huella,contexto_doc,config_doc,manifiesto_doc,resultado_doc,DATE_FORMAT(creado_utc,'%Y-%m-%dT%H:%i:%sZ') AS creado_utc,DATE_FORMAT(expira_utc,'%Y-%m-%dT%H:%i:%sZ') AS expira_utc FROM ra_integracion_plan WHERE plan_id=? AND usuario_ref=? AND ambito_ref=? AND nivel_ref=? AND version_id=?",[data.planId,...parameters]));
    check(['REVISABLE','BLOQUEADO'].includes(row.estado)&&Number(row.revision)===data.revision&&String(row.huella).toLowerCase()===data.planHash,'STORED_PLAN_INVALID');
    let context,snapshot,manifest,result;try{context=JSON.parse(row.contexto_doc);snapshot=JSON.parse(row.config_doc);manifest=JSON.parse(row.manifiesto_doc);result=JSON.parse(row.resultado_doc);}catch{throw Error('DOC114_STORED_JSON_INVALID');}
    check(context.UsuarioRef===identity.userRef&&context.AmbitoRef===identity.ambitoRef&&context.NivelRef===profile.nivelRef,'STORED_CONTEXT_INVALID');
    const comparison=comparePreparationDocuments(snapshot,data.snapshot,result,data.resultado);
    check(comparison.snapshotEqual&&comparison.resultEqual,'STORED_RESULT_CHANGED');
    check(row.expira_utc===data.expiraUtc&&Number.isFinite(Date.parse(row.creado_utc))&&Date.parse(row.expira_utc)>Date.parse(row.creado_utc),'STORED_EXPIRY_INVALID');
    const compare=(a,b)=>a<b?-1:a>b?1:0;
    const expected={Raices:request.raices.map(r=>({SeleccionId:r.seleccionId,IdCliente:r.idCliente,Nombre:r.nombre})).sort((a,b)=>compare(a.IdCliente,b.IdCliente)),
      Archivos:request.archivos.map(a=>({IdCliente:a.idCliente,RaizIdCliente:a.raizIdCliente,Nombre:a.nombre,RutaRelativa:a.rutaRelativa,Bytes:a.bytes,UltimaModificacionLocal:a.ultimaModificacionLocal})).sort((a,b)=>compare(a.RaizIdCliente,b.RaizIdCliente)||compare(a.IdCliente,b.IdCliente))};
    check(stable(manifest)===stable(expected),'STORED_MANIFEST_CHANGED');
    return {verified:true};
  }
  async function readExisting(planId,expectedHash){
    check(id(planId)&&/^[a-f0-9]{64}$/.test(expectedHash),'RESUME_REFERENCE_INVALID');
    const row=one(await read('RESUME_PLAN','RA',"SELECT revision,HEX(plan_hash) AS huella,config_doc,resultado_doc,DATE_FORMAT(expira_utc,'%Y-%m-%dT%H:%i:%sZ') AS expira_utc FROM ra_integracion_plan WHERE plan_id=? AND usuario_ref=? AND ambito_ref=? AND nivel_ref=? AND version_id=?",[planId,...parameters]));
    check(String(row.huella).toLowerCase()===expectedHash,'RESUME_PLAN_HASH_CHANGED');
    let snapshot,result;try{snapshot=JSON.parse(row.config_doc);result=JSON.parse(row.resultado_doc);}catch{throw Error('DOC114_STORED_JSON_INVALID');}
    return {planId,revision:Number(row.revision),planHash:expectedHash,nivelRef:profile.nivelRef,expiraUtc:row.expira_utc,snapshot,resultado:result};
  }
  function unchanged(before,after,preparations){
    check(Number.isInteger(preparations)&&preparations>=0&&BigInt(after.plans)-BigInt(before.plans)===BigInt(preparations),'PLAN_COUNT_CHANGED');
    check(['lots','roots','documents'].every(k=>after[k]===before[k]),'DOCUMENT_EFFECT_FOUND');return true;
  }
  return {snapshot,verifyPlan,unchanged,readExisting};
}
function validateResumePlan(session,resume){
  if(resume===undefined)return null;
  check(object(resume)&&Object.keys(resume).length===3&&((session.scenario==='PLAN-01'&&resume.caseKey==='valid-format-a')||(session.scenario==='PLAN-03'&&resume.caseKey==='baseline'))&&id(resume.planId)&&/^[a-f0-9]{64}$/.test(resume.planHash)&&session.cases[0].key===resume.caseKey&&!session.cases[0].rejectCodes,'RESUME_SCOPE_INVALID');
  return resume;
}
function validateSession(session){
  const closed=(v,keys)=>check(object(v)&&Object.keys(v).every(k=>keys.includes(k)),'SESSION_CONTRACT_INVALID');
  closed(session,['version','scenario','identity','profiles','cases','foreignLevel','invalidVersion','runtimeRoot']);
  check(Buffer.byteLength(JSON.stringify(session),'utf8')<=28000,'SESSION_TRANSPORT_BUDGET_EXCEEDED');
  check(session.version===1&&SCENARIOS.includes(session.scenario),'SESSION_INVALID');
  check(typeof session.runtimeRoot==='string'&&require('node:path').isAbsolute(session.runtimeRoot),'RUNTIME_ROOT_REQUIRED');
  closed(session.identity,['userRef','ambitoRef']);check(id(session.identity.userRef)&&id(session.identity.ambitoRef),'CONTROL_IDENTITY_REQUIRED');
  check(Array.isArray(session.profiles)&&session.profiles.length>=1&&session.profiles.length<=8,'PROFILES_REQUIRED');
  session.profiles.forEach(validateProfile);
  const first=session.profiles[0];
  check(session.profiles.every(p=>['baseUrl','module','odbcDsn','docuarchiOdbcDsn','ignoreHttpsErrors'].every(k=>p[k]===first[k])),'ENVIRONMENT_CHANGED');
  if(session.foreignLevel!==undefined)check(id(session.foreignLevel)&&session.profiles.every(p=>p.nivelRef!==session.foreignLevel),'FOREIGN_LEVEL_INVALID');
  if(session.invalidVersion!==undefined)check(id(session.invalidVersion)&&session.profiles.every(p=>p.versionId!==session.invalidVersion),'INVALID_VERSION_INVALID');
  check(Array.isArray(session.cases)&&session.cases.length>=1&&session.cases.length<=64,'CASES_REQUIRED');
  check(session.cases.every(object),'CASE_INVALID');
  check(new Set(session.cases.map(c=>c.key)).size===session.cases.length,'DUPLICATE_CASE');
  for(const c of session.cases){
    closed(c,['key','profile','request','expected','rejectCodes']);
    check(typeof c.key==='string'&&/^[a-z][a-z0-9-]{1,63}$/.test(c.key)&&Number.isInteger(c.profile)&&c.profile>=0&&c.profile<session.profiles.length,'CASE_INVALID');
    const p=session.profiles[c.profile],reject=Array.isArray(c.rejectCodes)&&c.rejectCodes.length>0;
    check(c.rejectCodes===undefined||reject&&c.rejectCodes.every(k=>['SOLICITUD_INVALIDA','ACCESO_DENEGADO','CONFIGURACION_INVALIDA','CONFLICTO_REVISION','LIMITE_EXCEDIDO','CAPACIDAD_NO_DISPONIBLE'].includes(k)),'REJECTION_CODES_INVALID');
    closed(c.request,['nivelRef','versionId','raices','archivos',...(reject?['resultado','snapshot']:[])]);
    check(c.request.nivelRef===p.nivelRef||reject&&c.request.nivelRef===session.foreignLevel,'LEVEL_NOT_AUTHORIZED');
    check(c.request.versionId===p.versionId||reject&&c.request.versionId===session.invalidVersion,'VERSION_NOT_AUTHORIZED');
    check(Array.isArray(c.request.raices)&&Array.isArray(c.request.archivos)&&Buffer.byteLength(JSON.stringify(c.request),'utf8')<=1024*1024,'CASE_MANIFEST_INVALID');
    if(!reject){
      closed(c.expected,['confirmable','eligible','incidences','values']);
      check(typeof c.expected.confirmable==='boolean'&&Array.isArray(c.expected.eligible)&&Array.isArray(c.expected.incidences)&&Array.isArray(c.expected.values),'EXPECTED_RESULT_REQUIRED');
      for(const i of c.expected.incidences){closed(i,['code','root','file']);check(typeof i.code==='string'&&/^[A-Z][A-Z0-9_]{1,79}$/.test(i.code),'EXPECTED_INCIDENCE_INVALID');}
      for(const v of c.expected.values){closed(v,['root','file','type','ref','value','deferred']);check(typeof v.root==='string'&&typeof v.type==='string'&&typeof v.ref==='string'&&(v.value===null||typeof v.value==='string')&&typeof v.deferred==='boolean','EXPECTED_VALUE_INVALID');}
    }else check(c.expected===undefined,'REJECTED_CASE_HAS_RESULT');
  }
  const keys=new Set(session.cases.map(c=>c.key)),find=k=>session.cases.find(c=>c.key===k);
  const fixtures=require('../../fixtures/doc114-plan-cases.json');
  const required=session.scenario==='PLAN-01'?fixtures.plan01.map(c=>c.key):session.scenario==='PLAN-02'?fixtures.plan02.map(c=>c.key):fixtures.plan03.map(c=>c.key);
  check(required.every(k=>keys.has(k)),'SCENARIO_COVERAGE_INCOMPLETE');
  if(session.scenario==='PLAN-01'){
    const a=find('valid-format-a'),b=find('valid-format-b');
    check(session.profiles[a.profile].versionId!==session.profiles[b.profile].versionId,'TWO_PUBLISHED_FORMATS_REQUIRED');
    for(const [index,c]of [a,b].entries()){
      const format=fixtures.formats[index];
      check(c.request.raices.some(r=>r?.nombre===format.root)&&!c.rejectCodes&&c.expected.values.some(v=>v.deferred&&v.value===null)&&c.expected.values.some(v=>!v.deferred&&v.value===format.expectedCode)&&c.expected.values.some(v=>!v.deferred&&v.value===format.expectedDate),'FORMAT_VALUE_ASSERTIONS_REQUIRED');
    }
    for(const c of fixtures.plan01.filter(c=>c.incidence))check(find(c.key)?.expected?.incidences.some(i=>i.code===c.incidence),'ERROR_ASSERTION_REQUIRED');
  }
  if(session.scenario==='PLAN-02'){
    for(const [key,mode,confirmable]of [['complete-expediente','EXPEDIENTE',true],['complete-lote','LOTE',false],['partial','PARCIAL',true]]){
      const c=find(key);check(session.profiles[c.profile].mode===mode&&c.expected?.confirmable===confirmable&&c.request.raices.length===2,'POLICY_MATRIX_INVALID');
      check(stable([...c.expected.eligible].sort())===stable(mode==='LOTE'?[]:['r1'])&&c.expected.incidences.some(i=>i.root==='r2'),'POLICY_ASSERTIONS_REQUIRED');
    }
    const mixed=find('mixed-partial-root');check(session.profiles[mixed.profile].mode==='PARCIAL'&&mixed.expected?.confirmable===true&&stable([...mixed.expected.eligible].sort())===stable(['r1','r2'])&&mixed.request.archivos.filter(a=>a.raizIdCliente==='r2').length>=2&&mixed.expected.incidences.some(i=>i.root==='r2'&&i.file),'PARTIAL_MIX_ASSERTIONS_REQUIRED');
    const global=find('global-error');check(global.rejectCodes?.length>0||global.expected?.confirmable===false&&global.expected.eligible.length===0,'GLOBAL_ASSERTION_REQUIRED');
    check(find('foreign-level').request.nivelRef===session.foreignLevel&&stable(find('foreign-level').rejectCodes)===stable(['ACCESO_DENEGADO'])&&stable(find('inconsistent-manifest').rejectCodes)===stable(['SOLICITUD_INVALIDA']),'GLOBAL_NEGATIVES_REQUIRED');
  }
  if(session.scenario==='PLAN-03'){
    check(first.baseUrl==='https://localhost/GestionDocumental-Docuarchi.net/','REVOCATION_ENVIRONMENT_NOT_SUPPORTED');
    check(typeof session.runtimeRoot==='string'&&require('node:path').isAbsolute(session.runtimeRoot),'REVOCATION_RUNTIME_ROOT_REQUIRED');
    for(const key of ['injected-result','injected-snapshot','foreign-level','incompatible-version','revoked-permission'])check(find(key)?.rejectCodes?.length>0,'NEGATIVE_ASSERTION_REQUIRED');
    check(Object.prototype.hasOwnProperty.call(find('injected-result').request,'resultado')&&Object.prototype.hasOwnProperty.call(find('injected-snapshot').request,'snapshot'),'INJECTION_CASE_REQUIRED');
    check(['injected-result','injected-snapshot'].every(k=>stable(find(k).rejectCodes)===stable(['CONFIGURACION_INVALIDA']))&&stable(find('foreign-level').rejectCodes)===stable(['ACCESO_DENEGADO'])&&find('incompatible-version').rejectCodes.every(k=>['ACCESO_DENEGADO','CONFIGURACION_INVALIDA','SOLICITUD_INVALIDA','CONFLICTO_REVISION'].includes(k)),'NEGATIVE_CODES_NOT_SPECIFIC');
    check(find('foreign-level').request.nivelRef===session.foreignLevel&&find('incompatible-version').request.versionId===session.invalidVersion,'NEGATIVE_REFERENCE_REQUIRED');
    const compatible=find('compatible-version-control');check(!compatible.rejectCodes&&keys.has('baseline')&&session.profiles[compatible.profile].versionId!==session.profiles[find('baseline').profile].versionId,'COMPATIBLE_CONTROL_REQUIRED');
    const revoked=find('revoked-permission');check(revoked.request.nivelRef===session.profiles[revoked.profile].nivelRef&&revoked.request.versionId===session.profiles[revoked.profile].versionId&&stable(revoked.rejectCodes)===stable(['ACCESO_DENEGADO']),'REVOCATION_CASE_INVALID');
    check(find('baseline').profile===revoked.profile&&stable(find('baseline').request)===stable(revoked.request)&&session.cases.indexOf(find('baseline'))<session.cases.indexOf(revoked),'REVOCATION_BASELINE_REQUIRED');
  }
  return session;
}
async function runPlanSession(auth,session,authorization,query,onReference=()=>{},options={}){
  validateSession(session);
  const resume=validateResumePlan(session,options.resumePlan);
  if(session.scenario==='PLAN-03')check(authorization.revocation===true,'REVOCATION_AUTHORIZATION_REQUIRED');
  let attempts=0,preparations=0,rejections=0,resumedPlans=0,restored=false;const saved=[];
  for(const c of session.cases){
    const progress=phase=>options.onProgress?.({caseKey:c.key,versionId:session.profiles[c.profile].versionId,phase});
    progress('CONTROL_BEFORE');
    const p=session.profiles[c.profile],scope={...authorization,scenario:session.scenario,maximumPreparations:1,foreignLevel:session.foreignLevel,invalidVersion:session.invalidVersion};
    const controls=createPlanControls(query,p,session.identity),before=await controls.snapshot();
    const client=createPreparationClient(auth,p,scope,onReference);await client.refreshToken();
    let response;
    const reused=resume?.caseKey===c.key;
    progress(reused?'READ_EXISTING_PLAN':'PREPARATION');
    if(reused){response=await controls.readExisting(resume.planId,resume.planHash);onReference({planId:response.planId});resumedPlans++;}
    else if(c.key==='revoked-permission'){
      check(session.scenario==='PLAN-03'&&saved.length>0,'REVOCATION_BASELINE_REQUIRED');
      const baseline=saved.find(s=>s.profile===c.profile&&stable(s.request)===stable(c.request));check(baseline,'REVOCATION_BASELINE_REQUIRED');
      const start=options.startBridge||(()=>require('./doc110-permission-revocation.cjs').startDatabaseBridge('doc110-permission-db.ps1',{operationalImport:true,runtimeRoot:session.runtimeRoot,planId:baseline.data.planId,planHash:baseline.data.planHash}));
      const bridge=start();let changed=false;
      try{
        const ready=await bridge.ready();check(ready.ok&&ready.stage==='READY'&&ready.originalGranted&&ready.permissionColumn==='IMPORTAR_CARPETAS'&&ready.userRef===session.identity.userRef&&ready.companyRef===session.identity.ambitoRef,'REVOCATION_TARGET_MISMATCH');
        progress('PERMISSION_READY');changed=true;progress('PERMISSION_REVOKING');const revoked=await bridge.send('REVOKE');check(revoked.ok&&revoked.stage==='REVOKED'&&revoked.otherFieldsUnchanged,'REVOCATION_NOT_CONFIRMED');progress('PERMISSION_REVOKED');
        response=await client.request('PrepararPlan',c.request,c.rejectCodes);
      }finally{
        try{if(changed){progress('PERMISSION_RESTORING');const result=await bridge.send('RESTORE');check(result.ok&&result.stage==='RESTORED'&&result.otherFieldsUnchanged,'RESTORATION_NOT_CONFIRMED');restored=true;progress('PERMISSION_RESTORED');}}
        finally{await bridge.close();}
      }
      await client.request('AbrirImportacion',{nivelRef:p.nivelRef});
    }else response=await client.request('PrepararPlan',c.request,c.rejectCodes||[]);
    attempts+=client.attempts;
    if(c.rejectCodes)rejections++;
    else{
      progress('VERIFY_PLAN');
      const fingerprint=assertPreparation(response,c.request,c.expected),configuration=response.snapshot.Configuracion;
      const policy=p.mode==='PARCIAL'?['PARCIAL_PERMITIDO','NO_APLICA']:['COMPLETO_OBLIGATORIO',p.mode];
      check(configuration?.ModoIntegridad===policy[0]&&configuration?.AmbitoIntegridad===policy[1],'PUBLISHED_POLICY_CHANGED');
      await controls.verifyPlan(response,c.request);if(!reused)preparations++;
      options.onPlanVerified?.({caseKey:c.key,planId:response.planId,planHash:response.planHash});
      if(c.key==='determinism-control'){
        const previous=saved.find(x=>x.profile===c.profile&&stable(x.request)===stable(c.request));check(previous&&previous.fingerprint===fingerprint,'DETERMINISM_NOT_VERIFIED');
      }
      saved.push({data:response,request:c.request,controls,fingerprint,profile:c.profile});
    }
    progress('CONTROL_AFTER');controls.unchanged(before,await controls.snapshot(),reused||c.rejectCodes?0:1);
  }
  for(const item of saved)await item.controls.verifyPlan(item.data,item.request);
  check(session.scenario!=='PLAN-03'||restored,'RESTORATION_NOT_CONFIRMED');
  return {scenario:session.scenario,status:'PASS',attempts,preparations,rejections,...(resume?{resumedPlans}:{}),confirmations:0,noDocumentEffects:true,readOnlyControls:true,restorationVerified:session.scenario==='PLAN-03'?restored:null};
}
module.exports={SCENARIOS,authorize,createPreparationClient,assertPreparation,createPlanControls,validateSession,runPlanSession,comparePreparationDocuments,comparePublishedSnapshot,validateResumePlan,safePlanObservation};
