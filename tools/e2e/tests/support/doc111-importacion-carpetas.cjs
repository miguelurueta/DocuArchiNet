'use strict';
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const {safePost} = require('./doc110-safe-request.cjs');
const {createWorkflowLoginPageSession,LOGIN_SELECTORS} = require('./authenticated-workflow-session.cjs');

const MODES = Object.freeze(['EXPEDIENTE', 'LOTE', 'PARCIAL']);
const OPERATIONS = Object.freeze(['AbrirImportacion', 'ListarPerfilesImportacion', 'PrepararPlan', 'ConfirmarPlan', 'ConsultarLote', 'ConsultarRecursosLote', 'AvanzarLote', 'SolicitarDetencion', 'CerrarReservaSinInicio', 'ReconciliarRecurso', 'RecuperarDocumentoV2']);
const FILE_NAME = 'DOC111 Órdenes 0007.pdf';
function fail(code) { throw new Error('DOC111_' + code); }
function check(value, code) { if (!value) fail(code); }
function id(value) { return typeof value === 'string' && /^[1-9][0-9]{0,9}$/.test(value) && BigInt(value) <= 4294967295n; }
function closed(value, fields) { check(value && typeof value === 'object' && !Array.isArray(value) && Object.keys(value).every(k => fields.includes(k)), 'CONTRACT_INVALID'); }
function recoveryRequest(value) {
  const fields=['loteId','intentoId','revisionLoteEsperada','revisionArchivoEsperada'];
  closed(value,fields);check(Object.keys(value).length===fields.length,'CONTRACT_INVALID');
  const request=Object.fromEntries(fields.map(k=>[k,value[k]]));
  check(id(request.loteId)&&id(request.intentoId)&&['revisionLoteEsperada','revisionArchivoEsperada'].every(k=>Number.isInteger(request[k])&&request[k]>0&&request[k]<=4294967295),'CONTRACT_INVALID');
  return request;
}
function recoveryResponse(value,request,code) {
  const fields=['loteId','intentoId','fase','estado','codigo','resuelta','repeticion','cierreSinInvocacion','documentoConfirmado','expedienteId','relacionNivelId','gabineteId','documentoId','inventarioId'];
  check(value&&typeof value==='object'&&!Array.isArray(value)&&Object.keys(value).length===fields.length&&Object.keys(value).every(k=>fields.includes(k)),'RESPONSE_INVALID');
  check(value.loteId===request.loteId&&value.intentoId===request.intentoId&&value.fase==='ALMACENAR'&&value.resuelta===true&&typeof value.repeticion==='boolean'&&['expedienteId','relacionNivelId','gabineteId'].every(k=>id(value[k]))&&value.codigo===code,'RESPONSE_INVALID');
  const closedControl=value.estado==='RECHAZADO'&&value.codigo==='EJECUCION_CERRADA_SIN_INVOCACION'&&value.cierreSinInvocacion===true&&value.documentoConfirmado===false&&value.documentoId===null&&value.inventarioId===null;
  const confirmed=value.estado==='CONFIRMADO'&&value.codigo==='DOCUMENTO_CONFIRMADO'&&value.cierreSinInvocacion===false&&value.documentoConfirmado===true&&id(value.documentoId)&&id(value.inventarioId);
  check(closedControl||confirmed,'RESPONSE_INVALID');
  return Object.fromEntries(fields.map(k=>[k,value[k]]));
}
function validateProfile(p) {
  closed(p, ['baseUrl', 'module', 'ignoreHttpsErrors', 'timeoutMilliseconds', 'odbcDsn', 'docuarchiOdbcDsn', 'nivelRef', 'versionId', 'mode', 'pdfFixture', 'rootNames', 'expectations']);
  check([p.odbcDsn,p.docuarchiOdbcDsn].every(d=>typeof d==='string'&&/^[A-Za-z0-9 _.-]{1,128}$/.test(d)), 'PROFILE_DSN_INVALID');
  let url; try { url = new URL(p.baseUrl); } catch { fail('PROFILE_INVALID'); }
  check(['http:', 'https:'].includes(url.protocol) && !url.username && !url.password && !url.search && !url.hash && url.pathname.endsWith('/'), 'PROFILE_INVALID');
  check(p.module === 'GESTOR' && typeof p.ignoreHttpsErrors === 'boolean' && Number.isInteger(p.timeoutMilliseconds) && p.timeoutMilliseconds >= 30000 && p.timeoutMilliseconds <= 900000, 'PROFILE_INVALID');
  check(id(p.nivelRef) && id(p.versionId) && MODES.includes(p.mode), 'PROFILE_INVALID');
  check(typeof p.pdfFixture === 'string' && /^[A-Za-z0-9_-]+\.pdf$/.test(p.pdfFixture), 'PDF_INVALID');
  check(Array.isArray(p.rootNames) && p.rootNames.length === 2 && p.rootNames.every(n => typeof n === 'string' && n.length > 0 && n.length <= 120 && !/[\\/|<>:"?*\x00-\x1f]/.test(n)), 'ROOTS_INVALID');
  check(Array.isArray(p.expectations) && p.expectations.length === 2, 'EXPECTATIONS_REQUIRED');
  for (const e of p.expectations) {
    closed(e, ['codigo', 'nombre', 'tipologiaId', 'indices']);
    check(typeof e.codigo === 'string' && e.codigo.length > 0 && typeof e.nombre === 'string' && e.nombre.length > 0 && id(e.tipologiaId) && Array.isArray(e.indices) && e.indices.length > 0 && e.indices.length <= 128, 'EXPECTATIONS_INVALID');
    check(new Set(e.indices.map(i => i.referencia)).size === e.indices.length, 'EXPECTATIONS_INVALID');
    for (const i of e.indices) { closed(i, ['referencia', 'tipo', 'valor']); check(id(i.referencia) && ['TEXTO', 'REFERENCIA', 'ENTERO', 'DECIMAL', 'FECHA'].includes(i.tipo) && (i.valor === null || typeof i.valor === 'string'), 'EXPECTATIONS_INVALID'); }
  }
  return p;
}
function readFixture(profile) {
  const root = fs.realpathSync(path.resolve(__dirname, '../../fixtures'));
  const target = fs.realpathSync(path.join(root, profile.pdfFixture));
  check(path.dirname(target) === root, 'PDF_INVALID');
  const bytes = fs.readFileSync(target);
  check(bytes.length > 0 && bytes.length <= 2 * 1024 * 1024 && bytes.subarray(0, 5).toString() === '%PDF-', 'PDF_INVALID');
  return bytes;
}
function assertAuthorizations(a) {
  check(a && ['environment', 'account', 'effects', 'discardable', 'select'].every(k => a[k] === true), 'AUTHORIZATION_REQUIRED');
}
function classifyLoginMessage(message){
  if(typeof message!=='string'||message.length>8192)return null;
  const text=message.normalize('NFD').replace(/[\u0300-\u036f]/g,'').replace(/\s+/g,' ').trim().toLowerCase();
  if(/^la contrasena no (?:es valida para|coincide[n]? con) el modulo\b/.test(text))return 'LOGIN_CREDENTIALS_REJECTED';
  if(/^el usuario no coincide con el modulo\b/.test(text))return 'LOGIN_ACCOUNT_NOT_FOUND';
  if(/^el usuario esta bloqueado en la aplicacion\b/.test(text))return 'LOGIN_ACCOUNT_BLOCKED';
  if(/^por favor (?:digite el usuario|digite la contrasena|seleccione el modulo de aplicacion)$/.test(text))return 'LOGIN_INPUT_REQUIRED';
  if(text==='second_factor_required')return 'LOGIN_SECOND_FACTOR_REQUIRED';
  if(/^(?:second_factor_[a-z_]+|preauthentication_failed|login_[a-z_]+)$/.test(text))return 'LOGIN_SERVICE_REJECTED';
  return null;
}
function classifyLoginPostback(body){
  if(typeof body!=='string')return null;
  // Solo la llamada emitida por gestor.aspx.vb, no la presencia del widget.
  if(/if\s*\(window\.DocuArchiSegundoFactor\)\s*\{\s*window\.DocuArchiSegundoFactor\.iniciar\(\);?\s*\}/.test(body))return 'LOGIN_SECOND_FACTOR_REQUIRED';
  // Showscripman emite un bloque con una unica llamada alert. No conservar
  // el cuerpo, mensajes desconocidos, credenciales o contenido del dialogo.
  const scripts=body.matchAll(/<script\b[^>]*>\s*alert\(\s*(['"])([^\r\n]*?)\1\s*\);?\s*<\/script>/gi);
  for(const match of scripts){const code=classifyLoginMessage(match[2]);if(code)return code;}
  return null;
}
function loginObservation(value){
  if(!value||typeof value!=='object')return null;
  const enums={finish:['NOT_OBSERVED','COMPLETE','UNAVAILABLE'],body:['NOT_OBSERVED','AVAILABLE','UNAVAILABLE','REDIRECT'],unavailableAt:['NONE','FINISH','BODY'],category:['NONE','TARGET_CLOSED','NAVIGATION_ABORTED','BODY_NOT_RETAINED','TIMEOUT','UNCLASSIFIED'],postedFields:['NOT_OBSERVED','MATCHED','MISSING','DIFFERENT']};
  const result={};
  for(const key of ['postbackStatus','productionStatus','probeStatus']){const status=value[key];if(status!==null&&(!Number.isInteger(status)||status<100||status>599))return null;result[key]=status;}
  for(const [key,allowed] of Object.entries(enums)){if(!allowed.includes(value[key]))return null;result[key]=value[key];}
  return result;
}
function loginReadFailure(error){
  const text=String(error?.message||'');
  if(/Target (?:page|closed)|context.*closed|browser.*closed/i.test(text))return 'TARGET_CLOSED';
  if(/ERR_ABORTED/i.test(text))return 'NAVIGATION_ABORTED';
  if(/No (?:resource|data).*identifier|response body.*(?:unavailable|evicted)|No resource with given identifier/i.test(text))return 'BODY_NOT_RETAINED';
  if(/timeout|timed out/i.test(text))return 'TIMEOUT';
  return 'UNCLASSIFIED';
}
async function openSession(browser, profile, environment, authorization, runtime = {}) {
  check(authorization?.environment===true&&authorization?.account===true,'AUTHORIZATION_REQUIRED');
  let loginIssue=null,auth;
  const observation={postbackStatus:null,productionStatus:null,probeStatus:null,finish:'NOT_OBSERVED',body:'NOT_OBSERVED',unavailableAt:'NONE',category:'NONE',postedFields:'NOT_OBSERVED'};
  try{auth = await (runtime.createSession||createWorkflowLoginPageSession)(browser, {baseUrl: profile.baseUrl, environment,
    moduleEnvironmentVariable: 'DOC111_E2E_MODULE', userEnvironmentVariable: 'DOC111_E2E_USER', passwordEnvironmentVariable: 'DOC111_E2E_PASSWORD',
    ignoreHTTPSErrors: profile.ignoreHttpsErrors, timeoutMilliseconds: profile.timeoutMilliseconds,
    onDialogCompleted:async(_page,message)=>{loginIssue=classifyLoginMessage(message)||'LOGIN_SERVER_REJECTED';},
    onPostbackCompleted:async(_page,response)=>{
      if(typeof response.status==='function')observation.postbackStatus=response.status();
      // Comparar en memoria lo enviado con la entrada efimera, nunca conservar
      // el formulario, nombres de cuenta, contrasenas o viewstate.
      try{
        if(typeof response.request==='function'&&typeof response.request().postData==='function'){
          const form=new URLSearchParams(response.request().postData()||'');
          const field=suffix=>[...form.keys()].filter(k=>k===suffix||k.endsWith('$'+suffix));
          const users=field('TextBoxuser'),passwords=field('TextBoxpasw');
          observation.postedFields=users.length===1&&passwords.length===1?(form.get(users[0])===environment.DOC111_E2E_USER&&form.get(passwords[0])===environment.DOC111_E2E_PASSWORD?'MATCHED':'DIFFERENT'):'MISSING';
        }
      }catch{}
      if(observation.postbackStatus>=400){loginIssue='LOGIN_HTTP_REJECTED';return;}
      if(typeof response.finished==='function'){
        try{const error=await response.finished();if(error)throw error;observation.finish='COMPLETE';}
        catch(error){observation.finish='UNAVAILABLE';observation.unavailableAt='FINISH';observation.category=loginReadFailure(error);return;}
      }
      if(observation.postbackStatus>=300&&observation.postbackStatus<400){observation.body='REDIRECT';return;}
      try{const issue=classifyLoginPostback(await response.text());observation.body='AVAILABLE';if(issue)loginIssue=issue;}
      catch(error){observation.body='UNAVAILABLE';observation.unavailableAt='BODY';observation.category=loginReadFailure(error);}
    }});}catch{const error=new Error('DOC111_LOGIN_TRANSPORT_FAILED');error.loginObservation=loginObservation(observation);throw error;}
  try {
    if(loginIssue)fail(loginIssue);
    const response=await auth.page.goto(new URL('Gestion/WebFormProducionDocumental.aspx', profile.baseUrl).href,{waitUntil:'domcontentloaded',timeout:profile.timeoutMilliseconds});
    observation.productionStatus=response?.status?.()??null;
    check(response&&response.status()===200,'PRODUCTION_OPEN_FAILED');
    check(await auth.page.locator(LOGIN_SELECTORS.password).count()===0,'AUTHENTICATED_PAGE_UNAVAILABLE');
    const control=auth.page.locator('#doc111-importacion');
    if(await control.count()!==1){
      // Same read-only missing-host diagnosis used by DOC-110. No token is
      // fabricated: the server still applies authority, policy and CSRF guards.
      let diagnostic='IMPORT_CONTROL_UNAVAILABLE';
      try {
        // An outer bounded run also counts this failure-path API probe.
        runtime.beforeDiagnosticPost?.('AbrirImportacion');
        const probe=await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/AbrirImportacion',profile.baseUrl).href,
          {maxRedirects:0,headers:{Origin:new URL(profile.baseUrl).origin},data:{solicitud:{nivelRef:profile.nivelRef}}},'AbrirImportacion');
        const raw=await probe.json(),body=raw?.d??raw;
        observation.probeStatus=probe.status();
        if(probe.status()===401&&body?.codigo==='SESION_REQUERIDA')diagnostic='SESSION_NOT_ESTABLISHED';
        else if(probe.status()===403&&body?.codigo==='ACCESO_DENEGADO')diagnostic='IMPORT_ACCESS_DENIED';
        else if(probe.status()===503&&body?.codigo==='CAPACIDAD_NO_DISPONIBLE')diagnostic='IMPORT_CAPABILITY_UNAVAILABLE';
      }catch{}
      fail(diagnostic);
    }
    const token=await control.getAttribute('data-token');
    check(typeof token==='string'&&token.length>0,'TOKEN_UNAVAILABLE');
    auth.loginObservation=loginObservation(observation);return auth;
  } catch(error) {
    await auth.context.close().catch(()=>{});
    error.loginObservation=loginObservation(observation);
    if(['DOC111_PRODUCTION_OPEN_FAILED','DOC111_AUTHENTICATED_PAGE_UNAVAILABLE','DOC111_IMPORT_CONTROL_UNAVAILABLE','DOC111_TOKEN_UNAVAILABLE','DOC111_SESSION_NOT_ESTABLISHED','DOC111_IMPORT_ACCESS_DENIED','DOC111_IMPORT_CAPABILITY_UNAVAILABLE',...['LOGIN_CREDENTIALS_REJECTED','LOGIN_ACCOUNT_NOT_FOUND','LOGIN_ACCOUNT_BLOCKED','LOGIN_INPUT_REQUIRED','LOGIN_SECOND_FACTOR_REQUIRED','LOGIN_SERVICE_REJECTED','LOGIN_SERVER_REJECTED','LOGIN_POSTBACK_UNAVAILABLE','LOGIN_HTTP_REJECTED'].map(c=>'DOC111_'+c)].includes(error?.message))throw error;
    const sanitized=new Error('DOC111_PRODUCTION_OPEN_FAILED');sanitized.loginObservation=loginObservation(observation);throw sanitized;
  }
}
function createDriver(auth, profile, authorization, options = {}) {
  if(options.readOnly)check(authorization&&['environment','account','select'].every(k=>authorization[k]===true)&&authorization.effects!==true&&authorization.discardable!==true,'AUTHORIZATION_REQUIRED');
  else assertAuthorizations(authorization);
  let token;
  async function refreshToken() {
    try { token = await auth.page.locator('#doc111-importacion').getAttribute('data-token'); check(typeof token === 'string' && token.length > 0, 'TOKEN_UNAVAILABLE'); }
    catch { fail('TOKEN_UNAVAILABLE'); }
  }
  async function request(operation, payload, expectFailure = false) {
    check(OPERATIONS.includes(operation), 'OPERATION_INVALID');
    if(options.readOnly)check(['ConsultarLote','ConsultarRecursosLote'].includes(operation),'READONLY_OPERATION_REQUIRED');
    if(operation==='RecuperarDocumentoV2')payload=recoveryRequest(payload);
    const response = await safePost(auth.context.request, new URL('webservice/WebServiceImportacionCarpetas.asmx/' + operation, profile.baseUrl).href,
      {maxRedirects:0, headers: {'X-Importacion-Carpetas-CSRF': token, Origin: new URL(profile.baseUrl).origin}, data: {solicitud: payload}}, operation);
    let raw; try { raw = await response.json(); } catch { fail('RESPONSE_INVALID'); }
    const body = raw?.d ?? raw;
    if(operation==='PrepararPlan'&&id(body?.datos?.planId))options.onReference?.({planId:body.datos.planId});
    if(operation==='ConfirmarPlan'&&id(body?.datos?.loteId))options.onReference?.({loteId:body.datos.loteId});
    check(body && typeof body.ok === 'boolean', 'RESPONSE_INVALID');
    if (expectFailure) { check(response.status() === 409 && !body.ok && body.codigo === 'OPERACION_EN_CURSO', 'BARRIER_NOT_ENFORCED'); return; }
    if(response.status() !== 200 || !body.ok){
      const error=new Error('DOC111_OPERATION_NOT_CONFIRMED');
      error.operation=operation;error.httpStatus=response.status();
      const codes=['CAPACIDAD_NO_DISPONIBLE','ACCESO_DENEGADO','SOLICITUD_INVALIDA','SESION_REQUERIDA','SOLICITUD_NO_VERIFICADA','CSRF_INVALIDO','ORIGEN_INVALIDO','ERROR_INTERNO','CONFIGURACION_INVALIDA','OPERACION_EN_CURSO','CONFLICTO_REVISION','ARCHIVO_RECHAZADO','RESULTADO_INCIERTO'];
      error.serverCode=codes.includes(body.codigo)?body.codigo:'NO_CLASIFICADO';
      throw error;
    }
    return operation==='RecuperarDocumentoV2'?recoveryResponse(body.datos,payload,body.codigo):body.datos;
  }
  async function pages(loteId, tipo) {
    const state = await request('ConsultarLote', {loteId});
    check(state.loteId === loteId && state.nivelRef === profile.nivelRef && Number.isInteger(state.maximoTamanoPagina) && state.maximoTamanoPagina > 0, 'DESTINATION_CHANGED');
    let revision, total; const records = [], size = Math.min(25, state.maximoTamanoPagina);
    for (let page = 1; page <= 100; page++) {
      const r = await request('ConsultarRecursosLote', {loteId, tipo, pagina: page, tamanoPagina: size});
      check(r.loteId === loteId && r.tipo === tipo && r.pagina === page && r.tamanoPagina === size && Array.isArray(r.elementos), 'PAGE_INVALID');
      if (page === 1) { revision = r.revisionLectura; total = r.totalFiltrado; }
      check(revision === r.revisionLectura && total === r.totalFiltrado && total === r.totalOriginal, 'PAGE_CHANGED');
      records.push(...r.elementos);
      if (records.length >= total) { check(records.length === total && new Set(records.map(i => i.recursoId)).size === total, 'PAGE_INVALID'); return records; }
    }
    fail('PAGE_BUDGET_EXCEEDED');
  }
  async function receive(loteId, file, bytes, originalName = FILE_NAME) {
    check(!options.readOnly,'READONLY_OPERATION_REQUIRED');
    check(originalName===FILE_NAME||typeof originalName==='string'&&/^[A-Za-z0-9][A-Za-z0-9 ._-]{0,119}\.pdf$/.test(originalName),'UPLOAD_NAME_INVALID');
    const response = await safePost(auth.context.request, new URL('generic_control/ImportacionCarpetasUpload.ashx', profile.baseUrl).href,
      {maxRedirects:0, headers: {'X-Importacion-Carpetas-CSRF': token, Origin: new URL(profile.baseUrl).origin}, multipart: {
        loteId, archivoId: file.recursoId, claveIntento: crypto.randomBytes(16).toString('hex'), file: {name: originalName, mimeType: 'application/pdf', buffer: bytes}}}, 'RecibirArchivo');
    let r; try { const raw = await response.json(); r = raw.d ?? raw; } catch { fail('RECEIPT_NOT_CONFIRMED'); }
    check(response.status() === 200 && r.ok && r.datos?.loteId === loteId && r.datos.archivoId === file.recursoId && r.datos.estadoRecepcion === 'RECIBIDO', 'RECEIPT_NOT_CONFIRMED');
  }
  async function reopen(loteId) {
    try {
      await auth.page.reload(); await refreshToken();
      await auth.page.locator('[data-doc111-lote]').fill(loteId);
      await auth.page.locator('[data-doc111-consultar]').click();
      await auth.page.getByRole('dialog').waitFor();
      await auth.page.waitForFunction(lot => document.querySelector('.doc111-content')?.textContent.includes('Lote ' + lot), loteId);
    } catch { fail('REOPEN_FAILED'); }
  }
  return {refreshToken, request, pages, receive, reopen};
}
function one(rows, code) { check(Array.isArray(rows) && rows.length === 1, code); return rows[0]; }
function json(text) { try { return typeof text === 'string' ? JSON.parse(text) : text; } catch { fail('STORED_PROOF_INVALID'); } }
function identifier(value) { check(typeof value === 'string' && /^[A-Za-z_][A-Za-z0-9_]{0,63}$/.test(value), 'PHYSICAL_IDENTIFIER_INVALID'); return '`' + value + '`'; }
function decimal(value) {
  const parts = /^([+-]?)(\d+)(?:\.(\d*))?$/.exec(String(value));
  if (!parts) return String(value);
  const whole = parts[2].replace(/^0+(?=\d)/, ''), fraction = (parts[3] || '').replace(/0+$/, '');
  return (parts[1] === '-' && (whole !== '0' || fraction) ? '-' : '') + whole + (fraction ? '.' + fraction : '');
}
function same(actual, expected, type) { return expected === null ? actual === null : actual !== null && (type === 'DECIMAL' ? decimal(actual) === decimal(expected) : String(actual) === expected); }
function assertEffective(values, expected) {
  check(Array.isArray(values) && values.length === expected.indices.length + 1 && values.filter(v => v.DestinoTipo === 'TIPOLOGIA' && v.Valor === expected.tipologiaId && !v.Diferido).length === 1, 'EFFECTIVE_VALUES_INVALID');
  for (const wanted of expected.indices) {
    const value = one(values.filter(v => v.DestinoTipo === 'INDICE_GABINETE' && v.DestinoRef === wanted.referencia), 'EFFECTIVE_VALUES_INVALID');
    check(value.TipoValor === wanted.tipo && value.Valor === wanted.valor && !value.Diferido, 'EFFECTIVE_VALUES_INVALID');
  }
}
function createSelectControls(query) {
  async function rows(origin, sql, args) {
    check(['RA', 'DA'].includes(origin) && /^SELECT\b/.test(sql) && !/;|\b(?:FOR UPDATE|INTO OUTFILE|INTO DUMPFILE)\b/i.test(sql), 'SELECT_REQUIRED');
    try { return await query(origin, sql, args); } catch { fail('SELECT_FAILED'); }
  }
  async function snapshot(loteId) {
    check(id(loteId), 'LOT_INVALID');
    const lot = one(await rows('RA', 'SELECT lote_id,config_doc,contexto_doc,estado,revision,detencion_solicitada,total_raices,total_archivos,total_bytes FROM ra_integracion_lote WHERE lote_id=?', [loteId]), 'LOT_NOT_FOUND');
    const roots = await rows('RA', 'SELECT raiz_id,cliente_raiz_id,datos_doc,estado,identidad_expediente_doc,intento_activo_id,revision FROM ra_integracion_raiz WHERE lote_id=? ORDER BY raiz_id', [loteId]);
    const files = await rows('RA', 'SELECT f.archivo_id,f.raiz_id,f.cliente_archivo_id,f.bytes_previstos,f.estado_recepcion,f.estado_efecto,f.valores_efectivos_doc,f.resultado_doc,f.intento_activo_id,f.revision FROM ra_integracion_archivo f INNER JOIN ra_integracion_raiz r ON r.raiz_id=f.raiz_id WHERE r.lote_id=? ORDER BY f.archivo_id', [loteId]);
    const attempts = await rows('RA', 'SELECT intento_id,raiz_id,archivo_id,fase,estado,evidencia_doc,fin_utc FROM ra_integracion_intento WHERE lote_id=? ORDER BY intento_id', [loteId]);
    const events = await rows('RA', 'SELECT evento_id,intento_id,recurso_tipo,recurso_id,accion,codigo FROM ra_integracion_evento WHERE lote_id=? ORDER BY evento_id', [loteId]);
    return {lot, roots, files, attempts, events};
  }
  async function readKnownDocumentBody(c,ctx,identity,expected,fileName,result,cabinet,documentClass,extension) {
      const selected = [], physicalNames = new Set();
      for (let i = 0; i < expected.indices.length; i++) {
        const wanted = expected.indices[i];
        const field = one(await rows('DA', 'SELECT CAMPO FROM detalle_gabienete WHERE id_gabinete=? AND id_detalle_gabinete=? AND VISIBLE=1', [c.GabineteId, wanted.referencia]), 'FIELD_AMBIGUOUS');
        check(typeof field.CAMPO === 'string' && !physicalNames.has(field.CAMPO.toLowerCase()), 'FIELD_AMBIGUOUS'); physicalNames.add(field.CAMPO.toLowerCase());
        selected.push('CAST(g.' + identifier(field.CAMPO) + ' AS CHAR) AS value_' + i);
      }
      const document = one(await rows('DA', 'SELECT g.ID,g.ID_EXPEDIENTE,g.ID_USUARIO_GESTION,g.NOMBRE,g.DBT,r.ID_CLASE_DOCUMENTO,r.ID_TIPO_DOCUMENTO,r.ID_EMPRESA_DOCUMENTO,r.ID_USUARIO_GESTION AS inventory_user,r.SEGUNDO_NOMBRE_DOCUMENTO,r.FORMATO AS inventory_format,i.segundo_nombre,i.formato AS index_format,i.expediente_archivo_ID_EXPEDIENTE AS indexed_expedition' + (selected.length ? ',' + selected.join(',') : '') + ' FROM ' + identifier(cabinet.NOMBRE) + ' g INNER JOIN registro_producion_documental r ON r.ID_REGISTRO_PRODUCION_DOCUMENTAL=g.ID_INVENTARIO_DOCUMENTAL AND r.ID_DOCUMENTO_DOCUARCHI_ALMACEN=g.ID AND r.EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=g.ID_EXPEDIENTE AND r.NOMBRE_GABINETE=? INNER JOIN ra_cert_indice_expediente i ON i.registro_producion_documental_ID_REGISTRO_PRODUCION_DOCUMENTAL=r.ID_REGISTRO_PRODUCION_DOCUMENTAL WHERE g.ID=? AND g.ID_INVENTARIO_DOCUMENTAL=?', [cabinet.NOMBRE, result.DocumentoId, result.InventarioId]), 'DOCUMENT_RELATION_INVALID');
      const nameConfig=await rows('DA',"SELECT VISIBLE FROM detalle_gabienete WHERE GABINETE=? AND UPPER(CAMPO)='NOMBRE' LIMIT 2",[cabinet.NOMBRE]);
      check(nameConfig.length<=1&&nameConfig.every(r=>['0','1'].includes(String(r.VISIBLE))),'NAME_METADATA_INVALID');
      const visible=nameConfig.length===1&&String(nameConfig[0].VISIBLE)==='1';
      const cabinetNameMatches=document.NOMBRE===fileName||(!visible&&(document.NOMBRE===null||document.NOMBRE===''));
      check(String(document.ID_EXPEDIENTE) === identity.ExpedienteId && String(document.indexed_expedition) === identity.ExpedienteId && String(document.ID_USUARIO_GESTION) === ctx.UsuarioRef && String(document.inventory_user) === ctx.UsuarioRef && String(document.ID_EMPRESA_DOCUMENTO) === ctx.AmbitoRef && String(document.ID_TIPO_DOCUMENTO) === expected.tipologiaId && String(document.ID_CLASE_DOCUMENTO) === String(documentClass.id) && String(document.DBT) === String(extension.id) && cabinetNameMatches && [document.SEGUNDO_NOMBRE_DOCUMENTO,document.segundo_nombre].every(n => n === fileName) && document.inventory_format === '.PDF' && document.index_format === '.PDF', 'DOCUMENT_VALUES_INVALID');
      expected.indices.forEach((v,i) => check(same(document['value_' + i], v.valor, v.tipo), 'PHYSICAL_INDEX_INVALID'));
    return document;
  }
  async function verify(loteId, profile, options = {}) {
    closed(options,['singleRoot','fileNames','expectedDates']);
    check(options.singleRoot===undefined||options.singleRoot===true&&profile.mode==='EXPEDIENTE','CONTROL_SCOPE_INVALID');
    const rootCount=options.singleRoot?1:2, fileNames=options.fileNames||Array(rootCount).fill(FILE_NAME);
    check(profile.expectations.length===rootCount&&Array.isArray(fileNames)&&fileNames.length===rootCount&&fileNames.every(n=>typeof n==='string'&&n.length>0&&!/[\\/\x00-\x1f]/.test(n)),'CONTROL_SCOPE_INVALID');
    check(options.expectedDates===undefined||Array.isArray(options.expectedDates)&&options.expectedDates.length===rootCount&&options.expectedDates.every(d=>/^\d{4}-\d{2}-\d{2}$/.test(d)),'CONTROL_SCOPE_INVALID');
    const view = await snapshot(loteId), snapshotProfile = json(view.lot.config_doc), c = snapshotProfile.Configuracion, ctx = json(view.lot.contexto_doc);
    check(snapshotProfile.VersionId === profile.versionId && c.ModoIntegridad === (profile.mode === 'PARCIAL' ? 'PARCIAL_PERMITIDO' : 'COMPLETO_OBLIGATORIO') && c.AmbitoIntegridad === (profile.mode === 'PARCIAL' ? 'NO_APLICA' : profile.mode), 'STORED_POLICY_CHANGED');
    check(ctx.NivelRef === profile.nivelRef && String(view.lot.total_raices) === String(rootCount) && String(view.lot.total_archivos) === (profile.mode === 'PARCIAL' ? '3' : String(rootCount)), 'ORIGINAL_TOTALS_CHANGED');
    check(view.lot.estado === (profile.mode === 'PARCIAL' ? 'PARCIAL' : 'COMPLETADO') && view.attempts.every(i => ['CONFIRMADO','RECHAZADO'].includes(i.estado)), 'FINAL_PROOF_INVALID');
    const cabinet = one(await rows('DA', 'SELECT NOMBRE FROM system1 WHERE id_gabinete=?', [c.GabineteId]), 'CABINET_AMBIGUOUS');
    const cycle = one(await rows('RA', 'SELECT Id_tipos_ciclo_archivo AS id,Nombre_Tipo_ciclo_archivo AS nombre FROM ra_de_tipos_ciclos_archivo WHERE Nombre_Tipo_ciclo_archivo=?', ['Archivo Gestión']), 'CYCLE_AMBIGUOUS');
    check(cycle.nombre === 'Archivo Gestión', 'CYCLE_CHANGED');
    const documentClass = one(await rows('RA', 'SELECT ID_TIPO_DOCUMENTO AS id FROM ra_tipo_documento WHERE DOCUMENTO=?', ['DOCUMENTO ELECTRONICO']), 'CLASS_AMBIGUOUS');
    const extension = one(await rows('DA', 'SELECT ESTADO_NORMAL AS id FROM DA_EXTENSION WHERE ESTENSION=?', ['.PDF']), 'FORMAT_AMBIGUOUS');
    check(view.roots.length === rootCount && view.files.length === Number(view.lot.total_archivos), 'ORIGINAL_TOTALS_CHANGED');
    check(view.files.reduce((sum,f)=>sum+BigInt(f.bytes_previstos),0n).toString() === String(view.lot.total_bytes), 'ORIGINAL_TOTALS_CHANGED');
    const physical = [], expeditionIds = new Set(), documentIds = new Set();
    for (let index = 0; index < rootCount; index++) {
      const root = one(view.roots.filter(r => r.cliente_raiz_id === 'r' + (index + 1)), 'ROOT_CORRELATION_INVALID'), identity = json(root.identidad_expediente_doc), expected = profile.expectations[index];
      check(root.estado === (profile.mode === 'PARCIAL' && index === 1 ? 'PARCIAL' : 'COMPLETADA') && !root.intento_activo_id, 'FINAL_PROOF_INVALID');
      check(identity?.NivelRef === profile.nivelRef && id(identity.ExpedienteId) && id(identity.RelacionNivelId) && !expeditionIds.has(identity.ExpedienteId), 'ROOT_IDENTITY_INVALID'); expeditionIds.add(identity.ExpedienteId);
      const proof = one(view.attempts.filter(i => String(i.raiz_id) === String(root.raiz_id) && i.fase === 'CREAR_EXPEDIENTE' && i.estado === 'CONFIRMADO' && i.fin_utc), 'CREATION_PROOF_INVALID');
      const creation = json(proof.evidencia_doc);
      check(creation?.Estado === 'CONFIRMADO' && creation.Creacion?.CicloArchivoId === String(cycle.id) && creation.Creacion.IntentoId === String(proof.intento_id) && creation.Expediente?.ExpedienteId === identity.ExpedienteId && creation.Expediente?.RelacionNivelId === identity.RelacionNivelId, 'CYCLE_CHANGED');
      const exp = one(await rows('RA', 'SELECT e.*'+(options.expectedDates?',CAST(e.FECHA_EXTREMA_INICIAL AS CHAR) AS doc117_fecha_inicial':'')+',r.ra_pro_niveles_id_nivel AS nivel FROM expediente_archivo e INNER JOIN ra_pro_niveles_has_expediente_archivo r ON r.expediente_archivo_ID_EXPEDIENTE=e.ID_EXPEDIENTE WHERE e.ID_EXPEDIENTE=? AND r.id_registro=?', [identity.ExpedienteId, identity.RelacionNivelId]), 'DESTINATION_NOT_VERIFIED');
      check(String(exp.nivel) === profile.nivelRef && String(exp.ID_USUARIO_GESTION) === ctx.UsuarioRef && String(exp.ID_EMPRESA_EXPEDIENTE) === ctx.AmbitoRef && exp.CODIGO_UNICO === expected.codigo && exp.ALEAS_EXPEDIENTE === expected.nombre && exp.TEMA_EXPEDIENTE === expected.nombre && String(exp.Id_tipos_ciclo_archivo) === String(cycle.id) && exp.NOMBRE_CICLO_ARCHIVO === 'Archivo Gestión', 'EXPEDITION_VALUES_INVALID');
      if(options.expectedDates)check(exp.doc117_fecha_inicial===options.expectedDates[index],'EXPEDITION_DATE_INVALID');
      for (const [column, key] of [['ID_FONDO','FondoRef'],['id_instrumento','InstrumentoRef'],['CODIGO_AREA_TRD','AreaRef'],['CODIGO_SERIE_TRD','SerieRef'],['CODIGO_SUB_SERIE_TRD','SubserieRef'],['RA_TIP_EXPE_ID_TIPO_EXPEDIENTE','TipoExpedienteRef'],['ID_TIPO_UNIDAD_DOCUMENTAL','UnidadDocumentalRef']]) check((exp[column] == null ? null : String(exp[column])) === (c[key] ?? null), 'CLASSIFICATION_INVALID');
      const file = one(view.files.filter(f => String(f.raiz_id) === String(root.raiz_id) && f.cliente_archivo_id === 'a' + (index + 1)), 'FILE_CORRELATION_INVALID'), result = json(file.resultado_doc), values = json(file.valores_efectivos_doc);
      check(file.estado_efecto === 'COMPLETADO' && file.estado_recepcion === 'RECIBIDO' && !file.intento_activo_id && result?.Estado === 'CONFIRMADO' && result.Expediente.ExpedienteId === identity.ExpedienteId && result.Expediente.RelacionNivelId === identity.RelacionNivelId && String(result.GabineteId) === String(c.GabineteId) && result.OrigenGabinete === 'DA' && id(result.DocumentoId) && id(result.InventarioId) && !documentIds.has(result.DocumentoId), 'DOCUMENT_PROOF_INVALID'); documentIds.add(result.DocumentoId);
      const documentProof = one(view.attempts.filter(a => String(a.archivo_id) === String(file.archivo_id) && a.fase === 'ALMACENAR' && a.estado === 'CONFIRMADO' && a.fin_utc), 'DOCUMENT_PROOF_INVALID');
      check(JSON.stringify(json(documentProof.evidencia_doc)) === JSON.stringify(result) && JSON.stringify(result.ValoresEfectivos) === JSON.stringify(values), 'DOCUMENT_PROOF_INVALID');
      assertEffective(values, expected);
      const document = await readKnownDocumentBody(c,ctx,identity,expected,fileNames[index],result,cabinet,documentClass,extension);
      physical.push(exp, document);
    }
    if (profile.mode === 'PARCIAL') check(view.files.some(f => f.cliente_archivo_id === 'bad' && (f.estado_efecto === 'RECHAZADO' || f.estado_recepcion === 'RECHAZADO')), 'REJECTION_LOST');
    return {fingerprint: crypto.createHash('sha256').update(JSON.stringify({view,physical})).digest('hex'), roots: rootCount, documents: rootCount};
  }
  // Physical check by known IDs. Does not project a final operational state.
  async function verifyKnownDocument(view,rootId,result,expected,fileName) {
    check(view?.lot&&id(String(rootId))&&id(result?.DocumentoId)&&id(result?.InventarioId)&&typeof fileName==='string'&&fileName.length>0&&!/[\\/\x00-\x1f]/.test(fileName),'CONTROL_SCOPE_INVALID');
    const config=json(view.lot.config_doc).Configuracion,ctx=json(view.lot.contexto_doc),root=one(view.roots.filter(r=>String(r.raiz_id)===String(rootId)),'ROOT_CORRELATION_INVALID'),identity=json(root.identidad_expediente_doc);
    check(id(identity?.ExpedienteId)&&id(identity?.RelacionNivelId)&&identity.OrigenLogico==='RA'&&identity.NivelRef===ctx.NivelRef,'ROOT_IDENTITY_INVALID');
    const cabinet=one(await rows('DA','SELECT NOMBRE FROM system1 WHERE id_gabinete=?',[config.GabineteId]),'CABINET_AMBIGUOUS');
    const cycle=one(await rows('RA','SELECT Id_tipos_ciclo_archivo AS id,Nombre_Tipo_ciclo_archivo AS nombre FROM ra_de_tipos_ciclos_archivo WHERE Nombre_Tipo_ciclo_archivo=?',['Archivo Gestión']),'CYCLE_AMBIGUOUS');
    const creation=json(one(view.attempts.filter(a=>String(a.raiz_id)===String(rootId)&&a.fase==='CREAR_EXPEDIENTE'&&a.estado==='CONFIRMADO'&&a.fin_utc),'CREATION_PROOF_INVALID').evidencia_doc);
    check(cycle.nombre==='Archivo Gestión'&&creation?.Creacion?.CicloArchivoId===String(cycle.id)&&creation.Expediente?.ExpedienteId===identity.ExpedienteId&&creation.Expediente.RelacionNivelId===identity.RelacionNivelId,'CREATION_PROOF_INVALID');
    const exp=one(await rows('RA','SELECT e.*,r.ra_pro_niveles_id_nivel AS nivel FROM expediente_archivo e INNER JOIN ra_pro_niveles_has_expediente_archivo r ON r.expediente_archivo_ID_EXPEDIENTE=e.ID_EXPEDIENTE WHERE e.ID_EXPEDIENTE=? AND r.id_registro=?',[identity.ExpedienteId,identity.RelacionNivelId]),'DESTINATION_NOT_VERIFIED');
    check(String(exp.nivel)===ctx.NivelRef&&String(exp.ID_USUARIO_GESTION)===ctx.UsuarioRef&&String(exp.ID_EMPRESA_EXPEDIENTE)===ctx.AmbitoRef&&exp.CODIGO_UNICO===expected.codigo&&exp.ALEAS_EXPEDIENTE===expected.nombre&&exp.TEMA_EXPEDIENTE===expected.nombre&&String(exp.Id_tipos_ciclo_archivo)===String(cycle.id)&&exp.NOMBRE_CICLO_ARCHIVO==='Archivo Gestión','EXPEDITION_VALUES_INVALID');
    for(const [column,key]of [['ID_FONDO','FondoRef'],['id_instrumento','InstrumentoRef'],['CODIGO_AREA_TRD','AreaRef'],['CODIGO_SERIE_TRD','SerieRef'],['CODIGO_SUB_SERIE_TRD','SubserieRef'],['RA_TIP_EXPE_ID_TIPO_EXPEDIENTE','TipoExpedienteRef'],['ID_TIPO_UNIDAD_DOCUMENTAL','UnidadDocumentalRef']])check((exp[column]==null?null:String(exp[column]))===(config[key]??null),'CLASSIFICATION_INVALID');
    const documentClass=one(await rows('RA','SELECT ID_TIPO_DOCUMENTO AS id FROM ra_tipo_documento WHERE DOCUMENTO=?',['DOCUMENTO ELECTRONICO']),'CLASS_AMBIGUOUS');
    const extension=one(await rows('DA','SELECT ESTADO_NORMAL AS id FROM DA_EXTENSION WHERE ESTENSION=?',['.PDF']),'FORMAT_AMBIGUOUS');
    const document=await readKnownDocumentBody(config,ctx,identity,expected,fileName,result,cabinet,documentClass,extension);
    for(const [origin,sql]of [['DA','SELECT COUNT(*) AS total FROM '+identifier(cabinet.NOMBRE)+' WHERE ID_EXPEDIENTE=?'],['RA','SELECT COUNT(*) AS total FROM registro_producion_documental WHERE EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=?'],['RA','SELECT COUNT(*) AS total FROM ra_cert_indice_expediente WHERE expediente_archivo_ID_EXPEDIENTE=?']])check(String(one(await rows(origin,sql,[identity.ExpedienteId]),'DOCUMENT_CONTROL_INVALID').total)==='1','DOCUMENT_UNIQUENESS_INVALID');
    return{fingerprint:crypto.createHash('sha256').update(JSON.stringify({exp,document})).digest('hex'),documents:1,knownIdsVerified:true};
  }
  async function assertNoDocuments(view) {
    const config = json(view.lot.config_doc).Configuracion;
    const cabinet = one(await rows('DA', 'SELECT NOMBRE FROM system1 WHERE id_gabinete=?', [config.GabineteId]), 'CABINET_AMBIGUOUS');
    for (const root of view.roots) {
      const identity = json(root.identidad_expediente_doc);
      check(id(identity?.ExpedienteId), 'ROOT_IDENTITY_INVALID');
      for (const [origin,sql] of [
        ['DA','SELECT COUNT(*) AS total FROM '+identifier(cabinet.NOMBRE)+' WHERE ID_EXPEDIENTE=?'],
        ['RA','SELECT COUNT(*) AS total FROM registro_producion_documental WHERE EXPEDIENTE_ARCHIVO_ID_EXPEDIENTE=?'],
        ['RA','SELECT COUNT(*) AS total FROM ra_cert_indice_expediente WHERE expediente_archivo_ID_EXPEDIENTE=?']
      ]) check(String(one(await rows(origin,sql,[identity.ExpedienteId]),'DOCUMENT_CONTROL_INVALID').total)==='0','DOCUMENT_ALREADY_PRESENT');
    }
  }
  return {snapshot, verify, assertNoDocuments, verifyKnownDocument};
}

// Recuperacion explicita de un lote existente. Nunca prepara/confirma/recibe/crea.
async function runResume(driver, controls, profile, bytes, loteId, intentoId, onReference = () => {}) {
  check(profile.mode==='LOTE'&&id(loteId)&&id(intentoId),'RESUME_SCOPE_INVALID');
  await driver.refreshToken();
  const state = await driver.request('ConsultarLote',{loteId});
  const before = await controls.snapshot(loteId), config=json(before.lot.config_doc), ctx=json(before.lot.contexto_doc);
  check(state.loteId===loteId&&state.nivelRef===profile.nivelRef&&state.estado==='EN_CURSO'&&before.lot.estado==='EN_CURSO'&&String(before.lot.detencion_solicitada)==='0'&&String(state.revision)===String(before.lot.revision),'RESUME_STATE_INVALID');
  check(config.VersionId===profile.versionId&&config.Configuracion.ModoIntegridad==='COMPLETO_OBLIGATORIO'&&config.Configuracion.AmbitoIntegridad==='LOTE'&&ctx.NivelRef===profile.nivelRef,'RESUME_SCOPE_INVALID');
  check(before.roots.length===2&&before.files.length===2&&String(before.lot.total_raices)==='2'&&String(before.lot.total_archivos)==='2'&&String(before.lot.total_bytes)===String(bytes.length*2),'ORIGINAL_TOTALS_CHANGED');
  const reserved=one(before.attempts.filter(a=>String(a.intento_id)===intentoId),'RESUME_ATTEMPT_INVALID');
  check(reserved.estado==='RESERVADO'&&reserved.fase==='ALMACENAR'&&!reserved.evidencia_doc&&!reserved.fin_utc,'RESUME_ATTEMPT_INVALID');
  const journal=before.events.filter(e=>String(e.intento_id)===intentoId);
  check(journal.length===1&&journal[0].accion==='RESERVAR_INTENTO'&&journal[0].codigo==='INTENTO_RESERVADO','RESUME_ATTEMPT_INVALID');
  const expeditionIds=new Set();
  for(let i=0;i<2;i++){
    const root=one(before.roots.filter(r=>r.cliente_raiz_id==='r'+(i+1)),'ROOT_CORRELATION_INVALID'), identity=json(root.identidad_expediente_doc), data=json(root.datos_doc);
    check(root.estado==='EXPEDIENTE_CONFIRMADO'&&!root.intento_activo_id&&data.NombreOriginal===profile.rootNames[i]&&identity?.NivelRef===profile.nivelRef&&id(identity.ExpedienteId)&&id(identity.RelacionNivelId)&&!expeditionIds.has(identity.ExpedienteId),'ROOT_IDENTITY_INVALID');
    expeditionIds.add(identity.ExpedienteId);
    const file=one(before.files.filter(f=>String(f.raiz_id)===String(root.raiz_id)&&f.cliente_archivo_id==='a'+(i+1)),'FILE_CORRELATION_INVALID');
    const isReserved=String(file.archivo_id)===String(reserved.archivo_id);
    check(file.estado_recepcion==='RECIBIDO'&&!file.resultado_doc&&String(file.bytes_previstos)===String(bytes.length)&&file.estado_efecto===(isReserved?'EN_CURSO':'NO_INICIADO')&&(isReserved?String(file.intento_activo_id)===intentoId:!file.intento_activo_id),'RESUME_FILE_INVALID');
    if(file.valores_efectivos_doc)assertEffective(json(file.valores_efectivos_doc),profile.expectations[i]);
  }
  check(before.attempts.filter(a=>a.fase==='ALMACENAR').length===1&&before.attempts.filter(a=>a.fase==='CREAR_EXPEDIENTE'&&a.estado==='CONFIRMADO'&&a.fin_utc).length===2,'RESUME_ATTEMPT_INVALID');
  await controls.assertNoDocuments(before);
  const closure={loteId,intentoId,revisionEsperada:state.revision};
  const closed=await driver.request('CerrarReservaSinInicio',closure);
  check(closed.estado==='CONFIRMADO'&&closed.codigo==='RESERVA_CERRADA_SIN_INICIO'&&closed.intentoId===intentoId&&closed.resuelta===true&&!closed.documentoId&&!closed.inventarioId,'RESUME_CLOSE_NOT_CONFIRMED');
  onReference({loteId,intentoId,closedWithoutStart:true});
  const afterClose=await controls.snapshot(loteId);
  const closedAttempt=one(afterClose.attempts.filter(a=>String(a.intento_id)===intentoId),'RESUME_CLOSE_NOT_CONFIRMED');
  check(closedAttempt.estado==='RECHAZADO'&&closedAttempt.fin_utc&&json(closedAttempt.evidencia_doc)?.Codigo==='RESERVA_CERRADA_SIN_INICIO'&&JSON.stringify(afterClose.roots)===JSON.stringify(before.roots),'RESUME_CLOSE_NOT_CONFIRMED');
  check((await driver.request('CerrarReservaSinInicio',closure)).repeticion===true,'RESUME_CLOSE_REPLAY_INVALID');
  check(JSON.stringify(await controls.snapshot(loteId))===JSON.stringify(afterClose),'RESUME_CLOSE_REPLAY_CHANGED');
  return finishPendingDocuments(driver,controls,profile,loteId,intentoId,{closedWithoutStart:true});
}
async function runReconciliationResume(driver,controls,profile,bytes,loteId,intentoId){
  check(profile.mode==='LOTE'&&id(loteId)&&id(intentoId),'RESUME_SCOPE_INVALID');
  await driver.refreshToken();
  const state=await driver.request('ConsultarLote',{loteId}),before=await controls.snapshot(loteId),config=json(before.lot.config_doc),ctx=json(before.lot.contexto_doc);
  check(state.loteId===loteId&&state.nivelRef===profile.nivelRef&&state.estado==='PENDIENTE_VERIFICACION'&&before.lot.estado===state.estado&&String(state.revision)===String(before.lot.revision)&&String(before.lot.detencion_solicitada)==='0','RESUME_STATE_INVALID');
  check(config.VersionId===profile.versionId&&config.Configuracion.ModoIntegridad==='COMPLETO_OBLIGATORIO'&&config.Configuracion.AmbitoIntegridad==='LOTE'&&ctx.NivelRef===profile.nivelRef,'RESUME_SCOPE_INVALID');
  check(before.roots.length===2&&before.files.length===2&&String(before.lot.total_raices)==='2'&&String(before.lot.total_archivos)==='2'&&String(before.lot.total_bytes)===String(bytes.length*2),'ORIGINAL_TOTALS_CHANGED');
  const attempt=one(before.attempts.filter(a=>String(a.intento_id)===intentoId),'RESUME_ATTEMPT_INVALID'),proof=json(attempt.evidencia_doc);
  check(attempt.fase==='ALMACENAR'&&attempt.estado==='INCIERTO'&&!attempt.fin_utc&&proof?.Estado==='INCIERTO'&&id(proof.DocumentoId)&&id(proof.InventarioId),'RESUME_ATTEMPT_INVALID');
  check(before.attempts.every(a=>String(a.intento_id)===intentoId||['CONFIRMADO','RECHAZADO'].includes(a.estado)),'RESUME_ATTEMPT_INVALID');
  const identities=[];
  for(let i=0;i<2;i++){
    const root=one(before.roots.filter(r=>r.cliente_raiz_id==='r'+(i+1)),'ROOT_CORRELATION_INVALID'),identity=json(root.identidad_expediente_doc),data=json(root.datos_doc);
    check(root.estado===(i===0?'INCIERTO':'EXPEDIENTE_CONFIRMADO')&&!root.intento_activo_id&&data.NombreOriginal===profile.rootNames[i]&&identity?.NivelRef===profile.nivelRef&&id(identity.ExpedienteId)&&id(identity.RelacionNivelId),'ROOT_IDENTITY_INVALID');identities.push(root.identidad_expediente_doc);
    const file=one(before.files.filter(f=>String(f.raiz_id)===String(root.raiz_id)&&f.cliente_archivo_id==='a'+(i+1)),'FILE_CORRELATION_INVALID');
    check(file.estado_recepcion==='RECIBIDO'&&String(file.bytes_previstos)===String(bytes.length)&&file.estado_efecto===(i===0?'INCIERTO':'NO_INICIADO'),'RESUME_FILE_INVALID');
    if(i===0){check(String(file.archivo_id)===String(attempt.archivo_id)&&String(file.raiz_id)===String(attempt.raiz_id)&&String(file.intento_activo_id)===intentoId&&JSON.stringify(json(file.resultado_doc))===JSON.stringify(proof)&&JSON.stringify(json(file.valores_efectivos_doc))===JSON.stringify(proof.ValoresEfectivos)&&proof.Expediente?.ExpedienteId===identity.ExpedienteId&&proof.Expediente?.RelacionNivelId===identity.RelacionNivelId,'DOCUMENT_PROOF_INVALID');assertEffective(proof.ValoresEfectivos,profile.expectations[i]);}
    else check(!file.resultado_doc&&!file.intento_activo_id,'RESUME_FILE_INVALID');
  }
  check(identities[0]!==identities[1],'ROOT_IDENTITY_INVALID');
  const request={loteId,intentoId,revisionEsperada:state.revision};
  const result=await driver.request('ReconciliarRecurso',request);
  check(result.estado==='CONFIRMADO'&&result.resuelta===true&&result.intentoId===intentoId&&result.documentoId===proof.DocumentoId&&result.inventarioId===proof.InventarioId,'RECONCILIATION_NOT_CONFIRMED');
  const reconciled=await controls.snapshot(loteId);
  check(reconciled.roots.every(r=>r.identidad_expediente_doc===one(before.roots.filter(old=>String(old.raiz_id)===String(r.raiz_id)),'ROOT_CORRELATION_INVALID').identidad_expediente_doc),'ROOT_IDENTITY_CHANGED');
  const saved=one(reconciled.files.filter(f=>String(f.archivo_id)===String(attempt.archivo_id)),'FILE_CORRELATION_INVALID'),savedProof=json(saved.resultado_doc);
  check(saved.estado_efecto==='COMPLETADO'&&!saved.intento_activo_id&&savedProof?.Estado==='CONFIRMADO'&&savedProof.DocumentoId===proof.DocumentoId&&savedProof.InventarioId===proof.InventarioId,'RECONCILIATION_NOT_CONFIRMED');
  const replay=await driver.request('ReconciliarRecurso',request);
  check(replay.repeticion===true&&replay.resuelta===true&&replay.documentoId===proof.DocumentoId&&replay.inventarioId===proof.InventarioId&&JSON.stringify(await controls.snapshot(loteId))===JSON.stringify(reconciled),'RECONCILIATION_REPLAY_CHANGED');
  const completed=await finishPendingDocuments(driver,controls,profile,loteId,intentoId,{existingDocumentReconciled:true},saved.archivo_id);
  const final=await controls.snapshot(loteId),first=one(final.files.filter(f=>String(f.archivo_id)===String(saved.archivo_id)),'FILE_CORRELATION_INVALID'),finalProof=json(first.resultado_doc);
  check(finalProof?.DocumentoId===proof.DocumentoId&&finalProof?.InventarioId===proof.InventarioId&&final.roots.every(r=>r.identidad_expediente_doc===one(before.roots.filter(old=>String(old.raiz_id)===String(r.raiz_id)),'ROOT_CORRELATION_INVALID').identidad_expediente_doc),'EXISTING_IDENTITIES_CHANGED');
  return completed;
}
async function finishPendingDocuments(driver,controls,profile,loteId,intentoId,flags,completedFileId=null){
  const commands=[];
  for(let i=0;i<2;i++){
    let row=one((await driver.pages(loteId,'ARCHIVOS')).filter(r=>r.idCliente==='a'+(i+1)),'FILE_CORRELATION_INVALID');
    let file=one((await controls.snapshot(loteId)).files.filter(f=>String(f.archivo_id)===row.recursoId),'FILE_CORRELATION_INVALID');
    if(completedFileId!==null&&String(file.archivo_id)===String(completedFileId)){
      check(file.estado_efecto==='COMPLETADO'&&!file.intento_activo_id&&json(file.resultado_doc)?.Estado==='CONFIRMADO','DOCUMENT_PROOF_INVALID');continue;
    }
    if(!file.valores_efectivos_doc){
      const materialized=await driver.request('AvanzarLote',{loteId,recursoId:row.recursoId,fase:'MATERIALIZAR_VALORES',revisionEsperada:row.revision,claveOperacion:crypto.randomBytes(16).toString('hex')});
      check(materialized.estado==='CONFIRMADO','PHASE_NOT_CONFIRMED');
      row=one((await driver.pages(loteId,'ARCHIVOS')).filter(r=>r.idCliente==='a'+(i+1)),'FILE_CORRELATION_INVALID');
      file=one((await controls.snapshot(loteId)).files.filter(f=>String(f.archivo_id)===row.recursoId),'FILE_CORRELATION_INVALID');
    }
    assertEffective(json(file.valores_efectivos_doc),profile.expectations[i]);
    check(file.estado_efecto==='NO_INICIADO'&&!file.intento_activo_id&&!file.resultado_doc,'RESUME_FILE_INVALID');
    const request={loteId,recursoId:row.recursoId,fase:'ALMACENAR',revisionEsperada:row.revision,claveOperacion:crypto.randomBytes(16).toString('hex')};
    const result=await driver.request('AvanzarLote',request);
    check(result.estado==='CONFIRMADO'&&id(result.documentoId)&&id(result.inventarioId),'PHASE_NOT_CONFIRMED');commands.push({request,result});
  }
  const verified=await controls.verify(loteId,profile);
  const signature=r=>JSON.stringify(['loteId','raizId','archivoId','intentoId','fase','expedienteId','relacionNivelId','gabineteId','documentoId','inventarioId'].map(k=>r[k]??null));
  for(const entry of commands)check(signature(await driver.request('AvanzarLote',entry.request))===signature(entry.result),'REPLAY_NOT_CONFIRMED');
  await driver.reopen(loteId);
  check((await controls.verify(loteId,profile)).fingerprint===verified.fingerprint,'REPLAY_OR_REOPEN_CHANGED_DATA');
  return {mode:profile.mode,loteId,intentoId,...flags,existingExpeditionsPreserved:true,idsAndIndicesVerified:true,replayAndReopenUnchanged:true,roots:2,documents:2};
}
async function runCompletedVerification(driver,controls,profile,bytes,loteId){
  check(profile.mode==='LOTE'&&id(loteId),'RESUME_SCOPE_INVALID');
  await driver.refreshToken();
  const state=await driver.request('ConsultarLote',{loteId});
  check(state.loteId===loteId&&state.nivelRef===profile.nivelRef&&state.estado==='COMPLETADO'&&state.totalRaices===2&&state.totalArchivos===2&&state.totalBytes===String(bytes.length*2),'FINAL_STATE_INVALID');
  const before=await controls.verify(loteId,profile);
  await driver.reopen(loteId);
  check((await driver.request('ConsultarLote',{loteId})).estado==='COMPLETADO','FINAL_STATE_INVALID');
  const after=await controls.verify(loteId,profile);
  check(before.fingerprint===after.fingerprint,'REOPEN_CHANGED_DATA');
  return {mode:profile.mode,loteId,roots:after.roots,documents:after.documents,completedVerified:true,reopenUnchanged:true};
}
async function runRec(driver, controls, profile, bytes, onReference = () => {}) {
  const commands = []; await driver.refreshToken();
  const opened = await driver.request('AbrirImportacion', {nivelRef: profile.nivelRef});
  check(opened.nivelRef === profile.nivelRef, 'DESTINATION_CHANGED');
  const mode = profile.mode === 'PARCIAL' ? 'PARCIAL_PERMITIDO' : 'COMPLETO_OBLIGATORIO', scope = profile.mode === 'PARCIAL' ? 'NO_APLICA' : profile.mode;
  const roots = profile.rootNames.map((nombre,i) => ({seleccionId:'s'+(i+1),idCliente:'r'+(i+1),nombre}));
  const files = roots.map((r,i) => ({idCliente:'a'+(i+1),raizIdCliente:r.idCliente,nombre:FILE_NAME,rutaRelativa:r.nombre+'/'+FILE_NAME,bytes:String(bytes.length),ultimaModificacionLocal:'0'}));
  if (profile.mode === 'PARCIAL') files.push({...files[1],idCliente:'bad',nombre:'rechazado.tif',rutaRelativa:roots[1].nombre+'/rechazado.tif'});
  const plan = await driver.request('PrepararPlan', {nivelRef:profile.nivelRef,versionId:profile.versionId,raices:roots,archivos:files});
  check(id(plan.planId) && plan.nivelRef === profile.nivelRef && plan.snapshot?.VersionId === profile.versionId && plan.snapshot.Configuracion.ModoIntegridad === mode && plan.snapshot.Configuracion.AmbitoIntegridad === scope && plan.resultado?.PuedeConfirmar === true && plan.resultado.UnidadesElegibles.length === 2, 'PREFLIGHT_NOT_ELIGIBLE');
  for (let i=0;i<2;i++) {
    const values = one(plan.resultado.Elementos.filter(e => e.Ambito === 'RAIZ' && e.RaizIdCliente === roots[i].idCliente), 'PLAN_CORRELATION_INVALID').Valores;
    for (const [field,key] of [['CODIGO','codigo'],['NOMBRE','nombre']]) check(one(values.filter(v=>v.DestinoRef===field),'PLAN_VALUES_INVALID').Valor === profile.expectations[i][key], 'PLAN_VALUES_INVALID');
  }
  const confirmation = {planId:plan.planId,revisionEsperada:plan.revision,planHashEsperado:plan.planHash,claveConfirmacion:crypto.randomBytes(16).toString('hex')};
  onReference({planId:plan.planId});
  const lot = await driver.request('ConfirmarPlan',confirmation); check(id(lot.loteId) && lot.nivelRef === profile.nivelRef, 'CONFIRMATION_INVALID');
  onReference({planId:plan.planId,loteId:lot.loteId});
  check((await driver.request('ConfirmarPlan',confirmation)).loteId === lot.loteId,'CONFIRMATION_REPLAY_CHANGED');
  let rootRows = await driver.pages(lot.loteId,'RAICES'), fileRows = await driver.pages(lot.loteId,'ARCHIVOS');
  check(rootRows.length===2 && fileRows.length===files.length,'ORIGINAL_TOTALS_CHANGED');
  const rootFor = i => one(rootRows.filter(r=>r.idCliente===roots[i].idCliente),'ROOT_CORRELATION_INVALID');
  const fileFor = i => one(fileRows.filter(r=>r.idCliente===files[i].idCliente),'FILE_CORRELATION_INVALID');
  function command(fase,row){return {loteId:lot.loteId,recursoId:row.recursoId,fase,revisionEsperada:row.revision,claveOperacion:crypto.randomBytes(16).toString('hex')};}
  await driver.receive(lot.loteId,fileFor(0),bytes);
  const beforeBarrier = await controls.snapshot(lot.loteId);
  const blockedRoot = rootFor(profile.mode==='LOTE'?0:1);
  await driver.request('AvanzarLote',command('CREAR_EXPEDIENTE',blockedRoot),true);
  check(JSON.stringify(await controls.snapshot(lot.loteId))===JSON.stringify(beforeBarrier),'BARRIER_CHANGED_DATA');
  async function create(i){rootRows=await driver.pages(lot.loteId,'RAICES');const cmd=command('CREAR_EXPEDIENTE',rootFor(i));const r=await driver.request('AvanzarLote',cmd);check(r.estado==='CONFIRMADO'&&r.loteId===lot.loteId&&r.raizId===cmd.recursoId&&r.fase===cmd.fase&&id(r.expedienteId)&&id(r.relacionNivelId),'CREATION_NOT_CONFIRMED');commands.push({request:cmd,result:r});}
  if(profile.mode!=='LOTE')await create(0);
  await driver.receive(lot.loteId,fileFor(1),bytes);
  if(profile.mode==='LOTE')await create(0);await create(1);
  for(let i=0;i<2;i++)for(const phase of ['MATERIALIZAR_VALORES','ALMACENAR']){
    fileRows=await driver.pages(lot.loteId,'ARCHIVOS');const cmd=command(phase,fileFor(i));const r=await driver.request('AvanzarLote',cmd);check(r.estado==='CONFIRMADO'&&r.loteId===lot.loteId&&r.archivoId===cmd.recursoId&&r.fase===cmd.fase,'PHASE_NOT_CONFIRMED');commands.push({request:cmd,result:r});
    if(phase==='MATERIALIZAR_VALORES'){
      const fixed=one((await controls.snapshot(lot.loteId)).files.filter(f=>String(f.archivo_id)===cmd.recursoId),'FILE_CORRELATION_INVALID');
      assertEffective(json(fixed.valores_efectivos_doc),profile.expectations[i]);
    }
  }
  const final = await driver.request('ConsultarLote',{loteId:lot.loteId});check(final.estado===(profile.mode==='PARCIAL'?'PARCIAL':'COMPLETADO')&&final.totalRaices===2&&final.totalArchivos===files.length&&final.totalBytes===String(bytes.length*files.length),'FINAL_STATE_INVALID');
  const before = await controls.verify(lot.loteId,profile);
  const identitySignature = r=>JSON.stringify(['loteId','raizId','archivoId','intentoId','fase','expedienteId','relacionNivelId','gabineteId','documentoId','inventarioId'].map(k=>r[k]??null));
  for(const entry of commands){const r=await driver.request('AvanzarLote',entry.request);check(r.estado==='CONFIRMADO'&&identitySignature(r)===identitySignature(entry.result),'REPLAY_NOT_CONFIRMED');}
  await driver.reopen(lot.loteId);
  const after = await controls.verify(lot.loteId,profile);check(before.fingerprint===after.fingerprint,'REPLAY_OR_REOPEN_CHANGED_DATA');
  return {mode:profile.mode,rec01:true,rec02:true,barrierPreserved:true,idsAndIndicesVerified:true,replayAndReopenUnchanged:true,roots:after.roots,documents:after.documents,originalFiles:files.length};
}
module.exports={MODES,FILE_NAME,validateProfile,readFixture,assertAuthorizations,classifyLoginMessage,classifyLoginPostback,loginObservation,openSession,createDriver,createSelectControls,runRec,runResume,runReconciliationResume,runCompletedVerification};
