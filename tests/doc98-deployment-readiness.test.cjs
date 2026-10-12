const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const root = path.resolve(__dirname, '..');
const docRoot = path.join(root, 'Doc', 'Actualizacion', 'Login', 'Implementacion', 'DOC-98');
const sqlRoot = path.join(docRoot, 'Sql');

const requiredFiles = [
  'README.md',
  '01-RUNBOOK-DESPLIEGUE-ACTIVACION.md',
  '02-MATRIZ-GO-NO-GO.md',
  '03-ROTACION-LIMPIEZA-ROLLBACK.md',
  '04-VALIDACION-Y-EVIDENCIA.md',
  'Sql/01-preflight-readonly.sql',
  'Sql/02-pilot-activation.sql',
  'Sql/03-pilot-disable-rollback.sql',
  'Sql/04-observability-readonly.sql',
  'Sql/05-principal-diagnostic-readonly.sql',
  'Sql/06-global-wave-activation.sql',
  'Sql/07-terminal-cleanup-preview-readonly.sql'
];

const requiredProjectItems = [
  'js\\login\\segundo-factor.js',
  'Styles\\login\\segundo-factor.css',
  'webservice\\WebServiceLoginSegundoFactor.asmx',
  'webservice\\WebServiceLoginSegundoFactor.asmx.vb',
  'Modelo\\Login\\SegundoFactor\\SegundoFactorModels.vb',
  'Modelo\\Login\\SegundoFactor\\SegundoFactorSmtpModels.vb',
  'Modelo\\Login\\SegundoFactor\\SegundoFactorInterfaces.vb',
  'Services\\Login\\SegundoFactor\\SecondFactorPreAuthenticationService.vb',
  'Services\\Login\\SegundoFactor\\SecondFactorAuthenticationService.vb',
  'Services\\Login\\SegundoFactor\\NullSecondFactorDiagnosticSink.vb',
  'Services\\Login\\SegundoFactor\\SecondFactorRecipientResolver.vb',
  'Infrastructure\\Repositories\\Login\\SegundoFactor\\OdbcSecondFactorLoginModuleRepository.vb',
  'Infrastructure\\Repositories\\Login\\SegundoFactor\\MySqlSecondFactorPrincipalRepositoryBase.vb',
  'Infrastructure\\Repositories\\Login\\SegundoFactor\\MySqlDocuarchiSecondFactorPrincipalRepository.vb',
  'Infrastructure\\Repositories\\Login\\SegundoFactor\\MySqlGestorSecondFactorPrincipalRepository.vb',
  'Infrastructure\\Repositories\\Login\\SegundoFactor\\MySqlRadicacionSecondFactorPrincipalRepository.vb',
  'Infrastructure\\Repositories\\Login\\SegundoFactor\\MySqlWorkflowSecondFactorPrincipalRepository.vb',
  'Infrastructure\\Repositories\\Login\\SegundoFactor\\SecondFactorPrincipalRepositoryResolver.vb',
  'Infrastructure\\Repositories\\Login\\SegundoFactor\\MySqlSecondFactorChallengeRepository.vb',
  'Infrastructure\\Repositories\\Login\\SegundoFactor\\MySqlSecondFactorSmtpConfigurationRepository.vb',
  'Infrastructure\\Login\\SegundoFactor\\Smtp\\FrameworkSmtpClientAdapter.vb',
  'Infrastructure\\Login\\SegundoFactor\\Smtp\\SecondFactorSmtpTransport.vb',
  'Infrastructure\\Login\\SegundoFactor\\Smtp\\SecondFactorSmtpEmailSender.vb',
  'Infrastructure\\Login\\SegundoFactor\\Diagnostics\\TraceSecondFactorDiagnosticSink.vb',
  'Infrastructure\\Login\\SegundoFactor\\Diagnostics\\EventLogSecondFactorDiagnosticSink.vb',
  'DTOs\\Login\\SegundoFactor\\SegundoFactorDtos.vb',
  'Infrastructure\\Login\\SegundoFactor\\Security\\SystemSecondFactorClock.vb',
  'Infrastructure\\Login\\SegundoFactor\\Security\\CryptographicSecondFactorOtpGenerator.vb',
  'Infrastructure\\Login\\SegundoFactor\\Security\\AppSettingsSecondFactorKeyProvider.vb',
  'Infrastructure\\Login\\SegundoFactor\\Security\\HmacSecondFactorCodeProtector.vb',
  'webservice\\Login\\SegundoFactor\\SessionPendingSecondFactorContextStore.vb',
  'webservice\\Login\\SegundoFactor\\LoginSecondFactorSessionGate.vb',
  'webservice\\Login\\SegundoFactor\\FormsAuthenticationTicketIssuer.vb',
  'webservice\\Login\\SegundoFactor\\LegacyLoginLocalRouteResolver.vb',
  'webservice\\Login\\SegundoFactor\\LoginSecondFactorPresentationMapper.vb',
  'webservice\\Login\\SegundoFactor\\LoginSecondFactorCompositionFactory.vb'
];

