'use strict';

const fs = require('node:fs');
const path = require('node:path');
const {
  collectConfirmation,
  collectValue,
  requireInteractiveConsole
} = require('./support/interactive-e2e-console.cjs');
const {
  createPendingSession,
  loadProfile,
  runVerifyScenario
} = require('./run-doc95-login-second-factor-interactive.cjs');

const root = path.resolve(__dirname, '..', '..', '..');
const snapshotPath = path.join(root, '.opsxj', 'runtime', 'DOC-98-module-snapshot.json');

function profilePath() {
  const index = process.argv.indexOf('--profile');
  if (index < 0 || !process.argv[index + 1]) throw new Error('DOC98_E2E_PROFILE_REQUIRED');
  return path.resolve(process.argv[index + 1]);
}

function loadModuleNames() {
  const snapshot = JSON.parse(fs.readFileSync(snapshotPath, 'utf8'));
  if (snapshot.version !== 1 || !Array.isArray(snapshot.targetRows) || snapshot.targetRows.length < 1) {
    throw new Error('DOC98_SNAPSHOT_INVALID');
  }
  const names = snapshot.targetRows.map(row => String(row.moduleName || '').trim());
  if (names.some(name => !name || /[\r\n=]/.test(name)) || new Set(names).size !== names.length) {
    throw new Error('DOC98_MODULE_MANIFEST_INVALID');
  }
  return names;
}

async function main() {
  requireInteractiveConsole();
  const baseProfile = loadProfile(profilePath());
  const moduleNames = loadModuleNames();
  const authorization = {};
  await collectConfirmation(authorization, 'environment', '¿Autoriza el ambiente de pruebas para la verificación transversal DOC-98?');
  await collectConfirmation(authorization, 'account', '¿Autoriza esta cuenta para probar todas las filas de módulo del manifiesto?');
  await collectConfirmation(authorization, 'mailbox', '¿Autoriza consultar el buzón y usar un OTP nuevo por módulo?');
  await collectConfirmation(authorization, 'challenges', '¿Autoriza crear y consumir un challenge descartable por módulo?');
  await collectConfirmation(authorization, 'discardable', '¿Confirma que cuenta, buzón y challenges son datos de prueba descartables?');
  if (baseProfile.ignoreHttpsErrors) await collectConfirmation(authorization, 'tls', '¿Autoriza el certificado autofirmado del ambiente?');

  const { chromium } = require('playwright');
  const browser = await chromium.launch({ headless: true });
  let context;
  let passed = 0;
  try {
    for (let index = 0; index < moduleNames.length; index += 1) {
      const profile = { ...baseProfile, module: moduleNames[index] };
      const secrets = {};
      await collectValue(secrets, 'account', `Cuenta autorizada para módulo ${index + 1} (${profile.module})`);
      await collectValue(secrets, 'password', 'Contraseña', { secret: true });
      const environment = {
        DOC95_E2E_MODULE: profile.module,
        DOC95_E2E_ACCOUNT: secrets.account,
        DOC95_E2E_PASSWORD: secrets.password
      };
      try {
        const pending = await createPendingSession(browser, profile, environment);
        context = pending.context;
        console.log(`DOC98_MODULE_${index + 1}_PENDING_OK=true`);
        await runVerifyScenario(context, profile);
        passed += 1;
        console.log(`DOC98_MODULE_${index + 1}_VERIFY_OK=true`);
      } finally {
        environment.DOC95_E2E_PASSWORD = '';
        secrets.password = '';
        await context?.close().catch(() => {});
        context = null;
      }
    }
    console.log(`DOC98_TRANSVERSAL_MODULES=${moduleNames.length}`);
    console.log(`DOC98_TRANSVERSAL_PASSED=${passed}`);
    console.log('DOC98_TRANSVERSAL_E2E_RESULT=PASSED');
  } finally {
    await context?.close().catch(() => {});
    await browser.close().catch(() => {});
  }
}

if (require.main === module) {
  main().catch(error => {
    const upstream = String(error?.code || error?.message || '').trim();
    const code = /^DOC(?:95|98)_[A-Z0-9_]{3,100}$/.test(upstream) ? upstream : 'DOC98_TRANSVERSAL_E2E_FAILED';
    console.error(`E2E transversal DOC-98 detenida de forma segura (${code}). Ejecute rollback con el snapshot antes de reintentar.`);
    process.exitCode = 2;
  });
}

module.exports = { loadModuleNames };
