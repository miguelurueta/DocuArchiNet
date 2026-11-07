'use strict';
const {test,expect}=require('@playwright/test');
const fs=require('node:fs');
const crypto=require('node:crypto');
const mysql=require('mysql2/promise');
const {createWorkflowLoginPageSession}=require('./support/authenticated-workflow-session.cjs');
const {safePost}=require('./support/doc110-safe-request.cjs');

// Usa runner, navegador, autenticación y reportero existentes. Sin rastros de login/tokens.
test.use({screenshot:'off',trace:'off',video:'off'});
test.skip(process.env.DOC110_E2E_AUTHORIZED!=='YES','NO_EJECUTADO: requiere autorización específica de ambiente, cuentas y datos descartables.');
function required(name) {
  if(!process.env[name])throw new Error('Falta configuración de pruebas: '+name);
  return process.env[name];
}
function fixture() {
  const result=JSON.parse(fs.readFileSync(required('DOC110_E2E_FIXTURE_PATH'),'utf8'));
  if(!result.configuracion || !Array.isArray(result.reglas) || !Array.isArray(result.ejemplos) || result.ejemplos.length<2)
    throw new Error('El fixture autorizado debe contener configuración, reglas y dos ejemplos con resultados esperados.');
  return result;
}
async function session(browser,restricted=false) {
  const baseUrl=required('DOC110_E2E_BASE_URL').replace(/\/?$/,'/');
  const auth=await createWorkflowLoginPageSession(browser,{
    baseUrl,moduleEnvironmentVariable:'DOC110_E2E_MODULE',
    userEnvironmentVariable:restricted?'DOC110_E2E_RESTRICTED_USER':'DOC110_E2E_USER',
    passwordEnvironmentVariable:restricted?'DOC110_E2E_RESTRICTED_PASSWORD':'DOC110_E2E_PASSWORD',
    ignoreHTTPSErrors:process.env.DOC10_E2E_IGNORE_HTTPS_ERRORS==='true'
  });
  try {
    await auth.page.goto(new URL('Gestion/WebFormProducionDocumental.aspx',baseUrl).href);
    return {...auth,baseUrl};
  } catch (_) {await auth.context.close();throw new Error('No se pudo abrir Producción con la sesión de prueba.');}
}
async function create(browser) {
  const auth=await session(browser);
  try {
    await expect(auth.page.locator('[data-doc110=abrir]')).toBeVisible();
    const first=auth.page.waitForRequest(r=>r.url().endsWith('/ListarPerfiles') && r.method()==='POST');
    await auth.page.locator('[data-doc110=abrir]').click();
    // Solo memoria de la prueba; nunca adjuntar cabeceras, cookies o trazas.
    auth.token=(await first).headers()['x-importacion-perfil-csrf'];
    await auth.page.locator('[data-doc110=nuevo]').click();
    const code='E2E-'+crypto.randomBytes(12).toString('hex');
    await auth.page.locator('[data-doc110=codigo]').fill(code);
    await auth.page.locator('[data-doc110=nombre]').fill('Perfil sintético DOC-110');
    const created=auth.page.waitForResponse(r=>r.url().endsWith('/CrearPerfil'));
    await auth.page.locator('[data-doc110=crear]').click();
    const body=await (await created).json();
    if(!body.d?.ok)throw new Error('La creación sintética no fue confirmada.');
    return {...auth,profile:body.d.datos};
  }catch(error){await auth.context.close();throw error;}
}
async function call(auth,method,payload) {
  const response=await safePost(auth.context.request,new URL('webservice/WebServiceImportacionCarpetas.asmx/'+method,auth.baseUrl).href,{
    headers:{'X-Importacion-Perfil-CSRF':auth.token},data:{solicitud:payload}
  },method);
  let body;try{body=await response.json();}catch(_){throw new Error('El endpoint del perfil no devolvió JSON.');}
  return {status:response.status(),body:body.d};
}
async function save(auth,profile,config,rules) {
  const result=await call(auth,'GuardarBorrador',{perfilId:profile.PerfilId,versionId:profile.Version.VersionId,
    revisionVersionEsperada:profile.Version.Revision,configuracion:config,reglas:rules});
  expect(result.status).toBe(200);expect(result.body.ok).toBe(true);return result.body.datos;
}
async function publish(auth,profile) {
  return call(auth,'PublicarVersion',{perfilId:profile.PerfilId,versionId:profile.Version.VersionId,
    revisionPerfilEsperada:profile.Revision,revisionVersionEsperada:profile.Version.Revision});
}
async function snapshot(versionId) {
  let connection;
  try {
    connection=await mysql.createConnection(required('DOC110_E2E_MYSQL_URL'));
    const [rows]=await connection.execute(
      'SELECT version_id,estado,revision,modo_integridad,ambito_integridad,configuracion_doc,HEX(config_hash) AS huella FROM ra_integracion_perfil_version WHERE version_id=?',[versionId]);
    if(rows.length!==1)throw new Error();
    const [rules]=await connection.execute(
      'SELECT destino_tipo,destino_ref,regla_doc FROM ra_integracion_perfil_regla WHERE version_id=? ORDER BY destino_tipo,destino_ref',[versionId]);
    return {version:rows[0],rules};
  } catch(_){throw new Error('No se pudo comprobar el snapshot mediante la conexión SELECT autorizada.');}
  finally{if(connection)await connection.end();}
}
async function reloadUI(auth) {
  await auth.page.locator('[data-doc110=recargar]').click();
  await expect(auth.page.locator('[data-doc110=estado]')).toContainText('Operación completada');
}
test('PER-01: ámbito obligatorio, publicación EXPEDIENTE y recarga persistida',async({browser})=>{
  const f=fixture(),auth=await create(browser);
  try {
    let p=await save(auth,auth.profile,{...f.configuracion,ModoIntegridad:'COMPLETO_OBLIGATORIO',AmbitoIntegridad:null},f.reglas);
    const rejected=await publish(auth,p);
    expect(rejected.status).toBe(400);expect(rejected.body.incidencias.some(x=>x.Campo==='AmbitoIntegridad')).toBe(true);
    p=await save(auth,p,{...p.Version.Configuracion,AmbitoIntegridad:'EXPEDIENTE'},p.Version.Reglas);
    await reloadUI(auth);
    const published=auth.page.waitForResponse(r=>r.url().endsWith('/PublicarVersion'));
    await auth.page.locator('[data-doc110=publicar]').click();
    expect((await (await published).json()).d.ok).toBe(true);
    await reloadUI(auth);
    await expect(auth.page.locator('[data-doc110=guardar]')).toHaveCount(0);
    const persisted=await snapshot(p.Version.VersionId);
    expect(persisted.version.estado).toBe('PUBLICADA');expect(persisted.version.ambito_integridad).toBe('EXPEDIENTE');
    expect(persisted.rules.length).toBe(f.reglas.length);
  }finally{auth.token=null;await auth.context.close();}
});
test('PER-02: segunda versión LOTE, historial inmutable y cuenta restringida',async({browser})=>{
  const f=fixture(),auth=await create(browser);let denied;
  try {
    let p=await save(auth,auth.profile,{...f.configuracion,AmbitoIntegridad:'EXPEDIENTE'},f.reglas);
    const publication=await publish(auth,p);expect(publication.body.ok).toBe(true);p=publication.body.datos;
    const before=await snapshot(p.Version.VersionId),firstId=p.Version.VersionId;
    const next=await call(auth,'CrearVersion',{perfilId:p.PerfilId,versionOrigenId:firstId,revisionPerfilEsperada:p.Revision,revisionOrigenEsperada:p.Version.Revision});
    expect(next.body.ok).toBe(true);
    let second=next.body.datos;
    second=await save(auth,second,{...second.Version.Configuracion,AmbitoIntegridad:'LOTE'},second.Version.Reglas);
    expect((await publish(auth,second)).body.ok).toBe(true);
    expect(await snapshot(firstId)).toEqual(before);
    expect((await snapshot(second.Version.VersionId)).version.ambito_integridad).toBe('LOTE');
    denied=await session(browser,true);
    await expect(denied.page.locator('[data-doc110=abrir]')).toHaveCount(0);
    // Incluso presentando el token de otra sesión, la cuenta sin capacidad no escribe.
    denied.token=auth.token;
    const forbidden=await call(denied,'PublicarVersion',{perfilId:p.PerfilId,versionId:firstId,revisionPerfilEsperada:p.Revision,revisionVersionEsperada:p.Version.Revision});
    expect(forbidden.status).toBe(403);expect(forbidden.body.ok).toBe(false);
    expect(await snapshot(firstId)).toEqual(before);
  }finally{if(denied){denied.token=null;await denied.context.close();}auth.token=null;await auth.context.close();}
});
test('PER-03: reglas persistidas, dos formatos y requerido/tipología inválidos',async({browser})=>{
  const f=fixture(),auth=await create(browser);
  try {
    let p=await save(auth,auth.profile,f.configuracion,f.reglas);
    for(const example of f.ejemplos) {
      if(example.reglas){p=await save(auth,p,p.Version.Configuracion,example.reglas);}
      const loaded=await call(auth,'ObtenerPerfil',{perfilId:p.PerfilId,versionId:p.Version.VersionId});
      expect(loaded.body.ok).toBe(true);p=loaded.body.datos;
      const result=await call(auth,'EnsayarReglas',{perfilId:p.PerfilId,versionId:p.Version.VersionId,revisionVersionEsperada:p.Version.Revision,
        configuracion:p.Version.Configuracion,reglas:p.Version.Reglas,ejemplos:[example.metadatos]});
      expect(result.body.ok).toBe(true);expect(result.body.datos[0].Incidencias).toEqual([]);
      for(const expected of example.esperados) {
        expect(result.body.datos[0].Valores.find(v=>v.DestinoTipo===expected.DestinoTipo && v.DestinoRef===expected.DestinoRef)?.Valor).toBe(expected.Valor);
      }
      expect((await snapshot(p.Version.VersionId)).rules.length).toBe(p.Version.Reglas.length);
    }
    const invalid=p.Version.Reglas.map(r=>r.DestinoTipo==='TIPOLOGIA'?{...r,FuenteTipo:'CONSTANTE',TipoValor:'REFERENCIA',ValorConstante:'0'}:r);
    const rejected=await call(auth,'GuardarBorrador',{perfilId:p.PerfilId,versionId:p.Version.VersionId,revisionVersionEsperada:p.Version.Revision,configuracion:p.Version.Configuracion,reglas:invalid});
    expect(rejected.status).toBe(400);expect(rejected.body.ok).toBe(false);
    p=await save(auth,p,p.Version.Configuracion,p.Version.Reglas.filter(r=>!(r.DestinoTipo==='CAMPO_EXPEDIENTE' && r.DestinoRef==='CODIGO')));
    const missing=await publish(auth,p);
    expect(missing.status).toBe(400);expect(missing.body.incidencias.some(x=>x.Codigo==='FUENTE_REQUERIDA')).toBe(true);
    expect((await snapshot(p.Version.VersionId)).version.estado).toBe('BORRADOR');
    await reloadUI(auth);
  }finally{auth.token=null;await auth.context.close();}
});
test('Persistencia: dos guardados concurrentes conservan una única revisión',async({browser})=>{
  const f=fixture(),auth=await create(browser);
  try {
    const p=await save(auth,auth.profile,f.configuracion,f.reglas);
    const base={perfilId:p.PerfilId,versionId:p.Version.VersionId,revisionVersionEsperada:p.Version.Revision,reglas:p.Version.Reglas};
    const results=await Promise.all([
      call(auth,'GuardarBorrador',{...base,configuracion:{...p.Version.Configuracion,NombreVisible:'Edición A'}}),
      call(auth,'GuardarBorrador',{...base,configuracion:{...p.Version.Configuracion,NombreVisible:'Edición B'}})
    ]);
    expect(results.map(r=>r.status).sort()).toEqual([200,409]);
    const persisted=await snapshot(p.Version.VersionId);
    expect(Number(persisted.version.revision)).toBe(p.Version.Revision+1);
    const winner=results.find(r=>r.status===200).body.datos.Version.Configuracion.NombreVisible;
    expect(JSON.parse(persisted.version.configuracion_doc).NombreVisible).toBe(winner);
  }finally{auth.token=null;await auth.context.close();}
});