function read(relative) {
  return fs.readFileSync(path.join(root, relative), 'utf8');
}

function stripSqlComments(sql) {
  return sql
    .replace(/\/\*[\s\S]*?\*\//g, ' ')
    .replace(/^\s*--.*$/gm, ' ');
}

function assertBalanced(text, file) {
  let depth = 0;
  let quote = null;
  for (let i = 0; i < text.length; i += 1) {
    const char = text[i];
    if (quote) {
      if (char === quote && text[i - 1] !== '\\') quote = null;
      continue;
    }
    if (char === "'" || char === '"') quote = char;
    else if (char === '(') depth += 1;
    else if (char === ')') depth -= 1;
    assert.ok(depth >= 0, `${file}: paréntesis de cierre sin apertura`);
  }
  assert.equal(quote, null, `${file}: literal sin cerrar`);
  assert.equal(depth, 0, `${file}: paréntesis desbalanceados`);
}

test('DOC-98 declara y conserva el inventario requerido', () => {
  for (const relative of requiredFiles) {
    assert.ok(fs.existsSync(path.join(docRoot, relative)), `${relative}: archivo requerido ausente`);
  }

  const project = read('GestionDocumental-Docuarchi.net.vbproj');
  for (const item of requiredProjectItems) {
    const escaped = item.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    const matches = project.match(new RegExp(`Include="${escaped}"`, 'g')) || [];
    assert.equal(matches.length, 1, `${item}: debe estar registrado exactamente una vez en el proyecto`);
    assert.ok(fs.existsSync(path.join(root, item.replaceAll('\\', path.sep))), `${item}: fuente registrada ausente`);
  }
});

test('consultas readonly contienen solo lectura y estructura estática válida', () => {
  const readonlyFiles = [
    '01-preflight-readonly.sql',
    '04-observability-readonly.sql',
    '05-principal-diagnostic-readonly.sql',
    '07-terminal-cleanup-preview-readonly.sql'
  ];
  for (const file of readonlyFiles) {
    const sql = stripSqlComments(fs.readFileSync(path.join(sqlRoot, file), 'utf8'));
    assertBalanced(sql, file);
    assert.doesNotMatch(sql, /\b(?:INSERT|UPDATE|DELETE|REPLACE|ALTER|CREATE|DROP|TRUNCATE|CALL|EXECUTE|GRANT|REVOKE)\b/i, `${file}: operación no permitida`);
    assert.match(sql, /\bSELECT\b/i, `${file}: debe contener SELECT`);
  }
  const preflight = fs.readFileSync(path.join(sqlRoot, '01-preflight-readonly.sql'), 'utf8');
  for (const column of ['Id','ChallengeId','AuthUserId','Provider','CodeHash','ExpiresAtUtc','Consumed','Attempts','CreatedAtUtc','AuthPayloadJson']) {
    assert.match(preflight, new RegExp(`['"]${column}['"]`), `preflight: falta columna ${column}`);
  }
  assert.match(preflight, /ESTADO_ENVIO\s*=\s*1/i);
});

test('scripts mutables nacen cerrados y limitan cada operación al manifiesto', () => {
  for (const file of ['02-pilot-activation.sql', '03-pilot-disable-rollback.sql', '06-global-wave-activation.sql']) {
    const sql = stripSqlComments(fs.readFileSync(path.join(sqlRoot, file), 'utf8'));
    assertBalanced(sql, file);
    assert.match(sql, /SET\s+@doc98_authorized\s*=\s*0\s*;/i, `${file}: autorización no inicia cerrada`);
    assert.match(sql, /START\s+TRANSACTION/i, `${file}: falta transacción`);
    assert.match(sql, /ROLLBACK\s*;/i, `${file}: debe finalizar seguro por defecto`);
    assert.match(sql, /EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA/i, `${file}: falta ID de empresa`);
    assert.match(sql, /ID_MODULO/i, `${file}: falta ID de módulo`);
    assert.doesNotMatch(sql, /\b(?:DELETE|ALTER|CREATE|DROP|TRUNCATE|GRANT|REVOKE)\b/i, `${file}: operación prohibida`);
  }
  const pilot = fs.readFileSync(path.join(sqlRoot, '02-pilot-activation.sql'), 'utf8');
  assert.match(pilot, /RequiereSegundoFactor\s*<=>\s*@snapshot_required/i);
  assert.match(pilot, /ROW_COUNT\(\)\s+AS\s+DOC98_AFFECTED_ROWS/i);

  const rollback = fs.readFileSync(path.join(sqlRoot, '03-pilot-disable-rollback.sql'), 'utf8');
  assert.match(rollback, /@restore_required\s+IS\s+NULL/i, 'rollback: debe poder restaurar el estado compatible NULL');

  const wave = fs.readFileSync(path.join(sqlRoot, '06-global-wave-activation.sql'), 'utf8');
  assert.match(wave, /INNER\s+JOIN\s*\(/i, 'oleada: falta manifiesto explícito');
  assert.match(wave, /@actual_candidate_rows\s*=\s*@expected_manifest_rows/i, 'oleada: falta guarda de cardinalidad');
  assert.match(wave, /old_required/i);
  assert.match(wave, /new_expiry/i);
});

test('activación es transversal por módulo y no existe selector por usuario', () => {
  const docs = requiredFiles.filter(file => file.endsWith('.md')).map(file => fs.readFileSync(path.join(docRoot, file), 'utf8')).join('\n');
  assert.match(docs, /activaci[oó]n exclusivamente en la configuraci[oó]n central del m[oó]dulo|propiedad de la fila central del m[oó]dulo/i);
  assert.match(docs, /no existe configuraci[oó]n, lista, excepci[oó]n ni bypass por usuario/i);
  assert.match(docs, /falla cerrad[ao]/i);

  const principalDiagnostic = fs.readFileSync(path.join(sqlRoot, '05-principal-diagnostic-readonly.sql'), 'utf8');
  for (const table of ['usuarios_da', 'remit_dest_interno', 'usuario_radicador', 'usuario_workflow']) {
    assert.match(principalDiagnostic, new RegExp(`\\b${table}\\b`, 'i'), `diagnóstico: falta ${table}`);
  }
  assert.doesNotMatch(principalDiagnostic, /\bUPDATE\b/i);
});

test('configuración local conserva sesión de diez minutos y proveedor externo de llaves', () => {
  const webConfig = read('Web.config');
  assert.match(webConfig, /<appSettings\s+file="AppSettings\.local\.config"\s*>/i);
  assert.match(webConfig, /<sessionState\b[^>]*\bmode="InProc"[^>]*\btimeout="10"/i);

  const provider = read('Infrastructure/Login/SegundoFactor/Security/AppSettingsSecondFactorKeyProvider.vb');
  assert.match(provider, /LoginSecondFactorHmacActiveKeyId/);
  assert.match(provider, /LoginSecondFactorHmacKey\./);
  assert.match(provider, /FromBase64String/);
  assert.match(provider, /Length\s*<\s*32/);
});

test('observabilidad y documentación no exponen secretos ni datos personales', () => {
  const observability = fs.readFileSync(path.join(sqlRoot, '04-observability-readonly.sql'), 'utf8');
  const selectExpressions = observability.replace(/--.*$/gm, '').match(/SELECT[\s\S]*?FROM/gi) || [];
  for (const expression of selectExpressions) {
    assert.doesNotMatch(expression, /\b(?:AuthUserId|CodeHash|AuthPayloadJson|Correo_Usuario|Correo_Electronico|Login_Usuario)\b/i);
  }

  const allText = requiredFiles.map(file => fs.readFileSync(path.join(docRoot, file), 'utf8')).join('\n');
  assert.doesNotMatch(allText, /(?:password|contrase(?:ñ|n)a|pwd)\s*[=:]\s*(?!REPLACE|<)/i);
  assert.doesNotMatch(allText, /(?:Server|Data Source)\s*=.+;(?:Database|Initial Catalog)\s*=/i);
});

test('OpenSpec mantiene trazabilidad D-10/RQ-10 y estado real bloqueado', () => {
  assert.match(read('openspec/changes/doc-98-doble-factor-despliegue/design.md'), /D-10/);
  assert.match(read('openspec/changes/doc-98-doble-factor-despliegue/specs/doble-factor-despliegue/spec.md'), /RQ-10/);
  assert.match(read('openspec/changes/doc-98-doble-factor-despliegue/tasks.md'), /D-10[\s\S]*RQ-10|RQ-10[\s\S]*D-10/);
  assert.match(read('openspec/changes/doc-98-doble-factor-despliegue/specs/doble-factor-despliegue/spec.md'), /sin habilitaci[oó]n, lista, bandera ni excepci[oó]n por usuario/i);
  assert.match(read('Doc/Actualizacion/Login/Implementacion/DOC-98/04-VALIDACION-Y-EVIDENCIA.md'), /Activaci[oó]n transversal:\s*`BLOQUEADA`/i);
});

test('preflight interactivo DOC-98 es SELECT-only, saneado y está registrado', () => {
  const runnerPath = 'tools/e2e/scripts/inspect-doc98-transversal-readiness-interactive.cjs';
  const runner = read(runnerPath);
  const inspector = require(path.join(root, runnerPath));
  const packageJson = JSON.parse(read('tools/e2e/package.json'));
  assert.equal(packageJson.scripts['inspect:doc98:readiness'], 'node scripts/inspect-doc98-transversal-readiness-interactive.cjs');
  assert.match(runner, /requireInteractiveConsole\(\)/);
  assert.match(runner, /collectConfirmation/);
  assert.match(runner, /multipleStatements:\s*false/);
  assert.match(runner, /DOC98_PREFLIGHT_SELECT_ONLY=true/);
  assert.match(runner, /DOC98_PREFLIGHT_READY=/);
  assert.doesNotMatch(runner, /profile\.(?:password|passwd|secret|credential|mysqlUser|mysqlUrl|connectionString)/i);
  for (const type of ['DOCUARCHI CONTENEDOR', 'GESTOR DOCUMENTAL', 'RADICACION DOCUMENTAL', 'WORKFLOW DOCUMENTAL']) {
    assert.match(runner, new RegExp(type));
  }
  assert.doesNotThrow(() => inspector.assertReadOnly());
  const metadata = inspector.loadDatabaseMetadata(path.join(root, 'tools/e2e/profiles/doc90-radicacion-classic-attachment.local.runtime.json'));
  assert.deepEqual(Object.keys(metadata).sort(), ['database', 'host', 'port']);
  const schema = { challenge_columns: 10, primary_id: 1, unique_challenge: 1, auth_user_index: 1, module_columns: 8, smtp_columns: 9 };
  const modules = inspector.TARGET_TYPES.map(module_type => ({ module_type, total_count: 1, on_count: 0, off_count: 1, invalid_flag_count: 0, expiry_gap_count: 0 }));
  assert.equal(inspector.evaluate(schema, modules, { ambiguous_count: 0 }, { active_count: 1 }).ready, true);
  assert.equal(inspector.evaluate(schema, modules.slice(1), { ambiguous_count: 0 }, { active_count: 1 }).ready, false);
});

test('operador DOC-98 limita activación y rollback a snapshots exactos', () => {
  const runnerPath = 'tools/e2e/scripts/run-doc98-transversal-activation-interactive.cjs';
  const runner = read(runnerPath);
  const operator = require(path.join(root, runnerPath));
  const packageJson = JSON.parse(read('tools/e2e/package.json'));
  assert.equal(packageJson.scripts['run:doc98:activation'], 'node scripts/run-doc98-transversal-activation-interactive.cjs');
  assert.doesNotThrow(() => operator.assertSqlScope());
  assert.match(operator.ACTIVATE_SQL, /UPDATE\s+gestor_modulos/i);
  assert.match(operator.ACTIVATE_SQL, /ID_MODULO=\?/i);
  assert.match(operator.ACTIVATE_SQL, /EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=\?/i);
  assert.match(operator.ACTIVATE_SQL, /RequiereSegundoFactor\s*<=>\s*\?/i);
  assert.match(operator.RESTORE_SQL, /SegundoFactorTiempoExpira=\?/i);
  assert.match(runner, /flag:\s*'wx'/);
  assert.match(runner, /DOC98_OUTSIDE_ROWS_UNCHANGED=true/);
  assert.match(runner, /DOC98_SNAPSHOT_AVAILABLE=true/);
  assert.match(runner, /collectConfirmation/);
  assert.doesNotMatch(runner, /console\.log\([^\n]*(?:moduleName|companyId|moduleId|password|user)/i);

  const rows = [
    ['DOCUARCHI CONTENEDOR', 0],
    ['GESTOR DOCUMENTAL', 1],
    ['RADICACION DOCUMENTAL', null],
    ['WORKFLOW DOCUMENTAL', 0]
  ].map(([module_type, required_flag], index) => ({
    module_id: index + 1,
    company_id: 1,
    module_name: `M${index + 1}`,
    module_type,
    enabled: 1,
    required_flag,
    provider_type: 1,
    expiry_minutes: 10
  }));
  const state = operator.classify(rows);
  assert.equal(state.valid, true);
  assert.equal(state.on.length, 1);
  assert.equal(state.off.length, 3);
  assert.equal(operator.fingerprint(rows), operator.fingerprint(rows));
});

test('E2E transversal DOC-98 reutiliza DOC-95 y no persiste credenciales', () => {
  const runnerPath = 'tools/e2e/scripts/run-doc98-transversal-login-interactive.cjs';
  const runner = read(runnerPath);
  const packageJson = JSON.parse(read('tools/e2e/package.json'));
  assert.equal(packageJson.scripts['test:doc98:transversal-login'], 'node scripts/run-doc98-transversal-login-interactive.cjs');
  assert.match(runner, /require\('\.\/run-doc95-login-second-factor-interactive\.cjs'\)/);
  assert.match(runner, /createPendingSession/);
  assert.match(runner, /runVerifyScenario/);
  assert.match(runner, /requireInteractiveConsole\(\)/);
  assert.match(runner, /OTP nuevo por m[oó]dulo/);
  assert.match(runner, /DOC98_TRANSVERSAL_E2E_RESULT=PASSED/);
  assert.match(runner, /Ejecute rollback con el snapshot/);
  assert.doesNotMatch(runner, /writeFile|appendFile|console\.log\([^\n]*(?:secrets\.(?:account|password)|environment\.DOC95_E2E_(?:ACCOUNT|PASSWORD)|moduleNames\[(?:index|\d+)\])/i);
});
