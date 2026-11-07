'use strict';
const fs=require('node:fs');
const path=require('node:path');
const {collectValue,collectConfirmation,requireInteractiveConsole,runChild}=require('./support/interactive-e2e-console.cjs');
const {sourceFixture}=require('../tests/support/doc110-source-fixture.cjs');

async function main(){
  if(['--missing-schema','--acceptance','--draft','--publish','--concurrent','--concurrent-publish','--isolation','--permission-revocation','--session-expiry','--source-scope','--source-profile','--source-guards','--source-transactions','--provider-diagnostics','--provider-scope','--inheritance','--matching','--guards','--tabs','--updatepanel','--provider-lifecycle','--provider-profile','--provider-guards','--provider-concurrent','--provider-availability'].filter(flag=>process.argv.includes(flag)).length>1)throw new Error('DOC110_MODE_CONFLICT');
  const tableIndex=process.argv.indexOf('--source-table');
  if(['--resume-source','--cleanup-source','--continue-source'].filter(flag=>process.argv.includes(flag)).length>1)throw new Error('DOC110_SOURCE_ACTION_CONFLICT');
  if(process.argv.includes('--continue-source')&&!process.argv.includes('--source-profile'))throw new Error('DOC110_SOURCE_CONTINUE_REQUIRES_SOURCE_PROFILE');
  if(process.argv.includes('--cleanup-source')&&!process.argv.includes('--source-profile'))throw new Error('DOC110_SOURCE_CLEANUP_REQUIRES_SOURCE_PROFILE');
  if(process.argv.includes('--resume-source')&&!process.argv.includes('--source-profile'))throw new Error('DOC110_SOURCE_RESUME_REQUIRES_SOURCE_PROFILE');
  if(tableIndex!==-1&&!['--source-profile','--source-guards','--provider-diagnostics'].some(flag=>process.argv.includes(flag)))throw new Error('DOC110_SOURCE_TABLE_REQUIRES_SOURCE_PROFILE');
  const fixture=sourceFixture(tableIndex===-1?undefined:(process.argv[tableIndex+1]||''));
  const root=path.resolve(__dirname,'..');
  const profile=JSON.parse(fs.readFileSync(path.join(root,'profiles/doc88-production-document-upload.local.runtime.json'),'utf8').replace(/^\uFEFF/,''));
  const values={
    DOC110_E2E_BASE_URL:profile.baseUrl,
    DOC110_E2E_MODULE:profile.module,
    DOC110_E2E_DRAFT_LIFECYCLE:['--draft','--concurrent','--guards','--tabs'].some(flag=>process.argv.includes(flag))?'YES':'NO',
    DOC110_E2E_TABS:process.argv.includes('--tabs')?'YES':'NO',
    DOC110_E2E_INDEPENDENT_SESSIONS:['--concurrent','--concurrent-publish','--guards'].some(flag=>process.argv.includes(flag))?'YES':'NO',
    DOC110_E2E_GUARDS:process.argv.includes('--guards')?'YES':'NO',
    DOC110_E2E_PUBLICATION_LIFECYCLE:process.argv.includes('--publish') || process.argv.includes('--concurrent-publish') || process.argv.includes('--inheritance') || process.argv.includes('--matching')?'YES':'NO',
    DOC110_E2E_MATCHING:process.argv.includes('--matching')?'YES':'NO',
    DOC110_E2E_INHERITANCE:process.argv.includes('--inheritance')?'YES':'NO',
    DOC10_E2E_IGNORE_HTTPS_ERRORS:String(profile.ignoreHttpsErrors===true)
  };
  const needed=['DOC110_E2E_USER','DOC110_E2E_PASSWORD'];
  values.DOC110_E2E_MISSING_SCHEMA=process.argv.includes('--missing-schema')?'YES':'NO';
  if(values.DOC110_E2E_MISSING_SCHEMA==='YES')
    console.log('DOC-110 SIN ESQUEMA: autorizacion del usuario para usar las tablas del ambiente de prueba. Renombra temporalmente SOLO las tres tablas de perfiles, exige 503/aviso y carga un PDF sintetico en el expediente descartable 467 con el flujo oficial DOC-88. Restaura nombres y verifica hashes de datos/esquema/identidades en finally. No borra tablas, cambia permisos ni modifica SII. No ejecutar otra consola de pruebas simultaneamente.');
  values.DOC110_E2E_ACCEPTANCE=process.argv.includes('--acceptance')?'YES':'NO';
  if(values.DOC110_E2E_ACCEPTANCE==='YES')
    console.log('DOC-110 PER-01/02/03: crea dos perfiles descartables (borrador y dos publicaciones EXPEDIENTE/LOTE) y los desactiva. Reutiliza retirada temporal/restauracion autorizada SOLO del permiso de perfiles de LUZ.AGUILERA para comprobar guardar/publicar denegados. Registra SHA de codigo/DLL, UI/HTTP y controles SELECT de esos perfiles. No carga documentos ni modifica fuentes/SII.');
  values.DOC110_E2E_PERMISSION_REVOCATION=process.argv.includes('--permission-revocation')?'YES':'NO';
  if(values.DOC110_E2E_PERMISSION_REVOCATION==='YES')
    console.log('DOC-110 PERMISO: autorizacion explicita del usuario para retirar temporalmente SOLO ADMINISTRAR_PERFILES_IMPORTACION de LUZ.AGUILERA y restaurarlo. Comprueba rechazo 403 en sesion vigente, UI sin editor, recuperacion de acceso sin relogin y perfil/historial intactos. Usa RA del modulo configurado; exige una fila inequivoca y permiso inicial 1. No modifica otros campos ni SII.');
  values.DOC110_E2E_SESSION_EXPIRY=process.argv.includes('--session-expiry')?'YES':'NO';
  if(values.DOC110_E2E_SESSION_EXPIRY==='YES'){
    const {sessionIdleMinutes}=require('../tests/support/doc110-session-expiry.cjs');
    values.DOC110_E2E_SESSION_IDLE_MINUTES=String(sessionIdleMinutes(fs.readFileSync(path.resolve(root,'../../Web.config'),'utf8')));
    console.log('DOC-110 SESION: dos sesiones de la cuenta autorizada. Comprueba token ajeno y deja una sin paginas/solicitudes durante '+values.DOC110_E2E_SESSION_IDLE_MINUTES+' min + 75 s; mantiene activa solo la sesion control con lecturas. Exige 401 tras inactividad y perfil/historial intactos. No cambia timeout/permisos/datos ni borra cookies para simular expiracion.');
  }
  values.DOC110_E2E_PROVIDER_LIFECYCLE=['--provider-lifecycle','--provider-profile','--provider-guards','--provider-concurrent','--provider-availability'].some(flag=>process.argv.includes(flag))?'YES':'NO';
  values.DOC110_E2E_PROVIDER_CONCURRENT=['--provider-concurrent','--provider-availability'].some(flag=>process.argv.includes(flag))?'YES':'NO';
  values.DOC110_E2E_PROVIDER_AVAILABILITY=process.argv.includes('--provider-availability')?'YES':'NO';
  if(values.DOC110_E2E_PROVIDER_AVAILABILITY==='YES')console.log('DOC-110: activa temporalmente SOLO el proveedor descartable mediante dos sesiones concurrentes y lo desactiva en finally; conserva historial/versiones.');
  if(values.DOC110_E2E_PROVIDER_CONCURRENT==='YES')console.log('DOC-110: dos sesiones independientes compiten por guardar, publicar y crear version del proveedor descartable INACTIVO. Exige un ganador y un conflicto por operacion; no modifica fuentes ni origen.');
  values.DOC110_E2E_PROVIDER_GUARDS=process.argv.includes('--provider-guards')?'YES':'NO';
  if(values.DOC110_E2E_PROVIDER_GUARDS==='YES')
    console.log('DOC-110: verifica 33 rechazos en nueve endpoints de proveedor por CSRF ausente/invalido, sin sesion y revision obsoleta; exige documento inalterado tras cada intento. El ciclo UI crea/publica SOLO un proveedor descartable INACTIVO; no modifica fuentes ni perfiles.');
  values.DOC110_E2E_SOURCE_PROFILE=process.argv.includes('--source-profile')?'YES':'NO';
  values.DOC110_E2E_SOURCE_TRANSACTIONS=process.argv.includes('--source-transactions')?'YES':'NO';
  values.DOC110_E2E_SOURCE_TRANSACTIONS_AUTHORIZED=values.DOC110_E2E_SOURCE_TRANSACTIONS==='YES'?'true':'false';
  if(values.DOC110_E2E_SOURCE_TRANSACTIONS==='YES')
    console.log('DOC-110 TRANSACCIONES: verifica que DA imp_02_facturas no tenga autorizacion previa, crea desde UI una fuente NUEVA INACTIVA con NIT/NUMERO y prueba duplicado y edicion desde dos sesiones independientes. Nunca activa la fuente. No crea tablas/proveedores/perfiles ni consulta filas DA; solo metadatos. Si la fuente ya existe, se detiene sin modificarla.');
  values.DOC110_E2E_PROVIDER_DIAGNOSTICS=process.argv.includes('--provider-diagnostics')?'YES':'NO';
  values.DOC110_E2E_SOURCE_GUARDS=process.argv.includes('--source-guards')?'YES':'NO';
  if(values.DOC110_E2E_SOURCE_GUARDS==='YES')console.log('DOC-110 GUARD FUENTES: solicitudes que deben rechazarse por CSRF/sesion/revision/contrato. Usa fixture inactivo de corrida PASS, revision antigua e ID conservado en Crear para impedir alta. Comprueba documento/historial despues de cada intento; no concede permisos ni consulta DA.');
  if(values.DOC110_E2E_PROVIDER_DIAGNOSTICS==='YES')console.log('DOC-110 DIAGNOSTICO: solo lecturas. Reabre proveedor E2E publicado inactivo cuya fuente esta revocada; verifica aviso y referencias originales sin sustituir, guardar ni publicar.');
  values.DOC110_E2E_SOURCE_TABLE=fixture.table;
  values.DOC110_E2E_SOURCE_CLEANUP=process.argv.includes('--cleanup-source')?'YES':'NO';
  values.DOC110_E2E_SOURCE_CONTINUE=process.argv.includes('--continue-source')?'YES':'NO';
  values.DOC110_E2E_SOURCE_RESUME=process.argv.includes('--resume-source')?'YES':'NO';
  values.DOC110_E2E_SOURCE_CATALOG_AUTHORIZED='false';
  values.DOC110_E2E_PROVIDER_PROFILE=['--provider-profile','--source-profile'].some(flag=>process.argv.includes(flag))?'YES':'NO';
  if(values.DOC110_E2E_SOURCE_CLEANUP==='YES')values.DOC110_E2E_PROVIDER_PROFILE='NO';
  if(values.DOC110_E2E_SOURCE_PROFILE==='YES'&&values.DOC110_E2E_SOURCE_CLEANUP!=='YES'&&values.DOC110_E2E_SOURCE_CONTINUE!=='YES')
    console.log('DOC-110: registra DA '+fixture.table+' desde UI solo si no existe autorizacion previa; activa, edita columnas, conecta proveedor/perfil y revoca SOLO la nueva fuente al finalizar. Si ya existe, se detiene sin modificarla. No escribe en DA. Requiere autorizacion especifica adicional.');
  if(values.DOC110_E2E_SOURCE_RESUME==='YES')
    console.log('DOC-110 REANUDACION: no crea otra fuente. Solo permite continuar una fuente inactiva revision 1, con un unico ALTA que coincida con identidad, columnas, motivo y ventana UTC del reporte fallido local. Si no coincide, aborta sin escribir.');
  if(values.DOC110_E2E_SOURCE_CONTINUE==='YES')
    console.log('DOC-110 CONTINUACION: exige reporte de recuperacion PASS y fuente propia inactiva revision 4 con alta/activacion/edicion/revocacion verificadas. Reactiva revision 5, ejecuta proveedor/perfil y revoca revision 6. No repite alta ni edicion; conserva historial anterior.');
  if(values.DOC110_E2E_SOURCE_CLEANUP==='YES')console.log('DOC-110 RECUPERACION: solo verifica estado/historial de la fuente identificada por el reporte de la prueba fallida y revoca si permanece activa. No crea fuentes/proveedores/perfiles ni consulta DA.');
  if(values.DOC110_E2E_SOURCE_PROFILE==='YES'&&values.DOC110_E2E_SOURCE_CLEANUP!=='YES'&&!fixture.ambiguousKey)needed.push('DOC110_E2E_AMBIGUOUS_KEY');
  if(values.DOC110_E2E_PROVIDER_PROFILE==='YES'){
    needed.push('DOC110_E2E_UNIQUE_KEY');
    console.log('DOC-110: activa temporalmente SOLO el proveedor descartable, lo conecta por UI a un perfil descartable y verifica el indice CONSECUTIVO. Lo desactiva para comprobar rechazo de publicacion sin cambios al perfil/historial y lo restaura con revision vigente para continuar. Desactiva proveedor al finalizar.');
  }
  if(values.DOC110_E2E_PROVIDER_LIFECYCLE==='YES' && values.DOC110_E2E_PROVIDER_CONCURRENT!=='YES')
    console.log('DOC-110: crea proveedor descartable INACTIVO desde UI para DA imp_02_egresos/CONSECUTIVO, prueba ausente y ambiguo (44), publica dos versiones. No modifica fuentes ni registros de origen.');
  values.DOC110_E2E_UPDATEPANEL=process.argv.includes('--updatepanel')?'YES':'NO';
  if(values.DOC110_E2E_UPDATEPANEL==='YES')
    console.log('DOC-110: dos actualizaciones parciales reales del listado y reapertura del editor por teclado. No crea perfiles, expedientes ni documentos.');
  values.DOC110_E2E_PROVIDER_SCOPE=process.argv.includes('--provider-scope')?'YES':'NO';
  if(values.DOC110_E2E_PROVIDER_SCOPE==='YES')
    console.log('DOC-110: metadatos RA/DA restringidos a imp_02 y validacion de proveedores ya guardados fuera del prefijo, sin excepciones. Comprueba documento intacto y fixture legado sintetico inactivo cuando hay reporte de preparacion. Esta corrida no crea proveedores ni consulta filas de negocio. Si no hay candidato registra NO EJECUTADO.');
  values.DOC110_E2E_SOURCE_SCOPE=process.argv.includes('--source-scope')?'YES':'NO';
  if(values.DOC110_E2E_SOURCE_SCOPE==='YES')
    console.log('DOC-110: solo metadatos RA/DA; verifica prefijo literal imp_02 sin excepciones y rechazo de tablas ajenas. No consulta registros ni modifica fuentes.');
  values.DOC110_E2E_ACCESS_ISOLATION=process.argv.includes('--isolation')?'YES':'NO';
  if(values.DOC110_E2E_ACCESS_ISOLATION==='YES'){
    needed.push('DOC110_E2E_SECOND_USER','DOC110_E2E_SECOND_PASSWORD');
    console.log('DOC-110: solo lecturas con dos cuentas autorizadas. La segunda debe carecer de acceso a perfiles y administracion de proveedores. Comprueba listado, detalle y metadatos; no modifica permisos, fuentes ni proveedores. No acredita aislamiento entre empresas.');
  }
  if(values.DOC110_E2E_INDEPENDENT_SESSIONS==='YES')
    console.log('DOC-110: dos sesiones independientes de la cuenta autorizada; prueba revisiones concurrentes. No acredita aislamiento entre cuentas o empresas.');
  if(values.DOC110_E2E_PUBLICATION_LIFECYCLE==='YES')
    console.log('DOC-110: consulta catálogos autorizados, crea un perfil descartable, publica dos versiones y lo desactiva. No crea expedientes ni documentos.');
  if(values.DOC110_E2E_DRAFT_LIFECYCLE==='YES')
    console.log('DOC-110: crea un perfil descartable, prueba guardado/reglas/concurrencia y lo desactiva al completar. No carga documentos.');
  for(const name of needed)if(process.env[name])values[name]=process.env[name];
  if(needed.some(name=>!values[name])){
    requireInteractiveConsole();
    if(!values.DOC110_E2E_USER)await collectValue(values,'DOC110_E2E_USER','Cuenta de prueba autorizada para DOC-110');
    if(!values.DOC110_E2E_PASSWORD)await collectValue(values,'DOC110_E2E_PASSWORD','Contraseña',{secret:true});
    if(values.DOC110_E2E_PROVIDER_PROFILE==='YES' && !values.DOC110_E2E_UNIQUE_KEY)
      await collectValue(values,'DOC110_E2E_UNIQUE_KEY','CONSECUTIVO con un unico registro en '+fixture.table,{secret:true});
    if(values.DOC110_E2E_SOURCE_PROFILE==='YES'&&values.DOC110_E2E_SOURCE_CLEANUP!=='YES'&&!fixture.ambiguousKey&&!values.DOC110_E2E_AMBIGUOUS_KEY)
      await collectValue(values,'DOC110_E2E_AMBIGUOUS_KEY','CONSECUTIVO duplicado en '+fixture.table+'; escriba NO_APLICA si no dispone de uno (escenario ambiguo no ejecutado)',{secret:true});
    if(values.DOC110_E2E_ACCESS_ISOLATION==='YES'){
      if(!values.DOC110_E2E_SECOND_USER)await collectValue(values,'DOC110_E2E_SECOND_USER','Segunda cuenta de prueba expresamente autorizada');
      if(!values.DOC110_E2E_SECOND_PASSWORD)await collectValue(values,'DOC110_E2E_SECOND_PASSWORD','Contraseña de segunda cuenta',{secret:true});
    }
  }
  if(values.DOC110_E2E_SOURCE_PROFILE==='YES'){
    requireInteractiveConsole();
    await collectConfirmation(values,'DOC110_E2E_SOURCE_CATALOG_AUTHORIZED',
      'Autoriza en el ambiente configurado y con esta cuenta '+(values.DOC110_E2E_SOURCE_CLEANUP==='YES'?'verificar y dejar inactiva SOLO la fuente de prueba identificada por reporte e historial DA ':values.DOC110_E2E_SOURCE_CONTINUE==='YES'?'continuar desde la recuperacion PASS, reactivar y revocar SOLO la fuente de prueba DA ':values.DOC110_E2E_SOURCE_RESUME==='YES'?'reanudar exclusivamente el alta de la prueba fallida y activar/editar/revocar su autorizacion DA ':'crear/activar/editar/revocar una NUEVA autorizacion DA ')+fixture.table+(values.DOC110_E2E_SOURCE_CLEANUP==='YES'?'':', con proveedor y perfil descartables'));
  }
  const environment={...process.env,...values};
  try{
    const args=[
      path.join(root,'node_modules/@playwright/test/cli.js'),'test',
      'tests/doc110-perfil-importacion-session.spec.cjs','--reporter=list'
    ];
    // Opción nativa del runner existente; no modifica la configuración global.
    if(process.argv.includes('--headed'))args.push('--headed');
    const result=await runChild(process.execPath,args,root,environment);
    process.exitCode=result.code;
  }finally{
    for(const name of Object.keys(values)){delete values[name];delete environment[name];}
  }
}
main().catch(()=>{
  console.error('DOC110_SESSION_BLOCKED: faltan credenciales efímeras. Ejecute este comando en su consola interactiva para ingresarlas; no se inició login.');
  process.exitCode=2;
});
